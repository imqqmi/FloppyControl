namespace DskX.Core;

/// <summary>
/// Describes the physical and logical geometry of a FAT12 floppy disk.
/// </summary>
public class DiskGeometry
{
    /// <summary>
    /// Gets or sets the total number of tracks (interleaved: cylinders * heads).
    /// Example: 160 tracks = 80 cylinders * 2 heads.
    /// </summary>
    public int TotalTracks { get; set; }
    
    /// <summary>
    /// Gets or sets the number of sectors per track.
    /// Example: 9 for DD (720KB), 18 for HD (1.44MB).
    /// </summary>
    public int SectorsPerTrack { get; set; }
    
    /// <summary>
    /// Gets or sets the number of bytes per sector (typically 512).
    /// </summary>
    public int BytesPerSector { get; set; }
    
    /// <summary>
    /// Gets or sets the number of sectors per cluster (FAT allocation unit).
    /// Example: 1 or 2 for floppy disks.
    /// </summary>
    public int SectorsPerCluster { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of sectors on the disk.
    /// </summary>
    public int TotalSectors { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of data clusters available.
    /// </summary>
    public int TotalClusters { get; set; }
    
    /// <summary>
    /// Gets or sets a human-readable description of the disk type.
    /// Example: "720KB DD" or "1.44MB HD".
    /// </summary>
    public string DiskType { get; set; } = string.Empty;
    
    /// <summary>
    /// Creates a DiskGeometry for a standard 720KB double-density 3.5" floppy.
    /// </summary>
    public static DiskGeometry Create720KbDD()
    {
        return new DiskGeometry
        {
            TotalTracks = 160,        // 80 cylinders * 2 heads
            SectorsPerTrack = 9,
            BytesPerSector = 512,
            SectorsPerCluster = 2,
            TotalSectors = 1440,      // 160 * 9
            TotalClusters = 713,      // Approximate (depends on FAT/root dir size)
            DiskType = "720KB DD"
        };
    }
    
    /// <summary>
    /// Creates a DiskGeometry for a standard 1.44MB high-density 3.5" floppy.
    /// </summary>
    public static DiskGeometry Create144MbHD()
    {
        return new DiskGeometry
        {
            TotalTracks = 160,        // 80 cylinders * 2 heads
            SectorsPerTrack = 18,
            BytesPerSector = 512,
            SectorsPerCluster = 1,
            TotalSectors = 2880,      // 160 * 18
            TotalClusters = 2847,     // Approximate (depends on FAT/root dir size)
            DiskType = "1.44MB HD"
        };
    }
}
