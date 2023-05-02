using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using FloppyControlApp.MyClasses;
using FloppyControlApp.MyClasses.Processing;

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
        

        private void GetAllMFMMarkerPositionsDiskspare(int threadid)
        {
            byte[] amigamarkerbytes = AMIGAMARKER;
            uint searchcnt = 0;
            int rxbufcnt = 0;
            int markerpositionscntthread = 0;

            // Find all the sector markers

            //DiskSpare marker
            byte[] amigadsmarkerbytes = AMIGADSMARKER;


            //=============================================================================================
            //Find diskspare sector markers
            if (indexrxbuf > rxbuf.Length) indexrxbuf = rxbuf.Length - 1;

            if ((diskformat == DiskFormat.unknown || diskformat == DiskFormat.diskspare))
            {
                rxbufcnt = procsettings.start;
                searchcnt = 0;
                // Find markers
                for (int i = 0; i < mfmlengths[threadid]; i++)
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
                    for (int j = 0; j < amigadsmarkerbytes.Length; j++)
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
                            // Todo: bottleneck for multithreading!
                            if (!sectordata2.TryAdd(sectordata2.Count, sectordata))
                            {
                                tbreceived.Append("Failed to add to Sectordata dictionary " + markerpositionscntthread + "\r\n");
                                return;
                            }

                            markerpositionscntthread++;
                            break;
                        }
                    }
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
        }

        private void GetAllMFMMarkerPositionsADOS(int threadid)
        {
            byte[] amigamarkerbytes = AMIGAMARKER;
            uint searchcnt;
            int rxbufcnt;
            int markerpositionscntthread = 0;

            //Find AmigaDOS markers
            //=============================================================================================
            if ((diskformat == DiskFormat.unknown || diskformat == DiskFormat.amigados))
            {
                rxbufcnt = procsettings.start;
                searchcnt = 0;
                // Find AmigaDOS markers
                for (int i = 0; i < mfmlengths[threadid]; i++)
                {
                    if (i % 1048576 == 1048575) { progresses[threadid] = i; }
                    if (mfms[threadid][i] == 1) // counting 1's matches the number of bytes in rxbuf + start offset
                        rxbufcnt++;
                    if (rxbufcnt >= rxbuf.Length) break;
                    while (rxbuf[rxbufcnt] < 4 && rxbufcnt < indexrxbuf - 1) rxbufcnt++;
                    for (int j = 0; j < amigamarkerbytes.Length; j++)
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
        }

        public class MFM2Sector
        {
            public byte Tracknr;
            public byte Sectornr;
            public bool Headerok;
            public bool Dataok;
            public byte[] DecodedData;
        }

        private MFM2Sector MFMToBytesADOS(int threadid, int sectorindex, StringBuilder trackboxtemp)
        {
            byte[] headerchecksum;
            byte[] datachecksum;
            byte[] checksum;
            byte[] savechecksum = new byte[4];
            MFMData sectordatathread;
            sectordatathread = sectordata2[sectorindex];

            if (sectorindex % 50 == 49) { progresses[threadid] = (int)sectorindex; }
            if (stop == 1) return null;
            int mrkridx;
            mrkridx = (int)sectordatathread.MarkerPositions;

            byte[] dec1 = new byte[1];

            string checksumok;
            byte headercheckok = 0;
            byte datacheckok = 0;

            byte tracknr = 0;
            byte sectornr = 0;
            byte gapoffset;

            //Decode mfm to bytes
            //====================================================================================================
            //AmigaDOS disk format for ADOS and PFS
            
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
            trackboxtemp.Append(checksumok);
            

            return new MFM2Sector()
            {
                Tracknr = tracknr,
                Sectornr = sectornr,
                Headerok = headercheckok == 1,
                Dataok = datacheckok == 1,
                DecodedData = dec1
            };
        }

        private MFM2Sector MFMToBytesDiskSpare(int threadid, int sectorindex, StringBuilder trackboxtemp)
        {
            byte[] savechecksum = new byte[4];
            MFMData sectordatathread;
            sectordatathread = sectordata2[sectorindex];

            if (sectorindex % 50 == 49) { progresses[threadid] = (int)sectorindex; }
            if (stop == 1) return null;
            int mrkridx;
            mrkridx = (int)sectordatathread.MarkerPositions;

            byte[] dec1;

            string checksumok;
            byte headercheckok;
            byte datacheckok;

            byte tracknr;
            byte sectornr;
            byte gapoffset;

            //Decode mfm to bytes
            //====================================================================================================
            //AmigaDOS disk format for ADOS and PFS

            //Diskspare sector format:
            // <AAAA><4489><4489><AAAA> <TT><SS> <OddChkHi> <OddChkLo> <EvenChkHi> <EvenChkLo>

            byte[] dsdatachecksum = new byte[4];

            headercheckok = 1; // Diskspare doesn't do separate header and data checksums

            // Get track sector and checksum within one uint32: 0xTTSSCCCC
            dec1 = amigamfmdecodebytes(mfms[threadid], mrkridx + 8 * 8, 4 * 16); //At uint 0

            tracknr = dec1[0];
            sectornr = dec1[1];
            dsdatachecksum[0] = dec1[2];
            dsdatachecksum[1] = dec1[3];
            dsdatachecksum[2] = 0;
            dsdatachecksum[3] = 0;

            //Clear display info buffer
            trackboxtemp.Clear();

            uint dchecksum;

            uint offset;
            byte[] tmp;
            dec1 = new byte[520];

            for (int j = 0; j < 512; j += 4)
            {
                tmp = amigamfmdecodebytes(mfms[threadid], mrkridx + (int)j * 16 + 16 * 8, 4 * 16);
                dec1[j] = tmp[0];
                dec1[j + 1] = tmp[1];
                dec1[j + 2] = tmp[2];
                dec1[j + 3] = tmp[3];
            }

            dchecksum = (uint)((mfm2ushort(mfms[threadid], mrkridx + 8 * 16)) & 0x7FFF);
            ushort tmp1;
            offset = 9;
            for (uint j = offset; j < 520; j++)
            {
                tmp1 = mfm2ushort(mfms[threadid], (int)(mrkridx + j * 16));
                dchecksum ^= (uint)(tmp1 & 0xffff);
            }
            savechecksum[0] = (byte)(dchecksum >> 8);
            savechecksum[1] = (byte)(dchecksum & 0xFF);
            savechecksum[2] = 0;
            savechecksum[3] = 0;

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
            else trackboxtemp.Append(checksumok);


            return new MFM2Sector()
            {
                Tracknr = tracknr,
                Sectornr = sectornr,
                Headerok = headercheckok == 1,
                Dataok = datacheckok == 1,
                DecodedData = dec1
            };
        }

        // bytes version of ProcessAmiga
        private void ProcessAmigaMFMbytes(ProcSettings procsettings, int threadid)
        {
            int i;
            uint j;
            
            int markerpositionscntthread = 0;
            int bytespersectorthread = 512;
            int badsectorcntthread = 0;
            //uint totaltime = 0;
            //uint reltime = 0;
            bool debuginfo = false;
            SHA256 mySHA256 = SHA256Managed.Create();
            int sectordata2oldcnt = sectordata2.Count;
#region Find markers
            GetAllMFMMarkerPositionsDiskspare(threadid);

            progresses[threadid] = (int)mfmlengths[threadid];
            ProcessStatus[threadid] = "Find AmigaDOS MFM markers...";

            GetAllMFMMarkerPositionsADOS(threadid);

#endregion
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

            StringBuilder trackbox = new StringBuilder();
            StringBuilder trackboxtemp = new StringBuilder();
            StringBuilder decodedamigaText = new StringBuilder();

            int sectorindex;
            byte[] savechecksum = new byte[4];
            
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

                string checksumok;
                bool headercheckok = false;
                bool datacheckok = false;

                byte tracknr = 0;
                byte sectornr = 0;
                byte gapoffset;

                MFM2Sector Mfm2Sector = null;

                //Decode mfm to bytes
                //AmigaDOS disk format for ADOS and PFS
                if (diskformat == DiskFormat.amigados) //AmigaDOS disk format for ADOS and PFS
                {
                    Mfm2Sector = MFMToBytesADOS(threadid, sectorindex, trackboxtemp);
                }
                else if (diskformat == DiskFormat.diskspare) // Assume it's DiskSpare format
                {
                    Mfm2Sector = MFMToBytesDiskSpare(threadid, sectorindex, trackboxtemp);
                }

                if( Mfm2Sector != null)
                {
                    tracknr         = Mfm2Sector.Tracknr;
                    sectornr        = Mfm2Sector.Sectornr;
                    headercheckok   = Mfm2Sector.Headerok;
                    datacheckok     = Mfm2Sector.Dataok;
                    dec1            = Mfm2Sector.DecodedData;
                }

                sectorspertrack = 11;

                if (diskformat == DiskFormat.diskspare) // Diskspare
                    sectorspertrack = 12;

                // Write sector to disk image array in its proper location
                // The image file doesn't need checksum and track info, it's implied
                // With the location of the data, track 0 sector 0 starts at disk[0], track 1 sector 1
                // at disk[6144] etc
                if (diskformat == DiskFormat.diskspare) // DiskSpare doesn't have header checksum
                {
                    if (tracknr < 164 && sectornr < sectorspertrack)
                        if (datacheckok) sectormap.sectorokLatestScan[tracknr, sectornr]++;
                }
                else if (headercheckok)
                    if (tracknr < 164 && sectornr < 20) sectormap.sectorokLatestScan[tracknr, sectornr]++;


                //If the checksum is correct and sector and track numbers within range and no sector data has already been captured
                //if (headercheckok == 1 && datacheckok == 1 && sectornr >= 0 && sectornr < 13 && tracknr >= 0 && tracknr < 164 && sectormap.sectorok[tracknr, sectornr] != 1)
                if (headercheckok && datacheckok && sectornr >= 0 && sectornr < 13 && tracknr >= 0 && tracknr < 164) // collect good sectors
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
                else if (headercheckok && !datacheckok && sectornr >= 0 && sectornr < 13 && tracknr >= 0 && tracknr < 164) // collect good headers but bad sectors
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
