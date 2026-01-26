# Phase 2 Specification: Sector Mapping for FloppyControlApp Integration

**Created**: 2026-01-26  
**Status**: Draft  
**Goal**: Add sector-level file location mapping to enable visual overlay in FloppyControlApp

---

## Overview

Extend DskX.Core to provide sector-level location information for all files and system areas on a FAT12 disk. This enables FloppyControlApp to display which sectors each file occupies, crucial for bad sector recovery workflows.

### Use Case

FloppyControlApp displays a sector map showing good/bad sectors from flux decoding. Users need to:
1. Select a file from a dropdown
2. See which sectors that file occupies (highlighted overlay)
3. Identify when a file spans a bad sector
4. Correlate file corruption with physical sector damage

This enables targeted recovery: "PHOTO.JPG uses sectors 47, 48, 52-58. Sector 48 has 3 decode variants. Try each variant to find the best JPEG."

---

## FloppyControlApp Sector Addressing

### Track Numbering
Tracks are **interleaved by head** (not sequential):
- Track 0 = Cylinder 0, Head 0
- Track 1 = Cylinder 0, Head 1
- Track 2 = Cylinder 1, Head 0
- Track 3 = Cylinder 1, Head 1
- ...

For a standard 80-cylinder disk:
- Tracks 0-159 for double-sided disks
- Track N: Cylinder = N / 2, Head = N % 2

### Sector Numbering
Sectors are **0-based** within each track:
- Double Density (720KB): Sectors 0-8 (9 sectors/track)
- High Density (1.44MB): Sectors 0-17 (18 sectors/track)

### Conversion Formulas
```
Linear Sector → Track/Sector:
  track = linear_sector / sectors_per_track
  sector = linear_sector % sectors_per_track

Track/Sector → Cylinder/Head/Sector:
  cylinder = track / 2
  head = track % 2
```

---

## Requirements

### Data Structures

#### SectorLocation
Represents a single sector in FloppyControlApp's addressing scheme.
```csharp
public record SectorLocation(int Track, int Sector)
{
    // Convenience properties for CHS conversion
    public int Cylinder => Track / 2;
    public int Head => Track % 2;
}
```

#### SectorRange
Represents a contiguous range of sectors (for fragmentation visualization).
```csharp
public record SectorRange(SectorLocation Start, SectorLocation End, int SectorCount);
```

#### FileSectorMap
Complete sector information for a single file.
```csharp
public class FileSectorMap
{
    public string FileName { get; set; }
    public string FullPath { get; set; }
    public long FileSize { get; set; }
    
    /// <summary>
    /// All sectors this file occupies, in FAT chain order.
    /// First sector contains start of file, last contains end.
    /// </summary>
    public List<SectorLocation> Sectors { get; set; }
    
    /// <summary>
    /// Contiguous ranges (fewer = less fragmented).
    /// </summary>
    public List<SectorRange> Ranges { get; set; }
    
    /// <summary>
    /// Number of fragments (1 = contiguous, >1 = fragmented).
    /// </summary>
    public int FragmentCount => Ranges.Count;
    
    /// <summary>
    /// Cluster chain from FAT (for debugging/advanced use).
    /// </summary>
    public List<int> ClusterChain { get; set; }
}
```

#### DiskSectorMap
Complete sector map for the entire disk.
```csharp
public class DiskSectorMap
{
    /// <summary>
    /// Disk geometry information.
    /// </summary>
    public DiskGeometry Geometry { get; set; }
    
    /// <summary>
    /// Boot sector (always Track 0, Sector 0).
    /// </summary>
    public SectorLocation BootSector { get; set; }
    
    /// <summary>
    /// FAT table regions (typically 2 copies).
    /// </summary>
    public List<SectorRange> FatRegions { get; set; }
    
    /// <summary>
    /// Root directory region.
    /// </summary>
    public SectorRange RootDirectory { get; set; }
    
    /// <summary>
    /// First sector of data area (where file clusters begin).
    /// </summary>
    public SectorLocation DataAreaStart { get; set; }
    
    /// <summary>
    /// All files with their sector maps.
    /// </summary>
    public List<FileSectorMap> Files { get; set; }
    
    /// <summary>
    /// Free (unallocated) sectors in data area.
    /// </summary>
    public List<SectorLocation> FreeSectors { get; set; }
    
    /// <summary>
    /// Bad sectors marked in FAT (cluster value 0xFF7).
    /// </summary>
    public List<SectorLocation> BadSectorsInFat { get; set; }
}

public class DiskGeometry
{
    public int TotalTracks { get; set; }      // e.g., 160 for 80 cylinders * 2 heads
    public int SectorsPerTrack { get; set; }  // e.g., 9 (DD) or 18 (HD)
    public int BytesPerSector { get; set; }   // typically 512
    public int SectorsPerCluster { get; set; } // typically 1 or 2
    public int TotalSectors { get; set; }
    public int TotalClusters { get; set; }
    public string DiskType { get; set; }      // "720KB DD" or "1.44MB HD"
}
```

### API Methods

Add to `Fat12Extractor` (or new `SectorMapper` class):

