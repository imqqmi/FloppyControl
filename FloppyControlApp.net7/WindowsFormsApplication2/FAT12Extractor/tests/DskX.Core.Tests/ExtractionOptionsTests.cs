namespace DskX.Core.Tests;

public class ExtractionOptionsTests
{
    [Fact]
    public void DefaultOptions_ExcludesDeletedFiles()
    {
        // Arrange
        var options = new ExtractionOptions();
        var normalFile = new FileEntry { Name = "FILE.TXT", IsDeleted = false };
        var deletedFile = new FileEntry { Name = "DELETED.TXT", IsDeleted = true };

        // Act & Assert
        Assert.True(options.ShouldInclude(normalFile));
        Assert.False(options.ShouldInclude(deletedFile));
    }

    [Fact]
    public void IncludeDeleted_IncludesBothNormalAndDeletedFiles()
    {
        // Arrange
        var options = new ExtractionOptions { IncludeDeleted = true };
        var normalFile = new FileEntry { Name = "FILE.TXT", IsDeleted = false };
        var deletedFile = new FileEntry { Name = "DELETED.TXT", IsDeleted = true };

        // Act & Assert
        Assert.True(options.ShouldInclude(normalFile));
        Assert.True(options.ShouldInclude(deletedFile));
    }

    [Fact]
    public void DeletedOnly_IncludesOnlyDeletedFiles()
    {
        // Arrange
        var options = new ExtractionOptions { DeletedOnly = true };
        var normalFile = new FileEntry { Name = "FILE.TXT", IsDeleted = false };
        var deletedFile = new FileEntry { Name = "DELETED.TXT", IsDeleted = true };

        // Act & Assert
        Assert.False(options.ShouldInclude(normalFile));
        Assert.True(options.ShouldInclude(deletedFile));
    }

    [Fact]
    public void SpecificFilePath_IncludesOnlyMatchingFile()
    {
        // Arrange
        var options = new ExtractionOptions { SpecificFilePath = @"DOCS\README.TXT" };
        var matchingFile = new FileEntry { FullPath = @"DOCS\README.TXT", IsDeleted = false };
        var otherFile = new FileEntry { FullPath = @"DATA\FILE.DAT", IsDeleted = false };

        // Act & Assert
        Assert.True(options.ShouldInclude(matchingFile));
        Assert.False(options.ShouldInclude(otherFile));
    }

    [Fact]
    public void SpecificFilePath_IsCaseInsensitive()
    {
        // Arrange
        var options = new ExtractionOptions { SpecificFilePath = @"docs\readme.txt" };
        var matchingFile = new FileEntry { FullPath = @"DOCS\README.TXT", IsDeleted = false };

        // Act & Assert
        Assert.True(options.ShouldInclude(matchingFile));
    }

    [Fact]
    public void SpecificFilePath_OverridesDeletedOnlyFlag()
    {
        // Arrange
        var options = new ExtractionOptions
        {
            SpecificFilePath = @"DOCS\README.TXT",
            DeletedOnly = true
        };
        var matchingNormalFile = new FileEntry { FullPath = @"DOCS\README.TXT", IsDeleted = false };

        // Act & Assert
        Assert.True(options.ShouldInclude(matchingNormalFile));
    }
}
