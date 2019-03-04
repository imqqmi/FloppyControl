# FloppyControl

## Introduction
This project started in 2015. I had 300 floppy disks that I kept from my Amiga 500 and Amiga 1200 period. I planned to buy a second hand Amiga after I sold them for a new PC. That of course didn't happen. The data stored on the floppy disks was mostly my personal files, videos, music modules, painted pictures, 3D objects and renderings etc. And a complete backup of my system and data files. I wanted to reconstruct the files that I used to have. 

However, the Amiga disk format is not something any PC could read, at least not in Windows in a simple way. The disks were in a poor state with fungus and dust. So I set out to clean them. The 3D printable Floppy cleaning kit was a result of this. 

##This is the stategy I followed to get the most data without the risk of losing it:
- The most valuable data will be recovered once I can be certain the recovery proces is sound
- No captured data is thrown away. The first capture could be the best one because the disk may disintegrate/scratch during the capture process due to the disk surface letting loose.
- Extensive analysis is done to make sure that every opportunity is taken to get the data. This includes doing oscilloscope captures of the analoque flux signal, error correct and looking for duplicate data for reconstruction
- Low cost, off the shelve parts
- Windows application to control the hardware (C#)

##Status
This is the first commit of the project. I will call it version 1.0. It's capable of capturing Amiga and PC DOS disks and some variants of both. MSX disks for example are also supported. All disks can be captured, though not all disks can be decoded to a disk image.



##Hardware
The first version I made was based on a PIC16F1938 overclocked at 80MHz. An USB2RS232 donlge with a PL2303 chip was used at 5mbit/s.
The second (current) version uses a standard Arduino Due. I'm in the process of designing a shield for it. The native usb is used which eliminates the USB2RS232 dongle and makes things a lot more stable.

##Software
The real magic happens in the software that converts the flux transitions (or periods) into MFM, then into data and finally into sectors and disk images.

The following disk formats are supported:
- Double density and high density (720KB and 1.44MB) 3.5" disks
- 360KB 5.25" PCDOS disks.
- M2 special format for storing larger amounts of data
- AmigaDOS and Amiga DiskSpare disk formats. PFS is also supported since the format is similar to AmigaDOS.

File formats that can be imported:
- .bin files produces by FloppyControl App, a simple stream dump file format. Each byte contains a period of 2, 4, 6 or 8us. Timer tick frequency may vary.
- .SCP files can be imported
- .raw files from Kryoflux can be imported.

There are some experimental error correction functions in FloppyControl that allows you to do some analysis and limited error correction. It's still a very time consuming and manual process.

Other features include:
- Oscilloscope analogue flux transition capture support. At this point only the Rigol DS1054Z is supported and tested. Other scopes may work but it's not tested. Please let me know if it works. A modified floppy drive is needed for this. The oscilloscope should have at least 20MB storage to store a full track in memory.
- Oscilloscope captures can be edited so that errors can be corrected manually. A waveform editor is included. 
- The waveform can be converted into sector data using the processing capabilities (differential filtering, time based filtering, period conversion and data conversion).
- A sectormap shows you exactly the state of the capture and conversion. Bad sectors can be recaptured and existing data can be reprocessed with different settings, allowing you to 'scan' for good sectors in the dataset.
- 
