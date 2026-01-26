# Phase 1 Execution Plan: FAT12 File Integration

**Created**: 2026-01-26  
**Status**: In Progress (Items 1-3 Complete)  
**Estimated Items**: 7

---

## Item 1: Add DskX.Core Project Reference

**Status**: ✅ Complete  
**Priority**: High  
**Complexity**: Low  
**Commit**: 6d621f5

### Description
Add DskX.Core as a project reference to FloppyControlApp so it can use Fat12Extractor classes.

### Files
- `FloppyControlApp.net7/WindowsFormsApplication2/WindowsFormsApplication2/FloppyControlApp.csproj`

### Implementation Details
1. Add `<ProjectReference>` to DskX.Core project
2. Verify build succeeds with reference

### Sub-tasks
- [x] Add ProjectReference to csproj
- [x] Build and verify no errors
- [x] Fixed pre-existing Designer build errors (settings1 -> settings2)

### Acceptance Criteria
- ✅ FloppyControlApp builds successfully with DskX.Core reference
- ✅ Can reference `DskX.Core` namespace in FloppyControlApp code

### Notes
- Also included FAT12Extractor directory in repository (removed embedded .git)
- Fixed incomplete settings1->settings2 rename in FloppyControlApp.Designer.cs

---

## Item 2: Extend DiskImageReader to Accept Stream

**Status**: ✅ Complete  
**Priority**: High  
**Complexity**: Low  
**Commit**: 6d621f5

### Description
Modify DiskImageReader to accept a Stream (MemoryStream) in addition to file path, enabling FloppyControlApp to pass its in-memory disk image.

### Files
- `FAT12Extractor/src/DskX.Core/DiskImageReader.cs`

### Implementation Details
1. Add new constructor `DiskImageReader(Stream stream)`
2. Add `Open(Stream stream)` method overload
3. Modify internal logic to work with provided stream (no file ownership)
4. Ensure `Dispose()` doesn't close externally-provided streams

### Integration Points
- **Upstream**: FloppyControlApp creates MemoryStream from `processing.disk[]`
- **Downstream**: Fat12Extractor uses the stream for file enumeration/extraction

