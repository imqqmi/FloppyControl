using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace FloppyControlApp.MyClasses.Graphics
{
	// Graph class
	// line plot graph
	public class Graph
    {
        public PictureBox panel;
        public int Width { set; get; }
        public int Height { set; get; }
        public int Density { set; get; }
        public int Dataoffset { set; get; }
        public int Datalength { set; get; }
        public int YOffset { set; get; }
        public int BmpXOffset { set; get; }
        public int XRelative { get; set; }

        public bool Changed { get; set; }
        public bool Dragging { get; set; }

        public byte[] Data { set; get; }
        long[,] g;
        long maxvalue;

        public float XScale { set; get; }
        public float YScale { set; get; }

        public Color Graphcolor { set; get; }
        public Color FillColor { set; get; }

        public StringBuilder TBRreceived { set; get; }

        public Bitmap BmpBuf { get; set; }

        public Graph()
        {
            Dragging = false;
            BmpXOffset = 0;
            Changed = true;
            XScale = 1.0f;
            YScale = 1.0f;
            Density = 23;
            Graphcolor = Color.Black;
            FillColor = Color.White;

            Datalength = 0;
            Dataoffset = 0;
        }

        public void DoGraph(bool clear)
        {
            int x1, y1, x2, y2;
            int yoffset2;
            double step;
            double datastep;
            double i;
            //int k;
            if (Data != null)
            {
                if (Datalength == 0)
                    Datalength = Data.Length;

                if (Dataoffset >= Data.Length)
                    return;

                using System.Drawing.Graphics formGraphics = panel.CreateGraphics();
                if (Changed)
                {

                    BmpBuf?.Dispose();
                    BmpBuf = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
                    {

                        LockBitmap lockBitmap = new(BmpBuf);
                        lockBitmap.LockBits();
                        if (clear) lockBitmap.FillBitmap(FillColor);

                        step = Width / (float)(Datalength - 1);
                        //if (step >= width) step = 1;
                        datastep = (Datalength - 1.0f) / Width;
                        //if (datastep >= width && datastep < width * 4) datastep /= 4;
                        //if (datastep < width) datastep = 1;

                        /*
						tbreceived.Append("datastep:" + datastep + " step: " + step + " datalength: " + datalength +
							"number of steps in for():" + datalength / datastep + " width:" + width + "\r\n");
						tbreceived.Append("Start:" + (dataoffset + datastep).ToString() + " stop:" + (dataoffset + datalength).ToString() + "\r\n");
						*/
                        x1 = 0;
                        y1 = (int)(Data[Dataoffset] * YScale - 128 * YScale);

                        yoffset2 = YOffset + Height / 2;
                        int xpos = 1;
                        if (Datalength + Dataoffset < Data.Length)
                            for (i = Dataoffset + datastep; i < Dataoffset + Datalength; i += datastep)
                            {
                                x2 = xpos;
                                y2 = (int)(Data[(int)i] * YScale - 128 * YScale);

                                lockBitmap.Line(x1, Height - (y1 + yoffset2), x2, Height - (y2 + yoffset2), Graphcolor);
                                xpos++;
                                x1 = x2;
                                y1 = y2;
                            }

                        lockBitmap.UnlockBits();
                        formGraphics.DrawImage(BmpBuf, BmpXOffset, 0);
                    }
                    Changed = false;
                }
                else
                {
                    formGraphics.DrawImage(BmpBuf, BmpXOffset, 0);

                }
            }
        }

        public void DoGraph2(bool clear)
        {
            int x1, y1, x2, y2, x, y;
            int yoffset2;
            double step;
            double datastep;
            double i;
            //int k;

            if (Data != null)
            {
                //tbreceived.Append("Dataoffset: " + dataoffset + " length: " + data.Length + " Datalength:" + datalength + "\r\n");
                if (Datalength == 0)
                    Datalength = Data.Length;

                if (Dataoffset >= Data.Length)
                {
                    TBRreceived.Append("DoGraph2() dataoffset >= data.length!\r\nDataoffset: " + Dataoffset + " length: " + Data.Length + "\r\n");
                    return;
                }

                using System.Drawing.Graphics formGraphics = panel.CreateGraphics();
                if (Changed)
                {
                    BmpBuf?.Dispose();
                    BmpBuf = new Bitmap(Width, Height);
                    {
                        maxvalue = 1;
                        g = new long[Width + 1, Height + 1];

                        LockBitmap lockBitmap = new(BmpBuf);
                        lockBitmap.LockBits();
                        if (clear) lockBitmap.FillBitmap(FillColor);

                        step = Width / (double)(Datalength - 1) * Density;
                        //if (step >= width) step = 1;
                        datastep = (Datalength - 1.0f) / Width;
                        //if (datastep >= width && datastep < width * 4) datastep /= 4;
                        //if (datastep < width) datastep = 1;

                        /*
						tbreceived.Append("datastep:" + datastep + " step: " + step + " datalength: " + datalength +
							"number of steps in for():" + datalength / datastep + " width:" + width + "\r\n");
						tbreceived.Append("Start:" + (dataoffset + datastep).ToString() + " stop:" + (dataoffset + datalength).ToString() + "\r\n");
						*/
                        x1 = 0;
                        y1 = (int)(Data[Dataoffset] * YScale - 128 * YScale);

                        yoffset2 = YOffset + Height / 2;
                        double xpos = 1;
                        if (Datalength + Dataoffset < Data.Length)
                            for (i = Dataoffset + datastep; i < Dataoffset + Datalength; i += Density)
                            {
                                x2 = (int)xpos;
                                y2 = (int)(Data[(int)i] * YScale - 128 * YScale);

                                Line(x1, Height - (y1 + yoffset2), x2, Height - (y2 + yoffset2), 255);
                                xpos += step;
                                x1 = x2;
                                y1 = y2;
                            }
                        byte val;
                        for (x = 0; x < Width; x++)
                            for (y = 0; y < Height; y++)
                            {
                                if (maxvalue < 512) maxvalue = 255;

                                int val1 = (int)(g[x, y] / (maxvalue / 255));

                                if (val1 > 255) val = 255;
                                else val = (byte)val1;

                                if (val != 0)
                                {
                                    float factor = val / 255.0f;
                                    Color c = Color.FromArgb(255, (byte)(Graphcolor.R * factor), (byte)(Graphcolor.G * factor), (byte)(Graphcolor.B * factor));
                                    lockBitmap.SetPixel(x, y, c);
                                }
                                //else
                                //{
                                //    lockBitmap.SetPixel(x, y, fillcolor);
                                // }
                            }
                        lockBitmap.UnlockBits();
                        formGraphics.DrawImage(BmpBuf, BmpXOffset, 0);

                    }
                    Changed = false;

                }
                else
                {
                    formGraphics.DrawImage(BmpBuf, BmpXOffset, 0);

                }
            }
        }

        // Line drawing routine taken from:
        // http://www.edepot.com/linee.html
        public void Line(int x, int y, int x2, int y2, byte val)
        {
            bool yLonger = false;
            int shortLen = y2 - y;
            int longLen = x2 - x;
            bool skip = false;

            if (x < 0 || x >= Width) return;
            if (y < 0 || y >= Height) return;

            if (x2 < 0 || x2 >= Width) return;
            if (y2 < 0 || y2 >= Height) return;


            if (Math.Abs(shortLen) > Math.Abs(longLen))
            {
                (longLen, shortLen) = (shortLen, longLen);
                yLonger = true;
            }
            int decInc;
            if (longLen == 0) decInc = 0;
            else decInc = (shortLen << 16) / longLen;

            if (yLonger)
            {
                if (longLen > 0)
                {
                    longLen += y;
                    for (int j = 0x8000 + (x << 16); y <= longLen; ++y)
                    {
                        if (skip) g[j >> 16, y] += val;
                        if (g[j >> 16, y] > maxvalue) maxvalue = g[j >> 16, y];
                        j += decInc;
                        skip = true;
                    }
                    return;
                }
                longLen += y;
                for (int j = 0x8000 + (x << 16); y >= longLen; --y)
                {
                    if (skip) g[j >> 16, y] += val;
                    if (g[j >> 16, y] > maxvalue) maxvalue = g[j >> 16, y];
                    j -= decInc;
                    skip = true;
                }
                return;
            }

            if (longLen > 0)
            {
                longLen += x;
                for (int j = 0x8000 + (y << 16); x <= longLen; ++x)
                {
                    if (skip) g[x, j >> 16] += val;
                    if (g[x, j >> 16] > maxvalue) maxvalue = g[j >> 16, y];
                    j += decInc;
                    skip = true;
                }
                return;
            }
            longLen += x;
            for (int j = 0x8000 + (y << 16); x >= longLen; --x)
            {
                if (skip) g[x, j >> 16] += val;
                if (g[x, j >> 16] > maxvalue) maxvalue = g[j >> 16, y];
                j -= decInc;
                skip = true;
            }
        }
    }
} // Namespace