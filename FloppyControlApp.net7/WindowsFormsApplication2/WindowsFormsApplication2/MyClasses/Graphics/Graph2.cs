using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using FloppyControlApp.MyClasses.Capture.Models;
using FloppyControlApp.MyClasses.Utility;

namespace FloppyControlApp.MyClasses.Graphics
{
    /// <summary>
    /// Draw a line graph with interpolation so that details are still visible 
    /// even with a lot of data zoomed out all the way. Has some digital
    /// signal processing and editing of the graph capabilities.
    /// </summary>
    public class Graph2
    {
        public int Width { set; get; }
        public int Height { set; get; }
        public int Density { set; get; }
        public int DataOffset { set; get; }
        public int DataLength { set; get; }
        public int YOffset { set; get; }
        public int ZOrder { get; set; }

        private byte[] EditTempBuf;
        public bool EditGraphActive { get; set; }
        public int EditIndex { get; set; }
        public List<UndoData> Undo { get; set; }
        public int RStart { get; set; }
        public int REnd { get; set; }
        public int NewEditIndex { get; set; }
        public int EditTempBufIndex { get; set; }
        public int EditPeriodExtend { get; set; }

        //public int xrelative { get; set; }
        public bool Changed { get; set; }


        public Color GraphColor { set; get; }
        public Bitmap BmpBuf { get; set; }
        public StringBuilder TBReceived { get; set; }

        public byte[] Data { set; get; }
        long[,] g;
        //long maxvalue;
        long Average;
        long TotalNumberOfPoints;

        public float XScale { set; get; }
        public float YScale { set; get; }

        public Graph2()
        {
            ZOrder = 0;
            Undo = new List<UndoData>();
            EditIndex = 0;
            EditGraphActive = false;
            Changed = true;
            XScale = 1.0f;
            YScale = 1.0f;
            Density = 23;
            GraphColor = Color.Black;

            DataLength = 0;
            DataOffset = 0;
        }

