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
using FloppyControlApp.MyClasses.Graphics;
using FloppyControlApp.MyClasses.Capture;
using FloppyControlApp.MyClasses.Capture.Models;
using FloppyControlApp.MyClasses.FileIO;

namespace FloppyControlApp
{
	public partial class FloppyControl : Form
    {
        #region variable definitions
        private string GuiMode;
        private FDDProcessing processing;
        private ControlFloppy controlfloppy;
        private ScopeCapture scope = new ScopeCapture();
        private Point BadSectorTooltipPos;
        private StringBuilder tbreceived = new StringBuilder();
        private int bytesReceived = 0;
        //private Graphset graphset;
        private Histogram ECHisto;
        private Histogram ScatterHisto;
        private ScatterPlot scatterplot;
        //private static uint markerpositionscnt;
        private string subpath;
        //private string path = "";
        private string selectedPortName;
        //private string[] openfilespaths;
        private int disablecatchkey = 0;
        private int binfilecount = 0; // Keep saving each capture under a different filename as to keep all captured data
        private float capturetime = 0;
        
        private int selectedBaudRate = 5000000;
        private int graphselect = 0;
        
        private byte[] TempSector = new byte[550];
        private byte[][] graphwaveform = new byte[15][];
        //private bool AddData = false;
        public bool openFilesDlgUsed = false;
        public bool ScpOpenFilesDlgUsed = false;
        private bool scanactive = false;
        private bool stopupdatingGraph = false;
        private int[] mfmbyteenc = new int[256];
        private Version version;
        private FileIO fileio;
        private WaveformEdit oscilloscope;
        #endregion

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
                    if (processing.sectordata2[i].rxbufMarkerPositions > scatterplot.RxbufClickIndex)
                    {
                        offset = scatterplot.RxbufClickIndex - processing.sectordata2[i - 1].rxbufMarkerPositions;
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
                UpdateECInterface();
            }
            else
            {
                if (grphcnt == 0)
                {
                    return;
                }
                for (i = 0; i < grphcnt; i++)
                {
                    oscilloscope.graphset.Graphs[i].DataLength = 1000;
                    oscilloscope.graphset.Graphs[i].DataOffset = scatterplot.GraphIndex - 500;

                    if (oscilloscope.graphset.Graphs[i].DataOffset < 0)
                        oscilloscope.graphset.Graphs[i].DataOffset = 0;

                    oscilloscope.graphset.Graphs[i].Changed = true;
                    oscilloscope.graphset.Graphs[i].Density = 1;
                }

                oscilloscope.graphset.UpdateGraphs();
                MainTabControl.SelectedTab = AnalysisTab2;
            }

        }

