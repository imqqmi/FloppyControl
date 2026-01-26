namespace DskX.Core;

/// <summary>
/// Represents a contiguous range of sectors on a floppy disk.
/// Used for visualizing fragmentation and system areas.
/// </summary>
/// <param name="Start">First sector in the range (inclusive)</param>
/// <param name="End">Last sector in the range (inclusive)</param>
/// <param name="SectorCount">Total number of sectors in this range</param>
public record SectorRange(SectorLocation Start, SectorLocation End, int SectorCount)
{
    /// <summary>
    /// Checks if the given sector location is within this range.
    /// </summary>
    /// <param name="location">The sector location to check.</param>
    /// <returns>True if the sector is within this range, false otherwise.</returns>
    public bool Contains(SectorLocation location)
    {
        return location.CompareTo(Start) >= 0 && location.CompareTo(End) <= 0;
    }
    
    /// <summary>
    /// Returns a string representation in the format "Start-End (count sectors)".
    /// </summary>
    public override string ToString() => $"{Start}-{End} ({SectorCount} sector{(SectorCount != 1 ? "s" : "")})";
}
