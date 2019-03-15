using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Security;
using System.Collections.Generic;
using FloppyControlApp.MyClasses;
using System.Reflection;
using FloppyControlApp.Properties;

namespace FloppyControlApp
{
    
    /*
        This application produces bin files of captured floppy disk RDATA. The format is:
        The stream can start in any track and any sector within the track or on the gap.
        Markers determine where data is:
        
        0x01 Index pulse is received, at this point in the data the Index marker was found and the start of the track,
        which is valid for PC DOS but not for Amiga as it doesn't use/ignores the index. The index is indicated using 
        the index offset within the header.
        
        0x02 Track marker, the next byte contains the track number. 
        All other bytes are data ranging from 0x04 to 0xff being counter data with an interval of 50ns (currently, 
        depending on the Osc used on the PIC micro controller).
    */

    public partial class FloppyControl : Form
    {
        class badsectorkeyval
        {
            public string name { get; set; }
            public int id { get; set; }
            public int threadid { get; set; }
        }


        public class ComboboxItem
        {
            public string Text { get; set; }
            public int id { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        

        class SectorMapContextMenu
        {
            public int track { get; set; }
            public int sector { get; set; }
            public int duration { get; set; }
            public MFMData sectordata { get; set; }
            public int cmd { get; set; }
        }

        private FDDProcessing processing;
        private ControlFloppy controlfloppy;
        private connectsocketNIVisa2 scope = new connectsocketNIVisa2();
        private BinaryReader reader;
        private StringBuilder SectorInfo = new StringBuilder();
        private StringBuilder tbtxt = new StringBuilder();
        private BinaryWriter writer;
        private Point BadSectorTooltipPos;
        private StringBuilder tbreceived = new StringBuilder();
        //private Graphset graphset;
        private Histogram ECHisto;
        private Histogram ScatterHisto;
        private ScatterPlot scatterplot;
        private static readonly object lockaddmarker = new object();
        //private static uint markerpositionscnt;
        private string subpath;
        private string path = "";
        private string selectedPortName;
        private string[] openfilespaths;
        private int disablecatchkey = 0;
        private int binfilecount = 0; // Keep saving each capture under a different filename as to keep all captured data
        private int capturetime = 0;
        private int capturing = 0;
        private int selectedBaudRate = 5000000;
        private int graphselect = 0;
        private int maxthreads = 2;
        private int byteinsector = 0;
        //private int stepspertrack = 8;
        private byte[] TempSector = new byte[550];
        private byte[][] graphwaveform = new byte[15][];
        private bool AddData = false;
        private bool openFilesDlgUsed = false;
        private bool scanactive = false;
        private bool stopupdatingGraph = false;
        private int[] mfmbyteenc = new int[256];
        private int indexrxbufprevious = 0;
        Version version;
        FileIO fileio;
        WaveformEdit oscilloscope;


        public FloppyControl()
        {
            version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1)
                                    .AddDays(version.Build).AddSeconds(version.Revision * 2);
            string displayableVersion = $"{version} ({buildDate})";

            ECHisto = new Histogram();
            ScatterHisto = new Histogram();

            int i;
            InitializeComponent();
            this.Text+= " v"+version.ToString();
            processing = new FDDProcessing();

            processing.GetProcSettingsCallback += GetProcSettingsCallback;
            processing.rtbSectorMap = rtbSectorMap;
            processing.tbreceived = tbreceived;
            processing.sectormap.SectorMapUpdateGUICallback += SectorMapUpdateGUICallback;
            processing.sectormap.rtbSectorMap = rtbSectorMap;

            controlfloppy = new ControlFloppy();
            controlfloppy.rxbuf = processing.rxbuf;
            controlfloppy.processing = processing;

            scatterplot = new ScatterPlot(processing, processing.sectordata2, 0, 0, ScatterPictureBox);
            scatterplot.tbreiceved = tbreceived;
            scatterplot.rxbuf = processing.rxbuf;
            scatterplot.UpdateEvent += updateAnScatterPlot;
            scatterplot.ShowGraph += ScatterPlotShowGraphCallback;
            scatterplot.EditScatterplot = EditScatterPlotcheckBox.Checked;
            processing.indexrxbuf = 0;

            
            outputfilename.Text = (string)Properties.Settings.Default["BaseFileName"];
            DirectStepCheckBox.Checked = (bool)Properties.Settings.Default["DirectStep"];
            MicrostepsPerTrackUpDown.Value = (int)Properties.Settings.Default["MicroStepping"];
            TRK00OffsetUpDown.Value = (int)Properties.Settings.Default["TRK00Offset"];

            subpath = @Properties.Settings.Default["PathToRecoveredDisks"].ToString();

            fileio = new FileIO();
            fileio.FilesAvailableCallback += FilesAvailableCallback;
            fileio.processing = processing;
            fileio.resetinput += resetinput;
            fileio.textBoxFilesLoaded = textBoxFilesLoaded;
            fileio.tbreceived = tbreceived;
            fileio.rtbSectorMap = rtbSectorMap;

            oscilloscope = new WaveformEdit(GraphPictureBox, fileio, processing);
            
            oscilloscope.updateGraphCallback += updateGraphCallback;
            oscilloscope.GraphsetGetControlValuesCallback += GraphsetGetControlValuesCallback;
            oscilloscope.resetinput += resetinput;
            oscilloscope.FilterGuiUpdateCallback += FilterGuiUpdateCallback;
            oscilloscope.Filter2GuiCallback += Filter2GuiCallback;
            EditOptioncomboBox.SelectedIndex = 0;
            EditModecomboBox.SelectedIndex = 0;

            textBoxReceived.AppendText("PortName: " + selectedPortName + "\r\n");

            //comboBoxPort.SelectedItem = "COM9";
            updateSliderLabels();

            // Set the steps per track default to MicroStepping, so a full step is used.
            // Note that due to the tracks are separated by 1 track, two full steps are taken
            // To do this, this value is multiplied by 2. Due to this you can only use the
            // smallest step 1 multiplied by 2. To get to the first step you can use TRK00 offset
            // increase or decrease by one.

            timer1.Start();
            MainTabControl.SelectedTab = ProcessingTab;
            //MainTabControl.SelectedTab = AnalysisPage;
            BadSectorTooltip.Hide();
            timer5.Start();
            GUITimer.Start();
            BluetoRedByteCopyToolBtn.Tag = new int();
            BluetoRedByteCopyToolBtn.Tag = 0;
            

            //ScatterPictureBox.MouseWheel += ScatterPictureBox_MouseWheel;

            ECHisto.setPanel(AnHistogramPanel);
            ScatterHisto.setPanel(Histogrampanel1);
            ProcessingTab.Enabled = false;
            PeriodBeyond8uscomboBox.SelectedIndex = 0;

            ChangeDiskTypeComboBox.Items.AddRange(Enum.GetNames(typeof(DiskFormat)));
            QChangeDiskTypeComboBox.Items.AddRange(Enum.GetNames(typeof(DiskFormat)));
            ProcessingModeComboBox.Items.AddRange(Enum.GetNames(typeof(ProcessingType)));
            ProcessingModeComboBox.SelectedItem = ProcessingType.adaptive1.ToString();

            QProcessingModeComboBox.Items.AddRange(Enum.GetNames(typeof(ProcessingType)));
            QProcessingModeComboBox.SelectedItem = ProcessingType.adaptive1.ToString();

            ScanComboBox.Items.AddRange(Enum.GetNames(typeof(ScanMode)));
            ScanComboBox.SelectedItem = ScanMode.AdaptiveRate.ToString();

            QScanComboBox.Items.AddRange(Enum.GetNames(typeof(ScanMode)));
            QScanComboBox.SelectedItem = ScanMode.AdaptiveRate.ToString();


            QMinUpDown.Value = MinvScrollBar.Value;
            QFourSixUpDown.Value = FourvScrollBar.Value;
            QSixEightUpDown.Value = SixvScrollBar.Value;
            QMaxUpDown.Value = EightvScrollBar.Value;
            QOffsetUpDown.Value = OffsetvScrollBar1.Value;

            if (HDCheckBox.Checked)
            {
                ScatterHisto.hd = 1;
                processing.procsettings.hd = 1;
                //hddiv = 2;
            }
            else
            {
                ScatterHisto.hd = 0;
                processing.procsettings.hd = 0;
                //hddiv = 1;
            }

            ProcessStatusLabel.BackColor = Color.Transparent;
            //BSEditByteUpDown.Tag = 0;

            // build gradient for scatter plot

            //int p;

            // 3F 64 8D
            // 63 100 141

        }

        private void ScatterPlotShowGraphCallback()
        {
            int grphcnt = oscilloscope.graphset.Graphs.Count;

            int i;


            if (MainTabControl.SelectedTab == ErrorCorrectionTab)
            {
                int offset = 0;
                for (i = 0; i < processing.sectordata2.Count; i++)
                {
                    if (processing.sectordata2[i].rxbufMarkerPositions > scatterplot.rxbufclickindex)
                    {
                        offset = scatterplot.rxbufclickindex - processing.sectordata2[i - 1].rxbufMarkerPositions;
                        break;
                    }
                }

                MFMData sd = processing.sectordata2[i - 1];

                var mfms = processing.mfms[sd.threadid];
                int index = 0;
                for (i = 0; i < mfms.Length; i++)
                {
                    if (mfms[i + sd.MarkerPositions] == 1)
                    {
                        index++;
                    }
                    if (index == offset)
                    {
                        break;
                    }
                }

                int mfmindex = i;
                mfmindex /= 8;
                mfmindex *= 8;
                int offsetmfmindex = 0;
                switch ((int)processing.diskformat)
                {
                    case 0:
                        return;
                        break;
                    case 1: //AmigaDos
                        offsetmfmindex = 48;
                        break;
                    case 2://diskspare
                        offsetmfmindex = 24;
                        break;
                    case 3://pc2m
                        offsetmfmindex = 0;
                        break;
                    case 4://pcdd
                        offsetmfmindex = 0;
                        break;
                    case 5://pchd
                        offsetmfmindex = 0;
                        break;
                }


                MFMByteStartUpDown.Value = mfmindex + offsetmfmindex;
                ScatterMinTrackBar.Value = offset;
                ScatterMaxTrackBar.Value = offset + 14;
                updateECInterface();
            }
            else
            {
                if (grphcnt == 0)
                {
                    return;
                }
                for (i = 0; i < grphcnt; i++)
                {
                    oscilloscope.graphset.Graphs[i].datalength = 1000;
                    oscilloscope.graphset.Graphs[i].dataoffset = scatterplot.graphindex - 500;

                    if (oscilloscope.graphset.Graphs[i].dataoffset < 0)
                        oscilloscope.graphset.Graphs[i].dataoffset = 0;

                    oscilloscope.graphset.Graphs[i].changed = true;
                    oscilloscope.graphset.Graphs[i].density = 1;
                }

                oscilloscope.graphset.UpdateGraphs();
                MainTabControl.SelectedTab = AnalysisTab2;
            }

        }

        private void SectorMapUpdateGUICallback()
        {
            RecoveredSectorsLabel.Text = processing.sectormap.recoveredsectorcount.ToString();
            RecoveredSectorsWithErrorsLabel.Text = processing.sectormap.RecoveredSectorWithErrorsCount.ToString();
            GoodHdrCntLabel.Text = processing.GoodSectorHeaderCount.ToString();
            MarkersLabel.Text = processing.sectordata2.Count.ToString();
            BadSectorsCntLabel.Text = processing.sectormap.badsectorcnt.ToString();
            statsLabel.Text = processing.sectormap.s1.ToString("0.00") + " " +
                                processing.sectormap.s2.ToString("0.00") + " " +
                                processing.sectormap.s3.ToString("0.00");

            if (this.Width == 1620 || this.Width == 1630 || this.Width == 1680)
            {
                this.Width = processing.sectormap.WindowWidth;
            }

            switch ((int)processing.diskformat)
            {
                case 0:
                    DiskTypeLabel.Text = "Unknown";
                    //processing.processing.sectorspertrack = 0;
                    break;
                case 1:
                    DiskTypeLabel.Text = "AmigaDOS";
                    //processing.sectorspertrack = 11;
                    break;
                case 2:
                    DiskTypeLabel.Text = "DiskSpare";
                    //processing.sectorspertrack = 12;
                    break;
                case 3:
                    DiskTypeLabel.Text = "PC/MSX DD";
                    //processing.sectorspertrack = 9;
                    break;
                case 4:
                    DiskTypeLabel.Text = "PC HD";
                    //processing.sectorspertrack = 18;
                    break;
                case 5:
                    DiskTypeLabel.Text = "PC 2M";
                    //processing.sectorspertrack = 11;
                    break;

                default:
                    DiskTypeLabel.Text = processing.diskformat.ToString();
                    break;
            }

        }
        private void ProcessingGUICallback()
        {

        }

        private void GetProcSettingsCallback()
        {
            ProcessingType procmode = ProcessingType.adaptive1;
            if (ProcessingModeComboBox.SelectedItem.ToString() != "")
                procmode = (ProcessingType)Enum.Parse(typeof(ProcessingType), ProcessingModeComboBox.SelectedItem.ToString(), true);
            processing.procsettings.processingtype = procmode;
            //if (NormalradioButton.Checked == true) processing.procsettings.processingtype = ProcessingType.normal;
            //else if (AdaptradioButton.Checked == true) processing.procsettings.processingtype = ProcessingType.adaptive;
            //else if (AufitRadioButton.Checked == true) processing.procsettings.processingtype = ProcessingType.aufit;
            processing.procsettings.NumberOfDups = (int)DupsUpDown.Value;
            processing.procsettings.pattern = PeriodBeyond8uscomboBox.SelectedIndex;
            //tbreceived.Append("Combobox:" + PeriodBeyond8uscomboBox.SelectedIndex + "\r\n");

            processing.procsettings.offset = OffsetvScrollBar1.Value;
            processing.procsettings.min = MinvScrollBar.Value + processing.procsettings.offset;
            processing.procsettings.four = FourvScrollBar.Value + processing.procsettings.offset;
            processing.procsettings.six = SixvScrollBar.Value + processing.procsettings.offset;
            processing.procsettings.max = EightvScrollBar.Value + processing.procsettings.offset;
            processing.procsettings.SkipPeriodData = false;
            processing.procsettings.AutoRefreshSectormap = AutoRefreshSectorMapCheck.Checked;
            processing.procsettings.start = (int)rxbufStartUpDown.Value;
            processing.procsettings.end = (int)rxbufEndUpDown.Value;

            processing.procsettings.finddupes = FindDupesCheckBox.Checked;
            processing.procsettings.rateofchange = (float)RateOfChangeUpDown.Value;
            //processing.procsettings.platform = platform; // 1 = Amiga
            processing.procsettings.UseErrorCorrection = ECOnRadio.Checked;
            processing.procsettings.OnlyBadSectors = OnlyBadSectorsRadio.Checked;
            processing.procsettings.AddNoise = AddNoisecheckBox.Checked;

            processing.procsettings.limittotrack = (int)LimitToTrackUpDown.Value;
            processing.procsettings.limittosector = (int)LimitToSectorUpDown.Value;
            processing.procsettings.NumberOfDups = (int)DupsUpDown.Value;
            processing.procsettings.LimitTSOn = LimitTSCheckBox.Checked;
            processing.procsettings.IgnoreHeaderError = IgnoreHeaderErrorCheckBox.Checked;
            //processing.procsettings.AdaptOffset = (int)AdaptOffsetUpDown.Value;
            processing.procsettings.AdaptOffset2 = (float)AdaptOfsset2UpDown.Value;
            processing.procsettings.rateofchange2 = (int)RateOfChange2UpDown.Value;

            if (LimitToScttrViewcheckBox.Checked == true && OnlyBadSectorsRadio.Checked == true)
            {
                processing.procsettings.addnoiselimitstart = ScatterMinTrackBar.Value + 50;
                processing.procsettings.addnoiselimitend = ScatterMaxTrackBar.Value + 50;
            }
            else
            {
                processing.procsettings.addnoiselimitstart = 0;
                processing.procsettings.addnoiselimitend = processing.indexrxbuf;
            }
            processing.procsettings.addnoiserndamount = (int)RndAmountUpDown.Value;
        }

        

        