### Sub-tasks
- [x] Add Stream-based constructor (not needed - used Open method)
- [x] Add Open(Stream) method overload
- [x] Handle stream ownership (don't dispose external streams)
- [ ] Add unit test for Stream-based opening (deferred)

### Acceptance Criteria
- ✅ Can create DiskImageReader from MemoryStream via Open(Stream)
- ✅ Existing file-based functionality unchanged
- ✅ Stream not disposed when DiskImageReader is disposed (external ownership)

### Notes
- Changed private field from FileStream to Stream for flexibility
- Added _ownsStream flag to track ownership
- Modified Dispose() to only dispose owned streams

---

## Item 3: Create DiskImageFiles Class

**Status**: ✅ Complete  
**Priority**: High  
**Complexity**: Medium  
**Commit**: 76bf195

### Description
Create new class to handle FAT12 file enumeration and provide file/sector information for FloppyControlApp integration.

### Files
- `MyClasses/Processing/DiskImageFiles.cs` (new file)

### Implementation Details

```csharp
namespace FloppyControlApp.MyClasses.Processing
{
    /// <summary>
    /// Provides FAT12 file listing and sector mapping for recovered PC DOS disk images.
    /// </summary>
    public class DiskImageFiles
    {
        private FDDProcessing _processing;
        private DiskImageReader _reader;
        private Fat12Extractor _extractor;
        
        // Properties
        public bool IsValidFilesystem { get; private set; }
        public string ValidationError { get; private set; }
        public List<DiskFileInfo> Files { get; private set; }
        
        // Methods
        public bool Initialize(FDDProcessing processing);
        public List<DiskFileInfo> GetDiskImageFiles();
        public DiskFileInfo GetFileByName(string filename);
        public List<SectorLocation> GetFileSectors(DiskFileInfo file);
        public void Dispose();
    }
    
    /// <summary>
    /// Represents a file on the disk with its sector information.
    /// </summary>
    public class DiskFileInfo
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public long FileSize { get; set; }
        public List<SectorLocation> Sectors { get; set; }
        public int BadSectorCount { get; set; }  // Calculated from sectormap
    }
}
```

### Integration Points
- **Triggers**: Called when LoadFilesButton clicked
- **Outputs**: List of files for combobox, sector info for highlighting
- **Downstream**: SectorMap uses sector list for highlighting

### Sub-tasks
- [x] Create DiskImageFiles.cs file
- [x] Implement Initialize() - create MemoryStream from processing.disk[]
- [x] Implement GetDiskImageFiles() - enumerate files with sector info
- [x] Implement GetFileSectors() - return sector locations for a file
- [x] Add validation (check disk format is PC DD/HD)
- [x] Handle filesystem errors gracefully (invalid/corrupted FAT)
- [x] Calculate BadSectorCount from SectorMap.sectorok[]

### Acceptance Criteria
- ✅ Returns empty list with error for non-PC formats
- ✅ Returns file list with sector info for valid FAT12 disks
- ✅ Handles corrupted filesystems without crashing
- ✅ BadSectorCount accurately reflects bad sectors in file's area

### Notes
- Created DiskFileInfo class to hold file metadata and sector locations
- Filters out directories (files only)
- Checks SectorMapStatus for BadSectorUnique, HeadOkDataBad, CrcBad, AmigaHeadOkDataBad
- Implements IDisposable for proper resource cleanup

---

## Item 4: Add Sector Highlighting to SectorMap

**Status**: Pending  
**Priority**: High  
**Complexity**: Medium

### Description
Extend SectorMap class to highlight specific sectors in the RichTextBox with a green background color.

### Files
- `MyClasses/Processing/SectorMap.cs`

### Implementation Details

1. Add method to calculate character position from (track, sector)
2. Add method to highlight sectors with background color
3. Add method to clear highlighting
4. Use RichTextBox selection to apply BackColor

```csharp
// New methods in SectorMap class:

/// <summary>
/// Highlights the specified sectors with a background color.
/// </summary>
public void HighlightSectors(List<SectorLocation> sectors, Color backgroundColor);

/// <summary>
/// Clears all sector highlighting, restoring default colors.
/// </summary>
public void ClearHighlighting();

/// <summary>
/// Gets the character index in the RichTextBox for a given track/sector.
/// </summary>
private int GetSectorCharIndex(int track, int sector);
```

### Character Position Calculation
The sectormap format is: `T000 ......... T001 ......... ...`
- Each track starts with "T" + 3-digit number + space = 5 chars
- Then sectors (1 char each)
- Then space
- Pattern: `Tnnn .........` repeated

For track T and sector S:
- Track block width = 5 + sectorspertrack + 1 (space)
- Character index = (track * block_width) + 5 + sector

### Integration Points
- **Triggers**: FilesComboBox1.SelectedIndexChanged
- **Inputs**: List of SectorLocation from DiskImageFiles
- **Outputs**: Visual highlighting in rtbSectorMap/QrtbSectorMap

### Sub-tasks
- [ ] Add GetSectorCharIndex() method
- [ ] Add HighlightSectors() method with BeginUpdate/EndUpdate for performance
- [ ] Add ClearHighlighting() method
- [ ] Handle both rtbSectorMap and QrtbSectorMap (via property)
- [ ] Use color #80B080 (light green) for highlighting

### Acceptance Criteria
- Selecting file highlights its sectors in green
- Selecting different file clears previous and highlights new
- Bad sectors (X) remain visible with green background
- Performance acceptable (no UI freeze)

---

## Item 5: Implement LoadFilesButton Event Handler

**Status**: Pending  
**Priority**: High  
**Complexity**: Medium

### Description
Implement the click handler for LoadFilesButton to validate filesystem and populate FilesComboBox1.

### Files
- `FloppyControlApp.cs` - Add event handler method
- `FloppyControlApp.Designer.cs` - Wire up event (if not already)

### Implementation Details

```csharp
private DiskImageFiles _diskImageFiles;

private void LoadFilesButton_Click(object sender, EventArgs e)
{
    // 1. Check disk format
    if (!IsPCDiskFormat())
    {
        MessageBox.Show("File listing is only available for PC DOS disk formats.", 
                        "Unsupported Format", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
    }
    
    // 2. Initialize DiskImageFiles
    _diskImageFiles = new DiskImageFiles();
    if (!_diskImageFiles.Initialize(processing))
    {
        MessageBox.Show($"Could not read filesystem: {_diskImageFiles.ValidationError}", 
                        "Filesystem Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }
    
    // 3. Get files and populate combobox
    var files = _diskImageFiles.GetDiskImageFiles();
    FilesComboBox1.Items.Clear();
    FilesComboBox1.DisplayMember = "FileName";
    
    foreach (var file in files)
    {
        FilesComboBox1.Items.Add(file);
    }
    
    // 4. Show summary
    tbreceived.Append($"Found {files.Count} files on disk.\r\n");
}

private bool IsPCDiskFormat()
{
    return processing.diskformat == DiskFormat.pcdd || 
           processing.diskformat == DiskFormat.pchd;
}
```

### Integration Points
- **Triggers**: LoadFilesButton.Click event
- **Inputs**: processing.disk[], processing.diskformat
- **Outputs**: Populated FilesComboBox1
- **Downstream**: FilesComboBox1.SelectedIndexChanged triggers highlighting

### Sub-tasks
- [ ] Add _diskImageFiles field to FloppyControl class
- [ ] Implement LoadFilesButton_Click handler
- [ ] Implement IsPCDiskFormat() helper
- [ ] Wire up event handler in Designer (if not wired)
- [ ] Add error handling for filesystem read failures
- [ ] Show feedback in tbreceived

### Acceptance Criteria
- Clicking button with PC disk populates combobox
- Clicking button with non-PC disk shows message
- Invalid filesystem shows appropriate error message
- Files displayed by filename only

---

## Item 6: Implement FilesComboBox Selection Handler

**Status**: Pending  
**Priority**: High  
**Complexity**: Medium

### Description
Handle file selection in FilesComboBox1 to highlight the file's sectors in the sectormap.

### Files
- `FloppyControlApp.cs` - Add event handler method

### Implementation Details

```csharp
private void FilesComboBox1_SelectedIndexChanged(object sender, EventArgs e)
{
    // 1. Clear previous highlighting
    processing.SectorMap.ClearHighlighting();
    
    // 2. Get selected file
    if (FilesComboBox1.SelectedItem == null)
        return;
        
    var selectedFile = (DiskFileInfo)FilesComboBox1.SelectedItem;
    
    // 3. Get sector locations
    var sectors = selectedFile.Sectors;
    
    // 4. Highlight sectors
    Color highlightColor = Color.FromArgb(0x80, 0xB0, 0x80);  // #80B080
    processing.SectorMap.HighlightSectors(sectors, highlightColor);
    
    // 5. Show file info
    tbreceived.Append($"Selected: {selectedFile.FileName} - {selectedFile.FileSize} bytes, " +
                      $"{sectors.Count} sectors, {selectedFile.BadSectorCount} bad\r\n");
}
```

### Integration Points
- **Triggers**: FilesComboBox1.SelectedIndexChanged
- **Inputs**: Selected DiskFileInfo from combobox
- **Outputs**: Highlighted sectors in sectormap
- **Downstream**: User can visually see which sectors belong to file

### Sub-tasks
- [ ] Implement FilesComboBox1_SelectedIndexChanged handler
- [ ] Wire up event handler in Designer (if not wired)
- [ ] Clear highlighting when selection cleared
- [ ] Show file summary in tbreceived
- [ ] Test with files spanning multiple tracks

### Acceptance Criteria
- Selecting file highlights its sectors in green
- Selecting different file clears old highlighting
- File info displayed in log area
- Works for both rtbSectorMap and QrtbSectorMap

---

## Item 7: Implement ExportFilesButton Handler

**Status**: Pending  
**Priority**: High  
**Complexity**: High

### Description
Implement export functionality that saves the selected file multiple times, each with a different bad sector substitution.

### Files
- `FloppyControlApp.cs` or `FloppyControlErrorCorrection.cs` - Add event handler
- May need new method in `DiskImageFiles.cs` for extraction

### Implementation Details

```csharp
private void ExportFilesButton_Click(object sender, EventArgs e)
{
    // 1. Validate selection
    if (FilesComboBox1.SelectedItem == null)
    {
        MessageBox.Show("Please select a file to export.", "No File Selected", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
    }
    
    if (BadSectorListBox.SelectedIndices.Count == 0)
    {
        MessageBox.Show("Please select bad sector candidates from the list.", 
                        "No Sectors Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
    }
    
    var selectedFile = (DiskFileInfo)FilesComboBox1.SelectedItem;
    
    // 2. Create output directory
    string basePath = fileio.PathToRecoveredDisks;
    string outputDir = Path.Combine(basePath, outputfilename.Text, "recovery");
    Directory.CreateDirectory(outputDir);
    
    // 3. Get original sector data for restoration
    var originalSectorData = BackupSectorData(selectedFile.Sectors);
    
    // 4. Loop through selected bad sectors
    int counter = 1;
    foreach (int index in BadSectorListBox.SelectedIndices)
    {
        var badsector = (Badsectorkeyval)BadSectorListBox.Items[index];
        var sectordata = processing.sectordata2[badsector.Id];
        
        // 5. Substitute bad sector data into disk array
        SubstituteSectorData(sectordata);
        
        // 6. Extract file with substituted sector
        string filename = Path.GetFileNameWithoutExtension(selectedFile.FileName);
        string extension = Path.GetExtension(selectedFile.FileName);
        string outputPath = Path.Combine(outputDir, $"{filename}_{counter:D3}{extension}");
        
        ExtractFileFromDisk(selectedFile, outputPath);
        
        // 7. Restore original sector data
        RestoreSectorData(originalSectorData);
        
        counter++;
        tbreceived.Append($"Exported: {outputPath}\r\n");
    }
    
    tbreceived.Append($"Export complete. {counter - 1} file variations saved.\r\n");
    
    // 8. Open output folder
    Process.Start("explorer.exe", outputDir);
}
```

### Integration Points
- **Triggers**: ExportFilesButton.Click on Error Correction tab
- **Inputs**: 
  - Selected file from FilesComboBox1
  - Selected bad sectors from BadSectorListBox
- **Outputs**: Multiple file variations in recovery folder
- **Failure handling**: Restore original data if extraction fails

### Sub-tasks
- [ ] Implement ExportFilesButton_Click handler
- [ ] Implement BackupSectorData() - save original sector bytes
- [ ] Implement SubstituteSectorData() - copy bad sector data to disk[]
- [ ] Implement RestoreSectorData() - restore original bytes
- [ ] Implement ExtractFileFromDisk() - use Fat12Extractor to extract
- [ ] Wire up event handler in Designer
- [ ] Create output directory structure
- [ ] Handle extraction errors gracefully
- [ ] Show progress/completion in tbreceived
- [ ] Open output folder when complete

### Acceptance Criteria
- Button disabled when no file selected
- Each bad sector creates one output file
- Files named correctly: `{filename}_{001}.{ext}`
- Output folder opened after export
- Original disk data unchanged after export

---

## Item 8: Enable/Disable Controls Based on Disk Format

**Status**: Pending  
**Priority**: Medium  
**Complexity**: Low

### Description
Disable FAT12-related controls when disk format is not PC DOS (DD or HD).

### Files
- `FloppyControlApp.cs` - Add format checking logic

### Implementation Details

Add to format change handling and initialization:

```csharp
private void UpdateFAT12ControlsState()
{
    bool isPCFormat = processing.diskformat == DiskFormat.pcdd || 
                      processing.diskformat == DiskFormat.pchd;
    
    LoadFilesButton.Enabled = isPCFormat;
    FilesComboBox1.Enabled = isPCFormat;
    
    // Clear combobox if format changed to non-PC
    if (!isPCFormat)
    {
        FilesComboBox1.Items.Clear();
        processing.SectorMap.ClearHighlighting();
    }
}
```

Call this method when:
- Disk format is detected/changed
- After processing completes
- On ChangeDiskTypeComboBox change

### Integration Points
- **Triggers**: Disk format detection, manual format change
- **Outputs**: Enabled/disabled state of controls

### Sub-tasks
- [ ] Implement UpdateFAT12ControlsState() method
- [ ] Call after processing completes
- [ ] Call on ChangeDiskTypeComboBox_SelectedIndexChanged
- [ ] Call on QChangeDiskTypeComboBox_SelectedIndexChanged
- [ ] Clear file selection when format changes

### Acceptance Criteria
- Controls enabled for pcdd and pchd formats
- Controls disabled for amigados, diskspare, other formats
- Combobox cleared when switching to non-PC format
- Highlighting cleared when switching formats

---

## Testing Checklist

### Manual Testing
- [ ] Load PC DOS DD disk image, verify files listed
- [ ] Load PC DOS HD disk image, verify files listed
- [ ] Load Amiga disk, verify controls disabled
- [ ] Select file, verify sectors highlighted in green
- [ ] Select different file, verify highlighting changes
- [ ] Export file with 3 bad sector candidates, verify 3 output files
- [ ] Verify output file naming: `filename_001.ext`, `filename_002.ext`, etc.
- [ ] Verify original disk data unchanged after export
- [ ] Test with file spanning multiple tracks
- [ ] Test with file containing no bad sectors (should still export)

### Edge Cases
- [ ] Empty disk (no files)
- [ ] Corrupted FAT table
- [ ] File with 0 bytes
- [ ] Very large file (>100 sectors)
- [ ] File in subdirectory

---

## Dependencies Between Items

```
Item 1 (Project Reference)
    |
    v
Item 2 (Stream Support) -----> Item 3 (DiskImageFiles)
                                    |
                                    v
                        Item 4 (Highlighting) <---- Item 5 (LoadFiles)
                                    |                    |
                                    v                    v
                        Item 6 (Selection) ---------> Item 7 (Export)
                                                          |
                                                          v
                                                    Item 8 (Enable/Disable)
```

**Recommended Order**: 1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 8 -> 7

Items 1-2 must be done first (infrastructure).
Items 3-6 form the core file listing and visualization.
Item 8 (enable/disable) can be done in parallel with 5-6.
Item 7 (export) should be last as it depends on all other items.
