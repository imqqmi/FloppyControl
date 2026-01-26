namespace DskX.Core;

/// <summary>
/// Represents a file or directory entry from a disk image.
/// </summary>
public class FileEntry
{
    /// <summary>
    /// Gets or sets the file or directory name (e.g., "README.TXT").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full path from the disk root (e.g., "DOCS\README.TXT").
    /// </summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes (0 for directories).
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Gets or sets whether this entry is a directory.
    /// </summary>
    public bool IsDirectory { get; set; }

    /// <summary>
    /// Gets or sets whether this entry is marked as deleted (first byte = 0xE5).
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the file attributes (read-only, hidden, system, etc.).
    /// </summary>
    public FileAttributes Attributes { get; set; }

    /// <summary>
    /// Gets or sets the creation date/time (may be null if not available).
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    /// Gets or sets the last modified date/time (may be null if not available).
    /// </summary>
    public DateTime? Modified { get; set; }

    /// <summary>
    /// Generates a safe output filename for this entry.
    /// For deleted files, replaces the missing first character with underscore.
    /// Handles collision by appending _001, _002, etc. if needed.
    /// </summary>
    /// <param name="existingFiles">Set of filenames that already exist in the output directory.</param>
    /// <returns>A safe filename that won't collide with existing files.</returns>
    public string GetSafeFileName(HashSet<string>? existingFiles = null)
    {
        string baseName = Name;

        // If deleted, replace first character with underscore
        if (IsDeleted && !string.IsNullOrEmpty(baseName))
        {
            baseName = "_" + baseName.Substring(1);
        }

        // If no collision tracking, return the base name
        if (existingFiles == null)
        {
            return baseName;
        }

        // Check for collisions and add counter if needed
        string safeName = baseName;
        if (existingFiles.Contains(safeName))
        {
            // Extract extension
            string nameWithoutExt = Path.GetFileNameWithoutExtension(baseName);
            string extension = Path.GetExtension(baseName);

            int counter = 1;
            do
            {
                safeName = $"{nameWithoutExt}_{counter:D3}{extension}";
                counter++;
            }
            while (existingFiles.Contains(safeName));
        }

        return safeName;
    }
}
