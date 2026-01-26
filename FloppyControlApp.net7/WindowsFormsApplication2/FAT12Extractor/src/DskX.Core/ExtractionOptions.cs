namespace DskX.Core;

/// <summary>
/// Options for controlling file extraction behavior.
/// </summary>
public class ExtractionOptions
{
    /// <summary>
    /// Gets or sets whether to include deleted files in the extraction.
    /// Default is false (only extract non-deleted files).
    /// </summary>
    public bool IncludeDeleted { get; set; }

    /// <summary>
    /// Gets or sets whether to extract ONLY deleted files.
    /// Default is false (extract all files based on IncludeDeleted setting).
    /// </summary>
    public bool DeletedOnly { get; set; }

    /// <summary>
    /// Gets or sets a specific file path to extract (e.g., "DOCS\README.TXT").
    /// If null or empty, all files matching the other criteria will be extracted.
    /// </summary>
    public string? SpecificFilePath { get; set; }

    /// <summary>
    /// Creates default extraction options (non-deleted files only).
    /// </summary>
    public ExtractionOptions()
    {
        IncludeDeleted = false;
        DeletedOnly = false;
        SpecificFilePath = null;
    }

    /// <summary>
    /// Determines whether a file entry should be included based on these options.
    /// </summary>
    /// <param name="entry">The file entry to check.</param>
    /// <returns>True if the file should be included; otherwise, false.</returns>
    public bool ShouldInclude(FileEntry entry)
    {
        // If a specific file path is requested, only include that file
        if (!string.IsNullOrEmpty(SpecificFilePath))
        {
            return entry.FullPath.Equals(SpecificFilePath, StringComparison.OrdinalIgnoreCase);
        }

        // If DeletedOnly is true, only include deleted files
        if (DeletedOnly)
        {
            return entry.IsDeleted;
        }

        // If IncludeDeleted is false, exclude deleted files
        if (!IncludeDeleted && entry.IsDeleted)
        {
            return false;
        }

        // Otherwise, include the file
        return true;
    }
}
