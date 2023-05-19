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
using FDCPackage;
using FloppyControlApp.MyClasses.Processing;
using FloppyControlApp.MyClasses.Processing.ProcessingTypes;
using static FloppyControlApp.MyClasses.Processing.ProcessingTypes.ProcessingTypes;

namespace FloppyControlApp
{
    public partial class FDDProcessing
    {
        public ConcurrentDictionary<int, MFMData> sectordata2;
        public byte[] disk = new byte[2000000]; // Assuming a disk isn't larger than 2MB, which is false for EHD disks. 
        public DiskFormat diskformat = 0;// 0x1 = ADOS/PFS, 0x02 = DiskSpare, 0x03 = PCDD, 0x04 = PCHD
        public ProcSettings ProcSettings { get; set; }
        public SectorMap SectorMap { get; set; }
        private System.Diagnostics.Stopwatch SW = new System.Diagnostics.Stopwatch();
        public StringBuilder TBReceived { get; set; }
        public StringBuilder FoundGoodSectorInfo = new StringBuilder();
        public string CurrentFiles;
        public RichTextBox RtbSectorMap { get; set; } // for sectormap
        private BinaryWriter writer;
        public byte[] RxBbuf { get; set; } // This is a super large 200MB buffer to hold timing data captured by the floppy controller
        public int Indexrxbuf { get; set; }
        public int[] Rxbuftograph { get; set; }

        // Index adress marker
        //public string A1MARKER = "010001001000100101000100100010010100010010001001"; 
        //                          94  6C 94  6C 4994  6C 94  6C 4994  6C 94  6C
        //                         000010100000101000001010 
        //                             101000011010000110100001
        //private string IAM =  "0101001000100100010100100010010001010010001001000101010101010010"; // Index adress marker
        //private string IDAM = "0100010010001001010001001000100101000100100010010101010101010100"; // ID adress marker 4489 4489 4489 5554
        //private string DAM =  "0100010010001001010001001000100101000100100010010101010101000101"; // data adres marker 4489 4489 4489 5545
        //private string AMIGAMARKER = "1010101010101010101010101010101001000100100010010100010010001001";

        //public string AMIGADSMARKER = "010001001000100101000100100010010010101010101010";
        public int Debuglevel { get; set; }
        public byte[][] mfms = new byte[50000][]; // replaces mfm array, dynamically allocating array. threadid is the key
        public int mfmsindex = 0;
        public int[] mfmlengths = new int[50000]; // replaces mfmlength, the length of data in mfms. threadid is the key

        public byte[][] badsectorhash = new byte[500000][]; // (badsectorcnt)

        //public int[] rxbufMarkerPositions = new int[5000000];  // the index var is markerpositionscnt This is rxbuf
        public int[] progresses = new int[50000]; // keeps track of the progress of different threads, threadid is the key
        public int[] progressesstart = new int[50000]; // threadid is the key
        public int[] progressesend = new int[50000]; // threadid is the key
        public string[] ProcessStatus = new string[50000]; // threadid is the key
        public int sectorspertrack = 0;
        public int bytespersector = 512; // 1024 for 2M, 512 for everything else
        public int stop;
        private Thread[] Threads { get; set; }
        public int NumberOfThreads { get; set; }
        public int processing = 0;
        public Dictionary<DiskFormat, DiskGeometry> diskGeometry = new Dictionary<DiskFormat, DiskGeometry>();

        public float[] entropy;
        public float[] threshold4;
        public float[] threshold6;
        public float[] threshold8;

