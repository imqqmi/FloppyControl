namespace DskX.Core;

/// <summary>
/// Maps files to their physical sector locations on a FAT12 disk.
/// Provides complete sector-level information for FloppyControlApp integration.
/// </summary>
public class SectorMapper
{
    private readonly Stream _stream;
    private Fat12BootSector? _bootSector;
    private Fat12Table? _fatTable;
    private DiskSectorMap? _cachedMap;
    private Dictionary<(int Track, int Sector), FileEntry>? _reverseLookup;
    
    /// <summary>
    /// Initializes a new instance of the SectorMapper class.
    /// </summary>
    /// <param name="stream">Stream to the disk image (must be seekable).</param>
    public SectorMapper(Stream stream)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        
        if (!stream.CanSeek)
            throw new ArgumentException("Stream must be seekable.", nameof(stream));
    }
    
    /// <summary>
    /// Builds the complete disk sector map.
    /// This operation is cached - subsequent calls return the same map.
    /// </summary>
    /// <returns>Complete disk sector map with all files and system areas.</returns>
    public DiskSectorMap BuildDiskSectorMap()
    {
        // Return cached map if available
        if (_cachedMap != null)
            return _cachedMap;
        
        // Parse boot sector
        _bootSector = Fat12BootSector.Parse(_stream);
        
        // Load FAT table
        _fatTable = Fat12Table.Load(_stream, _bootSector);
        
        // Create disk sector map
        var map = new DiskSectorMap
        {
            Geometry = _bootSector.ToDiskGeometry(),
            BootSector = new SectorLocation(0, 0)
        };
        
        // Map system areas
        MapSystemAreas(map, _bootSector);
        
        // Parse directory tree
        var parser = new Fat12DirectoryParser(_stream, _bootSector, _fatTable);
        var parsedFiles = parser.ParseAll();
        
        // Build file sector maps
        var usedSectors = new HashSet<(int Track, int Sector)>();
        
        foreach (var file in parsedFiles)
        {
            var fileSectorMap = BuildFileSectorMap(file, _bootSector, _fatTable);
            map.Files.Add(fileSectorMap);
            
            // Track used sectors
            foreach (var sector in fileSectorMap.Sectors)
            {
                usedSectors.Add((sector.Track, sector.Sector));
            }
        }
        
        // Identify bad sectors from FAT
        MapBadSectors(map, _bootSector, _fatTable);
        
        // Identify free sectors
        MapFreeSectors(map, _bootSector, usedSectors);
        
        // Cache the map
        _cachedMap = map;
        
        return map;
    }
    
    /// <summary>
    /// Gets the sector map for a specific file.
    /// </summary>
    /// <param name="entry">File entry to map.</param>
    /// <returns>Sector map for the file, or null if not found.</returns>
    public FileSectorMap? GetFileSectorMap(FileEntry entry)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));
        
        var diskMap = BuildDiskSectorMap();
        
        // Find matching file by path (case-insensitive for FAT12)
        return diskMap.Files.FirstOrDefault(f => 
            string.Equals(f.FullPath, entry.FullPath, StringComparison.OrdinalIgnoreCase));
    }
    
    /// <summary>
    /// Finds which file (if any) occupies the given sector.
    /// </summary>
    /// <param name="track">Track number.</param>
    /// <param name="sector">Sector number within track.</param>
    /// <returns>File entry occupying this sector, or null if free/system sector.</returns>
    public FileEntry? GetFileAtSector(int track, int sector)
    {
        // Build reverse lookup on first call
        if (_reverseLookup == null)
        {
            BuildReverseLookup();
        }
        
        _reverseLookup!.TryGetValue((track, sector), out var file);
        return file;
    }
    
    /// <summary>
    /// Checks if a sector is in a system area (boot, FAT, root directory).
    /// </summary>
    /// <param name="track">Track number.</param>
    /// <param name="sector">Sector number within track.</param>
    /// <returns>System area type, or null if not a system sector.</returns>
    public SystemAreaType? GetSystemAreaAtSector(int track, int sector)
    {
        var diskMap = BuildDiskSectorMap();
        var location = new SectorLocation(track, sector);
        
        // Check boot sector
        if (location.Equals(diskMap.BootSector))
            return SystemAreaType.BootSector;
        
        // Check FAT regions
        foreach (var fatRange in diskMap.FatRegions)
        {
            if (fatRange.Contains(location))
                return SystemAreaType.Fat;
        }
        
        // Check root directory
        if (diskMap.RootDirectory != null && diskMap.RootDirectory.Contains(location))
            return SystemAreaType.RootDirectory;
        
        return null;
    }
    
    /// <summary>
    /// Maps system areas (boot sector, FATs, root directory).
    /// </summary>
    private void MapSystemAreas(DiskSectorMap map, Fat12BootSector bootSector)
    {
        // Boot sector is always at track 0, sector 0
        map.BootSector = new SectorLocation(0, 0);
        
        // Map FAT regions
        int fatStartSector = bootSector.ReservedSectors;
        for (int i = 0; i < bootSector.NumberOfFats; i++)
        {
            int fatStart = fatStartSector + (i * bootSector.SectorsPerFat);
            int fatEnd = fatStart + bootSector.SectorsPerFat - 1;
            
            var startLocation = bootSector.LinearSectorToLocation(fatStart);
            var endLocation = bootSector.LinearSectorToLocation(fatEnd);
            
            map.FatRegions.Add(new SectorRange(startLocation, endLocation, bootSector.SectorsPerFat));
        }
        
        // Map root directory
        int rootDirStart = bootSector.ReservedSectors + (bootSector.NumberOfFats * bootSector.SectorsPerFat);
        int rootDirEnd = rootDirStart + bootSector.RootDirectorySectors - 1;
        
        var rootStart = bootSector.LinearSectorToLocation(rootDirStart);
        var rootEnd = bootSector.LinearSectorToLocation(rootDirEnd);
        
        map.RootDirectory = new SectorRange(rootStart, rootEnd, bootSector.RootDirectorySectors);
        
        // Data area starts after root directory
        map.DataAreaStart = bootSector.LinearSectorToLocation(bootSector.FirstDataSector);
    }
    
    /// <summary>
    /// Builds a file sector map from a parsed file.
    /// </summary>
    private FileSectorMap BuildFileSectorMap(
        Fat12DirectoryParser.ParsedFile file, 
        Fat12BootSector bootSector, 
        Fat12Table fatTable)
    {
        var map = new FileSectorMap
        {
            FileName = file.FileName,
            FullPath = file.FullPath,
            FileSize = file.FileSize
        };
        
        // Get cluster chain
        if (file.StartCluster >= 2)
        {
            map.ClusterChain = fatTable.GetClusterChain(file.StartCluster);
            
            // Convert clusters to sectors
            var allSectors = new List<SectorLocation>();
            foreach (int cluster in map.ClusterChain)
            {
                var clusterSectors = bootSector.ClusterToSectors(cluster);
                foreach (int linearSector in clusterSectors)
                {
                    allSectors.Add(bootSector.LinearSectorToLocation(linearSector));
                }
            }
            
            map.Sectors = allSectors;
            
            // Build contiguous ranges
            map.Ranges = BuildSectorRanges(allSectors, bootSector.SectorsPerTrack);
        }
        
        return map;
    }
    
    /// <summary>
    /// Builds contiguous sector ranges from a list of sectors.
    /// Handles track boundaries correctly.
    /// </summary>
    private List<SectorRange> BuildSectorRanges(List<SectorLocation> sectors, int sectorsPerTrack)
    {
        var ranges = new List<SectorRange>();
        
        if (sectors.Count == 0)
            return ranges;
        
        // Sort sectors
        var sortedSectors = sectors.OrderBy(s => s).ToList();
        
        SectorLocation rangeStart = sortedSectors[0];
        SectorLocation rangeLast = sortedSectors[0];
        int rangeCount = 1;
        
        for (int i = 1; i < sortedSectors.Count; i++)
        {
            var current = sortedSectors[i];
            
            // Check if contiguous with previous sector
            bool isContiguous = false;
            
            if (current.Track == rangeLast.Track && current.Sector == rangeLast.Sector + 1)
            {
                // Same track, next sector
                isContiguous = true;
            }
            else if (current.Track == rangeLast.Track + 1 && 
                     current.Sector == 0 && 
                     rangeLast.Sector == sectorsPerTrack - 1)
            {
                // Next track, wrapped around
                isContiguous = true;
            }
            
            if (isContiguous)
            {
                // Extend current range
                rangeLast = current;
                rangeCount++;
            }
            else
            {
                // Save current range and start new one
                ranges.Add(new SectorRange(rangeStart, rangeLast, rangeCount));
                rangeStart = current;
                rangeLast = current;
                rangeCount = 1;
            }
        }
        
        // Add final range
        ranges.Add(new SectorRange(rangeStart, rangeLast, rangeCount));
        
        return ranges;
    }
    
    /// <summary>
    /// Maps bad sectors marked in the FAT.
    /// </summary>
    private void MapBadSectors(DiskSectorMap map, Fat12BootSector bootSector, Fat12Table fatTable)
    {
        var badClusters = fatTable.GetBadClusters();
        
        foreach (int cluster in badClusters)
        {
            var clusterSectors = bootSector.ClusterToSectors(cluster);
            foreach (int linearSector in clusterSectors)
            {
                map.BadSectorsInFat.Add(bootSector.LinearSectorToLocation(linearSector));
            }
        }
    }
    
    /// <summary>
    /// Maps free (unallocated) sectors in the data area.
    /// </summary>
    private void MapFreeSectors(
        DiskSectorMap map, 
        Fat12BootSector bootSector, 
        HashSet<(int Track, int Sector)> usedSectors)
    {
        // Iterate through all data area sectors
        for (int linearSector = bootSector.FirstDataSector; linearSector < bootSector.TotalSectors; linearSector++)
        {
            var location = bootSector.LinearSectorToLocation(linearSector);
            
            if (!usedSectors.Contains((location.Track, location.Sector)))
            {
                map.FreeSectors.Add(location);
            }
        }
    }
    
    /// <summary>
    /// Builds reverse lookup index (sector → file).
    /// </summary>
    private void BuildReverseLookup()
    {
        var diskMap = BuildDiskSectorMap();
        _reverseLookup = new Dictionary<(int Track, int Sector), FileEntry>();
        
        foreach (var file in diskMap.Files)
        {
            // Create a FileEntry for this file (for compatibility with existing code)
            var fileEntry = new FileEntry
            {
                Name = file.FileName,
                FullPath = file.FullPath,
                Size = file.FileSize,
                IsDirectory = false
            };
            
            foreach (var sector in file.Sectors)
            {
                _reverseLookup[(sector.Track, sector.Sector)] = fileEntry;
            }
        }
    }
}
