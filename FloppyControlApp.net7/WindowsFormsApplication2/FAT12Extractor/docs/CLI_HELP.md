# DskX CLI Help

**Version**: 1.0.0 (Phase 1)  
**Framework**: .NET 7.0  
**Last Updated**: 2026-01-11

---

## Overview

DskX is a command-line tool for extracting files from FAT12 floppy disk images (720KB and 1.44MB formats). It's designed for data recovery workflows where disk images may contain corrupted sectors.

---

## Installation & Setup

### Prerequisites

- .NET 7.0 Runtime or SDK

### WSL2 Ubuntu Setup

If you're running DskX in WSL2 Ubuntu and encounter the error "You must install .NET to run this application", you need to add the dotnet directory to your PATH.

#### Quick Fix (Temporary - Current Session Only)

```bash
export PATH="$HOME/.dotnet:$PATH"
```

#### Permanent Fix (Recommended)

Add the following line to your `~/.bashrc` or `~/.profile`:

```bash
echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.bashrc
source ~/.bashrc
```

Or for `~/.profile`:

```bash
echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.profile
source ~/.profile
```

#### Verify Installation

```bash
dotnet --version
# Should output: 7.0.120 (or similar)
```

---

## Building DskX

### From Source

```bash
# Navigate to project root
cd /home/imqqmi/floppycontrol/FAT12Extractor

# Build the solution
dotnet build

# Or build in Release mode
dotnet build -c Release
```

### Output Location

- **Debug build**: `src/DskX.Cli/bin/Debug/net7.0/dskx`
- **Release build**: `src/DskX.Cli/bin/Release/net7.0/dskx`

### Running Without Installation

```bash
# From project root
dotnet run --project src/DskX.Cli -- <command> <args>

# Example: List files
dotnet run --project src/DskX.Cli -- l "ExampleDiskImages/A001 Modules1_disk_018.DSK"
```

### Creating a System-Wide Alias (Optional)

Add to your `~/.bashrc`:

```bash
alias dskx='dotnet /home/imqqmi/floppycontrol/FAT12Extractor/src/DskX.Cli/bin/Debug/net7.0/dskx.dll'
```

Or create a wrapper script in `~/.local/bin/dskx`:

```bash
#!/bin/bash
exec dotnet /home/imqqmi/floppycontrol/FAT12Extractor/src/DskX.Cli/bin/Debug/net7.0/dskx.dll "$@"
```

Make it executable:

```bash
chmod +x ~/.local/bin/dskx
```

---

## Usage

### Command Syntax

```
dskx <command> [options] <arguments>
```

### Commands

#### List Files (`l`)

Display contents of a disk image.

```bash
# List all files
dskx l <image>

# List all files including deleted
dskx l <image> --deleted

# List only deleted files
dskx l <image> --deleted-only
```

**Examples:**

```bash
# List normal files
dskx l "ExampleDiskImages/A001 Modules1_disk_018.DSK"

# List all files including deleted
dskx l disk.dsk --deleted

# List only deleted files
dskx l disk.dsk --deleted-only
```

#### Extract Files (`x`)

Extract files from a disk image to a destination directory.

```bash
# Extract all files
dskx x <image> <destination>

# Extract all files including deleted
dskx x <image> <destination> --deleted

# Extract only deleted files
dskx x <image> <destination> --deleted-only

# Extract a specific file
dskx x <image> <destination> --file "PATH\TO\FILE.TXT"
```

**Examples:**

```bash
# Extract all files to current directory
dskx x image.dsk .

# Extract to specific folder
dskx x image.dsk /home/user/recovered

# Extract all files including deleted
dskx x image.dsk ./output --deleted

# Extract only deleted files
dskx x image.dsk ./recovered --deleted-only

# Extract a specific file
dskx x image.dsk ./output --file "DOCS\README.TXT"
```

---

## Options Reference

| Option | Short | Description |
|--------|-------|-------------|
| `--deleted` | `-d` | Include deleted files in operation |
| `--deleted-only` | | Only process deleted files |
| `--file <path>` | `-f` | Extract specific file by path |
| `--help` | `-h` | Show help information |
| `--version` | | Show version information |

---

## Path Handling

### Spaces in Paths

Always use double quotes for paths containing spaces:

```bash
dskx l "My Disk Image.dsk"
dskx x "Disk 001.dsk" "C:\My Files\Recovery"
```

### Relative vs Absolute Paths

Both relative and absolute paths are supported:

```bash
# Relative paths
dskx x ../images/disk.dsk ./output

# Absolute paths
dskx x /home/user/images/disk.dsk /home/user/output
```

### File Paths in Disk Images

When using `--file`, use the path as it appears in the disk image (typically DOS-style with backslashes):

```bash
dskx x image.dsk ./output --file "SUBDIR\FILE.TXT"
```

---

## Behavior Details

### Extraction Behavior

- **Directory Structure**: Preserved from source disk image
- **Recursive**: All subdirectories extracted automatically
- **Overwrite**: Existing files are overwritten without prompting
- **Output Mode**: Quiet (minimal output)

### Error Handling (Recovery-Oriented)

