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

namespace FloppyControlApp
{
    [Serializable]
    public class ProcSettings : ICloneable
    {
        private int poffset, pmin, pfour, psix, pmax, pstart, pend, ppattern;
        private int pplatform; // 0 = PC, 1 = Amiga
        private float prateofchange;
        bool pSkipPeriodData, pfinddupes, pUseErrorCorrection, pAddnoise;
        ProcessingType pprocessingtype;

        public int offset { get { return poffset; } set { poffset = value; } }
        public int min { get { return pmin; } set { pmin = value; } }
        public int four { get { return pfour; } set { pfour = value; } }
        public int six { get { return psix; } set { psix = value; } }
        public int max { get { return pmax; } set { pmax = value; } }
        public int start { get { return pstart; } set { pstart = value; } }
        public int end { get { return pend; } set { pend = value; } }
        public int platform { get { return pplatform; } set { pplatform = value; } }
        public int pattern { get { return ppattern; } set { ppattern = value; } }
        public int addnoiselimitstart { get; set; }
        public int addnoiselimitend { get; set; }
        public int addnoiserndamount { get; set; }
        public int NumberOfDups { get; set; }
        public int limittotrack { get; set; }
        public int limittosector { get; set; }
        public int hd { get; set; }
        public int AdaptOffset { get; set; }
        public float AdaptOffset2 { get; set; }

        public float rateofchange { get { return prateofchange; } set { prateofchange = value; } }
        public float rateofchange2 { get; set; }
        public bool SkipPeriodData { get { return pSkipPeriodData; } set { pSkipPeriodData = value; } }
        public bool finddupes { get { return pfinddupes; } set { pfinddupes = value; } }
        public bool UseErrorCorrection { get { return pUseErrorCorrection; } set { pUseErrorCorrection = value; } }
        public bool OnlyBadSectors { get; set; }
        public bool AddNoise { get { return pAddnoise; } set { pAddnoise = value; } }
        public bool LimitTSOn { get; set; }
        public bool IgnoreHeaderError { get; set; }
        public bool AutoRefreshSectormap { get; set; }
        public string outputfilename { get; set; }

        public ProcessingType processingtype { get { return pprocessingtype; } set { pprocessingtype = value; } }

