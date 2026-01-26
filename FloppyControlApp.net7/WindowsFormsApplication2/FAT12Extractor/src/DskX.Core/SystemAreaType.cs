namespace DskX.Core;

/// <summary>
/// Identifies the type of system area on a FAT12 disk.
/// </summary>
public enum SystemAreaType
{
    /// <summary>
    /// Boot sector (always at track 0, sector 0).
    /// </summary>
    BootSector,
    
    /// <summary>
    /// File Allocation Table (FAT) region.
    /// </summary>
    Fat,
    
    /// <summary>
    /// Root directory region.
    /// </summary>
    RootDirectory
}
