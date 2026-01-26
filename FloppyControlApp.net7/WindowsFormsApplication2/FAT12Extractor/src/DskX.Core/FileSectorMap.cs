namespace DskX.Core;

/// <summary>
/// Contains complete sector-level location information for a single file on a FAT12 disk.
/// </summary>
public class FileSectorMap
{
    /// <summary>
    /// Gets or sets the file name (e.g., "README.TXT").
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the full path from disk root (e.g., "DOCS\README.TXT").
    /// </summary>
    public string FullPath { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// Gets or sets all sectors this file occupies, in FAT chain order.
    /// The first sector contains the start of the file, the last contains the end.
    /// </summary>
    public List<SectorLocation> Sectors { get; set; } = new List<SectorLocation>();
    
    /// <summary>
    /// Gets or sets contiguous sector ranges.
    /// Fewer ranges indicates less fragmentation (1 range = completely contiguous).
    /// </summary>
    public List<SectorRange> Ranges { get; set; } = new List<SectorRange>();
    
    /// <summary>
    /// Gets the number of fragments (1 = contiguous file, >1 = fragmented).
    /// </summary>
    public int FragmentCount => Ranges.Count;
    
    /// <summary>
    /// Gets or sets the cluster chain from the FAT (for debugging/advanced use).
    /// Cluster numbers are in FAT order (first cluster is the file's start cluster).
    /// </summary>
    public List<int> ClusterChain { get; set; } = new List<int>();
}
