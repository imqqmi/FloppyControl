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
using FloppyControlApp.MyClasses.Processing;

namespace FloppyControlApp
{
    public partial class FDDProcessing
    {
        public static byte[] A1MARKER = { 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1 };
        public Action GetProcSettingsCallback { get; set; }
        public int Peak1 { get; set; }
        public int Peak2 { get; set; }
        public int Peak3 { get; set; }
        public int GoodSectorHeaderCount { get; set; }
        public int sectordata2oldcnt;
        bool debuginfo = false;

        /// <summary>
        /// Cleaned up version. Converts MFM data into sectors for PC DOS and M2
        /// </summary>
        /// <param name="procsettings">Contains all the processing settings and configuration.</param>
        /// <param name="threadid">For multithreading, store data into its own array to prevent collisions 
        /// with other threads and keep data together for reference during analysys</param>
        /// <param name="tbReceived">The string for logging to be output to a text box.</param>
        private void ProcessPCMFM2Sectordata(ProcSettings procsettings, int threadid, StringBuilder tbReceived)
        {
            //bool writemfm = true;
            int i;
            int previoustrack = 0xff;
            byte previousheadnr = 0xff;
            sectordata2oldcnt = sectordata2.Count;

            SHA256 mySHA256 = SHA256Managed.Create();
                
            int j;

            int markerpositionscntthread = 0;
            
            // Load good sector found info so we can append to it, then at the end

            progressesstart[threadid] = 0;
            progressesend[threadid] = mfmlengths[threadid];
            ProcessStatus[threadid] = "Find MFM markers...";
            //if (i % 250000 == 249999) { progresses[threadid] = i; }

            if (stop != 0) return;

            // Find markers

            // with multithreading, the counting of rxbuf is misaligned with mfms, because
            // it doesn't start at the start of rxbuf. There's no way to know at this point
            // how many 1's there are up to this point. Either do the calculation after processing
            // then adjust the values or not use multithreading.
            
            if (Indexrxbuf > RxBbuf.Length) Indexrxbuf = RxBbuf.Length - 1;
                
            FindAllPCMarkers(threadid);
            // Todo: check if new sector markers are found, otherwise return.
            if (sectordata2.Count > 0 && diskformat == DiskFormat.unknown) // if markers are found and the diskformat has not been set previously, assume PC DD
                diskformat = DiskFormat.pcdd;

            TBReceived.Append(" Markers: " + markerpositionscntthread + " ");
            int markerindex;
            byte[] SectorBlock;
            byte[] sectorbuf = new byte[2050];
            ushort datacrc = 0, datacrcchk; //headercrc is from the captured data, the chk is calculated from the data.

            GoodSectorHeaderCount = 0;
            progressesstart[threadid] = 0;
            progressesend[threadid] = sectordata2.Count;
            ProcessStatus[threadid] = "Converting MFM to sectors...";
            int prevmarkerindex;
            for (markerindex = sectordata2oldcnt; markerindex < sectordata2.Count; markerindex++)
            {
                MFMData SectorHeader = sectordata2[markerindex];

                // Skip already processed sectors and check threadid, only process current thread
                // Was the marker placed by the correct thread? If not, continue to next marker
                if (SectorHeader.processed == true || SectorHeader.threadid != threadid) continue;

                // mark as processed
                SectorHeader.processed = true; 

                //Update progress
                if (markerindex % 250 == 249) { progresses[threadid] = markerindex; if (stop == 1) break; }
                
                datacrcchk = 0xFFFF;

                // First find the IDAM, 10 bytes
                var Idam = MFM2Bytes(SectorHeader.MarkerPositions, 10, threadid);

                if (Idam[3] != 0xFE) continue;
                
                if (debuginfo) TBReceived.Append(" IDAM");

                ExtractSectorHeaderInfo(ref Idam, ref SectorHeader);
                
                int DiskImageSectorOffset;
                DiskImageSectorOffset = (SectorHeader.track * sectorspertrack * SectorHeader.sectorlength * 2)
                                      + (SectorHeader.head * sectorspertrack * SectorHeader.sectorlength)
                                      + (SectorHeader.sector * SectorHeader.sectorlength);

                // Validation method may change sectorsize in SectorHeader if IgnoreHeaderError is true.
                if (!ValidateSectorSize(ref SectorHeader)) continue;

                // If the headerchecksum is non zero, there's an error in the header. 
                // There's code further on to guess the sector and track based on surrounding
                // sectors with correct header, but to speed things up I'll try continueing 
                // Checking the IgnoreHeaderErrorCheckBox skips this part and does decode the sector data
                if ( !ValidateSectorHeaderInfo(ref Idam, ref SectorHeader) && procsettings.IgnoreHeaderError == false)
                {
                    continue;
                }

                //Get sector data and checksum
                // First find the DAM

                //*********************
                // Potential problem if marker for sector data skips to another marker unintentionally
                // Should test if the offset isn't impossibly high
                prevmarkerindex = markerindex; //Go to next marker

                // markerindex will be updated if found, hence the ref.
                FindNextMarker(ref markerindex, threadid, out MFMData SectorData);
                if ( SectorData == null) continue;

                if (debuginfo) TBReceived.Append("\r\nT" + SectorHeader.trackhead + " S" + SectorHeader.sector + " Sector data:\r\n\r\n\r\n");
                SectorBlock = MFM2Bytes(SectorData.MarkerPositions, SectorHeader.sectorlength + 16, threadid);
                
                // Are the data markers there as expected? If not, backtrack and skip to next marker
                if (!(SectorBlock[3] == 0xFB || SectorBlock[3] == 0xF8))
                {
                    markerindex = prevmarkerindex;
                    continue;
                }

                // Validate sector data checksum
                ValidateSectorDataBlock(ref datacrcchk, 
                                        ref datacrc,
                                        ref SectorBlock,
                                        ref sectorbuf,
                                        ref SectorHeader,
                                        ref SectorData);

                StripSectorOfHeader(in SectorBlock, out sectorbuf, in SectorHeader);

                // EXPERIMENTAL, not safe to use! Makes assumptions that are incorrect in some cases
                // This is an attempt to recover a sector with a bad header but good sector data
                // This only works if all the data is already processed and a single sector per track is missing
                // This sector can't be sector 0 on the track. It takes the previous succesful track
                // Then finds the first bad sector on it and puts the good data/bad header in the disk array
                if (procsettings.IgnoreHeaderError)
                {
                    HandleIngoreBadHeader(in SectorHeader, in SectorData, threadid, previoustrack, previousheadnr);
                    continue;
                }

                // use the previous known good tracknr in case of a reconstruction
                if (SectorHeader.Status == SectorMapStatus.CrcOk && SectorHeader.track < 82)
                {
                    previousheadnr = (byte)SectorHeader.head;
                    previoustrack = SectorHeader.track;
                }

                // DD, HD or 2M
                SetSectorsPerTrackByDiskFormat();

                //If this sector was already captured ok, don't overwrite it with data!
                if (SectorMap.sectorok[SectorHeader.trackhead, SectorHeader.sector] == SectorMapStatus.CrcOk)
                    continue;
                // If sector length is invalid, skip to next marker
                if (sectorbuf.Length <= 500) continue;
                if (SectorHeader.Status != SectorMapStatus.CrcOk) continue;
                
                //Happyflow, if everything checks out, save sector data to disk image array.
                if (SectorData.Status == SectorMapStatus.CrcOk 
                    && SectorHeader.sector >= 0 
                    && SectorHeader.sector < 18 
                    && SectorHeader.head < 3 
                    && SectorHeader.track >= 0 
                    && SectorHeader.track < 82 
                    && SectorMap.sectorok[SectorHeader.trackhead, SectorHeader.sector] != SectorMapStatus.CrcOk)
                {
                    // Determine diskformat
                    // If HD disk is actually 2M, clear sectorok map. Only on track 0 sector 0.
                    DetectDiskFormat(SectorHeader, sectorbuf);

                    // Mark Sector is CRC pass on sectormap for user feedback
                    SectorMap.sectorok[SectorHeader.trackhead, SectorHeader.sector] = SectorMapStatus.CrcOk;
                    HandleErrorCorrection( ref SectorHeader, ref SectorData,   in sectorbuf,
                                            in SectorBlock,  in  procsettings,    markerindex,
                                               datacrc,          threadid);
                    ReportFoundSector(SectorHeader, procsettings, datacrcchk);
                    SaveSectorToDiskImage(ref SectorHeader, DiskImageSectorOffset, sectorbuf);
                    
                    continue;
                }
                
                //If checksum is not ok, we can still use the data, better than nothing strategy, we will show it in the sectormap
                if (SectorData.Status ==  SectorMapStatus.CrcBad 
                    && SectorHeader.sector >= 0 
                    && SectorHeader.sector < 18 
                    && SectorHeader.head < 3 
                    && SectorHeader.track >= 0 
                    && SectorHeader.track < 82)
                {
                    // if sectormap has no data, we can safely mark bad sector.
                    if (SectorMap.sectorok[SectorHeader.trackhead, SectorHeader.sector] == SectorMapStatus.empty)
                        SectorMap.sectorok[SectorHeader.trackhead, SectorHeader.sector] = SectorMapStatus.HeadOkDataBad;

                    DetectDDOrHD(SectorHeader);
                    Detect2MDiskFormat(in SectorHeader, sectorbuf);

                    HandleErrorCorrection(ref SectorHeader, ref SectorData, in sectorbuf,
                                            in SectorBlock, in procsettings, markerindex,
                                               datacrc, threadid, true);

                    if (SectorMap.sectorok[SectorHeader.trackhead, SectorHeader.sector] == SectorMapStatus.empty)
                        if (DiskImageSectorOffset < 2000000 - SectorHeader.sectorlength && SectorHeader.sectorlength == sectorbuf.Length + 2)
                            for (i = 0; i < SectorHeader.sectorlength; i++)
                            {
                                if (disk[i + DiskImageSectorOffset] == 0)
                                    disk[i + DiskImageSectorOffset] = sectorbuf[i];
                            }
                    
                }
            }
            progresses[threadid] = sectordata2.Count;
            //tbreceived.Append(relativetime().ToString() + "Convert MFM to sector data.\r\n");
            //tbreceived.Append(tbreceived.ToString());
            tbReceived.Append(TBReceived);
            

            // If the bootsector is found, get the correct disk format based on FAT12 spec
            if (SectorMap.sectorok[0, 0] == SectorMapStatus.CrcOk)
            {
                int bytesPerSector = disk[12] * 256 + disk[11];
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
            int markerindex;
            byte[] crcinsectordatadecoded = new byte[100];
            int crcinsectordata;
            int bitshifted;
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
                TBReceived.Append("Selection too large, please make it smaller, 50 max.\r\n");
                return null;
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

            TBReceived.Append("Selection: period start:" + periodSelectionStart + " period end: " + periodSelectionEnd + "\r\n");
            TBReceived.Append("mfm start: " + mfmSelectionStart + " mfm end:" + mfmSelectionEnd + "\r\n");

            bytestart = mfmSelectionStart / 16;
            byteend = mfmSelectionEnd / 16;

            mfmAlignedStart = bytestart * 16;
            mfmAlignedEnd = (byteend + 1) * 16;

            TBReceived.Append("bytestart: " + bytestart + " byte end: " + byteend + "\r\n");
            TBReceived.Append("mfmAlignedstart: " + mfmAlignedStart + " mfmAlignedEnd: " + mfmAlignedEnd + "\r\n");

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
                TBReceived.Append("ScanTemp: i: " + indexS1 + " Bitshift is too large. (" + bitshifted + ")\r\n");
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
                TBReceived.Append("ScanTemp: No marker found.\r\n");
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
                TBReceived.Append("ProcessRealign4E: T" + track.ToString("d3") + "S" + sector + " Found 4E aligned CRC: i:" + indexS1 + " " + crcinsectordata.ToString("X4") + ". Last two bytes: " + lasttwosectorbytes[0].ToString("X2") +
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

                // Add the newly created aligned bad sector to the bad sector list
                // First clone all the data

                int badsectorold = indexS1;

                MFMData sectordata = new MFMData
                {
                    threadid = threadid,
                    MarkerPositions = sectordata2[badsectorold].MarkerPositions,
                    rxbufMarkerPositions = sectordata2[badsectorold].rxbufMarkerPositions,
                    Status = sectordata2[badsectorold].Status, // 2 = bad sector data
                    track = sectordata2[badsectorold].trackhead,
                    sector = sectordata2[badsectorold].sector,
                    sectorlength = sectordata2[badsectorold].sectorlength,
                    crc = sectordata2[badsectorold].crc,
                    sectorbytes = bytebuf
                };

                sectordata2.TryAdd(sectordata2.Count, sectordata);
                //sectordata2[badsectorcnt2].sectorbytes = bytebuf;

                //return sectordata;
                ECResult result = new ECResult
                {
                    index = sectordata2.Count - 1,
                    sectordata = sectordata
                };

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
            int _4Eindex;
            int bitshifted;
            int periodSelectionStart, mfmSelectionStart = 0;
            int periodSelectionEnd, mfmSelectionEnd = 0;
            int bytestart, byteend;
            int mfmAlignedStart;
            int indexS1 = ecSettings.indexS1;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();

            // User selected part to be brute forced:
            periodSelectionStart = ecSettings.periodSelectionStart;
            periodSelectionEnd = ecSettings.periodSelectionEnd;

            // Stop if selection is too large, taking too long.
            if (periodSelectionEnd - periodSelectionStart > 50)
            {
                TBReceived.Append("Selection too large, please make it smaller, 50 max.\r\n");
                return;
            }

            // Copy mfm data from mfms
            int sectorlength = sectordata2[indexS1].sectorlength;
            byte[] mfmbuf = mfms[sectordata2[indexS1].threadid].SubArray(sectordata2[indexS1].MarkerPositions, (sectorlength + 100) * 16);

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

            TBReceived.Append("Selection: period start:" + periodSelectionStart + " period end: " + periodSelectionEnd + "\r\n");
            TBReceived.Append("mfm start: " + mfmSelectionStart + " mfm end:" + mfmSelectionEnd + "\r\n");

            bytestart = mfmSelectionStart / 16;
            byteend = mfmSelectionEnd / 16;

            mfmAlignedStart = bytestart * 16;

            TBReceived.Append("bytestart: " + bytestart + " byte end: " + byteend + "\r\n");


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
            mfmSelectionEnd -= bitshifted;

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
            TBReceived.Append("Bitshifted: " + bitshifted + "\r\n");
            TBReceived.Append("periodSelectionStart:" + periodSelectionStart + " periodSelectionEnd: " + periodSelectionEnd + "\r\n");
            TBReceived.Append("mfmSelectionStart: " + mfmAlignedStart + " mfmSelectionEnd: " + mfmSelectionEnd + "\r\n");
            int j, p, q;
            int mfmcorrectedindex;
            byte[] combinations = new byte[100];
            int detectioncnt = 0;
            int numberofitems = periodSelectionEnd - periodSelectionStart;
            int numberofmfmitems = mfmSelectionEnd - mfmSelectionStart;
            ulong c6;
            ulong c8 = 0;
            int c6_max;
            int c8_max;
            uint c6cnt = 0;
            uint c8cnt = 0;
            int combs = ecSettings.combinations;

            
            stop = 0;
            // Brute force with weighing of 4/6/8us
            for (c8_max = ecSettings.C8Start; c8_max < numberofitems; c8_max++)
            {
                TBReceived.Append("c8_max: " + c8_max + "\r\n");
                for (c6_max = ecSettings.C6Start; c6_max < numberofitems; c6_max++)
                {
                    TBReceived.Append("c6_max: " + c6_max + "\r\n");
                    for (p = 0; p < combs; p++)
                    {
                        if (p % 25000 == 24999)
                        {
                            TBReceived.Append("p: " + p + "\r\n");
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
                            TBReceived.Append("FindBruteForce: Correction found! q = " + p + "Count: " + detectioncnt + "\r\nData: ");
                            for (i = 0; i < 528; i++)
                            {
                                TBReceived.Append(data[i].ToString("X2") + " ");
                                if (i % 16 == 15) TBReceived.Append("\r\n");
                                if (i == mfmSelectionStart / 16 || i == mfmSelectionEnd / 16) TBReceived.Append("--");
                                //dat[offset + i] = data[offset + i];
                            }
                            TBReceived.Append("c6_max:" + c6_max + " c8_max:" + c8_max + "\r\n");
                            TBReceived.Append("Time: " + sw.ElapsedMilliseconds + "ms\r\n");
                            //Save recovered sector to disk array
                            int diskoffset = sectordata2[indexS1].trackhead * sectorspertrack * 512 + sectordata2[indexS1].sector * 512;
                            SectorMap.sectorok[sectordata2[indexS1].track, sectordata2[indexS1].sector] = SectorMapStatus.ErrorCorrected; // Error corrected (shows up as 'c')
                            for (i = 0; i < bytespersector; i++)
                            {
                                disk[i + diskoffset] = data[i + 4];
                            }
                            //sectormap.RefreshSectorMap();
                            TBReceived.Append("\r\n");
                            Application.DoEvents();
                            //return q;
                            stop = 1;
                            break;
                        }

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
            TBReceived.Append("Done.\r\n");
            return;

        } // ECCluster2

        

        

        // MFM Encoded byte error correction with MFM bytes sorted by frequency of occurrance
        public void ProcessClusterMFMEnc(ECSettings ecSettings)
        {
            int i;
            byte[] _4EMarker = { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0 };// 3x 4E
            int _4Eindex;
            int bitshifted;
            int periodSelectionStart, mfmSelectionStart = 0;
            int periodSelectionEnd, mfmSelectionEnd = 0;
            int bytestart, byteend;
            int mfmAlignedStart, mfmAlignedEnd;
            int[] combi = new int[32];
            int combilimit;
            int indexS1 = ecSettings.indexS1;

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
                TBReceived.Append("Selection too large, please make it smaller, 50 max.\r\n");
                return;
            }

            // Copy mfm data from mfms
            int sectorlength = sectordata2[indexS1].sectorlength;
            byte[] mfmbuf = mfms[sectordata2[indexS1].threadid].SubArray(sectordata2[indexS1].MarkerPositions, (sectorlength + 100) * 16);

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

            TBReceived.Append("Selection: period start:" + periodSelectionStart + " period end: " + periodSelectionEnd + "\r\n");
            TBReceived.Append("mfm start: " + mfmSelectionStart + " mfm end:" + mfmSelectionEnd + "\r\n");

            bytestart = mfmSelectionStart / 16;
            byteend = mfmSelectionEnd / 16;

            //mfmAlignedStart = bytestart * 16;
            //mfmAlignedEnd = (byteend + 1) * 16;

            TBReceived.Append("bytestart: " + bytestart + " byte end: " + byteend + "\r\n");


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

            // Copy mfm data to aligned array
            byte[] mfmaligned = new byte[mfmbuf.Length + 32];
            for (i = 0; i < mfmbuf.Length; i++)
                mfmaligned[i] = mfmbuf[i];

            for (i = mfmAlignedStart; i < mfmbuf.Length - bitshifted; i++)
                mfmaligned[i] = mfmbuf[i + bitshifted];


            byte[] data = new byte[(mfmaligned.Length) / 16 + 1];

            //for (i = 0; i < (mfmaligned.Length/16); i++)
            //    data[i] = processing.MFMBits2BINbyte(ref mfmaligned, (i * 16));

            // then back up one, we want to end with a '0'
            
            TBReceived.Append("Bitshifted: " + bitshifted + "\r\n");
            TBReceived.Append("periodSelectionStart:" + periodSelectionStart + " periodSelectionEnd: " + periodSelectionEnd + "\r\n");
            TBReceived.Append("mfmSelectionStart: " + mfmAlignedStart + " mfmSelectionEnd: " + mfmAlignedEnd + "\r\n");
            int j, q;
            int detectioncnt = 0;

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

                TBReceived.Append("Iterations: " + iterations + "\r\n");
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
                        TBReceived.Append("CRC ok! iteration: " + combinations + "\r\n");
                        PrintArray(combi, NumberOfMfmBytes);
                        for (i = 0; i < 528; i++)
                        {
                            TBReceived.Append(data[i].ToString("X2") + " ");
                            if (i % 16 == 15) TBReceived.Append("\r\n");
                            if (i == mfmAlignedStart / 16 || i == mfmAlignedEnd / 16) TBReceived.Append("--");
                            //dat[offset + i] = data[offset + i];
                        }
                        //tbreceived.Append("c6_max:" + c6_max + " c8_max:" + c8_max + "\r\n");
                        TBReceived.Append("Time: " + sw.ElapsedMilliseconds + "ms\r\n");
                        //Save recovered sector to disk array
                        int diskoffset = sectordata2[indexS1].trackhead * sectorspertrack * 512 + sectordata2[indexS1].sector * 512;
                        SectorMap.sectorok[sectordata2[indexS1].trackhead, sectordata2[indexS1].sector] = SectorMapStatus.ErrorCorrected; // Error corrected (shows up as 'c')
                        for (i = 0; i < bytespersector; i++)
                        {
                            disk[i + diskoffset] = data[i + 4];
                        }
                        SectorMap.RefreshSectorMap();
                        TBReceived.Append("\r\n");
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
            TBReceived.Append("Combinations:" + combinations + "\r\n");
        }

        public void PrintArray(int[] a, int length)
        {
            int i;
            for (i = 0; i < length; i++)
            {
                TBReceived.Append(a[i] + ",");
            }
            TBReceived.Append("\r\n");
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
                        return i;
                    }
                }
            }
            if (overflow == 1)
                return -2; // overflow!
            else return -1; // nothing found
        }

        private bool ValidateSectorSize(ref MFMData SectorData)
        {
            if (SectorData.sectorlength > 1024)
            {
                TBReceived.Append("Error: T" + (SectorData.track * 2 + SectorData.head).ToString("d3") + 
                    " S" + SectorData.sector + " size too large: " + SectorData.sectorlength
                    + "\r\n");

                if (ProcSettings.IgnoreHeaderError == true)
                {
                    SectorData.sectorlength = 512;
                    return true;
                }
                return false; // skip to next sector
            }
            return true;
        }

        private bool ValidateSectorHeaderInfo(ref byte[] Idam, ref MFMData SectorHeader)
        {
            // Check header crc
            // If headercrcchk == 0000, header data integrety is good.
            // The CRC is calculated from the A1A1A1FE header to and including the CRC16 checksum bytes.
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
            var headercrcchk = crc.ComputeChecksum(Idam.SubArray(0, 10));

            // trackhead is used for easier index referencing in sectormap[][]
            SectorHeader.trackhead = 1;

            if (headercrcchk == 0)
            {
                GoodSectorHeaderCount++;
                SectorHeader.Status = SectorMapStatus.CrcOk;
                SectorHeader.trackhead = (SectorHeader.track * 2) + SectorHeader.head;
                SectorHeader.MarkerType = MarkerType.header;
                SectorMap.sectorokLatestScan[SectorHeader.trackhead, SectorHeader.sector] = SectorMapStatus.CrcOk;
                return true;
            }
            else
            {
                SectorHeader.Status = SectorMapStatus.HeadOkDataBad;
                return false;
            }
        }

        private void ExtractSectorHeaderInfo(ref byte[] Idam, ref MFMData SectorHeader)
        {
            SectorHeader.track = Idam[4];
            SectorHeader.head = Idam[5];
            SectorHeader.sector = (byte)(Idam[6] - 1);
            SectorHeader.sectorlength = 128 << Idam[7];
        }

        private void HandleIngoreBadHeader(in MFMData SectorHeader, in MFMData SectorData, int threadid, int previoustrack, int previousheadnr)
        {
            int bytespersectorthread = 512;
            var SectorBlock = MFM2Bytes(SectorData.MarkerPositions, bytespersectorthread + 16, threadid);
            ushort datacrcchk = 0xffff;
            byte[] sectorbuf = new byte[0];

            if (debuginfo) TBReceived.Append("\r\n\r\n");
            if (SectorBlock[3] == 0xFB || SectorBlock[3] == 0xF8)
            {
                Crc16Ccitt crc;
                crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
                //Checksum is at the end of the header, so when that is passed into the crc it should result in good = 0x0000
                datacrcchk = crc.ComputeChecksum(SectorBlock.SubArray(0, bytespersectorthread + 6));

                // Todo: seems duplicate of above, doesn't make sense? Can probably be removed.
                if (datacrcchk != 0)
                {
                    datacrcchk = crc.ComputeChecksum(SectorBlock.SubArray(0, 512 + 6));
                }

                sectorbuf = SectorBlock.SubArray(4, bytespersectorthread + 2);
            }
            if (datacrcchk == 0x00 && sectorbuf.Length > 500)
            {
                SectorHeader.trackhead = (previoustrack * 2) + previousheadnr;
                byte headnr = (byte)previousheadnr;
                if (SectorHeader.trackhead < 200)
                {
                    int i;
                    for (i = 0; i < 9; i++)
                    {
                        if (SectorMap.sectorok[SectorHeader.trackhead, i] == SectorMapStatus.empty)
                            break;
                    }

                    // Now assuming the sectornr = i, and we're using previoustrack because
                    // the tracknr info in the header may not be correct
                    byte sectornr = (byte)i;
                    if (SectorMap.sectorok[SectorHeader.trackhead, sectornr] == 0 || SectorMap.sectorok[SectorHeader.trackhead, i] == SectorMapStatus.HeadOkDataBad) // do not overwrite any existing good data
                    {
                        SectorMap.sectorok[SectorHeader.trackhead, sectornr] = SectorMapStatus.DuplicatesFound; // Header is not CRC pass, attempt to reconstuct
                                                                                                                //offset = (track * sectorspertrack * bytespersectorthread * 2) + (previousheadnr * sectorspertrack * bytespersectorthread) + (i * bytespersectorthread);

                        int DiskImageSectorOffset = (previoustrack * sectorspertrack * bytespersectorthread * 2)
                            + (headnr * sectorspertrack * bytespersectorthread)
                            + (sectornr * bytespersectorthread);
                        Array.Copy(sectorbuf, 0, disk, DiskImageSectorOffset, bytespersectorthread);
                    }
                }
            }
        }

        private void SetSectorsPerTrackByDiskFormat()
        {
            if (diskformat == DiskFormat.pcdd) // DD
                sectorspertrack = 9;
            else if (diskformat == DiskFormat.pchd) // HD
                sectorspertrack = 18;
            else if (diskformat == DiskFormat.pc2m) //2M
                sectorspertrack = 11;
        }

        private void FindAllPCMarkers(int threadid)
        {
            // Find markers

            byte[] m = mfms[threadid];
            int rxbufcnt, searchcnt;
            int retries;
            byte[] A1markerbytes = A1MARKER;
            int markerpositionscntthread = 0;
            // with multithreading, the counting of rxbuf is misaligned with mfms, because
            // it doesn't start at the start of rxbuf. There's no way to know at this point
            // how many 1's there are up to this point. Either do the calculation after processing
            // then adjust the values or not use multithreading.

            //m = mfms[threadid]; // speed up processing by removing one extra reference
            int A1MarkerLength = A1markerbytes.Length;
            int A1MarkerLengthMinusOne = A1markerbytes.Length - 1;
            int mfmlength = mfmlengths[threadid];
            if (Indexrxbuf > RxBbuf.Length) Indexrxbuf = RxBbuf.Length - 1;
            rxbufcnt = 0;
            searchcnt = 0;
            for (int i = 0; i < mfmlength; i++)
            {
                if (i % 250000 == 249999) { progresses[threadid] = i; if (stop == 1) break; }

                for (int j = 0; j < A1MarkerLength; j++)
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
                        MFMData sectordata = new MFMData
                        {
                            MarkerPositions = i,
                            rxbufMarkerPositions = rxbufcnt,
                            processed = false,
                            threadid = threadid
                        };
                        retries = 0;

                        while (!sectordata2.TryAdd(sectordata2.Count, sectordata) || retries > 100)
                        {
                            if (Debuglevel > 9)
                                TBReceived.Append("Failed to add to Sectordata dictionary " + markerpositionscntthread + "\r\n");
                            retries++;
                        }
                        markerpositionscntthread++;
                    }
                }
                if (mfms[threadid][i] == 1) // counting 1's matches the number of bytes in rxbuf + start offset
                    rxbufcnt++;
                if (RxBbuf.Length > rxbufcnt)
                    while (RxBbuf[rxbufcnt] < 4 && rxbufcnt < Indexrxbuf - 1) rxbufcnt++;
            }
        }

