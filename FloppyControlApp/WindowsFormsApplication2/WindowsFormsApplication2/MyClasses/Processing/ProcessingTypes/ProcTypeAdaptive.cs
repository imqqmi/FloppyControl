using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloppyControlApp.MyClasses.Processing.ProcessingTypes
{
    public partial class ProcessingTypes
    {
        public byte[] ProcTypeAdaptive(ProcTypeArgs ProctypeArgs, int ThreadId, ref int Stop)
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

            int basethreshold4us = ((SIXUS - FOURUS) / 2);
            int basethreshold6us = ((EIGHTUS - SIXUS) / 2);

            //int q;
            int lookahead = 10;
            //int d4cnt = 0, d6cnt = 0, d8cnt = 0;
            float averagetime = 0;
            int val2;

            
            threshold4us = FOURUS + basethreshold4us;
            threshold6us = SIXUS + basethreshold6us;
            float _4usavg = FOURUS, _6usavg = SIXUS, _8usavg = EIGHTUS;
            float fourus = FOURUS, sixus = SIXUS, eightus = EIGHTUS;
            Random rnd = new Random();
            int rand = 0;
            int sectorboundary = 0;

            float[] lowpass4;
            float[] lowpass6;
            float[] lowpass8;

            int lowpassradius = (int)Procsettings.rateofchange2;
            try
            {
                lowpass4 = new float[lowpassradius];
                lowpass6 = new float[lowpassradius];
                lowpass8 = new float[lowpassradius];
            }
            catch (OutOfMemoryException)
            {
                tbreceived.Append("Oops, we ran out of memory. Try restarting FloppyControlApp or use a smaller dataset.\r\n");
                GC.Collect();
                return null;
            }

            for (i = 0; i < lowpassradius; i++)
            {
                lowpass4[i] = fourus;
                lowpass6[i] = sixus;
                lowpass8[i] = eightus;
            }
            _4usavg = lowpassradius * fourus;
            _6usavg = lowpassradius * sixus;
            _8usavg = lowpassradius * eightus;

            float val4 = 0, val6 = 0, val8 = 0;

            for (i = start; i < end - lookahead; i++)
            {
                if (i % 250000 == 249999) { Progresses[ThreadId] = i; if (Stop == 1) break; }
                sectorboundary = (i - start + 50) % 9000;
                if (Procsettings.AddNoise == true && sectorboundary >= Procsettings.addnoiselimitstart && sectorboundary <= Procsettings.addnoiselimitend)
                    rand = rnd.Next(-1, 1) * Procsettings.addnoiserndamount;
                //    rand = rnd.Next(0 - procsettings.addnoiserndamount, procsettings.addnoiserndamount);
                else
                    rand = 0;

                value = (rxbuf[i] << Procsettings.hd) + rand; // If it's a HD (user selectable option), multiply data by 2

                val2 = value;
                value -= (int)(averagetime / RateOfChange);// + procsettings.AdaptOffset;
                                                           //if (procsettings.UseErrorCorrection == false)
                if (val2 < 4) continue;

                //rxbuf[i] = (byte)value;
                //if (i > scatterplotstart && i < scatterplotend)
                //    tbreceived.Append("" + value + "\t" + threshold4us.ToString("0.00")+"\t"+threshold6us.ToString("0.00")+"\r\n");
                if (value <= threshold4us) // 4us
                {

                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                    _4us = fourus - val2;
                    if (Procsettings.rateofchange2 != 0)
                    {
                        val4 = lowpass4[(i + 1) % lowpassradius];
                        _4usavg -= val4;
                        lowpass4[(i + 1) % lowpassradius] = val2;
                        _4usavg += val2;
                        fourus = _4usavg / lowpassradius;
                        //_4usavg = _4usavg + ((val2 - fourus) / procsettings.rateofchange2);

                    }
                    averagetime = _4us;
                }
                else
                if (value > threshold4us && value < threshold6us) // 6us
                {
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                    _6us = sixus - val2;
                    if (Procsettings.rateofchange2 != 0)
                    {
                        val6 = lowpass6[(i + 1) % lowpassradius];
                        _6usavg -= val6;
                        lowpass6[(i + 1) % lowpassradius] = val2;
                        _6usavg += val2;
                        sixus = _6usavg / lowpassradius;
                    }

                    averagetime = _6us;
                }
                else
                if (value >= threshold6us) // 8us
                {
                    m[mfmlengths[ThreadId]++] = 1;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                    m[mfmlengths[ThreadId]++] = 0;
                    _8us = eightus - val2;
                    if (Procsettings.rateofchange2 != 0)
                    {
                        val8 = lowpass8[(i + 1) % lowpassradius];
                        _8usavg -= val8;
                        lowpass8[(i + 1) % lowpassradius] = val2;
                        _8usavg += val2;
                        eightus = _8usavg / lowpassradius;
                    }

                    averagetime = _8us;
                }
            }
            //entropy = null;
            //threshold4 = null;
            //threshold6 = null;
            //threshold8 = null;
            lowpass4 = null;
            lowpass6 = null;
            lowpass8 = null;
            GC.Collect();

            return m;
        }
    }
}
