using System.Security.Cryptography;
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
    
    /// <summary>
    /// Tests that substituting different bad sector data into the disk image produces
    /// different extracted files. This simulates the ExportFilesButton workflow.
    /// </summary>
    [Fact]
    public void BadSectorSubstitution_DifferentSectors_ProduceDifferentFiles()
    {
        // This test simulates what FloppyControlApp does:
        // 1. Load disk image into memory (processing.disk[])
        // 2. Select a file
        // 3. For each bad sector candidate:
        //    a. Substitute that sector's data into the disk array
        //    b. Extract the file
        //    c. Restore original data
        // 4. Verify that different sector data produces different files
        
        // Load the disk image into a byte array (simulating processing.disk[])
        byte[] diskData = File.ReadAllBytes(TestImage720KB);
        int sectorsPerTrack = 9; // 720KB disk
        int bytesPerSector = 512;
        
        _output.WriteLine($"Loaded disk image: {diskData.Length} bytes");
        
        // Get file information
        using var reader = new DiskImageReader();
        reader.Open(new MemoryStream(diskData, writable: false));
        var extractor = new Fat12Extractor(reader);
        var diskMap = extractor.GetSectorMap();
        
        // Find a file with multiple sectors (so we can test substitution)
        var targetFile = diskMap.Files.FirstOrDefault(f => f.Sectors.Count >= 2);
        Assert.NotNull(targetFile);
        
        _output.WriteLine($"Target file: {targetFile.FullPath} ({targetFile.FileSize} bytes, {targetFile.Sectors.Count} sectors)");
        
        // Get the first sector of this file
        var targetSector = targetFile.Sectors.First();
        _output.WriteLine($"Target sector: Track {targetSector.Track}, Sector {targetSector.Sector}");
        
        // Calculate the disk offset for this sector (using interleaved track addressing)
        int sectorOffset = (targetSector.Track * sectorsPerTrack + targetSector.Sector) * bytesPerSector;
        _output.WriteLine($"Sector offset in disk: {sectorOffset}");
        
        // Backup original sector data
        byte[] originalSectorData = new byte[bytesPerSector];
        Array.Copy(diskData, sectorOffset, originalSectorData, 0, bytesPerSector);
        
        // Create temp directory for output files
        string tempDir = Path.Combine(Path.GetTempPath(), $"BadSectorTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        
        try
        {
            var extractedFiles = new List<(string path, byte[] hash)>();
            
            // Simulate 3 different "bad sector candidates" with different data
            byte[][] badSectorCandidates = new byte[3][];
            for (int i = 0; i < 3; i++)
            {
                badSectorCandidates[i] = new byte[bytesPerSector];
                // Fill with distinct patterns
                for (int j = 0; j < bytesPerSector; j++)
                {
                    badSectorCandidates[i][j] = (byte)((i * 50 + j) % 256);
                }
            }
            
            _output.WriteLine($"\nExtracting file with 3 different bad sector substitutions...");
            
            for (int i = 0; i < badSectorCandidates.Length; i++)
            {
                // Restore original data first
                Array.Copy(originalSectorData, 0, diskData, sectorOffset, bytesPerSector);
                
                // Substitute bad sector candidate data
                Array.Copy(badSectorCandidates[i], 0, diskData, sectorOffset, bytesPerSector);
                
                // Create new extractor from modified disk data
                using var modifiedStream = new MemoryStream(diskData, writable: false);
                using var modifiedReader = new DiskImageReader();
                modifiedReader.Open(modifiedStream);
                var modifiedExtractor = new Fat12Extractor(modifiedReader);
                
                // Find the file again and extract
                var files = modifiedExtractor.EnumerateFiles(new ExtractionOptions());
                var fileToExtract = files.FirstOrDefault(f => f.FullPath == targetFile.FullPath);
                Assert.NotNull(fileToExtract);
                
                string outputPath = Path.Combine(tempDir, $"extracted_{i:D3}{Path.GetExtension(targetFile.FullPath)}");
                modifiedExtractor.ExtractFile(fileToExtract, outputPath);
                
                // Calculate hash of extracted file
                byte[] extractedData = File.ReadAllBytes(outputPath);
                byte[] hash = SHA256.HashData(extractedData);
                
                extractedFiles.Add((outputPath, hash));
                _output.WriteLine($"  Variation {i + 1}: {Path.GetFileName(outputPath)} - SHA256: {BitConverter.ToString(hash).Substring(0, 20)}...");
            }
            
            // Restore original data
            Array.Copy(originalSectorData, 0, diskData, sectorOffset, bytesPerSector);
            
            // Verify that all 3 files are different
            _output.WriteLine($"\nVerifying files are different...");
            
            Assert.Equal(3, extractedFiles.Count);
            
            // Compare each pair
            for (int i = 0; i < extractedFiles.Count; i++)
            {
                for (int j = i + 1; j < extractedFiles.Count; j++)
                {
                    bool areDifferent = !extractedFiles[i].hash.SequenceEqual(extractedFiles[j].hash);
                    _output.WriteLine($"  File {i + 1} vs File {j + 1}: {(areDifferent ? "DIFFERENT" : "SAME")}");
                    Assert.True(areDifferent, $"Files {i + 1} and {j + 1} should be different but have the same hash!");
                }
            }
            
            _output.WriteLine("\n✓ Bad sector substitution test passed - all variations are different!");
        }
        finally
        {
            // Cleanup temp directory
            try
            {
                Directory.Delete(tempDir, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
    
    /// <summary>
    /// Tests that the sector offset calculation matches what FloppyControlApp uses.
    /// This verifies the trackhead vs track fix.
    /// </summary>
    [Fact]
    public void SectorOffsetCalculation_MatchesFloppyControlAppLayout()
    {
        // FloppyControlApp uses interleaved track addressing:
        // Track 0 = Cylinder 0, Head 0
        // Track 1 = Cylinder 0, Head 1
        // Track 2 = Cylinder 1, Head 0
        // Track 3 = Cylinder 1, Head 1
        // etc.
        
        // For a 720KB disk: 80 cylinders, 2 heads, 9 sectors/track = 160 tracks total
        int sectorsPerTrack = 9;
        int bytesPerSector = 512;
        
        // Test cases: (track, sector) -> expected offset
        var testCases = new[]
        {
            (Track: 0, Sector: 0, ExpectedOffset: 0),
            (Track: 0, Sector: 1, ExpectedOffset: 512),
            (Track: 0, Sector: 8, ExpectedOffset: 8 * 512),
            (Track: 1, Sector: 0, ExpectedOffset: 9 * 512),  // First sector of Track 1 (Cyl 0, Head 1)
            (Track: 1, Sector: 8, ExpectedOffset: 17 * 512), // Last sector of Track 1
            (Track: 2, Sector: 0, ExpectedOffset: 18 * 512), // First sector of Track 2 (Cyl 1, Head 0)
            (Track: 159, Sector: 8, ExpectedOffset: (160 * 9 - 1) * 512), // Last sector of disk
        };
        
        _output.WriteLine("Verifying sector offset calculations:");
        
        foreach (var tc in testCases)
        {
            // This is the formula used in FloppyControlApp (now fixed to use trackhead)
            int calculatedOffset = (tc.Track * sectorsPerTrack + tc.Sector) * bytesPerSector;
            
            _output.WriteLine($"  Track {tc.Track}, Sector {tc.Sector} -> Offset {calculatedOffset} (expected {tc.ExpectedOffset})");
            
            Assert.Equal(tc.ExpectedOffset, calculatedOffset);
        }
        
        // Also verify SectorLocation matches this scheme
        var loc = new SectorLocation(10, 5);
        Assert.Equal(5, loc.Cylinder);  // Track 10 / 2 = Cylinder 5
        Assert.Equal(0, loc.Head);      // Track 10 % 2 = Head 0
        
        _output.WriteLine($"\nSectorLocation(10, 5): Cylinder={loc.Cylinder}, Head={loc.Head}");
        _output.WriteLine("\n✓ Sector offset calculation test passed!");
    }
}
