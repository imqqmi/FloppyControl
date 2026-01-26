using Xunit;
using Xunit.Abstractions;

namespace DskX.Core.Tests;

/// <summary>
/// Integration test simulating FloppyControlApp usage.
/// </summary>
public class FloppyControlAppIntegrationTest
{
    private readonly ITestOutputHelper _output;
    private static readonly string TestImage720KB = Path.Combine("..", "..", "..", "..", "..", "ExampleDiskImages", "A001 Modules1_disk_018.DSK");
    
    public FloppyControlAppIntegrationTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void FloppyControlApp_WorkflowSimulation()
    {
        // Simulate FloppyControlApp workflow:
        // 1. Open disk image
        // 2. Get sector map
        // 3. Populate file dropdown
        // 4. Select a file and get its sectors
        // 5. Check if file overlaps with a "bad sector"
        // 6. Reverse lookup: click a sector to see what file it belongs to
        
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        
        // Step 1: Get complete sector map
        _output.WriteLine("Step 1: Getting sector map...");
        var diskMap = extractor.GetSectorMap();
        
        Assert.NotNull(diskMap);
        Assert.NotNull(diskMap.Geometry);
        _output.WriteLine($"Disk: {diskMap.Geometry.DiskType}");
        _output.WriteLine($"Geometry: {diskMap.Geometry.TotalTracks} tracks, {diskMap.Geometry.SectorsPerTrack} sectors/track");
        
        // Step 2: Populate file dropdown (simulate UI)
        _output.WriteLine("\nStep 2: Populating file dropdown...");
        var fileList = new List<string>();
        foreach (var file in diskMap.Files)
        {
            fileList.Add(file.FullPath);
            _output.WriteLine($"  - {file.FullPath} ({file.FileSize} bytes)");
        }
        
        Assert.NotEmpty(fileList);
        
        // Step 3: User selects a file (simulate selection)
        _output.WriteLine("\nStep 3: User selects file...");
        string selectedFile = fileList[0];
        _output.WriteLine($"Selected: {selectedFile}");
        
        var fileSectorMap = diskMap.Files.First(f => f.FullPath == selectedFile);
        Assert.NotNull(fileSectorMap);
        
        // Step 4: Highlight file sectors in UI
        _output.WriteLine("\nStep 4: Highlighting file sectors...");
        _output.WriteLine($"File occupies {fileSectorMap.Sectors.Count} sectors in {fileSectorMap.FragmentCount} fragment(s):");
        
        foreach (var range in fileSectorMap.Ranges)
        {
            _output.WriteLine($"  Range: {range}");
        }
        
        // Show first few sectors
        var firstSectors = fileSectorMap.Sectors.Take(5).ToList();
        foreach (var sector in firstSectors)
        {
            _output.WriteLine($"  Track {sector.Track}, Sector {sector.Sector} (Cyl {sector.Cylinder}, Head {sector.Head})");
        }
        
        // Step 5: Simulate bad sector check
        _output.WriteLine("\nStep 5: Checking for bad sector overlap...");
        // Simulate a bad sector at track 10, sector 5
        var simulatedBadSector = new SectorLocation(10, 5);
        bool fileHasBadSector = fileSectorMap.Sectors.Contains(simulatedBadSector);
        _output.WriteLine($"File overlaps with simulated bad sector (Track 10, Sector 5): {fileHasBadSector}");
        
        // Step 6: Reverse lookup - user clicks a sector
        _output.WriteLine("\nStep 6: Reverse lookup (user clicks sector)...");
        var clickedSector = fileSectorMap.Sectors.First();
        _output.WriteLine($"User clicked: Track {clickedSector.Track}, Sector {clickedSector.Sector}");
        
        var fileAtSector = extractor.GetFileAtSector(clickedSector.Track, clickedSector.Sector);
        Assert.NotNull(fileAtSector);
        _output.WriteLine($"This sector belongs to: {fileAtSector.FullPath}");
        
        // Step 7: Check system areas
        _output.WriteLine("\nStep 7: Identifying system areas...");
        _output.WriteLine($"Boot sector: Track {diskMap.BootSector.Track}, Sector {diskMap.BootSector.Sector}");
        
        var systemAreaAtBoot = extractor.GetSystemAreaAtSector(0, 0);
        Assert.Equal(SystemAreaType.BootSector, systemAreaAtBoot);
        _output.WriteLine($"System area at (0,0): {systemAreaAtBoot}");
        
        _output.WriteLine($"\nFAT regions: {diskMap.FatRegions.Count}");
        foreach (var fat in diskMap.FatRegions)
        {
            _output.WriteLine($"  {fat}");
        }
        
        _output.WriteLine($"\nRoot directory: {diskMap.RootDirectory}");
        
        // Step 8: Verify sector accounting
        _output.WriteLine("\nStep 8: Verifying sector accounting...");
        int systemSectors = 1 + diskMap.FatRegions.Sum(r => r.SectorCount) + diskMap.RootDirectory!.SectorCount;
        int fileSectors = diskMap.Files.Sum(f => f.Sectors.Count);
        int freeSectors = diskMap.FreeSectors.Count;
        
        _output.WriteLine($"System sectors: {systemSectors}");
        _output.WriteLine($"File sectors: {fileSectors}");
        _output.WriteLine($"Free sectors: {freeSectors}");
        _output.WriteLine($"Total accounted: {systemSectors + fileSectors + freeSectors}");
        _output.WriteLine($"Disk total: {diskMap.Geometry.TotalSectors}");
        
        // All sectors should be accounted for
        Assert.True(systemSectors + fileSectors + freeSectors <= diskMap.Geometry.TotalSectors);
        
        _output.WriteLine("\n✓ FloppyControlApp integration test complete!");
    }
}
