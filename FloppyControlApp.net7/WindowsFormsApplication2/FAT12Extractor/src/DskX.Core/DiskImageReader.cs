using DiscUtils;
using DiscUtils.Fat;
using DiscUtils.Streams;

namespace DskX.Core;

/// <summary>
/// Wraps DiscUtils to provide access to FAT12 disk images.
/// Implements IDisposable to ensure proper cleanup of resources.
/// </summary>
public class DiskImageReader : IDisposable
{
    private Stream? _imageStream;
    private FatFileSystem? _fileSystem;
    private bool _disposed;
    private bool _ownsStream;

    /// <summary>
    /// Gets the underlying FAT file system.
    /// </summary>
    public FatFileSystem FileSystem
    {
        get
        {
            if (_fileSystem == null)
            {
                throw new InvalidOperationException("Disk image has not been opened. Call Open() first.");
            }
            return _fileSystem;
        }
    }
    
    /// <summary>
    /// Gets the underlying disk image stream.
    /// Used by SectorMapper for direct FAT12 parsing.
    /// </summary>
    public Stream ImageStream
    {
        get
        {
            if (_imageStream == null)
            {
                throw new InvalidOperationException("Disk image has not been opened. Call Open() first.");
            }
            return _imageStream;
        }
    }

    /// <summary>
    /// Opens a disk image file and initializes the FAT file system.
    /// Supports both 720KB and 1.44MB FAT12 disk images.
    /// </summary>
    /// <param name="imagePath">Path to the disk image file (.DSK, .IMG, etc.).</param>
    /// <exception cref="FileNotFoundException">Thrown if the image file doesn't exist.</exception>
    /// <exception cref="InvalidDataException">Thrown if the image is not a valid FAT12 disk.</exception>
    public void Open(string imagePath)
    {
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException($"Disk image not found: {imagePath}");
        }

        try
        {
            // Open the disk image file
            _imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _ownsStream = true; // We created the stream, so we own it

            // Create a disk from the stream
            var disk = new DiscUtils.Raw.Disk(_imageStream, Ownership.None);

            // Get the first partition (or the whole disk if no partitions)
            // For floppy images, typically the entire disk is the file system
            _fileSystem = new FatFileSystem(disk.Content);
        }
        catch (Exception ex) when (ex is not FileNotFoundException)
        {
            // Clean up on error
            if (_ownsStream)
            {
                _imageStream?.Dispose();
            }
            _imageStream = null;
            _fileSystem = null;
            _ownsStream = false;

            throw new InvalidDataException($"Failed to open disk image as FAT12: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Opens a disk image from a stream and initializes the FAT file system.
    /// Supports both 720KB and 1.44MB FAT12 disk images.
    /// The stream is not disposed when DiskImageReader is disposed (caller retains ownership).
    /// </summary>
    /// <param name="imageStream">Stream containing the disk image data.</param>
    /// <exception cref="ArgumentNullException">Thrown if imageStream is null.</exception>
    /// <exception cref="InvalidDataException">Thrown if the image is not a valid FAT12 disk.</exception>
    public void Open(Stream imageStream)
    {
        if (imageStream == null)
        {
            throw new ArgumentNullException(nameof(imageStream));
        }

        try
        {
            _imageStream = imageStream;
            _ownsStream = false; // External stream, caller owns it

            // Create a disk from the stream
            var disk = new DiscUtils.Raw.Disk(_imageStream, Ownership.None);

            // Get the first partition (or the whole disk if no partitions)
            // For floppy images, typically the entire disk is the file system
            _fileSystem = new FatFileSystem(disk.Content);
        }
        catch (Exception ex) when (ex is not ArgumentNullException)
        {
            // Clean up on error (but don't dispose the external stream)
            _imageStream = null;
            _fileSystem = null;
            _ownsStream = false;

            throw new InvalidDataException($"Failed to open disk image as FAT12: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Releases all resources used by the DiskImageReader.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and optionally managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _fileSystem?.Dispose();
                
                // Only dispose the stream if we own it
                if (_ownsStream)
                {
                    _imageStream?.Dispose();
                }
            }

            _fileSystem = null;
            _imageStream = null;
            _ownsStream = false;
            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizer to ensure resources are released if Dispose is not called.
    /// </summary>
    ~DiskImageReader()
    {
        Dispose(false);
    }
}
