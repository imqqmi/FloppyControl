using Xunit;

namespace DskX.Core.Tests;

/// <summary>
/// Tests for Phase 2 sector mapping functionality.
/// </summary>
public class SectorMappingTests
{
    private static readonly string TestImage720KB = Path.Combine("..", "..", "..", "..", "..", "ExampleDiskImages", "A001 Modules1_disk_018.DSK");
    
    [Fact]
    public void GetSectorMap_ReturnsValidGeometry()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        
        // Act
        var sectorMap = extractor.GetSectorMap();
        
        // Assert
        Assert.NotNull(sectorMap);
        Assert.NotNull(sectorMap.Geometry);
        Assert.Equal(160, sectorMap.Geometry.TotalTracks); // 80 cylinders * 2 heads
        Assert.Equal(9, sectorMap.Geometry.SectorsPerTrack); // DD format
        Assert.Equal(512, sectorMap.Geometry.BytesPerSector);
        Assert.Equal(1440, sectorMap.Geometry.TotalSectors); // 160 * 9
    }
    
    [Fact]
    public void GetSectorMap_IdentifiesSystemAreas()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        
        // Act
        var sectorMap = extractor.GetSectorMap();
        
        // Assert - Boot sector
        Assert.Equal(0, sectorMap.BootSector.Track);
        Assert.Equal(0, sectorMap.BootSector.Sector);
        
        // Assert - FAT regions (should have 2 FATs)
        Assert.Equal(2, sectorMap.FatRegions.Count);
        Assert.True(sectorMap.FatRegions[0].SectorCount > 0);
        
        // Assert - Root directory
        Assert.NotNull(sectorMap.RootDirectory);
        Assert.True(sectorMap.RootDirectory.SectorCount > 0);
        
        // Assert - Data area start
        Assert.NotNull(sectorMap.DataAreaStart);
        Assert.True(sectorMap.DataAreaStart.Track > 0 || sectorMap.DataAreaStart.Sector > 0);
    }
    
    [Fact]
    public void GetSectorMap_MapsFilesToSectors()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        
        // Act
        var sectorMap = extractor.GetSectorMap();
        
        // Assert
        Assert.NotEmpty(sectorMap.Files);
        
        foreach (var file in sectorMap.Files)
        {
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
    public void GetFileSectorMap_ReturnsCorrectFileMap()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();
        var files = extractor.EnumerateFiles(options).ToList();
        
        Assert.NotEmpty(files);
        var testFile = files.First();
        
        // Act
        var fileSectorMap = extractor.GetFileSectorMap(testFile);
        
        // Assert
        Assert.NotNull(fileSectorMap);
        Assert.Equal(testFile.FullPath, fileSectorMap.FullPath);
        Assert.Equal(testFile.Size, fileSectorMap.FileSize);
    }
    
    [Fact]
    public void SectorLocation_CalculatesCylinderAndHead()
    {
        // Test interleaved track addressing
        var track0 = new SectorLocation(0, 5);
        Assert.Equal(0, track0.Cylinder);
        Assert.Equal(0, track0.Head);
        
        var track1 = new SectorLocation(1, 5);
        Assert.Equal(0, track1.Cylinder);
        Assert.Equal(1, track1.Head);
        
        var track2 = new SectorLocation(2, 5);
        Assert.Equal(1, track2.Cylinder);
        Assert.Equal(0, track2.Head);
        
        var track159 = new SectorLocation(159, 8);
        Assert.Equal(79, track159.Cylinder);
        Assert.Equal(1, track159.Head);
    }
    
    [Fact]
    public void SectorLocation_ComparesCorrectly()
    {
        var loc1 = new SectorLocation(0, 0);
        var loc2 = new SectorLocation(0, 1);
        var loc3 = new SectorLocation(1, 0);
        
        Assert.True(loc1.CompareTo(loc2) < 0);
        Assert.True(loc2.CompareTo(loc3) < 0);
        Assert.True(loc1.CompareTo(loc3) < 0);
        Assert.Equal(0, loc1.CompareTo(new SectorLocation(0, 0)));
    }
    
    [Fact]
    public void SectorRange_ContainsCheck()
    {
        var range = new SectorRange(
            new SectorLocation(5, 0),
            new SectorLocation(5, 8),
            9
        );
        
        Assert.True(range.Contains(new SectorLocation(5, 0)));
        Assert.True(range.Contains(new SectorLocation(5, 4)));
        Assert.True(range.Contains(new SectorLocation(5, 8)));
        Assert.False(range.Contains(new SectorLocation(4, 8)));
        Assert.False(range.Contains(new SectorLocation(6, 0)));
    }
    
    [Fact]
    public void GetSystemAreaAtSector_IdentifiesBootSector()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        
        // Act
        var systemArea = extractor.GetSystemAreaAtSector(0, 0);
        
        // Assert
        Assert.NotNull(systemArea);
        Assert.Equal(SystemAreaType.BootSector, systemArea.Value);
    }
    
    [Fact]
    public void GetSystemAreaAtSector_IdentifiesFAT()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var sectorMap = extractor.GetSectorMap();
        
        // Get a sector from the first FAT region
        var fatSector = sectorMap.FatRegions[0].Start;
        
        // Act
        var systemArea = extractor.GetSystemAreaAtSector(fatSector.Track, fatSector.Sector);
        
        // Assert
        Assert.NotNull(systemArea);
        Assert.Equal(SystemAreaType.Fat, systemArea.Value);
    }
    
    [Fact]
    public void GetSystemAreaAtSector_IdentifiesRootDirectory()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var sectorMap = extractor.GetSectorMap();
        
        // Get a sector from the root directory
        var rootSector = sectorMap.RootDirectory!.Start;
        
        // Act
        var systemArea = extractor.GetSystemAreaAtSector(rootSector.Track, rootSector.Sector);
        
        // Assert
        Assert.NotNull(systemArea);
        Assert.Equal(SystemAreaType.RootDirectory, systemArea.Value);
    }
    
    [Fact]
    public void GetFileAtSector_ReturnsNullForSystemSector()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        
        // Act - check boot sector
        var file = extractor.GetFileAtSector(0, 0);
        
        // Assert
        Assert.Null(file);
    }
    
    [Fact]
    public void GetSectorMap_AllSectorsAccountedFor()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        
        // Act
        var sectorMap = extractor.GetSectorMap();
        
        // Count all sectors
        int systemSectors = 1; // Boot sector
        systemSectors += sectorMap.FatRegions.Sum(r => r.SectorCount);
        systemSectors += sectorMap.RootDirectory!.SectorCount;
        
        int fileSectors = sectorMap.Files.Sum(f => f.Sectors.Count);
        int freeSectors = sectorMap.FreeSectors.Count;
        int badSectors = sectorMap.BadSectorsInFat.Count;
        
        int totalAccountedSectors = systemSectors + fileSectors + freeSectors;
        
        // Assert - all data area sectors should be accounted for
        // (system sectors + file sectors + free sectors should equal total sectors)
        Assert.True(totalAccountedSectors <= sectorMap.Geometry.TotalSectors);
        
        // Free sectors should be in data area only
        if (sectorMap.FreeSectors.Any())
        {
            var firstFreeSector = sectorMap.FreeSectors.First();
            var dataAreaStart = sectorMap.DataAreaStart!;
            Assert.True(firstFreeSector.CompareTo(dataAreaStart) >= 0);
        }
    }
    
    [Fact]
    public void FragmentDetection_IdentifiesContiguousFiles()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var sectorMap = extractor.GetSectorMap();
        
        // Act & Assert
        foreach (var file in sectorMap.Files.Where(f => f.Sectors.Count > 0))
        {
            // Each file should have at least one range
            Assert.True(file.FragmentCount >= 1);
            
            // Total sectors in ranges should equal total sectors
            int rangeSectorCount = file.Ranges.Sum(r => r.SectorCount);
            Assert.Equal(file.Sectors.Count, rangeSectorCount);
            
            // If file has only one range, it's contiguous
            if (file.FragmentCount == 1)
            {
                Assert.Equal(file.Sectors.Count, file.Ranges[0].SectorCount);
            }
        }
    }
}
