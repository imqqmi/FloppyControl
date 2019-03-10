using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloppyControlApp.MyClasses
{
    class ZeroCrossingData
    {
        public int before;
        public int after;
        public int negpos; // 0= negative, 1 = positive going
        public int zcbeforeafter;
        public int index;
    }

    class WaveformEdit
    {
        public FileIO fileio { get; set; }
        public Graphset graphset { get; set; }
        public int offset { get; set; }
        public int GraphYScaleTrackBar { get; set; }
        public string GraphScaleYLabel { get; set; }
        public int DiffOffsetUpDown { get; set; }
        public int AnDensityUpDown { get; set; }
        public string GraphLengthLabel { get; set; }
        public FDDProcessing processing { get; set; }

        private BinaryReader reader;

        public Action updateGraphCallback { get; set; }
        public Action GraphsetGetControlValuesCallback { get; set; }
        public Action resetinput { get; set; }
        public Action FilterGuiUpdateCallback { get; set; }
        public Action Filter2GuiCallback { get; set; }

        public WaveformEdit(PictureBox GraphPictureBox, FileIO fio, FDDProcessing proc)
        {
            fileio = fio;
            processing = proc;
            graphset = new Graphset(GraphPictureBox, Color.Black);
            graphset.UpdateGUI += updateGraphCallback;
            graphset.GetControlValues += GraphsetGetControlValuesCallback;
            graphset.tbreceived = fileio.tbreceived;
        }

        public void OpenOscilloscopeFile()
        {
            byte[] temp;


            OpenFileDialog loadwave = new OpenFileDialog();
            loadwave.InitialDirectory = fileio.PathToRecoveredDisks + @"\" + fileio.BaseFileName;
            loadwave.Filter = "wvfrm files (*.wvfrm)|*.wvfrm|wfm files (*.wfm)|*.wfm|All files(*.*)|*.*";
            //Bin files (*.bin)|*.bin|All files (*.*)|*.*

            if (loadwave.ShowDialog() == DialogResult.OK)
            {

                //try
                {
                    string file = loadwave.FileName;
                    string ext = Path.GetExtension(file);
                    string filename = Path.GetFileName(file);

                    fileio.textBoxFilesLoaded.AppendText(filename + "\r\n");
                    fileio.SetBaseFileNameFromPath(loadwave.FileName);

                    graphset.filename = filename;
                    // D:\data\Projects\FloppyControl\DiskRecoveries\M003 MusicDisk\ScopeCaptures
                    //string file = @"D:\data\Projects\FloppyControl\DiskRecoveries\M003 MusicDisk\ScopeCaptures\diff4_T02_H1.wfm";
                    reader = new BinaryReader(new FileStream(file, FileMode.Open));

                    //string path1 = Path.GetFileName(file);

                    //textBoxFilesLoaded.Text += path1 + "\r\n";
                    //processing.CurrentFiles += path1 + "\r\n";
                    //outputfilename.Text = path1.Substring(0, path1.IndexOf("_"));

                    if (ext == ".wvfrm")
                    {
                        //reader.BaseStream.Length


                        //int channels = reader.Read()
                        byte channels = reader.ReadByte();
                        int wvfrmlength = reader.ReadInt32();
                        long length = reader.BaseStream.Length;

                        if (channels > 15)
                        {
                            fileio.tbreceived.Append("File header error. Too many channels: " + channels + "\r\n");
                            return;
                        }
                        int i;

                        int dataoffset = 0;
                        int datalength = 1000;
                        int density = 23;
                        int flag = 0;

                        if (graphset.Graphs.Count > 0)
                        {

                            if (wvfrmlength == graphset.Graphs[0].data.Length)
                            {
                                dataoffset = graphset.Graphs[0].dataoffset;
                                datalength = graphset.Graphs[0].datalength;
                                density = graphset.Graphs[0].density;
                            }
                            else
                            {
                                dataoffset = 0;
                                datalength = (int)wvfrmlength - 1;
                                density = 23;
                            }

                            flag = 1;
                        }
                        graphset.Graphs.Clear();
                        int cnt = graphset.Graphs.Count;


                        for (i = 0; i < channels - cnt; i++)
                        {
                            graphset.AddGraph(new byte[wvfrmlength]);
                            if (flag == 1)
                            {
                                graphset.Graphs[i].dataoffset = dataoffset;
                                graphset.Graphs[i].datalength = datalength;
                                graphset.Graphs[i].density = density;
                            }
                        }


                        for (i = 0; i < graphset.Graphs.Count; i++)
                        {
                            if (graphset.Graphs[i].data.Length != length || flag == 0)
                            {
                                graphset.Graphs[i].data = new byte[length];
                                if (graphset.Graphs[i].datalength > length)
                                    graphset.Graphs[i].datalength = (int)length - 1;
                                if (graphset.Graphs[i].dataoffset + graphset.Graphs[i].datalength > length)
                                {
                                    graphset.Graphs[i].datalength = (int)length - 1;
                                    graphset.Graphs[i].dataoffset = 0;
                                }
                            }
                        }

                        if (graphset.Graphs[0].undo.Count > 0)
                            graphset.Graphs[0].undo.Clear();


                        var gr = graphset.Graphs;

                        //graphwaveform[2] = new byte[wvfrmlength]; // Create empty waves for storage of result data
                        //graphwaveform[3] = new byte[wvfrmlength];

                        if ((wvfrmlength * channels) < length)
                        {
                            for (i = 0; i < channels; i++)
                            {
                                gr[i].data = reader.ReadBytes(wvfrmlength);
                            }

                            fileio.tbreceived.Append(loadwave.FileName + "\r\n");
                            fileio.tbreceived.Append(Path.GetFileName(loadwave.FileName) + "\r\n");
                            fileio.tbreceived.Append("FileLength: " + reader.BaseStream.Length + "\r\n");

                            reader.Close();
                        }
                        else
                        {
                            fileio.tbreceived.Append("Waveform load error: File seems to be too short!\r\n");
                        }

                        if (channels == 3)
                        {
                            int max = 0, min = 255;
                            int offset = (int)DiffOffsetUpDown;
                            for (i = Math.Abs(offset); i < wvfrmlength - (Math.Abs(offset)); i++)
                            {

                                gr[0].data[i] = (byte)(127 + (gr[0].data[i] - gr[1].data[i + offset]) / 2);
                                if (gr[0].data[i] > max) max = gr[0].data[i];
                                if (gr[0].data[i] < min) min = gr[0].data[i];

                                //gr[0].data[i] = (byte)(127 + (gr[0].data[i] - gr[1].data[i + offset]) / 2);
                            }
                            gr[0].yscale = 192.0f / (float)(max - min);
                            GraphYScaleTrackBar = (int)(gr[0].yscale * 100.0f);
                            GraphScaleYLabel = "" + gr[0].yscale;
                        }
                        else
                        {
                            int max = 0, min = 255;
                            int offset = (int)DiffOffsetUpDown;
                            for (i = Math.Abs(offset); i < wvfrmlength - (Math.Abs(offset)); i++)
                            {
                                if (gr[0].data[i] > max) max = gr[0].data[i];
                                if (gr[0].data[i] < min) min = gr[0].data[i];
                            }
                            gr[0].yscale = 192.0f / (float)(max - min);
                            if (graphset.Graphs.Count >= 5)
                                graphset.Graphs[4].yscale = 192.0f / (float)(max - min);
                            GraphYScaleTrackBar = (int)(gr[0].yscale * 100.0f);
                            GraphScaleYLabel = "" + gr[0].yscale;
                        }

                    }
                    else
                    {
                        //reader.BaseStream.Length
                        fileio.tbreceived.Append(loadwave.FileName + "\r\n");
                        fileio.tbreceived.Append(Path.GetFileName(loadwave.FileName) + "\r\n");
                        fileio.tbreceived.Append("FileLength: " + reader.BaseStream.Length + "\r\n");

                        temp = reader.ReadBytes((int)reader.BaseStream.Length);
                        long length = reader.BaseStream.Length;
                        int i;
                        int channels = 4;

                        if (graphset.Graphs.Count < 4)
                            for (i = 0; i < 4; i++)
                                graphset.AddGraph(new byte[length]);


                        int cnt = 0;
                        var gr = graphset.Graphs;
                        int j;
                        for (i = 3200; i < length - channels; i += channels)
                        {
                            for (j = 0; j < channels; j++)
                            {
                                gr[j].data[cnt] = temp[i + j];
                            }
                            cnt++;
                        }
                        reader.Close();
                    }
                }

                CreateGraphs();
                //catch (Exception ex)
                //{
                //    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                //}
            }
        }

        private void CreateGraphs()
        {
            graphset.SetAllChanged();
            var gr = graphset.Graphs;
            if (graphset.Graphs.Count < 4)
            {
                int cnt = graphset.Graphs.Count;
                int channels = 4;
                int i;

                if (cnt < channels)
                {
                    for (i = 0; i < channels - cnt; i++)
                    {
                        graphset.AddGraph(new byte[gr[0].data.Length]);
                        graphset.Graphs[graphset.Graphs.Count - 1].datalength = graphset.Graphs[0].datalength;
                        graphset.Graphs[graphset.Graphs.Count - 1].density = graphset.Graphs[0].density;
                        graphset.Graphs[graphset.Graphs.Count - 1].dataoffset = graphset.Graphs[0].dataoffset;
                    }
                }
            }
            if (graphset.Graphs.Count >= 4)
            {
                graphset.Graphs[0].yoffset = -200;
                //graphset.Graphs[0].yscale = 2.86f;

                graphset.Graphs[1].yoffset = 0;
                graphset.Graphs[1].yscale = 0.36f;

                graphset.Graphs[2].yoffset = 0;
                graphset.Graphs[2].yscale = 5;

                graphset.Graphs[3].yoffset = 175;
                graphset.Graphs[3].yscale = 1;
            }
            if (graphset.Graphs.Count >= 5)
            {
                gr[0].zorder = 10;
                gr[4].zorder = 9;
                var src = graphset.Graphs[0];
                var dst = graphset.Graphs[4];
                dst.datalength = src.datalength;
                dst.dataoffset = src.dataoffset;
                dst.density = src.density;
                //graphset.Graphs[0].yoffset = -200;
                //graphset.Graphs[0].yscale = 2.86f;
                dst.yscale = src.yscale;
                dst.yoffset = src.yoffset;
                src.zorder = 10;
                dst.zorder = 9;
            }

            if (graphset.Graphs[0].datalength == 0)
                graphset.Graphs[0].datalength = graphset.Graphs[0].data.Length;

            if (!(graphset.Graphs[0].dataoffset < graphset.Graphs[0].data.Length - graphset.Graphs[0].datalength))
            {
                int datalength = gr[0].data.Length - 1;
                int density = (int)Math.Log(datalength / 512.0, 1.4f);//datalength/graph[0].width;
                if (density <= 0) density = 1;
                if (datalength < 1000) density = 1;
                AnDensityUpDown = density;
                GraphLengthLabel = gr[0].data.Length.ToString();//DataLengthTrackBar.Value.ToString();

                for (int i = 0; i < gr.Count; i++)
                {
                    graphset.Graphs[i].dataoffset = 0;
                    graphset.Graphs[i].datalength = datalength;
                    graphset.Graphs[i].density = density;
                }
            }

            GraphLengthLabel = (gr[0].data.Length - 1000).ToString();
            
            graphset.UpdateGraphs();

        }

        public void Filter( 
            int diffdist, 
            float diffgain, 
            int diffdist2,
            int diffthreshold, 
            int smoothing, 
            int DiffMinDeviation, 
            int DiffMinDeviation2, 
            int adaptlookahead, 
            bool adaptivegainenable,
            bool invert, 
            double SignalDistRatio,
            bool AnReplacerxbufBox)
        {
            var gr = graphset.Graphs;
            if (gr.Count >= 4)
            {
                int i;
                double val = 0;
                double valadapt = 0;
                double val2 = 0;
                //double RateOfChange = (float)GraphFilterUpDown.Value;
                int length = gr[0].data.Length;
                
                double[] t = new double[length];
                double[] t1 = new double[length];
                double totalmin = 255;
                double totalmax = 0;
                double totalamplitude = 0;

                int smoothingstart = 0 - smoothing;

                //double SignalRatio = (double)SingalRationUpDown.Value;
                

                double DCoffset = 0;
                int[] history = new int[smoothing * 2 + 1];
                //int hcnt = 0;
                int total = 0;
                byte[] data = gr[0].data;
                //Smoothing pass
                if (smoothing != 0)
                {
                    for (i = smoothing; i < length - smoothing; i++)
                    {
                        total -= history[i % (smoothing * 2 + 1)]; // subtract oldest value
                        history[i % (smoothing * 2 + 1)] = data[i + smoothing];

                        total += data[i + smoothing];
                        val2 = total / (double)(smoothing * 2.0d);

                        DCoffset += val2;
                        //val = val + (((float)graphwaveform[0][i] - val) / RateOfChange);
                        //t[i] = (byte)((val * 0.4f) + (val2 * 0.6f));

                        if (invert)
                        {
                            t[i] = -(val2);
                        }
                        else
                        {
                            t[i] = val2;
                        }
                        if (i > 5000 && i < (length - smoothing - 5000))
                        {
                            if (totalmax < t[i]) totalmax = t[i];
                            if (totalmin > t[i]) totalmin = t[i];
                        }

                    }


                    // Differential pass
                    if (invert)
                        DCoffset = -DCoffset / (length - (smoothing * 2));
                    else
                        DCoffset = DCoffset / (length - (smoothing * 2));
                    totalmax -= DCoffset;
                    totalmin -= DCoffset;
                    totalamplitude = totalmax - totalmin;
                    fileio.tbreceived.Append("Totalmin:" + totalmin + " totalmax:" + totalmax + " totalamp:" + totalamplitude + "\r\n");
                }
                else
                {
                    for (i = 0; i < length; i++)
                    {
                        DCoffset += gr[0].data[i];
                        if (invert)
                        {
                            t[i] = -gr[0].data[i];
                        }
                        else
                        {
                            t[i] = gr[0].data[i];
                        }
                    }
                    // Differential pass
                    if (invert)
                        DCoffset = -DCoffset / length;
                    else
                        DCoffset = DCoffset / length;
                }



                //DCoffset = 0;
                int startdist;
                fileio.tbreceived.Append("DC offset:" + DCoffset + "\r\n");
                if (diffdist > diffdist2) startdist = diffdist;
                else startdist = diffdist2;

                double adaptivegain = 1;
                double adaptivegainnew = 0;
                double adaptivegainold = 0;
                double adaptiverateofchange = 0.01;
                double adaptivegainoldtonew = 0;
                double maxvalue = 0;
                double minvalue = 0;

                double[] adaptivegainhistory = new double[length];
                if (diffdist * SignalDistRatio == 0) return;
                for (i = startdist; i < length - adaptlookahead; i++)
                {

                    valadapt = t[i + adaptlookahead] - DCoffset;
                    val = t[i] - DCoffset;
                    if (maxvalue < valadapt) maxvalue = valadapt;
                    if (minvalue > valadapt) minvalue = valadapt;

                    if (i % (int)(diffdist * SignalDistRatio) == 0 && adaptivegainenable)
                    {
                        //adaptivegain = (adaptivegain + ((255-maxvalue)/2))/2.0;

                        //adaptivegain = (adaptivegain+(1.0-((maxvalue-minvalue)/256.0)))/2;
                        adaptivegainold = adaptivegainnew;
                        //adaptivegainnew = (1/((maxvalue - minvalue) / SignalRatio));
                        adaptivegainnew = totalamplitude / (maxvalue - minvalue);
                        //tbreceived.Append(" "+adaptivegainnew);
                        if (adaptivegain < 1.0)
                            adaptivegain = 1.0;
                        if (adaptivegain > 4)
                            adaptivegain = 4;

                        //tbreceived.Append(" i: "+i+" a"+adaptivegain+" mm"+(maxvalue-minvalue));
                        maxvalue = 0;
                        minvalue = 0;
                        adaptivegainoldtonew = 0;
                    }
                    if (adaptivegainenable)
                    {
                        if (adaptivegainoldtonew < 1)
                            adaptivegainoldtonew += adaptiverateofchange;
                        else
                            adaptivegainoldtonew = 1;
                        adaptivegain = adaptivegainold * (1 - adaptivegainoldtonew) + adaptivegainnew * adaptivegainoldtonew;
                        adaptivegainhistory[i] = adaptivegain;
                    }
                    else
                    {
                        adaptivegain = 1;
                        adaptivegainhistory[i] = 1;
                    }

                    val = ((val - (t[i - diffdist] - DCoffset)) * diffgain * adaptivegain);
                    val2 = ((val - (t[i - diffdist2] - DCoffset)) * diffgain * adaptivegain);
                    t1[i] = (128 + ((val * 0.5) + (val2 * 0.5)));

                    if (t1[i] > 255) gr[3].data[i] = 255;
                    else if (t1[i] < 0) gr[3].data[i] = 0;
                    else gr[3].data[i] = (byte)(t1[i]);

                }

                //int hyst = (int)GraphDiffHystUpDown.Value;
                int old = 0;
                int period = 0;

                if (AnReplacerxbufBox)
                {
                    resetinput();
                    //indexrxbuf = 0;
                    processing.indexrxbuf = 0;
                }

                int fluxdirection = 0;
                int orgDiffMinDeviation = DiffMinDeviation;
                float periodfactor = 2f;
                int periodoffset = -23;
                float rxbuftographlength = (length * (length / 3250000f)) / 13f;
                if (rxbuftographlength < 250000)
                    rxbuftographlength = 1250000;
                processing.rxbuftograph = new int[(int)rxbuftographlength];
                // Zero crossing pass
                for (i = 0; i < length - diffdist; i++)
                {
                    if (fluxdirection == 0) // is the direction upwards?
                    {
                        if (adaptivegainhistory[i + diffdist] >= 2)
                            DiffMinDeviation = DiffMinDeviation2;
                        else
                            DiffMinDeviation = orgDiffMinDeviation;
                        if (t1[i] >= diffthreshold + DiffMinDeviation) // is the signal crossing zero point (unsigned byte zero = 128)
                        {
                            fluxdirection = 1; // Switch checking direction
                            period = i - old; // Calculate period
                            if (period > 10 && period < 120) // Tthis works as the time domain filter
                            {
                                processing.rxbuftograph[processing.indexrxbuf] = i;
                                processing.rxbuf[processing.indexrxbuf++] = (byte)(period * periodfactor + periodoffset);

                                //tbreceived.Append(period + " ");
                                gr[1].data[i] = 200;
                                old = i;
                            }
                            /*else if (period >= 100)
                            {
                                rxbuf[processing.indexrxbuf++] = 120;
                                old = i;
                                byte flip = 0;
                                for (int q = 0; q < 500; q++) 
                                {
                                    if ((q & 1) == 0)
                                        flip = 120;
                                    else flip = 100;
                                    gr[2].data[i + q] = flip;
                                }
                            }*/
                        }
                        else gr[1].data[i] = 20; // No crossing detected
                    }
                    else // is the direction downwards?
                    {
                        if (t1[i] < diffthreshold - DiffMinDeviation)
                        {
                            fluxdirection = 0;
                            period = i - old;
                            if (period > 10 && period < 120)
                            {
                                processing.rxbuftograph[processing.indexrxbuf] = i;
                                processing.rxbuf[processing.indexrxbuf++] = (byte)(period * periodfactor + periodoffset);

                                //tbreceived.Append(period + " ");
                                gr[1].data[i] = 200;
                                old = i;
                            }
                            /*
                            else if (period >= 100)
                            {
                                rxbuf[processing.indexrxbuf++] = 120;
                                old = i;

                                byte flip = 0;
                                for (int q = 0; q < 500; q++)
                                {
                                    if ((q & 1) == 0)
                                        flip = 120;
                                    else flip = 100;
                                    gr[2].data[i + q] = flip;
                                    
                                }
                            }
                            */
                        }
                        else gr[1].data[i] = 20;
                    }
                    period = i - old;
                    if (period > 120)
                    {
                        processing.rxbuftograph[processing.indexrxbuf] = i;
                        processing.rxbuf[processing.indexrxbuf++] = 10;
                        processing.rxbuftograph[processing.indexrxbuf] = i;
                        processing.rxbuf[processing.indexrxbuf++] = 20;
                        //processing.rxbuftograph[processing.indexrxbuf] = i;
                        //processing.rxbuf[processing.indexrxbuf++] = 10;
                        //processing.rxbuftograph[processing.indexrxbuf] = i;
                        //processing.rxbuf[processing.indexrxbuf++] = 10;
                        //processing.rxbuftograph[processing.indexrxbuf] = i;
                        //processing.rxbuf[processing.indexrxbuf++] = 10;
                        old = i;
                        period = 0;
                        //byte flip = 0;

                        //Use graph2 as a marker
                        /*
                        if ( i+500 < gr[2].data.Length )
                            for (int q = 0; q < 500; q++)
                            {
                                if ((q & 1) == 0)
                                    flip = 120;
                                else flip = 100;
                                gr[2].data[i + q] = flip;
                            }
                            */
                    }

                }
                
                FilterGuiUpdateCallback();

            }
        }

        public void Filter2(
            int diffdist,
            float diffgain,
            int diffdist2,
            int diffthreshold,
            int smoothing,
            int DiffMinDeviation,
            int DiffMinDeviation2,
            int adaptlookahead,
            bool adaptivegainenable,
            bool invert,
            double SignalDistRatio,
            bool AnReplacerxbufBox)
        {
            var gr = graphset.Graphs;
            int i;

            
            int length = gr[0].data.Length;

            

            
            double[] t = new double[length];
            byte[] t1 = gr[3].data;
            //double totalmin = 255;
            //double totalmax = 0;
            //double totalamplitude = 0;

            int smoothingstart = 0 - smoothing;

            //double SignalRatio = (double)SingalRationUpDown.Value;
            

            if (AnReplacerxbufBox)
            {
                resetinput();
                //indexrxbuf = 0;
                processing.indexrxbuf = 0;
            }

            //double DCoffset = 0;
            int[] history = new int[smoothing * 2 + 1];
            //int hcnt = 0;
            //int total = 0;
            byte[] data = gr[0].data;

            int fluxdirection = 0;
            int orgDiffMinDeviation = DiffMinDeviation;
            float periodfactor = 2f;
            int periodoffset = -23;
            float rxbuftographlength = (length * (length / 3250000f)) / 13f;
            if (rxbuftographlength < 250000)
                rxbuftographlength = 250000;
            processing.rxbuftograph = new int[(int)rxbuftographlength];
            // Zero crossing pass
            int period, old = 0;
            for (i = 0; i < length - diffdist; i++)
            {
                if (fluxdirection == 0) // is the direction upwards?
                {
                    if (t1[i] >= diffthreshold + DiffMinDeviation) // is the signal crossing zero point (unsigned byte zero = 128)
                    {
                        fluxdirection = 1; // Switch checking direction
                        period = i - old; // Calculate period
                        if (period > 10 && period < 120) // Tthis works as the time domain filter
                        {
                            processing.rxbuftograph[processing.indexrxbuf] = i;
                            processing.rxbuf[processing.indexrxbuf++] = (byte)(period * periodfactor + periodoffset);

                            //tbreceived.Append(period + " ");
                            gr[1].data[i] = 200;
                            old = i;
                        }
                        /*else if (period >= 100)
                        {
                            rxbuf[processing.indexrxbuf++] = 120;
                            old = i;
                            byte flip = 0;
                            for (int q = 0; q < 500; q++) 
                            {
                                if ((q & 1) == 0)
                                    flip = 120;
                                else flip = 100;
                                gr[2].data[i + q] = flip;
                            }
                        }*/
                    }
                    else gr[1].data[i] = 20; // No crossing detected
                }
                else // is the direction downwards?
                {
                    if (t1[i] < diffthreshold - DiffMinDeviation)
                    {
                        fluxdirection = 0;
                        period = i - old;
                        if (period > 10 && period < 120)
                        {
                            processing.rxbuftograph[processing.indexrxbuf] = i;
                            processing.rxbuf[processing.indexrxbuf++] = (byte)(period * periodfactor + periodoffset);

                            //tbreceived.Append(period + " ");
                            gr[1].data[i] = 200;
                            old = i;
                        }
                        /*
                        else if (period >= 100)
                        {
                            rxbuf[processing.indexrxbuf++] = 120;
                            old = i;

                            byte flip = 0;
                            for (int q = 0; q < 500; q++)
                            {
                                if ((q & 1) == 0)
                                    flip = 120;
                                else flip = 100;
                                gr[2].data[i + q] = flip;

                            }
                        }
                        */
                    }
                    else gr[1].data[i] = 20;
                }
                period = i - old;
                if (period > 120)
                {
                    processing.rxbuftograph[processing.indexrxbuf] = i;
                    processing.rxbuf[processing.indexrxbuf++] = 10;
                    processing.rxbuftograph[processing.indexrxbuf] = i;
                    processing.rxbuf[processing.indexrxbuf++] = 20;
                    //processing.rxbuftograph[processing.indexrxbuf] = i;
                    //processing.rxbuf[processing.indexrxbuf++] = 10;
                    //processing.rxbuftograph[processing.indexrxbuf] = i;
                    //processing.rxbuf[processing.indexrxbuf++] = 10;
                    //processing.rxbuftograph[processing.indexrxbuf] = i;
                    //processing.rxbuf[processing.indexrxbuf++] = 10;
                    old = i;
                    period = 0;
                    //byte flip = 0;

                    //Use graph2 as a marker
                    /*
                    if ( i+500 < gr[2].data.Length )
                        for (int q = 0; q < 500; q++)
                        {
                            if ((q & 1) == 0)
                                flip = 120;
                            else flip = 100;
                            gr[2].data[i + q] = flip;
                        }
                        */
                }

            }

            Filter2GuiCallback();
        }

        public void Fix8us(
            int diffdist,
            int threshold,
            int thresholddev,
            int thresholdtest
            )
        {
            int i;
            byte[] d = graphset.Graphs[0].data;
            byte[] g3 = graphset.Graphs[2].data;
            byte[] g4 = graphset.Graphs[3].data;
            int diff;
            
            int start = graphset.Graphs[0].dataoffset;
            int length = graphset.Graphs[0].datalength;
            
            
            
            //for (i = diffdist; i < d.Length-diffdist; i++)
            int skip = 0;
            int iold = start + diffdist;
            int min = 255;
            int max = 0;
            int amplitude = 0;
            int amplitudeavgcnt = 0;
            int amplitudeavg = 0;
            int before = 0;
            int after = 0;
            int zerocrossingfilter = 0;
            int zerocrossingbeforeafter = 0;
            int zerocrossingdistance = 15;

            ZeroCrossingData[] zc = new ZeroCrossingData[300000];

            //graphset.Graphs[0].Lowpass2(3);
            //if( graphset.Graphs.Count > 5)
            //    graphset.Graphs[4].Lowpass2(3);

            int zcindex = 0;
            //int lengthamplitude = (start + length - diffdist) - (start + diffdist);
            for (i = start + diffdist; i < start + length - diffdist; i++)
            {
                diff = d[i] - d[i - diffdist];
                diff = (diff + (d[i] - d[i - diffdist * 4])) / 2;
                diff = (diff + (d[i] - d[i - diffdist * 3])) / 2;
                diff = (diff + (d[i] - d[i - diffdist * 2])) / 2;
                if (i > start + diffdist + zerocrossingdistance)
                    before -= d[i - zerocrossingdistance];
                before += d[i];

                if (i > start + diffdist + zerocrossingdistance)
                    after -= d[i];
                after += d[i + zerocrossingdistance];


                g4[i] = (byte)(diff + 127);
                skip++;
                zerocrossingfilter++;
                g3[i] = 110;
                if (i % 500 == 0)
                {
                    amplitude = max - min;
                    amplitudeavgcnt += amplitude;


                    if ((i - (start + diffdist)) > 0)
                        amplitudeavg = amplitudeavgcnt / 500;
                    threshold = amplitudeavg / 2 - thresholddev;
                    amplitudeavgcnt = 0;
                    min = 255;
                    max = 0;
                }
                if (d[i + 500] > max) max = d[i + 500];
                if (d[i + 500] < min) min = d[i + 500];

                amplitude = max - min;
                amplitudeavgcnt += amplitude;

                // Zero crossing
                // Going positive
                if (d[i - 1] < 128 && d[i] >= 128 && zerocrossingfilter > 30)
                {
                    zerocrossingbeforeafter = (after / zerocrossingdistance - 127) - (before / zerocrossingdistance - 127);
                    fileio.tbreceived.Append("Pos Zero crossing: i " + i + " before: " + (before / zerocrossingdistance - 127) +
                        " after: " + (after / zerocrossingdistance - 127) + " B/A: " + zerocrossingbeforeafter + "\r\n");
                    zc[zcindex] = new ZeroCrossingData();
                    zc[zcindex].after = after / zerocrossingdistance - 127;
                    zc[zcindex].before = before / zerocrossingdistance - 127;
                    zc[zcindex].negpos = 1;
                    zc[zcindex].zcbeforeafter = zerocrossingbeforeafter;
                    zc[zcindex].index = i;
                    zcindex++;

                    if (zerocrossingbeforeafter < thresholdtest)
                    {
                        g3[i] = 85;
                        zerocrossingfilter = 15;
                    }
                    else
                    {
                        g3[i] = 90;
                        zerocrossingfilter = 0;
                    }

                }
                // Going negative
                if (d[i - 1] >= 126 && d[i] < 126 && zerocrossingfilter > 30)
                {
                    zerocrossingbeforeafter = (before / zerocrossingdistance - 127) - (after / zerocrossingdistance - 127);
                    fileio.tbreceived.Append("Neg Zero crossing: i " + i + " before: " + (before / zerocrossingdistance - 127) +
                        " after: " + (after / zerocrossingdistance - 127) + " B/A: " + zerocrossingbeforeafter + "\r\n");
                    zc[zcindex] = new ZeroCrossingData();
                    zc[zcindex].after = after / zerocrossingdistance - 127;
                    zc[zcindex].before = before / zerocrossingdistance - 127;
                    zc[zcindex].negpos = 0;
                    zc[zcindex].zcbeforeafter = zerocrossingbeforeafter;
                    zc[zcindex].index = i;
                    zcindex++;

                    if (zerocrossingbeforeafter < thresholdtest)
                    {
                        g3[i] = 80;
                        zerocrossingfilter = 15;
                    }
                    else
                    {
                        g3[i] = 90;
                        zerocrossingfilter = 0;
                    }
                }

                /*
                // Differential check
                if (diff > threshold && skip > 30)
                {
                    tbreceived.Append(" i " + i + " " + diff+" period "+(i-iold)+" amplitude: "+threshold+"\r\n");
                    skip = 0;
                    iold = i;
                    g3[i] = 100;
                }
                if (diff < 0-threshold && skip > 30)
                {
                    tbreceived.Append(" i " + i + " " + diff + " period " + (i - iold) + " amplitude: " + threshold + "\r\n");
                    skip = 0;
                    iold = i;
                    g3[i] = 100;
                }
                */
            }
            /*
            int j;
            
            for (i = 1; i < zcindex-1; i++)
            {
                if (zc[i].zcbeforeafter < thresholdtest)
                {
                    if( zc[i-1].zcbeforeafter > thresholdtest && zc[i + 1].zcbeforeafter > thresholdtest)
                    if (zc[i].before < 11 && zc[i].after < 11)
                        if (zc[i + 1].index - zc[i - 1].index > 70 && zc[i + 1].index - zc[i - 1].index < 90)
                        {
                            for (j = 0; j < 50; j++)
                            {
                                if (zc[i-1].negpos == 0)
                                    d[zc[i].index + j - 25] = 127 - 10;
                                else
                                    d[zc[i-1].index + j - 25] = 127 + 10;
                            }
                        }
                }
            }
            */
        }
    } // class
}