        private void Detect2MDiskFormat(in MFMData SectorHeader, byte[] Sectorbuf)
        {
            if (SectorHeader.track == 0 && SectorHeader.sector == 0 && diskformat == DiskFormat.pchd) // From HD to 2M, reset sectormap.sectorok
            {
                if (Sectorbuf[401] == '2' && Sectorbuf[402] == 'M') // Detect 2M format
                {
                    diskformat = DiskFormat.pc2m; // Assume 2M
                    sectorspertrack = 11;
                    // When we're in the process and only detecting 2M after many other sectors were processed,
                    // the previous sectors were processed with assumption diskformat = HD. We need to invalidate all found sectors
                    // except T000
                    for (int i = 0; i < 200; i++)
                    {
                        for (int j = 0; j < 25; j++)
                        {
                            SectorMap.sectorok[i, j] = SectorMapStatus.empty;
                        }
                    }
                }
            }
        }

        private void HandleErrorCorrection( ref MFMData SectorHeader, 
                                            ref MFMData SectorData, 
                                            in byte[] sectorbuf, 
                                            in byte[] SectorBlock, 
                                            in ProcSettings procsettings, 
                                            int markerindex,
                                            ushort datacrc,
                                            int threadid, bool BadSector=false)
        {
            if (procsettings.UseErrorCorrection)
            {
                //Create hash
                SHA256 mySHA256 = SHA256Managed.Create();
                sectorbuf[SectorHeader.sectorlength] = (byte)SectorHeader.trackhead;
                sectorbuf[SectorHeader.sectorlength + 1] = (byte)SectorHeader.sector;
                byte[] secthash = mySHA256.ComputeHash(sectorbuf);

                // Check if there's a duplicate
                int isunique = -1;
                // Todo: check thread safety and if deduplication is still a thing
                if (procsettings.finddupes)
                    for (int i = 0; i < sectordata2.Count; i++)
                    {
                        //isunique = Array.FindIndex(badsectorhash, (x => x.Equals(secthash)));
                        isunique = IndexOfBytes(badsectorhash[i], secthash, 0, 32);
                        if (isunique != -1)
                        {
                            break;
                        }
                    }

                if (isunique == -1 || !procsettings.finddupes)
                {
                    byte[] b = new byte[SectorHeader.sectorlength + 16];
                    Array.Copy(SectorBlock, b, SectorHeader.sectorlength + 16);
                    SectorData.Status = SectorMapStatus.CrcOk; // 1 = good sector data
                    if ( BadSector )
                        SectorData.Status = SectorMapStatus.HeadOkDataBad; // 2 = bad sector data
                    
                    badsectorhash[markerindex] = secthash;
                    if (threadid != SectorHeader.threadid)
                        TBReceived.Append("threadid mismatch!\r\n");
                    
                    SectorData.track = SectorHeader.track;
                    SectorData.trackhead = SectorHeader.trackhead;
                    SectorData.sector = SectorHeader.sector;
                    SectorData.sectorlength = SectorHeader.sectorlength;
                    SectorData.crc = (int)datacrc;
                    SectorData.sectorbytes = b;
                    SectorData.MarkerType = MarkerType.data;
                    SectorHeader.MarkerType = MarkerType.header;
                    SectorHeader.DataIndex = markerindex;
                }
            }
        }

