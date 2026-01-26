using System.Text;

namespace DskX.Core;

/// <summary>
/// Parses and represents the FAT12 boot sector (BIOS Parameter Block).
/// The boot sector is always at sector 0 and contains critical disk geometry information.
/// </summary>
public class Fat12BootSector
{
    // Raw BPB fields
    public ushort BytesPerSector { get; private set; }
    public byte SectorsPerCluster { get; private set; }
    public ushort ReservedSectors { get; private set; }
    public byte NumberOfFats { get; private set; }
    public ushort RootEntries { get; private set; }
    public ushort TotalSectors16 { get; private set; }
    public byte MediaDescriptor { get; private set; }
    public ushort SectorsPerFat { get; private set; }
    public ushort SectorsPerTrack { get; private set; }
    public ushort NumberOfHeads { get; private set; }
    public uint HiddenSectors { get; private set; }
    public uint TotalSectors32 { get; private set; }
    
    // Computed properties
    
    /// <summary>
    /// Gets the number of sectors occupied by the root directory.
    /// </summary>
    public int RootDirectorySectors => 
        (RootEntries * 32 + BytesPerSector - 1) / BytesPerSector;
    
    /// <summary>
    /// Gets the first sector of the data area (where file clusters begin).
    /// Cluster 2 starts at this sector.
    /// </summary>
    public int FirstDataSector => 
        ReservedSectors + (NumberOfFats * SectorsPerFat) + RootDirectorySectors;
    
    /// <summary>
    /// Gets the total number of sectors on the disk.
    /// Uses TotalSectors16 if non-zero, otherwise TotalSectors32.
    /// </summary>
    public int TotalSectors => 
        TotalSectors16 != 0 ? TotalSectors16 : (int)TotalSectors32;
    
    /// <summary>
    /// Gets the total number of data clusters available.
    /// </summary>
    public int TotalDataClusters => 
        (TotalSectors - FirstDataSector) / SectorsPerCluster;
    
    /// <summary>
    /// Gets whether this is a valid FAT12 filesystem (cluster count < 4085).
    /// </summary>
    public bool IsFat12 => TotalDataClusters < 4085;
    
    /// <summary>
    /// Gets the total number of tracks (cylinders * heads).
    /// </summary>
    public int TotalTracks => (TotalSectors / SectorsPerTrack);
    
    /// <summary>
    /// Parses the boot sector from a stream.
    /// </summary>
    /// <param name="stream">Stream positioned at the start of the boot sector (sector 0).</param>
    /// <returns>Parsed boot sector information.</returns>
    /// <exception cref="InvalidDataException">Thrown if the boot sector is invalid.</exception>
    public static Fat12BootSector Parse(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        
        // Read the entire boot sector (512 bytes)
        byte[] buffer = new byte[512];
        stream.Position = 0;
        int bytesRead = stream.Read(buffer, 0, 512);
        
        if (bytesRead < 512)
            throw new InvalidDataException("Boot sector is incomplete (less than 512 bytes).");
        
        // Verify boot sector signature (0x55AA at offset 510)
        if (buffer[510] != 0x55 || buffer[511] != 0xAA)
            throw new InvalidDataException("Invalid boot sector signature (expected 0x55AA at offset 510).");
        
        var bootSector = new Fat12BootSector
        {
            BytesPerSector = ReadUInt16(buffer, 0x0B),
            SectorsPerCluster = buffer[0x0D],
            ReservedSectors = ReadUInt16(buffer, 0x0E),
            NumberOfFats = buffer[0x10],
            RootEntries = ReadUInt16(buffer, 0x11),
            TotalSectors16 = ReadUInt16(buffer, 0x13),
            MediaDescriptor = buffer[0x15],
            SectorsPerFat = ReadUInt16(buffer, 0x16),
            SectorsPerTrack = ReadUInt16(buffer, 0x18),
            NumberOfHeads = ReadUInt16(buffer, 0x1A),
            HiddenSectors = ReadUInt32(buffer, 0x1C),
            TotalSectors32 = ReadUInt32(buffer, 0x20)
        };
        
        // Validate critical fields
        if (bootSector.BytesPerSector != 512)
            throw new InvalidDataException($"Unsupported bytes per sector: {bootSector.BytesPerSector} (expected 512).");
        
        if (bootSector.SectorsPerCluster == 0)
            throw new InvalidDataException("Invalid sectors per cluster (0).");
        
        if (bootSector.NumberOfFats == 0)
            throw new InvalidDataException("Invalid number of FATs (0).");
        
        if (bootSector.TotalSectors == 0)
            throw new InvalidDataException("Invalid total sectors (0).");
        
        if (!bootSector.IsFat12)
            throw new InvalidDataException($"Not a FAT12 filesystem (cluster count: {bootSector.TotalDataClusters}, expected < 4085).");
        
        return bootSector;
    }
    
