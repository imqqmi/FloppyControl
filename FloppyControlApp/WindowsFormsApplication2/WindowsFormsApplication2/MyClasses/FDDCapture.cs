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
        public int crc { get; set; }
        public int threadid { get; set; }
        public int MarkerPositions { get; set; }
        public int rxbufMarkerPositions { get; set; }
        public SectorMapStatus mfmMarkerStatus { get; set; }
        public int track { get; set; }
        public int sector { get; set; }
        public int sectorlength { get; set; }
        public int DataIndex { get; set; }
        public byte[] sectorbytes { get; set; }
        public MarkerType MarkerType { get; set; }
        public bool processed { get; set; }
        
    }

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
            subpath = @FloppyControlApp.Properties.Settings.Default["PathToRecoveredDisks"].ToString();
            selectedBaudRate = (int)FloppyControlApp.Properties.Settings.Default["DefaultBaud"];
            selectedPortName = (string)FloppyControlApp.Properties.Settings.Default["DefaultPort"];
            StepStickMicrostepping = Decimal.ToInt32((decimal)FloppyControlApp.Properties.Settings.Default["MicroStepsPerTrack"]);
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
                try {
                    serialPort1.Open();
                }
                catch(Exception ex)
                {
                    tbr.Append("Connection failed.\r\n"+ex.Message+"\r\n");
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
                if( DirectStep == false)
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
                if((t & 1) == 0)
                {
                    serialPort1.Write('h'.ToString()); // Head 0

                }
                else
                {
                    serialPort1.Write('j'.ToString()); // Head 1
                }

                int tabs = Math.Abs(t/2);

                for (int i = 0; i < tabs; i++)
                {
                    serialPort1.Write('t'.ToString()); // previous track
                }
            }
            else
            {
                if((t & 1) == 0)
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
            if (CurrentTrack >= (int)EndTrack)
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
            CurrentTrack += ((double)MicrostepsPerTrack / (double)StepStickMicrostepping);
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
            processing.Indexrxbuf = processing.RxBbuf.Length-1;
            //processing.indexrxbuf = actualindexrxbuf;
            rxbuf = processing.RxBbuf;
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

            for (i = 0; i < processing.Indexrxbuf; i++)
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
            catch (Exception)
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
                    
                        
                        controlfloppy.GotoTrack(controlfloppy.StartTrack);
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
