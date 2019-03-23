using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Collections.Concurrent;

//using FDCPackage;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using FloppyControlApp.MyClasses;

namespace FloppyControlApp
{
    public partial class FDDProcessing
    {
        public static byte[] AMIGAMARKER = { 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1 };
        //public string AMIGAMARKER = "01000100100010010100010010001001"; // Length=32
        public static byte[] AMIGADSMARKER = { 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 };
        public int stat4us;
        public int stat6us;
        public int stat8us;
        public int scatterplotstart { get; set; }
        public int scatterplotend { get; set; }
        

        // bytes version of ProcessAmiga
        private void ProcessAmigaMFMbytes(ProcSettings procsettings, int threadid)
        {
            int i;
            uint j;
            uint searchcnt = 0;
            int rxbufcnt = 0;
            int overflow = 0;
            int markerpositionscntthread = 0;
            int bytespersectorthread = 512;
            int badsectorcntthread = 0;
            //uint totaltime = 0;
            //uint reltime = 0;
            bool debuginfo = false;
            SHA256 mySHA256 = SHA256Managed.Create();
            int sectordata2oldcnt = sectordata2.Count;


            byte[] amigamarkerbytes = AMIGAMARKER;

            // Find all the sector markers

            //DiskSpare marker
            byte[] amigadsmarkerbytes = AMIGADSMARKER;

            //tbreceived.Append("mfmlength: " + mfmlengths[threadid] + " ");
            //totaltime += reltime = relativetime();
            //int previousSDcount = sectordata2.Count;
            //=============================================================================================
            //Find diskspare sector markers
            if (indexrxbuf > rxbuf.Length) indexrxbuf = rxbuf.Length - 1;

            if ((diskformat == DiskFormat.unknown || diskformat == DiskFormat.diskspare))
            {
                rxbufcnt = procsettings.start;
                searchcnt = 0;
                // Find markers
                for (i = 0; i < mfmlengths[threadid]; i++)
                {
                    if (i % 1048576 == 1048575)
                    {
                        progresses[threadid] = i;
                        if (stop == 1) break;
                    }
                    if (mfms[threadid][i] == 1) // counting 1's matches the number of bytes in rxbuf + start offset
                        rxbufcnt++;

                    if (rxbufcnt < rxbuf.Length - 1)
                    {
                        while (rxbuf[rxbufcnt] < 4)
                        {
                            if (rxbufcnt < rxbuf.Length - 1)
                                rxbufcnt++;
                        else
                            break;
                        }
                    }
                    for (j = 0; j < amigadsmarkerbytes.Length; j++)
                    {
                        if (mfms[threadid][i + j] == amigadsmarkerbytes[j]) searchcnt++;
                        else
                        {
                            searchcnt = 0;
                            break;
                        }
                        if (searchcnt == amigadsmarkerbytes.Length - 1)
                        {
                            searchcnt = 0;

                            MFMData sectordata = new MFMData();
                            sectordata.MarkerPositions = i - 16;
                            sectordata.rxbufMarkerPositions = rxbufcnt; // start of 44894489 uint32, not at AAAAAAAA

                            if (!sectordata2.TryAdd(sectordata2.Count, sectordata))
                            {
                                tbreceived.Append("Failed to add to Sectordata dictionary " + markerpositionscntthread + "\r\n");
                                return;
                            }

                            markerpositionscntthread++;
                            break;
                        }
                    }
                    //if (overflow == 1) break;
                }
                if (markerpositionscntthread > 0)
                {
                    tbreceived.Append("Format: DiskSpare ");
                    diskformat = DiskFormat.diskspare; // Diskspare format
                                                       //DiskTypeLabel.Text = "DiskSpare";
                                                       //ShowDiskFormat();
                }
                else
                {
                    tbreceived.Append("\r\nNo DiskSpare markers found\r\n");

                }
            }
            progresses[threadid] = (int)mfmlengths[threadid];
            ProcessStatus[threadid] = "Find AmigaDOS MFM markers...";

            searchcnt = 0;
            rxbufcnt = 0;
            overflow = 0;
            //Find AmigaDOS markers
            //=============================================================================================
            if ((diskformat == DiskFormat.unknown || diskformat == DiskFormat.amigados))
            {
                rxbufcnt = procsettings.start;
                searchcnt = 0;
                // Find AmigaDOS markers
                for (i = 0; i < mfmlengths[threadid]; i++)
                {
                    if (i % 1048576 == 1048575) { progresses[threadid] = i; }
                    if (mfms[threadid][i] == 1) // counting 1's matches the number of bytes in rxbuf + start offset
                        rxbufcnt++;
                    if (rxbufcnt >= rxbuf.Length) break;
                    while (rxbuf[rxbufcnt] < 4 && rxbufcnt < indexrxbuf-1) rxbufcnt++;
                    for (j = 0; j < amigamarkerbytes.Length; j++)
                    {
                        if (mfms[threadid][i + j] == amigamarkerbytes[j]) searchcnt++;
                        else
                        {
                            searchcnt = 0;
                            break;
                        }
                        if (searchcnt == amigamarkerbytes.Length - 1)
                        {
                            searchcnt = 0;
                            MFMData sectordata = new MFMData();

                            sectordata.MarkerPositions = i - 32; // start at AAAAAAAA
                            sectordata.rxbufMarkerPositions = rxbufcnt; // start of 44894489 uint32, not at AAAAAAAA
                            
                            if (!sectordata2.TryAdd(sectordata2.Count, sectordata))
                            {
                                tbreceived.Append("Failed to add to Sectordata dictionary " + markerpositionscntthread + "\r\n");
                            }
                            markerpositionscntthread++;
                        }
                    }
                    //if (overflow == 1) break;
                }

                if (markerpositionscntthread > 0)
                {
                    tbreceived.Append("Format: AmigaDOS ");
                    diskformat = DiskFormat.amigados; // AmigaDOS format
                }
                else //No valid diskdata recognized
                {
                    tbreceived.Append("\r\nNo valid disk data recognized.\r\n");
                    return;
                }
            }
            progresses[threadid] = (int)mfmlengths[threadid];

            //totaltime += reltime = relativetime();
            //tbreceived.Append(reltime + "ms finding markers.\r\n");
            tbreceived.Append("Marker count: " + markerpositionscntthread.ToString() + " ");
            /*
            for (i=0; i<markerpositionscntthread; i++)
            {
                TxtTextBox1.Text += markerpositions[i].ToString() + "\r\n";
            }
            */
            
            //=============================================================================================
            //The bit count per sector is 8704, taking 10k just in case. 
            //If the marker is missed, the size will be a multiple of 8705, but anything beyond 8705
            // can be safely cut off. 8705 is the number of bits. We're going from MFM bits to uint32
            //So 8705/32 = 272. We'll take 300.
            //uint[,] mfmmarkerdata = new uint[markerpositionscntthread, mfmsectorlength];

            uint[][] mfmmarkerdata = new uint[markerpositionscntthread][];

            //int x;
            StringBuilder txtbuild = new StringBuilder();

            StringBuilder txtbox = new StringBuilder();
            StringBuilder trackbox = new StringBuilder();
            StringBuilder trackboxtemp = new StringBuilder();
            StringBuilder decodedamigaText2 = new StringBuilder();
            StringBuilder decodedamigaText = new StringBuilder();


            int sectorindex;
            byte[] savechecksum = new byte[4];

            //totaltime += reltime = relativetime();
            //tbreceived.Append(reltime + "ms Putting all markers and sector data in two dim array. \r\n");

            ProcessStatus[threadid] = "Converting mfm to sectors...";
            progressesstart[threadid] = 0;
            progressesend[threadid] = (int)markerpositionscntthread;

            byte[] headerchecksum;
            byte[] datachecksum;

            int mrkridx;
            //Now loop through all sectors to decode data
            byte[] checksum;

            MFMData sectordatathread;

            for (sectorindex = sectordata2oldcnt; sectorindex < sectordata2.Count; sectorindex++)
            {
                sectordatathread = sectordata2[sectorindex];

                if (sectorindex % 50 == 49) { progresses[threadid] = (int)sectorindex; if (stop == 1) break; }

                mrkridx = (int)sectordatathread.MarkerPositions;

                byte[] dec1 = new byte[1];

                string checksumok = "";
                byte headercheckok = 0;
                byte datacheckok = 0;

                byte tracknr = 0;
                byte sectornr = 0;
                byte gapoffset = 0;

                //Decode mfm to bytes
                //====================================================================================================
                //AmigaDOS disk format for ADOS and PFS
                if (diskformat == DiskFormat.amigados) //AmigaDOS disk format for ADOS and PFS
                {

                    //Get checksums first:
                    headerchecksum = amigamfmdecodebytes(mfms[threadid], mrkridx + (48 * 8), 4 * 16); // At uint 6
                    datachecksum = amigamfmdecodebytes(mfms[threadid], mrkridx + (56 * 8), 4 * 16); // At uint 6

                    dec1 = amigamfmdecodebytes(mfms[threadid], mrkridx + (8 * 8), 4 * 16); //At byte position 8

                    tracknr = dec1[1];
                    sectornr = dec1[2];
                    gapoffset = dec1[3];

                    checksum = amigachecksum(mfms[threadid], mrkridx + (8 * 8), 4 * 16); // Get header checksum from sector header

                    // Compare disk stored checksum and calculated checksum when decoding:
                    if (headerchecksum.SequenceEqual(checksum))
                    {
                        checksumok = "H:OK ";
                        headercheckok = 1;
                    }
                    else checksumok = "H:ER ";
                    trackboxtemp.Append(checksumok);

                    //Sector data
                    dec1 = amigamfmdecodebytes(mfms[threadid], mrkridx + (64 * 8), 512 * 16); ;
                    checksum = amigachecksum(mfms[threadid], mrkridx + (64 * 8), 512 * 16); // Get header checksum from sector header
                    savechecksum[0] = checksum[0]; savechecksum[1] = checksum[1]; savechecksum[2] = checksum[2]; savechecksum[3] = checksum[3];
                    // Do the data checksum check:
                    if (datachecksum.SequenceEqual(checksum)) // checksum is changed everytime amigamfmdecode() is called
                    {
                        //checksumok = "D:OK ";
                        datacheckok = 1;
                    }
                    else checksumok = "D:ER ";
                }
                //====================================================================================================

                else if (diskformat == DiskFormat.diskspare) // Assume it's DiskSpare format
                {
                    //Diskspare sector format:
                    // <AAAA><4489><4489><AAAA> <TT><SS> <OddChkHi> <OddChkLo> <EvenChkHi> <EvenChkLo>

                    uint header;
                    byte[] dsdatachecksum = new byte[4];
                    //int test;

                    headercheckok = 1; // Diskspare doesn't do separate header and data checksums

                    // Get track sector and checksum within one uint32: 0xTTSSCCCC
                    dec1 = amigamfmdecodebytes(mfms[threadid], mrkridx + 8 * 8, 4 * 16); //At uint 0
                    header = dec1[0];
                    tracknr = dec1[0];
                    sectornr = dec1[1];
                    dsdatachecksum[0] = dec1[2];
                    dsdatachecksum[1] = dec1[3];
                    dsdatachecksum[2] = 0;
                    dsdatachecksum[3] = 0;

                    //Clear display info buffer
                    trackboxtemp.Clear();

                    uint dchecksum = 0;
                    uint testcount = 0;

                    uint offset;
                    byte[] tmp;
                    dec1 = new byte[520];
                    offset = 4;
                    //int cnt = 0;

                    //checksum = 0;
                    for (j = 0; j < 512; j += 4)
                    {
                        tmp = amigamfmdecodebytes(mfms[threadid], mrkridx + (int)j * 16 + 16 * 8, 4 * 16);
                        dec1[j] = tmp[0];
                        dec1[j + 1] = tmp[1];
                        dec1[j + 2] = tmp[2];
                        dec1[j + 3] = tmp[3];
                    }

                    dchecksum = (uint)((mfm2ushort(mfms[threadid], mrkridx + 8 * 16)) & 0x7FFF);
                    ushort tmp1 = 0;
                    offset = 9;
                    for (j = offset; j < 520; j++)
                    {
                        tmp1 = mfm2ushort(mfms[threadid], (int)(mrkridx + j * 16));
                        dchecksum ^= (uint)(tmp1 & 0xffff);
                    }
                    savechecksum[0] = (byte)(dchecksum >> 8);
                    savechecksum[1] = (byte)(dchecksum & 0xFF);
                    savechecksum[2] = 0;
                    savechecksum[3] = 0;

                    if (debuginfo)
                    {
                        tbreceived.Append("testcount: " + testcount);
                        tbreceived.Append(mfmmarkerdata[sectorindex][offset].ToString("X8") + " " + header.ToString("X8") +
                            "dsdatachecksum: " + dsdatachecksum[0].ToString("X2") + dsdatachecksum[1].ToString("X2") +
                            " markerpos: " + sectordatathread.MarkerPositions.ToString() +
                            " checksum: " + dchecksum.ToString("X4") + "\r\n");
                    }

                    // Do the data checksum check:
                    if (dsdatachecksum.SequenceEqual(savechecksum)) // checksum is changed everytime amigamfmdecode() is called
                    {
                        checksumok = "D:OK ";
                        datacheckok = 1;
                    }
                    else
                    {
                        checksumok = "D:ER ";
                        datacheckok = 0;
                        headercheckok = 1; // If diskspare, data = bad? header can't be trusted
                    }

                    trackboxtemp.Append(checksumok + "\r\n");

                    if (checksumok == "D:OK ")
                        trackboxtemp.Clear();
                    else trackbox.Append(trackboxtemp);
                }
                //====================================================================================================

                /*
                if (debuginfo)
                    for (i = 0; i < dec1.Length; i++)
                    {
                        t = dec1[i];
                        decodedamigaText.Append(t.ToString("X8") + " ");
                        t1 = (char)(t >> 24 & 0xff);
                        t2 = (char)(t >> 16 & 0xff);
                        t3 = (char)(t >> 8 & 0xff);
                        t4 = (char)(t & 0xff);

                        if (t1 > 32 && t1 < 128) decodedamigaText2.Append(t1);
                        if (t2 > 32 && t2 < 128) decodedamigaText2.Append(t2);
                        if (t3 > 32 && t3 < 128) decodedamigaText2.Append(t3);
                        if (t4 > 32 && t4 < 128) decodedamigaText2.Append(t4);

                    }
                */

                sectorspertrack = 11;

                if (diskformat == DiskFormat.diskspare) // Diskspare
                    sectorspertrack = 12;

                // Write sector to disk image array in its proper location
                // The image file doesn't need checksum and track info, it's implied
                // With the location of the data, track 0 sector 0 starts at disk[0], track 1 sector 1
                // at disk[6144] etc
                if (diskformat == DiskFormat.diskspare) // DiskSpare doesn't have header checksum
                {
                    if (tracknr < 164 && sectornr < 20)
                        if (datacheckok == 1) sectormap.sectorokLatestScan[tracknr, sectornr]++;
                }
                else if (headercheckok == 1)
                    if (tracknr < 164 && sectornr < 20) sectormap.sectorokLatestScan[tracknr, sectornr]++;


                //If the checksum is correct and sector and track numbers within range and no sector data has already been captured
                //if (headercheckok == 1 && datacheckok == 1 && sectornr >= 0 && sectornr < 13 && tracknr >= 0 && tracknr < 164 && sectormap.sectorok[tracknr, sectornr] != 1)
                if (headercheckok == 1 && datacheckok == 1 && sectornr >= 0 && sectornr < 13 && tracknr >= 0 && tracknr < 164) // collect good sectors
                {
                    // add error correction data
                    if (procsettings.UseErrorCorrection)
                    {
                        byte[] bytebuf = new byte[514];
                        for (i = 0; i < 512; i++)
                        {
                            bytebuf[i] = dec1[i];
                        }
                        bytebuf[512] = tracknr;
                        bytebuf[513] = sectornr;
                        //Create hash
                        byte[] secthash = mySHA256.ComputeHash(bytebuf);

                        // Check if there's a duplicate
                        int isunique = -1;
                        for (i = 0; i < markerpositionscntthread; i++)
                        {
                            isunique = IndexOfBytes(badsectorhash[i], secthash, 0, 32);
                            if (isunique != -1)
                            {
                                //tbreceived.Append("Duplicate found!\r\n");
                                break;
                            }
                        }

                        if (isunique == -1 || !procsettings.finddupes)
                        {
                            //lock (lockbadsector)
                            {
                                //int badsectorcnt2 = badsectorcnt;
                                badsectorcntthread++;
                                badsectorhash[sectorindex] = secthash;

                                sectordatathread.threadid = threadid;
                                sectordatathread.mfmMarkerStatus = SectorMapStatus.CrcOk; // 1 = Good sector data
                                sectordatathread.track = tracknr;
                                sectordatathread.sector = sectornr;
                                sectordatathread.sectorlength = bytespersectorthread;
                                sectordatathread.crc = (int)((savechecksum[0] << 24) | (savechecksum[1] << 16) | (savechecksum[2]) << 8 | (savechecksum[3]));
                                sectordatathread.sectorbytes = bytebuf;
                                sectordatathread.MarkerType = MarkerType.headerAndData;

                            }
                        }
                    }
                    // Prevent overwriting good sector data with other good sector data.
                    if (sectormap.sectorok[tracknr, sectornr] != SectorMapStatus.CrcOk)
                    {
                        sectormap.sectorok[tracknr, sectornr] = SectorMapStatus.CrcOk;
                        FoundGoodSectorInfo.Append("T" + tracknr.ToString("D3") + " S" + sectornr + " crc:" + sectordatathread.crc.ToString("X4") + " markerindex:" + sectorindex + " Method: ");
                        if (procsettings.processingtype == ProcessingType.aufit) // aufit
                        {
                            FoundGoodSectorInfo.Append("Aufit min:" + procsettings.min.ToString("X2") + " 4/6:" + procsettings.four.ToString("X2"));
                        }
                        else
                        if (procsettings.processingtype == ProcessingType.adaptive1)
                        {
                            FoundGoodSectorInfo.Append("Adaptive Rate:" + procsettings.rateofchange);

                        }
                        else
                        if (procsettings.processingtype == ProcessingType.normal)
                        {
                            FoundGoodSectorInfo.Append("Normal min:" + procsettings.min.ToString("X2") + " 4/6:" + procsettings.four.ToString("X2") +
                                " 6/8:" + procsettings.six.ToString("X2") + " max:" + procsettings.max.ToString("X2") +
                                " offset:" + procsettings.offset.ToString());
                        }
                        FoundGoodSectorInfo.Append("\r\n");
                        FoundGoodSectorInfo.Append(CurrentFiles);

                        int offset;
                        // T00 S00 H0 = 0
                        // T00 S00 H1 = 
                        offset = (tracknr * sectorspertrack * 512) + (sectornr * 512);
                        int q = 0, sum = 0;
                        for (i = 0; i < 512; i++)
                        {
                            if (disk[i + offset] != 0x00000000 && sectormap.sectorok[tracknr, sectornr] == SectorMapStatus.empty) tbreceived.Append("Overwriting Offset: " + offset + " i: " + i + " track:" + tracknr + " sector: " + sectornr + "\r\n");

                            sum += disk[i + offset] = dec1[q];
                            q++;
                        }
                        if (sum == 0) sectormap.sectorok[tracknr, sectornr] = SectorMapStatus.SectorOKButZeroed; // If the entire sector is zeroes, allow new data
                    }
                }
                else if (headercheckok == 1 && datacheckok == 0 && sectornr >= 0 && sectornr < 13 && tracknr >= 0 && tracknr < 164) // collect good sectors
                {
                    if (sectormap.sectorok[tracknr, sectornr] == SectorMapStatus.empty)
                        sectormap.sectorok[tracknr, sectornr] = SectorMapStatus.HeadOkDataBad;
                    else if (sectormap.sectorok[tracknr, sectornr] != SectorMapStatus.HeadOkDataBad) // if it's a bad sector, capture more data, otherwise, go to next marker
                        continue;

                    if (procsettings.UseErrorCorrection)
                    {
                        byte[] bytebuf = new byte[514];
                        for (i = 0; i < 512; i++)
                        {
                            bytebuf[i] = dec1[i];
                        }
                        bytebuf[512] = tracknr;
                        bytebuf[513] = sectornr;
                        //Create hash
                        byte[] secthash = mySHA256.ComputeHash(bytebuf);

                        // Check if there's a duplicate
                        int isunique = -1;
                        for (i = 0; i < markerpositionscntthread; i++)
                        {
                            isunique = IndexOfBytes(badsectorhash[i], secthash, 0, 32);
                            if (isunique != -1)
                            {
                                //tbreceived.Append("Duplicate found! T"+tracknr.ToString("d3")+" S"+sectornr+"\r\n");
                                break;
                            }
                        }

                        if (isunique == -1 || !procsettings.finddupes)
                        {
                            //lock (lockbadsector)
                            {
                                //int badsectorcnt2 = badsectorcnt;
                                badsectorcntthread++;
                                badsectorhash[sectorindex] = secthash;

                                sectordatathread.threadid = threadid;
                                sectordatathread.mfmMarkerStatus = SectorMapStatus.HeadOkDataBad; // 2 = bad sector data
                                sectordatathread.track = tracknr;
                                sectordatathread.sector = sectornr;
                                sectordatathread.sectorlength = bytespersectorthread;
                                sectordatathread.crc = (int)((savechecksum[0] << 24) | (savechecksum[1] << 16) | (savechecksum[2]) << 8 | (savechecksum[3]));
                                sectordatathread.sectorbytes = bytebuf;
                                sectordatathread.MarkerType = MarkerType.headerAndData;
                            }
                        }
                    }
                }
                //If checksum is not ok, we can still use the data, better than nothing strategy, we will show it in the sectormap

                // This works on PCDOS floppies because the header has its own CRC16 checksum, which is, considering
                // the few bytes it covers, very strong. AmigaDOS has a weaker form of header and data crc.
                // DiskSpare OTOH only has one checksum for the entire
                // sector and its meta data. I've noticed that track info may vary wildly, overwriting good sectors because
                // the track and sector data is corrupted. Commented out for now. I may do a stronger check like if all 512 bytes are empty
                // Only then it may write. 
                /*
                else if (headercheckok == 1 && datacheckok == 0 && sectornr >= 0 && sectornr < 13 && tracknr >= 0 && tracknr < 164 && sectormap.sectorok[tracknr, sectornr] != 1 && sectormap.sectorok[tracknr, sectornr] != 2)
                {
                    //RecoveredSectorWithErrorsCount++;
                    sectormap.sectorok[tracknr, sectornr] = 2;
                    int offset;
                    // T00 S00 H0 = 0
                    // T00 S00 H1 = 
                    offset = (tracknr * sectorspertrack * 512) + (sectornr * 512);
                    //offset /= 4;
                    int q = 0;
                    for (i = 0; i < 512; i += 4)
                    {
                        if (disk[i + offset] != 0x00000000 && sectormap.sectorok[tracknr, sectornr] == 0) tbreceived.Append("Overwriting Offset: " + offset + " i: " + i + " track:" + tracknr + " sector: " + sectornr + "\r\n");

                        disk[i + offset] = (byte)(dec1[q] >> 24);
                        disk[i + offset + 1] = (byte)((dec1[q] >> 16) & 0xff);
                        disk[i + offset + 2] = (byte)((dec1[q] >> 8) & 0xff);
                        disk[i + offset + 3] = (byte)(dec1[q] & 0xff);
                        q++;
                    }
                }

                */
                // Process hashes to check if there are false positives using the sector checksums which are pretty weak for the amiga 32 bit, no polynomial
                // Especially diskspare (16bit checksum without polynomial)
                // This code is not yet thread safe. It references sectordatatable which is a 'global'
                // I need to create a local sectordatatable, then at the main thread copy the data to the global sectordatatable
                /*
                if (procsettings.finddupes)
                    if (headercheckok == 1 && datacheckok == 1 && sectornr >= 0 && sectornr < 18 && tracknr >= 0 && tracknr < 164)
                    {
                        int offset;
                        string hashstring = "";

                        offset = (tracknr * sectorspertrack * 512) + (sectornr * 512);

                        byte[] sdata = new byte[512]; int q = 0;
                        for (i = 0; i < 512; i += 4)
                        {
                            sdata[i] = (byte)(dec1[q] >> 24);
                            sdata[i + 1] = (byte)((dec1[q] >> 16) & 0xff);
                            sdata[i + 2] = (byte)((dec1[q] >> 8) & 0xff);
                            sdata[i + 3] = (byte)(dec1[q] & 0xff);
                            q++;
                        }

                        sectorhash = mySHA256.ComputeHash(sdata);
                        foreach (byte b in sectorhash) hashstring += b.ToString("X2");

                        int found = 0;

                        // Is the hash already in the table? Skip
                        foreach (DataRow row in sectordatatable.Rows)
                        {
                            if (row.Field<string>("hashstring") == hashstring)
                            {
                                found = 1;
                                break;
                            }
                        }

                        // if not, add to table
                        if (found != 1)
                        {

                            sectordatatable.Rows.Add(null, tracknr, sectornr, offset, sectorhash, hashstring, sdata);
                        }
                    }
                    */
            }
            progresses[threadid] = (int)sectordata2.Count;
            ProcessStatus[threadid] = "Done!";
            //progressesstart[threadid] = 0;
            progressesend[threadid] = (int)sectordata2.Count;

            //sectordata[threadid] = sectordatathread;
            //markerpositionscounts[threadid] = markerpositionscntthread;
            //totaltime += reltime = relativetime();

            //tbreceived.Append(reltime + "ms Convert to sector data.\r\n");
            //tbreceived.Append(totaltime + "ms total.\r\n");

            if (debuginfo)
            {
                sectormap.rtbSectorMap.Text += decodedamigaText.ToString();
            }
        }

