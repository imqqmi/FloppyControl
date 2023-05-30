using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace FloppyControlApp.MyClasses.Graphics
{
	/// <summary>
    /// Generates a histogram, value vs frequency.
    /// </summary>
    public class Histogram
    {
        private byte[] histogram = new byte[256];
        private Panel panel;
        private float scaling;
        public int HD { get; set; }
        public byte[] Data { get; set; }
        public long Offset { get; set; }
        public long Length { get; set; }
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
        public void DoHistogram(byte[] d, long offset1, long length1)
        {
            Data = d;
            Offset = offset1;
            Length = length1;
            DoHistogram();
        }

        public void DoHistogram()
        {
            long i;

            if (Data == null) return;
            if (Offset + Length > Data.Length) return;

            int[] histogramint = new int[256];

            int histogrammax;

            if (Length == 0) Length = Data.Length;

            if (!(Data.Length >= Offset + Length)) return;

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

            scaling = (float)100 / histogrammax;
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
            formGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            formGraphics.CompositingMode = CompositingMode.SourceCopy;
            formGraphics.PixelOffsetMode = PixelOffsetMode.None;
            formGraphics.SmoothingMode = SmoothingMode.None;

            using (var bmp = new Bitmap(580, 102, PixelFormat.Format32bppPArgb))
            {
                LockBitmap lockBitmap = new(bmp);
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
} // Namespace