    /// <summary>
    /// Converts a cluster number to its first sector number.
    /// </summary>
    /// <param name="cluster">Cluster number (must be >= 2).</param>
    /// <returns>Linear sector number (0-based).</returns>
    public int ClusterToFirstSector(int cluster)
    {
        if (cluster < 2)
            throw new ArgumentException("Cluster numbers start at 2.", nameof(cluster));
        
        return FirstDataSector + (cluster - 2) * SectorsPerCluster;
    }
    
    /// <summary>
    /// Converts a cluster number to all its sector numbers.
    /// </summary>
    /// <param name="cluster">Cluster number (must be >= 2).</param>
    /// <returns>List of linear sector numbers in this cluster.</returns>
    public List<int> ClusterToSectors(int cluster)
    {
        int firstSector = ClusterToFirstSector(cluster);
        var sectors = new List<int>();
        
        for (int i = 0; i < SectorsPerCluster; i++)
        {
            sectors.Add(firstSector + i);
        }
        
        return sectors;
    }
    
    /// <summary>
    /// Converts a linear sector number to a SectorLocation (track/sector addressing).
    /// Uses FloppyControlApp's interleaved track scheme.
    /// </summary>
    /// <param name="linearSector">Linear sector number (0-based).</param>
    /// <returns>Sector location with track and sector.</returns>
    public SectorLocation LinearSectorToLocation(int linearSector)
    {
        int track = linearSector / SectorsPerTrack;
        int sector = linearSector % SectorsPerTrack;
        
        return new SectorLocation(track, sector);
    }
    
    /// <summary>
    /// Creates a DiskGeometry object from this boot sector.
    /// </summary>
    public DiskGeometry ToDiskGeometry()
    {
        // Determine disk type string
        string diskType;
        if (TotalSectors == 1440 && SectorsPerTrack == 9)
            diskType = "720KB DD";
        else if (TotalSectors == 2880 && SectorsPerTrack == 18)
            diskType = "1.44MB HD";
        else
            diskType = $"{TotalSectors * BytesPerSector / 1024}KB";
        
        return new DiskGeometry
        {
            TotalTracks = TotalTracks,
            SectorsPerTrack = SectorsPerTrack,
            BytesPerSector = BytesPerSector,
            SectorsPerCluster = SectorsPerCluster,
            TotalSectors = TotalSectors,
            TotalClusters = TotalDataClusters,
            DiskType = diskType
        };
    }
    
    // Helper methods for reading little-endian values
    
    private static ushort ReadUInt16(byte[] buffer, int offset)
    {
        return (ushort)(buffer[offset] | (buffer[offset + 1] << 8));
    }
    
    private static uint ReadUInt32(byte[] buffer, int offset)
    {
        return (uint)(buffer[offset] | 
                     (buffer[offset + 1] << 8) | 
                     (buffer[offset + 2] << 16) | 
                     (buffer[offset + 3] << 24));
    }
}