        private void DetectDDOrHD(MFMData SectorHeader)
        {
            if (SectorHeader.sector > 8 && diskformat != DiskFormat.pc2m) // from DD to HD
            {
                diskformat = DiskFormat.pchd; // Assume HD
                sectorspertrack = 18;
            }
        }

        private void DetectDiskFormat(MFMData SectorHeader, byte[] sectorbuf)
        {
            DetectDDOrHD(SectorHeader);
            Detect2MDiskFormat(SectorHeader, sectorbuf);
        }

        private void ReportFoundSector(in MFMData SectorHeader, in ProcSettings procsettings, ushort datacrcchk)
        {
            FoundGoodSectorInfo.Append("T" + SectorHeader.track.ToString("D3") + " S" + SectorHeader.sector + " crc:" +
                                 datacrcchk.ToString("X4") + " rxbufindex:" + SectorHeader.rxbufMarkerPositions + " Method: ");

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
        }

        private void SaveSectorToDiskImage(ref MFMData SectorHeader, int DiskImageSectorOffset, in byte[] sectorbuf )
        {
            int sum = 0;
            // 2M first track sector length is always 512
            if (SectorHeader.trackhead == 0) SectorHeader.sectorlength = 512;
            if (DiskImageSectorOffset < 2000000 - SectorHeader.sectorlength)
            {
                if (sectorbuf.Length == SectorHeader.sectorlength + 2)
                {
                    Array.Copy(sectorbuf, 0, disk, DiskImageSectorOffset, SectorHeader.sectorlength);
                    sum = sectorbuf.Sum(x => x);
                    if (sum == 0) SectorMap.sectorok[SectorHeader.trackhead, SectorHeader.sector] = SectorMapStatus.SectorOKButZeroed; // If the entire sector is zeroes, allow new data
                }
            }
        }

