namespace DskX.Core.Tests;

public class DiskImageReaderTests
{
    // Use paths relative to the solution root (3 levels up from test bin directory)
    private static readonly string TestImage720KB = Path.Combine("..", "..", "..", "..", "..", "ExampleDiskImages", "A001 Modules1_disk_018.DSK");
    private static readonly string TestImage144MB = Path.Combine("..", "..", "..", "..", "..", "ExampleDiskImages", "PictureDiskTestA_disk_027.DSK");

    [Fact]
    public void Open_720KBImage_Succeeds()
    {
        // Arrange
        var reader = new DiskImageReader();

        // Act & Assert
        try
        {
            reader.Open(TestImage720KB);
            Assert.NotNull(reader.FileSystem);
        }
        finally
        {
            reader.Dispose();
        }
    }

    [Fact]
    public void Open_144MBImage_Succeeds()
    {
        // Arrange
        var reader = new DiskImageReader();

        // Act & Assert
        try
        {
            reader.Open(TestImage144MB);
            Assert.NotNull(reader.FileSystem);
        }
        finally
        {
            reader.Dispose();
        }
    }

    [Fact]
    public void Open_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var reader = new DiskImageReader();

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => reader.Open("nonexistent.dsk"));
    }

    [Fact]
    public void FileSystem_BeforeOpen_ThrowsInvalidOperationException()
    {
        // Arrange
        var reader = new DiskImageReader();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _ = reader.FileSystem);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var reader = new DiskImageReader();
        reader.Open(TestImage720KB);

        // Act & Assert - should not throw
        reader.Dispose();
        reader.Dispose();
    }

    [Fact]
    public void UsingPattern_DisposesCorrectly()
    {
        // Arrange & Act
        using (var reader = new DiskImageReader())
        {
            reader.Open(TestImage720KB);
            Assert.NotNull(reader.FileSystem);
        }

        // Assert - if we get here without exception, disposal worked correctly
        Assert.True(true);
    }
}
