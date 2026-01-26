namespace DskX.Core;

/// <summary>
/// Contains complete sector-level mapping information for an entire FAT12 disk.
/// Includes system areas (boot, FAT, root directory), file locations, and free space.
/// </summary>
public class DiskSectorMap
{
    /// <summary>
    /// Gets or sets the disk geometry information.
    /// </summary>
    public DiskGeometry Geometry { get; set; } = new DiskGeometry();
    
    /// <summary>
    /// Gets or sets the boot sector location (always track 0, sector 0).
    /// </summary>
    public SectorLocation BootSector { get; set; } = new SectorLocation(0, 0);
    
    /// <summary>
    /// Gets or sets the FAT table regions (typically 2 copies for redundancy).
    /// </summary>
    public List<SectorRange> FatRegions { get; set; } = new List<SectorRange>();
    
    /// <summary>
    /// Gets or sets the root directory region.
    /// </summary>
    public SectorRange? RootDirectory { get; set; }
    
    /// <summary>
    /// Gets or sets the first sector of the data area (where file clusters begin).
    /// </summary>
    public SectorLocation? DataAreaStart { get; set; }
    
    /// <summary>
    /// Gets or sets all files with their sector maps.
    /// </summary>
    public List<FileSectorMap> Files { get; set; } = new List<FileSectorMap>();
    
    /// <summary>
    /// Gets or sets free (unallocated) sectors in the data area.
    /// </summary>
    public List<SectorLocation> FreeSectors { get; set; } = new List<SectorLocation>();
    
    /// <summary>
    /// Gets or sets bad sectors marked in the FAT (cluster value 0xFF7).
    /// </summary>
    public List<SectorLocation> BadSectorsInFat { get; set; } = new List<SectorLocation>();
}
