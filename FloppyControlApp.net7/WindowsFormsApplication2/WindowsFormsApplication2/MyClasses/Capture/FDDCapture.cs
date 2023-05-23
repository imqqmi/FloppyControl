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

namespace FloppyControlApp.MyClasses.Capture
{

	public class ScopeCapture
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

        public ScopeCapture()
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
                        tbr.Append("\r\nSend command " + commands.ElementAt(0) + "\r\n");
                        string cmd = commands.ElementAt(0);
                        if (cmd.Length > 5)
                        {
                            if (cmd.Substring(0, 5) == "&WAIT")
                            {
                                string a = commands.ElementAt(0);
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
                                Send(commands.ElementAt(0));
                                networkstate = 1;
                            }
                        }
                        else
                        {
                            Send(commands.ElementAt(0));
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

                string subpath = Properties.Settings.Default["PathToRecoveredDisks"].ToString();

                string path = subpath + @"\" + outputfilename + @"\";

                string fullpath = path + outputfilename +
                    "_T" + StartTrack.ToString("D3")
                    + "_" + binfilecount.ToString("D3") + ".wvfrm";

                Directory.CreateDirectory(path);

                while (File.Exists(fullpath))
                {
                    binfilecount++;
                    fullpath = path + outputfilename +
                    "_T" + StartTrack.ToString("D3") +
                    "_" + binfilecount.ToString("D3") + ".wvfrm";
                }
                tbr.Append("Saving file " + fullpath + "\r\n");
                // Write with counter so no overwrite is performed
                BinaryWriter writer;
                writer = new BinaryWriter(new FileStream(fullpath, FileMode.Create));
                writer.Write((byte)3); // number of waveforms
                writer.Write(ScopeMemDepth); // length of waveform
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
