using FDCPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloppyControlApp.MyClasses.Processing.ProcessingTypes
{
    public partial class ProcessingTypes
    {
        public byte[] ProcTypeAufit(ProcTypeArgs ProctypeArgs, int ThreadId, ref int Stop)
        {
            int MINUS, FOURUS, SIXUS, EIGHTUS, start, end;
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

            int bitwindow = 0;
            int currenttime = 0;
            
            DPLL dpll = new DPLL();

            for (int i = start; i < end; i++)
            {
                if (i % 250000 == 249999) { Progresses[ThreadId] = i; if (Stop == 1) break; }
                if (rxbuf[i] < 4 && Procsettings.UseErrorCorrection == false) continue;
                currenttime += (int)(((rxbuf[i] << Procsettings.hd) + FOURUS) * MINUS);

                bitwindow = dpll.bitSpacing(currenttime);

                //if (i > scatterplotstart && i < scatterplotend)
                //    tbreceived.Append("" + rxbuf[i] + "\t" + bitwindow + "\t" + currenttime +"\t"+dpll.error+ "\r\n");

                int check = mfmlengths[ThreadId];
                if (bitwindow == 2)
                {
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                }
                else
                if (bitwindow == 3)
                {
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                }
                else
                if (bitwindow == 4)
                {
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                }
                else
                {
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                }
            }

            return m;
        }
    }
}