        public void DoGraph2()
        {
            int x1, y1, x2, y2, x, y;
            int yoffset2;
            double step;
            double datastep;
            double i;
            Stopwatch sw = new();

            sw.Reset();
            sw.Start();
            if (Data != null)
            {
                //tbreceived.Append("Dataoffset: " + dataoffset + " length: " + data.Length + " Datalength:" + datalength + "\r\n");
                if (DataLength <= 0)
                    DataLength = Data.Length;
                if (DataLength <= 0)
                    return;
                if (DataOffset < 0) DataOffset = 0;

                if (DataOffset >= Data.Length)
                {
                    TBReceived.Append("DoGraph2() dataoffset >= data.length!\r\nDataoffset: " + DataOffset + " length: " + Data.Length + "\r\n");
                    return;
                }

                if (Changed)
                {

                    if (BmpBuf != null)
                    {
                        BmpBuf.Dispose();
                        BmpBuf = null;
                    }
                    //tbreceived.Append(" 1. " + sw.ElapsedMilliseconds); sw.Restart();

                    BmpBuf = new Bitmap(Width, Height);
                    {
                        //uint Time1 = (uint)(Environment.TickCount + int.MaxValue);
                        //tbreceived.Append("Time: " + (uint)(Environment.TickCount + Int32.MaxValue - Time1) + "ms\r\n");
                        //maxvalue = 1024;
                        Average = 0;
                        TotalNumberOfPoints = 0;
                        g = new long[Width + 1, Height + 1];

                        LockBitmap lockBitmap = new(BmpBuf);
                        lockBitmap.LockBits();
                        //if (clear) lockBitmap.FillBitmap(fillcolor);

                        step = Width / (double)(DataLength - 1) * Density;
                        //if (step >= width) step = 1;
                        datastep = (DataLength - 1.0f) / Width;
                        //if (datastep >= width && datastep < width * 4) datastep /= 4;
                        //if (datastep < width) datastep = 1;

                        /*
                        tbreceived.Append("datastep:" + datastep + " step: " + step + " datalength: " + datalength +
                            "number of steps in for():" + datalength / datastep + " width:" + width + "\r\n");
                        tbreceived.Append("Start:" + (dataoffset + datastep).ToString() + " stop:" + (dataoffset + datalength).ToString() + "\r\n");
                        */
                        x1 = 0;
                        y1 = (int)(Data[DataOffset] * YScale - 127 * YScale);
                        float yscale127 = 127 * YScale;
                        int ymax = 0;
                        yoffset2 = YOffset + Height / 2;
                        double xpos = 1;
                        //tbreceived.Append(" 2. " + sw.ElapsedMilliseconds); sw.Restart();
                        if (DataLength + DataOffset < Data.Length)
                            for (i = DataOffset + datastep; i < DataOffset + DataLength; i += Density)
                            {
                                x2 = (int)xpos;
                                y2 = (int)(Data[(int)i] * YScale - yscale127);
                                if (ymax < y2) ymax = y2;
                                Line(x1, Height - (y1 + yoffset2), x2, Height - (y2 + yoffset2), 255);
                                xpos += step;
                                x1 = x2;
                                y1 = y2;
                            }

                        byte val;
                        int yend = Height;
                        int ystart = Height - (ymax + yoffset2);
                        if (ystart < 0) ystart = 0;
                        //tbreceived.Append("ystart:" + ystart + " yend:" + yend+"\r\n");
                        long val2;
                        for (x = 0; x < Width; x++)
                            for (y = ystart; y < yend; y++)
                            {
                                val2 = g[x, y];
                                if (val2 > 0)
                                {
                                    TotalNumberOfPoints++;
                                    Average += val2;
                                }
                            }
                        //tbreceived.Append(" 3. " + sw.ElapsedMilliseconds); sw.Restart();
                        if (TotalNumberOfPoints == 0) TotalNumberOfPoints = 1;
                        Average /= TotalNumberOfPoints;
                        //tbreceived.Append("Average: " + average + " maxvalue:" + maxvalue + "\r\n");
                        float brightness;
                        //if (average < maxvalue / 4)
                        //    brightness = average;
                        //else brightness = maxvalue;
                        if (DataLength > 8000)
                            brightness = Average * 1.25f;
                        else brightness = Average;

                        for (x = 0; x < Width; x++)
                            for (y = ystart; y < yend; y++)
                            {
                                //if (maxvalue < 1024) maxvalue = 255;

                                int val1 = (int)(g[x, y] / (brightness / 255.0f));

                                if (val1 > 255) val = 255;
                                else val = (byte)val1;

                                if (val != 0)
                                {
                                    float factor = val / 255.0f;
                                    Color c = Color.FromArgb(255, (byte)(GraphColor.R * factor), (byte)(GraphColor.G * factor), (byte)(GraphColor.B * factor));
                                    lockBitmap.SetPixel(x, y, c);
                                }
                            }
                        //tbreceived.Append(" 4. " + sw.ElapsedMilliseconds + "\r\n"); sw.Restart();
                        y1 = (int)(127 * YScale - 127 * YScale);
                        lockBitmap.Line(0, Height - (y1 + yoffset2), Width, Height - (y1 + yoffset2), Color.FromArgb(128, GraphColor.R, GraphColor.G, GraphColor.B));
                        lockBitmap.UnlockBits();
                        //tbreceived.Append("Time: " + (uint)(Environment.TickCount + Int32.MaxValue - Time1) + "ms\r\n");
                    }
                    Changed = false;
                }
            }
        }

        public int DCOffset()
        {
            int dcoffset = 0;
            int i;
            int length = Data.Length;

            for (i = 0; i < length; i++)
                dcoffset += Data[i];

            dcoffset /= length;

            //dcoffset = dcoffset;

            for (i = 0; i < length; i++)
                Data[i] = (byte)(Data[i] - (byte)dcoffset + 127);

            return dcoffset;
        }

