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
using System.Diagnostics;
using FloppyControlApp.MyClasses.Processing;

namespace FloppyControlApp
{
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
        #region variable definitions
        private string GuiMode;
        private FDDProcessing processing;
        private ControlFloppy controlfloppy;
        private connectsocketNIVisa2 scope = new connectsocketNIVisa2();
        //private BinaryReader reader;
        private StringBuilder SectorInfo = new StringBuilder();
        private StringBuilder tbtxt = new StringBuilder();
        //private BinaryWriter writer;
        private Point BadSectorTooltipPos;
        private StringBuilder tbreceived = new StringBuilder();
        private int bytesReceived = 0;
        //private Graphset graphset;
        private Histogram ECHisto;
        private Histogram ScatterHisto;
        private ScatterPlot scatterplot;
        private static readonly object lockaddmarker = new object();
        //private static uint markerpositionscnt;
        private string subpath;
        //private string path = "";
        private string selectedPortName;
        //private string[] openfilespaths;
        private int disablecatchkey = 0;
        private int binfilecount = 0; // Keep saving each capture under a different filename as to keep all captured data
        private float capturetime = 0;
        //private int capturing = 0;
        private int selectedBaudRate = 5000000;
        private int graphselect = 0;
        //private int maxthreads = 2;
        private int byteinsector = 0;
        //private int stepspertrack = 8;
        private byte[] TempSector = new byte[550];
        private byte[][] graphwaveform = new byte[15][];
        //private bool AddData = false;
        public bool openFilesDlgUsed = false;
        public bool ScpOpenFilesDlgUsed = false;
        private bool scanactive = false;
        private bool stopupdatingGraph = false;
        private int[] mfmbyteenc = new int[256];
        private int indexrxbufprevious = 0;
        private Version version;
        private FileIO fileio;
        private WaveformEdit oscilloscope;
        #endregion
        public void InitializeFloppyControl()
        {
            version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1)
                                    .AddDays(version.Build).AddSeconds(version.Revision * 2);
            string displayableVersion = $"{version} ({buildDate})";

            ECHisto = new Histogram();
            ScatterHisto = new Histogram();

            InitializeComponent();
            this.Text += " v" + version.ToString();
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

            GuiMode = (string)Properties.Settings.Default["GuiMode"];
            SetGuiMode(GuiMode);
            outputfilename.Text = (string)Properties.Settings.Default["BaseFileName"];
            DirectStepCheckBox.Checked = (bool)Properties.Settings.Default["DirectStep"];
            //MicrostepsPerTrackUpDown.Value = Properties.Settings.Default["MicroStepsPerTrack"];
            //TRK00OffsetUpDown.Value = (int)Properties.Settings.Default["TRK00Offset"];

            bool directstep =
            QDirectStepCheckBox.Checked = (bool)Properties.Settings.Default["DirectStep"];
            if (directstep == true)
            {
                QDirectStepPresetBtn.PerformClick();
            }
            else
            {
                StepStickPresetBtn.PerformClick();
            }
            //QMicrostepsPerTrackUpDown.Value = (int)Properties.Settings.Default["MicroStepsPerTrack"];
            //QTRK00OffsetUpDown.Value = (int)Properties.Settings.Default["TRK00Offset"];

            subpath = @Properties.Settings.Default["PathToRecoveredDisks"].ToString();

            fileio = new FileIO();
            //fileio.FilesAvailableCallback += FilesAvailableCallback;
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
            MainTabControl.SelectedTab = QuickTab;
            //MainTabControl.SelectedTab = AnalysisPage;
            BadSectorTooltip.Hide();
            timer5.Start();
            GUITimer.Start();
            BluetoRedByteCopyToolBtn.Tag = new int();
            BluetoRedByteCopyToolBtn.Tag = 0;


            //ScatterPictureBox.MouseWheel += ScatterPictureBox_MouseWheel;

            ECHisto.setPanel(AnHistogramPanel);
            ECHisto.tbreceived = tbreceived;
            ScatterHisto.setPanel(Histogrampanel1);
            ScatterHisto.tbreceived = tbreceived;
            ProcessingTab.Enabled = false;
            PeriodBeyond8uscomboBox.SelectedIndex = 0;

            ChangeDiskTypeComboBox.Items.AddRange(Enum.GetNames(typeof(DiskFormat)));
            QChangeDiskTypeComboBox.Items.AddRange(Enum.GetNames(typeof(DiskFormat)));
            ProcessingModeComboBox.Items.AddRange(Enum.GetNames(typeof(ProcessingType)));
            ProcessingModeComboBox.SelectedItem = ProcessingType.adaptive1.ToString();

            QProcessingModeComboBox.Items.AddRange(Enum.GetNames(typeof(ProcessingType)));
            QProcessingModeComboBox.SelectedItem = ProcessingType.adaptive1.ToString();

            ScanComboBox.Items.AddRange(Enum.GetNames(typeof(ScanMode)));
            ScanComboBox.SelectedItem = ScanMode.AdaptiveDeep.ToString();

