using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Collections.Concurrent;
using FloppyControlApp.MyClasses;

namespace FloppyControlApp
{
    // Pass settings to processing methods, threaded


    

    /// <summary>
    /// Implements a 32-bit CRC hash algorithm compatible with Zip etc.
    /// </summary>
    /// <remarks>
    /// Crc32 should only be used for backward compatibility with older file formats
    /// and algorithms. It is not secure enough for new applications.
    /// If you need to call multiple times for the same data either use the HashAlgorithm
    /// interface or remember that the result of one Compute call needs to be ~ (XOR) before
    /// being passed in as the seed for the next Compute call.
    /// </remarks>
    public class Crc32 : HashAlgorithm
    {
        public const UInt32 DefaultPolynomial = 0xedb88320u;
        public const UInt32 DefaultSeed = 0xffffffffu;

        static UInt32[] defaultTable;

        readonly UInt32 seed;
        readonly UInt32[] table;
        UInt32 hash;

        public Crc32()
            : this(DefaultPolynomial, DefaultSeed)
        {
        }

        public Crc32(UInt32 polynomial, UInt32 seed)
        {
            table = InitializeTable(polynomial);
            this.seed = hash = seed;
        }

        public override void Initialize()
        {
            hash = seed;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            hash = CalculateHash(table, hash, array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt32ToBigEndianBytes(~hash);
            HashValue = hashBuffer;
            return hashBuffer;
        }

        public override int HashSize { get { return 32; } }

        public static UInt32 Compute(byte[] buffer)
        {
            return Compute(DefaultSeed, buffer);
        }

        public static UInt32 Compute(UInt32 seed, byte[] buffer)
        {
            return Compute(DefaultPolynomial, seed, buffer);
        }

        public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
        }

        static UInt32[] InitializeTable(UInt32 polynomial)
        {
            if (polynomial == DefaultPolynomial && defaultTable != null)
                return defaultTable;

            var createTable = new UInt32[256];
            for (var i = 0; i < 256; i++)
            {
                var entry = (UInt32)i;
                for (var j = 0; j < 8; j++)
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry >>= 1;
                createTable[i] = entry;
            }

            if (polynomial == DefaultPolynomial)
                defaultTable = createTable;

            return createTable;
        }

        static UInt32 CalculateHash(UInt32[] table, UInt32 seed, IList<byte> buffer, int start, int size)
        {
            var crc = seed;
            for (var i = start; i < size - start; i++)
                crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
            return crc;
        }

        static byte[] UInt32ToBigEndianBytes(UInt32 uint32)
        {
            var result = BitConverter.GetBytes(uint32);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }
    }

    

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

        public System.Drawing.Bitmap BmpBuf { get; set; }

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

