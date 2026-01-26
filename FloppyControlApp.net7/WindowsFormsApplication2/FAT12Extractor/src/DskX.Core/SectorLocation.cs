namespace DskX.Core;

/// <summary>
/// Represents a physical sector location on a floppy disk.
/// Uses FloppyControlApp's addressing scheme: interleaved tracks (head multiplied into track number),
/// with 0-based sector numbering.
/// </summary>
/// <param name="Track">Track number (interleaved: Track 0 = Cyl 0 Head 0, Track 1 = Cyl 0 Head 1, etc.)</param>
/// <param name="Sector">Sector number within the track (0-based: 0-8 for DD, 0-17 for HD)</param>
public record SectorLocation(int Track, int Sector) : IComparable<SectorLocation>
{
    /// <summary>
    /// Gets the cylinder number (physical cylinder on disk).
    /// </summary>
    public int Cylinder => Track / 2;
    
    /// <summary>
    /// Gets the head number (0 or 1 for double-sided disks).
    /// </summary>
    public int Head => Track % 2;
    
    /// <summary>
    /// Compares sector locations for sorting (by track, then sector).
    /// </summary>
    public int CompareTo(SectorLocation? other)
    {
        if (other == null) return 1;
        
        int trackCompare = Track.CompareTo(other.Track);
        if (trackCompare != 0) return trackCompare;
        
        return Sector.CompareTo(other.Sector);
    }
    
    /// <summary>
    /// Returns a string representation in the format "T:S" (e.g., "5:12").
    /// </summary>
    public override string ToString() => $"{Track}:{Sector}";
}