        public void ProcessClusterAmigaMFMEnc(ECSettings ecSettings)
        {
            int i;

            byte[] Marker = new byte[0];
            int periodshift = 0;
            byte[] crcinsectordatadecoded = new byte[100];

            int bitshifted = 0;
            byte[] lasttwosectorbytes = new byte[100];
            int periodSelectionStart;//, mfmSelectionStart = 0;
            int periodSelectionEnd;//, mfmSelectionEnd = 0;
            int mfmAlignedStart, mfmAlignedEnd;

            int[] combi = new int[32];
            int combilimit;

            int indexS1 = ecSettings.indexS1;
            int threadid = ecSettings.threadid;

            mfmAlignedStart = ecSettings.MFMByteStart;
            mfmAlignedEnd = mfmAlignedStart + (ecSettings.MFMByteLength * 8);
            MFMByteEncPreset mfmpreset = new MFMByteEncPreset();

            if (diskformat == DiskFormat.amigados)
            {
                Marker = FDDProcessing.AMIGAMARKER;
                periodshift = 16;
            }
            else
            if (diskformat == DiskFormat.diskspare)
            {
                Marker = FDDProcessing.AMIGADSMARKER;
                periodshift = 8;
            }

            if (Marker.Length == 0)
            {
                tbreceived.Append("No amiga format found!\r\n");
                return;
            }
            //StopWatch sw = new StopWatch;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();

            // User selected part to be brute forced:
            periodSelectionStart = ecSettings.periodSelectionStart;
            periodSelectionEnd = ecSettings.periodSelectionEnd;

            // Stop if selection is too large, taking too long.
            if (periodSelectionEnd - periodSelectionStart > 50)
            {
                tbreceived.Append("Selection too large, please make it smaller, 50 max.\r\n");
                return;
            }

            // Copy mfm data from mfms
            int sectorlength = sectordata2[indexS1].sectorlength;


            byte[] mfmbuf = mfms[sectordata2[indexS1].threadid].SubArray(sectordata2[indexS1].MarkerPositions, (sectorlength + 100) * 16);
            byte[] bytebuf = new byte[sectorlength + 6];

            tbreceived.Append("mfmAlignedstart: " + mfmAlignedStart + " mfmAlignedEnd: " + mfmAlignedEnd + "\r\n");

            // Find 4E right after the crc bytes at the end of the sector
            // 4E bytes are padding bytes between header and data. 
            // When the 4E markers are found it will increase the chance of 
            // getting a proper crc, even if it's bit shifted caused by corrupt data

            int Markerindex = FindMarker(ref mfmbuf, mfmbuf.Length, (sectorlength) + 4 * 16, ref Marker);
            // The number of bits shifted with regards to where the 4E padding should or expected to be
            if (Markerindex == -1)
            {
                tbreceived.Append("Marker not found. Can't continue.\r\n");
                return;
            }
            if (diskformat == DiskFormat.amigados)
                bitshifted = Markerindex - 8736;
            else if (diskformat == DiskFormat.diskspare)
                bitshifted = Markerindex - 8336;

            tbreceived.Append("Bitshift: " + bitshifted + "\r\n");

            //mfmSelectionEnd = mfmSelectionEnd - bitshifted;

            // Copy mfm data to aligned array
            byte[] mfmaligned = new byte[mfmbuf.Length + 32];
            for (i = 0; i < mfmbuf.Length; i++)
                mfmaligned[i] = mfmbuf[i];

            for (i = mfmAlignedStart; i < mfmbuf.Length - bitshifted; i++)
                mfmaligned[i] = mfmbuf[i + bitshifted];

            byte[] data = new byte[(mfmaligned.Length) / 16 + 1];

            //byte[] combinations = new byte[100];
            int detectioncnt = 0;

            // int combs = (int)CombinationsUpDown.Value;

            stop = 0;

            int k, l, u, j, p, q;
            int combinations = 0;
            int NumberOfMfmBytes = ecSettings.MFMByteLength;
            int MaxIndex = 25;
            int iterations;

            combilimit = 1;

            for (j = 0; j < MaxIndex; j++)
            {
                combilimit++;

                iterations = combilimit;
                for (q = 0; q < NumberOfMfmBytes - 1; q++)
                    iterations *= combilimit;

                tbreceived.Append("Iterations: " + iterations + "\r\n");
                Application.DoEvents();
                for (u = 0; u < iterations; u++)
                {
                    for (k = 0; k < NumberOfMfmBytes; k++)
                    {
                        if (stop == 1) break;
                        for (l = 0; l < 8; l++)
                        {
                            mfmaligned[mfmAlignedStart + l + (k * 8)] = mfmpreset.MFMPC[combi[k], l];
                        }
                    }

                    // Check result
                    int datacrcchk = 0;
                    byte[] checksum;
                    byte[] datachecksum;

                    if (diskformat == DiskFormat.amigados)
                    {
                        datachecksum = amigamfmdecodebytes(mfmaligned, (56 * 8), 4 * 16); // At uint 6
                        data = amigamfmdecodebytes(mfmaligned, (64 * 8), 512 * 16);
                        checksum = amigachecksum(mfmaligned, (64 * 8), 512 * 16); // Get header checksum from sector header

                        // Do the data checksum check:
                        if (datachecksum.SequenceEqual(checksum)) // checksum is changed everytime amigamfmdecode() is called
                        {
                            datacrcchk = 1;
                        }
                    }
                    else if (diskformat == DiskFormat.diskspare)
                    {
                        byte[] dsdatachecksum = new byte[4];

                        // Get track sector and checksum within one uint32: 0xTTSSCCCC
                        byte[] dec1 = amigamfmdecodebytes(mfmaligned, 8 * 8, 4 * 16); //At uint 0

                        //tracknr = data[0];
                        //sectornr = data[1];
                        dsdatachecksum[0] = dec1[2];
                        dsdatachecksum[1] = dec1[3];
                        dsdatachecksum[2] = 0;
                        dsdatachecksum[3] = 0;

                        uint dchecksum = 0;
                        uint offset;
                        byte[] tmp;

                        offset = 4;

                        //checksum = 0;
                        for (p = 0; p < 512; p += 4)
                        {
                            tmp = amigamfmdecodebytes(mfmaligned, (int)p * 16 + 16 * 8, 4 * 16);
                            data[p] = tmp[0];
                            data[p + 1] = tmp[1];
                            data[p + 2] = tmp[2];
                            data[p + 3] = tmp[3];
                        }

                        dchecksum = (uint)((mfm2ushort(mfmaligned, 8 * 16)) & 0x7FFF);
                        ushort tmp1 = 0;
                        offset = 9;
                        for (p = (int)offset; p < 520; p++)
                        {
                            tmp1 = mfm2ushort(mfmaligned, (int)(p * 16));
                            dchecksum ^= (uint)(tmp1 & 0xffff);
                        }
                        byte[] savechecksum = new byte[4];
                        savechecksum[0] = (byte)(dchecksum >> 8);
                        savechecksum[1] = (byte)(dchecksum & 0xFF);
                        savechecksum[2] = 0;
                        savechecksum[3] = 0;

                        // Do the data checksum check:
                        if (dsdatachecksum.SequenceEqual(savechecksum)) // checksum is changed everytime amigamfmdecode() is called
                            datacrcchk = 1;
                        else datacrcchk = 0;
                    }


                    if (datacrcchk == 1)
                    {
                        detectioncnt++;
                        tbreceived.Append("CRC ok! iteration: " + combinations + "\r\n");
                        printarray(combi, NumberOfMfmBytes);
                        for (i = 0; i < 512; i++)
                        {
                            tbreceived.Append(data[i].ToString("X2") + " ");
                            if (i % 16 == 15) tbreceived.Append("\r\n");
                            if (i == mfmAlignedStart / 16 || i == mfmAlignedEnd / 16) tbreceived.Append("--");
                            //dat[offset + i] = data[offset + i];
                        }
                        //tbreceived.Append("\r\n\r\nc6_max:" + c6_max + " c8_max:" + c8_max + "\r\n");
                        tbreceived.Append("Time: " + sw.ElapsedMilliseconds + "ms\r\n");
                        //Save recovered sector to disk array
                        int diskoffset = sectordata2[indexS1].track * sectorspertrack * 512 + sectordata2[indexS1].sector * 512;
                        sectormap.sectorok[sectordata2[indexS1].track, sectordata2[indexS1].sector] = SectorMapStatus.ErrorCorrected; // Error corrected (shows up as 'c')
                        for (i = 0; i < bytespersector; i++)
                        {
                            disk[i + diskoffset] = data[i];
                        }
                        sectormap.RefreshSectorMap();
                        tbreceived.Append("\r\n");
                        Application.DoEvents();
                        //return q;
                        stop = 1;
                        break;
                    }
                    if (stop == 1) break;
                    Application.DoEvents();
                    combi[0]++;
                    for (k = 0; k < NumberOfMfmBytes; k++)
                    {
                        if (combi[k] >= combilimit)
                        {
                            combi[k] = 0;
                            combi[k + 1]++;
                        }
                    }
                    combinations++;
                }
                if (stop == 1) break;
            }
            tbreceived.Append("Combinations:" + combinations + "\r\n");

            return;
        }

