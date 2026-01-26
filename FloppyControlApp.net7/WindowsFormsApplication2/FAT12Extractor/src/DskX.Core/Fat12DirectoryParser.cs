namespace DskX.Core;

/// <summary>
/// Parses FAT12 directory structures to extract file listings with start clusters.
/// </summary>
internal class Fat12DirectoryParser
{
    private readonly Stream _stream;
    private readonly Fat12BootSector _bootSector;
    private readonly Fat12Table _fatTable;
    
    /// <summary>
    /// Represents a parsed file/directory with its full path and metadata.
    /// </summary>
    public class ParsedFile
    {
        public string FileName { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public ushort StartCluster { get; set; }
        public uint FileSize { get; set; }
        public bool IsDirectory { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }
    
    public Fat12DirectoryParser(Stream stream, Fat12BootSector bootSector, Fat12Table fatTable)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        _bootSector = bootSector ?? throw new ArgumentNullException(nameof(bootSector));
        _fatTable = fatTable ?? throw new ArgumentNullException(nameof(fatTable));
    }
    
    /// <summary>
    /// Parses the root directory and all subdirectories.
    /// </summary>
    /// <returns>List of all files and directories with their full paths.</returns>
    public List<ParsedFile> ParseAll()
    {
        var allFiles = new List<ParsedFile>();
        
        // Parse root directory
        var rootEntries = ParseRootDirectory();
        
        foreach (var entry in rootEntries)
        {
            if (entry.IsDirectory)
            {
                // Recursively parse subdirectory
                var subFiles = ParseSubdirectory(entry.StartCluster, entry.FullPath);
                allFiles.AddRange(subFiles);
            }
            else
            {
                // Add file
                allFiles.Add(entry);
            }
        }
        
        return allFiles;
    }
    
    /// <summary>
    /// Parses the root directory entries.
    /// </summary>
    private List<ParsedFile> ParseRootDirectory()
    {
        var files = new List<ParsedFile>();
        
        // Calculate root directory location
        int rootDirOffset = (_bootSector.ReservedSectors + 
                            (_bootSector.NumberOfFats * _bootSector.SectorsPerFat)) * 
                            _bootSector.BytesPerSector;
        
        int rootDirSize = _bootSector.RootEntries * 32;
        
        // Read root directory
        byte[] rootDirData = new byte[rootDirSize];
        _stream.Position = rootDirOffset;
        int bytesRead = _stream.Read(rootDirData, 0, rootDirSize);
        
        if (bytesRead < rootDirSize)
        {
            Console.Error.WriteLine($"Warning: Root directory incomplete (expected {rootDirSize} bytes, got {bytesRead}).");
        }
        
        // Parse entries
        for (int offset = 0; offset < rootDirSize; offset += 32)
        {
            var entry = Fat12DirectoryEntry.Parse(rootDirData, offset);
            if (entry == null)
            {
                // Check if this is truly end of directory (first byte is 0x00)
                if (rootDirData[offset] == 0x00)
                    break; // End of directory
                // Otherwise, skip this entry (deleted or invalid) and continue
                continue;
            }
            
            files.Add(new ParsedFile
            {
                FileName = entry.FullName,
                FullPath = entry.FullName,
                StartCluster = entry.StartCluster,
                FileSize = entry.FileSize,
                IsDirectory = entry.IsDirectory,
                Created = entry.Created,
                Modified = entry.Modified
            });
        }
        
        return files;
    }
    
    /// <summary>
    /// Parses a subdirectory given its start cluster.
    /// </summary>
    private List<ParsedFile> ParseSubdirectory(int startCluster, string parentPath)
    {
        var files = new List<ParsedFile>();
        
        if (startCluster < 2)
            return files; // Invalid cluster
        
        // Get cluster chain for this directory
        var clusterChain = _fatTable.GetClusterChain(startCluster);
        
        // Read all clusters in the chain
        var dirData = new List<byte>();
        foreach (int cluster in clusterChain)
        {
            var sectors = _bootSector.ClusterToSectors(cluster);
            foreach (int sector in sectors)
            {
                byte[] sectorData = new byte[_bootSector.BytesPerSector];
                _stream.Position = sector * _bootSector.BytesPerSector;
                int bytesRead = _stream.Read(sectorData, 0, _bootSector.BytesPerSector);
                
                if (bytesRead == _bootSector.BytesPerSector)
                {
                    dirData.AddRange(sectorData);
                }
            }
        }
        
        // Parse directory entries
        byte[] dirBuffer = dirData.ToArray();
        for (int offset = 0; offset < dirBuffer.Length; offset += 32)
        {
            var entry = Fat12DirectoryEntry.Parse(dirBuffer, offset);
            if (entry == null)
                break; // End of directory
            
            // Skip "." and ".." entries
            if (entry.FileName == "." || entry.FileName == "..")
                continue;
            
            string fullPath = $"{parentPath}\\{entry.FullName}";
            
            if (entry.IsDirectory)
            {
                // Recursively parse subdirectory
                var subFiles = ParseSubdirectory(entry.StartCluster, fullPath);
                files.AddRange(subFiles);
            }
            else
            {
                files.Add(new ParsedFile
                {
                    FileName = entry.FullName,
                    FullPath = fullPath,
                    StartCluster = entry.StartCluster,
                    FileSize = entry.FileSize,
                    IsDirectory = false,
                    Created = entry.Created,
                    Modified = entry.Modified
                });
            }
        }
        
        return files;
    }
}
