using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DskX.Core;
using FloppyControlApp.MyClasses;

namespace FloppyControlApp.MyClasses.Processing
{
    /// <summary>
    /// Provides FAT12 file listing and sector mapping for recovered PC DOS disk images.
    /// Wraps DskX.Core functionality to integrate FAT12 extraction with FloppyControlApp.
    /// </summary>
    public class DiskImageFiles : IDisposable
    {
        private FDDProcessing _processing;
        private MemoryStream _diskStream;
        private DiskImageReader _reader;
        private Fat12Extractor _extractor;
        private SectorMapper _sectorMapper;
        private bool _disposed;

        /// <summary>
        /// Gets whether the disk image has a valid FAT12 filesystem.
        /// </summary>
        public bool IsValidFilesystem { get; private set; }

        /// <summary>
        /// Gets the validation error message if filesystem is invalid.
        /// </summary>
        public string ValidationError { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the list of files found on the disk.
        /// Only populated after successful initialization.
        /// </summary>
        public List<DiskFileInfo> Files { get; private set; } = new List<DiskFileInfo>();

        /// <summary>
        /// Initializes the FAT12 file reader from a FDDProcessing object.
        /// Creates a MemoryStream from processing.disk[] and attempts to read the filesystem.
        /// </summary>
        /// <param name="processing">The FDDProcessing object containing the disk image.</param>
        /// <returns>True if initialization succeeded, false if filesystem is invalid or corrupted.</returns>
        public bool Initialize(FDDProcessing processing)
        {
            if (processing == null)
            {
                throw new ArgumentNullException(nameof(processing));
            }

            _processing = processing;

            // Check disk format is PC DOS (DD or HD)
            if (processing.diskformat != DiskFormat.pcdd && processing.diskformat != DiskFormat.pchd)
            {
                IsValidFilesystem = false;
                ValidationError = $"Disk format is {processing.diskformat}, only PC DD and HD formats are supported.";
                return false;
            }

            try
            {
                // Determine disk size based on format
                int diskSize = processing.diskformat == DiskFormat.pcdd 
                    ? 720 * 1024    // 720KB
                    : 1440 * 1024;  // 1.44MB

                // Create MemoryStream from disk byte array
                _diskStream = new MemoryStream(processing.disk, 0, diskSize, writable: false);

                // Open disk image with DiskImageReader
                _reader = new DiskImageReader();
                _reader.Open(_diskStream);

                // Create Fat12Extractor
                _extractor = new Fat12Extractor(_reader);

                // Create SectorMapper for sector-level information
                _sectorMapper = new SectorMapper(_diskStream);

                IsValidFilesystem = true;
                ValidationError = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                // Handle filesystem errors (corrupted FAT, invalid boot sector, etc.)
                IsValidFilesystem = false;
                ValidationError = $"Failed to read FAT12 filesystem: {ex.Message}";

                // Clean up on error
                CleanupResources();

                return false;
            }
        }

        /// <summary>
        /// Enumerates all files on the disk and returns them with sector information.
        /// Must be called after successful Initialize().
        /// </summary>
        /// <returns>List of files with sector locations and bad sector counts.</returns>
        /// <exception cref="InvalidOperationException">Thrown if Initialize() was not called or failed.</exception>
        public List<DiskFileInfo> GetDiskImageFiles()
        {
            if (!IsValidFilesystem || _extractor == null || _sectorMapper == null || _processing == null)
            {
                throw new InvalidOperationException("DiskImageFiles has not been initialized or initialization failed.");
            }

            Files.Clear();

            try
            {
                // Enumerate files using Fat12Extractor
                var options = new ExtractionOptions
                {
                    IncludeDeleted = false,      // Phase 1: Active files only
                    DeletedOnly = false
                };

                var fileEntries = _extractor.EnumerateFiles(options).Where(e => !e.IsDirectory);

                // Build file info list with sector information
                foreach (var entry in fileEntries)
                {
                    var fileSectorMap = _sectorMapper.GetFileSectorMap(entry);
                    
                    if (fileSectorMap == null)
                    {
                        // File has no sector map (possibly 0-byte file or error)
                        continue;
                    }

                    // Calculate bad sector count
                    int badSectorCount = CountBadSectors(fileSectorMap.Sectors);

                    var fileInfo = new DiskFileInfo
                    {
                        FileName = entry.Name,
                        FullPath = entry.FullPath,
                        FileSize = entry.Size,
                        Sectors = fileSectorMap.Sectors,
                        BadSectorCount = badSectorCount
                    };

                    Files.Add(fileInfo);
                }

                return Files;
            }
            catch (Exception ex)
            {
                // If enumeration fails, return empty list and log error
                ValidationError = $"Error enumerating files: {ex.Message}";
                return new List<DiskFileInfo>();
            }
        }

        /// <summary>
        /// Gets a file by its filename.
        /// </summary>
        /// <param name="filename">The filename to search for (case-insensitive).</param>
        /// <returns>The file info, or null if not found.</returns>
        public DiskFileInfo GetFileByName(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }

            return Files.FirstOrDefault(f => 
                string.Equals(f.FileName, filename, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the sector locations for a specific file.
        /// </summary>
        /// <param name="file">The file to get sectors for.</param>
        /// <returns>List of sector locations.</returns>
        public List<SectorLocation> GetFileSectors(DiskFileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return file.Sectors;
        }

        /// <summary>
        /// Extracts a file from the disk image to the specified destination.
        /// This method re-creates the disk image stream from the current processing.disk[] array
        /// to ensure any modifications are included in the extraction.
        /// </summary>
        /// <param name="file">The file to extract.</param>
        /// <param name="destinationPath">The full path where the file should be written.</param>
        /// <exception cref="ArgumentNullException">Thrown if file or destinationPath is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if DiskImageFiles is not initialized.</exception>
        public void ExtractFile(DiskFileInfo file, string destinationPath)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                throw new ArgumentNullException(nameof(destinationPath));
            }

            if (!IsValidFilesystem || _extractor == null || _processing == null)
            {
                throw new InvalidOperationException("DiskImageFiles has not been initialized or initialization failed.");
            }

            // Re-create the disk stream from current processing.disk[] to include any modifications
            int diskSize = _processing.diskformat == DiskFormat.pcdd 
                ? 720 * 1024    // 720KB
                : 1440 * 1024;  // 1.44MB

            using (var tempStream = new MemoryStream(_processing.disk, 0, diskSize, writable: false))
            using (var tempReader = new DiskImageReader())
            {
                tempReader.Open(tempStream);
                var tempExtractor = new Fat12Extractor(tempReader);

                // Enumerate files to find the matching FileEntry
                var options = new ExtractionOptions
                {
                    IncludeDeleted = false,
                    DeletedOnly = false
                };

                var fileEntry = tempExtractor.EnumerateFiles(options)
                    .FirstOrDefault(e => string.Equals(e.FullPath, file.FullPath, StringComparison.OrdinalIgnoreCase));

                if (fileEntry == null)
                {
                    throw new InvalidOperationException($"File '{file.FullPath}' not found in disk image.");
                }

                // Extract the file
                tempExtractor.ExtractFile(fileEntry, destinationPath);
            }
        }

        /// <summary>
        /// Counts the number of bad sectors within a file's sector list.
        /// Uses processing.SectorMap.sectorok[] to check sector status.
        /// </summary>
        /// <param name="sectors">List of sector locations to check.</param>
        /// <returns>Number of bad sectors found.</returns>
        private int CountBadSectors(List<SectorLocation> sectors)
        {
            if (_processing == null || sectors == null)
            {
                return 0;
            }

            int badCount = 0;

            foreach (var sector in sectors)
            {
                // Check if sector is marked as bad in the sector map
                var status = _processing.SectorMap.sectorok[sector.Track, sector.Sector];
                
                // SectorMapStatus: Consider various "bad" statuses
                // BadSectorUnique = 3: Sector is bad and unique (unrecoverable)
                // HeadOkDataBad = 2: Header OK but data is bad
                // CrcBad = 13: CRC check failed
                if (status == SectorMapStatus.BadSectorUnique || 
                    status == SectorMapStatus.HeadOkDataBad || 
                    status == SectorMapStatus.CrcBad ||
                    status == SectorMapStatus.AmigaHeadOkDataBad)
                {
                    badCount++;
                }
            }

            return badCount;
        }

        /// <summary>
        /// Cleans up resources (streams, readers, etc.).
        /// </summary>
        private void CleanupResources()
        {
            _extractor = null;
            _sectorMapper = null;

            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }

            if (_diskStream != null)
            {
                _diskStream.Dispose();
                _diskStream = null;
            }
        }

        /// <summary>
        /// Disposes all resources used by DiskImageFiles.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                CleanupResources();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Represents a file on the disk with its sector information.
    /// </summary>
    public class DiskFileInfo
    {
        /// <summary>
        /// Gets or sets the filename (e.g., "README.TXT").
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
        /// Gets or sets the sector locations this file occupies.
        /// </summary>
        public List<SectorLocation> Sectors { get; set; } = new List<SectorLocation>();

        /// <summary>
        /// Gets or sets the number of bad sectors within this file's area.
        /// Calculated from SectorMap.sectorok[] status.
        /// </summary>
        public int BadSectorCount { get; set; }

        /// <summary>
        /// Returns a string representation of the file info.
        /// </summary>
        public override string ToString()
        {
            string badInfo = BadSectorCount > 0 ? $" ({BadSectorCount} bad)" : "";
            return $"{FileName} - {FileSize} bytes, {Sectors.Count} sectors{badInfo}";
        }
    }
}
