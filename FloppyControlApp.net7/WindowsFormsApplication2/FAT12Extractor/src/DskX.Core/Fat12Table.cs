namespace DskX.Core;

/// <summary>
/// Reads and parses the FAT12 File Allocation Table.
/// FAT12 uses 12-bit entries packed into bytes (1.5 bytes per entry).
/// </summary>
public class Fat12Table
{
    private readonly byte[] _fatData;
    private readonly Fat12BootSector _bootSector;
    
    // FAT12 special values
    private const int FREE_CLUSTER = 0x000;
    private const int BAD_CLUSTER = 0xFF7;
    private const int END_OF_CHAIN_MIN = 0xFF8;
    private const int END_OF_CHAIN_MAX = 0xFFF;
    
    /// <summary>
    /// Initializes a new instance of the Fat12Table class.
    /// </summary>
    /// <param name="fatData">Raw FAT table data.</param>
    /// <param name="bootSector">Boot sector containing disk geometry.</param>
    private Fat12Table(byte[] fatData, Fat12BootSector bootSector)
    {
        _fatData = fatData;
        _bootSector = bootSector;
    }
    
    /// <summary>
    /// Loads the FAT table from a stream.
    /// </summary>
    /// <param name="stream">Disk image stream.</param>
    /// <param name="bootSector">Parsed boot sector.</param>
    /// <returns>Loaded FAT table.</returns>
    public static Fat12Table Load(Stream stream, Fat12BootSector bootSector)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        if (bootSector == null)
            throw new ArgumentNullException(nameof(bootSector));
        
        // Calculate FAT offset and size
        int fatOffset = bootSector.ReservedSectors * bootSector.BytesPerSector;
        int fatSize = bootSector.SectorsPerFat * bootSector.BytesPerSector;
        
        // Read the first FAT (we'll use FAT #0, ignoring redundant copies)
        byte[] fatData = new byte[fatSize];
        stream.Position = fatOffset;
        int bytesRead = stream.Read(fatData, 0, fatSize);
        
        if (bytesRead < fatSize)
            throw new InvalidDataException($"Failed to read complete FAT table (expected {fatSize} bytes, got {bytesRead}).");
        
        return new Fat12Table(fatData, bootSector);
    }
    
    /// <summary>
    /// Gets the next cluster in the chain for the given cluster.
    /// </summary>
    /// <param name="cluster">Current cluster number.</param>
    /// <returns>Next cluster number, or a special value (end-of-chain, bad, free).</returns>
    public int GetNextCluster(int cluster)
    {
        if (cluster < 2)
            throw new ArgumentException("Cluster numbers start at 2.", nameof(cluster));
        
        if (cluster >= _bootSector.TotalDataClusters + 2)
            return END_OF_CHAIN_MAX; // Beyond valid range
        
        return ReadFatEntry(cluster);
    }
    
    /// <summary>
    /// Follows the FAT chain starting from the given cluster.
    /// </summary>
    /// <param name="startCluster">Starting cluster number.</param>
    /// <returns>List of all clusters in the chain (including the start cluster).</returns>
    public List<int> GetClusterChain(int startCluster)
    {
        var chain = new List<int>();
        var visited = new HashSet<int>(); // Detect circular chains
        int currentCluster = startCluster;
        
        const int MAX_CHAIN_LENGTH = 10000; // Safety limit
        
        while (true)
        {
            // Safety checks
            if (chain.Count > MAX_CHAIN_LENGTH)
            {
                Console.Error.WriteLine($"Warning: FAT chain exceeds maximum length at cluster {currentCluster}. Breaking chain.");
                break;
            }
            
            if (visited.Contains(currentCluster))
            {
                Console.Error.WriteLine($"Warning: Circular FAT chain detected at cluster {currentCluster}. Breaking chain.");
                break;
            }
            
            visited.Add(currentCluster);
            chain.Add(currentCluster);
            
            // Get next cluster
            int nextCluster = GetNextCluster(currentCluster);
            
            // Check for end of chain
            if (IsEndOfChain(nextCluster))
                break;
            
            // Check for bad cluster
            if (IsBadCluster(nextCluster))
            {
                Console.Error.WriteLine($"Warning: Bad cluster {nextCluster} in FAT chain. Ending chain.");
                break;
            }
            
            // Check for free cluster (shouldn't happen in a valid chain)
            if (IsFreeCluster(nextCluster))
            {
                Console.Error.WriteLine($"Warning: Free cluster {nextCluster} in FAT chain. Ending chain.");
                break;
            }
            
            currentCluster = nextCluster;
        }
        
        return chain;
    }
    
    /// <summary>
    /// Checks if the given FAT value indicates end of chain.
    /// </summary>
    public bool IsEndOfChain(int value)
    {
        return value >= END_OF_CHAIN_MIN && value <= END_OF_CHAIN_MAX;
    }
    
    /// <summary>
    /// Checks if the given FAT value indicates a bad cluster.
    /// </summary>
    public bool IsBadCluster(int value)
    {
        return value == BAD_CLUSTER;
    }
    
    /// <summary>
    /// Checks if the given FAT value indicates a free cluster.
    /// </summary>
    public bool IsFreeCluster(int value)
    {
        return value == FREE_CLUSTER;
    }
    
    /// <summary>
    /// Finds all bad clusters marked in the FAT.
    /// </summary>
    /// <returns>List of cluster numbers marked as bad.</returns>
    public List<int> GetBadClusters()
    {
        var badClusters = new List<int>();
        
        // Scan all valid cluster entries (clusters 2 to TotalDataClusters+1)
        for (int cluster = 2; cluster < _bootSector.TotalDataClusters + 2; cluster++)
        {
            int value = ReadFatEntry(cluster);
            if (IsBadCluster(value))
            {
                badClusters.Add(cluster);
            }
        }
        
        return badClusters;
    }
    
    /// <summary>
    /// Reads a 12-bit FAT entry for the given cluster.
    /// FAT12 packs two entries into 3 bytes (1.5 bytes per entry).
    /// </summary>
    private int ReadFatEntry(int cluster)
    {
        // Calculate byte offset: cluster * 1.5
        int offset = cluster + (cluster / 2);
        
        if (offset + 1 >= _fatData.Length)
            return END_OF_CHAIN_MAX; // Beyond FAT bounds
        
        int value;
        
        if (cluster % 2 == 0)
        {
            // Even cluster: low 12 bits of the 16-bit word
            // Byte layout: [low 8 bits] [high 4 bits in low nibble]
            value = _fatData[offset] | ((_fatData[offset + 1] & 0x0F) << 8);
        }
        else
        {
            // Odd cluster: high 12 bits of the 16-bit word
            // Byte layout: [low 4 bits in high nibble] [high 8 bits]
            value = (_fatData[offset] >> 4) | (_fatData[offset + 1] << 4);
        }
        
        return value & 0xFFF; // Mask to 12 bits
    }
}