        // Some bad sectors only contain a cluster of bad periods
        // These can easily be reprocessed brute force wise if they
        // only cover like 20 periods. 
        public void ProcessClusterAmiga(ECSettings ecSettings)
        {
            int i;

            byte[] Marker = new byte[0];
            int periodshift = 0;
            byte[] crcinsectordatadecoded = new byte[100];

            int bitshifted = 0;
            byte[] lasttwosectorbytes = new byte[100];
            int periodSelectionStart, mfmSelectionStart = 0;
            int periodSelectionEnd, mfmSelectionEnd = 0;
            int bytestart, byteend;
            int mfmAlignedStart, mfmAlignedEnd;
            int indexS1 = ecSettings.indexS1;
            int threadid = ecSettings.threadid;

            if (diskformat == DiskFormat.amigados)
            {
                Marker = FDDProcessing.AMIGAMARKER;
                periodshift = 16;
            }
            else
            if (diskformat == DiskFormat.diskspare)
            {
                Marker = FDDProcessing.AMIGADSMARKER;
                periodshift = 8;
            }

            if (Marker.Length == 0)
            {
                tbreceived.Append("No amiga format found!\r\n");
                return;
            }
            //StopWatch sw = new StopWatch;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();

            // User selected part to be brute forced:
            periodSelectionStart = ecSettings.periodSelectionStart;
            periodSelectionEnd = ecSettings.periodSelectionEnd;

            // Stop if selection is too large, taking too long.
            if (periodSelectionEnd - periodSelectionStart > 50)
            {
                tbreceived.Append("Selection too large, please make it smaller, 50 max.\r\n");
                return;
            }

            // Copy mfm data from mfms
            int sectorlength = sectordata2[indexS1].sectorlength;


            byte[] mfmbuf = mfms[sectordata2[indexS1].threadid].SubArray(sectordata2[indexS1].MarkerPositions, (sectorlength + 100) * 16);
            byte[] bytebuf = new byte[sectorlength + 6];

            int cntperiods = 0;
            // Find where in the mfm data the periodSelectionStart is
            // Aparently the amiga data is shifted 16 periods
            for (i = 0; i < (sectorlength + 6) * 16; i++)
            {
                if (mfmbuf[i] == 1)
                    cntperiods++;
                if (cntperiods == periodSelectionStart + periodshift) mfmSelectionStart = i;
                if (cntperiods == periodSelectionEnd + periodshift)
                {
                    mfmSelectionEnd = i;
                    break;
                }
            }

            tbreceived.Append("Selection: period start:" + periodSelectionStart + " period end: " + periodSelectionEnd + "\r\n");
            tbreceived.Append("mfm start: " + mfmSelectionStart + " mfm end:" + mfmSelectionEnd + "\r\n");

            bytestart = mfmSelectionStart / 16;
            byteend = mfmSelectionEnd / 16;

            mfmAlignedStart = bytestart * 16;
            mfmAlignedEnd = (byteend + 1) * 16;

            tbreceived.Append("bytestart: " + bytestart + " byte end: " + byteend + "\r\n");
            tbreceived.Append("mfmAlignedstart: " + mfmAlignedStart + " mfmAlignedEnd: " + mfmAlignedEnd + "\r\n");

            // Find 4E right after the crc bytes at the end of the sector
            // 4E bytes are padding bytes between header and data. 
            // When the 4E markers are found it will increase the chance of 
            // getting a proper crc, even if it's bit shifted caused by corrupt data

            int Markerindex = FindMarker(ref mfmbuf, mfmbuf.Length, (sectorlength) + 4 * 16, ref Marker);
            // The number of bits shifted with regards to where the 4E padding should or expected to be
            if (Markerindex == -1)
            {
                tbreceived.Append("Marker not found. Can't continue.\r\n");
                return;
            }
            if (diskformat == DiskFormat.amigados)
                bitshifted = Markerindex - 8736;
            else if (diskformat == DiskFormat.diskspare)
                bitshifted = Markerindex - 8336;

            tbreceived.Append("Bitshift: " + bitshifted + "\r\n");

            mfmSelectionEnd = mfmSelectionEnd - bitshifted;

            // Copy mfm data to aligned array
            byte[] mfmaligned = new byte[mfmbuf.Length + 32];
            for (i = 0; i < mfmbuf.Length; i++)
                mfmaligned[i] = mfmbuf[i];

            for (i = mfmSelectionEnd; i < mfmbuf.Length - bitshifted; i++)
                mfmaligned[i] = mfmbuf[i + bitshifted];


            byte[] data = new byte[(mfmaligned.Length) / 16 + 1];

            //for (i = 0; i < (mfmaligned.Length/16); i++)
            //    data[i] = processing.MFMBits2BINbyte(ref mfmaligned, (i * 16));

            // Skip to next '1'
            for (i = mfmSelectionStart; i < mfmSelectionStart + 4; i++)
            {
                if (mfmaligned[i] == 1) break;
            }

            mfmSelectionStart = i;

            // Skip to next '1' for the selection end
            for (i = mfmSelectionEnd; i < mfmSelectionEnd + 4; i++)
            {
                if (mfmaligned[i] == 1) break;
            }
            // then back up one, we want to end with a '0'
            mfmSelectionEnd = i - 1;

            tbreceived.Append("mfmSelectionStart:" + mfmSelectionStart + " mfmselectionendAligned: " + (mfmSelectionEnd + bitshifted) + "\r\n");

            int j, p, q;
            int mfmcorrectedindex;
            byte[] combinations = new byte[100];
            int detectioncnt = 0;
            int numberofitems = periodSelectionEnd - periodSelectionStart;
            int numberofmfmitems = mfmSelectionEnd - mfmSelectionStart;
            ulong c6 = 0;
            ulong c8 = 0;
            int c6_max;
            int c8_max;
            int numberofitmssq = 1 << numberofitems;
            uint c6cnt = 0;
            uint c8cnt = 0;
            int combs = ecSettings.combinations;

            mfmcorrectedindex = 0;
            stop = 0;
            // Brute force with weighing of 4/6/8us
            for (c8_max = ecSettings.C8Start; c8_max < numberofitems; c8_max++)
            {
                tbreceived.Append("c8_max: " + c8_max + "\r\n");
                for (c6_max = ecSettings.C6Start; c6_max < numberofitems; c6_max++)
                {
                    tbreceived.Append("c6_max: " + c6_max + "\r\n");
                    for (p = 0; p < combs; p++)
                    {
                        if (p % 25000 == 24999)
                        {
                            tbreceived.Append("p: " + p + "\r\n");
                            Application.DoEvents();
                            progresses[mfmsindex] = p;
                        }
                        if (stop == 1) break;

                        mfmcorrectedindex = 0;
                        for (j = 0; j < numberofmfmitems; j++)
                        {

                            switch (combinations[j])
                            {
                                case 0:
                                    if (mfmcorrectedindex + 2 >= numberofmfmitems + 1) break;
                                    mfmaligned[mfmSelectionStart + mfmcorrectedindex++] = 1;
                                    mfmaligned[mfmSelectionStart + mfmcorrectedindex++] = 0;
                                    break;
                                case 1:
                                    if (mfmcorrectedindex + 3 >= numberofmfmitems + 1) break;
                                    mfmaligned[mfmSelectionStart + mfmcorrectedindex++] = 1;
                                    mfmaligned[mfmSelectionStart + mfmcorrectedindex++] = 0;
                                    mfmaligned[mfmSelectionStart + mfmcorrectedindex++] = 0;
                                    break;
                                case 2:
                                    if (mfmcorrectedindex + 4 >= numberofmfmitems + 1) break;
                                    mfmaligned[mfmSelectionStart + mfmcorrectedindex++] = 1;
                                    mfmaligned[mfmSelectionStart + mfmcorrectedindex++] = 0;
                                    mfmaligned[mfmSelectionStart + mfmcorrectedindex++] = 0;
                                    mfmaligned[mfmSelectionStart + mfmcorrectedindex++] = 0;
                                    break;
                            }
                            if (mfmcorrectedindex > numberofmfmitems) break;
                        }

                        int datacrcchk = 0;
                        byte[] checksum;
                        byte[] datachecksum;

                        if (diskformat == DiskFormat.amigados)
                        {
                            datachecksum = amigamfmdecodebytes(mfmaligned, (56 * 8), 4 * 16); // At uint 6
                            data = amigamfmdecodebytes(mfmaligned, (64 * 8), 512 * 16);
                            checksum = amigachecksum(mfmaligned, (64 * 8), 512 * 16); // Get header checksum from sector header

                            // Do the data checksum check:
                            if (datachecksum.SequenceEqual(checksum)) // checksum is changed everytime amigamfmdecode() is called
                            {
                                datacrcchk = 1;
                            }
                        }
                        else if (diskformat == DiskFormat.diskspare)
                        {
                            byte[] dsdatachecksum = new byte[4];

                            // Get track sector and checksum within one uint32: 0xTTSSCCCC
                            byte[] dec1 = amigamfmdecodebytes(mfmaligned, 8 * 8, 4 * 16); //At uint 0

                            //tracknr = data[0];
                            //sectornr = data[1];
                            dsdatachecksum[0] = dec1[2];
                            dsdatachecksum[1] = dec1[3];
                            dsdatachecksum[2] = 0;
                            dsdatachecksum[3] = 0;

                            uint dchecksum = 0;
                            uint offset;
                            byte[] tmp;

                            offset = 4;

                            //checksum = 0;
                            for (j = 0; j < 512; j += 4)
                            {
                                tmp = amigamfmdecodebytes(mfmaligned, (int)j * 16 + 16 * 8, 4 * 16);
                                data[j] = tmp[0];
                                data[j + 1] = tmp[1];
                                data[j + 2] = tmp[2];
                                data[j + 3] = tmp[3];
                            }

                            dchecksum = (uint)((mfm2ushort(mfmaligned, 8 * 16)) & 0x7FFF);
                            ushort tmp1 = 0;
                            offset = 9;
                            for (j = (int)offset; j < 520; j++)
                            {
                                tmp1 = mfm2ushort(mfmaligned, (int)(j * 16));
                                dchecksum ^= (uint)(tmp1 & 0xffff);
                            }
                            byte[] savechecksum = new byte[4];
                            savechecksum[0] = (byte)(dchecksum >> 8);
                            savechecksum[1] = (byte)(dchecksum & 0xFF);
                            savechecksum[2] = 0;
                            savechecksum[3] = 0;

                            // Do the data checksum check:
                            if (dsdatachecksum.SequenceEqual(savechecksum)) // checksum is changed everytime amigamfmdecode() is called
                                datacrcchk = 1;
                            else datacrcchk = 0;
                        }


                        if (datacrcchk == 1)
                        {
                            detectioncnt++;
                            tbreceived.Append("FindBruteForce: Correction found! q = " + p + "Count: " + detectioncnt + "\r\nData: ");
                            for (i = 0; i < 512; i++)
                            {
                                tbreceived.Append(data[i].ToString("X2") + " ");
                                if (i % 16 == 15) tbreceived.Append("\r\n");
                                if (i == mfmSelectionStart / 16 || i == mfmSelectionEnd / 16) tbreceived.Append("--");
                                //dat[offset + i] = data[offset + i];
                            }
                            tbreceived.Append("\r\n\r\nc6_max:" + c6_max + " c8_max:" + c8_max + "\r\n");
                            tbreceived.Append("Time: " + sw.ElapsedMilliseconds + "ms\r\n");
                            //Save recovered sector to disk array
                            int diskoffset = sectordata2[indexS1].track * sectorspertrack * 512 + sectordata2[indexS1].sector * 512;
                            sectormap.sectorok[sectordata2[indexS1].track, sectordata2[indexS1].sector] = SectorMapStatus.ErrorCorrected; // Error corrected (shows up as 'c')
                            for (i = 0; i < bytespersector; i++)
                            {
                                disk[i + diskoffset] = data[i];
                            }
                            sectormap.RefreshSectorMap();
                            tbreceived.Append("\r\n");
                            Application.DoEvents();
                            //return q;
                            stop = 1;
                            break;
                        }
                        mfmcorrectedindex = 0;

                        c6 = 0;
                        while (c6 == 0)
                        {
                            c6 = GetBitPattern(c6cnt++, c6_max);
                        }
                        if (((c6 >> numberofitems) & 1) == 1)
                        {
                            c6cnt = 0;
                            if (c8_max == 0)
                                break;
                            c8 = 0;
                            while (c8 == 0) c8 = GetBitPattern(c8cnt++, c8_max);

                            if (((c8 >> numberofitems) & 1) == 1)
                            {
                                c8cnt = 0;
                                break;
                            }

                        }
                        //tbreceived.Append(j + " ");
                        for (q = 0; q < numberofitems; q++)
                        {
                            if (((c8 >> q) & 1) == 1) combinations[q] = 2;
                            else
                            if (((c6 >> q) & 1) == 1) combinations[q] = 1;
                            else combinations[q] = 0;
                            //tbreceived.Append(combinations[q].ToString());
                        }
                        Application.DoEvents();

                    } // end loop through all combinations
                    if (stop == 1) break;
                } // End loop c6_max
                if (stop == 1) break;
            } // End loop c8_max
            tbreceived.Append("Done.\r\n");
            return;

        }

