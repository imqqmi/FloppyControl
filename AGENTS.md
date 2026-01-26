# AGENTS.md - FloppyControl Project Guide for AI Agents

## Project Overview

FloppyControl is a Windows application for capturing and recovering data from floppy disks (Amiga, PC DOS, MSX formats). Built with C# (.NET 7), it includes:
- **FloppyControlApp**: Main WinForms application for disk capture and processing
- **FAT12Extractor (DskX)**: CLI tool and library for extracting files from FAT12 disk images

## Repository Structure

```
FloppyControlApp.net7/WindowsFormsApplication2/
├── WindowsFormsApplication2/          # Main FloppyControlApp (WinForms)
├── FloppyControl.Test.net7/           # MSTest tests for FloppyControlApp
└── FAT12Extractor/
    ├── src/DskX.Core/                 # Core library for FAT12 extraction
    ├── src/DskX.Cli/                  # CLI tool (dskx)
    └── tests/DskX.Core.Tests/         # xUnit tests for DskX
```

## Build Commands

### FloppyControlApp (Main Application)
```bash
# Build from solution root
dotnet build FloppyControlApp.net7/WindowsFormsApplication2/FloppyControlAppSolution.sln

# Build specific project (x64 only)
dotnet build FloppyControlApp.net7/WindowsFormsApplication2/WindowsFormsApplication2/FloppyControlApp.csproj -c Debug
```

### FAT12Extractor (DskX)
```bash
dotnet build FloppyControlApp.net7/WindowsFormsApplication2/FAT12Extractor/FAT12Extractor.sln
```

## Test Commands

### Run All Tests
```bash
# FloppyControlApp tests (MSTest)
dotnet test FloppyControlApp.net7/WindowsFormsApplication2/FloppyControl.Test.net7/FloppyControl.Test.net7.csproj

# DskX tests (xUnit)
dotnet test FloppyControlApp.net7/WindowsFormsApplication2/FAT12Extractor/tests/DskX.Core.Tests/DskX.Core.Tests.csproj
```

### Run Single Test
```bash
# MSTest - run single test by fully qualified name
dotnet test --filter "FullyQualifiedName=FloppyControl.Test.net7.ProcessingTests.TestProcessPCData"

# xUnit - run single test by name
dotnet test --filter "Fat12ExtractorTests.EnumerateFiles_720KBImage_ReturnsFiles"

# xUnit - run tests in a specific class
dotnet test --filter "ClassName=DskX.Core.Tests.Fat12ExtractorTests"

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"
```

## Code Style Guidelines

### File Organization
- Namespaces match folder structure: `FloppyControlApp.MyClasses.Processing`
- One public class per file (except small related types)
- File-scoped namespaces in newer code: `namespace DskX.Core;`

### Naming Conventions
- **Classes/Interfaces**: PascalCase (`FDDProcessing`, `DiskImageReader`)
- **Methods**: PascalCase (`StartProcessing`, `EnumerateFiles`)
- **Properties**: PascalCase (`ProcSettings`, `SectorMap`)
- **Private fields**: camelCase with underscore prefix (`_reader`, `_disposed`)
- **Public fields**: camelCase (legacy code) or PascalCase (newer code)
- **Local variables**: camelCase (`sectorcount`, `tempDir`)
- **Constants**: PascalCase or UPPER_SNAKE_CASE

### Type Annotations
- Nullable reference types enabled in newer projects: `<Nullable>enable</Nullable>`
- Use nullable annotations: `string?`, `FileEntry?`
- Null checks with `ArgumentNullException`: `?? throw new ArgumentNullException(nameof(param))`

### Imports
- System namespaces first, then third-party, then project namespaces
- Implicit usings enabled in .NET 7 projects

### Error Handling
- Use specific exception types (`ArgumentNullException`, `InvalidOperationException`)
- XML documentation for public APIs with `<exception>` tags
- Graceful degradation with warnings to `Console.Error.WriteLine`
- Implement `IDisposable` for classes managing unmanaged resources

### Code Patterns
```csharp
// Null parameter validation
public void Method(object param)
{
    if (param == null)
        throw new ArgumentNullException(nameof(param));
}

// Disposable pattern
public class MyClass : IDisposable
{
    private bool _disposed;
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing) { /* dispose managed */ }
            _disposed = true;
        }
    }
}
```

### XML Documentation
- Required for all public members in library code (DskX.Core)
- Include `<summary>`, `<param>`, `<returns>`, `<exception>` tags
```csharp
/// <summary>
/// Extracts a single file from the disk image.
/// </summary>
/// <param name="entry">The file entry to extract.</param>
/// <param name="destinationPath">Destination path for the file.</param>
/// <exception cref="ArgumentNullException">Thrown if entry is null.</exception>
public void ExtractFile(FileEntry entry, string destinationPath)
```

### Test Patterns

#### xUnit (DskX.Core.Tests)
```csharp
public class MyClassTests
{
    [Fact]
    public void MethodName_Scenario_ExpectedResult()
    {
        // Arrange, Act, Assert
        var sut = new MyClass();
        var result = sut.Method();
        Assert.NotNull(result);
    }
}
```

#### MSTest (FloppyControl.Test.net7)
```csharp
[TestClass]
public class ProcessingTests
{
    [TestMethod]
    public void TestProcessPCData() { /* Arrange, Act, Assert */ }
}
```

### EditorConfig Rules (Applied)
- IDE0044 (readonly modifier): disabled
- CA1416 (platform compatibility): silent

## Project-Specific Notes

### Target Framework
- All projects target .NET 7.0
- FloppyControlApp targets `net7.0-windows` (WinForms)
- DskX.Core targets `net7.0` (cross-platform library)

### Platform
- FloppyControlApp is x64 only (`<PlatformTarget>x64</PlatformTarget>`)
- DskX supports Any CPU

### Key Dependencies
- **DiscUtils.Fat** (0.16.13): FAT filesystem parsing
- **System.CommandLine** (2.0.0-beta4): CLI argument parsing
- **xUnit** (2.4.2): Unit testing for DskX
- **MSTest** (2.2.10): Unit testing for FloppyControlApp

### Test Data Locations
- DskX tests use: `ExampleDiskImages/` (relative paths from test bin)
- FloppyControlApp tests use: `TestData/` in test project

## Common Tasks

### Adding a New Feature to DskX
1. Add classes to `src/DskX.Core/`
2. Add tests to `tests/DskX.Core.Tests/`
3. Update CLI in `src/DskX.Cli/Program.cs` if needed

### Working with Disk Images
```csharp
using var reader = new DiskImageReader();
reader.Open("path/to/image.dsk");
var extractor = new Fat12Extractor(reader);
var files = extractor.EnumerateFiles(new ExtractionOptions());
```

### Running the CLI Tool
```bash
dotnet run --project FloppyControlApp.net7/WindowsFormsApplication2/FAT12Extractor/src/DskX.Cli -- list image.dsk
dotnet run --project FloppyControlApp.net7/WindowsFormsApplication2/FAT12Extractor/src/DskX.Cli -- extract image.dsk -o output/
```

### New build items in Phases and Items
- New items will be stored in the ./Phases folder 
- Filename format phasexa_spec.md and phasexa_exec_plan.md. the 'a' in the filename denotes subphases (ie a, b, c etc) if the phase file is becoming too large, ie larger than 1000 lines. 
- Create items with enough detail so that Sonnet can implement them within 50k-100k context. 