        public FDDProcessing()
        {
            Debuglevel = 1;
            Indexrxbuf = 0;
            SectorMap = new SectorMap(RtbSectorMap, this);
            stop = 0;
            NumberOfThreads = 1;
            RxBbuf = new byte[200000];

            ProcSettings = new ProcSettings();
            int numProcs = Environment.ProcessorCount;
            int concurrencyLevel = numProcs * 2;
            sectordata2 = new ConcurrentDictionary<int, MFMData>(concurrencyLevel, 100);

            //PC
            DiskGeometry pcdosdsdd = new DiskGeometry
            {
                numberOfHeads = 2,
                tracksPerDisk = 80,
                sectorsPerTrack = 9,
                sectorSize = 512,
                fluxEncoding = FluxEncoding.MFM
            };
            diskGeometry.Add(DiskFormat.pcdd, pcdosdsdd);

            DiskGeometry pcdoshd = new DiskGeometry
            {
                numberOfHeads = 2,
                tracksPerDisk = 80,
                sectorsPerTrack = 18,
                sectorSize = 512,
                fluxEncoding = FluxEncoding.MFM
            };
            diskGeometry.Add(DiskFormat.pchd, pcdoshd);

            DiskGeometry pcdosssdd = new DiskGeometry
            {
                numberOfHeads = 1,
                tracksPerDisk = 80,
                sectorsPerTrack = 9,
                sectorSize = 512,
                fluxEncoding = FluxEncoding.MFM
            };
            diskGeometry.Add(DiskFormat.pcssdd, pcdosssdd);

            // I'm not sure, I'll have to check these settings
            DiskGeometry pcdos2m = new DiskGeometry
            {
                numberOfHeads = 2,
                tracksPerDisk = 80,
                sectorsPerTrack = 12,
                sectorSize = 1024,
                fluxEncoding = FluxEncoding.MFM
            };
            diskGeometry.Add(DiskFormat.pc2m, pcdos2m);

            //Amiga
            DiskGeometry amigados = new DiskGeometry
            {
                numberOfHeads = 2,
                tracksPerDisk = 80,
                sectorsPerTrack = 11,
                sectorSize = 512,
                fluxEncoding = FluxEncoding.MFM
            };
            diskGeometry.Add(DiskFormat.amigados, amigados);

            // My own extended diskspare format
            DiskGeometry amigadiskspareE = new DiskGeometry
            {
                numberOfHeads = 2,
                tracksPerDisk = 82,
                sectorsPerTrack = 12,
                sectorSize = 512,
                fluxEncoding = FluxEncoding.MFM
            };
            diskGeometry.Add(DiskFormat.diskspare984KB, amigadiskspareE);

            DiskGeometry amigadiskspare = new DiskGeometry
            {
                numberOfHeads = 2,
                tracksPerDisk = 80,
                sectorsPerTrack = 12,
                sectorSize = 512,
                fluxEncoding = FluxEncoding.MFM
            };
            diskGeometry.Add(DiskFormat.diskspare, amigadiskspare);
        }

        ~FDDProcessing()
        {
            for (var i = 0; i < mfms.Length; i++)
                mfms[i] = null;
            RxBbuf = null;
            disk = null;

            mfmlengths = null;
            badsectorhash = null;
            progresses = null;
            progressesstart = null;
            progressesend = null;
            ProcessStatus = null;
        }

        public void ClearNonBadSectors()
        {
            int i;
            for (i = 0; i < sectordata2.Count; i++)
            {
                if (sectordata2[i].Status != SectorMapStatus.HeadOkDataBad)
                {
                    sectordata2.TryRemove(i, out _);
                }
                else
                if (sectordata2[i].Status == SectorMapStatus.HeadOkDataBad)
                {
                    if (sectordata2[i].sectorbytes.Length == 0)
                        sectordata2.TryRemove(i, out _);
                }
            }
        }

        /// <summary>
        /// Increases the rxbuf size if the requested length is larger. Returns true if succesful.
        /// </summary>
        /// <param name="length"></param>
        /// <returns>true if succesful</returns>
        public bool IncreaseBufSize(int length)
        {
            int sizediff = length - RxBbuf.Length;

            if (sizediff > 0)
            {
                List<byte[]> tempbuffer = new List<byte[]>();
                byte[] addbuffer = new byte[sizediff];

                tempbuffer.Add(RxBbuf);
                tempbuffer.Add(addbuffer);
                RxBbuf = tempbuffer.SelectMany(a => a).ToArray();

                addbuffer = null;
                tempbuffer.Clear();
                GC.Collect();
                return true;
            }
            return false;
        }