        public ECResult ProcessRealignAmiga(ECSettings ecSettings)
        {
            int i;

            byte[] Marker = new byte[0];
            int periodshift = 0;
            byte[] crcinsectordatadecoded = new byte[100];

            int bitshifted = 0;
            byte[] lasttwosectorbytes = new byte[100];
            int periodSelectionStart, mfmSelectionStart = 0;
            int periodSelectionEnd, mfmSelectionEnd = 0;
            int bytestart, byteend;
            int mfmAlignedStart, mfmAlignedEnd;
            int indexS1 = ecSettings.indexS1;
            int threadid = ecSettings.threadid;


            if (diskformat == DiskFormat.amigados)
            {
                Marker = FDDProcessing.AMIGAMARKER;
                periodshift = 16;
            }
            else
            if (diskformat == DiskFormat.diskspare)
            {
                Marker = FDDProcessing.AMIGADSMARKER;
                periodshift = 8;
            }

            if (Marker.Length == 0)
            {
                tbreceived.Append("No amiga format found!\r\n");
                return null;
            }
            //StopWatch sw = new StopWatch;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();

            // User selected part to be brute forced:
            periodSelectionStart = ecSettings.periodSelectionStart;
            periodSelectionEnd = ecSettings.periodSelectionEnd;

            // Stop if selection is too large, taking too long.
            if (periodSelectionEnd - periodSelectionStart > 50)
            {
                tbreceived.Append("Selection too large, please make it smaller, 50 max.\r\n");
                return null;
            }

            // Copy mfm data from mfms
            int sectorlength = sectordata2[indexS1].sectorlength;
            int track = sectordata2[indexS1].track;
            int sector = sectordata2[indexS1].sector;

            //Get the mfm data from the large mfms array
            byte[] mfms1 = mfms[sectordata2[indexS1].threadid];
            int mfmsoffsetindex = sectordata2[indexS1].MarkerPositions;
            byte[] mfmbuf = mfms1.SubArray(mfmsoffsetindex, (sectorlength + 100) * 16);

            int cntperiods = 0;
            // Find where in the mfm data the periodSelectionStart is
            // Aparently the amiga data is shifted 16 periods
            for (i = 0; i < (sectorlength + 6) * 16; i++)
            {
                if (mfmbuf[i] == 1)
                    cntperiods++;
                if (cntperiods == periodSelectionStart + periodshift) mfmSelectionStart = i;
                if (cntperiods == periodSelectionEnd + periodshift)
                {
                    mfmSelectionEnd = i;
                    break;
                }
            }

            tbreceived.Append("Selection: period start:" + periodSelectionStart + " period end: " + periodSelectionEnd + "\r\n");
            tbreceived.Append("mfm start: " + mfmSelectionStart + " mfm end:" + mfmSelectionEnd + "\r\n");

            bytestart = mfmSelectionStart / 16;
            byteend = mfmSelectionEnd / 16;

            mfmAlignedStart = bytestart * 16;
            mfmAlignedEnd = (byteend + 1) * 16;

            tbreceived.Append("bytestart: " + bytestart + " byte end: " + byteend + "\r\n");
            tbreceived.Append("mfmAlignedstart: " + mfmAlignedStart + " mfmAlignedEnd: " + mfmAlignedEnd + "\r\n");

            // Find 4E right after the crc bytes at the end of the sector
            // 4E bytes are padding bytes between header and data. 
            // When the 4E markers are found it will increase the chance of 
            // getting a proper crc, even if it's bit shifted caused by corrupt data

            int Markerindex = FindMarker(ref mfmbuf, mfmbuf.Length, (sectorlength) + 4 * 16, ref Marker);
            // The number of bits shifted with regards to where the 4E padding should or expected to be
            if (Markerindex == -1)
            {
                tbreceived.Append("Marker not found. Can't continue.\r\n");
                return null;
            }
            if (diskformat == DiskFormat.amigados)
                bitshifted = Markerindex - 8736;
            else if (diskformat == DiskFormat.diskspare)
                bitshifted = Markerindex - 8336;

            tbreceived.Append("Bitshift: " + bitshifted + "\r\n");

            mfmSelectionEnd = mfmSelectionEnd - bitshifted;

            // Copy mfm data to aligned array
            byte[] mfmaligned = new byte[mfmbuf.Length + 32];
            for (i = 0; i < mfmbuf.Length; i++)
                mfmaligned[i] = mfmbuf[i];

            byte[] data = new byte[(mfmaligned.Length) / 16 + 1];

            //for (i = 0; i < (mfmaligned.Length/16); i++)
            //    data[i] = processing.MFMBits2BINbyte(ref mfmaligned, (i * 16));

            // Skip to next '1'
            for (i = mfmSelectionStart; i < mfmSelectionStart + 4; i++)
            {
                if (mfmaligned[i] == 1) break;
            }

            mfmSelectionStart = i;

            // Copy aligned end of the sectors
            int end = 0;
            if (bitshifted < 0)
                end = 0;
            else end = bitshifted;
            for (i = mfmSelectionStart + 8; i < mfmbuf.Length - end; i++)
            {
                mfmaligned[i] = mfmbuf[i + bitshifted];
                // Also copy the realigned data back to the main array for error correction map
                mfms1[mfmsoffsetindex + i] = mfmbuf[i + bitshifted];
            }

            // Skip to next '1' for the selection end
            for (i = mfmSelectionEnd; i < mfmSelectionEnd + 4; i++)
            {
                if (mfmaligned[i] == 1) break;
            }
            // then back up one, we want to end with a '0'
            mfmSelectionEnd = i - 1;

            tbreceived.Append("mfmSelectionStart:" + mfmSelectionStart + " mfmselectionendAligned: " + (mfmSelectionEnd + bitshifted) + "\r\n");
            int datacrcchk = 0;
            byte[] checksum;
            byte[] datachecksum;
            int j;
            byte[] combinations = new byte[100];
            int numberofitems = periodSelectionEnd - periodSelectionStart;
            int numberofmfmitems = mfmSelectionEnd - mfmSelectionStart;
            int numberofitmssq = 1 << numberofitems;
            stop = 0;
            // Brute force with weighing of 4/6/8us


            if (diskformat == DiskFormat.amigados)
            {
                datachecksum = amigamfmdecodebytes(mfmaligned, (56 * 8), 4 * 16); // At uint 6
                data = amigamfmdecodebytes(mfmaligned, (64 * 8), 512 * 16);
                checksum = amigachecksum(mfmaligned, (64 * 8), 512 * 16); // Get header checksum from sector header

                // Do the data checksum check:
                if (datachecksum.SequenceEqual(checksum)) // checksum is changed everytime amigamfmdecode() is called
                {
                    datacrcchk = 1;
                }
            }
            else if (diskformat == DiskFormat.diskspare)
            {
                byte[] dsdatachecksum = new byte[4];

                // Get track sector and checksum within one uint32: 0xTTSSCCCC
                byte[] dec1 = amigamfmdecodebytes(mfmaligned, 8 * 8, 4 * 16); //At uint 0

                dsdatachecksum[0] = dec1[2];
                dsdatachecksum[1] = dec1[3];
                dsdatachecksum[2] = 0;
                dsdatachecksum[3] = 0;

                uint dchecksum = 0;
                uint offset;
                byte[] tmp;

                offset = 4;

                //checksum = 0;
                for (j = 0; j < 512; j += 4)
                {
                    tmp = amigamfmdecodebytes(mfmaligned, (int)j * 16 + 16 * 8, 4 * 16);
                    data[j] = tmp[0];
                    data[j + 1] = tmp[1];
                    data[j + 2] = tmp[2];
                    data[j + 3] = tmp[3];
                }

                dchecksum = (uint)((mfm2ushort(mfmaligned, 8 * 16)) & 0x7FFF);
                ushort tmp1 = 0;
                offset = 9;
                for (j = (int)offset; j < 520; j++)
                {
                    tmp1 = mfm2ushort(mfmaligned, (int)(j * 16));
                    dchecksum ^= (uint)(tmp1 & 0xffff);
                }
                byte[] savechecksum = new byte[4];
                savechecksum[0] = (byte)(dchecksum >> 8);
                savechecksum[1] = (byte)(dchecksum & 0xFF);
                savechecksum[2] = 0;
                savechecksum[3] = 0;

                // Do the data checksum check:
                if (dsdatachecksum.SequenceEqual(savechecksum)) // checksum is changed everytime amigamfmdecode() is called
                    datacrcchk = 1;
                else datacrcchk = 0;
            }

            /*
            if (datacrcchk == 1)
            {
                for (i = 0; i < 512; i++)
                {
                    tbreceived.Append(data[i].ToString("X2") + " ");
                    if (i % 32 == 31) tbreceived.Append("\r\n");
                    if (i == mfmSelectionStart / 16 || i == mfmSelectionEnd / 16) tbreceived.Append("--");
                    //dat[offset + i] = data[offset + i];
                }

                tbreceived.Append("Time: " + sw.ElapsedMilliseconds + "ms\r\n");
                //Save recovered sector to disk array
                int diskoffset = processing.sectordata2[indexS1].track * processing.sectorspertrack * 512 + processing.sectordata2[indexS1].sector * 512;
                processing.sectormap.sectorok[processing.sectordata2[indexS1].track, processing.sectordata2[indexS1].sector] = 6; // Error corrected (shows up as 'c')
                for (i = 0; i < processing.bytespersector; i++)
                {
                    processing.disk[i + diskoffset] = data[i];
                }
                processing.sectormap.RefreshSectorMap();
                tbreceived.Append("\r\n");
                this.updateForm();

            }
            */
            int badsectorold = indexS1;
            int badsectorcnt2 = sectordata2.Count;
            //badsectorhash[badsectorcnt2];
            MFMData sectordata = new MFMData();

            //sectordata[threadid][badsectorcnt2].threadid = sectordata[threadid][badsectorold].threadid;
            sectordata.threadid = threadid;
            sectordata.MarkerPositions = sectordata2[badsectorold].MarkerPositions;
            sectordata.rxbufMarkerPositions = sectordata2[badsectorold].rxbufMarkerPositions;
            sectordata.mfmMarkerStatus = sectordata2[badsectorold].mfmMarkerStatus; // 2 = bad sector data
            sectordata.track = sectordata2[badsectorold].track;
            sectordata.sector = sectordata2[badsectorold].sector;
            sectordata.sectorlength = sectordata2[badsectorold].sectorlength;
            sectordata.crc = sectordata2[badsectorold].crc;
            sectordata.sectorbytes = data;

            sectordata2.TryAdd(sectordata2.Count, sectordata);
            //sectordata2[badsectorcnt2].sectorbytes = bytebuf;

            //return sectordata;
            ECResult result = new ECResult();

            result.index = sectordata2.Count - 1;
            result.sectordata = sectordata;

            return result;
        } // End ProcessRealignAmiga

