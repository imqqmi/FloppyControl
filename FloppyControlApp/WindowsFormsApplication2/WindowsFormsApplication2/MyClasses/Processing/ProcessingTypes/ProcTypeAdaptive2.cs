using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloppyControlApp.MyClasses.Processing.ProcessingTypes
{
    public partial class ProcessingTypes
    {
        public byte[] ProcTypeAdaptive2(ProcTypeArgs ProctypeArgs, int ThreadId, ref int Stop)
        {
            int i;
            int value;

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

            tbreceived.Append("Period length:" + (end - start) + " ");
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

            float threshold4us;
            float threshold6us;
            float _4us = FOURUS, _6us = SIXUS, _8us = EIGHTUS;

            //tbreceived.Append("Adaptive\r\n");

            for (i = start; i<end; i++)
            {
                if (i % 250000 == 249999) { Progresses[ThreadId] = i; if (Stop == 1) break; }
                    value = (rxbuf[i] << Procsettings.hd); // If it's a HD (user selectable option), multiply data by 2

                //if (value < 5 ) continue;

                if (_4us >= _6us || _6us >= _8us) // if out of control, reset
                {
                    _4us = FOURUS;
                    _6us = SIXUS;
                    _8us = EIGHTUS;
                }

                threshold4us = (_4us + ((_6us - _4us) / 2));
                threshold6us = (_6us + ((_8us - _6us) / 2));

                if (value <= threshold4us) // 4us
                {
                    //m[mfmlengths[threadid]++]
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                    _4us = _4us + (int) ((value - _4us) / RateOfChange);

                }
                else
                if (value > threshold4us && value<threshold6us) // 6us
                {
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                    _6us = _6us + (int) ((value - _6us) / RateOfChange);
                }
                else
                if (value >= threshold6us) // 8us
                {
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                    _8us = _8us + (int) ((value - _8us) / RateOfChange);
                }
            }

            return m;
        }
    }
}