        public void StartProcessing(Platform platform)
        {
            int threadid = 0, t, i;
            SW.Reset();
            SW.Start();
            Threads = null;
            //ProcessingType processingtype = ProcessingType.normal;

            if (GetProcSettingsCallback != null) GetProcSettingsCallback();
            else
            {
                TBReceived.Append("StartProcessing() GetProcSettingsCallback not set!\r\n");
                return;
            }
            //LoadGoodSectorFoundInfo();
            ProcSettings.platform = platform;
            if (stop == 0)
            {

                //Get the values from slider controls:

                StringBuilder outputfast = new StringBuilder();
                if (Threads == null)
                    Threads = new Thread[NumberOfThreads + 1];
                if (Threads.Length < NumberOfThreads)
                    Threads = new Thread[NumberOfThreads + 1];


                ProcessStatus[threadid] = "Period to MFM conversion...";
                progressesstart[threadid] = ProcSettings.start;
                progressesend[threadid] = ProcSettings.end;

                //int sectdataitems = 0;

                // Process one track/sector combo only, based on sectordata
                if (ProcSettings.OnlyBadSectors)
                {
                    int limittotrack = ProcSettings.limittotrack;
                    int limittosector = ProcSettings.limittosector;
                    int oldindexrxbuf = Indexrxbuf;

                    int mfmsindexold = mfmsindex;

                    for (i = 0; i < sectordata2.Count; i++)
                    {
                        if (!sectordata2.IsEmpty)
                        {
                            if (sectordata2[i].Status == SectorMapStatus.HeadOkDataBad)
                            {
                                if (SectorMap.sectorok[sectordata2[i].trackhead, sectordata2[i].sector] == SectorMapStatus.HeadOkDataBad)
                                {
                                    if (ProcSettings.LimitTSOn)
                                    {
                                        if (sectordata2[i].trackhead == ProcSettings.limittotrack && sectordata2[i].sector == ProcSettings.limittosector)
                                        {

                                            int q;
                                            int rxstart = (sectordata2[i].rxbufMarkerPositions - 500);
                                            if (oldindexrxbuf + 8600 > RxBbuf.Length)
                                            {
                                                List<byte[]> tempbuffer = new List<byte[]>();
                                                byte[] addbuffer = new byte[10000000];

                                                tempbuffer.Add(RxBbuf);
                                                tempbuffer.Add(addbuffer);
                                                RxBbuf = tempbuffer.SelectMany(a => a).ToArray();

                                                addbuffer = null;
                                                tempbuffer.Clear();
                                                GC.Collect();

                                                TBReceived.Append("Increased buffer by 10MB.\r\n");
                                            }
                                            for (int x = 0; x < ProcSettings.NumberOfDups; x++)
                                                for (q = 0; q < 8500; q++)
                                                    RxBbuf[oldindexrxbuf++] = RxBbuf[rxstart + q];
                                            if (oldindexrxbuf > RxBbuf.Length + 1) break;
                                        }
                                    }
                                    else // no T/S limit, process all bad sectors
                                    {
                                        //tbreceived.Append("Start: " + sectordata2[i].rxbufMarkerPositions + "\r\n");
                                        int q;
                                        int rxstart = (sectordata2[i].rxbufMarkerPositions - 500);
                                        if (rxstart < 0) rxstart = 0;
                                        // if the buffer is not large enough, add 10MB
                                        if (oldindexrxbuf + 8600 > RxBbuf.Length)
                                        {
                                            List<byte[]> tempbuffer = new List<byte[]>();
                                            byte[] addbuffer = new byte[10000000];

                                            tempbuffer.Add(RxBbuf);
                                            tempbuffer.Add(addbuffer);
                                            RxBbuf = tempbuffer.SelectMany(a => a).ToArray();

                                            addbuffer = null;
                                            tempbuffer.Clear();
                                            GC.Collect();

                                            TBReceived.Append("Increased buffer by 10MB.\r\n");
                                        }
                                        for (q = 0; q < 8500; q++)
                                        {
                                            if (oldindexrxbuf < RxBbuf.Length - 1)
                                                RxBbuf[oldindexrxbuf++] = RxBbuf[rxstart + q];
                                            else break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (oldindexrxbuf == Indexrxbuf)
                    {
                        stop = 1;
                        SectorMap.RefreshSectorMap();
                    }

                    ProcSettings.start = Indexrxbuf;
                    ProcSettings.end = oldindexrxbuf;
                    //SW.Restart();
                    //tbreceived.Append("before thread:"+SW.ElapsedMilliseconds + "ms\r\n");

                    // Define and start threads
                    for (t = 0; t < NumberOfThreads; t++)
                    {
                        // I may need to make more local vars to pass to Period2MFM
                        int t1 = t + mfmsindex; // thread () => is a lambda expression and t is passed by reference, so t will be the last value. This fixes the issue
                        mfmlengths[t1] = 0;

                        //procsettings.start = start + (end / NumberOfThreads * t);
                        //procsettings.end = procsettings.start + (end / NumberOfThreads);
                        progressesstart[t1] = ProcSettings.start;
                        progressesend[t1] = ProcSettings.end;

                        ProcSettings procsettings1 = ProcSettings;

                        Threads[t] = new Thread(() => Period2MFM(procsettings1, t1));
                        Threads[t].Start();
                    }
                    //SW.Restart();
                    //tbreceived.Append("after thread:" + SW.ElapsedMilliseconds + "ms\r\n");
                    // Sync
                    for (t = 0; t < NumberOfThreads; t++)
                    {
                        while (!Threads[t].Join(5))
                        {
                            Application.DoEvents();
                        }
                    }
                    //SW.Restart();
                    //tbreceived.Append("thread synced:" + SW.ElapsedMilliseconds + "ms\r\n");
                    //textBoxReceived.Clear();
                    Application.DoEvents();

                } // not Onlybadsectors
                else
                {
                    //SW.Restart();
                    //tbreceived.Append("before thread:" + SW.ElapsedMilliseconds + "ms\r\n");
                    // Define and start threads
                    int perThreadLength = (ProcSettings.end - ProcSettings.start) / NumberOfThreads;
                    int offset = 0;

                    for (t = 0; t < NumberOfThreads; t++)
                    {
                        // I may need to make more local vars to pass to Period2MFM
                        int t1 = t + mfmsindex; // thread () => is a lambda expression and t is passed by reference, so t will be the last value. This fixes the issue
                        mfmlengths[t1] = 0;

                        ProcSettings ps = new ProcSettings();

                        ps = (ProcSettings)ProcSettings.Clone();

                        ps.start = offset;
                        ps.end = offset + perThreadLength;
                        progressesstart[t1] = ps.start;
                        progressesend[t1] = ps.end;

                        //ProcSettings procsettings1 = procsettings;

                        Threads[t] = new Thread(() => Period2MFM(ps, t1));
                        Threads[t].Start();
                        offset += perThreadLength;
                    }
                    //SW.Restart();
                    //tbreceived.Append("After thread:" + SW.ElapsedMilliseconds + "ms\r\n");
                    // Synch
                    for (t = 0; t < NumberOfThreads; t++)
                    {
                        if(Threads[t]!=null)
                        while (!Threads[t].Join(5))
                        {
                            Application.DoEvents();
                        }
                        //tbreceived.Append("Thread "+t+" in sync!\r\n");
                        //Application.DoEvents();
                    }
                    //SW.Restart();
                    //tbreceived.Append("synced:" + SW.ElapsedMilliseconds + "ms\r\n");
                    Application.DoEvents();
                    mfmsindex += NumberOfThreads;

                    if (ProcSettings.AutoRefreshSectormap)
                        SectorMap.RefreshSectorMap();
                }
            }
            for( i=0; i< NumberOfThreads; i++)
                Threads[0] = null;
            
            SW.Stop();
            TBReceived.Append(SW.ElapsedMilliseconds + "ms\r\n");
            //SaveGoodSectorFoundInfo();
            //tbreceived.Append(relativetime().ToString() + "ms \r\n");
        }

        /// <summary>
        /// Write period data to disk in hex/text format, one value per line to be read in excel for analysis
        /// </summary>
        /// <param name="procsettings"></param>
        public void WriteMFMAsSingleValuePerLine(ProcSettings procsettings)
        {
            int i;
            // Write period data to disk in hex/text format, one value per line to be read in excel for analysis
            string subpath = @Properties.Settings.Default["PathToRecoveredDisks"].ToString();
            string path = subpath + @"\" + procsettings.outputfilename + @"\";
            writer = new BinaryWriter(new FileStream(path + procsettings.outputfilename + ".hex", FileMode.Create));
            for (i = 0; i < Indexrxbuf; i++)
            {
                writer.Write(RxBbuf[i].ToString("X2")[0]);
                writer.Write(RxBbuf[i].ToString("X2")[1]);
                writer.Write('\r');
                writer.Write('\n');
            }
            if (writer != null)
            {
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }

        }

        private void Period2MFM(ProcSettings procsettings, int threadid)
        {
            int i;

            bool writemfm = false;

            int MINUS, FOURUS, SIXUS, EIGHTUS, start, end;
            float RateOfChange;
            System.Diagnostics.Stopwatch SW = new System.Diagnostics.Stopwatch();
            //SW.Stop();
            //tbreceived.Append(SW.ElapsedMilliseconds + "ms\r\n");
            SW.Reset();
            SW.Start();
            MINUS = procsettings.min;
            FOURUS = procsettings.four;
            SIXUS = procsettings.six;
            EIGHTUS = procsettings.max;
            RateOfChange = procsettings.rateofchange;
            start = procsettings.start;
            end = procsettings.end;
            ProcessingType processingtype = procsettings.processingtype;

            // bounds checking
            if (end - start == 0)
            {
                TBReceived.Append("Period2MFM: Error: Length can't be zero.\r\n");
                //stop = 1;
                //PrintProperties(procsettings);
                return;
            }

            int rxbuflength = RxBbuf.Length;
            if (start > rxbuflength || end > rxbuflength)
            {
                TBReceived.Append("Start or end are larger than rxbuf length. Resetting to start=0 and end = rxbuflength\r\n");
                start = 0;
                end = rxbuflength - 1;
            }

            if (end < 0 || start < 0)
            {
                TBReceived.Append("Start or end can't be a negative value!\r\n");
                return;
            }

            TBReceived.Append("Period length:" + (end - start) + " ");
            byte[] m;
            try {
                if (procsettings.AddNoise || procsettings.pattern == 4)
                    m = new byte[((end - start) * 5)]; // mfm data can be max. 4x period data
                else m = new byte[((end - start) * 4)]; // mfm data can be max. 4x period data
            }
            catch(OutOfMemoryException)
            {
                TBReceived.Append("Out of memory. Please load a smaller dataset and try again.");
                return;
            }
            if (MINUS < 0) MINUS = 0;

            var ProctypeArgs = new ProcTypeArgs
            {
                MINUS = MINUS,
                FOURUS = FOURUS,
                SIXUS = SIXUS,
                EIGHTUS = EIGHTUS,
                Rxbuf = RxBbuf,
                Mfmlengths = mfmlengths,
                RateOfChange = RateOfChange,
                Progresses = progresses,
                entropy = entropy,
                Procsettings = procsettings,

                Tbreceived = TBReceived,
            };

            if (processingtype == ProcessingType.adaptive2) //************ Adaptive2 ****************
            {
                var Processingtypes = new ProcessingTypes();
                m = Processingtypes.ProcTypeAdaptive2(ProctypeArgs, threadid, ref stop);
            }
            else
            if (processingtype == ProcessingType.adaptive3) //************ Adaptive 3 ****************
            {
                var Processingtypes = new ProcessingTypes();
                m = Processingtypes.ProcTypeAdaptive3(ProctypeArgs, threadid, ref stop);
            }
            else
            if (processingtype == ProcessingType.adaptive1) //************ Adaptive ****************
            {
                var Processingtypes = new ProcessingTypes();
                AdaptiveEntropyResult result = Processingtypes.ProcTypeAdaptive(ProctypeArgs, threadid, ref stop);
                m = result.M;
                entropy = result.Entropy;
            }
            else
            if (processingtype == ProcessingType.adaptiveEntropy) //************ Adaptive Entropy ****************
            {

                var Processingtypes = new ProcessingTypes();
                AdaptiveEntropyResult result = Processingtypes.ProcTypeAdaptiveEntropy(ProctypeArgs, threadid, ref stop);
                m = result.M;
                entropy = result.Entropy;
                threshold4 = result.Threshold4;
                threshold6 = result.Threshold6;
                threshold8 = result.Threshold8;
            }
            else
            if (processingtype == ProcessingType.adaptivePredict) //************ Adaptive predict ****************
            {
                var Processingtypes = new ProcessingTypes();
                m = Processingtypes.ProcTypeAdaptivePredict(ProctypeArgs, threadid, ref stop);
            }
            else if (processingtype == ProcessingType.normal) //************ Normal ****************
            {
                var Processingtypes = new ProcessingTypes();
                m = Processingtypes.ProcTypeNormal(ProctypeArgs, threadid, ref stop);
            }
            else if (processingtype == ProcessingType.aufit) //************ Aufit ****************
            {
                var Processingtypes = new ProcessingTypes();
                m = Processingtypes.ProcTypeAufit(ProctypeArgs, threadid, ref stop);
            }

            // The last pulse ends here, so no following pulse is there
            // The start and end of a pulse is always a '1'

            progresses[threadid] = end;
            m[mfmlengths[threadid]++] = 1;
            m[mfmlengths[threadid]++] = 0;

            ///if (procsettings.UseErrorCorrection)
            mfms[threadid] = m;
            if (writemfm == true)
            {
                string subpath = @Properties.Settings.Default["PathToRecoveredDisks"].ToString();
                string path = subpath + @"\" + procsettings.outputfilename + @"\";

                bool exists = System.IO.Directory.Exists(path);

                if (!exists)
                    System.IO.Directory.CreateDirectory(path);

                writer = new BinaryWriter(new FileStream(path + @"\" + procsettings.outputfilename + ".mfm", FileMode.Create));

                for (i = 0; i < mfmlengths[threadid]; i++)
                    writer.Write((byte)(mfms[threadid][i] + 48));

                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
            }
            //tbreceived.Append("period to mfm:"+SW.ElapsedMilliseconds + "ms ");
            //SW.Restart();

            // Continue processing MFM data depending on platform
            if (procsettings.platform == Platform.PC)
                ProcessPCMFM2Sectordata(procsettings, threadid, TBReceived);
            else if (procsettings.platform == Platform.Amiga)
                ProcessAmigaMFMbytes(procsettings, threadid);

            //tbreceived.Append("mfm to sector:"+ SW.ElapsedMilliseconds + "ms\r\n");
            SW.Stop();


        }

        public void FindPeaks(int offset)
        {
            byte[] data;

            int length;
            if (Indexrxbuf < 100000)
                length = Indexrxbuf;
            else length = 100000;
            //int offset = HistogramhScrollBar1.Value;
            //int peak1, peak2, peak3;
            data = RxBbuf;

            int i;

            if (data == null) return;

            int[] histogramint = new int[256];

            int histogrammax;

            if (length == 0) length = data.Length;

            //Create histogram of the track period data

            // count the period lengths grouped by period length, skip 0
            for (i = offset; i < offset + length; i++)
            {
                //if (data[i] > 0)
                histogramint[data[i]]++;
            }

            // Find the maximum value so we can normalize the histogram down to 100 to fit inside histogram graph

            Peak1 = 0;
            Peak2 = 0;
            Peak3 = 0;

            histogrammax = 0;
            for (i = 1; i < 256; i++)
            {
                if (histogramint[i] > histogrammax)
                {
                    histogrammax = histogramint[i];
                    Peak1 = i;
                }
            }

            histogrammax = 0;
            for (i = Peak1 + 20; i < 256; i++)
            {
                if (histogramint[i] > histogrammax)
                {
                    histogrammax = histogramint[i];
                    Peak2 = i;
                }
            }

            histogrammax = 0;
            for (i = Peak2 + 20; i < 256; i++)
            {
                if (histogramint[i] > histogrammax)
                {
                    histogrammax = histogramint[i];
                    Peak3 = i;
                }
            }


            TBReceived.Append("Peak1: " + Peak1.ToString("X2") + "Peak2: " + Peak2.ToString("X2") + "Peak3: " + Peak3.ToString("X2") + "\r\n");
        }

        /*
        // mfmbits[] contains mfmbits encoded in single bytes 0 and 1 per byte
        // offset is the offset within mfmbits
        // length is the number of bytes returned. For every decoded byte two bytes of binary encoded MFM are needed
        // So the method returns byte pairs. Output always dividable by 2
        public byte[] MFM2ByteEncodedMFM(ref byte[] mfmbits, int offset, int length)
        {
            byte[] hex = new byte[length];
            int i, j;

            for (j = 0; j < length; j++)
            {
                for (i = 0; i < 8; i++)
                {
                    hex[j] <<= 1;
                    hex[j] |= (mfmbits[(offset + i) + (j * 8)]);
                }
            }

            return hex;
        }
        */
        /// <summary>
        /// Converts MFM to byte array.
        /// </summary>
        /// <param name="mfmbits">MFM data</param>
        /// <param name="offset">Offset in the MFM data</param>
        /// <param name="length">Number of bytes (in the resulting array) to be converted</param>
        /// <param name="threadid"></param>
        /// <param name="sectordatathread"></param>
        /// <returns></returns>
        public byte[] MFM2Bytes(int offset, int length, int threadid)
        {
            byte[] bytebuf = new byte[length];

            for (int i = 0; i < length; i++)
            {
                bytebuf[i] = MFMBits2BINbyte(ref mfms[threadid], offset + (i * 16));
            }
            return bytebuf;
        }

        public byte MFMBits2BINbyte(ref byte[] mfmbits, int offset)
        {
            byte hex = 0;
            int i;

            offset++;
            if (mfmbits.Length > offset + 16)
                for (i = 0; i < 16; i += 2)
                {
                    hex <<= 1;
                    hex |= (mfmbits[offset + i]);
                }
            return hex;
        }

        /// <summary>
        /// Converts a byte array containing the binary to MFM with one bit per byte
        /// and returns it as a byte array
        /// The returned array can be in plain text by setting inText to true
        /// </summary>
        /// <remarks>The output is 2*8 times the size of input</remarks>
        /// <param name="bin">Binary array is a byte array reference</param>
        /// <param name="length">length of the bin array</param>
        /// <param name="offset">offset into the bin array</param>
        /// <param name="inText">inText true is plain text, false is binary</param>

        public byte[] BIN2MFMbits(ref byte[] bin, int length, int offset, bool inText)
        {
            byte[] mfmbits = new byte[length * 16 + length];
            int i, mfmbitscnt = 0;
            int j;
            byte hex, bit, previousbit;
            byte ascii_zero, ascii_one;

            if (inText)
            {
                ascii_zero = 48;
                ascii_one = 49;
            }
            else
            {
                ascii_zero = 0;
                ascii_one = 1;
            }

            previousbit = 0;
            for (i = offset; i < length + offset; i++)
            {
                hex = bin[i];
                for (j = 8; j > 0; j--)
                {

                    bit = (byte)(hex >> (j - 1) & 1);
                    if (bit == 1)
                    {
                        mfmbits[mfmbitscnt++] = ascii_zero;
                        mfmbits[mfmbitscnt++] = ascii_one;
                    }
                    else if (previousbit == 0)
                    {
                        mfmbits[mfmbitscnt++] = ascii_one;
                        mfmbits[mfmbitscnt++] = ascii_zero;
                    }
                    else
                    {
                        mfmbits[mfmbitscnt++] = ascii_zero;
                        mfmbits[mfmbitscnt++] = ascii_zero;
                    }

                    previousbit = bit;
                }
                if (inText == true) mfmbits[mfmbitscnt++] = 0x20; // Add spaces between 16 bits
            }

            return mfmbits;
        }



        public static byte[] HexToBytes(string input)
        {
            // Filter the input first
            StringBuilder filtered = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if ((input[i] >= 48 && input[i] <= 57) || (input[i] >= 65 && input[i] <= 70) || (input[i] >= 97 && input[i] <= 102))
                {
                    filtered.Append(input[i]);
                }
            }

            string filtered2 = filtered.ToString();
            byte[] result = new byte[filtered2.Length / 2];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(filtered2.Substring(2 * i, 2), 16);
            }
            return result;
        }

        public string BytesToHexa(byte[] buf, int index, int length)
        {
            int i, j;
            byte b;
            StringBuilder hex = new StringBuilder();
            StringBuilder txt = new StringBuilder();
            j = 0;
            if (length == 0) length = buf.Length;
            for (i = index; i < index + length; i++)
            {
                b = buf[i];
                if (b > 0x20 && b < 0x7f) txt.Append((char)b);
                else txt.Append(".");
                hex.Append(b.ToString("X2") + " ");
                if (j % 48 == 47)
                    txt.Append("\r\n");
                if (j % 16 == 15)
                    hex.Append("\r\n");
                j++;
            }
            txt.Append("\r\n\r\n");
            txt.Append(hex);
            return txt.ToString();
        }

        // Converts a stringbuilder string holding MFM encoded data to a string with binary decoded data
        public string MFM2BIN(string MFM)
        {
            int i;
            string d = "";

            for (i = 0; i < MFM.Length - 1; i += 2)
            {
                if (MFM[i] == '0' && MFM[i + 1] == '1') // 01 = 1
                    d += "1";
                else
                if (MFM[i] == '1' && MFM[i + 1] == '0') // 10 = 0 (if preceded by 0)
                    d += "0";
                else
                if (MFM[i] == '0' && MFM[i + 1] == '0') // 00 = 0 (if preceded by 1)
                    d += "0";
            }

            return d;
        }

        // Converts two uint32 of MFM data into one uint32 and returns it.
        public uint MFM2BINuint(uint mfmhi, uint mfmlo)
        {
            uint hex = 0;
            int i;
            mfmhi &= 0x55555555;
            mfmlo &= 0x55555555;

            mfmhi <<= 1;
            mfmlo <<= 1;
            for (i = 0; i < 16; i++)
            {
                hex |= ((mfmhi & 0x80000000) >> i);
                mfmhi <<= 2;
            }

            for (i = 16; i < 32; i++)
            {
                hex |= ((mfmlo & 0x80000000) >> i);
                mfmlo <<= 2;
            }

            return hex;
        }

        // Amiga style mfm to byte
        public byte MFM2BINbyte(byte mfmhi, byte mfmlo)
        {
            byte hex = 0;
            int i;
            mfmhi &= 0x55;
            mfmlo &= 0x55;

            mfmhi <<= 1;
            mfmlo <<= 1;
            for (i = 0; i < 4; i++)
            {
                hex |= (byte)((mfmhi & 0x80) >> i);
                mfmhi <<= 2;
            }

            for (i = 4; i < 8; i++)
            {
                hex |= (byte)((mfmlo & 0x80) >> i);
                mfmlo <<= 2;
            }

            return hex;
        }
        // Converts a stringbuilder string holding MFM encoded data to a string with binary decoded data
        public string MFM2BINFast(string MFM)
        {
            int i;
            StringBuilder d = new StringBuilder();

            for (i = 0; i < MFM.Length - 1; i += 2)
            {
                if (MFM[i] == '0' && MFM[i + 1] == '1') // 01 = 1
                    d.Append("1");
                else
                if (MFM[i] == '1' && MFM[i + 1] == '0') // 10 = 0 (if preceded by 0)
                    d.Append("0");
                else
                if (MFM[i] == '0' && MFM[i + 1] == '0') // 00 = 0 (if preceded by 1)
                    d.Append("0");
            }

            return d.ToString();
        }



        // Converts bit encoded byte array of mfm to a byte encoded byte array
        // result is an array that's 8x smaller than length
        // Non thread safe
        // Todo: replace with MFM2Bytes with thread safe build in.
        public byte[] MFM2ByteArray(byte[] mfm, int offset, int length)
        {
            int i, j;
            int mindex = 0;
            byte[] m = new byte[length / 8];

            byte hex;
            for (i = offset; i < offset + length; i += 8)
            {
                hex = 0;
                for (j = 0; j < 8; j++)
                {
                    hex <<= 1;
                    hex |= mfm[i + j];
                }
                m[mindex++] = hex;
                //mindex++;
            }


            /*            byte hex = 0;
                        int i;

                        offset++;
                        if (mfmbits.Length > offset + 16)
                            for (i = 0; i < 16; i += 2)
                            {
                                hex <<= 1;
                                hex |= (mfmbits[offset + i]);
                            }
                        return hex;
                        */

            return m;
        }

        // Converts a binary string to HEX
        public string BIN2HEX(string bin)
        {
            string h;
            int n, i;

            h = "";

            for (i = 0; i < bin.Length; i += 8)
            {
                if (bin.Length >= (i + 8))
                {
                    n = Convert.ToInt32(bin.ToString().Substring(i, 8), 2);
                    if (n < 16)
                        h += "0" + n.ToString("X") + " ";
                    else h += n.ToString("X") + " ";
                }
            }
            return h;
        }

        // Converts a binary string to HEX without space between every byte
        public string BIN2HEXns(string bin)
        {
            string h;
            int n, i;

            h = "";

            for (i = 0; i < bin.Length; i += 8)
            {
                if (bin.Length >= (i + 8))
                {
                    n = Convert.ToInt32(bin.ToString().Substring(i, 8), 2);
                    if (n < 16)
                        h += "0" + n.ToString("X");
                    else h += n.ToString("X");
                }
            }
            return h;
        }
        // Searches for the marker, then returns string cropped anything before the marker.
        public string CropToMarker(string d, string marker)
        {
            int startindex;

            startindex = d.IndexOf(marker);
            //textBoxReceived.Text += "Start index:" + startindex.ToString() + "\r\n";
            if (startindex >= 0)
                return d.Remove(0, startindex);
            return "";
        }

        public void SaveGoodSectorFoundInfo()
        {
            string subpath = @Properties.Settings.Default["PathToRecoveredDisks"].ToString();
            string path = subpath + @"\" + ProcSettings.outputfilename + @"\";
            
            try
            {
                File.WriteAllText(path + ProcSettings.outputfilename + "_FoundGoodSectorInfo.txt", FoundGoodSectorInfo.ToString());
            }
            catch (IOException ex)
            {
                TBReceived.Append("IO error writing sector map: \r\n" + ex.ToString() + "\r\n");
            }
        }

        public void LoadGoodSectorFoundInfo()
        {
            string subpath = @Properties.Settings.Default["PathToRecoveredDisks"].ToString();
            string path = subpath + @"\" + ProcSettings.outputfilename + @"\";
            
            try
            {
                FoundGoodSectorInfo.Clear();
                FoundGoodSectorInfo.Append(File.ReadAllText(path + ProcSettings.outputfilename + "_FoundGoodSectorInfo.txt"));
            }
            catch (IOException ex)
            {
                TBReceived.Append("IO error Reading sector map: \r\n" + ex.ToString() + "\r\n");
            }
        }

        // Search method, searches pattern in array starting at startIndex with count the number of items in the array.
        public int IndexOfBytes(byte[] array, byte[] pattern, int startIndex, int count)
        {
            if (array == null || array.Length == 0 || pattern == null || pattern.Length == 0 || count == 0)
            {
                return -1;
            }
            int i = startIndex;
            int endIndex = count > 0 ? Math.Min(startIndex + count, array.Length) : array.Length;
            int fidx = 0;
            int lastFidx;

            while (i < endIndex)
            {
                lastFidx = fidx;
                fidx = (array[i] == pattern[fidx]) ? ++fidx : 0;
                if (fidx == pattern.Length)
                {
                    return i - fidx + 1;
                }
                if (lastFidx > 0 && fidx == 0)
                {
                    i -= lastFidx;
                }
                i++;
            }
            return -1;
        }

        public static uint SwapEndianness(uint value)
        {
            var b1 = (value >> 0) & 0xff;
            var b2 = (value >> 8) & 0xff;
            var b3 = (value >> 16) & 0xff;
            var b4 = (value >> 24) & 0xff;

            return b1 << 24 | b2 << 16 | b3 << 8 | b4 << 0;
        }

    } //class
}

