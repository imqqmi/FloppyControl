using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;

using NationalInstruments.VisaNS;
using FloppyControlApp.MyClasses;

namespace FloppyControlApp
{
    public struct UndoData
    {
        public int offset { get; set; }
        public byte[] undodata { get; set; }
    }
    

    public class MFMData
    {
        private int pMarkerPositions, prxbufMarkerPositions, pmfmMarkerStatus, ptrack, psector;
        private int pthreadid, pcrc, psectorlength;

        public int crc { get { return pcrc; } set { pcrc = value; } }
        public int threadid { get { return pthreadid; } set { pthreadid = value; } }
        public int MarkerPositions { get { return pMarkerPositions; } set { pMarkerPositions = value; } }
        public int rxbufMarkerPositions { get { return prxbufMarkerPositions; } set { prxbufMarkerPositions = value; } }
        public SectorMapStatus mfmMarkerStatus { get; set; }
        public int track { get { return ptrack; } set { ptrack = value; } }
        public int sector { get { return psector; } set { psector = value; } }
        public int sectorlength { get { return psectorlength; } set { psectorlength = value; } }
        public int DataIndex { get; set; }
        public byte[] sectorbytes { get; set; }
        public MarkerType MarkerType { get; set; }
        public bool processed { get; set; }
        
    }

    public class ControlFloppy
    {
        public SerialPort serialPort1 = new SerialPort();
        public FDDProcessing processing { get; set; }
        //int capturetime = 0;
        //int capturing = 0;
        int CaptureTracks = 0;
        public List<byte[]> tempbuffer = new List<byte[]>();
        int headselect = 0;
        public int capturecommand = 0;
        int GotoTrack = 0;
        public int tracktotrackdelay = 8;                 // delay in ms
        public int currenttrack { get; set; }
        public int currenttrackPrintable { get; set; }
        int stepspertrack = 2;                                  // Number of full steps per track. If DirectStep == true, stepspertrack = 1 else = 2
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

        private bool directstep;
        public bool DirectStep
        {
            get { return directstep; }
            set
            {
                directstep = value;
                if (value == true) stepspertrack = 1;
                else stepspertrack = 2;
            }
        } // If true there's no stepstick, /STEP signal directly connected to FDD, a full track is 1x step pulse)

        int selectedBaudRate = 0;
        string selectedPortName = "";


        System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer3 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer4 = new System.Windows.Forms.Timer();

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
            subpath = @FloppyControlApp.Properties.Settings.Default["PathToRecoveredDisks"].ToString();
            selectedBaudRate = (int)FloppyControlApp.Properties.Settings.Default["DefaultBaud"];
            selectedPortName = (string)FloppyControlApp.Properties.Settings.Default["DefaultPort"];
            StepStickMicrostepping = (int)FloppyControlApp.Properties.Settings.Default["MicroStepping"];
            serialPort1.BaudRate = selectedBaudRate;
            serialPort1.NewLine = "\r\n";
            serialPort1.ReceivedBytesThreshold = 500000;
            serialPort1.ReadBufferSize = 1048576;
            serialPort1.DtrEnable = true;
            if (selectedPortName.Length != 0)
                serialPort1.PortName = selectedPortName;

            StartTrack = 0;
            EndTrack = 0;
            EndTrackMicrosteps = 0;
            timer2.Interval = 10;
            timer2.Tick += timer2_Tick; // Add handler
            timer3.Interval = 200;
            timer3.Tick += timer3_Tick; // Add handler
            timer4.Interval = 100;
            timer4.Tick += timer4_Tick; // Add handler

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
                try {
                    serialPort1.Open();
                }
                catch(Exception ex)
                {
                    tbr.Append("Connection failed.\r\n"+ex.Message+"\r\n");
                }

                if (serialPort1.IsOpen)
                {
                    timer2.Start();
                    return 1;
                }
                else
                {
                    timer2.Stop();
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
                timer2.Stop();
            }
        }