        // Converts bits incoded in bytes to bytes
        // length is mfm length which is 16 bits per byte, and must be in multiples of 16
        // checksum is a global (not yet implemented => probably going to do it externally)
        // Each byte consists of 4 bits at index 0+offset and 4 bits at index + offset + length/2
        // The former are the odd bits, the latter the even bits. 
        public byte[] amigamfmdecodebytes(byte[] mfm, int offset, int length)
        {
            int i, j;
            byte[] output = new byte[length / 16];

            byte b;
            //byte m1, m2;

            if (mfm.Length > length + offset)
            {
                for (i = 0; i < length / 16; i++) // cycle through byte blocks
                {
                    b = 0;
                    for (j = 0; j < 8; j += 2) // cycle through mfm bits
                    {
                        //m1 = mfm[i * 8 + j + 1];
                        //m2 = mfm[i * 8 + j + (length / 2) + 1];
                        b = (byte)(b << 1);
                        b = (byte)(b | mfm[offset + i * 8 + j + 1]);
                        b = (byte)(b << 1);
                        b = (byte)(b | mfm[offset + i * 8 + j + (length / 2) + 1]);
                    }
                    output[i] = b;
                }
            }

            return output;
        }

        public byte[] amigamfmencodebytes(byte[] data, int offset, int length)
        {
            int i, j;
            byte[] output = new byte[length * 16];
            byte b = 0, previous;
            int cnt = 0;

            // Odd
            for (i = offset; i < offset + length; i++)
            {
                for (j = 0; j < 8; j += 2)
                {
                    previous = b;
                    b = (byte)(data[i] >> (7 - j) & 1);
                    if (b == 1)
                    {
                        output[cnt++] = 0;
                        output[cnt++] = 1;
                    }
                    else if (b == 0 && previous == 0)
                    {
                        output[cnt++] = 1;
                        output[cnt++] = 0;
                    }
                    else
                    {
                        output[cnt++] = 0;
                        output[cnt++] = 0;
                    }
                }

            }

            // even
            for (i = offset; i < length; i++)
            {
                for (j = 0; j < 8; j += 2)
                {
                    previous = b;
                    b = (byte)(data[i] >> ((7 - 1) - j) & 1);
                    if (b == 1)
                    {
                        output[cnt++] = 0;
                        output[cnt++] = 1;
                    }
                    else if (b == 0 && previous == 0)
                    {
                        output[cnt++] = 1;
                        output[cnt++] = 0;
                    }
                    else
                    {
                        output[cnt++] = 0;
                        output[cnt++] = 0;
                    }
                }
            }

            return output;
        }

