namespace DskX.Core.Tests;

public class FileEntryTests
{
    [Fact]
    public void GetSafeFileName_NonDeletedFile_ReturnsOriginalName()
    {
        // Arrange
        var entry = new FileEntry
        {
            Name = "README.TXT",
            IsDeleted = false
        };

        // Act
        var safeName = entry.GetSafeFileName();

        // Assert
        Assert.Equal("README.TXT", safeName);
    }

    [Fact]
    public void GetSafeFileName_DeletedFile_ReplacesFirstCharWithUnderscore()
    {
        // Arrange
        var entry = new FileEntry
        {
            Name = "RILE.TXT", // Simulating deleted file (first char would be 0xE5)
            IsDeleted = true
        };

        // Act
        var safeName = entry.GetSafeFileName();

        // Assert
        Assert.Equal("_ILE.TXT", safeName);
    }

    [Fact]
    public void GetSafeFileName_WithCollision_AddsCounter()
    {
        // Arrange
        var entry = new FileEntry
        {
            Name = "RILE.TXT",
            IsDeleted = true
        };
        var existingFiles = new HashSet<string> { "_ILE.TXT" };

        // Act
        var safeName = entry.GetSafeFileName(existingFiles);

        // Assert
        Assert.Equal("_ILE_001.TXT", safeName);
    }

    [Fact]
    public void GetSafeFileName_WithMultipleCollisions_IncrementsCounter()
    {
        // Arrange
        var entry = new FileEntry
        {
            Name = "RILE.TXT",
            IsDeleted = true
        };
        var existingFiles = new HashSet<string> { "_ILE.TXT", "_ILE_001.TXT", "_ILE_002.TXT" };

        // Act
        var safeName = entry.GetSafeFileName(existingFiles);

        // Assert
        Assert.Equal("_ILE_003.TXT", safeName);
    }

    [Fact]
    public void GetSafeFileName_NoExtension_HandlesCorrectly()
    {
        // Arrange
        var entry = new FileEntry
        {
            Name = "README",
            IsDeleted = false
        };
        var existingFiles = new HashSet<string> { "README" };

        // Act
        var safeName = entry.GetSafeFileName(existingFiles);

        // Assert
        Assert.Equal("README_001", safeName);
    }

    [Fact]
    public void GetSafeFileName_NullExistingFiles_ReturnsBaseName()
    {
        // Arrange
        var entry = new FileEntry
        {
            Name = "RILE.TXT",
            IsDeleted = true
        };

        // Act
        var safeName = entry.GetSafeFileName(null);

        // Assert
        Assert.Equal("_ILE.TXT", safeName);
    }
}
