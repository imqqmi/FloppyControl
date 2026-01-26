# Phase 1: FAT12 File Integration with FloppyControlApp

**Created**: 2026-01-26  
**Status**: Planning  
**Scope**: Integrate FAT12Extractor (DskX.Core) with FloppyControlApp to enable file listing, sector visualization, and recovery export

---

## Overview

This phase integrates the existing FAT12Extractor library with FloppyControlApp to provide:
1. File listing from recovered PC DOS disk images
2. Visual sector mapping (highlight file sectors in the sectormap)
3. Export files with bad sector variations for manual recovery inspection

## Goals

- Allow users to see which files exist on a recovered PC DOS disk
- Visually identify which sectors belong to each file
- Quickly identify if a file contains bad sectors
- Export multiple variations of files with different bad sector data for recovery comparison

## User Workflow

1. User captures/loads disk data and processes it (existing functionality)
2. User clicks **LoadFilesButton** on Processing tab
3. System validates filesystem and populates **FilesComboBox1** with file list
4. User selects a file from the combobox
5. System highlights the file's sectors in the sectormap (green background)
6. User can see if any highlighted sectors are marked as bad (X)
7. User navigates to Error Correction tab
8. User selects bad sector candidates in **BadSectorListBox**
9. User clicks **ExportFilesButton**
10. System exports the selected file multiple times, each with a different bad sector variation

## Technical Approach

### 1. Disk Image Source
- Create `MemoryStream` from `processing.disk[]` byte array
- Pass to DskX.Core's `DiskImageReader` (needs modification to accept Stream)
- Validate filesystem using DiscUtils (attempt to read FAT, catch exceptions on failure)

### 2. File Enumeration
- New class `DiskImageFiles` in `MyClasses/Processing/` folder
- Method `GetDiskImageFiles()` returns list of files with sector metadata
- Uses `Fat12Extractor.EnumerateFiles()` and `GetFileSectorMap()`

### 3. Sector Highlighting
- Extend `SectorMap` class with highlighting capability
- Map file's `SectorLocation` list to character positions in RichTextBox
- Apply green background color (#80B080) to file's sectors
- Clear previous highlighting when new file selected

### 4. Export with Bad Sector Variations
- For each selected bad sector in BadSectorListBox:
  - Temporarily substitute that sector's data into `processing.disk[]`
  - Extract the selected file using FAT12Extractor
  - Save to: `{PathToRecoveredDisks}\{outputfilename}\recovery\{filename}_{counter:D3}.{ext}`
  - Restore original sector data
- Counter increments for each variation (001, 002, 003...)

## Constraints

- **PC DOS only**: Feature disabled/grayed out for AmigaDOS, DiskSpare formats
- **FAT12 only**: Using DiscUtils.Fat which supports FAT12/FAT16
- **Disk sizes**: 720KB (DD) and 1.44MB (HD) PC DOS formats

## UI Changes

### Processing Tab (groupBox14)
- **FilesComboBox1** - Already exists, will be populated with filenames
- **LoadFilesButton** - Already exists, will trigger file loading
- Both controls disabled/grayed when disk format is not PC DD/HD

### Error Correction Tab
- **ExportFilesButton** - Already exists, will trigger export with variations
- Disabled when no file selected in FilesComboBox1

## Dependencies

- **DskX.Core** project (already in solution, needs project reference)
- **DiscUtils.Fat** NuGet package (already in DskX.Core)

## Success Criteria

1. LoadFilesButton populates combobox with files from valid FAT12 disk
2. Selecting file highlights its sectors in sectormap with green background
3. Bad sectors within highlighted area are visually distinguishable
4. ExportFilesButton creates multiple file variations in recovery folder
5. Controls properly disabled for non-PC disk formats

## Out of Scope (Future Phases)

- Deleted file recovery
- Combining multiple different bad sectors in all permutations
- Amiga filesystem support
- Subdirectory navigation in combobox
- File preview/inspection

---

## Files to Create/Modify

### New Files
- `MyClasses/Processing/DiskImageFiles.cs` - File enumeration and export logic

### Modified Files
- `FloppyControlApp.csproj` - Add DskX.Core project reference
- `FloppyControlApp.cs` - Event handlers for LoadFilesButton, FilesComboBox1, ExportFilesButton
- `MyClasses/Processing/SectorMap.cs` - Add highlighting methods
- `DskX.Core/DiskImageReader.cs` - Add constructor accepting Stream

### Designer Files (if needed)
- `FloppyControlApp.Designer.cs` - Wire up event handlers (may already exist)
