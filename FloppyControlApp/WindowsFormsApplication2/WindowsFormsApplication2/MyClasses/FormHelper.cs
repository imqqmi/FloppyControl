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
        public  void InitializeFloppyControl()
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
    }
}
