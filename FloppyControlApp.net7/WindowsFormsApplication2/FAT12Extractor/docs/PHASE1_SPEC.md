# Phase 1 Specification: DskX - FAT12 Disk Image Extractor

**Created**: 2026-01-11  
**Status**: Draft  
**Goal**: Build a .NET 7.0 CLI tool to extract files from FAT12 floppy disk images

---

## Overview

DskX is a command-line tool for extracting files from FAT12 floppy disk images (720KB and 1.44MB formats). It's designed for data recovery workflows where disk images may contain corrupted sectors, and the goal is to extract as much usable data as possible.

### Use Case Context

This tool integrates with a floppy disk recovery application that generates multiple disk images from raw flux transition data. Different captures may have different sector failures. The goal is to extract files from multiple image variants to find the best recoverable version of corrupted files (e.g., JPEGs with visual artifacts).

---

## Requirements

### Binary & Platform
- **Binary name**: `dskx.exe`
- **Framework**: .NET 7.0
- **Type**: Console application

### Supported Formats
- IBM PC compatible 3.5" floppy disk images
- 720KB (DD) FAT12
- 1.44MB (HD) FAT12
- (Future: AmigaDOS and other formats)

### Architecture
- **Separation of concerns**: CLI layer and extraction logic in separate projects
- **Reusable library**: `DskX.Core` can be used independently of CLI
- **External dependency**: DiscUtils library for FAT12 parsing

---

## CLI Commands

### List Files
```
dskx.exe l <image>                    # List all files
dskx.exe l <image> --deleted          # List all including deleted files  
dskx.exe l <image> --deleted-only     # List only deleted files
```

### Extract Files
```
dskx.exe x <image> <dest>                    # Extract all files
dskx.exe x <image> <dest> --deleted          # Extract all including deleted
dskx.exe x <image> <dest> --deleted-only     # Extract only deleted files
dskx.exe x <image> <dest> --file "PATH"      # Extract specific file by path
```

### Path Handling
- All paths support double quotes for spaces: `dskx.exe x "My Disk.dsk" "C:\My Files\"`
- Paths can be relative or absolute

---

## Behavior Specifications

### Extraction Behavior
- **Directory structure**: Preserved from source disk image
- **Recursive**: All subdirectories extracted automatically
- **Overwrite**: Existing files are overwritten without prompting
- **Output mode**: Quiet (minimal output) - verbosity options deferred to later phase

### Error Handling (Recovery-Oriented)
- **Philosophy**: Extract what we can, don't crash on corruption
- **Missing clusters**: Pad with 0x00 bytes
- **Corrupted entries**: Skip and continue with next file
- **Truncated images**: Process available data

### Deleted File Recovery
- FAT12 marks deleted files by replacing first character with 0xE5
- **Recovery naming**: Replace missing first character with underscore `_`
- **Collision handling**: If `_YFILE.TXT` exists, use counter: `_YFILE_001.TXT`, `_YFILE_002.TXT`, etc.

### Long Filenames
- Not supported in Phase 1 (standard 8.3 FAT12 names only)
- May be added in future phase if trivial with DiscUtils

---

## Project Structure

```
FAT12Extractor/
├── src/
│   ├── DskX.Cli/
│   │   ├── DskX.Cli.csproj
│   │   ├── Program.cs
│   │   └── Commands/
│   │       ├── ListCommand.cs
│   │       └── ExtractCommand.cs
│   └── DskX.Core/
│       ├── DskX.Core.csproj
│       ├── DiskImageReader.cs       # Wraps DiscUtils, loads disk image
│       ├── Fat12Extractor.cs        # Main extraction logic
│       ├── FileEntry.cs             # File/directory metadata
│       └── ExtractionOptions.cs     # Options (deleted files, etc.)
├── tests/
│   └── DskX.Core.Tests/
│       └── DskX.Core.Tests.csproj
├── ExampleDiskImages/
│   ├── AnthonyHull001_disk_012.DSK
│   └── A001 Modules1_disk_018.DSK
├── FAT12Extractor.sln
└── docs/
    ├── PHASE1_SPEC.md
    └── PHASE1_EXEC_PLAN.md
```

---

## Example Usage

```bash
# List contents of a disk image
dskx.exe l "A001 Modules1_disk_018.DSK"

# Extract all files to current directory
dskx.exe x image.dsk .

# Extract all files including deleted to a specific folder
dskx.exe x image.dsk "C:\Recovery\Output" --deleted

# Extract only deleted files
dskx.exe x image.dsk ./recovered --deleted-only

# Extract a specific file
dskx.exe x image.dsk ./output --file "DOCS\README.TXT"
```

---

## Success Criteria

1. Successfully extracts files from both 720KB and 1.44MB example images
2. Handles paths with spaces correctly
3. Deleted file recovery works with proper naming
4. Gracefully handles missing/corrupted clusters (pads with 0x00)
5. Library (`DskX.Core`) usable independently of CLI

---

## Out of Scope (Phase 1)

- Verbose/report output modes
- Long filename (VFAT) support
- AmigaDOS or other non-FAT12 formats
- Writing/modifying disk images
- Interactive mode
