I'd like to discuss a new project: FloppyExtractor.

# The project goal

I want to build a C# .net cli application that can read a floppydisk image ie IBM PC compatible 3.5" floppies 720KB and 1.44MB floppies (more formats like AmigaDOS may follow later).

Requirements
- Binary name: dskx.exe
- C# .net 7.0 cli application with usage "dskx.exe x <inputfile.dsk> ." which will extract all files to the current directory. The "." can be changed into any valid path
- Option to extract a file by its filename
- The cli interface and the logic to extract the files live in separate class files, ie the cli and the actual code are separated so that the FAT12 extraction methods can be easily used in another application.
- Use existing FAT12 libraries if available, otherwise create your own
- Example disk images can be found in ./ExampleDiskImages. There is a 720KB and 1.44MB variant. 
- Note that the cli application must be able to handle files and paths with spaces.
