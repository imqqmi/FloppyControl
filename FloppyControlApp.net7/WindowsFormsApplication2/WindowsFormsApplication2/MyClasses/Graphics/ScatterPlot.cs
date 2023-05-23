using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Concurrent;
using FloppyControlApp.MyClasses;

namespace FloppyControlApp.MyClasses.Graphics
{
	class ScatterPlot
    {
        FDDProcessing Processing { get; set; }
        public ConcurrentDictionary<int, MFMData> Sectordata2 { get; set; }
        public int IndexRxBuf { get; set; }
        public PictureBox Panel { get; set; }
        public byte[] Rxbuf { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Thresholdmin { get; set; }
        public int Threshold4us { get; set; }
        public int Threshold6us { get; set; }
        public int Thresholdmax { get; set; }
        public int Bmpxoffset { get; set; }
        public StringBuilder Tbreiceved { get; set; }
        public bool Dragging { get; set; }
        private byte[] Gradient1 = new byte[256];
        public int Xrelative { get; set; }
        public int AnScatViewlength { get; set; }
        public int AnScatViewoffset { get; set; }
        public int AnScatViewlargeoffset { get; set; }
        private int AnScatViewlargeoffsetold { get; set; }
        public int AnScatViewoffsetOld { get; set; }
        public int Maxdots { get; set; }
        public int GraphIndex;
        public int RxbufClickIndex;
        public bool EditScatterplot { get; set; }
        public Action UpdateEvent { get; set; }
        public Action ShowGraph { get; set; }
        public bool ShowEntropy { get; set; }
        int ID = 0;

        public ScatterPlot(FDDProcessing proc, ConcurrentDictionary<int, MFMData> sd, int indexstart, int indexend, PictureBox picturebox)
        {
            Maxdots = 100000;
            ShowEntropy = false;
            AnScatViewlength = Maxdots;
            AnScatViewoffset = 0;
            Processing = proc;
            Sectordata2 = sd;
            Start = indexstart;
            End = indexend;
            Panel = picturebox;
            Start = 0;
            End = 0;
            Threshold4us = 0;
            Threshold6us = 6;
            Dragging = false;
            EditScatterplot = false;
            int i, p;
            Random random = new();
            int ID = random.Next();
            proc.TBReceived.Append("scatterplot id: " + ID);
            for (i = 255; i > -1; i--)
                Gradient1[i] = 255;

            for (p = 47; p < 132; p += 40)
            {
                for (i = 0; i < 16; i++)
                {
                    Gradient1[i + p] = (byte)(255 - i * 15);

                }
                for (i = 16; i < 30; i++)
                {
                    Gradient1[i + p] = (byte)(255 - (15 - (i - 15)) * 15);

                }
            }

            // Assign event handlers
            AssignEvents();
        }

        public void AssignEvents()
        {
            Panel.MouseHover += PictureBox_MouseHover;
            Panel.MouseUp += PictureBox_MouseUp;
            Panel.MouseDown += PictureBox_MouseDown;
            Panel.MouseMove += PictureBox_MouseMove;
            Panel.MouseWheel += ScatterPictureBox_MouseWheel;
        }

        public void RemoveEvents()
        {
            Panel.MouseHover -= PictureBox_MouseHover;
            Panel.MouseUp -= PictureBox_MouseUp;
            Panel.MouseDown -= PictureBox_MouseDown;
            Panel.MouseMove -= PictureBox_MouseMove;
            Panel.MouseWheel -= ScatterPictureBox_MouseWheel;
        }

        public void DrawScatterPlot()
        {
            int i, datapoints;
            float posx;

            //int TrackPosInrxdatacount = controlfloppy.TrackPosInrxdatacount;
            //int[] TrackPosInrxdata = controlfloppy.TrackPosInrxdata;
            IndexRxBuf = Processing.Indexrxbuf;

            if (Start == End)
            {
                Tbreiceved.Append("Length is zero, nothing to do.\r\n");
                return;
            }

            //System.Drawing.Pen BlackPen, RedPen;
            //BlackPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 128, 128, 128));
            //RedPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 200, 0, 0));
            System.Drawing.Graphics formGraphics = Panel.CreateGraphics();
            //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.White);
            /*
            scrollbarcurrentpos = TrackPosInrxdatacount - 2;

            // Check if there are any tracks found in the data
            if (TrackPosInrxdatacount >= 1)
            {
                // Check if scrollbar out of bounds in the TrackPosInrxdata array
                if (scrollbarcurrentpos <= TrackPosInrxdatacount - 2)
                {
                    start = TrackPosInrxdata[scrollbarcurrentpos] + 2;
                    end = TrackPosInrxdata[scrollbarcurrentpos + 1] - 1;
                }
                else //if out of bounds use indexrxbuf as last position
                {
                    start = TrackPosInrxdata[TrackPosInrxdatacount - 1] + 2;
                    end = indexrxbuf;
                }
            }
            else // if no track information is found, use the old way to support older files
            {
                
            }
            */

            using (var bmp = new Bitmap(580, 446))
            {
                byte value = 0;
                float factor;

                LockBitmap lockBitmap = new(bmp);
                lockBitmap.LockBits();
                lockBitmap.FillBitmap(Color.White);
                datapoints = End - Start;

                if (datapoints == 0) datapoints = 1;

                int width = Panel.Width;
                factor = width / (float)datapoints;

                for (i = 0; i < datapoints; i++)
                {
                    posx = factor * i;
                    value = Rxbuf[i + Start];
                    lockBitmap.SetPixel((int)posx, value << Processing.ProcSettings.hd & 0xff, Color.FromArgb(255, 0, 255 - Gradient1[value], Gradient1[value]));
                    if (value == 0x01) // draw index markers
                        lockBitmap.Line((int)posx, 0, (int)posx, 255, Color.Black);
                }

                lockBitmap.Line(0, 62, 580, 62, Color.Black);
                lockBitmap.Line(0, 100, 580, 100, Color.Black);
                lockBitmap.Line(0, 141, 580, 141, Color.Black);

                //Show thresholds
                lockBitmap.Line(0, Threshold4us, 580, Threshold4us, Color.Red);
                lockBitmap.Line(0, Threshold6us, 580, Threshold6us, Color.Red);
                lockBitmap.UnlockBits();

                formGraphics.DrawImage(bmp, Bmpxoffset, 0);
            }

            //Cleanup
            formGraphics.Dispose();
        }

        public void DrawScatterPlot(int bufstart, int bufend, int drawcnt)
        {
            int i, datapoints, start, end;
            float posx;
            //int track;
            int bigpixels = 0;
            int j;

            start = bufstart + AnScatViewlargeoffset;
            end = bufend + AnScatViewlargeoffset;
            if (start > end)
            {
                start = 0;
                end = 100000;
            }

            int datalength = end - start;

            datapoints = end - start;
            if (bufend == 0) return;
            if (Rxbuf == null)
            {
                Tbreiceved.Append("rxbuf null detected! scatterplot ID=" + ID);
                return;
            }

            if (start + datapoints > Rxbuf.Length) return;

            System.Drawing.Graphics formGraphics = Panel.CreateGraphics();
            formGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            formGraphics.CompositingMode = CompositingMode.SourceCopy;
            formGraphics.PixelOffsetMode = PixelOffsetMode.None;
            formGraphics.SmoothingMode = SmoothingMode.None;

            //tbreceived.Append("datalength: " + datalength+"\r\n");

            if (datalength < 1250) bigpixels = 4;
            if (datalength >= 1250 && datalength < 5000) bigpixels = 3;
            if (datalength >= 5000) bigpixels = 2;
            int width = Panel.Width;
            int height = Panel.Height;
            using (var bmp = new Bitmap(width, height, PixelFormat.Format32bppPArgb))
            {
                byte value = 0;
                float factor;

                LockBitmap lockBitmap = new(bmp);
                lockBitmap.LockBits();
                lockBitmap.FillBitmap(Color.White);

                if (datapoints == 0) datapoints = 1;

                factor = width / (float)datapoints;
                //if (indexrxbuf > rxbuf.Length) indexrxbuf = rxbuf.Length - 1;
                if (datapoints > Rxbuf.Length) datapoints = Rxbuf.Length - 1;
                if (Processing.entropy == null) Tbreiceved.Append("entropy = null!");
                if (start > -1 && Processing.entropy != null && ShowEntropy)
                {
                    for (i = 0; i < datapoints; i++)
                    {
                        posx = factor * i;

                        value = Rxbuf[i + start];
                        if (value == 0x01) // draw index markers
                        {
                            for (j = 0; j < 256; j += 2)
                                lockBitmap.SetPixel((int)posx, j, Color.FromArgb(0, 128, 128, 128));
                        }

                        if (value < 4) continue;
                        if (i == drawcnt) break;
                        if (Processing.entropy.Length < i + start) break;
                        if (bigpixels > 1)
                        {
                            lockBitmap.FilledSquare((int)posx, value << Processing.ProcSettings.hd & 0xff, bigpixels, bigpixels, Color.FromArgb(255, 0, 255 - Gradient1[value], Gradient1[value]));
                            lockBitmap.FilledSquare((int)posx, (int)Processing.entropy[i + start] + 192, bigpixels, bigpixels, Color.FromArgb(255, 0, 0, 0));
                            if (Processing.threshold4 != null)
                                lockBitmap.FilledSquare((int)posx, (int)Processing.threshold4[i + start], bigpixels, bigpixels, Color.FromArgb(255, 255, 0, 0));
                            if (Processing.threshold6 != null)
                                lockBitmap.FilledSquare((int)posx, (int)Processing.threshold6[i + start], bigpixels, bigpixels, Color.FromArgb(255, 0, 255, 0));
                            if (Processing.threshold8 != null)
                                lockBitmap.FilledSquare((int)posx, (int)Processing.threshold8[i + start], bigpixels, bigpixels, Color.FromArgb(255, 0, 0, 255));
                        }
                        else
                        {
                            lockBitmap.SetPixel((int)posx, value << Processing.ProcSettings.hd & 0xff, Color.FromArgb(255, 0, 255 - Gradient1[value], Gradient1[value]));
                            if (i + start < Processing.entropy.Length)
                                lockBitmap.SetPixel((int)posx, (int)Processing.entropy[i + start] + 192, Color.FromArgb(255, 0, 0, 0));
                            if (Processing.threshold4 != null && i + start < Processing.threshold4.Length)
                                lockBitmap.SetPixel((int)posx, (int)Processing.threshold4[i + start], Color.FromArgb(255, 255, 0, 0));
                            if (Processing.threshold6 != null && i + start < Processing.threshold6.Length)
                                lockBitmap.SetPixel((int)posx, (int)Processing.threshold6[i + start], Color.FromArgb(255, 0, 255, 0));
                            if (Processing.threshold8 != null && i + start < Processing.threshold8.Length)
                                lockBitmap.SetPixel((int)posx, (int)Processing.threshold8[i + start], Color.FromArgb(255, 0, 0, 255));

                        }
                    }
                }
                else if (start > -1)
                    for (i = 0; i < datapoints; i++)
                    {
                        posx = factor * i;

                        value = Rxbuf[i + start];
                        if (value == 0x01) // draw index markers
                        {
                            for (j = 0; j < 256; j += 2)
                                lockBitmap.SetPixel((int)posx, j, Color.FromArgb(0, 128, 128, 128));
                        }

                        if (value < 4) continue;
                        //if (i == drawcnt) break;

                        if (bigpixels > 1)
                        {
                            lockBitmap.FilledSquare((int)posx, value << Processing.ProcSettings.hd & 0xff, bigpixels, bigpixels, Color.FromArgb(255, 0, 255 - Gradient1[value], Gradient1[value]));
                            //lockBitmap.filledsquare((int)posx, (int)processing.entropy[i + start] + 192, bigpixels, bigpixels, Color.FromArgb(255, 0, 0, 0));
                        }
                        else
                        {
                            lockBitmap.SetPixel((int)posx, value << Processing.ProcSettings.hd & 0xff, Color.FromArgb(255, 0, 255 - Gradient1[value], Gradient1[value]));
                            //lockBitmap.SetPixel((int)posx, (int)processing.entropy[i + start] + 192, Color.FromArgb(255, 0, 0, 0));
                        }
                    }

                // Draw in marker positions
                int markerpos, relativepos;
                Color c = Color.Black;

                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                string tracksector = "";
                RectangleF rectf;
                // Show marker positions if available
                MFMData sectordata;
                if (Sectordata2 != null)
                    for (i = 0; i < Sectordata2.Count; i++)
                    {
                        sectordata = Sectordata2[i];

                        markerpos = sectordata.rxbufMarkerPositions;
                        if (markerpos >= start && markerpos < end)
                        {
                            if (sectordata.Status == SectorMapStatus.CrcOk || sectordata.Status == SectorMapStatus.AmigaCrcOk)
                            {
                                c = Color.FromArgb(64, 0, 255, 0);
                                tracksector = "T" + sectordata.trackhead.ToString("D3") + " S" + sectordata.sector.ToString();
                            }
                            else
                            if (sectordata.Status == SectorMapStatus.HeadOkDataBad || sectordata.Status == SectorMapStatus.AmigaHeadOkDataBad)
                            {
                                c = Color.FromArgb(64, 255, 0, 0);
                                tracksector = "T" + sectordata.trackhead.ToString("D3") + " S" + sectordata.sector.ToString();
                            }
                            else
                            {
                                c = Color.FromArgb(64, 0, 0, 255);
                                tracksector = "";
                            }

                            relativepos = markerpos - start;
                            posx = factor * relativepos;
                            for (j = 0; j < 256; j++)
                                lockBitmap.SetPixel((int)posx, j, c);
                            if (sectordata.Status == SectorMapStatus.AmigaHeadOkDataBad)
                                tracksector = "";
                            if (datalength < 51000)
                            {
                                if (tracksector.Length != 0)
                                {
                                    lockBitmap.UnlockBits();
                                    if (sectordata.Status == SectorMapStatus.AmigaCrcOk)
                                        rectf = new RectangleF(posx, 266, 90, 20);
                                    else
                                    {
                                        if (sectordata.MarkerType == MarkerType.header)
                                            rectf = new RectangleF(posx, 256, 90, 20);
                                        else rectf = new RectangleF(posx, 266, 90, 20);
                                    }
                                    g.DrawString(tracksector, new Font("Tahoma", 8), Brushes.Black, rectf);
                                    lockBitmap.LockBits();
                                }
                            }
                        }
                    }


                lockBitmap.Line(0, 62, 580, 62, Color.Black);
                lockBitmap.Line(0, 100, 580, 100, Color.Black);
                lockBitmap.Line(0, 141, 580, 141, Color.Black);

                if (Threshold4us < 50)
                {
                    //throw new ArgumentException("Threshold4us too low!");
                }

                //Show thresholds
                lockBitmap.Line(0, Thresholdmin, 580, Thresholdmin, Color.Red);
                lockBitmap.Line(0, Threshold4us, 580, Threshold4us, Color.Red);
                lockBitmap.Line(0, Threshold6us, 580, Threshold6us, Color.Red);
                lockBitmap.Line(0, Thresholdmax, 580, Thresholdmax, Color.Red);
                lockBitmap.UnlockBits();

                RectangleF rectf2 = new(0, 280, 256, 40);
                //string.Format("{0:n0}", start);
                string startstop = "Start:\t" + string.Format("{0:n0}", start) + "\r\nEnd:\t" + string.Format("{0:n0}", end);
                g.DrawString(startstop, new Font("Tahoma", 8), Brushes.Black, rectf2);

                lockBitmap = null;
                g.Flush();

                formGraphics.DrawImage(bmp, Bmpxoffset, 0);
            }

            //Cleanup
            formGraphics.Dispose();
        }

        // Handle MouseWheel zoom on scatterplot
        private void ScatterPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int length = Maxdots;
            int minimumZoom = 10;

            double offsetfactor = x / (float)Panel.Width;
            //tbreceived.Append("indexrxbuf: "+indexrxbuf+"\r\n");

            if (AnScatViewlength == minimumZoom && e.Delta > 0) return;

            if (e.Delta < 0 && AnScatViewlength == length - 1) return;

            if (e.Delta > 0) // zoom in
            {
                if (AnScatViewlength / 2 < minimumZoom) AnScatViewlength = minimumZoom;
                else AnScatViewlength /= 2;

                AnScatViewoffset = (int)(AnScatViewoffset + AnScatViewlength * offsetfactor);
            }
            else // zoom out
            {
                if (AnScatViewlength * 2 > length - 1) AnScatViewlength = length - 1;
                else AnScatViewlength *= 2;
                AnScatViewoffset = (int)(AnScatViewoffset - AnScatViewlength / 2.0 * offsetfactor);
            }

            if (AnScatViewoffset < 0)
            {
                AnScatViewoffset = 0;
            }
            if (AnScatViewoffset + AnScatViewlength > Processing.Indexrxbuf)
            {
                AnScatViewlength = Processing.Indexrxbuf - AnScatViewoffset;
            }
            UpdateScatterPlot();
        }

