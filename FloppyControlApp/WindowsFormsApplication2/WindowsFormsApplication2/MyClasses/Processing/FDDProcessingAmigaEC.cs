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
       
        public void ProcessClusterAmigaMFMEnc(ECSettings ecSettings)
        {
            int i;

            byte[] Marker = new byte[0];
            int bitshifted = 0;
            int periodSelectionStart;
            int periodSelectionEnd;
            int mfmAlignedStart, mfmAlignedEnd;

            int[] combi = new int[32];
            int combilimit;

            int indexS1 = ecSettings.indexS1;

            mfmAlignedStart = ecSettings.MFMByteStart;
            mfmAlignedEnd = mfmAlignedStart + (ecSettings.MFMByteLength * 8);
            MFMByteEncPreset mfmpreset = new MFMByteEncPreset();

            if (diskformat == DiskFormat.amigados)
            {
                Marker = FDDProcessing.AMIGAMARKER;
                //periodshift = 16;
            }
            else
            if (diskformat == DiskFormat.diskspare)
            {
                Marker = FDDProcessing.AMIGADSMARKER;
                //periodshift = 8;
            }

            if (Marker.Length == 0)
            {
                TBReceived.Append("No amiga format found!\r\n");
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
                TBReceived.Append("Selection too large, please make it smaller, 50 max.\r\n");
                return;
            }

            // Copy mfm data from mfms
            int sectorlength = sectordata2[indexS1].sectorlength;


            byte[] mfmbuf = mfms[sectordata2[indexS1].threadid].SubArray(sectordata2[indexS1].MarkerPositions, (sectorlength + 100) * 16);

            TBReceived.Append("mfmAlignedstart: " + mfmAlignedStart + " mfmAlignedEnd: " + mfmAlignedEnd + "\r\n");

            // Find 4E right after the crc bytes at the end of the sector
            // 4E bytes are padding bytes between header and data. 
            // When the 4E markers are found it will increase the chance of 
            // getting a proper crc, even if it's bit shifted caused by corrupt data

            int Markerindex = FindMarker(ref mfmbuf, mfmbuf.Length, (sectorlength) + 4 * 16, ref Marker);
            // The number of bits shifted with regards to where the 4E padding should or expected to be
            if (Markerindex == -1)
            {
                TBReceived.Append("Marker not found. Can't continue.\r\n");
                return;
            }
            if (diskformat == DiskFormat.amigados)
                bitshifted = Markerindex - 8736;
            else if (diskformat == DiskFormat.diskspare)
                bitshifted = Markerindex - 8336;

            TBReceived.Append("Bitshift: " + bitshifted + "\r\n");

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

                TBReceived.Append("Iterations: " + iterations + "\r\n");
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
                        datachecksum = AmigaMfmDecodeBytes(mfmaligned, (56 * 8), 4 * 16); // At uint 6
                        data = AmigaMfmDecodeBytes(mfmaligned, (64 * 8), 512 * 16);
                        checksum = AmigaChecksum(mfmaligned, (64 * 8), 512 * 16); // Get header checksum from sector header

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
                        byte[] dec1 = AmigaMfmDecodeBytes(mfmaligned, 8 * 8, 4 * 16); //At uint 0

                        //tracknr = data[0];
                        //sectornr = data[1];
                        dsdatachecksum[0] = dec1[2];
                        dsdatachecksum[1] = dec1[3];
                        dsdatachecksum[2] = 0;
                        dsdatachecksum[3] = 0;

                        uint dchecksum;
                        uint offset;
                        byte[] tmp;

                        for (p = 0; p < 512; p += 4)
                        {
                            tmp = AmigaMfmDecodeBytes(mfmaligned, (int)p * 16 + 16 * 8, 4 * 16);
                            data[p] = tmp[0];
                            data[p + 1] = tmp[1];
                            data[p + 2] = tmp[2];
                            data[p + 3] = tmp[3];
                        }

                        dchecksum = (uint)((Mfm2UShort(mfmaligned, 8 * 16)) & 0x7FFF);
                        ushort tmp1;
                        offset = 9;
                        for (p = (int)offset; p < 520; p++)
                        {
                            tmp1 = Mfm2UShort(mfmaligned, (int)(p * 16));
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
                        TBReceived.Append("CRC ok! iteration: " + combinations + "\r\n");
                        PrintArray(combi, NumberOfMfmBytes);
                        for (i = 0; i < 512; i++)
                        {
                            TBReceived.Append(data[i].ToString("X2") + " ");
                            if (i % 16 == 15) TBReceived.Append("\r\n");
                            if (i == mfmAlignedStart / 16 || i == mfmAlignedEnd / 16) TBReceived.Append("--");
                            //dat[offset + i] = data[offset + i];
                        }
                        //tbreceived.Append("\r\n\r\nc6_max:" + c6_max + " c8_max:" + c8_max + "\r\n");
                        TBReceived.Append("Time: " + sw.ElapsedMilliseconds + "ms\r\n");
                        //Save recovered sector to disk array
                        int diskoffset = sectordata2[indexS1].trackhead * sectorspertrack * 512 + sectordata2[indexS1].sector * 512;
                        SectorMap.sectorok[sectordata2[indexS1].track, sectordata2[indexS1].sector] = SectorMapStatus.ErrorCorrected; // Error corrected (shows up as 'c')
                        for (i = 0; i < bytespersector; i++)
                        {
                            disk[i + diskoffset] = data[i];
                        }
                        //sectormap.RefreshSectorMap();
                        TBReceived.Append("\r\n");
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
            TBReceived.Append("Combinations:" + combinations + "\r\n");

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
            int bitshifted = 0;
            int periodSelectionStart, mfmSelectionStart = 0;
            int periodSelectionEnd, mfmSelectionEnd = 0;
            int bytestart, byteend;
            int mfmAlignedStart, mfmAlignedEnd;
            int indexS1 = ecSettings.indexS1;

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
                TBReceived.Append("No amiga format found!\r\n");
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
                TBReceived.Append("Selection too large, please make it smaller, 50 max.\r\n");
                return;
            }

            // Copy mfm data from mfms
            int sectorlength = sectordata2[indexS1].sectorlength;


            byte[] mfmbuf = mfms[sectordata2[indexS1].threadid].SubArray(sectordata2[indexS1].MarkerPositions, (sectorlength + 100) * 16);

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

            int Markerindex = FindMarker(ref mfmbuf, mfmbuf.Length, (sectorlength) + 4 * 16, ref Marker);
            // The number of bits shifted with regards to where the 4E padding should or expected to be
            if (Markerindex == -1)
            {
                TBReceived.Append("Marker not found. Can't continue.\r\n");
                return;
            }
            if (diskformat == DiskFormat.amigados)
                bitshifted = Markerindex - 8736;
            else if (diskformat == DiskFormat.diskspare)
                bitshifted = Markerindex - 8336;

            TBReceived.Append("Bitshift: " + bitshifted + "\r\n");

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

            TBReceived.Append("mfmSelectionStart:" + mfmSelectionStart + " mfmselectionendAligned: " + (mfmSelectionEnd + bitshifted) + "\r\n");

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

                        int datacrcchk = 0;
                        byte[] checksum;
                        byte[] datachecksum;

                        if (diskformat == DiskFormat.amigados)
                        {
                            datachecksum = AmigaMfmDecodeBytes(mfmaligned, (56 * 8), 4 * 16); // At uint 6
                            data = AmigaMfmDecodeBytes(mfmaligned, (64 * 8), 512 * 16);
                            checksum = AmigaChecksum(mfmaligned, (64 * 8), 512 * 16); // Get header checksum from sector header

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
                            byte[] dec1 = AmigaMfmDecodeBytes(mfmaligned, 8 * 8, 4 * 16); //At uint 0

                            //tracknr = data[0];
                            //sectornr = data[1];
                            dsdatachecksum[0] = dec1[2];
                            dsdatachecksum[1] = dec1[3];
                            dsdatachecksum[2] = 0;
                            dsdatachecksum[3] = 0;

                            uint dchecksum;
                            uint offset;
                            byte[] tmp;

                            for (j = 0; j < 512; j += 4)
                            {
                                tmp = AmigaMfmDecodeBytes(mfmaligned, (int)j * 16 + 16 * 8, 4 * 16);
                                data[j] = tmp[0];
                                data[j + 1] = tmp[1];
                                data[j + 2] = tmp[2];
                                data[j + 3] = tmp[3];
                            }

                            dchecksum = (uint)((Mfm2UShort(mfmaligned, 8 * 16)) & 0x7FFF);
                            ushort tmp1;
                            offset = 9;
                            for (j = (int)offset; j < 520; j++)
                            {
                                tmp1 = Mfm2UShort(mfmaligned, (int)(j * 16));
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
                            TBReceived.Append("FindBruteForce: Correction found! q = " + p + "Count: " + detectioncnt + "\r\nData: ");
                            for (i = 0; i < 512; i++)
                            {
                                TBReceived.Append(data[i].ToString("X2") + " ");
                                if (i % 16 == 15) TBReceived.Append("\r\n");
                                if (i == mfmSelectionStart / 16 || i == mfmSelectionEnd / 16) TBReceived.Append("--");
                                //dat[offset + i] = data[offset + i];
                            }
                            TBReceived.Append("\r\n\r\nc6_max:" + c6_max + " c8_max:" + c8_max + "\r\n");
                            TBReceived.Append("Time: " + sw.ElapsedMilliseconds + "ms\r\n");
                            //Save recovered sector to disk array
                            int diskoffset = sectordata2[indexS1].trackhead * sectorspertrack * 512 + sectordata2[indexS1].sector * 512;
                            SectorMap.sectorok[sectordata2[indexS1].track, sectordata2[indexS1].sector] = SectorMapStatus.ErrorCorrected; // Error corrected (shows up as 'c')
                            for (i = 0; i < bytespersector; i++)
                            {
                                disk[i + diskoffset] = data[i];
                            }
                            SectorMap.RefreshSectorMap();
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

        }

        public ECResult ProcessRealignAmiga(ECSettings ecSettings)
        {
            int i;

            byte[] Marker = new byte[0];
            int periodshift = 0;
            int bitshifted = 0;
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
                TBReceived.Append("No amiga format found!\r\n");
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
                TBReceived.Append("Selection too large, please make it smaller, 50 max.\r\n");
                return null;
            }

            // Copy mfm data from mfms
            int sectorlength = sectordata2[indexS1].sectorlength;

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

            int Markerindex = FindMarker(ref mfmbuf, mfmbuf.Length, (sectorlength) + 4 * 16, ref Marker);
            // The number of bits shifted with regards to where the 4E padding should or expected to be
            if (Markerindex == -1)
            {
                TBReceived.Append("Marker not found. Can't continue.\r\n");
                return null;
            }
            if (diskformat == DiskFormat.amigados)
                bitshifted = Markerindex - 8736;
            else if (diskformat == DiskFormat.diskspare)
                bitshifted = Markerindex - 8336;

            TBReceived.Append("Bitshift: " + bitshifted + "\r\n");

            mfmSelectionEnd -= bitshifted;

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
            int end;
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

            TBReceived.Append("mfmSelectionStart:" + mfmSelectionStart + " mfmselectionendAligned: " + (mfmSelectionEnd + bitshifted) + "\r\n");
            //int datacrcchk;
            byte[] checksum;
            byte[] datachecksum;
            stop = 0;
            // Brute force with weighing of 4/6/8us


            if (diskformat == DiskFormat.amigados)
            {
                datachecksum = AmigaMfmDecodeBytes(mfmaligned, (56 * 8), 4 * 16); // At uint 6
                data = AmigaMfmDecodeBytes(mfmaligned, (64 * 8), 512 * 16);
                checksum = AmigaChecksum(mfmaligned, (64 * 8), 512 * 16); // Get header checksum from sector header

                // Do the data checksum check:
                // Todo, actually use datacrcchk or can it be removed?
                if (datachecksum.SequenceEqual(checksum)) // checksum is changed everytime amigamfmdecode() is called
                {
                    //datacrcchk = 1;
                }
            }
            else if (diskformat == DiskFormat.diskspare)
            {
                byte[] dsdatachecksum = new byte[4];

                // Get track sector and checksum within one uint32: 0xTTSSCCCC
                byte[] dec1 = AmigaMfmDecodeBytes(mfmaligned, 8 * 8, 4 * 16); //At uint 0

                dsdatachecksum[0] = dec1[2];
                dsdatachecksum[1] = dec1[3];
                dsdatachecksum[2] = 0;
                dsdatachecksum[3] = 0;

                uint dchecksum;
                uint offset;
                byte[] tmp;

                for (int j = 0; j < 512; j += 4)
                {
                    tmp = AmigaMfmDecodeBytes(mfmaligned, (int)j * 16 + 16 * 8, 4 * 16);
                    data[j] = tmp[0];
                    data[j + 1] = tmp[1];
                    data[j + 2] = tmp[2];
                    data[j + 3] = tmp[3];
                }

                dchecksum = (uint)((Mfm2UShort(mfmaligned, 8 * 16)) & 0x7FFF);
                ushort tmp1;
                offset = 9;
                for (int j = (int)offset; j < 520; j++)
                {
                    tmp1 = Mfm2UShort(mfmaligned, (int)(j * 16));
                    dchecksum ^= (uint)(tmp1 & 0xffff);
                }
                byte[] savechecksum = new byte[4];
                savechecksum[0] = (byte)(dchecksum >> 8);
                savechecksum[1] = (byte)(dchecksum & 0xFF);
                savechecksum[2] = 0;
                savechecksum[3] = 0;

                // Do the data checksum check:
                //if (dsdatachecksum.SequenceEqual(savechecksum)) // checksum is changed everytime amigamfmdecode() is called
                    //datacrcchk = 1;
                //else datacrcchk = 0;
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
                sectorbytes = data
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
        } // End ProcessRealignAmiga

    }
}