DskX is designed for data recovery and follows a "best effort" approach:

- **Missing Clusters**: Padded with 0x00 bytes
- **Corrupted Entries**: Skipped; processing continues with next file
- **Truncated Images**: Available data is processed

### Deleted File Recovery

FAT12 marks deleted files by replacing the first character with `0xE5`. DskX recovers these files with the following naming convention:

- **First character replacement**: Underscore `_` replaces the missing character
- **Collision handling**: If `_YFILE.TXT` exists, counter is added: `_YFILE_001.TXT`, `_YFILE_002.TXT`, etc.

**Example:**

```
Original file: MYFILE.TXT (deleted)
Recovered as:  _YFILE.TXT

If _YFILE.TXT exists:
Recovered as:  _YFILE_001.TXT
```

---

## Supported Formats

### Currently Supported (Phase 1)

- IBM PC compatible 3.5" floppy disk images
- 720KB (DD) FAT12
- 1.44MB (HD) FAT12

### Limitations

- **Long Filenames**: Not supported (8.3 FAT12 names only)
- **Other Formats**: AmigaDOS and other formats not yet supported

---

## Examples

### Basic Recovery Workflow

```bash
# 1. List contents to see what's available
dskx l "disk_001.dsk"

# 2. Extract all normal files
dskx x "disk_001.dsk" ./output

# 3. Check for deleted files
dskx l "disk_001.dsk" --deleted-only

# 4. Extract deleted files to separate folder
dskx x "disk_001.dsk" ./deleted --deleted-only
```

### Multi-Image Recovery

When you have multiple captures of the same disk with different sector failures:

```bash
# Extract from each variant
dskx x "capture_001.dsk" ./variant1 --deleted
dskx x "capture_002.dsk" ./variant2 --deleted
dskx x "capture_003.dsk" ./variant3 --deleted

# Compare outputs to find best version of each file
```

### Specific File Extraction

```bash
# Extract just one important file
dskx x "disk.dsk" ./output --file "PHOTOS\FAMILY.JPG"

# Extract from subdirectory
dskx x "disk.dsk" ./output --file "DOCS\REPORTS\ANNUAL.DOC"
```

---

## Troubleshooting

### "You must install .NET to run this application"

**Cause**: The dotnet runtime is not in your PATH.

**Solution**: Add `~/.dotnet` to your PATH (see [WSL2 Ubuntu Setup](#wsl2-ubuntu-setup) above).

### "Could not find file or directory"

**Cause**: Path contains spaces and is not quoted.

**Solution**: Use double quotes around paths:

```bash
# Wrong
dskx l My Disk.dsk

# Correct
dskx l "My Disk.dsk"
```

### "Invalid disk image format"

**Cause**: The file is not a valid FAT12 disk image, or is corrupted.

**Solution**: 
- Verify the file is a FAT12 floppy disk image (720KB or 1.44MB)
- Check file integrity
- Try a different disk image variant if available

### Extracted Files Are Corrupted

**Cause**: Source disk image has corrupted sectors.

**Solution**:
- DskX pads missing clusters with 0x00 bytes
- If you have multiple captures, extract from each and compare
- For images (JPEGs, etc.), visual inspection may reveal which variant is best

---

## Testing

### Run Unit Tests

```bash
# From project root
dotnet test

# With detailed output
dotnet test -v normal
```

### Test with Example Images

```bash
# List contents of example images
dskx l "ExampleDiskImages/AnthonyHull001_disk_012.DSK"
dskx l "ExampleDiskImages/A001 Modules1_disk_018.DSK"

# Extract to test output
mkdir -p test_output
dskx x "ExampleDiskImages/A001 Modules1_disk_018.DSK" test_output
```

---

## Development

### Project Structure

```
FAT12Extractor/
├── src/
│   ├── DskX.Cli/          # CLI application
│   │   ├── Program.cs
│   │   └── Commands/
│   └── DskX.Core/         # Core extraction library
│       ├── DiskImageReader.cs
│       ├── Fat12Extractor.cs
│       ├── FileEntry.cs
│       └── ExtractionOptions.cs
├── tests/
│   └── DskX.Core.Tests/   # Unit tests
├── ExampleDiskImages/      # Sample disk images
└── docs/                   # Documentation
```

### Using DskX.Core Library

The core extraction logic is available as a separate library (`DskX.Core`) that can be used independently:

```csharp
using DskX.Core;

// Open disk image
using var reader = new DiskImageReader("image.dsk");

// Create extractor
var extractor = new Fat12Extractor(reader);

// Extract files
var options = new ExtractionOptions
{
    IncludeDeleted = true,
    OutputPath = "./output"
};

extractor.ExtractAll(options);
```

---

## Getting Help

### Command-Line Help

```bash
# General help
dskx --help

# Command-specific help
dskx l --help
dskx x --help
```

### Documentation

- **Phase 1 Specification**: `docs/PHASE1_SPEC.md`
- **Execution Plan**: `docs/PHASE1_EXEC_PLAN.md`
- **This Document**: `docs/CLI_HELP.md`

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2026-01-11 | Initial Phase 1 release |

---

## License

See `LICENSE.txt` in the project root.