        public void Highpass(int smoothing)
        {
            double DCoffset = 0;
            int i;
            int length = Data.Length;

            double val2;
            double totalmax = 0;
            double totalmin = 255;
            double[] t = new double[length];

            int[] history = new int[smoothing * 2 + 1];

            int total = 0;
            for (i = smoothing; i < length - smoothing; i++)
            {
                total -= history[i % (smoothing * 2 + 1)]; // subtract oldest value
                history[i % (smoothing * 2 + 1)] = Data[i + smoothing];

                total += Data[i + smoothing];
                val2 = total / (double)(smoothing * 2.0d);
                //DCoffset += val2;
                //val = val + (((float)graphwaveform[0][i] - val) / RateOfChange);
                //t[i] = (byte)((val * 0.4f) + (val2 * 0.6f));

                t[i] = Data[i] - (val2 + 127);
                DCoffset += t[i];
                if (i > 5000 && i < length - smoothing - 5000)
                {
                    if (totalmax < t[i]) totalmax = t[i];
                    if (totalmin > t[i]) totalmin = t[i];
                }
            }

            DCoffset /= length - smoothing * 2;

            for (i = 0; i < length; i++)
                Data[i] = (byte)(t[i] - (byte)DCoffset + 127);

        }

        public void Lowpass(int smoothing)
        {
            double DCoffset = 0;
            int i, j;
            int length = Data.Length;
            int smoothingstart = 0 - smoothing;
            double val2 = 0;
            double totalmax = 0;
            double totalmin = 255;
            double[] t = new double[length];

            for (i = smoothing; i < length - smoothing; i++)
            {
                for (j = smoothingstart; j < smoothing; j++)
                {
                    val2 += Data[i + j];
                }
                val2 /= smoothing * 2.0;
                DCoffset += val2;
                //val = val + (((float)graphwaveform[0][i] - val) / RateOfChange);
                //t[i] = (byte)((val * 0.4f) + (val2 * 0.6f));

                t[i] = val2;
                if (i > 5000 && i < length - smoothing - 5000)
                {
                    if (totalmax < t[i]) totalmax = t[i];
                    if (totalmin > t[i]) totalmin = t[i];
                }
            }

            DCoffset /= length - smoothing * 2;

            for (i = 0; i < length; i++)
                Data[i] = (byte)(t[i] - (byte)DCoffset + 127.0);

        }

        public void Lowpass2(int smoothing)
        {
            double DCoffset = 0;
            int i;
            int length = Data.Length;
            double totalmax = 0;
            double totalmin = 255;
            double val2;
            double[] t = new double[length];
            int[] history = new int[smoothing * 2 + 1];
            int total = 0;
            for (i = smoothing; i < length - smoothing; i++)
            {
                total -= history[i % (smoothing * 2 + 1)]; // subtract oldest value
                history[i % (smoothing * 2 + 1)] = Data[i + smoothing];

                total += Data[i + smoothing];
                val2 = total / (double)(smoothing * 2.0d);

                DCoffset += val2;

                t[i] = val2;
                if (i > 5000 && i < length - smoothing - 5000)
                {
                    if (totalmax < t[i]) totalmax = t[i];
                    if (totalmin > t[i]) totalmin = t[i];
                }
            }

            DCoffset /= length - smoothing * 2;

            for (i = 0; i < length; i++)
                Data[i] = (byte)(t[i] - (byte)DCoffset + 127.0);

        }

        public int ViewToDataY(int viewy)
        {
            int result;
            float yscale127 = 127 * YScale;

            //int y2 = (int)(data[0] * yscale - yscale127);

            int yoffset2 = YOffset + Height / 2;

            //int viewy2 = height - (y2 + yoffset2);

            //viewy2 - height = y2 + yoffset2;
            int y2 = Height - (yoffset2 + viewy);

            //y2 + yscale127 = data[0] * yscale;
            //(y2 + yscale127) / yscale = data[0];


            result = (int)((y2 + yscale127) / YScale);

            return result;
        }

