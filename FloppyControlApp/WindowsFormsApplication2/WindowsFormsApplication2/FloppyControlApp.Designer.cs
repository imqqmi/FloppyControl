﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace FloppyControlApp
{
    partial class FloppyControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (scope != null)
            {
                try
                {
                    scope.Disconnect();
                }
                catch (FileNotFoundException e)
                {
                    // National instruments wasn't loaded, ignore the exception.
                    Trace.TraceInformation(e.ToString());
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    Trace.TraceInformation(e.ToString());
                }
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FloppyControl));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.outputfilename = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.tbSectorMap = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BytesPerSecondLabel = new System.Windows.Forms.Label();
            this.BytesReceivedLabel = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.CurrentTrackLabel = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.RecoveredSectorsLabel = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.RecoveredSectorsWithErrorsLabel = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ScatterPlottabPage = new System.Windows.Forms.TabPage();
            this.label81 = new System.Windows.Forms.Label();
            this.JumpTocomboBox = new System.Windows.Forms.ComboBox();
            this.label58 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.ScatterPictureBox = new System.Windows.Forms.PictureBox();
            this.label56 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.TrackInfotextBox = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textBoxFilesLoaded = new System.Windows.Forms.TextBox();
            this.ShowSectorTab = new System.Windows.Forms.TabPage();
            this.textBoxSector = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.LabelStatus = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxReceived = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.hlabel = new System.Windows.Forms.Label();
            this.wlabel = new System.Windows.Forms.Label();
            this.statsLabel = new System.Windows.Forms.Label();
            this.label72 = new System.Windows.Forms.Label();
            this.BadSectorsCntLabel = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.MarkersLabel = new System.Windows.Forms.Label();
            this.GoodHdrCntLabel = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.CaptureTimeLabel = new System.Windows.Forms.Label();
            this.DiskTypeLabel = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.LimitToSectorUpDown = new System.Windows.Forms.NumericUpDown();
            this.label41 = new System.Windows.Forms.Label();
            this.LimitToTrackUpDown = new System.Windows.Forms.NumericUpDown();
            this.label42 = new System.Windows.Forms.Label();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.QuickTab = new System.Windows.Forms.TabPage();
            this.QrtbSectorMap = new System.Windows.Forms.RichTextBox();
            this.QProcessingGroupBox = new System.Windows.Forms.GroupBox();
            this.button11 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.QOnlyBadSectorsRadio = new System.Windows.Forms.RadioButton();
            this.QECOnRadio = new System.Windows.Forms.RadioButton();
            this.label89 = new System.Windows.Forms.Label();
            this.QChangeDiskTypeComboBox = new System.Windows.Forms.ComboBox();
            this.QProcessingModeComboBox = new System.Windows.Forms.ComboBox();
            this.QClearDatacheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.QOffsetUpDown = new System.Windows.Forms.NumericUpDown();
            this.QMaxUpDown = new System.Windows.Forms.NumericUpDown();
            this.QSixEightUpDown = new System.Windows.Forms.NumericUpDown();
            this.QFourSixUpDown = new System.Windows.Forms.NumericUpDown();
            this.QMinUpDown = new System.Windows.Forms.NumericUpDown();
            this.QOffsetLabel = new System.Windows.Forms.Label();
            this.QFourSixLabel = new System.Windows.Forms.Label();
            this.QMinLabel = new System.Windows.Forms.Label();
            this.QSixEightLabel = new System.Windows.Forms.Label();
            this.QMaxLabel = new System.Windows.Forms.Label();
            this.QFindDupesCheckBox = new System.Windows.Forms.CheckBox();
            this.QAutoRefreshSectorMapCheck = new System.Windows.Forms.CheckBox();
            this.QIgnoreHeaderErrorCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.QHistoPanel = new System.Windows.Forms.Panel();
            this.QHistogramhScrollBar1 = new System.Windows.Forms.TrackBar();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.QLimitTSCheckBox = new System.Windows.Forms.CheckBox();
            this.QRateOfChange2UpDown = new System.Windows.Forms.NumericUpDown();
            this.label88 = new System.Windows.Forms.Label();
            this.label102 = new System.Windows.Forms.Label();
            this.QRateOfChangeUpDown = new System.Windows.Forms.NumericUpDown();
            this.QScanComboBox = new System.Windows.Forms.ComboBox();
            this.button47 = new System.Windows.Forms.Button();
            this.QAdaptOfsset2UpDown = new System.Windows.Forms.NumericUpDown();
            this.label90 = new System.Windows.Forms.Label();
            this.QLimitToSectorUpDown = new System.Windows.Forms.NumericUpDown();
            this.label91 = new System.Windows.Forms.Label();
            this.QLimitToTrackUpDown = new System.Windows.Forms.NumericUpDown();
            this.QHDCheckBox = new System.Windows.Forms.CheckBox();
            this.label114 = new System.Windows.Forms.Label();
            this.QProcAmigaBtn = new System.Windows.Forms.Button();
            this.QProcPCBtn = new System.Windows.Forms.Button();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.GluedDiskPreset = new System.Windows.Forms.Button();
            this.StepStickPresetBtn = new System.Windows.Forms.Button();
            this.QDirectStepPresetBtn = new System.Windows.Forms.Button();
            this.QRecaptureAllBtn = new System.Windows.Forms.Button();
            this.MainTabControlImageList = new System.Windows.Forms.ImageList(this.components);
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.QDirectStepCheckBox = new System.Windows.Forms.CheckBox();
            this.QCaptureBtn = new System.Windows.Forms.Button();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.PresetCaptureDuration50s = new System.Windows.Forms.Button();
            this.PresetCaptureDuration2s = new System.Windows.Forms.Button();
            this.PresetCaptureDuration1s = new System.Windows.Forms.Button();
            this.PresetCaptureDuration5s = new System.Windows.Forms.Button();
            this.PresetTrack78_164 = new System.Windows.Forms.Button();
            this.PresetTrack80_90 = new System.Windows.Forms.Button();
            this.PresetCaptureDefaultBtn = new System.Windows.Forms.Button();
            this.button37 = new System.Windows.Forms.Button();
            this.QTrackDurationUpDown = new System.Windows.Forms.NumericUpDown();
            this.QStartTrackUpDown = new System.Windows.Forms.NumericUpDown();
            this.QEndTracksUpDown = new System.Windows.Forms.NumericUpDown();
            this.label82 = new System.Windows.Forms.Label();
            this.QTRK00OffsetUpDown = new System.Windows.Forms.NumericUpDown();
            this.QMicrostepsPerTrackUpDown = new System.Windows.Forms.NumericUpDown();
            this.label83 = new System.Windows.Forms.Label();
            this.label84 = new System.Windows.Forms.Label();
            this.label85 = new System.Windows.Forms.Label();
            this.label86 = new System.Windows.Forms.Label();
            this.CaptureTab = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rxbufEndUpDown = new System.Windows.Forms.NumericUpDown();
            this.rxbufStartUpDown = new System.Windows.Forms.NumericUpDown();
            this.BufferSizeLabel = new System.Windows.Forms.Label();
            this.HistogramLengthLabel = new System.Windows.Forms.Label();
            this.HistogramStartLabel = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.SaveTrimmedBadbutton = new System.Windows.Forms.Button();
            this.button49 = new System.Windows.Forms.Button();
            this.button48 = new System.Windows.Forms.Button();
            this.button46 = new System.Windows.Forms.Button();
            this.button45 = new System.Windows.Forms.Button();
            this.button40 = new System.Windows.Forms.Button();
            this.button39 = new System.Windows.Forms.Button();
            this.button36 = new System.Windows.Forms.Button();
            this.CaptureClassbutton = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.TrackPreset4Button = new System.Windows.Forms.Button();
            this.TrackPreset2Button = new System.Windows.Forms.Button();
            this.TrackPreset3Button = new System.Windows.Forms.Button();
            this.TrackPreset1Button = new System.Windows.Forms.Button();
            this.TrackDurationUpDown = new System.Windows.Forms.NumericUpDown();
            this.StartTrackUpDown = new System.Windows.Forms.NumericUpDown();
            this.EndTracksUpDown = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.TRK00OffsetUpDown = new System.Windows.Forms.NumericUpDown();
            this.MicrostepsPerTrackUpDown = new System.Windows.Forms.NumericUpDown();
            this.label38 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.DirectStepCheckBox = new System.Windows.Forms.CheckBox();
            this.ProcessingTab = new System.Windows.Forms.TabPage();
            this.button5 = new System.Windows.Forms.Button();
            this.ResetBuffersBtn = new System.Windows.Forms.Button();
            this.DupsUpDown = new System.Windows.Forms.NumericUpDown();
            this.label69 = new System.Windows.Forms.Label();
            this.AddNoiseKnumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label68 = new System.Windows.Forms.Label();
            this.rtbSectorMap = new System.Windows.Forms.RichTextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.jESEnd = new System.Windows.Forms.NumericUpDown();
            this.jESStart = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.SettingsLabel = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.iESEnd = new System.Windows.Forms.NumericUpDown();
            this.iESStart = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.FullHistBtn = new System.Windows.Forms.Button();
            this.OnlyBadSectorsRadio = new System.Windows.Forms.RadioButton();
            this.ECOnRadio = new System.Windows.Forms.RadioButton();
            this.label78 = new System.Windows.Forms.Label();
            this.ChangeDiskTypeComboBox = new System.Windows.Forms.ComboBox();
            this.ProcessingModeComboBox = new System.Windows.Forms.ComboBox();
            this.ClearDatacheckBox = new System.Windows.Forms.CheckBox();
            this.LimitTSCheckBox = new System.Windows.Forms.CheckBox();
            this.RateOfChange2UpDown = new System.Windows.Forms.NumericUpDown();
            this.AdaptOfsset2UpDown = new System.Windows.Forms.NumericUpDown();
            this.label74 = new System.Windows.Forms.Label();
            this.PeriodBeyond8uscomboBox = new System.Windows.Forms.ComboBox();
            this.RndAmountUpDown = new System.Windows.Forms.NumericUpDown();
            this.label67 = new System.Windows.Forms.Label();
            this.LimitToScttrViewcheckBox = new System.Windows.Forms.CheckBox();
            this.AddNoisecheckBox = new System.Windows.Forms.CheckBox();
            this.ThresholdsGroupBox = new System.Windows.Forms.GroupBox();
            this.MinvScrollBar = new System.Windows.Forms.VScrollBar();
            this.FourLabel = new System.Windows.Forms.Label();
            this.MinLabel = new System.Windows.Forms.Label();
            this.SixLabel = new System.Windows.Forms.Label();
            this.EightLabel = new System.Windows.Forms.Label();
            this.Offsetlabel = new System.Windows.Forms.Label();
            this.FourvScrollBar = new System.Windows.Forms.VScrollBar();
            this.SixvScrollBar = new System.Windows.Forms.VScrollBar();
            this.EightvScrollBar = new System.Windows.Forms.VScrollBar();
            this.OffsetvScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.POffsetLabel = new System.Windows.Forms.Label();
            this.PFourSixLabel = new System.Windows.Forms.Label();
            this.PMinLabel = new System.Windows.Forms.Label();
            this.PSixEightLabel = new System.Windows.Forms.Label();
            this.PMaxLabel = new System.Windows.Forms.Label();
            this.FindDupesCheckBox = new System.Windows.Forms.CheckBox();
            this.AutoRefreshSectorMapCheck = new System.Windows.Forms.CheckBox();
            this.label50 = new System.Windows.Forms.Label();
            this.RateOfChangeUpDown = new System.Windows.Forms.NumericUpDown();
            this.IgnoreHeaderErrorCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.Histogrampanel1 = new System.Windows.Forms.Panel();
            this.HistogramhScrollBar1 = new System.Windows.Forms.TrackBar();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ScanComboBox = new System.Windows.Forms.ComboBox();
            this.ScanButton = new System.Windows.Forms.Button();
            this.HDCheckBox = new System.Windows.Forms.CheckBox();
            this.label37 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ProcessBtn = new System.Windows.Forms.Button();
            this.ProcessPCBtn = new System.Windows.Forms.Button();
            this.ErrorCorrectionTab = new System.Windows.Forms.TabPage();
            this.CombinationsUpDown = new System.Windows.Forms.NumericUpDown();
            this.label79 = new System.Windows.Forms.Label();
            this.button44 = new System.Windows.Forms.Button();
            this.ECMFMByteEncbutton = new System.Windows.Forms.Button();
            this.MFMByteLengthUpDown = new System.Windows.Forms.NumericUpDown();
            this.MFMByteStartUpDown = new System.Windows.Forms.NumericUpDown();
            this.label76 = new System.Windows.Forms.Label();
            this.label77 = new System.Windows.Forms.Label();
            this.button38 = new System.Windows.Forms.Button();
            this.ECMFMcheckBox = new System.Windows.Forms.CheckBox();
            this.label71 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.C8StartUpDown = new System.Windows.Forms.NumericUpDown();
            this.C6StartUpDown = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.BadSectorsCheckBox = new System.Windows.Forms.CheckBox();
            this.GoodSectorsCheckBox = new System.Windows.Forms.CheckBox();
            this.ECRealign4E = new System.Windows.Forms.Button();
            this.ECInfoTabs = new System.Windows.Forms.TabControl();
            this.ECTabSectorData = new System.Windows.Forms.TabPage();
            this.antbSectorData = new System.Windows.Forms.TextBox();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.ECtbMFM = new System.Windows.Forms.TextBox();
            this.ECZoomOutBtn = new System.Windows.Forms.Button();
            this.SelectionDifLabel = new System.Windows.Forms.Label();
            this.ScatterOffsetUpDown = new System.Windows.Forms.NumericUpDown();
            this.ScatterMinUpDown = new System.Windows.Forms.NumericUpDown();
            this.ScatterMaxUpDown = new System.Windows.Forms.NumericUpDown();
            this.ScatterMaxTrackBar = new System.Windows.Forms.TrackBar();
            this.ScatterMinTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.HistScalingLabel = new System.Windows.Forms.Label();
            this.AnHistogramPanel = new System.Windows.Forms.Panel();
            this.RedCrcCheckLabel = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.BSEditByteLabel = new System.Windows.Forms.Label();
            this.BluetoRedByteCopyToolBtn = new System.Windows.Forms.Button();
            this.CopySectorToBlueBtn = new System.Windows.Forms.Button();
            this.label55 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.BSRedTempRadio = new System.Windows.Forms.RadioButton();
            this.BSRedFromlistRadio = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.BlueTempRadio = new System.Windows.Forms.RadioButton();
            this.BSBlueFromListRadio = new System.Windows.Forms.RadioButton();
            this.BSBlueSectormapRadio = new System.Windows.Forms.RadioButton();
            this.label54 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.BadSectorListBox = new System.Windows.Forms.ListBox();
            this.Sector2UpDown = new System.Windows.Forms.NumericUpDown();
            this.Track2UpDown = new System.Windows.Forms.NumericUpDown();
            this.label48 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.Sector1UpDown = new System.Windows.Forms.NumericUpDown();
            this.Track1UpDown = new System.Windows.Forms.NumericUpDown();
            this.BlueCrcCheckLabel = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.ECSectorOverlayBtn = new System.Windows.Forms.Button();
            this.BadSectorPanel = new System.Windows.Forms.Panel();
            this.BadSectorTooltip = new System.Windows.Forms.Label();
            this.ScatterOffsetTrackBar = new System.Windows.Forms.TrackBar();
            this.AnalysisPage = new System.Windows.Forms.TabPage();
            this.button20 = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.AmigaMFMRadio = new System.Windows.Forms.RadioButton();
            this.ANAmigaDiskSpareRadio = new System.Windows.Forms.RadioButton();
            this.ANAmigaRadio = new System.Windows.Forms.RadioButton();
            this.ANPCRadio = new System.Windows.Forms.RadioButton();
            this.AntxtBox = new System.Windows.Forms.TextBox();
            this.button25 = new System.Windows.Forms.Button();
            this.button26 = new System.Windows.Forms.Button();
            this.button23 = new System.Windows.Forms.Button();
            this.button21 = new System.Windows.Forms.Button();
            this.tbMFM = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.ConvertToMFMBtn = new System.Windows.Forms.Button();
            this.tbBIN = new System.Windows.Forms.TextBox();
            this.tbTest = new System.Windows.Forms.TextBox();
            this.AnalysisTab2 = new System.Windows.Forms.TabPage();
            this.rxbufOffsetLabel = new System.Windows.Forms.Label();
            this.label80 = new System.Windows.Forms.Label();
            this.ThresholdTestUpDown = new System.Windows.Forms.NumericUpDown();
            this.DiffTest2UpDown = new System.Windows.Forms.NumericUpDown();
            this.DiffTestUpDown = new System.Windows.Forms.NumericUpDown();
            this.button34 = new System.Windows.Forms.Button();
            this.AnAutoUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.button31 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label73 = new System.Windows.Forms.Label();
            this.PeriodExtendUpDown = new System.Windows.Forms.NumericUpDown();
            this.EditOptioncomboBox = new System.Windows.Forms.ComboBox();
            this.EditModecomboBox = new System.Windows.Forms.ComboBox();
            this.HighpassThresholdUpDown = new System.Windows.Forms.NumericUpDown();
            this.button33 = new System.Windows.Forms.Button();
            this.Undolevelslabel = new System.Windows.Forms.Label();
            this.Lowpassbutton = new System.Windows.Forms.Button();
            this.DCOffsetbutton = new System.Windows.Forms.Button();
            this.label70 = new System.Windows.Forms.Label();
            this.button32 = new System.Windows.Forms.Button();
            this.SaveWaveformButton = new System.Windows.Forms.Button();
            this.EditUndobutton = new System.Windows.Forms.Button();
            this.AdaptLookAheadUpDown = new System.Windows.Forms.NumericUpDown();
            this.DiffMinDeviation2UpDown = new System.Windows.Forms.NumericUpDown();
            this.button18 = new System.Windows.Forms.Button();
            this.DiffOffsetUpDown = new System.Windows.Forms.NumericUpDown();
            this.InvertcheckBox = new System.Windows.Forms.CheckBox();
            this.AdaptiveGaincheckBox = new System.Windows.Forms.CheckBox();
            this.SignalRatioDistUpDown = new System.Windows.Forms.NumericUpDown();
            this.DiffMinDeviationUpDown = new System.Windows.Forms.NumericUpDown();
            this.SmoothingUpDown = new System.Windows.Forms.NumericUpDown();
            this.AnDensityUpDown = new System.Windows.Forms.NumericUpDown();
            this.AnReplacerxbufBox = new System.Windows.Forms.CheckBox();
            this.button19 = new System.Windows.Forms.Button();
            this.DiffDistUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label62 = new System.Windows.Forms.Label();
            this.DiffThresholdUpDown = new System.Windows.Forms.NumericUpDown();
            this.label61 = new System.Windows.Forms.Label();
            this.DiffGainUpDown = new System.Windows.Forms.NumericUpDown();
            this.label60 = new System.Windows.Forms.Label();
            this.DiffDistUpDown = new System.Windows.Forms.NumericUpDown();
            this.label52 = new System.Windows.Forms.Label();
            this.GraphFilterButton = new System.Windows.Forms.Button();
            this.GraphLengthLabel = new System.Windows.Forms.Label();
            this.GraphXOffsetLabel = new System.Windows.Forms.Label();
            this.GraphYOffsetlabel = new System.Windows.Forms.Label();
            this.GraphScaleYLabel = new System.Windows.Forms.Label();
            this.OpenWavefrmbutton = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.Graph5SelRadioButton = new System.Windows.Forms.RadioButton();
            this.Graph4SelRadioButton = new System.Windows.Forms.RadioButton();
            this.Graph3SelRadioButton = new System.Windows.Forms.RadioButton();
            this.Graph2SelRadioButton = new System.Windows.Forms.RadioButton();
            this.Graph1SelRadioButton = new System.Windows.Forms.RadioButton();
            this.label19 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.GraphYScaleTrackBar = new System.Windows.Forms.TrackBar();
            this.GraphOffsetTrackBar = new System.Windows.Forms.TrackBar();
            this.GraphPictureBox = new System.Windows.Forms.PictureBox();
            this.NetworkTab = new System.Windows.Forms.TabPage();
            this.button41 = new System.Windows.Forms.Button();
            this.button42 = new System.Windows.Forms.Button();
            this.button43 = new System.Windows.Forms.Button();
            this.xscalemvUpDown = new System.Windows.Forms.NumericUpDown();
            this.label75 = new System.Windows.Forms.Label();
            this.button35 = new System.Windows.Forms.Button();
            this.NetworkUseAveragingCheckBox = new System.Windows.Forms.CheckBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.NetworkDoAllBad = new System.Windows.Forms.RadioButton();
            this.NetCaptureRangecheckBox = new System.Windows.Forms.RadioButton();
            this.label66 = new System.Windows.Forms.Label();
            this.label65 = new System.Windows.Forms.Label();
            this.NetworkCaptureTrackEndUpDown = new System.Windows.Forms.NumericUpDown();
            this.NumberOfPointsUpDown = new System.Windows.Forms.NumericUpDown();
            this.label64 = new System.Windows.Forms.Label();
            this.NetworkCaptureTrackStartUpDown = new System.Windows.Forms.NumericUpDown();
            this.label63 = new System.Windows.Forms.Label();
            this.button29 = new System.Windows.Forms.Button();
            this.button28 = new System.Windows.Forms.Button();
            this.SectorUpDown = new System.Windows.Forms.NumericUpDown();
            this.TrackUpDown = new System.Windows.Forms.NumericUpDown();
            this.label39 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.ProcessStatusLabel = new System.Windows.Forms.Label();
            this.GUITimer = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.GCbutton = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.EditScatterPlotcheckBox = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.loadProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scpFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scpFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.trimmedBinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.badSectorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableTooltipsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.basicModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ThreadsUpDown = new System.Windows.Forms.NumericUpDown();
            this.label59 = new System.Windows.Forms.Label();
            this.StopButton = new System.Windows.Forms.Button();
            this.ExploreHereBtn = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.ScatterPlottabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterPictureBox)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.ShowSectorTab.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LimitToSectorUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LimitToTrackUpDown)).BeginInit();
            this.MainTabControl.SuspendLayout();
            this.QuickTab.SuspendLayout();
            this.QProcessingGroupBox.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.groupBox15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.QOffsetUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QMaxUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QSixEightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QFourSixUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QMinUpDown)).BeginInit();
            this.groupBox16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.QHistogramhScrollBar1)).BeginInit();
            this.groupBox17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.QRateOfChange2UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QRateOfChangeUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QAdaptOfsset2UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QLimitToSectorUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QLimitToTrackUpDown)).BeginInit();
            this.groupBox12.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.QTrackDurationUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QStartTrackUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QEndTracksUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QTRK00OffsetUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QMicrostepsPerTrackUpDown)).BeginInit();
            this.CaptureTab.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rxbufEndUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rxbufStartUpDown)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackDurationUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartTrackUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EndTracksUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TRK00OffsetUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MicrostepsPerTrackUpDown)).BeginInit();
            this.ProcessingTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DupsUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AddNoiseKnumericUpDown)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.jESEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jESStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iESEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iESStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RateOfChange2UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AdaptOfsset2UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RndAmountUpDown)).BeginInit();
            this.ThresholdsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RateOfChangeUpDown)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HistogramhScrollBar1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.ErrorCorrectionTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CombinationsUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MFMByteLengthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MFMByteStartUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.C8StartUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.C6StartUpDown)).BeginInit();
            this.ECInfoTabs.SuspendLayout();
            this.ECTabSectorData.SuspendLayout();
            this.tabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterOffsetUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterMinUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterMaxUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterMaxTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterMinTrackBar)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Sector2UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Track2UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Sector1UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Track1UpDown)).BeginInit();
            this.BadSectorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterOffsetTrackBar)).BeginInit();
            this.AnalysisPage.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.AnalysisTab2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ThresholdTestUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffTest2UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffTestUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PeriodExtendUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HighpassThresholdUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AdaptLookAheadUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffMinDeviation2UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffOffsetUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SignalRatioDistUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffMinDeviationUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SmoothingUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnDensityUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffDistUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffThresholdUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffGainUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffDistUpDown)).BeginInit();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GraphYScaleTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GraphOffsetTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GraphPictureBox)).BeginInit();
            this.NetworkTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xscalemvUpDown)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NetworkCaptureTrackEndUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumberOfPointsUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NetworkCaptureTrackStartUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SectorUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackUpDown)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ThreadsUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Multiselect = true;
            // 
            // outputfilename
            // 
            this.outputfilename.Location = new System.Drawing.Point(90, 28);
            this.outputfilename.Name = "outputfilename";
            this.outputfilename.Size = new System.Drawing.Size(269, 20);
            this.outputfilename.TabIndex = 1;
            this.outputfilename.Text = "Dump";
            this.toolTip1.SetToolTip(this.outputfilename, "Before capturing, set a name for the folder and files to be generated during capt" +
        "ure and saving/exporting data.");
            this.outputfilename.TextChanged += new System.EventHandler(this.outputfilename_TextChanged);
            this.outputfilename.Enter += new System.EventHandler(this.outputfilename_Enter);
            this.outputfilename.Leave += new System.EventHandler(this.outputfilename_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Base filename";
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // tbSectorMap
            // 
            this.tbSectorMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSectorMap.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSectorMap.Location = new System.Drawing.Point(0, 0);
            this.tbSectorMap.Multiline = true;
            this.tbSectorMap.Name = "tbSectorMap";
            this.tbSectorMap.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbSectorMap.Size = new System.Drawing.Size(597, 363);
            this.tbSectorMap.TabIndex = 18;
            this.tbSectorMap.Tag = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 357);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Sector map (output)";
            // 
            // BytesPerSecondLabel
            // 
            this.BytesPerSecondLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BytesPerSecondLabel.AutoSize = true;
            this.BytesPerSecondLabel.Location = new System.Drawing.Point(624, 26);
            this.BytesPerSecondLabel.Name = "BytesPerSecondLabel";
            this.BytesPerSecondLabel.Size = new System.Drawing.Size(13, 13);
            this.BytesPerSecondLabel.TabIndex = 1;
            this.BytesPerSecondLabel.Text = "0";
            // 
            // BytesReceivedLabel
            // 
            this.BytesReceivedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BytesReceivedLabel.AutoSize = true;
            this.BytesReceivedLabel.Location = new System.Drawing.Point(624, 13);
            this.BytesReceivedLabel.Name = "BytesReceivedLabel";
            this.BytesReceivedLabel.Size = new System.Drawing.Size(13, 13);
            this.BytesReceivedLabel.TabIndex = 1;
            this.BytesReceivedLabel.Text = "0";
            // 
            // label23
            // 
            this.label23.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(554, 13);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(66, 13);
            this.label23.TabIndex = 27;
            this.label23.Text = "Bytes recvd:";
            // 
            // label24
            // 
            this.label24.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(546, 26);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(74, 13);
            this.label24.TabIndex = 27;
            this.label24.Text = "Bytes per sec:";
            // 
            // label25
            // 
            this.label25.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(424, 13);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(38, 13);
            this.label25.TabIndex = 27;
            this.label25.Text = "Track:";
            // 
            // CurrentTrackLabel
            // 
            this.CurrentTrackLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentTrackLabel.AutoSize = true;
            this.CurrentTrackLabel.Location = new System.Drawing.Point(460, 13);
            this.CurrentTrackLabel.Name = "CurrentTrackLabel";
            this.CurrentTrackLabel.Size = new System.Drawing.Size(19, 13);
            this.CurrentTrackLabel.TabIndex = 27;
            this.CurrentTrackLabel.Text = "00";
            // 
            // label26
            // 
            this.label26.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(360, 26);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(102, 13);
            this.label26.TabIndex = 27;
            this.label26.Text = "Recovered Sectors:";
            // 
            // RecoveredSectorsLabel
            // 
            this.RecoveredSectorsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RecoveredSectorsLabel.AutoSize = true;
            this.RecoveredSectorsLabel.Location = new System.Drawing.Point(460, 26);
            this.RecoveredSectorsLabel.Name = "RecoveredSectorsLabel";
            this.RecoveredSectorsLabel.Size = new System.Drawing.Size(13, 13);
            this.RecoveredSectorsLabel.TabIndex = 27;
            this.RecoveredSectorsLabel.Text = "0";
            // 
            // label33
            // 
            this.label33.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(138, 26);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(148, 13);
            this.label33.TabIndex = 27;
            this.label33.Text = "Recovered Sectors with error:";
            // 
            // RecoveredSectorsWithErrorsLabel
            // 
            this.RecoveredSectorsWithErrorsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RecoveredSectorsWithErrorsLabel.AutoSize = true;
            this.RecoveredSectorsWithErrorsLabel.Location = new System.Drawing.Point(292, 26);
            this.RecoveredSectorsWithErrorsLabel.Name = "RecoveredSectorsWithErrorsLabel";
            this.RecoveredSectorsWithErrorsLabel.Size = new System.Drawing.Size(13, 13);
            this.RecoveredSectorsWithErrorsLabel.TabIndex = 27;
            this.RecoveredSectorsWithErrorsLabel.Text = "0";
            // 
            // label34
            // 
            this.label34.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(232, 13);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(54, 13);
            this.label34.TabIndex = 27;
            this.label34.Text = "Disk type:";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.ScatterPlottabPage);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.ShowSectorTab);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(991, 393);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(616, 473);
            this.tabControl1.TabIndex = 42;
            // 
            // ScatterPlottabPage
            // 
            this.ScatterPlottabPage.Controls.Add(this.label81);
            this.ScatterPlottabPage.Controls.Add(this.JumpTocomboBox);
            this.ScatterPlottabPage.Controls.Add(this.label58);
            this.ScatterPlottabPage.Controls.Add(this.label57);
            this.ScatterPlottabPage.Controls.Add(this.ScatterPictureBox);
            this.ScatterPlottabPage.Controls.Add(this.label56);
            this.ScatterPlottabPage.Location = new System.Drawing.Point(4, 22);
            this.ScatterPlottabPage.Name = "ScatterPlottabPage";
            this.ScatterPlottabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ScatterPlottabPage.Size = new System.Drawing.Size(608, 447);
            this.ScatterPlottabPage.TabIndex = 2;
            this.ScatterPlottabPage.Text = "Scatter plot";
            this.ScatterPlottabPage.UseVisualStyleBackColor = true;
            // 
            // label81
            // 
            this.label81.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label81.AutoSize = true;
            this.label81.Location = new System.Drawing.Point(-2, 194);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(32, 13);
            this.label81.TabIndex = 98;
            this.label81.Text = "Entro";
            // 
            // JumpTocomboBox
            // 
            this.JumpTocomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.JumpTocomboBox.FormattingEnabled = true;
            this.JumpTocomboBox.Location = new System.Drawing.Point(32, 392);
            this.JumpTocomboBox.Name = "JumpTocomboBox";
            this.JumpTocomboBox.Size = new System.Drawing.Size(181, 21);
            this.JumpTocomboBox.TabIndex = 97;
            this.JumpTocomboBox.SelectedIndexChanged += new System.EventHandler(this.JumpTocomboBox_SelectedIndexChanged);
            // 
            // label58
            // 
            this.label58.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(6, 142);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(24, 13);
            this.label58.TabIndex = 86;
            this.label58.Text = "8us";
            // 
            // label57
            // 
            this.label57.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(6, 100);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(24, 13);
            this.label57.TabIndex = 85;
            this.label57.Text = "6us";
            // 
            // ScatterPictureBox
            // 
            this.ScatterPictureBox.Location = new System.Drawing.Point(32, 8);
            this.ScatterPictureBox.Name = "ScatterPictureBox";
            this.ScatterPictureBox.Size = new System.Drawing.Size(573, 378);
            this.ScatterPictureBox.TabIndex = 1;
            this.ScatterPictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.ScatterPictureBox, "The scatter plot shows you the signal, with pulse length on the vertical axis and" +
        " datapoints/time on the horizontal axis. \r\nZoom with scroll wheel, drag to move " +
        "the view.");
            // 
            // label56
            // 
            this.label56.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(6, 62);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(24, 13);
            this.label56.TabIndex = 84;
            this.label56.Text = "4us";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.TrackInfotextBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(608, 447);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Track info";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // TrackInfotextBox
            // 
            this.TrackInfotextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrackInfotextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackInfotextBox.Location = new System.Drawing.Point(9, 6);
            this.TrackInfotextBox.MaxLength = 2000000;
            this.TrackInfotextBox.Multiline = true;
            this.TrackInfotextBox.Name = "TrackInfotextBox";
            this.TrackInfotextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TrackInfotextBox.Size = new System.Drawing.Size(581, 350);
            this.TrackInfotextBox.TabIndex = 21;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(608, 447);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Sector Data";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.textBoxFilesLoaded);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(608, 447);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Files";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // textBoxFilesLoaded
            // 
            this.textBoxFilesLoaded.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilesLoaded.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxFilesLoaded.Location = new System.Drawing.Point(0, 3);
            this.textBoxFilesLoaded.MaxLength = 200000;
            this.textBoxFilesLoaded.Multiline = true;
            this.textBoxFilesLoaded.Name = "textBoxFilesLoaded";
            this.textBoxFilesLoaded.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxFilesLoaded.Size = new System.Drawing.Size(590, 353);
            this.textBoxFilesLoaded.TabIndex = 57;
            // 
            // ShowSectorTab
            // 
            this.ShowSectorTab.Controls.Add(this.textBoxSector);
            this.ShowSectorTab.Location = new System.Drawing.Point(4, 22);
            this.ShowSectorTab.Name = "ShowSectorTab";
            this.ShowSectorTab.Padding = new System.Windows.Forms.Padding(3);
            this.ShowSectorTab.Size = new System.Drawing.Size(608, 447);
            this.ShowSectorTab.TabIndex = 4;
            this.ShowSectorTab.Text = "Sector";
            this.ShowSectorTab.UseVisualStyleBackColor = true;
            // 
            // textBoxSector
            // 
            this.textBoxSector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSector.Font = new System.Drawing.Font("Courier New", 8F);
            this.textBoxSector.Location = new System.Drawing.Point(1, 0);
            this.textBoxSector.MaxLength = 200000;
            this.textBoxSector.Multiline = true;
            this.textBoxSector.Name = "textBoxSector";
            this.textBoxSector.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSector.Size = new System.Drawing.Size(590, 428);
            this.textBoxSector.TabIndex = 58;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tbSectorMap);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(608, 447);
            this.tabPage4.TabIndex = 5;
            this.tabPage4.Text = "DebugInfo";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // LabelStatus
            // 
            this.LabelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelStatus.AutoSize = true;
            this.LabelStatus.Location = new System.Drawing.Point(42, 26);
            this.LabelStatus.Name = "LabelStatus";
            this.LabelStatus.Size = new System.Drawing.Size(73, 13);
            this.LabelStatus.TabIndex = 54;
            this.LabelStatus.Text = "Disconnected";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 13);
            this.label7.TabIndex = 53;
            this.label7.Text = "Status:";
            // 
            // textBoxReceived
            // 
            this.textBoxReceived.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxReceived.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxReceived.Location = new System.Drawing.Point(991, 6);
            this.textBoxReceived.MaxLength = 2000000;
            this.textBoxReceived.Multiline = true;
            this.textBoxReceived.Name = "textBoxReceived";
            this.textBoxReceived.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxReceived.Size = new System.Drawing.Size(616, 364);
            this.textBoxReceived.TabIndex = 56;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.hlabel);
            this.panel2.Controls.Add(this.wlabel);
            this.panel2.Controls.Add(this.statsLabel);
            this.panel2.Controls.Add(this.label72);
            this.panel2.Controls.Add(this.BadSectorsCntLabel);
            this.panel2.Controls.Add(this.label44);
            this.panel2.Controls.Add(this.MarkersLabel);
            this.panel2.Controls.Add(this.GoodHdrCntLabel);
            this.panel2.Controls.Add(this.label45);
            this.panel2.Controls.Add(this.label46);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.CaptureTimeLabel);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.BytesPerSecondLabel);
            this.panel2.Controls.Add(this.BytesReceivedLabel);
            this.panel2.Controls.Add(this.LabelStatus);
            this.panel2.Controls.Add(this.label23);
            this.panel2.Controls.Add(this.label25);
            this.panel2.Controls.Add(this.label26);
            this.panel2.Controls.Add(this.label33);
            this.panel2.Controls.Add(this.CurrentTrackLabel);
            this.panel2.Controls.Add(this.label34);
            this.panel2.Controls.Add(this.RecoveredSectorsLabel);
            this.panel2.Controls.Add(this.label24);
            this.panel2.Controls.Add(this.RecoveredSectorsWithErrorsLabel);
            this.panel2.Controls.Add(this.DiskTypeLabel);
            this.panel2.Location = new System.Drawing.Point(15, 862);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1193, 47);
            this.panel2.TabIndex = 57;
            
            // 
            // hlabel
            // 
            this.hlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hlabel.AutoSize = true;
            this.hlabel.Location = new System.Drawing.Point(1040, 26);
            this.hlabel.Name = "hlabel";
            this.hlabel.Size = new System.Drawing.Size(13, 13);
            this.hlabel.TabIndex = 66;
            this.hlabel.Text = "0";
            // 
            // wlabel
            // 
            this.wlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wlabel.AutoSize = true;
            this.wlabel.Location = new System.Drawing.Point(1039, 13);
            this.wlabel.Name = "wlabel";
            this.wlabel.Size = new System.Drawing.Size(13, 13);
            this.wlabel.TabIndex = 65;
            this.wlabel.Text = "0";
            // 
            // statsLabel
            // 
            this.statsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statsLabel.AutoSize = true;
            this.statsLabel.Location = new System.Drawing.Point(951, 26);
            this.statsLabel.Name = "statsLabel";
            this.statsLabel.Size = new System.Drawing.Size(13, 13);
            this.statsLabel.TabIndex = 63;
            this.statsLabel.Text = "0";
            // 
            // label72
            // 
            this.label72.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label72.AutoSize = true;
            this.label72.Location = new System.Drawing.Point(924, 26);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(29, 13);
            this.label72.TabIndex = 64;
            this.label72.Text = "Stat:";
            // 
            // BadSectorsCntLabel
            // 
            this.BadSectorsCntLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BadSectorsCntLabel.AutoSize = true;
            this.BadSectorsCntLabel.Location = new System.Drawing.Point(950, 13);
            this.BadSectorsCntLabel.Name = "BadSectorsCntLabel";
            this.BadSectorsCntLabel.Size = new System.Drawing.Size(13, 13);
            this.BadSectorsCntLabel.TabIndex = 61;
            this.BadSectorsCntLabel.Text = "0";
            // 
            // label44
            // 
            this.label44.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(874, 13);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(79, 13);
            this.label44.TabIndex = 62;
            this.label44.Text = "Bad sector cnt:";
            // 
            // MarkersLabel
            // 
            this.MarkersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MarkersLabel.AutoSize = true;
            this.MarkersLabel.Location = new System.Drawing.Point(828, 26);
            this.MarkersLabel.Name = "MarkersLabel";
            this.MarkersLabel.Size = new System.Drawing.Size(13, 13);
            this.MarkersLabel.TabIndex = 57;
            this.MarkersLabel.Text = "0";
            // 
            // GoodHdrCntLabel
            // 
            this.GoodHdrCntLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GoodHdrCntLabel.AutoSize = true;
            this.GoodHdrCntLabel.Location = new System.Drawing.Point(828, 13);
            this.GoodHdrCntLabel.Name = "GoodHdrCntLabel";
            this.GoodHdrCntLabel.Size = new System.Drawing.Size(13, 13);
            this.GoodHdrCntLabel.TabIndex = 58;
            this.GoodHdrCntLabel.Text = "0";
            // 
            // label45
            // 
            this.label45.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(758, 13);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(75, 13);
            this.label45.TabIndex = 59;
            this.label45.Text = "Good Hdr Cnt:";
            // 
            // label46
            // 
            this.label46.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(784, 26);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(48, 13);
            this.label46.TabIndex = 60;
            this.label46.Text = "Markers:";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 13);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(33, 13);
            this.label13.TabIndex = 55;
            this.label13.Text = "Time:";
            // 
            // CaptureTimeLabel
            // 
            this.CaptureTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CaptureTimeLabel.AutoSize = true;
            this.CaptureTimeLabel.Location = new System.Drawing.Point(43, 13);
            this.CaptureTimeLabel.Name = "CaptureTimeLabel";
            this.CaptureTimeLabel.Size = new System.Drawing.Size(13, 13);
            this.CaptureTimeLabel.TabIndex = 56;
            this.CaptureTimeLabel.Text = "0";
            // 
            // DiskTypeLabel
            // 
            this.DiskTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DiskTypeLabel.AutoSize = true;
            this.DiskTypeLabel.Location = new System.Drawing.Point(292, 13);
            this.DiskTypeLabel.Name = "DiskTypeLabel";
            this.DiskTypeLabel.Size = new System.Drawing.Size(53, 13);
            this.DiskTypeLabel.TabIndex = 27;
            this.DiskTypeLabel.Text = "Unknown";
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            this.openFileDialog2.Filter = "Projects (*.prj)|*.prj|All files (*.*|*.*)";
            // 
            // LimitToSectorUpDown
            // 
            this.LimitToSectorUpDown.Location = new System.Drawing.Point(443, 213);
            this.LimitToSectorUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.LimitToSectorUpDown.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            -2147483648});
            this.LimitToSectorUpDown.Name = "LimitToSectorUpDown";
            this.LimitToSectorUpDown.Size = new System.Drawing.Size(39, 20);
            this.LimitToSectorUpDown.TabIndex = 80;
            this.LimitToSectorUpDown.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(396, 215);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(38, 13);
            this.label41.TabIndex = 80;
            this.label41.Text = "S Limit";
            // 
            // LimitToTrackUpDown
            // 
            this.LimitToTrackUpDown.Location = new System.Drawing.Point(443, 187);
            this.LimitToTrackUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.LimitToTrackUpDown.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            -2147483648});
            this.LimitToTrackUpDown.Name = "LimitToTrackUpDown";
            this.LimitToTrackUpDown.Size = new System.Drawing.Size(39, 20);
            this.LimitToTrackUpDown.TabIndex = 79;
            this.LimitToTrackUpDown.Value = new decimal(new int[] {
            11,
            0,
            0,
            0});
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(399, 188);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(38, 13);
            this.label42.TabIndex = 78;
            this.label42.Text = "T Limit";
            // 
            // MainTabControl
            // 
            this.MainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTabControl.Controls.Add(this.QuickTab);
            this.MainTabControl.Controls.Add(this.CaptureTab);
            this.MainTabControl.Controls.Add(this.ProcessingTab);
            this.MainTabControl.Controls.Add(this.ErrorCorrectionTab);
            this.MainTabControl.Controls.Add(this.AnalysisPage);
            this.MainTabControl.Controls.Add(this.AnalysisTab2);
            this.MainTabControl.Controls.Add(this.NetworkTab);
            this.MainTabControl.ImageList = this.MainTabControlImageList;
            this.MainTabControl.Location = new System.Drawing.Point(13, 54);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(974, 802);
            this.MainTabControl.TabIndex = 56;
            this.MainTabControl.SelectedIndexChanged += new System.EventHandler(this.MainTabControl_SelectedIndexChanged);
            // 
            // QuickTab
            // 
            this.QuickTab.BackColor = System.Drawing.SystemColors.Control;
            this.QuickTab.Controls.Add(this.QrtbSectorMap);
            this.QuickTab.Controls.Add(this.QProcessingGroupBox);
            this.QuickTab.Controls.Add(this.groupBox12);
            this.QuickTab.ImageIndex = 3;
            this.QuickTab.Location = new System.Drawing.Point(4, 23);
            this.QuickTab.Name = "QuickTab";
            this.QuickTab.Padding = new System.Windows.Forms.Padding(3);
            this.QuickTab.Size = new System.Drawing.Size(966, 775);
            this.QuickTab.TabIndex = 6;
            this.QuickTab.Text = "Quick";
            // 
            // QrtbSectorMap
            // 
            this.QrtbSectorMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.QrtbSectorMap.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QrtbSectorMap.HideSelection = false;
            this.QrtbSectorMap.Location = new System.Drawing.Point(1, 378);
            this.QrtbSectorMap.Name = "QrtbSectorMap";
            this.QrtbSectorMap.Size = new System.Drawing.Size(963, 397);
            this.QrtbSectorMap.TabIndex = 114;
            this.QrtbSectorMap.Text = "";
            this.toolTip1.SetToolTip(this.QrtbSectorMap, "The sectormap shows you in detail what the status is for every sector. Right clic" +
        "k on a sector for more options. Double click to refresh.");
            this.QrtbSectorMap.DoubleClick += new System.EventHandler(this.rtbSectorMap_DoubleClick);
            this.QrtbSectorMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtbSectorMap_MouseDown);
            // 
            // QProcessingGroupBox
            // 
            this.QProcessingGroupBox.BackColor = System.Drawing.SystemColors.Control;
            this.QProcessingGroupBox.Controls.Add(this.button11);
            this.QProcessingGroupBox.Controls.Add(this.button15);
            this.QProcessingGroupBox.Controls.Add(this.groupBox14);
            this.QProcessingGroupBox.Controls.Add(this.QProcAmigaBtn);
            this.QProcessingGroupBox.Controls.Add(this.QProcPCBtn);
            this.QProcessingGroupBox.Enabled = false;
            this.QProcessingGroupBox.Location = new System.Drawing.Point(6, 138);
            this.QProcessingGroupBox.Name = "QProcessingGroupBox";
            this.QProcessingGroupBox.Size = new System.Drawing.Size(933, 234);
            this.QProcessingGroupBox.TabIndex = 113;
            this.QProcessingGroupBox.TabStop = false;
            this.QProcessingGroupBox.Text = "Processing";
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(0, 158);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(67, 38);
            this.button11.TabIndex = 119;
            this.button11.Text = "Reset output";
            this.toolTip1.SetToolTip(this.button11, "Clears all data generated by processing so you can start fresh with the same inpu" +
        "t data.");
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button5_Click);
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(0, 112);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(63, 39);
            this.button15.TabIndex = 118;
            this.button15.Text = "Reset input";
            this.toolTip1.SetToolTip(this.button15, "Clears the input buffer so you can load or capture new data but keeps recovered s" +
        "ectors in tact.");
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.ResetBuffersBtn_Click);
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.QOnlyBadSectorsRadio);
            this.groupBox14.Controls.Add(this.QECOnRadio);
            this.groupBox14.Controls.Add(this.label89);
            this.groupBox14.Controls.Add(this.QChangeDiskTypeComboBox);
            this.groupBox14.Controls.Add(this.QProcessingModeComboBox);
            this.groupBox14.Controls.Add(this.QClearDatacheckBox);
            this.groupBox14.Controls.Add(this.groupBox15);
            this.groupBox14.Controls.Add(this.QFindDupesCheckBox);
            this.groupBox14.Controls.Add(this.QAutoRefreshSectorMapCheck);
            this.groupBox14.Controls.Add(this.QIgnoreHeaderErrorCheckBox);
            this.groupBox14.Controls.Add(this.groupBox16);
            this.groupBox14.Controls.Add(this.groupBox17);
            this.groupBox14.Controls.Add(this.QHDCheckBox);
            this.groupBox14.Controls.Add(this.label114);
            this.groupBox14.Location = new System.Drawing.Point(94, 14);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(833, 217);
            this.groupBox14.TabIndex = 52;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Processing options";
            // 
            // QOnlyBadSectorsRadio
            // 
            this.QOnlyBadSectorsRadio.AutoSize = true;
            this.QOnlyBadSectorsRadio.Location = new System.Drawing.Point(6, 92);
            this.QOnlyBadSectorsRadio.Name = "QOnlyBadSectorsRadio";
            this.QOnlyBadSectorsRadio.Size = new System.Drawing.Size(104, 17);
            this.QOnlyBadSectorsRadio.TabIndex = 106;
            this.QOnlyBadSectorsRadio.Text = "Only bad sectors";
            this.toolTip1.SetToolTip(this.QOnlyBadSectorsRadio, "Only reprocess bad sectors. Can only work if some bad sectors have already been f" +
        "ound. This will speed up scanning a lot but may miss some sectors.");
            this.QOnlyBadSectorsRadio.UseVisualStyleBackColor = true;
            this.QOnlyBadSectorsRadio.CheckedChanged += new System.EventHandler(this.QOnlyBadSectorsRadio_CheckedChanged);
            // 
            // QECOnRadio
            // 
            this.QECOnRadio.AutoSize = true;
            this.QECOnRadio.Checked = true;
            this.QECOnRadio.Location = new System.Drawing.Point(6, 115);
            this.QECOnRadio.Name = "QECOnRadio";
            this.QECOnRadio.Size = new System.Drawing.Size(118, 17);
            this.QECOnRadio.TabIndex = 105;
            this.QECOnRadio.TabStop = true;
            this.QECOnRadio.Text = "Use error correction";
            this.toolTip1.SetToolTip(this.QECOnRadio, "Collects error correction data to help correcting it in the Error Correction tab." +
        " No correction is performed during processing.");
            this.QECOnRadio.UseVisualStyleBackColor = true;
            this.QECOnRadio.CheckedChanged += new System.EventHandler(this.QECOnRadio_CheckedChanged);
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Location = new System.Drawing.Point(118, 19);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(63, 13);
            this.label89.TabIndex = 104;
            this.label89.Text = "Disk Format";
            // 
            // QChangeDiskTypeComboBox
            // 
            this.QChangeDiskTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QChangeDiskTypeComboBox.FormattingEnabled = true;
            this.QChangeDiskTypeComboBox.Location = new System.Drawing.Point(121, 35);
            this.QChangeDiskTypeComboBox.Name = "QChangeDiskTypeComboBox";
            this.QChangeDiskTypeComboBox.Size = new System.Drawing.Size(75, 21);
            this.QChangeDiskTypeComboBox.TabIndex = 102;
            this.toolTip1.SetToolTip(this.QChangeDiskTypeComboBox, "The format is normally recognized automatically but it may happen that a single d" +
        "isk contains multiple disk types due to copying disks partly.");
            this.QChangeDiskTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.QChangeDiskTypeComboBox_SelectedIndexChanged);
            // 
            // QProcessingModeComboBox
            // 
            this.QProcessingModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QProcessingModeComboBox.FormattingEnabled = true;
            this.QProcessingModeComboBox.Location = new System.Drawing.Point(6, 35);
            this.QProcessingModeComboBox.Name = "QProcessingModeComboBox";
            this.QProcessingModeComboBox.Size = new System.Drawing.Size(106, 21);
            this.QProcessingModeComboBox.TabIndex = 103;
            this.toolTip1.SetToolTip(this.QProcessingModeComboBox, "Adaptive1 is both fast and accurate. Normal uses simple thresholds which works fi" +
        "ne for good quality captures. Aufit may recognize the odd sector. Other options " +
        "are experimental.");
            this.QProcessingModeComboBox.SelectedIndexChanged += new System.EventHandler(this.QProcessingModeComboBox_SelectedIndexChanged);
            // 
            // QClearDatacheckBox
            // 
            this.QClearDatacheckBox.AutoSize = true;
            this.QClearDatacheckBox.Location = new System.Drawing.Point(6, 69);
            this.QClearDatacheckBox.Name = "QClearDatacheckBox";
            this.QClearDatacheckBox.Size = new System.Drawing.Size(106, 17);
            this.QClearDatacheckBox.TabIndex = 101;
            this.QClearDatacheckBox.Text = "Clear sector data";
            this.toolTip1.SetToolTip(this.QClearDatacheckBox, "Clear recovered data that was found during the last processing job. This can spee" +
        "d up scanning.");
            this.QClearDatacheckBox.UseVisualStyleBackColor = true;
            this.QClearDatacheckBox.CheckedChanged += new System.EventHandler(this.QClearDatacheckBox_CheckedChanged);
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.QOffsetUpDown);
            this.groupBox15.Controls.Add(this.QMaxUpDown);
            this.groupBox15.Controls.Add(this.QSixEightUpDown);
            this.groupBox15.Controls.Add(this.QFourSixUpDown);
            this.groupBox15.Controls.Add(this.QMinUpDown);
            this.groupBox15.Controls.Add(this.QOffsetLabel);
            this.groupBox15.Controls.Add(this.QFourSixLabel);
            this.groupBox15.Controls.Add(this.QMinLabel);
            this.groupBox15.Controls.Add(this.QSixEightLabel);
            this.groupBox15.Controls.Add(this.QMaxLabel);
            this.groupBox15.Location = new System.Drawing.Point(295, 15);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(246, 71);
            this.groupBox15.TabIndex = 89;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Thresholds";
            // 
            // QOffsetUpDown
            // 
            this.QOffsetUpDown.Location = new System.Drawing.Point(196, 40);
            this.QOffsetUpDown.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.QOffsetUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.QOffsetUpDown.Name = "QOffsetUpDown";
            this.QOffsetUpDown.Size = new System.Drawing.Size(39, 20);
            this.QOffsetUpDown.TabIndex = 80;
            this.QOffsetUpDown.ValueChanged += new System.EventHandler(this.QMinUpDown_ValueChanged);
            // 
            // QMaxUpDown
            // 
            this.QMaxUpDown.Location = new System.Drawing.Point(148, 40);
            this.QMaxUpDown.Maximum = new decimal(new int[] {
            264,
            0,
            0,
            0});
            this.QMaxUpDown.Name = "QMaxUpDown";
            this.QMaxUpDown.Size = new System.Drawing.Size(39, 20);
            this.QMaxUpDown.TabIndex = 80;
            this.QMaxUpDown.Value = new decimal(new int[] {
            147,
            0,
            0,
            0});
            this.QMaxUpDown.ValueChanged += new System.EventHandler(this.QMinUpDown_ValueChanged);
            // 
            // QSixEightUpDown
            // 
            this.QSixEightUpDown.Location = new System.Drawing.Point(97, 39);
            this.QSixEightUpDown.Maximum = new decimal(new int[] {
            264,
            0,
            0,
            0});
            this.QSixEightUpDown.Name = "QSixEightUpDown";
            this.QSixEightUpDown.Size = new System.Drawing.Size(39, 20);
            this.QSixEightUpDown.TabIndex = 80;
            this.QSixEightUpDown.Value = new decimal(new int[] {
            108,
            0,
            0,
            0});
            this.QSixEightUpDown.ValueChanged += new System.EventHandler(this.QMinUpDown_ValueChanged);
            // 
            // QFourSixUpDown
            // 
            this.QFourSixUpDown.Location = new System.Drawing.Point(52, 39);
            this.QFourSixUpDown.Maximum = new decimal(new int[] {
            264,
            0,
            0,
            0});
            this.QFourSixUpDown.Name = "QFourSixUpDown";
            this.QFourSixUpDown.Size = new System.Drawing.Size(39, 20);
            this.QFourSixUpDown.TabIndex = 80;
            this.QFourSixUpDown.Value = new decimal(new int[] {
            69,
            0,
            0,
            0});
            this.QFourSixUpDown.ValueChanged += new System.EventHandler(this.QMinUpDown_ValueChanged);
            // 
            // QMinUpDown
            // 
            this.QMinUpDown.Location = new System.Drawing.Point(6, 39);
            this.QMinUpDown.Maximum = new decimal(new int[] {
            264,
            0,
            0,
            0});
            this.QMinUpDown.Name = "QMinUpDown";
            this.QMinUpDown.Size = new System.Drawing.Size(39, 20);
            this.QMinUpDown.TabIndex = 80;
            this.QMinUpDown.ValueChanged += new System.EventHandler(this.QMinUpDown_ValueChanged);
            // 
            // QOffsetLabel
            // 
            this.QOffsetLabel.AutoSize = true;
            this.QOffsetLabel.Location = new System.Drawing.Point(193, 22);
            this.QOffsetLabel.Name = "QOffsetLabel";
            this.QOffsetLabel.Size = new System.Drawing.Size(35, 13);
            this.QOffsetLabel.TabIndex = 56;
            this.QOffsetLabel.Text = "Offset";
            // 
            // QFourSixLabel
            // 
            this.QFourSixLabel.AutoSize = true;
            this.QFourSixLabel.Location = new System.Drawing.Point(51, 23);
            this.QFourSixLabel.Name = "QFourSixLabel";
            this.QFourSixLabel.Size = new System.Drawing.Size(24, 13);
            this.QFourSixLabel.TabIndex = 64;
            this.QFourSixLabel.Text = "4/6";
            // 
            // QMinLabel
            // 
            this.QMinLabel.AutoSize = true;
            this.QMinLabel.Location = new System.Drawing.Point(6, 23);
            this.QMinLabel.Name = "QMinLabel";
            this.QMinLabel.Size = new System.Drawing.Size(23, 13);
            this.QMinLabel.TabIndex = 63;
            this.QMinLabel.Text = "min";
            // 
            // QSixEightLabel
            // 
            this.QSixEightLabel.AutoSize = true;
            this.QSixEightLabel.Location = new System.Drawing.Point(94, 22);
            this.QSixEightLabel.Name = "QSixEightLabel";
            this.QSixEightLabel.Size = new System.Drawing.Size(24, 13);
            this.QSixEightLabel.TabIndex = 61;
            this.QSixEightLabel.Text = "6/8";
            // 
            // QMaxLabel
            // 
            this.QMaxLabel.AutoSize = true;
            this.QMaxLabel.Location = new System.Drawing.Point(148, 22);
            this.QMaxLabel.Name = "QMaxLabel";
            this.QMaxLabel.Size = new System.Drawing.Size(26, 13);
            this.QMaxLabel.TabIndex = 58;
            this.QMaxLabel.Text = "max";
            // 
            // QFindDupesCheckBox
            // 
            this.QFindDupesCheckBox.AccessibleDescription = "";
            this.QFindDupesCheckBox.AutoSize = true;
            this.QFindDupesCheckBox.Location = new System.Drawing.Point(129, 92);
            this.QFindDupesCheckBox.Name = "QFindDupesCheckBox";
            this.QFindDupesCheckBox.Size = new System.Drawing.Size(83, 17);
            this.QFindDupesCheckBox.TabIndex = 88;
            this.QFindDupesCheckBox.Text = "Deduplicate";
            this.toolTip1.SetToolTip(this.QFindDupesCheckBox, "When processing, a hash is calculated. If the new sector is unique, it will only " +
        "then be added to the list. This avoids processing duplicate data and may save ti" +
        "me.");
            this.QFindDupesCheckBox.UseVisualStyleBackColor = true;
            this.QFindDupesCheckBox.CheckedChanged += new System.EventHandler(this.QFindDupesCheckBox_CheckedChanged);
            // 
            // QAutoRefreshSectorMapCheck
            // 
            this.QAutoRefreshSectorMapCheck.AccessibleDescription = "";
            this.QAutoRefreshSectorMapCheck.AutoSize = true;
            this.QAutoRefreshSectorMapCheck.Checked = true;
            this.QAutoRefreshSectorMapCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.QAutoRefreshSectorMapCheck.Location = new System.Drawing.Point(129, 118);
            this.QAutoRefreshSectorMapCheck.Name = "QAutoRefreshSectorMapCheck";
            this.QAutoRefreshSectorMapCheck.Size = new System.Drawing.Size(135, 17);
            this.QAutoRefreshSectorMapCheck.TabIndex = 85;
            this.QAutoRefreshSectorMapCheck.Text = "Auto refresh sectormap";
            this.toolTip1.SetToolTip(this.QAutoRefreshSectorMapCheck, resources.GetString("QAutoRefreshSectorMapCheck.ToolTip"));
            this.QAutoRefreshSectorMapCheck.UseVisualStyleBackColor = true;
            this.QAutoRefreshSectorMapCheck.CheckedChanged += new System.EventHandler(this.QAutoRefreshSectorMapCheck_CheckedChanged);
            // 
            // QIgnoreHeaderErrorCheckBox
            // 
            this.QIgnoreHeaderErrorCheckBox.AutoSize = true;
            this.QIgnoreHeaderErrorCheckBox.Location = new System.Drawing.Point(129, 69);
            this.QIgnoreHeaderErrorCheckBox.Name = "QIgnoreHeaderErrorCheckBox";
            this.QIgnoreHeaderErrorCheckBox.Size = new System.Drawing.Size(116, 17);
            this.QIgnoreHeaderErrorCheckBox.TabIndex = 50;
            this.QIgnoreHeaderErrorCheckBox.Text = "Ignore header error";
            this.toolTip1.SetToolTip(this.QIgnoreHeaderErrorCheckBox, "In rare cases the crc = not ok on some headers can be ignored. On one msx disk th" +
        "is was some kind of copy protection.");
            this.QIgnoreHeaderErrorCheckBox.UseVisualStyleBackColor = true;
            this.QIgnoreHeaderErrorCheckBox.CheckedChanged += new System.EventHandler(this.QIgnoreHeaderErrorCheckBox_CheckedChanged);
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.QHistoPanel);
            this.groupBox16.Controls.Add(this.QHistogramhScrollBar1);
            this.groupBox16.Location = new System.Drawing.Point(549, 8);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(278, 166);
            this.groupBox16.TabIndex = 53;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Histogram";
            // 
            // QHistoPanel
            // 
            this.QHistoPanel.Location = new System.Drawing.Point(6, 19);
            this.QHistoPanel.Name = "QHistoPanel";
            this.QHistoPanel.Size = new System.Drawing.Size(260, 109);
            this.QHistoPanel.TabIndex = 36;
            this.QHistoPanel.Click += new System.EventHandler(this.Histogrampanel1_Click);
            this.QHistoPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.Histogrampanel1_Paint);
            // 
            // QHistogramhScrollBar1
            // 
            this.QHistogramhScrollBar1.LargeChange = 10000;
            this.QHistogramhScrollBar1.Location = new System.Drawing.Point(1, 145);
            this.QHistogramhScrollBar1.Maximum = 4000;
            this.QHistogramhScrollBar1.Name = "QHistogramhScrollBar1";
            this.QHistogramhScrollBar1.Size = new System.Drawing.Size(277, 45);
            this.QHistogramhScrollBar1.TabIndex = 105;
            this.QHistogramhScrollBar1.TickFrequency = 2000;
            this.QHistogramhScrollBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.QHistogramhScrollBar1.Scroll += new System.EventHandler(this.QHistogramhScrollBar1_Scroll);
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.QLimitTSCheckBox);
            this.groupBox17.Controls.Add(this.QRateOfChange2UpDown);
            this.groupBox17.Controls.Add(this.label88);
            this.groupBox17.Controls.Add(this.label102);
            this.groupBox17.Controls.Add(this.QRateOfChangeUpDown);
            this.groupBox17.Controls.Add(this.QScanComboBox);
            this.groupBox17.Controls.Add(this.button47);
            this.groupBox17.Controls.Add(this.QAdaptOfsset2UpDown);
            this.groupBox17.Controls.Add(this.label90);
            this.groupBox17.Controls.Add(this.QLimitToSectorUpDown);
            this.groupBox17.Controls.Add(this.label91);
            this.groupBox17.Controls.Add(this.QLimitToTrackUpDown);
            this.groupBox17.Location = new System.Drawing.Point(295, 92);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(248, 119);
            this.groupBox17.TabIndex = 54;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "Scan";
            // 
            // QLimitTSCheckBox
            // 
            this.QLimitTSCheckBox.AutoSize = true;
            this.QLimitTSCheckBox.Location = new System.Drawing.Point(10, 75);
            this.QLimitTSCheckBox.Name = "QLimitTSCheckBox";
            this.QLimitTSCheckBox.Size = new System.Drawing.Size(69, 17);
            this.QLimitTSCheckBox.TabIndex = 109;
            this.QLimitTSCheckBox.Text = "Limit T/S";
            this.toolTip1.SetToolTip(this.QLimitTSCheckBox, "Limits processing to this track and sector.");
            this.QLimitTSCheckBox.UseVisualStyleBackColor = true;
            this.QLimitTSCheckBox.CheckedChanged += new System.EventHandler(this.QLimitTSCheckBox_CheckedChanged);
            // 
            // QRateOfChange2UpDown
            // 
            this.QRateOfChange2UpDown.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.QRateOfChange2UpDown.Location = new System.Drawing.Point(186, 42);
            this.QRateOfChange2UpDown.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.QRateOfChange2UpDown.Name = "QRateOfChange2UpDown";
            this.QRateOfChange2UpDown.Size = new System.Drawing.Size(48, 20);
            this.QRateOfChange2UpDown.TabIndex = 108;
            this.toolTip1.SetToolTip(this.QRateOfChange2UpDown, "How quickly to adapt to change, larger number adapts slower to change in the sign" +
        "al. Use low numbers for glued disks or disks with lots of bad sectors or noise.");
            this.QRateOfChange2UpDown.Value = new decimal(new int[] {
            1300,
            0,
            0,
            0});
            this.QRateOfChange2UpDown.ValueChanged += new System.EventHandler(this.QRateOfChange2UpDown_ValueChanged);
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Location = new System.Drawing.Point(120, 44);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(62, 13);
            this.label88.TabIndex = 107;
            this.label88.Text = "Adapt track";
            // 
            // label102
            // 
            this.label102.AutoSize = true;
            this.label102.Location = new System.Drawing.Point(7, 45);
            this.label102.Name = "label102";
            this.label102.Size = new System.Drawing.Size(56, 13);
            this.label102.TabIndex = 106;
            this.label102.Text = "Adapt rate";
            // 
            // QRateOfChangeUpDown
            // 
            this.QRateOfChangeUpDown.DecimalPlaces = 2;
            this.QRateOfChangeUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.QRateOfChangeUpDown.Location = new System.Drawing.Point(66, 43);
            this.QRateOfChangeUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.QRateOfChangeUpDown.Name = "QRateOfChangeUpDown";
            this.QRateOfChangeUpDown.Size = new System.Drawing.Size(48, 20);
            this.QRateOfChangeUpDown.TabIndex = 105;
            this.QRateOfChangeUpDown.Value = new decimal(new int[] {
            12,
            0,
            0,
            65536});
            this.QRateOfChangeUpDown.ValueChanged += new System.EventHandler(this.QRateOfChangeUpDown_ValueChanged);
            // 
            // QScanComboBox
            // 
            this.QScanComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QScanComboBox.FormattingEnabled = true;
            this.QScanComboBox.Location = new System.Drawing.Point(6, 16);
            this.QScanComboBox.Name = "QScanComboBox";
            this.QScanComboBox.Size = new System.Drawing.Size(163, 21);
            this.QScanComboBox.TabIndex = 104;
            this.QScanComboBox.SelectedIndexChanged += new System.EventHandler(this.QScanComboBox_SelectedIndexChanged);
            // 
            // button47
            // 
            this.button47.Location = new System.Drawing.Point(175, 14);
            this.button47.Name = "button47";
            this.button47.Size = new System.Drawing.Size(40, 23);
            this.button47.TabIndex = 39;
            this.button47.Text = "Scan";
            this.toolTip1.SetToolTip(this.button47, "Use scanning to try different combinations of settings in order to find yet to be" +
        " recovered sectors.");
            this.button47.UseVisualStyleBackColor = true;
            this.button47.Click += new System.EventHandler(this.ScanBtn_Click_1);
            // 
            // QAdaptOfsset2UpDown
            // 
            this.QAdaptOfsset2UpDown.DecimalPlaces = 2;
            this.QAdaptOfsset2UpDown.Location = new System.Drawing.Point(186, 66);
            this.QAdaptOfsset2UpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.QAdaptOfsset2UpDown.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.QAdaptOfsset2UpDown.Name = "QAdaptOfsset2UpDown";
            this.QAdaptOfsset2UpDown.Size = new System.Drawing.Size(48, 20);
            this.QAdaptOfsset2UpDown.TabIndex = 99;
            this.QAdaptOfsset2UpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.QAdaptOfsset2UpDown.ValueChanged += new System.EventHandler(this.QAdaptOfsset2UpDown_ValueChanged);
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Location = new System.Drawing.Point(7, 95);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(38, 13);
            this.label90.TabIndex = 78;
            this.label90.Text = "T Limit";
            // 
            // QLimitToSectorUpDown
            // 
            this.QLimitToSectorUpDown.Location = new System.Drawing.Point(130, 94);
            this.QLimitToSectorUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.QLimitToSectorUpDown.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            -2147483648});
            this.QLimitToSectorUpDown.Name = "QLimitToSectorUpDown";
            this.QLimitToSectorUpDown.Size = new System.Drawing.Size(39, 20);
            this.QLimitToSectorUpDown.TabIndex = 80;
            this.QLimitToSectorUpDown.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.QLimitToSectorUpDown.ValueChanged += new System.EventHandler(this.QLimitToSectorUpDown_ValueChanged);
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(93, 96);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(38, 13);
            this.label91.TabIndex = 80;
            this.label91.Text = "S Limit";
            // 
            // QLimitToTrackUpDown
            // 
            this.QLimitToTrackUpDown.Location = new System.Drawing.Point(43, 93);
            this.QLimitToTrackUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.QLimitToTrackUpDown.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            -2147483648});
            this.QLimitToTrackUpDown.Name = "QLimitToTrackUpDown";
            this.QLimitToTrackUpDown.Size = new System.Drawing.Size(39, 20);
            this.QLimitToTrackUpDown.TabIndex = 79;
            this.QLimitToTrackUpDown.Value = new decimal(new int[] {
            11,
            0,
            0,
            0});
            this.QLimitToTrackUpDown.ValueChanged += new System.EventHandler(this.QLimitToTrackUpDown_ValueChanged);
            // 
            // QHDCheckBox
            // 
            this.QHDCheckBox.AutoSize = true;
            this.QHDCheckBox.Enabled = false;
            this.QHDCheckBox.Location = new System.Drawing.Point(5, 138);
            this.QHDCheckBox.Name = "QHDCheckBox";
            this.QHDCheckBox.Size = new System.Drawing.Size(86, 17);
            this.QHDCheckBox.TabIndex = 51;
            this.QHDCheckBox.Text = "High Density";
            this.toolTip1.SetToolTip(this.QHDCheckBox, "Is the disk incorrectly recognized as double density or vice versa? Change it her" +
        "e.");
            this.QHDCheckBox.UseVisualStyleBackColor = true;
            this.QHDCheckBox.CheckedChanged += new System.EventHandler(this.QHDCheckBox_CheckedChanged);
            // 
            // label114
            // 
            this.label114.AutoSize = true;
            this.label114.Location = new System.Drawing.Point(3, 19);
            this.label114.Name = "label114";
            this.label114.Size = new System.Drawing.Size(85, 13);
            this.label114.TabIndex = 44;
            this.label114.Text = "MFM processing";
            // 
            // QProcAmigaBtn
            // 
            this.QProcAmigaBtn.Location = new System.Drawing.Point(0, 19);
            this.QProcAmigaBtn.Name = "QProcAmigaBtn";
            this.QProcAmigaBtn.Size = new System.Drawing.Size(75, 40);
            this.QProcAmigaBtn.TabIndex = 51;
            this.QProcAmigaBtn.Text = "Process Amiga!";
            this.QProcAmigaBtn.UseVisualStyleBackColor = true;
            this.QProcAmigaBtn.Click += new System.EventHandler(this.ProcessAmigaBtn_Click);
            // 
            // QProcPCBtn
            // 
            this.QProcPCBtn.Location = new System.Drawing.Point(0, 65);
            this.QProcPCBtn.Name = "QProcPCBtn";
            this.QProcPCBtn.Size = new System.Drawing.Size(75, 40);
            this.QProcPCBtn.TabIndex = 50;
            this.QProcPCBtn.Text = "Process PC!";
            this.QProcPCBtn.UseVisualStyleBackColor = true;
            this.QProcPCBtn.Click += new System.EventHandler(this.ProcessPCBtn_Click);
            // 
            // groupBox12
            // 
            this.groupBox12.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox12.Controls.Add(this.GluedDiskPreset);
            this.groupBox12.Controls.Add(this.StepStickPresetBtn);
            this.groupBox12.Controls.Add(this.QDirectStepPresetBtn);
            this.groupBox12.Controls.Add(this.QRecaptureAllBtn);
            this.groupBox12.Controls.Add(this.button12);
            this.groupBox12.Controls.Add(this.button13);
            this.groupBox12.Controls.Add(this.button14);
            this.groupBox12.Controls.Add(this.QDirectStepCheckBox);
            this.groupBox12.Controls.Add(this.QCaptureBtn);
            this.groupBox12.Controls.Add(this.groupBox10);
            this.groupBox12.Location = new System.Drawing.Point(6, 6);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(939, 125);
            this.groupBox12.TabIndex = 112;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Capture";
            // 
            // GluedDiskPreset
            // 
            this.GluedDiskPreset.Location = new System.Drawing.Point(344, 91);
            this.GluedDiskPreset.Name = "GluedDiskPreset";
            this.GluedDiskPreset.Size = new System.Drawing.Size(98, 23);
            this.GluedDiskPreset.TabIndex = 121;
            this.GluedDiskPreset.Text = "Glued preset";
            this.toolTip1.SetToolTip(this.GluedDiskPreset, "If you use a stepstick, click this button to setup all settings for this mode of " +
        "operation.");
            this.GluedDiskPreset.Click += new System.EventHandler(this.GluedDiskPreset_Click);
            // 
            // StepStickPresetBtn
            // 
            this.StepStickPresetBtn.Location = new System.Drawing.Point(240, 91);
            this.StepStickPresetBtn.Name = "StepStickPresetBtn";
            this.StepStickPresetBtn.Size = new System.Drawing.Size(98, 23);
            this.StepStickPresetBtn.TabIndex = 120;
            this.StepStickPresetBtn.Text = "StepStick preset";
            this.toolTip1.SetToolTip(this.StepStickPresetBtn, "If you use a stepstick, click this button to setup all settings for this mode of " +
        "operation.");
            this.StepStickPresetBtn.Click += new System.EventHandler(this.StepStickPresetBtn_Click);
            // 
            // QDirectStepPresetBtn
            // 
            this.QDirectStepPresetBtn.Location = new System.Drawing.Point(134, 91);
            this.QDirectStepPresetBtn.Name = "QDirectStepPresetBtn";
            this.QDirectStepPresetBtn.Size = new System.Drawing.Size(98, 23);
            this.QDirectStepPresetBtn.TabIndex = 119;
            this.QDirectStepPresetBtn.Text = "Direct preset";
            this.toolTip1.SetToolTip(this.QDirectStepPresetBtn, "If you don\'t use a stepstick, click this button to setup all settings for this mo" +
        "de of operation.");
            this.QDirectStepPresetBtn.Click += new System.EventHandler(this.DirectPresetBtn_Click);
            // 
            // QRecaptureAllBtn
            // 
            this.QRecaptureAllBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.QRecaptureAllBtn.ImageIndex = 0;
            this.QRecaptureAllBtn.ImageList = this.MainTabControlImageList;
            this.QRecaptureAllBtn.Location = new System.Drawing.Point(6, 65);
            this.QRecaptureAllBtn.Name = "QRecaptureAllBtn";
            this.QRecaptureAllBtn.Size = new System.Drawing.Size(91, 40);
            this.QRecaptureAllBtn.TabIndex = 118;
            this.QRecaptureAllBtn.Text = "Recapture all";
            this.QRecaptureAllBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.QRecaptureAllBtn.UseVisualStyleBackColor = true;
            this.QRecaptureAllBtn.Click += new System.EventHandler(this.button48_Click);
            // 
            // MainTabControlImageList
            // 
            this.MainTabControlImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MainTabControlImageList.ImageStream")));
            this.MainTabControlImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.MainTabControlImageList.Images.SetKeyName(0, "IconRecord.png");
            this.MainTabControlImageList.Images.SetKeyName(1, "IconCheckmarkGreen.png");
            this.MainTabControlImageList.Images.SetKeyName(2, "IconScope.png");
            this.MainTabControlImageList.Images.SetKeyName(3, "IconProcessing2.png");
            this.MainTabControlImageList.Images.SetKeyName(4, "IconGraphEditor.png");
            this.MainTabControlImageList.Images.SetKeyName(5, "IconAnalysis.png");
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(861, 92);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(72, 25);
            this.button12.TabIndex = 117;
            this.button12.Text = "Step >";
            this.toolTip1.SetToolTip(this.button12, "(micro) step forward. Can be used if the head alignment is off.");
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.StepForwardBtn_Click);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(783, 92);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(72, 25);
            this.button13.TabIndex = 116;
            this.button13.Text = "Step <";
            this.toolTip1.SetToolTip(this.button13, "(micro) step back. Can be used if the head alignment is off.");
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.StepBackBtn_Click);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(705, 92);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(72, 25);
            this.button14.TabIndex = 115;
            this.button14.Text = "Microstep 8";
            this.toolTip1.SetToolTip(this.button14, "Set microstepping to 8 microsteps.");
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.Microstep8Btn_Click);
            // 
            // QDirectStepCheckBox
            // 
            this.QDirectStepCheckBox.AutoSize = true;
            this.QDirectStepCheckBox.Checked = global::FloppyControlApp.Properties.Settings.Default.DirectStep;
            this.QDirectStepCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FloppyControlApp.Properties.Settings.Default, "DirectStep", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.QDirectStepCheckBox.Location = new System.Drawing.Point(614, 97);
            this.QDirectStepCheckBox.Name = "QDirectStepCheckBox";
            this.QDirectStepCheckBox.Size = new System.Drawing.Size(76, 17);
            this.QDirectStepCheckBox.TabIndex = 114;
            this.QDirectStepCheckBox.Text = "DirectStep";
            this.toolTip1.SetToolTip(this.QDirectStepCheckBox, "If you don\'t use a step stick, check this box. If you do, clear it.");
            this.QDirectStepCheckBox.UseVisualStyleBackColor = true;
            this.QDirectStepCheckBox.CheckedChanged += new System.EventHandler(this.QDirectStepCheckBox_CheckedChanged);
            // 
            // QCaptureBtn
            // 
            this.QCaptureBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.QCaptureBtn.ImageIndex = 0;
            this.QCaptureBtn.ImageList = this.MainTabControlImageList;
            this.QCaptureBtn.Location = new System.Drawing.Point(6, 19);
            this.QCaptureBtn.Name = "QCaptureBtn";
            this.QCaptureBtn.Size = new System.Drawing.Size(69, 40);
            this.QCaptureBtn.TabIndex = 113;
            this.QCaptureBtn.Text = "Capture";
            this.QCaptureBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.QCaptureBtn, "Immediately start capturing using the start track, end track and duration setting" +
        "s.");
            this.QCaptureBtn.UseVisualStyleBackColor = true;
            this.QCaptureBtn.Click += new System.EventHandler(this.CaptureClassbutton_Click);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.groupBox11);
            this.groupBox10.Controls.Add(this.QTrackDurationUpDown);
            this.groupBox10.Controls.Add(this.QStartTrackUpDown);
            this.groupBox10.Controls.Add(this.QEndTracksUpDown);
            this.groupBox10.Controls.Add(this.label82);
            this.groupBox10.Controls.Add(this.QTRK00OffsetUpDown);
            this.groupBox10.Controls.Add(this.QMicrostepsPerTrackUpDown);
            this.groupBox10.Controls.Add(this.label83);
            this.groupBox10.Controls.Add(this.label84);
            this.groupBox10.Controls.Add(this.label85);
            this.groupBox10.Controls.Add(this.label86);
            this.groupBox10.Location = new System.Drawing.Point(134, 16);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(799, 70);
            this.groupBox10.TabIndex = 112;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Capture options";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.PresetCaptureDuration50s);
            this.groupBox11.Controls.Add(this.PresetCaptureDuration2s);
            this.groupBox11.Controls.Add(this.PresetCaptureDuration1s);
            this.groupBox11.Controls.Add(this.PresetCaptureDuration5s);
            this.groupBox11.Controls.Add(this.PresetTrack78_164);
            this.groupBox11.Controls.Add(this.PresetTrack80_90);
            this.groupBox11.Controls.Add(this.PresetCaptureDefaultBtn);
            this.groupBox11.Controls.Add(this.button37);
            this.groupBox11.Location = new System.Drawing.Point(421, 17);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(372, 53);
            this.groupBox11.TabIndex = 58;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Preset capture";
            // 
            // PresetCaptureDuration50s
            // 
            this.PresetCaptureDuration50s.Location = new System.Drawing.Point(332, 19);
            this.PresetCaptureDuration50s.Name = "PresetCaptureDuration50s";
            this.PresetCaptureDuration50s.Size = new System.Drawing.Size(37, 23);
            this.PresetCaptureDuration50s.TabIndex = 61;
            this.PresetCaptureDuration50s.Text = "50s";
            this.PresetCaptureDuration50s.UseVisualStyleBackColor = true;
            this.PresetCaptureDuration50s.Click += new System.EventHandler(this.PresetCaptureDuration50s_Click);
            // 
            // PresetCaptureDuration2s
            // 
            this.PresetCaptureDuration2s.Location = new System.Drawing.Point(271, 19);
            this.PresetCaptureDuration2s.Name = "PresetCaptureDuration2s";
            this.PresetCaptureDuration2s.Size = new System.Drawing.Size(26, 23);
            this.PresetCaptureDuration2s.TabIndex = 60;
            this.PresetCaptureDuration2s.Text = "2s";
            this.PresetCaptureDuration2s.UseVisualStyleBackColor = true;
            this.PresetCaptureDuration2s.Click += new System.EventHandler(this.PresetCaptureDuration2s_Click);
            // 
            // PresetCaptureDuration1s
            // 
            this.PresetCaptureDuration1s.Location = new System.Drawing.Point(241, 19);
            this.PresetCaptureDuration1s.Name = "PresetCaptureDuration1s";
            this.PresetCaptureDuration1s.Size = new System.Drawing.Size(26, 23);
            this.PresetCaptureDuration1s.TabIndex = 59;
            this.PresetCaptureDuration1s.Text = "1s";
            this.PresetCaptureDuration1s.UseVisualStyleBackColor = true;
            this.PresetCaptureDuration1s.Click += new System.EventHandler(this.PresetCaptureDuration1s_Click);
            // 
            // PresetCaptureDuration5s
            // 
            this.PresetCaptureDuration5s.Location = new System.Drawing.Point(300, 19);
            this.PresetCaptureDuration5s.Name = "PresetCaptureDuration5s";
            this.PresetCaptureDuration5s.Size = new System.Drawing.Size(26, 23);
            this.PresetCaptureDuration5s.TabIndex = 55;
            this.PresetCaptureDuration5s.Text = "5s";
            this.PresetCaptureDuration5s.UseVisualStyleBackColor = true;
            this.PresetCaptureDuration5s.Click += new System.EventHandler(this.PresetCaptureDuration5s_Click);
            // 
            // PresetTrack78_164
            // 
            this.PresetTrack78_164.Location = new System.Drawing.Point(183, 19);
            this.PresetTrack78_164.Name = "PresetTrack78_164";
            this.PresetTrack78_164.Size = new System.Drawing.Size(56, 23);
            this.PresetTrack78_164.TabIndex = 56;
            this.PresetTrack78_164.Text = "78-164";
            this.PresetTrack78_164.Click += new System.EventHandler(this.PresetTrack78_164_Click);
            // 
            // PresetTrack80_90
            // 
            this.PresetTrack80_90.Location = new System.Drawing.Point(124, 19);
            this.PresetTrack80_90.Name = "PresetTrack80_90";
            this.PresetTrack80_90.Size = new System.Drawing.Size(57, 23);
            this.PresetTrack80_90.TabIndex = 52;
            this.PresetTrack80_90.Text = "80-90";
            this.PresetTrack80_90.UseVisualStyleBackColor = true;
            this.PresetTrack80_90.Click += new System.EventHandler(this.PresetTrack80_90_Click);
            // 
            // PresetCaptureDefaultBtn
            // 
            this.PresetCaptureDefaultBtn.Location = new System.Drawing.Point(7, 19);
            this.PresetCaptureDefaultBtn.Name = "PresetCaptureDefaultBtn";
            this.PresetCaptureDefaultBtn.Size = new System.Drawing.Size(56, 23);
            this.PresetCaptureDefaultBtn.TabIndex = 57;
            this.PresetCaptureDefaultBtn.Text = "Default";
            this.PresetCaptureDefaultBtn.Click += new System.EventHandler(this.PresetCaptureDefaultBtn_Click);
            // 
            // button37
            // 
            this.button37.Location = new System.Drawing.Point(65, 19);
            this.button37.Name = "button37";
            this.button37.Size = new System.Drawing.Size(57, 23);
            this.button37.TabIndex = 58;
            this.button37.Text = "0-10";
            this.button37.Click += new System.EventHandler(this.PresetTrack0_10_Click);
            // 
            // QTrackDurationUpDown
            // 
            this.QTrackDurationUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FloppyControlApp.Properties.Settings.Default, "TrackDuration", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.QTrackDurationUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.QTrackDurationUpDown.Location = new System.Drawing.Point(323, 35);
            this.QTrackDurationUpDown.Maximum = new decimal(new int[] {
            2000000,
            0,
            0,
            0});
            this.QTrackDurationUpDown.Name = "QTrackDurationUpDown";
            this.QTrackDurationUpDown.Size = new System.Drawing.Size(52, 20);
            this.QTrackDurationUpDown.TabIndex = 50;
            this.toolTip1.SetToolTip(this.QTrackDurationUpDown, resources.GetString("QTrackDurationUpDown.ToolTip"));
            this.QTrackDurationUpDown.Value = global::FloppyControlApp.Properties.Settings.Default.TrackDuration;
            // 
            // QStartTrackUpDown
            // 
            this.QStartTrackUpDown.Location = new System.Drawing.Point(200, 35);
            this.QStartTrackUpDown.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.QStartTrackUpDown.Name = "QStartTrackUpDown";
            this.QStartTrackUpDown.Size = new System.Drawing.Size(52, 20);
            this.QStartTrackUpDown.TabIndex = 0;
            this.toolTip1.SetToolTip(this.QStartTrackUpDown, "Start track to capture.");
            // 
            // QEndTracksUpDown
            // 
            this.QEndTracksUpDown.Location = new System.Drawing.Point(262, 35);
            this.QEndTracksUpDown.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.QEndTracksUpDown.Name = "QEndTracksUpDown";
            this.QEndTracksUpDown.Size = new System.Drawing.Size(52, 20);
            this.QEndTracksUpDown.TabIndex = 49;
            this.toolTip1.SetToolTip(this.QEndTracksUpDown, "End track to capture.");
            this.QEndTracksUpDown.Value = new decimal(new int[] {
            162,
            0,
            0,
            0});
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.Location = new System.Drawing.Point(321, 19);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(94, 13);
            this.label82.TabIndex = 53;
            this.label82.Text = "Track Duration ms";
            // 
            // QTRK00OffsetUpDown
            // 
            this.QTRK00OffsetUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FloppyControlApp.Properties.Settings.Default, "TRK00Offset", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.QTRK00OffsetUpDown.Location = new System.Drawing.Point(102, 34);
            this.QTRK00OffsetUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.QTRK00OffsetUpDown.Name = "QTRK00OffsetUpDown";
            this.QTRK00OffsetUpDown.Size = new System.Drawing.Size(52, 20);
            this.QTRK00OffsetUpDown.TabIndex = 47;
            this.toolTip1.SetToolTip(this.QTRK00OffsetUpDown, "Track offset. When seeking TRK00 you can move farther back if you\'re having troub" +
        "le getting track 0 to capture well. Can be negative.");
            this.QTRK00OffsetUpDown.Value = global::FloppyControlApp.Properties.Settings.Default.TRK00Offset;
            // 
            // QMicrostepsPerTrackUpDown
            // 
            this.QMicrostepsPerTrackUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FloppyControlApp.Properties.Settings.Default, "MicroStepsPerTrack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.QMicrostepsPerTrackUpDown.Location = new System.Drawing.Point(8, 34);
            this.QMicrostepsPerTrackUpDown.Name = "QMicrostepsPerTrackUpDown";
            this.QMicrostepsPerTrackUpDown.Size = new System.Drawing.Size(52, 20);
            this.QMicrostepsPerTrackUpDown.TabIndex = 46;
            this.toolTip1.SetToolTip(this.QMicrostepsPerTrackUpDown, "If you use a step stick, anything under 8 microsteps will read between the tracks" +
        ". For a glued disk, use 2 microsteps for a full recovery.");
            this.QMicrostepsPerTrackUpDown.Value = global::FloppyControlApp.Properties.Settings.Default.MicroStepsPerTrack;
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.Location = new System.Drawing.Point(100, 19);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(70, 13);
            this.label83.TabIndex = 54;
            this.label83.Text = "TRK00 offset";
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Location = new System.Drawing.Point(6, 17);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(88, 13);
            this.label84.TabIndex = 55;
            this.label84.Text = "Track microsteps";
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Location = new System.Drawing.Point(196, 19);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(56, 13);
            this.label85.TabIndex = 56;
            this.label85.Text = "Start track";
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(259, 19);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(53, 13);
            this.label86.TabIndex = 57;
            this.label86.Text = "End track";
            // 
            // CaptureTab
            // 
            this.CaptureTab.BackColor = System.Drawing.SystemColors.Control;
            this.CaptureTab.Controls.Add(this.groupBox4);
            this.CaptureTab.Controls.Add(this.button8);
            this.CaptureTab.Controls.Add(this.button7);
            this.CaptureTab.Controls.Add(this.SaveTrimmedBadbutton);
            this.CaptureTab.Controls.Add(this.button49);
            this.CaptureTab.Controls.Add(this.button48);
            this.CaptureTab.Controls.Add(this.button46);
            this.CaptureTab.Controls.Add(this.button45);
            this.CaptureTab.Controls.Add(this.button40);
            this.CaptureTab.Controls.Add(this.button39);
            this.CaptureTab.Controls.Add(this.button36);
            this.CaptureTab.Controls.Add(this.CaptureClassbutton);
            this.CaptureTab.Controls.Add(this.groupBox7);
            this.CaptureTab.Controls.Add(this.DirectStepCheckBox);
            this.CaptureTab.ImageIndex = 0;
            this.CaptureTab.Location = new System.Drawing.Point(4, 23);
            this.CaptureTab.Name = "CaptureTab";
            this.CaptureTab.Padding = new System.Windows.Forms.Padding(3);
            this.CaptureTab.Size = new System.Drawing.Size(966, 775);
            this.CaptureTab.TabIndex = 0;
            this.CaptureTab.Text = "Capture";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rxbufEndUpDown);
            this.groupBox4.Controls.Add(this.rxbufStartUpDown);
            this.groupBox4.Controls.Add(this.BufferSizeLabel);
            this.groupBox4.Controls.Add(this.HistogramLengthLabel);
            this.groupBox4.Controls.Add(this.HistogramStartLabel);
            this.groupBox4.Controls.Add(this.label29);
            this.groupBox4.Controls.Add(this.label28);
            this.groupBox4.Controls.Add(this.label27);
            this.groupBox4.Controls.Add(this.label32);
            this.groupBox4.Controls.Add(this.label31);
            this.groupBox4.Location = new System.Drawing.Point(638, 611);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(294, 78);
            this.groupBox4.TabIndex = 102;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Input buffer";
            this.groupBox4.Visible = false;
            // 
            // rxbufEndUpDown
            // 
            this.rxbufEndUpDown.Location = new System.Drawing.Point(65, 51);
            this.rxbufEndUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.rxbufEndUpDown.Name = "rxbufEndUpDown";
            this.rxbufEndUpDown.Size = new System.Drawing.Size(77, 20);
            this.rxbufEndUpDown.TabIndex = 40;
            // 
            // rxbufStartUpDown
            // 
            this.rxbufStartUpDown.Location = new System.Drawing.Point(65, 25);
            this.rxbufStartUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.rxbufStartUpDown.Name = "rxbufStartUpDown";
            this.rxbufStartUpDown.Size = new System.Drawing.Size(77, 20);
            this.rxbufStartUpDown.TabIndex = 39;
            // 
            // BufferSizeLabel
            // 
            this.BufferSizeLabel.AutoSize = true;
            this.BufferSizeLabel.Location = new System.Drawing.Point(178, 58);
            this.BufferSizeLabel.Name = "BufferSizeLabel";
            this.BufferSizeLabel.Size = new System.Drawing.Size(13, 13);
            this.BufferSizeLabel.TabIndex = 31;
            this.BufferSizeLabel.Text = "0";
            // 
            // HistogramLengthLabel
            // 
            this.HistogramLengthLabel.AutoSize = true;
            this.HistogramLengthLabel.Location = new System.Drawing.Point(178, 39);
            this.HistogramLengthLabel.Name = "HistogramLengthLabel";
            this.HistogramLengthLabel.Size = new System.Drawing.Size(13, 13);
            this.HistogramLengthLabel.TabIndex = 32;
            this.HistogramLengthLabel.Text = "0";
            // 
            // HistogramStartLabel
            // 
            this.HistogramStartLabel.AutoSize = true;
            this.HistogramStartLabel.Location = new System.Drawing.Point(178, 18);
            this.HistogramStartLabel.Name = "HistogramStartLabel";
            this.HistogramStartLabel.Size = new System.Drawing.Size(13, 13);
            this.HistogramStartLabel.TabIndex = 33;
            this.HistogramStartLabel.Text = "0";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(153, 58);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(30, 13);
            this.label29.TabIndex = 34;
            this.label29.Text = "Size:";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(154, 39);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(29, 13);
            this.label28.TabIndex = 35;
            this.label28.Text = "End:";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(151, 18);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(32, 13);
            this.label27.TabIndex = 36;
            this.label27.Text = "Start:";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(8, 55);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(51, 13);
            this.label32.TabIndex = 37;
            this.label32.Text = "rxbuf end";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(6, 26);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(53, 13);
            this.label31.TabIndex = 38;
            this.label31.Text = "rxbuf start";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(7, 133);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(98, 23);
            this.button8.TabIndex = 101;
            this.button8.Text = "StepStick preset";
            this.button8.Click += new System.EventHandler(this.StepStickPresetBtn_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(7, 104);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(98, 23);
            this.button7.TabIndex = 100;
            this.button7.Text = "Direct preset";
            this.button7.Click += new System.EventHandler(this.DirectPresetBtn_Click);
            // 
            // SaveTrimmedBadbutton
            // 
            this.SaveTrimmedBadbutton.Location = new System.Drawing.Point(103, 435);
            this.SaveTrimmedBadbutton.Name = "SaveTrimmedBadbutton";
            this.SaveTrimmedBadbutton.Size = new System.Drawing.Size(91, 40);
            this.SaveTrimmedBadbutton.TabIndex = 99;
            this.SaveTrimmedBadbutton.Text = "Save only bad sector data";
            this.SaveTrimmedBadbutton.UseVisualStyleBackColor = true;
            this.SaveTrimmedBadbutton.Click += new System.EventHandler(this.SaveTrimmedBadbutton_Click);
            // 
            // button49
            // 
            this.button49.Location = new System.Drawing.Point(6, 435);
            this.button49.Name = "button49";
            this.button49.Size = new System.Drawing.Size(91, 40);
            this.button49.TabIndex = 98;
            this.button49.Text = "Save trimmed bin file";
            this.button49.UseVisualStyleBackColor = true;
            this.button49.Click += new System.EventHandler(this.button49_Click);
            // 
            // button48
            // 
            this.button48.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button48.ImageIndex = 0;
            this.button48.ImageList = this.MainTabControlImageList;
            this.button48.Location = new System.Drawing.Point(6, 55);
            this.button48.Name = "button48";
            this.button48.Size = new System.Drawing.Size(91, 40);
            this.button48.TabIndex = 97;
            this.button48.Text = "Recapture all";
            this.button48.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button48.UseVisualStyleBackColor = true;
            this.button48.Click += new System.EventHandler(this.button48_Click);
            // 
            // button46
            // 
            this.button46.Location = new System.Drawing.Point(81, 355);
            this.button46.Name = "button46";
            this.button46.Size = new System.Drawing.Size(69, 40);
            this.button46.TabIndex = 96;
            this.button46.Text = "Save SCP";
            this.button46.UseVisualStyleBackColor = true;
            this.button46.Click += new System.EventHandler(this.button46_Click);
            // 
            // button45
            // 
            this.button45.Location = new System.Drawing.Point(6, 355);
            this.button45.Name = "button45";
            this.button45.Size = new System.Drawing.Size(69, 40);
            this.button45.TabIndex = 95;
            this.button45.Text = "Open SCP";
            this.button45.UseVisualStyleBackColor = true;
            this.button45.Click += new System.EventHandler(this.button45_Click);
            // 
            // button40
            // 
            this.button40.Location = new System.Drawing.Point(162, 286);
            this.button40.Name = "button40";
            this.button40.Size = new System.Drawing.Size(72, 40);
            this.button40.TabIndex = 94;
            this.button40.Text = "Step >";
            this.button40.UseVisualStyleBackColor = true;
            this.button40.Click += new System.EventHandler(this.StepForwardBtn_Click);
            // 
            // button39
            // 
            this.button39.Location = new System.Drawing.Point(84, 286);
            this.button39.Name = "button39";
            this.button39.Size = new System.Drawing.Size(72, 40);
            this.button39.TabIndex = 93;
            this.button39.Text = "Step <";
            this.button39.UseVisualStyleBackColor = true;
            this.button39.Click += new System.EventHandler(this.StepBackBtn_Click);
            // 
            // button36
            // 
            this.button36.Location = new System.Drawing.Point(6, 286);
            this.button36.Name = "button36";
            this.button36.Size = new System.Drawing.Size(72, 40);
            this.button36.TabIndex = 92;
            this.button36.Text = "Microstep 8";
            this.button36.UseVisualStyleBackColor = true;
            this.button36.Click += new System.EventHandler(this.Microstep8Btn_Click);
            // 
            // CaptureClassbutton
            // 
            this.CaptureClassbutton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CaptureClassbutton.ImageIndex = 0;
            this.CaptureClassbutton.ImageList = this.MainTabControlImageList;
            this.CaptureClassbutton.Location = new System.Drawing.Point(6, 9);
            this.CaptureClassbutton.Name = "CaptureClassbutton";
            this.CaptureClassbutton.Size = new System.Drawing.Size(69, 40);
            this.CaptureClassbutton.TabIndex = 52;
            this.CaptureClassbutton.Text = "Capture";
            this.CaptureClassbutton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CaptureClassbutton.UseVisualStyleBackColor = true;
            this.CaptureClassbutton.Click += new System.EventHandler(this.CaptureClassbutton_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.groupBox1);
            this.groupBox7.Controls.Add(this.TrackDurationUpDown);
            this.groupBox7.Controls.Add(this.StartTrackUpDown);
            this.groupBox7.Controls.Add(this.EndTracksUpDown);
            this.groupBox7.Controls.Add(this.label22);
            this.groupBox7.Controls.Add(this.TRK00OffsetUpDown);
            this.groupBox7.Controls.Add(this.MicrostepsPerTrackUpDown);
            this.groupBox7.Controls.Add(this.label38);
            this.groupBox7.Controls.Add(this.label36);
            this.groupBox7.Controls.Add(this.label21);
            this.groupBox7.Controls.Add(this.label20);
            this.groupBox7.Location = new System.Drawing.Point(137, 6);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(250, 270);
            this.groupBox7.TabIndex = 50;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Capture options";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.TrackPreset4Button);
            this.groupBox1.Controls.Add(this.TrackPreset2Button);
            this.groupBox1.Controls.Add(this.TrackPreset3Button);
            this.groupBox1.Controls.Add(this.TrackPreset1Button);
            this.groupBox1.Location = new System.Drawing.Point(126, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(91, 239);
            this.groupBox1.TabIndex = 58;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preset capture";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(10, 116);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(57, 23);
            this.button4.TabIndex = 59;
            this.button4.Text = "1000ms";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.PresetCaptureDuration1s_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(10, 139);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(57, 23);
            this.button6.TabIndex = 55;
            this.button6.Text = "5000ms";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.PresetCaptureDuration5s_Click);
            // 
            // TrackPreset4Button
            // 
            this.TrackPreset4Button.Location = new System.Drawing.Point(10, 92);
            this.TrackPreset4Button.Name = "TrackPreset4Button";
            this.TrackPreset4Button.Size = new System.Drawing.Size(56, 23);
            this.TrackPreset4Button.TabIndex = 56;
            this.TrackPreset4Button.Text = "78-164";
            this.TrackPreset4Button.Click += new System.EventHandler(this.PresetTrack78_164_Click);
            // 
            // TrackPreset2Button
            // 
            this.TrackPreset2Button.Location = new System.Drawing.Point(9, 68);
            this.TrackPreset2Button.Name = "TrackPreset2Button";
            this.TrackPreset2Button.Size = new System.Drawing.Size(57, 23);
            this.TrackPreset2Button.TabIndex = 52;
            this.TrackPreset2Button.Text = "80-90";
            this.TrackPreset2Button.UseVisualStyleBackColor = true;
            this.TrackPreset2Button.Click += new System.EventHandler(this.PresetTrack80_90_Click);
            // 
            // TrackPreset3Button
            // 
            this.TrackPreset3Button.Location = new System.Drawing.Point(10, 19);
            this.TrackPreset3Button.Name = "TrackPreset3Button";
            this.TrackPreset3Button.Size = new System.Drawing.Size(56, 23);
            this.TrackPreset3Button.TabIndex = 57;
            this.TrackPreset3Button.Text = "Default";
            this.TrackPreset3Button.Click += new System.EventHandler(this.PresetCaptureDefaultBtn_Click);
            // 
            // TrackPreset1Button
            // 
            this.TrackPreset1Button.Location = new System.Drawing.Point(9, 44);
            this.TrackPreset1Button.Name = "TrackPreset1Button";
            this.TrackPreset1Button.Size = new System.Drawing.Size(57, 23);
            this.TrackPreset1Button.TabIndex = 58;
            this.TrackPreset1Button.Text = "0-10";
            this.TrackPreset1Button.Click += new System.EventHandler(this.PresetTrack0_10_Click);
            // 
            // TrackDurationUpDown
            // 
            this.TrackDurationUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FloppyControlApp.Properties.Settings.Default, "TrackDuration", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TrackDurationUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.TrackDurationUpDown.Location = new System.Drawing.Point(10, 207);
            this.TrackDurationUpDown.Maximum = new decimal(new int[] {
            2000000,
            0,
            0,
            0});
            this.TrackDurationUpDown.Name = "TrackDurationUpDown";
            this.TrackDurationUpDown.Size = new System.Drawing.Size(52, 20);
            this.TrackDurationUpDown.TabIndex = 50;
            this.TrackDurationUpDown.Value = global::FloppyControlApp.Properties.Settings.Default.TrackDuration;
            // 
            // StartTrackUpDown
            // 
            this.StartTrackUpDown.Location = new System.Drawing.Point(10, 122);
            this.StartTrackUpDown.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.StartTrackUpDown.Name = "StartTrackUpDown";
            this.StartTrackUpDown.Size = new System.Drawing.Size(52, 20);
            this.StartTrackUpDown.TabIndex = 48;
            this.StartTrackUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // EndTracksUpDown
            // 
            this.EndTracksUpDown.Location = new System.Drawing.Point(10, 165);
            this.EndTracksUpDown.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.EndTracksUpDown.Name = "EndTracksUpDown";
            this.EndTracksUpDown.Size = new System.Drawing.Size(52, 20);
            this.EndTracksUpDown.TabIndex = 49;
            this.EndTracksUpDown.Value = new decimal(new int[] {
            19,
            0,
            0,
            0});
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(8, 191);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(94, 13);
            this.label22.TabIndex = 53;
            this.label22.Text = "Track Duration ms";
            // 
            // TRK00OffsetUpDown
            // 
            this.TRK00OffsetUpDown.Location = new System.Drawing.Point(10, 77);
            this.TRK00OffsetUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.TRK00OffsetUpDown.Name = "TRK00OffsetUpDown";
            this.TRK00OffsetUpDown.Size = new System.Drawing.Size(52, 20);
            this.TRK00OffsetUpDown.TabIndex = 47;
            this.TRK00OffsetUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.TRK00OffsetUpDown.ValueChanged += new System.EventHandler(this.TRK00OffsetUpDown_ValueChanged);
            // 
            // MicrostepsPerTrackUpDown
            // 
            this.MicrostepsPerTrackUpDown.Location = new System.Drawing.Point(8, 34);
            this.MicrostepsPerTrackUpDown.Name = "MicrostepsPerTrackUpDown";
            this.MicrostepsPerTrackUpDown.Size = new System.Drawing.Size(52, 20);
            this.MicrostepsPerTrackUpDown.TabIndex = 46;
            this.MicrostepsPerTrackUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MicrostepsPerTrackUpDown.ValueChanged += new System.EventHandler(this.MicrostepsPerTrackUpDown_ValueChanged);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(8, 62);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(70, 13);
            this.label38.TabIndex = 54;
            this.label38.Text = "TRK00 offset";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(6, 17);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(102, 13);
            this.label36.TabIndex = 55;
            this.label36.Text = "microsteps per track";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(10, 106);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(56, 13);
            this.label21.TabIndex = 56;
            this.label21.Text = "Start track";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(10, 149);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(53, 13);
            this.label20.TabIndex = 57;
            this.label20.Text = "End track";
            // 
            // DirectStepCheckBox
            // 
            this.DirectStepCheckBox.AutoSize = true;
            this.DirectStepCheckBox.Checked = global::FloppyControlApp.Properties.Settings.Default.DirectStep;
            this.DirectStepCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DirectStepCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FloppyControlApp.Properties.Settings.Default, "DirectStep", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DirectStepCheckBox.Location = new System.Drawing.Point(9, 229);
            this.DirectStepCheckBox.Name = "DirectStepCheckBox";
            this.DirectStepCheckBox.Size = new System.Drawing.Size(76, 17);
            this.DirectStepCheckBox.TabIndex = 91;
            this.DirectStepCheckBox.Text = "DirectStep";
            this.DirectStepCheckBox.UseVisualStyleBackColor = true;
            this.DirectStepCheckBox.CheckedChanged += new System.EventHandler(this.DirectStepCheckBox_CheckedChanged);
            // 
            // ProcessingTab
            // 
            this.ProcessingTab.BackColor = System.Drawing.SystemColors.Control;
            this.ProcessingTab.Controls.Add(this.button5);
            this.ProcessingTab.Controls.Add(this.ResetBuffersBtn);
            this.ProcessingTab.Controls.Add(this.DupsUpDown);
            this.ProcessingTab.Controls.Add(this.label69);
            this.ProcessingTab.Controls.Add(this.AddNoiseKnumericUpDown);
            this.ProcessingTab.Controls.Add(this.label68);
            this.ProcessingTab.Controls.Add(this.rtbSectorMap);
            this.ProcessingTab.Controls.Add(this.groupBox6);
            this.ProcessingTab.Controls.Add(this.ProcessBtn);
            this.ProcessingTab.Controls.Add(this.ProcessPCBtn);
            this.ProcessingTab.ImageIndex = 3;
            this.ProcessingTab.Location = new System.Drawing.Point(4, 23);
            this.ProcessingTab.Name = "ProcessingTab";
            this.ProcessingTab.Padding = new System.Windows.Forms.Padding(3);
            this.ProcessingTab.Size = new System.Drawing.Size(966, 775);
            this.ProcessingTab.TabIndex = 1;
            this.ProcessingTab.Text = "Processing";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(7, 145);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(67, 38);
            this.button5.TabIndex = 117;
            this.button5.Text = "Reset output";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // ResetBuffersBtn
            // 
            this.ResetBuffersBtn.Location = new System.Drawing.Point(7, 99);
            this.ResetBuffersBtn.Name = "ResetBuffersBtn";
            this.ResetBuffersBtn.Size = new System.Drawing.Size(63, 39);
            this.ResetBuffersBtn.TabIndex = 116;
            this.ResetBuffersBtn.Text = "Reset input";
            this.ResetBuffersBtn.UseVisualStyleBackColor = true;
            this.ResetBuffersBtn.Click += new System.EventHandler(this.ResetBuffersBtn_Click);
            // 
            // DupsUpDown
            // 
            this.DupsUpDown.Location = new System.Drawing.Point(42, 266);
            this.DupsUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.DupsUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DupsUpDown.Name = "DupsUpDown";
            this.DupsUpDown.Size = new System.Drawing.Size(39, 20);
            this.DupsUpDown.TabIndex = 94;
            this.DupsUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Location = new System.Drawing.Point(6, 268);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(32, 13);
            this.label69.TabIndex = 93;
            this.label69.Text = "Dups";
            // 
            // AddNoiseKnumericUpDown
            // 
            this.AddNoiseKnumericUpDown.Location = new System.Drawing.Point(41, 240);
            this.AddNoiseKnumericUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.AddNoiseKnumericUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.AddNoiseKnumericUpDown.Name = "AddNoiseKnumericUpDown";
            this.AddNoiseKnumericUpDown.Size = new System.Drawing.Size(39, 20);
            this.AddNoiseKnumericUpDown.TabIndex = 92;
            this.AddNoiseKnumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(21, 242);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(16, 13);
            this.label68.TabIndex = 91;
            this.label68.Text = "k:";
            // 
            // rtbSectorMap
            // 
            this.rtbSectorMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbSectorMap.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbSectorMap.HideSelection = false;
            this.rtbSectorMap.Location = new System.Drawing.Point(0, 354);
            this.rtbSectorMap.Name = "rtbSectorMap";
            this.rtbSectorMap.Size = new System.Drawing.Size(963, 421);
            this.rtbSectorMap.TabIndex = 84;
            this.rtbSectorMap.Text = "";
            this.rtbSectorMap.DoubleClick += new System.EventHandler(this.rtbSectorMap_DoubleClick);
            this.rtbSectorMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtbSectorMap_MouseDown);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.jESEnd);
            this.groupBox6.Controls.Add(this.jESStart);
            this.groupBox6.Controls.Add(this.label17);
            this.groupBox6.Controls.Add(this.SettingsLabel);
            this.groupBox6.Controls.Add(this.label35);
            this.groupBox6.Controls.Add(this.iESEnd);
            this.groupBox6.Controls.Add(this.iESStart);
            this.groupBox6.Controls.Add(this.label8);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Controls.Add(this.FullHistBtn);
            this.groupBox6.Controls.Add(this.OnlyBadSectorsRadio);
            this.groupBox6.Controls.Add(this.ECOnRadio);
            this.groupBox6.Controls.Add(this.label78);
            this.groupBox6.Controls.Add(this.ChangeDiskTypeComboBox);
            this.groupBox6.Controls.Add(this.ProcessingModeComboBox);
            this.groupBox6.Controls.Add(this.ClearDatacheckBox);
            this.groupBox6.Controls.Add(this.LimitTSCheckBox);
            this.groupBox6.Controls.Add(this.RateOfChange2UpDown);
            this.groupBox6.Controls.Add(this.AdaptOfsset2UpDown);
            this.groupBox6.Controls.Add(this.label74);
            this.groupBox6.Controls.Add(this.PeriodBeyond8uscomboBox);
            this.groupBox6.Controls.Add(this.LimitToTrackUpDown);
            this.groupBox6.Controls.Add(this.RndAmountUpDown);
            this.groupBox6.Controls.Add(this.LimitToSectorUpDown);
            this.groupBox6.Controls.Add(this.label67);
            this.groupBox6.Controls.Add(this.label42);
            this.groupBox6.Controls.Add(this.LimitToScttrViewcheckBox);
            this.groupBox6.Controls.Add(this.label41);
            this.groupBox6.Controls.Add(this.AddNoisecheckBox);
            this.groupBox6.Controls.Add(this.ThresholdsGroupBox);
            this.groupBox6.Controls.Add(this.FindDupesCheckBox);
            this.groupBox6.Controls.Add(this.AutoRefreshSectorMapCheck);
            this.groupBox6.Controls.Add(this.label50);
            this.groupBox6.Controls.Add(this.RateOfChangeUpDown);
            this.groupBox6.Controls.Add(this.IgnoreHeaderErrorCheckBox);
            this.groupBox6.Controls.Add(this.groupBox5);
            this.groupBox6.Controls.Add(this.groupBox3);
            this.groupBox6.Controls.Add(this.HDCheckBox);
            this.groupBox6.Controls.Add(this.label37);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Location = new System.Drawing.Point(95, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(795, 342);
            this.groupBox6.TabIndex = 49;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Processing options";
            // 
            // jESEnd
            // 
            this.jESEnd.Location = new System.Drawing.Point(556, 307);
            this.jESEnd.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.jESEnd.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.jESEnd.Name = "jESEnd";
            this.jESEnd.Size = new System.Drawing.Size(39, 20);
            this.jESEnd.TabIndex = 112;
            this.jESEnd.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // jESStart
            // 
            this.jESStart.Location = new System.Drawing.Point(556, 281);
            this.jESStart.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.jESStart.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.jESStart.Name = "jESStart";
            this.jESStart.Size = new System.Drawing.Size(39, 20);
            this.jESStart.TabIndex = 111;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(499, 311);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(47, 13);
            this.label17.TabIndex = 114;
            this.label17.Text = "j ES end";
            // 
            // SettingsLabel
            // 
            this.SettingsLabel.AutoSize = true;
            this.SettingsLabel.Location = new System.Drawing.Point(392, 263);
            this.SettingsLabel.Name = "SettingsLabel";
            this.SettingsLabel.Size = new System.Drawing.Size(49, 13);
            this.SettingsLabel.TabIndex = 113;
            this.SettingsLabel.Text = "Counters";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(497, 282);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(51, 13);
            this.label35.TabIndex = 115;
            this.label35.Text = "j ES Start";
            // 
            // iESEnd
            // 
            this.iESEnd.Location = new System.Drawing.Point(449, 308);
            this.iESEnd.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.iESEnd.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.iESEnd.Name = "iESEnd";
            this.iESEnd.Size = new System.Drawing.Size(39, 20);
            this.iESEnd.TabIndex = 110;
            this.iESEnd.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // iESStart
            // 
            this.iESStart.Location = new System.Drawing.Point(449, 282);
            this.iESStart.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.iESStart.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.iESStart.Name = "iESStart";
            this.iESStart.Size = new System.Drawing.Size(39, 20);
            this.iESStart.TabIndex = 109;
            this.iESStart.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(392, 312);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 107;
            this.label8.Text = "i ES end";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(390, 283);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 108;
            this.label9.Text = "i ES start";
            // 
            // FullHistBtn
            // 
            this.FullHistBtn.Location = new System.Drawing.Point(308, 188);
            this.FullHistBtn.Name = "FullHistBtn";
            this.FullHistBtn.Size = new System.Drawing.Size(85, 23);
            this.FullHistBtn.TabIndex = 105;
            this.FullHistBtn.Text = "Full histogram";
            this.FullHistBtn.UseVisualStyleBackColor = true;
            this.FullHistBtn.Click += new System.EventHandler(this.FullHistBtn_Click);
            // 
            // OnlyBadSectorsRadio
            // 
            this.OnlyBadSectorsRadio.AutoSize = true;
            this.OnlyBadSectorsRadio.Location = new System.Drawing.Point(6, 92);
            this.OnlyBadSectorsRadio.Name = "OnlyBadSectorsRadio";
            this.OnlyBadSectorsRadio.Size = new System.Drawing.Size(104, 17);
            this.OnlyBadSectorsRadio.TabIndex = 106;
            this.OnlyBadSectorsRadio.Text = "Only bad sectors";
            this.OnlyBadSectorsRadio.UseVisualStyleBackColor = true;
            // 
            // ECOnRadio
            // 
            this.ECOnRadio.AutoSize = true;
            this.ECOnRadio.Checked = true;
            this.ECOnRadio.Location = new System.Drawing.Point(6, 115);
            this.ECOnRadio.Name = "ECOnRadio";
            this.ECOnRadio.Size = new System.Drawing.Size(118, 17);
            this.ECOnRadio.TabIndex = 105;
            this.ECOnRadio.TabStop = true;
            this.ECOnRadio.Text = "Use error correction";
            this.ECOnRadio.UseVisualStyleBackColor = true;
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Location = new System.Drawing.Point(223, 20);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(63, 13);
            this.label78.TabIndex = 104;
            this.label78.Text = "Disk Format";
            // 
            // ChangeDiskTypeComboBox
            // 
            this.ChangeDiskTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChangeDiskTypeComboBox.FormattingEnabled = true;
            this.ChangeDiskTypeComboBox.Location = new System.Drawing.Point(226, 36);
            this.ChangeDiskTypeComboBox.Name = "ChangeDiskTypeComboBox";
            this.ChangeDiskTypeComboBox.Size = new System.Drawing.Size(75, 21);
            this.ChangeDiskTypeComboBox.TabIndex = 102;
            this.ChangeDiskTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // ProcessingModeComboBox
            // 
            this.ProcessingModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ProcessingModeComboBox.FormattingEnabled = true;
            this.ProcessingModeComboBox.Location = new System.Drawing.Point(6, 35);
            this.ProcessingModeComboBox.Name = "ProcessingModeComboBox";
            this.ProcessingModeComboBox.Size = new System.Drawing.Size(106, 21);
            this.ProcessingModeComboBox.TabIndex = 103;
            // 
            // ClearDatacheckBox
            // 
            this.ClearDatacheckBox.AutoSize = true;
            this.ClearDatacheckBox.Location = new System.Drawing.Point(6, 69);
            this.ClearDatacheckBox.Name = "ClearDatacheckBox";
            this.ClearDatacheckBox.Size = new System.Drawing.Size(106, 17);
            this.ClearDatacheckBox.TabIndex = 101;
            this.ClearDatacheckBox.Text = "Clear sector data";
            this.ClearDatacheckBox.UseVisualStyleBackColor = true;
            // 
            // LimitTSCheckBox
            // 
            this.LimitTSCheckBox.AutoSize = true;
            this.LimitTSCheckBox.Location = new System.Drawing.Point(408, 239);
            this.LimitTSCheckBox.Name = "LimitTSCheckBox";
            this.LimitTSCheckBox.Size = new System.Drawing.Size(69, 17);
            this.LimitTSCheckBox.TabIndex = 90;
            this.LimitTSCheckBox.Text = "Limit T/S";
            this.LimitTSCheckBox.UseVisualStyleBackColor = true;
            // 
            // RateOfChange2UpDown
            // 
            this.RateOfChange2UpDown.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.RateOfChange2UpDown.Location = new System.Drawing.Point(550, 212);
            this.RateOfChange2UpDown.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.RateOfChange2UpDown.Name = "RateOfChange2UpDown";
            this.RateOfChange2UpDown.Size = new System.Drawing.Size(48, 20);
            this.RateOfChange2UpDown.TabIndex = 100;
            this.RateOfChange2UpDown.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // AdaptOfsset2UpDown
            // 
            this.AdaptOfsset2UpDown.DecimalPlaces = 2;
            this.AdaptOfsset2UpDown.Location = new System.Drawing.Point(550, 236);
            this.AdaptOfsset2UpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.AdaptOfsset2UpDown.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.AdaptOfsset2UpDown.Name = "AdaptOfsset2UpDown";
            this.AdaptOfsset2UpDown.Size = new System.Drawing.Size(48, 20);
            this.AdaptOfsset2UpDown.TabIndex = 99;
            this.AdaptOfsset2UpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Location = new System.Drawing.Point(484, 214);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(62, 13);
            this.label74.TabIndex = 98;
            this.label74.Text = "Adapt track";
            // 
            // PeriodBeyond8uscomboBox
            // 
            this.PeriodBeyond8uscomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PeriodBeyond8uscomboBox.FormattingEnabled = true;
            this.PeriodBeyond8uscomboBox.Items.AddRange(new object[] {
            "None",
            "10",
            "100",
            "1000",
            "Random"});
            this.PeriodBeyond8uscomboBox.Location = new System.Drawing.Point(129, 35);
            this.PeriodBeyond8uscomboBox.Name = "PeriodBeyond8uscomboBox";
            this.PeriodBeyond8uscomboBox.Size = new System.Drawing.Size(75, 21);
            this.PeriodBeyond8uscomboBox.TabIndex = 96;
            // 
            // RndAmountUpDown
            // 
            this.RndAmountUpDown.Location = new System.Drawing.Point(76, 208);
            this.RndAmountUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.RndAmountUpDown.Name = "RndAmountUpDown";
            this.RndAmountUpDown.Size = new System.Drawing.Size(39, 20);
            this.RndAmountUpDown.TabIndex = 95;
            this.RndAmountUpDown.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(10, 210);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(65, 13);
            this.label67.TabIndex = 94;
            this.label67.Text = "Rnd amount";
            // 
            // LimitToScttrViewcheckBox
            // 
            this.LimitToScttrViewcheckBox.AccessibleDescription = "";
            this.LimitToScttrViewcheckBox.AutoSize = true;
            this.LimitToScttrViewcheckBox.Location = new System.Drawing.Point(6, 161);
            this.LimitToScttrViewcheckBox.Name = "LimitToScttrViewcheckBox";
            this.LimitToScttrViewcheckBox.Size = new System.Drawing.Size(107, 17);
            this.LimitToScttrViewcheckBox.TabIndex = 93;
            this.LimitToScttrViewcheckBox.Text = "Limit to scttr view";
            this.LimitToScttrViewcheckBox.UseVisualStyleBackColor = true;
            // 
            // AddNoisecheckBox
            // 
            this.AddNoisecheckBox.AccessibleDescription = "";
            this.AddNoisecheckBox.AutoSize = true;
            this.AddNoisecheckBox.Location = new System.Drawing.Point(6, 184);
            this.AddNoisecheckBox.Name = "AddNoisecheckBox";
            this.AddNoisecheckBox.Size = new System.Drawing.Size(73, 17);
            this.AddNoisecheckBox.TabIndex = 92;
            this.AddNoisecheckBox.Text = "Add noise";
            this.AddNoisecheckBox.UseVisualStyleBackColor = true;
            // 
            // ThresholdsGroupBox
            // 
            this.ThresholdsGroupBox.Controls.Add(this.MinvScrollBar);
            this.ThresholdsGroupBox.Controls.Add(this.FourLabel);
            this.ThresholdsGroupBox.Controls.Add(this.MinLabel);
            this.ThresholdsGroupBox.Controls.Add(this.SixLabel);
            this.ThresholdsGroupBox.Controls.Add(this.EightLabel);
            this.ThresholdsGroupBox.Controls.Add(this.Offsetlabel);
            this.ThresholdsGroupBox.Controls.Add(this.FourvScrollBar);
            this.ThresholdsGroupBox.Controls.Add(this.SixvScrollBar);
            this.ThresholdsGroupBox.Controls.Add(this.EightvScrollBar);
            this.ThresholdsGroupBox.Controls.Add(this.OffsetvScrollBar1);
            this.ThresholdsGroupBox.Controls.Add(this.POffsetLabel);
            this.ThresholdsGroupBox.Controls.Add(this.PFourSixLabel);
            this.ThresholdsGroupBox.Controls.Add(this.PMinLabel);
            this.ThresholdsGroupBox.Controls.Add(this.PSixEightLabel);
            this.ThresholdsGroupBox.Controls.Add(this.PMaxLabel);
            this.ThresholdsGroupBox.Location = new System.Drawing.Point(604, 16);
            this.ThresholdsGroupBox.Name = "ThresholdsGroupBox";
            this.ThresholdsGroupBox.Size = new System.Drawing.Size(175, 250);
            this.ThresholdsGroupBox.TabIndex = 89;
            this.ThresholdsGroupBox.TabStop = false;
            this.ThresholdsGroupBox.Text = "Thresholds";
            // 
            // MinvScrollBar
            // 
            this.MinvScrollBar.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FloppyControlApp.Properties.Settings.Default, "Min", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MinvScrollBar.Location = new System.Drawing.Point(13, 39);
            this.MinvScrollBar.Maximum = 264;
            this.MinvScrollBar.Name = "MinvScrollBar";
            this.MinvScrollBar.Size = new System.Drawing.Size(15, 184);
            this.MinvScrollBar.TabIndex = 65;
            this.MinvScrollBar.Value = global::FloppyControlApp.Properties.Settings.Default.Min;
            this.MinvScrollBar.ValueChanged += new System.EventHandler(this.FourvScrollBar_ValueChanged);
            // 
            // FourLabel
            // 
            this.FourLabel.AutoSize = true;
            this.FourLabel.Location = new System.Drawing.Point(48, 232);
            this.FourLabel.Name = "FourLabel";
            this.FourLabel.Size = new System.Drawing.Size(13, 13);
            this.FourLabel.TabIndex = 62;
            this.FourLabel.Text = "0";
            // 
            // MinLabel
            // 
            this.MinLabel.AutoSize = true;
            this.MinLabel.Location = new System.Drawing.Point(17, 232);
            this.MinLabel.Name = "MinLabel";
            this.MinLabel.Size = new System.Drawing.Size(13, 13);
            this.MinLabel.TabIndex = 60;
            this.MinLabel.Text = "0";
            // 
            // SixLabel
            // 
            this.SixLabel.AutoSize = true;
            this.SixLabel.Location = new System.Drawing.Point(81, 232);
            this.SixLabel.Name = "SixLabel";
            this.SixLabel.Size = new System.Drawing.Size(13, 13);
            this.SixLabel.TabIndex = 59;
            this.SixLabel.Text = "0";
            // 
            // EightLabel
            // 
            this.EightLabel.AutoSize = true;
            this.EightLabel.Location = new System.Drawing.Point(120, 232);
            this.EightLabel.Name = "EightLabel";
            this.EightLabel.Size = new System.Drawing.Size(13, 13);
            this.EightLabel.TabIndex = 57;
            this.EightLabel.Text = "0";
            // 
            // Offsetlabel
            // 
            this.Offsetlabel.AutoSize = true;
            this.Offsetlabel.Location = new System.Drawing.Point(149, 232);
            this.Offsetlabel.Name = "Offsetlabel";
            this.Offsetlabel.Size = new System.Drawing.Size(13, 13);
            this.Offsetlabel.TabIndex = 55;
            this.Offsetlabel.Text = "0";
            // 
            // FourvScrollBar
            // 
            this.FourvScrollBar.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FloppyControlApp.Properties.Settings.Default, "FourSix", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FourvScrollBar.Location = new System.Drawing.Point(45, 39);
            this.FourvScrollBar.Maximum = 264;
            this.FourvScrollBar.Name = "FourvScrollBar";
            this.FourvScrollBar.Size = new System.Drawing.Size(15, 184);
            this.FourvScrollBar.TabIndex = 66;
            this.FourvScrollBar.Value = global::FloppyControlApp.Properties.Settings.Default.FourSix;
            this.FourvScrollBar.ValueChanged += new System.EventHandler(this.FourvScrollBar_ValueChanged);
            // 
            // SixvScrollBar
            // 
            this.SixvScrollBar.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FloppyControlApp.Properties.Settings.Default, "SixEight", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.SixvScrollBar.Location = new System.Drawing.Point(79, 39);
            this.SixvScrollBar.Maximum = 255;
            this.SixvScrollBar.Name = "SixvScrollBar";
            this.SixvScrollBar.Size = new System.Drawing.Size(15, 184);
            this.SixvScrollBar.TabIndex = 67;
            this.SixvScrollBar.Value = global::FloppyControlApp.Properties.Settings.Default.SixEight;
            this.SixvScrollBar.ValueChanged += new System.EventHandler(this.FourvScrollBar_ValueChanged);
            // 
            // EightvScrollBar
            // 
            this.EightvScrollBar.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FloppyControlApp.Properties.Settings.Default, "Max", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.EightvScrollBar.Location = new System.Drawing.Point(118, 39);
            this.EightvScrollBar.Maximum = 264;
            this.EightvScrollBar.Name = "EightvScrollBar";
            this.EightvScrollBar.Size = new System.Drawing.Size(15, 184);
            this.EightvScrollBar.TabIndex = 68;
            this.EightvScrollBar.Value = global::FloppyControlApp.Properties.Settings.Default.Max;
            this.EightvScrollBar.ValueChanged += new System.EventHandler(this.FourvScrollBar_ValueChanged);
            // 
            // OffsetvScrollBar1
            // 
            this.OffsetvScrollBar1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FloppyControlApp.Properties.Settings.Default, "Offset", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.OffsetvScrollBar1.Location = new System.Drawing.Point(147, 39);
            this.OffsetvScrollBar1.Maximum = 50;
            this.OffsetvScrollBar1.Minimum = -50;
            this.OffsetvScrollBar1.Name = "OffsetvScrollBar1";
            this.OffsetvScrollBar1.Size = new System.Drawing.Size(15, 184);
            this.OffsetvScrollBar1.TabIndex = 69;
            this.OffsetvScrollBar1.Value = global::FloppyControlApp.Properties.Settings.Default.Offset;
            this.OffsetvScrollBar1.ValueChanged += new System.EventHandler(this.FourvScrollBar_ValueChanged);
            // 
            // POffsetLabel
            // 
            this.POffsetLabel.AutoSize = true;
            this.POffsetLabel.Location = new System.Drawing.Point(142, 23);
            this.POffsetLabel.Name = "POffsetLabel";
            this.POffsetLabel.Size = new System.Drawing.Size(35, 13);
            this.POffsetLabel.TabIndex = 56;
            this.POffsetLabel.Text = "Offset";
            // 
            // PFourSixLabel
            // 
            this.PFourSixLabel.AutoSize = true;
            this.PFourSixLabel.Location = new System.Drawing.Point(40, 23);
            this.PFourSixLabel.Name = "PFourSixLabel";
            this.PFourSixLabel.Size = new System.Drawing.Size(24, 13);
            this.PFourSixLabel.TabIndex = 64;
            this.PFourSixLabel.Text = "4/6";
            // 
            // PMinLabel
            // 
            this.PMinLabel.AutoSize = true;
            this.PMinLabel.Location = new System.Drawing.Point(12, 23);
            this.PMinLabel.Name = "PMinLabel";
            this.PMinLabel.Size = new System.Drawing.Size(23, 13);
            this.PMinLabel.TabIndex = 63;
            this.PMinLabel.Text = "min";
            // 
            // PSixEightLabel
            // 
            this.PSixEightLabel.AutoSize = true;
            this.PSixEightLabel.Location = new System.Drawing.Point(74, 23);
            this.PSixEightLabel.Name = "PSixEightLabel";
            this.PSixEightLabel.Size = new System.Drawing.Size(24, 13);
            this.PSixEightLabel.TabIndex = 61;
            this.PSixEightLabel.Text = "6/8";
            // 
            // PMaxLabel
            // 
            this.PMaxLabel.AutoSize = true;
            this.PMaxLabel.Location = new System.Drawing.Point(113, 23);
            this.PMaxLabel.Name = "PMaxLabel";
            this.PMaxLabel.Size = new System.Drawing.Size(26, 13);
            this.PMaxLabel.TabIndex = 58;
            this.PMaxLabel.Text = "max";
            // 
            // FindDupesCheckBox
            // 
            this.FindDupesCheckBox.AccessibleDescription = "";
            this.FindDupesCheckBox.AutoSize = true;
            this.FindDupesCheckBox.Location = new System.Drawing.Point(129, 158);
            this.FindDupesCheckBox.Name = "FindDupesCheckBox";
            this.FindDupesCheckBox.Size = new System.Drawing.Size(83, 17);
            this.FindDupesCheckBox.TabIndex = 88;
            this.FindDupesCheckBox.Text = "Deduplicate";
            this.FindDupesCheckBox.UseVisualStyleBackColor = true;
            // 
            // AutoRefreshSectorMapCheck
            // 
            this.AutoRefreshSectorMapCheck.AccessibleDescription = "";
            this.AutoRefreshSectorMapCheck.AutoSize = true;
            this.AutoRefreshSectorMapCheck.Checked = true;
            this.AutoRefreshSectorMapCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoRefreshSectorMapCheck.Location = new System.Drawing.Point(129, 184);
            this.AutoRefreshSectorMapCheck.Name = "AutoRefreshSectorMapCheck";
            this.AutoRefreshSectorMapCheck.Size = new System.Drawing.Size(135, 17);
            this.AutoRefreshSectorMapCheck.TabIndex = 85;
            this.AutoRefreshSectorMapCheck.Text = "Auto refresh sectormap";
            this.AutoRefreshSectorMapCheck.UseVisualStyleBackColor = true;
            this.AutoRefreshSectorMapCheck.CheckedChanged += new System.EventHandler(this.AutoRefreshSectorMapCheck_CheckedChanged);
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(491, 191);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(56, 13);
            this.label50.TabIndex = 80;
            this.label50.Text = "Adapt rate";
            // 
            // RateOfChangeUpDown
            // 
            this.RateOfChangeUpDown.DecimalPlaces = 2;
            this.RateOfChangeUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.RateOfChangeUpDown.Location = new System.Drawing.Point(550, 189);
            this.RateOfChangeUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.RateOfChangeUpDown.Name = "RateOfChangeUpDown";
            this.RateOfChangeUpDown.Size = new System.Drawing.Size(48, 20);
            this.RateOfChangeUpDown.TabIndex = 79;
            this.RateOfChangeUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // IgnoreHeaderErrorCheckBox
            // 
            this.IgnoreHeaderErrorCheckBox.AutoSize = true;
            this.IgnoreHeaderErrorCheckBox.Location = new System.Drawing.Point(129, 135);
            this.IgnoreHeaderErrorCheckBox.Name = "IgnoreHeaderErrorCheckBox";
            this.IgnoreHeaderErrorCheckBox.Size = new System.Drawing.Size(116, 17);
            this.IgnoreHeaderErrorCheckBox.TabIndex = 50;
            this.IgnoreHeaderErrorCheckBox.Text = "Ignore header error";
            this.IgnoreHeaderErrorCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.Histogrampanel1);
            this.groupBox5.Controls.Add(this.HistogramhScrollBar1);
            this.groupBox5.Location = new System.Drawing.Point(307, 13);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(278, 166);
            this.groupBox5.TabIndex = 53;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Histogram";
            // 
            // Histogrampanel1
            // 
            this.Histogrampanel1.Location = new System.Drawing.Point(6, 19);
            this.Histogrampanel1.Name = "Histogrampanel1";
            this.Histogrampanel1.Size = new System.Drawing.Size(260, 109);
            this.Histogrampanel1.TabIndex = 36;
            this.Histogrampanel1.Click += new System.EventHandler(this.Histogrampanel1_Click);
            this.Histogrampanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Histogrampanel1_Paint);
            // 
            // HistogramhScrollBar1
            // 
            this.HistogramhScrollBar1.LargeChange = 10000;
            this.HistogramhScrollBar1.Location = new System.Drawing.Point(1, 145);
            this.HistogramhScrollBar1.Maximum = 4000;
            this.HistogramhScrollBar1.Name = "HistogramhScrollBar1";
            this.HistogramhScrollBar1.Size = new System.Drawing.Size(277, 45);
            this.HistogramhScrollBar1.TabIndex = 105;
            this.HistogramhScrollBar1.TickFrequency = 2000;
            this.HistogramhScrollBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.HistogramhScrollBar1.Scroll += new System.EventHandler(this.HistogramhScrollBar1_Scroll);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ScanComboBox);
            this.groupBox3.Controls.Add(this.ScanButton);
            this.groupBox3.Location = new System.Drawing.Point(6, 244);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(227, 46);
            this.groupBox3.TabIndex = 54;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Scan";
            // 
            // ScanComboBox
            // 
            this.ScanComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ScanComboBox.FormattingEnabled = true;
            this.ScanComboBox.Location = new System.Drawing.Point(6, 16);
            this.ScanComboBox.Name = "ScanComboBox";
            this.ScanComboBox.Size = new System.Drawing.Size(163, 21);
            this.ScanComboBox.TabIndex = 104;
            // 
            // ScanButton
            // 
            this.ScanButton.Location = new System.Drawing.Point(175, 14);
            this.ScanButton.Name = "ScanButton";
            this.ScanButton.Size = new System.Drawing.Size(40, 23);
            this.ScanButton.TabIndex = 39;
            this.ScanButton.Text = "Scan";
            this.ScanButton.UseVisualStyleBackColor = true;
            this.ScanButton.Click += new System.EventHandler(this.ScanBtn_Click_1);
            // 
            // HDCheckBox
            // 
            this.HDCheckBox.AutoSize = true;
            this.HDCheckBox.Enabled = false;
            this.HDCheckBox.Location = new System.Drawing.Point(5, 138);
            this.HDCheckBox.Name = "HDCheckBox";
            this.HDCheckBox.Size = new System.Drawing.Size(86, 17);
            this.HDCheckBox.TabIndex = 51;
            this.HDCheckBox.Text = "High Density";
            this.HDCheckBox.UseVisualStyleBackColor = true;
            this.HDCheckBox.CheckedChanged += new System.EventHandler(this.HDCheckBox_CheckedChanged);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(3, 19);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(85, 13);
            this.label37.TabIndex = 44;
            this.label37.Text = "MFM processing";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(126, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 45;
            this.label2.Text = "Period > 8us";
            // 
            // ProcessBtn
            // 
            this.ProcessBtn.Location = new System.Drawing.Point(6, 6);
            this.ProcessBtn.Name = "ProcessBtn";
            this.ProcessBtn.Size = new System.Drawing.Size(75, 40);
            this.ProcessBtn.TabIndex = 4;
            this.ProcessBtn.Text = "Process Amiga!";
            this.ProcessBtn.UseVisualStyleBackColor = true;
            this.ProcessBtn.Click += new System.EventHandler(this.ProcessAmigaBtn_Click);
            // 
            // ProcessPCBtn
            // 
            this.ProcessPCBtn.Location = new System.Drawing.Point(6, 52);
            this.ProcessPCBtn.Name = "ProcessPCBtn";
            this.ProcessPCBtn.Size = new System.Drawing.Size(75, 40);
            this.ProcessPCBtn.TabIndex = 3;
            this.ProcessPCBtn.Text = "Process PC!";
            this.ProcessPCBtn.UseVisualStyleBackColor = true;
            this.ProcessPCBtn.Click += new System.EventHandler(this.ProcessPCBtn_Click);
            // 
            // ErrorCorrectionTab
            // 
            this.ErrorCorrectionTab.BackColor = System.Drawing.SystemColors.Control;
            this.ErrorCorrectionTab.Controls.Add(this.CombinationsUpDown);
            this.ErrorCorrectionTab.Controls.Add(this.label79);
            this.ErrorCorrectionTab.Controls.Add(this.button44);
            this.ErrorCorrectionTab.Controls.Add(this.ECMFMByteEncbutton);
            this.ErrorCorrectionTab.Controls.Add(this.MFMByteLengthUpDown);
            this.ErrorCorrectionTab.Controls.Add(this.MFMByteStartUpDown);
            this.ErrorCorrectionTab.Controls.Add(this.label76);
            this.ErrorCorrectionTab.Controls.Add(this.label77);
            this.ErrorCorrectionTab.Controls.Add(this.button38);
            this.ErrorCorrectionTab.Controls.Add(this.ECMFMcheckBox);
            this.ErrorCorrectionTab.Controls.Add(this.label71);
            this.ErrorCorrectionTab.Controls.Add(this.label6);
            this.ErrorCorrectionTab.Controls.Add(this.C8StartUpDown);
            this.ErrorCorrectionTab.Controls.Add(this.C6StartUpDown);
            this.ErrorCorrectionTab.Controls.Add(this.button1);
            this.ErrorCorrectionTab.Controls.Add(this.BadSectorsCheckBox);
            this.ErrorCorrectionTab.Controls.Add(this.GoodSectorsCheckBox);
            this.ErrorCorrectionTab.Controls.Add(this.ECRealign4E);
            this.ErrorCorrectionTab.Controls.Add(this.ECInfoTabs);
            this.ErrorCorrectionTab.Controls.Add(this.ECZoomOutBtn);
            this.ErrorCorrectionTab.Controls.Add(this.SelectionDifLabel);
            this.ErrorCorrectionTab.Controls.Add(this.ScatterOffsetUpDown);
            this.ErrorCorrectionTab.Controls.Add(this.ScatterMinUpDown);
            this.ErrorCorrectionTab.Controls.Add(this.ScatterMaxUpDown);
            this.ErrorCorrectionTab.Controls.Add(this.ScatterMaxTrackBar);
            this.ErrorCorrectionTab.Controls.Add(this.ScatterMinTrackBar);
            this.ErrorCorrectionTab.Controls.Add(this.groupBox2);
            this.ErrorCorrectionTab.Controls.Add(this.RedCrcCheckLabel);
            this.ErrorCorrectionTab.Controls.Add(this.label43);
            this.ErrorCorrectionTab.Controls.Add(this.BSEditByteLabel);
            this.ErrorCorrectionTab.Controls.Add(this.BluetoRedByteCopyToolBtn);
            this.ErrorCorrectionTab.Controls.Add(this.CopySectorToBlueBtn);
            this.ErrorCorrectionTab.Controls.Add(this.label55);
            this.ErrorCorrectionTab.Controls.Add(this.panel4);
            this.ErrorCorrectionTab.Controls.Add(this.panel3);
            this.ErrorCorrectionTab.Controls.Add(this.label54);
            this.ErrorCorrectionTab.Controls.Add(this.label53);
            this.ErrorCorrectionTab.Controls.Add(this.BadSectorListBox);
            this.ErrorCorrectionTab.Controls.Add(this.Sector2UpDown);
            this.ErrorCorrectionTab.Controls.Add(this.Track2UpDown);
            this.ErrorCorrectionTab.Controls.Add(this.label48);
            this.ErrorCorrectionTab.Controls.Add(this.label49);
            this.ErrorCorrectionTab.Controls.Add(this.Sector1UpDown);
            this.ErrorCorrectionTab.Controls.Add(this.Track1UpDown);
            this.ErrorCorrectionTab.Controls.Add(this.BlueCrcCheckLabel);
            this.ErrorCorrectionTab.Controls.Add(this.label47);
            this.ErrorCorrectionTab.Controls.Add(this.ECSectorOverlayBtn);
            this.ErrorCorrectionTab.Controls.Add(this.BadSectorPanel);
            this.ErrorCorrectionTab.Controls.Add(this.ScatterOffsetTrackBar);
            this.ErrorCorrectionTab.ImageIndex = 1;
            this.ErrorCorrectionTab.Location = new System.Drawing.Point(4, 23);
            this.ErrorCorrectionTab.Name = "ErrorCorrectionTab";
            this.ErrorCorrectionTab.Padding = new System.Windows.Forms.Padding(3);
            this.ErrorCorrectionTab.Size = new System.Drawing.Size(966, 775);
            this.ErrorCorrectionTab.TabIndex = 2;
            this.ErrorCorrectionTab.Text = "Error Correction";
            // 
            // CombinationsUpDown
            // 
            this.CombinationsUpDown.Location = new System.Drawing.Point(334, 297);
            this.CombinationsUpDown.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.CombinationsUpDown.Name = "CombinationsUpDown";
            this.CombinationsUpDown.Size = new System.Drawing.Size(77, 20);
            this.CombinationsUpDown.TabIndex = 4027;
            this.CombinationsUpDown.Value = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Location = new System.Drawing.Point(259, 301);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(70, 13);
            this.label79.TabIndex = 4026;
            this.label79.Text = "Combinations";
            // 
            // button44
            // 
            this.button44.Location = new System.Drawing.Point(791, 495);
            this.button44.Name = "button44";
            this.button44.Size = new System.Drawing.Size(78, 39);
            this.button44.TabIndex = 4025;
            this.button44.Text = "Iterator test";
            this.button44.UseVisualStyleBackColor = true;
            this.button44.Click += new System.EventHandler(this.button44_Click);
            // 
            // ECMFMByteEncbutton
            // 
            this.ECMFMByteEncbutton.Location = new System.Drawing.Point(7, 271);
            this.ECMFMByteEncbutton.Name = "ECMFMByteEncbutton";
            this.ECMFMByteEncbutton.Size = new System.Drawing.Size(78, 39);
            this.ECMFMByteEncbutton.TabIndex = 4024;
            this.ECMFMByteEncbutton.Text = "EC MFM byte enc";
            this.ECMFMByteEncbutton.UseVisualStyleBackColor = true;
            this.ECMFMByteEncbutton.Click += new System.EventHandler(this.ECMFMByteEncbutton_Click);
            // 
            // MFMByteLengthUpDown
            // 
            this.MFMByteLengthUpDown.Location = new System.Drawing.Point(186, 297);
            this.MFMByteLengthUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.MFMByteLengthUpDown.Name = "MFMByteLengthUpDown";
            this.MFMByteLengthUpDown.Size = new System.Drawing.Size(59, 20);
            this.MFMByteLengthUpDown.TabIndex = 4022;
            this.MFMByteLengthUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // MFMByteStartUpDown
            // 
            this.MFMByteStartUpDown.Location = new System.Drawing.Point(186, 271);
            this.MFMByteStartUpDown.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.MFMByteStartUpDown.Name = "MFMByteStartUpDown";
            this.MFMByteStartUpDown.Size = new System.Drawing.Size(59, 20);
            this.MFMByteStartUpDown.TabIndex = 4020;
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(97, 299);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(91, 13);
            this.label76.TabIndex = 4021;
            this.label76.Text = "MFM Byte Length";
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Location = new System.Drawing.Point(100, 273);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(79, 13);
            this.label77.TabIndex = 4023;
            this.label77.Text = "MFM Start byte";
            // 
            // button38
            // 
            this.button38.Location = new System.Drawing.Point(791, 450);
            this.button38.Name = "button38";
            this.button38.Size = new System.Drawing.Size(78, 39);
            this.button38.TabIndex = 4019;
            this.button38.Text = "Show mfmbyteenc";
            this.button38.UseVisualStyleBackColor = true;
            this.button38.Click += new System.EventHandler(this.button38_Click);
            // 
            // ECMFMcheckBox
            // 
            this.ECMFMcheckBox.AutoSize = true;
            this.ECMFMcheckBox.Checked = true;
            this.ECMFMcheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ECMFMcheckBox.Location = new System.Drawing.Point(196, 157);
            this.ECMFMcheckBox.Name = "ECMFMcheckBox";
            this.ECMFMcheckBox.Size = new System.Drawing.Size(50, 17);
            this.ECMFMcheckBox.TabIndex = 4018;
            this.ECMFMcheckBox.Text = "MFM";
            this.ECMFMcheckBox.UseVisualStyleBackColor = true;
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Location = new System.Drawing.Point(235, 222);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(43, 13);
            this.label71.TabIndex = 4017;
            this.label71.Text = "C8 start";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(180, 222);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 4016;
            this.label6.Text = "C6 start";
            // 
            // C8StartUpDown
            // 
            this.C8StartUpDown.Location = new System.Drawing.Point(238, 237);
            this.C8StartUpDown.Name = "C8StartUpDown";
            this.C8StartUpDown.Size = new System.Drawing.Size(40, 20);
            this.C8StartUpDown.TabIndex = 4015;
            // 
            // C6StartUpDown
            // 
            this.C6StartUpDown.Location = new System.Drawing.Point(180, 237);
            this.C6StartUpDown.Name = "C6StartUpDown";
            this.C6StartUpDown.Size = new System.Drawing.Size(43, 20);
            this.C6StartUpDown.TabIndex = 4014;
            this.C6StartUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(89, 226);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 39);
            this.button1.TabIndex = 4013;
            this.button1.Text = "Process Cluster2";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BadSectorsCheckBox
            // 
            this.BadSectorsCheckBox.AutoSize = true;
            this.BadSectorsCheckBox.Checked = true;
            this.BadSectorsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.BadSectorsCheckBox.Location = new System.Drawing.Point(196, 134);
            this.BadSectorsCheckBox.Name = "BadSectorsCheckBox";
            this.BadSectorsCheckBox.Size = new System.Drawing.Size(82, 17);
            this.BadSectorsCheckBox.TabIndex = 4012;
            this.BadSectorsCheckBox.Text = "Bad sectors";
            this.BadSectorsCheckBox.UseVisualStyleBackColor = true;
            // 
            // GoodSectorsCheckBox
            // 
            this.GoodSectorsCheckBox.AutoSize = true;
            this.GoodSectorsCheckBox.Location = new System.Drawing.Point(196, 116);
            this.GoodSectorsCheckBox.Name = "GoodSectorsCheckBox";
            this.GoodSectorsCheckBox.Size = new System.Drawing.Size(89, 17);
            this.GoodSectorsCheckBox.TabIndex = 4011;
            this.GoodSectorsCheckBox.Text = "Good sectors";
            this.GoodSectorsCheckBox.UseVisualStyleBackColor = true;
            // 
            // ECRealign4E
            // 
            this.ECRealign4E.Location = new System.Drawing.Point(89, 181);
            this.ECRealign4E.Name = "ECRealign4E";
            this.ECRealign4E.Size = new System.Drawing.Size(78, 39);
            this.ECRealign4E.TabIndex = 4010;
            this.ECRealign4E.Text = "Realign 4E";
            this.ECRealign4E.UseVisualStyleBackColor = true;
            this.ECRealign4E.Click += new System.EventHandler(this.ECRealign4E_Click);
            // 
            // ECInfoTabs
            // 
            this.ECInfoTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ECInfoTabs.Controls.Add(this.ECTabSectorData);
            this.ECInfoTabs.Controls.Add(this.tabPage8);
            this.ECInfoTabs.Location = new System.Drawing.Point(6, 316);
            this.ECInfoTabs.Name = "ECInfoTabs";
            this.ECInfoTabs.SelectedIndex = 0;
            this.ECInfoTabs.Size = new System.Drawing.Size(452, 450);
            this.ECInfoTabs.TabIndex = 4009;
            // 
            // ECTabSectorData
            // 
            this.ECTabSectorData.Controls.Add(this.antbSectorData);
            this.ECTabSectorData.Location = new System.Drawing.Point(4, 22);
            this.ECTabSectorData.Name = "ECTabSectorData";
            this.ECTabSectorData.Padding = new System.Windows.Forms.Padding(3);
            this.ECTabSectorData.Size = new System.Drawing.Size(444, 424);
            this.ECTabSectorData.TabIndex = 0;
            this.ECTabSectorData.Text = "Sector data";
            this.ECTabSectorData.UseVisualStyleBackColor = true;
            // 
            // antbSectorData
            // 
            this.antbSectorData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.antbSectorData.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.antbSectorData.Location = new System.Drawing.Point(0, 0);
            this.antbSectorData.MaxLength = 200000;
            this.antbSectorData.Multiline = true;
            this.antbSectorData.Name = "antbSectorData";
            this.antbSectorData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.antbSectorData.Size = new System.Drawing.Size(441, 420);
            this.antbSectorData.TabIndex = 86;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.ECtbMFM);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(444, 424);
            this.tabPage8.TabIndex = 1;
            this.tabPage8.Text = "MFM data";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // ECtbMFM
            // 
            this.ECtbMFM.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ECtbMFM.Location = new System.Drawing.Point(0, 0);
            this.ECtbMFM.MaxLength = 200000;
            this.ECtbMFM.Multiline = true;
            this.ECtbMFM.Name = "ECtbMFM";
            this.ECtbMFM.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ECtbMFM.Size = new System.Drawing.Size(441, 394);
            this.ECtbMFM.TabIndex = 88;
            // 
            // ECZoomOutBtn
            // 
            this.ECZoomOutBtn.Location = new System.Drawing.Point(486, 574);
            this.ECZoomOutBtn.Name = "ECZoomOutBtn";
            this.ECZoomOutBtn.Size = new System.Drawing.Size(65, 23);
            this.ECZoomOutBtn.TabIndex = 4007;
            this.ECZoomOutBtn.Text = "Zoom out";
            this.ECZoomOutBtn.UseVisualStyleBackColor = true;
            this.ECZoomOutBtn.Click += new System.EventHandler(this.ECZoomOutBtn_Click);
            // 
            // SelectionDifLabel
            // 
            this.SelectionDifLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectionDifLabel.AutoSize = true;
            this.SelectionDifLabel.Location = new System.Drawing.Point(483, 618);
            this.SelectionDifLabel.Name = "SelectionDifLabel";
            this.SelectionDifLabel.Size = new System.Drawing.Size(45, 13);
            this.SelectionDifLabel.TabIndex = 63;
            this.SelectionDifLabel.Text = "Periods:";
            // 
            // ScatterOffsetUpDown
            // 
            this.ScatterOffsetUpDown.Location = new System.Drawing.Point(557, 574);
            this.ScatterOffsetUpDown.Maximum = new decimal(new int[] {
            40000,
            0,
            0,
            0});
            this.ScatterOffsetUpDown.Minimum = new decimal(new int[] {
            40000,
            0,
            0,
            -2147483648});
            this.ScatterOffsetUpDown.Name = "ScatterOffsetUpDown";
            this.ScatterOffsetUpDown.Size = new System.Drawing.Size(56, 20);
            this.ScatterOffsetUpDown.TabIndex = 4006;
            // 
            // ScatterMinUpDown
            // 
            this.ScatterMinUpDown.Location = new System.Drawing.Point(557, 604);
            this.ScatterMinUpDown.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.ScatterMinUpDown.Minimum = new decimal(new int[] {
            50000,
            0,
            0,
            -2147483648});
            this.ScatterMinUpDown.Name = "ScatterMinUpDown";
            this.ScatterMinUpDown.Size = new System.Drawing.Size(56, 20);
            this.ScatterMinUpDown.TabIndex = 4005;
            // 
            // ScatterMaxUpDown
            // 
            this.ScatterMaxUpDown.Location = new System.Drawing.Point(557, 631);
            this.ScatterMaxUpDown.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.ScatterMaxUpDown.Minimum = new decimal(new int[] {
            50000,
            0,
            0,
            -2147483648});
            this.ScatterMaxUpDown.Name = "ScatterMaxUpDown";
            this.ScatterMaxUpDown.Size = new System.Drawing.Size(58, 20);
            this.ScatterMaxUpDown.TabIndex = 4004;
            this.ScatterMaxUpDown.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // ScatterMaxTrackBar
            // 
            this.ScatterMaxTrackBar.LargeChange = 25;
            this.ScatterMaxTrackBar.Location = new System.Drawing.Point(619, 626);
            this.ScatterMaxTrackBar.Maximum = 10000;
            this.ScatterMaxTrackBar.Minimum = -10000;
            this.ScatterMaxTrackBar.Name = "ScatterMaxTrackBar";
            this.ScatterMaxTrackBar.Size = new System.Drawing.Size(344, 45);
            this.ScatterMaxTrackBar.TabIndex = 103;
            this.ScatterMaxTrackBar.TickFrequency = 10;
            this.ScatterMaxTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.ScatterMaxTrackBar.Value = 10000;
            this.ScatterMaxTrackBar.Scroll += new System.EventHandler(this.BadSectorListBox_SelectedIndexChanged);
            // 
            // ScatterMinTrackBar
            // 
            this.ScatterMinTrackBar.LargeChange = 25;
            this.ScatterMinTrackBar.Location = new System.Drawing.Point(619, 600);
            this.ScatterMinTrackBar.Maximum = 10000;
            this.ScatterMinTrackBar.Minimum = -10000;
            this.ScatterMinTrackBar.Name = "ScatterMinTrackBar";
            this.ScatterMinTrackBar.Size = new System.Drawing.Size(344, 45);
            this.ScatterMinTrackBar.TabIndex = 102;
            this.ScatterMinTrackBar.TickFrequency = 10;
            this.ScatterMinTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.ScatterMinTrackBar.Scroll += new System.EventHandler(this.BadSectorListBox_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.HistScalingLabel);
            this.groupBox2.Controls.Add(this.AnHistogramPanel);
            this.groupBox2.Location = new System.Drawing.Point(697, 282);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(278, 154);
            this.groupBox2.TabIndex = 101;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Histogram";
            // 
            // HistScalingLabel
            // 
            this.HistScalingLabel.AutoSize = true;
            this.HistScalingLabel.Location = new System.Drawing.Point(6, 131);
            this.HistScalingLabel.Name = "HistScalingLabel";
            this.HistScalingLabel.Size = new System.Drawing.Size(45, 13);
            this.HistScalingLabel.TabIndex = 102;
            this.HistScalingLabel.Text = "Scaling:";
            // 
            // AnHistogramPanel
            // 
            this.AnHistogramPanel.Location = new System.Drawing.Point(6, 19);
            this.AnHistogramPanel.Name = "AnHistogramPanel";
            this.AnHistogramPanel.Size = new System.Drawing.Size(260, 109);
            this.AnHistogramPanel.TabIndex = 36;
            // 
            // RedCrcCheckLabel
            // 
            this.RedCrcCheckLabel.AutoSize = true;
            this.RedCrcCheckLabel.Location = new System.Drawing.Point(108, 79);
            this.RedCrcCheckLabel.Name = "RedCrcCheckLabel";
            this.RedCrcCheckLabel.Size = new System.Drawing.Size(26, 13);
            this.RedCrcCheckLabel.TabIndex = 98;
            this.RedCrcCheckLabel.Text = "Crc:";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(83, 39);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(44, 13);
            this.label43.TabIndex = 97;
            this.label43.Text = "Sector1";
            // 
            // BSEditByteLabel
            // 
            this.BSEditByteLabel.AutoSize = true;
            this.BSEditByteLabel.Location = new System.Drawing.Point(235, 68);
            this.BSEditByteLabel.Name = "BSEditByteLabel";
            this.BSEditByteLabel.Size = new System.Drawing.Size(52, 13);
            this.BSEditByteLabel.TabIndex = 96;
            this.BSEditByteLabel.Text = "Byte: 512";
            // 
            // BluetoRedByteCopyToolBtn
            // 
            this.BluetoRedByteCopyToolBtn.Location = new System.Drawing.Point(5, 226);
            this.BluetoRedByteCopyToolBtn.Name = "BluetoRedByteCopyToolBtn";
            this.BluetoRedByteCopyToolBtn.Size = new System.Drawing.Size(78, 39);
            this.BluetoRedByteCopyToolBtn.TabIndex = 93;
            this.BluetoRedByteCopyToolBtn.Tag = "1";
            this.BluetoRedByteCopyToolBtn.Text = "Copy byte to blue";
            this.BluetoRedByteCopyToolBtn.UseVisualStyleBackColor = true;
            this.BluetoRedByteCopyToolBtn.Click += new System.EventHandler(this.BluetoRedByteCopyToolBtn_Click);
            // 
            // CopySectorToBlueBtn
            // 
            this.CopySectorToBlueBtn.Location = new System.Drawing.Point(5, 181);
            this.CopySectorToBlueBtn.Name = "CopySectorToBlueBtn";
            this.CopySectorToBlueBtn.Size = new System.Drawing.Size(78, 39);
            this.CopySectorToBlueBtn.TabIndex = 84;
            this.CopySectorToBlueBtn.Text = "Copy sector to blue";
            this.CopySectorToBlueBtn.UseVisualStyleBackColor = true;
            this.CopySectorToBlueBtn.Click += new System.EventHandler(this.CopySectorToBlueBtn_Click);
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(288, 10);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(73, 13);
            this.label55.TabIndex = 92;
            this.label55.Text = "Bad sector list";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(208)))), ((int)(((byte)(192)))));
            this.panel4.Controls.Add(this.BSRedTempRadio);
            this.panel4.Controls.Add(this.BSRedFromlistRadio);
            this.panel4.Controls.Add(this.radioButton6);
            this.panel4.Location = new System.Drawing.Point(106, 95);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(78, 62);
            this.panel4.TabIndex = 91;
            // 
            // BSRedTempRadio
            // 
            this.BSRedTempRadio.AutoSize = true;
            this.BSRedTempRadio.Location = new System.Drawing.Point(4, 39);
            this.BSRedTempRadio.Name = "BSRedTempRadio";
            this.BSRedTempRadio.Size = new System.Drawing.Size(52, 17);
            this.BSRedTempRadio.TabIndex = 0;
            this.BSRedTempRadio.Text = "Temp";
            this.BSRedTempRadio.UseVisualStyleBackColor = true;
            // 
            // BSRedFromlistRadio
            // 
            this.BSRedFromlistRadio.AutoSize = true;
            this.BSRedFromlistRadio.Checked = true;
            this.BSRedFromlistRadio.Location = new System.Drawing.Point(4, 20);
            this.BSRedFromlistRadio.Name = "BSRedFromlistRadio";
            this.BSRedFromlistRadio.Size = new System.Drawing.Size(63, 17);
            this.BSRedFromlistRadio.TabIndex = 0;
            this.BSRedFromlistRadio.TabStop = true;
            this.BSRedFromlistRadio.Text = "From list";
            this.BSRedFromlistRadio.UseVisualStyleBackColor = true;
            // 
            // radioButton6
            // 
            this.radioButton6.AutoSize = true;
            this.radioButton6.Location = new System.Drawing.Point(5, 1);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(14, 13);
            this.radioButton6.TabIndex = 0;
            this.radioButton6.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.panel3.Controls.Add(this.BlueTempRadio);
            this.panel3.Controls.Add(this.BSBlueFromListRadio);
            this.panel3.Controls.Add(this.BSBlueSectormapRadio);
            this.panel3.Location = new System.Drawing.Point(2, 95);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(78, 62);
            this.panel3.TabIndex = 90;
            // 
            // BlueTempRadio
            // 
            this.BlueTempRadio.AutoSize = true;
            this.BlueTempRadio.Location = new System.Drawing.Point(4, 39);
            this.BlueTempRadio.Name = "BlueTempRadio";
            this.BlueTempRadio.Size = new System.Drawing.Size(52, 17);
            this.BlueTempRadio.TabIndex = 0;
            this.BlueTempRadio.Text = "Temp";
            this.BlueTempRadio.UseVisualStyleBackColor = true;
            this.BlueTempRadio.CheckedChanged += new System.EventHandler(this.BlueTempRadio_CheckedChanged);
            // 
            // BSBlueFromListRadio
            // 
            this.BSBlueFromListRadio.AutoSize = true;
            this.BSBlueFromListRadio.Checked = true;
            this.BSBlueFromListRadio.Location = new System.Drawing.Point(4, 20);
            this.BSBlueFromListRadio.Name = "BSBlueFromListRadio";
            this.BSBlueFromListRadio.Size = new System.Drawing.Size(63, 17);
            this.BSBlueFromListRadio.TabIndex = 0;
            this.BSBlueFromListRadio.TabStop = true;
            this.BSBlueFromListRadio.Text = "From list";
            this.BSBlueFromListRadio.UseVisualStyleBackColor = true;
            this.BSBlueFromListRadio.CheckedChanged += new System.EventHandler(this.BSBlueFromListRadio_CheckedChanged);
            // 
            // BSBlueSectormapRadio
            // 
            this.BSBlueSectormapRadio.AutoSize = true;
            this.BSBlueSectormapRadio.Location = new System.Drawing.Point(5, 1);
            this.BSBlueSectormapRadio.Name = "BSBlueSectormapRadio";
            this.BSBlueSectormapRadio.Size = new System.Drawing.Size(76, 17);
            this.BSBlueSectormapRadio.TabIndex = 0;
            this.BSBlueSectormapRadio.Text = "Sectormap";
            this.BSBlueSectormapRadio.UseVisualStyleBackColor = true;
            this.BSBlueSectormapRadio.CheckedChanged += new System.EventHandler(this.BSBlueSectormapRadio_CheckedChanged);
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(108, 66);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(27, 13);
            this.label54.TabIndex = 89;
            this.label54.Text = "Red";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(17, 66);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(28, 13);
            this.label53.TabIndex = 88;
            this.label53.Text = "Blue";
            // 
            // BadSectorListBox
            // 
            this.BadSectorListBox.FormattingEnabled = true;
            this.BadSectorListBox.Location = new System.Drawing.Point(291, 25);
            this.BadSectorListBox.Name = "BadSectorListBox";
            this.BadSectorListBox.ScrollAlwaysVisible = true;
            this.BadSectorListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.BadSectorListBox.Size = new System.Drawing.Size(134, 251);
            this.BadSectorListBox.TabIndex = 87;
            this.BadSectorListBox.SelectedIndexChanged += new System.EventHandler(this.BadSectorListBox_SelectedIndexChanged);
            this.BadSectorListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BadSectorListBox_KeyDown);
            // 
            // Sector2UpDown
            // 
            this.Sector2UpDown.Location = new System.Drawing.Point(234, 35);
            this.Sector2UpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.Sector2UpDown.Name = "Sector2UpDown";
            this.Sector2UpDown.Size = new System.Drawing.Size(39, 20);
            this.Sector2UpDown.TabIndex = 84;
            this.Sector2UpDown.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // Track2UpDown
            // 
            this.Track2UpDown.Location = new System.Drawing.Point(234, 9);
            this.Track2UpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.Track2UpDown.Name = "Track2UpDown";
            this.Track2UpDown.Size = new System.Drawing.Size(39, 20);
            this.Track2UpDown.TabIndex = 82;
            this.Track2UpDown.Value = new decimal(new int[] {
            160,
            0,
            0,
            0});
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(190, 37);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(44, 13);
            this.label48.TabIndex = 83;
            this.label48.Text = "Sector2";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(193, 11);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(41, 13);
            this.label49.TabIndex = 84;
            this.label49.Text = "Track2";
            // 
            // Sector1UpDown
            // 
            this.Sector1UpDown.Location = new System.Drawing.Point(133, 37);
            this.Sector1UpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.Sector1UpDown.Name = "Sector1UpDown";
            this.Sector1UpDown.Size = new System.Drawing.Size(39, 20);
            this.Sector1UpDown.TabIndex = 83;
            this.Sector1UpDown.ValueChanged += new System.EventHandler(this.Sector1UpDown_ValueChanged);
            // 
            // Track1UpDown
            // 
            this.Track1UpDown.Location = new System.Drawing.Point(133, 11);
            this.Track1UpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.Track1UpDown.Name = "Track1UpDown";
            this.Track1UpDown.Size = new System.Drawing.Size(39, 20);
            this.Track1UpDown.TabIndex = 81;
            this.Track1UpDown.ValueChanged += new System.EventHandler(this.Track1UpDown_ValueChanged);
            // 
            // BlueCrcCheckLabel
            // 
            this.BlueCrcCheckLabel.AutoSize = true;
            this.BlueCrcCheckLabel.Location = new System.Drawing.Point(4, 79);
            this.BlueCrcCheckLabel.Name = "BlueCrcCheckLabel";
            this.BlueCrcCheckLabel.Size = new System.Drawing.Size(26, 13);
            this.BlueCrcCheckLabel.TabIndex = 79;
            this.BlueCrcCheckLabel.Text = "Crc:";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(92, 13);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(41, 13);
            this.label47.TabIndex = 80;
            this.label47.Text = "Track1";
            // 
            // ECSectorOverlayBtn
            // 
            this.ECSectorOverlayBtn.Location = new System.Drawing.Point(6, 6);
            this.ECSectorOverlayBtn.Name = "ECSectorOverlayBtn";
            this.ECSectorOverlayBtn.Size = new System.Drawing.Size(67, 38);
            this.ECSectorOverlayBtn.TabIndex = 12;
            this.ECSectorOverlayBtn.Text = "Sector overlay";
            this.ECSectorOverlayBtn.UseVisualStyleBackColor = true;
            this.ECSectorOverlayBtn.Click += new System.EventHandler(this.ECSectorOverlayBtn_Click);
            // 
            // BadSectorPanel
            // 
            this.BadSectorPanel.Controls.Add(this.BadSectorTooltip);
            this.BadSectorPanel.Location = new System.Drawing.Point(430, 6);
            this.BadSectorPanel.Name = "BadSectorPanel";
            this.BadSectorPanel.Size = new System.Drawing.Size(518, 270);
            this.BadSectorPanel.TabIndex = 0;
            this.BadSectorPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.BadSectorPictureBox_Paint);
            this.BadSectorPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BadSectorPanel_MouseDown);
            this.BadSectorPanel.MouseLeave += new System.EventHandler(this.BadSectorPanel_MouseLeave);
            this.BadSectorPanel.MouseHover += new System.EventHandler(this.BadSectorPanel_MouseHover);
            this.BadSectorPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BadSectorPanel_MouseMove);
            // 
            // BadSectorTooltip
            // 
            this.BadSectorTooltip.AutoSize = true;
            this.BadSectorTooltip.BackColor = System.Drawing.Color.White;
            this.BadSectorTooltip.Location = new System.Drawing.Point(482, 257);
            this.BadSectorTooltip.Name = "BadSectorTooltip";
            this.BadSectorTooltip.Size = new System.Drawing.Size(33, 13);
            this.BadSectorTooltip.TabIndex = 88;
            this.BadSectorTooltip.Text = "Label";
            // 
            // ScatterOffsetTrackBar
            // 
            this.ScatterOffsetTrackBar.LargeChange = 25;
            this.ScatterOffsetTrackBar.Location = new System.Drawing.Point(619, 564);
            this.ScatterOffsetTrackBar.Maximum = 4000;
            this.ScatterOffsetTrackBar.Minimum = -4000;
            this.ScatterOffsetTrackBar.Name = "ScatterOffsetTrackBar";
            this.ScatterOffsetTrackBar.Size = new System.Drawing.Size(344, 45);
            this.ScatterOffsetTrackBar.TabIndex = 104;
            this.ScatterOffsetTrackBar.TickFrequency = 2000;
            this.ScatterOffsetTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.ScatterOffsetTrackBar.Scroll += new System.EventHandler(this.BadSectorListBox_SelectedIndexChanged);
            // 
            // AnalysisPage
            // 
            this.AnalysisPage.BackColor = System.Drawing.SystemColors.Control;
            this.AnalysisPage.Controls.Add(this.button20);
            this.AnalysisPage.Controls.Add(this.groupBox8);
            this.AnalysisPage.Controls.Add(this.AntxtBox);
            this.AnalysisPage.Controls.Add(this.button25);
            this.AnalysisPage.Controls.Add(this.button26);
            this.AnalysisPage.Controls.Add(this.button23);
            this.AnalysisPage.Controls.Add(this.button21);
            this.AnalysisPage.Controls.Add(this.tbMFM);
            this.AnalysisPage.Controls.Add(this.button2);
            this.AnalysisPage.Controls.Add(this.ConvertToMFMBtn);
            this.AnalysisPage.Controls.Add(this.tbBIN);
            this.AnalysisPage.Controls.Add(this.tbTest);
            this.AnalysisPage.ImageIndex = 5;
            this.AnalysisPage.Location = new System.Drawing.Point(4, 23);
            this.AnalysisPage.Name = "AnalysisPage";
            this.AnalysisPage.Padding = new System.Windows.Forms.Padding(3);
            this.AnalysisPage.Size = new System.Drawing.Size(966, 775);
            this.AnalysisPage.TabIndex = 3;
            this.AnalysisPage.Text = "Analysis";
            // 
            // button20
            // 
            this.button20.Location = new System.Drawing.Point(9, 209);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(71, 38);
            this.button20.TabIndex = 68;
            this.button20.Text = "Amiga Checksum";
            this.button20.UseVisualStyleBackColor = true;
            this.button20.Click += new System.EventHandler(this.button20_Click_1);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.AmigaMFMRadio);
            this.groupBox8.Controls.Add(this.ANAmigaDiskSpareRadio);
            this.groupBox8.Controls.Add(this.ANAmigaRadio);
            this.groupBox8.Controls.Add(this.ANPCRadio);
            this.groupBox8.Location = new System.Drawing.Point(9, 335);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(165, 118);
            this.groupBox8.TabIndex = 67;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Platform";
            // 
            // AmigaMFMRadio
            // 
            this.AmigaMFMRadio.AutoSize = true;
            this.AmigaMFMRadio.Checked = true;
            this.AmigaMFMRadio.Location = new System.Drawing.Point(6, 62);
            this.AmigaMFMRadio.Name = "AmigaMFMRadio";
            this.AmigaMFMRadio.Size = new System.Drawing.Size(78, 17);
            this.AmigaMFMRadio.TabIndex = 3;
            this.AmigaMFMRadio.TabStop = true;
            this.AmigaMFMRadio.Text = "AmigaMFM";
            this.AmigaMFMRadio.UseVisualStyleBackColor = true;
            // 
            // ANAmigaDiskSpareRadio
            // 
            this.ANAmigaDiskSpareRadio.AutoSize = true;
            this.ANAmigaDiskSpareRadio.Location = new System.Drawing.Point(6, 85);
            this.ANAmigaDiskSpareRadio.Name = "ANAmigaDiskSpareRadio";
            this.ANAmigaDiskSpareRadio.Size = new System.Drawing.Size(69, 17);
            this.ANAmigaDiskSpareRadio.TabIndex = 2;
            this.ANAmigaDiskSpareRadio.Text = "AmigaDS";
            this.ANAmigaDiskSpareRadio.UseVisualStyleBackColor = true;
            // 
            // ANAmigaRadio
            // 
            this.ANAmigaRadio.AutoSize = true;
            this.ANAmigaRadio.Location = new System.Drawing.Point(6, 39);
            this.ANAmigaRadio.Name = "ANAmigaRadio";
            this.ANAmigaRadio.Size = new System.Drawing.Size(126, 17);
            this.ANAmigaRadio.TabIndex = 1;
            this.ANAmigaRadio.Text = "HexEncMFMToBytes";
            this.ANAmigaRadio.UseVisualStyleBackColor = true;
            // 
            // ANPCRadio
            // 
            this.ANPCRadio.AutoSize = true;
            this.ANPCRadio.Location = new System.Drawing.Point(6, 16);
            this.ANPCRadio.Name = "ANPCRadio";
            this.ANPCRadio.Size = new System.Drawing.Size(39, 17);
            this.ANPCRadio.TabIndex = 0;
            this.ANPCRadio.Text = "PC";
            this.ANPCRadio.UseVisualStyleBackColor = true;
            // 
            // AntxtBox
            // 
            this.AntxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AntxtBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AntxtBox.Location = new System.Drawing.Point(587, 335);
            this.AntxtBox.MaxLength = 200000;
            this.AntxtBox.Multiline = true;
            this.AntxtBox.Name = "AntxtBox";
            this.AntxtBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.AntxtBox.Size = new System.Drawing.Size(390, 324);
            this.AntxtBox.TabIndex = 66;
            // 
            // button25
            // 
            this.button25.Location = new System.Drawing.Point(9, 154);
            this.button25.Name = "button25";
            this.button25.Size = new System.Drawing.Size(71, 38);
            this.button25.TabIndex = 65;
            this.button25.Text = "Good Hex CRC check";
            this.button25.UseVisualStyleBackColor = true;
            this.button25.Click += new System.EventHandler(this.button25_Click);
            // 
            // button26
            // 
            this.button26.Location = new System.Drawing.Point(9, 110);
            this.button26.Name = "button26";
            this.button26.Size = new System.Drawing.Size(71, 38);
            this.button26.TabIndex = 64;
            this.button26.Text = "Good Ascii CRC check";
            this.button26.UseVisualStyleBackColor = true;
            this.button26.Click += new System.EventHandler(this.button26_Click);
            // 
            // button23
            // 
            this.button23.Location = new System.Drawing.Point(83, 50);
            this.button23.Name = "button23";
            this.button23.Size = new System.Drawing.Size(71, 38);
            this.button23.TabIndex = 63;
            this.button23.Text = "Hex CRC check";
            this.button23.UseVisualStyleBackColor = true;
            this.button23.Click += new System.EventHandler(this.button23_Click);
            // 
            // button21
            // 
            this.button21.Location = new System.Drawing.Point(83, 6);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(71, 38);
            this.button21.TabIndex = 62;
            this.button21.Text = "Ascii CRC check";
            this.button21.UseVisualStyleBackColor = true;
            this.button21.Click += new System.EventHandler(this.AsciiCrcCheckBtn_Click);
            // 
            // tbMFM
            // 
            this.tbMFM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMFM.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMFM.Location = new System.Drawing.Point(191, 336);
            this.tbMFM.MaxLength = 200000;
            this.tbMFM.Multiline = true;
            this.tbMFM.Name = "tbMFM";
            this.tbMFM.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbMFM.Size = new System.Drawing.Size(390, 318);
            this.tbMFM.TabIndex = 61;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 50);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(71, 38);
            this.button2.TabIndex = 60;
            this.button2.Text = "Convert to BIN";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ConvertToMFMBtn
            // 
            this.ConvertToMFMBtn.Location = new System.Drawing.Point(6, 6);
            this.ConvertToMFMBtn.Name = "ConvertToMFMBtn";
            this.ConvertToMFMBtn.Size = new System.Drawing.Size(71, 38);
            this.ConvertToMFMBtn.TabIndex = 59;
            this.ConvertToMFMBtn.Text = "Convert to MFM";
            this.ConvertToMFMBtn.UseVisualStyleBackColor = true;
            this.ConvertToMFMBtn.Click += new System.EventHandler(this.ConvertToMFMBtn_Click);
            // 
            // tbBIN
            // 
            this.tbBIN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBIN.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbBIN.Location = new System.Drawing.Point(191, 6);
            this.tbBIN.MaxLength = 200000;
            this.tbBIN.Multiline = true;
            this.tbBIN.Name = "tbBIN";
            this.tbBIN.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbBIN.Size = new System.Drawing.Size(390, 324);
            this.tbBIN.TabIndex = 58;
            this.tbBIN.Text = "Test";
            // 
            // tbTest
            // 
            this.tbTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTest.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTest.Location = new System.Drawing.Point(585, 6);
            this.tbTest.MaxLength = 200000;
            this.tbTest.Multiline = true;
            this.tbTest.Name = "tbTest";
            this.tbTest.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbTest.Size = new System.Drawing.Size(390, 324);
            this.tbTest.TabIndex = 57;
            // 
            // AnalysisTab2
            // 
            this.AnalysisTab2.BackColor = System.Drawing.SystemColors.Control;
            this.AnalysisTab2.Controls.Add(this.rxbufOffsetLabel);
            this.AnalysisTab2.Controls.Add(this.label80);
            this.AnalysisTab2.Controls.Add(this.ThresholdTestUpDown);
            this.AnalysisTab2.Controls.Add(this.DiffTest2UpDown);
            this.AnalysisTab2.Controls.Add(this.DiffTestUpDown);
            this.AnalysisTab2.Controls.Add(this.button34);
            this.AnalysisTab2.Controls.Add(this.AnAutoUpdateCheckBox);
            this.AnalysisTab2.Controls.Add(this.button31);
            this.AnalysisTab2.Controls.Add(this.button3);
            this.AnalysisTab2.Controls.Add(this.label73);
            this.AnalysisTab2.Controls.Add(this.PeriodExtendUpDown);
            this.AnalysisTab2.Controls.Add(this.EditOptioncomboBox);
            this.AnalysisTab2.Controls.Add(this.EditModecomboBox);
            this.AnalysisTab2.Controls.Add(this.HighpassThresholdUpDown);
            this.AnalysisTab2.Controls.Add(this.button33);
            this.AnalysisTab2.Controls.Add(this.Undolevelslabel);
            this.AnalysisTab2.Controls.Add(this.Lowpassbutton);
            this.AnalysisTab2.Controls.Add(this.DCOffsetbutton);
            this.AnalysisTab2.Controls.Add(this.label70);
            this.AnalysisTab2.Controls.Add(this.button32);
            this.AnalysisTab2.Controls.Add(this.SaveWaveformButton);
            this.AnalysisTab2.Controls.Add(this.EditUndobutton);
            this.AnalysisTab2.Controls.Add(this.AdaptLookAheadUpDown);
            this.AnalysisTab2.Controls.Add(this.DiffMinDeviation2UpDown);
            this.AnalysisTab2.Controls.Add(this.button18);
            this.AnalysisTab2.Controls.Add(this.DiffOffsetUpDown);
            this.AnalysisTab2.Controls.Add(this.InvertcheckBox);
            this.AnalysisTab2.Controls.Add(this.AdaptiveGaincheckBox);
            this.AnalysisTab2.Controls.Add(this.SignalRatioDistUpDown);
            this.AnalysisTab2.Controls.Add(this.DiffMinDeviationUpDown);
            this.AnalysisTab2.Controls.Add(this.SmoothingUpDown);
            this.AnalysisTab2.Controls.Add(this.AnDensityUpDown);
            this.AnalysisTab2.Controls.Add(this.AnReplacerxbufBox);
            this.AnalysisTab2.Controls.Add(this.button19);
            this.AnalysisTab2.Controls.Add(this.DiffDistUpDown2);
            this.AnalysisTab2.Controls.Add(this.label62);
            this.AnalysisTab2.Controls.Add(this.DiffThresholdUpDown);
            this.AnalysisTab2.Controls.Add(this.label61);
            this.AnalysisTab2.Controls.Add(this.DiffGainUpDown);
            this.AnalysisTab2.Controls.Add(this.label60);
            this.AnalysisTab2.Controls.Add(this.DiffDistUpDown);
            this.AnalysisTab2.Controls.Add(this.label52);
            this.AnalysisTab2.Controls.Add(this.GraphFilterButton);
            this.AnalysisTab2.Controls.Add(this.GraphLengthLabel);
            this.AnalysisTab2.Controls.Add(this.GraphXOffsetLabel);
            this.AnalysisTab2.Controls.Add(this.GraphYOffsetlabel);
            this.AnalysisTab2.Controls.Add(this.GraphScaleYLabel);
            this.AnalysisTab2.Controls.Add(this.OpenWavefrmbutton);
            this.AnalysisTab2.Controls.Add(this.groupBox9);
            this.AnalysisTab2.Controls.Add(this.label19);
            this.AnalysisTab2.Controls.Add(this.label4);
            this.AnalysisTab2.Controls.Add(this.label3);
            this.AnalysisTab2.Controls.Add(this.label51);
            this.AnalysisTab2.Controls.Add(this.GraphYScaleTrackBar);
            this.AnalysisTab2.Controls.Add(this.GraphOffsetTrackBar);
            this.AnalysisTab2.Controls.Add(this.GraphPictureBox);
            this.AnalysisTab2.ImageIndex = 4;
            this.AnalysisTab2.Location = new System.Drawing.Point(4, 23);
            this.AnalysisTab2.Name = "AnalysisTab2";
            this.AnalysisTab2.Padding = new System.Windows.Forms.Padding(3);
            this.AnalysisTab2.Size = new System.Drawing.Size(966, 775);
            this.AnalysisTab2.TabIndex = 4;
            this.AnalysisTab2.Text = "Waveform Editor";
            this.AnalysisTab2.Enter += new System.EventHandler(this.AnalysisTab2_Enter_1);
            // 
            // rxbufOffsetLabel
            // 
            this.rxbufOffsetLabel.AutoSize = true;
            this.rxbufOffsetLabel.BackColor = System.Drawing.Color.Transparent;
            this.rxbufOffsetLabel.Location = new System.Drawing.Point(848, 122);
            this.rxbufOffsetLabel.Name = "rxbufOffsetLabel";
            this.rxbufOffsetLabel.Size = new System.Drawing.Size(13, 13);
            this.rxbufOffsetLabel.TabIndex = 166;
            this.rxbufOffsetLabel.Text = "0";
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.BackColor = System.Drawing.Color.Transparent;
            this.label80.Location = new System.Drawing.Point(797, 122);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(50, 13);
            this.label80.TabIndex = 165;
            this.label80.Text = "rxb offset";
            // 
            // ThresholdTestUpDown
            // 
            this.ThresholdTestUpDown.Location = new System.Drawing.Point(202, 9);
            this.ThresholdTestUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.ThresholdTestUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.ThresholdTestUpDown.Name = "ThresholdTestUpDown";
            this.ThresholdTestUpDown.Size = new System.Drawing.Size(48, 20);
            this.ThresholdTestUpDown.TabIndex = 164;
            this.ThresholdTestUpDown.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // DiffTest2UpDown
            // 
            this.DiffTest2UpDown.Location = new System.Drawing.Point(255, 30);
            this.DiffTest2UpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.DiffTest2UpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.DiffTest2UpDown.Name = "DiffTest2UpDown";
            this.DiffTest2UpDown.Size = new System.Drawing.Size(48, 20);
            this.DiffTest2UpDown.TabIndex = 163;
            this.DiffTest2UpDown.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            // 
            // DiffTestUpDown
            // 
            this.DiffTestUpDown.Location = new System.Drawing.Point(255, 9);
            this.DiffTestUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.DiffTestUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.DiffTestUpDown.Name = "DiffTestUpDown";
            this.DiffTestUpDown.Size = new System.Drawing.Size(48, 20);
            this.DiffTestUpDown.TabIndex = 162;
            this.DiffTestUpDown.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            // 
            // button34
            // 
            this.button34.Location = new System.Drawing.Point(676, 109);
            this.button34.Name = "button34";
            this.button34.Size = new System.Drawing.Size(68, 23);
            this.button34.TabIndex = 161;
            this.button34.Text = "Fix8us";
            this.button34.UseVisualStyleBackColor = true;
            this.button34.Click += new System.EventHandler(this.button34_Click);
            // 
            // AnAutoUpdateCheckBox
            // 
            this.AnAutoUpdateCheckBox.AutoSize = true;
            this.AnAutoUpdateCheckBox.Checked = true;
            this.AnAutoUpdateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AnAutoUpdateCheckBox.Location = new System.Drawing.Point(202, 114);
            this.AnAutoUpdateCheckBox.Name = "AnAutoUpdateCheckBox";
            this.AnAutoUpdateCheckBox.Size = new System.Drawing.Size(84, 17);
            this.AnAutoUpdateCheckBox.TabIndex = 160;
            this.AnAutoUpdateCheckBox.Text = "Auto update";
            this.AnAutoUpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // button31
            // 
            this.button31.Location = new System.Drawing.Point(6, 112);
            this.button31.Name = "button31";
            this.button31.Size = new System.Drawing.Size(49, 23);
            this.button31.TabIndex = 159;
            this.button31.Text = "Filter2";
            this.button31.UseVisualStyleBackColor = true;
            this.button31.Click += new System.EventHandler(this.button31_Click_2);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(824, 43);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(68, 23);
            this.button3.TabIndex = 158;
            this.button3.Text = "Lowpass2";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.BackColor = System.Drawing.Color.Transparent;
            this.label73.Location = new System.Drawing.Point(542, 96);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(41, 13);
            this.label73.TabIndex = 157;
            this.label73.Text = "periodx";
            // 
            // PeriodExtendUpDown
            // 
            this.PeriodExtendUpDown.Location = new System.Drawing.Point(545, 113);
            this.PeriodExtendUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.PeriodExtendUpDown.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.PeriodExtendUpDown.Name = "PeriodExtendUpDown";
            this.PeriodExtendUpDown.Size = new System.Drawing.Size(48, 20);
            this.PeriodExtendUpDown.TabIndex = 156;
            this.PeriodExtendUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // EditOptioncomboBox
            // 
            this.EditOptioncomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EditOptioncomboBox.FormattingEnabled = true;
            this.EditOptioncomboBox.Items.AddRange(new object[] {
            "4us",
            "6us",
            "8us"});
            this.EditOptioncomboBox.Location = new System.Drawing.Point(489, 65);
            this.EditOptioncomboBox.Name = "EditOptioncomboBox";
            this.EditOptioncomboBox.Size = new System.Drawing.Size(75, 21);
            this.EditOptioncomboBox.TabIndex = 155;
            // 
            // EditModecomboBox
            // 
            this.EditModecomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EditModecomboBox.FormattingEnabled = true;
            this.EditModecomboBox.Items.AddRange(new object[] {
            "Edit manually",
            "Edit fixd"});
            this.EditModecomboBox.Location = new System.Drawing.Point(408, 65);
            this.EditModecomboBox.Name = "EditModecomboBox";
            this.EditModecomboBox.Size = new System.Drawing.Size(75, 21);
            this.EditModecomboBox.TabIndex = 154;
            // 
            // HighpassThresholdUpDown
            // 
            this.HighpassThresholdUpDown.Location = new System.Drawing.Point(750, 61);
            this.HighpassThresholdUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.HighpassThresholdUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.HighpassThresholdUpDown.Name = "HighpassThresholdUpDown";
            this.HighpassThresholdUpDown.Size = new System.Drawing.Size(48, 20);
            this.HighpassThresholdUpDown.TabIndex = 153;
            this.HighpassThresholdUpDown.Value = new decimal(new int[] {
            33,
            0,
            0,
            0});
            // 
            // button33
            // 
            this.button33.Location = new System.Drawing.Point(676, 61);
            this.button33.Name = "button33";
            this.button33.Size = new System.Drawing.Size(68, 23);
            this.button33.TabIndex = 152;
            this.button33.Text = "Highpass";
            this.button33.UseVisualStyleBackColor = true;
            this.button33.Click += new System.EventHandler(this.button33_Click_1);
            // 
            // Undolevelslabel
            // 
            this.Undolevelslabel.AutoSize = true;
            this.Undolevelslabel.BackColor = System.Drawing.Color.Transparent;
            this.Undolevelslabel.Location = new System.Drawing.Point(459, 93);
            this.Undolevelslabel.Name = "Undolevelslabel";
            this.Undolevelslabel.Size = new System.Drawing.Size(75, 13);
            this.Undolevelslabel.TabIndex = 151;
            this.Undolevelslabel.Text = "Undo levels: 0";
            // 
            // Lowpassbutton
            // 
            this.Lowpassbutton.Location = new System.Drawing.Point(602, 60);
            this.Lowpassbutton.Name = "Lowpassbutton";
            this.Lowpassbutton.Size = new System.Drawing.Size(68, 23);
            this.Lowpassbutton.TabIndex = 150;
            this.Lowpassbutton.Text = "Lowpass";
            this.Lowpassbutton.UseVisualStyleBackColor = true;
            this.Lowpassbutton.Click += new System.EventHandler(this.Lowpassbutton_Click);
            // 
            // DCOffsetbutton
            // 
            this.DCOffsetbutton.Location = new System.Drawing.Point(602, 84);
            this.DCOffsetbutton.Name = "DCOffsetbutton";
            this.DCOffsetbutton.Size = new System.Drawing.Size(68, 23);
            this.DCOffsetbutton.TabIndex = 149;
            this.DCOffsetbutton.Text = "DC offset";
            this.DCOffsetbutton.UseVisualStyleBackColor = true;
            this.DCOffsetbutton.Click += new System.EventHandler(this.button33_Click);
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Location = new System.Drawing.Point(155, 114);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(45, 13);
            this.label70.TabIndex = 148;
            this.label70.Text = "A.Adapt";
            // 
            // button32
            // 
            this.button32.Location = new System.Drawing.Point(602, 108);
            this.button32.Name = "button32";
            this.button32.Size = new System.Drawing.Size(68, 23);
            this.button32.TabIndex = 147;
            this.button32.Text = "Copy G0";
            this.button32.UseVisualStyleBackColor = true;
            this.button32.Click += new System.EventHandler(this.button32_Click);
            // 
            // SaveWaveformButton
            // 
            this.SaveWaveformButton.Location = new System.Drawing.Point(6, 50);
            this.SaveWaveformButton.Name = "SaveWaveformButton";
            this.SaveWaveformButton.Size = new System.Drawing.Size(49, 39);
            this.SaveWaveformButton.TabIndex = 146;
            this.SaveWaveformButton.Text = "Save";
            this.SaveWaveformButton.UseVisualStyleBackColor = true;
            this.SaveWaveformButton.Click += new System.EventHandler(this.SaveWaveformButton_Click);
            // 
            // EditUndobutton
            // 
            this.EditUndobutton.Location = new System.Drawing.Point(464, 108);
            this.EditUndobutton.Name = "EditUndobutton";
            this.EditUndobutton.Size = new System.Drawing.Size(43, 23);
            this.EditUndobutton.TabIndex = 145;
            this.EditUndobutton.Text = "Undo";
            this.EditUndobutton.UseVisualStyleBackColor = true;
            this.EditUndobutton.Click += new System.EventHandler(this.button31_Click_1);
            // 
            // AdaptLookAheadUpDown
            // 
            this.AdaptLookAheadUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.AdaptLookAheadUpDown.Location = new System.Drawing.Point(255, 87);
            this.AdaptLookAheadUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.AdaptLookAheadUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.AdaptLookAheadUpDown.Name = "AdaptLookAheadUpDown";
            this.AdaptLookAheadUpDown.Size = new System.Drawing.Size(48, 20);
            this.AdaptLookAheadUpDown.TabIndex = 144;
            this.AdaptLookAheadUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.AdaptLookAheadUpDown.ValueChanged += new System.EventHandler(this.AdaptLookAheadUpDown_ValueChanged);
            // 
            // DiffMinDeviation2UpDown
            // 
            this.DiffMinDeviation2UpDown.Location = new System.Drawing.Point(255, 60);
            this.DiffMinDeviation2UpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.DiffMinDeviation2UpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.DiffMinDeviation2UpDown.Name = "DiffMinDeviation2UpDown";
            this.DiffMinDeviation2UpDown.Size = new System.Drawing.Size(48, 20);
            this.DiffMinDeviation2UpDown.TabIndex = 143;
            this.DiffMinDeviation2UpDown.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            this.DiffMinDeviation2UpDown.ValueChanged += new System.EventHandler(this.DiffMinDeviation2UpDown_ValueChanged);
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(910, 55);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(49, 39);
            this.button18.TabIndex = 142;
            this.button18.Text = "Offset recalc";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.button18_Click);
            // 
            // DiffOffsetUpDown
            // 
            this.DiffOffsetUpDown.Location = new System.Drawing.Point(410, 111);
            this.DiffOffsetUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.DiffOffsetUpDown.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.DiffOffsetUpDown.Name = "DiffOffsetUpDown";
            this.DiffOffsetUpDown.Size = new System.Drawing.Size(48, 20);
            this.DiffOffsetUpDown.TabIndex = 141;
            this.DiffOffsetUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.DiffOffsetUpDown.ValueChanged += new System.EventHandler(this.DiffOffsetUpDown_ValueChanged);
            // 
            // InvertcheckBox
            // 
            this.InvertcheckBox.AutoSize = true;
            this.InvertcheckBox.Location = new System.Drawing.Point(841, 79);
            this.InvertcheckBox.Name = "InvertcheckBox";
            this.InvertcheckBox.Size = new System.Drawing.Size(53, 17);
            this.InvertcheckBox.TabIndex = 140;
            this.InvertcheckBox.Text = "Invert";
            this.InvertcheckBox.UseVisualStyleBackColor = true;
            // 
            // AdaptiveGaincheckBox
            // 
            this.AdaptiveGaincheckBox.AutoSize = true;
            this.AdaptiveGaincheckBox.Checked = true;
            this.AdaptiveGaincheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AdaptiveGaincheckBox.Location = new System.Drawing.Point(841, 102);
            this.AdaptiveGaincheckBox.Name = "AdaptiveGaincheckBox";
            this.AdaptiveGaincheckBox.Size = new System.Drawing.Size(90, 17);
            this.AdaptiveGaincheckBox.TabIndex = 139;
            this.AdaptiveGaincheckBox.Text = "AdaptiveGain";
            this.AdaptiveGaincheckBox.UseVisualStyleBackColor = true;
            // 
            // SignalRatioDistUpDown
            // 
            this.SignalRatioDistUpDown.DecimalPlaces = 2;
            this.SignalRatioDistUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.SignalRatioDistUpDown.Location = new System.Drawing.Point(202, 87);
            this.SignalRatioDistUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.SignalRatioDistUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.SignalRatioDistUpDown.Name = "SignalRatioDistUpDown";
            this.SignalRatioDistUpDown.Size = new System.Drawing.Size(48, 20);
            this.SignalRatioDistUpDown.TabIndex = 138;
            this.SignalRatioDistUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.SignalRatioDistUpDown.ValueChanged += new System.EventHandler(this.SignalRatioDistUpDown_ValueChanged);
            // 
            // DiffMinDeviationUpDown
            // 
            this.DiffMinDeviationUpDown.Location = new System.Drawing.Point(202, 60);
            this.DiffMinDeviationUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.DiffMinDeviationUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.DiffMinDeviationUpDown.Name = "DiffMinDeviationUpDown";
            this.DiffMinDeviationUpDown.Size = new System.Drawing.Size(48, 20);
            this.DiffMinDeviationUpDown.TabIndex = 136;
            this.DiffMinDeviationUpDown.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            this.DiffMinDeviationUpDown.ValueChanged += new System.EventHandler(this.DiffMinDeviationUpDown_ValueChanged);
            // 
            // SmoothingUpDown
            // 
            this.SmoothingUpDown.Location = new System.Drawing.Point(148, 11);
            this.SmoothingUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.SmoothingUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.SmoothingUpDown.Name = "SmoothingUpDown";
            this.SmoothingUpDown.Size = new System.Drawing.Size(48, 20);
            this.SmoothingUpDown.TabIndex = 135;
            this.SmoothingUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // AnDensityUpDown
            // 
            this.AnDensityUpDown.Location = new System.Drawing.Point(856, 13);
            this.AnDensityUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.AnDensityUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.AnDensityUpDown.Name = "AnDensityUpDown";
            this.AnDensityUpDown.Size = new System.Drawing.Size(48, 20);
            this.AnDensityUpDown.TabIndex = 134;
            this.AnDensityUpDown.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
            // 
            // AnReplacerxbufBox
            // 
            this.AnReplacerxbufBox.AutoSize = true;
            this.AnReplacerxbufBox.Checked = true;
            this.AnReplacerxbufBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AnReplacerxbufBox.Location = new System.Drawing.Point(65, 114);
            this.AnReplacerxbufBox.Name = "AnReplacerxbufBox";
            this.AnReplacerxbufBox.Size = new System.Drawing.Size(92, 17);
            this.AnReplacerxbufBox.TabIndex = 133;
            this.AnReplacerxbufBox.Text = "Replace rxbuf";
            this.AnReplacerxbufBox.UseVisualStyleBackColor = true;
            // 
            // button19
            // 
            this.button19.Location = new System.Drawing.Point(910, 10);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(49, 39);
            this.button19.TabIndex = 131;
            this.button19.Text = "Proc R data";
            this.button19.UseVisualStyleBackColor = true;
            this.button19.Click += new System.EventHandler(this.button19_Click);
            // 
            // DiffDistUpDown2
            // 
            this.DiffDistUpDown2.Location = new System.Drawing.Point(202, 35);
            this.DiffDistUpDown2.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.DiffDistUpDown2.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.DiffDistUpDown2.Name = "DiffDistUpDown2";
            this.DiffDistUpDown2.Size = new System.Drawing.Size(48, 20);
            this.DiffDistUpDown2.TabIndex = 130;
            this.DiffDistUpDown2.Value = new decimal(new int[] {
            49,
            0,
            0,
            0});
            this.DiffDistUpDown2.ValueChanged += new System.EventHandler(this.DiffDistUpDown2_ValueChanged);
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(73, 64);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(69, 13);
            this.label62.TabIndex = 127;
            this.label62.Text = "Diff threshold";
            // 
            // DiffThresholdUpDown
            // 
            this.DiffThresholdUpDown.Location = new System.Drawing.Point(148, 60);
            this.DiffThresholdUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.DiffThresholdUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.DiffThresholdUpDown.Name = "DiffThresholdUpDown";
            this.DiffThresholdUpDown.Size = new System.Drawing.Size(48, 20);
            this.DiffThresholdUpDown.TabIndex = 126;
            this.DiffThresholdUpDown.Value = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.DiffThresholdUpDown.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(92, 90);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(49, 13);
            this.label61.TabIndex = 125;
            this.label61.Text = "Diff. gain";
            // 
            // DiffGainUpDown
            // 
            this.DiffGainUpDown.DecimalPlaces = 2;
            this.DiffGainUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.DiffGainUpDown.Location = new System.Drawing.Point(148, 88);
            this.DiffGainUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.DiffGainUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.DiffGainUpDown.Name = "DiffGainUpDown";
            this.DiffGainUpDown.Size = new System.Drawing.Size(48, 20);
            this.DiffGainUpDown.TabIndex = 124;
            this.DiffGainUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            65536});
            this.DiffGainUpDown.ValueChanged += new System.EventHandler(this.DiffGainUpDown_ValueChanged);
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(72, 37);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(69, 13);
            this.label60.TabIndex = 123;
            this.label60.Text = "Diff. distance";
            // 
            // DiffDistUpDown
            // 
            this.DiffDistUpDown.Location = new System.Drawing.Point(148, 35);
            this.DiffDistUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.DiffDistUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.DiffDistUpDown.Name = "DiffDistUpDown";
            this.DiffDistUpDown.Size = new System.Drawing.Size(48, 20);
            this.DiffDistUpDown.TabIndex = 122;
            this.DiffDistUpDown.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.DiffDistUpDown.ValueChanged += new System.EventHandler(this.DiffDistUpDown_ValueChanged);
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(84, 13);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(57, 13);
            this.label52.TabIndex = 121;
            this.label52.Text = "Smoothing";
            // 
            // GraphFilterButton
            // 
            this.GraphFilterButton.Location = new System.Drawing.Point(6, 89);
            this.GraphFilterButton.Name = "GraphFilterButton";
            this.GraphFilterButton.Size = new System.Drawing.Size(49, 23);
            this.GraphFilterButton.TabIndex = 119;
            this.GraphFilterButton.Text = "Filter";
            this.GraphFilterButton.UseVisualStyleBackColor = true;
            this.GraphFilterButton.Click += new System.EventHandler(this.GraphFilterButton_Click);
            // 
            // GraphLengthLabel
            // 
            this.GraphLengthLabel.AutoSize = true;
            this.GraphLengthLabel.BackColor = System.Drawing.Color.Transparent;
            this.GraphLengthLabel.Location = new System.Drawing.Point(775, 111);
            this.GraphLengthLabel.Name = "GraphLengthLabel";
            this.GraphLengthLabel.Size = new System.Drawing.Size(13, 13);
            this.GraphLengthLabel.TabIndex = 117;
            this.GraphLengthLabel.Text = "0";
            // 
            // GraphXOffsetLabel
            // 
            this.GraphXOffsetLabel.AutoSize = true;
            this.GraphXOffsetLabel.BackColor = System.Drawing.Color.Transparent;
            this.GraphXOffsetLabel.Location = new System.Drawing.Point(774, 93);
            this.GraphXOffsetLabel.Name = "GraphXOffsetLabel";
            this.GraphXOffsetLabel.Size = new System.Drawing.Size(13, 13);
            this.GraphXOffsetLabel.TabIndex = 116;
            this.GraphXOffsetLabel.Text = "0";
            // 
            // GraphYOffsetlabel
            // 
            this.GraphYOffsetlabel.AutoSize = true;
            this.GraphYOffsetlabel.BackColor = System.Drawing.Color.Transparent;
            this.GraphYOffsetlabel.Location = new System.Drawing.Point(797, 11);
            this.GraphYOffsetlabel.Name = "GraphYOffsetlabel";
            this.GraphYOffsetlabel.Size = new System.Drawing.Size(13, 13);
            this.GraphYOffsetlabel.TabIndex = 115;
            this.GraphYOffsetlabel.Text = "0";
            // 
            // GraphScaleYLabel
            // 
            this.GraphScaleYLabel.AutoSize = true;
            this.GraphScaleYLabel.BackColor = System.Drawing.Color.Transparent;
            this.GraphScaleYLabel.Location = new System.Drawing.Point(797, 40);
            this.GraphScaleYLabel.Name = "GraphScaleYLabel";
            this.GraphScaleYLabel.Size = new System.Drawing.Size(13, 13);
            this.GraphScaleYLabel.TabIndex = 118;
            this.GraphScaleYLabel.Text = "0";
            // 
            // OpenWavefrmbutton
            // 
            this.OpenWavefrmbutton.Location = new System.Drawing.Point(6, 6);
            this.OpenWavefrmbutton.Name = "OpenWavefrmbutton";
            this.OpenWavefrmbutton.Size = new System.Drawing.Size(49, 39);
            this.OpenWavefrmbutton.TabIndex = 114;
            this.OpenWavefrmbutton.Text = "Open";
            this.OpenWavefrmbutton.UseVisualStyleBackColor = true;
            this.OpenWavefrmbutton.Click += new System.EventHandler(this.OpenWavefrmbutton_Click_1);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.Graph5SelRadioButton);
            this.groupBox9.Controls.Add(this.Graph4SelRadioButton);
            this.groupBox9.Controls.Add(this.Graph3SelRadioButton);
            this.groupBox9.Controls.Add(this.Graph2SelRadioButton);
            this.groupBox9.Controls.Add(this.Graph1SelRadioButton);
            this.groupBox9.Location = new System.Drawing.Point(322, 5);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(80, 126);
            this.groupBox9.TabIndex = 113;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Graphs";
            // 
            // Graph5SelRadioButton
            // 
            this.Graph5SelRadioButton.AutoSize = true;
            this.Graph5SelRadioButton.Location = new System.Drawing.Point(6, 103);
            this.Graph5SelRadioButton.Name = "Graph5SelRadioButton";
            this.Graph5SelRadioButton.Size = new System.Drawing.Size(63, 17);
            this.Graph5SelRadioButton.TabIndex = 4;
            this.Graph5SelRadioButton.Text = "Graph 5";
            this.Graph5SelRadioButton.UseVisualStyleBackColor = true;
            this.Graph5SelRadioButton.Click += new System.EventHandler(this.Graph5SelRadioButton_CheckedChanged);
            // 
            // Graph4SelRadioButton
            // 
            this.Graph4SelRadioButton.AutoSize = true;
            this.Graph4SelRadioButton.Location = new System.Drawing.Point(6, 84);
            this.Graph4SelRadioButton.Name = "Graph4SelRadioButton";
            this.Graph4SelRadioButton.Size = new System.Drawing.Size(63, 17);
            this.Graph4SelRadioButton.TabIndex = 3;
            this.Graph4SelRadioButton.Text = "Graph 4";
            this.Graph4SelRadioButton.UseVisualStyleBackColor = true;
            this.Graph4SelRadioButton.Click += new System.EventHandler(this.Graph4SelRadioButton_CheckedChanged);
            // 
            // Graph3SelRadioButton
            // 
            this.Graph3SelRadioButton.AutoSize = true;
            this.Graph3SelRadioButton.Location = new System.Drawing.Point(6, 63);
            this.Graph3SelRadioButton.Name = "Graph3SelRadioButton";
            this.Graph3SelRadioButton.Size = new System.Drawing.Size(63, 17);
            this.Graph3SelRadioButton.TabIndex = 2;
            this.Graph3SelRadioButton.Text = "Graph 3";
            this.Graph3SelRadioButton.UseVisualStyleBackColor = true;
            this.Graph3SelRadioButton.Click += new System.EventHandler(this.Graph3SelRadioButton_CheckedChanged);
            // 
            // Graph2SelRadioButton
            // 
            this.Graph2SelRadioButton.AutoSize = true;
            this.Graph2SelRadioButton.Location = new System.Drawing.Point(6, 41);
            this.Graph2SelRadioButton.Name = "Graph2SelRadioButton";
            this.Graph2SelRadioButton.Size = new System.Drawing.Size(63, 17);
            this.Graph2SelRadioButton.TabIndex = 1;
            this.Graph2SelRadioButton.Text = "Graph 2";
            this.Graph2SelRadioButton.UseVisualStyleBackColor = true;
            this.Graph2SelRadioButton.Click += new System.EventHandler(this.Graph2SelRadioButton_CheckedChanged);
            // 
            // Graph1SelRadioButton
            // 
            this.Graph1SelRadioButton.AutoSize = true;
            this.Graph1SelRadioButton.Checked = true;
            this.Graph1SelRadioButton.Location = new System.Drawing.Point(6, 19);
            this.Graph1SelRadioButton.Name = "Graph1SelRadioButton";
            this.Graph1SelRadioButton.Size = new System.Drawing.Size(63, 17);
            this.Graph1SelRadioButton.TabIndex = 0;
            this.Graph1SelRadioButton.TabStop = true;
            this.Graph1SelRadioButton.Text = "Graph 1";
            this.Graph1SelRadioButton.UseVisualStyleBackColor = true;
            this.Graph1SelRadioButton.Click += new System.EventHandler(this.Graph1SelRadioButton_CheckedChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.Color.Transparent;
            this.label19.Location = new System.Drawing.Point(732, 111);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(40, 13);
            this.label19.TabIndex = 110;
            this.label19.Text = "Length";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(731, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 109;
            this.label4.Text = "x offset";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(408, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 108;
            this.label3.Text = "y offset";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.BackColor = System.Drawing.Color.Transparent;
            this.label51.Location = new System.Drawing.Point(408, 44);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(42, 13);
            this.label51.TabIndex = 112;
            this.label51.Text = "Scale y";
            // 
            // GraphYScaleTrackBar
            // 
            this.GraphYScaleTrackBar.LargeChange = 25;
            this.GraphYScaleTrackBar.Location = new System.Drawing.Point(447, 32);
            this.GraphYScaleTrackBar.Maximum = 1000;
            this.GraphYScaleTrackBar.Minimum = 1;
            this.GraphYScaleTrackBar.Name = "GraphYScaleTrackBar";
            this.GraphYScaleTrackBar.Size = new System.Drawing.Size(344, 45);
            this.GraphYScaleTrackBar.TabIndex = 111;
            this.GraphYScaleTrackBar.TickFrequency = 100;
            this.GraphYScaleTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.GraphYScaleTrackBar.Value = 100;
            this.GraphYScaleTrackBar.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // GraphOffsetTrackBar
            // 
            this.GraphOffsetTrackBar.LargeChange = 25;
            this.GraphOffsetTrackBar.Location = new System.Drawing.Point(447, -1);
            this.GraphOffsetTrackBar.Maximum = 500;
            this.GraphOffsetTrackBar.Minimum = -500;
            this.GraphOffsetTrackBar.Name = "GraphOffsetTrackBar";
            this.GraphOffsetTrackBar.Size = new System.Drawing.Size(344, 45);
            this.GraphOffsetTrackBar.TabIndex = 107;
            this.GraphOffsetTrackBar.TickFrequency = 100;
            this.GraphOffsetTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.GraphOffsetTrackBar.Scroll += new System.EventHandler(this.GraphOffsetTrackBar_Scroll);
            // 
            // GraphPictureBox
            // 
            this.GraphPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GraphPictureBox.BackColor = System.Drawing.Color.Black;
            this.GraphPictureBox.Location = new System.Drawing.Point(0, 137);
            this.GraphPictureBox.Name = "GraphPictureBox";
            this.GraphPictureBox.Size = new System.Drawing.Size(971, 625);
            this.GraphPictureBox.TabIndex = 0;
            this.GraphPictureBox.TabStop = false;
            // 
            // NetworkTab
            // 
            this.NetworkTab.BackColor = System.Drawing.SystemColors.Control;
            this.NetworkTab.Controls.Add(this.button41);
            this.NetworkTab.Controls.Add(this.button42);
            this.NetworkTab.Controls.Add(this.button43);
            this.NetworkTab.Controls.Add(this.xscalemvUpDown);
            this.NetworkTab.Controls.Add(this.label75);
            this.NetworkTab.Controls.Add(this.button35);
            this.NetworkTab.Controls.Add(this.NetworkUseAveragingCheckBox);
            this.NetworkTab.Controls.Add(this.panel5);
            this.NetworkTab.Controls.Add(this.label66);
            this.NetworkTab.Controls.Add(this.label65);
            this.NetworkTab.Controls.Add(this.NetworkCaptureTrackEndUpDown);
            this.NetworkTab.Controls.Add(this.NumberOfPointsUpDown);
            this.NetworkTab.Controls.Add(this.label64);
            this.NetworkTab.Controls.Add(this.NetworkCaptureTrackStartUpDown);
            this.NetworkTab.Controls.Add(this.label63);
            this.NetworkTab.Controls.Add(this.button29);
            this.NetworkTab.Controls.Add(this.button28);
            this.NetworkTab.ImageIndex = 2;
            this.NetworkTab.Location = new System.Drawing.Point(4, 23);
            this.NetworkTab.Name = "NetworkTab";
            this.NetworkTab.Padding = new System.Windows.Forms.Padding(3);
            this.NetworkTab.Size = new System.Drawing.Size(966, 775);
            this.NetworkTab.TabIndex = 5;
            this.NetworkTab.Text = "Network";
            // 
            // button41
            // 
            this.button41.Location = new System.Drawing.Point(415, 134);
            this.button41.Name = "button41";
            this.button41.Size = new System.Drawing.Size(72, 40);
            this.button41.TabIndex = 145;
            this.button41.Text = "Step >";
            this.button41.UseVisualStyleBackColor = true;
            this.button41.Click += new System.EventHandler(this.button41_Click);
            // 
            // button42
            // 
            this.button42.Location = new System.Drawing.Point(337, 134);
            this.button42.Name = "button42";
            this.button42.Size = new System.Drawing.Size(72, 40);
            this.button42.TabIndex = 144;
            this.button42.Text = "Step <";
            this.button42.UseVisualStyleBackColor = true;
            this.button42.Click += new System.EventHandler(this.button42_Click);
            // 
            // button43
            // 
            this.button43.Location = new System.Drawing.Point(259, 134);
            this.button43.Name = "button43";
            this.button43.Size = new System.Drawing.Size(72, 40);
            this.button43.TabIndex = 143;
            this.button43.Text = "Microstep 8";
            this.button43.UseVisualStyleBackColor = true;
            this.button43.Click += new System.EventHandler(this.button43_Click);
            // 
            // xscalemvUpDown
            // 
            this.xscalemvUpDown.Location = new System.Drawing.Point(400, 70);
            this.xscalemvUpDown.Maximum = new decimal(new int[] {
            12000000,
            0,
            0,
            0});
            this.xscalemvUpDown.Name = "xscalemvUpDown";
            this.xscalemvUpDown.Size = new System.Drawing.Size(87, 20);
            this.xscalemvUpDown.TabIndex = 141;
            this.xscalemvUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Location = new System.Drawing.Point(400, 54);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(73, 13);
            this.label75.TabIndex = 142;
            this.label75.Text = "X Scale in mV";
            // 
            // button35
            // 
            this.button35.Location = new System.Drawing.Point(359, 6);
            this.button35.Name = "button35";
            this.button35.Size = new System.Drawing.Size(99, 39);
            this.button35.TabIndex = 140;
            this.button35.Text = "Capture data current track";
            this.button35.UseVisualStyleBackColor = true;
            this.button35.Click += new System.EventHandler(this.button35_Click);
            // 
            // NetworkUseAveragingCheckBox
            // 
            this.NetworkUseAveragingCheckBox.AutoSize = true;
            this.NetworkUseAveragingCheckBox.Location = new System.Drawing.Point(291, 98);
            this.NetworkUseAveragingCheckBox.Name = "NetworkUseAveragingCheckBox";
            this.NetworkUseAveragingCheckBox.Size = new System.Drawing.Size(95, 17);
            this.NetworkUseAveragingCheckBox.TabIndex = 139;
            this.NetworkUseAveragingCheckBox.Text = "Use averaging";
            this.NetworkUseAveragingCheckBox.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.NetworkDoAllBad);
            this.panel5.Controls.Add(this.NetCaptureRangecheckBox);
            this.panel5.Location = new System.Drawing.Point(26, 69);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(140, 43);
            this.panel5.TabIndex = 138;
            // 
            // NetworkDoAllBad
            // 
            this.NetworkDoAllBad.AutoSize = true;
            this.NetworkDoAllBad.Location = new System.Drawing.Point(4, 20);
            this.NetworkDoAllBad.Name = "NetworkDoAllBad";
            this.NetworkDoAllBad.Size = new System.Drawing.Size(138, 17);
            this.NetworkDoAllBad.TabIndex = 0;
            this.NetworkDoAllBad.Text = "Do all bad/unrecovered";
            this.NetworkDoAllBad.UseVisualStyleBackColor = true;
            // 
            // NetCaptureRangecheckBox
            // 
            this.NetCaptureRangecheckBox.AutoSize = true;
            this.NetCaptureRangecheckBox.Checked = true;
            this.NetCaptureRangecheckBox.Location = new System.Drawing.Point(5, 1);
            this.NetCaptureRangecheckBox.Name = "NetCaptureRangecheckBox";
            this.NetCaptureRangecheckBox.Size = new System.Drawing.Size(92, 17);
            this.NetCaptureRangecheckBox.TabIndex = 0;
            this.NetCaptureRangecheckBox.TabStop = true;
            this.NetCaptureRangecheckBox.Text = "Capture range";
            this.NetCaptureRangecheckBox.UseVisualStyleBackColor = true;
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.BackColor = System.Drawing.Color.Transparent;
            this.label66.Location = new System.Drawing.Point(182, 97);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(16, 13);
            this.label66.TabIndex = 137;
            this.label66.Text = "to";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.BackColor = System.Drawing.Color.Transparent;
            this.label65.Location = new System.Drawing.Point(172, 71);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(27, 13);
            this.label65.TabIndex = 136;
            this.label65.Text = "from";
            // 
            // NetworkCaptureTrackEndUpDown
            // 
            this.NetworkCaptureTrackEndUpDown.Location = new System.Drawing.Point(206, 95);
            this.NetworkCaptureTrackEndUpDown.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.NetworkCaptureTrackEndUpDown.Name = "NetworkCaptureTrackEndUpDown";
            this.NetworkCaptureTrackEndUpDown.Size = new System.Drawing.Size(52, 20);
            this.NetworkCaptureTrackEndUpDown.TabIndex = 135;
            this.NetworkCaptureTrackEndUpDown.Value = new decimal(new int[] {
            13,
            0,
            0,
            0});
            // 
            // NumberOfPointsUpDown
            // 
            this.NumberOfPointsUpDown.Location = new System.Drawing.Point(291, 69);
            this.NumberOfPointsUpDown.Maximum = new decimal(new int[] {
            12000000,
            0,
            0,
            0});
            this.NumberOfPointsUpDown.Name = "NumberOfPointsUpDown";
            this.NumberOfPointsUpDown.Size = new System.Drawing.Size(87, 20);
            this.NumberOfPointsUpDown.TabIndex = 97;
            this.NumberOfPointsUpDown.Value = new decimal(new int[] {
            3250000,
            0,
            0,
            0});
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(291, 53);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(87, 13);
            this.label64.TabIndex = 98;
            this.label64.Text = "Number of points";
            // 
            // NetworkCaptureTrackStartUpDown
            // 
            this.NetworkCaptureTrackStartUpDown.Location = new System.Drawing.Point(206, 69);
            this.NetworkCaptureTrackStartUpDown.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.NetworkCaptureTrackStartUpDown.Name = "NetworkCaptureTrackStartUpDown";
            this.NetworkCaptureTrackStartUpDown.Size = new System.Drawing.Size(52, 20);
            this.NetworkCaptureTrackStartUpDown.TabIndex = 95;
            this.NetworkCaptureTrackStartUpDown.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(206, 53);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(71, 13);
            this.label63.TabIndex = 96;
            this.label63.Text = "Capture track";
            // 
            // button29
            // 
            this.button29.BackColor = System.Drawing.Color.Cornsilk;
            this.button29.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button29.Location = new System.Drawing.Point(272, 6);
            this.button29.Name = "button29";
            this.button29.Size = new System.Drawing.Size(75, 40);
            this.button29.TabIndex = 93;
            this.button29.Text = "Stop!";
            this.button29.UseVisualStyleBackColor = false;
            this.button29.Click += new System.EventHandler(this.button29_Click);
            // 
            // button28
            // 
            this.button28.Location = new System.Drawing.Point(206, 6);
            this.button28.Name = "button28";
            this.button28.Size = new System.Drawing.Size(60, 39);
            this.button28.TabIndex = 92;
            this.button28.Text = "Capture data";
            this.button28.UseVisualStyleBackColor = true;
            this.button28.Click += new System.EventHandler(this.CaptureDataBtn_Click);
            // 
            // SectorUpDown
            // 
            this.SectorUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SectorUpDown.Location = new System.Drawing.Point(1127, 373);
            this.SectorUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.SectorUpDown.Name = "SectorUpDown";
            this.SectorUpDown.Size = new System.Drawing.Size(36, 20);
            this.SectorUpDown.TabIndex = 78;
            this.toolTip1.SetToolTip(this.SectorUpDown, "Show data for this sector in Sector data tab.");
            this.SectorUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.SectorUpDown.ValueChanged += new System.EventHandler(this.TrackUpDown2_ValueChanged);
            // 
            // TrackUpDown
            // 
            this.TrackUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TrackUpDown.Location = new System.Drawing.Point(1029, 373);
            this.TrackUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.TrackUpDown.Name = "TrackUpDown";
            this.TrackUpDown.Size = new System.Drawing.Size(36, 20);
            this.TrackUpDown.TabIndex = 77;
            this.toolTip1.SetToolTip(this.TrackUpDown, "Show data for this track in Sector data tab.");
            this.TrackUpDown.Value = new decimal(new int[] {
            29,
            0,
            0,
            0});
            this.TrackUpDown.ValueChanged += new System.EventHandler(this.TrackUpDown2_ValueChanged);
            // 
            // label39
            // 
            this.label39.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(1083, 375);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(38, 13);
            this.label39.TabIndex = 75;
            this.label39.Text = "Sector";
            // 
            // label40
            // 
            this.label40.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(988, 375);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(35, 13);
            this.label40.TabIndex = 76;
            this.label40.Text = "Track";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(651, 54);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(334, 8);
            this.progressBar1.TabIndex = 83;
            // 
            // timer5
            // 
            this.timer5.Interval = 10;
            this.timer5.Tick += new System.EventHandler(this.timer5_Tick);
            // 
            // ProcessStatusLabel
            // 
            this.ProcessStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessStatusLabel.BackColor = System.Drawing.Color.Transparent;
            this.ProcessStatusLabel.Location = new System.Drawing.Point(895, 38);
            this.ProcessStatusLabel.Name = "ProcessStatusLabel";
            this.ProcessStatusLabel.Size = new System.Drawing.Size(90, 13);
            this.ProcessStatusLabel.TabIndex = 59;
            this.ProcessStatusLabel.Text = "Processing status";
            this.ProcessStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // GUITimer
            // 
            this.GUITimer.Tick += new System.EventHandler(this.GUITimer_Tick);
            // 
            // timer1
            // 
            this.timer1.Interval = 250;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // GCbutton
            // 
            this.GCbutton.Location = new System.Drawing.Point(921, 6);
            this.GCbutton.Name = "GCbutton";
            this.GCbutton.Size = new System.Drawing.Size(49, 23);
            this.GCbutton.TabIndex = 88;
            this.GCbutton.Text = "GC";
            this.GCbutton.UseVisualStyleBackColor = true;
            this.GCbutton.Visible = false;
            this.GCbutton.Click += new System.EventHandler(this.GCbutton_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // EditScatterPlotcheckBox
            // 
            this.EditScatterPlotcheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EditScatterPlotcheckBox.AutoSize = true;
            this.EditScatterPlotcheckBox.Location = new System.Drawing.Point(1214, 375);
            this.EditScatterPlotcheckBox.Name = "EditScatterPlotcheckBox";
            this.EditScatterPlotcheckBox.Size = new System.Drawing.Size(98, 17);
            this.EditScatterPlotcheckBox.TabIndex = 167;
            this.EditScatterPlotcheckBox.Text = "Edit Scatterplot";
            this.toolTip1.SetToolTip(this.EditScatterPlotcheckBox, "Edit the flux period length manually. Only for Amiga formatted disks for now.");
            this.EditScatterPlotcheckBox.UseVisualStyleBackColor = true;
            this.EditScatterPlotcheckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip1.Location = new System.Drawing.Point(1, 1);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(300, 27);
            this.menuStrip1.Stretch = false;
            this.menuStrip1.TabIndex = 168;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.addToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveToolStripMenuItem,
            this.toolStripSeparator2,
            this.loadProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.toolStripSeparator3,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.toolStripSeparator5,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 23);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenBinFilebutton_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.addToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.AddDataButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(195, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.saveToolStripMenuItem.Text = "Save disk image";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveDiskImageButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(195, 6);
            // 
            // loadProjectToolStripMenuItem
            // 
            this.loadProjectToolStripMenuItem.Name = "loadProjectToolStripMenuItem";
            this.loadProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.loadProjectToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.loadProjectToolStripMenuItem.Text = "Load project";
            this.loadProjectToolStripMenuItem.Click += new System.EventHandler(this.LoadPrjBtn_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.saveProjectToolStripMenuItem.Text = "Save project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.SavePrjBtn_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(195, 6);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scpFileToolStripMenuItem});
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.importToolStripMenuItem.Text = "Import";
            // 
            // scpFileToolStripMenuItem
            // 
            this.scpFileToolStripMenuItem.Name = "scpFileToolStripMenuItem";
            this.scpFileToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.scpFileToolStripMenuItem.Text = "Scp file";
            this.scpFileToolStripMenuItem.Click += new System.EventHandler(this.button45_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scpFileToolStripMenuItem1,
            this.trimmedBinToolStripMenuItem,
            this.badSectorsToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // scpFileToolStripMenuItem1
            // 
            this.scpFileToolStripMenuItem1.Name = "scpFileToolStripMenuItem1";
            this.scpFileToolStripMenuItem1.Size = new System.Drawing.Size(141, 22);
            this.scpFileToolStripMenuItem1.Text = "Scp file";
            this.scpFileToolStripMenuItem1.Click += new System.EventHandler(this.button46_Click);
            // 
            // trimmedBinToolStripMenuItem
            // 
            this.trimmedBinToolStripMenuItem.Name = "trimmedBinToolStripMenuItem";
            this.trimmedBinToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.trimmedBinToolStripMenuItem.Text = "Trimmed bin";
            this.trimmedBinToolStripMenuItem.Click += new System.EventHandler(this.button49_Click);
            // 
            // badSectorsToolStripMenuItem
            // 
            this.badSectorsToolStripMenuItem.Name = "badSectorsToolStripMenuItem";
            this.badSectorsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.badSectorsToolStripMenuItem.Text = "Bad sectors";
            this.badSectorsToolStripMenuItem.Click += new System.EventHandler(this.SaveTrimmedBadbutton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(195, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.disableTooltipsToolStripMenuItem,
            this.toolStripSeparator4,
            this.basicModeToolStripMenuItem,
            this.advancedModeToolStripMenuItem,
            this.devModeToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 23);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = global::FloppyControlApp.Properties.Resources.IconSettings;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.SettingsButton_Click);
            // 
            // disableTooltipsToolStripMenuItem
            // 
            this.disableTooltipsToolStripMenuItem.Checked = global::FloppyControlApp.Properties.Settings.Default.TooltipDisable;
            this.disableTooltipsToolStripMenuItem.CheckOnClick = true;
            this.disableTooltipsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.disableTooltipsToolStripMenuItem.Name = "disableTooltipsToolStripMenuItem";
            this.disableTooltipsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.disableTooltipsToolStripMenuItem.Text = "Enable tooltips";
            this.disableTooltipsToolStripMenuItem.Click += new System.EventHandler(this.disableTooltipsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(158, 6);
            // 
            // basicModeToolStripMenuItem
            // 
            this.basicModeToolStripMenuItem.CheckOnClick = true;
            this.basicModeToolStripMenuItem.Name = "basicModeToolStripMenuItem";
            this.basicModeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.basicModeToolStripMenuItem.Text = "Basic mode";
            this.basicModeToolStripMenuItem.Click += new System.EventHandler(this.basicModeToolStripMenuItem_Click);
            // 
            // advancedModeToolStripMenuItem
            // 
            this.advancedModeToolStripMenuItem.CheckOnClick = true;
            this.advancedModeToolStripMenuItem.Name = "advancedModeToolStripMenuItem";
            this.advancedModeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.advancedModeToolStripMenuItem.Text = "Advanced mode";
            this.advancedModeToolStripMenuItem.Click += new System.EventHandler(this.advancedModeToolStripMenuItem_Click);
            // 
            // devModeToolStripMenuItem
            // 
            this.devModeToolStripMenuItem.CheckOnClick = true;
            this.devModeToolStripMenuItem.Name = "devModeToolStripMenuItem";
            this.devModeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.devModeToolStripMenuItem.Text = "Dev mode";
            this.devModeToolStripMenuItem.Click += new System.EventHandler(this.devModeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 23);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutButton_Click);
            // 
            // ThreadsUpDown
            // 
            this.ThreadsUpDown.Location = new System.Drawing.Point(868, 9);
            this.ThreadsUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.ThreadsUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.ThreadsUpDown.Name = "ThreadsUpDown";
            this.ThreadsUpDown.Size = new System.Drawing.Size(39, 20);
            this.ThreadsUpDown.TabIndex = 172;
            this.ThreadsUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ThreadsUpDown.ValueChanged += new System.EventHandler(this.ThreadsUpDown_ValueChanged);
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(811, 13);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(46, 13);
            this.label59.TabIndex = 171;
            this.label59.Text = "Threads";
            // 
            // StopButton
            // 
            this.StopButton.BackColor = System.Drawing.Color.Cornsilk;
            this.StopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StopButton.Location = new System.Drawing.Point(651, 13);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(75, 36);
            this.StopButton.TabIndex = 173;
            this.StopButton.Text = "Stop!";
            this.toolTip1.SetToolTip(this.StopButton, "Stops all capturing, error correction and processing tasks.");
            this.StopButton.UseVisualStyleBackColor = false;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // ExploreHereBtn
            // 
            this.ExploreHereBtn.Location = new System.Drawing.Point(365, 28);
            this.ExploreHereBtn.Name = "ExploreHereBtn";
            this.ExploreHereBtn.Size = new System.Drawing.Size(76, 23);
            this.ExploreHereBtn.TabIndex = 174;
            this.ExploreHereBtn.Text = "Explore here";
            this.ExploreHereBtn.UseVisualStyleBackColor = true;
            this.ExploreHereBtn.Click += new System.EventHandler(this.ExploreHereBtn_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.Active = global::FloppyControlApp.Properties.Settings.Default.TooltipDisable;
            // 
            // FloppyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1619, 910);
            this.Controls.Add(this.ExploreHereBtn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.ThreadsUpDown);
            this.Controls.Add(this.label59);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.EditScatterPlotcheckBox);
            this.Controls.Add(this.GCbutton);
            this.Controls.Add(this.ProcessStatusLabel);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.textBoxReceived);
            this.Controls.Add(this.SectorUpDown);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.TrackUpDown);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label40);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outputfilename);
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FloppyControl";
            this.Text = "Floppy Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FloppyControl_FormClosing);
            this.SizeChanged += new System.EventHandler(this.FloppyControl_SizeChanged);
            this.Click += new System.EventHandler(this.FloppyControl_Click);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FloppyControl_KeyDown);
            this.Resize += new System.EventHandler(this.FloppyControl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ScatterPlottabPage.ResumeLayout(false);
            this.ScatterPlottabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterPictureBox)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ShowSectorTab.ResumeLayout(false);
            this.ShowSectorTab.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LimitToSectorUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LimitToTrackUpDown)).EndInit();
            this.MainTabControl.ResumeLayout(false);
            this.QuickTab.ResumeLayout(false);
            this.QProcessingGroupBox.ResumeLayout(false);
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.QOffsetUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QMaxUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QSixEightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QFourSixUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QMinUpDown)).EndInit();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.QHistogramhScrollBar1)).EndInit();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.QRateOfChange2UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QRateOfChangeUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QAdaptOfsset2UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QLimitToSectorUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QLimitToTrackUpDown)).EndInit();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.QTrackDurationUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QStartTrackUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QEndTracksUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QTRK00OffsetUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QMicrostepsPerTrackUpDown)).EndInit();
            this.CaptureTab.ResumeLayout(false);
            this.CaptureTab.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rxbufEndUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rxbufStartUpDown)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TrackDurationUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartTrackUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EndTracksUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TRK00OffsetUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MicrostepsPerTrackUpDown)).EndInit();
            this.ProcessingTab.ResumeLayout(false);
            this.ProcessingTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DupsUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AddNoiseKnumericUpDown)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.jESEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jESStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iESEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iESStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RateOfChange2UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AdaptOfsset2UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RndAmountUpDown)).EndInit();
            this.ThresholdsGroupBox.ResumeLayout(false);
            this.ThresholdsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RateOfChangeUpDown)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HistogramhScrollBar1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ErrorCorrectionTab.ResumeLayout(false);
            this.ErrorCorrectionTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CombinationsUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MFMByteLengthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MFMByteStartUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.C8StartUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.C6StartUpDown)).EndInit();
            this.ECInfoTabs.ResumeLayout(false);
            this.ECTabSectorData.ResumeLayout(false);
            this.ECTabSectorData.PerformLayout();
            this.tabPage8.ResumeLayout(false);
            this.tabPage8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterOffsetUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterMinUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterMaxUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterMaxTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterMinTrackBar)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Sector2UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Track2UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Sector1UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Track1UpDown)).EndInit();
            this.BadSectorPanel.ResumeLayout(false);
            this.BadSectorPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScatterOffsetTrackBar)).EndInit();
            this.AnalysisPage.ResumeLayout(false);
            this.AnalysisPage.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.AnalysisTab2.ResumeLayout(false);
            this.AnalysisTab2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ThresholdTestUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffTest2UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffTestUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PeriodExtendUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HighpassThresholdUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AdaptLookAheadUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffMinDeviation2UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffOffsetUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SignalRatioDistUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffMinDeviationUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SmoothingUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnDensityUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffDistUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffThresholdUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffGainUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffDistUpDown)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GraphYScaleTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GraphOffsetTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GraphPictureBox)).EndInit();
            this.NetworkTab.ResumeLayout(false);
            this.NetworkTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xscalemvUpDown)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NetworkCaptureTrackEndUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumberOfPointsUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NetworkCaptureTrackStartUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SectorUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackUpDown)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ThreadsUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
         
        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox outputfilename;
        private System.Windows.Forms.Label label1;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbSectorMap;
        private System.Windows.Forms.Label BytesPerSecondLabel;
        private System.Windows.Forms.Label BytesReceivedLabel;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label CurrentTrackLabel;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label RecoveredSectorsLabel;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label RecoveredSectorsWithErrorsLabel;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox TrackInfotextBox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage ScatterPlottabPage;
        private System.Windows.Forms.TextBox textBoxReceived;
        private System.Windows.Forms.Label LabelStatus;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label CaptureTimeLabel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox textBoxFilesLoaded;
        private System.Windows.Forms.TabPage ShowSectorTab;
        private System.Windows.Forms.TextBox textBoxSector;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.NumericUpDown LimitToSectorUpDown;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.NumericUpDown LimitToTrackUpDown;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label MarkersLabel;
        private System.Windows.Forms.Label GoodHdrCntLabel;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage CaptureTab;
        private System.Windows.Forms.TabPage ProcessingTab;
        private System.Windows.Forms.Button ProcessPCBtn;
        private System.Windows.Forms.Button ProcessBtn;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.NumericUpDown SectorUpDown;
        private System.Windows.Forms.NumericUpDown TrackUpDown;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.VScrollBar OffsetvScrollBar1;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.VScrollBar EightvScrollBar;
        private System.Windows.Forms.VScrollBar SixvScrollBar;
        private System.Windows.Forms.VScrollBar MinvScrollBar;
        private System.Windows.Forms.VScrollBar FourvScrollBar;
        private System.Windows.Forms.Label Offsetlabel;
        private System.Windows.Forms.Label POffsetLabel;
        private System.Windows.Forms.Label EightLabel;
        private System.Windows.Forms.Label PMaxLabel;
        private System.Windows.Forms.Label SixLabel;
        private System.Windows.Forms.Label MinLabel;
        private System.Windows.Forms.Label PSixEightLabel;
        private System.Windows.Forms.Label FourLabel;
        private System.Windows.Forms.Label PMinLabel;
        private System.Windows.Forms.Label PFourSixLabel;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Panel Histogrampanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button ScanButton;
        private System.Windows.Forms.CheckBox HDCheckBox;
        private System.Windows.Forms.CheckBox IgnoreHeaderErrorCheckBox;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button TrackPreset4Button;
        private System.Windows.Forms.Button TrackPreset2Button;
        private System.Windows.Forms.Button TrackPreset3Button;
        private System.Windows.Forms.Button TrackPreset1Button;
        private System.Windows.Forms.NumericUpDown TrackDurationUpDown;
        private System.Windows.Forms.NumericUpDown StartTrackUpDown;
        private System.Windows.Forms.NumericUpDown EndTracksUpDown;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.NumericUpDown TRK00OffsetUpDown;
        private System.Windows.Forms.NumericUpDown MicrostepsPerTrackUpDown;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TabPage ErrorCorrectionTab;
        private System.Windows.Forms.Button ECSectorOverlayBtn;
        private System.Windows.Forms.Label BadSectorsCntLabel;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.NumericUpDown Sector2UpDown;
        private System.Windows.Forms.NumericUpDown Track2UpDown;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.NumericUpDown Sector1UpDown;
        private System.Windows.Forms.NumericUpDown Track1UpDown;
        private System.Windows.Forms.Label BlueCrcCheckLabel;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.ListBox BadSectorListBox;
        private System.Windows.Forms.NumericUpDown RateOfChangeUpDown;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel BadSectorPanel;
        private System.Windows.Forms.Label BadSectorTooltip;
        private System.Windows.Forms.Timer timer5;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton BlueTempRadio;
        private System.Windows.Forms.RadioButton BSBlueFromListRadio;
        private System.Windows.Forms.RadioButton BSBlueSectormapRadio;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.RadioButton BSRedTempRadio;
        private System.Windows.Forms.RadioButton BSRedFromlistRadio;
        private System.Windows.Forms.RadioButton radioButton6;
        private System.Windows.Forms.Button CopySectorToBlueBtn;
        private System.Windows.Forms.Button BluetoRedByteCopyToolBtn;
        private System.Windows.Forms.Label BSEditByteLabel;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label RedCrcCheckLabel;
        private System.Windows.Forms.TabPage AnalysisPage;
        private System.Windows.Forms.Button ConvertToMFMBtn;
        private System.Windows.Forms.TextBox tbBIN;
        private System.Windows.Forms.TextBox tbTest;
        private System.Windows.Forms.TextBox tbMFM;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox AutoRefreshSectorMapCheck;
        private System.Windows.Forms.Label ProcessStatusLabel;
        private System.Windows.Forms.CheckBox FindDupesCheckBox;
        private System.Windows.Forms.TabPage AnalysisTab2;
        private System.Windows.Forms.Timer GUITimer;
        private System.Windows.Forms.RichTextBox rtbSectorMap;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel AnHistogramPanel;
        private System.Windows.Forms.Label HistScalingLabel;
        private System.Windows.Forms.TrackBar ScatterMaxTrackBar;
        private System.Windows.Forms.TrackBar ScatterMinTrackBar;
        private System.Windows.Forms.TrackBar ScatterOffsetTrackBar;
        private System.Windows.Forms.GroupBox ThresholdsGroupBox;
        private System.Windows.Forms.Button button21;
        private System.Windows.Forms.Button button23;
        private System.Windows.Forms.Button button25;
        private System.Windows.Forms.Button button26;
        private System.Windows.Forms.NumericUpDown ScatterMinUpDown;
        private System.Windows.Forms.NumericUpDown ScatterMaxUpDown;
        private System.Windows.Forms.NumericUpDown ScatterOffsetUpDown;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.PictureBox ScatterPictureBox;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.Label SelectionDifLabel;
        private System.Windows.Forms.TextBox AntxtBox;
        private System.Windows.Forms.TextBox antbSectorData;
        private System.Windows.Forms.Button ECZoomOutBtn;
        private System.Windows.Forms.TabControl ECInfoTabs;
        private System.Windows.Forms.TabPage ECTabSectorData;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.TextBox ECtbMFM;
        private System.Windows.Forms.Button ECRealign4E;
        private System.Windows.Forms.CheckBox BadSectorsCheckBox;
        private System.Windows.Forms.CheckBox GoodSectorsCheckBox;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.RadioButton ANAmigaDiskSpareRadio;
        private System.Windows.Forms.RadioButton ANAmigaRadio;
        private System.Windows.Forms.RadioButton ANPCRadio;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.RadioButton AmigaMFMRadio;
        private System.Windows.Forms.Button button20;
        private System.Windows.Forms.CheckBox LimitTSCheckBox;
        private System.Windows.Forms.CheckBox DirectStepCheckBox;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar GraphOffsetTrackBar;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.TrackBar GraphYScaleTrackBar;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.RadioButton Graph3SelRadioButton;
        private System.Windows.Forms.RadioButton Graph2SelRadioButton;
        private System.Windows.Forms.RadioButton Graph1SelRadioButton;
        private System.Windows.Forms.Button OpenWavefrmbutton;
        private System.Windows.Forms.Label GraphLengthLabel;
        private System.Windows.Forms.Label GraphXOffsetLabel;
        private System.Windows.Forms.Label GraphYOffsetlabel;
        private System.Windows.Forms.Label GraphScaleYLabel;
        private System.Windows.Forms.RadioButton Graph4SelRadioButton;
        private System.Windows.Forms.Button GraphFilterButton;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.NumericUpDown DiffDistUpDown;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.NumericUpDown DiffGainUpDown;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.NumericUpDown DiffThresholdUpDown;
        private System.Windows.Forms.NumericUpDown DiffDistUpDown2;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.TabPage NetworkTab;
        private System.Windows.Forms.Button button28;
        private System.Windows.Forms.Button button29;
        private System.Windows.Forms.NumericUpDown NetworkCaptureTrackStartUpDown;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.Button CaptureClassbutton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NumericUpDown NumberOfPointsUpDown;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.CheckBox AnReplacerxbufBox;
        private System.Windows.Forms.NumericUpDown AnDensityUpDown;
        private System.Windows.Forms.TrackBar HistogramhScrollBar1;
        private System.Windows.Forms.Button GCbutton;
        private System.Windows.Forms.NumericUpDown SmoothingUpDown;
        private System.Windows.Forms.NumericUpDown DiffMinDeviationUpDown;
        private System.Windows.Forms.CheckBox AddNoisecheckBox;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.NumericUpDown NetworkCaptureTrackEndUpDown;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.RadioButton NetworkDoAllBad;
        private System.Windows.Forms.RadioButton NetCaptureRangecheckBox;
        private System.Windows.Forms.PictureBox GraphPictureBox;
        private System.Windows.Forms.CheckBox LimitToScttrViewcheckBox;
        private System.Windows.Forms.NumericUpDown RndAmountUpDown;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.NumericUpDown AddNoiseKnumericUpDown;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.NumericUpDown SignalRatioDistUpDown;
        private System.Windows.Forms.CheckBox AdaptiveGaincheckBox;
        private System.Windows.Forms.CheckBox InvertcheckBox;
        private System.Windows.Forms.NumericUpDown DiffOffsetUpDown;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.NumericUpDown DiffMinDeviation2UpDown;
        private System.Windows.Forms.NumericUpDown AdaptLookAheadUpDown;
        private System.Windows.Forms.ComboBox PeriodBeyond8uscomboBox;
        private System.Windows.Forms.NumericUpDown DupsUpDown;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.Button EditUndobutton;
        private System.Windows.Forms.Button SaveWaveformButton;
        private System.Windows.Forms.Button button32;
        private System.Windows.Forms.Label label70;
        private System.Windows.Forms.Button DCOffsetbutton;
        private System.Windows.Forms.Button Lowpassbutton;
        private System.Windows.Forms.Label Undolevelslabel;
        private System.Windows.Forms.Button button33;
        private System.Windows.Forms.NumericUpDown HighpassThresholdUpDown;
        private System.Windows.Forms.Label statsLabel;
        private System.Windows.Forms.Label label72;
        private System.Windows.Forms.ComboBox EditModecomboBox;
        private System.Windows.Forms.ComboBox EditOptioncomboBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label71;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown C8StartUpDown;
        private System.Windows.Forms.NumericUpDown C6StartUpDown;
        private System.Windows.Forms.Label label73;
        private System.Windows.Forms.NumericUpDown PeriodExtendUpDown;
        private System.Windows.Forms.CheckBox NetworkUseAveragingCheckBox;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.NumericUpDown AdaptOfsset2UpDown;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button31;
        private System.Windows.Forms.CheckBox AnAutoUpdateCheckBox;
        private System.Windows.Forms.Button button34;
        private System.Windows.Forms.NumericUpDown DiffTestUpDown;
        private System.Windows.Forms.NumericUpDown DiffTest2UpDown;
        private System.Windows.Forms.NumericUpDown ThresholdTestUpDown;
        private System.Windows.Forms.Button button35;
        private System.Windows.Forms.RadioButton Graph5SelRadioButton;
        private System.Windows.Forms.ComboBox JumpTocomboBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.NumericUpDown RateOfChange2UpDown;
        private System.Windows.Forms.CheckBox ECMFMcheckBox;
        private System.Windows.Forms.Button button38;
        private System.Windows.Forms.Button button40;
        private System.Windows.Forms.Button button39;
        private System.Windows.Forms.Button button36;
        private System.Windows.Forms.ImageList MainTabControlImageList;
        private System.Windows.Forms.NumericUpDown xscalemvUpDown;
        private System.Windows.Forms.Label label75;
        private System.Windows.Forms.Button button41;
        private System.Windows.Forms.Button button42;
        private System.Windows.Forms.Button button43;
        private System.Windows.Forms.Label hlabel;
        private System.Windows.Forms.Label wlabel;
        private System.Windows.Forms.NumericUpDown MFMByteLengthUpDown;
        private System.Windows.Forms.NumericUpDown MFMByteStartUpDown;
        private System.Windows.Forms.Label label76;
        private System.Windows.Forms.Label label77;
        private System.Windows.Forms.Button ECMFMByteEncbutton;
        private System.Windows.Forms.Button button44;
        private System.Windows.Forms.Button button45;
        private System.Windows.Forms.Button button46;
        private System.Windows.Forms.CheckBox ClearDatacheckBox;
        private System.Windows.Forms.ComboBox ChangeDiskTypeComboBox;
        private System.Windows.Forms.Label DiskTypeLabel;
        private System.Windows.Forms.Button button48;
        private System.Windows.Forms.Button button49;
        private System.Windows.Forms.Button SaveTrimmedBadbutton;
        private System.Windows.Forms.ComboBox ProcessingModeComboBox;
        private System.Windows.Forms.ComboBox ScanComboBox;
        private System.Windows.Forms.Label label78;
        private System.Windows.Forms.Label rxbufOffsetLabel;
        private System.Windows.Forms.Label label80;
        private System.Windows.Forms.NumericUpDown CombinationsUpDown;
        private System.Windows.Forms.Label label79;
        private System.Windows.Forms.CheckBox EditScatterPlotcheckBox;
        private System.Windows.Forms.RadioButton ECOnRadio;
        private System.Windows.Forms.RadioButton OnlyBadSectorsRadio;
        private System.Windows.Forms.Label label81;
        private System.Windows.Forms.Button FullHistBtn;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TabPage QuickTab;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.Button StepStickPresetBtn;
        private System.Windows.Forms.Button QDirectStepPresetBtn;
        private System.Windows.Forms.Button QRecaptureAllBtn;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.CheckBox QDirectStepCheckBox;
        private System.Windows.Forms.Button QCaptureBtn;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Button PresetCaptureDuration1s;
        private System.Windows.Forms.Button PresetCaptureDuration5s;
        private System.Windows.Forms.Button PresetTrack78_164;
        private System.Windows.Forms.Button PresetTrack80_90;
        private System.Windows.Forms.Button PresetCaptureDefaultBtn;
        private System.Windows.Forms.Button button37;
        private System.Windows.Forms.NumericUpDown QTrackDurationUpDown;
        private System.Windows.Forms.NumericUpDown QStartTrackUpDown;
        private System.Windows.Forms.NumericUpDown QEndTracksUpDown;
        private System.Windows.Forms.Label label82;
        private System.Windows.Forms.NumericUpDown QTRK00OffsetUpDown;
        private System.Windows.Forms.NumericUpDown QMicrostepsPerTrackUpDown;
        private System.Windows.Forms.Label label83;
        private System.Windows.Forms.Label label84;
        private System.Windows.Forms.Label label85;
        private System.Windows.Forms.Label label86;
        private System.Windows.Forms.GroupBox QProcessingGroupBox;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.RadioButton QOnlyBadSectorsRadio;
        private System.Windows.Forms.RadioButton QECOnRadio;
        private System.Windows.Forms.ComboBox QProcessingModeComboBox;
        private System.Windows.Forms.CheckBox QClearDatacheckBox;
        private System.Windows.Forms.NumericUpDown QAdaptOfsset2UpDown;
        private System.Windows.Forms.NumericUpDown QLimitToTrackUpDown;
        private System.Windows.Forms.NumericUpDown QLimitToSectorUpDown;
        private System.Windows.Forms.Label label90;
        private System.Windows.Forms.Label label91;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.Label QOffsetLabel;
        private System.Windows.Forms.Label QFourSixLabel;
        private System.Windows.Forms.Label QMinLabel;
        private System.Windows.Forms.Label QSixEightLabel;
        private System.Windows.Forms.Label QMaxLabel;
        private System.Windows.Forms.CheckBox QFindDupesCheckBox;
        private System.Windows.Forms.CheckBox QAutoRefreshSectorMapCheck;
        private System.Windows.Forms.CheckBox QIgnoreHeaderErrorCheckBox;
        private System.Windows.Forms.GroupBox groupBox16;
        private System.Windows.Forms.Panel QHistoPanel;
        private System.Windows.Forms.TrackBar QHistogramhScrollBar1;
        private System.Windows.Forms.GroupBox groupBox17;
        private System.Windows.Forms.ComboBox QScanComboBox;
        private System.Windows.Forms.Button button47;
        private System.Windows.Forms.CheckBox QHDCheckBox;
        private System.Windows.Forms.Label label114;
        private System.Windows.Forms.Button QProcAmigaBtn;
        private System.Windows.Forms.Button QProcPCBtn;
        private System.Windows.Forms.RichTextBox QrtbSectorMap;
        private System.Windows.Forms.Label label89;
        private System.Windows.Forms.ComboBox QChangeDiskTypeComboBox;
        private System.Windows.Forms.NumericUpDown QOffsetUpDown;
        private System.Windows.Forms.NumericUpDown QMaxUpDown;
        private System.Windows.Forms.NumericUpDown QSixEightUpDown;
        private System.Windows.Forms.NumericUpDown QFourSixUpDown;
        private System.Windows.Forms.NumericUpDown QMinUpDown;
        private System.Windows.Forms.NumericUpDown QRateOfChange2UpDown;
        private System.Windows.Forms.Label label88;
        private System.Windows.Forms.Label label102;
        private System.Windows.Forms.NumericUpDown QRateOfChangeUpDown;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem loadProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scpFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scpFileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem trimmedBinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem badSectorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown ThreadsUpDown;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown rxbufEndUpDown;
        private System.Windows.Forms.NumericUpDown rxbufStartUpDown;
        private System.Windows.Forms.Label BufferSizeLabel;
        private System.Windows.Forms.Label HistogramLengthLabel;
        private System.Windows.Forms.Label HistogramStartLabel;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button ResetBuffersBtn;
        private System.Windows.Forms.NumericUpDown jESEnd;
        private System.Windows.Forms.NumericUpDown jESStart;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label SettingsLabel;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.NumericUpDown iESEnd;
        private System.Windows.Forms.NumericUpDown iESStart;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.CheckBox QLimitTSCheckBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableTooltipsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem basicModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem devModeToolStripMenuItem;
        private System.Windows.Forms.Button GluedDiskPreset;
        private System.Windows.Forms.Button PresetCaptureDuration50s;
        private System.Windows.Forms.Button PresetCaptureDuration2s;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Button ExploreHereBtn;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem closeToolStripMenuItem;
    }
}

