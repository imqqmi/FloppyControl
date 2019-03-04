// Asynchronous version, about 20KB/s
public class connectsocket
{
    private Socket client;
    public byte[] data = new byte[250100];
    private int size = 250100;
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
    private int capturedatastate = 0;
    public int capturedatablocklength { get; set; }
    public byte[] capturedata { get; set; }
    public int capturedataindex { set; get; }
    private System.Windows.Forms.Timer TimerCaptureData = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer networktimer = new System.Windows.Forms.Timer();
    public int iswaveform { get; set; }

    public connectsocket()
    {
        expectresponse = 0;
        senddone = 0;
        recvdone = 0;
        connectionStatus = 0;
        receivedlength = 0;
        packetsize = 6012;
        capturedatablocklength = 6000;
        networktimer.Interval = 10;
        TimerCaptureData.Interval = 100;
        TimerCaptureData.Tick += TimerCaptureData_Tick;
        networktimer.Tick += DoNetworkStateMachine;
        capturedata = new byte[256000];
        iswaveform = 0;
    }

    public void Connect( string ipaddress, int port)
    {
        if (connectionStatus == 0)
        {
            tbr.Append("Connecting...\r\n");
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newsock.ReceiveBufferSize = 1024*256;
            

            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(ipaddress), port);
            newsock.BeginConnect(iep, new AsyncCallback(Connected), newsock);
        }
    }

    public void Disconnect()
    {
        connectionStatus = 0;
        client.Close();
        tbr.Append("Disconnected\r\n");
    }

    // Callback after Connect()
    public void Connected(IAsyncResult iar)
    {
        client = (Socket)iar.AsyncState;
        try
        {
            connectionStatus = 1;
            client.EndConnect(iar);
            tbr.Append("Connected to: " + client.RemoteEndPoint.ToString() + "\r\n");
            client.BeginReceive(data, 0, size, SocketFlags.None, new AsyncCallback(ReceiveData2), client);
        }
        catch (SocketException)
        {
            connectionStatus = 0;
            tbr.Append("Error connecting");
        }
    }

    public void Send( string cmd)
    {
        //Thread.Sleep(100);
        receiveddatalength = 0;
        recvdone = 0;
        senddone = 0;

        if (cmd.IndexOf("?") != -1)
            expectresponse = 1;
        else
            expectresponse = 0;

        if( cmd == ":WAV:DATA?")
        {
            iswaveform = 1;
            capturedataindex = 0;
        }

        byte[] message = Encoding.ASCII.GetBytes(cmd+"\n");

        
        if ( client != null)
            client.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendData), client);
    }

    // Callback after Send()
    public void SendData(IAsyncResult iar)
    {
        senddone = 1;
        //recvdone = 1;
        Socket remote = (Socket)iar.AsyncState;
        int sent = remote.EndSend(iar);
        /*
        if (expectresponse == 1)
        {
            recvdone = 0;
            remote.BeginReceive(data, 0, size, SocketFlags.None, new AsyncCallback(ReceiveData), remote);
        }
        */
    }

    // Callback after Connected() and SendData()
    public void ReceiveData(IAsyncResult iar)
    {
        Socket remote = (Socket)iar.AsyncState;
        //try {
        receivedlength = remote.EndReceive(iar);

        tbr.Append("ReceiveData(): "+receivedlength+"\r\n");

        string stringData = Encoding.ASCII.GetString(data, 0, 13);
        tbr.Append("Recvd: " + stringData + "\r\n");

        if( stringData == "command error")
        {
            recvdone = 1;
        }

        if (receivedlength > 1000)
        {
            receiveddatalength += receivedlength;
            if (receiveddatalength < packetsize+12)
            {
                remote.BeginReceive(data, receiveddatalength, packetsize+12, SocketFlags.None, new AsyncCallback(ReceiveData), remote);
                    
            }
            else
            {
                recvdone = 1;
            }
        }
        else recvdone = 1;
        //}
        //catch(Exception e)
       // {
        //    tbr.Append("Exception: " + e.Message+"\r\n");
        //}
        
    }

    // Callback after Connected() and SendData()
    public void ReceiveData2(IAsyncResult iar)
    {
        Socket remote = (Socket)iar.AsyncState;
        receivedlength = remote.EndReceive(iar);

        remote.ReceiveBufferSize = 2000*1024;

        int qq = 0;

        if (receivedlength == 0)
        {
            qq = 1;

        }

       
        tbr.Append("ReceiveData(): " + receivedlength + " capturedataindex: "+ capturedataindex + "\r\n");
        string stringData = "";
        if (receivedlength < 50)
        {
            stringData = Encoding.ASCII.GetString(data, 0, receivedlength);
            tbr.Append("Recvd: " + stringData + "\r\n");
        }

        if( iswaveform == 1)
        {
            int i;

            for(i=0;i< receivedlength; i++)
            {
                capturedata[i + capturedataindex] = data[i];
            }
            capturedataindex += receivedlength;
            if (capturedataindex == packetsize + 12)
            {
                recvdone = 1;
                iswaveform = 0;
            }
        }
        else recvdone = 1;

        remote.BeginReceive(data, 0, 250000, SocketFlags.None, new AsyncCallback(ReceiveData2), remote);

    }

    public void DoNetworkStateMachine(object sender, EventArgs e)
    {
        //Thread.Sleep(0);
        switch (networkstate)
        {
            case 0:
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
                    connectionStatus = 0;
                }
                break;
            case 1:
                if (senddone == 1)
                {
                    tbr.Append("Send command complete\r\n");
                    if (expectresponse == 1)
                        networkstate = 2;
                    else
                    {
                        //commands.RemoveAt(0);
                        networkstate = 3;
                    }
                }
                break;
            case 2:
                //tbr.Append("-");
                if (recvdone == 1)
                {
                    recvdone = 0;
                    tbr.Append("Response received. Length: " + receivedlength + "\r\n");

                    string stringData = Encoding.ASCII.GetString(data, 0, receivedlength);
                    if (stringData != "command error")
                    {
                        //if( stringData == "0;1")
                           // commands.RemoveAt(0);
                    }

                    networkstate = 3;
                    /*
                    if (stringData.Length > 100)
                        tbr.Append(stringData.Substring(0, 50) + "\r\n");
                    else
                        tbr.Append(stringData + "\r\n");
                    */
                }
                break;
            case 3: // poll operation complete?
                tbr.Append("\r\nSend command SYST:ERR?\r\n");
                Send("SYST:ERR?");
                networkstate = 4;
                break;
            case 4:
                //tbr.Append(",");
                if (recvdone == 1)
                {
                    string stringData = Encoding.ASCII.GetString(data, 0, receivedlength);
                    string[] errorcode = stringData.Split(',');

                    int err = 0;

                    if (errorcode.Length > 1)
                    {
                        try {
                            err = Convert.ToInt32(errorcode[0]);
                        }
                        catch(Exception ex)
                        {
                            tbr.Append("DoNetworkStateMachine() error: "+ex.Message+"\r\n");
                            err = 0;
                            networkstate = 0;
                        }
                    }

                    //tbr.Append("poll: "+ stringData+"\r\n");
                    if (stringData == "command error")
                    {
                        networkstate = 3; // SYST:ERR? returned command error, retry syst:err?
                    }
                    else
                    {
                        if (data[0] == 48)
                        {
                            networkstate = 0; // command ok, next command
                            commands.RemoveAt(0);
                        }
                        else if(err < 0)
                        {
                            if (err == -410)
                            {
                                networkstate = 3; // retry SYST:err?
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
                }
                break;

            default:
                tbr.Append("Network error at state: " + networkstate + "\r\n");
                break;
        }
    }

    private void TimerCaptureData_Tick(object sender, EventArgs e)
    {
        switch (capturedatastate)
        {
            case 0: // start condition

                packetsize = capturedatablocklength;
                packetsize = 12000;
                commands.Add(":STOP");
                commands.Add(":ACQ:MDEP 12000000");

                commands.Add(":CHAN1:DISP ON");
                commands.Add(":CHAN2:DISP OFF");
                commands.Add(":CHAN3:DISP ON");
                commands.Add(":CHAN4:DISP ON");

                commands.Add(":CHAN1:COUP AC");
                commands.Add(":CHAN3:COUP AC");
                commands.Add(":CHAN4:COUP DC");

                commands.Add(":CHAN1:OFFS 0.00");
                commands.Add(":CHAN3:OFFS 1.020000e+01");
                commands.Add(":CHAN4:OFFS 1.300000e+01");

                commands.Add(":CHAN1:SCAL 1.000000e-01");
                commands.Add(":CHAN3:SCAL 5.000000e+00");
                commands.Add(":CHAN4:SCAL 5.000000e+00");

                commands.Add(":WAV:MODE RAW");
                commands.Add(":WAV:FORM BYTE");

                commands.Add(":WAV:SOUR CHAN1");
                commands.Add(":WAV:STAR 1");
                commands.Add(":WAV:STOP " + capturedatablocklength);
                packetsize = capturedatablocklength;
                commands.Add(":WAV:DATA?");

                //commands.Add(":RUN");
                tbr.Append("Capture length: " + capturedatablocklength + "\r\n");

                networktimerstart();

                capturedatastate = 1;
                break;
            case 1: // first data received
                if (connectionStatus == 0)
                {
                    int i;
                    capturedatastate = 2;
                    for (i = 0; i < capturedatablocklength; i++)
                    {
                        capturedata[i + capturedataindex++] = data[i + 12];
                    }
                    if (capturedataindex >= 24000) capturedatastate = 3;
                    tbr.Append("Data recorded in array. Length: " + capturedataindex + "\r\n");
                }
                else
                {
                    //tbr.Append("Waiting for data...\r\n");
                    tbr.Append(".");
                }
                break;
            case 2:
                connectionStatus = 1;
                capturedatastate = 1;
                commands.Add(":WAV:STAR " + (capturedataindex + 1));
                commands.Add(":WAV:STOP " + (capturedataindex + capturedatablocklength));

                commands.Add(":WAV:DATA?");
                tbr.Append("Capture" + (capturedataindex + 1) + " to " + (capturedataindex + 1 + capturedatablocklength) + "\r\n");
                networktimerstart();
                break;
            case 3: // end capture data
                TimerCaptureData.Stop();
                tbr.Append("Capture complete. Number of bytes: " + capturedataindex + "\r\n");
                Disconnect();
                break;
        }
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

// Do it synchronously.
public class connectsocketSync
{
    private Socket client;
    public byte[] data = new byte[250100];
    private int size = 250100;
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
    private int capturedatastate = 0;
    public int capturedatablocklength { get; set; }
    public byte[] capturedata { get; set; }
    public int capturedataindex { set; get; }
    private System.Windows.Forms.Timer TimerCaptureData = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer networktimer = new System.Windows.Forms.Timer();
    public int iswaveform { get; set; }

    public connectsocketSync()
    {
        expectresponse = 0;
        senddone = 0;
        recvdone = 0;
        connectionStatus = 0;
        receivedlength = 0;
        packetsize = 6012;
        capturedatablocklength = 6000;
        networktimer.Interval = 10;
        TimerCaptureData.Interval = 100;
        TimerCaptureData.Tick += TimerCaptureData_Tick;
        networktimer.Tick += DoNetworkStateMachine;
        capturedata = new byte[256000];
        iswaveform = 0;
    }

    public void Connect(string ipaddress, int port)
    {
        if (connectionStatus == 0)
        {
            tbr.Append("Connecting...\r\n");
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.ReceiveBufferSize = 250000;
            client.ReceiveTimeout = 5000;
            client.SendTimeout = 5000;

            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(ipaddress), port);
            client.Connect(iep);
            tbr.Append("Connected: " + client.Connected);
            connectionStatus = 1;
        }
    }

    public void Disconnect()
    {
        connectionStatus = 0;
        client.Close();
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

        if (cmd == ":WAV:DATA?")
        {
            iswaveform = 1;
            capturedataindex = 0;
        }

        byte[] message = Encoding.ASCII.GetBytes(cmd + "\n");


        if (client != null)
            client.Send(message);
    }

    public void Receive()
    {
        
        try
        {
            receivedlength = client.Receive(data);
           
            string stringData = Encoding.ASCII.GetString(data, 0, 13);
            tbr.Append("Recvd: " + stringData + "\r\n");
        }
        catch (Exception ex)
        {
            receivedlength = 0;
        }

    }

    // Callback after Connected() and SendData()
    public void ReceiveData2(IAsyncResult iar)
    {
        Socket remote = (Socket)iar.AsyncState;
        receivedlength = remote.EndReceive(iar);

        remote.ReceiveBufferSize = 2000 * 1024;

        int qq = 0;

        if (receivedlength == 0)
        {
            qq = 1;

        }


        tbr.Append("ReceiveData(): " + receivedlength + " capturedataindex: " + capturedataindex + "\r\n");
        string stringData = "";
        if (receivedlength < 50)
        {
            stringData = Encoding.ASCII.GetString(data, 0, receivedlength);
            tbr.Append("Recvd: " + stringData + "\r\n");
        }

        if (iswaveform == 1)
        {
            int i;

            for (i = 0; i < receivedlength; i++)
            {
                capturedata[i + capturedataindex] = data[i];
            }
            capturedataindex += receivedlength;
            if (capturedataindex == packetsize + 12)
            {
                recvdone = 1;
                iswaveform = 0;
            }
        }
        else recvdone = 1;

        remote.BeginReceive(data, 0, 250000, SocketFlags.None, new AsyncCallback(ReceiveData2), remote);

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
                    connectionStatus = 0;
                }
                break;
            case 1: // Send done
                tbr.Append("Send command complete\r\n");
                if (expectresponse == 1)
                {
                    if (iswaveform == 1)
                    {
                        while (recvdone != 1)
                        {
                            Application.DoEvents();
                            Receive();
                            tbr.Append("ReceiveData(): " + receivedlength + " capturedataindex: " + capturedataindex + "\r\n");
                            int i;

                            for (i = 0; i < receivedlength; i++)
                            {
                                capturedata[i + capturedataindex] = data[i];
                            }
                            capturedataindex += receivedlength;
                            if (capturedataindex == packetsize + 12)
                            {
                                recvdone = 1;
                                iswaveform = 0;
                            }
                        }
                    }
                    else Receive();
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
        if(connectionStatus != 0) networktimer.Start();
    }

    private void TimerCaptureData_Tick(object sender, EventArgs e)
    {
        switch (capturedatastate)
        {
            case 0: // start condition

                packetsize = capturedatablocklength;
                packetsize = 12000;
                commands.Add(":STOP");
                commands.Add(":ACQ:MDEP 12000000");

                commands.Add(":CHAN1:DISP ON");
                commands.Add(":CHAN2:DISP OFF");
                commands.Add(":CHAN3:DISP ON");
                commands.Add(":CHAN4:DISP ON");

                commands.Add(":CHAN1:COUP AC");
                commands.Add(":CHAN3:COUP AC");
                commands.Add(":CHAN4:COUP DC");

                commands.Add(":CHAN1:OFFS 0.00");
                commands.Add(":CHAN3:OFFS 1.020000e+01");
                commands.Add(":CHAN4:OFFS 1.300000e+01");

                commands.Add(":CHAN1:SCAL 1.000000e-01");
                commands.Add(":CHAN3:SCAL 5.000000e+00");
                commands.Add(":CHAN4:SCAL 5.000000e+00");

                commands.Add(":WAV:MODE RAW");
                commands.Add(":WAV:FORM BYTE");

                commands.Add(":WAV:SOUR CHAN1");
                commands.Add(":WAV:STAR 1");
                commands.Add(":WAV:STOP " + capturedatablocklength);
                packetsize = capturedatablocklength;
                commands.Add(":WAV:DATA?");

                //commands.Add(":RUN");
                tbr.Append("Capture length: " + capturedatablocklength + "\r\n");

                networktimerstart();

                capturedatastate = 1;
                break;
            case 1: // first data received
                if (connectionStatus == 0)
                {
                    int i;
                    capturedatastate = 2;
                    for (i = 0; i < capturedatablocklength; i++)
                    {
                        capturedata[i + capturedataindex++] = data[i + 12];
                    }
                    if (capturedataindex >= 24000) capturedatastate = 3;
                    tbr.Append("Data recorded in array. Length: " + capturedataindex + "\r\n");
                }
                else
                {
                    //tbr.Append("Waiting for data...\r\n");
                    tbr.Append(".");
                }
                break;
            case 2:
                connectionStatus = 1;
                capturedatastate = 1;
                commands.Add(":WAV:STAR " + (capturedataindex + 1));
                commands.Add(":WAV:STOP " + (capturedataindex + capturedatablocklength));

                commands.Add(":WAV:DATA?");
                tbr.Append("Capture" + (capturedataindex + 1) + " to " + (capturedataindex + 1 + capturedatablocklength) + "\r\n");
                networktimerstart();
                break;
            case 3: // end capture data
                TimerCaptureData.Stop();
                tbr.Append("Capture complete. Number of bytes: " + capturedataindex + "\r\n");
                Disconnect();
                break;
        }
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

public class connectsocketSyncTcpClient
{
    private TcpClient client;
    public byte[] data = new byte[250100];
    private int size = 250100;
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
    private int capturedatastate = 0;
    public int capturedatablocklength { get; set; }
    public byte[] capturedata { get; set; }
    public int capturedataindex { set; get; }
    private System.Windows.Forms.Timer TimerCaptureData = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer networktimer = new System.Windows.Forms.Timer();
    public int iswaveform { get; set; }

    public connectsocketSyncTcpClient()
    {
        expectresponse = 0;
        senddone = 0;
        recvdone = 0;
        connectionStatus = 0;
        receivedlength = 0;
        packetsize = 6012;
        capturedatablocklength = 6000;
        networktimer.Interval = 10;
        TimerCaptureData.Interval = 100;
        TimerCaptureData.Tick += TimerCaptureData_Tick;
        networktimer.Tick += DoNetworkStateMachine;
        capturedata = new byte[256000];
        iswaveform = 0;
    }

    public void Connect(string ipaddress, int port)
    {
        if (connectionStatus == 0)
        {
            tbr.Append("Connecting...\r\n");
            client = new TcpClient(ipaddress, port);
            
            //client.ReceiveBufferSize = 250000;
            //client.ReceiveTimeout = 5000;
            //client.SendTimeout = 5000;

            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(ipaddress), port);
            client.Connect(iep);
            tbr.Append("Connected: " + client.Connected);
            connectionStatus = 1;
        }
    }

    public void Disconnect()
    {
        connectionStatus = 0;
        client.Close();
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

        if (cmd == ":WAV:DATA?")
        {
            iswaveform = 1;
            capturedataindex = 0;
        }

        byte[] message = Encoding.ASCII.GetBytes(cmd + "\n");


        if (client != null)
            client.GetStream().Write(message,0,message.Length);
    }

    public void Receive()
    {

        try
        {
            receivedlength = client.GetStream().Read(data,0,size);

            string stringData = Encoding.ASCII.GetString(data, 0, 13);
            tbr.Append("Recvd: " + stringData + "\r\n");
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
                    connectionStatus = 0;
                }
                break;
            case 1: // Send done
                tbr.Append("Send command complete\r\n");
                if (expectresponse == 1)
                {
                    if (iswaveform == 1)
                    {
                        while (recvdone != 1)
                        {
                            //Application.DoEvents();
                            Receive();
                            tbr.Append("ReceiveData(): " + receivedlength + " capturedataindex: " + capturedataindex + "\r\n");
                            int i;

                            for (i = 0; i < receivedlength; i++)
                            {
                                capturedata[i + capturedataindex] = data[i];
                            }
                            capturedataindex += receivedlength;
                            if (capturedataindex == packetsize + 12)
                            {
                                recvdone = 1;
                                iswaveform = 0;
                            }
                        }
                    }
                    else Receive();
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
        if (connectionStatus != 0) networktimer.Start();
    }

    private void TimerCaptureData_Tick(object sender, EventArgs e)
    {
        switch (capturedatastate)
        {
            case 0: // start condition

                packetsize = capturedatablocklength;
                packetsize = 12000;
                commands.Add(":STOP");
                commands.Add(":ACQ:MDEP 12000000");

                commands.Add(":CHAN1:DISP ON");
                commands.Add(":CHAN2:DISP OFF");
                commands.Add(":CHAN3:DISP ON");
                commands.Add(":CHAN4:DISP ON");

                commands.Add(":CHAN1:COUP AC");
                commands.Add(":CHAN3:COUP AC");
                commands.Add(":CHAN4:COUP DC");

                commands.Add(":CHAN1:OFFS 0.00");
                commands.Add(":CHAN3:OFFS 1.020000e+01");
                commands.Add(":CHAN4:OFFS 1.300000e+01");

                commands.Add(":CHAN1:SCAL 1.000000e-01");
                commands.Add(":CHAN3:SCAL 5.000000e+00");
                commands.Add(":CHAN4:SCAL 5.000000e+00");

                commands.Add(":WAV:MODE RAW");
                commands.Add(":WAV:FORM BYTE");

                commands.Add(":WAV:SOUR CHAN1");
                commands.Add(":WAV:STAR 1");
                commands.Add(":WAV:STOP " + capturedatablocklength);
                packetsize = capturedatablocklength;
                commands.Add(":WAV:DATA?");

                //commands.Add(":RUN");
                tbr.Append("Capture length: " + capturedatablocklength + "\r\n");

                networktimerstart();

                capturedatastate = 1;
                break;
            case 1: // first data received
                if (connectionStatus == 0)
                {
                    int i;
                    capturedatastate = 2;
                    for (i = 0; i < capturedatablocklength; i++)
                    {
                        capturedata[i + capturedataindex++] = data[i + 12];
                    }
                    if (capturedataindex >= 24000) capturedatastate = 3;
                    tbr.Append("Data recorded in array. Length: " + capturedataindex + "\r\n");
                }
                else
                {
                    //tbr.Append("Waiting for data...\r\n");
                    tbr.Append(".");
                }
                break;
            case 2:
                connectionStatus = 1;
                capturedatastate = 1;
                commands.Add(":WAV:STAR " + (capturedataindex + 1));
                commands.Add(":WAV:STOP " + (capturedataindex + capturedatablocklength));

                commands.Add(":WAV:DATA?");
                tbr.Append("Capture" + (capturedataindex + 1) + " to " + (capturedataindex + 1 + capturedatablocklength) + "\r\n");
                networktimerstart();
                break;
            case 3: // end capture data
                TimerCaptureData.Stop();
                tbr.Append("Capture complete. Number of bytes: " + capturedataindex + "\r\n");
                Disconnect();
                break;
        }
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