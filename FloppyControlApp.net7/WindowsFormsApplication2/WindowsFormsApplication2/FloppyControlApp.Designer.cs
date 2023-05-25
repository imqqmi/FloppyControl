using System;
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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FloppyControl));
			Properties.Settings settings1 = new Properties.Settings();
			openFileDialog1 = new OpenFileDialog();
			outputfilename = new TextBox();
			label1 = new Label();
			fileSystemWatcher1 = new FileSystemWatcher();
			tbSectorMap = new TextBox();
			label5 = new Label();
			BytesPerSecondLabel = new Label();
			BytesReceivedLabel = new Label();
			label23 = new Label();
			label24 = new Label();
			label25 = new Label();
			CurrentTrackLabel = new Label();
			label26 = new Label();
			RecoveredSectorsLabel = new Label();
			label33 = new Label();
			RecoveredSectorsWithErrorsLabel = new Label();
			label34 = new Label();
			tabControl1 = new TabControl();
			ScatterPlottabPage = new TabPage();
			label81 = new Label();
			JumpTocomboBox = new ComboBox();
			label58 = new Label();
			label57 = new Label();
			ScatterPictureBox = new PictureBox();
			label56 = new Label();
			tabPage1 = new TabPage();
			TrackInfotextBox = new TextBox();
			tabPage2 = new TabPage();
			tabPage3 = new TabPage();
			textBoxFilesLoaded = new TextBox();
			ShowSectorTab = new TabPage();
			textBoxSector = new TextBox();
			tabPage4 = new TabPage();
			LabelStatus = new Label();
			label7 = new Label();
			textBoxReceived = new TextBox();
			panel2 = new Panel();
			hlabel = new Label();
			wlabel = new Label();
			statsLabel = new Label();
			label72 = new Label();
			BadSectorsCntLabel = new Label();
			label44 = new Label();
			MarkersLabel = new Label();
			GoodHdrCntLabel = new Label();
			label45 = new Label();
			label46 = new Label();
			label13 = new Label();
			CaptureTimeLabel = new Label();
			DiskTypeLabel = new Label();
			backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			openFileDialog2 = new OpenFileDialog();
			LimitToSectorUpDown = new NumericUpDown();
			label41 = new Label();
			LimitToTrackUpDown = new NumericUpDown();
			label42 = new Label();
			MainTabControl = new TabControl();
			QuickTab = new TabPage();
			QrtbSectorMap = new RichTextBox();
			QProcessingGroupBox = new GroupBox();
			button11 = new Button();
			button15 = new Button();
			groupBox14 = new GroupBox();
			QOnlyBadSectorsRadio = new RadioButton();
			QECOnRadio = new RadioButton();
			label89 = new Label();
			QChangeDiskTypeComboBox = new ComboBox();
			QProcessingModeComboBox = new ComboBox();
			QClearDatacheckBox = new CheckBox();
			groupBox15 = new GroupBox();
			QOffsetUpDown = new NumericUpDown();
			QMaxUpDown = new NumericUpDown();
			QSixEightUpDown = new NumericUpDown();
			QFourSixUpDown = new NumericUpDown();
			QMinUpDown = new NumericUpDown();
			QOffsetLabel = new Label();
			QFourSixLabel = new Label();
			QMinLabel = new Label();
			QSixEightLabel = new Label();
			QMaxLabel = new Label();
			QFindDupesCheckBox = new CheckBox();
			QAutoRefreshSectorMapCheck = new CheckBox();
			QIgnoreHeaderErrorCheckBox = new CheckBox();
			groupBox16 = new GroupBox();
			QHistoPanel = new Panel();
			QHistogramhScrollBar1 = new TrackBar();
			groupBox17 = new GroupBox();
			QLimitTSCheckBox = new CheckBox();
			QRateOfChange2UpDown = new NumericUpDown();
			label88 = new Label();
			label102 = new Label();
			QRateOfChangeUpDown = new NumericUpDown();
			QScanComboBox = new ComboBox();
			button47 = new Button();
			QAdaptOfsset2UpDown = new NumericUpDown();
			label90 = new Label();
			QLimitToSectorUpDown = new NumericUpDown();
			label91 = new Label();
			QLimitToTrackUpDown = new NumericUpDown();
			QHDCheckBox = new CheckBox();
			label114 = new Label();
			QProcAmigaBtn = new Button();
			QProcPCBtn = new Button();
			groupBox12 = new GroupBox();
			GluedDiskPreset = new Button();
			StepStickPresetBtn = new Button();
			QDirectStepPresetBtn = new Button();
			QRecaptureAllBtn = new Button();
			MainTabControlImageList = new ImageList(components);
			button12 = new Button();
			button13 = new Button();
			button14 = new Button();
			QDirectStepCheckBox = new CheckBox();
			QCaptureBtn = new Button();
			groupBox10 = new GroupBox();
			groupBox11 = new GroupBox();
			PresetCaptureDuration50s = new Button();
			PresetCaptureDuration2s = new Button();
			PresetCaptureDuration1s = new Button();
			PresetCaptureDuration5s = new Button();
			PresetTrack78_164 = new Button();
			PresetTrack80_90 = new Button();
			PresetCaptureDefaultBtn = new Button();
			button37 = new Button();
			QTrackDurationUpDown = new NumericUpDown();
			QStartTrackUpDown = new NumericUpDown();
			QEndTracksUpDown = new NumericUpDown();
			label82 = new Label();
			QTRK00OffsetUpDown = new NumericUpDown();
			QMicrostepsPerTrackUpDown = new NumericUpDown();
			label83 = new Label();
			label84 = new Label();
			label85 = new Label();
			label86 = new Label();
			CaptureTab = new TabPage();
			groupBox4 = new GroupBox();
			rxbufEndUpDown = new NumericUpDown();
			rxbufStartUpDown = new NumericUpDown();
			BufferSizeLabel = new Label();
			HistogramLengthLabel = new Label();
			HistogramStartLabel = new Label();
			label29 = new Label();
			label28 = new Label();
			label27 = new Label();
			label32 = new Label();
			label31 = new Label();
			button8 = new Button();
			button7 = new Button();
			SaveTrimmedBadbutton = new Button();
			button49 = new Button();
			button48 = new Button();
			button46 = new Button();
			button45 = new Button();
			button40 = new Button();
			button39 = new Button();
			button36 = new Button();
			CaptureClassbutton = new Button();
			groupBox7 = new GroupBox();
			groupBox1 = new GroupBox();
			button4 = new Button();
			button6 = new Button();
			TrackPreset4Button = new Button();
			TrackPreset2Button = new Button();
			TrackPreset3Button = new Button();
			TrackPreset1Button = new Button();
			TrackDurationUpDown = new NumericUpDown();
			StartTrackUpDown = new NumericUpDown();
			EndTracksUpDown = new NumericUpDown();
			label22 = new Label();
			TRK00OffsetUpDown = new NumericUpDown();
			MicrostepsPerTrackUpDown = new NumericUpDown();
			label38 = new Label();
			label36 = new Label();
			label21 = new Label();
			label20 = new Label();
			DirectStepCheckBox = new CheckBox();
			ProcessingTab = new TabPage();
			button5 = new Button();
			ResetBuffersBtn = new Button();
			DupsUpDown = new NumericUpDown();
			label69 = new Label();
			AddNoiseKnumericUpDown = new NumericUpDown();
			label68 = new Label();
			rtbSectorMap = new RichTextBox();
			groupBox6 = new GroupBox();
			SkipAlreadyCrcOkcheckBox1 = new CheckBox();
			jESEnd = new NumericUpDown();
			jESStart = new NumericUpDown();
			label17 = new Label();
			SettingsLabel = new Label();
			label35 = new Label();
			iESEnd = new NumericUpDown();
			iESStart = new NumericUpDown();
			label8 = new Label();
			label9 = new Label();
			FullHistBtn = new Button();
			OnlyBadSectorsRadio = new RadioButton();
			ECOnRadio = new RadioButton();
			label78 = new Label();
			ChangeDiskTypeComboBox = new ComboBox();
			ProcessingModeComboBox = new ComboBox();
			ClearDatacheckBox = new CheckBox();
			LimitTSCheckBox = new CheckBox();
			RateOfChange2UpDown = new NumericUpDown();
			AdaptOfsset2UpDown = new NumericUpDown();
			label74 = new Label();
			PeriodBeyond8uscomboBox = new ComboBox();
			RndAmountUpDown = new NumericUpDown();
			label67 = new Label();
			LimitToScttrViewcheckBox = new CheckBox();
			AddNoisecheckBox = new CheckBox();
			ThresholdsGroupBox = new GroupBox();
			MinvScrollBar = new VScrollBar();
			FourLabel = new Label();
			MinLabel = new Label();
			SixLabel = new Label();
			EightLabel = new Label();
			Offsetlabel = new Label();
			FourvScrollBar = new VScrollBar();
			SixvScrollBar = new VScrollBar();
			EightvScrollBar = new VScrollBar();
			OffsetvScrollBar1 = new VScrollBar();
			POffsetLabel = new Label();
			PFourSixLabel = new Label();
			PMinLabel = new Label();
			PSixEightLabel = new Label();
			PMaxLabel = new Label();
			FindDupesCheckBox = new CheckBox();
			AutoRefreshSectorMapCheck = new CheckBox();
			label50 = new Label();
			RateOfChangeUpDown = new NumericUpDown();
			IgnoreHeaderErrorCheckBox = new CheckBox();
			groupBox5 = new GroupBox();
			Histogrampanel1 = new Panel();
			HistogramhScrollBar1 = new TrackBar();
			groupBox3 = new GroupBox();
			ScanComboBox = new ComboBox();
			ScanButton = new Button();
			HDCheckBox = new CheckBox();
			label37 = new Label();
			label2 = new Label();
			ProcessBtn = new Button();
			ProcessPCBtn = new Button();
			ErrorCorrectionTab = new TabPage();
			EntropySpliceBtn = new Button();
			AvgPeriodsFromListSelBtn = new Button();
			CombinationsUpDown = new NumericUpDown();
			label79 = new Label();
			button44 = new Button();
			ECMFMByteEncbutton = new Button();
			MFMByteLengthUpDown = new NumericUpDown();
			MFMByteStartUpDown = new NumericUpDown();
			label76 = new Label();
			label77 = new Label();
			button38 = new Button();
			ECMFMcheckBox = new CheckBox();
			label71 = new Label();
			label6 = new Label();
			C8StartUpDown = new NumericUpDown();
			C6StartUpDown = new NumericUpDown();
			button1 = new Button();
			BadSectorsCheckBox = new CheckBox();
			GoodSectorsCheckBox = new CheckBox();
			ECRealign4E = new Button();
			ECInfoTabs = new TabControl();
			ECTabSectorData = new TabPage();
			antbSectorData = new TextBox();
			tabPage8 = new TabPage();
			ECtbMFM = new TextBox();
			ECZoomOutBtn = new Button();
			SelectionDifLabel = new Label();
			ScatterOffsetUpDown = new NumericUpDown();
			ScatterMinUpDown = new NumericUpDown();
			ScatterMaxUpDown = new NumericUpDown();
			ScatterMaxTrackBar = new TrackBar();
			ScatterMinTrackBar = new TrackBar();
			groupBox2 = new GroupBox();
			HistScalingLabel = new Label();
			AnHistogramPanel = new Panel();
			RedCrcCheckLabel = new Label();
			label43 = new Label();
			BSEditByteLabel = new Label();
			BluetoRedByteCopyToolBtn = new Button();
			CopySectorToBlueBtn = new Button();
			label55 = new Label();
			panel4 = new Panel();
			BSRedTempRadio = new RadioButton();
			BSRedFromlistRadio = new RadioButton();
			radioButton6 = new RadioButton();
			panel3 = new Panel();
			BlueTempRadio = new RadioButton();
			BSBlueFromListRadio = new RadioButton();
			BSBlueSectormapRadio = new RadioButton();
			label54 = new Label();
			label53 = new Label();
			BadSectorListBox = new ListBox();
			Sector2UpDown = new NumericUpDown();
			Track2UpDown = new NumericUpDown();
			label48 = new Label();
			label49 = new Label();
			Sector1UpDown = new NumericUpDown();
			Track1UpDown = new NumericUpDown();
			BlueCrcCheckLabel = new Label();
			label47 = new Label();
			ECSectorOverlayBtn = new Button();
			BadSectorPanel = new Panel();
			BadSectorTooltip = new Label();
			ScatterOffsetTrackBar = new TrackBar();
			AnalysisPage = new TabPage();
			button20 = new Button();
			groupBox8 = new GroupBox();
			AmigaMFMRadio = new RadioButton();
			ANAmigaDiskSpareRadio = new RadioButton();
			ANAmigaRadio = new RadioButton();
			ANPCRadio = new RadioButton();
			AntxtBox = new TextBox();
			button25 = new Button();
			button26 = new Button();
			button23 = new Button();
			button21 = new Button();
			tbMFM = new TextBox();
			button2 = new Button();
			ConvertToMFMBtn = new Button();
			tbBIN = new TextBox();
			tbTest = new TextBox();
			AnalysisTab2 = new TabPage();
			rxbufOffsetLabel = new Label();
			label80 = new Label();
			ThresholdTestUpDown = new NumericUpDown();
			DiffTest2UpDown = new NumericUpDown();
			DiffTestUpDown = new NumericUpDown();
			button34 = new Button();
			AnAutoUpdateCheckBox = new CheckBox();
			button31 = new Button();
			button3 = new Button();
			label73 = new Label();
			PeriodExtendUpDown = new NumericUpDown();
			EditOptioncomboBox = new ComboBox();
			EditModecomboBox = new ComboBox();
			HighpassThresholdUpDown = new NumericUpDown();
			button33 = new Button();
			Undolevelslabel = new Label();
			Lowpassbutton = new Button();
			DCOffsetbutton = new Button();
			label70 = new Label();
			button32 = new Button();
			SaveWaveformButton = new Button();
			EditUndobutton = new Button();
			AdaptLookAheadUpDown = new NumericUpDown();
			DiffMinDeviation2UpDown = new NumericUpDown();
			button18 = new Button();
			DiffOffsetUpDown = new NumericUpDown();
			InvertcheckBox = new CheckBox();
			AdaptiveGaincheckBox = new CheckBox();
			SignalRatioDistUpDown = new NumericUpDown();
			DiffMinDeviationUpDown = new NumericUpDown();
			SmoothingUpDown = new NumericUpDown();
			AnDensityUpDown = new NumericUpDown();
			AnReplacerxbufBox = new CheckBox();
			button19 = new Button();
			DiffDistUpDown2 = new NumericUpDown();
			label62 = new Label();
			DiffThresholdUpDown = new NumericUpDown();
			label61 = new Label();
			DiffGainUpDown = new NumericUpDown();
			label60 = new Label();
			DiffDistUpDown = new NumericUpDown();
			label52 = new Label();
			GraphFilterButton = new Button();
			GraphLengthLabel = new Label();
			GraphXOffsetLabel = new Label();
			GraphYOffsetlabel = new Label();
			GraphScaleYLabel = new Label();
			OpenWavefrmbutton = new Button();
			groupBox9 = new GroupBox();
			Graph5SelRadioButton = new RadioButton();
			Graph4SelRadioButton = new RadioButton();
			Graph3SelRadioButton = new RadioButton();
			Graph2SelRadioButton = new RadioButton();
			Graph1SelRadioButton = new RadioButton();
			label19 = new Label();
			label4 = new Label();
			label3 = new Label();
			label51 = new Label();
			GraphYScaleTrackBar = new TrackBar();
			GraphOffsetTrackBar = new TrackBar();
			GraphPictureBox = new PictureBox();
			NetworkTab = new TabPage();
			button41 = new Button();
			button42 = new Button();
			button43 = new Button();
			xscalemvUpDown = new NumericUpDown();
			label75 = new Label();
			button35 = new Button();
			NetworkUseAveragingCheckBox = new CheckBox();
			panel5 = new Panel();
			NetworkDoAllBad = new RadioButton();
			NetCaptureRangecheckBox = new RadioButton();
			label66 = new Label();
			label65 = new Label();
			NetworkCaptureTrackEndUpDown = new NumericUpDown();
			NumberOfPointsUpDown = new NumericUpDown();
			label64 = new Label();
			NetworkCaptureTrackStartUpDown = new NumericUpDown();
			label63 = new Label();
			button29 = new Button();
			button28 = new Button();
			SectorUpDown = new NumericUpDown();
			TrackUpDown = new NumericUpDown();
			label39 = new Label();
			label40 = new Label();
			progressBar1 = new ProgressBar();
			timer5 = new Timer(components);
			ProcessStatusLabel = new Label();
			GUITimer = new Timer(components);
			backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
			timer1 = new Timer(components);
			GCbutton = new Button();
			contextMenuStrip1 = new ContextMenuStrip(components);
			EditScatterPlotcheckBox = new CheckBox();
			menuStrip1 = new MenuStrip();
			fileToolStripMenuItem = new ToolStripMenuItem();
			openToolStripMenuItem = new ToolStripMenuItem();
			addToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator1 = new ToolStripSeparator();
			saveToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator2 = new ToolStripSeparator();
			loadProjectToolStripMenuItem = new ToolStripMenuItem();
			saveProjectToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator3 = new ToolStripSeparator();
			importToolStripMenuItem = new ToolStripMenuItem();
			scpFileToolStripMenuItem = new ToolStripMenuItem();
			exportToolStripMenuItem = new ToolStripMenuItem();
			scpFileToolStripMenuItem1 = new ToolStripMenuItem();
			trimmedBinToolStripMenuItem = new ToolStripMenuItem();
			badSectorsToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator5 = new ToolStripSeparator();
			closeToolStripMenuItem = new ToolStripMenuItem();
			optionsToolStripMenuItem = new ToolStripMenuItem();
			settingsToolStripMenuItem = new ToolStripMenuItem();
			disableTooltipsToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator4 = new ToolStripSeparator();
			basicModeToolStripMenuItem = new ToolStripMenuItem();
			advancedModeToolStripMenuItem = new ToolStripMenuItem();
			devModeToolStripMenuItem = new ToolStripMenuItem();
			helpToolStripMenuItem = new ToolStripMenuItem();
			aboutToolStripMenuItem = new ToolStripMenuItem();
			ThreadsUpDown = new NumericUpDown();
			label59 = new Label();
			StopButton = new Button();
			ExploreHereBtn = new Button();
			toolTip1 = new ToolTip(components);
			((System.ComponentModel.ISupportInitialize)fileSystemWatcher1).BeginInit();
			tabControl1.SuspendLayout();
			ScatterPlottabPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)ScatterPictureBox).BeginInit();
			tabPage1.SuspendLayout();
			tabPage3.SuspendLayout();
			ShowSectorTab.SuspendLayout();
			tabPage4.SuspendLayout();
			panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)LimitToSectorUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)LimitToTrackUpDown).BeginInit();
			MainTabControl.SuspendLayout();
			QuickTab.SuspendLayout();
			QProcessingGroupBox.SuspendLayout();
			groupBox14.SuspendLayout();
			groupBox15.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)QOffsetUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QMaxUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QSixEightUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QFourSixUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QMinUpDown).BeginInit();
			groupBox16.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)QHistogramhScrollBar1).BeginInit();
			groupBox17.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)QRateOfChange2UpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QRateOfChangeUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QAdaptOfsset2UpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QLimitToSectorUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QLimitToTrackUpDown).BeginInit();
			groupBox12.SuspendLayout();
			groupBox10.SuspendLayout();
			groupBox11.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)QTrackDurationUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QStartTrackUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QEndTracksUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QTRK00OffsetUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)QMicrostepsPerTrackUpDown).BeginInit();
			CaptureTab.SuspendLayout();
			groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)rxbufEndUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)rxbufStartUpDown).BeginInit();
			groupBox7.SuspendLayout();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)TrackDurationUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)StartTrackUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)EndTracksUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)TRK00OffsetUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)MicrostepsPerTrackUpDown).BeginInit();
			ProcessingTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)DupsUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)AddNoiseKnumericUpDown).BeginInit();
			groupBox6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)jESEnd).BeginInit();
			((System.ComponentModel.ISupportInitialize)jESStart).BeginInit();
			((System.ComponentModel.ISupportInitialize)iESEnd).BeginInit();
			((System.ComponentModel.ISupportInitialize)iESStart).BeginInit();
			((System.ComponentModel.ISupportInitialize)RateOfChange2UpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)AdaptOfsset2UpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)RndAmountUpDown).BeginInit();
			ThresholdsGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)RateOfChangeUpDown).BeginInit();
			groupBox5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)HistogramhScrollBar1).BeginInit();
			groupBox3.SuspendLayout();
			ErrorCorrectionTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)CombinationsUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)MFMByteLengthUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)MFMByteStartUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)C8StartUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)C6StartUpDown).BeginInit();
			ECInfoTabs.SuspendLayout();
			ECTabSectorData.SuspendLayout();
			tabPage8.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)ScatterOffsetUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)ScatterMinUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)ScatterMaxUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)ScatterMaxTrackBar).BeginInit();
			((System.ComponentModel.ISupportInitialize)ScatterMinTrackBar).BeginInit();
			groupBox2.SuspendLayout();
			panel4.SuspendLayout();
			panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)Sector2UpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)Track2UpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)Sector1UpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)Track1UpDown).BeginInit();
			BadSectorPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)ScatterOffsetTrackBar).BeginInit();
			AnalysisPage.SuspendLayout();
			groupBox8.SuspendLayout();
			AnalysisTab2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)ThresholdTestUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)DiffTest2UpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)DiffTestUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)PeriodExtendUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)HighpassThresholdUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)AdaptLookAheadUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)DiffMinDeviation2UpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)DiffOffsetUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)SignalRatioDistUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)DiffMinDeviationUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)SmoothingUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)AnDensityUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)DiffDistUpDown2).BeginInit();
			((System.ComponentModel.ISupportInitialize)DiffThresholdUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)DiffGainUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)DiffDistUpDown).BeginInit();
			groupBox9.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)GraphYScaleTrackBar).BeginInit();
			((System.ComponentModel.ISupportInitialize)GraphOffsetTrackBar).BeginInit();
			((System.ComponentModel.ISupportInitialize)GraphPictureBox).BeginInit();
			NetworkTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)xscalemvUpDown).BeginInit();
			panel5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)NetworkCaptureTrackEndUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)NumberOfPointsUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)NetworkCaptureTrackStartUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)SectorUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)TrackUpDown).BeginInit();
			menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)ThreadsUpDown).BeginInit();
			SuspendLayout();
			// 
			// openFileDialog1
			// 
			openFileDialog1.Multiselect = true;
			// 
			// outputfilename
			// 
			outputfilename.Location = new System.Drawing.Point(105, 32);
			outputfilename.Margin = new Padding(4, 3, 4, 3);
			outputfilename.Name = "outputfilename";
			outputfilename.Size = new System.Drawing.Size(313, 23);
			outputfilename.TabIndex = 1;
			outputfilename.Text = "Dump";
			toolTip1.SetToolTip(outputfilename, "Before capturing, set a name for the folder and files to be generated during capture and saving/exporting data.");
			outputfilename.TextChanged += Outputfilename_TextChanged;
			outputfilename.Enter += Outputfilename_Enter;
			outputfilename.Leave += Outputfilename_Leave;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(14, 36);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(80, 15);
			label1.TabIndex = 1;
			label1.Text = "Base filename";
			// 
			// fileSystemWatcher1
			// 
			fileSystemWatcher1.EnableRaisingEvents = true;
			fileSystemWatcher1.SynchronizingObject = this;
			// 
			// tbSectorMap
			// 
			tbSectorMap.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			tbSectorMap.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			tbSectorMap.Location = new System.Drawing.Point(0, 0);
			tbSectorMap.Margin = new Padding(4, 3, 4, 3);
			tbSectorMap.Multiline = true;
			tbSectorMap.Name = "tbSectorMap";
			tbSectorMap.ScrollBars = ScrollBars.Vertical;
			tbSectorMap.Size = new System.Drawing.Size(696, 418);
			tbSectorMap.TabIndex = 18;
			tbSectorMap.Tag = "";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(14, 412);
			label5.Margin = new Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(114, 15);
			label5.TabIndex = 1;
			label5.Text = "Sector map (output)";
			// 
			// BytesPerSecondLabel
			// 
			BytesPerSecondLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			BytesPerSecondLabel.AutoSize = true;
			BytesPerSecondLabel.Location = new System.Drawing.Point(728, 30);
			BytesPerSecondLabel.Margin = new Padding(4, 0, 4, 0);
			BytesPerSecondLabel.Name = "BytesPerSecondLabel";
			BytesPerSecondLabel.Size = new System.Drawing.Size(13, 15);
			BytesPerSecondLabel.TabIndex = 1;
			BytesPerSecondLabel.Text = "0";
			// 
			// BytesReceivedLabel
			// 
			BytesReceivedLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			BytesReceivedLabel.AutoSize = true;
			BytesReceivedLabel.Location = new System.Drawing.Point(728, 15);
			BytesReceivedLabel.Margin = new Padding(4, 0, 4, 0);
			BytesReceivedLabel.Name = "BytesReceivedLabel";
			BytesReceivedLabel.Size = new System.Drawing.Size(13, 15);
			BytesReceivedLabel.TabIndex = 1;
			BytesReceivedLabel.Text = "0";
			// 
			// label23
			// 
			label23.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label23.AutoSize = true;
			label23.Location = new System.Drawing.Point(646, 15);
			label23.Margin = new Padding(4, 0, 4, 0);
			label23.Name = "label23";
			label23.Size = new System.Drawing.Size(70, 15);
			label23.TabIndex = 27;
			label23.Text = "Bytes recvd:";
			// 
			// label24
			// 
			label24.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label24.AutoSize = true;
			label24.Location = new System.Drawing.Point(637, 30);
			label24.Margin = new Padding(4, 0, 4, 0);
			label24.Name = "label24";
			label24.Size = new System.Drawing.Size(78, 15);
			label24.TabIndex = 27;
			label24.Text = "Bytes per sec:";
			// 
			// label25
			// 
			label25.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label25.AutoSize = true;
			label25.Location = new System.Drawing.Point(495, 15);
			label25.Margin = new Padding(4, 0, 4, 0);
			label25.Name = "label25";
			label25.Size = new System.Drawing.Size(37, 15);
			label25.TabIndex = 27;
			label25.Text = "Track:";
			// 
			// CurrentTrackLabel
			// 
			CurrentTrackLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			CurrentTrackLabel.AutoSize = true;
			CurrentTrackLabel.Location = new System.Drawing.Point(537, 15);
			CurrentTrackLabel.Margin = new Padding(4, 0, 4, 0);
			CurrentTrackLabel.Name = "CurrentTrackLabel";
			CurrentTrackLabel.Size = new System.Drawing.Size(19, 15);
			CurrentTrackLabel.TabIndex = 27;
			CurrentTrackLabel.Text = "00";
			// 
			// label26
			// 
			label26.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label26.AutoSize = true;
			label26.Location = new System.Drawing.Point(420, 30);
			label26.Margin = new Padding(4, 0, 4, 0);
			label26.Name = "label26";
			label26.Size = new System.Drawing.Size(106, 15);
			label26.TabIndex = 27;
			label26.Text = "Recovered Sectors:";
			// 
			// RecoveredSectorsLabel
			// 
			RecoveredSectorsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			RecoveredSectorsLabel.AutoSize = true;
			RecoveredSectorsLabel.Location = new System.Drawing.Point(537, 30);
			RecoveredSectorsLabel.Margin = new Padding(4, 0, 4, 0);
			RecoveredSectorsLabel.Name = "RecoveredSectorsLabel";
			RecoveredSectorsLabel.Size = new System.Drawing.Size(13, 15);
			RecoveredSectorsLabel.TabIndex = 27;
			RecoveredSectorsLabel.Text = "0";
			// 
			// label33
			// 
			label33.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label33.AutoSize = true;
			label33.Location = new System.Drawing.Point(161, 30);
			label33.Margin = new Padding(4, 0, 4, 0);
			label33.Name = "label33";
			label33.Size = new System.Drawing.Size(160, 15);
			label33.TabIndex = 27;
			label33.Text = "Recovered Sectors with error:";
			// 
			// RecoveredSectorsWithErrorsLabel
			// 
			RecoveredSectorsWithErrorsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			RecoveredSectorsWithErrorsLabel.AutoSize = true;
			RecoveredSectorsWithErrorsLabel.Location = new System.Drawing.Point(341, 30);
			RecoveredSectorsWithErrorsLabel.Margin = new Padding(4, 0, 4, 0);
			RecoveredSectorsWithErrorsLabel.Name = "RecoveredSectorsWithErrorsLabel";
			RecoveredSectorsWithErrorsLabel.Size = new System.Drawing.Size(13, 15);
			RecoveredSectorsWithErrorsLabel.TabIndex = 27;
			RecoveredSectorsWithErrorsLabel.Text = "0";
			// 
			// label34
			// 
			label34.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label34.AutoSize = true;
			label34.Location = new System.Drawing.Point(271, 15);
			label34.Margin = new Padding(4, 0, 4, 0);
			label34.Name = "label34";
			label34.Size = new System.Drawing.Size(58, 15);
			label34.TabIndex = 27;
			label34.Text = "Disk type:";
			// 
			// tabControl1
			// 
			tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			tabControl1.Controls.Add(ScatterPlottabPage);
			tabControl1.Controls.Add(tabPage1);
			tabControl1.Controls.Add(tabPage2);
			tabControl1.Controls.Add(tabPage3);
			tabControl1.Controls.Add(ShowSectorTab);
			tabControl1.Controls.Add(tabPage4);
			tabControl1.Location = new System.Drawing.Point(1156, 453);
			tabControl1.Margin = new Padding(4, 3, 4, 3);
			tabControl1.Name = "tabControl1";
			tabControl1.SelectedIndex = 0;
			tabControl1.Size = new System.Drawing.Size(719, 546);
			tabControl1.TabIndex = 42;
			// 
			// ScatterPlottabPage
			// 
			ScatterPlottabPage.Controls.Add(label81);
			ScatterPlottabPage.Controls.Add(JumpTocomboBox);
			ScatterPlottabPage.Controls.Add(label58);
			ScatterPlottabPage.Controls.Add(label57);
			ScatterPlottabPage.Controls.Add(ScatterPictureBox);
			ScatterPlottabPage.Controls.Add(label56);
			ScatterPlottabPage.Location = new System.Drawing.Point(4, 24);
			ScatterPlottabPage.Margin = new Padding(4, 3, 4, 3);
			ScatterPlottabPage.Name = "ScatterPlottabPage";
			ScatterPlottabPage.Padding = new Padding(4, 3, 4, 3);
			ScatterPlottabPage.Size = new System.Drawing.Size(711, 518);
			ScatterPlottabPage.TabIndex = 2;
			ScatterPlottabPage.Text = "Scatter plot";
			ScatterPlottabPage.UseVisualStyleBackColor = true;
			// 
			// label81
			// 
			label81.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label81.AutoSize = true;
			label81.Location = new System.Drawing.Point(-2, 224);
			label81.Margin = new Padding(4, 0, 4, 0);
			label81.Name = "label81";
			label81.Size = new System.Drawing.Size(35, 15);
			label81.TabIndex = 98;
			label81.Text = "Entro";
			// 
			// JumpTocomboBox
			// 
			JumpTocomboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			JumpTocomboBox.FormattingEnabled = true;
			JumpTocomboBox.Location = new System.Drawing.Point(37, 452);
			JumpTocomboBox.Margin = new Padding(4, 3, 4, 3);
			JumpTocomboBox.Name = "JumpTocomboBox";
			JumpTocomboBox.Size = new System.Drawing.Size(210, 23);
			JumpTocomboBox.TabIndex = 97;
			JumpTocomboBox.SelectedIndexChanged += JumpTocomboBox_SelectedIndexChanged;
			// 
			// label58
			// 
			label58.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label58.AutoSize = true;
			label58.Location = new System.Drawing.Point(7, 164);
			label58.Margin = new Padding(4, 0, 4, 0);
			label58.Name = "label58";
			label58.Size = new System.Drawing.Size(25, 15);
			label58.TabIndex = 86;
			label58.Text = "8us";
			// 
			// label57
			// 
			label57.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label57.AutoSize = true;
			label57.Location = new System.Drawing.Point(7, 115);
			label57.Margin = new Padding(4, 0, 4, 0);
			label57.Name = "label57";
			label57.Size = new System.Drawing.Size(25, 15);
			label57.TabIndex = 85;
			label57.Text = "6us";
			// 
			// ScatterPictureBox
			// 
			ScatterPictureBox.Location = new System.Drawing.Point(37, 9);
			ScatterPictureBox.Margin = new Padding(4, 3, 4, 3);
			ScatterPictureBox.Name = "ScatterPictureBox";
			ScatterPictureBox.Size = new System.Drawing.Size(668, 436);
			ScatterPictureBox.TabIndex = 1;
			ScatterPictureBox.TabStop = false;
			toolTip1.SetToolTip(ScatterPictureBox, "The scatter plot shows you the signal, with pulse length on the vertical axis and datapoints/time on the horizontal axis. \r\nZoom with scroll wheel, drag to move the view.");
			// 
			// label56
			// 
			label56.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label56.AutoSize = true;
			label56.Location = new System.Drawing.Point(7, 72);
			label56.Margin = new Padding(4, 0, 4, 0);
			label56.Name = "label56";
			label56.Size = new System.Drawing.Size(25, 15);
			label56.TabIndex = 84;
			label56.Text = "4us";
			// 
			// tabPage1
			// 
			tabPage1.Controls.Add(TrackInfotextBox);
			tabPage1.Location = new System.Drawing.Point(4, 24);
			tabPage1.Margin = new Padding(4, 3, 4, 3);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new Padding(4, 3, 4, 3);
			tabPage1.Size = new System.Drawing.Size(711, 518);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "Track info";
			tabPage1.UseVisualStyleBackColor = true;
			// 
			// TrackInfotextBox
			// 
			TrackInfotextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			TrackInfotextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			TrackInfotextBox.Location = new System.Drawing.Point(10, 7);
			TrackInfotextBox.Margin = new Padding(4, 3, 4, 3);
			TrackInfotextBox.MaxLength = 2000000;
			TrackInfotextBox.Multiline = true;
			TrackInfotextBox.Name = "TrackInfotextBox";
			TrackInfotextBox.ScrollBars = ScrollBars.Vertical;
			TrackInfotextBox.Size = new System.Drawing.Size(677, 403);
			TrackInfotextBox.TabIndex = 21;
			// 
			// tabPage2
			// 
			tabPage2.Location = new System.Drawing.Point(4, 24);
			tabPage2.Margin = new Padding(4, 3, 4, 3);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new Padding(4, 3, 4, 3);
			tabPage2.Size = new System.Drawing.Size(711, 518);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "Sector Data";
			tabPage2.UseVisualStyleBackColor = true;
			// 
			// tabPage3
			// 
			tabPage3.Controls.Add(textBoxFilesLoaded);
			tabPage3.Location = new System.Drawing.Point(4, 24);
			tabPage3.Margin = new Padding(4, 3, 4, 3);
			tabPage3.Name = "tabPage3";
			tabPage3.Padding = new Padding(4, 3, 4, 3);
			tabPage3.Size = new System.Drawing.Size(711, 518);
			tabPage3.TabIndex = 3;
			tabPage3.Text = "Files";
			tabPage3.UseVisualStyleBackColor = true;
			// 
			// textBoxFilesLoaded
			// 
			textBoxFilesLoaded.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			textBoxFilesLoaded.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			textBoxFilesLoaded.Location = new System.Drawing.Point(0, 3);
			textBoxFilesLoaded.Margin = new Padding(4, 3, 4, 3);
			textBoxFilesLoaded.MaxLength = 200000;
			textBoxFilesLoaded.Multiline = true;
			textBoxFilesLoaded.Name = "textBoxFilesLoaded";
			textBoxFilesLoaded.ScrollBars = ScrollBars.Vertical;
			textBoxFilesLoaded.Size = new System.Drawing.Size(688, 407);
			textBoxFilesLoaded.TabIndex = 57;
			// 
			// ShowSectorTab
			// 
			ShowSectorTab.Controls.Add(textBoxSector);
			ShowSectorTab.Location = new System.Drawing.Point(4, 24);
			ShowSectorTab.Margin = new Padding(4, 3, 4, 3);
			ShowSectorTab.Name = "ShowSectorTab";
			ShowSectorTab.Padding = new Padding(4, 3, 4, 3);
			ShowSectorTab.Size = new System.Drawing.Size(711, 518);
			ShowSectorTab.TabIndex = 4;
			ShowSectorTab.Text = "Sector";
			ShowSectorTab.UseVisualStyleBackColor = true;
			// 
			// textBoxSector
			// 
			textBoxSector.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			textBoxSector.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			textBoxSector.Location = new System.Drawing.Point(1, 0);
			textBoxSector.Margin = new Padding(4, 3, 4, 3);
			textBoxSector.MaxLength = 200000;
			textBoxSector.Multiline = true;
			textBoxSector.Name = "textBoxSector";
			textBoxSector.ScrollBars = ScrollBars.Vertical;
			textBoxSector.Size = new System.Drawing.Size(688, 493);
			textBoxSector.TabIndex = 58;
			// 
			// tabPage4
			// 
			tabPage4.Controls.Add(tbSectorMap);
			tabPage4.Location = new System.Drawing.Point(4, 24);
			tabPage4.Margin = new Padding(4, 3, 4, 3);
			tabPage4.Name = "tabPage4";
			tabPage4.Padding = new Padding(4, 3, 4, 3);
			tabPage4.Size = new System.Drawing.Size(711, 518);
			tabPage4.TabIndex = 5;
			tabPage4.Text = "DebugInfo";
			tabPage4.UseVisualStyleBackColor = true;
			// 
			// LabelStatus
			// 
			LabelStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LabelStatus.AutoSize = true;
			LabelStatus.Location = new System.Drawing.Point(49, 30);
			LabelStatus.Margin = new Padding(4, 0, 4, 0);
			LabelStatus.Name = "LabelStatus";
			LabelStatus.Size = new System.Drawing.Size(79, 15);
			LabelStatus.TabIndex = 54;
			LabelStatus.Text = "Disconnected";
			// 
			// label7
			// 
			label7.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(4, 30);
			label7.Margin = new Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(42, 15);
			label7.TabIndex = 53;
			label7.Text = "Status:";
			// 
			// textBoxReceived
			// 
			textBoxReceived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			textBoxReceived.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			textBoxReceived.Location = new System.Drawing.Point(1156, 7);
			textBoxReceived.Margin = new Padding(4, 3, 4, 3);
			textBoxReceived.MaxLength = 2000000;
			textBoxReceived.Multiline = true;
			textBoxReceived.Name = "textBoxReceived";
			textBoxReceived.ScrollBars = ScrollBars.Vertical;
			textBoxReceived.Size = new System.Drawing.Size(718, 419);
			textBoxReceived.TabIndex = 56;
			// 
			// panel2
			// 
			panel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			panel2.BackColor = System.Drawing.Color.Transparent;
			panel2.Controls.Add(hlabel);
			panel2.Controls.Add(wlabel);
			panel2.Controls.Add(statsLabel);
			panel2.Controls.Add(label72);
			panel2.Controls.Add(BadSectorsCntLabel);
			panel2.Controls.Add(label44);
			panel2.Controls.Add(MarkersLabel);
			panel2.Controls.Add(GoodHdrCntLabel);
			panel2.Controls.Add(label45);
			panel2.Controls.Add(label46);
			panel2.Controls.Add(label13);
			panel2.Controls.Add(CaptureTimeLabel);
			panel2.Controls.Add(label7);
			panel2.Controls.Add(BytesPerSecondLabel);
			panel2.Controls.Add(BytesReceivedLabel);
			panel2.Controls.Add(LabelStatus);
			panel2.Controls.Add(label23);
			panel2.Controls.Add(label25);
			panel2.Controls.Add(label26);
			panel2.Controls.Add(label33);
			panel2.Controls.Add(CurrentTrackLabel);
			panel2.Controls.Add(label34);
			panel2.Controls.Add(RecoveredSectorsLabel);
			panel2.Controls.Add(label24);
			panel2.Controls.Add(RecoveredSectorsWithErrorsLabel);
			panel2.Controls.Add(DiskTypeLabel);
			panel2.Location = new System.Drawing.Point(18, 995);
			panel2.Margin = new Padding(4, 3, 4, 3);
			panel2.Name = "panel2";
			panel2.Size = new System.Drawing.Size(1392, 54);
			panel2.TabIndex = 57;
			// 
			// hlabel
			// 
			hlabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			hlabel.AutoSize = true;
			hlabel.Location = new System.Drawing.Point(1213, 30);
			hlabel.Margin = new Padding(4, 0, 4, 0);
			hlabel.Name = "hlabel";
			hlabel.Size = new System.Drawing.Size(13, 15);
			hlabel.TabIndex = 66;
			hlabel.Text = "0";
			// 
			// wlabel
			// 
			wlabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			wlabel.AutoSize = true;
			wlabel.Location = new System.Drawing.Point(1212, 15);
			wlabel.Margin = new Padding(4, 0, 4, 0);
			wlabel.Name = "wlabel";
			wlabel.Size = new System.Drawing.Size(13, 15);
			wlabel.TabIndex = 65;
			wlabel.Text = "0";
			// 
			// statsLabel
			// 
			statsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			statsLabel.AutoSize = true;
			statsLabel.Location = new System.Drawing.Point(1110, 30);
			statsLabel.Margin = new Padding(4, 0, 4, 0);
			statsLabel.Name = "statsLabel";
			statsLabel.Size = new System.Drawing.Size(13, 15);
			statsLabel.TabIndex = 63;
			statsLabel.Text = "0";
			// 
			// label72
			// 
			label72.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label72.AutoSize = true;
			label72.Location = new System.Drawing.Point(1078, 30);
			label72.Margin = new Padding(4, 0, 4, 0);
			label72.Name = "label72";
			label72.Size = new System.Drawing.Size(30, 15);
			label72.TabIndex = 64;
			label72.Text = "Stat:";
			// 
			// BadSectorsCntLabel
			// 
			BadSectorsCntLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			BadSectorsCntLabel.AutoSize = true;
			BadSectorsCntLabel.Location = new System.Drawing.Point(1108, 15);
			BadSectorsCntLabel.Margin = new Padding(4, 0, 4, 0);
			BadSectorsCntLabel.Name = "BadSectorsCntLabel";
			BadSectorsCntLabel.Size = new System.Drawing.Size(13, 15);
			BadSectorsCntLabel.TabIndex = 61;
			BadSectorsCntLabel.Text = "0";
			// 
			// label44
			// 
			label44.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label44.AutoSize = true;
			label44.Location = new System.Drawing.Point(1020, 15);
			label44.Margin = new Padding(4, 0, 4, 0);
			label44.Name = "label44";
			label44.Size = new System.Drawing.Size(85, 15);
			label44.TabIndex = 62;
			label44.Text = "Bad sector cnt:";
			// 
			// MarkersLabel
			// 
			MarkersLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			MarkersLabel.AutoSize = true;
			MarkersLabel.Location = new System.Drawing.Point(966, 30);
			MarkersLabel.Margin = new Padding(4, 0, 4, 0);
			MarkersLabel.Name = "MarkersLabel";
			MarkersLabel.Size = new System.Drawing.Size(13, 15);
			MarkersLabel.TabIndex = 57;
			MarkersLabel.Text = "0";
			// 
			// GoodHdrCntLabel
			// 
			GoodHdrCntLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			GoodHdrCntLabel.AutoSize = true;
			GoodHdrCntLabel.Location = new System.Drawing.Point(966, 15);
			GoodHdrCntLabel.Margin = new Padding(4, 0, 4, 0);
			GoodHdrCntLabel.Name = "GoodHdrCntLabel";
			GoodHdrCntLabel.Size = new System.Drawing.Size(13, 15);
			GoodHdrCntLabel.TabIndex = 58;
			GoodHdrCntLabel.Text = "0";
			// 
			// label45
			// 
			label45.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label45.AutoSize = true;
			label45.Location = new System.Drawing.Point(884, 15);
			label45.Margin = new Padding(4, 0, 4, 0);
			label45.Name = "label45";
			label45.Size = new System.Drawing.Size(84, 15);
			label45.TabIndex = 59;
			label45.Text = "Good Hdr Cnt:";
			// 
			// label46
			// 
			label46.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label46.AutoSize = true;
			label46.Location = new System.Drawing.Point(915, 30);
			label46.Margin = new Padding(4, 0, 4, 0);
			label46.Name = "label46";
			label46.Size = new System.Drawing.Size(52, 15);
			label46.TabIndex = 60;
			label46.Text = "Markers:";
			// 
			// label13
			// 
			label13.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(12, 15);
			label13.Margin = new Padding(4, 0, 4, 0);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(36, 15);
			label13.TabIndex = 55;
			label13.Text = "Time:";
			// 
			// CaptureTimeLabel
			// 
			CaptureTimeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			CaptureTimeLabel.AutoSize = true;
			CaptureTimeLabel.Location = new System.Drawing.Point(50, 15);
			CaptureTimeLabel.Margin = new Padding(4, 0, 4, 0);
			CaptureTimeLabel.Name = "CaptureTimeLabel";
			CaptureTimeLabel.Size = new System.Drawing.Size(13, 15);
			CaptureTimeLabel.TabIndex = 56;
			CaptureTimeLabel.Text = "0";
			// 
			// DiskTypeLabel
			// 
			DiskTypeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			DiskTypeLabel.AutoSize = true;
			DiskTypeLabel.Location = new System.Drawing.Point(341, 15);
			DiskTypeLabel.Margin = new Padding(4, 0, 4, 0);
			DiskTypeLabel.Name = "DiskTypeLabel";
			DiskTypeLabel.Size = new System.Drawing.Size(58, 15);
			DiskTypeLabel.TabIndex = 27;
			DiskTypeLabel.Text = "Unknown";
			// 
			// openFileDialog2
			// 
			openFileDialog2.FileName = "openFileDialog2";
			openFileDialog2.Filter = "Projects (*.prj)|*.prj|All files (*.*|*.*)";
			// 
			// LimitToSectorUpDown
			// 
			LimitToSectorUpDown.Location = new System.Drawing.Point(517, 246);
			LimitToSectorUpDown.Margin = new Padding(4, 3, 4, 3);
			LimitToSectorUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			LimitToSectorUpDown.Minimum = new decimal(new int[] { 16, 0, 0, int.MinValue });
			LimitToSectorUpDown.Name = "LimitToSectorUpDown";
			LimitToSectorUpDown.Size = new System.Drawing.Size(46, 23);
			LimitToSectorUpDown.TabIndex = 80;
			LimitToSectorUpDown.Value = new decimal(new int[] { 9, 0, 0, 0 });
			// 
			// label41
			// 
			label41.AutoSize = true;
			label41.Location = new System.Drawing.Point(462, 248);
			label41.Margin = new Padding(4, 0, 4, 0);
			label41.Name = "label41";
			label41.Size = new System.Drawing.Size(43, 15);
			label41.TabIndex = 80;
			label41.Text = "S Limit";
			// 
			// LimitToTrackUpDown
			// 
			LimitToTrackUpDown.Location = new System.Drawing.Point(517, 216);
			LimitToTrackUpDown.Margin = new Padding(4, 3, 4, 3);
			LimitToTrackUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			LimitToTrackUpDown.Minimum = new decimal(new int[] { 16, 0, 0, int.MinValue });
			LimitToTrackUpDown.Name = "LimitToTrackUpDown";
			LimitToTrackUpDown.Size = new System.Drawing.Size(46, 23);
			LimitToTrackUpDown.TabIndex = 79;
			LimitToTrackUpDown.Value = new decimal(new int[] { 11, 0, 0, 0 });
			// 
			// label42
			// 
			label42.AutoSize = true;
			label42.Location = new System.Drawing.Point(465, 217);
			label42.Margin = new Padding(4, 0, 4, 0);
			label42.Name = "label42";
			label42.Size = new System.Drawing.Size(43, 15);
			label42.TabIndex = 78;
			label42.Text = "T Limit";
			// 
			// MainTabControl
			// 
			MainTabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			MainTabControl.Controls.Add(QuickTab);
			MainTabControl.Controls.Add(CaptureTab);
			MainTabControl.Controls.Add(ProcessingTab);
			MainTabControl.Controls.Add(ErrorCorrectionTab);
			MainTabControl.Controls.Add(AnalysisPage);
			MainTabControl.Controls.Add(AnalysisTab2);
			MainTabControl.Controls.Add(NetworkTab);
			MainTabControl.ImageList = MainTabControlImageList;
			MainTabControl.Location = new System.Drawing.Point(15, 62);
			MainTabControl.Margin = new Padding(4, 3, 4, 3);
			MainTabControl.Name = "MainTabControl";
			MainTabControl.SelectedIndex = 0;
			MainTabControl.Size = new System.Drawing.Size(1136, 925);
			MainTabControl.TabIndex = 56;
			MainTabControl.SelectedIndexChanged += MainTabControl_SelectedIndexChanged;
			// 
			// QuickTab
			// 
			QuickTab.BackColor = System.Drawing.SystemColors.Control;
			QuickTab.Controls.Add(QrtbSectorMap);
			QuickTab.Controls.Add(QProcessingGroupBox);
			QuickTab.Controls.Add(groupBox12);
			QuickTab.ImageIndex = 3;
			QuickTab.Location = new System.Drawing.Point(4, 24);
			QuickTab.Margin = new Padding(4, 3, 4, 3);
			QuickTab.Name = "QuickTab";
			QuickTab.Padding = new Padding(4, 3, 4, 3);
			QuickTab.Size = new System.Drawing.Size(1128, 897);
			QuickTab.TabIndex = 6;
			QuickTab.Text = "Quick";
			// 
			// QrtbSectorMap
			// 
			QrtbSectorMap.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			QrtbSectorMap.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			QrtbSectorMap.HideSelection = false;
			QrtbSectorMap.Location = new System.Drawing.Point(1, 436);
			QrtbSectorMap.Margin = new Padding(4, 3, 4, 3);
			QrtbSectorMap.Name = "QrtbSectorMap";
			QrtbSectorMap.Size = new System.Drawing.Size(1123, 457);
			QrtbSectorMap.TabIndex = 114;
			QrtbSectorMap.Text = "";
			toolTip1.SetToolTip(QrtbSectorMap, "The sectormap shows you in detail what the status is for every sector. Right click on a sector for more options. Double click to refresh.");
			QrtbSectorMap.DoubleClick += RtbSectorMap_DoubleClick;
			QrtbSectorMap.MouseDown += RtbSectorMap_MouseDown;
			// 
			// QProcessingGroupBox
			// 
			QProcessingGroupBox.BackColor = System.Drawing.SystemColors.Control;
			QProcessingGroupBox.Controls.Add(button11);
			QProcessingGroupBox.Controls.Add(button15);
			QProcessingGroupBox.Controls.Add(groupBox14);
			QProcessingGroupBox.Controls.Add(QProcAmigaBtn);
			QProcessingGroupBox.Controls.Add(QProcPCBtn);
			QProcessingGroupBox.Enabled = false;
			QProcessingGroupBox.Location = new System.Drawing.Point(7, 159);
			QProcessingGroupBox.Margin = new Padding(4, 3, 4, 3);
			QProcessingGroupBox.Name = "QProcessingGroupBox";
			QProcessingGroupBox.Padding = new Padding(4, 3, 4, 3);
			QProcessingGroupBox.Size = new System.Drawing.Size(1088, 270);
			QProcessingGroupBox.TabIndex = 113;
			QProcessingGroupBox.TabStop = false;
			QProcessingGroupBox.Text = "Processing";
			// 
			// button11
			// 
			button11.Location = new System.Drawing.Point(0, 182);
			button11.Margin = new Padding(4, 3, 4, 3);
			button11.Name = "button11";
			button11.Size = new System.Drawing.Size(78, 44);
			button11.TabIndex = 119;
			button11.Text = "Reset output";
			toolTip1.SetToolTip(button11, "Clears all data generated by processing so you can start fresh with the same input data.");
			button11.UseVisualStyleBackColor = true;
			button11.Click += Button5_Click;
			// 
			// button15
			// 
			button15.Location = new System.Drawing.Point(0, 129);
			button15.Margin = new Padding(4, 3, 4, 3);
			button15.Name = "button15";
			button15.Size = new System.Drawing.Size(74, 45);
			button15.TabIndex = 118;
			button15.Text = "Reset input";
			toolTip1.SetToolTip(button15, "Clears the input buffer so you can load or capture new data but keeps recovered sectors in tact.");
			button15.UseVisualStyleBackColor = true;
			button15.Click += ResetBuffersBtn_Click;
			// 
			// groupBox14
			// 
			groupBox14.Controls.Add(QOnlyBadSectorsRadio);
			groupBox14.Controls.Add(QECOnRadio);
			groupBox14.Controls.Add(label89);
			groupBox14.Controls.Add(QChangeDiskTypeComboBox);
			groupBox14.Controls.Add(QProcessingModeComboBox);
			groupBox14.Controls.Add(QClearDatacheckBox);
			groupBox14.Controls.Add(groupBox15);
			groupBox14.Controls.Add(QFindDupesCheckBox);
			groupBox14.Controls.Add(QAutoRefreshSectorMapCheck);
			groupBox14.Controls.Add(QIgnoreHeaderErrorCheckBox);
			groupBox14.Controls.Add(groupBox16);
			groupBox14.Controls.Add(groupBox17);
			groupBox14.Controls.Add(QHDCheckBox);
			groupBox14.Controls.Add(label114);
			groupBox14.Location = new System.Drawing.Point(110, 16);
			groupBox14.Margin = new Padding(4, 3, 4, 3);
			groupBox14.Name = "groupBox14";
			groupBox14.Padding = new Padding(4, 3, 4, 3);
			groupBox14.Size = new System.Drawing.Size(972, 250);
			groupBox14.TabIndex = 52;
			groupBox14.TabStop = false;
			groupBox14.Text = "Processing options";
			// 
			// QOnlyBadSectorsRadio
			// 
			QOnlyBadSectorsRadio.AutoSize = true;
			QOnlyBadSectorsRadio.Location = new System.Drawing.Point(7, 106);
			QOnlyBadSectorsRadio.Margin = new Padding(4, 3, 4, 3);
			QOnlyBadSectorsRadio.Name = "QOnlyBadSectorsRadio";
			QOnlyBadSectorsRadio.Size = new System.Drawing.Size(113, 19);
			QOnlyBadSectorsRadio.TabIndex = 106;
			QOnlyBadSectorsRadio.Text = "Only bad sectors";
			toolTip1.SetToolTip(QOnlyBadSectorsRadio, "Only reprocess bad sectors. Can only work if some bad sectors have already been found. This will speed up scanning a lot but may miss some sectors.");
			QOnlyBadSectorsRadio.UseVisualStyleBackColor = true;
			QOnlyBadSectorsRadio.CheckedChanged += QOnlyBadSectorsRadio_CheckedChanged;
			// 
			// QECOnRadio
			// 
			QECOnRadio.AutoSize = true;
			QECOnRadio.Checked = true;
			QECOnRadio.Location = new System.Drawing.Point(7, 133);
			QECOnRadio.Margin = new Padding(4, 3, 4, 3);
			QECOnRadio.Name = "QECOnRadio";
			QECOnRadio.Size = new System.Drawing.Size(129, 19);
			QECOnRadio.TabIndex = 105;
			QECOnRadio.TabStop = true;
			QECOnRadio.Text = "Use error correction";
			toolTip1.SetToolTip(QECOnRadio, "Collects error correction data to help correcting it in the Error Correction tab. No correction is performed during processing.");
			QECOnRadio.UseVisualStyleBackColor = true;
			QECOnRadio.CheckedChanged += QECOnRadio_CheckedChanged;
			// 
			// label89
			// 
			label89.AutoSize = true;
			label89.Location = new System.Drawing.Point(138, 22);
			label89.Margin = new Padding(4, 0, 4, 0);
			label89.Name = "label89";
			label89.Size = new System.Drawing.Size(70, 15);
			label89.TabIndex = 104;
			label89.Text = "Disk Format";
			// 
			// QChangeDiskTypeComboBox
			// 
			QChangeDiskTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			QChangeDiskTypeComboBox.FormattingEnabled = true;
			QChangeDiskTypeComboBox.Location = new System.Drawing.Point(141, 40);
			QChangeDiskTypeComboBox.Margin = new Padding(4, 3, 4, 3);
			QChangeDiskTypeComboBox.Name = "QChangeDiskTypeComboBox";
			QChangeDiskTypeComboBox.Size = new System.Drawing.Size(87, 23);
			QChangeDiskTypeComboBox.TabIndex = 102;
			toolTip1.SetToolTip(QChangeDiskTypeComboBox, "The format is normally recognized automatically but it may happen that a single disk contains multiple disk types due to copying disks partly.");
			QChangeDiskTypeComboBox.SelectedIndexChanged += QChangeDiskTypeComboBox_SelectedIndexChanged;
			// 
			// QProcessingModeComboBox
			// 
			QProcessingModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			QProcessingModeComboBox.FormattingEnabled = true;
			QProcessingModeComboBox.Location = new System.Drawing.Point(7, 40);
			QProcessingModeComboBox.Margin = new Padding(4, 3, 4, 3);
			QProcessingModeComboBox.Name = "QProcessingModeComboBox";
			QProcessingModeComboBox.Size = new System.Drawing.Size(123, 23);
			QProcessingModeComboBox.TabIndex = 103;
			toolTip1.SetToolTip(QProcessingModeComboBox, "Adaptive1 is both fast and accurate. Normal uses simple thresholds which works fine for good quality captures. Aufit may recognize the odd sector. Other options are experimental.");
			QProcessingModeComboBox.SelectedIndexChanged += QProcessingModeComboBox_SelectedIndexChanged;
			// 
			// QClearDatacheckBox
			// 
			QClearDatacheckBox.AutoSize = true;
			QClearDatacheckBox.Location = new System.Drawing.Point(7, 80);
			QClearDatacheckBox.Margin = new Padding(4, 3, 4, 3);
			QClearDatacheckBox.Name = "QClearDatacheckBox";
			QClearDatacheckBox.Size = new System.Drawing.Size(114, 19);
			QClearDatacheckBox.TabIndex = 101;
			QClearDatacheckBox.Text = "Clear sector data";
			toolTip1.SetToolTip(QClearDatacheckBox, "Clear recovered data that was found during the last processing job. This can speed up scanning.");
			QClearDatacheckBox.UseVisualStyleBackColor = true;
			QClearDatacheckBox.CheckedChanged += QClearDatacheckBox_CheckedChanged;
			// 
			// groupBox15
			// 
			groupBox15.Controls.Add(QOffsetUpDown);
			groupBox15.Controls.Add(QMaxUpDown);
			groupBox15.Controls.Add(QSixEightUpDown);
			groupBox15.Controls.Add(QFourSixUpDown);
			groupBox15.Controls.Add(QMinUpDown);
			groupBox15.Controls.Add(QOffsetLabel);
			groupBox15.Controls.Add(QFourSixLabel);
			groupBox15.Controls.Add(QMinLabel);
			groupBox15.Controls.Add(QSixEightLabel);
			groupBox15.Controls.Add(QMaxLabel);
			groupBox15.Location = new System.Drawing.Point(344, 17);
			groupBox15.Margin = new Padding(4, 3, 4, 3);
			groupBox15.Name = "groupBox15";
			groupBox15.Padding = new Padding(4, 3, 4, 3);
			groupBox15.Size = new System.Drawing.Size(287, 82);
			groupBox15.TabIndex = 89;
			groupBox15.TabStop = false;
			groupBox15.Text = "Thresholds";
			// 
			// QOffsetUpDown
			// 
			QOffsetUpDown.Location = new System.Drawing.Point(229, 46);
			QOffsetUpDown.Margin = new Padding(4, 3, 4, 3);
			QOffsetUpDown.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
			QOffsetUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			QOffsetUpDown.Name = "QOffsetUpDown";
			QOffsetUpDown.Size = new System.Drawing.Size(46, 23);
			QOffsetUpDown.TabIndex = 80;
			QOffsetUpDown.ValueChanged += QMinUpDown_ValueChanged;
			// 
			// QMaxUpDown
			// 
			QMaxUpDown.Location = new System.Drawing.Point(173, 46);
			QMaxUpDown.Margin = new Padding(4, 3, 4, 3);
			QMaxUpDown.Maximum = new decimal(new int[] { 264, 0, 0, 0 });
			QMaxUpDown.Name = "QMaxUpDown";
			QMaxUpDown.Size = new System.Drawing.Size(46, 23);
			QMaxUpDown.TabIndex = 80;
			QMaxUpDown.Value = new decimal(new int[] { 147, 0, 0, 0 });
			QMaxUpDown.ValueChanged += QMinUpDown_ValueChanged;
			// 
			// QSixEightUpDown
			// 
			QSixEightUpDown.Location = new System.Drawing.Point(113, 45);
			QSixEightUpDown.Margin = new Padding(4, 3, 4, 3);
			QSixEightUpDown.Maximum = new decimal(new int[] { 264, 0, 0, 0 });
			QSixEightUpDown.Name = "QSixEightUpDown";
			QSixEightUpDown.Size = new System.Drawing.Size(46, 23);
			QSixEightUpDown.TabIndex = 80;
			QSixEightUpDown.Value = new decimal(new int[] { 108, 0, 0, 0 });
			QSixEightUpDown.ValueChanged += QMinUpDown_ValueChanged;
			// 
			// QFourSixUpDown
			// 
			QFourSixUpDown.Location = new System.Drawing.Point(61, 45);
			QFourSixUpDown.Margin = new Padding(4, 3, 4, 3);
			QFourSixUpDown.Maximum = new decimal(new int[] { 264, 0, 0, 0 });
			QFourSixUpDown.Name = "QFourSixUpDown";
			QFourSixUpDown.Size = new System.Drawing.Size(46, 23);
			QFourSixUpDown.TabIndex = 80;
			QFourSixUpDown.Value = new decimal(new int[] { 69, 0, 0, 0 });
			QFourSixUpDown.ValueChanged += QMinUpDown_ValueChanged;
			// 
			// QMinUpDown
			// 
			QMinUpDown.Location = new System.Drawing.Point(7, 45);
			QMinUpDown.Margin = new Padding(4, 3, 4, 3);
			QMinUpDown.Maximum = new decimal(new int[] { 264, 0, 0, 0 });
			QMinUpDown.Name = "QMinUpDown";
			QMinUpDown.Size = new System.Drawing.Size(46, 23);
			QMinUpDown.TabIndex = 80;
			QMinUpDown.ValueChanged += QMinUpDown_ValueChanged;
			// 
			// QOffsetLabel
			// 
			QOffsetLabel.AutoSize = true;
			QOffsetLabel.Location = new System.Drawing.Point(225, 25);
			QOffsetLabel.Margin = new Padding(4, 0, 4, 0);
			QOffsetLabel.Name = "QOffsetLabel";
			QOffsetLabel.Size = new System.Drawing.Size(39, 15);
			QOffsetLabel.TabIndex = 56;
			QOffsetLabel.Text = "Offset";
			// 
			// QFourSixLabel
			// 
			QFourSixLabel.AutoSize = true;
			QFourSixLabel.Location = new System.Drawing.Point(59, 27);
			QFourSixLabel.Margin = new Padding(4, 0, 4, 0);
			QFourSixLabel.Name = "QFourSixLabel";
			QFourSixLabel.Size = new System.Drawing.Size(24, 15);
			QFourSixLabel.TabIndex = 64;
			QFourSixLabel.Text = "4/6";
			// 
			// QMinLabel
			// 
			QMinLabel.AutoSize = true;
			QMinLabel.Location = new System.Drawing.Point(7, 27);
			QMinLabel.Margin = new Padding(4, 0, 4, 0);
			QMinLabel.Name = "QMinLabel";
			QMinLabel.Size = new System.Drawing.Size(28, 15);
			QMinLabel.TabIndex = 63;
			QMinLabel.Text = "min";
			// 
			// QSixEightLabel
			// 
			QSixEightLabel.AutoSize = true;
			QSixEightLabel.Location = new System.Drawing.Point(110, 25);
			QSixEightLabel.Margin = new Padding(4, 0, 4, 0);
			QSixEightLabel.Name = "QSixEightLabel";
			QSixEightLabel.Size = new System.Drawing.Size(24, 15);
			QSixEightLabel.TabIndex = 61;
			QSixEightLabel.Text = "6/8";
			// 
			// QMaxLabel
			// 
			QMaxLabel.AutoSize = true;
			QMaxLabel.Location = new System.Drawing.Point(173, 25);
			QMaxLabel.Margin = new Padding(4, 0, 4, 0);
			QMaxLabel.Name = "QMaxLabel";
			QMaxLabel.Size = new System.Drawing.Size(30, 15);
			QMaxLabel.TabIndex = 58;
			QMaxLabel.Text = "max";
			// 
			// QFindDupesCheckBox
			// 
			QFindDupesCheckBox.AccessibleDescription = "";
			QFindDupesCheckBox.AutoSize = true;
			QFindDupesCheckBox.Location = new System.Drawing.Point(150, 106);
			QFindDupesCheckBox.Margin = new Padding(4, 3, 4, 3);
			QFindDupesCheckBox.Name = "QFindDupesCheckBox";
			QFindDupesCheckBox.Size = new System.Drawing.Size(89, 19);
			QFindDupesCheckBox.TabIndex = 88;
			QFindDupesCheckBox.Text = "Deduplicate";
			toolTip1.SetToolTip(QFindDupesCheckBox, "When processing, a hash is calculated. If the new sector is unique, it will only then be added to the list. This avoids processing duplicate data and may save time.");
			QFindDupesCheckBox.UseVisualStyleBackColor = true;
			QFindDupesCheckBox.CheckedChanged += QFindDupesCheckBox_CheckedChanged;
			// 
			// QAutoRefreshSectorMapCheck
			// 
			QAutoRefreshSectorMapCheck.AccessibleDescription = "";
			QAutoRefreshSectorMapCheck.AutoSize = true;
			QAutoRefreshSectorMapCheck.Checked = true;
			QAutoRefreshSectorMapCheck.CheckState = CheckState.Checked;
			QAutoRefreshSectorMapCheck.Location = new System.Drawing.Point(150, 136);
			QAutoRefreshSectorMapCheck.Margin = new Padding(4, 3, 4, 3);
			QAutoRefreshSectorMapCheck.Name = "QAutoRefreshSectorMapCheck";
			QAutoRefreshSectorMapCheck.Size = new System.Drawing.Size(150, 19);
			QAutoRefreshSectorMapCheck.TabIndex = 85;
			QAutoRefreshSectorMapCheck.Text = "Auto refresh sectormap";
			toolTip1.SetToolTip(QAutoRefreshSectorMapCheck, resources.GetString("QAutoRefreshSectorMapCheck.ToolTip"));
			QAutoRefreshSectorMapCheck.UseVisualStyleBackColor = true;
			QAutoRefreshSectorMapCheck.CheckedChanged += QAutoRefreshSectorMapCheck_CheckedChanged;
			// 
			// QIgnoreHeaderErrorCheckBox
			// 
			QIgnoreHeaderErrorCheckBox.AutoSize = true;
			QIgnoreHeaderErrorCheckBox.Location = new System.Drawing.Point(150, 80);
			QIgnoreHeaderErrorCheckBox.Margin = new Padding(4, 3, 4, 3);
			QIgnoreHeaderErrorCheckBox.Name = "QIgnoreHeaderErrorCheckBox";
			QIgnoreHeaderErrorCheckBox.Size = new System.Drawing.Size(127, 19);
			QIgnoreHeaderErrorCheckBox.TabIndex = 50;
			QIgnoreHeaderErrorCheckBox.Text = "Ignore header error";
			toolTip1.SetToolTip(QIgnoreHeaderErrorCheckBox, "In rare cases the crc = not ok on some headers can be ignored. On one msx disk this was some kind of copy protection.");
			QIgnoreHeaderErrorCheckBox.UseVisualStyleBackColor = true;
			QIgnoreHeaderErrorCheckBox.CheckedChanged += QIgnoreHeaderErrorCheckBox_CheckedChanged;
			// 
			// groupBox16
			// 
			groupBox16.Controls.Add(QHistoPanel);
			groupBox16.Controls.Add(QHistogramhScrollBar1);
			groupBox16.Location = new System.Drawing.Point(640, 9);
			groupBox16.Margin = new Padding(4, 3, 4, 3);
			groupBox16.Name = "groupBox16";
			groupBox16.Padding = new Padding(4, 3, 4, 3);
			groupBox16.Size = new System.Drawing.Size(324, 192);
			groupBox16.TabIndex = 53;
			groupBox16.TabStop = false;
			groupBox16.Text = "Histogram";
			// 
			// QHistoPanel
			// 
			QHistoPanel.Location = new System.Drawing.Point(7, 22);
			QHistoPanel.Margin = new Padding(4, 3, 4, 3);
			QHistoPanel.Name = "QHistoPanel";
			QHistoPanel.Size = new System.Drawing.Size(303, 126);
			QHistoPanel.TabIndex = 36;
			QHistoPanel.Click += Histogrampanel1_Click;
			QHistoPanel.Paint += Histogrampanel1_Paint;
			// 
			// QHistogramhScrollBar1
			// 
			QHistogramhScrollBar1.LargeChange = 10000;
			QHistogramhScrollBar1.Location = new System.Drawing.Point(1, 167);
			QHistogramhScrollBar1.Margin = new Padding(4, 3, 4, 3);
			QHistogramhScrollBar1.Maximum = 4000;
			QHistogramhScrollBar1.Name = "QHistogramhScrollBar1";
			QHistogramhScrollBar1.Size = new System.Drawing.Size(323, 45);
			QHistogramhScrollBar1.TabIndex = 105;
			QHistogramhScrollBar1.TickFrequency = 2000;
			QHistogramhScrollBar1.TickStyle = TickStyle.None;
			QHistogramhScrollBar1.Scroll += QHistogramhScrollBar1_Scroll;
			// 
			// groupBox17
			// 
			groupBox17.Controls.Add(QLimitTSCheckBox);
			groupBox17.Controls.Add(QRateOfChange2UpDown);
			groupBox17.Controls.Add(label88);
			groupBox17.Controls.Add(label102);
			groupBox17.Controls.Add(QRateOfChangeUpDown);
			groupBox17.Controls.Add(QScanComboBox);
			groupBox17.Controls.Add(button47);
			groupBox17.Controls.Add(QAdaptOfsset2UpDown);
			groupBox17.Controls.Add(label90);
			groupBox17.Controls.Add(QLimitToSectorUpDown);
			groupBox17.Controls.Add(label91);
			groupBox17.Controls.Add(QLimitToTrackUpDown);
			groupBox17.Location = new System.Drawing.Point(344, 106);
			groupBox17.Margin = new Padding(4, 3, 4, 3);
			groupBox17.Name = "groupBox17";
			groupBox17.Padding = new Padding(4, 3, 4, 3);
			groupBox17.Size = new System.Drawing.Size(289, 137);
			groupBox17.TabIndex = 54;
			groupBox17.TabStop = false;
			groupBox17.Text = "Scan";
			// 
			// QLimitTSCheckBox
			// 
			QLimitTSCheckBox.AutoSize = true;
			QLimitTSCheckBox.Location = new System.Drawing.Point(12, 87);
			QLimitTSCheckBox.Margin = new Padding(4, 3, 4, 3);
			QLimitTSCheckBox.Name = "QLimitTSCheckBox";
			QLimitTSCheckBox.Size = new System.Drawing.Size(73, 19);
			QLimitTSCheckBox.TabIndex = 109;
			QLimitTSCheckBox.Text = "Limit T/S";
			toolTip1.SetToolTip(QLimitTSCheckBox, "Limits processing to this track and sector.");
			QLimitTSCheckBox.UseVisualStyleBackColor = true;
			QLimitTSCheckBox.CheckedChanged += QLimitTSCheckBox_CheckedChanged;
			// 
			// QRateOfChange2UpDown
			// 
			QRateOfChange2UpDown.Increment = new decimal(new int[] { 8, 0, 0, 0 });
			QRateOfChange2UpDown.Location = new System.Drawing.Point(217, 48);
			QRateOfChange2UpDown.Margin = new Padding(4, 3, 4, 3);
			QRateOfChange2UpDown.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
			QRateOfChange2UpDown.Name = "QRateOfChange2UpDown";
			QRateOfChange2UpDown.Size = new System.Drawing.Size(56, 23);
			QRateOfChange2UpDown.TabIndex = 108;
			toolTip1.SetToolTip(QRateOfChange2UpDown, "How quickly to adapt to change, larger number adapts slower to change in the signal. Use low numbers for glued disks or disks with lots of bad sectors or noise.");
			QRateOfChange2UpDown.Value = new decimal(new int[] { 1300, 0, 0, 0 });
			QRateOfChange2UpDown.ValueChanged += QRateOfChange2UpDown_ValueChanged;
			// 
			// label88
			// 
			label88.AutoSize = true;
			label88.Location = new System.Drawing.Point(140, 51);
			label88.Margin = new Padding(4, 0, 4, 0);
			label88.Name = "label88";
			label88.Size = new System.Drawing.Size(68, 15);
			label88.TabIndex = 107;
			label88.Text = "Adapt track";
			// 
			// label102
			// 
			label102.AutoSize = true;
			label102.Location = new System.Drawing.Point(8, 52);
			label102.Margin = new Padding(4, 0, 4, 0);
			label102.Name = "label102";
			label102.Size = new System.Drawing.Size(62, 15);
			label102.TabIndex = 106;
			label102.Text = "Adapt rate";
			// 
			// QRateOfChangeUpDown
			// 
			QRateOfChangeUpDown.DecimalPlaces = 2;
			QRateOfChangeUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
			QRateOfChangeUpDown.Location = new System.Drawing.Point(77, 50);
			QRateOfChangeUpDown.Margin = new Padding(4, 3, 4, 3);
			QRateOfChangeUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			QRateOfChangeUpDown.Name = "QRateOfChangeUpDown";
			QRateOfChangeUpDown.Size = new System.Drawing.Size(56, 23);
			QRateOfChangeUpDown.TabIndex = 105;
			QRateOfChangeUpDown.Value = new decimal(new int[] { 12, 0, 0, 65536 });
			QRateOfChangeUpDown.ValueChanged += QRateOfChangeUpDown_ValueChanged;
			// 
			// QScanComboBox
			// 
			QScanComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			QScanComboBox.FormattingEnabled = true;
			QScanComboBox.Location = new System.Drawing.Point(7, 18);
			QScanComboBox.Margin = new Padding(4, 3, 4, 3);
			QScanComboBox.Name = "QScanComboBox";
			QScanComboBox.Size = new System.Drawing.Size(190, 23);
			QScanComboBox.TabIndex = 104;
			QScanComboBox.SelectedIndexChanged += QScanComboBox_SelectedIndexChanged;
			// 
			// button47
			// 
			button47.Location = new System.Drawing.Point(204, 16);
			button47.Margin = new Padding(4, 3, 4, 3);
			button47.Name = "button47";
			button47.Size = new System.Drawing.Size(47, 27);
			button47.TabIndex = 39;
			button47.Text = "Scan";
			toolTip1.SetToolTip(button47, "Use scanning to try different combinations of settings in order to find yet to be recovered sectors.");
			button47.UseVisualStyleBackColor = true;
			button47.Click += ScanBtn_Click_1;
			// 
			// QAdaptOfsset2UpDown
			// 
			QAdaptOfsset2UpDown.DecimalPlaces = 2;
			QAdaptOfsset2UpDown.Location = new System.Drawing.Point(217, 76);
			QAdaptOfsset2UpDown.Margin = new Padding(4, 3, 4, 3);
			QAdaptOfsset2UpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			QAdaptOfsset2UpDown.Minimum = new decimal(new int[] { 2000, 0, 0, int.MinValue });
			QAdaptOfsset2UpDown.Name = "QAdaptOfsset2UpDown";
			QAdaptOfsset2UpDown.Size = new System.Drawing.Size(56, 23);
			QAdaptOfsset2UpDown.TabIndex = 99;
			QAdaptOfsset2UpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			QAdaptOfsset2UpDown.ValueChanged += QAdaptOfsset2UpDown_ValueChanged;
			// 
			// label90
			// 
			label90.AutoSize = true;
			label90.Location = new System.Drawing.Point(8, 110);
			label90.Margin = new Padding(4, 0, 4, 0);
			label90.Name = "label90";
			label90.Size = new System.Drawing.Size(43, 15);
			label90.TabIndex = 78;
			label90.Text = "T Limit";
			// 
			// QLimitToSectorUpDown
			// 
			QLimitToSectorUpDown.Location = new System.Drawing.Point(152, 108);
			QLimitToSectorUpDown.Margin = new Padding(4, 3, 4, 3);
			QLimitToSectorUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			QLimitToSectorUpDown.Minimum = new decimal(new int[] { 16, 0, 0, int.MinValue });
			QLimitToSectorUpDown.Name = "QLimitToSectorUpDown";
			QLimitToSectorUpDown.Size = new System.Drawing.Size(46, 23);
			QLimitToSectorUpDown.TabIndex = 80;
			QLimitToSectorUpDown.Value = new decimal(new int[] { 9, 0, 0, 0 });
			QLimitToSectorUpDown.ValueChanged += QLimitToSectorUpDown_ValueChanged;
			// 
			// label91
			// 
			label91.AutoSize = true;
			label91.Location = new System.Drawing.Point(108, 111);
			label91.Margin = new Padding(4, 0, 4, 0);
			label91.Name = "label91";
			label91.Size = new System.Drawing.Size(43, 15);
			label91.TabIndex = 80;
			label91.Text = "S Limit";
			// 
			// QLimitToTrackUpDown
			// 
			QLimitToTrackUpDown.Location = new System.Drawing.Point(50, 107);
			QLimitToTrackUpDown.Margin = new Padding(4, 3, 4, 3);
			QLimitToTrackUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			QLimitToTrackUpDown.Minimum = new decimal(new int[] { 16, 0, 0, int.MinValue });
			QLimitToTrackUpDown.Name = "QLimitToTrackUpDown";
			QLimitToTrackUpDown.Size = new System.Drawing.Size(46, 23);
			QLimitToTrackUpDown.TabIndex = 79;
			QLimitToTrackUpDown.Value = new decimal(new int[] { 11, 0, 0, 0 });
			QLimitToTrackUpDown.ValueChanged += QLimitToTrackUpDown_ValueChanged;
			// 
			// QHDCheckBox
			// 
			QHDCheckBox.AutoSize = true;
			QHDCheckBox.Enabled = false;
			QHDCheckBox.Location = new System.Drawing.Point(6, 159);
			QHDCheckBox.Margin = new Padding(4, 3, 4, 3);
			QHDCheckBox.Name = "QHDCheckBox";
			QHDCheckBox.Size = new System.Drawing.Size(94, 19);
			QHDCheckBox.TabIndex = 51;
			QHDCheckBox.Text = "High Density";
			toolTip1.SetToolTip(QHDCheckBox, "Is the disk incorrectly recognized as double density or vice versa? Change it here.");
			QHDCheckBox.UseVisualStyleBackColor = true;
			QHDCheckBox.CheckedChanged += QHDCheckBox_CheckedChanged;
			// 
			// label114
			// 
			label114.AutoSize = true;
			label114.Location = new System.Drawing.Point(4, 22);
			label114.Margin = new Padding(4, 0, 4, 0);
			label114.Name = "label114";
			label114.Size = new System.Drawing.Size(95, 15);
			label114.TabIndex = 44;
			label114.Text = "MFM processing";
			// 
			// QProcAmigaBtn
			// 
			QProcAmigaBtn.Location = new System.Drawing.Point(0, 22);
			QProcAmigaBtn.Margin = new Padding(4, 3, 4, 3);
			QProcAmigaBtn.Name = "QProcAmigaBtn";
			QProcAmigaBtn.Size = new System.Drawing.Size(88, 46);
			QProcAmigaBtn.TabIndex = 51;
			QProcAmigaBtn.Text = "Process Amiga!";
			QProcAmigaBtn.UseVisualStyleBackColor = true;
			QProcAmigaBtn.Click += ProcessAmigaBtn_Click;
			// 
			// QProcPCBtn
			// 
			QProcPCBtn.Location = new System.Drawing.Point(0, 75);
			QProcPCBtn.Margin = new Padding(4, 3, 4, 3);
			QProcPCBtn.Name = "QProcPCBtn";
			QProcPCBtn.Size = new System.Drawing.Size(88, 46);
			QProcPCBtn.TabIndex = 50;
			QProcPCBtn.Text = "Process PC!";
			QProcPCBtn.UseVisualStyleBackColor = true;
			QProcPCBtn.Click += ProcessPCBtn_Click;
			// 
			// groupBox12
			// 
			groupBox12.BackColor = System.Drawing.SystemColors.Control;
			groupBox12.Controls.Add(GluedDiskPreset);
			groupBox12.Controls.Add(StepStickPresetBtn);
			groupBox12.Controls.Add(QDirectStepPresetBtn);
			groupBox12.Controls.Add(QRecaptureAllBtn);
			groupBox12.Controls.Add(button12);
			groupBox12.Controls.Add(button13);
			groupBox12.Controls.Add(button14);
			groupBox12.Controls.Add(QDirectStepCheckBox);
			groupBox12.Controls.Add(QCaptureBtn);
			groupBox12.Controls.Add(groupBox10);
			groupBox12.Location = new System.Drawing.Point(7, 7);
			groupBox12.Margin = new Padding(4, 3, 4, 3);
			groupBox12.Name = "groupBox12";
			groupBox12.Padding = new Padding(4, 3, 4, 3);
			groupBox12.Size = new System.Drawing.Size(1096, 144);
			groupBox12.TabIndex = 112;
			groupBox12.TabStop = false;
			groupBox12.Text = "Capture";
			// 
			// GluedDiskPreset
			// 
			GluedDiskPreset.Location = new System.Drawing.Point(401, 105);
			GluedDiskPreset.Margin = new Padding(4, 3, 4, 3);
			GluedDiskPreset.Name = "GluedDiskPreset";
			GluedDiskPreset.Size = new System.Drawing.Size(114, 27);
			GluedDiskPreset.TabIndex = 121;
			GluedDiskPreset.Text = "Glued preset";
			toolTip1.SetToolTip(GluedDiskPreset, "If you use a stepstick, click this button to setup all settings for this mode of operation.");
			GluedDiskPreset.Click += GluedDiskPreset_Click;
			// 
			// StepStickPresetBtn
			// 
			StepStickPresetBtn.Location = new System.Drawing.Point(280, 105);
			StepStickPresetBtn.Margin = new Padding(4, 3, 4, 3);
			StepStickPresetBtn.Name = "StepStickPresetBtn";
			StepStickPresetBtn.Size = new System.Drawing.Size(114, 27);
			StepStickPresetBtn.TabIndex = 120;
			StepStickPresetBtn.Text = "StepStick preset";
			toolTip1.SetToolTip(StepStickPresetBtn, "If you use a stepstick, click this button to setup all settings for this mode of operation.");
			StepStickPresetBtn.Click += StepStickPresetBtn_Click;
			// 
			// QDirectStepPresetBtn
			// 
			QDirectStepPresetBtn.Location = new System.Drawing.Point(156, 105);
			QDirectStepPresetBtn.Margin = new Padding(4, 3, 4, 3);
			QDirectStepPresetBtn.Name = "QDirectStepPresetBtn";
			QDirectStepPresetBtn.Size = new System.Drawing.Size(114, 27);
			QDirectStepPresetBtn.TabIndex = 119;
			QDirectStepPresetBtn.Text = "Direct preset";
			toolTip1.SetToolTip(QDirectStepPresetBtn, "If you don't use a stepstick, click this button to setup all settings for this mode of operation.");
			QDirectStepPresetBtn.Click += DirectPresetBtn_Click;
			// 
			// QRecaptureAllBtn
			// 
			QRecaptureAllBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			QRecaptureAllBtn.ImageIndex = 0;
			QRecaptureAllBtn.ImageList = MainTabControlImageList;
			QRecaptureAllBtn.Location = new System.Drawing.Point(7, 75);
			QRecaptureAllBtn.Margin = new Padding(4, 3, 4, 3);
			QRecaptureAllBtn.Name = "QRecaptureAllBtn";
			QRecaptureAllBtn.Size = new System.Drawing.Size(106, 46);
			QRecaptureAllBtn.TabIndex = 118;
			QRecaptureAllBtn.Text = "Recapture all";
			QRecaptureAllBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			QRecaptureAllBtn.UseVisualStyleBackColor = true;
			QRecaptureAllBtn.Click += Button48_Click;
			// 
			// MainTabControlImageList
			// 
			MainTabControlImageList.ColorDepth = ColorDepth.Depth32Bit;
			MainTabControlImageList.ImageStream = (ImageListStreamer)resources.GetObject("MainTabControlImageList.ImageStream");
			MainTabControlImageList.TransparentColor = System.Drawing.Color.Transparent;
			MainTabControlImageList.Images.SetKeyName(0, "IconRecord.png");
			MainTabControlImageList.Images.SetKeyName(1, "IconCheckmarkGreen.png");
			MainTabControlImageList.Images.SetKeyName(2, "IconScope.png");
			MainTabControlImageList.Images.SetKeyName(3, "IconProcessing2.png");
			MainTabControlImageList.Images.SetKeyName(4, "IconGraphEditor.png");
			MainTabControlImageList.Images.SetKeyName(5, "IconAnalysis.png");
			// 
			// button12
			// 
			button12.Location = new System.Drawing.Point(1004, 106);
			button12.Margin = new Padding(4, 3, 4, 3);
			button12.Name = "button12";
			button12.Size = new System.Drawing.Size(84, 29);
			button12.TabIndex = 117;
			button12.Text = "Step >";
			toolTip1.SetToolTip(button12, "(micro) step forward. Can be used if the head alignment is off.");
			button12.UseVisualStyleBackColor = true;
			button12.Click += StepForwardBtn_Click;
			// 
			// button13
			// 
			button13.Location = new System.Drawing.Point(913, 106);
			button13.Margin = new Padding(4, 3, 4, 3);
			button13.Name = "button13";
			button13.Size = new System.Drawing.Size(84, 29);
			button13.TabIndex = 116;
			button13.Text = "Step <";
			toolTip1.SetToolTip(button13, "(micro) step back. Can be used if the head alignment is off.");
			button13.UseVisualStyleBackColor = true;
			button13.Click += StepBackBtn_Click;
			// 
			// button14
			// 
			button14.Location = new System.Drawing.Point(822, 106);
			button14.Margin = new Padding(4, 3, 4, 3);
			button14.Name = "button14";
			button14.Size = new System.Drawing.Size(84, 29);
			button14.TabIndex = 115;
			button14.Text = "Microstep 8";
			toolTip1.SetToolTip(button14, "Set microstepping to 8 microsteps.");
			button14.UseVisualStyleBackColor = true;
			button14.Click += Microstep8Btn_Click;
			// 
			// QDirectStepCheckBox
			// 
			QDirectStepCheckBox.AutoSize = true;
			settings1.BaseFileName = "";
			settings1.DefaultBaud = 5000000;
			settings1.DefaultPort = "COM21";
			settings1.DirectStep = false;
			settings1.FourSix = 0;
			settings1.GuiMode = "dev";
			settings1.Max = 0;
			settings1.MicroStepsPerTrack = new decimal(new int[] { 8, 0, 0, 0 });
			settings1.Min = 0;
			settings1.Offset = 0;
			settings1.PathToRecoveredDisks = "D:\\data\\Projects\\FloppyControl\\DiskRecoveries";
			settings1.ScopeConnection = "";
			settings1.SettingsKey = "";
			settings1.SixEight = 0;
			settings1.StepStickMicrostepping = new decimal(new int[] { 8, 0, 0, 0 });
			settings1.TooltipDisable = false;
			settings1.TrackDuration = new decimal(new int[] { 330, 0, 0, 0 });
			settings1.TRK00Offset = new decimal(new int[] { 0, 0, 0, 0 });
			settings1.WindowSizeX = 1892;
			settings1.WindowSizeY = 1090;
			QDirectStepCheckBox.DataBindings.Add(new Binding("Checked", settings1, "DirectStep", true, DataSourceUpdateMode.OnPropertyChanged));
			QDirectStepCheckBox.Location = new System.Drawing.Point(716, 112);
			QDirectStepCheckBox.Margin = new Padding(4, 3, 4, 3);
			QDirectStepCheckBox.Name = "QDirectStepCheckBox";
			QDirectStepCheckBox.Size = new System.Drawing.Size(80, 19);
			QDirectStepCheckBox.TabIndex = 114;
			QDirectStepCheckBox.Text = "DirectStep";
			toolTip1.SetToolTip(QDirectStepCheckBox, "If you don't use a step stick, check this box. If you do, clear it.");
			QDirectStepCheckBox.UseVisualStyleBackColor = true;
			QDirectStepCheckBox.CheckedChanged += QDirectStepCheckBox_CheckedChanged;
			// 
			// QCaptureBtn
			// 
			QCaptureBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			QCaptureBtn.ImageIndex = 0;
			QCaptureBtn.ImageList = MainTabControlImageList;
			QCaptureBtn.Location = new System.Drawing.Point(7, 22);
			QCaptureBtn.Margin = new Padding(4, 3, 4, 3);
			QCaptureBtn.Name = "QCaptureBtn";
			QCaptureBtn.Size = new System.Drawing.Size(80, 46);
			QCaptureBtn.TabIndex = 113;
			QCaptureBtn.Text = "Capture";
			QCaptureBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			toolTip1.SetToolTip(QCaptureBtn, "Immediately start capturing using the start track, end track and duration settings.");
			QCaptureBtn.UseVisualStyleBackColor = true;
			QCaptureBtn.Click += CaptureClassbutton_Click;
			// 
			// groupBox10
			// 
			groupBox10.Controls.Add(groupBox11);
			groupBox10.Controls.Add(QTrackDurationUpDown);
			groupBox10.Controls.Add(QStartTrackUpDown);
			groupBox10.Controls.Add(QEndTracksUpDown);
			groupBox10.Controls.Add(label82);
			groupBox10.Controls.Add(QTRK00OffsetUpDown);
			groupBox10.Controls.Add(QMicrostepsPerTrackUpDown);
			groupBox10.Controls.Add(label83);
			groupBox10.Controls.Add(label84);
			groupBox10.Controls.Add(label85);
			groupBox10.Controls.Add(label86);
			groupBox10.Location = new System.Drawing.Point(156, 18);
			groupBox10.Margin = new Padding(4, 3, 4, 3);
			groupBox10.Name = "groupBox10";
			groupBox10.Padding = new Padding(4, 3, 4, 3);
			groupBox10.Size = new System.Drawing.Size(932, 81);
			groupBox10.TabIndex = 112;
			groupBox10.TabStop = false;
			groupBox10.Text = "Capture options";
			// 
			// groupBox11
			// 
			groupBox11.Controls.Add(PresetCaptureDuration50s);
			groupBox11.Controls.Add(PresetCaptureDuration2s);
			groupBox11.Controls.Add(PresetCaptureDuration1s);
			groupBox11.Controls.Add(PresetCaptureDuration5s);
			groupBox11.Controls.Add(PresetTrack78_164);
			groupBox11.Controls.Add(PresetTrack80_90);
			groupBox11.Controls.Add(PresetCaptureDefaultBtn);
			groupBox11.Controls.Add(button37);
			groupBox11.Location = new System.Drawing.Point(491, 20);
			groupBox11.Margin = new Padding(4, 3, 4, 3);
			groupBox11.Name = "groupBox11";
			groupBox11.Padding = new Padding(4, 3, 4, 3);
			groupBox11.Size = new System.Drawing.Size(434, 61);
			groupBox11.TabIndex = 58;
			groupBox11.TabStop = false;
			groupBox11.Text = "Preset capture";
			// 
			// PresetCaptureDuration50s
			// 
			PresetCaptureDuration50s.Location = new System.Drawing.Point(387, 22);
			PresetCaptureDuration50s.Margin = new Padding(4, 3, 4, 3);
			PresetCaptureDuration50s.Name = "PresetCaptureDuration50s";
			PresetCaptureDuration50s.Size = new System.Drawing.Size(43, 27);
			PresetCaptureDuration50s.TabIndex = 61;
			PresetCaptureDuration50s.Text = "50s";
			PresetCaptureDuration50s.UseVisualStyleBackColor = true;
			PresetCaptureDuration50s.Click += PresetCaptureDuration50s_Click;
			// 
			// PresetCaptureDuration2s
			// 
			PresetCaptureDuration2s.Location = new System.Drawing.Point(316, 22);
			PresetCaptureDuration2s.Margin = new Padding(4, 3, 4, 3);
			PresetCaptureDuration2s.Name = "PresetCaptureDuration2s";
			PresetCaptureDuration2s.Size = new System.Drawing.Size(30, 27);
			PresetCaptureDuration2s.TabIndex = 60;
			PresetCaptureDuration2s.Text = "2s";
			PresetCaptureDuration2s.UseVisualStyleBackColor = true;
			PresetCaptureDuration2s.Click += PresetCaptureDuration2s_Click;
			// 
			// PresetCaptureDuration1s
			// 
			PresetCaptureDuration1s.Location = new System.Drawing.Point(281, 22);
			PresetCaptureDuration1s.Margin = new Padding(4, 3, 4, 3);
			PresetCaptureDuration1s.Name = "PresetCaptureDuration1s";
			PresetCaptureDuration1s.Size = new System.Drawing.Size(30, 27);
			PresetCaptureDuration1s.TabIndex = 59;
			PresetCaptureDuration1s.Text = "1s";
			PresetCaptureDuration1s.UseVisualStyleBackColor = true;
			PresetCaptureDuration1s.Click += PresetCaptureDuration1s_Click;
			// 
			// PresetCaptureDuration5s
			// 
			PresetCaptureDuration5s.Location = new System.Drawing.Point(350, 22);
			PresetCaptureDuration5s.Margin = new Padding(4, 3, 4, 3);
			PresetCaptureDuration5s.Name = "PresetCaptureDuration5s";
			PresetCaptureDuration5s.Size = new System.Drawing.Size(30, 27);
			PresetCaptureDuration5s.TabIndex = 55;
			PresetCaptureDuration5s.Text = "5s";
			PresetCaptureDuration5s.UseVisualStyleBackColor = true;
			PresetCaptureDuration5s.Click += PresetCaptureDuration5s_Click;
			// 
			// PresetTrack78_164
			// 
			PresetTrack78_164.Location = new System.Drawing.Point(214, 22);
			PresetTrack78_164.Margin = new Padding(4, 3, 4, 3);
			PresetTrack78_164.Name = "PresetTrack78_164";
			PresetTrack78_164.Size = new System.Drawing.Size(65, 27);
			PresetTrack78_164.TabIndex = 56;
			PresetTrack78_164.Text = "78-164";
			PresetTrack78_164.Click += PresetTrack78_164_Click;
			// 
			// PresetTrack80_90
			// 
			PresetTrack80_90.Location = new System.Drawing.Point(145, 22);
			PresetTrack80_90.Margin = new Padding(4, 3, 4, 3);
			PresetTrack80_90.Name = "PresetTrack80_90";
			PresetTrack80_90.Size = new System.Drawing.Size(66, 27);
			PresetTrack80_90.TabIndex = 52;
			PresetTrack80_90.Text = "80-90";
			PresetTrack80_90.UseVisualStyleBackColor = true;
			PresetTrack80_90.Click += PresetTrack80_90_Click;
			// 
			// PresetCaptureDefaultBtn
			// 
			PresetCaptureDefaultBtn.Location = new System.Drawing.Point(8, 22);
			PresetCaptureDefaultBtn.Margin = new Padding(4, 3, 4, 3);
			PresetCaptureDefaultBtn.Name = "PresetCaptureDefaultBtn";
			PresetCaptureDefaultBtn.Size = new System.Drawing.Size(65, 27);
			PresetCaptureDefaultBtn.TabIndex = 57;
			PresetCaptureDefaultBtn.Text = "Default";
			PresetCaptureDefaultBtn.Click += PresetCaptureDefaultBtn_Click;
			// 
			// button37
			// 
			button37.Location = new System.Drawing.Point(76, 22);
			button37.Margin = new Padding(4, 3, 4, 3);
			button37.Name = "button37";
			button37.Size = new System.Drawing.Size(66, 27);
			button37.TabIndex = 58;
			button37.Text = "0-10";
			button37.Click += PresetTrack0_10_Click;
			// 
			// QTrackDurationUpDown
			// 
			QTrackDurationUpDown.DataBindings.Add(new Binding("Value", settings1, "TrackDuration", true, DataSourceUpdateMode.OnPropertyChanged));
			QTrackDurationUpDown.Increment = new decimal(new int[] { 100, 0, 0, 0 });
			QTrackDurationUpDown.Location = new System.Drawing.Point(377, 40);
			QTrackDurationUpDown.Margin = new Padding(4, 3, 4, 3);
			QTrackDurationUpDown.Maximum = new decimal(new int[] { 2000000, 0, 0, 0 });
			QTrackDurationUpDown.Name = "QTrackDurationUpDown";
			QTrackDurationUpDown.Size = new System.Drawing.Size(61, 23);
			QTrackDurationUpDown.TabIndex = 50;
			toolTip1.SetToolTip(QTrackDurationUpDown, resources.GetString("QTrackDurationUpDown.ToolTip"));
			QTrackDurationUpDown.Value = new decimal(new int[] { 330, 0, 0, 0 });
			// 
			// QStartTrackUpDown
			// 
			QStartTrackUpDown.Location = new System.Drawing.Point(233, 40);
			QStartTrackUpDown.Margin = new Padding(4, 3, 4, 3);
			QStartTrackUpDown.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
			QStartTrackUpDown.Name = "QStartTrackUpDown";
			QStartTrackUpDown.Size = new System.Drawing.Size(61, 23);
			QStartTrackUpDown.TabIndex = 0;
			toolTip1.SetToolTip(QStartTrackUpDown, "Start track to capture.");
			// 
			// QEndTracksUpDown
			// 
			QEndTracksUpDown.Location = new System.Drawing.Point(306, 40);
			QEndTracksUpDown.Margin = new Padding(4, 3, 4, 3);
			QEndTracksUpDown.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
			QEndTracksUpDown.Name = "QEndTracksUpDown";
			QEndTracksUpDown.Size = new System.Drawing.Size(61, 23);
			QEndTracksUpDown.TabIndex = 49;
			toolTip1.SetToolTip(QEndTracksUpDown, "End track to capture.");
			QEndTracksUpDown.Value = new decimal(new int[] { 162, 0, 0, 0 });
			// 
			// label82
			// 
			label82.AutoSize = true;
			label82.Location = new System.Drawing.Point(374, 22);
			label82.Margin = new Padding(4, 0, 4, 0);
			label82.Name = "label82";
			label82.Size = new System.Drawing.Size(102, 15);
			label82.TabIndex = 53;
			label82.Text = "Track Duration ms";
			// 
			// QTRK00OffsetUpDown
			// 
			QTRK00OffsetUpDown.DataBindings.Add(new Binding("Value", settings1, "TRK00Offset", true, DataSourceUpdateMode.OnPropertyChanged));
			QTRK00OffsetUpDown.Location = new System.Drawing.Point(119, 39);
			QTRK00OffsetUpDown.Margin = new Padding(4, 3, 4, 3);
			QTRK00OffsetUpDown.Minimum = new decimal(new int[] { 100, 0, 0, int.MinValue });
			QTRK00OffsetUpDown.Name = "QTRK00OffsetUpDown";
			QTRK00OffsetUpDown.Size = new System.Drawing.Size(61, 23);
			QTRK00OffsetUpDown.TabIndex = 47;
			toolTip1.SetToolTip(QTRK00OffsetUpDown, "Track offset. When seeking TRK00 you can move farther back if you're having trouble getting track 0 to capture well. Can be negative.");
			// 
			// QMicrostepsPerTrackUpDown
			// 
			QMicrostepsPerTrackUpDown.DataBindings.Add(new Binding("Value", settings1, "MicroStepsPerTrack", true, DataSourceUpdateMode.OnPropertyChanged));
			QMicrostepsPerTrackUpDown.Location = new System.Drawing.Point(9, 39);
			QMicrostepsPerTrackUpDown.Margin = new Padding(4, 3, 4, 3);
			QMicrostepsPerTrackUpDown.Name = "QMicrostepsPerTrackUpDown";
			QMicrostepsPerTrackUpDown.Size = new System.Drawing.Size(61, 23);
			QMicrostepsPerTrackUpDown.TabIndex = 46;
			toolTip1.SetToolTip(QMicrostepsPerTrackUpDown, "If you use a step stick, anything under 8 microsteps will read between the tracks. For a glued disk, use 2 microsteps for a full recovery.");
			QMicrostepsPerTrackUpDown.Value = new decimal(new int[] { 8, 0, 0, 0 });
			// 
			// label83
			// 
			label83.AutoSize = true;
			label83.Location = new System.Drawing.Point(117, 22);
			label83.Margin = new Padding(4, 0, 4, 0);
			label83.Name = "label83";
			label83.Size = new System.Drawing.Size(72, 15);
			label83.TabIndex = 54;
			label83.Text = "TRK00 offset";
			// 
			// label84
			// 
			label84.AutoSize = true;
			label84.Location = new System.Drawing.Point(7, 20);
			label84.Margin = new Padding(4, 0, 4, 0);
			label84.Name = "label84";
			label84.Size = new System.Drawing.Size(95, 15);
			label84.TabIndex = 55;
			label84.Text = "Track microsteps";
			// 
			// label85
			// 
			label85.AutoSize = true;
			label85.Location = new System.Drawing.Point(229, 22);
			label85.Margin = new Padding(4, 0, 4, 0);
			label85.Name = "label85";
			label85.Size = new System.Drawing.Size(60, 15);
			label85.TabIndex = 56;
			label85.Text = "Start track";
			// 
			// label86
			// 
			label86.AutoSize = true;
			label86.Location = new System.Drawing.Point(302, 22);
			label86.Margin = new Padding(4, 0, 4, 0);
			label86.Name = "label86";
			label86.Size = new System.Drawing.Size(56, 15);
			label86.TabIndex = 57;
			label86.Text = "End track";
			// 
			// CaptureTab
			// 
			CaptureTab.BackColor = System.Drawing.SystemColors.Control;
			CaptureTab.Controls.Add(groupBox4);
			CaptureTab.Controls.Add(button8);
			CaptureTab.Controls.Add(button7);
			CaptureTab.Controls.Add(SaveTrimmedBadbutton);
			CaptureTab.Controls.Add(button49);
			CaptureTab.Controls.Add(button48);
			CaptureTab.Controls.Add(button46);
			CaptureTab.Controls.Add(button45);
			CaptureTab.Controls.Add(button40);
			CaptureTab.Controls.Add(button39);
			CaptureTab.Controls.Add(button36);
			CaptureTab.Controls.Add(CaptureClassbutton);
			CaptureTab.Controls.Add(groupBox7);
			CaptureTab.Controls.Add(DirectStepCheckBox);
			CaptureTab.ImageIndex = 0;
			CaptureTab.Location = new System.Drawing.Point(4, 24);
			CaptureTab.Margin = new Padding(4, 3, 4, 3);
			CaptureTab.Name = "CaptureTab";
			CaptureTab.Padding = new Padding(4, 3, 4, 3);
			CaptureTab.Size = new System.Drawing.Size(1128, 897);
			CaptureTab.TabIndex = 0;
			CaptureTab.Text = "Capture";
			// 
			// groupBox4
			// 
			groupBox4.Controls.Add(rxbufEndUpDown);
			groupBox4.Controls.Add(rxbufStartUpDown);
			groupBox4.Controls.Add(BufferSizeLabel);
			groupBox4.Controls.Add(HistogramLengthLabel);
			groupBox4.Controls.Add(HistogramStartLabel);
			groupBox4.Controls.Add(label29);
			groupBox4.Controls.Add(label28);
			groupBox4.Controls.Add(label27);
			groupBox4.Controls.Add(label32);
			groupBox4.Controls.Add(label31);
			groupBox4.Location = new System.Drawing.Point(744, 705);
			groupBox4.Margin = new Padding(4, 3, 4, 3);
			groupBox4.Name = "groupBox4";
			groupBox4.Padding = new Padding(4, 3, 4, 3);
			groupBox4.Size = new System.Drawing.Size(343, 90);
			groupBox4.TabIndex = 102;
			groupBox4.TabStop = false;
			groupBox4.Text = "Input buffer";
			groupBox4.Visible = false;
			// 
			// rxbufEndUpDown
			// 
			rxbufEndUpDown.Location = new System.Drawing.Point(76, 59);
			rxbufEndUpDown.Margin = new Padding(4, 3, 4, 3);
			rxbufEndUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			rxbufEndUpDown.Name = "rxbufEndUpDown";
			rxbufEndUpDown.Size = new System.Drawing.Size(90, 23);
			rxbufEndUpDown.TabIndex = 40;
			// 
			// rxbufStartUpDown
			// 
			rxbufStartUpDown.Location = new System.Drawing.Point(76, 29);
			rxbufStartUpDown.Margin = new Padding(4, 3, 4, 3);
			rxbufStartUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			rxbufStartUpDown.Name = "rxbufStartUpDown";
			rxbufStartUpDown.Size = new System.Drawing.Size(90, 23);
			rxbufStartUpDown.TabIndex = 39;
			// 
			// BufferSizeLabel
			// 
			BufferSizeLabel.AutoSize = true;
			BufferSizeLabel.Location = new System.Drawing.Point(208, 67);
			BufferSizeLabel.Margin = new Padding(4, 0, 4, 0);
			BufferSizeLabel.Name = "BufferSizeLabel";
			BufferSizeLabel.Size = new System.Drawing.Size(13, 15);
			BufferSizeLabel.TabIndex = 31;
			BufferSizeLabel.Text = "0";
			// 
			// HistogramLengthLabel
			// 
			HistogramLengthLabel.AutoSize = true;
			HistogramLengthLabel.Location = new System.Drawing.Point(208, 45);
			HistogramLengthLabel.Margin = new Padding(4, 0, 4, 0);
			HistogramLengthLabel.Name = "HistogramLengthLabel";
			HistogramLengthLabel.Size = new System.Drawing.Size(13, 15);
			HistogramLengthLabel.TabIndex = 32;
			HistogramLengthLabel.Text = "0";
			// 
			// HistogramStartLabel
			// 
			HistogramStartLabel.AutoSize = true;
			HistogramStartLabel.Location = new System.Drawing.Point(208, 21);
			HistogramStartLabel.Margin = new Padding(4, 0, 4, 0);
			HistogramStartLabel.Name = "HistogramStartLabel";
			HistogramStartLabel.Size = new System.Drawing.Size(13, 15);
			HistogramStartLabel.TabIndex = 33;
			HistogramStartLabel.Text = "0";
			// 
			// label29
			// 
			label29.AutoSize = true;
			label29.Location = new System.Drawing.Point(178, 67);
			label29.Margin = new Padding(4, 0, 4, 0);
			label29.Name = "label29";
			label29.Size = new System.Drawing.Size(30, 15);
			label29.TabIndex = 34;
			label29.Text = "Size:";
			label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label28
			// 
			label28.AutoSize = true;
			label28.Location = new System.Drawing.Point(180, 45);
			label28.Margin = new Padding(4, 0, 4, 0);
			label28.Name = "label28";
			label28.Size = new System.Drawing.Size(30, 15);
			label28.TabIndex = 35;
			label28.Text = "End:";
			label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label27
			// 
			label27.AutoSize = true;
			label27.Location = new System.Drawing.Point(176, 21);
			label27.Margin = new Padding(4, 0, 4, 0);
			label27.Name = "label27";
			label27.Size = new System.Drawing.Size(34, 15);
			label27.TabIndex = 36;
			label27.Text = "Start:";
			label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label32
			// 
			label32.AutoSize = true;
			label32.Location = new System.Drawing.Point(9, 63);
			label32.Margin = new Padding(4, 0, 4, 0);
			label32.Name = "label32";
			label32.Size = new System.Drawing.Size(58, 15);
			label32.TabIndex = 37;
			label32.Text = "rxbuf end";
			// 
			// label31
			// 
			label31.AutoSize = true;
			label31.Location = new System.Drawing.Point(7, 30);
			label31.Margin = new Padding(4, 0, 4, 0);
			label31.Name = "label31";
			label31.Size = new System.Drawing.Size(61, 15);
			label31.TabIndex = 38;
			label31.Text = "rxbuf start";
			// 
			// button8
			// 
			button8.Location = new System.Drawing.Point(8, 153);
			button8.Margin = new Padding(4, 3, 4, 3);
			button8.Name = "button8";
			button8.Size = new System.Drawing.Size(114, 27);
			button8.TabIndex = 101;
			button8.Text = "StepStick preset";
			button8.Click += StepStickPresetBtn_Click;
			// 
			// button7
			// 
			button7.Location = new System.Drawing.Point(8, 120);
			button7.Margin = new Padding(4, 3, 4, 3);
			button7.Name = "button7";
			button7.Size = new System.Drawing.Size(114, 27);
			button7.TabIndex = 100;
			button7.Text = "Direct preset";
			button7.Click += DirectPresetBtn_Click;
			// 
			// SaveTrimmedBadbutton
			// 
			SaveTrimmedBadbutton.Location = new System.Drawing.Point(120, 502);
			SaveTrimmedBadbutton.Margin = new Padding(4, 3, 4, 3);
			SaveTrimmedBadbutton.Name = "SaveTrimmedBadbutton";
			SaveTrimmedBadbutton.Size = new System.Drawing.Size(106, 46);
			SaveTrimmedBadbutton.TabIndex = 99;
			SaveTrimmedBadbutton.Text = "Save only bad sector data";
			SaveTrimmedBadbutton.UseVisualStyleBackColor = true;
			SaveTrimmedBadbutton.Click += SaveTrimmedBadbutton_Click;
			// 
			// button49
			// 
			button49.Location = new System.Drawing.Point(7, 502);
			button49.Margin = new Padding(4, 3, 4, 3);
			button49.Name = "button49";
			button49.Size = new System.Drawing.Size(106, 46);
			button49.TabIndex = 98;
			button49.Text = "Save trimmed bin file";
			button49.UseVisualStyleBackColor = true;
			button49.Click += Button49_Click;
			// 
			// button48
			// 
			button48.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			button48.ImageIndex = 0;
			button48.ImageList = MainTabControlImageList;
			button48.Location = new System.Drawing.Point(7, 63);
			button48.Margin = new Padding(4, 3, 4, 3);
			button48.Name = "button48";
			button48.Size = new System.Drawing.Size(106, 46);
			button48.TabIndex = 97;
			button48.Text = "Recapture all";
			button48.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			button48.UseVisualStyleBackColor = true;
			button48.Click += Button48_Click;
			// 
			// button46
			// 
			button46.Location = new System.Drawing.Point(94, 410);
			button46.Margin = new Padding(4, 3, 4, 3);
			button46.Name = "button46";
			button46.Size = new System.Drawing.Size(80, 46);
			button46.TabIndex = 96;
			button46.Text = "Save SCP";
			button46.UseVisualStyleBackColor = true;
			button46.Click += SaveSCP_Click;
			// 
			// button45
			// 
			button45.Location = new System.Drawing.Point(7, 410);
			button45.Margin = new Padding(4, 3, 4, 3);
			button45.Name = "button45";
			button45.Size = new System.Drawing.Size(80, 46);
			button45.TabIndex = 95;
			button45.Text = "Open SCP";
			button45.UseVisualStyleBackColor = true;
			button45.Click += OpenSCP_Click;
			// 
			// button40
			// 
			button40.Location = new System.Drawing.Point(189, 330);
			button40.Margin = new Padding(4, 3, 4, 3);
			button40.Name = "button40";
			button40.Size = new System.Drawing.Size(84, 46);
			button40.TabIndex = 94;
			button40.Text = "Step >";
			button40.UseVisualStyleBackColor = true;
			button40.Click += StepForwardBtn_Click;
			// 
			// button39
			// 
			button39.Location = new System.Drawing.Point(98, 330);
			button39.Margin = new Padding(4, 3, 4, 3);
			button39.Name = "button39";
			button39.Size = new System.Drawing.Size(84, 46);
			button39.TabIndex = 93;
			button39.Text = "Step <";
			button39.UseVisualStyleBackColor = true;
			button39.Click += StepBackBtn_Click;
			// 
			// button36
			// 
			button36.Location = new System.Drawing.Point(7, 330);
			button36.Margin = new Padding(4, 3, 4, 3);
			button36.Name = "button36";
			button36.Size = new System.Drawing.Size(84, 46);
			button36.TabIndex = 92;
			button36.Text = "Microstep 8";
			button36.UseVisualStyleBackColor = true;
			button36.Click += Microstep8Btn_Click;
			// 
			// CaptureClassbutton
			// 
			CaptureClassbutton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			CaptureClassbutton.ImageIndex = 0;
			CaptureClassbutton.ImageList = MainTabControlImageList;
			CaptureClassbutton.Location = new System.Drawing.Point(7, 10);
			CaptureClassbutton.Margin = new Padding(4, 3, 4, 3);
			CaptureClassbutton.Name = "CaptureClassbutton";
			CaptureClassbutton.Size = new System.Drawing.Size(80, 46);
			CaptureClassbutton.TabIndex = 52;
			CaptureClassbutton.Text = "Capture";
			CaptureClassbutton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			CaptureClassbutton.UseVisualStyleBackColor = true;
			CaptureClassbutton.Click += CaptureClassbutton_Click;
			// 
			// groupBox7
			// 
			groupBox7.Controls.Add(groupBox1);
			groupBox7.Controls.Add(TrackDurationUpDown);
			groupBox7.Controls.Add(StartTrackUpDown);
			groupBox7.Controls.Add(EndTracksUpDown);
			groupBox7.Controls.Add(label22);
			groupBox7.Controls.Add(TRK00OffsetUpDown);
			groupBox7.Controls.Add(MicrostepsPerTrackUpDown);
			groupBox7.Controls.Add(label38);
			groupBox7.Controls.Add(label36);
			groupBox7.Controls.Add(label21);
			groupBox7.Controls.Add(label20);
			groupBox7.Location = new System.Drawing.Point(160, 7);
			groupBox7.Margin = new Padding(4, 3, 4, 3);
			groupBox7.Name = "groupBox7";
			groupBox7.Padding = new Padding(4, 3, 4, 3);
			groupBox7.Size = new System.Drawing.Size(292, 312);
			groupBox7.TabIndex = 50;
			groupBox7.TabStop = false;
			groupBox7.Text = "Capture options";
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(button4);
			groupBox1.Controls.Add(button6);
			groupBox1.Controls.Add(TrackPreset4Button);
			groupBox1.Controls.Add(TrackPreset2Button);
			groupBox1.Controls.Add(TrackPreset3Button);
			groupBox1.Controls.Add(TrackPreset1Button);
			groupBox1.Location = new System.Drawing.Point(147, 20);
			groupBox1.Margin = new Padding(4, 3, 4, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(4, 3, 4, 3);
			groupBox1.Size = new System.Drawing.Size(106, 276);
			groupBox1.TabIndex = 58;
			groupBox1.TabStop = false;
			groupBox1.Text = "Preset capture";
			// 
			// button4
			// 
			button4.Location = new System.Drawing.Point(12, 134);
			button4.Margin = new Padding(4, 3, 4, 3);
			button4.Name = "button4";
			button4.Size = new System.Drawing.Size(66, 27);
			button4.TabIndex = 59;
			button4.Text = "1000ms";
			button4.UseVisualStyleBackColor = true;
			button4.Click += PresetCaptureDuration1s_Click;
			// 
			// button6
			// 
			button6.Location = new System.Drawing.Point(12, 160);
			button6.Margin = new Padding(4, 3, 4, 3);
			button6.Name = "button6";
			button6.Size = new System.Drawing.Size(66, 27);
			button6.TabIndex = 55;
			button6.Text = "5000ms";
			button6.UseVisualStyleBackColor = true;
			button6.Click += PresetCaptureDuration5s_Click;
			// 
			// TrackPreset4Button
			// 
			TrackPreset4Button.Location = new System.Drawing.Point(12, 106);
			TrackPreset4Button.Margin = new Padding(4, 3, 4, 3);
			TrackPreset4Button.Name = "TrackPreset4Button";
			TrackPreset4Button.Size = new System.Drawing.Size(65, 27);
			TrackPreset4Button.TabIndex = 56;
			TrackPreset4Button.Text = "78-164";
			TrackPreset4Button.Click += PresetTrack78_164_Click;
			// 
			// TrackPreset2Button
			// 
			TrackPreset2Button.Location = new System.Drawing.Point(10, 78);
			TrackPreset2Button.Margin = new Padding(4, 3, 4, 3);
			TrackPreset2Button.Name = "TrackPreset2Button";
			TrackPreset2Button.Size = new System.Drawing.Size(66, 27);
			TrackPreset2Button.TabIndex = 52;
			TrackPreset2Button.Text = "80-90";
			TrackPreset2Button.UseVisualStyleBackColor = true;
			TrackPreset2Button.Click += PresetTrack80_90_Click;
			// 
			// TrackPreset3Button
			// 
			TrackPreset3Button.Location = new System.Drawing.Point(12, 22);
			TrackPreset3Button.Margin = new Padding(4, 3, 4, 3);
			TrackPreset3Button.Name = "TrackPreset3Button";
			TrackPreset3Button.Size = new System.Drawing.Size(65, 27);
			TrackPreset3Button.TabIndex = 57;
			TrackPreset3Button.Text = "Default";
			TrackPreset3Button.Click += PresetCaptureDefaultBtn_Click;
			// 
			// TrackPreset1Button
			// 
			TrackPreset1Button.Location = new System.Drawing.Point(10, 51);
			TrackPreset1Button.Margin = new Padding(4, 3, 4, 3);
			TrackPreset1Button.Name = "TrackPreset1Button";
			TrackPreset1Button.Size = new System.Drawing.Size(66, 27);
			TrackPreset1Button.TabIndex = 58;
			TrackPreset1Button.Text = "0-10";
			TrackPreset1Button.Click += PresetTrack0_10_Click;
			// 
			// TrackDurationUpDown
			// 
			TrackDurationUpDown.DataBindings.Add(new Binding("Value", settings1, "TrackDuration", true, DataSourceUpdateMode.OnPropertyChanged));
			TrackDurationUpDown.Increment = new decimal(new int[] { 100, 0, 0, 0 });
			TrackDurationUpDown.Location = new System.Drawing.Point(12, 239);
			TrackDurationUpDown.Margin = new Padding(4, 3, 4, 3);
			TrackDurationUpDown.Maximum = new decimal(new int[] { 2000000, 0, 0, 0 });
			TrackDurationUpDown.Name = "TrackDurationUpDown";
			TrackDurationUpDown.Size = new System.Drawing.Size(61, 23);
			TrackDurationUpDown.TabIndex = 50;
			TrackDurationUpDown.Value = new decimal(new int[] { 330, 0, 0, 0 });
			// 
			// StartTrackUpDown
			// 
			StartTrackUpDown.Location = new System.Drawing.Point(12, 141);
			StartTrackUpDown.Margin = new Padding(4, 3, 4, 3);
			StartTrackUpDown.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
			StartTrackUpDown.Name = "StartTrackUpDown";
			StartTrackUpDown.Size = new System.Drawing.Size(61, 23);
			StartTrackUpDown.TabIndex = 48;
			StartTrackUpDown.Value = new decimal(new int[] { 10, 0, 0, 0 });
			// 
			// EndTracksUpDown
			// 
			EndTracksUpDown.Location = new System.Drawing.Point(12, 190);
			EndTracksUpDown.Margin = new Padding(4, 3, 4, 3);
			EndTracksUpDown.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
			EndTracksUpDown.Name = "EndTracksUpDown";
			EndTracksUpDown.Size = new System.Drawing.Size(61, 23);
			EndTracksUpDown.TabIndex = 49;
			EndTracksUpDown.Value = new decimal(new int[] { 19, 0, 0, 0 });
			// 
			// label22
			// 
			label22.AutoSize = true;
			label22.Location = new System.Drawing.Point(9, 220);
			label22.Margin = new Padding(4, 0, 4, 0);
			label22.Name = "label22";
			label22.Size = new System.Drawing.Size(102, 15);
			label22.TabIndex = 53;
			label22.Text = "Track Duration ms";
			// 
			// TRK00OffsetUpDown
			// 
			TRK00OffsetUpDown.Location = new System.Drawing.Point(12, 89);
			TRK00OffsetUpDown.Margin = new Padding(4, 3, 4, 3);
			TRK00OffsetUpDown.Minimum = new decimal(new int[] { 100, 0, 0, int.MinValue });
			TRK00OffsetUpDown.Name = "TRK00OffsetUpDown";
			TRK00OffsetUpDown.Size = new System.Drawing.Size(61, 23);
			TRK00OffsetUpDown.TabIndex = 47;
			TRK00OffsetUpDown.Value = new decimal(new int[] { 1, 0, 0, int.MinValue });
			TRK00OffsetUpDown.ValueChanged += TRK00OffsetUpDown_ValueChanged;
			// 
			// MicrostepsPerTrackUpDown
			// 
			MicrostepsPerTrackUpDown.Location = new System.Drawing.Point(9, 39);
			MicrostepsPerTrackUpDown.Margin = new Padding(4, 3, 4, 3);
			MicrostepsPerTrackUpDown.Name = "MicrostepsPerTrackUpDown";
			MicrostepsPerTrackUpDown.Size = new System.Drawing.Size(61, 23);
			MicrostepsPerTrackUpDown.TabIndex = 46;
			MicrostepsPerTrackUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			MicrostepsPerTrackUpDown.ValueChanged += MicrostepsPerTrackUpDown_ValueChanged;
			// 
			// label38
			// 
			label38.AutoSize = true;
			label38.Location = new System.Drawing.Point(9, 72);
			label38.Margin = new Padding(4, 0, 4, 0);
			label38.Name = "label38";
			label38.Size = new System.Drawing.Size(72, 15);
			label38.TabIndex = 54;
			label38.Text = "TRK00 offset";
			// 
			// label36
			// 
			label36.AutoSize = true;
			label36.Location = new System.Drawing.Point(7, 20);
			label36.Margin = new Padding(4, 0, 4, 0);
			label36.Name = "label36";
			label36.Size = new System.Drawing.Size(114, 15);
			label36.TabIndex = 55;
			label36.Text = "microsteps per track";
			// 
			// label21
			// 
			label21.AutoSize = true;
			label21.Location = new System.Drawing.Point(12, 122);
			label21.Margin = new Padding(4, 0, 4, 0);
			label21.Name = "label21";
			label21.Size = new System.Drawing.Size(60, 15);
			label21.TabIndex = 56;
			label21.Text = "Start track";
			// 
			// label20
			// 
			label20.AutoSize = true;
			label20.Location = new System.Drawing.Point(12, 172);
			label20.Margin = new Padding(4, 0, 4, 0);
			label20.Name = "label20";
			label20.Size = new System.Drawing.Size(56, 15);
			label20.TabIndex = 57;
			label20.Text = "End track";
			// 
			// DirectStepCheckBox
			// 
			DirectStepCheckBox.AutoSize = true;
			DirectStepCheckBox.Checked = true;
			DirectStepCheckBox.CheckState = CheckState.Checked;
			DirectStepCheckBox.DataBindings.Add(new Binding("Checked", settings1, "DirectStep", true, DataSourceUpdateMode.OnPropertyChanged));
			DirectStepCheckBox.Location = new System.Drawing.Point(10, 264);
			DirectStepCheckBox.Margin = new Padding(4, 3, 4, 3);
			DirectStepCheckBox.Name = "DirectStepCheckBox";
			DirectStepCheckBox.Size = new System.Drawing.Size(80, 19);
			DirectStepCheckBox.TabIndex = 91;
			DirectStepCheckBox.Text = "DirectStep";
			DirectStepCheckBox.UseVisualStyleBackColor = true;
			DirectStepCheckBox.CheckedChanged += DirectStepCheckBox_CheckedChanged;
			// 
			// ProcessingTab
			// 
			ProcessingTab.BackColor = System.Drawing.SystemColors.Control;
			ProcessingTab.Controls.Add(button5);
			ProcessingTab.Controls.Add(ResetBuffersBtn);
			ProcessingTab.Controls.Add(DupsUpDown);
			ProcessingTab.Controls.Add(label69);
			ProcessingTab.Controls.Add(AddNoiseKnumericUpDown);
			ProcessingTab.Controls.Add(label68);
			ProcessingTab.Controls.Add(rtbSectorMap);
			ProcessingTab.Controls.Add(groupBox6);
			ProcessingTab.Controls.Add(ProcessBtn);
			ProcessingTab.Controls.Add(ProcessPCBtn);
			ProcessingTab.ImageIndex = 3;
			ProcessingTab.Location = new System.Drawing.Point(4, 24);
			ProcessingTab.Margin = new Padding(4, 3, 4, 3);
			ProcessingTab.Name = "ProcessingTab";
			ProcessingTab.Padding = new Padding(4, 3, 4, 3);
			ProcessingTab.Size = new System.Drawing.Size(1128, 897);
			ProcessingTab.TabIndex = 1;
			ProcessingTab.Text = "Processing";
			// 
			// button5
			// 
			button5.Location = new System.Drawing.Point(8, 167);
			button5.Margin = new Padding(4, 3, 4, 3);
			button5.Name = "button5";
			button5.Size = new System.Drawing.Size(78, 44);
			button5.TabIndex = 117;
			button5.Text = "Reset output";
			button5.UseVisualStyleBackColor = true;
			button5.Click += Button5_Click;
			// 
			// ResetBuffersBtn
			// 
			ResetBuffersBtn.Location = new System.Drawing.Point(8, 114);
			ResetBuffersBtn.Margin = new Padding(4, 3, 4, 3);
			ResetBuffersBtn.Name = "ResetBuffersBtn";
			ResetBuffersBtn.Size = new System.Drawing.Size(74, 45);
			ResetBuffersBtn.TabIndex = 116;
			ResetBuffersBtn.Text = "Reset input";
			ResetBuffersBtn.UseVisualStyleBackColor = true;
			ResetBuffersBtn.Click += ResetBuffersBtn_Click;
			// 
			// DupsUpDown
			// 
			DupsUpDown.Location = new System.Drawing.Point(49, 307);
			DupsUpDown.Margin = new Padding(4, 3, 4, 3);
			DupsUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			DupsUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			DupsUpDown.Name = "DupsUpDown";
			DupsUpDown.Size = new System.Drawing.Size(46, 23);
			DupsUpDown.TabIndex = 94;
			DupsUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			// 
			// label69
			// 
			label69.AutoSize = true;
			label69.Location = new System.Drawing.Point(7, 309);
			label69.Margin = new Padding(4, 0, 4, 0);
			label69.Name = "label69";
			label69.Size = new System.Drawing.Size(34, 15);
			label69.TabIndex = 93;
			label69.Text = "Dups";
			// 
			// AddNoiseKnumericUpDown
			// 
			AddNoiseKnumericUpDown.Location = new System.Drawing.Point(48, 277);
			AddNoiseKnumericUpDown.Margin = new Padding(4, 3, 4, 3);
			AddNoiseKnumericUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			AddNoiseKnumericUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			AddNoiseKnumericUpDown.Name = "AddNoiseKnumericUpDown";
			AddNoiseKnumericUpDown.Size = new System.Drawing.Size(46, 23);
			AddNoiseKnumericUpDown.TabIndex = 92;
			AddNoiseKnumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			// 
			// label68
			// 
			label68.AutoSize = true;
			label68.Location = new System.Drawing.Point(24, 279);
			label68.Margin = new Padding(4, 0, 4, 0);
			label68.Name = "label68";
			label68.Size = new System.Drawing.Size(16, 15);
			label68.TabIndex = 91;
			label68.Text = "k:";
			// 
			// rtbSectorMap
			// 
			rtbSectorMap.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			rtbSectorMap.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			rtbSectorMap.HideSelection = false;
			rtbSectorMap.Location = new System.Drawing.Point(0, 408);
			rtbSectorMap.Margin = new Padding(4, 3, 4, 3);
			rtbSectorMap.Name = "rtbSectorMap";
			rtbSectorMap.Size = new System.Drawing.Size(1123, 485);
			rtbSectorMap.TabIndex = 84;
			rtbSectorMap.Text = "";
			rtbSectorMap.DoubleClick += RtbSectorMap_DoubleClick;
			rtbSectorMap.MouseDown += RtbSectorMap_MouseDown;
			// 
			// groupBox6
			// 
			groupBox6.Controls.Add(SkipAlreadyCrcOkcheckBox1);
			groupBox6.Controls.Add(jESEnd);
			groupBox6.Controls.Add(jESStart);
			groupBox6.Controls.Add(label17);
			groupBox6.Controls.Add(SettingsLabel);
			groupBox6.Controls.Add(label35);
			groupBox6.Controls.Add(iESEnd);
			groupBox6.Controls.Add(iESStart);
			groupBox6.Controls.Add(label8);
			groupBox6.Controls.Add(label9);
			groupBox6.Controls.Add(FullHistBtn);
			groupBox6.Controls.Add(OnlyBadSectorsRadio);
			groupBox6.Controls.Add(ECOnRadio);
			groupBox6.Controls.Add(label78);
			groupBox6.Controls.Add(ChangeDiskTypeComboBox);
			groupBox6.Controls.Add(ProcessingModeComboBox);
			groupBox6.Controls.Add(ClearDatacheckBox);
			groupBox6.Controls.Add(LimitTSCheckBox);
			groupBox6.Controls.Add(RateOfChange2UpDown);
			groupBox6.Controls.Add(AdaptOfsset2UpDown);
			groupBox6.Controls.Add(label74);
			groupBox6.Controls.Add(PeriodBeyond8uscomboBox);
			groupBox6.Controls.Add(LimitToTrackUpDown);
			groupBox6.Controls.Add(RndAmountUpDown);
			groupBox6.Controls.Add(LimitToSectorUpDown);
			groupBox6.Controls.Add(label67);
			groupBox6.Controls.Add(label42);
			groupBox6.Controls.Add(LimitToScttrViewcheckBox);
			groupBox6.Controls.Add(label41);
			groupBox6.Controls.Add(AddNoisecheckBox);
			groupBox6.Controls.Add(ThresholdsGroupBox);
			groupBox6.Controls.Add(FindDupesCheckBox);
			groupBox6.Controls.Add(AutoRefreshSectorMapCheck);
			groupBox6.Controls.Add(label50);
			groupBox6.Controls.Add(RateOfChangeUpDown);
			groupBox6.Controls.Add(IgnoreHeaderErrorCheckBox);
			groupBox6.Controls.Add(groupBox5);
			groupBox6.Controls.Add(groupBox3);
			groupBox6.Controls.Add(HDCheckBox);
			groupBox6.Controls.Add(label37);
			groupBox6.Controls.Add(label2);
			groupBox6.Location = new System.Drawing.Point(111, 7);
			groupBox6.Margin = new Padding(4, 3, 4, 3);
			groupBox6.Name = "groupBox6";
			groupBox6.Padding = new Padding(4, 3, 4, 3);
			groupBox6.Size = new System.Drawing.Size(1009, 395);
			groupBox6.TabIndex = 49;
			groupBox6.TabStop = false;
			groupBox6.Text = "Processing options";
			// 
			// SkipAlreadyCrcOkcheckBox1
			// 
			SkipAlreadyCrcOkcheckBox1.AutoSize = true;
			SkipAlreadyCrcOkcheckBox1.Checked = true;
			SkipAlreadyCrcOkcheckBox1.CheckState = CheckState.Checked;
			SkipAlreadyCrcOkcheckBox1.Location = new System.Drawing.Point(150, 129);
			SkipAlreadyCrcOkcheckBox1.Margin = new Padding(4, 3, 4, 3);
			SkipAlreadyCrcOkcheckBox1.Name = "SkipAlreadyCrcOkcheckBox1";
			SkipAlreadyCrcOkcheckBox1.Size = new System.Drawing.Size(125, 19);
			SkipAlreadyCrcOkcheckBox1.TabIndex = 116;
			SkipAlreadyCrcOkcheckBox1.Text = "Skip already CrcOk";
			SkipAlreadyCrcOkcheckBox1.UseVisualStyleBackColor = true;
			SkipAlreadyCrcOkcheckBox1.CheckedChanged += SkipAlreadyCrcOkcheckBox1_CheckedChanged;
			// 
			// jESEnd
			// 
			jESEnd.Location = new System.Drawing.Point(649, 354);
			jESEnd.Margin = new Padding(4, 3, 4, 3);
			jESEnd.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			jESEnd.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			jESEnd.Name = "jESEnd";
			jESEnd.Size = new System.Drawing.Size(46, 23);
			jESEnd.TabIndex = 112;
			jESEnd.Value = new decimal(new int[] { 16, 0, 0, 0 });
			// 
			// jESStart
			// 
			jESStart.Location = new System.Drawing.Point(649, 324);
			jESStart.Margin = new Padding(4, 3, 4, 3);
			jESStart.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			jESStart.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			jESStart.Name = "jESStart";
			jESStart.Size = new System.Drawing.Size(46, 23);
			jESStart.TabIndex = 111;
			// 
			// label17
			// 
			label17.AutoSize = true;
			label17.Location = new System.Drawing.Point(582, 359);
			label17.Margin = new Padding(4, 0, 4, 0);
			label17.Name = "label17";
			label17.Size = new System.Drawing.Size(48, 15);
			label17.TabIndex = 114;
			label17.Text = "j ES end";
			// 
			// SettingsLabel
			// 
			SettingsLabel.AutoSize = true;
			SettingsLabel.Location = new System.Drawing.Point(457, 303);
			SettingsLabel.Margin = new Padding(4, 0, 4, 0);
			SettingsLabel.Name = "SettingsLabel";
			SettingsLabel.Size = new System.Drawing.Size(55, 15);
			SettingsLabel.TabIndex = 113;
			SettingsLabel.Text = "Counters";
			// 
			// label35
			// 
			label35.AutoSize = true;
			label35.Location = new System.Drawing.Point(580, 325);
			label35.Margin = new Padding(4, 0, 4, 0);
			label35.Name = "label35";
			label35.Size = new System.Drawing.Size(52, 15);
			label35.TabIndex = 115;
			label35.Text = "j ES Start";
			// 
			// iESEnd
			// 
			iESEnd.Location = new System.Drawing.Point(524, 355);
			iESEnd.Margin = new Padding(4, 3, 4, 3);
			iESEnd.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			iESEnd.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			iESEnd.Name = "iESEnd";
			iESEnd.Size = new System.Drawing.Size(46, 23);
			iESEnd.TabIndex = 110;
			iESEnd.Value = new decimal(new int[] { 16, 0, 0, 0 });
			// 
			// iESStart
			// 
			iESStart.Location = new System.Drawing.Point(524, 325);
			iESStart.Margin = new Padding(4, 3, 4, 3);
			iESStart.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			iESStart.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			iESStart.Name = "iESStart";
			iESStart.Size = new System.Drawing.Size(46, 23);
			iESStart.TabIndex = 109;
			iESStart.Value = new decimal(new int[] { 2, 0, 0, 0 });
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(457, 360);
			label8.Margin = new Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(48, 15);
			label8.TabIndex = 107;
			label8.Text = "i ES end";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(455, 327);
			label9.Margin = new Padding(4, 0, 4, 0);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(51, 15);
			label9.TabIndex = 108;
			label9.Text = "i ES start";
			// 
			// FullHistBtn
			// 
			FullHistBtn.Location = new System.Drawing.Point(359, 217);
			FullHistBtn.Margin = new Padding(4, 3, 4, 3);
			FullHistBtn.Name = "FullHistBtn";
			FullHistBtn.Size = new System.Drawing.Size(99, 27);
			FullHistBtn.TabIndex = 105;
			FullHistBtn.Text = "Full histogram";
			FullHistBtn.UseVisualStyleBackColor = true;
			FullHistBtn.Click += FullHistBtn_Click;
			// 
			// OnlyBadSectorsRadio
			// 
			OnlyBadSectorsRadio.AutoSize = true;
			OnlyBadSectorsRadio.Location = new System.Drawing.Point(7, 106);
			OnlyBadSectorsRadio.Margin = new Padding(4, 3, 4, 3);
			OnlyBadSectorsRadio.Name = "OnlyBadSectorsRadio";
			OnlyBadSectorsRadio.Size = new System.Drawing.Size(113, 19);
			OnlyBadSectorsRadio.TabIndex = 106;
			OnlyBadSectorsRadio.Text = "Only bad sectors";
			OnlyBadSectorsRadio.UseVisualStyleBackColor = true;
			// 
			// ECOnRadio
			// 
			ECOnRadio.AutoSize = true;
			ECOnRadio.Checked = true;
			ECOnRadio.Location = new System.Drawing.Point(7, 133);
			ECOnRadio.Margin = new Padding(4, 3, 4, 3);
			ECOnRadio.Name = "ECOnRadio";
			ECOnRadio.Size = new System.Drawing.Size(129, 19);
			ECOnRadio.TabIndex = 105;
			ECOnRadio.TabStop = true;
			ECOnRadio.Text = "Use error correction";
			ECOnRadio.UseVisualStyleBackColor = true;
			// 
			// label78
			// 
			label78.AutoSize = true;
			label78.Location = new System.Drawing.Point(260, 23);
			label78.Margin = new Padding(4, 0, 4, 0);
			label78.Name = "label78";
			label78.Size = new System.Drawing.Size(70, 15);
			label78.TabIndex = 104;
			label78.Text = "Disk Format";
			// 
			// ChangeDiskTypeComboBox
			// 
			ChangeDiskTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			ChangeDiskTypeComboBox.FormattingEnabled = true;
			ChangeDiskTypeComboBox.Location = new System.Drawing.Point(264, 42);
			ChangeDiskTypeComboBox.Margin = new Padding(4, 3, 4, 3);
			ChangeDiskTypeComboBox.Name = "ChangeDiskTypeComboBox";
			ChangeDiskTypeComboBox.Size = new System.Drawing.Size(87, 23);
			ChangeDiskTypeComboBox.TabIndex = 102;
			ChangeDiskTypeComboBox.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
			// 
			// ProcessingModeComboBox
			// 
			ProcessingModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			ProcessingModeComboBox.FormattingEnabled = true;
			ProcessingModeComboBox.Location = new System.Drawing.Point(7, 40);
			ProcessingModeComboBox.Margin = new Padding(4, 3, 4, 3);
			ProcessingModeComboBox.Name = "ProcessingModeComboBox";
			ProcessingModeComboBox.Size = new System.Drawing.Size(123, 23);
			ProcessingModeComboBox.TabIndex = 103;
			// 
			// ClearDatacheckBox
			// 
			ClearDatacheckBox.AutoSize = true;
			ClearDatacheckBox.Location = new System.Drawing.Point(7, 80);
			ClearDatacheckBox.Margin = new Padding(4, 3, 4, 3);
			ClearDatacheckBox.Name = "ClearDatacheckBox";
			ClearDatacheckBox.Size = new System.Drawing.Size(114, 19);
			ClearDatacheckBox.TabIndex = 101;
			ClearDatacheckBox.Text = "Clear sector data";
			ClearDatacheckBox.UseVisualStyleBackColor = true;
			// 
			// LimitTSCheckBox
			// 
			LimitTSCheckBox.AutoSize = true;
			LimitTSCheckBox.Location = new System.Drawing.Point(476, 276);
			LimitTSCheckBox.Margin = new Padding(4, 3, 4, 3);
			LimitTSCheckBox.Name = "LimitTSCheckBox";
			LimitTSCheckBox.Size = new System.Drawing.Size(73, 19);
			LimitTSCheckBox.TabIndex = 90;
			LimitTSCheckBox.Text = "Limit T/S";
			LimitTSCheckBox.UseVisualStyleBackColor = true;
			// 
			// RateOfChange2UpDown
			// 
			RateOfChange2UpDown.Increment = new decimal(new int[] { 8, 0, 0, 0 });
			RateOfChange2UpDown.Location = new System.Drawing.Point(642, 245);
			RateOfChange2UpDown.Margin = new Padding(4, 3, 4, 3);
			RateOfChange2UpDown.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
			RateOfChange2UpDown.Name = "RateOfChange2UpDown";
			RateOfChange2UpDown.Size = new System.Drawing.Size(56, 23);
			RateOfChange2UpDown.TabIndex = 100;
			RateOfChange2UpDown.Value = new decimal(new int[] { 128, 0, 0, 0 });
			// 
			// AdaptOfsset2UpDown
			// 
			AdaptOfsset2UpDown.DecimalPlaces = 2;
			AdaptOfsset2UpDown.Location = new System.Drawing.Point(642, 272);
			AdaptOfsset2UpDown.Margin = new Padding(4, 3, 4, 3);
			AdaptOfsset2UpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			AdaptOfsset2UpDown.Minimum = new decimal(new int[] { 2000, 0, 0, int.MinValue });
			AdaptOfsset2UpDown.Name = "AdaptOfsset2UpDown";
			AdaptOfsset2UpDown.Size = new System.Drawing.Size(56, 23);
			AdaptOfsset2UpDown.TabIndex = 99;
			AdaptOfsset2UpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			// 
			// label74
			// 
			label74.AutoSize = true;
			label74.Location = new System.Drawing.Point(565, 247);
			label74.Margin = new Padding(4, 0, 4, 0);
			label74.Name = "label74";
			label74.Size = new System.Drawing.Size(68, 15);
			label74.TabIndex = 98;
			label74.Text = "Adapt track";
			// 
			// PeriodBeyond8uscomboBox
			// 
			PeriodBeyond8uscomboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			PeriodBeyond8uscomboBox.FormattingEnabled = true;
			PeriodBeyond8uscomboBox.Items.AddRange(new object[] { "None", "10", "100", "1000", "Random" });
			PeriodBeyond8uscomboBox.Location = new System.Drawing.Point(150, 40);
			PeriodBeyond8uscomboBox.Margin = new Padding(4, 3, 4, 3);
			PeriodBeyond8uscomboBox.Name = "PeriodBeyond8uscomboBox";
			PeriodBeyond8uscomboBox.Size = new System.Drawing.Size(87, 23);
			PeriodBeyond8uscomboBox.TabIndex = 96;
			// 
			// RndAmountUpDown
			// 
			RndAmountUpDown.Location = new System.Drawing.Point(89, 240);
			RndAmountUpDown.Margin = new Padding(4, 3, 4, 3);
			RndAmountUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			RndAmountUpDown.Name = "RndAmountUpDown";
			RndAmountUpDown.Size = new System.Drawing.Size(46, 23);
			RndAmountUpDown.TabIndex = 95;
			RndAmountUpDown.Value = new decimal(new int[] { 4, 0, 0, 0 });
			// 
			// label67
			// 
			label67.AutoSize = true;
			label67.Location = new System.Drawing.Point(12, 242);
			label67.Margin = new Padding(4, 0, 4, 0);
			label67.Name = "label67";
			label67.Size = new System.Drawing.Size(73, 15);
			label67.TabIndex = 94;
			label67.Text = "Rnd amount";
			// 
			// LimitToScttrViewcheckBox
			// 
			LimitToScttrViewcheckBox.AccessibleDescription = "";
			LimitToScttrViewcheckBox.AutoSize = true;
			LimitToScttrViewcheckBox.Location = new System.Drawing.Point(7, 186);
			LimitToScttrViewcheckBox.Margin = new Padding(4, 3, 4, 3);
			LimitToScttrViewcheckBox.Name = "LimitToScttrViewcheckBox";
			LimitToScttrViewcheckBox.Size = new System.Drawing.Size(120, 19);
			LimitToScttrViewcheckBox.TabIndex = 93;
			LimitToScttrViewcheckBox.Text = "Limit to scttr view";
			LimitToScttrViewcheckBox.UseVisualStyleBackColor = true;
			// 
			// AddNoisecheckBox
			// 
			AddNoisecheckBox.AccessibleDescription = "";
			AddNoisecheckBox.AutoSize = true;
			AddNoisecheckBox.Location = new System.Drawing.Point(7, 212);
			AddNoisecheckBox.Margin = new Padding(4, 3, 4, 3);
			AddNoisecheckBox.Name = "AddNoisecheckBox";
			AddNoisecheckBox.Size = new System.Drawing.Size(79, 19);
			AddNoisecheckBox.TabIndex = 92;
			AddNoisecheckBox.Text = "Add noise";
			AddNoisecheckBox.UseVisualStyleBackColor = true;
			// 
			// ThresholdsGroupBox
			// 
			ThresholdsGroupBox.Controls.Add(MinvScrollBar);
			ThresholdsGroupBox.Controls.Add(FourLabel);
			ThresholdsGroupBox.Controls.Add(MinLabel);
			ThresholdsGroupBox.Controls.Add(SixLabel);
			ThresholdsGroupBox.Controls.Add(EightLabel);
			ThresholdsGroupBox.Controls.Add(Offsetlabel);
			ThresholdsGroupBox.Controls.Add(FourvScrollBar);
			ThresholdsGroupBox.Controls.Add(SixvScrollBar);
			ThresholdsGroupBox.Controls.Add(EightvScrollBar);
			ThresholdsGroupBox.Controls.Add(OffsetvScrollBar1);
			ThresholdsGroupBox.Controls.Add(POffsetLabel);
			ThresholdsGroupBox.Controls.Add(PFourSixLabel);
			ThresholdsGroupBox.Controls.Add(PMinLabel);
			ThresholdsGroupBox.Controls.Add(PSixEightLabel);
			ThresholdsGroupBox.Controls.Add(PMaxLabel);
			ThresholdsGroupBox.Location = new System.Drawing.Point(706, 18);
			ThresholdsGroupBox.Margin = new Padding(4, 3, 4, 3);
			ThresholdsGroupBox.Name = "ThresholdsGroupBox";
			ThresholdsGroupBox.Padding = new Padding(4, 3, 4, 3);
			ThresholdsGroupBox.Size = new System.Drawing.Size(207, 288);
			ThresholdsGroupBox.TabIndex = 89;
			ThresholdsGroupBox.TabStop = false;
			ThresholdsGroupBox.Text = "Thresholds";
			// 
			// MinvScrollBar
			// 
			MinvScrollBar.DataBindings.Add(new Binding("Value", settings1, "Min", true, DataSourceUpdateMode.OnPropertyChanged));
			MinvScrollBar.Location = new System.Drawing.Point(15, 45);
			MinvScrollBar.Maximum = 264;
			MinvScrollBar.Name = "MinvScrollBar";
			MinvScrollBar.Size = new System.Drawing.Size(15, 212);
			MinvScrollBar.TabIndex = 65;
			MinvScrollBar.ValueChanged += FourvScrollBar_ValueChanged;
			// 
			// FourLabel
			// 
			FourLabel.AutoSize = true;
			FourLabel.Location = new System.Drawing.Point(56, 268);
			FourLabel.Margin = new Padding(4, 0, 4, 0);
			FourLabel.Name = "FourLabel";
			FourLabel.Size = new System.Drawing.Size(19, 15);
			FourLabel.TabIndex = 62;
			FourLabel.Text = "60";
			// 
			// MinLabel
			// 
			MinLabel.AutoSize = true;
			MinLabel.Location = new System.Drawing.Point(20, 268);
			MinLabel.Margin = new Padding(4, 0, 4, 0);
			MinLabel.Name = "MinLabel";
			MinLabel.Size = new System.Drawing.Size(13, 15);
			MinLabel.TabIndex = 60;
			MinLabel.Text = "0";
			// 
			// SixLabel
			// 
			SixLabel.AutoSize = true;
			SixLabel.Location = new System.Drawing.Point(94, 268);
			SixLabel.Margin = new Padding(4, 0, 4, 0);
			SixLabel.Name = "SixLabel";
			SixLabel.Size = new System.Drawing.Size(25, 15);
			SixLabel.TabIndex = 59;
			SixLabel.Text = "100";
			// 
			// EightLabel
			// 
			EightLabel.AutoSize = true;
			EightLabel.Location = new System.Drawing.Point(140, 268);
			EightLabel.Margin = new Padding(4, 0, 4, 0);
			EightLabel.Name = "EightLabel";
			EightLabel.Size = new System.Drawing.Size(25, 15);
			EightLabel.TabIndex = 57;
			EightLabel.Text = "140";
			// 
			// Offsetlabel
			// 
			Offsetlabel.AutoSize = true;
			Offsetlabel.Location = new System.Drawing.Point(174, 268);
			Offsetlabel.Margin = new Padding(4, 0, 4, 0);
			Offsetlabel.Name = "Offsetlabel";
			Offsetlabel.Size = new System.Drawing.Size(13, 15);
			Offsetlabel.TabIndex = 55;
			Offsetlabel.Text = "0";
			// 
			// FourvScrollBar
			// 
			FourvScrollBar.DataBindings.Add(new Binding("Value", settings1, "FourSix", true, DataSourceUpdateMode.OnPropertyChanged));
			FourvScrollBar.Location = new System.Drawing.Point(52, 45);
			FourvScrollBar.Maximum = 264;
			FourvScrollBar.Name = "FourvScrollBar";
			FourvScrollBar.Size = new System.Drawing.Size(15, 212);
			FourvScrollBar.TabIndex = 66;
			FourvScrollBar.Value = 60;
			FourvScrollBar.ValueChanged += FourvScrollBar_ValueChanged;
			// 
			// SixvScrollBar
			// 
			SixvScrollBar.DataBindings.Add(new Binding("Value", settings1, "SixEight", true, DataSourceUpdateMode.OnPropertyChanged));
			SixvScrollBar.Location = new System.Drawing.Point(92, 45);
			SixvScrollBar.Maximum = 255;
			SixvScrollBar.Name = "SixvScrollBar";
			SixvScrollBar.Size = new System.Drawing.Size(15, 212);
			SixvScrollBar.TabIndex = 67;
			SixvScrollBar.Value = 100;
			SixvScrollBar.ValueChanged += FourvScrollBar_ValueChanged;
			// 
			// EightvScrollBar
			// 
			EightvScrollBar.DataBindings.Add(new Binding("Value", settings1, "Max", true, DataSourceUpdateMode.OnPropertyChanged));
			EightvScrollBar.Location = new System.Drawing.Point(138, 45);
			EightvScrollBar.Maximum = 264;
			EightvScrollBar.Name = "EightvScrollBar";
			EightvScrollBar.Size = new System.Drawing.Size(15, 212);
			EightvScrollBar.TabIndex = 68;
			EightvScrollBar.Value = 140;
			EightvScrollBar.ValueChanged += FourvScrollBar_ValueChanged;
			// 
			// OffsetvScrollBar1
			// 
			OffsetvScrollBar1.DataBindings.Add(new Binding("Value", settings1, "Offset", true, DataSourceUpdateMode.OnPropertyChanged));
			OffsetvScrollBar1.Location = new System.Drawing.Point(172, 45);
			OffsetvScrollBar1.Maximum = 50;
			OffsetvScrollBar1.Minimum = -50;
			OffsetvScrollBar1.Name = "OffsetvScrollBar1";
			OffsetvScrollBar1.Size = new System.Drawing.Size(15, 212);
			OffsetvScrollBar1.TabIndex = 69;
			OffsetvScrollBar1.ValueChanged += FourvScrollBar_ValueChanged;
			// 
			// POffsetLabel
			// 
			POffsetLabel.AutoSize = true;
			POffsetLabel.Location = new System.Drawing.Point(166, 27);
			POffsetLabel.Margin = new Padding(4, 0, 4, 0);
			POffsetLabel.Name = "POffsetLabel";
			POffsetLabel.Size = new System.Drawing.Size(39, 15);
			POffsetLabel.TabIndex = 56;
			POffsetLabel.Text = "Offset";
			// 
			// PFourSixLabel
			// 
			PFourSixLabel.AutoSize = true;
			PFourSixLabel.Location = new System.Drawing.Point(47, 27);
			PFourSixLabel.Margin = new Padding(4, 0, 4, 0);
			PFourSixLabel.Name = "PFourSixLabel";
			PFourSixLabel.Size = new System.Drawing.Size(24, 15);
			PFourSixLabel.TabIndex = 64;
			PFourSixLabel.Text = "4/6";
			// 
			// PMinLabel
			// 
			PMinLabel.AutoSize = true;
			PMinLabel.Location = new System.Drawing.Point(14, 27);
			PMinLabel.Margin = new Padding(4, 0, 4, 0);
			PMinLabel.Name = "PMinLabel";
			PMinLabel.Size = new System.Drawing.Size(28, 15);
			PMinLabel.TabIndex = 63;
			PMinLabel.Text = "min";
			// 
			// PSixEightLabel
			// 
			PSixEightLabel.AutoSize = true;
			PSixEightLabel.Location = new System.Drawing.Point(86, 27);
			PSixEightLabel.Margin = new Padding(4, 0, 4, 0);
			PSixEightLabel.Name = "PSixEightLabel";
			PSixEightLabel.Size = new System.Drawing.Size(24, 15);
			PSixEightLabel.TabIndex = 61;
			PSixEightLabel.Text = "6/8";
			// 
			// PMaxLabel
			// 
			PMaxLabel.AutoSize = true;
			PMaxLabel.Location = new System.Drawing.Point(132, 27);
			PMaxLabel.Margin = new Padding(4, 0, 4, 0);
			PMaxLabel.Name = "PMaxLabel";
			PMaxLabel.Size = new System.Drawing.Size(30, 15);
			PMaxLabel.TabIndex = 58;
			PMaxLabel.Text = "max";
			// 
			// FindDupesCheckBox
			// 
			FindDupesCheckBox.AccessibleDescription = "";
			FindDupesCheckBox.AutoSize = true;
			FindDupesCheckBox.Location = new System.Drawing.Point(150, 182);
			FindDupesCheckBox.Margin = new Padding(4, 3, 4, 3);
			FindDupesCheckBox.Name = "FindDupesCheckBox";
			FindDupesCheckBox.Size = new System.Drawing.Size(89, 19);
			FindDupesCheckBox.TabIndex = 88;
			FindDupesCheckBox.Text = "Deduplicate";
			FindDupesCheckBox.UseVisualStyleBackColor = true;
			// 
			// AutoRefreshSectorMapCheck
			// 
			AutoRefreshSectorMapCheck.AccessibleDescription = "";
			AutoRefreshSectorMapCheck.AutoSize = true;
			AutoRefreshSectorMapCheck.Checked = true;
			AutoRefreshSectorMapCheck.CheckState = CheckState.Checked;
			AutoRefreshSectorMapCheck.Location = new System.Drawing.Point(150, 212);
			AutoRefreshSectorMapCheck.Margin = new Padding(4, 3, 4, 3);
			AutoRefreshSectorMapCheck.Name = "AutoRefreshSectorMapCheck";
			AutoRefreshSectorMapCheck.Size = new System.Drawing.Size(150, 19);
			AutoRefreshSectorMapCheck.TabIndex = 85;
			AutoRefreshSectorMapCheck.Text = "Auto refresh sectormap";
			AutoRefreshSectorMapCheck.UseVisualStyleBackColor = true;
			AutoRefreshSectorMapCheck.CheckedChanged += AutoRefreshSectorMapCheck_CheckedChanged;
			// 
			// label50
			// 
			label50.AutoSize = true;
			label50.Location = new System.Drawing.Point(573, 220);
			label50.Margin = new Padding(4, 0, 4, 0);
			label50.Name = "label50";
			label50.Size = new System.Drawing.Size(62, 15);
			label50.TabIndex = 80;
			label50.Text = "Adapt rate";
			// 
			// RateOfChangeUpDown
			// 
			RateOfChangeUpDown.DecimalPlaces = 2;
			RateOfChangeUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
			RateOfChangeUpDown.Location = new System.Drawing.Point(642, 218);
			RateOfChangeUpDown.Margin = new Padding(4, 3, 4, 3);
			RateOfChangeUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			RateOfChangeUpDown.Name = "RateOfChangeUpDown";
			RateOfChangeUpDown.Size = new System.Drawing.Size(56, 23);
			RateOfChangeUpDown.TabIndex = 79;
			RateOfChangeUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			// 
			// IgnoreHeaderErrorCheckBox
			// 
			IgnoreHeaderErrorCheckBox.AutoSize = true;
			IgnoreHeaderErrorCheckBox.Location = new System.Drawing.Point(150, 156);
			IgnoreHeaderErrorCheckBox.Margin = new Padding(4, 3, 4, 3);
			IgnoreHeaderErrorCheckBox.Name = "IgnoreHeaderErrorCheckBox";
			IgnoreHeaderErrorCheckBox.Size = new System.Drawing.Size(127, 19);
			IgnoreHeaderErrorCheckBox.TabIndex = 50;
			IgnoreHeaderErrorCheckBox.Text = "Ignore header error";
			IgnoreHeaderErrorCheckBox.UseVisualStyleBackColor = true;
			// 
			// groupBox5
			// 
			groupBox5.Controls.Add(Histogrampanel1);
			groupBox5.Controls.Add(HistogramhScrollBar1);
			groupBox5.Location = new System.Drawing.Point(358, 15);
			groupBox5.Margin = new Padding(4, 3, 4, 3);
			groupBox5.Name = "groupBox5";
			groupBox5.Padding = new Padding(4, 3, 4, 3);
			groupBox5.Size = new System.Drawing.Size(324, 192);
			groupBox5.TabIndex = 53;
			groupBox5.TabStop = false;
			groupBox5.Text = "Histogram";
			// 
			// Histogrampanel1
			// 
			Histogrampanel1.Location = new System.Drawing.Point(7, 22);
			Histogrampanel1.Margin = new Padding(4, 3, 4, 3);
			Histogrampanel1.Name = "Histogrampanel1";
			Histogrampanel1.Size = new System.Drawing.Size(303, 126);
			Histogrampanel1.TabIndex = 36;
			Histogrampanel1.Click += Histogrampanel1_Click;
			Histogrampanel1.Paint += Histogrampanel1_Paint;
			// 
			// HistogramhScrollBar1
			// 
			HistogramhScrollBar1.LargeChange = 10000;
			HistogramhScrollBar1.Location = new System.Drawing.Point(1, 167);
			HistogramhScrollBar1.Margin = new Padding(4, 3, 4, 3);
			HistogramhScrollBar1.Maximum = 4000;
			HistogramhScrollBar1.Name = "HistogramhScrollBar1";
			HistogramhScrollBar1.Size = new System.Drawing.Size(323, 45);
			HistogramhScrollBar1.TabIndex = 105;
			HistogramhScrollBar1.TickFrequency = 2000;
			HistogramhScrollBar1.TickStyle = TickStyle.None;
			HistogramhScrollBar1.Scroll += HistogramhScrollBar1_Scroll;
			// 
			// groupBox3
			// 
			groupBox3.Controls.Add(ScanComboBox);
			groupBox3.Controls.Add(ScanButton);
			groupBox3.Location = new System.Drawing.Point(7, 282);
			groupBox3.Margin = new Padding(4, 3, 4, 3);
			groupBox3.Name = "groupBox3";
			groupBox3.Padding = new Padding(4, 3, 4, 3);
			groupBox3.Size = new System.Drawing.Size(265, 53);
			groupBox3.TabIndex = 54;
			groupBox3.TabStop = false;
			groupBox3.Text = "Scan";
			// 
			// ScanComboBox
			// 
			ScanComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			ScanComboBox.FormattingEnabled = true;
			ScanComboBox.Location = new System.Drawing.Point(7, 18);
			ScanComboBox.Margin = new Padding(4, 3, 4, 3);
			ScanComboBox.Name = "ScanComboBox";
			ScanComboBox.Size = new System.Drawing.Size(190, 23);
			ScanComboBox.TabIndex = 104;
			// 
			// ScanButton
			// 
			ScanButton.Location = new System.Drawing.Point(204, 16);
			ScanButton.Margin = new Padding(4, 3, 4, 3);
			ScanButton.Name = "ScanButton";
			ScanButton.Size = new System.Drawing.Size(47, 27);
			ScanButton.TabIndex = 39;
			ScanButton.Text = "Scan";
			ScanButton.UseVisualStyleBackColor = true;
			ScanButton.Click += ScanBtn_Click_1;
			// 
			// HDCheckBox
			// 
			HDCheckBox.AutoSize = true;
			HDCheckBox.Enabled = false;
			HDCheckBox.Location = new System.Drawing.Point(6, 159);
			HDCheckBox.Margin = new Padding(4, 3, 4, 3);
			HDCheckBox.Name = "HDCheckBox";
			HDCheckBox.Size = new System.Drawing.Size(94, 19);
			HDCheckBox.TabIndex = 51;
			HDCheckBox.Text = "High Density";
			HDCheckBox.UseVisualStyleBackColor = true;
			HDCheckBox.CheckedChanged += HDCheckBox_CheckedChanged;
			// 
			// label37
			// 
			label37.AutoSize = true;
			label37.Location = new System.Drawing.Point(4, 22);
			label37.Margin = new Padding(4, 0, 4, 0);
			label37.Name = "label37";
			label37.Size = new System.Drawing.Size(95, 15);
			label37.TabIndex = 44;
			label37.Text = "MFM processing";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(147, 22);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(73, 15);
			label2.TabIndex = 45;
			label2.Text = "Period > 8us";
			// 
			// ProcessBtn
			// 
			ProcessBtn.Location = new System.Drawing.Point(7, 7);
			ProcessBtn.Margin = new Padding(4, 3, 4, 3);
			ProcessBtn.Name = "ProcessBtn";
			ProcessBtn.Size = new System.Drawing.Size(88, 46);
			ProcessBtn.TabIndex = 4;
			ProcessBtn.Text = "Process Amiga!";
			ProcessBtn.UseVisualStyleBackColor = true;
			ProcessBtn.Click += ProcessAmigaBtn_Click;
			// 
			// ProcessPCBtn
			// 
			ProcessPCBtn.Location = new System.Drawing.Point(7, 60);
			ProcessPCBtn.Margin = new Padding(4, 3, 4, 3);
			ProcessPCBtn.Name = "ProcessPCBtn";
			ProcessPCBtn.Size = new System.Drawing.Size(88, 46);
			ProcessPCBtn.TabIndex = 3;
			ProcessPCBtn.Text = "Process PC!";
			ProcessPCBtn.UseVisualStyleBackColor = true;
			ProcessPCBtn.Click += ProcessPCBtn_Click;
			// 
			// ErrorCorrectionTab
			// 
			ErrorCorrectionTab.BackColor = System.Drawing.SystemColors.Control;
			ErrorCorrectionTab.Controls.Add(EntropySpliceBtn);
			ErrorCorrectionTab.Controls.Add(AvgPeriodsFromListSelBtn);
			ErrorCorrectionTab.Controls.Add(CombinationsUpDown);
			ErrorCorrectionTab.Controls.Add(label79);
			ErrorCorrectionTab.Controls.Add(button44);
			ErrorCorrectionTab.Controls.Add(ECMFMByteEncbutton);
			ErrorCorrectionTab.Controls.Add(MFMByteLengthUpDown);
			ErrorCorrectionTab.Controls.Add(MFMByteStartUpDown);
			ErrorCorrectionTab.Controls.Add(label76);
			ErrorCorrectionTab.Controls.Add(label77);
			ErrorCorrectionTab.Controls.Add(button38);
			ErrorCorrectionTab.Controls.Add(ECMFMcheckBox);
			ErrorCorrectionTab.Controls.Add(label71);
			ErrorCorrectionTab.Controls.Add(label6);
			ErrorCorrectionTab.Controls.Add(C8StartUpDown);
			ErrorCorrectionTab.Controls.Add(C6StartUpDown);
			ErrorCorrectionTab.Controls.Add(button1);
			ErrorCorrectionTab.Controls.Add(BadSectorsCheckBox);
			ErrorCorrectionTab.Controls.Add(GoodSectorsCheckBox);
			ErrorCorrectionTab.Controls.Add(ECRealign4E);
			ErrorCorrectionTab.Controls.Add(ECInfoTabs);
			ErrorCorrectionTab.Controls.Add(ECZoomOutBtn);
			ErrorCorrectionTab.Controls.Add(SelectionDifLabel);
			ErrorCorrectionTab.Controls.Add(ScatterOffsetUpDown);
			ErrorCorrectionTab.Controls.Add(ScatterMinUpDown);
			ErrorCorrectionTab.Controls.Add(ScatterMaxUpDown);
			ErrorCorrectionTab.Controls.Add(ScatterMaxTrackBar);
			ErrorCorrectionTab.Controls.Add(ScatterMinTrackBar);
			ErrorCorrectionTab.Controls.Add(groupBox2);
			ErrorCorrectionTab.Controls.Add(RedCrcCheckLabel);
			ErrorCorrectionTab.Controls.Add(label43);
			ErrorCorrectionTab.Controls.Add(BSEditByteLabel);
			ErrorCorrectionTab.Controls.Add(BluetoRedByteCopyToolBtn);
			ErrorCorrectionTab.Controls.Add(CopySectorToBlueBtn);
			ErrorCorrectionTab.Controls.Add(label55);
			ErrorCorrectionTab.Controls.Add(panel4);
			ErrorCorrectionTab.Controls.Add(panel3);
			ErrorCorrectionTab.Controls.Add(label54);
			ErrorCorrectionTab.Controls.Add(label53);
			ErrorCorrectionTab.Controls.Add(BadSectorListBox);
			ErrorCorrectionTab.Controls.Add(Sector2UpDown);
			ErrorCorrectionTab.Controls.Add(Track2UpDown);
			ErrorCorrectionTab.Controls.Add(label48);
			ErrorCorrectionTab.Controls.Add(label49);
			ErrorCorrectionTab.Controls.Add(Sector1UpDown);
			ErrorCorrectionTab.Controls.Add(Track1UpDown);
			ErrorCorrectionTab.Controls.Add(BlueCrcCheckLabel);
			ErrorCorrectionTab.Controls.Add(label47);
			ErrorCorrectionTab.Controls.Add(ECSectorOverlayBtn);
			ErrorCorrectionTab.Controls.Add(BadSectorPanel);
			ErrorCorrectionTab.Controls.Add(ScatterOffsetTrackBar);
			ErrorCorrectionTab.ImageIndex = 1;
			ErrorCorrectionTab.Location = new System.Drawing.Point(4, 24);
			ErrorCorrectionTab.Margin = new Padding(4, 3, 4, 3);
			ErrorCorrectionTab.Name = "ErrorCorrectionTab";
			ErrorCorrectionTab.Padding = new Padding(4, 3, 4, 3);
			ErrorCorrectionTab.Size = new System.Drawing.Size(1128, 897);
			ErrorCorrectionTab.TabIndex = 2;
			ErrorCorrectionTab.Text = "Error Correction";
			// 
			// EntropySpliceBtn
			// 
			EntropySpliceBtn.Location = new System.Drawing.Point(825, 571);
			EntropySpliceBtn.Margin = new Padding(4, 3, 4, 3);
			EntropySpliceBtn.Name = "EntropySpliceBtn";
			EntropySpliceBtn.Size = new System.Drawing.Size(91, 45);
			EntropySpliceBtn.TabIndex = 4029;
			EntropySpliceBtn.Text = "Entropy Splice";
			EntropySpliceBtn.UseVisualStyleBackColor = true;
			EntropySpliceBtn.Click += EntropySpliceBtn_Click;
			// 
			// AvgPeriodsFromListSelBtn
			// 
			AvgPeriodsFromListSelBtn.Location = new System.Drawing.Point(825, 519);
			AvgPeriodsFromListSelBtn.Margin = new Padding(4, 3, 4, 3);
			AvgPeriodsFromListSelBtn.Name = "AvgPeriodsFromListSelBtn";
			AvgPeriodsFromListSelBtn.Size = new System.Drawing.Size(91, 45);
			AvgPeriodsFromListSelBtn.TabIndex = 4028;
			AvgPeriodsFromListSelBtn.Text = "Avg periods from list sel";
			AvgPeriodsFromListSelBtn.UseVisualStyleBackColor = true;
			AvgPeriodsFromListSelBtn.Click += AvgPeriodsFromListSelBtn_Click;
			// 
			// CombinationsUpDown
			// 
			CombinationsUpDown.Location = new System.Drawing.Point(390, 343);
			CombinationsUpDown.Margin = new Padding(4, 3, 4, 3);
			CombinationsUpDown.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
			CombinationsUpDown.Name = "CombinationsUpDown";
			CombinationsUpDown.Size = new System.Drawing.Size(90, 23);
			CombinationsUpDown.TabIndex = 4027;
			CombinationsUpDown.Value = new decimal(new int[] { 10000000, 0, 0, 0 });
			// 
			// label79
			// 
			label79.AutoSize = true;
			label79.Location = new System.Drawing.Point(302, 347);
			label79.Margin = new Padding(4, 0, 4, 0);
			label79.Name = "label79";
			label79.Size = new System.Drawing.Size(82, 15);
			label79.TabIndex = 4026;
			label79.Text = "Combinations";
			// 
			// button44
			// 
			button44.Location = new System.Drawing.Point(923, 571);
			button44.Margin = new Padding(4, 3, 4, 3);
			button44.Name = "button44";
			button44.Size = new System.Drawing.Size(91, 45);
			button44.TabIndex = 4025;
			button44.Text = "Iterator test";
			button44.UseVisualStyleBackColor = true;
			button44.Click += Button44_Click;
			// 
			// ECMFMByteEncbutton
			// 
			ECMFMByteEncbutton.Location = new System.Drawing.Point(8, 313);
			ECMFMByteEncbutton.Margin = new Padding(4, 3, 4, 3);
			ECMFMByteEncbutton.Name = "ECMFMByteEncbutton";
			ECMFMByteEncbutton.Size = new System.Drawing.Size(91, 45);
			ECMFMByteEncbutton.TabIndex = 4024;
			ECMFMByteEncbutton.Text = "EC MFM byte enc";
			ECMFMByteEncbutton.UseVisualStyleBackColor = true;
			ECMFMByteEncbutton.Click += ECMFMByteEncbutton_Click;
			// 
			// MFMByteLengthUpDown
			// 
			MFMByteLengthUpDown.Location = new System.Drawing.Point(217, 343);
			MFMByteLengthUpDown.Margin = new Padding(4, 3, 4, 3);
			MFMByteLengthUpDown.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
			MFMByteLengthUpDown.Name = "MFMByteLengthUpDown";
			MFMByteLengthUpDown.Size = new System.Drawing.Size(69, 23);
			MFMByteLengthUpDown.TabIndex = 4022;
			MFMByteLengthUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
			// 
			// MFMByteStartUpDown
			// 
			MFMByteStartUpDown.Location = new System.Drawing.Point(217, 313);
			MFMByteStartUpDown.Margin = new Padding(4, 3, 4, 3);
			MFMByteStartUpDown.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
			MFMByteStartUpDown.Minimum = new decimal(new int[] { 20000, 0, 0, int.MinValue });
			MFMByteStartUpDown.Name = "MFMByteStartUpDown";
			MFMByteStartUpDown.Size = new System.Drawing.Size(69, 23);
			MFMByteStartUpDown.TabIndex = 4020;
			// 
			// label76
			// 
			label76.AutoSize = true;
			label76.Location = new System.Drawing.Point(113, 345);
			label76.Margin = new Padding(4, 0, 4, 0);
			label76.Name = "label76";
			label76.Size = new System.Drawing.Size(101, 15);
			label76.TabIndex = 4021;
			label76.Text = "MFM Byte Length";
			// 
			// label77
			// 
			label77.AutoSize = true;
			label77.Location = new System.Drawing.Point(117, 315);
			label77.Margin = new Padding(4, 0, 4, 0);
			label77.Name = "label77";
			label77.Size = new System.Drawing.Size(88, 15);
			label77.TabIndex = 4023;
			label77.Text = "MFM Start byte";
			// 
			// button38
			// 
			button38.Location = new System.Drawing.Point(923, 519);
			button38.Margin = new Padding(4, 3, 4, 3);
			button38.Name = "button38";
			button38.Size = new System.Drawing.Size(91, 45);
			button38.TabIndex = 4019;
			button38.Text = "Show mfmbyteenc";
			button38.UseVisualStyleBackColor = true;
			button38.Click += Button38_Click;
			// 
			// ECMFMcheckBox
			// 
			ECMFMcheckBox.AutoSize = true;
			ECMFMcheckBox.Checked = true;
			ECMFMcheckBox.CheckState = CheckState.Checked;
			ECMFMcheckBox.Location = new System.Drawing.Point(229, 181);
			ECMFMcheckBox.Margin = new Padding(4, 3, 4, 3);
			ECMFMcheckBox.Name = "ECMFMcheckBox";
			ECMFMcheckBox.Size = new System.Drawing.Size(54, 19);
			ECMFMcheckBox.TabIndex = 4018;
			ECMFMcheckBox.Text = "MFM";
			ECMFMcheckBox.UseVisualStyleBackColor = true;
			// 
			// label71
			// 
			label71.AutoSize = true;
			label71.Location = new System.Drawing.Point(274, 256);
			label71.Margin = new Padding(4, 0, 4, 0);
			label71.Name = "label71";
			label71.Size = new System.Drawing.Size(47, 15);
			label71.TabIndex = 4017;
			label71.Text = "C8 start";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(210, 256);
			label6.Margin = new Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(47, 15);
			label6.TabIndex = 4016;
			label6.Text = "C6 start";
			// 
			// C8StartUpDown
			// 
			C8StartUpDown.Location = new System.Drawing.Point(278, 273);
			C8StartUpDown.Margin = new Padding(4, 3, 4, 3);
			C8StartUpDown.Name = "C8StartUpDown";
			C8StartUpDown.Size = new System.Drawing.Size(47, 23);
			C8StartUpDown.TabIndex = 4015;
			// 
			// C6StartUpDown
			// 
			C6StartUpDown.Location = new System.Drawing.Point(210, 273);
			C6StartUpDown.Margin = new Padding(4, 3, 4, 3);
			C6StartUpDown.Name = "C6StartUpDown";
			C6StartUpDown.Size = new System.Drawing.Size(50, 23);
			C6StartUpDown.TabIndex = 4014;
			C6StartUpDown.Value = new decimal(new int[] { 3, 0, 0, 0 });
			// 
			// button1
			// 
			button1.Location = new System.Drawing.Point(104, 261);
			button1.Margin = new Padding(4, 3, 4, 3);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(91, 45);
			button1.TabIndex = 4013;
			button1.Text = "Process Cluster2";
			button1.UseVisualStyleBackColor = true;
			button1.Click += Button1_Click;
			// 
			// BadSectorsCheckBox
			// 
			BadSectorsCheckBox.AutoSize = true;
			BadSectorsCheckBox.Checked = true;
			BadSectorsCheckBox.CheckState = CheckState.Checked;
			BadSectorsCheckBox.Location = new System.Drawing.Point(229, 155);
			BadSectorsCheckBox.Margin = new Padding(4, 3, 4, 3);
			BadSectorsCheckBox.Name = "BadSectorsCheckBox";
			BadSectorsCheckBox.Size = new System.Drawing.Size(86, 19);
			BadSectorsCheckBox.TabIndex = 4012;
			BadSectorsCheckBox.Text = "Bad sectors";
			BadSectorsCheckBox.UseVisualStyleBackColor = true;
			// 
			// GoodSectorsCheckBox
			// 
			GoodSectorsCheckBox.AutoSize = true;
			GoodSectorsCheckBox.Location = new System.Drawing.Point(229, 134);
			GoodSectorsCheckBox.Margin = new Padding(4, 3, 4, 3);
			GoodSectorsCheckBox.Name = "GoodSectorsCheckBox";
			GoodSectorsCheckBox.Size = new System.Drawing.Size(95, 19);
			GoodSectorsCheckBox.TabIndex = 4011;
			GoodSectorsCheckBox.Text = "Good sectors";
			GoodSectorsCheckBox.UseVisualStyleBackColor = true;
			// 
			// ECRealign4E
			// 
			ECRealign4E.Location = new System.Drawing.Point(104, 209);
			ECRealign4E.Margin = new Padding(4, 3, 4, 3);
			ECRealign4E.Name = "ECRealign4E";
			ECRealign4E.Size = new System.Drawing.Size(91, 45);
			ECRealign4E.TabIndex = 4010;
			ECRealign4E.Text = "Realign 4E";
			ECRealign4E.UseVisualStyleBackColor = true;
			ECRealign4E.Click += ECRealign4E_Click;
			// 
			// ECInfoTabs
			// 
			ECInfoTabs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			ECInfoTabs.Controls.Add(ECTabSectorData);
			ECInfoTabs.Controls.Add(tabPage8);
			ECInfoTabs.Location = new System.Drawing.Point(7, 365);
			ECInfoTabs.Margin = new Padding(4, 3, 4, 3);
			ECInfoTabs.Name = "ECInfoTabs";
			ECInfoTabs.SelectedIndex = 0;
			ECInfoTabs.Size = new System.Drawing.Size(527, 519);
			ECInfoTabs.TabIndex = 4009;
			// 
			// ECTabSectorData
			// 
			ECTabSectorData.Controls.Add(antbSectorData);
			ECTabSectorData.Location = new System.Drawing.Point(4, 24);
			ECTabSectorData.Margin = new Padding(4, 3, 4, 3);
			ECTabSectorData.Name = "ECTabSectorData";
			ECTabSectorData.Padding = new Padding(4, 3, 4, 3);
			ECTabSectorData.Size = new System.Drawing.Size(519, 491);
			ECTabSectorData.TabIndex = 0;
			ECTabSectorData.Text = "Sector data";
			ECTabSectorData.UseVisualStyleBackColor = true;
			// 
			// antbSectorData
			// 
			antbSectorData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			antbSectorData.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			antbSectorData.Location = new System.Drawing.Point(0, 0);
			antbSectorData.Margin = new Padding(4, 3, 4, 3);
			antbSectorData.MaxLength = 200000;
			antbSectorData.Multiline = true;
			antbSectorData.Name = "antbSectorData";
			antbSectorData.ScrollBars = ScrollBars.Vertical;
			antbSectorData.Size = new System.Drawing.Size(514, 482);
			antbSectorData.TabIndex = 86;
			// 
			// tabPage8
			// 
			tabPage8.Controls.Add(ECtbMFM);
			tabPage8.Location = new System.Drawing.Point(4, 24);
			tabPage8.Margin = new Padding(4, 3, 4, 3);
			tabPage8.Name = "tabPage8";
			tabPage8.Padding = new Padding(4, 3, 4, 3);
			tabPage8.Size = new System.Drawing.Size(519, 491);
			tabPage8.TabIndex = 1;
			tabPage8.Text = "MFM data";
			tabPage8.UseVisualStyleBackColor = true;
			// 
			// ECtbMFM
			// 
			ECtbMFM.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			ECtbMFM.Location = new System.Drawing.Point(0, 0);
			ECtbMFM.Margin = new Padding(4, 3, 4, 3);
			ECtbMFM.MaxLength = 200000;
			ECtbMFM.Multiline = true;
			ECtbMFM.Name = "ECtbMFM";
			ECtbMFM.ScrollBars = ScrollBars.Vertical;
			ECtbMFM.Size = new System.Drawing.Size(514, 454);
			ECtbMFM.TabIndex = 88;
			// 
			// ECZoomOutBtn
			// 
			ECZoomOutBtn.Location = new System.Drawing.Point(567, 662);
			ECZoomOutBtn.Margin = new Padding(4, 3, 4, 3);
			ECZoomOutBtn.Name = "ECZoomOutBtn";
			ECZoomOutBtn.Size = new System.Drawing.Size(76, 27);
			ECZoomOutBtn.TabIndex = 4007;
			ECZoomOutBtn.Text = "Zoom out";
			ECZoomOutBtn.UseVisualStyleBackColor = true;
			ECZoomOutBtn.Click += ECZoomOutBtn_Click;
			// 
			// SelectionDifLabel
			// 
			SelectionDifLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			SelectionDifLabel.AutoSize = true;
			SelectionDifLabel.Location = new System.Drawing.Point(564, 713);
			SelectionDifLabel.Margin = new Padding(4, 0, 4, 0);
			SelectionDifLabel.Name = "SelectionDifLabel";
			SelectionDifLabel.Size = new System.Drawing.Size(49, 15);
			SelectionDifLabel.TabIndex = 63;
			SelectionDifLabel.Text = "Periods:";
			// 
			// ScatterOffsetUpDown
			// 
			ScatterOffsetUpDown.Location = new System.Drawing.Point(650, 662);
			ScatterOffsetUpDown.Margin = new Padding(4, 3, 4, 3);
			ScatterOffsetUpDown.Maximum = new decimal(new int[] { 40000, 0, 0, 0 });
			ScatterOffsetUpDown.Minimum = new decimal(new int[] { 40000, 0, 0, int.MinValue });
			ScatterOffsetUpDown.Name = "ScatterOffsetUpDown";
			ScatterOffsetUpDown.Size = new System.Drawing.Size(65, 23);
			ScatterOffsetUpDown.TabIndex = 4006;
			// 
			// ScatterMinUpDown
			// 
			ScatterMinUpDown.Location = new System.Drawing.Point(650, 697);
			ScatterMinUpDown.Margin = new Padding(4, 3, 4, 3);
			ScatterMinUpDown.Maximum = new decimal(new int[] { 50000, 0, 0, 0 });
			ScatterMinUpDown.Minimum = new decimal(new int[] { 50000, 0, 0, int.MinValue });
			ScatterMinUpDown.Name = "ScatterMinUpDown";
			ScatterMinUpDown.Size = new System.Drawing.Size(65, 23);
			ScatterMinUpDown.TabIndex = 4005;
			// 
			// ScatterMaxUpDown
			// 
			ScatterMaxUpDown.Location = new System.Drawing.Point(650, 728);
			ScatterMaxUpDown.Margin = new Padding(4, 3, 4, 3);
			ScatterMaxUpDown.Maximum = new decimal(new int[] { 50000, 0, 0, 0 });
			ScatterMaxUpDown.Minimum = new decimal(new int[] { 50000, 0, 0, int.MinValue });
			ScatterMaxUpDown.Name = "ScatterMaxUpDown";
			ScatterMaxUpDown.Size = new System.Drawing.Size(68, 23);
			ScatterMaxUpDown.TabIndex = 4004;
			// 
			// ScatterMaxTrackBar
			// 
			ScatterMaxTrackBar.LargeChange = 25;
			ScatterMaxTrackBar.Location = new System.Drawing.Point(722, 722);
			ScatterMaxTrackBar.Margin = new Padding(4, 3, 4, 3);
			ScatterMaxTrackBar.Maximum = 10000;
			ScatterMaxTrackBar.Minimum = -10000;
			ScatterMaxTrackBar.Name = "ScatterMaxTrackBar";
			ScatterMaxTrackBar.Size = new System.Drawing.Size(401, 45);
			ScatterMaxTrackBar.TabIndex = 103;
			ScatterMaxTrackBar.TickFrequency = 10;
			ScatterMaxTrackBar.TickStyle = TickStyle.None;
			ScatterMaxTrackBar.Scroll += BadSectorListBox_SelectedIndexChanged;
			// 
			// ScatterMinTrackBar
			// 
			ScatterMinTrackBar.LargeChange = 25;
			ScatterMinTrackBar.Location = new System.Drawing.Point(722, 692);
			ScatterMinTrackBar.Margin = new Padding(4, 3, 4, 3);
			ScatterMinTrackBar.Maximum = 10000;
			ScatterMinTrackBar.Minimum = -10000;
			ScatterMinTrackBar.Name = "ScatterMinTrackBar";
			ScatterMinTrackBar.Size = new System.Drawing.Size(401, 45);
			ScatterMinTrackBar.TabIndex = 102;
			ScatterMinTrackBar.TickFrequency = 10;
			ScatterMinTrackBar.TickStyle = TickStyle.None;
			ScatterMinTrackBar.Scroll += BadSectorListBox_SelectedIndexChanged;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(HistScalingLabel);
			groupBox2.Controls.Add(AnHistogramPanel);
			groupBox2.Location = new System.Drawing.Point(813, 325);
			groupBox2.Margin = new Padding(4, 3, 4, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new Padding(4, 3, 4, 3);
			groupBox2.Size = new System.Drawing.Size(324, 178);
			groupBox2.TabIndex = 101;
			groupBox2.TabStop = false;
			groupBox2.Text = "Histogram";
			// 
			// HistScalingLabel
			// 
			HistScalingLabel.AutoSize = true;
			HistScalingLabel.Location = new System.Drawing.Point(7, 151);
			HistScalingLabel.Margin = new Padding(4, 0, 4, 0);
			HistScalingLabel.Name = "HistScalingLabel";
			HistScalingLabel.Size = new System.Drawing.Size(48, 15);
			HistScalingLabel.TabIndex = 102;
			HistScalingLabel.Text = "Scaling:";
			// 
			// AnHistogramPanel
			// 
			AnHistogramPanel.Location = new System.Drawing.Point(7, 22);
			AnHistogramPanel.Margin = new Padding(4, 3, 4, 3);
			AnHistogramPanel.Name = "AnHistogramPanel";
			AnHistogramPanel.Size = new System.Drawing.Size(303, 126);
			AnHistogramPanel.TabIndex = 36;
			// 
			// RedCrcCheckLabel
			// 
			RedCrcCheckLabel.AutoSize = true;
			RedCrcCheckLabel.Location = new System.Drawing.Point(126, 91);
			RedCrcCheckLabel.Margin = new Padding(4, 0, 4, 0);
			RedCrcCheckLabel.Name = "RedCrcCheckLabel";
			RedCrcCheckLabel.Size = new System.Drawing.Size(28, 15);
			RedCrcCheckLabel.TabIndex = 98;
			RedCrcCheckLabel.Text = "Crc:";
			// 
			// label43
			// 
			label43.AutoSize = true;
			label43.Location = new System.Drawing.Point(97, 45);
			label43.Margin = new Padding(4, 0, 4, 0);
			label43.Name = "label43";
			label43.Size = new System.Drawing.Size(46, 15);
			label43.TabIndex = 97;
			label43.Text = "Sector1";
			// 
			// BSEditByteLabel
			// 
			BSEditByteLabel.AutoSize = true;
			BSEditByteLabel.Location = new System.Drawing.Point(274, 78);
			BSEditByteLabel.Margin = new Padding(4, 0, 4, 0);
			BSEditByteLabel.Name = "BSEditByteLabel";
			BSEditByteLabel.Size = new System.Drawing.Size(54, 15);
			BSEditByteLabel.TabIndex = 96;
			BSEditByteLabel.Text = "Byte: 512";
			// 
			// BluetoRedByteCopyToolBtn
			// 
			BluetoRedByteCopyToolBtn.Location = new System.Drawing.Point(6, 261);
			BluetoRedByteCopyToolBtn.Margin = new Padding(4, 3, 4, 3);
			BluetoRedByteCopyToolBtn.Name = "BluetoRedByteCopyToolBtn";
			BluetoRedByteCopyToolBtn.Size = new System.Drawing.Size(91, 45);
			BluetoRedByteCopyToolBtn.TabIndex = 93;
			BluetoRedByteCopyToolBtn.Tag = "1";
			BluetoRedByteCopyToolBtn.Text = "Copy byte to blue";
			BluetoRedByteCopyToolBtn.UseVisualStyleBackColor = true;
			BluetoRedByteCopyToolBtn.Click += BluetoRedByteCopyToolBtn_Click;
			// 
			// CopySectorToBlueBtn
			// 
			CopySectorToBlueBtn.Location = new System.Drawing.Point(6, 209);
			CopySectorToBlueBtn.Margin = new Padding(4, 3, 4, 3);
			CopySectorToBlueBtn.Name = "CopySectorToBlueBtn";
			CopySectorToBlueBtn.Size = new System.Drawing.Size(91, 45);
			CopySectorToBlueBtn.TabIndex = 84;
			CopySectorToBlueBtn.Text = "Copy sector to blue";
			CopySectorToBlueBtn.UseVisualStyleBackColor = true;
			CopySectorToBlueBtn.Click += CopySectorToBlueBtn_Click;
			// 
			// label55
			// 
			label55.AutoSize = true;
			label55.Location = new System.Drawing.Point(336, 12);
			label55.Margin = new Padding(4, 0, 4, 0);
			label55.Name = "label55";
			label55.Size = new System.Drawing.Size(80, 15);
			label55.TabIndex = 92;
			label55.Text = "Bad sector list";
			// 
			// panel4
			// 
			panel4.BackColor = System.Drawing.Color.FromArgb(255, 208, 192);
			panel4.Controls.Add(BSRedTempRadio);
			panel4.Controls.Add(BSRedFromlistRadio);
			panel4.Controls.Add(radioButton6);
			panel4.Location = new System.Drawing.Point(124, 110);
			panel4.Margin = new Padding(4, 3, 4, 3);
			panel4.Name = "panel4";
			panel4.Size = new System.Drawing.Size(91, 72);
			panel4.TabIndex = 91;
			// 
			// BSRedTempRadio
			// 
			BSRedTempRadio.AutoSize = true;
			BSRedTempRadio.Location = new System.Drawing.Point(5, 45);
			BSRedTempRadio.Margin = new Padding(4, 3, 4, 3);
			BSRedTempRadio.Name = "BSRedTempRadio";
			BSRedTempRadio.Size = new System.Drawing.Size(54, 19);
			BSRedTempRadio.TabIndex = 0;
			BSRedTempRadio.Text = "Temp";
			BSRedTempRadio.UseVisualStyleBackColor = true;
			// 
			// BSRedFromlistRadio
			// 
			BSRedFromlistRadio.AutoSize = true;
			BSRedFromlistRadio.Checked = true;
			BSRedFromlistRadio.Location = new System.Drawing.Point(5, 23);
			BSRedFromlistRadio.Margin = new Padding(4, 3, 4, 3);
			BSRedFromlistRadio.Name = "BSRedFromlistRadio";
			BSRedFromlistRadio.Size = new System.Drawing.Size(71, 19);
			BSRedFromlistRadio.TabIndex = 0;
			BSRedFromlistRadio.TabStop = true;
			BSRedFromlistRadio.Text = "From list";
			BSRedFromlistRadio.UseVisualStyleBackColor = true;
			// 
			// radioButton6
			// 
			radioButton6.AutoSize = true;
			radioButton6.Location = new System.Drawing.Point(6, 1);
			radioButton6.Margin = new Padding(4, 3, 4, 3);
			radioButton6.Name = "radioButton6";
			radioButton6.Size = new System.Drawing.Size(14, 13);
			radioButton6.TabIndex = 0;
			radioButton6.UseVisualStyleBackColor = true;
			// 
			// panel3
			// 
			panel3.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
			panel3.Controls.Add(BlueTempRadio);
			panel3.Controls.Add(BSBlueFromListRadio);
			panel3.Controls.Add(BSBlueSectormapRadio);
			panel3.Location = new System.Drawing.Point(2, 110);
			panel3.Margin = new Padding(4, 3, 4, 3);
			panel3.Name = "panel3";
			panel3.Size = new System.Drawing.Size(91, 72);
			panel3.TabIndex = 90;
			// 
			// BlueTempRadio
			// 
			BlueTempRadio.AutoSize = true;
			BlueTempRadio.Location = new System.Drawing.Point(5, 45);
			BlueTempRadio.Margin = new Padding(4, 3, 4, 3);
			BlueTempRadio.Name = "BlueTempRadio";
			BlueTempRadio.Size = new System.Drawing.Size(54, 19);
			BlueTempRadio.TabIndex = 0;
			BlueTempRadio.Text = "Temp";
			BlueTempRadio.UseVisualStyleBackColor = true;
			BlueTempRadio.CheckedChanged += BlueTempRadio_CheckedChanged;
			// 
			// BSBlueFromListRadio
			// 
			BSBlueFromListRadio.AutoSize = true;
			BSBlueFromListRadio.Checked = true;
			BSBlueFromListRadio.Location = new System.Drawing.Point(5, 23);
			BSBlueFromListRadio.Margin = new Padding(4, 3, 4, 3);
			BSBlueFromListRadio.Name = "BSBlueFromListRadio";
			BSBlueFromListRadio.Size = new System.Drawing.Size(71, 19);
			BSBlueFromListRadio.TabIndex = 0;
			BSBlueFromListRadio.TabStop = true;
			BSBlueFromListRadio.Text = "From list";
			BSBlueFromListRadio.UseVisualStyleBackColor = true;
			BSBlueFromListRadio.CheckedChanged += BSBlueFromListRadio_CheckedChanged;
			// 
			// BSBlueSectormapRadio
			// 
			BSBlueSectormapRadio.AutoSize = true;
			BSBlueSectormapRadio.Location = new System.Drawing.Point(6, 1);
			BSBlueSectormapRadio.Margin = new Padding(4, 3, 4, 3);
			BSBlueSectormapRadio.Name = "BSBlueSectormapRadio";
			BSBlueSectormapRadio.Size = new System.Drawing.Size(82, 19);
			BSBlueSectormapRadio.TabIndex = 0;
			BSBlueSectormapRadio.Text = "Sectormap";
			BSBlueSectormapRadio.UseVisualStyleBackColor = true;
			BSBlueSectormapRadio.CheckedChanged += BSBlueSectormapRadio_CheckedChanged;
			// 
			// label54
			// 
			label54.AutoSize = true;
			label54.Location = new System.Drawing.Point(126, 76);
			label54.Margin = new Padding(4, 0, 4, 0);
			label54.Name = "label54";
			label54.Size = new System.Drawing.Size(27, 15);
			label54.TabIndex = 89;
			label54.Text = "Red";
			// 
			// label53
			// 
			label53.AutoSize = true;
			label53.Location = new System.Drawing.Point(20, 76);
			label53.Margin = new Padding(4, 0, 4, 0);
			label53.Name = "label53";
			label53.Size = new System.Drawing.Size(30, 15);
			label53.TabIndex = 88;
			label53.Text = "Blue";
			// 
			// BadSectorListBox
			// 
			BadSectorListBox.FormattingEnabled = true;
			BadSectorListBox.ItemHeight = 15;
			BadSectorListBox.Location = new System.Drawing.Point(340, 29);
			BadSectorListBox.Margin = new Padding(4, 3, 4, 3);
			BadSectorListBox.Name = "BadSectorListBox";
			BadSectorListBox.ScrollAlwaysVisible = true;
			BadSectorListBox.SelectionMode = SelectionMode.MultiExtended;
			BadSectorListBox.Size = new System.Drawing.Size(156, 289);
			BadSectorListBox.TabIndex = 87;
			BadSectorListBox.SelectedIndexChanged += BadSectorListBox_SelectedIndexChanged;
			BadSectorListBox.KeyDown += BadSectorListBox_KeyDown;
			// 
			// Sector2UpDown
			// 
			Sector2UpDown.Location = new System.Drawing.Point(273, 40);
			Sector2UpDown.Margin = new Padding(4, 3, 4, 3);
			Sector2UpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			Sector2UpDown.Name = "Sector2UpDown";
			Sector2UpDown.Size = new System.Drawing.Size(46, 23);
			Sector2UpDown.TabIndex = 84;
			Sector2UpDown.Value = new decimal(new int[] { 9, 0, 0, 0 });
			// 
			// Track2UpDown
			// 
			Track2UpDown.Location = new System.Drawing.Point(273, 10);
			Track2UpDown.Margin = new Padding(4, 3, 4, 3);
			Track2UpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			Track2UpDown.Name = "Track2UpDown";
			Track2UpDown.Size = new System.Drawing.Size(46, 23);
			Track2UpDown.TabIndex = 82;
			Track2UpDown.Value = new decimal(new int[] { 160, 0, 0, 0 });
			// 
			// label48
			// 
			label48.AutoSize = true;
			label48.Location = new System.Drawing.Point(222, 43);
			label48.Margin = new Padding(4, 0, 4, 0);
			label48.Name = "label48";
			label48.Size = new System.Drawing.Size(46, 15);
			label48.TabIndex = 83;
			label48.Text = "Sector2";
			// 
			// label49
			// 
			label49.AutoSize = true;
			label49.Location = new System.Drawing.Point(225, 13);
			label49.Margin = new Padding(4, 0, 4, 0);
			label49.Name = "label49";
			label49.Size = new System.Drawing.Size(40, 15);
			label49.TabIndex = 84;
			label49.Text = "Track2";
			// 
			// Sector1UpDown
			// 
			Sector1UpDown.Location = new System.Drawing.Point(155, 43);
			Sector1UpDown.Margin = new Padding(4, 3, 4, 3);
			Sector1UpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			Sector1UpDown.Name = "Sector1UpDown";
			Sector1UpDown.Size = new System.Drawing.Size(46, 23);
			Sector1UpDown.TabIndex = 83;
			Sector1UpDown.ValueChanged += Sector1UpDown_ValueChanged;
			// 
			// Track1UpDown
			// 
			Track1UpDown.Location = new System.Drawing.Point(155, 13);
			Track1UpDown.Margin = new Padding(4, 3, 4, 3);
			Track1UpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			Track1UpDown.Name = "Track1UpDown";
			Track1UpDown.Size = new System.Drawing.Size(46, 23);
			Track1UpDown.TabIndex = 81;
			Track1UpDown.ValueChanged += Track1UpDown_ValueChanged;
			// 
			// BlueCrcCheckLabel
			// 
			BlueCrcCheckLabel.AutoSize = true;
			BlueCrcCheckLabel.Location = new System.Drawing.Point(5, 91);
			BlueCrcCheckLabel.Margin = new Padding(4, 0, 4, 0);
			BlueCrcCheckLabel.Name = "BlueCrcCheckLabel";
			BlueCrcCheckLabel.Size = new System.Drawing.Size(28, 15);
			BlueCrcCheckLabel.TabIndex = 79;
			BlueCrcCheckLabel.Text = "Crc:";
			// 
			// label47
			// 
			label47.AutoSize = true;
			label47.Location = new System.Drawing.Point(107, 15);
			label47.Margin = new Padding(4, 0, 4, 0);
			label47.Name = "label47";
			label47.Size = new System.Drawing.Size(40, 15);
			label47.TabIndex = 80;
			label47.Text = "Track1";
			// 
			// ECSectorOverlayBtn
			// 
			ECSectorOverlayBtn.Location = new System.Drawing.Point(7, 7);
			ECSectorOverlayBtn.Margin = new Padding(4, 3, 4, 3);
			ECSectorOverlayBtn.Name = "ECSectorOverlayBtn";
			ECSectorOverlayBtn.Size = new System.Drawing.Size(78, 44);
			ECSectorOverlayBtn.TabIndex = 12;
			ECSectorOverlayBtn.Text = "Sector overlay";
			ECSectorOverlayBtn.UseVisualStyleBackColor = true;
			ECSectorOverlayBtn.Click += ECSectorOverlayBtn_Click;
			// 
			// BadSectorPanel
			// 
			BadSectorPanel.Controls.Add(BadSectorTooltip);
			BadSectorPanel.Location = new System.Drawing.Point(502, 7);
			BadSectorPanel.Margin = new Padding(4, 3, 4, 3);
			BadSectorPanel.Name = "BadSectorPanel";
			BadSectorPanel.Size = new System.Drawing.Size(604, 312);
			BadSectorPanel.TabIndex = 0;
			BadSectorPanel.Paint += BadSectorPictureBox_Paint;
			BadSectorPanel.MouseDown += BadSectorPanel_MouseDown;
			BadSectorPanel.MouseLeave += BadSectorPanel_MouseLeave;
			BadSectorPanel.MouseHover += BadSectorPanel_MouseHover;
			BadSectorPanel.MouseMove += BadSectorPanel_MouseMove;
			// 
			// BadSectorTooltip
			// 
			BadSectorTooltip.AutoSize = true;
			BadSectorTooltip.BackColor = System.Drawing.Color.White;
			BadSectorTooltip.Location = new System.Drawing.Point(562, 297);
			BadSectorTooltip.Margin = new Padding(4, 0, 4, 0);
			BadSectorTooltip.Name = "BadSectorTooltip";
			BadSectorTooltip.Size = new System.Drawing.Size(35, 15);
			BadSectorTooltip.TabIndex = 88;
			BadSectorTooltip.Text = "Label";
			// 
			// ScatterOffsetTrackBar
			// 
			ScatterOffsetTrackBar.LargeChange = 25;
			ScatterOffsetTrackBar.Location = new System.Drawing.Point(722, 651);
			ScatterOffsetTrackBar.Margin = new Padding(4, 3, 4, 3);
			ScatterOffsetTrackBar.Maximum = 4000;
			ScatterOffsetTrackBar.Minimum = -4000;
			ScatterOffsetTrackBar.Name = "ScatterOffsetTrackBar";
			ScatterOffsetTrackBar.Size = new System.Drawing.Size(401, 45);
			ScatterOffsetTrackBar.TabIndex = 104;
			ScatterOffsetTrackBar.TickFrequency = 2000;
			ScatterOffsetTrackBar.TickStyle = TickStyle.TopLeft;
			ScatterOffsetTrackBar.Scroll += BadSectorListBox_SelectedIndexChanged;
			// 
			// AnalysisPage
			// 
			AnalysisPage.BackColor = System.Drawing.SystemColors.Control;
			AnalysisPage.Controls.Add(button20);
			AnalysisPage.Controls.Add(groupBox8);
			AnalysisPage.Controls.Add(AntxtBox);
			AnalysisPage.Controls.Add(button25);
			AnalysisPage.Controls.Add(button26);
			AnalysisPage.Controls.Add(button23);
			AnalysisPage.Controls.Add(button21);
			AnalysisPage.Controls.Add(tbMFM);
			AnalysisPage.Controls.Add(button2);
			AnalysisPage.Controls.Add(ConvertToMFMBtn);
			AnalysisPage.Controls.Add(tbBIN);
			AnalysisPage.Controls.Add(tbTest);
			AnalysisPage.ImageIndex = 5;
			AnalysisPage.Location = new System.Drawing.Point(4, 24);
			AnalysisPage.Margin = new Padding(4, 3, 4, 3);
			AnalysisPage.Name = "AnalysisPage";
			AnalysisPage.Padding = new Padding(4, 3, 4, 3);
			AnalysisPage.Size = new System.Drawing.Size(1128, 897);
			AnalysisPage.TabIndex = 3;
			AnalysisPage.Text = "Analysis";
			// 
			// button20
			// 
			button20.Location = new System.Drawing.Point(10, 241);
			button20.Margin = new Padding(4, 3, 4, 3);
			button20.Name = "button20";
			button20.Size = new System.Drawing.Size(83, 44);
			button20.TabIndex = 68;
			button20.Text = "Amiga Checksum";
			button20.UseVisualStyleBackColor = true;
			button20.Click += button20_Click_1;
			// 
			// groupBox8
			// 
			groupBox8.Controls.Add(AmigaMFMRadio);
			groupBox8.Controls.Add(ANAmigaDiskSpareRadio);
			groupBox8.Controls.Add(ANAmigaRadio);
			groupBox8.Controls.Add(ANPCRadio);
			groupBox8.Location = new System.Drawing.Point(10, 387);
			groupBox8.Margin = new Padding(4, 3, 4, 3);
			groupBox8.Name = "groupBox8";
			groupBox8.Padding = new Padding(4, 3, 4, 3);
			groupBox8.Size = new System.Drawing.Size(192, 136);
			groupBox8.TabIndex = 67;
			groupBox8.TabStop = false;
			groupBox8.Text = "Platform";
			// 
			// AmigaMFMRadio
			// 
			AmigaMFMRadio.AutoSize = true;
			AmigaMFMRadio.Checked = true;
			AmigaMFMRadio.Location = new System.Drawing.Point(7, 72);
			AmigaMFMRadio.Margin = new Padding(4, 3, 4, 3);
			AmigaMFMRadio.Name = "AmigaMFMRadio";
			AmigaMFMRadio.Size = new System.Drawing.Size(88, 19);
			AmigaMFMRadio.TabIndex = 3;
			AmigaMFMRadio.TabStop = true;
			AmigaMFMRadio.Text = "AmigaMFM";
			AmigaMFMRadio.UseVisualStyleBackColor = true;
			// 
			// ANAmigaDiskSpareRadio
			// 
			ANAmigaDiskSpareRadio.AutoSize = true;
			ANAmigaDiskSpareRadio.Location = new System.Drawing.Point(7, 98);
			ANAmigaDiskSpareRadio.Margin = new Padding(4, 3, 4, 3);
			ANAmigaDiskSpareRadio.Name = "ANAmigaDiskSpareRadio";
			ANAmigaDiskSpareRadio.Size = new System.Drawing.Size(74, 19);
			ANAmigaDiskSpareRadio.TabIndex = 2;
			ANAmigaDiskSpareRadio.Text = "AmigaDS";
			ANAmigaDiskSpareRadio.UseVisualStyleBackColor = true;
			// 
			// ANAmigaRadio
			// 
			ANAmigaRadio.AutoSize = true;
			ANAmigaRadio.Location = new System.Drawing.Point(7, 45);
			ANAmigaRadio.Margin = new Padding(4, 3, 4, 3);
			ANAmigaRadio.Name = "ANAmigaRadio";
			ANAmigaRadio.Size = new System.Drawing.Size(133, 19);
			ANAmigaRadio.TabIndex = 1;
			ANAmigaRadio.Text = "HexEncMFMToBytes";
			ANAmigaRadio.UseVisualStyleBackColor = true;
			// 
			// ANPCRadio
			// 
			ANPCRadio.AutoSize = true;
			ANPCRadio.Location = new System.Drawing.Point(7, 18);
			ANPCRadio.Margin = new Padding(4, 3, 4, 3);
			ANPCRadio.Name = "ANPCRadio";
			ANPCRadio.Size = new System.Drawing.Size(40, 19);
			ANPCRadio.TabIndex = 0;
			ANPCRadio.Text = "PC";
			ANPCRadio.UseVisualStyleBackColor = true;
			// 
			// AntxtBox
			// 
			AntxtBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			AntxtBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			AntxtBox.Location = new System.Drawing.Point(685, 387);
			AntxtBox.Margin = new Padding(4, 3, 4, 3);
			AntxtBox.MaxLength = 200000;
			AntxtBox.Multiline = true;
			AntxtBox.Name = "AntxtBox";
			AntxtBox.ScrollBars = ScrollBars.Vertical;
			AntxtBox.Size = new System.Drawing.Size(454, 373);
			AntxtBox.TabIndex = 66;
			// 
			// button25
			// 
			button25.Location = new System.Drawing.Point(10, 178);
			button25.Margin = new Padding(4, 3, 4, 3);
			button25.Name = "button25";
			button25.Size = new System.Drawing.Size(83, 44);
			button25.TabIndex = 65;
			button25.Text = "Good Hex CRC check";
			button25.UseVisualStyleBackColor = true;
			button25.Click += button25_Click;
			// 
			// button26
			// 
			button26.Location = new System.Drawing.Point(10, 127);
			button26.Margin = new Padding(4, 3, 4, 3);
			button26.Name = "button26";
			button26.Size = new System.Drawing.Size(83, 44);
			button26.TabIndex = 64;
			button26.Text = "Good Ascii CRC check";
			button26.UseVisualStyleBackColor = true;
			button26.Click += button26_Click;
			// 
			// button23
			// 
			button23.Location = new System.Drawing.Point(97, 58);
			button23.Margin = new Padding(4, 3, 4, 3);
			button23.Name = "button23";
			button23.Size = new System.Drawing.Size(83, 44);
			button23.TabIndex = 63;
			button23.Text = "Hex CRC check";
			button23.UseVisualStyleBackColor = true;
			button23.Click += button23_Click;
			// 
			// button21
			// 
			button21.Location = new System.Drawing.Point(97, 7);
			button21.Margin = new Padding(4, 3, 4, 3);
			button21.Name = "button21";
			button21.Size = new System.Drawing.Size(83, 44);
			button21.TabIndex = 62;
			button21.Text = "Ascii CRC check";
			button21.UseVisualStyleBackColor = true;
			button21.Click += AsciiCrcCheckBtn_Click;
			// 
			// tbMFM
			// 
			tbMFM.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbMFM.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			tbMFM.Location = new System.Drawing.Point(223, 388);
			tbMFM.Margin = new Padding(4, 3, 4, 3);
			tbMFM.MaxLength = 200000;
			tbMFM.Multiline = true;
			tbMFM.Name = "tbMFM";
			tbMFM.ScrollBars = ScrollBars.Vertical;
			tbMFM.Size = new System.Drawing.Size(454, 366);
			tbMFM.TabIndex = 61;
			// 
			// button2
			// 
			button2.Location = new System.Drawing.Point(7, 58);
			button2.Margin = new Padding(4, 3, 4, 3);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(83, 44);
			button2.TabIndex = 60;
			button2.Text = "Convert to BIN";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// ConvertToMFMBtn
			// 
			ConvertToMFMBtn.Location = new System.Drawing.Point(7, 7);
			ConvertToMFMBtn.Margin = new Padding(4, 3, 4, 3);
			ConvertToMFMBtn.Name = "ConvertToMFMBtn";
			ConvertToMFMBtn.Size = new System.Drawing.Size(83, 44);
			ConvertToMFMBtn.TabIndex = 59;
			ConvertToMFMBtn.Text = "Convert to MFM";
			ConvertToMFMBtn.UseVisualStyleBackColor = true;
			ConvertToMFMBtn.Click += ConvertToMFMBtn_Click;
			// 
			// tbBIN
			// 
			tbBIN.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbBIN.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			tbBIN.Location = new System.Drawing.Point(223, 7);
			tbBIN.Margin = new Padding(4, 3, 4, 3);
			tbBIN.MaxLength = 200000;
			tbBIN.Multiline = true;
			tbBIN.Name = "tbBIN";
			tbBIN.ScrollBars = ScrollBars.Vertical;
			tbBIN.Size = new System.Drawing.Size(454, 373);
			tbBIN.TabIndex = 58;
			tbBIN.Text = "Test";
			// 
			// tbTest
			// 
			tbTest.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbTest.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			tbTest.Location = new System.Drawing.Point(682, 7);
			tbTest.Margin = new Padding(4, 3, 4, 3);
			tbTest.MaxLength = 200000;
			tbTest.Multiline = true;
			tbTest.Name = "tbTest";
			tbTest.ScrollBars = ScrollBars.Vertical;
			tbTest.Size = new System.Drawing.Size(454, 373);
			tbTest.TabIndex = 57;
			// 
			// AnalysisTab2
			// 
			AnalysisTab2.BackColor = System.Drawing.SystemColors.Control;
			AnalysisTab2.Controls.Add(rxbufOffsetLabel);
			AnalysisTab2.Controls.Add(label80);
			AnalysisTab2.Controls.Add(ThresholdTestUpDown);
			AnalysisTab2.Controls.Add(DiffTest2UpDown);
			AnalysisTab2.Controls.Add(DiffTestUpDown);
			AnalysisTab2.Controls.Add(button34);
			AnalysisTab2.Controls.Add(AnAutoUpdateCheckBox);
			AnalysisTab2.Controls.Add(button31);
			AnalysisTab2.Controls.Add(button3);
			AnalysisTab2.Controls.Add(label73);
			AnalysisTab2.Controls.Add(PeriodExtendUpDown);
			AnalysisTab2.Controls.Add(EditOptioncomboBox);
			AnalysisTab2.Controls.Add(EditModecomboBox);
			AnalysisTab2.Controls.Add(HighpassThresholdUpDown);
			AnalysisTab2.Controls.Add(button33);
			AnalysisTab2.Controls.Add(Undolevelslabel);
			AnalysisTab2.Controls.Add(Lowpassbutton);
			AnalysisTab2.Controls.Add(DCOffsetbutton);
			AnalysisTab2.Controls.Add(label70);
			AnalysisTab2.Controls.Add(button32);
			AnalysisTab2.Controls.Add(SaveWaveformButton);
			AnalysisTab2.Controls.Add(EditUndobutton);
			AnalysisTab2.Controls.Add(AdaptLookAheadUpDown);
			AnalysisTab2.Controls.Add(DiffMinDeviation2UpDown);
			AnalysisTab2.Controls.Add(button18);
			AnalysisTab2.Controls.Add(DiffOffsetUpDown);
			AnalysisTab2.Controls.Add(InvertcheckBox);
			AnalysisTab2.Controls.Add(AdaptiveGaincheckBox);
			AnalysisTab2.Controls.Add(SignalRatioDistUpDown);
			AnalysisTab2.Controls.Add(DiffMinDeviationUpDown);
			AnalysisTab2.Controls.Add(SmoothingUpDown);
			AnalysisTab2.Controls.Add(AnDensityUpDown);
			AnalysisTab2.Controls.Add(AnReplacerxbufBox);
			AnalysisTab2.Controls.Add(button19);
			AnalysisTab2.Controls.Add(DiffDistUpDown2);
			AnalysisTab2.Controls.Add(label62);
			AnalysisTab2.Controls.Add(DiffThresholdUpDown);
			AnalysisTab2.Controls.Add(label61);
			AnalysisTab2.Controls.Add(DiffGainUpDown);
			AnalysisTab2.Controls.Add(label60);
			AnalysisTab2.Controls.Add(DiffDistUpDown);
			AnalysisTab2.Controls.Add(label52);
			AnalysisTab2.Controls.Add(GraphFilterButton);
			AnalysisTab2.Controls.Add(GraphLengthLabel);
			AnalysisTab2.Controls.Add(GraphXOffsetLabel);
			AnalysisTab2.Controls.Add(GraphYOffsetlabel);
			AnalysisTab2.Controls.Add(GraphScaleYLabel);
			AnalysisTab2.Controls.Add(OpenWavefrmbutton);
			AnalysisTab2.Controls.Add(groupBox9);
			AnalysisTab2.Controls.Add(label19);
			AnalysisTab2.Controls.Add(label4);
			AnalysisTab2.Controls.Add(label3);
			AnalysisTab2.Controls.Add(label51);
			AnalysisTab2.Controls.Add(GraphYScaleTrackBar);
			AnalysisTab2.Controls.Add(GraphOffsetTrackBar);
			AnalysisTab2.Controls.Add(GraphPictureBox);
			AnalysisTab2.ImageIndex = 4;
			AnalysisTab2.Location = new System.Drawing.Point(4, 24);
			AnalysisTab2.Margin = new Padding(4, 3, 4, 3);
			AnalysisTab2.Name = "AnalysisTab2";
			AnalysisTab2.Padding = new Padding(4, 3, 4, 3);
			AnalysisTab2.Size = new System.Drawing.Size(1128, 897);
			AnalysisTab2.TabIndex = 4;
			AnalysisTab2.Text = "Waveform Editor";
			AnalysisTab2.Enter += AnalysisTab2_Enter_1;
			// 
			// rxbufOffsetLabel
			// 
			rxbufOffsetLabel.AutoSize = true;
			rxbufOffsetLabel.BackColor = System.Drawing.Color.Transparent;
			rxbufOffsetLabel.Location = new System.Drawing.Point(989, 141);
			rxbufOffsetLabel.Margin = new Padding(4, 0, 4, 0);
			rxbufOffsetLabel.Name = "rxbufOffsetLabel";
			rxbufOffsetLabel.Size = new System.Drawing.Size(13, 15);
			rxbufOffsetLabel.TabIndex = 166;
			rxbufOffsetLabel.Text = "0";
			// 
			// label80
			// 
			label80.AutoSize = true;
			label80.BackColor = System.Drawing.Color.Transparent;
			label80.Location = new System.Drawing.Point(930, 141);
			label80.Margin = new Padding(4, 0, 4, 0);
			label80.Name = "label80";
			label80.Size = new System.Drawing.Size(57, 15);
			label80.TabIndex = 165;
			label80.Text = "rxb offset";
			// 
			// ThresholdTestUpDown
			// 
			ThresholdTestUpDown.Location = new System.Drawing.Point(236, 10);
			ThresholdTestUpDown.Margin = new Padding(4, 3, 4, 3);
			ThresholdTestUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			ThresholdTestUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			ThresholdTestUpDown.Name = "ThresholdTestUpDown";
			ThresholdTestUpDown.Size = new System.Drawing.Size(56, 23);
			ThresholdTestUpDown.TabIndex = 164;
			ThresholdTestUpDown.Value = new decimal(new int[] { 15, 0, 0, 0 });
			// 
			// DiffTest2UpDown
			// 
			DiffTest2UpDown.Location = new System.Drawing.Point(298, 35);
			DiffTest2UpDown.Margin = new Padding(4, 3, 4, 3);
			DiffTest2UpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			DiffTest2UpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			DiffTest2UpDown.Name = "DiffTest2UpDown";
			DiffTest2UpDown.Size = new System.Drawing.Size(56, 23);
			DiffTest2UpDown.TabIndex = 163;
			DiffTest2UpDown.Value = new decimal(new int[] { 21, 0, 0, 0 });
			// 
			// DiffTestUpDown
			// 
			DiffTestUpDown.Location = new System.Drawing.Point(298, 10);
			DiffTestUpDown.Margin = new Padding(4, 3, 4, 3);
			DiffTestUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			DiffTestUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			DiffTestUpDown.Name = "DiffTestUpDown";
			DiffTestUpDown.Size = new System.Drawing.Size(56, 23);
			DiffTestUpDown.TabIndex = 162;
			DiffTestUpDown.Value = new decimal(new int[] { 21, 0, 0, 0 });
			// 
			// button34
			// 
			button34.Location = new System.Drawing.Point(789, 126);
			button34.Margin = new Padding(4, 3, 4, 3);
			button34.Name = "button34";
			button34.Size = new System.Drawing.Size(79, 27);
			button34.TabIndex = 161;
			button34.Text = "Fix8us";
			button34.UseVisualStyleBackColor = true;
			button34.Click += Button34_Click;
			// 
			// AnAutoUpdateCheckBox
			// 
			AnAutoUpdateCheckBox.AutoSize = true;
			AnAutoUpdateCheckBox.Checked = true;
			AnAutoUpdateCheckBox.CheckState = CheckState.Checked;
			AnAutoUpdateCheckBox.Location = new System.Drawing.Point(236, 132);
			AnAutoUpdateCheckBox.Margin = new Padding(4, 3, 4, 3);
			AnAutoUpdateCheckBox.Name = "AnAutoUpdateCheckBox";
			AnAutoUpdateCheckBox.Size = new System.Drawing.Size(92, 19);
			AnAutoUpdateCheckBox.TabIndex = 160;
			AnAutoUpdateCheckBox.Text = "Auto update";
			AnAutoUpdateCheckBox.UseVisualStyleBackColor = true;
			// 
			// button31
			// 
			button31.Location = new System.Drawing.Point(7, 129);
			button31.Margin = new Padding(4, 3, 4, 3);
			button31.Name = "button31";
			button31.Size = new System.Drawing.Size(57, 27);
			button31.TabIndex = 159;
			button31.Text = "Filter2";
			button31.UseVisualStyleBackColor = true;
			button31.Click += Button31_Click_2;
			// 
			// button3
			// 
			button3.Location = new System.Drawing.Point(961, 50);
			button3.Margin = new Padding(4, 3, 4, 3);
			button3.Name = "button3";
			button3.Size = new System.Drawing.Size(79, 27);
			button3.TabIndex = 158;
			button3.Text = "Lowpass2";
			button3.UseVisualStyleBackColor = true;
			button3.Click += Button3_Click;
			// 
			// label73
			// 
			label73.AutoSize = true;
			label73.BackColor = System.Drawing.Color.Transparent;
			label73.Location = new System.Drawing.Point(632, 111);
			label73.Margin = new Padding(4, 0, 4, 0);
			label73.Name = "label73";
			label73.Size = new System.Drawing.Size(47, 15);
			label73.TabIndex = 157;
			label73.Text = "periodx";
			// 
			// PeriodExtendUpDown
			// 
			PeriodExtendUpDown.Location = new System.Drawing.Point(636, 130);
			PeriodExtendUpDown.Margin = new Padding(4, 3, 4, 3);
			PeriodExtendUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			PeriodExtendUpDown.Minimum = new decimal(new int[] { 2000, 0, 0, int.MinValue });
			PeriodExtendUpDown.Name = "PeriodExtendUpDown";
			PeriodExtendUpDown.Size = new System.Drawing.Size(56, 23);
			PeriodExtendUpDown.TabIndex = 156;
			PeriodExtendUpDown.Value = new decimal(new int[] { 5, 0, 0, 0 });
			// 
			// EditOptioncomboBox
			// 
			EditOptioncomboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			EditOptioncomboBox.FormattingEnabled = true;
			EditOptioncomboBox.Items.AddRange(new object[] { "4us", "6us", "8us" });
			EditOptioncomboBox.Location = new System.Drawing.Point(570, 75);
			EditOptioncomboBox.Margin = new Padding(4, 3, 4, 3);
			EditOptioncomboBox.Name = "EditOptioncomboBox";
			EditOptioncomboBox.Size = new System.Drawing.Size(87, 23);
			EditOptioncomboBox.TabIndex = 155;
			// 
			// EditModecomboBox
			// 
			EditModecomboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			EditModecomboBox.FormattingEnabled = true;
			EditModecomboBox.Items.AddRange(new object[] { "Edit manually", "Edit fixd" });
			EditModecomboBox.Location = new System.Drawing.Point(476, 75);
			EditModecomboBox.Margin = new Padding(4, 3, 4, 3);
			EditModecomboBox.Name = "EditModecomboBox";
			EditModecomboBox.Size = new System.Drawing.Size(87, 23);
			EditModecomboBox.TabIndex = 154;
			// 
			// HighpassThresholdUpDown
			// 
			HighpassThresholdUpDown.Location = new System.Drawing.Point(875, 70);
			HighpassThresholdUpDown.Margin = new Padding(4, 3, 4, 3);
			HighpassThresholdUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			HighpassThresholdUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			HighpassThresholdUpDown.Name = "HighpassThresholdUpDown";
			HighpassThresholdUpDown.Size = new System.Drawing.Size(56, 23);
			HighpassThresholdUpDown.TabIndex = 153;
			HighpassThresholdUpDown.Value = new decimal(new int[] { 33, 0, 0, 0 });
			// 
			// button33
			// 
			button33.Location = new System.Drawing.Point(789, 70);
			button33.Margin = new Padding(4, 3, 4, 3);
			button33.Name = "button33";
			button33.Size = new System.Drawing.Size(79, 27);
			button33.TabIndex = 152;
			button33.Text = "Highpass";
			button33.UseVisualStyleBackColor = true;
			button33.Click += Button33_Click_1;
			// 
			// Undolevelslabel
			// 
			Undolevelslabel.AutoSize = true;
			Undolevelslabel.BackColor = System.Drawing.Color.Transparent;
			Undolevelslabel.Location = new System.Drawing.Point(536, 107);
			Undolevelslabel.Margin = new Padding(4, 0, 4, 0);
			Undolevelslabel.Name = "Undolevelslabel";
			Undolevelslabel.Size = new System.Drawing.Size(80, 15);
			Undolevelslabel.TabIndex = 151;
			Undolevelslabel.Text = "Undo levels: 0";
			// 
			// Lowpassbutton
			// 
			Lowpassbutton.Location = new System.Drawing.Point(702, 69);
			Lowpassbutton.Margin = new Padding(4, 3, 4, 3);
			Lowpassbutton.Name = "Lowpassbutton";
			Lowpassbutton.Size = new System.Drawing.Size(79, 27);
			Lowpassbutton.TabIndex = 150;
			Lowpassbutton.Text = "Lowpass";
			Lowpassbutton.UseVisualStyleBackColor = true;
			Lowpassbutton.Click += Lowpassbutton_Click;
			// 
			// DCOffsetbutton
			// 
			DCOffsetbutton.Location = new System.Drawing.Point(702, 97);
			DCOffsetbutton.Margin = new Padding(4, 3, 4, 3);
			DCOffsetbutton.Name = "DCOffsetbutton";
			DCOffsetbutton.Size = new System.Drawing.Size(79, 27);
			DCOffsetbutton.TabIndex = 149;
			DCOffsetbutton.Text = "DC offset";
			DCOffsetbutton.UseVisualStyleBackColor = true;
			DCOffsetbutton.Click += Button33_Click;
			// 
			// label70
			// 
			label70.AutoSize = true;
			label70.Location = new System.Drawing.Point(181, 132);
			label70.Margin = new Padding(4, 0, 4, 0);
			label70.Name = "label70";
			label70.Size = new System.Drawing.Size(50, 15);
			label70.TabIndex = 148;
			label70.Text = "A.Adapt";
			// 
			// button32
			// 
			button32.Location = new System.Drawing.Point(702, 125);
			button32.Margin = new Padding(4, 3, 4, 3);
			button32.Name = "button32";
			button32.Size = new System.Drawing.Size(79, 27);
			button32.TabIndex = 147;
			button32.Text = "Copy G0";
			button32.UseVisualStyleBackColor = true;
			button32.Click += Button32_Click;
			// 
			// SaveWaveformButton
			// 
			SaveWaveformButton.Location = new System.Drawing.Point(7, 58);
			SaveWaveformButton.Margin = new Padding(4, 3, 4, 3);
			SaveWaveformButton.Name = "SaveWaveformButton";
			SaveWaveformButton.Size = new System.Drawing.Size(57, 45);
			SaveWaveformButton.TabIndex = 146;
			SaveWaveformButton.Text = "Save";
			SaveWaveformButton.UseVisualStyleBackColor = true;
			SaveWaveformButton.Click += SaveWaveformButton_Click;
			// 
			// EditUndobutton
			// 
			EditUndobutton.Location = new System.Drawing.Point(541, 125);
			EditUndobutton.Margin = new Padding(4, 3, 4, 3);
			EditUndobutton.Name = "EditUndobutton";
			EditUndobutton.Size = new System.Drawing.Size(50, 27);
			EditUndobutton.TabIndex = 145;
			EditUndobutton.Text = "Undo";
			EditUndobutton.UseVisualStyleBackColor = true;
			EditUndobutton.Click += Button31_Click_1;
			// 
			// AdaptLookAheadUpDown
			// 
			AdaptLookAheadUpDown.Increment = new decimal(new int[] { 10, 0, 0, 0 });
			AdaptLookAheadUpDown.Location = new System.Drawing.Point(298, 100);
			AdaptLookAheadUpDown.Margin = new Padding(4, 3, 4, 3);
			AdaptLookAheadUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			AdaptLookAheadUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			AdaptLookAheadUpDown.Name = "AdaptLookAheadUpDown";
			AdaptLookAheadUpDown.Size = new System.Drawing.Size(56, 23);
			AdaptLookAheadUpDown.TabIndex = 144;
			AdaptLookAheadUpDown.Value = new decimal(new int[] { 100, 0, 0, 0 });
			AdaptLookAheadUpDown.ValueChanged += AdaptLookAheadUpDown_ValueChanged;
			// 
			// DiffMinDeviation2UpDown
			// 
			DiffMinDeviation2UpDown.Location = new System.Drawing.Point(298, 69);
			DiffMinDeviation2UpDown.Margin = new Padding(4, 3, 4, 3);
			DiffMinDeviation2UpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			DiffMinDeviation2UpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			DiffMinDeviation2UpDown.Name = "DiffMinDeviation2UpDown";
			DiffMinDeviation2UpDown.Size = new System.Drawing.Size(56, 23);
			DiffMinDeviation2UpDown.TabIndex = 143;
			DiffMinDeviation2UpDown.Value = new decimal(new int[] { 21, 0, 0, 0 });
			DiffMinDeviation2UpDown.ValueChanged += DiffMinDeviation2UpDown_ValueChanged;
			// 
			// button18
			// 
			button18.Location = new System.Drawing.Point(1062, 63);
			button18.Margin = new Padding(4, 3, 4, 3);
			button18.Name = "button18";
			button18.Size = new System.Drawing.Size(57, 45);
			button18.TabIndex = 142;
			button18.Text = "Offset recalc";
			button18.UseVisualStyleBackColor = true;
			button18.Click += Button18_Click;
			// 
			// DiffOffsetUpDown
			// 
			DiffOffsetUpDown.Location = new System.Drawing.Point(478, 128);
			DiffOffsetUpDown.Margin = new Padding(4, 3, 4, 3);
			DiffOffsetUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			DiffOffsetUpDown.Minimum = new decimal(new int[] { 2000, 0, 0, int.MinValue });
			DiffOffsetUpDown.Name = "DiffOffsetUpDown";
			DiffOffsetUpDown.Size = new System.Drawing.Size(56, 23);
			DiffOffsetUpDown.TabIndex = 141;
			DiffOffsetUpDown.Value = new decimal(new int[] { 10, 0, 0, int.MinValue });
			DiffOffsetUpDown.ValueChanged += DiffOffsetUpDown_ValueChanged;
			// 
			// InvertcheckBox
			// 
			InvertcheckBox.AutoSize = true;
			InvertcheckBox.Location = new System.Drawing.Point(981, 91);
			InvertcheckBox.Margin = new Padding(4, 3, 4, 3);
			InvertcheckBox.Name = "InvertcheckBox";
			InvertcheckBox.Size = new System.Drawing.Size(56, 19);
			InvertcheckBox.TabIndex = 140;
			InvertcheckBox.Text = "Invert";
			InvertcheckBox.UseVisualStyleBackColor = true;
			// 
			// AdaptiveGaincheckBox
			// 
			AdaptiveGaincheckBox.AutoSize = true;
			AdaptiveGaincheckBox.Checked = true;
			AdaptiveGaincheckBox.CheckState = CheckState.Checked;
			AdaptiveGaincheckBox.Location = new System.Drawing.Point(981, 118);
			AdaptiveGaincheckBox.Margin = new Padding(4, 3, 4, 3);
			AdaptiveGaincheckBox.Name = "AdaptiveGaincheckBox";
			AdaptiveGaincheckBox.Size = new System.Drawing.Size(97, 19);
			AdaptiveGaincheckBox.TabIndex = 139;
			AdaptiveGaincheckBox.Text = "AdaptiveGain";
			AdaptiveGaincheckBox.UseVisualStyleBackColor = true;
			// 
			// SignalRatioDistUpDown
			// 
			SignalRatioDistUpDown.DecimalPlaces = 2;
			SignalRatioDistUpDown.Increment = new decimal(new int[] { 10, 0, 0, 0 });
			SignalRatioDistUpDown.Location = new System.Drawing.Point(236, 100);
			SignalRatioDistUpDown.Margin = new Padding(4, 3, 4, 3);
			SignalRatioDistUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			SignalRatioDistUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			SignalRatioDistUpDown.Name = "SignalRatioDistUpDown";
			SignalRatioDistUpDown.Size = new System.Drawing.Size(56, 23);
			SignalRatioDistUpDown.TabIndex = 138;
			SignalRatioDistUpDown.Value = new decimal(new int[] { 30, 0, 0, 0 });
			SignalRatioDistUpDown.ValueChanged += SignalRatioDistUpDown_ValueChanged;
			// 
			// DiffMinDeviationUpDown
			// 
			DiffMinDeviationUpDown.Location = new System.Drawing.Point(236, 69);
			DiffMinDeviationUpDown.Margin = new Padding(4, 3, 4, 3);
			DiffMinDeviationUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			DiffMinDeviationUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			DiffMinDeviationUpDown.Name = "DiffMinDeviationUpDown";
			DiffMinDeviationUpDown.Size = new System.Drawing.Size(56, 23);
			DiffMinDeviationUpDown.TabIndex = 136;
			DiffMinDeviationUpDown.Value = new decimal(new int[] { 21, 0, 0, 0 });
			DiffMinDeviationUpDown.ValueChanged += DiffMinDeviationUpDown_ValueChanged;
			// 
			// SmoothingUpDown
			// 
			SmoothingUpDown.Location = new System.Drawing.Point(173, 13);
			SmoothingUpDown.Margin = new Padding(4, 3, 4, 3);
			SmoothingUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			SmoothingUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			SmoothingUpDown.Name = "SmoothingUpDown";
			SmoothingUpDown.Size = new System.Drawing.Size(56, 23);
			SmoothingUpDown.TabIndex = 135;
			SmoothingUpDown.Value = new decimal(new int[] { 3, 0, 0, 0 });
			// 
			// AnDensityUpDown
			// 
			AnDensityUpDown.Location = new System.Drawing.Point(999, 15);
			AnDensityUpDown.Margin = new Padding(4, 3, 4, 3);
			AnDensityUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			AnDensityUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			AnDensityUpDown.Name = "AnDensityUpDown";
			AnDensityUpDown.Size = new System.Drawing.Size(56, 23);
			AnDensityUpDown.TabIndex = 134;
			AnDensityUpDown.Value = new decimal(new int[] { 23, 0, 0, 0 });
			// 
			// AnReplacerxbufBox
			// 
			AnReplacerxbufBox.AutoSize = true;
			AnReplacerxbufBox.Checked = true;
			AnReplacerxbufBox.CheckState = CheckState.Checked;
			AnReplacerxbufBox.Location = new System.Drawing.Point(76, 132);
			AnReplacerxbufBox.Margin = new Padding(4, 3, 4, 3);
			AnReplacerxbufBox.Name = "AnReplacerxbufBox";
			AnReplacerxbufBox.Size = new System.Drawing.Size(98, 19);
			AnReplacerxbufBox.TabIndex = 133;
			AnReplacerxbufBox.Text = "Replace rxbuf";
			AnReplacerxbufBox.UseVisualStyleBackColor = true;
			// 
			// button19
			// 
			button19.Location = new System.Drawing.Point(1062, 12);
			button19.Margin = new Padding(4, 3, 4, 3);
			button19.Name = "button19";
			button19.Size = new System.Drawing.Size(57, 45);
			button19.TabIndex = 131;
			button19.Text = "Proc R data";
			button19.UseVisualStyleBackColor = true;
			button19.Click += Button19_Click;
			// 
			// DiffDistUpDown2
			// 
			DiffDistUpDown2.Location = new System.Drawing.Point(236, 40);
			DiffDistUpDown2.Margin = new Padding(4, 3, 4, 3);
			DiffDistUpDown2.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			DiffDistUpDown2.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			DiffDistUpDown2.Name = "DiffDistUpDown2";
			DiffDistUpDown2.Size = new System.Drawing.Size(56, 23);
			DiffDistUpDown2.TabIndex = 130;
			DiffDistUpDown2.Value = new decimal(new int[] { 49, 0, 0, 0 });
			DiffDistUpDown2.ValueChanged += DiffDistUpDown2_ValueChanged;
			// 
			// label62
			// 
			label62.AutoSize = true;
			label62.Location = new System.Drawing.Point(85, 74);
			label62.Margin = new Padding(4, 0, 4, 0);
			label62.Name = "label62";
			label62.Size = new System.Drawing.Size(79, 15);
			label62.TabIndex = 127;
			label62.Text = "Diff threshold";
			// 
			// DiffThresholdUpDown
			// 
			DiffThresholdUpDown.Location = new System.Drawing.Point(173, 69);
			DiffThresholdUpDown.Margin = new Padding(4, 3, 4, 3);
			DiffThresholdUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			DiffThresholdUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			DiffThresholdUpDown.Name = "DiffThresholdUpDown";
			DiffThresholdUpDown.Size = new System.Drawing.Size(56, 23);
			DiffThresholdUpDown.TabIndex = 126;
			DiffThresholdUpDown.Value = new decimal(new int[] { 127, 0, 0, 0 });
			DiffThresholdUpDown.ValueChanged += NumericUpDown1_ValueChanged;
			// 
			// label61
			// 
			label61.AutoSize = true;
			label61.Location = new System.Drawing.Point(107, 104);
			label61.Margin = new Padding(4, 0, 4, 0);
			label61.Name = "label61";
			label61.Size = new System.Drawing.Size(55, 15);
			label61.TabIndex = 125;
			label61.Text = "Diff. gain";
			// 
			// DiffGainUpDown
			// 
			DiffGainUpDown.DecimalPlaces = 2;
			DiffGainUpDown.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
			DiffGainUpDown.Location = new System.Drawing.Point(173, 102);
			DiffGainUpDown.Margin = new Padding(4, 3, 4, 3);
			DiffGainUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			DiffGainUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			DiffGainUpDown.Name = "DiffGainUpDown";
			DiffGainUpDown.Size = new System.Drawing.Size(56, 23);
			DiffGainUpDown.TabIndex = 124;
			DiffGainUpDown.Value = new decimal(new int[] { 3, 0, 0, 65536 });
			DiffGainUpDown.ValueChanged += DiffGainUpDown_ValueChanged;
			// 
			// label60
			// 
			label60.AutoSize = true;
			label60.Location = new System.Drawing.Point(84, 43);
			label60.Margin = new Padding(4, 0, 4, 0);
			label60.Name = "label60";
			label60.Size = new System.Drawing.Size(76, 15);
			label60.TabIndex = 123;
			label60.Text = "Diff. distance";
			// 
			// DiffDistUpDown
			// 
			DiffDistUpDown.Location = new System.Drawing.Point(173, 40);
			DiffDistUpDown.Margin = new Padding(4, 3, 4, 3);
			DiffDistUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			DiffDistUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			DiffDistUpDown.Name = "DiffDistUpDown";
			DiffDistUpDown.Size = new System.Drawing.Size(56, 23);
			DiffDistUpDown.TabIndex = 122;
			DiffDistUpDown.Value = new decimal(new int[] { 24, 0, 0, 0 });
			DiffDistUpDown.ValueChanged += DiffDistUpDown_ValueChanged;
			// 
			// label52
			// 
			label52.AutoSize = true;
			label52.Location = new System.Drawing.Point(98, 15);
			label52.Margin = new Padding(4, 0, 4, 0);
			label52.Name = "label52";
			label52.Size = new System.Drawing.Size(66, 15);
			label52.TabIndex = 121;
			label52.Text = "Smoothing";
			// 
			// GraphFilterButton
			// 
			GraphFilterButton.Location = new System.Drawing.Point(7, 103);
			GraphFilterButton.Margin = new Padding(4, 3, 4, 3);
			GraphFilterButton.Name = "GraphFilterButton";
			GraphFilterButton.Size = new System.Drawing.Size(57, 27);
			GraphFilterButton.TabIndex = 119;
			GraphFilterButton.Text = "Filter";
			GraphFilterButton.UseVisualStyleBackColor = true;
			GraphFilterButton.Click += GraphFilterButton_Click;
			// 
			// GraphLengthLabel
			// 
			GraphLengthLabel.AutoSize = true;
			GraphLengthLabel.BackColor = System.Drawing.Color.Transparent;
			GraphLengthLabel.Location = new System.Drawing.Point(904, 128);
			GraphLengthLabel.Margin = new Padding(4, 0, 4, 0);
			GraphLengthLabel.Name = "GraphLengthLabel";
			GraphLengthLabel.Size = new System.Drawing.Size(13, 15);
			GraphLengthLabel.TabIndex = 117;
			GraphLengthLabel.Text = "0";
			// 
			// GraphXOffsetLabel
			// 
			GraphXOffsetLabel.AutoSize = true;
			GraphXOffsetLabel.BackColor = System.Drawing.Color.Transparent;
			GraphXOffsetLabel.Location = new System.Drawing.Point(903, 107);
			GraphXOffsetLabel.Margin = new Padding(4, 0, 4, 0);
			GraphXOffsetLabel.Name = "GraphXOffsetLabel";
			GraphXOffsetLabel.Size = new System.Drawing.Size(13, 15);
			GraphXOffsetLabel.TabIndex = 116;
			GraphXOffsetLabel.Text = "0";
			// 
			// GraphYOffsetlabel
			// 
			GraphYOffsetlabel.AutoSize = true;
			GraphYOffsetlabel.BackColor = System.Drawing.Color.Transparent;
			GraphYOffsetlabel.Location = new System.Drawing.Point(930, 13);
			GraphYOffsetlabel.Margin = new Padding(4, 0, 4, 0);
			GraphYOffsetlabel.Name = "GraphYOffsetlabel";
			GraphYOffsetlabel.Size = new System.Drawing.Size(13, 15);
			GraphYOffsetlabel.TabIndex = 115;
			GraphYOffsetlabel.Text = "0";
			// 
			// GraphScaleYLabel
			// 
			GraphScaleYLabel.AutoSize = true;
			GraphScaleYLabel.BackColor = System.Drawing.Color.Transparent;
			GraphScaleYLabel.Location = new System.Drawing.Point(930, 46);
			GraphScaleYLabel.Margin = new Padding(4, 0, 4, 0);
			GraphScaleYLabel.Name = "GraphScaleYLabel";
			GraphScaleYLabel.Size = new System.Drawing.Size(13, 15);
			GraphScaleYLabel.TabIndex = 118;
			GraphScaleYLabel.Text = "0";
			// 
			// OpenWavefrmbutton
			// 
			OpenWavefrmbutton.Location = new System.Drawing.Point(7, 7);
			OpenWavefrmbutton.Margin = new Padding(4, 3, 4, 3);
			OpenWavefrmbutton.Name = "OpenWavefrmbutton";
			OpenWavefrmbutton.Size = new System.Drawing.Size(57, 45);
			OpenWavefrmbutton.TabIndex = 114;
			OpenWavefrmbutton.Text = "Open";
			OpenWavefrmbutton.UseVisualStyleBackColor = true;
			OpenWavefrmbutton.Click += OpenWavefrmbutton_Click_1;
			// 
			// groupBox9
			// 
			groupBox9.Controls.Add(Graph5SelRadioButton);
			groupBox9.Controls.Add(Graph4SelRadioButton);
			groupBox9.Controls.Add(Graph3SelRadioButton);
			groupBox9.Controls.Add(Graph2SelRadioButton);
			groupBox9.Controls.Add(Graph1SelRadioButton);
			groupBox9.Location = new System.Drawing.Point(376, 6);
			groupBox9.Margin = new Padding(4, 3, 4, 3);
			groupBox9.Name = "groupBox9";
			groupBox9.Padding = new Padding(4, 3, 4, 3);
			groupBox9.Size = new System.Drawing.Size(93, 145);
			groupBox9.TabIndex = 113;
			groupBox9.TabStop = false;
			groupBox9.Text = "Graphs";
			// 
			// Graph5SelRadioButton
			// 
			Graph5SelRadioButton.AutoSize = true;
			Graph5SelRadioButton.Location = new System.Drawing.Point(7, 119);
			Graph5SelRadioButton.Margin = new Padding(4, 3, 4, 3);
			Graph5SelRadioButton.Name = "Graph5SelRadioButton";
			Graph5SelRadioButton.Size = new System.Drawing.Size(66, 19);
			Graph5SelRadioButton.TabIndex = 4;
			Graph5SelRadioButton.Text = "Graph 5";
			Graph5SelRadioButton.UseVisualStyleBackColor = true;
			Graph5SelRadioButton.Click += Graph5SelRadioButton_CheckedChanged;
			// 
			// Graph4SelRadioButton
			// 
			Graph4SelRadioButton.AutoSize = true;
			Graph4SelRadioButton.Location = new System.Drawing.Point(7, 97);
			Graph4SelRadioButton.Margin = new Padding(4, 3, 4, 3);
			Graph4SelRadioButton.Name = "Graph4SelRadioButton";
			Graph4SelRadioButton.Size = new System.Drawing.Size(66, 19);
			Graph4SelRadioButton.TabIndex = 3;
			Graph4SelRadioButton.Text = "Graph 4";
			Graph4SelRadioButton.UseVisualStyleBackColor = true;
			Graph4SelRadioButton.Click += Graph4SelRadioButton_CheckedChanged;
			// 
			// Graph3SelRadioButton
			// 
			Graph3SelRadioButton.AutoSize = true;
			Graph3SelRadioButton.Location = new System.Drawing.Point(7, 73);
			Graph3SelRadioButton.Margin = new Padding(4, 3, 4, 3);
			Graph3SelRadioButton.Name = "Graph3SelRadioButton";
			Graph3SelRadioButton.Size = new System.Drawing.Size(66, 19);
			Graph3SelRadioButton.TabIndex = 2;
			Graph3SelRadioButton.Text = "Graph 3";
			Graph3SelRadioButton.UseVisualStyleBackColor = true;
			Graph3SelRadioButton.Click += Graph3SelRadioButton_CheckedChanged;
			// 
			// Graph2SelRadioButton
			// 
			Graph2SelRadioButton.AutoSize = true;
			Graph2SelRadioButton.Location = new System.Drawing.Point(7, 47);
			Graph2SelRadioButton.Margin = new Padding(4, 3, 4, 3);
			Graph2SelRadioButton.Name = "Graph2SelRadioButton";
			Graph2SelRadioButton.Size = new System.Drawing.Size(66, 19);
			Graph2SelRadioButton.TabIndex = 1;
			Graph2SelRadioButton.Text = "Graph 2";
			Graph2SelRadioButton.UseVisualStyleBackColor = true;
			Graph2SelRadioButton.Click += Graph2SelRadioButton_CheckedChanged;
			// 
			// Graph1SelRadioButton
			// 
			Graph1SelRadioButton.AutoSize = true;
			Graph1SelRadioButton.Checked = true;
			Graph1SelRadioButton.Location = new System.Drawing.Point(7, 22);
			Graph1SelRadioButton.Margin = new Padding(4, 3, 4, 3);
			Graph1SelRadioButton.Name = "Graph1SelRadioButton";
			Graph1SelRadioButton.Size = new System.Drawing.Size(66, 19);
			Graph1SelRadioButton.TabIndex = 0;
			Graph1SelRadioButton.TabStop = true;
			Graph1SelRadioButton.Text = "Graph 1";
			Graph1SelRadioButton.UseVisualStyleBackColor = true;
			Graph1SelRadioButton.Click += Graph1SelRadioButton_CheckedChanged;
			// 
			// label19
			// 
			label19.AutoSize = true;
			label19.BackColor = System.Drawing.Color.Transparent;
			label19.Location = new System.Drawing.Point(854, 128);
			label19.Margin = new Padding(4, 0, 4, 0);
			label19.Name = "label19";
			label19.Size = new System.Drawing.Size(44, 15);
			label19.TabIndex = 110;
			label19.Text = "Length";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.BackColor = System.Drawing.Color.Transparent;
			label4.Location = new System.Drawing.Point(853, 106);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(46, 15);
			label4.TabIndex = 109;
			label4.Text = "x offset";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.BackColor = System.Drawing.Color.Transparent;
			label3.Location = new System.Drawing.Point(476, 13);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(46, 15);
			label3.TabIndex = 108;
			label3.Text = "y offset";
			// 
			// label51
			// 
			label51.AutoSize = true;
			label51.BackColor = System.Drawing.Color.Transparent;
			label51.Location = new System.Drawing.Point(476, 51);
			label51.Margin = new Padding(4, 0, 4, 0);
			label51.Name = "label51";
			label51.Size = new System.Drawing.Size(43, 15);
			label51.TabIndex = 112;
			label51.Text = "Scale y";
			// 
			// GraphYScaleTrackBar
			// 
			GraphYScaleTrackBar.LargeChange = 25;
			GraphYScaleTrackBar.Location = new System.Drawing.Point(522, 37);
			GraphYScaleTrackBar.Margin = new Padding(4, 3, 4, 3);
			GraphYScaleTrackBar.Maximum = 1000;
			GraphYScaleTrackBar.Minimum = 1;
			GraphYScaleTrackBar.Name = "GraphYScaleTrackBar";
			GraphYScaleTrackBar.Size = new System.Drawing.Size(401, 45);
			GraphYScaleTrackBar.TabIndex = 111;
			GraphYScaleTrackBar.TickFrequency = 100;
			GraphYScaleTrackBar.TickStyle = TickStyle.TopLeft;
			GraphYScaleTrackBar.Value = 100;
			GraphYScaleTrackBar.Scroll += TrackBar3_Scroll;
			// 
			// GraphOffsetTrackBar
			// 
			GraphOffsetTrackBar.LargeChange = 25;
			GraphOffsetTrackBar.Location = new System.Drawing.Point(522, -1);
			GraphOffsetTrackBar.Margin = new Padding(4, 3, 4, 3);
			GraphOffsetTrackBar.Maximum = 500;
			GraphOffsetTrackBar.Minimum = -500;
			GraphOffsetTrackBar.Name = "GraphOffsetTrackBar";
			GraphOffsetTrackBar.Size = new System.Drawing.Size(401, 45);
			GraphOffsetTrackBar.TabIndex = 107;
			GraphOffsetTrackBar.TickFrequency = 100;
			GraphOffsetTrackBar.TickStyle = TickStyle.TopLeft;
			GraphOffsetTrackBar.Scroll += GraphOffsetTrackBar_Scroll;
			// 
			// GraphPictureBox
			// 
			GraphPictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			GraphPictureBox.BackColor = System.Drawing.Color.Black;
			GraphPictureBox.Location = new System.Drawing.Point(0, 158);
			GraphPictureBox.Margin = new Padding(4, 3, 4, 3);
			GraphPictureBox.Name = "GraphPictureBox";
			GraphPictureBox.Size = new System.Drawing.Size(1133, 721);
			GraphPictureBox.TabIndex = 0;
			GraphPictureBox.TabStop = false;
			// 
			// NetworkTab
			// 
			NetworkTab.BackColor = System.Drawing.SystemColors.Control;
			NetworkTab.Controls.Add(button41);
			NetworkTab.Controls.Add(button42);
			NetworkTab.Controls.Add(button43);
			NetworkTab.Controls.Add(xscalemvUpDown);
			NetworkTab.Controls.Add(label75);
			NetworkTab.Controls.Add(button35);
			NetworkTab.Controls.Add(NetworkUseAveragingCheckBox);
			NetworkTab.Controls.Add(panel5);
			NetworkTab.Controls.Add(label66);
			NetworkTab.Controls.Add(label65);
			NetworkTab.Controls.Add(NetworkCaptureTrackEndUpDown);
			NetworkTab.Controls.Add(NumberOfPointsUpDown);
			NetworkTab.Controls.Add(label64);
			NetworkTab.Controls.Add(NetworkCaptureTrackStartUpDown);
			NetworkTab.Controls.Add(label63);
			NetworkTab.Controls.Add(button29);
			NetworkTab.Controls.Add(button28);
			NetworkTab.ImageIndex = 2;
			NetworkTab.Location = new System.Drawing.Point(4, 24);
			NetworkTab.Margin = new Padding(4, 3, 4, 3);
			NetworkTab.Name = "NetworkTab";
			NetworkTab.Padding = new Padding(4, 3, 4, 3);
			NetworkTab.Size = new System.Drawing.Size(1128, 897);
			NetworkTab.TabIndex = 5;
			NetworkTab.Text = "Network";
			// 
			// button41
			// 
			button41.Location = new System.Drawing.Point(484, 155);
			button41.Margin = new Padding(4, 3, 4, 3);
			button41.Name = "button41";
			button41.Size = new System.Drawing.Size(84, 46);
			button41.TabIndex = 145;
			button41.Text = "Step >";
			button41.UseVisualStyleBackColor = true;
			button41.Click += Button41_Click;
			// 
			// button42
			// 
			button42.Location = new System.Drawing.Point(393, 155);
			button42.Margin = new Padding(4, 3, 4, 3);
			button42.Name = "button42";
			button42.Size = new System.Drawing.Size(84, 46);
			button42.TabIndex = 144;
			button42.Text = "Step <";
			button42.UseVisualStyleBackColor = true;
			button42.Click += Button42_Click;
			// 
			// button43
			// 
			button43.Location = new System.Drawing.Point(302, 155);
			button43.Margin = new Padding(4, 3, 4, 3);
			button43.Name = "button43";
			button43.Size = new System.Drawing.Size(84, 46);
			button43.TabIndex = 143;
			button43.Text = "Microstep 8";
			button43.UseVisualStyleBackColor = true;
			button43.Click += Button43_Click;
			// 
			// xscalemvUpDown
			// 
			xscalemvUpDown.Location = new System.Drawing.Point(467, 81);
			xscalemvUpDown.Margin = new Padding(4, 3, 4, 3);
			xscalemvUpDown.Maximum = new decimal(new int[] { 12000000, 0, 0, 0 });
			xscalemvUpDown.Name = "xscalemvUpDown";
			xscalemvUpDown.Size = new System.Drawing.Size(102, 23);
			xscalemvUpDown.TabIndex = 141;
			xscalemvUpDown.Value = new decimal(new int[] { 100, 0, 0, 0 });
			// 
			// label75
			// 
			label75.AutoSize = true;
			label75.Location = new System.Drawing.Point(467, 62);
			label75.Margin = new Padding(4, 0, 4, 0);
			label75.Name = "label75";
			label75.Size = new System.Drawing.Size(78, 15);
			label75.TabIndex = 142;
			label75.Text = "X Scale in mV";
			// 
			// button35
			// 
			button35.Location = new System.Drawing.Point(419, 7);
			button35.Margin = new Padding(4, 3, 4, 3);
			button35.Name = "button35";
			button35.Size = new System.Drawing.Size(115, 45);
			button35.TabIndex = 140;
			button35.Text = "Capture data current track";
			button35.UseVisualStyleBackColor = true;
			button35.Click += Button35_Click;
			// 
			// NetworkUseAveragingCheckBox
			// 
			NetworkUseAveragingCheckBox.AutoSize = true;
			NetworkUseAveragingCheckBox.Location = new System.Drawing.Point(340, 113);
			NetworkUseAveragingCheckBox.Margin = new Padding(4, 3, 4, 3);
			NetworkUseAveragingCheckBox.Name = "NetworkUseAveragingCheckBox";
			NetworkUseAveragingCheckBox.Size = new System.Drawing.Size(100, 19);
			NetworkUseAveragingCheckBox.TabIndex = 139;
			NetworkUseAveragingCheckBox.Text = "Use averaging";
			NetworkUseAveragingCheckBox.UseVisualStyleBackColor = true;
			// 
			// panel5
			// 
			panel5.Controls.Add(NetworkDoAllBad);
			panel5.Controls.Add(NetCaptureRangecheckBox);
			panel5.Location = new System.Drawing.Point(30, 80);
			panel5.Margin = new Padding(4, 3, 4, 3);
			panel5.Name = "panel5";
			panel5.Size = new System.Drawing.Size(163, 50);
			panel5.TabIndex = 138;
			// 
			// NetworkDoAllBad
			// 
			NetworkDoAllBad.AutoSize = true;
			NetworkDoAllBad.Location = new System.Drawing.Point(5, 23);
			NetworkDoAllBad.Margin = new Padding(4, 3, 4, 3);
			NetworkDoAllBad.Name = "NetworkDoAllBad";
			NetworkDoAllBad.Size = new System.Drawing.Size(149, 19);
			NetworkDoAllBad.TabIndex = 0;
			NetworkDoAllBad.Text = "Do all bad/unrecovered";
			NetworkDoAllBad.UseVisualStyleBackColor = true;
			// 
			// NetCaptureRangecheckBox
			// 
			NetCaptureRangecheckBox.AutoSize = true;
			NetCaptureRangecheckBox.Checked = true;
			NetCaptureRangecheckBox.Location = new System.Drawing.Point(6, 1);
			NetCaptureRangecheckBox.Margin = new Padding(4, 3, 4, 3);
			NetCaptureRangecheckBox.Name = "NetCaptureRangecheckBox";
			NetCaptureRangecheckBox.Size = new System.Drawing.Size(100, 19);
			NetCaptureRangecheckBox.TabIndex = 0;
			NetCaptureRangecheckBox.TabStop = true;
			NetCaptureRangecheckBox.Text = "Capture range";
			NetCaptureRangecheckBox.UseVisualStyleBackColor = true;
			// 
			// label66
			// 
			label66.AutoSize = true;
			label66.BackColor = System.Drawing.Color.Transparent;
			label66.Location = new System.Drawing.Point(212, 112);
			label66.Margin = new Padding(4, 0, 4, 0);
			label66.Name = "label66";
			label66.Size = new System.Drawing.Size(18, 15);
			label66.TabIndex = 137;
			label66.Text = "to";
			// 
			// label65
			// 
			label65.AutoSize = true;
			label65.BackColor = System.Drawing.Color.Transparent;
			label65.Location = new System.Drawing.Point(201, 82);
			label65.Margin = new Padding(4, 0, 4, 0);
			label65.Name = "label65";
			label65.Size = new System.Drawing.Size(33, 15);
			label65.TabIndex = 136;
			label65.Text = "from";
			// 
			// NetworkCaptureTrackEndUpDown
			// 
			NetworkCaptureTrackEndUpDown.Location = new System.Drawing.Point(240, 110);
			NetworkCaptureTrackEndUpDown.Margin = new Padding(4, 3, 4, 3);
			NetworkCaptureTrackEndUpDown.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
			NetworkCaptureTrackEndUpDown.Name = "NetworkCaptureTrackEndUpDown";
			NetworkCaptureTrackEndUpDown.Size = new System.Drawing.Size(61, 23);
			NetworkCaptureTrackEndUpDown.TabIndex = 135;
			NetworkCaptureTrackEndUpDown.Value = new decimal(new int[] { 13, 0, 0, 0 });
			// 
			// NumberOfPointsUpDown
			// 
			NumberOfPointsUpDown.Location = new System.Drawing.Point(340, 80);
			NumberOfPointsUpDown.Margin = new Padding(4, 3, 4, 3);
			NumberOfPointsUpDown.Maximum = new decimal(new int[] { 12000000, 0, 0, 0 });
			NumberOfPointsUpDown.Name = "NumberOfPointsUpDown";
			NumberOfPointsUpDown.Size = new System.Drawing.Size(102, 23);
			NumberOfPointsUpDown.TabIndex = 97;
			NumberOfPointsUpDown.Value = new decimal(new int[] { 3250000, 0, 0, 0 });
			// 
			// label64
			// 
			label64.AutoSize = true;
			label64.Location = new System.Drawing.Point(340, 61);
			label64.Margin = new Padding(4, 0, 4, 0);
			label64.Name = "label64";
			label64.Size = new System.Drawing.Size(101, 15);
			label64.TabIndex = 98;
			label64.Text = "Number of points";
			// 
			// NetworkCaptureTrackStartUpDown
			// 
			NetworkCaptureTrackStartUpDown.Location = new System.Drawing.Point(240, 80);
			NetworkCaptureTrackStartUpDown.Margin = new Padding(4, 3, 4, 3);
			NetworkCaptureTrackStartUpDown.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
			NetworkCaptureTrackStartUpDown.Name = "NetworkCaptureTrackStartUpDown";
			NetworkCaptureTrackStartUpDown.Size = new System.Drawing.Size(61, 23);
			NetworkCaptureTrackStartUpDown.TabIndex = 95;
			NetworkCaptureTrackStartUpDown.Value = new decimal(new int[] { 12, 0, 0, 0 });
			// 
			// label63
			// 
			label63.AutoSize = true;
			label63.Location = new System.Drawing.Point(240, 61);
			label63.Margin = new Padding(4, 0, 4, 0);
			label63.Name = "label63";
			label63.Size = new System.Drawing.Size(78, 15);
			label63.TabIndex = 96;
			label63.Text = "Capture track";
			// 
			// button29
			// 
			button29.BackColor = System.Drawing.Color.Cornsilk;
			button29.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			button29.Location = new System.Drawing.Point(317, 7);
			button29.Margin = new Padding(4, 3, 4, 3);
			button29.Name = "button29";
			button29.Size = new System.Drawing.Size(88, 46);
			button29.TabIndex = 93;
			button29.Text = "Stop!";
			button29.UseVisualStyleBackColor = false;
			button29.Click += Button29_Click;
			// 
			// button28
			// 
			button28.Location = new System.Drawing.Point(240, 7);
			button28.Margin = new Padding(4, 3, 4, 3);
			button28.Name = "button28";
			button28.Size = new System.Drawing.Size(70, 45);
			button28.TabIndex = 92;
			button28.Text = "Capture data";
			button28.UseVisualStyleBackColor = true;
			button28.Click += CaptureDataBtn_Click;
			// 
			// SectorUpDown
			// 
			SectorUpDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			SectorUpDown.Location = new System.Drawing.Point(1315, 430);
			SectorUpDown.Margin = new Padding(4, 3, 4, 3);
			SectorUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			SectorUpDown.Name = "SectorUpDown";
			SectorUpDown.Size = new System.Drawing.Size(42, 23);
			SectorUpDown.TabIndex = 78;
			toolTip1.SetToolTip(SectorUpDown, "Show data for this sector in Sector data tab.");
			SectorUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
			SectorUpDown.ValueChanged += TrackUpDown2_ValueChanged;
			// 
			// TrackUpDown
			// 
			TrackUpDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			TrackUpDown.Location = new System.Drawing.Point(1200, 430);
			TrackUpDown.Margin = new Padding(4, 3, 4, 3);
			TrackUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			TrackUpDown.Name = "TrackUpDown";
			TrackUpDown.Size = new System.Drawing.Size(42, 23);
			TrackUpDown.TabIndex = 77;
			toolTip1.SetToolTip(TrackUpDown, "Show data for this track in Sector data tab.");
			TrackUpDown.Value = new decimal(new int[] { 29, 0, 0, 0 });
			TrackUpDown.ValueChanged += TrackUpDown2_ValueChanged;
			// 
			// label39
			// 
			label39.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label39.AutoSize = true;
			label39.Location = new System.Drawing.Point(1264, 433);
			label39.Margin = new Padding(4, 0, 4, 0);
			label39.Name = "label39";
			label39.Size = new System.Drawing.Size(40, 15);
			label39.TabIndex = 75;
			label39.Text = "Sector";
			// 
			// label40
			// 
			label40.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label40.AutoSize = true;
			label40.Location = new System.Drawing.Point(1153, 433);
			label40.Margin = new Padding(4, 0, 4, 0);
			label40.Name = "label40";
			label40.Size = new System.Drawing.Size(34, 15);
			label40.TabIndex = 76;
			label40.Text = "Track";
			// 
			// progressBar1
			// 
			progressBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			progressBar1.Location = new System.Drawing.Point(760, 62);
			progressBar1.Margin = new Padding(4, 3, 4, 3);
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new System.Drawing.Size(390, 9);
			progressBar1.TabIndex = 83;
			// 
			// timer5
			// 
			timer5.Interval = 10;
			timer5.Tick += Timer5_Tick;
			// 
			// ProcessStatusLabel
			// 
			ProcessStatusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			ProcessStatusLabel.BackColor = System.Drawing.Color.Transparent;
			ProcessStatusLabel.Location = new System.Drawing.Point(1044, 44);
			ProcessStatusLabel.Margin = new Padding(4, 0, 4, 0);
			ProcessStatusLabel.Name = "ProcessStatusLabel";
			ProcessStatusLabel.Size = new System.Drawing.Size(105, 15);
			ProcessStatusLabel.TabIndex = 59;
			ProcessStatusLabel.Text = "Processing status";
			ProcessStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// GUITimer
			// 
			GUITimer.Tick += GUITimer_Tick;
			// 
			// timer1
			// 
			timer1.Interval = 250;
			timer1.Tick += Timer1_Tick;
			// 
			// GCbutton
			// 
			GCbutton.Location = new System.Drawing.Point(1074, 7);
			GCbutton.Margin = new Padding(4, 3, 4, 3);
			GCbutton.Name = "GCbutton";
			GCbutton.Size = new System.Drawing.Size(57, 27);
			GCbutton.TabIndex = 88;
			GCbutton.Text = "GC";
			GCbutton.UseVisualStyleBackColor = true;
			GCbutton.Visible = false;
			GCbutton.Click += GCbutton_Click;
			// 
			// contextMenuStrip1
			// 
			contextMenuStrip1.Name = "contextMenuStrip1";
			contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			// 
			// EditScatterPlotcheckBox
			// 
			EditScatterPlotcheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			EditScatterPlotcheckBox.AutoSize = true;
			EditScatterPlotcheckBox.Location = new System.Drawing.Point(1424, 433);
			EditScatterPlotcheckBox.Margin = new Padding(4, 3, 4, 3);
			EditScatterPlotcheckBox.Name = "EditScatterPlotcheckBox";
			EditScatterPlotcheckBox.Size = new System.Drawing.Size(106, 19);
			EditScatterPlotcheckBox.TabIndex = 167;
			EditScatterPlotcheckBox.Text = "Edit Scatterplot";
			toolTip1.SetToolTip(EditScatterPlotcheckBox, "Edit the flux period length manually. Only for Amiga formatted disks for now.");
			EditScatterPlotcheckBox.UseVisualStyleBackColor = true;
			EditScatterPlotcheckBox.CheckedChanged += CheckBox1_CheckedChanged;
			// 
			// menuStrip1
			// 
			menuStrip1.AutoSize = false;
			menuStrip1.Dock = DockStyle.None;
			menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, optionsToolStripMenuItem, helpToolStripMenuItem });
			menuStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			menuStrip1.Location = new System.Drawing.Point(1, 1);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Padding = new Padding(7, 2, 0, 2);
			menuStrip1.Size = new System.Drawing.Size(350, 31);
			menuStrip1.Stretch = false;
			menuStrip1.TabIndex = 168;
			menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, addToolStripMenuItem, toolStripSeparator1, saveToolStripMenuItem, toolStripSeparator2, loadProjectToolStripMenuItem, saveProjectToolStripMenuItem, toolStripSeparator3, importToolStripMenuItem, exportToolStripMenuItem, toolStripSeparator5, closeToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(37, 27);
			fileToolStripMenuItem.Text = "File";
			// 
			// openToolStripMenuItem
			// 
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
			openToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			openToolStripMenuItem.Text = "Open";
			openToolStripMenuItem.Click += OpenBinFilebutton_Click;
			// 
			// addToolStripMenuItem
			// 
			addToolStripMenuItem.Name = "addToolStripMenuItem";
			addToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;
			addToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			addToolStripMenuItem.Text = "Add";
			addToolStripMenuItem.Click += AddDataButton_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(195, 6);
			// 
			// saveToolStripMenuItem
			// 
			saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
			saveToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			saveToolStripMenuItem.Text = "Save disk image";
			saveToolStripMenuItem.Click += SaveDiskImageButton_Click;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(195, 6);
			// 
			// loadProjectToolStripMenuItem
			// 
			loadProjectToolStripMenuItem.Name = "loadProjectToolStripMenuItem";
			loadProjectToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;
			loadProjectToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			loadProjectToolStripMenuItem.Text = "Load project";
			loadProjectToolStripMenuItem.Click += LoadPrjBtn_Click;
			// 
			// saveProjectToolStripMenuItem
			// 
			saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
			saveProjectToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			saveProjectToolStripMenuItem.Text = "Save project";
			saveProjectToolStripMenuItem.Click += SavePrjBtn_Click;
			// 
			// toolStripSeparator3
			// 
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(195, 6);
			// 
			// importToolStripMenuItem
			// 
			importToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { scpFileToolStripMenuItem });
			importToolStripMenuItem.Name = "importToolStripMenuItem";
			importToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			importToolStripMenuItem.Text = "Import";
			// 
			// scpFileToolStripMenuItem
			// 
			scpFileToolStripMenuItem.Name = "scpFileToolStripMenuItem";
			scpFileToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			scpFileToolStripMenuItem.Text = "Scp file";
			scpFileToolStripMenuItem.Click += OpenSCP_Click;
			// 
			// exportToolStripMenuItem
			// 
			exportToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { scpFileToolStripMenuItem1, trimmedBinToolStripMenuItem, badSectorsToolStripMenuItem });
			exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			exportToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			exportToolStripMenuItem.Text = "Export";
			// 
			// scpFileToolStripMenuItem1
			// 
			scpFileToolStripMenuItem1.Name = "scpFileToolStripMenuItem1";
			scpFileToolStripMenuItem1.Size = new System.Drawing.Size(141, 22);
			scpFileToolStripMenuItem1.Text = "Scp file";
			scpFileToolStripMenuItem1.Click += SaveSCP_Click;
			// 
			// trimmedBinToolStripMenuItem
			// 
			trimmedBinToolStripMenuItem.Name = "trimmedBinToolStripMenuItem";
			trimmedBinToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			trimmedBinToolStripMenuItem.Text = "Trimmed bin";
			trimmedBinToolStripMenuItem.Click += trimmedBinToolStripMenuItem_Click;
			// 
			// badSectorsToolStripMenuItem
			// 
			badSectorsToolStripMenuItem.Name = "badSectorsToolStripMenuItem";
			badSectorsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			badSectorsToolStripMenuItem.Text = "Bad sectors";
			badSectorsToolStripMenuItem.Click += SaveTrimmedBadbutton_Click;
			// 
			// toolStripSeparator5
			// 
			toolStripSeparator5.Name = "toolStripSeparator5";
			toolStripSeparator5.Size = new System.Drawing.Size(195, 6);
			// 
			// closeToolStripMenuItem
			// 
			closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			closeToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			closeToolStripMenuItem.Text = "Close";
			closeToolStripMenuItem.Click += CloseToolStripMenuItem_Click;
			// 
			// optionsToolStripMenuItem
			// 
			optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { settingsToolStripMenuItem, disableTooltipsToolStripMenuItem, toolStripSeparator4, basicModeToolStripMenuItem, advancedModeToolStripMenuItem, devModeToolStripMenuItem });
			optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 27);
			optionsToolStripMenuItem.Text = "Options";
			// 
			// settingsToolStripMenuItem
			// 
			settingsToolStripMenuItem.Image = Properties.Resources.IconSettings;
			settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			settingsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
			settingsToolStripMenuItem.Text = "Settings";
			settingsToolStripMenuItem.Click += SettingsButton_Click;
			// 
			// disableTooltipsToolStripMenuItem
			// 
			disableTooltipsToolStripMenuItem.Checked = true;
			disableTooltipsToolStripMenuItem.CheckOnClick = true;
			disableTooltipsToolStripMenuItem.CheckState = CheckState.Checked;
			disableTooltipsToolStripMenuItem.Name = "disableTooltipsToolStripMenuItem";
			disableTooltipsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
			disableTooltipsToolStripMenuItem.Text = "Enable tooltips";
			disableTooltipsToolStripMenuItem.Click += DisableTooltipsToolStripMenuItem_Click;
			// 
			// toolStripSeparator4
			// 
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new System.Drawing.Size(158, 6);
			// 
			// basicModeToolStripMenuItem
			// 
			basicModeToolStripMenuItem.CheckOnClick = true;
			basicModeToolStripMenuItem.Name = "basicModeToolStripMenuItem";
			basicModeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
			basicModeToolStripMenuItem.Text = "Basic mode";
			basicModeToolStripMenuItem.Click += BasicModeToolStripMenuItem_Click;
			// 
			// advancedModeToolStripMenuItem
			// 
			advancedModeToolStripMenuItem.CheckOnClick = true;
			advancedModeToolStripMenuItem.Name = "advancedModeToolStripMenuItem";
			advancedModeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
			advancedModeToolStripMenuItem.Text = "Advanced mode";
			advancedModeToolStripMenuItem.Click += AdvancedModeToolStripMenuItem_Click;
			// 
			// devModeToolStripMenuItem
			// 
			devModeToolStripMenuItem.CheckOnClick = true;
			devModeToolStripMenuItem.Name = "devModeToolStripMenuItem";
			devModeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
			devModeToolStripMenuItem.Text = "Dev mode";
			devModeToolStripMenuItem.Click += DevModeToolStripMenuItem_Click;
			// 
			// helpToolStripMenuItem
			// 
			helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
			helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			helpToolStripMenuItem.Size = new System.Drawing.Size(44, 27);
			helpToolStripMenuItem.Text = "Help";
			// 
			// aboutToolStripMenuItem
			// 
			aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			aboutToolStripMenuItem.Text = "About";
			aboutToolStripMenuItem.Click += AboutButton_Click;
			// 
			// ThreadsUpDown
			// 
			ThreadsUpDown.Location = new System.Drawing.Point(1013, 10);
			ThreadsUpDown.Margin = new Padding(4, 3, 4, 3);
			ThreadsUpDown.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
			ThreadsUpDown.Minimum = new decimal(new int[] { 50, 0, 0, int.MinValue });
			ThreadsUpDown.Name = "ThreadsUpDown";
			ThreadsUpDown.Size = new System.Drawing.Size(46, 23);
			ThreadsUpDown.TabIndex = 172;
			ThreadsUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			ThreadsUpDown.ValueChanged += ThreadsUpDown_ValueChanged;
			// 
			// label59
			// 
			label59.AutoSize = true;
			label59.Location = new System.Drawing.Point(946, 15);
			label59.Margin = new Padding(4, 0, 4, 0);
			label59.Name = "label59";
			label59.Size = new System.Drawing.Size(48, 15);
			label59.TabIndex = 171;
			label59.Text = "Threads";
			// 
			// StopButton
			// 
			StopButton.BackColor = System.Drawing.Color.Cornsilk;
			StopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			StopButton.Location = new System.Drawing.Point(760, 15);
			StopButton.Margin = new Padding(4, 3, 4, 3);
			StopButton.Name = "StopButton";
			StopButton.Size = new System.Drawing.Size(88, 42);
			StopButton.TabIndex = 173;
			StopButton.Text = "Stop!";
			toolTip1.SetToolTip(StopButton, "Stops all capturing, error correction and processing tasks.");
			StopButton.UseVisualStyleBackColor = false;
			StopButton.Click += StopButton_Click;
			// 
			// ExploreHereBtn
			// 
			ExploreHereBtn.Location = new System.Drawing.Point(426, 32);
			ExploreHereBtn.Margin = new Padding(4, 3, 4, 3);
			ExploreHereBtn.Name = "ExploreHereBtn";
			ExploreHereBtn.Size = new System.Drawing.Size(89, 27);
			ExploreHereBtn.TabIndex = 174;
			ExploreHereBtn.Text = "Explore here";
			ExploreHereBtn.UseVisualStyleBackColor = true;
			ExploreHereBtn.Click += ExploreHereBtn_Click;
			// 
			// toolTip1
			// 
			toolTip1.Active = false;
			// 
			// FloppyControl
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoScroll = true;
			ClientSize = new System.Drawing.Size(1889, 1050);
			Controls.Add(ExploreHereBtn);
			Controls.Add(progressBar1);
			Controls.Add(StopButton);
			Controls.Add(ThreadsUpDown);
			Controls.Add(label59);
			Controls.Add(menuStrip1);
			Controls.Add(EditScatterPlotcheckBox);
			Controls.Add(GCbutton);
			Controls.Add(ProcessStatusLabel);
			Controls.Add(MainTabControl);
			Controls.Add(textBoxReceived);
			Controls.Add(SectorUpDown);
			Controls.Add(label39);
			Controls.Add(TrackUpDown);
			Controls.Add(tabControl1);
			Controls.Add(label40);
			Controls.Add(label5);
			Controls.Add(label1);
			Controls.Add(outputfilename);
			Controls.Add(panel2);
			DoubleBuffered = true;
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			MainMenuStrip = menuStrip1;
			Margin = new Padding(4, 3, 4, 3);
			Name = "FloppyControl";
			Text = "Floppy Control";
			FormClosing += FloppyControl_FormClosing;
			SizeChanged += FloppyControl_SizeChanged;
			Click += FloppyControl_Click;
			KeyDown += FloppyControl_KeyDown;
			Resize += FloppyControl_Resize;
			((System.ComponentModel.ISupportInitialize)fileSystemWatcher1).EndInit();
			tabControl1.ResumeLayout(false);
			ScatterPlottabPage.ResumeLayout(false);
			ScatterPlottabPage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)ScatterPictureBox).EndInit();
			tabPage1.ResumeLayout(false);
			tabPage1.PerformLayout();
			tabPage3.ResumeLayout(false);
			tabPage3.PerformLayout();
			ShowSectorTab.ResumeLayout(false);
			ShowSectorTab.PerformLayout();
			tabPage4.ResumeLayout(false);
			tabPage4.PerformLayout();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)LimitToSectorUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)LimitToTrackUpDown).EndInit();
			MainTabControl.ResumeLayout(false);
			QuickTab.ResumeLayout(false);
			QProcessingGroupBox.ResumeLayout(false);
			groupBox14.ResumeLayout(false);
			groupBox14.PerformLayout();
			groupBox15.ResumeLayout(false);
			groupBox15.PerformLayout();
			((System.ComponentModel.ISupportInitialize)QOffsetUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QMaxUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QSixEightUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QFourSixUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QMinUpDown).EndInit();
			groupBox16.ResumeLayout(false);
			groupBox16.PerformLayout();
			((System.ComponentModel.ISupportInitialize)QHistogramhScrollBar1).EndInit();
			groupBox17.ResumeLayout(false);
			groupBox17.PerformLayout();
			((System.ComponentModel.ISupportInitialize)QRateOfChange2UpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QRateOfChangeUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QAdaptOfsset2UpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QLimitToSectorUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QLimitToTrackUpDown).EndInit();
			groupBox12.ResumeLayout(false);
			groupBox12.PerformLayout();
			groupBox10.ResumeLayout(false);
			groupBox10.PerformLayout();
			groupBox11.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)QTrackDurationUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QStartTrackUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QEndTracksUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QTRK00OffsetUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)QMicrostepsPerTrackUpDown).EndInit();
			CaptureTab.ResumeLayout(false);
			CaptureTab.PerformLayout();
			groupBox4.ResumeLayout(false);
			groupBox4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)rxbufEndUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)rxbufStartUpDown).EndInit();
			groupBox7.ResumeLayout(false);
			groupBox7.PerformLayout();
			groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)TrackDurationUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)StartTrackUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)EndTracksUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)TRK00OffsetUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)MicrostepsPerTrackUpDown).EndInit();
			ProcessingTab.ResumeLayout(false);
			ProcessingTab.PerformLayout();
			((System.ComponentModel.ISupportInitialize)DupsUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)AddNoiseKnumericUpDown).EndInit();
			groupBox6.ResumeLayout(false);
			groupBox6.PerformLayout();
			((System.ComponentModel.ISupportInitialize)jESEnd).EndInit();
			((System.ComponentModel.ISupportInitialize)jESStart).EndInit();
			((System.ComponentModel.ISupportInitialize)iESEnd).EndInit();
			((System.ComponentModel.ISupportInitialize)iESStart).EndInit();
			((System.ComponentModel.ISupportInitialize)RateOfChange2UpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)AdaptOfsset2UpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)RndAmountUpDown).EndInit();
			ThresholdsGroupBox.ResumeLayout(false);
			ThresholdsGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)RateOfChangeUpDown).EndInit();
			groupBox5.ResumeLayout(false);
			groupBox5.PerformLayout();
			((System.ComponentModel.ISupportInitialize)HistogramhScrollBar1).EndInit();
			groupBox3.ResumeLayout(false);
			ErrorCorrectionTab.ResumeLayout(false);
			ErrorCorrectionTab.PerformLayout();
			((System.ComponentModel.ISupportInitialize)CombinationsUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)MFMByteLengthUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)MFMByteStartUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)C8StartUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)C6StartUpDown).EndInit();
			ECInfoTabs.ResumeLayout(false);
			ECTabSectorData.ResumeLayout(false);
			ECTabSectorData.PerformLayout();
			tabPage8.ResumeLayout(false);
			tabPage8.PerformLayout();
			((System.ComponentModel.ISupportInitialize)ScatterOffsetUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)ScatterMinUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)ScatterMaxUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)ScatterMaxTrackBar).EndInit();
			((System.ComponentModel.ISupportInitialize)ScatterMinTrackBar).EndInit();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)Sector2UpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)Track2UpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)Sector1UpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)Track1UpDown).EndInit();
			BadSectorPanel.ResumeLayout(false);
			BadSectorPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)ScatterOffsetTrackBar).EndInit();
			AnalysisPage.ResumeLayout(false);
			AnalysisPage.PerformLayout();
			groupBox8.ResumeLayout(false);
			groupBox8.PerformLayout();
			AnalysisTab2.ResumeLayout(false);
			AnalysisTab2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)ThresholdTestUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)DiffTest2UpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)DiffTestUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)PeriodExtendUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)HighpassThresholdUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)AdaptLookAheadUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)DiffMinDeviation2UpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)DiffOffsetUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)SignalRatioDistUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)DiffMinDeviationUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)SmoothingUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)AnDensityUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)DiffDistUpDown2).EndInit();
			((System.ComponentModel.ISupportInitialize)DiffThresholdUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)DiffGainUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)DiffDistUpDown).EndInit();
			groupBox9.ResumeLayout(false);
			groupBox9.PerformLayout();
			((System.ComponentModel.ISupportInitialize)GraphYScaleTrackBar).EndInit();
			((System.ComponentModel.ISupportInitialize)GraphOffsetTrackBar).EndInit();
			((System.ComponentModel.ISupportInitialize)GraphPictureBox).EndInit();
			NetworkTab.ResumeLayout(false);
			NetworkTab.PerformLayout();
			((System.ComponentModel.ISupportInitialize)xscalemvUpDown).EndInit();
			panel5.ResumeLayout(false);
			panel5.PerformLayout();
			((System.ComponentModel.ISupportInitialize)NetworkCaptureTrackEndUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)NumberOfPointsUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)NetworkCaptureTrackStartUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)SectorUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)TrackUpDown).EndInit();
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)ThreadsUpDown).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private OpenFileDialog openFileDialog1;
		private TextBox outputfilename;
		private Label label1;
		private FileSystemWatcher fileSystemWatcher1;
		private Label label5;
		private TextBox tbSectorMap;
		private Label BytesPerSecondLabel;
		private Label BytesReceivedLabel;
		private Label label24;
		private Label CurrentTrackLabel;
		private Label label25;
		private Label label23;
		private Label RecoveredSectorsLabel;
		private Label label26;
		private Label RecoveredSectorsWithErrorsLabel;
		private Label label33;
		private Label label34;
		private TabControl tabControl1;
		private TabPage tabPage1;
		private TextBox TrackInfotextBox;
		private TabPage tabPage2;
		private TabPage ScatterPlottabPage;
		private TextBox textBoxReceived;
		private Label LabelStatus;
		private Label label7;
		private Panel panel2;
		private Label label13;
		private Label CaptureTimeLabel;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private TabPage tabPage3;
		private TextBox textBoxFilesLoaded;
		private TabPage ShowSectorTab;
		private TextBox textBoxSector;
		private OpenFileDialog openFileDialog2;
		private NumericUpDown LimitToSectorUpDown;
		private Label label41;
		private NumericUpDown LimitToTrackUpDown;
		private Label label42;
		private Label MarkersLabel;
		private Label GoodHdrCntLabel;
		private Label label45;
		private Label label46;
		private TabControl MainTabControl;
		private TabPage CaptureTab;
		private TabPage ProcessingTab;
		private Button ProcessPCBtn;
		private Button ProcessBtn;
		private GroupBox groupBox6;
		private NumericUpDown SectorUpDown;
		private NumericUpDown TrackUpDown;
		private Label label39;
		private VScrollBar OffsetvScrollBar1;
		private Label label40;
		private VScrollBar EightvScrollBar;
		private VScrollBar SixvScrollBar;
		private VScrollBar MinvScrollBar;
		private VScrollBar FourvScrollBar;
		private Label Offsetlabel;
		private Label POffsetLabel;
		private Label EightLabel;
		private Label PMaxLabel;
		private Label SixLabel;
		private Label MinLabel;
		private Label PSixEightLabel;
		private Label FourLabel;
		private Label PMinLabel;
		private Label PFourSixLabel;
		private GroupBox groupBox5;
		private Panel Histogrampanel1;
		private GroupBox groupBox3;
		private Button ScanButton;
		private CheckBox HDCheckBox;
		private CheckBox IgnoreHeaderErrorCheckBox;
		private Label label37;
		private Label label2;
		private GroupBox groupBox7;
		private GroupBox groupBox1;
		private Button button6;
		private Button TrackPreset4Button;
		private Button TrackPreset2Button;
		private Button TrackPreset3Button;
		private Button TrackPreset1Button;
		private NumericUpDown TrackDurationUpDown;
		private NumericUpDown StartTrackUpDown;
		private NumericUpDown EndTracksUpDown;
		private Label label22;
		private NumericUpDown TRK00OffsetUpDown;
		private NumericUpDown MicrostepsPerTrackUpDown;
		private Label label38;
		private Label label36;
		private Label label21;
		private Label label20;
		private TabPage ErrorCorrectionTab;
		private Button ECSectorOverlayBtn;
		private Label BadSectorsCntLabel;
		private Label label44;
		private NumericUpDown Sector2UpDown;
		private NumericUpDown Track2UpDown;
		private Label label48;
		private Label label49;
		private NumericUpDown Sector1UpDown;
		private NumericUpDown Track1UpDown;
		private Label BlueCrcCheckLabel;
		private Label label47;
		private ListBox BadSectorListBox;
		private NumericUpDown RateOfChangeUpDown;
		private Label label50;
		private ProgressBar progressBar1;
		private Panel BadSectorPanel;
		private Label BadSectorTooltip;
		private Timer timer5;
		private Panel panel3;
		private RadioButton BlueTempRadio;
		private RadioButton BSBlueFromListRadio;
		private RadioButton BSBlueSectormapRadio;
		private Label label54;
		private Label label53;
		private Label label55;
		private Panel panel4;
		private RadioButton BSRedTempRadio;
		private RadioButton BSRedFromlistRadio;
		private RadioButton radioButton6;
		private Button CopySectorToBlueBtn;
		private Button BluetoRedByteCopyToolBtn;
		private Label BSEditByteLabel;
		private Label label43;
		private Label RedCrcCheckLabel;
		private TabPage AnalysisPage;
		private Button ConvertToMFMBtn;
		private TextBox tbBIN;
		private TextBox tbTest;
		private TextBox tbMFM;
		private Button button2;
		private CheckBox AutoRefreshSectorMapCheck;
		private Label ProcessStatusLabel;
		private CheckBox FindDupesCheckBox;
		private TabPage AnalysisTab2;
		private Timer GUITimer;
		private RichTextBox rtbSectorMap;
		private TabPage tabPage4;
		private GroupBox groupBox2;
		private Panel AnHistogramPanel;
		private Label HistScalingLabel;
		private TrackBar ScatterMaxTrackBar;
		private TrackBar ScatterMinTrackBar;
		private TrackBar ScatterOffsetTrackBar;
		private GroupBox ThresholdsGroupBox;
		private Button button21;
		private Button button23;
		private Button button25;
		private Button button26;
		private NumericUpDown ScatterMinUpDown;
		private NumericUpDown ScatterMaxUpDown;
		private NumericUpDown ScatterOffsetUpDown;
		private Label label58;
		private Label label57;
		private PictureBox ScatterPictureBox;
		private Label label56;
		private Label SelectionDifLabel;
		private TextBox AntxtBox;
		private TextBox antbSectorData;
		private Button ECZoomOutBtn;
		private TabControl ECInfoTabs;
		private TabPage ECTabSectorData;
		private TabPage tabPage8;
		private TextBox ECtbMFM;
		private Button ECRealign4E;
		private CheckBox BadSectorsCheckBox;
		private CheckBox GoodSectorsCheckBox;
		private GroupBox groupBox8;
		private RadioButton ANAmigaDiskSpareRadio;
		private RadioButton ANAmigaRadio;
		private RadioButton ANPCRadio;
		private System.ComponentModel.BackgroundWorker backgroundWorker2;
		private RadioButton AmigaMFMRadio;
		private Button button20;
		private CheckBox LimitTSCheckBox;
		private CheckBox DirectStepCheckBox;
		private Label label19;
		private Label label4;
		private Label label3;
		private TrackBar GraphOffsetTrackBar;
		private Label label51;
		private TrackBar GraphYScaleTrackBar;
		private GroupBox groupBox9;
		private RadioButton Graph3SelRadioButton;
		private RadioButton Graph2SelRadioButton;
		private RadioButton Graph1SelRadioButton;
		private Button OpenWavefrmbutton;
		private Label GraphLengthLabel;
		private Label GraphXOffsetLabel;
		private Label GraphYOffsetlabel;
		private Label GraphScaleYLabel;
		private RadioButton Graph4SelRadioButton;
		private Button GraphFilterButton;
		private Label label52;
		private Label label60;
		private NumericUpDown DiffDistUpDown;
		private Label label61;
		private NumericUpDown DiffGainUpDown;
		private Label label62;
		private NumericUpDown DiffThresholdUpDown;
		private NumericUpDown DiffDistUpDown2;
		private Button button19;
		private TabPage NetworkTab;
		private Button button28;
		private Button button29;
		private NumericUpDown NetworkCaptureTrackStartUpDown;
		private Label label63;
		private Button CaptureClassbutton;
		private Timer timer1;
		private NumericUpDown NumberOfPointsUpDown;
		private Label label64;
		private CheckBox AnReplacerxbufBox;
		private NumericUpDown AnDensityUpDown;
		private TrackBar HistogramhScrollBar1;
		private Button GCbutton;
		private NumericUpDown SmoothingUpDown;
		private NumericUpDown DiffMinDeviationUpDown;
		private CheckBox AddNoisecheckBox;
		private Label label66;
		private Label label65;
		private NumericUpDown NetworkCaptureTrackEndUpDown;
		private Panel panel5;
		private RadioButton NetworkDoAllBad;
		private RadioButton NetCaptureRangecheckBox;
		private PictureBox GraphPictureBox;
		private CheckBox LimitToScttrViewcheckBox;
		private NumericUpDown RndAmountUpDown;
		private Label label67;
		private NumericUpDown AddNoiseKnumericUpDown;
		private Label label68;
		private NumericUpDown SignalRatioDistUpDown;
		private CheckBox AdaptiveGaincheckBox;
		private CheckBox InvertcheckBox;
		private NumericUpDown DiffOffsetUpDown;
		private Button button18;
		private NumericUpDown DiffMinDeviation2UpDown;
		private NumericUpDown AdaptLookAheadUpDown;
		private ComboBox PeriodBeyond8uscomboBox;
		private NumericUpDown DupsUpDown;
		private Label label69;
		private Button EditUndobutton;
		private Button SaveWaveformButton;
		private Button button32;
		private Label label70;
		private Button DCOffsetbutton;
		private Button Lowpassbutton;
		private Label Undolevelslabel;
		private Button button33;
		private NumericUpDown HighpassThresholdUpDown;
		private Label statsLabel;
		private Label label72;
		private ComboBox EditModecomboBox;
		private ComboBox EditOptioncomboBox;
		private Button button1;
		private Label label71;
		private Label label6;
		private NumericUpDown C8StartUpDown;
		private NumericUpDown C6StartUpDown;
		private Label label73;
		private NumericUpDown PeriodExtendUpDown;
		private CheckBox NetworkUseAveragingCheckBox;
		private Label label74;
		private NumericUpDown AdaptOfsset2UpDown;
		private Button button3;
		private Button button31;
		private CheckBox AnAutoUpdateCheckBox;
		private Button button34;
		private NumericUpDown DiffTestUpDown;
		private NumericUpDown DiffTest2UpDown;
		private NumericUpDown ThresholdTestUpDown;
		private Button button35;
		private RadioButton Graph5SelRadioButton;
		private ComboBox JumpTocomboBox;
		private ContextMenuStrip contextMenuStrip1;
		private NumericUpDown RateOfChange2UpDown;
		private CheckBox ECMFMcheckBox;
		private Button button38;
		private Button button40;
		private Button button39;
		private Button button36;
		private ImageList MainTabControlImageList;
		private NumericUpDown xscalemvUpDown;
		private Label label75;
		private Button button41;
		private Button button42;
		private Button button43;
		private Label hlabel;
		private Label wlabel;
		private NumericUpDown MFMByteLengthUpDown;
		private NumericUpDown MFMByteStartUpDown;
		private Label label76;
		private Label label77;
		private Button ECMFMByteEncbutton;
		private Button button44;
		private Button button45;
		private Button button46;
		private CheckBox ClearDatacheckBox;
		private ComboBox ChangeDiskTypeComboBox;
		private Label DiskTypeLabel;
		private Button button48;
		private Button button49;
		private Button SaveTrimmedBadbutton;
		private ComboBox ProcessingModeComboBox;
		private ComboBox ScanComboBox;
		private Label label78;
		private Label rxbufOffsetLabel;
		private Label label80;
		private NumericUpDown CombinationsUpDown;
		private Label label79;
		private CheckBox EditScatterPlotcheckBox;
		private RadioButton ECOnRadio;
		private RadioButton OnlyBadSectorsRadio;
		private Label label81;
		private Button FullHistBtn;
		private Button button4;
		private Button button8;
		private Button button7;
		private TabPage QuickTab;
		private GroupBox groupBox12;
		private Button StepStickPresetBtn;
		private Button QDirectStepPresetBtn;
		private Button QRecaptureAllBtn;
		private Button button12;
		private Button button13;
		private Button button14;
		private CheckBox QDirectStepCheckBox;
		private Button QCaptureBtn;
		private GroupBox groupBox10;
		private GroupBox groupBox11;
		private Button PresetCaptureDuration1s;
		private Button PresetCaptureDuration5s;
		private Button PresetTrack78_164;
		private Button PresetTrack80_90;
		private Button PresetCaptureDefaultBtn;
		private Button button37;
		private NumericUpDown QTrackDurationUpDown;
		private NumericUpDown QStartTrackUpDown;
		private NumericUpDown QEndTracksUpDown;
		private Label label82;
		private NumericUpDown QTRK00OffsetUpDown;
		private NumericUpDown QMicrostepsPerTrackUpDown;
		private Label label83;
		private Label label84;
		private Label label85;
		private Label label86;
		private GroupBox QProcessingGroupBox;
		private GroupBox groupBox14;
		private RadioButton QOnlyBadSectorsRadio;
		private RadioButton QECOnRadio;
		private ComboBox QProcessingModeComboBox;
		private CheckBox QClearDatacheckBox;
		private NumericUpDown QAdaptOfsset2UpDown;
		private NumericUpDown QLimitToTrackUpDown;
		private NumericUpDown QLimitToSectorUpDown;
		private Label label90;
		private Label label91;
		private GroupBox groupBox15;
		private Label QOffsetLabel;
		private Label QFourSixLabel;
		private Label QMinLabel;
		private Label QSixEightLabel;
		private Label QMaxLabel;
		private CheckBox QFindDupesCheckBox;
		private CheckBox QAutoRefreshSectorMapCheck;
		private CheckBox QIgnoreHeaderErrorCheckBox;
		private GroupBox groupBox16;
		private Panel QHistoPanel;
		private TrackBar QHistogramhScrollBar1;
		private GroupBox groupBox17;
		private ComboBox QScanComboBox;
		private Button button47;
		private CheckBox QHDCheckBox;
		private Label label114;
		private Button QProcAmigaBtn;
		private Button QProcPCBtn;
		private RichTextBox QrtbSectorMap;
		private Label label89;
		private ComboBox QChangeDiskTypeComboBox;
		private NumericUpDown QOffsetUpDown;
		private NumericUpDown QMaxUpDown;
		private NumericUpDown QSixEightUpDown;
		private NumericUpDown QFourSixUpDown;
		private NumericUpDown QMinUpDown;
		private NumericUpDown QRateOfChange2UpDown;
		private Label label88;
		private Label label102;
		private NumericUpDown QRateOfChangeUpDown;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem openToolStripMenuItem;
		private ToolStripMenuItem addToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem saveToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem loadProjectToolStripMenuItem;
		private ToolStripMenuItem saveProjectToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem importToolStripMenuItem;
		private ToolStripMenuItem scpFileToolStripMenuItem;
		private ToolStripMenuItem exportToolStripMenuItem;
		private ToolStripMenuItem scpFileToolStripMenuItem1;
		private ToolStripMenuItem trimmedBinToolStripMenuItem;
		private ToolStripMenuItem badSectorsToolStripMenuItem;
		private ToolStripMenuItem helpToolStripMenuItem;
		private ToolStripMenuItem aboutToolStripMenuItem;
		private NumericUpDown ThreadsUpDown;
		private Label label59;
		private GroupBox groupBox4;
		private NumericUpDown rxbufEndUpDown;
		private NumericUpDown rxbufStartUpDown;
		private Label BufferSizeLabel;
		private Label HistogramLengthLabel;
		private Label HistogramStartLabel;
		private Label label29;
		private Label label28;
		private Label label27;
		private Label label32;
		private Label label31;
		private Button button5;
		private Button ResetBuffersBtn;
		private NumericUpDown jESEnd;
		private NumericUpDown jESStart;
		private Label label17;
		private Label SettingsLabel;
		private Label label35;
		private NumericUpDown iESEnd;
		private NumericUpDown iESStart;
		private Label label8;
		private Label label9;
		private Button StopButton;
		private Button button11;
		private Button button15;
		private CheckBox QLimitTSCheckBox;
		private ToolTip toolTip1;
		private ToolStripMenuItem optionsToolStripMenuItem;
		private ToolStripMenuItem settingsToolStripMenuItem;
		private ToolStripMenuItem disableTooltipsToolStripMenuItem;
		private ToolStripMenuItem basicModeToolStripMenuItem;
		private ToolStripMenuItem advancedModeToolStripMenuItem;
		private ToolStripMenuItem devModeToolStripMenuItem;
		private Button GluedDiskPreset;
		private Button PresetCaptureDuration50s;
		private Button PresetCaptureDuration2s;
		private ToolStripSeparator toolStripSeparator4;
		private Button ExploreHereBtn;
		private ToolStripSeparator toolStripSeparator5;
		private ToolStripMenuItem closeToolStripMenuItem;
		private Button AvgPeriodsFromListSelBtn;
		private Button EntropySpliceBtn;
		private CheckBox SkipAlreadyCrcOkcheckBox1;
	}
}

