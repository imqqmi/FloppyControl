using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace FloppyControlApp.MyClasses.Graphics
{
	public class Graphset
    {
        public string Filename { get; set; }
        public List<Graph2> Graphs { get; set; }
        public PictureBox Panel { get; set; }
        public bool Dragging { get; set; }
        public Color Fillcolor { set; get; }
        public StringBuilder Tbreceived { set; get; }
        private Bitmap Bmp { get; set; }
        public int Bmpxoffset { set; get; }
        public int Xrelative { set; get; }
        public int Binfilecount { get; set; }
        public int Editmode { get; set; }
        public int Editoption { get; set; }
        public int Editperiodextend { get; set; }
        public Color[] Colors { get; set; }
        private Timer RepaintDelay { get; set; } = new();
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
            RepaintDelay.Interval = 500;
            RepaintDelay.Tick += Repaintdelay_Tick;

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
            RepaintDelay.Stop();
            Allowrepaint = true;
            DoUpdateGraphs();
        }

        public void Dispose()
        {
            Bmp.Dispose();
        }

        public void Resize()
        {
            if (Panel != null
                && Panel.Height != 0
                && Panel.Width != 0)
                Bmp = new Bitmap(Panel.Width, Panel.Height);
        }

        public void AddGraph(byte[] data)
        {
            Graph2 newgraph = new()
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
                RepaintDelay.Start();
            }
        }

        public void DoUpdateGraphs()
        {
            Process proc = Process.GetCurrentProcess();
            LockBitmap lockbitmap = new(Bmp);

            lockbitmap.LockBits();

            //Debug.WriteLine("5a proc: " + string.Format("{0:n0}", proc.PrivateMemorySize64));
            //tbreceived.Append("UpdateGraphs! "+Graphs[0].changed+"\r\n");
            lockbitmap.FillBitmap(Fillcolor);
            lockbitmap.Line(Panel.Width / 2, 0, Panel.Width / 2, Panel.Height, Color.FromArgb(255, 64, 64, 64));
            lockbitmap.UnlockBits();
            //Debug.WriteLine("8 proc: " + string.Format("{0:n0}", proc.PrivateMemorySize64));

            System.Drawing.Graphics panelgraphics = Panel.CreateGraphics();
            
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(Bmp);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.PixelOffsetMode = PixelOffsetMode.None;
            g.SmoothingMode = SmoothingMode.None;

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
                Binfilecount = int.Parse(fsplit[2].Split('.')[0]);
            }
            catch
            {
                Binfilecount = 0;
            }
            string subpath = Properties.Settings.Default["PathToRecoveredDisks"].ToString();

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
            writer.Write(Graphs[0].Data.Length); // length of waveform
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
            float offsetfactor = x / (float)Graphs[0].Width;

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

                    Graphs[i].DataOffset = (int)(Graphs[i].DataOffset + Graphs[0].DataLength * offsetfactor); //center zoom
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
                    Graphs[i].DataOffset = (int)(Graphs[i].DataOffset - Graphs[i].DataLength / 2.0 * offsetfactor);
                }
            }

            //DataOffsetTrackBar.Maximum = offsetmax;
            int offset = Graphs[0].DataOffset;
            int viewdatalength = Graphs[0].DataLength;
            int datalength = Graphs[0].Data.Length;

            if (offset + viewdatalength > datalength - 1)
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
                if (Graphs.Count > 0)
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

                float factor = Graphs[0].DataLength / (float)Graphs[0].Width;

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
                        Graphs[i].DataOffset = Graphs[i].Data.Length - 1 - Graphs[i].DataLength;
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
                if (!Dragging) return;
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
                if (Graphs.Count > 0)
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
} // Namespace