        public int ViewToGraphIndex(int x)
        {
            float offsetfactor = x / (float)Width;
            int result = (int)(DataOffset + DataLength * offsetfactor);
            return result;
        }

        public void EditGraph(int y, int radius, int editmode, int editoption, int extendradius)
        {
            float d;
            float d1;
            float dresult;

            radius += extendradius;

            // Tried to get a smooth curve to manipulate the waveform. PeriodExtend controls the width and may be negative. -19 is max.
            if (EditGraphActive)
            {
                if (editmode == 0)
                {
                    if (EditTempBuf == null)
                        if (EditIndex - radius > -1 && EditIndex + radius < Data.Length)
                            EditTempBuf = Data.SubArray(EditIndex - radius, radius * 2 + 1);

                    for (int i = 0; i < EditTempBuf.Length; i++)
                    {
                        if (i <= radius)
                            d = i / (float)radius;
                        else
                            d = 1 - (i - radius) / (float)radius;

                        d1 = 1 - d;
                        d1 *= d1;

                        d = 1 - d1;
                        //d *= d1;
                        dresult = EditTempBuf[i] * (1 - d) + y * d;
                        Data[EditIndex - radius + i] = (byte)dresult;
                    }
                }
                else if (editmode == 1)
                {
                    radius = (40 + extendradius + editoption * 20) / 2;
                    if (editoption == 0) radius -= extendradius;
                    if (EditTempBuf != null)
                        if (EditTempBuf.Length > 0)
                            for (int i = 0; i < EditTempBuf.Length; i++)
                                Data[EditTempBufIndex + i] = EditTempBuf[i];

                    //Find zero crossing
                    byte start = Data[EditIndex - radius];
                    int direction = start - 127;
                    RStart = 0;
                    if (direction != 0)
                    {
                        for (int i = 1; i < 20; i++)
                        {
                            if (direction < 0)
                            {
                                if (Data[EditIndex - radius - i] >= 127)
                                {
                                    RStart = EditIndex - radius - i;
                                    break;
                                }
                                if (Data[EditIndex - radius + i] >= 127)
                                {
                                    RStart = EditIndex - radius + i;
                                    break;
                                }
                            }
                            if (direction > 0)
                            {
                                if (Data[EditIndex - radius - i] <= 127)
                                {
                                    RStart = EditIndex - radius - i;
                                    break;
                                }
                                if (Data[EditIndex - radius + i] <= 127)
                                {
                                    RStart = EditIndex - radius + i;
                                    break;
                                }
                            }
                        }
                    }

                    REnd = 0;
                    if (direction != 0)
                    {
                        for (int i = 1; i < 20; i++)
                        {
                            if (direction < 0)
                            {
                                if (Data[EditIndex + radius - i] >= 127)
                                {
                                    REnd = EditIndex + radius - i;
                                    break;
                                }
                                if (Data[EditIndex + radius + i] >= 127)
                                {
                                    REnd = EditIndex + radius + i;
                                    break;
                                }
                            }
                            if (direction > 0)
                            {
                                if (Data[EditIndex + radius - i] <= 127)
                                {
                                    REnd = EditIndex + radius - i;
                                    break;
                                }
                                if (Data[EditIndex + radius + i] <= 127)
                                {
                                    REnd = EditIndex + radius + i;
                                    break;
                                }
                            }
                        }
                    }


                    if (RStart != 0 && REnd != 0)
                    {
                        NewEditIndex = (RStart + REnd) / 2;

                        if (EditTempBuf == null)
                            if (RStart > -1 && REnd < Data.Length)
                            {
                                EditTempBufIndex = NewEditIndex - radius - 5;
                                EditTempBuf = Data.SubArray(NewEditIndex - radius - 5, radius * 2 + 5);
                            }
                        for (int i = NewEditIndex - radius; i < NewEditIndex + radius; i++)
                        {
                            dresult = y;
                            //if (y < 127) dresult = 100;
                            //else dresult = 127+27;
                            Data[i] = (byte)dresult;
                        }

                        /*
                        float sag = -5f;
                        float sagx = 0.7f;
                        float shoulder = 1;
                        float shoulderfactor = 12f;
                        for (i = neweditindex-radius; i < neweditindex+radius; i++)
                        {
                            dresult = y;
                            

                            if (y < 127)
                            {
                                if (i == neweditindex) shoulderfactor = 1.0129f;

                                //tbreceived.Append("shoulder: "+shoulderfactor+"\r\n");
                                if (i > neweditindex + radius - 10) { sag += shoulderfactor; shoulderfactor *= shoulderfactor; }
                                if (i < (neweditindex - radius) + 10) { sag -= shoulderfactor; shoulderfactor = (float)Math.Sqrt((double)shoulderfactor); }

                                if (i >= neweditindex)
                                    sag = sag - sagx;
                                else sag = sag + sagx;
                            }
                            if (y >= 127)
                            {
                                if (i == neweditindex) shoulderfactor = 1.0129f;

                                //tbreceived.Append("shoulder: "+shoulderfactor+"\r\n");
                                if (i > neweditindex + radius - 10) { sag -= shoulderfactor; shoulderfactor *= shoulderfactor; }
                                if (i < (neweditindex - radius) + 10) { sag += shoulderfactor; shoulderfactor = (float)Math.Sqrt((double)shoulderfactor); }


                                if (i >= neweditindex)
                                    sag = sag + sagx;
                                else sag = sag - sagx;

                                //if (i < neweditindex - radius + 15) { dresult += shoulder; shoulder = (shoulder + 1) / 2; }
                                //if (i < neweditindex + radius - 15) { dresult += shoulder; shoulder = (shoulder + 5) / 2; }
                                
                            }
                                
                            data[i] = (byte)(dresult+sag);
                            }        
                    */

                        //data[rstart] = 127;
                        //data[rend] = 127;
                    }

                }
            }
            else
            {
                if (editmode == 0)
                {
                    UndoData ud = new()
                    {
                        offset = EditIndex - radius,
                        undodata = EditTempBuf
                    };
                    Undo.Add(ud);
                    EditTempBuf = null;
                }

                if (editmode == 1)
                {
                    if (EditTempBuf != null)
                    {
                        UndoData ud = new()
                        {
                            offset = EditTempBufIndex,
                            undodata = EditTempBuf
                        };
                        Undo.Add(ud);
                        EditTempBuf = null;
                    }
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

                        if (skip)
                        {
                            g[j >> 16, y] += val;
                        }

                        //if (g[j >> 16, y] > maxvalue) maxvalue = g[j >> 16, y];
                        j += decInc;
                        skip = true;
                    }
                    return;
                }
                longLen += y;
                for (int j = 0x8000 + (x << 16); y >= longLen; --y)
                {
                    if (skip)
                    {
                        g[j >> 16, y] += val;
                    }
                    //if (g[j >> 16, y] > maxvalue) maxvalue = g[j >> 16, y];
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
                    if (skip)
                    {
                        g[x, j >> 16] += val;
                    }
                    //if (g[x, j >> 16] > maxvalue) maxvalue = g[j >> 16, y];
                    j += decInc;
                    skip = true;
                }
                return;
            }
            longLen += x;
            for (int j = 0x8000 + (y << 16); x >= longLen; --x)
            {
                if (skip)
                {
                    g[x, j >> 16] += val;
                }
                //if (g[x, j >> 16] > maxvalue) maxvalue = g[j >> 16, y];
                j -= decInc;
                skip = true;
            }
        }
    }
} // Namespace