                using (System.Drawing.Graphics formGraphics = panel.CreateGraphics())
                {
                    if (Changed)
                    {

                        if (BmpBuf != null)
                            BmpBuf.Dispose();
                        BmpBuf = new System.Drawing.Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
                        {

                            LockBitmap lockBitmap = new LockBitmap(BmpBuf);
                            lockBitmap.LockBits();
                            if (clear) lockBitmap.FillBitmap(FillColor);

                            step = (float)Width / (float)(Datalength - 1);
                            //if (step >= width) step = 1;
                            datastep = (Datalength - 1.0f) / (float)Width;
                            //if (datastep >= width && datastep < width * 4) datastep /= 4;
                            //if (datastep < width) datastep = 1;

                            /*
                            tbreceived.Append("datastep:" + datastep + " step: " + step + " datalength: " + datalength +
                                "number of steps in for():" + datalength / datastep + " width:" + width + "\r\n");
                            tbreceived.Append("Start:" + (dataoffset + datastep).ToString() + " stop:" + (dataoffset + datalength).ToString() + "\r\n");
                            */
                            x1 = 0;
                            y1 = (int)(Data[Dataoffset] * YScale - (128 * YScale));

                            yoffset2 = YOffset + (Height / 2);
                            int xpos = 1;
                            if (Datalength + Dataoffset < Data.Length)
                                for (i = Dataoffset + datastep; i < Dataoffset + Datalength; i += datastep)
                                {
                                    x2 = xpos;
                                    y2 = (int)(Data[(int)i] * YScale - (128 * YScale));

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

                using (System.Drawing.Graphics formGraphics = panel.CreateGraphics())
                {
                    if (Changed)
                    {
                        if (BmpBuf != null)
                            BmpBuf.Dispose();
                        BmpBuf = new System.Drawing.Bitmap(Width, Height);
                        {
                            maxvalue = 1;
                            g = new long[Width + 1, Height + 1];

                            LockBitmap lockBitmap = new LockBitmap(BmpBuf);
                            lockBitmap.LockBits();
                            if (clear) lockBitmap.FillBitmap(FillColor);

                            step = ((double)Width / (double)(Datalength - 1)) * Density;
                            //if (step >= width) step = 1;
                            datastep = (Datalength - 1.0f) / (float)Width;
                            //if (datastep >= width && datastep < width * 4) datastep /= 4;
                            //if (datastep < width) datastep = 1;

                            /*
                            tbreceived.Append("datastep:" + datastep + " step: " + step + " datalength: " + datalength +
                                "number of steps in for():" + datalength / datastep + " width:" + width + "\r\n");
                            tbreceived.Append("Start:" + (dataoffset + datastep).ToString() + " stop:" + (dataoffset + datalength).ToString() + "\r\n");
                            */
                            x1 = 0;
                            y1 = (int)(Data[Dataoffset] * YScale - (128 * YScale));

                            yoffset2 = YOffset + (Height / 2);
                            double xpos = 1;
                            if (Datalength + Dataoffset < Data.Length)
                                for (i = Dataoffset + datastep; i < Dataoffset + Datalength; i += Density)
                                {
                                    x2 = (int)xpos;
                                    y2 = (int)(Data[(int)i] * YScale - (128 * YScale));

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

                                    int val1 = (int)((g[x, y]) / (maxvalue / 255));

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
                int swap = shortLen;
                shortLen = longLen;
                longLen = swap;
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
        public System.Drawing.Bitmap BmpBuf { get; set; }
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
            Stopwatch sw = new Stopwatch();

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

                    BmpBuf = new System.Drawing.Bitmap(Width, Height);
                    {
                        //uint Time1 = (uint)(Environment.TickCount + int.MaxValue);
                        //tbreceived.Append("Time: " + (uint)(Environment.TickCount + Int32.MaxValue - Time1) + "ms\r\n");
                        //maxvalue = 1024;
                        Average = 0;
                        TotalNumberOfPoints = 0;
                        g = new long[Width + 1, Height + 1];

                        LockBitmap lockBitmap = new LockBitmap(BmpBuf);
                        lockBitmap.LockBits();
                        //if (clear) lockBitmap.FillBitmap(fillcolor);

                        step = ((double)Width / (double)(DataLength - 1)) * Density;
                        //if (step >= width) step = 1;
                        datastep = (DataLength - 1.0f) / (float)Width;
                        //if (datastep >= width && datastep < width * 4) datastep /= 4;
                        //if (datastep < width) datastep = 1;

                        /*
                        tbreceived.Append("datastep:" + datastep + " step: " + step + " datalength: " + datalength +
                            "number of steps in for():" + datalength / datastep + " width:" + width + "\r\n");
                        tbreceived.Append("Start:" + (dataoffset + datastep).ToString() + " stop:" + (dataoffset + datalength).ToString() + "\r\n");
                        */
                        x1 = 0;
                        y1 = (int)(Data[DataOffset] * YScale - (127 * YScale));
                        float yscale127 = (127 * YScale);
                        int ymax = 0;
                        yoffset2 = YOffset + (Height / 2);
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

                                int val1 = (int)((g[x, y]) / (brightness / 255.0f));

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
                        y1 = (int)(127 * YScale - (127 * YScale));
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
                Data[i] = (byte)((Data[i] - (byte)dcoffset) + 127);

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
                if (i > 5000 && i < (length - smoothing - 5000))
                {
                    if (totalmax < t[i]) totalmax = t[i];
                    if (totalmin > t[i]) totalmin = t[i];
                }
            }

            DCoffset /= (length - (smoothing * 2));
            
            for (i = 0; i < length; i++)
                Data[i] = (byte)((t[i] - (byte)DCoffset) + 127);

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
                    val2 += (float)Data[i + j];
                }
                val2 /= (smoothing * 2.0);
                DCoffset += val2;
                //val = val + (((float)graphwaveform[0][i] - val) / RateOfChange);
                //t[i] = (byte)((val * 0.4f) + (val2 * 0.6f));

                t[i] = val2;
                if (i > 5000 && i < (length - smoothing - 5000))
                {
                    if (totalmax < t[i]) totalmax = t[i];
                    if (totalmin > t[i]) totalmin = t[i];
                }
            }

            DCoffset /= (length - (smoothing * 2));
            
            for (i = 0; i < length; i++)
                Data[i] = (byte)(((t[i] - (byte)DCoffset) + 127.0));

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
            int[] history = new int[smoothing*2+1];
            int total = 0;
            for (i = smoothing; i < length - smoothing; i++)
            {
                total -= history[i % (smoothing * 2+1)]; // subtract oldest value
                history[i % (smoothing * 2+1)] = Data[i + smoothing];
                
                total += Data[i+smoothing];
                val2 = total/(double)(smoothing*2.0d);
                
                DCoffset += val2;
                
                t[i] = val2;
                if (i > 5000 && i < (length - smoothing - 5000))
                {
                    if (totalmax < t[i]) totalmax = t[i];
                    if (totalmin > t[i]) totalmin = t[i];
                }
            }

            DCoffset /= (length - (smoothing * 2));

            for (i = 0; i < length; i++)
                Data[i] = (byte)(((t[i] - (byte)DCoffset) + 127.0));

        }

        public int ViewToDataY(int viewy)
        {
            int result;
            float yscale127 = (127 * YScale);

            //int y2 = (int)(data[0] * yscale - yscale127);

            int yoffset2 = YOffset + (Height / 2);

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
            float offsetfactor = (float)x / (float)Width;
            int result = (int)(DataOffset + (DataLength * offsetfactor));
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
                            d = (float)i / (float)radius;
                        else
                            d = 1 - ((float)(i - radius) / (float)radius);

                        d1 = 1 - d;
                        d1 *= d1;

                        d = 1 - d1;
                        //d *= d1;
                        dresult = ((EditTempBuf[i] * (1 - d)) + (y * d));
                        Data[EditIndex - radius + i] = (byte)dresult;
                    }
                }
                else if (editmode == 1)
                {
                    radius = (40+extendradius + editoption * 20)/2;
                    if (editoption == 0) radius -= extendradius;
                    if( EditTempBuf != null)
                    if ( EditTempBuf.Length > 0)
                        for (int i  = 0; i < EditTempBuf.Length; i++)
                            Data[EditTempBufIndex+i] = EditTempBuf[i];

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
                        for (int i  = 1; i < 20; i++)
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
                                EditTempBufIndex = NewEditIndex - radius -5;
                                EditTempBuf = Data.SubArray(NewEditIndex - radius-5, radius * 2 + 5);
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
                    UndoData ud = new UndoData
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
                        UndoData ud = new UndoData
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
                int swap = shortLen;
                shortLen = longLen;
                longLen = swap;
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

    public class Graphset
    {
        public string Filename { get; set; }
        public List<Graph2> Graphs { get; set; }
        public PictureBox Panel { get; set; }
        public bool Dragging { get; set; }
        public Color Fillcolor { set; get; }
        public StringBuilder Tbreceived { set; get; }
        private Bitmap Bmp;
        public int Bmpxoffset { set; get; }
        public int Xrelative { set; get; }
        public int Binfilecount { get; set; }
        public int Editmode { get; set; }
        public int Editoption { get; set; }
        public int Editperiodextend { get; set; }
        public Color[] Colors { get; set; }
        private System.Windows.Forms.Timer repaintdelay = new System.Windows.Forms.Timer();
        public bool Allowrepaint { get; set; }

        public Action UpdateGUI { get; set; }
        public Action GetControlValues { get; set; }

        public Graphset(PictureBox p, Color fill)
        {
            Editmode = 0;
            Editoption = 0;
            Colors = new Color[5];
            Colors[0] = Color.FromArgb(255, 255, 255, 0);
            Colors[1] = Color.FromArgb(255, 240, 0, 0);
            Colors[2] = Color.FromArgb(255, 0, 200, 0);
            Colors[3] = Color.FromArgb(255, 128, 255, 255);
            Colors[4] = Color.FromArgb(255, 255, 0, 255);

            Allowrepaint = true;
            repaintdelay.Interval = 500;
            repaintdelay.Tick += Repaintdelay_Tick;

            Graphs = new List<Graph2>();
            Dragging = false;
            Fillcolor = Color.White;
            Fillcolor = fill;
            Panel = p;

            Panel.MouseDown += GraphPictureBox_MouseDown;
            Panel.MouseUp += GraphPictureBox_MouseUp;
            Panel.MouseMove += GraphPictureBox_MouseMove;
            Panel.MouseEnter += GraphPictureBox_MouseEnter;
            Panel.Paint += GraphPictureBox_Paint_1;
            Panel.MouseWheel += GraphPictureBox_MouseWheel;

            Bmp = new Bitmap(Panel.Width, Panel.Height, PixelFormat.Format32bppPArgb);
            //lockbitmap = new LockBitmap(bmp);
        }

        private void Repaintdelay_Tick(object sender, EventArgs e)
        {
            repaintdelay.Stop();
            Allowrepaint = true;
            DoUpdateGraphs();
        }

        public void Dispose()
        {
            Bmp.Dispose();
        }

        public void Resize()
        {
            if(    Panel != null 
                && Panel.Height != 0 
                && Panel.Width != 0)
                Bmp = new Bitmap(Panel.Width, Panel.Height);
        }

        public void AddGraph(byte[] data)
        {
            Graph2 newgraph = new Graph2
            {
                GraphColor = Colors[Graphs.Count],
                Data = data,
                Width = Panel.Width,
                Height = Panel.Height,
                TBReceived = Tbreceived
            };
            Graphs.Add(newgraph);

        }

        public void UpdateGraphs()
        {

            UpdateGUI?.Invoke();
            if (Allowrepaint == true)
            {
                DoUpdateGraphs();
            }
            else
            {
                repaintdelay.Start();
            }
        }

        public void DoUpdateGraphs()
        {
            Process proc = Process.GetCurrentProcess();
            LockBitmap lockbitmap = new LockBitmap(Bmp);

            lockbitmap.LockBits();

            //Debug.WriteLine("5a proc: " + string.Format("{0:n0}", proc.PrivateMemorySize64));
            //tbreceived.Append("UpdateGraphs! "+Graphs[0].changed+"\r\n");
            lockbitmap.FillBitmap(Fillcolor);
            lockbitmap.Line(Panel.Width / 2, 0, Panel.Width / 2, Panel.Height, Color.FromArgb(255, 64, 64, 64));
            lockbitmap.UnlockBits();
            //Debug.WriteLine("8 proc: " + string.Format("{0:n0}", proc.PrivateMemorySize64));

            System.Drawing.Graphics panelgraphics = Panel.CreateGraphics();

            Graphics g = Graphics.FromImage(Bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            List<Graph2> zordered = Graphs.OrderBy(o => o.ZOrder).ToList();

            foreach (var gr in zordered)
            {
                gr.Width = Panel.Width;
                gr.Height = Panel.Height;

                if (gr.Changed == true)
                    gr.DoGraph2();
                g.DrawImage(gr.BmpBuf, 0, 0);
            }

            panelgraphics.DrawImage(Bmp, Bmpxoffset, 0);

            g.Dispose();
            panelgraphics.Dispose();

            lockbitmap.Dispose();
            lockbitmap = null;

        }

        public void SetAllChanged()
        {
            foreach (var gr in Graphs)
            {
                gr.Changed = true;
            }
        }

        public void SaveAll()
        {
            //int i;
            if (Graphs.Count == 0) return;
            string[] fsplit = Filename.Split('_');

            string outputfilename = fsplit[0];
            try
            {
                Binfilecount = Int32.Parse((fsplit[2].Split('.')[0]));
            }
            catch
            {
                Binfilecount = 0;
            }
            string subpath = @FloppyControlApp.Properties.Settings.Default["PathToRecoveredDisks"].ToString();

            string path = subpath + @"\" + outputfilename + @"\";

            string fullpath = path + outputfilename +
                "_" + fsplit[1]
                + "_" + Binfilecount.ToString("D3") + ".wvfrm";

            Directory.CreateDirectory(path);

            while (File.Exists(fullpath))
            {
                Binfilecount++;
                fullpath = path + outputfilename + "_" +
                 fsplit[1] +
                "_" + Binfilecount.ToString("D3") + ".wvfrm";
            }
            Tbreceived.Append("Saving file " + fullpath + "\r\n");

            // Write with counter so no overwrite is performed
            BinaryWriter writer;
            writer = new BinaryWriter(new FileStream(fullpath, FileMode.Create));
            int j;
            writer.Write((byte)Graphs.Count); // number of waveforms
            writer.Write((int)Graphs[0].Data.Length); // length of waveform
            for (j = 0; j < Graphs.Count; j++)
            {
                writer.Write(Graphs[j].Data);
            }

            if (writer != null)
            {
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }

        // Handle MouseWheel zoom on graph
        private void GraphPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            //tbreceived.Append("MouseWheel: "+e.Delta+" X: "+e.X+"\r\n");
            int i;
            int grphcnt = Graphs.Count;
            int x = e.X;
            if (Graphs.Count == 0)
            {
                return;
            }
            float offsetfactor = (float)x / (float)Graphs[0].Width;

            for (i = 0; i < grphcnt; i++)
            {
                if (Graphs[i].DataLength == 500 && e.Delta > 0) return;
                if (e.Delta < 0 && Graphs[i].DataLength == Graphs[i].Data.Length - 1) return;
            }

            if (e.Delta > 0) // zoom in
            {
                for (i = 0; i < grphcnt; i++)
                {
                    if (Graphs[i].DataLength / 2 < 500) Graphs[i].DataLength = 500;
                    else Graphs[i].DataLength /= 2;
                    Graphs[i].Changed = true;

                    Graphs[i].DataOffset = (int)(Graphs[i].DataOffset + (Graphs[0].DataLength * offsetfactor)); //center zoom
                    //graph[i].dataoffset = (int)(graph[i].dataoffset + (graph[0].datalength /2)); //center zoom
                }
            }
            else // zoom out
            {
                for (i = 0; i < grphcnt; i++)
                {
                    if (Graphs[i].DataLength * 2 > Graphs[i].Data.Length - 1) Graphs[i].DataLength = Graphs[i].Data.Length - 1;
                    else Graphs[i].DataLength *= 2;
                    Graphs[i].Changed = true;
                    Graphs[i].DataOffset = (int)((Graphs[i].DataOffset) - (Graphs[i].DataLength / 2.0 * offsetfactor));
                }
            }
            
            //DataOffsetTrackBar.Maximum = offsetmax;
            int offset = Graphs[0].DataOffset;
            int viewdatalength = Graphs[0].DataLength;
            int datalength = Graphs[0].Data.Length;

            if (offset + viewdatalength > (datalength - 1))
            {
                offset = 0;
                for (i = 0; i < grphcnt; i++)
                    Graphs[i].DataOffset = offset;

            }
            if (offset < 0)
            {
                offset = 0;
                for (i = 0; i < grphcnt; i++)
                    Graphs[i].DataOffset = offset;

            }
            //DataOffsetTrackBar.Value = Graphs[0].dataoffset;

            //DataLengthTrackBar.Value = viewdatalength;
            int density = (int)Math.Log(viewdatalength / 512.0, 1.4f);//datalength/graph[0].width;
            if (density <= 0) density = 1;
            if (viewdatalength <= 64000) density = 1;

            for (i = 0; i < Graphs.Count; i++)
                Graphs[i].Density = density;

            //string.Format("{0:n0}", indexrxbuf);
            UpdateGraphs();
        }

        private void GraphPictureBox_Paint_1(object sender, PaintEventArgs e)
        {
            if (Graphs.Count > 0)
            {
                UpdateGraphs();
            }
        }

        private void GraphPictureBox_MouseEnter(object sender, EventArgs e)
        {
            Panel.Focus();
        }

        private void GraphPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Dragging = true;
                Xrelative = e.X;
            }
            else if (e.Button == MouseButtons.Right)
            {
                if( Graphs.Count > 0)
                if (Graphs[0].DataLength < 2001)
                {
                    //editmode = 
                    if (GetControlValues == null)
                    {
                        Tbreceived.Append("GetControlValues not set!");
                        return;
                    }
                    GetControlValues();


                    int index = Graphs[0].ViewToGraphIndex(e.X);

                    if (Graphs[0].EditGraphActive == false)
                    {
                        Graphs[0].EditGraphActive = true;
                        Graphs[0].EditIndex = index;
                    }

                    int datay = Graphs[0].ViewToDataY(e.Y);
                    byte data = Graphs[0].Data[index];

                    Graphs[0].EditGraph(datay, 20, Editmode, Editoption, Editperiodextend);
                    Tbreceived.Append("RMB Down index:" + index + " datay:" + datay + " data:" + data + "\r\n");
                    Graphs[0].Changed = true;

                    UpdateGraphs();
                }
            }
        }

        private void GraphPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            //tbreceived.Append("Mouse button up before processing\r\n");
            if (e.Button == MouseButtons.Left)
            {
                int grphcnt = Graphs.Count;

                Dragging = false;
                int i;
                int offset;

                if (Graphs.Count == 0) return;

                float factor = (float)Graphs[0].DataLength / (float)Graphs[0].Width;

                for (i = 0; i < grphcnt; i++)
                {
                    offset = (int)((Xrelative - e.X) * factor);
                    if (Graphs[i].DataOffset + offset < 0)
                    {
                        offset = 0;
                        Graphs[i].DataOffset = 0;
                    }

                    if (Graphs[i].DataOffset + Graphs[i].DataLength + offset > Graphs[i].Data.Length - 1)
                    {
                        offset = 0;
                        Graphs[i].DataOffset = (Graphs[i].Data.Length - 1) - Graphs[i].DataLength;
                    }

                    Graphs[i].DataOffset += offset;
                    Graphs[i].Changed = true;
                    Bmpxoffset = 0;
                    //tbreceived.Append("Offset: " + offset+ "\r\n");
                    
                }
                UpdateGraphs();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Graphs.Count == 0) return;
                int datay = Graphs[0].ViewToDataY(e.Y);
                //byte data = Graphs[0].data[index];
                Graphs[0].EditGraphActive = false;
                Graphs[0].EditGraph(datay, 20, Editmode, Editoption, Editperiodextend);

                //tbreceived.Append("RMB index:" + index + " datay:" + datay + " data:" + data + "\r\n");
                Graphs[0].Changed = true;

                UpdateGraphs();

                Tbreceived.Append("RMB up\r\n");
                
            }

        }

        private void GraphPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!(Dragging)) return;
                //int i;
                if (Graphs.Count == 0) return;

                float zoomlevel = Graphs[0].DataLength / (float)Graphs[0].Data.Length;

                //tbreceived.Append("zoomlevel: " + zoomlevel + "\r\n");

                if (zoomlevel > 0.99) return;
                int xoffset = e.X - Xrelative;
                if (Graphs[0].DataOffset == 0 && xoffset > 0) return;
                if (Graphs[0].DataOffset + Graphs[0].DataLength >= Graphs[0].Data.Length - 1 && xoffset < 0) return;
                Bmpxoffset = xoffset;
                UpdateGraphs();
            }
            else
            if (e.Button == MouseButtons.Right)
            {
                if( Graphs.Count > 0)
                if (Graphs[0].DataLength < 2000)
                {
                    int datay = Graphs[0].ViewToDataY(e.Y);
                    //byte data = Graphs[0].data[index];

                    Graphs[0].EditGraph(datay, 20, Editmode, Editoption, Editperiodextend);
                    //tbreceived.Append("RMB index:" + index + " datay:" + datay + " data:" + data + "\r\n");
                    Graphs[0].Changed = true;

                    UpdateGraphs();

                    Tbreceived.Append("RMBMove\r\n");
                }
            }
        }
    }

    public class Histogram
    {
        private byte[] histogram = new byte[256];
        private Panel panel;
        private float scaling;
        public int HD { get; set; }
        public byte[] Data { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }
        public StringBuilder TBReceived { get; set; }

        public Histogram()
        {
            HD = 0;
        }

        public float GetScaling()
        {
            return scaling;
        }

        public void SetPanel(Panel p)
        {
            panel = p;
        }


        // The histogram is 256x100 pixels.
        /// <summary>
        /// Prepare the histogram data before rendering
        /// </summary>
        public void DoHistogram(byte[] d, int offset1, int length1)
        {
            Data = d;
            Offset = offset1;
            Length = length1;
            DoHistogram();
        }

        public void DoHistogram()
        {
            int i;

            if (Data == null) return;
            if (Offset + Length > Data.Length) return;

            int[] histogramint = new int[256];

            int histogrammax;

            if (Length == 0) Length = Data.Length;

            if (!(Data.Length >= (Offset + Length))) return;

            //Create histogram of the track period data

            //reset the histogram
            for (i = 0; i < 256; i++)
            {
                histogram[i] = 0;
            }

            // count the period lengths grouped by period length, skip 0
            for (i = Offset; i < Offset + Length; i++)
            {
                //if (data[i] > 0)
                histogramint[Data[i] << HD & 0xff]++;
            }

            // Find the maximum value so we can normalize the histogram down to 100 to fit inside histogram graph
            histogrammax = 0;
            for (i = 0; i < 256; i++)
            {
                if (histogramint[i] > histogrammax)
                {
                    histogrammax = histogramint[i];
                }
            }

            scaling = ((float)100 / histogrammax);
            int temp;
            //Scale the histogram to fit inside 256x100 pixels
            for (i = 0; i < 256; i++)
            {
                temp = (int)(histogramint[i] * scaling); //We use the second highest peak to normalize
                if (temp > 100) temp = 100; // if the highest peak is too high, clip it to 100
                if (temp == 0 && histogramint[i] > 0) temp = 1;
                histogram[i] = (byte)temp;
                //textBoxReceived.Text += "Histogram: " + i.ToString() + " " + histogramint[i] + 
                //    " hist:" + histogram[i].ToString() + "\r\n";
            }

            // Draw the histogram
            HistogramDraw();
        }

        /// <summary>
        /// Draw a histogram on a panel form object
        /// </summary>
        public void HistogramDraw()
        {
            int i;

            System.Drawing.Graphics formGraphics = panel.CreateGraphics();
            formGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            formGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            formGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
            formGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            using (var bmp = new System.Drawing.Bitmap(580, 102, PixelFormat.Format32bppPArgb))
            {
                LockBitmap lockBitmap = new LockBitmap(bmp);
                lockBitmap.LockBits();
                lockBitmap.FillBitmap(SystemColors.Control);

                lockBitmap.Line(000, 000, 256, 000, Color.Black);
                lockBitmap.Line(256, 000, 256, 100, Color.Black);
                lockBitmap.Line(256, 101, 000, 101, Color.Black);
                lockBitmap.Line(000, 100, 000, 000, Color.Black);

                for (i = 0; i < 256; i++)
                {
                    if (histogram[i] > 0)
                        lockBitmap.Line(i + 1, 100 - histogram[i], i + 1, 100, Color.Black);
                }

                lockBitmap.UnlockBits();
                formGraphics.DrawImage(bmp, 0, 0);
            }
            formGraphics.Dispose();
        }
    }

    public class LockBitmap
    {
        Bitmap source = null;
        IntPtr Iptr = IntPtr.Zero;
        BitmapData bitmapData = null;

        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int XPos { get; private set; }
        public int YPos { get; private set; }

        public LockBitmap(Bitmap source)
        {
            this.source = source;
        }

        /// <summary>
        /// Lock bitmap data
        /// </summary>
        public void LockBits()
        {
            try
            {
                //Process proc = Process.GetCurrentProcess();
                //Debug.WriteLine("1 proc: "+ string.Format("{0:n0}", proc.PrivateMemorySize64));
                // Get width and height of bitmap
                Width = source.Width;
                Height = source.Height;

                // get total locked pixels count
                int PixelCount = Width * Height;

                // Create rectangle to lock
                Rectangle rect = new Rectangle(0, 0, Width, Height);

                // get source bitmap pixel format size
                Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);

                // Check if bpp (Bits Per Pixel) is 8, 24, or 32
                if (Depth != 8 && Depth != 24 && Depth != 32)
                {
                    throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                }
                //Debug.WriteLine("2 proc: " + string.Format("{0:n0}", proc.PrivateMemorySize64) );
                // Lock bitmap and return bitmap data
                bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
                                             source.PixelFormat);
                //Debug.WriteLine("3 proc: " + string.Format("{0:n0}", proc.PrivateMemorySize64));
                // create byte array to copy pixel values
                int step = Depth / 8;
                Pixels = new byte[PixelCount * step];
                //Debug.WriteLine("4 proc: " + string.Format("{0:n0}", proc.PrivateMemorySize64));
                Iptr = bitmapData.Scan0;

                // Copy data from pointer to array
                Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
                //Debug.WriteLine("5 proc: " + string.Format("{0:n0}", proc.PrivateMemorySize64));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            bitmapData = null;
            Pixels = null;

        }

        /// <summary>
        /// Unlock bitmap data
        /// </summary>
        public void UnlockBits()
        {
            //Process proc = Process.GetCurrentProcess();
            //Debug.WriteLine("6 proc: " + string.Format("{0:n0}", proc.PrivateMemorySize64));
            try
            {
                // Copy data from byte array to pointer
                Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

                // Unlock bitmap data
                source.UnlockBits(bitmapData);
                Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Debug.WriteLine("7 proc: " + string.Format("{0:n0}", proc.PrivateMemorySize64));
        }

        /// <summary>
        /// Get the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            Color clr = Color.Empty;

            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                byte a = Pixels[i + 3]; // a
                clr = Color.FromArgb(a, r, g, b);
            }
            if (Depth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                clr = Color.FromArgb(r, g, b);
            }
            if (Depth == 8)
            // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                byte c = Pixels[i];
                clr = Color.FromArgb(c, c, c);
            }
            return clr;
        }

        /// <summary>
        /// Set the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, Color color)
        {
            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (i >= Pixels.Length)
                return;

            if (i < 0)
                return;

            if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
                Pixels[i + 3] = color.A;
            }
            if (Depth == 24) // For 24 bpp set Red, Green and Blue
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
            }
            if (Depth == 8)
            // For 8 bpp set color value (Red, Green and Blue values are the same)
            {
                Pixels[i] = color.B;
            }
        }

        public void FilledSquare(int xpos, int ypos, int width, int height, Color color)
        {
            int w, h;

            for (h = 0; h < height; h++)
                for (w = 0; w < width; w++)
                    SetPixel(xpos + w, ypos + h, color);
        }

        // Quickest way to fill the bitmap with color
        public void FillBitmap(Color color)
        {
            int length = Pixels.Length;
            byte r, g, b, a;
            int i;

            r = color.R;
            g = color.G;
            b = color.B;
            a = color.A;

            if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
            {
                for (i = 0; i < length; i += 4)
                {
                    Pixels[i] = b;
                    Pixels[i + 1] = g;
                    Pixels[i + 2] = r;
                    Pixels[i + 3] = a;
                }
            }
            if (Depth == 24) // For 24 bpp set Red, Green and Blue
            {
                for (i = 0; i < length; i += 3)
                {
                    Pixels[i] = b;
                    Pixels[i + 1] = g;
                    Pixels[i + 2] = r;
                }
            }
            if (Depth == 8)
            // For 8 bpp set color value (Red, Green and Blue values are the same)
            {
                for (i = 0; i < length; i++)
                {
                    Pixels[i] = b;
                }
            }
        }

        public void LineTo(int x2, int y2, Color col)
        {

            Line(XPos, YPos, x2, y2, col);
            XPos = x2;
            YPos = y2;
        }

        // Line drawing routine taken from:
        // http://www.edepot.com/linee.html
        public void Line(int x, int y, int x2, int y2, Color col)
        {
            bool yLonger = false;
            int shortLen = y2 - y;
            int longLen = x2 - x;

            if (Math.Abs(shortLen) > Math.Abs(longLen))
            {
                int swap = shortLen;
                shortLen = longLen;
                longLen = swap;
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
                        SetPixel(j >> 16, y, col);
                        j += decInc;
                    }
                    return;
                }
                longLen += y;
                for (int j = 0x8000 + (x << 16); y >= longLen; --y)
                {
                    SetPixel(j >> 16, y, col);
                    j -= decInc;
                }
                return;
            }

            if (longLen > 0)
            {
                longLen += x;
                for (int j = 0x8000 + (y << 16); x <= longLen; ++x)
                {
                    SetPixel(x, j >> 16, col);
                    j += decInc;
                }
                return;
            }
            longLen += x;
            for (int j = 0x8000 + (y << 16); x >= longLen; --x)
            {
                SetPixel(x, j >> 16, col);
                j -= decInc;
            }
        }
    }

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
            int i,p;
            Random random = new Random();
            int ID = random.Next();
            proc.TBReceived.Append("scatterplot id: " + ID);
            for (i = 255; i > -1; i--)
                Gradient1[i] = 255;

            for (p = 47; p < 132; p += 40)
            {
                for (i = 0; i < 16; i++)
                {
                    Gradient1[i + p] = (byte)(255 - (i * 15));

                }
                for (i = 16; i < 30; i++)
                {
                    Gradient1[i + p] = (byte)(255 - ((15 - (i - 15)) * 15));

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

            using (var bmp = new System.Drawing.Bitmap(580, 446))
            {
                byte value = 0;
                float factor;

                LockBitmap lockBitmap = new LockBitmap(bmp);
                lockBitmap.LockBits();
                lockBitmap.FillBitmap(Color.White);
                datapoints = End - Start;

                if (datapoints == 0) datapoints = 1;

                int width = Panel.Width;
                factor = (float)width / (float)datapoints;

                for (i = 0; i < datapoints; i++)
                {
                    posx = factor * i;
                    value = Rxbuf[i + Start];
                    lockBitmap.SetPixel((int)posx, ((value << Processing.ProcSettings.hd)) & 0xff, Color.FromArgb(255, 0, (255 - Gradient1[value]), Gradient1[value]));
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
                Tbreiceved.Append("rxbuf null detected! scatterplot ID="+ID);
                return;
            }

            if (start + datapoints > Rxbuf.Length) return;

            Graphics formGraphics = Panel.CreateGraphics();
            formGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            formGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            formGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
            formGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            //tbreceived.Append("datalength: " + datalength+"\r\n");
            
            if (datalength < 1250) bigpixels = 3;
            if (datalength >= 1250 && datalength < 5000) bigpixels = 2;
            if (datalength >= 5000) bigpixels = 1;

            using (var bmp = new System.Drawing.Bitmap(580, 446, PixelFormat.Format32bppPArgb))
            {
                byte value = 0;
                float factor;

                LockBitmap lockBitmap = new LockBitmap(bmp);
                lockBitmap.LockBits();
                lockBitmap.FillBitmap(Color.White);

                if (datapoints == 0) datapoints = 1;
                int width = Panel.Width;
                factor = (float)width / (float)datapoints;
                //if (indexrxbuf > rxbuf.Length) indexrxbuf = rxbuf.Length - 1;
                if (datapoints > Rxbuf.Length) datapoints = Rxbuf.Length - 1;
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
                            lockBitmap.FilledSquare((int)posx, ((value << Processing.ProcSettings.hd)) & 0xff, bigpixels, bigpixels, Color.FromArgb(255, 0, (255 - Gradient1[value]), Gradient1[value]));
                            lockBitmap.FilledSquare((int)posx, (int)Processing.entropy[i + start] + 192, bigpixels, bigpixels, Color.FromArgb(255, 0, 0, 0));
                            lockBitmap.FilledSquare((int)posx, (int)Processing.threshold4[i + start], bigpixels, bigpixels, Color.FromArgb(255, 255, 0, 0));
                            lockBitmap.FilledSquare((int)posx, (int)Processing.threshold6[i + start], bigpixels, bigpixels, Color.FromArgb(255, 0, 255, 0));
                            lockBitmap.FilledSquare((int)posx, (int)Processing.threshold8[i + start], bigpixels, bigpixels, Color.FromArgb(255, 0, 0, 255));
                        }
                        else
                        {
                            lockBitmap.SetPixel((int)posx, ((value << Processing.ProcSettings.hd)) & 0xff, Color.FromArgb(255, 0, (255 - Gradient1[value]), Gradient1[value]));
                            if( i+start < Processing.entropy.Length)
                                lockBitmap.SetPixel((int)posx, (int)Processing.entropy[i + start] + 192, Color.FromArgb(255, 0, 0, 0));
                            if (i + start < Processing.threshold4.Length)
                                lockBitmap.SetPixel((int)posx, (int)Processing.threshold4[i + start], Color.FromArgb(255, 255,0, 0));
                            if (i + start < Processing.threshold6.Length)
                                lockBitmap.SetPixel((int)posx, (int)Processing.threshold6[i + start], Color.FromArgb(255, 0, 255, 0));
                            if (i + start < Processing.threshold8.Length)
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
                        if (i == drawcnt) break;

                        if (bigpixels > 1)
                        {
                            lockBitmap.FilledSquare((int)posx, ((value << Processing.ProcSettings.hd)) & 0xff, bigpixels, bigpixels, Color.FromArgb(255, 0, (255 - Gradient1[value]), Gradient1[value]));
                            //lockBitmap.filledsquare((int)posx, (int)processing.entropy[i + start] + 192, bigpixels, bigpixels, Color.FromArgb(255, 0, 0, 0));
                        }
                        else
                        {
                            lockBitmap.SetPixel((int)posx, ((value << Processing.ProcSettings.hd)) & 0xff, Color.FromArgb(255, 0, (255 - Gradient1[value]), Gradient1[value]));
                            //lockBitmap.SetPixel((int)posx, (int)processing.entropy[i + start] + 192, Color.FromArgb(255, 0, 0, 0));
                        }
                    }

                // Draw in marker positions
                int markerpos, relativepos;
                Color c = Color.Black;

                Graphics g = Graphics.FromImage(bmp);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                string tracksector = "";
                RectangleF rectf;
                // Show marker positions if available
                MFMData sectordata;
                if (Sectordata2 != null)
                    for (i = Processing.sectordata2oldcnt; i < Sectordata2.Count; i++)
                    {
                        sectordata = Sectordata2[i];

                        markerpos = sectordata.rxbufMarkerPositions;
                        if (markerpos >= start && markerpos < end)
                        {
                            if (sectordata.Status == SectorMapStatus.CrcOk || sectordata.Status == SectorMapStatus.AmigaCrcOk)
                            {
                                c = Color.FromArgb(64, 0, 255, 0);
                                tracksector = "T" + sectordata.track.ToString("D3") + " S" + sectordata.sector.ToString();
                            }
                            else
                            if (sectordata.Status == SectorMapStatus.HeadOkDataBad || sectordata.Status == SectorMapStatus.AmigaHeadOkDataBad)
                            {
                                c = Color.FromArgb(64, 255, 0, 0);
                                tracksector = "T" + sectordata.track.ToString("D3") + " S" + sectordata.sector.ToString();
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
                                        if(sectordata.MarkerType != MarkerType.data)
                                            rectf = new RectangleF(posx, 266, 90, 20);
                                        else rectf = new RectangleF(posx, 256, 90, 20);
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

                if( Threshold4us < 50)
                {
                    //throw new ArgumentException("Threshold4us too low!");
                }

                //Show thresholds
                lockBitmap.Line(0, Thresholdmin, 580, Thresholdmin, Color.Red);
                lockBitmap.Line(0, Threshold4us, 580, Threshold4us, Color.Red);
                lockBitmap.Line(0, Threshold6us, 580, Threshold6us, Color.Red);
                lockBitmap.Line(0, Thresholdmax, 580, Thresholdmax, Color.Red);
                lockBitmap.UnlockBits();

                RectangleF rectf2 = new RectangleF(0, 280, 256, 40);
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

            double offsetfactor = (float)x / (float)Panel.Width;
            //tbreceived.Append("indexrxbuf: "+indexrxbuf+"\r\n");

            if (AnScatViewlength == minimumZoom && e.Delta > 0) return;

            if (e.Delta < 0 && AnScatViewlength == length - 1) return;

            if (e.Delta > 0) // zoom in
            {
                if (AnScatViewlength / 2 < minimumZoom) AnScatViewlength = minimumZoom;
                else AnScatViewlength /= 2;

                AnScatViewoffset = (int)(AnScatViewoffset + (AnScatViewlength * offsetfactor));
            }
            else // zoom out
            {
                if (AnScatViewlength * 2 > length - 1) AnScatViewlength = length - 1;
                else AnScatViewlength *= 2;
                AnScatViewoffset = (int)((AnScatViewoffset) - (AnScatViewlength / 2.0 * offsetfactor));
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
                if( RxbufClickIndex < Rxbuf.Length-1)
                //tbreiceved.Append("Y: "+e.Y+" val: "+processing.rxbuf[rxbufclickindex]+" rxclickindex: "+rxbufclickindex+"\r\n");

                Xrelative = e.X;
                Tbreiceved.Append("xrelative:"+Xrelative+"\r\n");
                AnScatViewoffsetOld = AnScatViewoffset;
                AnScatViewlargeoffsetold = AnScatViewlargeoffset;
            }
            else if (e.Button == MouseButtons.Right)
            {
                RxbufClickIndex = ViewToGraphIndex(e.X);
                if (Processing.Rxbuftograph != null)
                {
                    if(RxbufClickIndex< Processing.Rxbuftograph.Length)
                        GraphIndex = Processing.Rxbuftograph[RxbufClickIndex-1];
                    
                }
                ShowGraph();
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            RxbufClickIndex = ViewToGraphIndex(e.X);
            
            if (e.Button == MouseButtons.Left)
            {
                DoDragging(sender,  e);
            }
            if (e.Button == MouseButtons.Right)
            {
                if (EditScatterplot == true && AnScatViewlength <= 0x40)
                {
                    int xoffset = (int)(Panel.Width / AnScatViewlength) / 2;

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
                AnScatViewoffset = (Maxdots - 1) - AnScatViewlength;
            }
                
            Bmpxoffset = 0;
            Start = AnScatViewoffset;
            End = Start + AnScatViewlength;
                
            UpdateScatterPlot();
        }

        public int ViewToGraphIndex(int x)
        {
            float offsetfactor = (float)x / (float)Panel.Width;
            int result = (int)(AnScatViewoffset + (AnScatViewlength * offsetfactor)+ AnScatViewlargeoffset);
            return result;
        }

    } // Scatterplot class
} // Namespace