        public void UpdateScatterPlot()
        {
            UpdateEvent?.Invoke();

            DrawScatterPlot(AnScatViewoffset, AnScatViewoffset + AnScatViewlength, 200000);

        }

        private void PictureBox_MouseHover(object sender, EventArgs e)
        {
            Panel.Focus();
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (Rxbuf == null)
            {
                Tbreiceved.Append("rxbuf null detected! scatterplot ID=" + ID);
                return;
            }
            RxbufClickIndex = ViewToGraphIndex(e.X);
            if (e.Button == MouseButtons.Left)
            {
                Dragging = true;
                if (RxbufClickIndex < Rxbuf.Length - 1)
                    //tbreiceved.Append("Y: "+e.Y+" val: "+processing.rxbuf[rxbufclickindex]+" rxclickindex: "+rxbufclickindex+"\r\n");

                    Xrelative = e.X;
                Tbreiceved.Append("xrelative:" + Xrelative + "\r\n");
                AnScatViewoffsetOld = AnScatViewoffset;
                AnScatViewlargeoffsetold = AnScatViewlargeoffset;
            }
            else if (e.Button == MouseButtons.Right)
            {
                RxbufClickIndex = ViewToGraphIndex(e.X);
                if (Processing.Rxbuftograph != null)
                {
                    if (RxbufClickIndex < Processing.Rxbuftograph.Length)
                        GraphIndex = Processing.Rxbuftograph[RxbufClickIndex - 1];

                }
                ShowGraph();
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            RxbufClickIndex = ViewToGraphIndex(e.X);

            if (e.Button == MouseButtons.Left)
            {
                DoDragging(sender, e);
            }
            if (e.Button == MouseButtons.Right)
            {
                if (EditScatterplot == true && AnScatViewlength <= 0x40)
                {
                    int xoffset = Panel.Width / AnScatViewlength / 2;

                    RxbufClickIndex = ViewToGraphIndex(e.X + xoffset);
                    Processing.RxBbuf[RxbufClickIndex] = (byte)e.Y;
                    //tbreiceved.Append("Y: " + e.Y + " val: " + processing.rxbuf[rxbufclickindex] + " rxclickindex: " + rxbufclickindex + "\r\n");
                    UpdateScatterPlot();
                }
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Dragging = false;
                DoDragging(sender, e);
            }
        }

        private void DoDragging(object sender, MouseEventArgs e)
        {
            int offset;
            float factor = AnScatViewlength / (float)Panel.Width;

            offset = (int)((Xrelative - e.X) * factor);

            AnScatViewoffset = AnScatViewoffsetOld + offset;

            if (AnScatViewoffset + AnScatViewlength + offset > Maxdots - 1)
            {
                AnScatViewlargeoffset = AnScatViewlargeoffsetold + offset;
                if (AnScatViewlargeoffset + AnScatViewlength > Processing.RxBbuf.Length - 1)
                    AnScatViewlargeoffset = Processing.RxBbuf.Length - 1;
                //offset = 0;
                AnScatViewoffset = Maxdots - 1 - AnScatViewlength;
            }

            Bmpxoffset = 0;
            Start = AnScatViewoffset;
            End = Start + AnScatViewlength;

            UpdateScatterPlot();
        }

        public int ViewToGraphIndex(int x)
        {
            float offsetfactor = x / (float)Panel.Width;
            int result = (int)(AnScatViewoffset + AnScatViewlength * offsetfactor + AnScatViewlargeoffset);
            return result;
        }

    } // Scatterplot class
} // Namespace