        private void SectorMapUpdateGUICallback()
        {
            RecoveredSectorsLabel.Text = processing.SectorMap.recoveredsectorcount.ToString();
            RecoveredSectorsWithErrorsLabel.Text = processing.SectorMap.RecoveredSectorWithErrorsCount.ToString();
            GoodHdrCntLabel.Text = processing.GoodSectorHeaderCount.ToString();
            MarkersLabel.Text = processing.sectordata2.Count.ToString();
            BadSectorsCntLabel.Text = processing.SectorMap.badsectorcnt.ToString();
            statsLabel.Text = processing.SectorMap.s1.ToString("0.00") + " " +
                                processing.SectorMap.s2.ToString("0.00") + " " +
                                processing.SectorMap.s3.ToString("0.00");

            if (this.Width == 1620 || this.Width == 1630 || this.Width == 1680)
            {
                this.Width = processing.SectorMap.WindowWidth;
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
            processing.ProcSettings.processingtype = procmode;
            //if (NormalradioButton.Checked == true) processing.procsettings.processingtype = ProcessingType.normal;
            //else if (AdaptradioButton.Checked == true) processing.procsettings.processingtype = ProcessingType.adaptive;
            //else if (AufitRadioButton.Checked == true) processing.procsettings.processingtype = ProcessingType.aufit;
            processing.ProcSettings.NumberOfDups = (int)DupsUpDown.Value;
            processing.ProcSettings.pattern = PeriodBeyond8uscomboBox.SelectedIndex;
            //tbreceived.Append("Combobox:" + PeriodBeyond8uscomboBox.SelectedIndex + "\r\n");

            processing.ProcSettings.offset = OffsetvScrollBar1.Value;
            processing.ProcSettings.min = MinvScrollBar.Value + processing.ProcSettings.offset;
            processing.ProcSettings.four = FourvScrollBar.Value + processing.ProcSettings.offset;
            processing.ProcSettings.six = SixvScrollBar.Value + processing.ProcSettings.offset;
            processing.ProcSettings.max = EightvScrollBar.Value + processing.ProcSettings.offset;
            processing.ProcSettings.SkipPeriodData = false;
            processing.ProcSettings.AutoRefreshSectormap = AutoRefreshSectorMapCheck.Checked;
            processing.ProcSettings.start = (int)rxbufStartUpDown.Value;
            processing.ProcSettings.end = (int)rxbufEndUpDown.Value;
            processing.ProcSettings.finddupes = FindDupesCheckBox.Checked;
            processing.ProcSettings.rateofchange = (float)RateOfChangeUpDown.Value;
            //processing.procsettings.platform = platform; // 1 = Amiga
            processing.ProcSettings.UseErrorCorrection = ECOnRadio.Checked;
            processing.ProcSettings.OnlyBadSectors = OnlyBadSectorsRadio.Checked;
            processing.ProcSettings.AddNoise = AddNoisecheckBox.Checked;

            processing.ProcSettings.limittotrack = (int)LimitToTrackUpDown.Value;
            processing.ProcSettings.limittosector = (int)LimitToSectorUpDown.Value;
            processing.ProcSettings.NumberOfDups = (int)DupsUpDown.Value;
            processing.ProcSettings.LimitTSOn = LimitTSCheckBox.Checked;
            processing.ProcSettings.IgnoreHeaderError = IgnoreHeaderErrorCheckBox.Checked;
            //processing.procsettings.AdaptOffset = (int)AdaptOffsetUpDown.Value;
            processing.ProcSettings.AdaptOffset2 = (float)AdaptOfsset2UpDown.Value;
            processing.ProcSettings.rateofchange2 = (int)RateOfChange2UpDown.Value;

            processing.ProcSettings.start = 0;
            processing.ProcSettings.end = processing.RxBbuf.Length-1;

            if (LimitToScttrViewcheckBox.Checked == true)
            {
                processing.ProcSettings.start = scatterplot.AnScatViewlargeoffset;
                processing.ProcSettings.end = scatterplot.AnScatViewlargeoffset + scatterplot.AnScatViewlength;
            }

            if (LimitToScttrViewcheckBox.Checked == true && OnlyBadSectorsRadio.Checked == true)
            {
                processing.ProcSettings.addnoiselimitstart = ScatterMinTrackBar.Value + 50;
                processing.ProcSettings.addnoiselimitend = ScatterMaxTrackBar.Value + 50;
            }
            else
            {
                processing.ProcSettings.addnoiselimitstart = 0;
                processing.ProcSettings.addnoiselimitend = processing.Indexrxbuf;
            }
            processing.ProcSettings.addnoiserndamount = (int)RndAmountUpDown.Value;
        }

        // Update scatterplot while capturing
        public void ControlFloppyScatterplotCallback()
        {
            scatterplot.Rxbuf = processing.RxBbuf;
            scatterplot.AnScatViewlargeoffset = processing.RxBbuf.Length - controlfloppy.recentreadbuflength;
            if (scatterplot.AnScatViewlargeoffset < 0)
                scatterplot.AnScatViewlargeoffset = 0;
            scatterplot.AnScatViewoffset = 0;
            scatterplot.AnScatViewlength = controlfloppy.recentreadbuflength;
            controlfloppy.recentreadbuflength = 0;
            //scatterplot.UpdateScatterPlot();
            CurrentTrackLabel.Text = controlfloppy.CurrentTrack.ToString("F2");
            UpdateAllGraphs();

        }

        void FilesAvailableCallback()
        {
            openFilesDlgUsed = true;
        }

        void ScpFilesAvailableCallback()
        {
            ScpOpenFilesDlgUsed = true;
        }

        private void UpdateGraphCallback()
        {
            if (stopupdatingGraph == false)
            {
                AnDensityUpDown.Value = oscilloscope.graphset.Graphs[0].Density;
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

                oscilloscope.graphset.Graphs[index].Changed = true;

                oscilloscope.graphset.Graphs[graphselect].YScale = (GraphYScaleTrackBar.Value / 100.0f);
                GraphScaleYLabel.Text = (GraphYScaleTrackBar.Value / 100.0f).ToString();
                /*
                foreach ( var gr in graphset.Graphs)
                {
                    gr.density = density;
                }
                AnDensityUpDown.Value = density;
                */
                GraphLengthLabel.Text = string.Format("{0:n0}", oscilloscope.graphset.Graphs[0].DataLength);
                GraphXOffsetLabel.Text = string.Format("{0:n0}", oscilloscope.graphset.Graphs[0].DataOffset);
                int i;
                int centerposition = oscilloscope.graphset.Graphs[0].DataOffset;
                //int centerposition = graphset.Graphs[0].dataoffset + (graphset.Graphs[0].datalength / 2);
                if (processing.sectordata2 != null && processing.Rxbuftograph != null)
                {
                    for (i = 0; i < processing.Rxbuftograph.Length; i++)
                    {
                        if (processing.Rxbuftograph[i] > centerposition)
                            break;

                    }
                    tbreceived.Append("rxbuftograph i " + i + "\r\n");
                    if (i < processing.Rxbuftograph.Length - 1)
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


                            rxbufOffsetLabel.Text = "T" + sectordata.track.ToString("D3") + " S" + sectordata.sector + " o:" + sectoroffset.ToString();
                        }
                    }
                }
                Undolevelslabel.Text = "Undo levels: " + (oscilloscope.graphset.Graphs[0].Undo.Count).ToString();
                //graphset.UpdateGraphs();
                scatterplot.UpdateScatterPlot();

            }
        }

        public void FilterGuiUpdateCallback()
        {
            FindPeaks();
            rxbufEndUpDown.Maximum = processing.Indexrxbuf;
            rxbufStartUpDown.Maximum = processing.Indexrxbuf;

            rxbufEndUpDown.Value = processing.Indexrxbuf;
            HistogramhScrollBar1.Minimum = 0;
            HistogramhScrollBar1.Maximum = processing.Indexrxbuf;

            oscilloscope.graphset.SetAllChanged();

            if (scatterplot.AnScatViewlength == 0 || scatterplot.AnScatViewlength == 100000)
                scatterplot.AnScatViewlength = processing.Indexrxbuf - 1;
            scatterplot.UpdateScatterPlot();
            oscilloscope.graphset.UpdateGraphs();
            if (processing.Indexrxbuf > 0)
                ProcessingTab.Enabled = true;
        }

        public void Filter2GuiCallback()
        {
            rxbufEndUpDown.Maximum = processing.Indexrxbuf;
            rxbufStartUpDown.Maximum = processing.Indexrxbuf;

            rxbufEndUpDown.Value = processing.Indexrxbuf;
            HistogramhScrollBar1.Minimum = 0;
            HistogramhScrollBar1.Maximum = processing.Indexrxbuf;
            //processing.indexrxbuf = indexrxbuf;

            oscilloscope.graphset.SetAllChanged();

            if (scatterplot.AnScatViewlength == 0)
                scatterplot.AnScatViewlength = processing.Indexrxbuf - 1;
            scatterplot.UpdateScatterPlot();
            oscilloscope.graphset.UpdateGraphs();
            if (processing.Indexrxbuf > 0)
                ProcessingTab.Enabled = true;
        }

		#endregion

		#region Capture
		
        private void ConnectToFloppyControlHardware()
		{
			if (MainTabControl.SelectedTab == ProcessingTab)
			{
				controlfloppy.DirectStep = DirectStepCheckBox.Checked;
				controlfloppy.MicrostepsPerTrack = (int)MicrostepsPerTrackUpDown.Value;
				controlfloppy.StepStickMicrostepping =
				controlfloppy.trk00offset = (int)TRK00OffsetUpDown.Value;
				controlfloppy.EndTrack = (int)EndTracksUpDown.Value;
				controlfloppy.StartTrack = (int)StartTrackUpDown.Value;
				controlfloppy.TrackDuration = (int)TrackDurationUpDown.Value;

			}
			else if (MainTabControl.SelectedTab == QuickTab)
			{
				controlfloppy.DirectStep = QDirectStepCheckBox.Checked;
				controlfloppy.MicrostepsPerTrack = (int)QMicrostepsPerTrackUpDown.Value;
				controlfloppy.trk00offset = (int)QTRK00OffsetUpDown.Value;
				controlfloppy.EndTrack = (int)QEndTracksUpDown.Value;
				controlfloppy.StartTrack = (int)QStartTrackUpDown.Value;
				controlfloppy.TrackDuration = (int)QTrackDurationUpDown.Value;
			}

			controlfloppy.binfilecount = binfilecount;
			controlfloppy.tbr = tbreceived;
			//processing.indexrxbuf            = indexrxbuf;
			controlfloppy.StepStickMicrostepping = Decimal.ToInt32((decimal)Properties.Settings.Default["StepStickMicrostepping"]);
			controlfloppy.outputfilename = outputfilename.Text;
			controlfloppy.rxbuf = processing.RxBbuf;

			// Callbacks
			controlfloppy.updateHistoAndSliders = UpdateHistoAndSliders;
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

		private void CaptureTracks()
		{
			ResetInput();
			processing.entropy = null;
			tabControl1.SelectedTab = ScatterPlottabPage;

			controlfloppy.outputfilename = outputfilename.Text;

			if (controlfloppy.serialPort1.IsOpen)
				controlfloppy.StartCapture();
			else
				tbreceived.Append("Not connected.\r\n");
		}

		public void RecaptureAllBadSectors()
		{
			int track, sector;

			var sectorok = processing.SectorMap.sectorok;
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

		#endregion

		#region Update form elements

		// Updates the labels under the sliders
		// as well as the indicators under the histogram
		private void UpdateSliderLabels()
        {

            int x, y;

            if ((OffsetvScrollBar1.Value + MinvScrollBar.Value) < 0)
                MinLabel.Text = 0.ToString("X2");
            else MinLabel.Text = (OffsetvScrollBar1.Value + MinvScrollBar.Value).ToString("X2");
            FourLabel.Text = (OffsetvScrollBar1.Value + FourvScrollBar.Value).ToString("X2");
            SixLabel.Text = (OffsetvScrollBar1.Value + SixvScrollBar.Value).ToString("X2");
            EightLabel.Text = (OffsetvScrollBar1.Value + EightvScrollBar.Value).ToString("X2");
            Offsetlabel.Text = OffsetvScrollBar1.Value.ToString("D2");

            processing.Peak1 = OffsetvScrollBar1.Value + FourvScrollBar.Value;
            processing.Peak2 = OffsetvScrollBar1.Value + SixvScrollBar.Value;
            processing.Peak3 = OffsetvScrollBar1.Value + EightvScrollBar.Value;

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
            if (processing.Indexrxbuf < 100000)
                scatterplot.AnScatViewlength = processing.Indexrxbuf;
            else scatterplot.AnScatViewlength = 99999;
            scatterplot.AnScatViewoffset = 0;
            scatterplot.UpdateScatterPlot();
            UpdateHistoAndSliders();
            //ScatterHisto.DoHistogram(rxbuf, (int)rxbufStartUpDown.Value, (int)rxbufEndUpDown.Value);
            if (processing.Indexrxbuf > 0)
                ProcessingTab.Enabled = true;
        }

        private void UpdateHistoAndSliders()
        {
            if (!LimitTSCheckBox.Checked)
            {
                //createhistogram();
                UpdateSliderLabels();
            }
        }

        private void UpdateECInterface()
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

        private void UpdateAnScatterPlot()
        {
            if (processing.processing == 1)
                return;
            scatterplot.Thresholdmin = MinvScrollBar.Value + OffsetvScrollBar1.Value;
            scatterplot.Threshold4us = FourvScrollBar.Value + OffsetvScrollBar1.Value;
            scatterplot.Threshold6us = SixvScrollBar.Value + OffsetvScrollBar1.Value;
            scatterplot.Thresholdmax = EightvScrollBar.Value + OffsetvScrollBar1.Value;

            HistogramhScrollBar1.Maximum = processing.Indexrxbuf;
            QHistogramhScrollBar1.Maximum = processing.Indexrxbuf;
            if (scatterplot.AnScatViewoffset + scatterplot.AnScatViewlargeoffset < 0)
            {
                scatterplot.AnScatViewoffset = 0;
                scatterplot.AnScatViewlargeoffset = 0;
            }

            if (processing.Indexrxbuf > 0)
            {
                if (MainTabControl.SelectedTab == ProcessingTab)
                {
                    int offset = scatterplot.AnScatViewoffset + scatterplot.AnScatViewlargeoffset;
                    int length = scatterplot.AnScatViewlength;
                    if (length < 0) length = 4000;
                    if (scatterplot.AnScatViewlargeoffset < processing.Indexrxbuf)
                        HistogramhScrollBar1.Value = scatterplot.AnScatViewlargeoffset;
                    ScatterHisto.SetPanel(Histogrampanel1);
                    ScatterHisto.DoHistogram(processing.RxBbuf, offset, length);
                }
                if (MainTabControl.SelectedTab == QuickTab)
                {
                    int offset = scatterplot.AnScatViewoffset + scatterplot.AnScatViewlargeoffset;
                    int length = scatterplot.AnScatViewlength;
                    if (length < 0) length = 4000;
                    if (scatterplot.AnScatViewlargeoffset < processing.Indexrxbuf)
                        QHistogramhScrollBar1.Value = scatterplot.AnScatViewlargeoffset;
                    ScatterHisto.SetPanel(QHistoPanel);
                    ScatterHisto.DoHistogram(processing.RxBbuf, offset, length);
                }
            }
        }

        private void UpdateAllGraphs()
        {
            if (controlfloppy.capturecommand == 1)
            {
                processing.RxBbuf = controlfloppy.tempbuffer.Skip(Math.Max(0, controlfloppy.tempbuffer.Count() - 30)).SelectMany(a => a).ToArray();
            }
            else
            {
                processing.RxBbuf = controlfloppy.tempbuffer.SelectMany(a => a).ToArray();
            }
            //processing.rxbuf = controlfloppy.tempbuffer.SelectMany(a => a).ToArray();

            Setrxbufcontrol();

            if (processing.Indexrxbuf < 100000)
                scatterplot.AnScatViewlength = processing.Indexrxbuf;
            else scatterplot.AnScatViewlength = 99999;
            scatterplot.AnScatViewoffset = 0;
            //scatterplot.UpdateScatterPlot();


            if (processing.Indexrxbuf > 0)
                ProcessingTab.Enabled = true;
            if (controlfloppy.capturecommand == 0)
            {
                if (HistogramhScrollBar1.Maximum < 0)
                    HistogramhScrollBar1.Maximum = 0;
                HistogramhScrollBar1.Value = 0;
            }
            if (processing.Indexrxbuf > 0)
            {
                //updateAnScatterPlot();
                scatterplot.AnScatViewlargeoffset = HistogramhScrollBar1.Value;
                scatterplot.UpdateScatterPlot();
                if (controlfloppy.capturecommand == 0)
                {
                    ScatterHisto.DoHistogram();
                    UpdateSliderLabels();
                    UpdateHistoAndSliders();
                }

            }
        }

        private void UpdateBadSectorListview(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) //Delete key
            {
                var selectedItems = BadSectorListBox.SelectedItems;
                if (BadSectorListBox.SelectedIndex != -1)
                {
                    for (int i = selectedItems.Count - 1; i >= 0; i--)
                    {
                        var badsectoritem = (Badsectorkeyval)selectedItems[i];

                        for (int j = 0; j < JumpTocomboBox.Items.Count; j++)
                        {
                            var jumpboxitem = (ComboboxItem)JumpTocomboBox.Items[j];

                            if (jumpboxitem.Id == badsectoritem.Id)
                                JumpTocomboBox.Items.RemoveAt(j);
                        }

                        BadSectorListBox.Items.Remove(selectedItems[i]);
                    }
                }
            }
        }

        #endregion
        
        #region Scan methods

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
                    AdaptiveDeepScan();
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

        private void OffsetScan()
        {
            ProcessingModeComboBox.SelectedItem = ProcessingType.normal.ToString();
            capturetime = 0;
            processing.processing = 1;
            processing.stop = 0;
            for (int i = -15; i < 19; i += 3)
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

                this.UpdateForm();
            }

            processing.stop = 0;
            processing.processing = 0;
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
                    UpdateHistoAndSliders();
                    for (k = 0; k < kmax; k++)
                    {
                        SettingsLabel.Text = "i = " + i + " j = " + j + " k = " + k;
                        gc_cnt++;
                        if (gc_cnt % 50 == 0)
                            GC.Collect();
                        if (processing.stop == 1)
                            break;
                        if (processing.diskformat == DiskFormat.amigados || processing.diskformat == DiskFormat.diskspare)
                            processing.StartProcessing(Platform.Amiga);
                        else
                            processing.StartProcessing(Platform.PC);
                        this.UpdateForm();
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
                    processing.StartProcessing(Platform.Amiga);
                else
                    processing.StartProcessing(Platform.PC);
                processing.SectorMap.RefreshSectorMap();
                this.UpdateForm();
            }
            MinvScrollBar.Value = 0x32;
            for (i = 0x07; i < 0x18; i += 2)
            {
                SettingsLabel.Text = "2. i = " + i;
                if (processing.stop == 1)
                    break;
                FourvScrollBar.Value = i;
                if ((int)processing.diskformat <= 2)
                    processing.StartProcessing(Platform.Amiga);
                else
                    processing.StartProcessing(Platform.PC);
                processing.SectorMap.RefreshSectorMap();
                this.UpdateForm();
            }

            scanactive = false;
            processing.stop = 0;
        }

        private void AdaptiveScan()
        {
            float i;

            for (i = 0.6f; i < 2f; i += 0.2f)
            {
                //processing.sectordata2.Clear();
                if (processing.stop == 1)
                    break;
                RateOfChangeUpDown.Value = (decimal)i;
                Application.DoEvents();
                if (processing.ProcSettings.platform == 0)
                    ProcessPC();
                else
                    ProcessAmiga();
            }
            //processing.sectormap.RefreshSectorMap();
        }

        private void AdaptiveScan2()
        {
            int OFFSET = OffsetvScrollBar1.Value;
            int step = (int)iESStart.Value;
            for (int l = -12; l < 13; l += step)
                for (float i = 0.6f; i < 2f; i += 0.2f)
                {
                    if (processing.stop == 1)
                        break;
                    RateOfChangeUpDown.Value = (decimal)i;
                    OffsetvScrollBar1.Value = OFFSET + l;

                    Application.DoEvents();

                    if (processing.ProcSettings.platform == 0)
                        ProcessPC();
                    else
                        ProcessAmiga();
                }

            OffsetvScrollBar1.Value = OFFSET;
        }

        private void AdaptiveDeepScan()
        {
            processing.processing = 1;
            int OFFSET = OffsetvScrollBar1.Value;
            int step = (int)iESStart.Value;
            int oldRateOfChange2UpDown = (int)RateOfChange2UpDown.Value;
            for (float k = 2048; k > 0.5; k /= 2)
            {
                RateOfChange2UpDown.Value = (int)k;
                for (int l = -12; l < 13; l += step)
                {
                    if (processing.stop == 1)
                        break;

                    OffsetvScrollBar1.Value = OFFSET + l;

                    Application.DoEvents();
                    if (processing.ProcSettings.platform == 0)
                        ProcessPC();
                    else
                        ProcessAmiga();
                }
            }
            OffsetvScrollBar1.Value = OFFSET;
            processing.processing = 0;
            RateOfChange2UpDown.Value = (decimal)oldRateOfChange2UpDown;
            //processing.sectormap.RefreshSectorMap();
        }

        private void AdaptiveScan4()
        {
            int OFFSET = OffsetvScrollBar1.Value;
            int step = (int)iESStart.Value;

            for (int l = 0; l < 4; l += step)
                for (float i = 0.6f; i < 2f; i += 0.2f)
                {
                    if (processing.stop == 1)
                        break;
                    RateOfChangeUpDown.Value = (decimal)i;
                    OffsetvScrollBar1.Value = OFFSET + l;

                    Application.DoEvents();
                    if (processing.ProcSettings.platform == 0)
                        ProcessPC();
                    else
                        ProcessAmiga();
                }

            OffsetvScrollBar1.Value = OFFSET;
        }

        private void AdaptiveNarrow()
        {
            int FOUR = FourvScrollBar.Value;
            int EIGHT = EightvScrollBar.Value;
            int OFFSET = OffsetvScrollBar1.Value;
            int step = (int)iESStart.Value;

            for (int l = 0; l < 8; l += step)
            //for (i = 0.5f; i < 2f; i += 0.2f)
            {
                if (processing.stop == 1) break;
                //RateOfChangeUpDown.Value = (decimal)i;
                FourvScrollBar.Value = FOUR + l;
                EightvScrollBar.Value = EIGHT - l;
                Application.DoEvents();
                if (processing.ProcSettings.platform == 0)
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
            int FOUR = FourvScrollBar.Value;
            int EIGHT = EightvScrollBar.Value;
            int OFFSET = OffsetvScrollBar1.Value;
            int step = (int)iESStart.Value;

            for (int l  = 0; l < 8; l += step)
                for (float i = 0.5f; i < 2f; i += 0.2f)
                {
                    if (processing.stop == 1) break;
                    RateOfChangeUpDown.Value = (decimal)i;
                    FourvScrollBar.Value = FOUR + l;
                    EightvScrollBar.Value = EIGHT - l;
                    Application.DoEvents();
                    processing.stop = 0;
                    if (processing.ProcSettings.platform == 0)
                        ProcessPC();
                    else
                        ProcessAmiga();
                }
            FourvScrollBar.Value = FOUR;
            EightvScrollBar.Value = EIGHT;
            OffsetvScrollBar1.Value = OFFSET;
        }

        #endregion

        #region Scope related

        private void ProcessOscilloscopeCapturedTrack()
        {
            if (graphwaveform[2] == null) return;
            int length = graphwaveform[2].Length;

            // The captured data has the high and low at 119 and 108
            processing.Indexrxbuf = 0;
            int old = 0;
            int period;
            //Smoothing pass
            for (int i = 0; i < length; i++)
            {
                if (graphwaveform[2][i] < 113)
                {
                    period = i - old;
                    processing.RxBbuf[processing.Indexrxbuf++] = (byte)period;
                    old = i;
                    for (int j = 0; j < 100; j++) // skip to end of pulse
                    {
                        if (graphwaveform[2][i] > 113) break;
                        if (i < length - 1) i++;
                        else break;
                    }
                }

            }

            rxbufEndUpDown.Maximum = processing.Indexrxbuf;
            rxbufStartUpDown.Maximum = processing.Indexrxbuf;

            rxbufEndUpDown.Value = processing.Indexrxbuf;
            HistogramhScrollBar1.Minimum = 0;
            HistogramhScrollBar1.Maximum = processing.Indexrxbuf;
            oscilloscope.graphset.SetAllChanged();

            oscilloscope.graphset.UpdateGraphs();
        }

        private void CaptureOscilloscopeTrack()
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
                controlfloppy.rxbuf = processing.RxBbuf;

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
                controlfloppy.rxbuf = processing.RxBbuf;

                // Callbacks
                controlfloppy.updateHistoAndSliders = UpdateHistoAndSliders;
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
                            if (processing.SectorMap.sectorok[i, j] == SectorMapStatus.empty || processing.SectorMap.sectorok[i, j] == SectorMapStatus.HeadOkDataBad)
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

        public void EditScopePlotUndo()
        {
            if (oscilloscope.graphset.Graphs.Count > 1)
                if (oscilloscope.graphset.Graphs[0].Undo.Count > 0)
                {
                    var undo = oscilloscope.graphset.Graphs[0].Undo;
                    int undolistindex = undo.Count - 1;
                    int offset = undo[undolistindex].offset;
                    byte[] d = undo[undolistindex].undodata;
                    int length = d.Length;

                    for (int i = 0; i < length; i++)
                    {
                        oscilloscope.graphset.Graphs[0].Data[i + offset] = d[i];
                    }
                    undo.Remove(undo[undolistindex]);

                    oscilloscope.graphset.Graphs[0].Changed = true;
                    oscilloscope.graphset.UpdateGraphs();
                }

        }

        public void EditScopePlotCopy()
        {
            Graph2 src = oscilloscope.graphset.Graphs[0];

            if (oscilloscope.graphset.Graphs.Count < 5)
                oscilloscope.graphset.AddGraph((byte[])src.Data.Clone());

            Graph2 dst = oscilloscope.graphset.Graphs[4];

            dst.Changed = true;
            dst.Data = Clone4(src.Data);
            dst.DataLength = src.DataLength;
            dst.DataOffset = src.DataOffset;
            dst.Density = src.Density;

            dst.YScale = src.YScale;
            dst.YOffset = src.YOffset;
            src.ZOrder = 10;
            dst.ZOrder = 9;
            oscilloscope.graphset.UpdateGraphs();
        }

        public void JumpTocomboBoxUpdateScopePlot()
        {
            int grphcnt = oscilloscope.graphset.Graphs.Count;
            int i;
            int index = JumpTocomboBox.SelectedIndex;
            ComboboxItem item;
            item = (ComboboxItem)JumpTocomboBox.Items[index];

            int id = item.Id;
            //tbreceived.Append("Item: " + id+"\r\n");


            // First position the scatterplot on the selected area
            /*int offset = 0;
            for (i = 1; i < processing.sectordata2.Count; i++)
            {
                if (processing.sectordata2[i].rxbufMarkerPositions > scatterplot.RxbufClickIndex)
                {
                    offset = scatterplot.RxbufClickIndex - processing.sectordata2[i - 1].rxbufMarkerPositions;
                    break;
                }
            }*/
            //ScatterMinTrackBar.Value = offset;
            //ScatterMaxTrackBar.Value = offset + 14;
            //updateECInterface();

            int SectorViewEnd = 0;
            var SectorHeaderOffset = 5000;
            if (processing.sectordata2.Count > id + 1)
            {
                SectorHeaderOffset = processing.sectordata2[processing.sectordata2[id].HeaderIndex].rxbufMarkerPositions;
                if (processing.sectordata2.Count > id + 1) SectorViewEnd = processing.sectordata2[id + 1].rxbufMarkerPositions;
            }
            if (SectorViewEnd - SectorHeaderOffset > 10000) SectorViewEnd = SectorHeaderOffset + 10000;
            int scatoffset = SectorHeaderOffset + (int)ScatterMinTrackBar.Value + (int)ScatterOffsetTrackBar.Value;
            int scatlength = SectorViewEnd + (int)ScatterMaxTrackBar.Value + (int)ScatterOffsetTrackBar.Value - scatoffset;

            /*int scatoffset = processing.sectordata2[id].rxbufMarkerPositions + (int)ScatterMinTrackBar.Value + (int)ScatterOffsetTrackBar.Value;
            int scatlength = processing.sectordata2[id].rxbufMarkerPositions + (int)ScatterMaxTrackBar.Value + (int)ScatterOffsetTrackBar.Value - scatoffset;*/
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
                oscilloscope.graphset.Graphs[i].DataLength = 2000;
                oscilloscope.graphset.Graphs[i].DataOffset = processing.Rxbuftograph[graphoffset] - 1000;

                if (oscilloscope.graphset.Graphs[i].DataOffset < 0)
                    oscilloscope.graphset.Graphs[i].DataOffset = 0;

                oscilloscope.graphset.Graphs[i].Changed = true;
                oscilloscope.graphset.Graphs[i].Density = 1;
            }
            //tbreceived.Append("rxbuf pos: "+ (processing.sectordata2[id].rxbufMarkerPositions + offset + 1000));
            oscilloscope.graphset.UpdateGraphs();
            MainTabControl.SelectedTab = QuickTab;
        }

        #endregion

        #region Presets

        private void SetThresholdLabels(ProcessingType type)
        {
            switch (type)
            {
                case ProcessingType.adaptive1:
                case ProcessingType.adaptive2:
                case ProcessingType.adaptive3:
                case ProcessingType.adaptiveEntropy:
                case ProcessingType.adaptivePredict:
                    PMinLabel.Text = QMinLabel.Text = "min";
                    PFourSixLabel.Text = QFourSixLabel.Text = "Peak1";
                    PSixEightLabel.Text = QSixEightLabel.Text = "Peak2";
                    PMaxLabel.Text = QMaxLabel.Text = "Peak3";
                    POffsetLabel.Text = QOffsetLabel.Text = "Offset";
                    break;
                case ProcessingType.normal:
                    PMinLabel.Text = QMinLabel.Text = "min";
                    PFourSixLabel.Text = QFourSixLabel.Text = "4/6";
                    PSixEightLabel.Text = QSixEightLabel.Text = "6/8";
                    PMaxLabel.Text = QMaxLabel.Text = "max";
                    POffsetLabel.Text = QOffsetLabel.Text = "Offset";
                    break;
                case ProcessingType.aufit:
                    PMinLabel.Text = QMinLabel.Text = "Factor";
                    PFourSixLabel.Text = QFourSixLabel.Text = "Offset";
                    PSixEightLabel.Text = QSixEightLabel.Text = "";
                    PMaxLabel.Text = QMaxLabel.Text = "";
                    POffsetLabel.Text = QOffsetLabel.Text = "";
                    break;
                default:
                    PMinLabel.Text = QMinLabel.Text = "min";
                    PFourSixLabel.Text = QFourSixLabel.Text = "Peak1";
                    PSixEightLabel.Text = QSixEightLabel.Text = "Peak2";
                    PMaxLabel.Text = QMaxLabel.Text = "Peak3";
                    POffsetLabel.Text = QOffsetLabel.Text = "Offset";
                    break;
            }
        }

        private void CopyThresholdsToQuick()
        {
            var scanold = scanactive;
            //Prevent change event loop
            scanactive = true;
            QMinUpDown.Value = MinvScrollBar.Value;
            QFourSixUpDown.Value = FourvScrollBar.Value;
            QSixEightUpDown.Value = SixvScrollBar.Value;
            QMaxUpDown.Value = EightvScrollBar.Value;
            QOffsetUpDown.Value = OffsetvScrollBar1.Value;
            scanactive = scanold;
        }

        private void CopyThresholdsToProcessing()
        {
            var scanold = scanactive;
            //Prevent change event loop
            scanactive = true;
            MinvScrollBar.Value = (int)QMinUpDown.Value;
            FourvScrollBar.Value = (int)QFourSixUpDown.Value;
            SixvScrollBar.Value = (int)QSixEightUpDown.Value;
            EightvScrollBar.Value = (int)QMaxUpDown.Value;
            OffsetvScrollBar1.Value = (int)QOffsetUpDown.Value;
            scanactive = scanold;
        }

		#endregion

		#region Error Correction

        private void EntropySplice()
        {
            int IndexS1, IndexSx, listlength, Threadid;
            var selected = BadSectorListBox.SelectedIndices;
            listlength = selected.Count;

            if (listlength >= 1)
            {
                // Get the first sector from the list
                IndexS1 = ((Badsectorkeyval)BadSectorListBox.Items[selected[0]]).Id;
                Threadid = ((Badsectorkeyval)BadSectorListBox.Items[selected[0]]).Threadid;
                var Sectordata = processing.sectordata2[IndexS1];
                var Sectorheader = processing.sectordata2[Sectordata.HeaderIndex];
                var Offset1 = Sectordata.rxbufMarkerPositions;
                byte ResultingVal;
                var SectordataEnd = processing.sectordata2[IndexS1 + 1];
                int Length = SectordataEnd.rxbufMarkerPositions - Offset1;
                float LowestEntropy, ThisEntropy;
                if (Length > 10000) Length = 10000;
                if (processing.entropy == null)
                {
                    textBoxReceived.AppendText("No entropy data! Process data first with error correction enabled.");
                    return;
                }

                for (int j = 0; j < Length; j++)
                {
                    ResultingVal = processing.RxBbuf[j + Offset1];
                    LowestEntropy = Math.Abs(processing.entropy[j + Offset1]);
                    for (int i = 1; i < listlength; i++)
                    {
                        if (processing.stop == 1)
                            break;
                        IndexSx = ((Badsectorkeyval)BadSectorListBox.Items[selected[i]]).Id;
                        Threadid = ((Badsectorkeyval)BadSectorListBox.Items[selected[i]]).Threadid;

                        var Sectordata2   = processing.sectordata2[IndexSx];
                        var Sectorheader2 = processing.sectordata2[Sectordata2.HeaderIndex];
                        
                        var offset2 = Sectordata2.rxbufMarkerPositions;
                        
                        ThisEntropy = Math.Abs(processing.entropy[j + offset2]);
                        
                        if (ThisEntropy < (LowestEntropy+10)) ResultingVal = processing.RxBbuf[j + offset2];
                    }
                    processing.RxBbuf[j + Offset1] = ResultingVal;
                }
            }
            else
            {
                textBoxReceived.AppendText("Error, no data selected.");
                return;
            }
        }

        private void AvgPeriodsFromListSelection()
        {
            int IndexS1, listlength, Threadid;
            var selected = BadSectorListBox.SelectedIndices;
            listlength = selected.Count;

            if (listlength >= 1)
            {
                // Get the first sector from the list
                IndexS1 = ((Badsectorkeyval)BadSectorListBox.Items[selected[0]]).Id;
                Threadid = ((Badsectorkeyval)BadSectorListBox.Items[selected[0]]).Threadid;
                var Sectordata = processing.sectordata2[IndexS1];
                var Sectorheader = processing.sectordata2[Sectordata.HeaderIndex];
                var Offset1 = Sectorheader.rxbufMarkerPositions;
                int ResultingVal;
                var SectordataEnd = processing.sectordata2[IndexS1+1];
                int Length = SectordataEnd.rxbufMarkerPositions - Offset1;
                if (Length > 10000) Length = 10000;
                for (int j = 0; j < Length; j++)
                {
                    ResultingVal = processing.RxBbuf[j + Offset1];
                    for (int i = 1; i < listlength; i++)
                    {
                        if (processing.stop == 1)
                            break;
                        IndexS1 = ((Badsectorkeyval)BadSectorListBox.Items[selected[i]]).Id;
                        Threadid = ((Badsectorkeyval)BadSectorListBox.Items[selected[i]]).Threadid;
                        var sectordata2 = processing.sectordata2[IndexS1];
                        var sectorheader2 = processing.sectordata2[Sectordata.HeaderIndex];
                        var offset2 = sectorheader2.rxbufMarkerPositions;
                        ResultingVal += processing.RxBbuf[j+offset2];
                    }
                    processing.RxBbuf[j + Offset1] = (byte)(ResultingVal/(listlength/1.2f));
                }
            }
            else
            {
                textBoxReceived.AppendText("Error, no data selected.");
                return;
            }
        }

        private void ErrorCorrectRealign4E()
        {
            int indexS1, listlength, threadid;
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
                for (int i = 0; i < listlength; i++)
                {
                    if (processing.stop == 1)
                        break;
                    indexS1 = ((Badsectorkeyval)BadSectorListBox.Items[selected[i]]).Id;
                    threadid = ((Badsectorkeyval)BadSectorListBox.Items[selected[i]]).Threadid;
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
            BadSectorListBox.Items.Add(new Badsectorkeyval
            {
                Name = "i: " + badsectorcnt2 + " " + key,
                Id = badsectorcnt2,
                Threadid = sectordata.threadid
            });
            //JumpTocomboBox.Items.Add()
            JumpTocomboBox.Items.Add(new ComboboxItem
            {
                Text = "i: " + badsectorcnt2 + " " + key,
                Id = badsectorcnt2,
            });
        }

		public void ECMFMByteEnc()
		{
			int indexS1, threadid;
			ECSettings ecSettings = new ECSettings
			{
				sectortextbox = textBoxSector
			};

			if (BadSectorListBox.SelectedIndices.Count >= 1)
			{
				indexS1 = ((Badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).Id;
				threadid = ((Badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).Threadid;
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
			if (processing.ProcSettings.platform == 0)
				processing.ProcessClusterMFMEnc(ecSettings);
			else processing.ProcessClusterAmigaMFMEnc(ecSettings);
		}

		public void ECIteratorTest()
		{
			int[] combi = new int[32];
			int[] combilimit = new int[32];
			int i, j, q, k;
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

		private void ECZoomOutBtnHandler()
		{
			int indexS1;

			if (BadSectorListBox.SelectedIndices.Count >= 1)
			{
				indexS1 = ((Badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).Id;

				int sectorlength = processing.sectordata2[indexS1].sectorlength;

				int factor = sectorlength / 512;

				ScatterMinTrackBar.Value = 0;
				ScatterMaxTrackBar.Value = 4500 * factor;
				UpdateECInterface();
			}
		}

		public void DoErrorCorrectionOnSelection()
		{
			int indexS1, threadid;

			ECSettings ecSettings = new ECSettings
			{
				sectortextbox = textBoxSector
			};

			if (BadSectorListBox.SelectedIndices.Count >= 1)
			{
				indexS1 = ((Badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).Id;
				threadid = ((Badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).Threadid;
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
			if (processing.ProcSettings.platform == 0)
			{
				processing.ECCluster2(ecSettings);
			}
			else processing.ProcessClusterAmiga(ecSettings);
		}

		#endregion

		#region GUI

		public void SectorMapInteractions(MouseEventArgs e)
		{
			RichTextBox rtb = null;
			if (MainTabControl.SelectedTab == QuickTab)
			{
				rtb = QrtbSectorMap;
			}
			else if (MainTabControl.SelectedTab == ProcessingTab)
			{
				rtb = rtbSectorMap;
			}
			ContextMenuStrip smmenu = new ContextMenuStrip();
			int sector, track;
			int i;
			int div = processing.sectorspertrack + 6;
			LimitToTrackUpDown.Value = track = (rtb.SelectionStart / div);
			LimitToSectorUpDown.Value = sector = (rtb.SelectionStart % div - 5);

			if (sector < 0) return;
			TrackUpDown.Value = track;
			SectorUpDown.Value = sector;

			if (e.Button == MouseButtons.Left)
			{
				tbreceived.Append("Track: " + track + " sector: " + sector + " div:" + div + "\r\n");
				for (i = 0; i < processing.sectordata2.Count; i++)
				{
					if (processing.sectordata2 == null) continue;
					if (processing.sectordata2.Count == 0) continue;
					if (!(processing.sectordata2[i].trackhead == track
						   && processing.sectordata2[i].sector == sector)) continue;
					if (!(processing.sectordata2[i].MarkerType == MarkerType.data
						|| processing.sectordata2[i].MarkerType == MarkerType.headerAndData)) continue;
					if (processing.sectordata2[i].MarkerType == MarkerType.headerAndData)
					{
						if (processing.sectordata2[i].Status != processing.SectorMap.sectorok[track, sector]) continue;
						if (processing.sectordata2.Count - 1 <= i) continue;
					}
					else
					{
						if (processing.sectordata2[i].Status != processing.SectorMap.sectorok[track, sector]) continue;
						if (processing.sectordata2.Count - 1 <= i) continue;
					}
					scatterplot.AnScatViewlargeoffset = processing.sectordata2[i].rxbufMarkerPositions - 50;
					scatterplot.AnScatViewoffset = 0;
					int markerSize = 2;
					if (processing.ProcSettings.platform == Platform.Amiga) markerSize = 1;
					scatterplot.AnScatViewlength = processing.sectordata2[i + markerSize].rxbufMarkerPositions - scatterplot.AnScatViewlargeoffset + 100;
					//tbreceived.Append("AnScatViewOffset"+ AnScatViewoffset+"\r\n");
					scatterplot.UpdateScatterPlot();
					break;
				}
			}
			else if (e.Button == MouseButtons.Right)
			{
				int index = rtb.GetCharIndexFromPosition(new Point(e.X, e.Y));
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

				menudata[menudataindex] = new SectorMapContextMenu
				{
					Sector = sector,
					Track = track,
					Duration = 1000,
					Cmd = 0
				};
				item[menudataindex] = smmenu.Items.Add("Recapture T" + track.ToString("D3") + " 1 sec", MainTabControlImageList.Images[0]);
				item[menudataindex].Tag = menudata[menudataindex];

				menudata[menudataindex] = new SectorMapContextMenu
				{
					Sector = sector,
					Track = track,
					Duration = 5000,
					Cmd = 0
				};
				item[menudataindex] = smmenu.Items.Add("Recapture T" + track.ToString("D3") + " 5 sec", MainTabControlImageList.Images[0]);
				item[menudataindex].Tag = menudata[menudataindex];

				// Capture tab
				menudataindex++;
				menudata[menudataindex] = new SectorMapContextMenu
				{
					Sector = sector,
					Track = track,
					Duration = 50000,
					Cmd = 0
				};
				item[menudataindex] = smmenu.Items.Add("Recapture T" + track.ToString("D3") + " 50 sec", MainTabControlImageList.Images[0]);
				item[menudataindex].Tag = menudata[menudataindex];

				//Error correction tab
				menudataindex++;
				menudata[menudataindex] = new SectorMapContextMenu
				{
					Sector = sector,
					Track = track,
					Cmd = 1
				};
				item[menudataindex] = smmenu.Items.Add("Error Correct T" + track.ToString("D3") + " S" + sector, MainTabControlImageList.Images[1]);
				item[menudataindex].Tag = menudata[menudataindex];

				//Scope waveform capture
				menudataindex++;
				menudata[menudataindex] = new SectorMapContextMenu
				{
					Sector = sector,
					Track = track,
					Cmd = 2
				};
				item[menudataindex] = smmenu.Items.Add("Get waveform T" + track.ToString("D3") + " S" + sector, MainTabControlImageList.Images[2]);
				item[menudataindex].Tag = menudata[menudataindex];

				//Select rxdata
				menudataindex++;
				menudata[menudataindex] = new SectorMapContextMenu
				{
					Sector = sector,
					Track = track,
					Cmd = 3
				};
				item[menudataindex] = smmenu.Items.Add("Limit rxdata T" + track.ToString("D3") + " S" + sector, MainTabControlImageList.Images[2]);
				item[menudataindex].Tag = menudata[menudataindex];

				// Limit processing to Track/Sector
				menudataindex++;
				menudata[menudataindex] = new SectorMapContextMenu
				{
					Sector = sector,
					Track = track,
					Cmd = 4
				};
				item[menudataindex] = smmenu.Items.Add("Limit processing T" + track.ToString("D3") + " S" + sector, MainTabControlImageList.Images[2]);
				item[menudataindex].Tag = menudata[menudataindex];

				Point ShowHere = new Point(Cursor.Position.X, Cursor.Position.Y + 10);
				smmenu.Show(ShowHere);
			}
		}

		public void SectorMapRightclickMenuHandler(ToolStripItemClickedEventArgs e)
		{
			SectorMapContextMenu menudata = (SectorMapContextMenu)e.ClickedItem.Tag;
			// recapture 1 sec
			if (menudata.Cmd == 0)
			{
				tbreceived.Append("Track: " + menudata.Track.ToString("D3") + " S" + menudata.Sector + "\r\n");
				//MainTabControl.SelectedTab = CaptureTab;
				StartTrackUpDown.Value = QStartTrackUpDown.Value = menudata.Track;
				EndTracksUpDown.Value = QEndTracksUpDown.Value = menudata.Track;
				TrackDurationUpDown.Value = QTrackDurationUpDown.Value = menudata.Duration;
			}
			// recapture 5 sec
			else if (menudata.Cmd == 1)
			{
				MainTabControl.SelectedTab = ErrorCorrectionTab;
				Track1UpDown.Value = menudata.Track;
				Sector1UpDown.Value = menudata.Sector;
				Track2UpDown.Value = menudata.Track;
				Sector2UpDown.Value = menudata.Sector;
			}
			// recapture 50 sec
			else if (menudata.Cmd == 2)
			{
				MainTabControl.SelectedTab = NetworkTab;
				NetworkCaptureTrackStartUpDown.Value = menudata.Track;
				NetworkCaptureTrackEndUpDown.Value = menudata.Track;
			}
			// Set track/sector error correction
			else if (menudata.Cmd == 3)
			{
				int i;
				var sd = processing.sectordata2;
				for (i = 0; i < processing.sectordata2.Count; i++)
				{
					if (sd[i].sector == menudata.Sector && sd[i].trackhead == menudata.Track && sd[i].Status == SectorMapStatus.HeadOkDataBad)
					{
						rxbufStartUpDown.Maximum = processing.Indexrxbuf;
						rxbufStartUpDown.Value = sd[i].rxbufMarkerPositions - 100;
						rxbufEndUpDown.Value = 15000;
						break;
					}
				}
			}
			// Limit processing track/sector
			else if (menudata.Cmd == 4)
			{
				QLimitTSCheckBox.Checked = true;
				QLimitToTrackUpDown.Value = menudata.Track;
				QLimitToSectorUpDown.Value = menudata.Sector;

				LimitTSCheckBox.Checked = true;
				LimitToTrackUpDown.Value = menudata.Track;
				LimitToSectorUpDown.Value = menudata.Sector;
			}
		}

		void DoChangeProcMode()
		{
			ProcessingType procmode = ProcessingType.adaptive1;
			if (ProcessingModeComboBox.SelectedItem.ToString() != "")
				procmode = (ProcessingType)Enum.Parse(typeof(ProcessingType), ProcessingModeComboBox.SelectedItem.ToString(), true);
			tbreceived.Append("Selected: " + procmode.ToString() + "\r\n");
			SetThresholdLabels(procmode);

			switch (procmode)
			{
				case ProcessingType.normal:
					FindPeaks();
					EightvScrollBar.Value = 0xff;
					scatterplot.ShowEntropy = false;
					break;
				case ProcessingType.aufit:
					MinvScrollBar.Value = 33;
					FourvScrollBar.Value = 0x0C;
					OffsetvScrollBar1.Value = 0;
					scatterplot.ShowEntropy = false;
					break;
				case ProcessingType.adaptive1:
					scatterplot.ShowEntropy = true;
					RateOfChangeUpDown.Value = (decimal)1.1;
					RateOfChange2UpDown.Value = 1300;

					FindPeaks();
					break;
				case ProcessingType.adaptive2:
				case ProcessingType.adaptive3:
					RateOfChangeUpDown.Value = (decimal)1.1;
					RateOfChange2UpDown.Value = 1300;

					FindPeaks();
					scatterplot.ShowEntropy = false;
					break;
				case ProcessingType.adaptivePredict:
					RateOfChangeUpDown.Value = (decimal)3;
					RateOfChange2UpDown.Value = 600;
					FindPeaks();
					scatterplot.ShowEntropy = false;
					break;
				case ProcessingType.adaptiveEntropy:
					scatterplot.ShowEntropy = true;
					break;
			}

			scanactive = true; // prevent triggering events on updown controls
			CopyThresholdsToQuick();
			scanactive = false;
			UpdateSliderLabels();
		}

		private void SetGuiMode(string mode)
		{
			GuiMode = mode;
			if (mode == "basic")
			{
				basicModeToolStripMenuItem.Checked = true;
				advancedModeToolStripMenuItem.Checked = false;
				devModeToolStripMenuItem.Checked = false;

				MainTabControl.TabPages.Remove(CaptureTab);
				MainTabControl.TabPages.Remove(ProcessingTab);
				MainTabControl.TabPages.Remove(AnalysisPage);
				MainTabControl.TabPages.Remove(AnalysisTab2);
				MainTabControl.TabPages.Remove(NetworkTab);
			}

			if (mode == "advanced")
			{
				basicModeToolStripMenuItem.Checked = false;
				advancedModeToolStripMenuItem.Checked = true;
				devModeToolStripMenuItem.Checked = false;

				MainTabControl.TabPages.Remove(CaptureTab);
				MainTabControl.TabPages.Remove(ProcessingTab);
				MainTabControl.TabPages.Remove(AnalysisPage);
				if (!MainTabControl.TabPages.Contains(AnalysisTab2)) MainTabControl.TabPages.Add(AnalysisTab2);
				if (!MainTabControl.TabPages.Contains(NetworkTab)) MainTabControl.TabPages.Add(NetworkTab);
			}

			if (mode == "dev")
			{
				basicModeToolStripMenuItem.Checked = false;
				advancedModeToolStripMenuItem.Checked = false;
				devModeToolStripMenuItem.Checked = true;

				if (!MainTabControl.TabPages.Contains(CaptureTab)) MainTabControl.TabPages.Add(CaptureTab);
				if (!MainTabControl.TabPages.Contains(ProcessingTab)) MainTabControl.TabPages.Add(ProcessingTab);
				if (!MainTabControl.TabPages.Contains(AnalysisPage)) MainTabControl.TabPages.Add(AnalysisPage);
				if (!MainTabControl.TabPages.Contains(AnalysisTab2)) MainTabControl.TabPages.Add(AnalysisTab2);
				if (!MainTabControl.TabPages.Contains(NetworkTab)) MainTabControl.TabPages.Add(NetworkTab);
			}

		}

		private void ShowDiskSector()
		{
			int i, track, sector;
			byte databyte;
			StringBuilder bytesstring = new StringBuilder();
			StringBuilder txtstring = new StringBuilder();

			track = (int)TrackUpDown.Value;
			sector = (int)SectorUpDown.Value;
			int index;
			int size = processing.disk.Length;
			for (i = 0; i < 512; i++)
			{
				index = track * 512 * processing.sectorspertrack + (512 * sector) + i;
				if (index > size)
					break;
				databyte = (byte)processing.disk[index];
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

		private void ShowSectorData()
		{
			int indexS1, threadid;
			int i;
			if (BadSectorListBox.SelectedIndices.Count >= 1)
			{
				indexS1 = ((Badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).Id;
				threadid = ((Badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).Threadid;

			}
			else return;

			antbSectorData.Clear();
			if (processing.sectordata2[indexS1].sectorbytes != null)
				antbSectorData.Text = (processing.BytesToHexa(processing.sectordata2[indexS1].sectorbytes, 0, processing.sectordata2[indexS1].sectorbytes.Length));
			else antbSectorData.Text = "";

			int mfmoffset = processing.sectordata2[indexS1].MarkerPositions;
			int length = (processing.sectordata2[indexS1].sectorlength + 1000) * 16;
			//threadid = sectordata[threadid][indexS1].threadid;
			StringBuilder mfmtxt = new StringBuilder();
			var mfmLength = processing.mfms[threadid].Length;
			for (i = 0; i < length; i++)
			{
				if (mfmLength > i + mfmoffset)
					mfmtxt.Append((char)(processing.mfms[threadid][i + mfmoffset] + 48));
			}
			ECtbMFM.Text = mfmtxt.ToString();
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

		#endregion

		#region Data handling

		private void ResetProcessedData()
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

		private void ResetInput()
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
			processing.RxBbuf = null;
			processing.RxBbuf = new byte[200000];

			//Array.Clear(processing.rxbuf, 0, processing.rxbuf.Length);
			//TrackPosInrxdatacount = 0;
			processing.Indexrxbuf = 0;
			processing.mfmsindex = 0;

			rxbufStartUpDown.Maximum = processing.Indexrxbuf;
			rxbufEndUpDown.Maximum = processing.Indexrxbuf;
			rxbufEndUpDown.Value = processing.Indexrxbuf;
			UpdateHistoAndSliders();
			scatterplot.AnScatViewlength = 100000;
			scatterplot.AnScatViewoffset = 0;
			scatterplot.AnScatViewlargeoffset = 0;
			scatterplot.AnScatViewoffsetOld = 0;
			scatterplot.UpdateScatterPlot();
			UpdateHistoAndScatterplot();
			BytesReceivedLabel.Text = String.Format("{0:n0}", processing.Indexrxbuf);
			GC.Collect();
		}

		// Resets all data that was produced by processing but keeps rxbuf intact
		private void Resetoutput()
		{
			HandleTabSwitching();
			var oldscrollvalue = HistogramhScrollBar1.Value;
			var oldscrollmaxvalue = HistogramhScrollBar1.Maximum;
			var qoldscrollvalue = QHistogramhScrollBar1.Value;
			var qoldscrollmaxvalue = QHistogramhScrollBar1.Maximum;
			var rxbuftemp = (byte[])processing.RxBbuf.Clone();
			for (var i = 0; i < processing.mfms.Length; i++)
				processing.mfms[i] = null;
			processing.RxBbuf = null;
			processing.disk = null;
			processing.mfmlengths = null;
			processing.badsectorhash = null;
			processing.progresses = null;
			processing.progressesstart = null;
			processing.progressesend = null;
			processing.ProcessStatus = null;
			processing.SectorMap.sectorok = null;
			processing.SectorMap.sectorokLatestScan = null;
			processing.SectorMap = null;
			processing = null;

			ECHisto = new Histogram();
			ScatterHisto = new Histogram();
			processing = new FDDProcessing
			{
				RxBbuf = rxbuftemp
			};
			processing.Indexrxbuf = processing.RxBbuf.Length / 2; // Divide by two as loading .bin files doubles the buffer
			processing.GetProcSettingsCallback += GetProcSettingsCallback;
			processing.RtbSectorMap = rtbSectorMap;
			processing.TBReceived = tbreceived;
			processing.SectorMap.SectorMapUpdateGUICallback += SectorMapUpdateGUICallback;
			processing.SectorMap.rtbSectorMap = rtbSectorMap;

			fileio.processing = processing;
			if (controlfloppy.serialPort1.IsOpen)
				controlfloppy.Disconnect();
			controlfloppy.Disconnect();
			controlfloppy.tempbuffer.Clear();
			controlfloppy.rxbuf = null;
			controlfloppy = null;
			controlfloppy = new ControlFloppy
			{
				rxbuf = rxbuftemp,
				processing = processing
			};
			scatterplot.RemoveEvents();
			scatterplot.Rxbuf = null;
			scatterplot.UpdateEvent -= UpdateAnScatterPlot;
			scatterplot.ShowGraph -= ScatterPlotShowGraphCallback;

			scatterplot = null;
			scatterplot = new ScatterPlot(processing, processing.sectordata2, 0, 0, ScatterPictureBox)
			{
				Tbreiceved = tbreceived,
				Rxbuf = rxbuftemp,
				EditScatterplot = EditScatterPlotcheckBox.Checked
			};
			scatterplot.UpdateEvent += UpdateAnScatterPlot;
			scatterplot.ShowGraph += ScatterPlotShowGraphCallback;

			EditOptioncomboBox.SelectedIndex = 0;
			EditModecomboBox.SelectedIndex = 0;
			textBoxReceived.AppendText("PortName: " + selectedPortName + "\r\n");

			BadSectorTooltip.Hide();
			timer5.Start();
			GUITimer.Start();
			BluetoRedByteCopyToolBtn.Tag = new int();
			BluetoRedByteCopyToolBtn.Tag = 0;
			ECHisto.SetPanel(AnHistogramPanel);
			ScatterHisto.SetPanel(Histogrampanel1);
			if (processing.Indexrxbuf < 100000)
				scatterplot.AnScatViewlength = processing.Indexrxbuf;
			else scatterplot.AnScatViewlength = 99999;
			scatterplot.AnScatViewoffset = 0;
			scatterplot.UpdateScatterPlot();
			UpdateSliderLabels();
			UpdateAnScatterPlot();

			HistogramhScrollBar1.Maximum = oldscrollmaxvalue;
			HistogramhScrollBar1.Value = oldscrollvalue;
			QHistogramhScrollBar1.Maximum = qoldscrollmaxvalue;
			QHistogramhScrollBar1.Value = qoldscrollvalue;
			HandleTabSwitching();
			GC.Collect();
		}

		public void Setrxbufcontrol()
		{
			//indexrxbuf = processing.indexrxbuf;
			rxbufStartUpDown.Maximum = processing.RxBbuf.Length;
			rxbufEndUpDown.Maximum = processing.RxBbuf.Length;
			rxbufEndUpDown.Value = processing.RxBbuf.Length;
			HistogramhScrollBar1.Minimum = 0;
			HistogramhScrollBar1.Maximum = processing.Indexrxbuf;
			scatterplot.Rxbuf = processing.RxBbuf;
		}

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
            processing = new FDDProcessing
            {
                RtbSectorMap = rtbSectorMap,
                TBReceived = tbreceived
            };
            processing.SectorMap.rtbSectorMap = rtbSectorMap;
            processing.GetProcSettingsCallback += GetProcSettingsCallback;
            processing.SectorMap.SectorMapUpdateGUICallback += SectorMapUpdateGUICallback;

            controlfloppy = new ControlFloppy
            {
                rxbuf = processing.RxBbuf,
                processing = processing
            };

            scatterplot = new ScatterPlot(processing, processing.sectordata2, 0, 0, ScatterPictureBox)
            {
                Tbreiceved = tbreceived,
                Rxbuf = processing.RxBbuf,
            };
            scatterplot.UpdateEvent += UpdateAnScatterPlot;
            scatterplot.ShowGraph += ScatterPlotShowGraphCallback;
            scatterplot.EditScatterplot = EditScatterPlotcheckBox.Checked;
            processing.Indexrxbuf = 0;

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

            fileio = new FileIO
            {
                processing = processing,
                textBoxFilesLoaded = textBoxFilesLoaded,
                tbreceived = tbreceived,
                rtbSectorMap = rtbSectorMap
            };
            fileio.resetinput += ResetInput;

            oscilloscope = new WaveformEdit(GraphPictureBox, fileio, processing, GraphsetGetControlValuesCallback);
            EditOptioncomboBox.SelectedIndex = 0;
            EditModecomboBox.SelectedIndex = 0;
            oscilloscope.updateGraphCallback += UpdateGraphCallback;
            oscilloscope.GraphsetGetControlValuesCallback += GraphsetGetControlValuesCallback;
            oscilloscope.resetinput += ResetInput;
            oscilloscope.FilterGuiUpdateCallback += FilterGuiUpdateCallback;
            oscilloscope.Filter2GuiCallback += Filter2GuiCallback;

            textBoxReceived.AppendText("PortName: " + selectedPortName + "\r\n");

            //comboBoxPort.SelectedItem = "COM9";
            UpdateSliderLabels();

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

            ECHisto.SetPanel(AnHistogramPanel);
            ECHisto.TBReceived = tbreceived;
            ScatterHisto.SetPanel(Histogrampanel1);
            ScatterHisto.TBReceived = tbreceived;
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

            SetThresholdLabels(processing.ProcSettings.processingtype);

            if (HDCheckBox.Checked)
            {
                ScatterHisto.HD = 1;
                processing.ProcSettings.hd = 1;
                //hddiv = 2;
            }
            else
            {
                ScatterHisto.HD = 0;
                processing.ProcSettings.hd = 0;
                //hddiv = 1;
            }

            ProcessStatusLabel.BackColor = Color.Transparent;
            HandleTabSwitching();
            this.ActiveControl = QMinUpDown;
        }

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
                    processing.SectorMap.RefreshSectorMap();
                }
                if (e.KeyCode == Keys.P)
                {
                    RateOfChange2UpDown.Focus();
                    Application.DoEvents();
                    ProcessPC();
                    processing.SectorMap.RefreshSectorMap();
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

        private void Timer1_Tick(object sender, EventArgs e)
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
                BufferSizeLabel.Text = string.Format("{0:n0}", processing.Indexrxbuf);

                controlfloppy.rxbuf = processing.RxBbuf;
                if (processing.RxBbuf.Length > 100000)
                    controlfloppy.recentreadbuflength = 100000; // controlfloppy.recentreadbuflength = processing.indexrxbuf - indexrxbufprevious;
                processing.Indexrxbuf = processing.RxBbuf.Length - 1;
                ControlFloppyScatterplotCallback();
            }

            if (processing.Indexrxbuf > 0)
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
                BytesReceivedLabel.Text = String.Format("{0:n0}", processing.Indexrxbuf);
                //createhistogram1();
            }

            if (ScpOpenFilesDlgUsed == true)
            {
                ScpOpenFilesDlgUsed = false;
                QProcessingGroupBox.Enabled = true;
                UpdateHistoAndScatterplot();
                BytesReceivedLabel.Text = String.Format("{0:n0}", processing.Indexrxbuf);
                //createhistogram1();
            }
        }

        static byte[] Clone4(byte[] array)
        {
            byte[] result = new byte[array.Length];
            Buffer.BlockCopy(array, 0, result, 0, array.Length * sizeof(byte));
            return result;
        }

        private void FindPeaks()
        {

            if (processing.Indexrxbuf == 0) return;
            processing.FindPeaks(HistogramhScrollBar1.Value);
            SuspendLayout();
            int peak1 = processing.Peak1;
            int peak2 = processing.Peak2;
            int peak3 = processing.Peak3;
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

                    QFourSixUpDown.Value = FourvScrollBar.Value = peak1;
                    QSixEightUpDown.Value = SixvScrollBar.Value = peak2;
                    QMaxUpDown.Value = EightvScrollBar.Value = peak3;

                    break;
            }
            Application.DoEvents();
            ResumeLayout();
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
            //updateSliderLabels();
        }
        
    }
}