        //Capture button
        public void StartCapture()
        {
            int i;

            //capturetime = 0;
            //capturing = 1;

            //microstep = (int)Properties.Settings.Default["MicroStepping"];

            tempbuffer.Clear();

            for (i = 0; i < TrackPosInrxdatacount; i++)
            {
                TrackPosInrxdata[i] = 0;
            }

            TrackPosInrxdatacount = 0;

            CaptureTracks = 0;
            headselect = 0;
            //Start motor and capture data from serial port
            processing.indexrxbuf = 0; // Reset the buffer, so we can repeatedly read from disk
            capturecommand = 1;

            // 2.5 sec the drive will spin up, after that the track should take 0.2 secs at most to
            // read 11+1 sectors

            if (EndTrack - StartTrack > 0)
            {
                if ((StartTrack & 1) == 1)
                {

                    EndTrack += 4;
                }
                else
                {
                    if (StartTrack > 1)
                    {
                        if (MicrostepsPerTrack == 1)
                        {
                            StartTrack -= 2;
                            EndTrack += 2;
                        }
                        if (MicrostepsPerTrack == 8)
                        {
                            StartTrack -= 4;
                            //EndTrack += 2;
                        }
                    }
                }
            }
            if (serialPort1.IsOpen)
            {
                gototrack(StartTrack);
            }
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
            timer3.Stop();
            capturecommand = 0;
            if (tbr == null) return;
            tbr.Append("Stopping...\r\n");
            

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
            byte[] extra = new byte[processing.indexrxbuf]; 
            tempbuffer.Add(extra);
            processing.rxbuf = tempbuffer.SelectMany(a => a).ToArray();
            processing.indexrxbuf = processing.rxbuf.Length-1;
            //processing.indexrxbuf = actualindexrxbuf;
            rxbuf = processing.rxbuf;
            // Write period data to disk in bin format

            if (Setrxbufcontrol == null) return;
            Setrxbufcontrol();

            path = subpath + @"\" + outputfilename + @"\";

            string fullpath = path + outputfilename +
                "_T" + (StartTrack).ToString("D3") + "_T" + (EndTrack).ToString("D3")
                + "_" + binfilecount.ToString("D3") + ".bin";

            Directory.CreateDirectory(path);

            while (File.Exists(fullpath))
            {
                binfilecount++;
                fullpath = path + outputfilename +
                "_T" + (StartTrack).ToString("D3") + "_T" + (EndTrack).ToString("D3")
                + "_" + binfilecount.ToString("D3") + ".bin";
            }

            // Write with counter so no overwrite is performed
            BinaryWriter writer;
            writer = new BinaryWriter(new FileStream(fullpath, FileMode.Create));

            for (i = 0; i < processing.indexrxbuf; i++)
                writer.Write(rxbuf[i]);

            if (writer != null)
            {
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
            if(ControlFloppyScatterplotCallback!=null)
                ControlFloppyScatterplotCallback();
            updateHistoAndSliders();
            Setrxbufcontrol();
            capturecommand = 0;
        }

        /// <summary>
        /// Go to track s
        /// </summary>
        /// <param name="t">Represents the track</param>
        public void gototrack(int t)
        {
            //int i;
            decimal temp;

            gototrackdone = 0;

            
            // Select head
            if ((t & 1) == 0)
            {
                //head = 0;
                headselect = 1;
                tbr.Append("head h\r\n");
                serialPort1.Write('h'.ToString()); // Select head 0
                Thread.Sleep(10);
            }
            else
            {
                //head = 1;
                headselect = 0;
                tbr.Append("head j\r\n");
                serialPort1.Write('j'.ToString()); // Select head 1
                Thread.Sleep(10);
            }

            GotoTrack = (t * StepStickMicrostepping) / 2 - ((t & 1) * StepStickMicrostepping) + StepStickMicrostepping;
            tbr.Append("Gototrack:" + GotoTrack + "\r\n");

            temp = (((int)EndTrack - t) * StepStickMicrostepping);//(int)StepsPerTrackUpDown.Value;
                                                     //temp *= ((decimal)microstep / StepsPerTrackUpDown.Value);
            EndTrackMicrosteps = (int)temp;

            TrackPosInrxdata[TrackPosInrxdatacount++] = processing.indexrxbuf; // Make a list of all track start positions
            //rxbuf[processing.indexrxbuf++] = 0x02;//Track marker
            //rxbuf[processing.indexrxbuf++] = (byte)((GotoTrack / StepStickMicrostepping) + (CaptureTracks / StepStickMicrostepping));

            byte[] trackmarker = new byte[2];
            trackmarker[0] = 0x02;
            trackmarker[1] = (byte)((GotoTrack / StepStickMicrostepping) + (CaptureTracks / StepStickMicrostepping)); 
            tempbuffer.Add(trackmarker);


            tbr.Append("Gototrack:" + GotoTrack + " EndTrackMicrosteps:" + EndTrackMicrosteps + "\r\n");

            currenttrack = 0;
            currenttrackPrintable = t;
            // Set microstep
            serialPort1.Write(']'.ToString()); // Full step
            Thread.Sleep(10);
            serialPort1.Write('.'.ToString()); // Stop capture
            Thread.Sleep(10);
            serialPort1.Write('a'.ToString()); // Motor on
            Thread.Sleep(10);
            serialPort1.Write('s'.ToString()); // Motor on

            serialPort1.Write('0'.ToString()); // TRK00
            Thread.Sleep(1500);                // Wait for the head to home, if you don't, the PIC will lock up!
            
            serialPort1.Write('g'.ToString()); // TRK00
            Thread.Sleep(50);
            serialPort1.Write('g'.ToString()); // TRK00
            Thread.Sleep(50);
            serialPort1.Write('g'.ToString()); // TRK00
            Thread.Sleep(50);
            
            if( t == 0)
            {
                serialPort1.Write('g'.ToString()); // TRK00
                Thread.Sleep(50);
                serialPort1.Write('g'.ToString()); // TRK00
                Thread.Sleep(50);
            }
            
            
            timer4.Interval = 5;
            timer4.Start();
        }

        // Handler for initial track control, skipping to the start track
        // Once done, the timer stops and starts timer3 for capture.
        private void timer4_Tick(object sender, EventArgs e)
        {
            int i;
            timer4.Stop();
            
            if (DirectStep == false)
            {
                
                if (currenttrack < GotoTrack)
                {
                    tbr.Append("Gototrack:" + GotoTrack + " currenttrack:" + currenttrack + " microstep: " + StepStickMicrostepping + "\r\n");
                    serialPort1.Write('t'.ToString()); // increase track number
                    Thread.Sleep(tracktotrackdelay);
                    serialPort1.Write('t'.ToString()); // increase track number
                    Thread.Sleep(tracktotrackdelay);
                    currenttrack += StepStickMicrostepping;
                    timer4.Start();
                }
                else
                {
                    timer4.Stop();

                    // Workaround to get the data in both scope and rdata capture correctly
                    //serialPort1.Write('t'.ToString()); // increase track number
                    //Thread.Sleep(tracktotrackdelay);

                    if (capturecommand == 1)
                    {

                        //serialPort1.Write(']'.ToString()); // Full step
                        //Thread.Sleep(10);
                        // Set microstep
                        if (StepStickMicrostepping > 1)
                            for (i = 0; i < StepStickMicrostepping-1; i++)
                            {
                                serialPort1.Write('['.ToString()); // Full step
                                Thread.Sleep(10);
                            }
                        
                        // Do microstep offset
                        if( StepStickMicrostepping == 8)
                            trk00offset -= 12;
                        //if (StepStickMicrostepping == 1)
                        //    trk00offset += 1;

                        if (DirectStep == false)
                        {
                            tbr.Append("Moving to offset... \r\n");
                            
                            for (i = 0; i < Math.Abs(trk00offset); i++)
                            {
                                tbr.Append("step"+i+"\r\n");
                                if (trk00offset < 0)
                                    serialPort1.Write('g'.ToString()); // decrement track by one
                                else serialPort1.Write('t'.ToString()); // increment track by one
                                Thread.Sleep(tracktotrackdelay);
                            }
                        }
                        if( TrackDuration < 1000)
                            timer3.Interval = 1000;
                        else timer3.Interval = TrackDuration;
                        
                        serialPort1.Write(','.ToString());// start capture
                        timer3.Start();
                    }
                }
            }
            else
            {
                tbr.Append("Fullstep direct based next track...\r\n");
                if (currenttrack < (GotoTrack / 2))
                {
                    serialPort1.Write('t'.ToString()); // increase track number
                    currenttrack += StepStickMicrostepping;
                    timer4.Start();
                }
                else
                {
                    timer4.Stop();
                    gototrackdone = 1;
                    if (capturecommand == 1)
                    {
                        if (TrackDuration < 1000)
                            timer3.Interval = 1000;
                        else timer3.Interval = TrackDuration;

                        serialPort1.Write(','.ToString());// start capture
                        tbr.Append("delay 1000\r\n");
                        timer3.Start();
                    }
                }
            }
        }

        // Capture handler
        private void timer2_Tick(object sender, EventArgs e)
        {
            // Read data from serial port, poll every 100ms
            // If data is there, read a block and write it to disk.
            int bytestoread = 0;
            //byte buf;

            timer2.Stop();
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
                    timer2.Stop();
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
                    
                    tbr.Append("Store captured block in list time: " + SW.Elapsed);
                    //processing.indexrxbuf += bytestoread;
                    

                    //update the scatterplot, this may have a performance hit
                    
                    if (Setrxbufcontrol == null) return;
                    Setrxbufcontrol();

                }
                timer2.Start();
            }
        }

        // Track to track handler
        private void timer3_Tick(object sender, EventArgs e)
        {
            int i;

            //serialPort1.Write('.'.ToString()); // Stop capture
            Thread.Sleep(10);
            timer3.Stop();
            tbr.Append("CaptureTracks" + CaptureTracks+"\r\n");
            if (CaptureTracks < EndTrackMicrosteps)
            {
                CaptureTracks += MicrostepsPerTrack;

                headselect++;
                //tbr.Append("Headselect:"+headselect+"\r\n");
                if ((headselect & 1) == 0)
                {
                    tbr.Append("Head j\r\n");
                    serialPort1.Write('j'.ToString()); //head 0
                    Thread.Sleep(tracktotrackdelay / MicrostepsPerTrack * 2);
                }
                else
                {
                    //tbr.Append("MicrostepsPerTrack* stepspertrack" + (MicrostepsPerTrack * stepspertrack) + "\r\n");
                    //tbr.Append("Head h\r\n");
                    serialPort1.Write('h'.ToString()); //head 1
                    
                    
                }
                currenttrackPrintable = (CaptureTracks/ StepStickMicrostepping) + StartTrack;
                currenttrack = ((GotoTrack / StepStickMicrostepping) + (CaptureTracks / StepStickMicrostepping));
                tbr.Append("currenttrack: "+currenttrack+" Currenttrackprintable: "+currenttrackPrintable+"\r\n");
                if ((currenttrack & 1) == 1 && CaptureTracks > 2)
                {
                    tbr.Append("Next track\r\n");
                    for (i = 0; i < MicrostepsPerTrack * stepspertrack; i++)
                    //for (i = 0; i < MicrostepsPerTrack; i++)
                    {
                        serialPort1.Write('t'.ToString()); //next track
                        if( MicrostepsPerTrack > 1)
                            Thread.Sleep(tracktotrackdelay/2);
                        else Thread.Sleep(tracktotrackdelay );
                    }
                }
                
                TrackPosInrxdata[TrackPosInrxdatacount++] = processing.indexrxbuf; // Make a list of all track start positions
                //rxbuf[processing.indexrxbuf++] = 0x02;//Track marker
                //rxbuf[processing.indexrxbuf++] = (byte)((GotoTrack / StepStickMicrostepping) + (CaptureTracks / StepStickMicrostepping));

                byte[] trackmarker = new byte[2];
                trackmarker[0] = 0x02;
                trackmarker[1] = (byte)((GotoTrack / StepStickMicrostepping) + (CaptureTracks / StepStickMicrostepping));
                tempbuffer.Add(trackmarker);

                //ControlFloppyScatterplotCallback();

                //Wait for the head to settle Edit: the histogram function acts as a wait loop :)
                tbr.Append("delay "+TrackDuration+"\r\n");
                timer3.Interval = TrackDuration;
                timer3.Start();
                //serialPort1.Write(','.ToString()); // resume capture
                Thread.Sleep(10);
                currenttrack = ((GotoTrack / StepStickMicrostepping) + (CaptureTracks / StepStickMicrostepping));
            }
            else
            {
                StopAndSave();
            }
        }
    }



    public class connectsocketNIVisa2
    {
        private MessageBasedSession client;
        public byte[] data = new byte[250100];
        //private int size = 250100;
        public int stop { get; set; }

        public int receivedlength { get; set; }
        public int connectionStatus { get; set; } // 0 = disconnected, 1 = connected
        public int senddone { get; set; }
        public int recvdone { get; set; }
        public StringBuilder tbr { get; set; }
        public List<string> commands = new List<string>();
        public int expectresponse { get; set; }
        public int receiveddatalength = 0;
        public int packetsize { get; set; }
        private int networkstate = 0;
        public int capturedatastate { get; set; }
        public int capturedatablocklength { get; set; }
        public byte[][] capturedata { get; set; }
        public int capturedataindex { set; get; }
        private System.Windows.Forms.Timer TimerCaptureData = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer networktimer = new System.Windows.Forms.Timer();
        public int iswaveform { get; set; }
        public int commandlistdone { get; set; }
        private string lastResourceString = "TCPIP0::192.168.1.138::INSTR";
        private int capturedataoffset = 0;
        public int ScopeMemDepth { get; set; }
        public bool SaveFinished { get; set; }
        public bool UseAveraging { get; set; }
        public bool NoControlFloppy { get; set; }
        public SerialPort serialPort1 { get; set; }
        public ControlFloppy controlfloppy { get; set; }
        public int xscalemv { get; set; }

        public connectsocketNIVisa2()
        {
            NoControlFloppy = false;
            serialPort1 = new SerialPort();
            stop = 0;
            UseAveraging = false;
            capturedatastate = 0;
            commandlistdone = 1;
            expectresponse = 0;
            senddone = 0;
            recvdone = 0;
            connectionStatus = 0;
            receivedlength = 0;
            packetsize = 6012;
            capturedatablocklength = 250000;
            networktimer.Interval = 10;
            TimerCaptureData.Interval = 100;
            TimerCaptureData.Tick += TimerCaptureData_Tick;
            networktimer.Tick += DoNetworkStateMachine;
            capturedata = new byte[4][];
            iswaveform = 0;
            ScopeMemDepth = 500000;
        }

        public void Connect(string connection)
        {
            string scopeconnection = "";
            if (connectionStatus == 0)
            {

                using (SelectResource sr = new SelectResource())
                {
                    DialogResult result;
                    if (lastResourceString != null)
                    {
                        sr.ResourceName = lastResourceString;
                    }
                    if (connection == "")
                    {
                        result = sr.ShowDialog();
                        scopeconnection = sr.ResourceName;
                        if (result == DialogResult.OK)
                        {
                            lastResourceString = sr.ResourceName;
                            Properties.Settings.Default["ScopeConnection"] = lastResourceString;
                            //Cursor.Current = Cursors.WaitCursor;
                            try
                            {
                                client = (MessageBasedSession)ResourceManager.GetLocalManager().Open(scopeconnection);
                                //SetupControlState(true);
                            }
                            catch (InvalidCastException)
                            {
                                MessageBox.Show("Resource selected must be a message-based session");
                            }
                            catch (Exception exp)
                            {
                                MessageBox.Show(exp.Message);
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else // We've already got a string
                    {
                        scopeconnection = connection;
                        //Cursor.Current = Cursors.WaitCursor;
                        try
                        {
                            client = (MessageBasedSession)ResourceManager.GetLocalManager().Open(scopeconnection);
                            //SetupControlState(true);
                        }
                        catch (InvalidCastException)
                        {
                            Properties.Settings.Default["ScopeConnection"] = "";
                            MessageBox.Show("Resource selected must be a message-based session");
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message);
                        }
                    }
                    if (client != null)
                    {

                        if (tbr != null)
                            tbr.Append("Connected: " + client.LastStatus + "\r\n");
                        connectionStatus = 1;
                    }
                }
            }
            
        }

        public void Disconnect()
        {
            connectionStatus = 0;
            //SetupControlState(false);
            if (client != null)
                client.Dispose();
            if (tbr != null)
                tbr.Append("Disconnected\r\n");
        }

        public void Send(string cmd)
        {
            //Thread.Sleep(100);
            receiveddatalength = 0;
            recvdone = 0;
            senddone = 0;

            if (cmd.IndexOf("?") != -1)
                expectresponse = 1;
            else
                expectresponse = 0;

            if (cmd == ":WAV:STAR 1")
            {
                capturedataoffset = 0;
            }

            if (cmd == ":WAV:DATA?")
            {
                iswaveform = 1;
                capturedataindex = 0;
            }

            byte[] message = Encoding.ASCII.GetBytes(cmd + "\n");
            tbr.Append("Send: " + cmd + "\r\n");

            if (client != null)
            {
                try
                {
                    client.Write(message);
                }
                catch (Exception exp)
                {
                    networktimer.Stop();
                    MessageBox.Show(exp.Message);
                }
            }
        }

        public void SendNFB(string cmd)
        {
            //Thread.Sleep(100);
            receiveddatalength = 0;
            recvdone = 0;
            senddone = 0;

            if (cmd.IndexOf("?") != -1)
                expectresponse = 1;
            else
                expectresponse = 0;

            if (cmd == ":WAV:STAR 1")
            {
                capturedataoffset = 0;
            }

            if (cmd == ":WAV:DATA?")
            {
                iswaveform = 1;
                capturedataindex = 0;
            }

            byte[] message = Encoding.ASCII.GetBytes(cmd + "\n");
            //tbr.Append("Send: " + cmd + "\r\n");

            if (client != null)
            {
                try
                {
                    client.Write(message);
                }
                catch (Exception exp)
                {
                    networktimer.Stop();
                    MessageBox.Show(exp.Message);
                }
            }
        }

        public void Receive()
        {
            try
            {
                data = client.ReadByteArray();
                receivedlength = data.Length;
                string stringData;

                if (receivedlength > 50)
                    stringData = Encoding.ASCII.GetString(data, 0, 50);
                else
                    stringData = Encoding.ASCII.GetString(data, 0, receivedlength);
                tbr.Append("Recvd: " + stringData + "\r\n");
            }
            catch (Exception ex)
            {
                tbr.Append("Error:" + ex.Message);
                receivedlength = 0;
            }

        }
        // No feedback to speed up data transfer
        public void ReceiveNoFB()
        {
            try
            {
                data = client.ReadByteArray();
                receivedlength = data.Length;
            }
            catch (Exception ex)
            {
                receivedlength = 0;
            }

        }

        public void DoNetworkStateMachine(object sender, EventArgs e)
        {
            networktimer.Stop();
            //Thread.Sleep(0);
            switch (networkstate)
            {
                case 0: // Send command
                    if (commands.Count > 0)
                    {
                        tbr.Append("\r\nSend command " + commands.ElementAt<string>(0) + "\r\n");
                        string cmd = commands.ElementAt<string>(0);
                        if (cmd.Length > 5)
                        {
                            if (cmd.Substring(0, 5) == "&WAIT")
                            {
                                string a = commands.ElementAt<string>(0);
                                string[] mysplit = a.Split(' ');
                                if (mysplit.Length > 1)
                                {
                                    networktimerstop();
                                    int delay = Convert.ToInt32(mysplit[1]);

                                    Thread.Sleep(delay);
                                    networktimerstart();
                                }
                                commands.RemoveAt(0);
                                networkstate = 0;
                            }
                            else
                            {
                                Send(commands.ElementAt<string>(0));
                                networkstate = 1;
                            }
                        }
                        else
                        {
                            Send(commands.ElementAt<string>(0));
                            networkstate = 1;
                        }
                    }
                    else
                    {
                        tbr.Append("network timer stopped.\r\n");
                        networktimer.Stop();
                        Thread.Sleep(100);
                        commandlistdone = 1;
                    }
                    break;
                case 1: // Send done
                    Application.DoEvents();
                    tbr.Append("Send command complete\r\n");
                    if (expectresponse == 1)
                    {
                        Receive();
                        tbr.Append("Response received. Length: " + receivedlength + "\r\n");

                        tbr.Append("\r\nSend command SYST:ERR?\r\n");
                        Send("SYST:ERR?");
                        networkstate = 4;
                    }
                    else
                    {
                        //commands.RemoveAt(0);
                        tbr.Append("\r\nSend command SYST:ERR?\r\n");
                        Send("SYST:ERR?");
                        networkstate = 4;
                    }

                    break;
                case 4: // SYST:ERR? response
                    Application.DoEvents();
                    //tbr.Append(",");
                    Receive();

                    string stringData = Encoding.ASCII.GetString(data, 0, receivedlength);
                    string[] errorcode = stringData.Split(',');

                    int err = 0;

                    if (errorcode.Length > 1)
                    {
                        try
                        {
                            err = Convert.ToInt32(errorcode[0]);
                        }
                        catch (Exception ex)
                        {
                            tbr.Append("DoNetworkStateMachine() error: " + ex.Message + "\r\n");
                            err = 0;
                            networkstate = 0;
                        }
                    }

                    //tbr.Append("poll: "+ stringData+"\r\n");
                    if (stringData == "command error")
                    {
                        tbr.Append("\r\nSend command SYST:ERR?\r\n");
                        Send("SYST:ERR?");
                        networkstate = 4; // SYST:ERR? returned command error, retry syst:err?
                    }
                    else
                    {
                        if (data[0] == 48)
                        {
                            networkstate = 0; // command ok, next command
                            commands.RemoveAt(0);
                        }
                        else if (err < 0)
                        {
                            if (err == -410)
                            {
                                tbr.Append("\r\nSend command SYST:ERR?\r\n");
                                Send("SYST:ERR?");
                                networkstate = 4;
                            }
                            else
                            {
                                networktimerstop();
                                capturetimerstop();
                                tbr.Append("Stopping. Error: " + err);
                                networkstate = 0; // command not ok, resend command
                            }
                        }
                    }

                    break;

                default:
                    tbr.Append("Network error at state: " + networkstate + "\r\n");
                    break;
            }
            if (commandlistdone == 0) networktimer.Start();
        }

        private void TimerCaptureData_Tick(object sender, EventArgs e)
        {
            Application.DoEvents();
            TimerCaptureData.Stop();
            switch (capturedatastate)
            {
                case 0: // start condition

                    if (NoControlFloppy == false)
                    {
                        if (controlfloppy.ConnectFDD() == 0)
                        {
                            tbr.Append("Could not connect to floppy controller.\r\n");
                            return;
                        }
                    
                        
                        controlfloppy.gototrack(controlfloppy.StartTrack);
                    }
                    controlfloppy.StartMotor();
                    if (NoControlFloppy == false)
                    {
                        int cnt = 0;
                        // Wait for floppy drive to be ready at the chosen track
                        while (cnt < 200)
                        {
                            Application.DoEvents();
                            if (controlfloppy.gototrackdone == 1)
                                cnt = 201;
                            Thread.Sleep(15);
                            cnt++;
                        }
                    }

                    Send("*CLS"); if (RecvError() != 0) break; // Clear all errors

                    Send(":RUN"); if (RecvError() != 0) break;

                    Send(":TIM:MAIN:SCAL 0.02"); if (RecvError() != 0) break;

                    if (UseAveraging)
                    {
                        Send(":ACQ:TYPE AVER"); if (RecvError() != 0) break;
                        Send(":ACQ:AVER 256"); if (RecvError() != 0) break;
                    }
                    else
                        Send(":ACQ:TYPE NORM"); if (RecvError() != 0) break;
                    Send(":CHAN1:DISP ON"); if (RecvError() != 0) break;
                    Send(":CHAN2:DISP ON"); if (RecvError() != 0) break;
                    Send(":CHAN3:DISP ON"); if (RecvError() != 0) break;
                    Send(":CHAN4:DISP ON"); if (RecvError() != 0) break;
                    Send(":ACQ:MDEP 6000000"); if (RecvError() != 0) break;

                    Send(":CHAN1:COUP AC"); if (RecvError() != 0) break;
                    Send(":CHAN2:COUP AC"); if (RecvError() != 0) break;
                    Send(":CHAN3:COUP AC"); if (RecvError() != 0) break;
                    Send(":CHAN4:COUP DC"); if (RecvError() != 0) break;

                    Send(":CHAN1:OFFS 0.00"); if (RecvError() != 0) break;
                    Send(":CHAN2:OFFS 0.00"); if (RecvError() != 0) break;
                    Send(":CHAN3:OFFS 1.020000e+01"); if (RecvError() != 0) break;
                    Send(":CHAN4:OFFS 1.300000e+01"); if (RecvError() != 0) break;

                    Send(":CHAN1:SCAL " + xscalemv + ".0000000e-03"); if (RecvError() != 0) break;
                    Send(":CHAN2:SCAL " + xscalemv + ".0000000e-03"); if (RecvError() != 0) break;
                    Send(":CHAN3:SCAL 5.000000e+00"); if (RecvError() != 0) break;
                    Send(":CHAN4:SCAL 5.000000e+00"); if (RecvError() != 0) break;

                    if (UseAveraging)
                        for (int i = 0; i < 300; i++)
                        {
                            Application.DoEvents();
                            Thread.Sleep(100);
                        }
                    else
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            Application.DoEvents();
                            Thread.Sleep(100);
                        }
                    }
                    Send(":STOP"); if (RecvError() != 0) break;

                    controlfloppy.StopMotor();
                    if (NoControlFloppy == false)
                    {
                        
                        controlfloppy.Disconnect();
                    }

                    

                    controlfloppy.TrackPosInrxdatacount = 0;

                    Send(":WAV:MODE RAW"); if (RecvError() != 0) break;
                    Send(":WAV:FORM BYTE"); if (RecvError() != 0) break;

                    Send(":WAV:STAR 1"); if (RecvError() != 0) break;
                    Send(":WAV:STOP 250000"); if (RecvError() != 0) break;
                    packetsize = 250000;

                    //Send(":WAV:DATA?");

                    //Send(":RUN");
                    capturedataindex = 0;
                    commandlistdone = 1;

                    capturedatastate = 1;
                    break;
                case 1: // first data received
                    Application.DoEvents();
                    if (commandlistdone == 1)
                    {
                        // Channel1 contains the analogue signal from the floppy drive head
                        // Channel3 contains the read data signal
                        // Channel4 contains the index signal (optional)
                        tbr.Append("Capturing Channel 1 (Analogue head signal)\r\n...");
                        Application.DoEvents();
                        capturedata[0] = new byte[ScopeMemDepth];
                        while (capturedataoffset < ScopeMemDepth)
                        {
                            SendNFB(":WAV:SOUR CHAN1"); if (RecvError() != 0) break;
                            SendNFB(":WAV:STAR " + (capturedataoffset + 1).ToString()); if (RecvError() != 0) break;
                            SendNFB(":WAV:STOP " + (capturedataoffset + packetsize).ToString()); if (RecvError() != 0) break;
                            SendNFB("WAV:DATA?");

                            recvdone = 0;
                            capturedataindex = 0;

                            while (recvdone != 1)
                            {
                                ReceiveNoFB();
                                int i;
                                int headeroffset = 0;
                                if (capturedataindex == 0) headeroffset = 12; // skip header
                                for (i = 0; i < receivedlength - headeroffset; i++)
                                {
                                    capturedata[0][i + capturedataindex + capturedataoffset] = data[i + headeroffset];
                                }
                                capturedataindex += receivedlength - headeroffset;
                                if (capturedataindex >= packetsize)
                                {
                                    capturedataoffset += packetsize;
                                    tbr.Append("Channel 1, Data: " + capturedataoffset + "\r\n");
                                    Application.DoEvents();
                                    recvdone = 1;
                                    iswaveform = 0;
                                }
                            }
                            if (RecvError() != 0) break;
                            //tbr.Append("Data recorded in array. Length: " + capturedataindex + "\r\n");
                        }

                        tbr.Append("Capturing Channel 2 (Read data signal)\r\n...");
                        Application.DoEvents();
                        capturedataindex = 0;
                        capturedata[1] = new byte[ScopeMemDepth];
                        capturedata[2] = new byte[ScopeMemDepth];
                        capturedataoffset = 0;
                        while (capturedataoffset < ScopeMemDepth)
                        {
                            SendNFB(":WAV:SOUR CHAN2"); if (RecvError() != 0) break;
                            SendNFB(":WAV:STAR " + (capturedataoffset + 1).ToString()); if (RecvError() != 0) break;
                            SendNFB(":WAV:STOP " + (capturedataoffset + packetsize).ToString()); if (RecvError() != 0) break;
                            SendNFB("WAV:DATA?");

                            recvdone = 0;
                            capturedataindex = 0;

                            while (recvdone != 1)
                            {
                                ReceiveNoFB();
                                int i;
                                int headeroffset = 0;
                                if (capturedataindex == 0) headeroffset = 12; // skip header
                                for (i = 0; i < receivedlength - headeroffset; i++)
                                {
                                    capturedata[1][i + capturedataindex + capturedataoffset] = data[i + headeroffset];
                                }
                                capturedataindex += receivedlength - headeroffset;
                                if (capturedataindex >= packetsize)
                                {

                                    capturedataoffset += packetsize;
                                    tbr.Append("Channel 3, Data: " + capturedataoffset + "\r\n");
                                    Application.DoEvents();
                                    recvdone = 1;
                                    iswaveform = 0;
                                }
                            }
                            if (RecvError() != 0) break;
                            //tbr.Append("Data recorded in array. Length: " + capturedataindex + "\r\n");
                        }

                        tbr.Append("Capturing Channel 3 (Read data signal)\r\n...");
                        Application.DoEvents();
                        capturedataindex = 0;
                        capturedata[2] = new byte[ScopeMemDepth];
                        capturedataoffset = 0;
                        while (capturedataoffset < ScopeMemDepth)
                        {
                            SendNFB(":WAV:SOUR CHAN3"); if (RecvError() != 0) break;
                            SendNFB(":WAV:STAR " + (capturedataoffset + 1).ToString()); if (RecvError() != 0) break;
                            SendNFB(":WAV:STOP " + (capturedataoffset + packetsize).ToString()); if (RecvError() != 0) break;
                            SendNFB("WAV:DATA?");

                            recvdone = 0;
                            capturedataindex = 0;

                            while (recvdone != 1)
                            {
                                ReceiveNoFB();
                                int i;
                                int headeroffset = 0;
                                if (capturedataindex == 0) headeroffset = 12; // skip header
                                for (i = 0; i < receivedlength - headeroffset; i++)
                                {
                                    capturedata[2][i + capturedataindex + capturedataoffset] = data[i + headeroffset];
                                }
                                capturedataindex += receivedlength - headeroffset;
                                if (capturedataindex >= packetsize)
                                {

                                    capturedataoffset += packetsize;
                                    tbr.Append("Channel 3, Data: " + capturedataoffset + "\r\n");
                                    Application.DoEvents();
                                    recvdone = 1;
                                    iswaveform = 0;
                                }
                            }
                            if (RecvError() != 0) break;
                            //tbr.Append("Data recorded in array. Length: " + capturedataindex + "\r\n");
                        }

                        // Save data to disk

                        capturedatastate = 3;
                    }
                    else
                    {
                        tbr.Append(".");
                    }
                    break;
                case 3: // end capture data
                    break;
            }
            if (capturedatastate != 3)
                TimerCaptureData.Start();
            else if (stop != 1)
            {
                TimerCaptureData.Stop();
                tbr.Append("Capture complete. Number of bytes: " + capturedataindex + "\r\n");

                int i;
                int StartTrack = controlfloppy.StartTrack;
                int EndTrack = controlfloppy.EndTrack;
                int binfilecount = controlfloppy.binfilecount;
                string outputfilename = controlfloppy.outputfilename;

                string subpath = @FloppyControlApp.Properties.Settings.Default["PathToRecoveredDisks"].ToString();

                string path = subpath + @"\" + outputfilename + @"\";

                string fullpath = path + outputfilename +
                    "_T" + (StartTrack).ToString("D3")
                    + "_" + binfilecount.ToString("D3") + ".wvfrm";

                Directory.CreateDirectory(path);

                while (File.Exists(fullpath))
                {
                    binfilecount++;
                    fullpath = path + outputfilename +
                    "_T" + (StartTrack).ToString("D3") +
                    "_" + binfilecount.ToString("D3") + ".wvfrm";
                }
                tbr.Append("Saving file " + fullpath + "\r\n");
                // Write with counter so no overwrite is performed
                BinaryWriter writer;
                writer = new BinaryWriter(new FileStream(fullpath, FileMode.Create));
                writer.Write((byte)3); // number of waveforms
                writer.Write((int)ScopeMemDepth); // length of waveform
                for (i = 0; i < capturedata[0].Length; i++)
                    writer.Write(capturedata[0][i]);
                for (i = 0; i < capturedata[1].Length; i++)
                    writer.Write(capturedata[1][i]);
                for (i = 0; i < capturedata[2].Length; i++)
                    writer.Write(capturedata[2][i]);

                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }



                if (StartTrack == EndTrack)
                    Disconnect();
                SaveFinished = true;
            }
            if (stop == 1)
            {
                TimerCaptureData.Stop();
                controlfloppy.StopCapture();
            }
        }

        public int RecvError()
        {
            int cnt = 0;
            Send("SYST:ERR?");
            Receive();
            //tbr.Append("Receivelength: "+receivedlength+"\r\n");
            Application.DoEvents();
            while (receivedlength == 0 && cnt < 200)
            {
                tbr.Append("+");
                Application.DoEvents();
                Send("SYST:ERR?");
                Receive();
                cnt++;
                Thread.Sleep(10);
                if (stop == 1) return 2;
            }

            string stringData = Encoding.ASCII.GetString(data, 0, receivedlength);
            string[] errorcode = stringData.Split(',');



            int err = 0;

            if (errorcode.Length > 1)
            {
                try
                {
                    err = Convert.ToInt32(errorcode[0]);
                }
                catch (Exception ex)
                {
                    tbr.Append("DoNetworkStateMachine() error: " + ex.Message + "\r\n");
                    err = 0;
                    networkstate = 0;
                }
            }

            //tbr.Append("poll: "+ stringData+"\r\n");
            if (stringData == "command error")
            {
                stop = 1;
                capturedatastate = 3;
                networktimerstop();
                capturetimerstop();
                tbr.Append("Stopping. Error: " + err + " error msg: " + stringData + "\r\n");
                return 1; //Command error
            }
            else
            {
                if (data[0] == 48)
                {
                    return 0; // All good
                              //networkstate = 0; // command ok, next command
                              //commands.RemoveAt(0);
                }
                else if (err < 0)
                {
                    stop = 1;
                    capturedatastate = 3;
                    networktimerstop();
                    capturetimerstop();
                    tbr.Append("Stopping. Error: " + err + " error msg: " + stringData + "\r\n");

                    return err; // Other error
                }
            }
            stop = 1;
            capturedatastate = 3;
            networktimerstop();
            capturetimerstop();
            tbr.Append("Stopping. Error: " + err + " error msg: " + stringData + " Length: " + receivedlength + "\r\n");
            return 2; // Error, no conditions were met
        }

        public void networktimerstart()
        {
            networktimer.Start();
        }

        public void networktimerstop()
        {
            networktimer.Stop();
        }

        public void capturetimerstart()
        {
            TimerCaptureData.Start();
        }

        public void capturetimerstop()
        {
            TimerCaptureData.Stop();
        }

    }
}