        public ProcSettings()
        {
        }
        public object Clone()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;
            object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }
    }

    public class DiskGeometry
    {
        public int sectorsPerTrack { get; set; }
        public int tracksPerDisk { get; set; }
        public int numberOfHeads { get; set; }
        public int sectorSize { get; set; }
        public FluxEncoding fluxEncoding { get; set; }
    }
    
    public class ECSettings
    {
        public int periodSelectionStart { get; set; }
        public int periodSelectionEnd { get; set; }
        public int indexS1 { get; set; }
        public int threadid { get; set; }
        public int combinations { get; set; }
        public int C6Start { get; set; }
        public int C8Start { get; set; }
        public int MFMByteStart { get; set; }
        public int MFMByteLength { get; set; }
        public TextBox sectortextbox { get; set; }
    }

    public class ECResult
    {
        public MFMData sectordata { get; set; }
        public int index { get; set; }
    }

    // MFM in byte encoded form sorted with most frequently occurring at the top. 
    // Used for error correction
    class MFMByteEncPreset
    {
        // Taken from disk A002
        public byte[,] MFMPC = new byte[32, 8]
        {
            {0,1,0,1,0,1,0,0}, // 54
            {1,0,0,1,0,0,1,0}, // 92
            {0,1,0,1,0,1,0,1}, // 55
            {0,1,0,0,0,1,0,0}, // 44
            {0,1,0,1,0,0,0,1}, // 51
            {0,1,0,0,1,0,0,1}, // 49
            {0,1,0,0,0,1,0,1}, // 45
            {1,0,1,0,1,0,1,0}, // AA
            {0,1,0,1,0,0,1,0}, // 52
            {0,1,0,0,1,0,1,0}, // 4A
            {0,0,1,0,1,0,1,0}, // 2A
            {0,0,0,1,0,0,1,0}, // 12
            {1,0,1,0,0,1,0,0}, // A4
            {0,0,0,1,0,1,0,1}, // 15
            {0,0,1,0,0,1,0,1}, // 25
            {1,0,1,0,1,0,0,1}, // A9
            {1,0,0,1,0,1,0,1}, // 95
            {0,0,0,1,0,0,0,1}, // 11
            {1,0,1,0,0,1,0,1}, // A5
            {0,0,1,0,0,1,0,0}, // 24
            {0,0,0,1,0,1,0,0}, // 14
            {1,0,0,1,0,1,0,0}, // 94
            {0,0,1,0,1,0,0,1}, // 29
            {1,0,0,1,0,0,0,1}, // 91
            {1,0,0,0,1,0,0,1}, // 89
            {1,0,1,0,0,0,1,0}, // A2
            {0,0,1,0,0,0,1,0}, // 22
            {1,0,0,0,1,0,0,0}, // 88
            {0,0,1,0,1,0,0,0}, // 28
            {1,0,0,0,1,0,1,0}, // 8A
            {1,0,1,0,1,0,0,0}, // A8
            {0,1,0,0,1,0,0,0}  // 48
        };
    };

    public partial class FDDProcessing
    {
        public ConcurrentDictionary<int, MFMData> sectordata2;
        public byte[] disk = new byte[2000000]; // Assuming a disk isn't larger than 2MB, which is false for EHD disks. 
        public DiskFormat diskformat = 0;// 0x1 = ADOS/PFS, 0x02 = DiskSpare, 0x03 = PCDD, 0x04 = PCHD
        public ProcSettings procsettings { get; set; }
        public SectorMap sectormap { get; set; }
        private System.Diagnostics.Stopwatch SW = new System.Diagnostics.Stopwatch();
        public StringBuilder tbreceived { get; set; }
        public StringBuilder FoundGoodSectorInfo = new StringBuilder();
        public string CurrentFiles;
        public RichTextBox rtbSectorMap { get; set; } // for sectormap
        private BinaryWriter writer;
        public byte[] rxbuf { get; set; } // This is a super large 200MB buffer to hold timing data captured by the floppy controller
        public int indexrxbuf { get; set; }
        public int[] rxbuftograph { get; set; }
        
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
        public int debuglevel { get; set; }
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
        public int stop { get; set; }
        private Thread[] threads { get; set; }
        public int NumberOfThreads { get; set; }
        public int processing = 0;
        public Dictionary<DiskFormat, DiskGeometry> diskGeometry = new Dictionary<DiskFormat, DiskGeometry>();

        public FDDProcessing()
        {
            debuglevel = 1;
            indexrxbuf = 0;
            sectormap = new SectorMap(rtbSectorMap, this);
            stop = 0;
            NumberOfThreads = 1;
            rxbuf = new byte[1000000];
            procsettings = new ProcSettings();
            int numProcs = Environment.ProcessorCount;
            int concurrencyLevel = numProcs * 2;
            sectordata2 = new ConcurrentDictionary<int, MFMData>(concurrencyLevel, 100);

            //PC
            DiskGeometry pcdosdsdd = new DiskGeometry();
            pcdosdsdd.numberOfHeads = 2;
            pcdosdsdd.tracksPerDisk = 80;
            pcdosdsdd.sectorsPerTrack = 9;
            pcdosdsdd.sectorSize = 512;
            pcdosdsdd.fluxEncoding = FluxEncoding.MFM;
            diskGeometry.Add(DiskFormat.pcdd, pcdosdsdd);

            DiskGeometry pcdoshd = new DiskGeometry();
            pcdoshd.numberOfHeads = 2;
            pcdoshd.tracksPerDisk = 80;
            pcdoshd.sectorsPerTrack = 18;
            pcdoshd.sectorSize = 512;
            pcdoshd.fluxEncoding = FluxEncoding.MFM;
            diskGeometry.Add(DiskFormat.pchd, pcdoshd);

            DiskGeometry pcdosssdd = new DiskGeometry();
            pcdosssdd.numberOfHeads = 1;
            pcdosssdd.tracksPerDisk = 80;
            pcdosssdd.sectorsPerTrack = 9;
            pcdosssdd.sectorSize = 512;
            pcdosssdd.fluxEncoding = FluxEncoding.MFM;
            diskGeometry.Add(DiskFormat.pcssdd, pcdosssdd);

            // I'm not sure, I'll have to check these settings
            DiskGeometry pcdos2m = new DiskGeometry();
            pcdos2m.numberOfHeads = 2;
            pcdos2m.tracksPerDisk = 80;
            pcdos2m.sectorsPerTrack = 12;
            pcdos2m.sectorSize = 1024;
            pcdos2m.fluxEncoding = FluxEncoding.MFM;
            diskGeometry.Add(DiskFormat.pc2m, pcdos2m);

            //Amiga
            DiskGeometry amigados = new DiskGeometry();
            amigados.numberOfHeads = 2;
            amigados.tracksPerDisk = 80;
            amigados.sectorsPerTrack = 11;
            amigados.sectorSize = 512;
            amigados.fluxEncoding = FluxEncoding.MFM;
            diskGeometry.Add(DiskFormat.amigados, amigados);

            // My own extended diskspare format
            DiskGeometry amigadiskspareE = new DiskGeometry();
            amigadiskspareE.numberOfHeads = 2;
            amigadiskspareE.tracksPerDisk = 82;
            amigadiskspareE.sectorsPerTrack = 12;
            amigadiskspareE.sectorSize = 512;
            amigadiskspareE.fluxEncoding = FluxEncoding.MFM;
            diskGeometry.Add(DiskFormat.diskspare984KB, amigadiskspareE);

            DiskGeometry amigadiskspare = new DiskGeometry();
            amigadiskspare.numberOfHeads = 2;
            amigadiskspare.tracksPerDisk = 80;
            amigadiskspare.sectorsPerTrack = 12;
            amigadiskspare.sectorSize = 512;
            amigadiskspare.fluxEncoding = FluxEncoding.MFM;
            diskGeometry.Add(DiskFormat.diskspare, amigadiskspare);
        }

        public void ClearNonBadSectors()
        {
            int i;
            MFMData deleted;
            for (i = 0; i < sectordata2.Count; i++)
            {
                if (sectordata2[i].mfmMarkerStatus != SectorMapStatus.HeadOkDataBad)
                {
                    sectordata2.TryRemove(i, out deleted);
                }
                else
                if (sectordata2[i].mfmMarkerStatus == SectorMapStatus.HeadOkDataBad)
                {
                    if (sectordata2[i].sectorbytes.Length == 0)
                        sectordata2.TryRemove(i, out deleted);
                }
            }
        }

        public void StartProcessing(int platform)
        {
            int threadid = 0, t, i;
            SW.Reset();
            SW.Start();

            //ProcessingType processingtype = ProcessingType.normal;

            if (GetProcSettingsCallback != null) GetProcSettingsCallback();
            else
            {
                tbreceived.Append("StartProcessing() GetProcSettingsCallback not set!\r\n");
                return;
            }
            //LoadGoodSectorFoundInfo();
            procsettings.platform = platform;
            if (stop == 0)
            {

                //Get the values from slider controls:

                StringBuilder outputfast = new StringBuilder();
                if (threads == null)
                    threads = new Thread[NumberOfThreads + 1];
                if (threads.Length < NumberOfThreads)
                    threads = new Thread[NumberOfThreads + 1];


                ProcessStatus[threadid] = "Period to MFM conversion...";
                progressesstart[threadid] = procsettings.start;
                progressesend[threadid] = procsettings.end;

                //int sectdataitems = 0;

                // Process one track/sector combo only, based on sectordata
                if (procsettings.OnlyBadSectors)
                {
                    int limittotrack = procsettings.limittotrack;
                    int limittosector = procsettings.limittosector;
                    int oldindexrxbuf = indexrxbuf;

                    int mfmsindexold = mfmsindex;

                    for (i = 0; i < sectordata2.Count; i++)
                    {
                        if (!sectordata2.IsEmpty)
                        {
                            if (sectordata2[i].mfmMarkerStatus == SectorMapStatus.HeadOkDataBad)
                            {
                                if (sectormap.sectorok[sectordata2[i].track, sectordata2[i].sector] == SectorMapStatus.HeadOkDataBad)
                                {
                                    if (procsettings.LimitTSOn)
                                    {
                                        if (sectordata2[i].track == procsettings.limittotrack && sectordata2[i].sector == procsettings.limittosector)
                                        {

                                            int q;
                                            int rxstart = (sectordata2[i].rxbufMarkerPositions - 500);
                                            for (int x = 0; x < procsettings.NumberOfDups; x++)
                                                for (q = 0; q < 8500; q++)
                                                    rxbuf[oldindexrxbuf++] = rxbuf[rxstart + q];
                                            if (oldindexrxbuf > rxbuf.Length + 1) break;
                                        }
                                    }
                                    else // no T/S limit, process all bad sectors
                                    {
                                        //tbreceived.Append("Start: " + sectordata2[i].rxbufMarkerPositions + "\r\n");
                                        int q;
                                        int rxstart = (sectordata2[i].rxbufMarkerPositions - 500);
                                        if (rxstart < 0) rxstart = 0;
                                        for (q = 0; q < 8500; q++)
                                        {
                                            if (oldindexrxbuf < rxbuf.Length - 1)
                                                rxbuf[oldindexrxbuf++] = rxbuf[rxstart + q];
                                            else break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (oldindexrxbuf == indexrxbuf)
                    {
                        stop = 1;
                        sectormap.RefreshSectorMap();
                    }

                    procsettings.start = indexrxbuf;
                    procsettings.end = oldindexrxbuf;
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
                        progressesstart[t1] = procsettings.start;
                        progressesend[t1] = procsettings.end;

                        ProcSettings procsettings1 = procsettings;

                        threads[t] = new Thread(() => Period2MFM(procsettings1, t1));
                        threads[t].Start();
                    }
                    //SW.Restart();
                    //tbreceived.Append("after thread:" + SW.ElapsedMilliseconds + "ms\r\n");
                    // Synch
                    for (t = 0; t < NumberOfThreads; t++)
                    {
                        while (!threads[t].Join(5))
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
                    int perThreadLength = (procsettings.end - procsettings.start) / NumberOfThreads;
                    int offset = 0;

                    for (t = 0; t < NumberOfThreads; t++)
                    {
                        // I may need to make more local vars to pass to Period2MFM
                        int t1 = t + mfmsindex; // thread () => is a lambda expression and t is passed by reference, so t will be the last value. This fixes the issue
                        mfmlengths[t1] = 0;

                        ProcSettings ps = new ProcSettings();

                        ps = (ProcSettings)procsettings.Clone();

                        ps.start = offset;
                        ps.end = offset + perThreadLength;
                        progressesstart[t1] = ps.start;
                        progressesend[t1] = ps.end;

                        //ProcSettings procsettings1 = procsettings;

                        threads[t] = new Thread(() => Period2MFM(ps, t1));
                        threads[t].Start();
                        offset += perThreadLength;
                    }
                    //SW.Restart();
                    //tbreceived.Append("After thread:" + SW.ElapsedMilliseconds + "ms\r\n");
                    // Synch
                    for (t = 0; t < NumberOfThreads; t++)
                    {
                        while (!threads[t].Join(5))
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

                    if (procsettings.AutoRefreshSectormap)
                        sectormap.RefreshSectorMap();
                }
            }

            SW.Stop();
            tbreceived.Append(SW.ElapsedMilliseconds + "ms\r\n");
            //SaveGoodSectorFoundInfo();
            //tbreceived.Append(relativetime().ToString() + "ms \r\n");
        }

        // The first four params represents the thresholds for the timing pulses
        private void Period2MFM(ProcSettings procsettings, int threadid)
        {
            int i;
            int value;

            bool writepulses = false;
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

            if (end - start == 0)
            {
                tbreceived.Append("Period2MFM: Error: Length can't be zero.\r\n");
                //stop = 1;
                //PrintProperties(procsettings);
                return;
            }

            tbreceived.Append("Period length:" + (end - start) + " ");
            byte[] m;
            if (procsettings.AddNoise || procsettings.pattern == 4)
                m = new byte[((end - start) * 5)]; // mfm data can be max. 4x period data
            else m = new byte[((end - start) * 4)]; // mfm data can be max. 4x period data


            //markerpositionscnt = 0;

            if (writepulses == true)
            {
                // Write period data to disk in hex/text format, one value per line to be read in excel for analysis
                string subpath = @Properties.Settings.Default["PathToRecoveredDisks"].ToString();
                string path = subpath + @"\" + procsettings.outputfilename + @"\";
                writer = new BinaryWriter(new FileStream(path + procsettings.outputfilename + ".hex", FileMode.Create));
                for (i = 0; i < indexrxbuf; i++)
                {
                    writer.Write(rxbuf[i].ToString("X2")[0]);
                    writer.Write(rxbuf[i].ToString("X2")[1]);
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

            if (MINUS < 0) MINUS = 0;

            if (processingtype == ProcessingType.adaptive2) //************ Adaptive2 ****************
            {
                float threshold4us;
                float threshold6us;
                float _4us = FOURUS, _6us = SIXUS, _8us = EIGHTUS;

                //tbreceived.Append("Adaptive\r\n");

                for (i = start; i < end; i++)
                {
                    if (i % 250000 == 249999) { progresses[threadid] = i; if (stop == 1) break; }
                    value = (rxbuf[i] << procsettings.hd); // If it's a HD (user selectable option), multiply data by 2

                    //if (value < 5 ) continue;

                    if (_4us >= _6us || _6us >= _8us) // if out of control, reset
                    {
                        _4us = FOURUS;
                        _6us = SIXUS;
                        _8us = EIGHTUS;
                    }

                    threshold4us = (_4us + ((_6us - _4us) / 2));
                    threshold6us = (_6us + ((_8us - _6us) / 2));

                    if (value <= threshold4us) // 4us
                    {
                        //m[mfmlengths[threadid]++]
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        _4us = _4us + (int)((value - _4us) / RateOfChange);

                    }
                    else
                    if (value > threshold4us && value < threshold6us) // 6us
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        _6us = _6us + (int)((value - _6us) / RateOfChange);
                    }
                    else
                    if (value >= threshold6us) // 8us
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        _8us = _8us + (int)((value - _8us) / RateOfChange);
                    }
                }
            }
            else
            if (processingtype == ProcessingType.adaptive3) //************ Adaptive 3 ****************
            {
                float threshold4us;
                float threshold6us;
                float _4us = FOURUS, _6us = SIXUS, _8us = EIGHTUS;

                int basethreshold4us = ((SIXUS - FOURUS) / 2);
                int basethreshold6us = ((EIGHTUS - SIXUS) / 2);


                //tbreceived.Append("Adaptive\r\n");
                //int limitdeviation4us = 5;
                //int limitdeviation6us = 5;
                //int limitdeviation6us = 20;
                int dampcnt4 = (int)RateOfChange;
                int dampcnt6 = (int)(RateOfChange * 0.8f);
                int dampcnt8 = (int)(RateOfChange * 0.1f);
                dampcnt8 = 10;
                float[] damp4 = new float[dampcnt4];
                float[] damp6 = new float[dampcnt6];
                float[] damp8 = new float[dampcnt8];
                //int q;
                int lookahead = 10;
                //int d4cnt = 0, d6cnt = 0, d8cnt = 0;
                float averagetime = 0;
                int val2;

                if (scatterplotend - scatterplotstart > 10000) scatterplotend = scatterplotstart + 2000;
                threshold4us = FOURUS + basethreshold4us;
                threshold6us = SIXUS + basethreshold6us;
                float _4usavg = FOURUS, _6usavg = SIXUS, _8usavg = EIGHTUS;
                float fourus = FOURUS, sixus = SIXUS, eightus = EIGHTUS;
                Random rnd = new Random();
                int rand = 0;
                int sectorboundary = 0;

                for (i = start; i < end - lookahead; i++)
                {
                    if (i % 250000 == 249999) { progresses[threadid] = i; if (stop == 1) break; }
                    sectorboundary = (i - start + 50) % 9000;
                    if (procsettings.AddNoise == true && sectorboundary >= procsettings.addnoiselimitstart && sectorboundary <= procsettings.addnoiselimitend)
                        rand = rnd.Next(-1, 1) * procsettings.addnoiserndamount;
                    //    rand = rnd.Next(0 - procsettings.addnoiserndamount, procsettings.addnoiserndamount);
                    else
                        rand = 0;

                    value = (rxbuf[i] << procsettings.hd) + rand; // If it's a HD (user selectable option), multiply data by 2

                    val2 = value;
                    value -= (int)(averagetime / RateOfChange);// + procsettings.AdaptOffset;
                                                               //if (procsettings.UseErrorCorrection == false)
                    if (val2 < 4) continue;

                    //rxbuf[i] = (byte)value;
                    //if (i > scatterplotstart && i < scatterplotend)
                    //    tbreceived.Append("" + value + "\t" + threshold4us.ToString("0.00")+"\t"+threshold6us.ToString("0.00")+"\r\n");
                    if (value <= threshold4us) // 4us
                    {

                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        _4us = fourus - val2;
                        if (procsettings.rateofchange2 != 0)
                        {
                            _4usavg = _4usavg + ((val2 - fourus) / procsettings.rateofchange2);
                            fourus = _4usavg;
                        }
                        averagetime = _4us;
                    }
                    else
                    if (value > threshold4us && value < threshold6us) // 6us
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        _6us = sixus - val2;
                        if (procsettings.rateofchange2 != 0)
                        {
                            _6usavg = _6usavg + ((val2 - sixus) / procsettings.rateofchange2);
                            sixus = _6usavg;
                        }
                        //SIXUS = (byte)_6usavg;
                        //damp6[(d6cnt++) % dampcnt6] = value * value;
                        //_6us = _6us + (int)((value - _6us) / RateOfChange);
                        averagetime = _6us;
                    }
                    else
                    if (value >= threshold6us) // 8us
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        _8us = eightus - val2;
                        if (procsettings.rateofchange2 != 0)
                        {
                            _8usavg = _8usavg + ((val2 - eightus) / procsettings.rateofchange2);
                            eightus = _8usavg;
                        }
                        //EIGHTUS = (byte)_8usavg;
                        //damp8[(d8cnt++) % dampcnt8] = value * value;
                        //_8us = _8us + (int)((value - _8us) / RateOfChange);
                        //averagetime = (averagetime + _8us)/2;
                        averagetime = _8us;
                    }
                }
            }
            else
            if (processingtype == ProcessingType.adaptive1) //************ Adaptive ****************
            {
                float threshold4us;
                float threshold6us;
                float _4us = FOURUS, _6us = SIXUS, _8us = EIGHTUS;

                int basethreshold4us = ((SIXUS - FOURUS) / 2);
                int basethreshold6us = ((EIGHTUS - SIXUS) / 2);


                //tbreceived.Append("Adaptive\r\n");
                //int limitdeviation4us = 5;
                //int limitdeviation6us = 5;
                //int limitdeviation6us = 20;
                int dampcnt4 = (int)RateOfChange;
                int dampcnt6 = (int)(RateOfChange * 0.8f);
                int dampcnt8 = (int)(RateOfChange * 0.1f);
                dampcnt8 = 10;
                float[] damp4 = new float[dampcnt4];
                float[] damp6 = new float[dampcnt6];
                float[] damp8 = new float[dampcnt8];
                //int q;
                int lookahead = 10;
                //int d4cnt = 0, d6cnt = 0, d8cnt = 0;
                float averagetime = 0;
                int val2;

                if (scatterplotend - scatterplotstart > 10000) scatterplotend = scatterplotstart + 2000;
                threshold4us = FOURUS + basethreshold4us;
                threshold6us = SIXUS + basethreshold6us;
                float _4usavg = FOURUS, _6usavg = SIXUS, _8usavg = EIGHTUS;
                float fourus = FOURUS, sixus = SIXUS, eightus = EIGHTUS;
                Random rnd = new Random();
                int rand = 0;
                int sectorboundary = 0;


                int lowpassradius = (int)procsettings.rateofchange2;
                float[] lowpass4 = new float[lowpassradius];
                float[] lowpass6 = new float[lowpassradius];
                float[] lowpass8 = new float[lowpassradius];
                for (i = 0; i < lowpassradius; i++)
                {
                    lowpass4[i] = fourus;
                    lowpass6[i] = sixus;
                    lowpass8[i] = eightus;
                }
                _4usavg = lowpassradius * fourus;
                _6usavg = lowpassradius * sixus;
                _8usavg = lowpassradius * eightus;

                float val4 = 0, val6 = 0, val8 = 0;

                entropy = new float[indexrxbuf];
                threshold4 = new float[indexrxbuf];
                threshold6 = new float[indexrxbuf];
                threshold8 = new float[indexrxbuf];

                for (i = start; i < end - lookahead; i++)
                {
                    if (i % 250000 == 249999) { progresses[threadid] = i; if (stop == 1) break; }
                    sectorboundary = (i - start + 50) % 9000;
                    if (procsettings.AddNoise == true && sectorboundary >= procsettings.addnoiselimitstart && sectorboundary <= procsettings.addnoiselimitend)
                        rand = rnd.Next(-1, 1) * procsettings.addnoiserndamount;
                    //    rand = rnd.Next(0 - procsettings.addnoiserndamount, procsettings.addnoiserndamount);
                    else
                        rand = 0;

                    value = (rxbuf[i] << procsettings.hd) + rand; // If it's a HD (user selectable option), multiply data by 2

                    val2 = value;
                    value -= (int)(averagetime / RateOfChange);// + procsettings.AdaptOffset;
                                                               //if (procsettings.UseErrorCorrection == false)
                    if (val2 < 4) continue;

                    //rxbuf[i] = (byte)value;
                    //if (i > scatterplotstart && i < scatterplotend)
                    //    tbreceived.Append("" + value + "\t" + threshold4us.ToString("0.00")+"\t"+threshold6us.ToString("0.00")+"\r\n");
                    if (value <= threshold4us) // 4us
                    {

                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        _4us = fourus - val2;
                        if (procsettings.rateofchange2 != 0)
                        {
                            val4 = lowpass4[(i + 1) % lowpassradius];
                            _4usavg -= val4;
                            lowpass4[(i + 1) % lowpassradius] = val2;
                            _4usavg += val2;
                            fourus = _4usavg / lowpassradius;
                            //_4usavg = _4usavg + ((val2 - fourus) / procsettings.rateofchange2);

                        }
                        averagetime = _4us;
                    }
                    else
                    if (value > threshold4us && value < threshold6us) // 6us
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        _6us = sixus - val2;
                        if (procsettings.rateofchange2 != 0)
                        {
                            val6 = lowpass6[(i + 1) % lowpassradius];
                            _6usavg -= val6;
                            lowpass6[(i + 1) % lowpassradius] = val2;
                            _6usavg += val2;
                            sixus = _6usavg / lowpassradius;
                        }

                        averagetime = _6us;
                    }
                    else
                    if (value >= threshold6us) // 8us
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        _8us = eightus - val2;
                        if (procsettings.rateofchange2 != 0)
                        {
                            val8 = lowpass8[(i + 1) % lowpassradius];
                            _8usavg -= val8;
                            lowpass8[(i + 1) % lowpassradius] = val2;
                            _8usavg += val2;
                            eightus = _8usavg / lowpassradius;
                        }

                        averagetime = _8us;
                    }
                }
            }
            else
            if (processingtype == ProcessingType.adaptiveEntropy) //************ Adaptive Entropy ****************
            {
                float threshold4us;
                float threshold6us;
                float _4us = FOURUS, _6us = SIXUS, _8us = EIGHTUS;

                int basethreshold4us = ((SIXUS - FOURUS) / 2);
                int basethreshold6us = ((EIGHTUS - SIXUS) / 2);


                //tbreceived.Append("Adaptive\r\n");
                //int limitdeviation4us = 5;
                //int limitdeviation6us = 5;
                //int limitdeviation6us = 20;
                int dampcnt4 = (int)RateOfChange;
                int dampcnt6 = (int)(RateOfChange * 0.8f);
                int dampcnt8 = (int)(RateOfChange * 0.1f);
                dampcnt8 = 10;
                float[] damp4 = new float[dampcnt4];
                float[] damp6 = new float[dampcnt6];
                float[] damp8 = new float[dampcnt8];
                //int q;
                int lookahead = 10;
                //int d4cnt = 0, d6cnt = 0, d8cnt = 0;
                float averagetime = 0;
                int val2;

                if (scatterplotend - scatterplotstart > 10000) scatterplotend = scatterplotstart + 2000;
                threshold4us = FOURUS + basethreshold4us;
                threshold6us = SIXUS + basethreshold6us;
                float _4usavg = FOURUS, _6usavg = SIXUS, _8usavg = EIGHTUS;
                float fourus = FOURUS, sixus = SIXUS, eightus = EIGHTUS;
                Random rnd = new Random();
                int rand = 0;
                int sectorboundary = 0;


                int lowpassradius = (int)procsettings.rateofchange2;
                float[] lowpass4 = new float[lowpassradius];
                float[] lowpass6 = new float[lowpassradius];
                float[] lowpass8 = new float[lowpassradius];
                for (i = 0; i < lowpassradius; i++)
                {
                    lowpass4[i] = fourus;
                    lowpass6[i] = sixus;
                    lowpass8[i] = eightus;
                }
                _4usavg = lowpassradius * fourus;
                _6usavg = lowpassradius * sixus;
                _8usavg = lowpassradius * eightus;

                float val4 = 0, val6 = 0, val8 = 0;

                int length = rxbuf.Length;

                entropy = new float[length];
                threshold4 = new float[length];
                threshold6 = new float[length];
                threshold8 = new float[length];

                for (i = start; i < end - lookahead; i++)
                {
                    if (i % 250000 == 249999) { progresses[threadid] = i; if (stop == 1) break; }
                    sectorboundary = (i - start + 50) % 9000;
                    if (procsettings.AddNoise == true && sectorboundary >= procsettings.addnoiselimitstart && sectorboundary <= procsettings.addnoiselimitend)
                        rand = rnd.Next(-1, 1) * procsettings.addnoiserndamount;
                    //    rand = rnd.Next(0 - procsettings.addnoiserndamount, procsettings.addnoiserndamount);
                    else
                        rand = 0;

                    value = (rxbuf[i] << procsettings.hd) + rand; // If it's a HD (user selectable option), multiply data by 2

                    val2 = value;
                    value -= (int)(averagetime / RateOfChange);// + procsettings.AdaptOffset;


                    //if (procsettings.UseErrorCorrection == false)
                    if (val2 < 4) continue;

                    //rxbuf[i] = (byte)value;
                    //if (i > scatterplotstart && i < scatterplotend)
                    //    tbreceived.Append("" + value + "\t" + threshold4us.ToString("0.00")+"\t"+threshold6us.ToString("0.00")+"\r\n");
                    threshold4[i] = fourus;
                    threshold6[i] = sixus;
                    threshold8[i] = eightus;

                    threshold4us = fourus + basethreshold4us;
                    threshold6us = sixus + basethreshold6us;
                    //procsettings.AdaptOffset2
                    if (value <= threshold4us + procsettings.AdaptOffset2) // 4us
                    {

                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        _4us = fourus - val2;
                        entropy[i] = _4us;
                        if (procsettings.rateofchange2 != 0)
                        {
                            val4 = lowpass4[(i + 1) % lowpassradius];
                            _4usavg -= val4;
                            lowpass4[(i + 1) % lowpassradius] = val2;
                            _4usavg += val2;
                            fourus = _4usavg / lowpassradius;

                            //_4usavg = _4usavg + ((val2 - fourus) / procsettings.rateofchange2);

                        }
                        averagetime = _4us;
                    }
                    else
                    if (value > threshold4us + procsettings.AdaptOffset2 && value < threshold6us - procsettings.AdaptOffset2) // 6us
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        _6us = sixus - val2;
                        entropy[i] = _6us;
                        if (procsettings.rateofchange2 != 0)
                        {
                            val6 = lowpass6[(i + 1) % lowpassradius];
                            _6usavg -= val6;
                            lowpass6[(i + 1) % lowpassradius] = val2;
                            _6usavg += val2;
                            sixus = _6usavg / lowpassradius;

                        }

                        averagetime = _6us;
                    }
                    else
                    if (value >= threshold6us - procsettings.AdaptOffset2) // 8us
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        _8us = eightus - val2;
                        entropy[i] = _8us;
                        if (procsettings.rateofchange2 != 0)
                        {
                            val8 = lowpass8[(i + 1) % lowpassradius];
                            _8usavg -= val8;
                            lowpass8[(i + 1) % lowpassradius] = val2;
                            _8usavg += val2;
                            eightus = _8usavg / lowpassradius;

                        }

                        averagetime = _8us;
                    }
                }
            }
            else
            if (processingtype == ProcessingType.adaptivePredict) //************ Adaptive predict ****************
            {
                float threshold4us;
                float threshold6us;
                float _4us = FOURUS, _6us = SIXUS, _8us = EIGHTUS;

                int basethreshold4us = ((SIXUS - FOURUS) / 2);
                int basethreshold6us = ((EIGHTUS - SIXUS) / 2);


                //tbreceived.Append("Adaptive\r\n");
                //int limitdeviation4us = 5;
                //int limitdeviation6us = 5;
                //int limitdeviation6us = 20;
                int dampcnt4 = (int)RateOfChange;
                int dampcnt6 = (int)(RateOfChange * 0.8f);
                int dampcnt8 = (int)(RateOfChange * 0.1f);
                dampcnt8 = 10;
                float[] damp4 = new float[dampcnt4];
                float[] damp6 = new float[dampcnt6];
                float[] damp8 = new float[dampcnt8];
                //int q;
                int lookahead = 10;
                //int d4cnt = 0, d6cnt = 0, d8cnt = 0;
                float averagetime = 0;
                int val2;

                if (scatterplotend - scatterplotstart > 10000) scatterplotend = scatterplotstart + 2000;
                threshold4us = FOURUS + basethreshold4us;
                threshold6us = SIXUS + basethreshold6us;
                float _4usavg = FOURUS, _6usavg = SIXUS, _8usavg = EIGHTUS;
                float fourus = FOURUS, sixus = SIXUS, eightus = EIGHTUS;
                Random rnd = new Random();
                int rand = 0;
                int sectorboundary = 0;


                int lowpassradius = (int)procsettings.rateofchange2;
                float[] lowpass4 = new float[lowpassradius];
                float[] lowpass6 = new float[lowpassradius];
                float[] lowpass8 = new float[lowpassradius];
                for (i = 0; i < lowpassradius; i++)
                {
                    lowpass4[i] = fourus;
                    lowpass6[i] = sixus;
                    lowpass8[i] = eightus;
                }
                _4usavg = lowpassradius * fourus;
                _6usavg = lowpassradius * sixus;
                _8usavg = lowpassradius * eightus;

                float val4 = 0, val6 = 0, val8 = 0;

                float timingcompensation = 0, prevtimingcomp;

                float timingcompfactor = procsettings.AdaptOffset2;

                for (i = start; i < end - lookahead; i++)
                {
                    if (i % 250000 == 249999) { progresses[threadid] = i; if (stop == 1) break; }
                    sectorboundary = (i - start + 50) % 9000;
                    if (procsettings.AddNoise == true && sectorboundary >= procsettings.addnoiselimitstart && sectorboundary <= procsettings.addnoiselimitend)
                        rand = rnd.Next(-1, 1) * procsettings.addnoiserndamount;
                    //    rand = rnd.Next(0 - procsettings.addnoiserndamount, procsettings.addnoiserndamount);
                    else
                        rand = 0;

                    value = (rxbuf[i] << procsettings.hd) + rand; // If it's a HD (user selectable option), multiply data by 2

                    val2 = value;
                    prevtimingcomp = timingcompensation;
                    timingcompensation = (averagetime / RateOfChange);
                    value -= (int)((timingcompensation - prevtimingcomp) / timingcompfactor);
                    //if (procsettings.UseErrorCorrection == false)
                    if (val2 < 4) continue;
                    //rxbuf[i] = (byte)value;
                    //rxbuf[i] = (byte)value;
                    //if (i > scatterplotstart && i < scatterplotend)
                    //    tbreceived.Append("" + value + "\t" + threshold4us.ToString("0.00")+"\t"+threshold6us.ToString("0.00")+"\r\n");
                    if (value <= threshold4us) // 4us
                    {

                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        _4us = fourus - val2;
                        if (procsettings.rateofchange2 != 0)
                        {
                            val4 = lowpass4[(i + 1) % lowpassradius];
                            _4usavg -= val4;
                            lowpass4[(i + 1) % lowpassradius] = val2;
                            _4usavg += val2;
                            fourus = _4usavg / lowpassradius;
                            //_4usavg = _4usavg + ((val2 - fourus) / procsettings.rateofchange2);

                        }
                        averagetime = _4us;
                    }
                    else
                    if (value > threshold4us && value < threshold6us) // 6us
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        _6us = sixus - val2;
                        if (procsettings.rateofchange2 != 0)
                        {
                            val6 = lowpass6[(i + 1) % lowpassradius];
                            _6usavg -= val6;
                            lowpass6[(i + 1) % lowpassradius] = val2;
                            _6usavg += val2;
                            sixus = _6usavg / lowpassradius;
                        }

                        averagetime = _6us;
                    }
                    else
                    if (value >= threshold6us) // 8us
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        _8us = eightus - val2;
                        if (procsettings.rateofchange2 != 0)
                        {
                            val8 = lowpass8[(i + 1) % lowpassradius];
                            _8usavg -= val8;
                            lowpass8[(i + 1) % lowpassradius] = val2;
                            _8usavg += val2;
                            eightus = _8usavg / lowpassradius;
                        }

                        averagetime = _8us;
                    }
                }
            }
            else if (processingtype == ProcessingType.normal) //************ Normal ****************
            {
                //tbreceived.Append("Normal\r\n");

                // This ensures that the period data can be synched with mfm and marker data
                // Which is important for error correction methods
                if (procsettings.UseErrorCorrection)
                {
                    MINUS = 0;
                    EIGHTUS = 255;
                }
                Random rnd = new Random();
                int rand = 0;
                int sectorboundary = 0;
                int val;
                //int k;
                stat4us = 0;
                stat6us = 0;
                stat8us = 0;
                for (i = start; i < end; i++)
                {
                    if (i % 250000 == 249999) { progresses[threadid] = i; if (stop == 1) break; }

                    sectorboundary = (i - start + 50) % 9000;

                    if (procsettings.AddNoise == true && sectorboundary >= procsettings.addnoiselimitstart && sectorboundary <= procsettings.addnoiselimitend)
                        rand = rnd.Next(-1, 1) * procsettings.addnoiserndamount;
                    //    rand = rnd.Next(0 - procsettings.addnoiserndamount, procsettings.addnoiserndamount);
                    else
                        rand = 0;
                    //value2 = (rxbuf[i]<<hd);
                    //value = value2 + rand;
                    val = rxbuf[i];
                    value = (val << procsettings.hd) + rand;
                    if (val < 4) continue;
                    if (value >= MINUS && value < FOURUS)
                    //if (value < FOURUS)
                    {
                        stat4us++;
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                    }
                    else
                    if (value >= FOURUS && value < SIXUS)
                    {
                        stat6us++;
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                    }
                    else
                    if (value >= SIXUS && value <= EIGHTUS)
                    //if (value >= SIXUS)
                    {
                        stat8us++;
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                    }
                    else
                    {
                        if (procsettings.pattern == 1)
                        {
                            m[mfmlengths[threadid]++] = 1;
                            m[mfmlengths[threadid]++] = 0;
                        }
                        else if (procsettings.pattern == 2)
                        {
                            m[mfmlengths[threadid]++] = 1;
                            m[mfmlengths[threadid]++] = 0;
                            m[mfmlengths[threadid]++] = 0;
                        }
                        else if (procsettings.pattern == 3)
                        {
                            m[mfmlengths[threadid]++] = 1;
                            m[mfmlengths[threadid]++] = 0;
                            m[mfmlengths[threadid]++] = 0;
                            m[mfmlengths[threadid]++] = 0;
                        }
                        else if (procsettings.pattern == 4)
                        {

                            float s1, s2, s3;
                            int total = stat4us + stat6us + stat8us;
                            if (total == 0) total = 1;
                            s1 = stat4us / (float)total * 100.0f;
                            s2 = (float)stat6us / (float)total * 100.0f;
                            s3 = (float)stat8us / (float)total * 100.0f;
                            int weight = rnd.Next(0, 99);
                            //s1 = 25;
                            //s2 = 50;
                            //s3 = 75;
                            m[mfmlengths[threadid]++] = 1;

                            if (weight < s1)
                                m[mfmlengths[threadid]++] = 0;
                            else
                            if (weight >= s2 && weight < s3)
                            {
                                m[mfmlengths[threadid]++] = 0;
                                m[mfmlengths[threadid]++] = 0;
                            }
                            else
                            if (weight >= s3)
                            {
                                m[mfmlengths[threadid]++] = 0;
                                m[mfmlengths[threadid]++] = 0;
                                m[mfmlengths[threadid]++] = 0;
                            }
                            /*
                            int r = rnd.Next(0, 3);
                            if( r != 0)
                            {
                                m[mfmlengths[threadid]++] = 1;

                                for (k = 0; k <  r; k++)
                                    m[mfmlengths[threadid]++] = 0;
                            }
                            */

                        }
                    }
                }
            }
            else if (processingtype == ProcessingType.aufit) //************ Aufit ****************
            {
                int bitwindow = 0;
                int currenttime = 0;
                if (scatterplotend - scatterplotstart > 10000) scatterplotend = scatterplotstart + 2000;
                DPLL dpll = new DPLL();

                for (i = start; i < end; i++)
                {
                    if (i % 250000 == 249999) { progresses[threadid] = i; if (stop == 1) break; }
                    if (rxbuf[i] < 4 && procsettings.UseErrorCorrection == false) continue;
                    currenttime += (int)(((rxbuf[i] << procsettings.hd) + FOURUS) * MINUS);

                    bitwindow = dpll.bitSpacing(currenttime);

                    //if (i > scatterplotstart && i < scatterplotend)
                    //    tbreceived.Append("" + rxbuf[i] + "\t" + bitwindow + "\t" + currenttime +"\t"+dpll.error+ "\r\n");

                    int check = mfmlengths[threadid];
                    if (bitwindow == 2)
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                    }
                    else
                    if (bitwindow == 3)
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                    }
                    else
                    if (bitwindow == 4)
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                        m[mfmlengths[threadid]++] = 0;
                    }
                    else
                    {
                        m[mfmlengths[threadid]++] = 1;
                        m[mfmlengths[threadid]++] = 0;
                    }
                }
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
            if (procsettings.platform == 0)
                ProcessPCMFM2Sectordata(procsettings, threadid);
            else if (procsettings.platform == 1)
                ProcessAmigaMFMbytes(procsettings, threadid);

            //tbreceived.Append("mfm to sector:"+ SW.ElapsedMilliseconds + "ms\r\n");
            SW.Stop();


        }

        public void FindPeaks(int offset)
        {
            byte[] data;

            int length = 0;
            if (indexrxbuf < 100000)
                length = indexrxbuf;
            else length = 100000;
            //int offset = HistogramhScrollBar1.Value;
            //int peak1, peak2, peak3;
            data = rxbuf;

            int i;

            if (data == null) return;

            int[] histogramint = new int[256];

            int histogrammax, histogrammaxprev;

            if (length == 0) length = data.Length;

            //Create histogram of the track period data

            // count the period lengths grouped by period length, skip 0
            for (i = offset; i < offset + length; i++)
            {
                //if (data[i] > 0)
                histogramint[data[i]]++;
            }

            // Find the maximum value so we can normalize the histogram down to 100 to fit inside histogram graph

            peak1 = 0;
            peak2 = 0;
            peak3 = 0;

            histogrammax = 0;
            histogrammaxprev = 0;
            for (i = 1; i < 256; i++)
            {
                if (histogramint[i] > histogrammax)
                {
                    histogrammaxprev = histogrammax;
                    histogrammax = histogramint[i];
                    peak1 = i;
                }
            }

            histogrammax = 0;
            histogrammaxprev = 0;
            for (i = peak1 + 20; i < 256; i++)
            {
                if (histogramint[i] > histogrammax)
                {
                    histogrammaxprev = histogrammax;
                    histogrammax = histogramint[i];
                    peak2 = i;
                }
            }

            histogrammax = 0;
            histogrammaxprev = 0;
            for (i = peak2 + 20; i < 256; i++)
            {
                if (histogramint[i] > histogrammax)
                {
                    histogrammaxprev = histogrammax;
                    histogrammax = histogramint[i];
                    peak3 = i;
                }
            }


            tbreceived.Append("Peak1: " + peak1.ToString("X2") + "Peak2: " + peak2.ToString("X2") + "Peak3: " + peak3.ToString("X2") + "\r\n");
        }

        // Dumps an object to tbreceived
        private void PrintProperties(Object myObj)
        {
            foreach (var prop in myObj.GetType().GetProperties())
            {
                tbreceived.Append(prop.Name + ": " + prop.GetValue(myObj, null));
            }

            foreach (var field in myObj.GetType().GetFields())
            {
                tbreceived.Append(field.Name + ": " + field.GetValue(myObj));
            }
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
            uint hex = 0, temp;
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
                temp = (mfmlo & 0x80000000);
                hex |= ((mfmlo & 0x80000000) >> i);
                mfmlo <<= 2;
            }

            return hex;
        }

        // Amiga style mfm to byte
        public byte MFM2BINbyte(byte mfmhi, byte mfmlo)
        {
            byte hex = 0, temp;
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
                temp = (byte)(mfmlo & 0x80);
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
        public byte[] MFM2ByteArray(byte[] mfm, int offset, int length)
        {
            int i, j;
            int mindex = 0;
            byte[] m = new byte[length / 8];

            byte hex = 0;
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
            string path = subpath + @"\" + procsettings.outputfilename + @"\";

            path = subpath + @"\" + procsettings.outputfilename + @"\";
            try
            {
                File.WriteAllText(path + procsettings.outputfilename + "_FoundGoodSectorInfo.txt", FoundGoodSectorInfo.ToString());
            }
            catch (IOException ex)
            {
                tbreceived.Append("IO error writing sector map: \r\n" + ex.ToString() + "\r\n");
            }
        }

        public void LoadGoodSectorFoundInfo()
        {
            string subpath = @Properties.Settings.Default["PathToRecoveredDisks"].ToString();
            string path = subpath + @"\" + procsettings.outputfilename + @"\";

            path = subpath + @"\" + procsettings.outputfilename + @"\";
            try
            {
                FoundGoodSectorInfo.Clear();
                FoundGoodSectorInfo.Append(File.ReadAllText(path + procsettings.outputfilename + "_FoundGoodSectorInfo.txt"));
            }
            catch (IOException ex)
            {
                tbreceived.Append("IO error Reading sector map: \r\n" + ex.ToString() + "\r\n");
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
            int lastFidx = 0;

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
                    i = i - lastFidx;
                    lastFidx = 0;
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
    }

}