            QScanComboBox.Items.AddRange(Enum.GetNames(typeof(ScanMode)));
            QScanComboBox.SelectedItem = ScanMode.AdaptiveDeep.ToString();


            QMinUpDown.Value = MinvScrollBar.Value;
            QFourSixUpDown.Value = (int)Properties.Settings.Default["FourSix"];
            QSixEightUpDown.Value = (int)Properties.Settings.Default["SixEight"];
            QMaxUpDown.Value = (int)Properties.Settings.Default["Max"];
            QOffsetUpDown.Value = (int)Properties.Settings.Default["Offset"];

            setThresholdLabels(processing.procsettings.processingtype);

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
            HandleTabSwitching();
            this.ActiveControl = QMinUpDown;
        }

        // Callbacks
        #region callbacks
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
            CurrentTrackLabel.Text = controlfloppy.CurrentTrack.ToString("F2");
            updateAllGraphs();

        }

        void FilesAvailableCallback()
        {
            openFilesDlgUsed = true;
        }

        void ScpFilesAvailableCallback()
        {
            ScpOpenFilesDlgUsed = true;
        }
        #endregion


        private void KeyboardShortcutHandler(KeyEventArgs e)
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
                    ProcessAmiga();
                    processing.sectormap.RefreshSectorMap();
                }
                if (e.KeyCode == Keys.P)
                {
                    RateOfChange2UpDown.Focus();
                    Application.DoEvents();
                    ProcessPC();
                    processing.sectormap.RefreshSectorMap();
                }
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

        // Updates the labels under the sliders
        // as well as the indicators under the histogram
        #region Update form elements
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

        private void updateHistoAndSliders()
        {
            if (!LimitTSCheckBox.Checked)
            {
                //createhistogram();
                updateSliderLabels();
            }
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (controlfloppy.capturecommand == 1 || processing.processing == 1)
                capturetime += timer1.Interval / 1000f;
            // bytes per second
            // and total bytes received

            if (controlfloppy.capturecommand == 1)
            {
                bytesReceived += controlfloppy.bytespersecond;
                BytesReceivedLabel.Text = string.Format("{0:n0}", bytesReceived);
                BytesPerSecondLabel.Text = string.Format("{0:n0}", controlfloppy.bytespersecond / ((double)timer1.Interval / 1000.0));
                CaptureTimeLabel.Text = ((int)capturetime).ToString();
                controlfloppy.bytespersecond = 0;
                BufferSizeLabel.Text = string.Format("{0:n0}", processing.indexrxbuf);

                indexrxbufprevious = processing.rxbuf.Length;
                //processing.rxbuf = controlfloppy.tempbuffer.Skip(Math.Max(0, controlfloppy.tempbuffer.Count()-30)).SelectMany(a => a).ToArray();

                controlfloppy.rxbuf = processing.rxbuf;
                if (processing.rxbuf.Length > 100000)
                    controlfloppy.recentreadbuflength = 100000; // controlfloppy.recentreadbuflength = processing.indexrxbuf - indexrxbufprevious;
                processing.indexrxbuf = processing.rxbuf.Length - 1;
                ControlFloppyScatterplotCallback();
            }

            if (processing.indexrxbuf > 0)
            {
                ProcessingTab.Enabled = true;
                QProcessingGroupBox.Enabled = true;
            }

            if (openFilesDlgUsed == true)
            {
                openFilesDlgUsed = false;
                fileio.openfiles();
                QProcessingGroupBox.Enabled = true;
                UpdateHistoAndScatterplot();
                BytesReceivedLabel.Text = String.Format("{0:n0}", processing.indexrxbuf);
                //createhistogram1();
            }

            if (ScpOpenFilesDlgUsed == true)
            {
                ScpOpenFilesDlgUsed = false;
                QProcessingGroupBox.Enabled = true;
                UpdateHistoAndScatterplot();
                BytesReceivedLabel.Text = String.Format("{0:n0}", processing.indexrxbuf);
                //createhistogram1();
            }
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
            QProcessingGroupBox.Enabled = false;
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
            UpdateHistoAndScatterplot();
            BytesReceivedLabel.Text = String.Format("{0:n0}", processing.indexrxbuf);
            GC.Collect();
        }

        private void resetoutput()
        {
            HandleTabSwitching();
            var oldscrollvalue = HistogramhScrollBar1.Value;
            var oldscrollmaxvalue = HistogramhScrollBar1.Maximum;
            var qoldscrollvalue = QHistogramhScrollBar1.Value;
            var qoldscrollmaxvalue = QHistogramhScrollBar1.Maximum;
            var rxbuftemp = (byte[])processing.rxbuf.Clone();
            for (var i = 0; i < processing.mfms.Length; i++)
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
            processing.indexrxbuf = processing.rxbuf.Length / 2; // Divide by two as loading .bin files doubles the buffer
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



    }
}