        private void FloppyControl_KeyDown(object sender, KeyEventArgs e)
        {
            processing.stop = 0;
            if (disablecatchkey == 0)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    scope.Disconnect();
                    this.Close();
                    return;
                }
                if (e.KeyCode == Keys.A)
                {
                    RateOfChange2UpDown.Focus();
                    Application.DoEvents();
                    processing.StartProcessing(1);
                }
                if (e.KeyCode == Keys.P)
                    ProcessPC();
                if (e.KeyCode == Keys.S)
                    ScanButton.PerformClick();

                if (MainTabControl.SelectedTab == AnalysisTab2)
                {
                    if (e.KeyCode == Keys.D1)
                        EditOptioncomboBox.SelectedIndex = 0;
                    if (e.KeyCode == Keys.D2)
                        EditOptioncomboBox.SelectedIndex = 1;
                    if (e.KeyCode == Keys.D3)
                        EditOptioncomboBox.SelectedIndex = 2;

                    if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z)
                        EditUndobutton.PerformClick();
                }

            }
        }

        // Do the Amiga sector data processing
        private void ProcessAmigaBtn_Click(object sender, EventArgs e)
        {
            processing.stop = 0;
            ProcessAmiga();
        }
        private void ProcessAmiga()
        {
            if (ClearDatacheckBox.Checked)
                resetprocesseddata();
            //textBoxReceived.Clear();
            processing.scatterplotstart = scatterplot.AnScatViewlargeoffset + scatterplot.AnScatViewoffset;
            processing.scatterplotend = scatterplot.AnScatViewlargeoffset + scatterplot.AnScatViewoffset + scatterplot.AnScatViewlength;
            processing.StartProcessing(1);
        }

        private void ProcessPC()
        {
            if (ClearDatacheckBox.Checked)
                resetprocesseddata();
            //textBoxReceived.Clear();
            processing.scatterplotstart = scatterplot.AnScatViewlargeoffset + scatterplot.AnScatViewoffset;
            processing.scatterplotend = scatterplot.AnScatViewlargeoffset + scatterplot.AnScatViewoffset + scatterplot.AnScatViewlength;
            processing.StartProcessing(0);
            ChangeDiskTypeComboBox.SelectedItem = processing.diskformat.ToString();
        }

        private void ProcessPCBtn_Click(object sender, EventArgs e)
        {
            processing.stop = 0;
            ProcessPC();
        }
        
        public void updateForm()
        {
            //RecoveredSectorsLabel.Text = recoveredsectorcount.ToString();

            //RecoveredSectorsLabel.Invalidate();
            //RecoveredSectorsLabel.Update();
            //RecoveredSectorsLabel.Refresh();
            Application.DoEvents();
        }

        // Updates the labels under the sliders
        // as well as the indicators under the histogram
        private void updateSliderLabels()
        {
            int x, y;

            if ((OffsetvScrollBar1.Value + MinvScrollBar.Value) < 0)
                MinLabel.Text = 0.ToString("X2");
            else MinLabel.Text = (OffsetvScrollBar1.Value + MinvScrollBar.Value).ToString("X2");
            FourLabel.Text = (OffsetvScrollBar1.Value + FourvScrollBar.Value).ToString("X2");
            SixLabel.Text = (OffsetvScrollBar1.Value + SixvScrollBar.Value).ToString("X2");
            EightLabel.Text = (OffsetvScrollBar1.Value + EightvScrollBar.Value).ToString("X2");
            Offsetlabel.Text = OffsetvScrollBar1.Value.ToString("D2");

            Graphics formGraphics = null;
            if (MainTabControl.SelectedTab == ProcessingTab)
            {
                formGraphics = Histogrampanel1.CreateGraphics();
            }
            if (MainTabControl.SelectedTab == QuickTab)
            {
                formGraphics = QHistoPanel.CreateGraphics();
            }

            if (formGraphics != null)
            {
                using (var bmp = new System.Drawing.Bitmap(580, 12))
                {
                    LockBitmap lockBitmap = new LockBitmap(bmp);
                    lockBitmap.LockBits();
                    lockBitmap.FillBitmap(SystemColors.Control);

                    x = MinvScrollBar.Value - 4 + OffsetvScrollBar1.Value;
                    y = 0;
                    lockBitmap.Line(000 + x, 005 + y, 005 + x, 000 + y, Color.Black);
                    lockBitmap.Line(005 + x, 000 + y, 010 + x, 005 + y, Color.Black);
                    lockBitmap.Line(010 + x, 005 + y, 000 + x, 005 + y, Color.Black);

                    x = FourvScrollBar.Value - 4 + OffsetvScrollBar1.Value;

                    lockBitmap.Line(000 + x, 005 + y, 005 + x, 000 + y, Color.Black);
                    lockBitmap.Line(005 + x, 000 + y, 010 + x, 005 + y, Color.Black);
                    lockBitmap.Line(010 + x, 005 + y, 000 + x, 005 + y, Color.Black);

                    x = SixvScrollBar.Value - 4 + OffsetvScrollBar1.Value;

                    lockBitmap.Line(000 + x, 005 + y, 005 + x, 000 + y, Color.Black);
                    lockBitmap.Line(005 + x, 000 + y, 010 + x, 005 + y, Color.Black);
                    lockBitmap.Line(010 + x, 005 + y, 000 + x, 005 + y, Color.Black);

                    x = EightvScrollBar.Value - 4 + OffsetvScrollBar1.Value;

                    lockBitmap.Line(000 + x, 005 + y, 005 + x, 000 + y, Color.Black);
                    lockBitmap.Line(005 + x, 000 + y, 010 + x, 005 + y, Color.Black);
                    lockBitmap.Line(010 + x, 005 + y, 000 + x, 005 + y, Color.Black);

                    lockBitmap.UnlockBits();
                    formGraphics.DrawImage(bmp, 0, 103);
                }
                formGraphics.Dispose();
            }
        }

        

        private void ConnectClassbutton_Click(object sender, EventArgs e)
        {
            ConnectToFloppyControlHardware();
        }

        private void ConnectToFloppyControlHardware()
        {
            controlfloppy.binfilecount = binfilecount;
            controlfloppy.DirectStep = DirectStepCheckBox.Checked;
            controlfloppy.MicrostepsPerTrack = (int)MicrostepsPerTrackUpDown.Value;
            controlfloppy.trk00offset = (int)TRK00OffsetUpDown.Value;
            controlfloppy.EndTrack = (int)EndTracksUpDown.Value;
            controlfloppy.StartTrack = (int)StartTrackUpDown.Value;
            controlfloppy.tbr = tbreceived;
            //processing.indexrxbuf            = indexrxbuf;
            controlfloppy.StepStickMicrostepping = (int)Properties.Settings.Default["MicroStepping"];
            controlfloppy.outputfilename = outputfilename.Text;
            controlfloppy.rxbuf = processing.rxbuf;

            // Callbacks
            controlfloppy.updateHistoAndSliders = updateHistoAndSliders;
            controlfloppy.ControlFloppyScatterplotCallback = ControlFloppyScatterplotCallback;
            controlfloppy.Setrxbufcontrol = Setrxbufcontrol;

            if (!controlfloppy.serialPort1.IsOpen) // Open connection if it's closed
            {
                controlfloppy.ConnectFDD();
                if (controlfloppy.serialPort1.IsOpen)
                {
                    LabelStatus.Text = "Connected.";
                }
                else
                {
                    LabelStatus.Text = "Disconnected.";
                }
            }
            else // Close connection if open
                DisconnectFromFloppyControlHardware();
        }

        private void DisconnectFromFloppyControlHardware()
        {
            if (controlfloppy.serialPort1.IsOpen)
            {
                controlfloppy.Disconnect();
                LabelStatus.Text = "Disconnected.";
            }
        }

        // Update scatterplot while capturing
        public void ControlFloppyScatterplotCallback()
        {
            
            scatterplot.rxbuf = processing.rxbuf;
            scatterplot.AnScatViewlargeoffset = processing.rxbuf.Length - controlfloppy.recentreadbuflength;
            if (scatterplot.AnScatViewlargeoffset < 0)
                scatterplot.AnScatViewlargeoffset = 0;
            scatterplot.AnScatViewoffset = 0;
            scatterplot.AnScatViewlength = controlfloppy.recentreadbuflength;
            controlfloppy.recentreadbuflength = 0;
            //scatterplot.UpdateScatterPlot();
            CurrentTrackLabel.Text = controlfloppy.currenttrackPrintable.ToString();
            updateAllGraphs();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (capturing == 1 || processing.processing == 1)
                capturetime++;
            // bytes per second
            // and total bytes received

            if (controlfloppy.capturecommand == 1)
            {
                BytesReceivedLabel.Text = string.Format("{0:n0}", processing.indexrxbuf);
                BytesPerSecondLabel.Text = string.Format("{0:n0}", controlfloppy.bytespersecond);
                CaptureTimeLabel.Text = capturetime.ToString();
                controlfloppy.bytespersecond = 0;
                BufferSizeLabel.Text = string.Format("{0:n0}", processing.indexrxbuf);

                indexrxbufprevious = processing.rxbuf.Length;
                //processing.rxbuf = controlfloppy.tempbuffer.Skip(Math.Max(0, controlfloppy.tempbuffer.Count()-30)).SelectMany(a => a).ToArray();
                

                //tbreceived.Append("indexrxbufprevious: "+indexrxbufprevious.ToString()+"processing.rxbuf.Length:"+ processing.rxbuf.Length.ToString());
                controlfloppy.rxbuf = processing.rxbuf;
                if (processing.rxbuf.Length > 100000)
                    controlfloppy.recentreadbuflength = 100000; // controlfloppy.recentreadbuflength = processing.indexrxbuf - indexrxbufprevious;
                //tbreceived.Append("Recent received:"+controlfloppy.recentreadbuflength.ToString());
                processing.indexrxbuf = processing.rxbuf.Length - 1;
                ControlFloppyScatterplotCallback();
            }

            if (processing.indexrxbuf > 0)
                ProcessingTab.Enabled = true;



            if (openFilesDlgUsed == true)
            {
                openFilesDlgUsed = false;
                fileio.openfiles();
                UpdateHistoAndScatterplot();
                //createhistogram1();
            }
        }

        void FilesAvailableCallback()
        {
            openFilesDlgUsed = true;
        }

        void UpdateHistoAndScatterplot()
        {
            Setrxbufcontrol();
            outputfilename.Text = fileio.BaseFileName;
            if (processing.indexrxbuf < 100000)
                scatterplot.AnScatViewlength = processing.indexrxbuf;
            else scatterplot.AnScatViewlength = 99999;
            scatterplot.AnScatViewoffset = 0;
            scatterplot.UpdateScatterPlot();
            updateHistoAndSliders();
            //ScatterHisto.DoHistogram(rxbuf, (int)rxbufStartUpDown.Value, (int)rxbufEndUpDown.Value);
            if (processing.indexrxbuf > 0)
                ProcessingTab.Enabled = true;
        }

        private void OpenBinFilebutton_Click(object sender, EventArgs e)
        {
            fileio.ShowOpenBinFiles();
        }

        private void AddDataButton_Click(object sender, EventArgs e)
        {
            fileio.ShowAddBinFiles();
        }

        private void Histogrampanel1_Paint(object sender, PaintEventArgs e)
        {
            if (processing.indexrxbuf > 0)
            {

                updateAnScatterPlot();
                ScatterHisto.DoHistogram();
                updateSliderLabels();
            }
        }

        private void FourvScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (!scanactive)
            {
                QMinUpDown.Value = MinvScrollBar.Value;
                QFourSixUpDown.Value = FourvScrollBar.Value;
                QSixEightUpDown.Value = SixvScrollBar.Value;
                QMaxUpDown.Value = EightvScrollBar.Value;
                QOffsetUpDown.Value = OffsetvScrollBar1.Value;
                updateSliderLabels();
                scatterplot.UpdateScatterPlot();
                scatterplot.UpdateScatterPlot();
            }
        }

        private void QMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!scanactive)
            {
                MinvScrollBar.Value = (int)QMinUpDown.Value;
                FourvScrollBar.Value = (int)QFourSixUpDown.Value;
                SixvScrollBar.Value = (int)QSixEightUpDown.Value;
                EightvScrollBar.Value = (int)QMaxUpDown.Value;
                OffsetvScrollBar1.Value = (int)QOffsetUpDown.Value;

                updateSliderLabels();
                scatterplot.UpdateScatterPlot();
                scatterplot.UpdateScatterPlot();
            }
        }

        private void SaveDiskImageButton_Click(object sender, EventArgs e)
        {
            fileio.SaveDiskImage();
        }

        // Resets all data that was produced by processing but keeps rxbuf intact
        private void resetprocesseddata()
        {
            

            int i;
            processing.badsectorhash = new byte[5000000][];

            BadSectorListBox.Items.Clear();
            processing.sectordata2.Clear();

            for (i = 0; i < processing.mfmsindex; i++)
            {

                //BadSectors[i] = new byte[0];
                processing.mfms[i] = new byte[0];
            }
            OnlyBadSectorsRadio.Checked = false; // When the input buffer is changed or empty, we can't scan for only bad sectors
            processing.mfmsindex = 0;
            GC.Collect();
        }

        private void resetinput()
        {
            int i;
            ProcessingTab.Enabled = false;
            processing.badsectorhash = null;
            processing.badsectorhash = new byte[5000000][];

            BadSectorListBox.Items.Clear();
            processing.sectordata2.Clear();

            for (i = 0; i < processing.mfmsindex; i++)
            {

                //BadSectors[i] = new byte[0];
                processing.mfms[i] = null;
                processing.mfms[i] = new byte[0];
            }
            OnlyBadSectorsRadio.Checked = false; // When the input buffer is changed or empty, we can't scan for only bad sectors
            ECOnRadio.Checked = true;
            StringBuilder t = new StringBuilder();
            //mfmlength = 0;
            processing.rxbuf = null;
            processing.rxbuf = new byte[200000];
            //Array.Clear(processing.rxbuf, 0, processing.rxbuf.Length);
            //TrackPosInrxdatacount = 0;
            processing.indexrxbuf = 0;
            processing.mfmsindex = 0;

            rxbufStartUpDown.Maximum = processing.indexrxbuf;
            rxbufEndUpDown.Maximum = processing.indexrxbuf;
            rxbufEndUpDown.Value = processing.indexrxbuf;
            updateHistoAndSliders();
            scatterplot.AnScatViewlength = 100000;
            scatterplot.AnScatViewoffset = 0;
            scatterplot.AnScatViewlargeoffset = 0;
            scatterplot.AnScatViewoffsetOld = 0;
            scatterplot.UpdateScatterPlot();
            GC.Collect();
        }

        private void resetoutput()
        {
            HandleTabSwitching();
            var oldscrollvalue = HistogramhScrollBar1.Value;
            var oldscrollmaxvalue = HistogramhScrollBar1.Maximum;
            var qoldscrollvalue = QHistogramhScrollBar1.Value;
            var qoldscrollmaxvalue = QHistogramhScrollBar1.Maximum;
            var rxbuftemp = (byte[]) processing.rxbuf.Clone();
            for(var i =0; i< processing.mfms.Length;i++)
                processing.mfms[i] = null;
            processing.rxbuf = null;
            processing.disk = null;
            processing.mfmlengths = null;
            processing.badsectorhash = null;
            processing.progresses = null;
            processing.progressesstart = null;
            processing.progressesend = null;
            processing.ProcessStatus = null;
            processing.sectormap.sectorok = null;
            processing.sectormap.sectorokLatestScan = null;
            processing.sectormap = null;
            processing = null;

            

            ECHisto = new Histogram();
            ScatterHisto = new Histogram();
            processing = new FDDProcessing();
            processing.rxbuf = rxbuftemp;
            processing.indexrxbuf = processing.rxbuf.Length/2; // Divide by two as loading .bin files doubles the buffer
            processing.GetProcSettingsCallback += GetProcSettingsCallback;
            processing.rtbSectorMap = rtbSectorMap;
            processing.tbreceived = tbreceived;
            processing.sectormap.SectorMapUpdateGUICallback += SectorMapUpdateGUICallback;
            processing.sectormap.rtbSectorMap = rtbSectorMap;

            fileio.processing = processing;
            if (controlfloppy.serialPort1.IsOpen)
                controlfloppy.Disconnect();
            controlfloppy.Disconnect();
            controlfloppy.tempbuffer.Clear();
            controlfloppy.rxbuf = null;
            controlfloppy = null;
            controlfloppy = new ControlFloppy();
            controlfloppy.rxbuf = rxbuftemp;
            controlfloppy.processing = processing;
            scatterplot.removeEvents();
            scatterplot.rxbuf = null;
            scatterplot.UpdateEvent -= updateAnScatterPlot;
            scatterplot.ShowGraph -= ScatterPlotShowGraphCallback;
            
            scatterplot = null;
            scatterplot = new ScatterPlot(processing, processing.sectordata2, 0, 0, ScatterPictureBox);
            scatterplot.tbreiceved = tbreceived;
            scatterplot.rxbuf = rxbuftemp;
            scatterplot.UpdateEvent += updateAnScatterPlot;
            scatterplot.ShowGraph += ScatterPlotShowGraphCallback;
            scatterplot.EditScatterplot = EditScatterPlotcheckBox.Checked;
            
            EditOptioncomboBox.SelectedIndex = 0;
            EditModecomboBox.SelectedIndex = 0;
            textBoxReceived.AppendText("PortName: " + selectedPortName + "\r\n");
            
            BadSectorTooltip.Hide();
            timer5.Start();
            GUITimer.Start();
            BluetoRedByteCopyToolBtn.Tag = new int();
            BluetoRedByteCopyToolBtn.Tag = 0;
            ECHisto.setPanel(AnHistogramPanel);
            ScatterHisto.setPanel(Histogrampanel1);
            if (processing.indexrxbuf < 100000)
                scatterplot.AnScatViewlength = processing.indexrxbuf;
            else scatterplot.AnScatViewlength = 99999;
            scatterplot.AnScatViewoffset = 0;
            scatterplot.UpdateScatterPlot();
            updateSliderLabels();
            updateAnScatterPlot();
            
            HistogramhScrollBar1.Maximum = oldscrollmaxvalue;
            HistogramhScrollBar1.Value = oldscrollvalue;
            QHistogramhScrollBar1.Maximum = qoldscrollmaxvalue;
            QHistogramhScrollBar1.Value = qoldscrollvalue;
            HandleTabSwitching();
            GC.Collect();
        }

        private void ResetInputBtn_Click(object sender, EventArgs e)
        {
            resetinput();
        }

        private void ResetOutputBtn_Click(object sender, EventArgs e)
        {
            resetoutput();
            rtbSectorMap.DeselectAll();
            Application.DoEvents();
            processing.sectormap.RefreshSectorMap();
                     
        }

        private void TrackPreset2Button_Click(object sender, EventArgs e)
        {
            StartTrackUpDown.Value = 80;
            EndTracksUpDown.Value = 90;
            TrackDurationUpDown.Value = 540;
        }

        private void updateHistoAndSliders()
        {
            if (!LimitTSCheckBox.Checked)
            {
                //createhistogram();
                updateSliderLabels();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TrackDurationUpDown.Value = 5000;
        }

        private void outputfilename_Enter(object sender, EventArgs e)
        {
            disablecatchkey = 1;
        }

        private void outputfilename_Leave(object sender, EventArgs e)
        {
            //tbreceived.Append("Output changed to: "+outputfilename.Text+"\r\n");
            disablecatchkey = 0;
            openFileDialog1.InitialDirectory = subpath + @"\" + outputfilename.Text;
            openFileDialog2.InitialDirectory = subpath + @"\" + outputfilename.Text;
            Properties.Settings.Default["BaseFileName"] = outputfilename.Text;
            Properties.Settings.Default.Save();
        }

        private void FloppyControl_Click(object sender, EventArgs e)
        {
            ScanButton.Focus();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (processing.stop == 1)
                processing.stop = 0;
            else
                processing.stop = 1;

            controlfloppy.StopCapture();
            DisconnectFromFloppyControlHardware();
            //indexrxbuf = processing.indexrxbuf;
            rxbufStartUpDown.Maximum = processing.indexrxbuf;
            rxbufEndUpDown.Maximum = processing.indexrxbuf;
            rxbufEndUpDown.Value = processing.indexrxbuf;

            tbreceived.Append("Stopping...\r\n");
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("FloppyControlApp v"+version.ToString()+" is created by\nJosha Beukema.\nCode snippets used from stack overflow and other places.\nAufit DPLL class Copyright (C) 2013-2015 Jean Louis-Guerin. ", "About");
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm SettingsForm1 = new SettingsForm(); // Create instance of settings form
            SettingsForm1.Show();
        }

        private void HDCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (HDCheckBox.Checked)
            {
                ScatterHisto.hd = 1;
                processing.procsettings.hd = 1;
                //hddiv = 2;
            }
            else
            {
                ScatterHisto.hd = 0;
                processing.procsettings.hd = 0;
                //hddiv = 1;
            }
            if (processing.indexrxbuf > 1)
                ScatterHisto.DoHistogram(processing.rxbuf, (int)rxbufStartUpDown.Value, (int)rxbufEndUpDown.Value);
        }

        private void FloppyControl_Resize(object sender, EventArgs e)
        {
            wlabel.Text = this.Width.ToString();
            hlabel.Text = this.Height.ToString();
            /*
            if (this.Width < 1600)
            {
                label6.Hide();
                textBoxReceived.Hide();
            }
            if (this.Width >= 1600)
            {
                label6.Show();
                textBoxReceived.Show();
            }
            */
        }

        private void OffsetScan()
        {
            int i;
            //NormalradioButton.Checked = true;
            ProcessingModeComboBox.SelectedItem = ProcessingType.normal.ToString();
            capturetime = 0;
            processing.processing = 1;
            processing.stop = 0;
            for (i = -15; i < 19; i += 3)
            {
                SettingsLabel.Text = "i = " + i;
                if (processing.stop == 1)
                    break;
                OffsetvScrollBar1.Value = i;
                ScanButton.PerformClick();

                //RefreshSectorMap();
                //this.updateForm();
            }
            processing.stop = 0;
            processing.processing = 0;
        }

        private void OffsetScan2()
        {
            int i;
            ProcessingModeComboBox.SelectedItem = ProcessingType.normal.ToString();
            //NormalradioButton.Checked = true;
            capturetime = 0;
            processing.processing = 1;
            processing.stop = 0;

            for (i = 0; i < 28; i += 1)
            {
                SettingsLabel.Text = "i = " + i;
                if (processing.stop == 1)
                    break;
                OffsetvScrollBar1.Value = ((28 - i) * -1) + 3;
                ScanButton.PerformClick();

                OffsetvScrollBar1.Value = 29 - i;
                ScanButton.PerformClick();

                this.updateForm();
            }

            processing.stop = 0;
            processing.processing = 0;
        }

        

        // Display sector data, only works for PC for now
        private void TrackUpDown2_ValueChanged(object sender, EventArgs e)
        {
            ShowDiskSector();
        }

        private void ShowDiskSector()
        {
            int i, track, sector, offset;
            byte databyte;
            StringBuilder bytesstring = new StringBuilder();
            StringBuilder txtstring = new StringBuilder();

            track = (int)TrackUpDown.Value;
            sector = (int)SectorUpDown.Value;
            offset = (track * 512 * processing.sectorspertrack) + (512 * sector);

            //txtstring.Append();

            for (i = 0; i < 512; i++)
            {
                databyte = (byte)processing.disk[track * 512 * processing.sectorspertrack + (512 * sector) + i];
                bytesstring.Append(databyte.ToString("X2"));
                if (databyte > 32 && databyte < 127)
                    txtstring.Append((char)databyte);
                else txtstring.Append(".");
                if (i % 32 == 31)
                {
                    txtstring.Append("\r\n");
                    bytesstring.Append("\r\n");
                }
            }
            textBoxSector.Text = txtstring.ToString() + "\r\n\r\n";
            textBoxSector.Text += bytesstring.ToString() + "\r\n";
        }

        private void SavePrjBtn_Click(object sender, EventArgs e)
        {
            fileio.SaveProject();
        }

        private void LoadPrjBtn_Click(object sender, EventArgs e)
        {
            fileio.ShowLoadProjectFiles();
        }

        private void ExtremeScan()
        {
            int i, j, k;

            //OffsetvScrollBar1.Value = 0;
            //MinvScrollBar.Value = 0x04;
            //EightvScrollBar.Value = 0xFE;
            int kmax = (int)AddNoiseKnumericUpDown.Value;
            int hd = HDCheckBox.Checked ? 1 : 0;
            int gc_cnt = 0;
            int step = 1 << hd;
            int _4us = FourvScrollBar.Value;
            int _6us = SixvScrollBar.Value;
            processing.stop = 0;
            ProcessingModeComboBox.SelectedItem = ProcessingType.normal.ToString();
            //NormalradioButton.Checked = true;

            uint ESTime;
            ESTime = (uint)(Environment.TickCount + Int32.MaxValue);

            for (i = (int)iESStart.Value; i < (int)iESEnd.Value; i += step)
            {
                //GC.Collect(); // Allow the system to recover some memory
                for (j = (int)jESStart.Value; j < (int)jESEnd.Value; j += step)
                {
                    if (processing.stop == 1)
                        break;

                    FourvScrollBar.Value = _4us + j;
                    SixvScrollBar.Value = _6us + i;

                    if (MinvScrollBar.Value < 4)
                        MinvScrollBar.Value = 4;
                    Application.DoEvents();
                    updateHistoAndSliders();
                    for (k = 0; k < kmax; k++)
                    {
                        SettingsLabel.Text = "i = " + i + " j = " + j + " k = " + k;
                        gc_cnt++;
                        if (gc_cnt % 50 == 0)
                            GC.Collect();
                        if (processing.stop == 1)
                            break;
                        if (processing.diskformat == DiskFormat.amigados || processing.diskformat == DiskFormat.diskspare)
                            processing.StartProcessing(1);
                        else
                            processing.StartProcessing(0);
                        this.updateForm();
                    }
                }
                if (processing.stop == 1)
                    break;
            }
            processing.stop = 0;
            FourvScrollBar.Value = _4us;
            SixvScrollBar.Value = _6us;
            //ESTime = (uint)(Environment.TickCount + Int32.MaxValue);
            tbreceived.Append("Time: " + (uint)(Environment.TickCount + Int32.MaxValue - ESTime) + "ms\r\n");
        }

        private void ECSectorOverlayBtn_Click(object sender, EventArgs e)
        {
            ECSectorOverlay();
        }

        private void BadSectorListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateECInterface();
        }
        private void updateECInterface()
        {
            ScatterMinUpDown.Value = ScatterMinTrackBar.Value + ScatterOffsetTrackBar.Value;
            ScatterMaxUpDown.Value = ScatterMaxTrackBar.Value + ScatterOffsetTrackBar.Value;
            ScatterOffsetUpDown.Value = ScatterOffsetTrackBar.Value;

            SelectionDifLabel.Text = "Periods:" + (ScatterMaxUpDown.Value - ScatterMinUpDown.Value).ToString();
            BadSectorDraw();
            BadSectorToolTip();
            var currentcontrol = FindFocusedControl(this);
            tabControl1.SelectedTab = ScatterPlottabPage;
            currentcontrol.Focus();
            ShowSectorData();
        }
        private void ShowSectorData()
        {
            int indexS1, threadid;
            int i;
            if (BadSectorListBox.SelectedIndices.Count >= 1)
            {
                indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;

            }
            else return;

            antbSectorData.Clear();
            antbSectorData.Text = (processing.BytesToHexa(processing.sectordata2[indexS1].sectorbytes, 0, processing.sectordata2[indexS1].sectorbytes.Length));

            int mfmoffset = processing.sectordata2[indexS1].MarkerPositions;
            int length = (processing.sectordata2[indexS1].sectorlength + 1000) * 16;
            //threadid = sectordata[threadid][indexS1].threadid;
            StringBuilder mfmtxt = new StringBuilder();
            for (i = 0; i < length; i++)
            {
                mfmtxt.Append((char)(processing.mfms[threadid][i + mfmoffset] + 48));
            }
            ECtbMFM.Text = mfmtxt.ToString();
        }

        private void BadSectorPanel_MouseDown(object sender, MouseEventArgs e)
        {
            BadSectorPanelClick();
        }

        private void CopySectorToBlueBtn_Click(object sender, EventArgs e)
        {
            CopySectorToBlue();
        }
      
        private void BadSectorPanel_MouseHover(object sender, EventArgs e)
        {
            BadSectorToolTip();
        }

        private void BadSectorPanel_MouseMove(object sender, MouseEventArgs e)
        {
            BadSectorToolTip();
        }
 
        private void BadSectorPictureBox_Paint(object sender, PaintEventArgs e)
        {
            BadSectorDraw();
        }

        private void BadSectorDraw()
        {
            if (ECMFMcheckBox.Checked)
                BadMFMSectorDraw();
            else
                BadSectorByteDraw();
        }

        private void BadSectorPanel_MouseLeave(object sender, EventArgs e)
        {
            BadSectorTooltip.Hide();
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            BadSectorTooltip.Location = BadSectorTooltipPos;
        }

        private void BluetoRedByteCopyToolBtn_Click(object sender, EventArgs e)
        {
            if ((int)(BluetoRedByteCopyToolBtn.Tag) == 0)
            {
                BluetoRedByteCopyToolBtn.Tag = 1; // Button is active
                BluetoRedByteCopyToolBtn.BackColor = Color.FromArgb(255, 255, 208, 192);
            }
            else
            {
                BluetoRedByteCopyToolBtn.Tag = 0; // Button is active
                BluetoRedByteCopyToolBtn.BackColor = SystemColors.Control;
            }
        }

        private void BSBlueFromListRadio_CheckedChanged(object sender, EventArgs e)
        {
            BadSectorDraw();
        }

        private void BSBlueSectormapRadio_CheckedChanged(object sender, EventArgs e)
        {
            BadSectorDraw();
        }

        private void BlueTempRadio_CheckedChanged(object sender, EventArgs e)
        {
            BadSectorDraw();
        }

        private void Track1UpDown_ValueChanged(object sender, EventArgs e)
        {
            BadSectorDraw();
        }

        private void Sector1UpDown_ValueChanged(object sender, EventArgs e)
        {
            BadSectorDraw();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void GUITimer_Tick(object sender, EventArgs e)
        {
            ProcessStatusLabel.Text = processing.ProcessStatus[processing.mfmsindex];
            progressBar1.Minimum = processing.progressesstart[processing.mfmsindex];
            progressBar1.Maximum = processing.progressesend[processing.mfmsindex];

            if (processing.progresses[processing.mfmsindex] >= processing.progressesstart[processing.mfmsindex] &&
                processing.progresses[processing.mfmsindex] <= processing.progressesend[processing.mfmsindex])
                if (processing.progresses[processing.mfmsindex] <= progressBar1.Maximum &&
                    processing.progresses[processing.mfmsindex] >= progressBar1.Minimum)
                    progressBar1.Value = processing.progresses[processing.mfmsindex];

            textBoxReceived.AppendText(tbreceived.ToString());
            tbreceived.Clear();
            this.updateForm();
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleTabSwitching();
        }

        private void HandleTabSwitching()
        {
            if (MainTabControl.SelectedTab == ErrorCorrectionTab)
            {
                //MainTabControl.TabPages[1].Controls.Remove(ThresholdsGroupBox);
                ErrorCorrectionTab.Controls.Add(ThresholdsGroupBox);
                ThresholdsGroupBox.Location = new Point(459, 290);
            }
            if (MainTabControl.SelectedTab == ProcessingTab)
            {
                groupBox6.Controls.Add(ThresholdsGroupBox);
                ThresholdsGroupBox.Location = new Point(600, 16);
                processing.rtbSectorMap = rtbSectorMap;
                processing.sectormap.rtbSectorMap = rtbSectorMap;
                processing.sectormap.RefreshSectorMap();
                ScatterHisto.setPanel(Histogrampanel1);
                updateAnScatterPlot();
            }
            if (MainTabControl.SelectedTab == QuickTab)
            {
                QHistogramhScrollBar1.Minimum = HistogramhScrollBar1.Minimum;
                QHistogramhScrollBar1.Maximum = HistogramhScrollBar1.Maximum;
                QHistogramhScrollBar1.Value = HistogramhScrollBar1.Value;
                processing.rtbSectorMap = QrtbSectorMap;
                processing.sectormap.rtbSectorMap = QrtbSectorMap;
                processing.sectormap.RefreshSectorMap();
                //ScatterHisto.setPanel(QHistoPanel);
                updateAnScatterPlot();
            }
        }

        private void ScatterMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            //ScatterMinTrackBar.Value = (int)ScatterMinUpDown.Value;
            //ScatterMaxTrackBar.Value = (int)ScatterMaxUpDown.Value;
        }

        private void ThreadsUpDown_ValueChanged(object sender, EventArgs e)
        {
            processing.NumberOfThreads = (int)ThreadsUpDown.Value;
        }

        private void BadSectorListBox_KeyDown(object sender, KeyEventArgs e)
        {
            //tbreceived.Append("KeyCode: "+(int)e.KeyCode+"\r\n");
            if (e.KeyCode == Keys.Delete) //Delete key
            {
                var selectedItems = BadSectorListBox.SelectedItems;
                var qq = selectedItems[0];
                if (BadSectorListBox.SelectedIndex != -1)
                {
                    for (int i = selectedItems.Count - 1; i >= 0; i--)
                    {
                        var badsectoritem = (badsectorkeyval)selectedItems[i];

                        for (int j = 0; j < JumpTocomboBox.Items.Count; j++)
                        {
                            var jumpboxitem = (ComboboxItem)JumpTocomboBox.Items[j];

                            if (jumpboxitem.id == badsectoritem.id)
                                JumpTocomboBox.Items.RemoveAt(j);
                        }

                        BadSectorListBox.Items.Remove(selectedItems[i]);
                    }
                }
            }
        }

        private void ECZoomOutBtn_Click(object sender, EventArgs e)
        {
            int indexS1, threadid;

            if (BadSectorListBox.SelectedIndices.Count >= 1)
            {
                indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;

                int sectorlength = processing.sectordata2[indexS1].sectorlength;

                int factor = sectorlength / 512;

                ScatterMinTrackBar.Value = 0;
                ScatterMaxTrackBar.Value = 4500 * factor;
                updateECInterface();
            }
        }

        private void ECRealign4E_Click(object sender, EventArgs e)
        {
            int indexS1, listlength = 0, i, threadid;
            var selected = BadSectorListBox.SelectedIndices;
            listlength = selected.Count;

            ECSettings ecSettings = new ECSettings();
            ECResult sectorresult;
            ecSettings.sectortextbox = textBoxSector;

            if (ScatterMaxTrackBar.Value - ScatterMinTrackBar.Value > 50)
            {
                tbreceived.Append("Error: selection can't be larger than 50!\r\n");
                return;
            }
            
            if (listlength >= 1)
            {
                for (i = 0; i < listlength; i++)
                {
                    if (processing.stop == 1)
                        break;
                    indexS1 = ((badsectorkeyval)BadSectorListBox.Items[selected[i]]).id;
                    threadid = ((badsectorkeyval)BadSectorListBox.Items[selected[i]]).threadid;
                    ecSettings.indexS1 = indexS1;
                    ecSettings.periodSelectionStart = (int)ScatterMinUpDown.Value;
                    ecSettings.periodSelectionEnd = (int)ScatterMaxUpDown.Value;
                    ecSettings.threadid = threadid;
                    if ((int)processing.diskformat > 2)
                    {
                        sectorresult = processing.ProcessRealign4E(ecSettings);
                        if (sectorresult != null)
                        {
                            AddRealignedToLists(sectorresult);
                        }
                    }
                    else
                    {
                        sectorresult = processing.ProcessRealignAmiga(ecSettings);
                        if (sectorresult != null)
                        {
                            AddRealignedToLists(sectorresult);
                        }
                    }
                }
            }
            else
            {
                textBoxReceived.AppendText("Error, no data selected.");
                return;
            }
        }

        private void AddRealignedToLists(ECResult sectorresult)
        {
            MFMData sectordata = sectorresult.sectordata;
            int badsectorcnt2 = sectorresult.index;
            int track = sectordata.track;
            int sector = sectordata.sector;

            var currentcontrol = FindFocusedControl(this);
            tabControl1.SelectedTab = ShowSectorTab;
            currentcontrol.Focus();

            string key = "Aligned: T" + track + " s" + sector;
            int index = BadSectorListBox.Items.Add(new badsectorkeyval
            {
                name = "i: " + badsectorcnt2 + " " + key,
                id = badsectorcnt2,
                threadid = sectordata.threadid
            });
            //JumpTocomboBox.Items.Add()
            int index2 = JumpTocomboBox.Items.Add(new ComboboxItem
            {
                Text = "i: " + badsectorcnt2 + " " + key,
                id = badsectorcnt2,
            });
        }

        public static Control FindFocusedControl(Control control)
        {
            var container = control as IContainerControl;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as IContainerControl;
            }
            return control;
        }

        

        private void AuScan()
        {
            int i;
            processing.stop = 0;
            //AufitRadioButton.Checked = true;
            ProcessingModeComboBox.SelectedItem = ProcessingType.aufit.ToString();
            scanactive = true;
            for (i = 0x2E; i < 0x3A; i += 2)
            {
                SettingsLabel.Text = "1. i = " + i;
                if (processing.stop == 1)
                    break;
                MinvScrollBar.Value = i;
                if ((int)processing.diskformat <= 2)
                    processing.StartProcessing(1);
                else
                    processing.StartProcessing(0);
                processing.sectormap.RefreshSectorMap();
                this.updateForm();
            }
            MinvScrollBar.Value = 0x32;
            for (i = 0x07; i < 0x18; i += 2)
            {
                SettingsLabel.Text = "2. i = " + i;
                if (processing.stop == 1)
                    break;
                FourvScrollBar.Value = i;
                if ((int)processing.diskformat <= 2)
                    processing.StartProcessing(1);
                else
                    processing.StartProcessing(0);
                processing.sectormap.RefreshSectorMap();
                this.updateForm();
            }

            scanactive = false;
            processing.stop = 0;
        }

        private void outputfilename_TextChanged(object sender, EventArgs e)
        {
            //tbreceived.Append("Output changed to: " + outputfilename.Text + "\r\n");
            openFileDialog1.InitialDirectory = subpath + @"\" + outputfilename.Text;
            openFileDialog2.InitialDirectory = subpath + @"\" + outputfilename.Text;
            if(fileio != null)
                fileio.BaseFileName = outputfilename.Text;
            Properties.Settings.Default["BaseFileName"] = outputfilename.Text;
            Properties.Settings.Default.Save();
        }

        private void CreateGraphs()
        {
            
            Graph1SelRadioButton.Checked = true;
            

        }

        private void GraphOffsetTrackBar_Scroll(object sender, EventArgs e)
        {
            int index = 0;
            if (Graph1SelRadioButton.Checked) index = 0;
            else
            if (Graph2SelRadioButton.Checked) index = 1;
            else
            if (Graph3SelRadioButton.Checked) index = 2;
            else
            if (Graph4SelRadioButton.Checked) index = 3;
            else
            if (Graph5SelRadioButton.Checked) index = 4;
            oscilloscope.graphset.Graphs[index].changed = true;

            oscilloscope.graphset.Graphs[graphselect].yoffset = GraphOffsetTrackBar.Value;
            oscilloscope.graphset.UpdateGraphs();
            GraphYOffsetlabel.Text = GraphOffsetTrackBar.Value.ToString();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            int index = 0;
            if (Graph1SelRadioButton.Checked) index = 0;
            else
            if (Graph2SelRadioButton.Checked) index = 1;
            else
            if (Graph3SelRadioButton.Checked) index = 2;
            else
            if (Graph4SelRadioButton.Checked) index = 3;
            else
            if (Graph5SelRadioButton.Checked) index = 4;

            oscilloscope.graphset.Graphs[index].changed = true;

            oscilloscope.graphset.Graphs[graphselect].yscale = (GraphYScaleTrackBar.Value / 100.0f);
            oscilloscope.graphset.UpdateGraphs();
            GraphScaleYLabel.Text = (GraphYScaleTrackBar.Value / 100.0f).ToString();
        }

        private void Graph1SelRadioButton_CheckedChanged(object sender, EventArgs e)
        {

            if (oscilloscope.graphset.Graphs.Count >= 1)
            {
                graphselect = 0;
                GraphOffsetTrackBar.Value = oscilloscope.graphset.Graphs[graphselect].yoffset;
                GraphYScaleTrackBar.Value = (int)(oscilloscope.graphset.Graphs[graphselect].yscale * 100);
                oscilloscope.graphset.UpdateGraphs();
            }
        }

        private void Graph2SelRadioButton_CheckedChanged(object sender, EventArgs e)
        {

            if (oscilloscope.graphset.Graphs.Count >= 2)
            {

                graphselect = 1;
                GraphOffsetTrackBar.Value = oscilloscope.graphset.Graphs[graphselect].yoffset;
                GraphYScaleTrackBar.Value = (int)(oscilloscope.graphset.Graphs[graphselect].yscale * 100);
                oscilloscope.graphset.UpdateGraphs();
            }
        }

        private void Graph3SelRadioButton_CheckedChanged(object sender, EventArgs e)
        {


            if (oscilloscope.graphset.Graphs.Count >= 3)
            {
                graphselect = 2;
                GraphOffsetTrackBar.Value = oscilloscope.graphset.Graphs[graphselect].yoffset;
                GraphYScaleTrackBar.Value = (int)(oscilloscope.graphset.Graphs[graphselect].yscale * 100);
                oscilloscope.graphset.UpdateGraphs();
            }
        }

        private void Graph4SelRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oscilloscope.graphset.Graphs.Count >= 4)
            {
                graphselect = 3;
                GraphOffsetTrackBar.Value = oscilloscope.graphset.Graphs[graphselect].yoffset;
                GraphYScaleTrackBar.Value = (int)(oscilloscope.graphset.Graphs[graphselect].yscale * 100);
                oscilloscope.graphset.UpdateGraphs();
            }
        }

        private void Graph5SelRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oscilloscope.graphset.Graphs.Count >= 5)
            {
                graphselect = 4;
                GraphOffsetTrackBar.Value = oscilloscope.graphset.Graphs[graphselect].yoffset;
                GraphYScaleTrackBar.Value = (int)(oscilloscope.graphset.Graphs[graphselect].yscale * 100);
                oscilloscope.graphset.UpdateGraphs();
            }
        }

        private void GraphsetGetControlValuesCallback()
        {
            oscilloscope.graphset.editmode = EditModecomboBox.SelectedIndex;
            oscilloscope.graphset.editoption = EditOptioncomboBox.SelectedIndex;
            oscilloscope.graphset.editperiodextend = (int)PeriodExtendUpDown.Value;
        }

        private void updateGraphCallback()
        {
            if (stopupdatingGraph == false)
            {
                AnDensityUpDown.Value = oscilloscope.graphset.Graphs[0].density;
                int index = 0;
                if (Graph1SelRadioButton.Checked) index = 0;
                else
                if (Graph2SelRadioButton.Checked) index = 1;
                else
                if (Graph3SelRadioButton.Checked) index = 2;
                else
                if (Graph4SelRadioButton.Checked) index = 3;
                else
                if (Graph5SelRadioButton.Checked) index = 4;

                oscilloscope.graphset.Graphs[index].changed = true;

                oscilloscope.graphset.Graphs[graphselect].yscale = (GraphYScaleTrackBar.Value / 100.0f);
                GraphScaleYLabel.Text = (GraphYScaleTrackBar.Value / 100.0f).ToString();
                /*
                foreach ( var gr in graphset.Graphs)
                {
                    gr.density = density;
                }
                AnDensityUpDown.Value = density;
                */
                GraphLengthLabel.Text = string.Format("{0:n0}", oscilloscope.graphset.Graphs[0].datalength);
                GraphXOffsetLabel.Text = string.Format("{0:n0}", oscilloscope.graphset.Graphs[0].dataoffset);
                int i;
                int centerposition = oscilloscope.graphset.Graphs[0].dataoffset;
                //int centerposition = graphset.Graphs[0].dataoffset + (graphset.Graphs[0].datalength / 2);
                if (processing.sectordata2 != null && processing.rxbuftograph != null)
                {
                    for (i = 0; i < processing.rxbuftograph.Length; i++)
                    {
                        if (processing.rxbuftograph[i] > centerposition) 
                            break;

                    }
                    tbreceived.Append("rxbuftograph i "+i+"\r\n");
                    if (i < processing.rxbuftograph.Length - 1)
                    {
                        int rxbufoffset = i;

                        for (i = 0; i < processing.sectordata2.Count; i++)
                        {
                            if (processing.sectordata2[i].rxbufMarkerPositions > rxbufoffset)
                            {
                                break;
                            }
                        }
                        tbreceived.Append("sectordata i " + i + "\r\n");
                        if (i > 1)
                        {
                            MFMData sectordata = processing.sectordata2[i - 1];
                            int sectoroffset = rxbufoffset - sectordata.rxbufMarkerPositions;


                            rxbufOffsetLabel.Text = "T"+sectordata.track.ToString("D3")+" S"+sectordata.sector+" o:"+sectoroffset.ToString();
                        }
                    }
                }
                Undolevelslabel.Text = "Undo levels: " + (oscilloscope.graphset.Graphs[0].undo.Count).ToString();
                //graphset.UpdateGraphs();
                scatterplot.UpdateScatterPlot();

            }
        }

        private void updateAnScatterPlot()
        {
            scatterplot.thresholdmin = MinvScrollBar.Value + OffsetvScrollBar1.Value;
            scatterplot.threshold4us = FourvScrollBar.Value + OffsetvScrollBar1.Value;
            scatterplot.threshold6us = SixvScrollBar.Value + OffsetvScrollBar1.Value;
            scatterplot.thresholdmax = EightvScrollBar.Value + OffsetvScrollBar1.Value;

            HistogramhScrollBar1.Maximum = processing.indexrxbuf;
            QHistogramhScrollBar1.Maximum = processing.indexrxbuf;
            if (scatterplot.AnScatViewoffset + scatterplot.AnScatViewlargeoffset < 0)
            {
                scatterplot.AnScatViewoffset = 0;
                scatterplot.AnScatViewlargeoffset = 0;
            }

            if (processing.indexrxbuf > 0)
            {
                if (MainTabControl.SelectedTab == ProcessingTab)
                {
                    int offset = scatterplot.AnScatViewoffset + scatterplot.AnScatViewlargeoffset;
                    int length = scatterplot.AnScatViewlength;
                    if (length < 0) length = 4000;
                    if (scatterplot.AnScatViewlargeoffset < processing.indexrxbuf)
                        HistogramhScrollBar1.Value = scatterplot.AnScatViewlargeoffset;
                    ScatterHisto.setPanel(Histogrampanel1);
                    ScatterHisto.DoHistogram(processing.rxbuf, offset, length);
                }
                if (MainTabControl.SelectedTab == QuickTab)
                {
                    int offset = scatterplot.AnScatViewoffset + scatterplot.AnScatViewlargeoffset;
                    int length = scatterplot.AnScatViewlength;
                    if (length < 0) length = 4000;
                    if (scatterplot.AnScatViewlargeoffset < processing.indexrxbuf)
                        QHistogramhScrollBar1.Value = scatterplot.AnScatViewlargeoffset;
                    ScatterHisto.setPanel(QHistoPanel);
                    ScatterHisto.DoHistogram(processing.rxbuf, offset, length);
                }
            }
        }

        private void OpenWavefrmbutton_Click_1(object sender, EventArgs e)
        {
            oscilloscope.OpenOscilloscopeFile();
            UpdateHistoAndScatterplot();            
        }
        public void GraphFilterButton_Click(object sender, EventArgs e)
        {
            oscilloscope.Filter(
                (int)DiffDistUpDown.Value,
                (float)DiffGainUpDown.Value,
                (int)DiffDistUpDown2.Value,
                (int)DiffThresholdUpDown.Value,
                (int)SmoothingUpDown.Value,
                (int)DiffMinDeviationUpDown.Value,
                (int)DiffMinDeviation2UpDown.Value,
                (int)AdaptLookAheadUpDown.Value,
                AdaptiveGaincheckBox.Checked,
                InvertcheckBox.Checked,
                (double)SignalRatioDistUpDown.Value,
                AnReplacerxbufBox.Checked);
            UpdateHistoAndScatterplot();
        }

        private void button31_Click_2(object sender, EventArgs e)
        {
            oscilloscope.Filter2(
                (int)DiffDistUpDown.Value,
                (float)DiffGainUpDown.Value,
                (int)DiffDistUpDown2.Value,
                (int)DiffThresholdUpDown.Value,
                (int)SmoothingUpDown.Value,
                (int)DiffMinDeviationUpDown.Value,
                (int)DiffMinDeviation2UpDown.Value,
                (int)AdaptLookAheadUpDown.Value,
                AdaptiveGaincheckBox.Checked,
                InvertcheckBox.Checked,
                (double)SignalRatioDistUpDown.Value,
                AnReplacerxbufBox.Checked
            );
        }
        
        private void DiffDistUpDown_ValueChanged(object sender, EventArgs e)
        {
            GraphFilterButton.PerformClick();
        }

        private void DiffGainUpDown_ValueChanged(object sender, EventArgs e)
        {
            GraphFilterButton.PerformClick();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            GraphFilterButton.PerformClick();
        }

        private void DiffDistUpDown2_ValueChanged(object sender, EventArgs e)
        {
            GraphFilterButton.PerformClick();
        }

        // Process the read data signal captured using the scope
        private void button19_Click(object sender, EventArgs e)
        {
            int i;
            //double val = 0;
            //double val2 = 0;
            //double RateOfChange = (float)GraphFilterUpDown.Value;
            int diffdist = (int)DiffDistUpDown.Value;
            float diffgain = (float)DiffGainUpDown.Value;
            int diffdist2 = (int)DiffDistUpDown2.Value;
            if (graphwaveform[2] == null) return;
            int length = graphwaveform[2].Length;

            int diffthreshold = (int)DiffThresholdUpDown.Value;

            // The captured data has the high and low at 119 and 108
            float[] t = new float[graphwaveform[0].Length];
            //indexrxbuf = 0;
            processing.indexrxbuf = 0;
            int j;
            int old = 0;
            int period;
            //Smoothing pass
            for (i = 0; i < length; i++)
            {
                if (graphwaveform[2][i] < 113)
                {
                    period = i - old;
                    processing.rxbuf[processing.indexrxbuf++] = (byte)period;
                    old = i;
                    for (j = 0; j < 100; j++) // skip to end of pulse
                    {
                        if (graphwaveform[2][i] > 113)
                        {
                            break;
                        }
                        if (i < length - 1)
                            i++;
                        else break;
                    }
                }

            }

            rxbufEndUpDown.Maximum = processing.indexrxbuf;
            rxbufStartUpDown.Maximum = processing.indexrxbuf;

            rxbufEndUpDown.Value = processing.indexrxbuf;
            HistogramhScrollBar1.Minimum = 0;
            HistogramhScrollBar1.Maximum = processing.indexrxbuf;
            oscilloscope.graphset.SetAllChanged();

            oscilloscope.graphset.UpdateGraphs();
        }
        
        
        private void CaptureDataBtn_Click(object sender, EventArgs e)
        {
            int i;

            scope.tbr = tbreceived;

            string connection = (string)Properties.Settings.Default["ScopeConnection"];
            scope.Connect(connection);
            for (i = 0; i < 20; i++)
            {
                Thread.Sleep(50);
                if (scope.connectionStatus == 1)
                    break;
            }
            if (i == 19)
            {
                tbreceived.Append("Connection failed\r\n");
            }
            else
            {
                controlfloppy.binfilecount = binfilecount;
                controlfloppy.DirectStep = DirectStepCheckBox.Checked;
                controlfloppy.MicrostepsPerTrack = (int)MicrostepsPerTrackUpDown.Value;
                controlfloppy.trk00offset = (int)TRK00OffsetUpDown.Value;
                controlfloppy.EndTrack = (int)EndTracksUpDown.Value;
                controlfloppy.StartTrack = (int)StartTrackUpDown.Value;
                controlfloppy.tbr = tbreceived;
                //processing.indexrxbuf            = indexrxbuf;
                
                controlfloppy.outputfilename = outputfilename.Text;
                controlfloppy.rxbuf = processing.rxbuf;

                selectedBaudRate = (int)Properties.Settings.Default["DefaultBaud"];
                selectedPortName = (string)Properties.Settings.Default["DefaultPort"];
                scope.serialPort1.PortName = selectedPortName;
                scope.serialPort1.BaudRate = selectedBaudRate;
                scope.ScopeMemDepth = (int)NumberOfPointsUpDown.Value;
                scope.UseAveraging = NetworkUseAveragingCheckBox.Checked;

                controlfloppy.binfilecount = binfilecount;
                controlfloppy.DirectStep = DirectStepCheckBox.Checked;
                controlfloppy.MicrostepsPerTrack = (int)MicrostepsPerTrackUpDown.Value;
                controlfloppy.StepStickMicrostepping = (int)Properties.Settings.Default["StepStickMicrostepping"];
                controlfloppy.trk00offset = (int)TRK00OffsetUpDown.Value;
                controlfloppy.EndTrack = (int)NetworkCaptureTrackEndUpDown.Value;

                controlfloppy.tbr = tbreceived;
                //processing.indexrxbuf = indexrxbuf;

                controlfloppy.outputfilename = outputfilename.Text;
                controlfloppy.rxbuf = processing.rxbuf;

                // Callbacks
                controlfloppy.updateHistoAndSliders = updateHistoAndSliders;
                controlfloppy.ControlFloppyScatterplotCallback = ControlFloppyScatterplotCallback;
                controlfloppy.Setrxbufcontrol = Setrxbufcontrol;


                scope.controlfloppy = controlfloppy; // reference the controlfloppy class

                if (NetCaptureRangecheckBox.Checked)
                {
                    int start, end;

                    start = (int)NetworkCaptureTrackStartUpDown.Value;
                    end = (int)NetworkCaptureTrackEndUpDown.Value;

                    for (i = start; i < end + 1; i++)
                    {
                        controlfloppy.EndTrack = i;
                        controlfloppy.StartTrack = i;

                        scope.capturedataindex = 0;
                        scope.capturedatablocklength = 250000;
                        scope.stop = 0;
                        scope.capturedatastate = 0;
                        scope.xscalemv = (int)xscalemvUpDown.Value;
                        scope.capturetimerstart();
                        while (scope.SaveFinished == false && processing.stop != 1)
                        {
                            Thread.Sleep(100);
                            Application.DoEvents();
                        }
                        scope.SaveFinished = false;
                        if (processing.stop == 1)
                            break;
                    }
                }
                else if (NetworkDoAllBad.Checked)
                {
                    int start, end;

                    start = (int)NetworkCaptureTrackStartUpDown.Value;
                    end = (int)NetworkCaptureTrackEndUpDown.Value;

                    int j, dotrack = 0;

                    for (i = start; i < end + 1; i++)
                    {
                        for (j = 0; j < processing.sectorspertrack; j++)
                            if (processing.sectormap.sectorok[i, j] == SectorMapStatus.empty || processing.sectormap.sectorok[i, j] == SectorMapStatus.HeadOkDataBad)
                            {
                                dotrack = 1;
                                break;
                            }
                            else
                            {
                                dotrack = 0;
                            }
                        if (dotrack == 1)
                        {
                            controlfloppy.StartTrack = i;

                            scope.capturedataindex = 0;
                            scope.capturedatablocklength = 250000;
                            scope.stop = 0;
                            scope.capturedatastate = 0;
                            scope.capturetimerstart();
                            while (scope.SaveFinished == false && processing.stop != 1)
                            {
                                Thread.Sleep(100);
                                Application.DoEvents();
                            }
                        }
                        scope.SaveFinished = false;
                        if (processing.stop == 1)
                            break;
                    }
                }
                else
                {
                    controlfloppy.StartTrack = (int)NetworkCaptureTrackStartUpDown.Value;

                    scope.capturedataindex = 0;
                    scope.capturedatablocklength = 250000;
                    scope.stop = 0;
                    scope.capturedatastate = 0;
                    scope.capturetimerstart();
                }
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            scope.stop = 1;
            scope.capturedatastate = 3;
            scope.networktimerstop();
            scope.capturetimerstop();
        }
        

        private void CaptureClassbutton_Click(object sender, EventArgs e)
        {
            ConnectToFloppyControlHardware();
            CaptureTracks();
        }

        private void CaptureTracks()
        {
            resetinput();
            processing.entropy = null;
            tabControl1.SelectedTab = ScatterPlottabPage;
            controlfloppy.MicrostepsPerTrack = (int)MicrostepsPerTrackUpDown.Value;
            controlfloppy.StepStickMicrostepping = (int)Properties.Settings.Default["StepStickMicrostepping"];
            controlfloppy.trk00offset = (int)TRK00OffsetUpDown.Value;
            controlfloppy.EndTrack = (int)EndTracksUpDown.Value;
            controlfloppy.StartTrack = (int)StartTrackUpDown.Value;
            if (controlfloppy.EndTrack == controlfloppy.StartTrack)
                controlfloppy.EndTrack++;
            controlfloppy.TrackDuration = (int)TrackDurationUpDown.Value;
            controlfloppy.outputfilename = outputfilename.Text;

            if (controlfloppy.serialPort1.IsOpen)
                controlfloppy.StartCapture();
            else
                tbreceived.Append("Not connected.\r\n");
        }

        public void Setrxbufcontrol()
        {
            //indexrxbuf = processing.indexrxbuf;
            rxbufStartUpDown.Maximum = processing.rxbuf.Length;
            rxbufEndUpDown.Maximum = processing.rxbuf.Length;
            rxbufEndUpDown.Value = processing.rxbuf.Length;
            HistogramhScrollBar1.Minimum = 0;
            HistogramhScrollBar1.Maximum = processing.indexrxbuf;
            scatterplot.rxbuf = processing.rxbuf;
        }

        private void HistogramhScrollBar1_Scroll(object sender, EventArgs e)
        {
            scatterplot.AnScatViewlargeoffset = HistogramhScrollBar1.Value;
            scatterplot.UpdateScatterPlot();
        }

        private void QHistogramhScrollBar1_Scroll(object sender, EventArgs e)
        {
            HistogramhScrollBar1.Value = QHistogramhScrollBar1.Value;
            scatterplot.AnScatViewlargeoffset = QHistogramhScrollBar1.Value;
            
            scatterplot.UpdateScatterPlot();
        }

        private void GCbutton_Click(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void AnalysisTab2_Enter_1(object sender, EventArgs e)
        {
            oscilloscope.graphset.allowrepaint = false;
        }

        private void rtbSectorMap_DoubleClick(object sender, EventArgs e)
        {
            rtbSectorMap.DeselectAll();
            RateOfChange2UpDown.Focus();
            Application.DoEvents();
            processing.sectormap.RefreshSectorMap();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            int i;
            int offset = (int)DiffOffsetUpDown.Value;
            for (i = Math.Abs(offset); i < graphwaveform[0].Length - (Math.Abs(offset)); i++)
            {
                graphwaveform[2][i] = (byte)(127 + (graphwaveform[0][i] - graphwaveform[1][i + offset]) / 2);
            }
        }

        private void DiffOffsetUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (graphwaveform[0] != null)
            {
                int i;
                int offset = (int)DiffOffsetUpDown.Value;
                for (i = Math.Abs(offset); i < graphwaveform[0].Length - (Math.Abs(offset)); i++)
                {
                    graphwaveform[2][i] = (byte)(127 + (graphwaveform[0][i] - graphwaveform[1][i + offset]) / 2);
                }
                oscilloscope.graphset.Graphs[2].changed = true;
                oscilloscope.graphset.UpdateGraphs();
            }
        }

        private void FloppyControl_SizeChanged(object sender, EventArgs e)
        {
            int i;

            for (i = 0; i < oscilloscope.graphset.Graphs.Count; i++)
            {
                oscilloscope.graphset.Graphs[i].width = GraphPictureBox.Width;
                oscilloscope.graphset.Graphs[i].height = GraphPictureBox.Height;
                oscilloscope.graphset.Resize();
            }
        }

        // Undo
        private void button31_Click_1(object sender, EventArgs e)
        {
            int i;
            if (oscilloscope.graphset.Graphs.Count > 1)
                if (oscilloscope.graphset.Graphs[0].undo.Count > 0)
                {
                    var undo = oscilloscope.graphset.Graphs[0].undo;
                    int undolistindex = undo.Count - 1;
                    int offset = undo[undolistindex].offset;
                    byte[] d = undo[undolistindex].undodata;
                    int length = d.Length;

                    for (i = 0; i < length; i++)
                    {
                        oscilloscope.graphset.Graphs[0].data[i + offset] = d[i];
                    }
                    undo.Remove(undo[undolistindex]);

                    oscilloscope.graphset.Graphs[0].changed = true;
                    oscilloscope.graphset.UpdateGraphs();
                }
        }

        private void SaveWaveformButton_Click(object sender, EventArgs e)
        {
            oscilloscope.graphset.saveAll();
        }

        //Copy graph[0]
        private void button32_Click(object sender, EventArgs e)
        {
            Graph2 src = oscilloscope.graphset.Graphs[0];

            if (oscilloscope.graphset.Graphs.Count < 5)
                oscilloscope.graphset.AddGraph((byte[])src.data.Clone());

            Graph2 dst = oscilloscope.graphset.Graphs[4];

            dst.changed = true;
            dst.data = Clone4(src.data);
            dst.datalength = src.datalength;
            dst.dataoffset = src.dataoffset;
            dst.density = src.density;

            dst.yscale = src.yscale;
            dst.yoffset = src.yoffset;
            src.zorder = 10;
            dst.zorder = 9;
            oscilloscope.graphset.UpdateGraphs();
        }

        static byte[] Clone4(byte[] array)
        {
            byte[] result = new byte[array.Length];
            Buffer.BlockCopy(array, 0, result, 0, array.Length * sizeof(byte));
            return result;
        }

        private void button33_Click(object sender, EventArgs e)
        {
            oscilloscope.graphset.Graphs[graphselect].DCOffset();
            oscilloscope.graphset.Graphs[graphselect].changed = true;
            oscilloscope.graphset.UpdateGraphs();
        }

        private void Lowpassbutton_Click(object sender, EventArgs e)
        {
            oscilloscope.graphset.Graphs[graphselect].Lowpass((int)SmoothingUpDown.Value);
            oscilloscope.graphset.Graphs[graphselect].changed = true;
            oscilloscope.graphset.UpdateGraphs();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            oscilloscope.graphset.Graphs[graphselect].Lowpass2((int)SmoothingUpDown.Value);
            oscilloscope.graphset.Graphs[graphselect].changed = true;
            oscilloscope.graphset.UpdateGraphs();
        }
        private void button33_Click_1(object sender, EventArgs e)
        {
            oscilloscope.graphset.Graphs[graphselect].Highpass((int)HighpassThresholdUpDown.Value);
            oscilloscope.graphset.Graphs[graphselect].changed = true;
            oscilloscope.graphset.UpdateGraphs();
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            int indexS1, threadid;

            ECSettings ecSettings = new ECSettings();
            ECResult sectorresult;
            ecSettings.sectortextbox = textBoxSector;

            if (BadSectorListBox.SelectedIndices.Count >= 1)
            {
                indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;
                ecSettings.indexS1 = indexS1;
                ecSettings.periodSelectionStart = (int)ScatterMinUpDown.Value;
                ecSettings.periodSelectionEnd = (int)ScatterMaxUpDown.Value;
                ecSettings.combinations = (int)CombinationsUpDown.Value;
                ecSettings.threadid = threadid;
                ecSettings.C6Start = (int)C6StartUpDown.Value;
                ecSettings.C8Start = (int)C8StartUpDown.Value;
            }
            else
            {
                textBoxReceived.AppendText("Error, no data selected.");
                return;
            }
            if (processing.procsettings.platform == 0)
            {
                processing.ECCluster2(ecSettings);
            }
            else processing.ProcessClusterAmiga(ecSettings);
        }

        private void AdaptiveScan()
        {
            float i;

            for (i = 0.6f; i < 2f; i += 0.2f)
            {
                if (processing.stop == 1)
                    break;
                RateOfChangeUpDown.Value = (decimal)i;
                Application.DoEvents();
                if (processing.procsettings.platform == 0)
                    ProcessPC();
                else
                    ProcessAmiga();
            }
            //processing.sectormap.RefreshSectorMap();
        }

        private void DirectStepCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["DirectStep"] = DirectStepCheckBox.Checked;
            Properties.Settings.Default.Save();
            controlfloppy.DirectStep = DirectStepCheckBox.Checked;
        }

        private void MicrostepsPerTrackUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["Microstepping"] = (int)MicrostepsPerTrackUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void SignalRatioDistUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (AnAutoUpdateCheckBox.Checked)
                GraphFilterButton.PerformClick();
        }

        private void AdaptLookAheadUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (AnAutoUpdateCheckBox.Checked)
                GraphFilterButton.PerformClick();
        }

        private void DiffMinDeviationUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (AnAutoUpdateCheckBox.Checked)
                GraphFilterButton.PerformClick();
        }

        private void DiffMinDeviation2UpDown_ValueChanged(object sender, EventArgs e)
        {
            if (AnAutoUpdateCheckBox.Checked)
                GraphFilterButton.PerformClick();
        }
        

        // Waveform editor tab, fix 8us method, an attempt to find and fix 8us waveform distortions
        private void button34_Click(object sender, EventArgs e)
        {
            oscilloscope.Fix8us(
                (int)DiffTest2UpDown.Value,
                (int)DiffTestUpDown.Value,
                (int)DiffTestUpDown.Value,
                (int)ThresholdTestUpDown.Value
                );
                 
            oscilloscope.graphset.SetAllChanged();
            oscilloscope.graphset.UpdateGraphs();
        }

        // Capture data current track button. Captures the track using the scope on the track that's last used when capturing.
        // It must still be connected on the Capture tab.
        private void button35_Click(object sender, EventArgs e)
        {
            int i;

            scope.tbr = tbreceived;

            string connection = (string)Properties.Settings.Default["ScopeConnection"];
            scope.Connect(connection);
            for (i = 0; i < 20; i++)
            {
                Thread.Sleep(50);
                if (scope.connectionStatus == 1)
                    break;
            }
            if (i == 19)
            {
                tbreceived.Append("Connection failed\r\n");
            }
            else
            {
                //scope.serialPort1 = serialPort1; // floppycontrol hardware
                selectedBaudRate = (int)Properties.Settings.Default["DefaultBaud"];
                selectedPortName = (string)Properties.Settings.Default["DefaultPort"];
                scope.serialPort1.PortName = selectedPortName;
                scope.serialPort1.BaudRate = selectedBaudRate;
                scope.ScopeMemDepth = (int)NumberOfPointsUpDown.Value;
                scope.UseAveraging = NetworkUseAveragingCheckBox.Checked;
                scope.xscalemv = (int)xscalemvUpDown.Value;

                controlfloppy.binfilecount = binfilecount;
                controlfloppy.DirectStep = DirectStepCheckBox.Checked;
                controlfloppy.MicrostepsPerTrack = (int)MicrostepsPerTrackUpDown.Value;
                controlfloppy.trk00offset = (int)TRK00OffsetUpDown.Value;
                controlfloppy.EndTrack = (int)NetworkCaptureTrackEndUpDown.Value;

                controlfloppy.tbr = tbreceived;
                //processing.indexrxbuf = indexrxbuf;
                controlfloppy.StepStickMicrostepping = (int)Properties.Settings.Default["StepStickMicrostepping"];
                controlfloppy.outputfilename = outputfilename.Text;
                controlfloppy.rxbuf = processing.rxbuf;
                scope.controlfloppy = controlfloppy; // reference the controlfloppy class

                int start, end;

                start = (int)NetworkCaptureTrackStartUpDown.Value;
                end = (int)NetworkCaptureTrackEndUpDown.Value;

                controlfloppy.StartTrack = i;

                scope.capturedataindex = 0;
                scope.capturedatablocklength = 250000;
                scope.stop = 0;
                scope.capturedatastate = 0;
                scope.NoControlFloppy = true;
                scope.capturetimerstart();

                while (scope.SaveFinished == false && processing.stop != 1)
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                }
                scope.SaveFinished = false;

            }
        }

        private void FindPeaks()
        {
            if (processing.indexrxbuf == 0) return;
            processing.FindPeaks(HistogramhScrollBar1.Value);

            int peak1 = processing.peak1;
            int peak2 = processing.peak2;
            int peak3 = processing.peak3;
            ProcessingType procmode = ProcessingType.adaptive1;
            if (ProcessingModeComboBox.SelectedItem.ToString() != "")
                procmode = (ProcessingType)Enum.Parse(typeof(ProcessingType), ProcessingModeComboBox.SelectedItem.ToString(), true);
            tbreceived.Append("Selected: " + procmode.ToString() + "\r\n");

            switch (procmode)
            {
                case ProcessingType.normal:
                    FourvScrollBar.Value = peak1 + ((peak2 - peak1) / 2);
                    SixvScrollBar.Value = peak2 + ((peak3 - peak2) / 2);
                    break;
                case ProcessingType.aufit:
                    break;
                case ProcessingType.adaptive1:
                case ProcessingType.adaptive2:
                case ProcessingType.adaptive3:
                case ProcessingType.adaptivePredict:
                    FourvScrollBar.Value = peak1+4;
                    SixvScrollBar.Value = peak2+2;
                    EightvScrollBar.Value = peak3;
                    break;
            }

            /*
            if (AdaptradioButton.Checked)
            {
                FourvScrollBar.Value = peak1;
                SixvScrollBar.Value = peak2;
                EightvScrollBar.Value = peak3;
            }
            else if (NormalradioButton.Checked)
            {
                FourvScrollBar.Value = peak1 + ((peak2 - peak1) / 2);
                SixvScrollBar.Value = peak2 + ((peak3 - peak2) / 2);
                //EightvScrollBar.Value = peak3;
            }
            */
            updateSliderLabels();
        }
        
        private void Histogrampanel1_Click(object sender, EventArgs e)
        {
            FindPeaks();
            updateAnScatterPlot();
        }

        private void JumpTocomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int grphcnt = oscilloscope.graphset.Graphs.Count;
            int i;
            int index = JumpTocomboBox.SelectedIndex;
            ComboboxItem item;
            item = (ComboboxItem)JumpTocomboBox.Items[index];

            int id = item.id;
            //tbreceived.Append("Item: " + id+"\r\n");


            // First position the scatterplot on the selected area
            int offset = 0;
            for (i = 1; i < processing.sectordata2.Count; i++)
            {
                if (processing.sectordata2[i].rxbufMarkerPositions > scatterplot.rxbufclickindex)
                {
                    offset = scatterplot.rxbufclickindex - processing.sectordata2[i - 1].rxbufMarkerPositions;
                    break;
                }
            }
            //ScatterMinTrackBar.Value = offset;
            //ScatterMaxTrackBar.Value = offset + 14;
            //updateECInterface();

            int scatoffset = processing.sectordata2[id].rxbufMarkerPositions + (int)ScatterMinTrackBar.Value + (int)ScatterOffsetTrackBar.Value;
            int scatlength = processing.sectordata2[id].rxbufMarkerPositions + (int)ScatterMaxTrackBar.Value + (int)ScatterOffsetTrackBar.Value - scatoffset;
            int graphoffset = scatoffset + (scatlength / 2);
            scatterplot.AnScatViewlargeoffset = scatoffset;
            scatterplot.AnScatViewoffset = 0;
            scatterplot.AnScatViewlength = scatlength;
            scatterplot.UpdateScatterPlot();

            if (grphcnt == 0)
            {
                return;
            }
            for (i = 0; i < grphcnt; i++)
            {
                oscilloscope.graphset.Graphs[i].datalength = 2000;
                oscilloscope.graphset.Graphs[i].dataoffset = processing.rxbuftograph[graphoffset] - 1000;

                if (oscilloscope.graphset.Graphs[i].dataoffset < 0)
                    oscilloscope.graphset.Graphs[i].dataoffset = 0;

                oscilloscope.graphset.Graphs[i].changed = true;
                oscilloscope.graphset.Graphs[i].density = 1;
            }
            //tbreceived.Append("rxbuf pos: "+ (processing.sectordata2[id].rxbufMarkerPositions + offset + 1000));
            oscilloscope.graphset.UpdateGraphs();
            MainTabControl.SelectedTab = AnalysisTab2;
        }

        private void rtbSectorMap_MouseDown(object sender, MouseEventArgs e)
        {
            ContextMenuStrip smmenu = new ContextMenuStrip();
            int sector, track;
            int i;
            int div = processing.sectorspertrack + 6;
            LimitToTrackUpDown.Value = track = (rtbSectorMap.SelectionStart / div);
            LimitToSectorUpDown.Value = sector = (rtbSectorMap.SelectionStart % div - 5);

            if (sector < 0) return;
            TrackUpDown.Value = track;
            SectorUpDown.Value = sector;

            if (e.Button == MouseButtons.Left)
            {
                tbreceived.Append("Track: " + track + " sector: " + sector + " div:" + div+"\r\n");
                for (i = 0; i < processing.sectordata2.Count; i++)
                {
                    if (processing.sectordata2 == null) continue;
                    if (processing.sectordata2.Count != 0)
                    {
                        if (processing.sectordata2[i].track == track && processing.sectordata2[i].sector == sector)
                        {
                            //int track1 = track, sector1 = sector;
                            if (processing.sectordata2[i].mfmMarkerStatus == processing.sectormap.sectorok[track, sector])
                            {
                                if (processing.sectordata2.Count-1 > i)
                                {
                                    scatterplot.AnScatViewlargeoffset = processing.sectordata2[i].rxbufMarkerPositions - 50;
                                    scatterplot.AnScatViewoffset = 0;

                                    scatterplot.AnScatViewlength = processing.sectordata2[i + 1].rxbufMarkerPositions - scatterplot.AnScatViewlargeoffset + 100;
                                    //tbreceived.Append("AnScatViewOffset"+ AnScatViewoffset+"\r\n");
                                    scatterplot.UpdateScatterPlot();
                                }
                                break;

                            }
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                int index = rtbSectorMap.GetCharIndexFromPosition(new Point(e.X, e.Y));
                tbreceived.Append("Index: " + index + "\r\n");
                div = processing.sectorspertrack + 6;
                LimitToTrackUpDown.Value = track = (index / div);
                LimitToSectorUpDown.Value = sector = (index % div - 5);
                tbreceived.Append("Track: " + track + " sector: " + sector + " div:" + div);
                if (sector < 0) return;

                smmenu.ItemClicked += Smmenu_ItemClicked;
                tbreceived.Append("Track: " + track + "\r\n");

                SectorMapContextMenu[] menudata = new SectorMapContextMenu[10];
                ToolStripItem[] item = new ToolStripItem[10];
                int menudataindex = 0;
                // Capture tab

                menudata[menudataindex] = new SectorMapContextMenu();
                menudata[menudataindex].sector = sector;
                menudata[menudataindex].track = track;
                menudata[menudataindex].duration = 5000;
                menudata[menudataindex].cmd = 0;
                item[menudataindex] = smmenu.Items.Add("Recapture T" + track.ToString("D3") + " 5 sec", MainTabControlImageList.Images[0]);
                item[menudataindex].Tag = menudata[menudataindex];

                // Capture tab
                menudataindex++;
                menudata[menudataindex] = new SectorMapContextMenu();
                menudata[menudataindex].sector = sector;
                menudata[menudataindex].track = track;
                menudata[menudataindex].duration = 50000;
                menudata[menudataindex].cmd = 0;
                item[menudataindex] = smmenu.Items.Add("Recapture T" + track.ToString("D3") + " 50 sec", MainTabControlImageList.Images[0]);
                item[menudataindex].Tag = menudata[menudataindex];

                //Error correction tab
                menudataindex++;
                menudata[menudataindex] = new SectorMapContextMenu();
                menudata[menudataindex].sector = sector;
                menudata[menudataindex].track = track;
                menudata[menudataindex].cmd = 1;
                item[menudataindex] = smmenu.Items.Add("Error Correct T" + track.ToString("D3") + " S" + sector, MainTabControlImageList.Images[1]);
                item[menudataindex].Tag = menudata[menudataindex];

                //Scope waveform capture
                menudataindex++;
                menudata[menudataindex] = new SectorMapContextMenu();
                menudata[menudataindex].sector = sector;
                menudata[menudataindex].track = track;
                menudata[menudataindex].cmd = 2;
                item[menudataindex] = smmenu.Items.Add("Get waveform T" + track.ToString("D3") + " S" + sector, MainTabControlImageList.Images[2]);
                item[menudataindex].Tag = menudata[menudataindex];

                //Select rxdata
                menudataindex++;
                menudata[menudataindex] = new SectorMapContextMenu();
                menudata[menudataindex].sector = sector;
                menudata[menudataindex].track = track;
                menudata[menudataindex].cmd = 3;
                item[menudataindex] = smmenu.Items.Add("Limit rxdata T" + track.ToString("D3") + " S" + sector, MainTabControlImageList.Images[2]);
                item[menudataindex].Tag = menudata[menudataindex];

                Point ShowHere = new Point(Cursor.Position.X, Cursor.Position.Y + 10);
                smmenu.Show(ShowHere);
            }
        }

        private void Smmenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            SectorMapContextMenu menudata = (SectorMapContextMenu)e.ClickedItem.Tag;
            if (menudata.cmd == 0)
            {
                tbreceived.Append("Track: " + menudata.track.ToString("D3") + " S" + menudata.sector + "\r\n");
                MainTabControl.SelectedTab = CaptureTab;
                StartTrackUpDown.Value = menudata.track;
                EndTracksUpDown.Value = menudata.track;
                TrackDurationUpDown.Value = menudata.duration;
            }
            else if (menudata.cmd == 1)
            {
                MainTabControl.SelectedTab = ErrorCorrectionTab;
                Track1UpDown.Value = menudata.track;
                Sector1UpDown.Value = menudata.sector;
                Track2UpDown.Value = menudata.track;
                Sector2UpDown.Value = menudata.sector;
            }
            else if (menudata.cmd == 2)
            {
                MainTabControl.SelectedTab = NetworkTab;
                NetworkCaptureTrackStartUpDown.Value = menudata.track;
                NetworkCaptureTrackEndUpDown.Value = menudata.track;
            }
            else if (menudata.cmd == 3)
            {
                int i;
                var sd = processing.sectordata2;
                for (i = 0; i < processing.sectordata2.Count; i++)
                {
                    if (sd[i].sector == menudata.sector && sd[i].track == menudata.track && sd[i].mfmMarkerStatus == SectorMapStatus.HeadOkDataBad)
                    {
                        rxbufStartUpDown.Maximum = processing.indexrxbuf;
                        rxbufStartUpDown.Value = sd[i].rxbufMarkerPositions - 100;
                        rxbufEndUpDown.Value = 15000;
                        break;
                    }
                }
            }
        }

        private void button38_Click(object sender, EventArgs e)
        {
            int i;
            tbSectorMap.AppendText("count\tdec\thex\tbin\r\n");
            for (i = 0; i < 256; i++)
            {
                if (mfmbyteenc[i] > 0)
                    tbSectorMap.AppendText(mfmbyteenc[i] + "\t" + i + "\t" + i.ToString("X2") + "\t" + Convert.ToString(i, 2).PadLeft(8, '0') + "\r\n");
                mfmbyteenc[i] = 0;
            }
        }

        private void Microstep8Btn_Click(object sender, EventArgs e)
        {
            int StepStickMicrostepping = 8;
            int i;
            controlfloppy.serialPort1.Write(']'.ToString()); // Full step
            Thread.Sleep(10);

            for (i = 0; i < StepStickMicrostepping - 1; i++)
            {
                controlfloppy.serialPort1.Write('['.ToString()); // Full step
                Thread.Sleep(10);
            }
        }

        private void StepBackBtn_Click(object sender, EventArgs e)
        {
            controlfloppy.serialPort1.Write('g'.ToString()); // increase track number
            Thread.Sleep(controlfloppy.tracktotrackdelay);
        }

        private void StepForwardBtn_Click(object sender, EventArgs e)
        {
            controlfloppy.serialPort1.Write('t'.ToString()); // increase track number
            Thread.Sleep(controlfloppy.tracktotrackdelay);
        }

        private void button43_Click(object sender, EventArgs e)
        {
            int StepStickMicrostepping = 8;
            int i;
            controlfloppy.serialPort1.Write(']'.ToString()); // Full step
            Thread.Sleep(10);

            for (i = 0; i < StepStickMicrostepping - 1; i++)
            {
                controlfloppy.serialPort1.Write('['.ToString()); // Full step
                Thread.Sleep(10);
            }
        }

        private void button42_Click(object sender, EventArgs e)
        {
            controlfloppy.serialPort1.Write('g'.ToString()); // increase track number
            Thread.Sleep(controlfloppy.tracktotrackdelay);
        }

        private void button41_Click(object sender, EventArgs e)
        {
            controlfloppy.serialPort1.Write('t'.ToString()); // increase track number
            Thread.Sleep(controlfloppy.tracktotrackdelay);
        }

        private void ECMFMByteEncbutton_Click(object sender, EventArgs e)
        {
            int indexS1, threadid;
            ECSettings ecSettings = new ECSettings();
            
            ecSettings.sectortextbox = textBoxSector;

            if (BadSectorListBox.SelectedIndices.Count >= 1)
            {
                indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;
                ecSettings.indexS1 = indexS1;
                ecSettings.periodSelectionStart = (int)ScatterMinUpDown.Value;
                ecSettings.periodSelectionEnd = (int)ScatterMaxUpDown.Value;
                //ecSettings.combinations = (int)CombinationsUpDown.Value;
                ecSettings.threadid = threadid;
                ecSettings.MFMByteStart = (int)MFMByteStartUpDown.Value;
                ecSettings.MFMByteLength = (int)MFMByteLengthUpDown.Value;
            }
            else
            {
                textBoxReceived.AppendText("Error, no data selected.");
                return;
            }
            if (processing.procsettings.platform == 0)
                processing.ProcessClusterMFMEnc(ecSettings);
            else processing.ProcessClusterAmigaMFMEnc(ecSettings);
        }

        //Iterator test 
        private void button44_Click(object sender, EventArgs e)
        {
            int[] combi = new int[32];
            int[] combilimit = new int[32];
            int i, j, p, q, k;
            int combinations = 0;
            int NumberOfMfmBytes = 3;
            int MaxIndex = 32;
            int iterations;
            for (i = 0; i < NumberOfMfmBytes; i++)
            {
                combilimit[i] = 1;
            }
            for (j = 0; j < MaxIndex; j++)
            {

                for (q = 0; q < NumberOfMfmBytes; q++)
                {
                    combilimit[q]++;

                }

                int temp = combilimit[0];
                iterations = temp;
                for (q = 0; q < NumberOfMfmBytes - 1; q++)
                {
                    iterations *= temp;
                }
                tbreceived.Append("Iterations: " + iterations + "\r\n");

                for (i = 0; i < iterations; i++)
                {
                    //printarray(combi, NumberOfMfmBytes);
                    combi[0]++;
                    for (k = 0; k < NumberOfMfmBytes; k++)
                    {

                        if (combi[k] >= combilimit[0])
                        {
                            combi[k] = 0;
                            combi[k + 1]++;
                        }
                    }

                    combinations++;
                }

            }
            tbreceived.Append("Combinations:" + combinations + "\r\n");
        }

        //Open SCP
        private void button45_Click(object sender, EventArgs e)
        {
            fileio.OpenScp();
            rxbufEndUpDown.Maximum = processing.indexrxbuf;
            rxbufEndUpDown.Value = processing.indexrxbuf;
        }
        

        //Save SCP
        private void button46_Click(object sender, EventArgs e)
        {
            fileio.SaveSCP(
                    FourvScrollBar.Value,
                    SixvScrollBar.Value,
                    EightvScrollBar.Value,
                    ProcessingModeComboBox.SelectedItem.ToString()
                );
        }

        private void AdaptiveScan2()
        {
            int j, k, l;
            float i;
            int FOUR = FourvScrollBar.Value;
            int SIX = SixvScrollBar.Value;
            int EIGHT = EightvScrollBar.Value;
            int OFFSET = OffsetvScrollBar1.Value;
            int step = (int)iESStart.Value;
            for (l = -12; l < 13; l += step)
                for (i = 0.6f; i < 2f; i += 0.2f)
                {
                    if (processing.stop == 1)
                        break;
                    RateOfChangeUpDown.Value = (decimal)i;
                    OffsetvScrollBar1.Value = OFFSET + l;

                    Application.DoEvents();
                    
                    if (processing.procsettings.platform == 0)
                        ProcessPC();
                    else
                        ProcessAmiga();
                }

            OffsetvScrollBar1.Value = OFFSET;
            //processing.sectormap.RefreshSectorMap();
            /*
            if (processing.stop == 0)
                for (j = -9; j < 10; j += 3)
                    for (k = -9; k < 10; k += 3)
                    {
                        GC.Collect();
                        for (l = -9; l < 10; l += 3)
                            for (i = 1f; i < 2f; i += 0.1f)
                            {
                                if (processing.stop == 1)
                                    break;
                                RateOfChangeUpDown.Value = (decimal)i;
                                FourvScrollBar.Value = FOUR + l;
                                SixvScrollBar.Value = SIX + k;
                                EightvScrollBar.Value = EIGHT + j;

                                Application.DoEvents();
                                processing.stop = 0;
                                if (processing.procsettings.platform == 0)
                                    ProcessPC();
                                else
                                    ProcessAmiga();
                            }
                        processing.sectormap.RefreshSectorMap();

                    }
            */
            //processing.sectormap.RefreshSectorMap();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DiskFormat diskformat = DiskFormat.unknown;
            if (ChangeDiskTypeComboBox.SelectedItem.ToString() != "")
                diskformat = (DiskFormat)Enum.Parse(typeof(DiskFormat), ChangeDiskTypeComboBox.SelectedItem.ToString(), true);
            tbreceived.Append("Selected: " + diskformat.ToString() + "\r\n");

            processing.diskformat = diskformat;
            
        }

        private void button48_Click(object sender, EventArgs e)
        {
            int track, sector;

            var sectorok = processing.sectormap.sectorok;
            int starttrack = (int)StartTrackUpDown.Value;
            int endtrack = (int)EndTracksUpDown.Value;

            MainTabControl.SelectedTab = ProcessingTab;
            processing.stop = 0;

            for (track = starttrack; track < endtrack + 1; track++)
            {
                for (sector = 0; sector < processing.sectorspertrack; sector++)
                {
                    if (sectorok[track, sector] == 0 || sectorok[track, sector] == SectorMapStatus.HeadOkDataBad)
                    {
                        tbreceived.Append("Track: " + track);
                        StartTrackUpDown.Value = track;

                        EndTracksUpDown.Value = track;
                        CaptureTracks();
                        while (controlfloppy.capturecommand == 1)
                        {
                            Thread.Sleep(20);
                            Application.DoEvents();
                        }
                        Application.DoEvents();
                        AdaptiveScan();
                        Application.DoEvents();
                        if (processing.stop == 1)
                            break;
                        break;
                    }
                }
                if (processing.stop == 1)
                    break;
            }
            StartTrackUpDown.Value = starttrack;
            EndTracksUpDown.Value = endtrack;

            tbreceived.Append("Done!\r\n");
        }

        private void button49_Click(object sender, EventArgs e)
        {
            fileio.SaveTrimmedBadBinFile(
                    FourvScrollBar.Value,
                    SixvScrollBar.Value,
                    EightvScrollBar.Value,
                    ProcessingModeComboBox.SelectedItem.ToString()
                );
        }

        private void SaveTrimmedBadbutton_Click(object sender, EventArgs e)
        {
            fileio.SaveTrimmedBadBinFile(
                    (int)FourvScrollBar.Value, 
                    (int)SixvScrollBar.Value, 
                    (int)EightvScrollBar.Value, 
                    ProcessingModeComboBox.SelectedItem.ToString()
                );
        }

        private void AdaptiveScan3()
        {
            int j, l;
            float i, k;
            float adaptrate = (float)RateOfChangeUpDown.Value;
            int FOUR = FourvScrollBar.Value;
            int SIX = SixvScrollBar.Value;
            int EIGHT = EightvScrollBar.Value;
            int OFFSET = OffsetvScrollBar1.Value;
            int step = (int)iESStart.Value;
            for (k = 2; k < 1200; k *= 2)
            {
                RateOfChange2UpDown.Value = (int)k;
                for (l = -12; l < 13; l += step)
                    for (i = 0.6f; i < 2f; i += 0.2f)
                    {
                        if (processing.stop == 1)
                            break;
                        RateOfChangeUpDown.Value = (decimal)i;
                        OffsetvScrollBar1.Value = OFFSET + l;

                        Application.DoEvents();
                        if (processing.procsettings.platform == 0)
                            ProcessPC();
                        else
                            ProcessAmiga();
                    }

            }
            OffsetvScrollBar1.Value = OFFSET;
            //processing.sectormap.RefreshSectorMap();
        }

        private void AdaptiveScan4()
        {
            int j, l;
            float i, k;
            float adaptrate = (float)RateOfChangeUpDown.Value;
            int FOUR = FourvScrollBar.Value;
            int SIX = SixvScrollBar.Value;
            int EIGHT = EightvScrollBar.Value;
            int OFFSET = OffsetvScrollBar1.Value;
            int step = (int)iESStart.Value;
            
            for (l = 0; l < 4; l += step)
                for (i = 0.6f; i < 2f; i += 0.2f)
                {
                    if (processing.stop == 1)
                        break;
                    RateOfChangeUpDown.Value = (decimal)i;
                    OffsetvScrollBar1.Value = OFFSET + l;

                    Application.DoEvents();
                    if (processing.procsettings.platform == 0)
                        ProcessPC();
                    else
                        ProcessAmiga();
                }

            OffsetvScrollBar1.Value = OFFSET;
        }
        private void AdaptiveNarrow()
        {
            int j, l;
            float i, k;
            float adaptrate = (float)RateOfChangeUpDown.Value;
            int FOUR = FourvScrollBar.Value;
            int SIX = SixvScrollBar.Value;
            int EIGHT = EightvScrollBar.Value;
            int OFFSET = OffsetvScrollBar1.Value;
            int step = (int)iESStart.Value;

            for (l = 0; l < 8; l += step)
                //for (i = 0.5f; i < 2f; i += 0.2f)
                {
                    if (processing.stop == 1) break;
                    //RateOfChangeUpDown.Value = (decimal)i;
                    FourvScrollBar.Value = FOUR + l;
                    EightvScrollBar.Value = EIGHT - l;
                    Application.DoEvents();
                    if (processing.procsettings.platform == 0)
                        ProcessPC();
                    else
                        ProcessAmiga();
                }
            FourvScrollBar.Value = FOUR;
            EightvScrollBar.Value = EIGHT;
            OffsetvScrollBar1.Value = OFFSET;
        }
        private void AdaptiveNarrowRate()
        {
            int j, l;
            float i, k;
            float adaptrate = (float)RateOfChangeUpDown.Value;
            int FOUR = FourvScrollBar.Value;
            int SIX = SixvScrollBar.Value;
            int EIGHT = EightvScrollBar.Value;
            int OFFSET = OffsetvScrollBar1.Value;
            int step = (int)iESStart.Value;

            for (l = 0; l < 8; l += step)
            for (i = 0.5f; i < 2f; i += 0.2f)
            {
                if (processing.stop == 1) break;
                RateOfChangeUpDown.Value = (decimal)i;
                FourvScrollBar.Value = FOUR + l;
                EightvScrollBar.Value = EIGHT - l;
                Application.DoEvents();
                processing.stop = 0;
                if (processing.procsettings.platform == 0)
                    ProcessPC();
                else
                    ProcessAmiga();
            }
            FourvScrollBar.Value = FOUR;
            EightvScrollBar.Value = EIGHT;
            OffsetvScrollBar1.Value = OFFSET;
        }

        private void ProcessingModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessingType procmode = ProcessingType.adaptive1;
            if (ProcessingModeComboBox.SelectedItem.ToString() != "")
                procmode = (ProcessingType)Enum.Parse(typeof(ProcessingType), ProcessingModeComboBox.SelectedItem.ToString(), true);
            tbreceived.Append("Selected: " + procmode.ToString()+"\r\n");

            
            switch (procmode)
            {
                case ProcessingType.normal:
                    FindPeaks();
                    EightvScrollBar.Value = 0xff;
                    scatterplot.showEntropy = false;
                    break;
                case ProcessingType.aufit:
                    MinvScrollBar.Value = 0x32;
                    FourvScrollBar.Value = 0x0C;
                    OffsetvScrollBar1.Value = 0;
                    scatterplot.showEntropy = false;
                    break;
                case ProcessingType.adaptive1:
                case ProcessingType.adaptive2:
                case ProcessingType.adaptive3:
                    RateOfChangeUpDown.Value = (decimal)1.1;
                    RateOfChange2UpDown.Value = 350;

                    FindPeaks();
                    scatterplot.showEntropy = false;
                    break;
                case ProcessingType.adaptivePredict:
                    RateOfChangeUpDown.Value = (decimal)3;
                    RateOfChange2UpDown.Value = 600;
                    FindPeaks();
                    scatterplot.showEntropy = false;
                    break;
                case ProcessingType.adaptiveEntropy:
                    scatterplot.showEntropy = true;
                    break;
            }
        }

        private void ScanBtn_Click_1(object sender, EventArgs e)
        {
            processing.stop = 0;
            DoScan();
            tbreceived.Append("\r\nDone!\r\n");
        }

        private void DoScan()
        {
            ScanMode procmode = ScanMode.AdaptiveRate;
            if (ScanComboBox.SelectedItem.ToString() != "")
                procmode = (ScanMode)Enum.Parse(typeof(ScanMode), ScanComboBox.SelectedItem.ToString(), true);
            tbreceived.Append("Selected: " + procmode.ToString() + "\r\n");


            switch (procmode)
            {
                case ScanMode.AdaptiveRate:
                    AdaptiveScan();
                    break;
                case ScanMode.AdaptiveOffsetRate:
                    AdaptiveScan2();
                    break;
                case ScanMode.AdaptiveDeep:
                    AdaptiveScan3();
                    break;
                case ScanMode.AdaptiveShallow:
                    AdaptiveScan4();
                    break;
                case ScanMode.AuScan:
                    AuScan();
                    break;
                case ScanMode.ExtremeScan:
                    ExtremeScan();
                    break;
                case ScanMode.OffsetScan:
                    OffsetScan();
                    break;
                case ScanMode.OffsetScan2:
                    OffsetScan2();
                    break;
                case ScanMode.AdaptiveNarrow:
                    AdaptiveNarrow();
                    break;
                case ScanMode.AdaptiveNarrowRate:
                    AdaptiveNarrowRate();
                    break;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            scatterplot.EditScatterplot = EditScatterPlotcheckBox.Checked;
        }

        private void TrackPreset1Button_Click_1(object sender, EventArgs e)
        {
            StartTrackUpDown.Value = 0;
            EndTracksUpDown.Value = 10;
            TrackDurationUpDown.Value = 260;
        }

        private void FullHistBtn_Click(object sender, EventArgs e)
        {
            ScatterHisto.DoHistogram(processing.rxbuf, (int)rxbufStartUpDown.Value, (int)rxbufEndUpDown.Value);
        }

        void updateAllGraphs()
        {
            if (controlfloppy.capturecommand == 1)
            {
                processing.rxbuf = controlfloppy.tempbuffer.Skip(Math.Max(0, controlfloppy.tempbuffer.Count() - 30)).SelectMany(a => a).ToArray();
            }
            else
            {
                processing.rxbuf = controlfloppy.tempbuffer.SelectMany(a => a).ToArray();
            }
            //processing.rxbuf = controlfloppy.tempbuffer.SelectMany(a => a).ToArray();

            Setrxbufcontrol();

            if (processing.indexrxbuf < 100000)
                scatterplot.AnScatViewlength = processing.indexrxbuf;
            else scatterplot.AnScatViewlength = 99999;
            scatterplot.AnScatViewoffset = 0;
            //scatterplot.UpdateScatterPlot();

            
            if (processing.indexrxbuf > 0)
                ProcessingTab.Enabled = true;
            if (controlfloppy.capturecommand == 0)
                HistogramhScrollBar1.Value = 0;
            if (processing.indexrxbuf > 0)
            {
                //updateAnScatterPlot();
                scatterplot.AnScatViewlargeoffset = HistogramhScrollBar1.Value;
                scatterplot.UpdateScatterPlot();
                if (controlfloppy.capturecommand == 0)
                {
                    ScatterHisto.DoHistogram();
                    updateSliderLabels();
                    updateHistoAndSliders();
                }
                
            }
        }

        private void TrackPreset3Button_Click(object sender, EventArgs e)
        {
            StartTrackUpDown.Value = 0;
            EndTracksUpDown.Value = 166;
            TrackDurationUpDown.Value = 260;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TrackDurationUpDown.Value = 1000;
        }

        private void TrackPreset4Button_Click(object sender, EventArgs e)
        {
            StartTrackUpDown.Value = 78;
            EndTracksUpDown.Value = 164;
            TrackDurationUpDown.Value = 260;
        }

        public void FilterGuiUpdateCallback()
        {
            FindPeaks();
            rxbufEndUpDown.Maximum = processing.indexrxbuf;
            rxbufStartUpDown.Maximum = processing.indexrxbuf;

            rxbufEndUpDown.Value = processing.indexrxbuf;
            HistogramhScrollBar1.Minimum = 0;
            HistogramhScrollBar1.Maximum = processing.indexrxbuf;

            oscilloscope.graphset.SetAllChanged();

            if (scatterplot.AnScatViewlength == 0 || scatterplot.AnScatViewlength == 100000)
                scatterplot.AnScatViewlength = processing.indexrxbuf - 1;
            scatterplot.UpdateScatterPlot();
            oscilloscope.graphset.UpdateGraphs();
            if (processing.indexrxbuf > 0)
                ProcessingTab.Enabled = true;
        }

        public void Filter2GuiCallback()
        {
            rxbufEndUpDown.Maximum = processing.indexrxbuf;
            rxbufStartUpDown.Maximum = processing.indexrxbuf;

            rxbufEndUpDown.Value = processing.indexrxbuf;
            HistogramhScrollBar1.Minimum = 0;
            HistogramhScrollBar1.Maximum = processing.indexrxbuf;
            //processing.indexrxbuf = indexrxbuf;

            oscilloscope.graphset.SetAllChanged();

            if (scatterplot.AnScatViewlength == 0)
                scatterplot.AnScatViewlength = processing.indexrxbuf - 1;
            scatterplot.UpdateScatterPlot();
            oscilloscope.graphset.UpdateGraphs();
            if (processing.indexrxbuf > 0)
                ProcessingTab.Enabled = true;
        }

        private void TRK00OffsetUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["TRK00Offset"] = (int)TRK00OffsetUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void DirectPresetBtn_Click(object sender, EventArgs e)
        {
            TRK00OffsetUpDown.Value = -1;
            MicrostepsPerTrackUpDown.Value = 1;
            DirectStepCheckBox.Checked = true;
            controlfloppy.StepStickMicrostepping = 1;
            controlfloppy.MicrostepsPerTrack = 1;
            controlfloppy.DirectStep = true;

            Properties.Settings.Default["StepStickMicrostepping"] = 1;
            Properties.Settings.Default["Microstepping"] = 1;
            Properties.Settings.Default["TRK00Offset"] = -1;
            Properties.Settings.Default["DirectStep"] = true;
            Properties.Settings.Default.Save();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            TRK00OffsetUpDown.Value = 16;
            MicrostepsPerTrackUpDown.Value = 8;
            DirectStepCheckBox.Checked = false;
            controlfloppy.StepStickMicrostepping = 8;
            controlfloppy.MicrostepsPerTrack = 8;
            controlfloppy.DirectStep = false;

            Properties.Settings.Default["StepStickMicrostepping"] = 8;
            Properties.Settings.Default["Microstepping"] = 8;
            Properties.Settings.Default["TRK00Offset"] = 16;
            Properties.Settings.Default["DirectStep"] = false;
            Properties.Settings.Default.Save();

        }

        private void FloppyControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }

        private void ResetBuffersBtn_Click(object sender, EventArgs e)
        {
            resetinput();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            resetoutput();
        }

        private void QProcessingModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessingModeComboBox.SelectedIndex = QProcessingModeComboBox.SelectedIndex;
        }

        private void QChangeDiskTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDiskTypeComboBox.SelectedIndex = QChangeDiskTypeComboBox.SelectedIndex;
        }

        private void QScanComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScanComboBox.SelectedIndex = QScanComboBox.SelectedIndex;
        }

        private void QClearDatacheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ClearDatacheckBox.Checked = QClearDatacheckBox.Checked;
        }

        private void QIgnoreHeaderErrorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            IgnoreHeaderErrorCheckBox.Checked = QIgnoreHeaderErrorCheckBox.Checked;
        }

        private void QFindDupesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            FindDupesCheckBox.Checked = QFindDupesCheckBox.Checked;
        }

        private void QAutoRefreshSectorMapCheck_CheckedChanged(object sender, EventArgs e)
        {
            AutoRefreshSectorMapCheck.Checked = QAutoRefreshSectorMapCheck.Checked;
        }

        private void QHDCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            HDCheckBox.Checked = QHDCheckBox.Checked;
        }

        private void QLimitTSCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LimitTSCheckBox.Checked = QLimitTSCheckBox.Checked;
        }

        private void QRateOfChange2UpDown_ValueChanged(object sender, EventArgs e)
        {
            RateOfChange2UpDown.Value = QRateOfChange2UpDown.Value;
        }

        private void QRateOfChangeUpDown_ValueChanged(object sender, EventArgs e)
        {
            RateOfChangeUpDown.Value = QRateOfChangeUpDown.Value;
        }

        private void QAdaptOfsset2UpDown_ValueChanged(object sender, EventArgs e)
        {
            AdaptOfsset2UpDown.Value = QAdaptOfsset2UpDown.Value;
        }

        private void QLimitToSectorUpDown_ValueChanged(object sender, EventArgs e)
        {
            LimitToSectorUpDown.Value = QLimitToSectorUpDown.Value;
        }

        private void QLimitToTrackUpDown_ValueChanged(object sender, EventArgs e)
        {
            LimitToTrackUpDown.Value = QLimitToTrackUpDown.Value;
        }
    } // end class
} // End namespace

