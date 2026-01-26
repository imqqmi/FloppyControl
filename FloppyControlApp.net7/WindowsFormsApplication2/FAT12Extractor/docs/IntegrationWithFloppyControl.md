# Integration of dskx into FloppyControlApp project

## Goal
I'd like to use dskx functionality (the core project) in my floppycontrollapp, which is a winforms C# .net 7.0 project. 

## Steps
This workflow I'd like to implement:
- Get all the good sectors as much as I can with normal processing
- When there's bad sectors left over
- Generate dsk file in ram (if it doesn't exist already)
- Extract file information with sector start/finish/chunks? so that we know what bad sectors are in the file of interest
- Put the files in a select box, user selects file of interest (flat view with paths)
- filter the bad sectors in the bad sector list to only show the ones that are used in the selected file
- Using limit track/sector in error correction (ie Track1UpDown/Track2UpDown, Sector1UpDown/Sector2UpDown), focus on one sector at a time

Loop bad sectors of the same track/sector
- Realign 4E markers (probably need a way of finding where the curruption takes place so that we know the range from the end back towards the corruption point.) ie compare two bad sectors of the same track and sector detect first corruption from header. then trace back from end, realign until we find some 4Es then trace back towards corruption point.
- Copy the realigned sector to the disk image in memory
- Extract the file of interest and save to disk with file counter in
  diskrecoveries\basefilename\filename\filename_001.ext
Loop next
- Done!

I will create the front end winforms stuff and I'll give it to the LLM
Move the dskx project to floppycontrolapp project folder for reference. 

## User workflow to extract files with bad sector variations 
My workflow will then be, to recover corrupted files and trying to get the best 'bad sector' from each track/sector location:
- Recover as many sectors as possible
- Go to Error correction
- Click Sector Overlay
- Click show files in disk image
- select file of interest > filter bad sector list (probably need to backup complete bad sector list)
- Bad sector list now shows all the affected bad sectors
- remaining bad sectors, if multiple sectors affect one file:
  - Go to processing tab, check checkboxes/radio control: 
    - process sectors with clear sector data
    - use error correction
    - skip already CrcOk
    - deduplicate
  - Right click sector map on bad sector known to be within the file, select Error correct track x sector y
  - Go to error correction tab
  - Click sector overlay, showing all the bad sectors of selected track/sector of the selected file
  - Click new button 'Export file with bad sector variations'
  - Files will be written with the steps above.
  - Identify the file with the best recovered bad sector > Need some kind of way to map the bad sector to the file counter, maybe a hash?
- Make sure the best bad sector that was found will be made permanent in the memory disk image
- Move on the the next bad sector rinse and repeat

