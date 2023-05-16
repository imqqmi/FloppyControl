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
        public FloppyControl()
        {
            InitializeFloppyControl();
        }

        private void FloppyControl_KeyDown(object sender, KeyEventArgs e)
        {
            KeyboardShortcutHandler(e);
        }

        // Do the Amiga sector data processing
        private void ProcessAmigaBtn_Click(object sender, EventArgs e)
        {
            SyncControlsBetweenTabs();
            processing.stop = 0;
            ProcessAmiga();
        }

        private void SyncControlsBetweenTabs()
        {
            if(MainTabControl.SelectedTab == QuickTab)
            {
                RateOfChangeUpDown.Value = QRateOfChangeUpDown.Value;
                RateOfChange2UpDown.Value = QRateOfChange2UpDown.Value;
                AdaptOfsset2UpDown.Value = QAdaptOfsset2UpDown.Value;
            }
            else if (MainTabControl.SelectedTab == ProcessingTab)
            {
                QRateOfChangeUpDown.Value = RateOfChangeUpDown.Value;
                QRateOfChange2UpDown.Value = RateOfChange2UpDown.Value;
                QAdaptOfsset2UpDown.Value = AdaptOfsset2UpDown.Value;
            }
        }
        
        private void ProcessAmiga()
        {
            if (ClearDatacheckBox.Checked)
                resetprocesseddata();
            //textBoxReceived.Clear();
            processing.scatterplotstart = scatterplot.AnScatViewlargeoffset + scatterplot.AnScatViewoffset;
            processing.scatterplotend = scatterplot.AnScatViewlargeoffset + scatterplot.AnScatViewoffset + scatterplot.AnScatViewlength;
            processing.StartProcessing(Platform.Amiga);
        }

        private void ProcessPC()
        {
            if (ClearDatacheckBox.Checked)
                resetprocesseddata();
            //textBoxReceived.Clear();
            processing.scatterplotstart = scatterplot.AnScatViewlargeoffset + scatterplot.AnScatViewoffset;
            processing.scatterplotend = scatterplot.AnScatViewlargeoffset + scatterplot.AnScatViewoffset + scatterplot.AnScatViewlength;
            processing.StartProcessing(Platform.PC);
            ChangeDiskTypeComboBox.SelectedItem = processing.diskformat.ToString();
        }

        private void ProcessPCBtn_Click(object sender, EventArgs e)
        {
            SyncControlsBetweenTabs();
            processing.stop = 0;
            ProcessPC();
            HandleTabSwitching();
        }

        public void UpdateForm()
        {
            Application.DoEvents();
        }

        private void DisconnectFromFloppyControlHardware()
        {
            if (controlfloppy.serialPort1.IsOpen)
            {
                controlfloppy.Disconnect();
                LabelStatus.Text = "Disconnected.";
            }
        }
         
        private void OpenBinFilebutton_Click(object sender, EventArgs e)
        {
            fileio.FilesAvailableCallback += FilesAvailableCallback;
            fileio.FilesAvailableCallback -= ScpFilesAvailableCallback;
            fileio.ShowOpenBinFiles();
        }

        private void AddDataButton_Click(object sender, EventArgs e)
        {
            fileio.FilesAvailableCallback += FilesAvailableCallback;
            fileio.FilesAvailableCallback -= ScpFilesAvailableCallback;
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

        private void PresetTrack80_90_Click(object sender, EventArgs e)
        {
            StartTrackUpDown.Value = 80;
            EndTracksUpDown.Value = 90;
            TrackDurationUpDown.Value = 540;

            QStartTrackUpDown.Value = 80;
            QEndTracksUpDown.Value = 90;
            QTrackDurationUpDown.Value = 540;
        }
        
        private void PresetCaptureDefaultBtn_Click(object sender, EventArgs e)
        {
            StartTrackUpDown.Value = 0;
            EndTracksUpDown.Value = 164;
            TrackDurationUpDown.Value = 330;

            QStartTrackUpDown.Value = 0;
            QEndTracksUpDown.Value = 164;
            QTrackDurationUpDown.Value = 330;
        }

        private void PresetCaptureDuration1s_Click(object sender, EventArgs e)
        {
            TrackDurationUpDown.Value = 1000;
            QTrackDurationUpDown.Value = 1000;
        }

        private void PresetTrack78_164_Click(object sender, EventArgs e)
        {
            StartTrackUpDown.Value = 78;
            EndTracksUpDown.Value = 164;
            TrackDurationUpDown.Value = 330;

            QStartTrackUpDown.Value = 78;
            QEndTracksUpDown.Value = 164;
            QTrackDurationUpDown.Value = 330;
        }

        private void PresetCaptureDuration2s_Click(object sender, EventArgs e)
        {
            TrackDurationUpDown.Value = 2000;
            QTrackDurationUpDown.Value = 2000;
        }

        private void PresetCaptureDuration5s_Click(object sender, EventArgs e)
        {
            TrackDurationUpDown.Value = 5000;
            QTrackDurationUpDown.Value = 5000;
        }

        private void PresetCaptureDuration50s_Click(object sender, EventArgs e)
        {
            TrackDurationUpDown.Value = 50000;
            QTrackDurationUpDown.Value = 50000;
        }

        private void Outputfilename_Enter(object sender, EventArgs e)
        {
            disablecatchkey = 1;
        }

        private void Outputfilename_Leave(object sender, EventArgs e)
        {
            disablecatchkey = 0;
            SetBaseName();
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
            MessageBox.Show("FloppyControlApp v" + version.ToString() + " is created by\nJosha Beukema.\nCode snippets used from stack overflow and other places.\nAufit DPLL class Copyright (C) 2013-2015 Jean Louis-Guerin. ", "About");
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
        }

        // Display sector data, only works for PC for now
        private void TrackUpDown2_ValueChanged(object sender, EventArgs e)
        {
            ShowDiskSector();
        }

        private void SavePrjBtn_Click(object sender, EventArgs e)
        {
            fileio.SaveProject();
        }

        private void LoadPrjBtn_Click(object sender, EventArgs e)
        {
            fileio.ShowLoadProjectFiles();
        }

        private void ECSectorOverlayBtn_Click(object sender, EventArgs e)
        {
            ECSectorOverlay();
        }

        private void BadSectorListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateECInterface();
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

        private void Timer5_Tick(object sender, EventArgs e)
        {
            BadSectorTooltip.Location = BadSectorTooltipPos;
        }

        private void GUITimer_Tick(object sender, EventArgs e)
        {
            ProcessStatusLabel.Text = processing.ProcessStatus[processing.mfmsindex];

            if (processing.progressesstart[processing.mfmsindex] < 0)
            {
                if (processing.progressesend[processing.mfmsindex] < 1)
                {
                    processing.progressesstart[processing.mfmsindex] = 0;
                    processing.progressesend[processing.mfmsindex] = 1;
                }
            }
            progressBar1.Minimum = processing.progressesstart[processing.mfmsindex];
            progressBar1.Maximum = processing.progressesend[processing.mfmsindex];

            if (processing.progresses[processing.mfmsindex] >= processing.progressesstart[processing.mfmsindex] &&
                processing.progresses[processing.mfmsindex] <= processing.progressesend[processing.mfmsindex])
                if (processing.progresses[processing.mfmsindex] <= progressBar1.Maximum &&
                    processing.progresses[processing.mfmsindex] >= progressBar1.Minimum)
                    progressBar1.Value = processing.progresses[processing.mfmsindex];

            textBoxReceived.AppendText(tbreceived.ToString());
            tbreceived.Clear();
            this.UpdateForm();
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
                CopyThresholdsToProcessing();

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
                CopyThresholdsToQuick();

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

        private void ThreadsUpDown_ValueChanged(object sender, EventArgs e)
        {
            processing.NumberOfThreads = (int)ThreadsUpDown.Value;
        }

        private void BadSectorListBox_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateBadSectorListview(e);
        }

        private void ECZoomOutBtn_Click(object sender, EventArgs e)
        {
            ECZoomOutBtnHandler();
        }

        private void ECRealign4E_Click(object sender, EventArgs e)
        {
            ErrorCorrectRealign4E();
        }

        private void Outputfilename_TextChanged(object sender, EventArgs e)
        {
            SetBaseName();
        }
        
        private void SetBaseName()
        {
            if (outputfilename.Text != "")
            {
                openFileDialog1.InitialDirectory = subpath + @"\" + outputfilename.Text;
                openFileDialog2.InitialDirectory = subpath + @"\" + outputfilename.Text;
                if (fileio != null)
                    fileio.BaseFileName = outputfilename.Text;
                Properties.Settings.Default["BaseFileName"] = outputfilename.Text;
                Properties.Settings.Default.Save();
            }
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

        private void TrackBar3_Scroll(object sender, EventArgs e)
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

        private void Button31_Click_2(object sender, EventArgs e)
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

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            GraphFilterButton.PerformClick();
        }

        private void DiffDistUpDown2_ValueChanged(object sender, EventArgs e)
        {
            GraphFilterButton.PerformClick();
        }

        // Process the read data signal captured using the scope
        private void Button19_Click(object sender, EventArgs e)
        {
            ProcessOscilloscopeCapturedTrack();
        }

        private void CaptureDataBtn_Click(object sender, EventArgs e)
        {
            CaptureOscilloscopeTrack();
        }

        private void Button29_Click(object sender, EventArgs e)
        {
            scope.stop = 1;
            scope.capturedatastate = 3;
            scope.networktimerstop();
            scope.capturetimerstop();
        }

        private void CaptureClassbutton_Click(object sender, EventArgs e)
        {
            bytesReceived = 0;
            ConnectToFloppyControlHardware();
            CaptureTracks();
        }

        private void ConnectClassbutton_Click(object sender, EventArgs e)
        {
            ConnectToFloppyControlHardware();
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

        private void RtbSectorMap_DoubleClick(object sender, EventArgs e)
        {
            rtbSectorMap.DeselectAll();
            RateOfChange2UpDown.Focus();
            Application.DoEvents();
            processing.sectormap.RefreshSectorMap();
        }

        private void Button18_Click(object sender, EventArgs e)
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
            for (int i = 0; i < oscilloscope.graphset.Graphs.Count; i++)
            {
                oscilloscope.graphset.Graphs[i].width = GraphPictureBox.Width;
                oscilloscope.graphset.Graphs[i].height = GraphPictureBox.Height;
                oscilloscope.graphset.Resize();
            }
        }

        // Undo
        private void Button31_Click_1(object sender, EventArgs e)
        {
            EditScopePlotUndo();        }

        private void SaveWaveformButton_Click(object sender, EventArgs e)
        {
            oscilloscope.graphset.saveAll();
        }

        //Copy graph[0]
        private void Button32_Click(object sender, EventArgs e)
        {
            EditScopePlotCopy();
        }

        private void Button33_Click(object sender, EventArgs e)
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

        private void Button3_Click(object sender, EventArgs e)
        {
            oscilloscope.graphset.Graphs[graphselect].Lowpass2((int)SmoothingUpDown.Value);
            oscilloscope.graphset.Graphs[graphselect].changed = true;
            oscilloscope.graphset.UpdateGraphs();
        }
        
        private void Button33_Click_1(object sender, EventArgs e)
        {
            oscilloscope.graphset.Graphs[graphselect].Highpass((int)HighpassThresholdUpDown.Value);
            oscilloscope.graphset.Graphs[graphselect].changed = true;
            oscilloscope.graphset.UpdateGraphs();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DoErrorCorrectionOnSelection();
        }

        private void DirectStepCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            QDirectStepCheckBox.Checked = DirectStepCheckBox.Checked;

            Properties.Settings.Default["DirectStep"] = DirectStepCheckBox.Checked;
            Properties.Settings.Default.Save();
            controlfloppy.DirectStep = DirectStepCheckBox.Checked;
        }

        private void MicrostepsPerTrackUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["MicroStepsPerTrack"] = (decimal)MicrostepsPerTrackUpDown.Value;
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
        private void Button34_Click(object sender, EventArgs e)
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
        private void Button35_Click(object sender, EventArgs e)
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

                //int start, end;

                //start = (int)NetworkCaptureTrackStartUpDown.Value;
                //end = (int)NetworkCaptureTrackEndUpDown.Value;

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

        private void Histogrampanel1_Click(object sender, EventArgs e)
        {
            FindPeaks();
            updateAnScatterPlot();
        }

        private void JumpTocomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            JumpTocomboBoxUpdateScopePlot();
        }

        //Rich text box sector map interactions
        private void RtbSectorMap_MouseDown(object sender, MouseEventArgs e)
        {
            SectorMapInteractions(e);
        }

        private void Smmenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            SectorMapRightclickMenuHandler(e);
        }

        private void Button38_Click(object sender, EventArgs e)
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

        private void Button43_Click(object sender, EventArgs e)
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

        private void Button42_Click(object sender, EventArgs e)
        {
            controlfloppy.serialPort1.Write('g'.ToString()); // increase track number
            Thread.Sleep(controlfloppy.tracktotrackdelay);
        }

        private void Button41_Click(object sender, EventArgs e)
        {
            controlfloppy.serialPort1.Write('t'.ToString()); // increase track number
            Thread.Sleep(controlfloppy.tracktotrackdelay);
        }

        private void ECMFMByteEncbutton_Click(object sender, EventArgs e)
        {
            ECMFMByteEnc();
        }

        //Iterator test 
        private void Button44_Click(object sender, EventArgs e)
        {
            ECIteratorTest();
        }

        //Open SCP
        private void OpenSCP_Click(object sender, EventArgs e)
        {
            fileio.FilesAvailableCallback -= FilesAvailableCallback;
            fileio.FilesAvailableCallback += ScpFilesAvailableCallback;
            fileio.OpenScp();
            rxbufEndUpDown.Maximum = processing.indexrxbuf;
            rxbufEndUpDown.Value = processing.indexrxbuf;
        }

        //Save SCP
        private void SaveSCP_Click(object sender, EventArgs e)
        {
            fileio.SaveSCP(
                    FourvScrollBar.Value,
                    SixvScrollBar.Value,
                    EightvScrollBar.Value,
                    ProcessingModeComboBox.SelectedItem.ToString()
                );
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DiskFormat diskformat = DiskFormat.unknown;
            if (ChangeDiskTypeComboBox.SelectedItem.ToString() != "")
                diskformat = (DiskFormat)Enum.Parse(typeof(DiskFormat), ChangeDiskTypeComboBox.SelectedItem.ToString(), true);
            tbreceived.Append("Selected: " + diskformat.ToString() + "\r\n");

            processing.diskformat = diskformat;

        }

        private void Button48_Click(object sender, EventArgs e)
        {
            RecaptureAllBadSectors();
        }

        private void Button49_Click(object sender, EventArgs e)
        {
            
            fileio.SaveTrimmedBinFile(
                    FourvScrollBar.Value,
                    SixvScrollBar.Value,
                    EightvScrollBar.Value,
                    ProcessingModeComboBox.SelectedItem.ToString(),
                    false
                );
        }

        private void SaveTrimmedBadbutton_Click(object sender, EventArgs e)
        {
            fileio.SaveTrimmedBinFile(
                    (int)FourvScrollBar.Value,
                    (int)SixvScrollBar.Value,
                    (int)EightvScrollBar.Value,
                    ProcessingModeComboBox.SelectedItem.ToString()
                );
        }

        private void QProcessingModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessingModeComboBox.SelectedIndex = QProcessingModeComboBox.SelectedIndex;
            DoChangeProcMode();
        }

        private void ScanBtn_Click_1(object sender, EventArgs e)
        {
            SyncControlsBetweenTabs();
            processing.stop = 0;
            DoScan();
            tbreceived.Append("\r\nDone!\r\n");
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            scatterplot.EditScatterplot = EditScatterPlotcheckBox.Checked;
        }

        private void PresetTrack0_10_Click(object sender, EventArgs e)
        {
            StartTrackUpDown.Value = 0;
            EndTracksUpDown.Value = 10;
            TrackDurationUpDown.Value = 330;

            QStartTrackUpDown.Value = 0;
            QEndTracksUpDown.Value = 10;
            QTrackDurationUpDown.Value = 330;
        }

        private void FullHistBtn_Click(object sender, EventArgs e)
        {
            ScatterHisto.DoHistogram(processing.rxbuf, (int)rxbufStartUpDown.Value, (int)rxbufEndUpDown.Value);
        }

        private void TRK00OffsetUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["TRK00Offset"] = (decimal)TRK00OffsetUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void DirectPresetBtn_Click(object sender, EventArgs e)
        {
            TRK00OffsetUpDown.Value = 0;
            MicrostepsPerTrackUpDown.Value = 1;

            QTRK00OffsetUpDown.Value = 0;
            QMicrostepsPerTrackUpDown.Value = 1;

            DirectStepCheckBox.Checked = true;
            QDirectStepCheckBox.Checked = true;
            controlfloppy.StepStickMicrostepping = 1;
            controlfloppy.MicrostepsPerTrack = 1;
            controlfloppy.DirectStep = true;

            Properties.Settings.Default["StepStickMicrostepping"] = (decimal)1;
            Properties.Settings.Default["MicroStepsPerTrack"] = (decimal)1;
            Properties.Settings.Default["TRK00Offset"] = (decimal) 0;
            Properties.Settings.Default["DirectStep"] = true;
            Properties.Settings.Default.Save();
        }

        private void StepStickPresetBtn_Click(object sender, EventArgs e)
        {
            TRK00OffsetUpDown.Value = 0;
            MicrostepsPerTrackUpDown.Value = 8;

            QTRK00OffsetUpDown.Value = 0;
            QMicrostepsPerTrackUpDown.Value = 8;

            DirectStepCheckBox.Checked = false;
            controlfloppy.StepStickMicrostepping = 8;
            controlfloppy.MicrostepsPerTrack = 8;
            controlfloppy.DirectStep = false;

            Properties.Settings.Default["StepStickMicrostepping"] = (decimal)8;
            Properties.Settings.Default["MicroStepsPerTrack"] = (decimal)8;
            Properties.Settings.Default["TRK00Offset"] = (decimal)0;
            Properties.Settings.Default["DirectStep"] = false;
            Properties.Settings.Default.Save();

        }

        private void FloppyControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            controlfloppy.StopCapture();
            controlfloppy.Disconnect();
            Settings.Default.Save();
        }

        private void ResetBuffersBtn_Click(object sender, EventArgs e)
        {
            resetinput();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            resetoutput();
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

        private void QDirectStepCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //DirectStepCheckBox.Checked = QDirectStepCheckBox.Checked;

            //Properties.Settings.Default["DirectStep"] = DirectStepCheckBox.Checked;
            //Properties.Settings.Default.Save();
            controlfloppy.DirectStep = DirectStepCheckBox.Checked;

        }

        private void DisableTooltipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var a = sender.GetType();
             var menuitem = (ToolStripMenuItem) sender;
            if (menuitem.Checked) toolTip1.Active = true;
            else toolTip1.Active = false;
        }

        private void BasicModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetGuiMode("basic");
        }

        private void AdvancedModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetGuiMode("advanced");
        }

        private void DevModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetGuiMode("dev");
        }

        private void GluedDiskPreset_Click(object sender, EventArgs e)
        {
            TRK00OffsetUpDown.Value = 0;
            MicrostepsPerTrackUpDown.Value = 2;

            QTRK00OffsetUpDown.Value = -16;
            QMicrostepsPerTrackUpDown.Value = 8;

            DirectStepCheckBox.Checked = false;
            controlfloppy.StepStickMicrostepping = 8;
            controlfloppy.MicrostepsPerTrack = 8;
            controlfloppy.DirectStep = false;

            Properties.Settings.Default["StepStickMicrostepping"] = (decimal)8;
            Properties.Settings.Default["MicroStepsPerTrack"] = (decimal)2;
            Properties.Settings.Default["TRK00Offset"] = (decimal)-16;
            Properties.Settings.Default["DirectStep"] = false;
            Properties.Settings.Default.Save();
        }

        private void QOnlyBadSectorsRadio_CheckedChanged(object sender, EventArgs e)
        {
            OnlyBadSectorsRadio.Checked = true;
        }

        private void QECOnRadio_CheckedChanged(object sender, EventArgs e)
        {
            ECOnRadio.Checked = true;
        }

        private void ExploreHereBtn_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", fileio.PathToRecoveredDisks+@"\"+fileio.BaseFileName);
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scope.Disconnect();
            this.Close();
            return;
        }

        private void AutoRefreshSectorMapCheck_CheckedChanged(object sender, EventArgs e)
        {
            processing.procsettings.AutoRefreshSectormap = AutoRefreshSectorMapCheck.Checked;
        }
    } // end class
} // End namespace

