namespace DskX.Core.Tests;

public class Fat12ExtractorTests
{
    // Use paths relative to the solution root (3 levels up from test bin directory)
    private static readonly string TestImage720KB = Path.Combine("..", "..", "..", "..", "..", "ExampleDiskImages", "A001 Modules1_disk_018.DSK");
    private static readonly string TestImage144MB = Path.Combine("..", "..", "..", "..", "..", "ExampleDiskImages", "PictureDiskTestA_disk_027.DSK");
    private static readonly string VerificationFiles720KB = Path.Combine("..", "..", "..", "..", "..", "ExampleDiskImages", "A001 Modules1_disk_018.DSK_verification_files");
    private static readonly string VerificationFiles144MB = Path.Combine("..", "..", "..", "..", "..", "ExampleDiskImages", "PictureDiskTestA_disk_027.DSK_verification_files");

    [Fact]
    public void Constructor_WithNullReader_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Fat12Extractor(null!));
    }

    [Fact]
    public void EnumerateFiles_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => extractor.EnumerateFiles(null!).ToList());
    }

    [Fact]
    public void EnumerateFiles_720KBImage_ReturnsFiles()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();

        // Act
        var files = extractor.EnumerateFiles(options).ToList();

        // Assert
        Assert.NotEmpty(files);
        
        // All entries should have names and paths
        Assert.All(files, file =>
        {
            Assert.NotNull(file.Name);
            Assert.NotEmpty(file.Name);
            Assert.NotNull(file.FullPath);
        });

        // Should not include directories in file enumeration (only files)
        Assert.All(files, file => Assert.False(file.IsDirectory));
    }

    [Fact]
    public void EnumerateFiles_144MBImage_CanEnumerate()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage144MB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();

        // Act
        var files = extractor.EnumerateFiles(options).ToList();

        // Assert
        // The image may be empty, but enumeration should not throw
        // If there are files, they should have valid metadata
        if (files.Any())
        {
            Assert.All(files, file =>
            {
                Assert.NotNull(file.Name);
                Assert.NotEmpty(file.Name);
                Assert.NotNull(file.FullPath);
            });
        }
    }

    [Fact]
    public void EnumerateFiles_DefaultOptions_ExcludesDeletedFiles()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions(); // Default: IncludeDeleted = false

        // Act
        var files = extractor.EnumerateFiles(options).ToList();

        // Assert
        // None of the files should be marked as deleted
        Assert.All(files, file => Assert.False(file.IsDeleted));
    }

    [Fact]
    public void EnumerateFiles_IncludeDeleted_IncludesAllFiles()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions { IncludeDeleted = true };

        // Act
        var files = extractor.EnumerateFiles(options).ToList();

        // Assert
        Assert.NotEmpty(files);
        
        // Note: Since DiscUtils doesn't expose deleted files, we won't actually
        // find any deleted files. This test verifies the option doesn't break anything.
    }

    [Fact]
    public void EnumerateFiles_DeletedOnly_ReturnsEmptyOrDeletedOnly()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions { DeletedOnly = true };

        // Act
        var files = extractor.EnumerateFiles(options).ToList();

        // Assert
        // Since DiscUtils doesn't expose deleted files, this should return empty
        // or only files marked as deleted (if any were found through other means)
        Assert.All(files, file => Assert.True(file.IsDeleted));
    }

    [Fact]
    public void EnumerateFiles_VerifyFileMetadata()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();

        // Act
        var files = extractor.EnumerateFiles(options).ToList();

        // Assert
        Assert.NotEmpty(files);

        // Check that at least some files have size > 0
        var filesWithSize = files.Where(f => f.Size > 0).ToList();
        Assert.NotEmpty(filesWithSize);

        // Verify all files have valid names (attributes can be 0 for normal files)
        Assert.All(files, file =>
        {
            Assert.NotNull(file.Name);
            Assert.NotEmpty(file.Name);
        });
    }

    [Fact]
    public void EnumerateFiles_HandlesSubdirectories()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();

        // Act
        var files = extractor.EnumerateFiles(options).ToList();

        // Assert
        // Check if any files have paths with directory separators
        // (indicating they're in subdirectories)
        var filesInSubdirs = files.Where(f => f.FullPath.Contains('\\')).ToList();
        
        // If there are subdirectories, verify the paths are properly formed
        if (filesInSubdirs.Any())
        {
            Assert.All(filesInSubdirs, file =>
            {
                Assert.DoesNotContain("\\\\", file.FullPath); // No double backslashes
                Assert.False(file.FullPath.StartsWith("\\")); // No leading backslash
            });
        }
    }

    [Fact]
    public void EnumerateFiles_SpecificFilePath_ReturnsOnlyThatFile()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        
        // First, get all files to find one to target
        var allFiles = extractor.EnumerateFiles(new ExtractionOptions()).ToList();
        Assert.NotEmpty(allFiles);
        
        var targetFile = allFiles.First();
        var options = new ExtractionOptions { SpecificFilePath = targetFile.FullPath };

        // Act
        var files = extractor.EnumerateFiles(options).ToList();

        // Assert
        Assert.Single(files);
        Assert.Equal(targetFile.FullPath, files[0].FullPath, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExtractFile_WithNullEntry_ThrowsArgumentNullException()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => extractor.ExtractFile(null!, "test.txt"));
    }

    [Fact]
    public void ExtractFile_WithNullDestination_ThrowsArgumentNullException()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var entry = new FileEntry { Name = "test.txt", FullPath = "test.txt", IsDirectory = false };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => extractor.ExtractFile(entry, null!));
    }

    [Fact]
    public void ExtractFile_WithDirectory_ThrowsArgumentException()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var entry = new FileEntry { Name = "testdir", FullPath = "testdir", IsDirectory = true };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => extractor.ExtractFile(entry, "testdir"));
    }

    [Fact]
    public void ExtractFile_SingleFile_ExtractsSuccessfully()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();
        var files = extractor.EnumerateFiles(options).ToList();
        Assert.NotEmpty(files);

        var testFile = files.First();
        string tempPath = Path.Combine(Path.GetTempPath(), "dskx_test_" + Guid.NewGuid().ToString(), testFile.Name);

        try
        {
            // Act
            extractor.ExtractFile(testFile, tempPath);

            // Assert
            Assert.True(File.Exists(tempPath), $"File should exist at {tempPath}");
            var fileInfo = new FileInfo(tempPath);
            Assert.Equal(testFile.Size, fileInfo.Length);
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
            string? parentDir = Path.GetDirectoryName(tempPath);
            if (parentDir != null && Directory.Exists(parentDir))
            {
                Directory.Delete(parentDir, true);
            }
        }
    }

    [Fact]
    public void ExtractFile_CreatesParentDirectories()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();
        var files = extractor.EnumerateFiles(options).ToList();
        Assert.NotEmpty(files);

        var testFile = files.First();
        string tempDir = Path.Combine(Path.GetTempPath(), "dskx_test_" + Guid.NewGuid().ToString());
        string tempPath = Path.Combine(tempDir, "subdir1", "subdir2", testFile.Name);

        try
        {
            // Act
            extractor.ExtractFile(testFile, tempPath);

            // Assert
            Assert.True(File.Exists(tempPath), "File should exist");
            Assert.True(Directory.Exists(Path.Combine(tempDir, "subdir1", "subdir2")), "Parent directories should be created");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public void ExtractAll_WithNullDestination_ThrowsArgumentNullException()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => extractor.ExtractAll(null!, options));
    }

    [Fact]
    public void ExtractAll_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => extractor.ExtractAll("test", null!));
    }

    [Fact]
    public void ExtractAll_720KBImage_ExtractsAllFiles()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();
        
        // Get expected file count
        var expectedFiles = extractor.EnumerateFiles(options).ToList();
        
        string tempDir = Path.Combine(Path.GetTempPath(), "dskx_test_" + Guid.NewGuid().ToString());

        try
        {
            // Act
            extractor.ExtractAll(tempDir, options);

            // Assert
            Assert.True(Directory.Exists(tempDir), "Destination directory should be created");
            
            // Count all files in the output directory (recursively)
            var extractedFiles = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);
            Assert.Equal(expectedFiles.Count, extractedFiles.Length);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public void ExtractAll_144MBImage_ExtractsAllFiles()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage144MB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();
        
        // Get expected file count
        var expectedFiles = extractor.EnumerateFiles(options).ToList();
        
        string tempDir = Path.Combine(Path.GetTempPath(), "dskx_test_" + Guid.NewGuid().ToString());

        try
        {
            // Act
            extractor.ExtractAll(tempDir, options);

            // Assert
            Assert.True(Directory.Exists(tempDir), "Destination directory should be created");
            
            // Count all files in the output directory (recursively)
            var extractedFiles = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);
            Assert.Equal(expectedFiles.Count, extractedFiles.Length);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public void ExtractAll_PreservesDirectoryStructure()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();
        
        string tempDir = Path.Combine(Path.GetTempPath(), "dskx_test_" + Guid.NewGuid().ToString());

        try
        {
            // Act
            extractor.ExtractAll(tempDir, options);

            // Assert
            var expectedFiles = extractor.EnumerateFiles(options).ToList();
            
            foreach (var expectedFile in expectedFiles)
            {
                string expectedPath = Path.Combine(tempDir, expectedFile.FullPath.Replace('\\', Path.DirectorySeparatorChar));
                Assert.True(File.Exists(expectedPath), $"File should exist at expected path: {expectedPath}");
            }
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public void ExtractAll_VerifiesFileSizes()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();
        
        string tempDir = Path.Combine(Path.GetTempPath(), "dskx_test_" + Guid.NewGuid().ToString());

        try
        {
            // Act
            extractor.ExtractAll(tempDir, options);

            // Assert
            var expectedFiles = extractor.EnumerateFiles(options).ToList();
            
            foreach (var expectedFile in expectedFiles)
            {
                string extractedPath = Path.Combine(tempDir, expectedFile.FullPath.Replace('\\', Path.DirectorySeparatorChar));
                var fileInfo = new FileInfo(extractedPath);
                Assert.Equal(expectedFile.Size, fileInfo.Length);
            }
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public void ExtractAll_720KBImage_MatchesVerificationFiles()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage720KB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();
        
        string tempDir = Path.Combine(Path.GetTempPath(), "dskx_test_" + Guid.NewGuid().ToString());

        try
        {
            // Act
            extractor.ExtractAll(tempDir, options);

            // Assert
            Assert.True(Directory.Exists(VerificationFiles720KB), "Verification files directory should exist");
            
            var verificationFiles = Directory.GetFiles(VerificationFiles720KB, "*", SearchOption.AllDirectories);
            Assert.NotEmpty(verificationFiles);

            foreach (var verificationFile in verificationFiles)
            {
                string fileName = Path.GetFileName(verificationFile);
                string extractedFile = Path.Combine(tempDir, fileName);
                
                Assert.True(File.Exists(extractedFile), $"Extracted file should exist: {fileName}");
                
                // Compare file sizes
                var verificationInfo = new FileInfo(verificationFile);
                var extractedInfo = new FileInfo(extractedFile);
                Assert.Equal(verificationInfo.Length, extractedInfo.Length);
                
                // Compare file contents (MD5 hash)
                string verificationHash = ComputeMD5(verificationFile);
                string extractedHash = ComputeMD5(extractedFile);
                Assert.Equal(verificationHash, extractedHash);
            }
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public void ExtractAll_144MBImage_MatchesVerificationFiles()
    {
        // Arrange
        using var reader = new DiskImageReader();
        reader.Open(TestImage144MB);
        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions();
        
        string tempDir = Path.Combine(Path.GetTempPath(), "dskx_test_" + Guid.NewGuid().ToString());

        try
        {
            // Act
            extractor.ExtractAll(tempDir, options);

            // Assert
            Assert.True(Directory.Exists(VerificationFiles144MB), "Verification files directory should exist");
            
            var verificationFiles = Directory.GetFiles(VerificationFiles144MB, "*", SearchOption.AllDirectories);
            Assert.NotEmpty(verificationFiles);

            foreach (var verificationFile in verificationFiles)
            {
                string fileName = Path.GetFileName(verificationFile);
                string extractedFile = Path.Combine(tempDir, fileName);
                
                Assert.True(File.Exists(extractedFile), $"Extracted file should exist: {fileName}");
                
                // Compare file sizes
                var verificationInfo = new FileInfo(verificationFile);
                var extractedInfo = new FileInfo(extractedFile);
                Assert.Equal(verificationInfo.Length, extractedInfo.Length);
                
                // Compare file contents (MD5 hash)
                string verificationHash = ComputeMD5(verificationFile);
                string extractedHash = ComputeMD5(extractedFile);
                Assert.Equal(verificationHash, extractedHash);
            }
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    private static string ComputeMD5(string filePath)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        using var stream = File.OpenRead(filePath);
        byte[] hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
