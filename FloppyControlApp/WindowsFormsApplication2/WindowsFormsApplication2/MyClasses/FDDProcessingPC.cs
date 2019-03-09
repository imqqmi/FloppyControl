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
        private static readonly Object lockbadsector = new Object();
        public static byte[] A1MARKER = { 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1 };
        public Action GetProcSettingsCallback { get; set; }
        public int peak1 { get; set; }
        public int peak2 { get; set; }
        public int peak3 { get; set; }
        public int GoodSectorHeaderCount { get; set; }
        uint checksum;

        private void ProcessPCMFM2Sectordata(ProcSettings procsettings, int threadid)
        {
            lock (lockbadsector)
            {
                //bool writemfm = true;
                int badsectorcntthread = 0;
                int i;
                int previoustrack = 0xff;
                byte previousheadnr = 0xff;
                bool debuginfo = false;
                int limitstart, limitend;

                SHA256 mySHA256 = SHA256Managed.Create();
                int rxbufcnt, searchcnt, overflow = 0;
                int j;

                int markerpositionscntthread = 0;

                limitstart = procsettings.limittotrack;
                limitend = procsettings.limittosector;

                // Load good sector found info so we can append to it, then at the end

                byte[] A1markerbytes = A1MARKER;

                progressesstart[threadid] = 0;
                progressesend[threadid] = mfmlengths[threadid];
                ProcessStatus[threadid] = "Find MFM markers...";
                //if (i % 250000 == 249999) { progresses[threadid] = i; }

                int retries = 0;
                if (stop == 0)
                {
                    rxbufcnt = 0;
                    searchcnt = 0;
                    // Find markers

                    byte[] m = mfms[threadid];

                    // with multithreading, the counting of rxbuf is misaligned with mfms, because
                    // it doesn't start at the start of rxbuf. There's no way to know at this point
                    // how many 1's there are up to this point. Either do the calculation after processing
                    // then adjust the values or not use multithreading.

                    //m = mfms[threadid]; // speed up processing by removing one extra reference
                    int A1MarkerLength = A1markerbytes.Length;
                    int A1MarkerLengthMinusOne = A1markerbytes.Length - 1;
                    int mfmlength = mfmlengths[threadid];
                    if (indexrxbuf > rxbuf.Length) indexrxbuf = rxbuf.Length - 1;
                    for (i = 0; i < mfmlength; i++)
                    {
                        if (i % 250000 == 249999) { progresses[threadid] = i; if (stop == 1) break; }

                        for (j = 0; j < A1MarkerLength; j++)
                        {
                            if (m[i + j] == A1markerbytes[j]) searchcnt++;
                            else
                            {
                                searchcnt = 0;
                                break;
                            }
                            if (searchcnt == A1MarkerLengthMinusOne)
                            {
                                searchcnt = 0;
                                MFMData sectordata = new MFMData();

                                sectordata.MarkerPositions = i;
                                sectordata.rxbufMarkerPositions = rxbufcnt;
                                sectordata.processed = false;
                                sectordata.threadid = threadid;
                                retries = 0;

                                while (!sectordata2.TryAdd(sectordata2.Count, sectordata) || retries > 100)
                                {
                                    if (debuglevel > 9)
                                        tbreceived.Append("Failed to add to Sectordata dictionary " + markerpositionscntthread + "\r\n");
                                    retries++;
                                }
                                markerpositionscntthread++;

                                /*
                                if (!sectordata2.TryAdd(sectordata2.Count, sectordata))
                                {
                                    tbreceived.Append("Failed to add to Sectordata dictionary " + markerpositionscntthread + "\r\n");
                                }
                                else markerpositionscntthread++;
                                */
                            }
                        }
                        if (mfms[threadid][i] == 1) // counting 1's matches the number of bytes in rxbuf + start offset
                            rxbufcnt++;
                        if( rxbuf.Length > rxbufcnt )
                            while (rxbuf[rxbufcnt] < 4 && rxbufcnt < indexrxbuf-1) rxbufcnt++;
                    }

                    //textBoxReceived.AppendText(periodhex.ToString());
                    //tbreceived.Append
                    //tbreceived.Append(relativetime().ToString() + "ms finding markers second pass.\r\n");
                    //tbreceived.Append("mfmlength: " + mfmlength + " markerpositionscnt: " + markerpositionscntthread + "\r\n");
                    /*
                    if (writemfm == true)
                    {
                        path = subpath + @"\" + outputfilename.Text + @"\";
                        bool exists = System.IO.Directory.Exists(path);

                        if (!exists)
                            System.IO.Directory.CreateDirectory(path);


                        writer = new BinaryWriter(new FileStream(path + @"\" + outputfilename.Text + ".mfm", FileMode.Create));

                        for (i = 0; i < mfmlength; i++)
                            writer.Write((byte)(mfms[threadid][i] + 48));

                        if (writer != null)
                        {
                            writer.Flush();
                            writer.Close();
                            writer.Dispose();
                        }
                    }
                    */

                    if (sectordata2.Count > 0 && diskformat == DiskFormat.unknown) // if markers are found and the diskformat has not been set previously, assume PC DD
                        diskformat = DiskFormat.pcdd;

                    tbreceived.Append(" Markers: " + markerpositionscntthread + " ");
                    int markerindex;

                    //StringBuilder tbtrackinfo = new StringBuilder();

                    byte[] bytebuf = new byte[5000];
                    byte[] sectorbuf = new byte[2050];
                    byte[] crctemp = new byte[6];
                    byte sectornr = 0xff, tracknr = 0xff, headnr = 0xff;
                    ushort headercrc, datacrc, headercrcchk = 0xFFFF, datacrcchk = 0xFFFF; //headercrc is from the captured data, the chk is calculated from the data.
                    int track = -1;
                    int bytespersectorthread;

                    GoodSectorHeaderCount = 0;
                    progressesstart[threadid] = 0;
                    progressesend[threadid] = sectordata2.Count;
                    ProcessStatus[threadid] = "Converting MFM to sectors...";
                    int prevmarkerindex = 0;
                    for (markerindex = 0; markerindex < sectordata2.Count; markerindex++)
                    {
                        MFMData sectordatathread = sectordata2[markerindex];
                        if (sectordatathread.processed == true || sectordatathread.threadid != threadid) continue; // skip if already processed
                        sectordatathread.processed = true; // mark as processed
                                                           // was the marker placed by the correct thread? If not, continue to next marker
                        if (markerindex % 250 == 249) { progresses[threadid] = markerindex; if (stop == 1) break; }
                        Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
                        sectornr = 0xff;
                        tracknr = 0xff;
                        headnr = 0xff;
                        datacrc = 0;

                        headercrcchk = 0xFFFF;
                        datacrcchk = 0xFFFF;

                        // First find the IDAM, 10 bytes

                        for (i = 0; i < 10; i++)
                        {
                            bytebuf[i] = MFMBits2BINbyte(ref mfms[threadid], sectordatathread.MarkerPositions + (i * 16));
                            if (debuginfo) tbreceived.Append(bytebuf[i].ToString("X2"));
                        }

                        int offset = 0;

                        if (bytebuf[3] == 0xFE)
                        {
                            if (debuginfo) tbreceived.Append(" IDAM");

                            tracknr = bytebuf[4];
                            headnr = bytebuf[5];
                            sectornr = (byte)(bytebuf[6] - 1);
                            bytespersectorthread = 128 << bytebuf[7];
                            if (bytespersectorthread > 1024)
                            {
                                tbreceived.Append("Error: T" + (tracknr * 2 + headnr).ToString("d3") + " S" + sectornr + " size too large: " + bytespersectorthread + "\r\n");

                                if (procsettings.IgnoreHeaderError == true)
                                {
                                    bytespersectorthread = 512;
                                }
                                else
                                    continue; // skip to next sector
                            }
                            headercrc = (ushort)((bytebuf[8] << 8) | bytebuf[9]);

                            if (debuginfo) tbreceived.Append("\r\n MFM:");
                            for (i = 0; i < crctemp.Length; i++)
                            {
                                if (debuginfo) tbreceived.Append(crctemp[i].ToString("X2"));
                            }

                            // Check header crc
                            // If headercrcchk == 0000, data integrety is good.
                            // The CRC is calculated from the A1A1A1FE header to and including the CRC16 code.
                            headercrcchk = crc.ComputeChecksum(bytebuf.SubArray(0, 10));

                            track = (tracknr * 2) + headnr;

                            if (headercrcchk == 0)
                            {
                                GoodSectorHeaderCount++;
                                sectordatathread.mfmMarkerStatus = SectorMapStatus.AmigaCrcOk;
                                sectordatathread.sector = sectornr;
                                sectordatathread.track = track;
                                sectordatathread.MarkerType = MarkerType.header;

                            }
                            else
                            {
                                sectordatathread.mfmMarkerStatus = SectorMapStatus.AmigaHeadOkDataBad;
                            }

                            if (headercrcchk == 0)
                            {
                                sectormap.sectorokLatestScan[track, sectornr]++;
                            }


                            // If the headerchecksum is non zero, there's an error in the header. 
                            // There's code further on to guess the sector and track based on surrounding
                            // sectors with correct header, but to speed things up I'll try continueing 
                            // Checking the IgnoreHeaderErrorCheckBox skips this part and does decode the sector data

                            if (headercrcchk != 0 && procsettings.IgnoreHeaderError == false)
                            {
                                continue;
                            }

                            //Get sector data and checksum
                            // First find the DAM

                            //*********************
                            // Potential problem if marker for sector data skips to another marker unintentionally
                            // Should test if the offset isn't impossibly high
                            prevmarkerindex = markerindex; //Go to next marker

                            // Find the next marker which should be the sector data
                            // Make sure the correct threadid is used since other threads also add to the
                            // dictionary. They are sequential though so we only need to search from markerindex
                            /*
                            try
                            {
                                sectordatathread = sectordata2[markerindex];

                            }
                            catch (Exception e)
                            {
                                tbreceived.Append("Error: could not find sectordata: "+markerindex+"\r\n");
                                continue;
                            }
                            */

                            try
                            {
                                int q;
                                for (q = markerindex + 1; q < sectordata2.Count; q++)
                                {
                                    if (sectordata2[q].threadid == threadid)
                                    {
                                        markerindex = q;
                                        sectordatathread = sectordata2[q];
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                tbreceived.Append("Error: could not find sectordata: " + markerindex + "\r\n");
                                continue;
                            }

                            if (debuginfo) tbreceived.Append("\r\nT" + track + " S" + sectornr + " Sector data:\r\n");

                            //int bytespersectortemp = bytespersectorthread;
                            //if (track == 0) bytespersectorthread = 512;

                            for (i = 0; i < bytespersectorthread + 16; i++)
                            {
                                bytebuf[i] = MFMBits2BINbyte(ref mfms[threadid], sectordatathread.MarkerPositions + (i * 16));
                                if (debuginfo)
                                {
                                    tbreceived.Append(bytebuf[i].ToString("X2"));
                                    if (i == 517) tbreceived.Append("\r\n");
                                }

                            }
                            if (debuginfo) tbreceived.Append("\r\n\r\n");
                            if (bytebuf[3] == 0xFB || bytebuf[3] == 0xF8)
                            {
                                datacrc = (ushort)((bytebuf[bytespersectorthread + 4] << 8) | bytebuf[bytespersectorthread + 5]);

                                datacrcchk = crc.ComputeChecksum(bytebuf.SubArray(0, bytespersectorthread + 6));
                                if (datacrcchk != 0)
                                {
                                    datacrcchk = crc.ComputeChecksum(bytebuf.SubArray(0, 512 + 6));
                                }
                                if (diskformat == DiskFormat.pc2m && track == 0)
                                    sectorbuf = bytebuf.SubArray(4, 514);
                                else sectorbuf = bytebuf.SubArray(4, bytespersectorthread + 2);
                                //sectorbuf = bytebuf.SubArray(4, bytespersectorthread);
                            }
                            else // if marker of the sector isn't good, backtrack one marker and continue
                            {
                                markerindex = prevmarkerindex;
                                continue;
                            }
                        }
                        else if (procsettings.IgnoreHeaderError)
                        {
                            headercrcchk = 1; // force ignore header

                            bytespersectorthread = 512;
                            for (i = 0; i < bytespersectorthread + 16; i++)
                            {
                                bytebuf[i] = MFMBits2BINbyte(ref mfms[threadid], sectordatathread.MarkerPositions + (i * 16));
                                if (debuginfo)
                                {
                                    tbreceived.Append(bytebuf[i].ToString("X2"));
                                    if (i == 517) tbreceived.Append("\r\n");
                                }

                            }
                            if (debuginfo) tbreceived.Append("\r\n\r\n");
                            if (bytebuf[3] == 0xFB || bytebuf[3] == 0xF8)
                            {
                                datacrc = (ushort)((bytebuf[bytespersectorthread + 4] << 8) | bytebuf[bytespersectorthread + 5]);

                                datacrcchk = crc.ComputeChecksum(bytebuf.SubArray(0, bytespersectorthread + 6));
                                if (datacrcchk != 0)
                                {
                                    datacrcchk = crc.ComputeChecksum(bytebuf.SubArray(0, 512 + 6));
                                }

                                sectorbuf = bytebuf.SubArray(4, bytespersectorthread + 2);
                                //sectorbuf = bytebuf.SubArray(4, bytespersectorthread);
                            }
                            if (datacrcchk == 0x00 && sectorbuf.Length > 500)
                            {
                                track = (previoustrack * 2) + previousheadnr;
                                headnr = (byte)previousheadnr;
                                if (track < 200)
                                {
                                    for (i = 0; i < 9; i++)
                                    {
                                        if (sectormap.sectorok[track, i] == SectorMapStatus.empty)
                                            break;
                                    }

                                    // Now assuming the sectornr = i, and we're using previoustrack because
                                    // the tracknr info in the header may not be correct
                                    sectornr = (byte)i;
                                    if (sectormap.sectorok[track, sectornr] == 0 || sectormap.sectorok[track, i] == SectorMapStatus.HeadOkDataBad) // do not overwrite any existing good data
                                    {
                                        sectormap.sectorok[track, sectornr] = SectorMapStatus.DuplicatesFound; // Header is not CRC pass, attempt to reconstuct
                                                                                                               //offset = (track * sectorspertrack * bytespersectorthread * 2) + (previousheadnr * sectorspertrack * bytespersectorthread) + (i * bytespersectorthread);

                                        offset = (previoustrack * sectorspertrack * bytespersectorthread * 2)
                                            + (headnr * sectorspertrack * bytespersectorthread)
                                            + (sectornr * bytespersectorthread);
                                        for (i = 0; i < bytespersectorthread; i++)
                                        {
                                            //if (disk[i + offset] == 0)
                                            disk[i + offset] = sectorbuf[i];
                                        }
                                        //stop = 1;
                                    }
                                }
                            }
                            continue;
                        }
                        else continue; // If no 0xFE is found

                        if (diskformat == DiskFormat.pcdd) // DD
                            sectorspertrack = 9;
                        else if (diskformat == DiskFormat.pchd) // HD
                            sectorspertrack = 18;
                        else if (diskformat == DiskFormat.pc2m) //2M
                            sectorspertrack = 11;

                        //If the checksum is correct and sector and track numbers within range and no sector data has already been captured

                        if (sectorbuf.Length > 500)
                            if (headercrcchk == 0x00)
                                if (datacrcchk == 0x00 && sectornr >= 0 && sectornr < 18 && headnr < 3 && tracknr >= 0 && tracknr < 82 && sectormap.sectorok[track, sectornr] != SectorMapStatus.CrcOk)
                                //if (datacrcchk == 0x00 && sectornr >= 0 && sectornr < 18 && headnr < 3 && tracknr >= 0 && tracknr < 82)
                                {
                                    // Determine diskformat
                                    if (sectornr > 8 && diskformat != DiskFormat.pc2m) // from DD to HD
                                    {
                                        diskformat = DiskFormat.pchd; // Assume HD
                                        sectorspertrack = 18;
                                    }

                                    if (tracknr == 0 && sectornr == 0 && diskformat == DiskFormat.pchd) // From HD to 2M, reset sectormap.sectorok
                                    {
                                        if (sectorbuf[401] == '2' && sectorbuf[402] == 'M') // Detect 2M format
                                        {
                                            diskformat = DiskFormat.pc2m; // Assume 2M
                                            sectorspertrack = 11;
                                            //bytespersectorthread = 1024;
                                            // When we're in the process and only detecting 2M after many other sectors were processed,
                                            // the previous sectors were processed with assumption diskformat = HD. We need to invalidate all found sectors
                                            // except T000
                                            for (i = 0; i < 200; i++)
                                            {
                                                for (j = 0; j < 25; j++)
                                                {
                                                    sectormap.sectorok[i, j] = SectorMapStatus.empty;
                                                }
                                            }
                                        }
                                    }
                                    if (procsettings.UseErrorCorrection)
                                    {
                                        //Create hash
                                        sectorbuf[bytespersectorthread] = (byte)track;
                                        sectorbuf[bytespersectorthread + 1] = sectornr;
                                        byte[] secthash = mySHA256.ComputeHash(sectorbuf);

                                        // Check if there's a duplicate
                                        int isunique = -1;
                                        for (i = 0; i < sectordata2.Count; i++)
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
                                            byte[] b = new byte[bytespersectorthread + 16];

                                            for (i = 0; i < bytespersectorthread + 16; i++)
                                            {
                                                b[i] = bytebuf[i];
                                            }

                                            //lock (lockbadsector)
                                            {
                                                //int badsectorcnt2 = badsectorcnt;
                                                //badsectorcnt++;
                                                badsectorhash[markerindex] = secthash;
                                                if (threadid != sectordatathread.threadid)
                                                    tbreceived.Append("threadid mismatch!\r\n");
                                                //sectordatathread.threadid = threadid;
                                                sectordatathread.mfmMarkerStatus = SectorMapStatus.CrcOk; // 1 = good sector data
                                                sectordatathread.track = track;
                                                sectordatathread.sector = sectornr;
                                                sectordatathread.sectorlength = bytespersectorthread;
                                                sectordatathread.crc = (int)datacrc;
                                                sectordatathread.sectorbytes = b;
                                                sectordatathread.MarkerType = MarkerType.data;
                                                sectordata2[prevmarkerindex].DataIndex = markerindex;
                                            }
                                        }
                                    }

                                    sectormap.sectorok[track, sectornr] = SectorMapStatus.CrcOk; // Sector is CRC pass
                                                                                                 // T 0 S0 H0 = 0x0000
                                                                                                 // T 1 S0 H0 = 0x2400
                                                                                                 // T 1 S0 H1 = 
                                    offset = (tracknr * sectorspertrack * bytespersectorthread * 2)
                                        + (headnr * sectorspertrack * bytespersectorthread)
                                        + (sectornr * bytespersectorthread);
                                    //offset2 = (tracks * sectorspertrack * bytespersector) + (sectornr * bytespersector);


                                    //string method = "";

                                    FoundGoodSectorInfo.Append("T" + tracknr.ToString("D3") + " S" + sectornr + " crc:" +
                                        datacrcchk.ToString("X4") + " rxbufindex:" + sectordatathread.rxbufMarkerPositions + " Method: ");


                                    if (procsettings.processingtype == ProcessingType.aufit)
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

                                    //if (offset != offset2)
                                    //    textBoxFilesLoaded.Text += "*** offset doesn't match with sectormap.sectorok!!! ***"+offset+" "+offset2+"\r\n";
                                    int bytespersectortemp = bytespersectorthread;
                                    int sum = 0;
                                    if (track == 0) bytespersectorthread = 512;
                                    if (offset < 2000000 - bytespersectorthread)
                                    {
                                        if (sectorbuf.Length == bytespersectorthread + 2)
                                        {
                                            for (i = 0; i < bytespersectorthread; i++)
                                            {
                                                disk[i + offset] = sectorbuf[i];
                                                sum += sectorbuf[i];
                                            }
                                            if (sum == 0) sectormap.sectorok[track, sectornr] = SectorMapStatus.SectorOKButZeroed; // If the entire sector is zeroes, allow new data
                                        }

                                    }
                                    bytespersectorthread = bytespersectortemp;
                                }
                                //If checksum is not ok, we can still use the data, better than nothing strategy, we will show it in the sectormap
                                else if (datacrcchk != 0x00 && sectornr >= 0 && sectornr < 18 && headnr < 3 && tracknr >= 0 && tracknr < 82)
                                {
                                    if (sectormap.sectorok[track, sectornr] == SectorMapStatus.empty)
                                        sectormap.sectorok[track, sectornr] = SectorMapStatus.HeadOkDataBad;
                                    else if (sectormap.sectorok[track, sectornr] != SectorMapStatus.HeadOkDataBad) // if it's a bad sector, capture more data, otherwise, go to next marker
                                        continue;

                                    if (sectorbuf.Length > 500)
                                    {
                                        if (sectornr > 8 && diskformat != DiskFormat.pc2m)
                                        {
                                            diskformat = DiskFormat.pchd; // Assume HD
                                            sectorspertrack = 18;
                                        }

                                        if (tracknr == 0 && sectornr == 0 && diskformat == DiskFormat.pchd)
                                        {
                                            if (sectorbuf[401] == '2' && sectorbuf[402] == 'M') // Detect 2M format
                                            {
                                                diskformat = DiskFormat.pc2m;
                                                sectorspertrack = 11;
                                                //bytespersectorthread = 1024;
                                                // When we're in the process and only detecting 2M after many other sectors were processed,
                                                // the previous sectors were processed with assumption diskformat = HD. We need to invalidate all found sectors
                                                // except T000
                                                for (i = 0; i < 200; i++)
                                                {
                                                    for (j = 0; j < 25; j++)
                                                    {
                                                        sectormap.sectorok[i, j] = SectorMapStatus.empty;
                                                    }
                                                }
                                            }
                                        }

                                        if (procsettings.UseErrorCorrection)
                                        {
                                            //Create hash
                                            sectorbuf[bytespersectorthread] = (byte)track;
                                            sectorbuf[bytespersectorthread + 1] = sectornr;
                                            byte[] secthash = mySHA256.ComputeHash(sectorbuf);

                                            // Check if there's a duplicate
                                            int isunique = -1;
                                            for (i = 0; i < sectordata2.Count; i++)
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
                                                byte[] b = new byte[bytespersectorthread + 16];

                                                for (i = 0; i < bytespersectorthread + 16; i++)
                                                {
                                                    b[i] = bytebuf[i];
                                                }

                                                //lock (lockbadsector)
                                                {
                                                    //int badsectorcnt2 = badsectorcnt;
                                                    badsectorcntthread++;
                                                    badsectorhash[markerindex] = secthash;

                                                    //sectordatathread.threadid = threadid;
                                                    sectordatathread.mfmMarkerStatus = SectorMapStatus.HeadOkDataBad; // 2 = bad sector data
                                                    sectordatathread.track = track;
                                                    sectordatathread.sector = sectornr;
                                                    sectordatathread.sectorlength = bytespersectorthread;
                                                    sectordatathread.crc = (int)datacrc;
                                                    sectordatathread.sectorbytes = b;
                                                    sectordatathread.MarkerType = MarkerType.data;
                                                    sectordata2[prevmarkerindex].DataIndex = markerindex;
                                                }
                                            }
                                            else
                                            {
                                                //tbreceived.Append("Bad sector duplicate found, skipping \r\n");
                                            }
                                        }



                                        offset = (tracknr * sectorspertrack * bytespersectorthread * 2)
                                            + (headnr * sectorspertrack * bytespersectorthread)
                                            + (sectornr * bytespersectorthread);
                                        int bytespersectortemp = bytespersectorthread;

                                        if (sectormap.sectorok[track, sectornr] == SectorMapStatus.empty)
                                            if (offset < 2000000 - bytespersectorthread && bytespersectorthread == sectorbuf.Length + 2)
                                                for (i = 0; i < bytespersectorthread; i++)
                                                {
                                                    if (disk[i + offset] == 0)
                                                        disk[i + offset] = sectorbuf[i];
                                                }
                                        //sectormap.sectorok[track, sectornr] = SectorMapType.headokbaddata; // Sector is not CRC pass
                                        bytespersectorthread = bytespersectortemp;
                                    }
                                }

                        // EXPERIMENTAL, not safe to use! Makes assumptions that are incorrect in some cases
                        // This is an attempt to recover a sector with a bad header but good sector data
                        // This only works if all the data is already processed and a single sector per track is missing
                        // This sector can't be sector 0 on the track. It takes the previous succesful track
                        // Then finds the first bad sector on it and puts the good data/bad header in the disk array


                        if (headercrcchk == 0x00 && tracknr < 82) // use the previous known good tracknr in case of a reconstruction
                        {
                            previousheadnr = headnr;
                            previoustrack = tracknr;
                        }
                    }
                    progresses[threadid] = sectordata2.Count;
                    //tbreceived.Append(relativetime().ToString() + "Convert MFM to sector data.\r\n");
                    //tbreceived.Append(tbreceived.ToString());
                } // if(stop == 0)
            }

            // If the bootsector is found, get the correct disk format based on FAT12 spec
            if (sectormap.sectorok[0, 0] == SectorMapStatus.CrcOk)
            {
                int bytesPerSector = disk[12] * 256 + disk[11];
                int sectorsPerCluster = disk[13];
                int totalSectorCount = disk[20] * 256 + disk[19];

                int disksize = bytesPerSector * totalSectorCount;

                switch (disksize)
                {
                    case 368640: // 360KB 5.25"
                        diskformat = DiskFormat.pc360kb525in;
                        break;

                    case 737280: // 720KB (DS DD) 3.5"
                        diskformat = DiskFormat.pcdd;
                        break;

                    case 1474560: // 1440KB (DS HD) 3.5"
                        diskformat = DiskFormat.pchd;
                        break;

                    default:
                        break;
                }
            }

        } // Method closed

        




        

        

        // Some bad sectors only contain a cluster of bad periods
        // These can easily be reprocessed brute force wise if they
        // only cover like 20 periods. 
        public ECResult ProcessRealign4E(ECSettings ecSettings)
        {
            int i;
            byte[] _4EMarker = { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0 };// 3x 4E
                                                                                                                                                                                  //byte[] _4489EMarker = { 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1 };// 44894489
                                                                                                                                                                                  //01000100100010010100010010001001
            int markerindex = 0;
            byte[] crcinsectordatadecoded = new byte[100];
            int crcinsectordata = 0;
            int bitshifted = 0;
            byte[] lasttwosectorbytes = new byte[100];
            int periodSelectionStart, mfmSelectionStart = 0;
            int periodSelectionEnd, mfmSelectionEnd = 0;
            int bytestart, byteend;
            int mfmAlignedStart, mfmAlignedEnd;

            // User selected part to be brute forced:
            periodSelectionStart = ecSettings.periodSelectionStart;
            periodSelectionEnd = ecSettings.periodSelectionEnd;
            int indexS1 = ecSettings.indexS1;
            int threadid = ecSettings.threadid;

            // Stop if selection is too large, taking too long.
            if (periodSelectionEnd - periodSelectionStart > 50)
            {
                tbreceived.Append("Selection too large, please make it smaller, 50 max.\r\n");
                return null;
            }

            // Copy mfm data from mfms
            int sectorlength = sectordata2[indexS1].sectorlength;

            byte[] mfmcorrected = new byte[(sectorlength + 6) * 16 + 1000];
            byte[] mfmbuf = mfms[sectordata2[indexS1].threadid].SubArray(sectordata2[indexS1].MarkerPositions, (sectorlength + 100) * 16);
            byte[] bytebuf = new byte[sectorlength + 6];

            int cntperiods = 0;
            //Find where in the mfm data the periodSelectionStart is
            for (i = 0; i < (sectorlength + 6) * 16; i++)
            {
                if (mfmbuf[i] == 1)
                    cntperiods++;
                if (cntperiods == periodSelectionStart) mfmSelectionStart = i;
                if (cntperiods == periodSelectionEnd)
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

            // Is processing.diskformat amiga or pc?
            // The number of bits shifted with regards to where the 4E padding should or expected to be
            markerindex = FindMarker(ref mfmbuf, mfmbuf.Length, (sectorlength + 4) * 16, ref _4EMarker);
            bitshifted = markerindex - ((sectorlength + 4 + 3) * 16);


            //bitshifted = markerindex - ((sectorlength + 7) * 16);

            // Skip processing if bitshift is too large
            if (bitshifted > 32 || bitshifted < -32)
            {
                tbreceived.Append("ScanTemp: i: " + indexS1 + " Bitshift is too large. (" + bitshifted + ")\r\n");
                return null;
            }

            int markeroffset = sectordata2[indexS1].MarkerPositions;
            byte[] mfmsdest = mfms[sectordata2[indexS1].threadid];

            //Copy the bitshift correct mfm data back to the large mfms array
            for (i = mfmAlignedStart + 24; i < mfmbuf.Length - bitshifted; i++)
            {
                mfmsdest[markeroffset + i] = mfmbuf[i + bitshifted];
            }

            // If no 4E markers are found, exit
            if (markerindex == -1 || markerindex == -2)
            {
                tbreceived.Append("ScanTemp: No marker found.\r\n");
                return null;
            }



            if ((int)diskformat > 2) // PC processing
            {
                // get the sector crc
                int crcindex = markerindex - 48;
                for (i = 0; i < 2; i++)
                {
                    crcinsectordatadecoded[i] = MFMBits2BINbyte(ref mfmbuf, crcindex + (i * 16));
                    //tbreceived.Append(lasttwosectorbytes[i].ToString("X2") );
                }
                crcinsectordata = crcinsectordatadecoded[0] << 8 | crcinsectordatadecoded[1];

                for (i = 0; i < 4; i++)
                {
                    lasttwosectorbytes[i] = MFMBits2BINbyte(ref mfmbuf, crcindex + (i * 16) - 32);
                    //tbreceived.Append(lasttwosectorbytes[i].ToString("X2"));
                }
                int track = sectordata2[indexS1].track;
                int sector = sectordata2[indexS1].sector;
                //tbreceived.Append("\r\n");
                tbreceived.Append("ProcessRealign4E: T" + track.ToString("d3") + "S" + sector + " Found 4E aligned CRC: i:" + indexS1 + " " + crcinsectordata.ToString("X4") + ". Last two bytes: " + lasttwosectorbytes[0].ToString("X2") +
                 lasttwosectorbytes[1].ToString("X2") + " Bitshifted: " + bitshifted + "\r\n");

                // Convert first part up to mfmAlignedStart
                // Convert it back to binary
                for (i = 0; i < bytestart; i++)
                {
                    bytebuf[i] = MFMBits2BINbyte(ref mfmbuf, (i * 16));
                }

                int start = i - 1;
                //int cnt = start+1;
                int mfmoffset = crcindex - ((sectorlength + 4) * 16);

                // Convert last part with realignedment based on the 4E 'marker':
                for (i = start + 1; i < (sectorlength + 6); i++)
                {
                    bytebuf[i] = MFMBits2BINbyte(ref mfmbuf, mfmoffset + (i * 16));

                }

                byte databyte;
                StringBuilder bytesstring = new StringBuilder();
                StringBuilder txtstring = new StringBuilder();
                // Convert to text and ascii encoded hex
                for (i = 0; i < (sectorlength + 6); i++)
                {
                    databyte = bytebuf[i];
                    bytesstring.Append(databyte.ToString("X2"));
                    if (databyte > 32 && databyte < 127)
                        txtstring.Append((char)databyte);
                    else txtstring.Append(".");
                    if (i == bytestart || i == byteend) bytesstring.Append(" ");
                    if (i % 32 == 31)
                    {
                        txtstring.Append("\r\n");
                        bytesstring.Append("\r\n");
                    }
                }
                ecSettings.sectortextbox.Text = txtstring.ToString() + "\r\n\r\n";
                ecSettings.sectortextbox.Text += bytesstring.ToString() + "\r\n";


                // Now we've got the sector sans corrupted data

                // Find the incoming period
                for (i = 1; i < 4; i++)
                {
                    if (mfmbuf[mfmAlignedStart - i] == 1)
                        break;
                }
                int startcondition = i;

                // Add the newly created aligned bad sector to the bad sector list
                // First clone all the data

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
                sectordata.sectorbytes = bytebuf;

                sectordata2.TryAdd(sectordata2.Count, sectordata);
                //sectordata2[badsectorcnt2].sectorbytes = bytebuf;

                //return sectordata;
                ECResult result = new ECResult();

                result.index = sectordata2.Count - 1;
                result.sectordata = sectordata;

                return result;

            } // End PC processing
            return null;
        } // End ProcessRealign4E()

        

        // Some bad sectors only contain a cluster of bad periods
        // These can easily be reprocessed brute force wise if they
        // only cover like 20 periods max. 
        public void ECCluster2(ECSettings ecSettings)
        {
            int i;
            byte[] _4EMarker = { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0 };// 3x 4E
            int _4Eindex = 0;
            byte[] crcinsectordatadecoded = new byte[100];

            int bitshifted = 0;
            byte[] lasttwosectorbytes = new byte[100];
            int periodSelectionStart, mfmSelectionStart = 0;
            int periodSelectionEnd, mfmSelectionEnd = 0;
            int bytestart, byteend;
            int mfmAlignedStart, mfmAlignedEnd;
            int indexS1 = ecSettings.indexS1;
            int threadid = ecSettings.threadid;

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
            //Find where in the mfm data the periodSelectionStart is
            for (i = 0; i < (sectorlength + 6) * 16; i++)
            {
                if (mfmbuf[i] == 1)
                    cntperiods++;
                if (cntperiods == periodSelectionStart) mfmSelectionStart = i;
                if (cntperiods == periodSelectionEnd)
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


            // Find 4E right after the crc bytes at the end of the sector
            // 4E bytes are padding bytes between header and data. 
            // When the 4E markers are found it will increase the chance of 
            // getting a proper crc, even if it's bit shifted caused by corrupt data

            _4Eindex = FindMarker(ref mfmbuf, mfmbuf.Length, (sectorlength) + 4 * 16, ref _4EMarker);
            // The number of bits shifted with regards to where the 4E padding should or expected to be
            bitshifted = _4Eindex - ((sectorlength + 7) * 16);

            // If there's a bitshift estimate the most likely number of missing periods, add to end of selection.
            //periodSelectionEnd = periodSelectionEnd;// + (-bitshifted / 2);
            // Add bitshift of mfmselection end
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
            tbreceived.Append("Bitshifted: " + bitshifted + "\r\n");
            tbreceived.Append("periodSelectionStart:" + periodSelectionStart + " periodSelectionEnd: " + periodSelectionEnd + "\r\n");
            tbreceived.Append("mfmSelectionStart: " + mfmAlignedStart + " mfmSelectionEnd: " + mfmSelectionEnd + "\r\n");
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

                        for (i = 0; i < 518; i++)
                            data[i] = MFMBits2BINbyte(ref mfmaligned, (i * 16));

                        // Check crc
                        ushort datacrcchk;
                        Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
                        datacrcchk = crc.ComputeChecksum(data);

                        if (datacrcchk == 0x0000)
                        {
                            detectioncnt++;
                            tbreceived.Append("FindBruteForce: Correction found! q = " + p + "Count: " + detectioncnt + "\r\nData: ");
                            for (i = 0; i < 528; i++)
                            {
                                tbreceived.Append(data[i].ToString("X2") + " ");
                                if (i % 16 == 15) tbreceived.Append("\r\n");
                                if (i == mfmSelectionStart / 16 || i == mfmSelectionEnd / 16) tbreceived.Append("--");
                                //dat[offset + i] = data[offset + i];
                            }
                            tbreceived.Append("c6_max:" + c6_max + " c8_max:" + c8_max + "\r\n");
                            tbreceived.Append("Time: " + sw.ElapsedMilliseconds + "ms\r\n");
                            //Save recovered sector to disk array
                            int diskoffset = sectordata2[indexS1].track * sectorspertrack * 512 + sectordata2[indexS1].sector * 512;
                            sectormap.sectorok[sectordata2[indexS1].track, sectordata2[indexS1].sector] = SectorMapStatus.ErrorCorrected; // Error corrected (shows up as 'c')
                            for (i = 0; i < bytespersector; i++)
                            {
                                disk[i + diskoffset] = data[i + 4];
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

        } // ECCluster2

        

        

        // MFM Encoded byte error correction with MFM bytes sorted by frequency of occurrance
        public void ProcessClusterMFMEnc(ECSettings ecSettings)
        {
            int i;
            byte[] _4EMarker = { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0 };// 3x 4E
            int _4Eindex = 0;
            byte[] crcinsectordatadecoded = new byte[100];

            int bitshifted = 0;
            byte[] lasttwosectorbytes = new byte[100];
            int periodSelectionStart, mfmSelectionStart = 0;
            int periodSelectionEnd, mfmSelectionEnd = 0;
            int bytestart, byteend;
            int mfmAlignedStart, mfmAlignedEnd;
            int[] combi = new int[32];
            int combilimit;
            int indexS1 = ecSettings.indexS1;
            int threadid = ecSettings.threadid;

            mfmAlignedStart = ecSettings.MFMByteStart;
            mfmAlignedEnd = mfmAlignedStart + (ecSettings.MFMByteLength * 8);
            MFMByteEncPreset mfmpreset = new MFMByteEncPreset();

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
            //Find where in the mfm data the periodSelectionStart is
            for (i = 0; i < (sectorlength + 6) * 16; i++)
            {
                if (mfmbuf[i] == 1)
                    cntperiods++;
                if (cntperiods == periodSelectionStart) mfmSelectionStart = i;
                if (cntperiods == periodSelectionEnd)
                {
                    mfmSelectionEnd = i;
                    break;
                }
            }

            tbreceived.Append("Selection: period start:" + periodSelectionStart + " period end: " + periodSelectionEnd + "\r\n");
            tbreceived.Append("mfm start: " + mfmSelectionStart + " mfm end:" + mfmSelectionEnd + "\r\n");

            bytestart = mfmSelectionStart / 16;
            byteend = mfmSelectionEnd / 16;

            //mfmAlignedStart = bytestart * 16;
            //mfmAlignedEnd = (byteend + 1) * 16;

            tbreceived.Append("bytestart: " + bytestart + " byte end: " + byteend + "\r\n");


            // Find 4E right after the crc bytes at the end of the sector
            // 4E bytes are padding bytes between header and data. 
            // When the 4E markers are found it will increase the chance of 
            // getting a proper crc, even if it's bit shifted caused by corrupt data

            _4Eindex = FindMarker(ref mfmbuf, mfmbuf.Length, (sectorlength) + 4 * 16, ref _4EMarker);
            // The number of bits shifted with regards to where the 4E padding should or expected to be
            bitshifted = _4Eindex - ((sectorlength + 7) * 16);

            // If there's a bitshift estimate the most likely number of missing periods, add to end of selection.
            //periodSelectionEnd = periodSelectionEnd;// + (-bitshifted / 2);
            // Add bitshift of mfmselection end
            mfmSelectionEnd = mfmSelectionEnd - bitshifted;

            // Copy mfm data to aligned array
            byte[] mfmaligned = new byte[mfmbuf.Length + 32];
            for (i = 0; i < mfmbuf.Length; i++)
                mfmaligned[i] = mfmbuf[i];

            for (i = mfmAlignedStart; i < mfmbuf.Length - bitshifted; i++)
                mfmaligned[i] = mfmbuf[i + bitshifted];


            byte[] data = new byte[(mfmaligned.Length) / 16 + 1];

            //for (i = 0; i < (mfmaligned.Length/16); i++)
            //    data[i] = processing.MFMBits2BINbyte(ref mfmaligned, (i * 16));

            mfmSelectionStart = i;


            // then back up one, we want to end with a '0'
            mfmSelectionEnd = i - 1;
            tbreceived.Append("Bitshifted: " + bitshifted + "\r\n");
            tbreceived.Append("periodSelectionStart:" + periodSelectionStart + " periodSelectionEnd: " + periodSelectionEnd + "\r\n");
            tbreceived.Append("mfmSelectionStart: " + mfmAlignedStart + " mfmSelectionEnd: " + mfmAlignedEnd + "\r\n");
            int j, p, q;
            int detectioncnt = 0;
            int numberofitems = periodSelectionEnd - periodSelectionStart;
            int numberofmfmitems = mfmSelectionEnd - mfmSelectionStart;
            int numberofitmssq = 1 << numberofitems;
            //int combs = (int)CombinationsUpDown.Value;


            stop = 0;
            // Brute force with weighing of 4/6/8us


            int k, l, u;
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
                    if (stop == 1) break;
                    Application.DoEvents();
                    for (k = 0; k < NumberOfMfmBytes; k++)
                    {
                        //if (processing.stop == 1) break;
                        for (l = 0; l < 8; l++)
                        {
                            mfmaligned[mfmAlignedStart + l + (k * 8)] = mfmpreset.MFMPC[combi[k], l];
                        }
                    }

                    // Check result
                    for (i = 0; i < 518; i++)
                        data[i] = MFMBits2BINbyte(ref mfmaligned, (i * 16));

                    // Check crc
                    ushort datacrcchk;
                    Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
                    datacrcchk = crc.ComputeChecksum(data);

                    if (datacrcchk == 0x0000)
                    {
                        detectioncnt++;
                        tbreceived.Append("CRC ok! iteration: " + combinations + "\r\n");
                        printarray(combi, NumberOfMfmBytes);
                        for (i = 0; i < 528; i++)
                        {
                            tbreceived.Append(data[i].ToString("X2") + " ");
                            if (i % 16 == 15) tbreceived.Append("\r\n");
                            if (i == mfmAlignedStart / 16 || i == mfmAlignedEnd / 16) tbreceived.Append("--");
                            //dat[offset + i] = data[offset + i];
                        }
                        //tbreceived.Append("c6_max:" + c6_max + " c8_max:" + c8_max + "\r\n");
                        tbreceived.Append("Time: " + sw.ElapsedMilliseconds + "ms\r\n");
                        //Save recovered sector to disk array
                        int diskoffset = sectordata2[indexS1].track * sectorspertrack * 512 + sectordata2[indexS1].sector * 512;
                        sectormap.sectorok[sectordata2[indexS1].track, sectordata2[indexS1].sector] = SectorMapStatus.ErrorCorrected; // Error corrected (shows up as 'c')
                        for (i = 0; i < bytespersector; i++)
                        {
                            disk[i + diskoffset] = data[i + 4];
                        }
                        sectormap.RefreshSectorMap();
                        tbreceived.Append("\r\n");
                        Application.DoEvents();
                        //return q;
                        stop = 1;
                        break;
                    }
                    if (stop == 1) break;
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
        }

        public void printarray(int[] a, int length)
        {
            int i;
            for (i = 0; i < length; i++)
            {
                tbreceived.Append(a[i] + ",");
            }
            tbreceived.Append("\r\n");
        }

        /// <summary>
        /// Returns a number with the requested number of ones. If the seed doesn't match the number of bits it returns 0.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="NumberOfOnes"></param>
        /// <returns></returns>
        private uint GetBitPattern(uint seed, int NumberOfOnes)
        {
            int j;
            int bitcnt = 0;

            for (j = 0; j < 32; j++)
            {
                if (((seed >> j) & 1) == 1) bitcnt++;
            }

            if (bitcnt == NumberOfOnes)
            {
                return seed;
            }

            return 0;
        }

        // Finds 'search' in b, from offset to length+offset.
        private int FindMarker(ref byte[] b, int length, int offset, ref byte[] search)
        {
            int i, j, searchcnt = 0, overflow = 0;
            for (i = offset; i < length - 48; i++)
            {
                for (j = 0; j < search.Length; j++)
                {
                    if (b[i + j] == search[j]) searchcnt++;
                    else
                    {
                        searchcnt = 0;
                        break;
                    }
                    if (searchcnt == search.Length - 1)
                    {
                        searchcnt = 0;
                        return i;
                    }
                }
            }
            if (overflow == 1)
                return -2; // overflow!
            else return -1; // nothing found
        }
    } // FDDProcessing end
}