        private void FindNextMarker(ref int markerindex, int threadid, out MFMData SectorData)
        {
            SectorData = null;
            // Find the next marker which should be the sector data
            // Make sure the correct threadid is used since other threads also add to the
            // dictionary. They are sequential though so we only need to search from markerindex
            try
            {
                int q;
                for (q = markerindex + 1; q < sectordata2.Count; q++)
                {
                    if (sectordata2[q].threadid == threadid)
                    {
                        markerindex = q;

                        SectorData = sectordata2[q];
                        break;
                    }
                }
            }
            catch (Exception)
            {
                TBReceived.Append("Error: could not find sectordata: " + markerindex + "\r\n");
                
            }
            
        }

        private void ValidateSectorDataBlock(ref ushort datacrcchk, 
                                             ref ushort datacrc, 
                                             ref byte[] SectorBlock, 
                                             ref byte[] sectorbuf,
                                             ref MFMData SectorHeader, 
                                             ref MFMData SectorData)
        {
            datacrc = (ushort)((SectorBlock[SectorHeader.sectorlength + 4] << 8) | SectorBlock[SectorHeader.sectorlength + 5]);
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
            datacrcchk = crc.ComputeChecksum(SectorBlock.SubArray(0, SectorHeader.sectorlength + 6));
            if (datacrcchk != 0)
            {
                datacrcchk = crc.ComputeChecksum(SectorBlock.SubArray(0, 512 + 6));
                SectorData.Status = SectorMapStatus.CrcBad;
            }
            else
            {
                SectorData.Status = SectorMapStatus.CrcOk;
            }
        }

        private void StripSectorOfHeader(in byte[] SectorBlock, out byte[] sectorbuf, in MFMData SectorHeader)
        {
            if (diskformat == DiskFormat.pc2m && SectorHeader.trackhead == 0)
                sectorbuf = SectorBlock.SubArray(4, 514);
            else sectorbuf = SectorBlock.SubArray(4, SectorHeader.sectorlength + 2);
        }

    } // FDDProcessing end
}

