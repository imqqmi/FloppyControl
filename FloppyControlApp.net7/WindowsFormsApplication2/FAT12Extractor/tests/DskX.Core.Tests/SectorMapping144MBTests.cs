using Xunit;
using Xunit.Abstractions;

namespace DskX.Core.Tests;

/// <summary>
/// Tests for Phase 2 sector mapping functionality with 1.44MB HD disk.
/// </summary>
public class SectorMapping144MBTests
{
    private readonly ITestOutputHelper _output;
    private static readonly string TestImage144MB = Path.Combine("..", "..", "..", "..", "..", "ExampleDiskImages", "PictureDiskTestA_disk_027.DSK");
    
    public SectorMapping144MBTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void GetSectorMap_144MB_ReturnsValidGeometry()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage144MB);
        var extractor = new Fat12Extractor(reader);
        
        // Act
        var sectorMap = extractor.GetSectorMap();
        
        // Assert
        Assert.NotNull(sectorMap);
        Assert.NotNull(sectorMap.Geometry);
        _output.WriteLine($"Disk type: {sectorMap.Geometry.DiskType}");
        _output.WriteLine($"Total tracks: {sectorMap.Geometry.TotalTracks}");
        _output.WriteLine($"Sectors per track: {sectorMap.Geometry.SectorsPerTrack}");
        _output.WriteLine($"Total sectors: {sectorMap.Geometry.TotalSectors}");
        
        Assert.Equal(160, sectorMap.Geometry.TotalTracks); // 80 cylinders * 2 heads
        Assert.Equal(18, sectorMap.Geometry.SectorsPerTrack); // HD format
        Assert.Equal(512, sectorMap.Geometry.BytesPerSector);
        Assert.Equal(2880, sectorMap.Geometry.TotalSectors); // 160 * 18
    }
    
    [Fact]
    public void GetSectorMap_144MB_IdentifiesSystemAreas()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage144MB);
        var extractor = new Fat12Extractor(reader);
        
        // Act
        var sectorMap = extractor.GetSectorMap();
        
        // Assert - Boot sector
        Assert.Equal(0, sectorMap.BootSector.Track);
        Assert.Equal(0, sectorMap.BootSector.Sector);
        
        // Assert - FAT regions (should have 2 FATs)
        Assert.Equal(2, sectorMap.FatRegions.Count);
        _output.WriteLine($"FAT 1: {sectorMap.FatRegions[0]}");
        _output.WriteLine($"FAT 2: {sectorMap.FatRegions[1]}");
        
        // Assert - Root directory
        Assert.NotNull(sectorMap.RootDirectory);
        _output.WriteLine($"Root directory: {sectorMap.RootDirectory}");
        
        // Assert - Data area start
        Assert.NotNull(sectorMap.DataAreaStart);
        _output.WriteLine($"Data area starts at: {sectorMap.DataAreaStart}");
    }
    
    [Fact]
    public void GetSectorMap_144MB_MapsFilesToSectors()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage144MB);
        var extractor = new Fat12Extractor(reader);
        
        // Act
        var sectorMap = extractor.GetSectorMap();
        
        // Assert
        _output.WriteLine($"Files found: {sectorMap.Files.Count}");
        Assert.NotEmpty(sectorMap.Files);
        
        foreach (var file in sectorMap.Files)
        {
            _output.WriteLine($"  {file.FullPath} - {file.FileSize} bytes, {file.Sectors.Count} sectors, {file.FragmentCount} fragment(s)");
            
            Assert.NotEmpty(file.FileName);
            Assert.NotEmpty(file.FullPath);
            
            if (file.FileSize > 0)
            {
                // Files with content should have sectors
                Assert.NotEmpty(file.Sectors);
                Assert.NotEmpty(file.ClusterChain);
                Assert.NotEmpty(file.Ranges);
                Assert.True(file.FragmentCount > 0);
            }
        }
    }
    
    [Fact]
    public void GetSectorMap_144MB_AllSectorsAccountedFor()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage144MB);
        var extractor = new Fat12Extractor(reader);
        
        // Act
        var sectorMap = extractor.GetSectorMap();
        
        // Count all sectors
        int systemSectors = 1; // Boot sector
        systemSectors += sectorMap.FatRegions.Sum(r => r.SectorCount);
        systemSectors += sectorMap.RootDirectory!.SectorCount;
        
        int fileSectors = sectorMap.Files.Sum(f => f.Sectors.Count);
        int freeSectors = sectorMap.FreeSectors.Count;
        
        _output.WriteLine($"System sectors: {systemSectors}");
        _output.WriteLine($"File sectors: {fileSectors}");
        _output.WriteLine($"Free sectors: {freeSectors}");
        _output.WriteLine($"Total accounted: {systemSectors + fileSectors + freeSectors}");
        _output.WriteLine($"Disk total: {sectorMap.Geometry.TotalSectors}");
        
        int totalAccountedSectors = systemSectors + fileSectors + freeSectors;
        
        // Assert - all sectors should be accounted for
        Assert.Equal(sectorMap.Geometry.TotalSectors, totalAccountedSectors);
    }
    
    [Fact]
    public void GetSectorMap_144MB_FragmentationDetection()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage144MB);
        var extractor = new Fat12Extractor(reader);
        var sectorMap = extractor.GetSectorMap();
        
        // Act & Assert
        _output.WriteLine("File fragmentation analysis:");
        foreach (var file in sectorMap.Files.Where(f => f.Sectors.Count > 0))
        {
            // Each file should have at least one range
            Assert.True(file.FragmentCount >= 1);
            
            // Total sectors in ranges should equal total sectors
            int rangeSectorCount = file.Ranges.Sum(r => r.SectorCount);
            Assert.Equal(file.Sectors.Count, rangeSectorCount);
            
            _output.WriteLine($"  {file.FullPath}: {file.FragmentCount} fragment(s)");
            foreach (var range in file.Ranges)
            {
                _output.WriteLine($"    - {range}");
            }
        }
    }
    
    [Fact]
    public void GetSectorMap_144MB_IntegrationWorkflow()
    {
        // Simulate FloppyControlApp workflow with 1.44MB disk
        using var reader = new DiskImageReader();
        reader.Open(TestImage144MB);
        var extractor = new Fat12Extractor(reader);
        
        _output.WriteLine("=== FloppyControlApp Integration Test (1.44MB HD) ===\n");
        
        // Step 1: Get sector map
        _output.WriteLine("Step 1: Getting sector map...");
        var diskMap = extractor.GetSectorMap();
        _output.WriteLine($"Disk: {diskMap.Geometry.DiskType}");
        _output.WriteLine($"Geometry: {diskMap.Geometry.TotalTracks} tracks, {diskMap.Geometry.SectorsPerTrack} sectors/track\n");
        
        // Step 2: List files
        _output.WriteLine("Step 2: Files on disk:");
        foreach (var file in diskMap.Files)
        {
            _output.WriteLine($"  - {file.FullPath} ({file.FileSize} bytes)");
        }
        
        // Step 3: Select first file and show sectors
        if (diskMap.Files.Any())
        {
            var firstFile = diskMap.Files[0];
            _output.WriteLine($"\nStep 3: Selected file: {firstFile.FullPath}");
            _output.WriteLine($"Occupies {firstFile.Sectors.Count} sectors in {firstFile.FragmentCount} fragment(s)");
            
            // Show first few sectors
            var firstSectors = firstFile.Sectors.Take(5).ToList();
            _output.WriteLine("First 5 sectors:");
            foreach (var sector in firstSectors)
            {
                _output.WriteLine($"  Track {sector.Track}, Sector {sector.Sector} (Cyl {sector.Cylinder}, Head {sector.Head})");
            }
            
            // Step 4: Reverse lookup
            _output.WriteLine($"\nStep 4: Reverse lookup on first sector...");
            var clickedSector = firstFile.Sectors.First();
            var fileAtSector = extractor.GetFileAtSector(clickedSector.Track, clickedSector.Sector);
            Assert.NotNull(fileAtSector);
            _output.WriteLine($"Sector (Track {clickedSector.Track}, Sector {clickedSector.Sector}) belongs to: {fileAtSector.FullPath}");
        }
        
        // Step 5: System areas
        _output.WriteLine($"\nStep 5: System areas:");
        _output.WriteLine($"Boot sector: {diskMap.BootSector}");
        _output.WriteLine($"FAT regions: {diskMap.FatRegions.Count}");
        foreach (var fat in diskMap.FatRegions)
        {
            _output.WriteLine($"  {fat}");
        }
        _output.WriteLine($"Root directory: {diskMap.RootDirectory}");
        
        _output.WriteLine("\n✓ Integration test complete!");
    }
}
