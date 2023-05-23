using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FloppyControlApp.MyClasses.Graphics
{
	/// <summary>
    /// Fast bitmap manipulation and double buffering for flicker free operations.
    /// </summary>
    public class LockBitmap
    {
        Bitmap source = null;
        nint Iptr = nint.Zero;
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
                Rectangle rect = new(0, 0, Width, Height);

                // get source bitmap pixel format size
                Depth = Image.GetPixelFormatSize(source.PixelFormat);

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
                Console.WriteLine(ex.ToString());
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
                Console.WriteLine(ex.ToString());
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
            int i = (y * Width + x) * cCount;

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
            int i = (y * Width + x) * cCount;

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
} // Namespace