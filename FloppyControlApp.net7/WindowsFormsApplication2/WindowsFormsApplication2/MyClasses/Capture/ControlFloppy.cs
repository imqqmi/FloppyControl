using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;

namespace FloppyControlApp.MyClasses.Capture
{
	public class ControlFloppy
    {
        public SerialPort serialPort1 = new SerialPort();
        public FDDProcessing processing { get; set; }
        public double CurrentTrack = 0;
        int trk00pos = 0;
        public List<byte[]> tempbuffer = new List<byte[]>();
        public int capturecommand = 0;
        public int tracktotrackdelay = 8;                 // delay in ms
        public int currenttrack { get; set; }
        //public int currenttrackPrintable { get; set; }

        public int StepStickMicrostepping { get; set; }         // number of microsteps of the stepstick
        public int MicrostepsPerTrack { get; set; }             // number of microsteps per track (2x microstep = 1 full track)
        public int gototrackdone { get; set; }
        public int head { get; set; }
        public int bytespersecond { get; set; }
        public int binfilecount { get; set; }
        //public int indexrxbuf { get; set; }
        public int trk00offset { get; set; }        // trk00 offset in microsteps, can be negative
        public int StartTrack { get; set; }
        public int TrackDuration { get; set; }          // track duration in ms
        public int EndTrack { get; set; }
        public int EndTrackMicrosteps { get; set; }
        public int hd { get; set; }

        public int TrackPosInrxdatacount { get; set; }
        public int recentreadbuflength { get; set; }
        public byte[] rxbuf { get; set; }
        public int[] TrackPosInrxdata { get; set; }
        public string outputfilename { get; set; }
        public string path { get; set; }
        public string subpath { get; set; }
        public StringBuilder tbr { get; set; }

        public Action ControlFloppyScatterplotCallback { get; set; }
        public Action updateHistoAndSliders { get; set; }
        public Action Setrxbufcontrol { get; set; }
        System.Diagnostics.Stopwatch SW = new System.Diagnostics.Stopwatch();

        public bool DirectStep { get; set; } // If true there's no stepstick, /STEP signal directly connected to FDD, a full track is 1x step pulse)

        int selectedBaudRate = 0;
        string selectedPortName = "";


        System.Windows.Forms.Timer timerDataCapture = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timerTrackToTrack = new System.Windows.Forms.Timer();

        public ControlFloppy()
        {
            recentreadbuflength = 0;
            gototrackdone = 0;
            //head = 0;
            bytespersecond = 0;
            TrackDuration = 260;
            trk00offset = 0;
            binfilecount = 0;
            DirectStep = false;
            MicrostepsPerTrack = 8;

            TrackPosInrxdatacount = 0;
            TrackPosInrxdata = new int[50000];
            subpath = Properties.Settings.Default["PathToRecoveredDisks"].ToString();
            selectedBaudRate = (int)Properties.Settings.Default["DefaultBaud"];
            selectedPortName = (string)Properties.Settings.Default["DefaultPort"];
            StepStickMicrostepping = decimal.ToInt32((decimal)Properties.Settings.Default["MicroStepsPerTrack"]);
            serialPort1.BaudRate = selectedBaudRate;
            serialPort1.NewLine = "\r\n";
            serialPort1.ReceivedBytesThreshold = 250000;
            serialPort1.ReadBufferSize = 1048576;
            serialPort1.DtrEnable = true;
            if (selectedPortName.Length != 0)
                serialPort1.PortName = selectedPortName;

            StartTrack = 0;
            EndTrack = 0;
            EndTrackMicrosteps = 0;
            timerDataCapture.Interval = 5;
            timerDataCapture.Tick += timerDataCapture_Tick; // Add handler
            timerTrackToTrack.Interval = 200;
            timerTrackToTrack.Tick += timerTrackToTrack_Tick; // Add handler

        }

        /// <summary>
        /// Connect to FDD
        /// </summary>
        /// <returns>1 if succesful, 0 if not succesful</returns>
        public int ConnectFDD()
        {
            if (!serialPort1.IsOpen) // Open connection if it's closed
            {
                selectedBaudRate = (int)Properties.Settings.Default["DefaultBaud"];
                selectedPortName = (string)Properties.Settings.Default["DefaultPort"];

                serialPort1.PortName = selectedPortName;
                serialPort1.BaudRate = selectedBaudRate;
                try
                {
                    serialPort1.Open();
                }
                catch (Exception ex)
                {
                    tbr.Append("Connection failed.\r\n" + ex.Message + "\r\n");
                }

                if (serialPort1.IsOpen)
                {
                    timerDataCapture.Start();
                    return 1;
                }
                else
                {
                    timerDataCapture.Stop();
                    return 0;
                }
            }
            else return 1;
        }

