using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloppyControlApp.MyClasses.Processing.ProcessingTypes
{
    public partial class ProcessingTypes
    {
        public byte[] ProcTypeNormal(ProcTypeArgs ProctypeArgs, long ThreadId, ref int Stop)
        {
			long i;
            int value;

			long MINUS, FOURUS, SIXUS, EIGHTUS, start, end;
            float RateOfChange;
            System.Diagnostics.Stopwatch SW = new System.Diagnostics.Stopwatch();

            //Expand ProcTypeArgs
            MINUS = ProctypeArgs.MINUS;
            SIXUS = ProctypeArgs.SIXUS;
            EIGHTUS = ProctypeArgs.EIGHTUS;
            FOURUS = ProctypeArgs.FOURUS;
            var tbreceived = ProctypeArgs.Tbreceived;
            var rxbuf = ProctypeArgs.Rxbuf;
            var mfmlengths = ProctypeArgs.Mfmlengths;
            var Procsettings = ProctypeArgs.Procsettings;
            var Progresses = ProctypeArgs.Progresses;
            RateOfChange = ProctypeArgs.RateOfChange;

            start = Procsettings.start;
            end = Procsettings.end;
            ProcessingType processingtype = Procsettings.processingtype;

            // bounds checking
            if (end - start == 0)
            {
                tbreceived.Append("Period2MFM: Error: Length can't be zero.\r\n");
                //stop = 1;
                //PrintProperties(procsettings);
                return null;
            }

            int rxbuflength = rxbuf.Length;
            if (start > rxbuflength || end > rxbuflength)
            {
                tbreceived.Append("Start or end are larger than rxbuf length. Resetting to start=0 and end = rxbuflength\r\n");
                start = 0;
                end = rxbuflength - 1;
            }

            if (end < 0 || start < 0)
            {
                tbreceived.Append("Start or end can't be a negative value!\r\n");
                return null;
            }

            byte[] m;
            try
            {
                if (Procsettings.AddNoise || Procsettings.pattern == 4)
                    m = new byte[((end - start) * 5)]; // mfm data can be max. 4x period data
                else m = new byte[((end - start) * 4)]; // mfm data can be max. 4x period data
            }
            catch (OutOfMemoryException)
            {
                tbreceived.Append("Out of memory. Please load a smaller dataset and try again.");
                return null;
            }
            if (MINUS < 0) MINUS = 0;

            //tbreceived.Append("Normal\r\n");

            // This ensures that the period data can be synched with mfm and marker data
            // Which is important for error correction methods
            if (Procsettings.UseErrorCorrection)
            {
                MINUS = 0;
                EIGHTUS = 255;
            }
            Random rnd = new Random();
            int rand = 0;
            int sectorboundary = 0;
            int val;
            //int k;
            int stat4us = 0;
            int stat6us = 0;
            int stat8us = 0;
            for (i = start; i < end; i++)
            {
                if (i % 250000 == 249999) { Progresses[ThreadId] = i; if (Stop == 1) break; }

                sectorboundary = (i - start + 50) % 9000;

                if (Procsettings.AddNoise == true 
                    && sectorboundary >= Procsettings.addnoiselimitstart 
                    && sectorboundary <= Procsettings.addnoiselimitend)
                    rand = rnd.Next(-1, 1) * Procsettings.addnoiserndamount;
                //    rand = rnd.Next(0 - procsettings.addnoiserndamount, procsettings.addnoiserndamount);
                else
                    rand = 0;
                //value2 = (rxbuf[i]<<hd);
                //value = value2 + rand;
                val = rxbuf[i];
                value = (val << Procsettings.hd) + rand;
                if (val < 4) continue;
                if (value >= MINUS && value < FOURUS)
                //if (value < FOURUS)
                {
                    stat4us++;
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                }
                else
                if (value >= FOURUS && value < SIXUS)
                {
                    stat6us++;
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                }
                else
                if (value >= SIXUS && value <= EIGHTUS)
                //if (value >= SIXUS)
                {
                    stat8us++;
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                }
                else
                {
                    if (Procsettings.pattern == 1)
                    {
                        m[mfmlengths[ThreadId]++] = 1;
                        m[mfmlengths[ThreadId]++] = 0;
                    }
                    else if (Procsettings.pattern == 2)
                    {
                        m[mfmlengths[ThreadId]++] = 1;
                        m[mfmlengths[ThreadId]++] = 0;
                        m[mfmlengths[ThreadId]++] = 0;
                    }
                    else if (Procsettings.pattern == 3)
                    {
                        m[mfmlengths[ThreadId]++] = 1;
                        m[mfmlengths[ThreadId]++] = 0;
                        m[mfmlengths[ThreadId]++] = 0;
                        m[mfmlengths[ThreadId]++] = 0;
                    }
                    else if (Procsettings.pattern == 4)
                    {

                        float s1, s2, s3;
                        int total = stat4us + stat6us + stat8us;
                        if (total == 0) total = 1;
                        s1 = stat4us / (float)total * 100.0f;
                        s2 = (float)stat6us / (float)total * 100.0f;
                        s3 = (float)stat8us / (float)total * 100.0f;
                        int weight = rnd.Next(0, 99);
                        //s1 = 25;
                        //s2 = 50;
                        //s3 = 75;
                        m[mfmlengths[ThreadId]++] = 1;

                        if (weight < s1)
                            m[mfmlengths[ThreadId]++] = 0;
                        else
                        if (weight >= s2 && weight < s3)
                        {
                            m[mfmlengths[ThreadId]++] = 0;
                            m[mfmlengths[ThreadId]++] = 0;
                        }
                        else
                        if (weight >= s3)
                        {
                            m[mfmlengths[ThreadId]++] = 0;
                            m[mfmlengths[ThreadId]++] = 0;
                            m[mfmlengths[ThreadId]++] = 0;
                        }
                        /*
                        int r = rnd.Next(0, 3);
                        if( r != 0)
                        {
                            m[mfmlengths[ThreadId]++] = 1;

                            for (k = 0; k <  r; k++)
                                m[mfmlengths[ThreadId]++] = 0;
                        }
                        */

                    }
                }
            }

            return m;
        }
    }
}