```csharp
/// <summary>
/// Gets complete sector map for the entire disk.
/// Parses FAT12 structures directly (independent of DiscUtils).
/// </summary>
public DiskSectorMap GetSectorMap();

/// <summary>
/// Gets sector map for a specific file.
/// </summary>
public FileSectorMap GetFileSectorMap(FileEntry entry);

/// <summary>
/// Finds which file (if any) occupies the given sector.
/// Returns null for free/system sectors.
/// </summary>
public FileEntry? GetFileAtSector(int track, int sector);

/// <summary>
/// Checks if a sector is in a system area (boot, FAT, root dir).
/// </summary>
public SystemAreaType? GetSystemAreaAtSector(int track, int sector);
```

```csharp
public enum SystemAreaType
{
    BootSector,
    Fat,
    RootDirectory
}
```

---

## Implementation Approach

### Direct FAT12 Parsing (No DiscUtils for sector mapping)

Parse FAT12 structures directly from the raw disk image stream:

1. **Boot Sector (Sector 0)**: Read BPB (BIOS Parameter Block) for geometry
2. **FAT Tables**: Read and parse 12-bit cluster entries
3. **Root Directory**: Parse 32-byte directory entries
4. **Cluster Chains**: Follow FAT chain for each file

This gives full control and doesn't depend on DiscUtils internals.

### FAT12 Boot Sector Layout (BPB)
```
Offset  Size  Description
0x00    3     Jump instruction
0x03    8     OEM name
0x0B    2     Bytes per sector (usually 512)
0x0D    1     Sectors per cluster
0x0E    2     Reserved sectors (before first FAT)
0x10    1     Number of FATs (usually 2)
0x11    2     Root directory entries (max)
0x13    2     Total sectors (if < 65536)
0x15    1     Media descriptor
0x16    2     Sectors per FAT
0x18    2     Sectors per track
0x1A    2     Number of heads
```

### Cluster to Sector Conversion
```
data_area_start = reserved_sectors + (num_fats * sectors_per_fat) + root_dir_sectors
sector = data_area_start + (cluster - 2) * sectors_per_cluster
```

Note: Clusters are numbered starting at 2 (0 and 1 are reserved in FAT).

---

## Integration with Existing Code

### Preserve DiscUtils for File Operations
- Continue using DiscUtils for file enumeration and extraction
- Use direct parsing only for sector mapping
- This separation keeps extraction robust while giving us sector-level access

### Coordinate with FileEntry
- `FileSectorMap.FullPath` should match `FileEntry.FullPath`
- Can add optional `SectorMap` property to `FileEntry` (lazy-loaded)

---

## Example Usage

### FloppyControlApp Integration
```csharp
using var reader = new DiskImageReader("disk.img");
var extractor = new Fat12Extractor(reader);

// Get complete sector map
var diskMap = extractor.GetSectorMap();

// Populate file dropdown
foreach (var file in diskMap.Files)
{
    fileDropdown.Items.Add(file.FullPath);
}

// Highlight system areas
HighlightSector(diskMap.BootSector, SystemColor);
foreach (var range in diskMap.FatRegions)
    HighlightRange(range, SystemColor);
HighlightRange(diskMap.RootDirectory, SystemColor);

// On file selection, highlight its sectors
void OnFileSelected(string path)
{
    ClearHighlights();
    var fileMap = diskMap.Files.First(f => f.FullPath == path);
    
    foreach (var sector in fileMap.Sectors)
    {
        var color = IsBadSector(sector) ? BadFileColor : FileColor;
        HighlightSector(sector.Track, sector.Sector, color);
    }
    
    // Show fragmentation info
    statusLabel.Text = $"{fileMap.FullPath}: {fileMap.Sectors.Count} sectors, {fileMap.FragmentCount} fragment(s)";
}

// Reverse lookup: click sector to see what's there
void OnSectorClicked(int track, int sector)
{
    var file = extractor.GetFileAtSector(track, sector);
    if (file != null)
    {
        ShowInfo($"Sector belongs to: {file.FullPath}");
    }
    else
    {
        var systemArea = extractor.GetSystemAreaAtSector(track, sector);
        if (systemArea.HasValue)
            ShowInfo($"System area: {systemArea.Value}");
        else
            ShowInfo("Free sector");
    }
}
```

---

## Success Criteria

1. Can retrieve sector locations for any file on a FAT12 disk
2. Track/sector numbering matches FloppyControlApp's scheme exactly
3. System areas (boot, FAT, root dir) are correctly identified
4. Fragmentation is detected and reported via ranges
5. Reverse lookup (sector → file) works efficiently
6. Works with both 720KB (DD) and 1.44MB (HD) disk images
7. Independent of DiscUtils internals (direct FAT12 parsing)

---

## Out of Scope (Phase 2)

- Deleted file sector mapping (would require parsing deleted directory entries)
- Subdirectory sector locations (directories themselves occupy clusters)
- Writing/modifying FAT structures
- Support for non-FAT12 filesystems
- Integration with FloppyControlApp UI (that's FloppyControlApp's responsibility)

---

## Future Enhancements

- **Phase 3?**: Deleted file recovery with sector mapping
- **Phase 3?**: Subdirectory cluster/sector tracking
- **Phase 3?**: "Reconstruct file from sector variants" API