        public void Disconnect()
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                //timer1.Stop();
                timerDataCapture.Stop();
            }
        }

        //Capture button
        public void StartCapture()
        {
            tempbuffer.Clear();

            CurrentTrack = StartTrack;
            processing.Indexrxbuf = 0; // Reset the buffer, so we can repeatedly read from disk
            capturecommand = 1;

            if (serialPort1.IsOpen)
            {
                serialPort1.Write(']'.ToString()); // Full step
                Thread.Sleep(10);
                serialPort1.Write('.'.ToString()); // Stop capture
                Thread.Sleep(10);
                serialPort1.Write('a'.ToString()); // Select drive
                Thread.Sleep(10);
                serialPort1.Write('s'.ToString()); // Motor on
                Thread.Sleep(10);
                if (DirectStep == false)
                    serialPort1.Write('-'.ToString()); // Non inverting step signal for stepstick
                else
                    serialPort1.Write('+'.ToString()); // Inverting step signal for floppy drive

                //CurrentTrack = (int)StartTrack * MicrostepsPerTrack / StepStickMicrostepping;
                trk00pos = GotoTrack((int)CurrentTrack);
                trk00pos = 8;
                serialPort1.Write(','.ToString()); // Start capture

                capturecommand = 1;

                //Do microstepping
                if (StepStickMicrostepping > 1)
                    for (int i = 0; i < StepStickMicrostepping - 1; i++)
                    {
                        serialPort1.Write('['.ToString()); // Full step
                    }

                // Do offset
                if (trk00offset != 0)
                {
                    int offset = Math.Abs(trk00offset);
                    for (int i = 0; i < offset; i++)
                    {
                        if (trk00offset < 0)
                        {
                            serialPort1.Write('g'.ToString());
                        }
                        else
                        {
                            serialPort1.Write('t'.ToString());
                        }
                    }
                }

                timerTrackToTrack.Interval = TrackDuration;
                timerTrackToTrack.Start();

            }
        }

        /// <summary>
        /// Go to track s. Returns the trk00 position in full steps
        /// </summary>
        /// <param name="t">Represents the track</param>
        public int GotoTrack(int t)
        {
            serialPort1.Write('0'.ToString()); // TRK00
            Thread.Sleep(1500);

            if (DirectStep == true)
            {
                if ((t & 1) == 0)
                {
                    serialPort1.Write('h'.ToString()); // Head 0

                }
                else
                {
                    serialPort1.Write('j'.ToString()); // Head 1
                }

                int tabs = Math.Abs(t / 2);

                for (int i = 0; i < tabs; i++)
                {
                    serialPort1.Write('t'.ToString()); // previous track
                }
            }
            else
            {
                if ((t & 1) == 0)
                {
                    t -= 2;
                    serialPort1.Write('h'.ToString()); // Head 1
                }
                else
                {
                    t -= 5;
                    serialPort1.Write('j'.ToString()); // Head 1
                }

                int tabs = Math.Abs(t);

                for (int i = 0; i < tabs; i++)
                {
                    if (t < 0)
                        serialPort1.Write('g'.ToString()); // previous track
                    if (t > 0)
                        serialPort1.Write('t'.ToString()); // previous track
                }
            }
            //Thread.Sleep(TrackDuration);
            return t;
        }

        // Track to track handler
        private void timerTrackToTrack_Tick(object sender, EventArgs e)
        {
            if (CurrentTrack >= EndTrack)
            {
                StopAndSave();
                return;
            }

            //tbr.Append("Track " + CurrentTrack);

            if (DirectStep == true)
            {
                if (((int)CurrentTrack & 1) == 0)
                {
                    serialPort1.Write('j'.ToString()); // Head 0

                }
                else
                {
                    serialPort1.Write('h'.ToString()); // Head 1
                    serialPort1.Write('t'.ToString()); // Next track
                }
            }
            else
            {
                if (((int)CurrentTrack & 1) == 0)
                {
                    trk00pos -= 2;
                    //tbr.Append(" head 1 -2 " + trk00pos + "\r\n");
                    serialPort1.Write('j'.ToString()); // Head 0
                    for (int i = 0; i < MicrostepsPerTrack * 2; i++)
                        serialPort1.Write('g'.ToString()); // Next track
                }
                else
                {
                    trk00pos += 4;

                    //tbr.Append(" head 0 +4 " + trk00pos + "\r\n");
                    serialPort1.Write('h'.ToString()); // Head 1
                    for (int i = 0; i < MicrostepsPerTrack * 4; i++)
                        serialPort1.Write('t'.ToString()); // Next track
                }
            }
            CurrentTrack += MicrostepsPerTrack / (double)StepStickMicrostepping;
        }

        public void StopCapture()
        {
            StopAndSave();
        }

        /// <summary>
        /// Start motor without capturing read data signal 
        /// </summary>
        public void StartMotor()
        {
            serialPort1.Write('a'.ToString()); // Motor on
            Thread.Sleep(10);
            serialPort1.Write('s'.ToString()); // Motor on
            Thread.Sleep(10);
        }

        /// <summary>
        /// Stops the motor
        /// </summary>
        public void StopMotor()
        {
            serialPort1.Write('q'.ToString()); // Motor off
            Thread.Sleep(10);
            serialPort1.Write('w'.ToString()); // Motor off
            Thread.Sleep(10);
        }

        private void StopAndSave()
        {
            int i;
            timerTrackToTrack.Stop();
            capturecommand = 0;
            if (tbr == null) return;
            tbr.Append("Stopping...\r\n");

            if (!serialPort1.IsOpen) return;
            //Stop motor
            serialPort1.Write('.'.ToString());
            Thread.Sleep(10);
            serialPort1.Write('q'.ToString());
            Thread.Sleep(10);
            serialPort1.Write('w'.ToString());
            Thread.Sleep(10);

            //capturing = 0; // Indicate seconds counter to stop
            // Add double the captured buffer if user is going to use only bad sector option
            //int actualindexrxbuf = processing.indexrxbuf;
            if (processing.Indexrxbuf <= 0) return;

            byte[] extra = new byte[processing.Indexrxbuf];
            tempbuffer.Add(extra);
            processing.RxBbuf = tempbuffer.SelectMany(a => a).ToArray();
            processing.Indexrxbuf = processing.RxBbuf.Length - 1;
            //processing.indexrxbuf = actualindexrxbuf;
            rxbuf = processing.RxBbuf;
            // Write period data to disk in bin format

            if (Setrxbufcontrol == null) return;
            Setrxbufcontrol();

            path = subpath + @"\" + outputfilename + @"\";

            string fullpath = path + outputfilename +
                "_T" + StartTrack.ToString("D3") + "_T" + EndTrack.ToString("D3")
                + "_" + binfilecount.ToString("D3") + ".bin";

            Directory.CreateDirectory(path);

            while (File.Exists(fullpath))
            {
                binfilecount++;
                fullpath = path + outputfilename +
                "_T" + StartTrack.ToString("D3") + "_T" + EndTrack.ToString("D3")
                + "_" + binfilecount.ToString("D3") + ".bin";
            }

            // Write with counter so no overwrite is performed
            BinaryWriter writer;
            writer = new BinaryWriter(new FileStream(fullpath, FileMode.Create));

            for (i = 0; i < processing.Indexrxbuf; i++)
                writer.Write(rxbuf[i]);

            if (writer != null)
            {
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
            if (ControlFloppyScatterplotCallback != null)
                ControlFloppyScatterplotCallback();
            updateHistoAndSliders();
            Setrxbufcontrol();
            capturecommand = 0;
            Disconnect();
        }

        // Capture handler
        private void timerDataCapture_Tick(object sender, EventArgs e)
        {
            // Read data from serial port, poll every 100ms
            // If data is there, read a block and write it to disk.
            int bytestoread = 0;
            //byte buf;

            timerDataCapture.Stop();
            try
            {
                bytestoread = serialPort1.BytesToRead;
            }

            catch (InvalidOperationException ex)
            {
                //textBoxReceived.Text += "Serial connection lost. Exception type:" + ex.ToString();
                tbr.Append("Serial connection lost. Exception type:" + ex.ToString());
                if ((uint)ex.HResult == 0x80131509)
                {
                    //LabelStatus.Text = "Disconnected.";
                    //ConnectBtn.Text = "Connect";
                    //ConnectBtn.BackColor = Color.FromArgb(0xF0, 0xF0, 0xF0);
                    //timer1.Stop();
                    timerDataCapture.Stop();
                }
            }

            if (serialPort1.IsOpen)
            {
                if (bytestoread != 0)
                {
                    bytespersecond += bytestoread;

                    byte[] temp = new byte[bytestoread];

                    if (serialPort1.IsOpen)
                        serialPort1.Read(temp, 0, bytestoread);

                    tempbuffer.Add(temp);

                    //tbr.Append("Store captured block in list time: " + SW.Elapsed);
                    //processing.indexrxbuf += bytestoread;


                    //update the scatterplot, this may have a performance hit

                    if (Setrxbufcontrol == null) return;
                    Setrxbufcontrol();

                }
                timerDataCapture.Start();
            }
        }
    }
}