        static bool isNotFour(byte n)
        {
            return n != 4;
        }


        // Decode Amiga MFM
        // length in bytes, dividable by 4, data should be 2x length
        // checksum is a global, so if you want to check the data, do so before calling
        // this function again or the checksum will be overwritten.
        public uint[] amigamfmdecode(ref uint[] data, int offset, int length)
        {
            int i, cnt;
            uint even, odd;
            uint[] outp = new uint[length / 4];

            checksum = 0;
            cnt = 0;
            for (i = offset; i < (offset + (length / 4)); i++)
            {
                even = data[i];
                odd = data[i + (length / 4)];
                checksum ^= even;
                checksum ^= odd;

                outp[cnt] = (odd & 0x55555555) | ((even & 0x55555555) << 1);
                cnt++;
            }
            checksum &= 0x55555555;
            return outp;
        }

        // Checksum on amiga is normally applied to mfm data
        // This routine uses the mfm data, splits even and odd, 
        // then outputs the checksum as 4 element byte array
        public byte[] amigachecksum(byte[] mfm, int offset, int length)
        {
            int i, j, k;
            byte e, o;
            int index;
            byte[] chk = new byte[4];
            int k1, i1;
            if (mfm.Length > length + offset)
            {
                // Each byte represented by 16 mfm bits stored as bytes of 0 or 1;
                for (i = 0; i < length / 16; i += 4) // cycle through 4*16 byte blocks
                {
                    i1 = offset + (i * 8);
                    for (k = 0; k < 4; k++) // mfm is normally stored as uint32, but we're processing bytes
                    {
                        k1 = k * 8 + i1;
                        o = 0;
                        e = 0;
                        for (j = 0; j < 8; j += 2) // cycle through mfm bits
                        {
                            //m1 = mfm[i * 8 + j + 1];
                            //m2 = mfm[i * 8 + j + (length / 2) + 1];
                            index = k1 + j + 1;
                            e = (byte)(e << 2);
                            e = (byte)(e | mfm[index]);
                            o = (byte)(o << 2);
                            o = (byte)(o | mfm[index + (length / 2)]);
                        }
                        chk[k] ^= e;
                        chk[k] ^= o;
                    }
                }
            }
            return chk;
        }

        public ushort mfm2ushort(byte[] mfm, int offset)
        {
            int i;
            ushort result = 0;
            if (mfm.Length > offset + 16)
                for (i = 0; i < 16; i++)
                {
                    result <<= 1;
                    result = (ushort)(result | mfm[offset + i]);
                }
            return result;
        }

    }
}
