using DiscUtils.Fat;

namespace DskX.Core;

/// <summary>
/// Main extraction logic for FAT12 disk images.
/// Handles file enumeration and extraction with support for deleted files.
/// 
/// NOTE ON DELETED FILE DETECTION:
/// DiscUtils.Fat (v0.16.13) does NOT expose deleted directory entries through its public API.
/// The FatFileSystem.GetFileSystemEntries() method only returns active (non-deleted) files.
/// 
/// To support deleted file recovery, we would need to:
/// 1. Read raw directory sectors directly from the disk
/// 2. Parse FAT12 directory entries manually
/// 3. Detect entries with 0xE5 as the first byte (deleted marker)
/// 4. Reconstruct file data by following the FAT chain
/// 
/// This is deferred as a future enhancement. For Phase 1, only active files are enumerated.
/// The infrastructure (FileEntry.IsDeleted, ExtractionOptions filtering) is in place for
/// when this capability is added.
/// </summary>
public class Fat12Extractor
{
    private readonly DiskImageReader _reader;
    private SectorMapper? _sectorMapper;

    /// <summary>
    /// Initializes a new instance of the Fat12Extractor class.
    /// </summary>
    /// <param name="reader">The disk image reader to use for accessing the file system.</param>
    /// <exception cref="ArgumentNullException">Thrown if reader is null.</exception>
    public Fat12Extractor(DiskImageReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    /// <summary>
    /// Enumerates all files in the disk image based on the provided options.
    /// </summary>
    /// <param name="options">Options controlling which files to include.</param>
    /// <returns>An enumerable collection of file entries matching the criteria.</returns>
    public IEnumerable<FileEntry> EnumerateFiles(ExtractionOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        // Start recursive traversal from the root directory
        return EnumerateDirectory("\\", options);
    }

    /// <summary>
    /// Recursively enumerates files and directories starting from the specified path.
    /// </summary>
    /// <param name="directoryPath">The directory path to enumerate (e.g., "\" or "\DOCS").</param>
    /// <param name="options">Options controlling which files to include.</param>
    /// <returns>An enumerable collection of file entries in this directory and subdirectories.</returns>
    private IEnumerable<FileEntry> EnumerateDirectory(string directoryPath, ExtractionOptions options)
    {
        FatFileSystem fs = _reader.FileSystem;

        // Get all entries in the current directory
        string[] entries;
        try
        {
            entries = fs.GetFileSystemEntries(directoryPath);
        }
        catch (Exception ex)
        {
            // If we can't read the directory, log and skip it
            // In a production system, we might want to use a logging framework
            Console.Error.WriteLine($"Warning: Failed to read directory '{directoryPath}': {ex.Message}");
            yield break;
        }

        foreach (string entryPath in entries)
        {
            FileEntry? entry = null;
            bool isDirectory = false;

            // Try to get entry information
            try
            {
                isDirectory = (fs.GetAttributes(entryPath) & FileAttributes.Directory) != 0;
                entry = CreateFileEntry(entryPath, isDirectory);
            }
            catch (Exception ex)
            {
                // If we can't read this specific entry, log and continue
                Console.Error.WriteLine($"Warning: Failed to read entry '{entryPath}': {ex.Message}");
                continue;
            }

            // Process the entry (outside of try-catch to allow yield return)
            if (isDirectory)
            {
                // Recursively enumerate subdirectory
                foreach (var subEntry in EnumerateDirectory(entryPath, options))
                {
                    yield return subEntry;
                }
            }
            else
            {
                // Apply filtering based on options
                if (options.ShouldInclude(entry))
                {
                    yield return entry;
                }
            }
        }
    }

    /// <summary>
    /// Creates a FileEntry object from a file system path.
    /// </summary>
    /// <param name="path">The full path to the file or directory.</param>
    /// <param name="isDirectory">Whether this entry is a directory.</param>
    /// <returns>A FileEntry object with metadata populated.</returns>
    private FileEntry CreateFileEntry(string path, bool isDirectory)
    {
        FatFileSystem fs = _reader.FileSystem;

        string normalizedPath = path.TrimStart('\\');  // Remove leading backslash for consistency
        
        var entry = new FileEntry
        {
            FullPath = normalizedPath,
            Name = normalizedPath.Contains('\\') ? normalizedPath.Substring(normalizedPath.LastIndexOf('\\') + 1) : normalizedPath,
            IsDirectory = isDirectory,
            IsDeleted = false,  // Will be updated if we detect deleted files
            Attributes = fs.GetAttributes(path)
        };

        // Get file size (0 for directories)
        if (!isDirectory)
        {
            try
            {
                entry.Size = fs.GetFileLength(path);
            }
            catch
            {
                entry.Size = 0;
            }
        }

        // Get timestamps
        try
        {
            entry.Created = fs.GetCreationTimeUtc(path);
        }
        catch
        {
            entry.Created = null;
        }

        try
        {
            entry.Modified = fs.GetLastWriteTimeUtc(path);
        }
        catch
        {
            entry.Modified = null;
        }

        return entry;
    }

    /// <summary>
    /// Extracts a single file from the disk image to the specified destination path.
    /// Creates parent directories as needed.
    /// </summary>
    /// <param name="entry">The file entry to extract.</param>
    /// <param name="destinationPath">The full path where the file should be written.</param>
    /// <exception cref="ArgumentNullException">Thrown if entry or destinationPath is null.</exception>
    /// <exception cref="ArgumentException">Thrown if entry is a directory.</exception>
    public void ExtractFile(FileEntry entry, string destinationPath)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        if (string.IsNullOrWhiteSpace(destinationPath))
        {
            throw new ArgumentNullException(nameof(destinationPath));
        }

        if (entry.IsDirectory)
        {
            throw new ArgumentException("Cannot extract a directory as a file. Use ExtractAll to extract directory contents.", nameof(entry));
        }

        // Ensure parent directory exists
        string? parentDir = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
        {
            Directory.CreateDirectory(parentDir);
        }

        // Extract the file with error resilience
        FatFileSystem fs = _reader.FileSystem;
        string sourcePath = "\\" + entry.FullPath;

        try
        {
            using (Stream sourceStream = fs.OpenFile(sourcePath, FileMode.Open, FileAccess.Read))
            using (FileStream destStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
            {
                // Try to copy the entire file
                try
                {
                    sourceStream.CopyTo(destStream);
                }
                catch (Exception ex)
                {
                    // If we encounter a read error mid-file, pad the rest with zeros
                    Console.Error.WriteLine($"Warning: Error reading '{entry.FullPath}': {ex.Message}");
                    Console.Error.WriteLine($"Padding remaining {entry.Size - destStream.Length} bytes with zeros.");

                    // Pad remaining bytes with 0x00
                    long remainingBytes = entry.Size - destStream.Length;
                    if (remainingBytes > 0)
                    {
                        byte[] padding = new byte[Math.Min(remainingBytes, 8192)]; // 8KB buffer
                        while (remainingBytes > 0)
                        {
                            int bytesToWrite = (int)Math.Min(remainingBytes, padding.Length);
                            destStream.Write(padding, 0, bytesToWrite);
                            remainingBytes -= bytesToWrite;
                        }
                    }
                }
            }

            // Preserve timestamps if available
            if (entry.Modified.HasValue)
            {
                try
                {
                    File.SetLastWriteTimeUtc(destinationPath, entry.Modified.Value);
                }
                catch
                {
                    // Ignore timestamp errors
                }
            }

            if (entry.Created.HasValue)
            {
                try
                {
                    File.SetCreationTimeUtc(destinationPath, entry.Created.Value);
                }
                catch
                {
                    // Ignore timestamp errors
                }
            }
        }
        catch (Exception ex)
        {
            // If we can't even open the file, create an empty placeholder
            Console.Error.WriteLine($"Error: Failed to extract '{entry.FullPath}': {ex.Message}");
            Console.Error.WriteLine($"Creating empty placeholder file.");

            try
            {
                // Create empty file or file filled with zeros
                using (FileStream destStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                {
                    if (entry.Size > 0)
                    {
                        byte[] padding = new byte[Math.Min(entry.Size, 8192)];
                        long remainingBytes = entry.Size;
                        while (remainingBytes > 0)
                        {
                            int bytesToWrite = (int)Math.Min(remainingBytes, padding.Length);
                            destStream.Write(padding, 0, bytesToWrite);
                            remainingBytes -= bytesToWrite;
                        }
                    }
                }
            }
            catch
            {
                // If we can't even create the placeholder, give up on this file
                Console.Error.WriteLine($"Error: Could not create placeholder for '{entry.FullPath}'");
            }
        }
    }

    /// <summary>
    /// Extracts all files matching the specified options to the destination directory.
    /// Preserves directory structure and handles collisions for deleted files.
    /// </summary>
    /// <param name="destinationPath">The root directory where files should be extracted.</param>
    /// <param name="options">Options controlling which files to extract.</param>
    /// <exception cref="ArgumentNullException">Thrown if destinationPath or options is null.</exception>
    public void ExtractAll(string destinationPath, ExtractionOptions options)
    {
        if (string.IsNullOrWhiteSpace(destinationPath))
        {
            throw new ArgumentNullException(nameof(destinationPath));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        // Create destination directory if it doesn't exist
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }

        // Track existing files for collision detection
        var existingFiles = new Dictionary<string, HashSet<string>>();

        // Enumerate and extract all matching files
        foreach (var entry in EnumerateFiles(options))
        {
            // Build the destination path, preserving directory structure
            string relativePath = entry.FullPath.Replace('\\', Path.DirectorySeparatorChar);
            string fullDestPath = Path.Combine(destinationPath, relativePath);

            // Get the directory for this file
            string? fileDir = Path.GetDirectoryName(fullDestPath);
            if (string.IsNullOrEmpty(fileDir))
            {
                fileDir = destinationPath;
            }

            // Track existing files in this directory for collision detection
            if (!existingFiles.ContainsKey(fileDir))
            {
                existingFiles[fileDir] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            // Get safe filename (handles deleted file collisions)
            string fileName = Path.GetFileName(fullDestPath);
            string safeFileName = entry.GetSafeFileName(existingFiles[fileDir]);
            existingFiles[fileDir].Add(safeFileName);

            // Build final destination path
            string finalDestPath = Path.Combine(fileDir, safeFileName);

            // Extract the file
            ExtractFile(entry, finalDestPath);
        }
    }
    
    // ===== PHASE 2: SECTOR MAPPING METHODS =====
    
    /// <summary>
    /// Gets the complete sector map for the disk.
    /// This operation is cached - subsequent calls return the same map.
    /// </summary>
    /// <returns>Complete disk sector map with all files and system areas.</returns>
    public DiskSectorMap GetSectorMap()
    {
        EnsureSectorMapper();
        return _sectorMapper!.BuildDiskSectorMap();
    }
    
    /// <summary>
    /// Gets the sector map for a specific file.
    /// </summary>
    /// <param name="entry">File entry to map.</param>
    /// <returns>Sector map for the file, or null if not found.</returns>
    public FileSectorMap? GetFileSectorMap(FileEntry entry)
    {
        EnsureSectorMapper();
        return _sectorMapper!.GetFileSectorMap(entry);
    }
    
    /// <summary>
    /// Finds which file (if any) occupies the given sector.
    /// </summary>
    /// <param name="track">Track number.</param>
    /// <param name="sector">Sector number within track.</param>
    /// <returns>File entry occupying this sector, or null if free/system sector.</returns>
    public FileEntry? GetFileAtSector(int track, int sector)
    {
        EnsureSectorMapper();
        return _sectorMapper!.GetFileAtSector(track, sector);
    }
    
    /// <summary>
    /// Checks if a sector is in a system area (boot, FAT, root directory).
    /// </summary>
    /// <param name="track">Track number.</param>
    /// <param name="sector">Sector number within track.</param>
    /// <returns>System area type, or null if not a system sector.</returns>
    public SystemAreaType? GetSystemAreaAtSector(int track, int sector)
    {
        EnsureSectorMapper();
        return _sectorMapper!.GetSystemAreaAtSector(track, sector);
    }
    
    /// <summary>
    /// Ensures the sector mapper is initialized.
    /// </summary>
    private void EnsureSectorMapper()
    {
        if (_sectorMapper == null)
        {
            _sectorMapper = new SectorMapper(_reader.ImageStream);
        }
    }
}
