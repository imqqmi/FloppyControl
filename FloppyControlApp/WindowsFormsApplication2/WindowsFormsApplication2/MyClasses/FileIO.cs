using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloppyControlApp.MyClasses
{
    class TrackSectorOffset
    {
        public int offsetstart { get; set; }
        public int offsetend { get; set; }
        public int length { get; set; }
    }

    public class FileIO
    {
        public bool AddData { get; set; }
        public string[] OpenFilesPaths { get; set; }
        public string BaseFileName { get; set; }
        public string PathToRecoveredDisks { get; set; }
        public FDDProcessing processing { get; set; }
        public TextBox textBoxFilesLoaded { get; set; }
        public RichTextBox rtbSectorMap { get; set; }
        public StringBuilder tbreceived { get; set; }

        public Action FilesAvailableCallback { get; set; }
        public Action resetinput { get; set; }
        public Action ScpGuiUpdateCallback { get; set; }

        private BinaryReader reader;
        private BinaryWriter writer;
        private OpenFileDialog openFileDialog1;
        private OpenFileDialog openFileDialog2;
        private int binfilecount = 0; // Keep saving each capture under a different filename as to keep all captured data


        public FileIO()
        {
            BaseFileName = (string)Properties.Settings.Default["BaseFileName"];
            PathToRecoveredDisks = (string)Properties.Settings.Default["PathToRecoveredDisks"];
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "Bin files (*.bin)|*.bin|Kryoflux (*.raw)|*.raw|All files (*.*)|*.*";
            openFileDialog1.FileOk += openFileDialog1_FileOk;
            openFileDialog1.AddExtension = true;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.ValidateNames = true;

            openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Filter = "Projects (*.prj)|*.prj|All files (*.*)|*.*";
            openFileDialog2.Multiselect = false;
            openFileDialog2.FileOk += openFileDialog2_FileOk;
            openFileDialog2.AddExtension = true;
            openFileDialog2.CheckFileExists = true;
            openFileDialog2.CheckPathExists = true;
            openFileDialog2.ValidateNames = true;
            AddData = true;
        }

        public void ShowOpenBinFiles()
        {
            AddData = false;
            openFileDialog1.InitialDirectory = PathToRecoveredDisks + @"\" + BaseFileName;
            openFileDialog1.Title = "Select files to open...";
            openFileDialog1.ShowDialog();
        }

        public void ShowAddBinFiles()
        {
            openFileDialog1.InitialDirectory = PathToRecoveredDisks + @"\" + BaseFileName;
            AddData = true;
            openFileDialog1.Title = "Select files to add...";
            openFileDialog1.ShowDialog();
        }

        public void ShowLoadProjectFiles()
        {
            openFileDialog2.InitialDirectory = PathToRecoveredDisks + @"\" + BaseFileName;
            openFileDialog1.Title = "Select project to load...";
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            OpenFilesPaths = openFileDialog1.FileNames;
            FilesAvailableCallback();
        }

        public void SetBaseFileNameFromPath(string path)
        {
            if (path != null)
            {
                string filename = Path.GetFileName(path);
                BaseFileName = filename.Substring(0, filename.IndexOf("_"));
                Properties.Settings.Default["BaseFileName"] = BaseFileName;
                Properties.Settings.Default.Save();
            }
        }

        public void openfiles()
        {
            int numberOfFiles, loaderror = 0;
            //byte a;
            byte[] temp = new byte[1];
            byte[] histogram = new byte[256];
            string path1 = @"";

            List<byte[]> tempbuf = new List<byte[]>();

            numberOfFiles = OpenFilesPaths.Length;

            if (AddData == false) // If open is clicked, replace the current data
            {
                resetinput();
                //TrackPosInrxdatacount = 0;
                processing.indexrxbuf = 0;

                // Clear all sectordata
                // if ( processing.sectordata2 != null)
                //     for (i = 0; i < processing.sectordata2.Count; i++)
                //
                //MFMData sectordata;
                if (processing.sectordata2 != null)
                {
                    processing.sectordata2.Clear();
                    //sectordata2.TryTake(out sectordata);
                }
            }

            //New!!!
            textBoxFilesLoaded.Text += "\r\n"; // Add a white line to indicate what groups of files are loaded
            processing.CurrentFiles = "";
            // Write period data to disk in bin format
            if (AddData == true)  // If Add data is clicked, the data is appended to the rxbuf array
            {
                tempbuf.Add(processing.rxbuf.SubArray(0, processing.indexrxbuf));
            }
            String ext;
            foreach (String file in OpenFilesPaths)
            {
                try
                {
                    reader = new BinaryReader(new FileStream(file, FileMode.Open));

                    path1 = Path.GetFileName(file);
                    ext = Path.GetExtension(file);
                    textBoxFilesLoaded.Text += path1 + "\r\n";
                    processing.CurrentFiles += path1 + "\r\n";
                    var indexof = path1.IndexOf("_");
                    if (indexof != -1)
                    {
                        BaseFileName = path1.Substring(0, path1.IndexOf("_"));
                        Properties.Settings.Default["BaseFileName"] = BaseFileName;
                        Properties.Settings.Default.Save();

                    }

                    //reader.BaseStream.Length
                    //tbSectorMap.Text += openFileDialog1.FileName + "\r\n";
                    //tbSectorMap.Text += Path.GetFileName(openFileDialog1.FileName) + "\r\n";
                    //tbSectorMap.Text += "FileLength: " + reader.BaseStream.Length + "\r\n";

                    if (ext == ".raw")
                    {
                        //filter kryoflux meta data
                        var tempdat = reader.ReadBytes((int)reader.BaseStream.Length);
                        byte[] tempdat2 = new byte[tempdat.Length];

                        int cnt = 0;

                        for (int i = 0; i < tempdat.Length - 4; i++)
                        {
                            if (tempdat[i] == 0x0d && tempdat[i + 1] == 0x02 && tempdat[i + 2] == 0x0c && tempdat[i + 3] == 0x00)
                            {
                                i += 16;
                            }
                            tempdat2[cnt++] = tempdat[i];
                        }

                        tempbuf.Add(tempdat2);
                    }
                    else tempbuf.Add(reader.ReadBytes((int)reader.BaseStream.Length));
                }
                catch (SecurityException ex)
                {
                    // The user lacks appropriate permissions to read files, discover paths, etc.
                    MessageBox.Show("Security error. Please contact your administrator for details.\n\n" +
                        "Error message: " + ex.Message + "\n\n" +
                        "Details (send to Support):\n\n" + ex.StackTrace
                    );
                    loaderror = 1;
                }
                catch (Exception ex)
                {
                    // Could not load the image - probably related to Windows file system permissions.
                    MessageBox.Show("Cannot display the image: " + file.Substring(file.LastIndexOf('\\'))
                        + ". You may not have permission to read the file, or " +
                        "it may be corrupt.\n\nReported error: " + ex.Message);
                    loaderror = 1;
                }
                if (loaderror != 1)
                {
                    //temp.CopyTo(processing.rxbuf, processing.indexrxbuf);
                    processing.indexrxbuf += (int)reader.BaseStream.Length;
                }
                if (reader != null)
                {
                    //reader.Flush();
                    reader.Close();
                    reader.Dispose();
                }

                //Application.DoEvents();
            }

            if (processing.rxbuf.Length < 100000)
            {
                byte[] extra = new byte[100000];
                tempbuf.Add(extra);
            }

            processing.rxbuf = tempbuf.SelectMany(a => a).ToArray();
        }

        public void SaveDiskImage()
        {
            int i, j, bytecount = 0;
            string extension = ".ADF";
            string diskformatstring = "";
            int disksize1 = 0;
            int ioerror = 0;

            //string fullpath = "";
            // Write period data to disk in bin format

            string path = PathToRecoveredDisks + @"\" + BaseFileName + @"\";

            if (processing.diskformat == DiskFormat.amigados)
            {
                bytecount = processing.bytespersector * processing.sectorspertrack * 160;
                extension = ".ADF";
                diskformatstring = "ADOS";
            }
            else if (processing.diskformat == DiskFormat.diskspare)
            {
                bytecount = processing.bytespersector * processing.sectorspertrack * 160;
                extension = ".ADF";
                diskformatstring = "DiskSpare";
            }
            else if (processing.diskformat == DiskFormat.pcdd) // DD
            {
                bytecount = processing.bytespersector * processing.sectorspertrack * 80 * 2;
                extension = ".DSK";
                diskformatstring = "PCDOS DD";
            }
            else if (processing.diskformat == DiskFormat.pchd) // HD
            {
                bytecount = processing.bytespersector * processing.sectorspertrack * 80 * 2;
                extension = ".DSK";
                diskformatstring = "PCDOS HD";
            }
            else if (processing.diskformat == DiskFormat.pc2m) // 2M
            {
                bytecount = 512 * 18 + (1024 * 11 * 83 * 2); // First track is normal PC HD format
                extension = ".DSK";
                diskformatstring = "PCDOS 2M";
            }
            else if (processing.diskformat == DiskFormat.pc360kb525in) // 360KB 5.25"
            {
                bytecount = processing.bytespersector * processing.sectorspertrack * 40 * 2;
                extension = ".DSK";
                diskformatstring = "PCDOS 360KB";
            }

            bool exists = System.IO.Directory.Exists(path);

            if (!exists)
                System.IO.Directory.CreateDirectory(path);

            //Only save if there's any data to save
            if (bytecount != 0)
            {
                path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_sectormap_" + binfilecount.ToString("D3") + ".txt";
                while (File.Exists(path))
                {
                    binfilecount++;
                    path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_sectormap_" + binfilecount.ToString("D3") + ".txt";
                }

                rtbSectorMap.AppendText("Disk Format: " + diskformatstring + "\r\nRecovered sectors: " + processing.sectormap.recoveredsectorcount + "\r\n");
                rtbSectorMap.AppendText("Recovered sectors with error: " + processing.sectormap.RecoveredSectorWithErrorsCount + "\r\n");

                //fullpath = path + outputfilename + "_trackinfo.txt";
                try
                {
                    File.WriteAllText(path, rtbSectorMap.Text);
                }
                catch (IOException ex)
                {
                    tbreceived.Append("IO error writing sector map: \r\n" + ex.ToString());
                }

                if (processing.diskformat == DiskFormat.diskspare) // 960 KB to 984 KB diskspare
                {


                    for (j = 80; j <= 83; j++) // Multiple DiskSpare images, not sure what format I used, so try it in WinUAE
                    {
                        bytecount = j * processing.bytespersector * 12 * 2; //Also save with the two extra tracks 984KB
                        disksize1 = bytecount / 1024;
                        path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_" + disksize1 + "KB" + "_disk_" + binfilecount.ToString("D3") + extension;
                        while (File.Exists(path))
                        {
                            binfilecount++;
                            path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_" + disksize1 + "KB" + "_disk_" + binfilecount.ToString("D3") + extension;
                        }

                        tbreceived.Append(path);
                        try
                        {
                            writer = new BinaryWriter(new FileStream(path, FileMode.Create));
                        }
                        catch (IOException ex)
                        {
                            tbreceived.Append("IO error writing DiskSpare image: \r\n" + ex.ToString());
                            ioerror = 1;
                        }
                        if (ioerror == 0) // skip writing on io error
                        {
                            for (i = 0; i < bytecount; i++) // writing uints
                                writer.Write((byte)processing.disk[i]);
                            if (writer != null)
                            {
                                writer.Flush();
                                writer.Close();
                                writer.Dispose();
                            }
                        }
                        else ioerror = 0;
                    }
                }

                if (processing.diskformat == DiskFormat.amigados) // ADOS 880 KB
                {
                    path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_disk_" + binfilecount.ToString("D3") + extension;
                    while (File.Exists(path))
                    {
                        binfilecount++;
                        path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_disk_" + binfilecount.ToString("D3") + extension;
                    }

                    bytecount = 80 * processing.bytespersector * 11 * 2; //Also save with the two extra tracks 984KB
                    disksize1 = bytecount / 1024;
                    try
                    {
                        writer = new BinaryWriter(new FileStream(path, FileMode.Create));
                    }
                    catch (IOException ex)
                    {
                        tbreceived.Append("IO error writing ADOS image: \r\n" + ex.ToString());
                        ioerror = 1;
                    }
                    if (ioerror == 0) // skip writing on io error
                    {
                        for (i = 0; i < bytecount; i++) // writing uints
                            writer.Write((byte)processing.disk[i]);
                        if (writer != null)
                        {
                            writer.Flush();
                            writer.Close();
                            writer.Dispose();
                        }
                    }
                    else ioerror = 0;
                }

                if (processing.diskformat == DiskFormat.pcdd ||
                    processing.diskformat == DiskFormat.pchd ||
                    processing.diskformat == DiskFormat.pc2m ||
                    processing.diskformat == DiskFormat.pc360kb525in) //PC 720 KB dd or 1440KB hd
                {
                    int SectorsPerDisk;
                    SectorsPerDisk = (processing.disk[20] << 8) | processing.disk[19];
                    tbreceived.Append("Sectors per disk: " + SectorsPerDisk + "\r\n");

                    if (SectorsPerDisk == 720 && processing.diskformat == DiskFormat.pcssdd)
                    {
                        path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_disk_" + binfilecount.ToString("D3") + extension;
                        while (File.Exists(path))
                        {
                            binfilecount++;
                            path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_disk_" + binfilecount.ToString("D3") + extension;
                        }

                        try
                        {
                            writer = new BinaryWriter(new FileStream(path, FileMode.Create));
                        }
                        catch (IOException ex)
                        {
                            tbreceived.Append("IO error: " + ex.ToString());
                            ioerror = 1;
                        }
                        if (ioerror == 0) // skip writing on io error
                        {
                            int interleave = 0;
                            for (i = 0; i < bytecount; i++)
                            {

                                writer.Write((byte)processing.disk[i]);
                                interleave++;
                                if (interleave == 512 * 9)
                                {
                                    interleave = 0;
                                    i += 512 * 9;
                                }
                            }
                            if (writer != null)
                            {
                                writer.Flush();
                                writer.Close();
                                writer.Dispose();
                            }
                        }
                        else ioerror = 0;
                    }
                    else
                    {
                        path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_disk_" + binfilecount.ToString("D3") + extension;
                        while (File.Exists(path))
                        {
                            binfilecount++;
                            path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_disk_" + binfilecount.ToString("D3") + extension;
                        }

                        try
                        {
                            writer = new BinaryWriter(new FileStream(path, FileMode.Create));
                        }
                        catch (IOException ex)
                        {
                            tbreceived.Append("IO error: " + ex.ToString());
                            ioerror = 1;
                        }
                        if (ioerror == 0) // skip writing on io error
                        {
                            for (i = 0; i < bytecount; i++)
                                writer.Write((byte)processing.disk[i]);
                            if (writer != null)
                            {
                                writer.Flush();
                                writer.Close();
                                writer.Dispose();
                            }
                        }
                        else ioerror = 0;
                    }
                }

            }
        }

        public void SaveProject()
        {
            int i, j, sectorcount = 0;
            string extension = ".ADF";
            int ioerror = 0;

            // Write period data to disk in bin format
            extension = ".prj";
            string path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_" + binfilecount.ToString("D3") + extension;

            tbreceived.Append("Path:" + path + "\r\n");

            bool exists = System.IO.Directory.Exists(path);

            if (processing.diskformat == DiskFormat.amigados)
            {
                sectorcount = 512 * processing.sectorspertrack * 164; // a couple of tracks extra to store possible other data
            }
            else if (processing.diskformat == DiskFormat.diskspare)
            {
                sectorcount = 1024 * 1024; // disk sizes can vary
            }
            else if (processing.diskformat == DiskFormat.pcdd)
            {
                sectorcount = 512 * processing.sectorspertrack * 82 * 2;
            }
            else if (processing.diskformat == DiskFormat.pchd)
            {
                sectorcount = 512 * processing.sectorspertrack * 82 * 2;
            }
            else if (processing.diskformat == DiskFormat.pc2m)
            {
                sectorcount = 2048 * 1024;
            }

            while (File.Exists(path))
            {
                binfilecount++;
                path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_" + binfilecount.ToString("D3") + extension;
            }

            //Only save if there's any data to save
            if (sectorcount != 0)
            {
                //if (processing.diskformat == 3 || processing.diskformat == 4) //PC 720 KB dd or 1440KB hd
                //{
                try
                {
                    writer = new BinaryWriter(new FileStream(path, FileMode.Create));
                }
                catch (IOException ex)
                {
                    tbreceived.Append("IO error: " + ex.ToString());
                    ioerror = 1;
                }
                if (ioerror == 0) // skip writing on io error
                {
                    writer.Write((byte)processing.diskformat);
                    writer.Write((int)sectorcount);
                    //Save sector data
                    for (i = 0; i < sectorcount; i++)
                        writer.Write((byte)processing.disk[i]);

                    //Save sector status
                    for (i = 0; i < 184; i++)
                        for (j = 0; j < 18; j++)
                            writer.Write((byte)processing.sectormap.sectorok[i, j]);

                    if (writer != null)
                    {
                        writer.Flush();
                        writer.Close();
                        writer.Dispose();
                    }
                }
                else ioerror = 0;
                //}
            }
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            int i, j, sectorcount = 0;
            string path1;
            int ioerror = 0;

            tbreceived.Append("Loading project...\r\n");

            // Write period data to disk in bin format

            string path = openFileDialog2.FileName;
            path1 = Path.GetFileName(path);
            BaseFileName = path1.Substring(0, path1.IndexOf("_"));
            Properties.Settings.Default["BaseFileName"] = BaseFileName;
            Properties.Settings.Default.Save();
            tbreceived.Append("Path: " + path + "\r\n");

            bool exists = System.IO.Directory.Exists(path);

            /*
            if (processing.diskformat == DiskFormat.amigados) // AmigaDOS
            {
                sectorcount = 512 * processing.sectorspertrack * 160;
            }
            else if (processing.diskformat == DiskFormat.diskspare) // DiskSpare
            {
                sectorcount = 512 * processing.sectorspertrack * 160;
            }
            else if (processing.diskformat == DiskFormat.pcdd) // PC DD
            {
                sectorcount = 512 * processing.sectorspertrack * 80 * 2;
            }
            else if (processing.diskformat == DiskFormat.pchd) // PC HD
            {
                sectorcount = 512 * processing.sectorspertrack * 80 * 2;
            }
            else if (processing.diskformat == DiskFormat.pc2m)
            {
                sectorcount = 2048 * 1024;
            }
            */
            try
            {
                reader = new BinaryReader(new FileStream(path, FileMode.Open));
            }
            catch (IOException ex)
            {
                tbreceived.Append("IO error: " + ex.ToString() + "\r\n");
                ioerror = 1;
            }
            if (ioerror == 0) // skip writing on io error
            {
                processing.diskformat = (DiskFormat)reader.ReadByte();

                sectorcount = reader.ReadInt32();

                //Load sector data
                for (i = 0; i < sectorcount; i++)
                    processing.disk[i] = reader.ReadByte();

                //Load sector status
                for (i = 0; i < 184; i++)
                    for (j = 0; j < 18; j++)
                        processing.sectormap.sectorok[i, j] = (SectorMapStatus)reader.ReadByte();
                reader.Close();
                reader.Dispose();
            }
            else ioerror = 0;

            tbreceived.Append("Load complete.\r\n");
            tbreceived.Append("Sectorcount: " + sectorcount + "\r\n");
            tbreceived.Append("diskformat:" + processing.diskformat + "\r\n");

            processing.sectormap.RefreshSectorMap();
            //ShowDiskFormat();
        }

        /// <summary>
        /// Saves the rxbuf data with only good or only bad sectors. Can be saved as a single bin file or separate files per sector (in a sector folder). 
        /// The separate sectors will overwrite already written data and is for debugging only.
        /// </summary>
        /// <param name="four"></param>
        /// <param name="six"></param>
        /// <param name="eight"></param>
        /// <param name="Processingmode"></param>
        /// <param name="OnlyBadSectors"></param>
        /// <param name="FilePerSector"></param>
        public void SaveTrimmedBinFile(int four, int six, int eight, string Processingmode, bool OnlyBadSectors = true, bool FilePerSector = false)
        {
            int i, j;
            string extension = "";
            int ioerror = 0;
            
            var sectordata2 = processing.sectordata2;

            i = 0;
            int q = 0;

            // Write period data to disk in bin format
            if( OnlyBadSectors == true)
                extension = "_trimmedbad.bin";
            else extension = "_trimmed.bin";

            string path = "";

            if (FilePerSector == true)
            {
                path = PathToRecoveredDisks + @"\" + BaseFileName + @"\sectors\";
                bool exists = System.IO.Directory.Exists(path);
                if (!exists)
                    System.IO.Directory.CreateDirectory(path);
            }
            else
            {
                path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_" + binfilecount.ToString("D3") + extension;
                bool exists = System.IO.Directory.Exists(path);

                while (File.Exists(path))
                {
                    binfilecount++;
                    path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_" + binfilecount.ToString("D3") + extension;
                }

                try
                {
                    writer = new BinaryWriter(new FileStream(path, FileMode.Create));
                }
                catch (IOException ex)
                {
                    tbreceived.Append("IO error: " + ex.ToString());
                    ioerror = 1;
                    return;
                }
            }

            tbreceived.Append("Path:" + path + "\r\n");

            //Only save if there's any data to save

            int badsectorcnt = 0;
            int bytessaved = 0;
            // Save all bad sectors
            int smallercount = 0;
            int increaseRange = 10;
            // longer offsets means often noise at the end, header was not found due to noise. If the noise is at the start of the next 
            // sector data, the adaptive period detector gets confused, missing the header enitrely.
            // To remedy this, use more of the data before the header.
            
            byte[,] sectordone = new byte[255, 25];
            MFMData sectordataheader, sectordata;
            if (processing.procsettings.platform == 0) // PC
            {
                for (i = 0; i < sectordata2.Count; i++)
                {
                    sectordataheader = sectordata2[i];
                    if (sectordataheader.track == 57 && sectordataheader.sector == 7)
                    {
                        int aaa = 1;
                    }
                    if (sectordataheader.track == 115 && sectordataheader.sector == 3)
                    {
                        int aaa = 2;
                    }
                    if (sectordataheader.MarkerType == MarkerType.header || sectordataheader.MarkerType == MarkerType.headerAndData)
                    {
                        if (sectordataheader.DataIndex != 0)
                            sectordata = sectordata2[sectordataheader.DataIndex];
                        else continue;

                        if (!OnlyBadSectors)
                        {

                            if (sectordata.mfmMarkerStatus == SectorMapStatus.CrcOk || sectordata.mfmMarkerStatus == SectorMapStatus.SectorOKButZeroed)
                            {
                                if (processing.sectormap.sectorok[sectordata.track, sectordata.sector] == SectorMapStatus.CrcOk ||
                                    processing.sectormap.sectorok[sectordata.track, sectordata.sector] == SectorMapStatus.SectorOKButZeroed)
                                {
                                    if (sectordone[sectordata.track, sectordata.sector] == 1)
                                        continue; // skip sectors that are done already.
                                    if (sectordataheader.track == 57 && sectordataheader.sector == 7)
                                    {
                                        int aaa = 1;
                                    }
                                    TrackSectorOffset tso = new TrackSectorOffset();

                                    tso.offsetstart = sectordataheader.rxbufMarkerPositions;

                                    tso.offsetend = tso.offsetstart + 6000;
                                    tso.length = tso.offsetend - tso.offsetstart;
                                    badsectorcnt++;
                                    tso.offsetstart -= increaseRange;
                                    tso.offsetend += increaseRange;

                                    //writer.Write("T" + track.ToString("D3") + "S" + sector);
                                    if (tso.offsetstart < 0) tso.offsetstart = 0;

                                    string path1 = path + BaseFileName+"_T" + sectordata.track.ToString("D3") + "_S" + sectordata.sector.ToString("D2")+extension;

                                    if (FilePerSector == true) // Save 1 file per sector for debugging
                                    {
                                        try
                                        {
                                            writer = new BinaryWriter(new FileStream(path1, FileMode.Create));
                                        }
                                        catch (IOException ex)
                                        {
                                            tbreceived.Append("IO error: " + ex.ToString());
                                            ioerror = 1;
                                        }

                                        for (q = tso.offsetstart; q < tso.offsetend; q++, bytessaved++)
                                            writer.Write((byte)(processing.rxbuf[q]));
                                        //tsoffset[track, sector] = tso;
                                        sectordone[sectordata.track, sectordata.sector] = 1;
                                        int len = tso.length;
                                        writer.Flush();
                                        writer.Close();
                                        writer.Dispose();
                                    }
                                    else // save 1 bin file for all data.
                                    {
                                        for (q = tso.offsetstart; q < tso.offsetend; q++, bytessaved++)
                                            writer.Write((byte)(processing.rxbuf[q]));
                                        //tsoffset[track, sector] = tso;
                                        sectordone[sectordata.track, sectordata.sector] = 1;
                                        int len = tso.length;
                                    }
                                }
                            }
                        }

                        else
                        {
                            if (sectordata.mfmMarkerStatus == SectorMapStatus.HeadOkDataBad)
                            {
                                if (processing.sectormap.sectorok[sectordata.track, sectordata.sector] != SectorMapStatus.HeadOkDataBad)
                                    continue; // skip if sector is already good
                                TrackSectorOffset tso = new TrackSectorOffset();
                                tso.offsetstart = sectordataheader.rxbufMarkerPositions;
                                for (q = i + 1; q < sectordata2.Count; q++)
                                {
                                    if (sectordata2[q].mfmMarkerStatus != 0 &&
                                        (sectordata2[q].MarkerType == MarkerType.header || sectordata2[q].MarkerType == MarkerType.headerAndData))
                                    {
                                        tso.offsetend = sectordata2[q].rxbufMarkerPositions;
                                        break;
                                    }
                                }
                                if (tso.offsetend == 0) tso.offsetend = tso.offsetstart + 10000;
                                tso.length = tso.offsetend - tso.offsetstart;
                                if (tso.length < 8000)
                                {

                                    tso.offsetend = tso.offsetstart + 20000 - tso.length;
                                    tso.length = tso.offsetend - tso.offsetstart;
                                }
                                badsectorcnt++;
                                //writer.Write("T" + track.ToString("D3") + "S" + sector);
                                for (q = tso.offsetstart; q < tso.offsetend; q++, bytessaved++)
                                    writer.Write((byte)(processing.rxbuf[q]));
                                //tsoffset[track, sector] = tso;
                            }
                        }

                    }

                }

                sectordone = null;
                int aaaa = smallercount;
            }
            else // Amiga
            {
                for (i = 0; i < sectordata2.Count; i++)
                {
                    sectordata = sectordata2[i];
                    if (sectordata.mfmMarkerStatus == SectorMapStatus.HeadOkDataBad)
                    {

                        TrackSectorOffset tso = new TrackSectorOffset();
                        tso.offsetstart = sectordata.rxbufMarkerPositions;
                        for (q = i + 1; q < sectordata2.Count; q++)
                        {
                            if (sectordata.mfmMarkerStatus != 0 &&
                                (sectordata.MarkerType == MarkerType.header || sectordata.MarkerType == MarkerType.headerAndData))
                            {
                                tso.offsetend = sectordata2[q].rxbufMarkerPositions;
                                break;
                            }
                        }
                        if (tso.offsetend == 0) tso.offsetend = tso.offsetstart + 10000;
                        tso.length = tso.offsetend - tso.offsetstart;
                        if (tso.length > 10000)
                        {

                            tso.offsetend = tso.offsetstart + 10000;
                            tso.length = tso.offsetend - tso.offsetstart;
                        }
                        badsectorcnt++;
                        //writer.Write("T" + track.ToString("D3") + "S" + sector);
                        for (q = tso.offsetstart; q < tso.offsetend; q++)
                            writer.Write((byte)(processing.rxbuf[q]));
                        //tsoffset[track, sector] = tso;
                    }
                }
            }

            tbreceived.Append("Sectors: " + badsectorcnt + " Trimmed bin file saved succesfully.\r\n");
            if (writer != null)
            {
                
                try
                {
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
                catch(Exception e)
                {
                    tbreceived.Append("File already closed.");
                }
                
            }
        }

        public void OpenScp()
        {
            byte[] temp;

            OpenFileDialog loadwave = new OpenFileDialog();
            loadwave.InitialDirectory = PathToRecoveredDisks + @"\" + BaseFileName;
            loadwave.Filter = "scp files (*.scp)|*.scp|All files(*.*)|*.*";
            //Bin files (*.bin)|*.bin|All files (*.*)|*.*

            if (loadwave.ShowDialog() == DialogResult.OK)
            {
                //try
                {
                    string file = loadwave.FileName;
                    string ext = Path.GetExtension(file);
                    string filename = Path.GetFileName(file);
                    textBoxFilesLoaded.AppendText(filename + "\r\n");
                    //graphset.filename = filename;
                    // D:\data\Projects\FloppyControl\DiskRecoveries\M003 MusicDisk\ScopeCaptures
                    //string file = @"D:\data\Projects\FloppyControl\DiskRecoveries\M003 MusicDisk\ScopeCaptures\diff4_T02_H1.wfm";
                    reader = new BinaryReader(new FileStream(file, FileMode.Open));

                    //string path1 = Path.GetFileName(file);

                    //textBoxFilesLoaded.Text += path1 + "\r\n";
                    //processing.CurrentFiles += path1 + "\r\n";
                    //BaseFileName = path1.Substring(0, path1.IndexOf("_"));

                    if (ext == ".scp")
                    {
                        //reader.BaseStream.Length
                        long length = reader.BaseStream.Length;

                        temp = reader.ReadBytes((int)length);
                        int i;

                        for (i = 0; i < length - 2; i += 2)
                        {
                            processing.rxbuf[processing.indexrxbuf++] = (byte)((temp[i] << 8 | temp[i + 1]) >> 1);
                            //processing.rxbuf[processing.indexrxbuf++] = (byte)((temp[i] << 8 | temp[i + 1]) - 127);
                        }

                    }

                    reader.Close();
                    reader.Dispose();
                }
            }
        }

        public void SaveSCP(int four, int six, int eight, string Processingmode)
        {
            int i, j;
            string extension = "";
            int ioerror = 0;
            int qq = 0;
            var sectordata2 = processing.sectordata2;

            // First check if there's sectordata2 data available and that all sectors are ok in sectorok array
            int sectorspertrack = processing.diskGeometry[processing.diskformat].sectorsPerTrack;
            int tracksperdisk = processing.diskGeometry[processing.diskformat].tracksPerDisk;
            int sectorsize = processing.diskGeometry[processing.diskformat].sectorSize;
            int numberofheads = processing.diskGeometry[processing.diskformat].numberOfHeads;

            TrackSectorOffset[,] tsoffset = new TrackSectorOffset[200, 20];


            int sectorstotal = sectorspertrack * tracksperdisk * numberofheads;

            byte scpformat;
            int[] tracklengthrxbuf = new int[200];

            int averagetimecompensation;
            ProcessingType procmode = ProcessingType.adaptive1;
            if (Processingmode != "")
                procmode = (ProcessingType)Enum.Parse(typeof(ProcessingType), Processingmode, true);

            if (procmode == ProcessingType.adaptive1 || procmode == ProcessingType.adaptive2 || procmode == ProcessingType.adaptive3)
                averagetimecompensation = ((80 - four) + (120 - six) + (160 - eight)) / 3;
            else
                averagetimecompensation = 5;

            // FloppyControl app DiskFormat to SCP format
            byte[] fca2ScpDiskFormat = new byte[] {
                0, // 0 not used
                0x04, // 1 AmigaDOS
                0x04, // 2 DiskSpare
                0x41, // 3 PC DS DD 
                0x43, // 4 PC HD
                0x43, // 5 PC 2M
                0x40, // 6 PC SS DD
                0x04, // DiskSpare 984KB
            };

            scpformat = fca2ScpDiskFormat[(int)processing.diskformat];

            //Checking sectorok data
            int track = 0, sector = 0;
            for (track = 0; track < tracksperdisk; track++)
            {
                for (sector = 0; sector < sectorspertrack; sector++)
                {
                    if (processing.sectormap.sectorok[track, sector] != SectorMapStatus.CrcOk)
                        break;
                }
            }

            if (track != tracksperdisk || sector != sectorspertrack)
            {
                tbreceived.Append("Error: not all sectors are good in the sectormap. Can't continue.\r\n");
                return;
            }

            byte[,] sectorchecked = new byte[200, 20];
            int sectorokchecksum = 0;
            int trackhead = tracksperdisk * numberofheads;
            //checking sectordata2 if all sectorok items are represented in sectordata2 
            for (track = 0; track < trackhead; track++)
            {
                for (sector = 0; sector < sectorspertrack; sector++)
                {

                    for (i = 0; i < sectordata2.Count; i++)
                    {
                        if (sectordata2[i].sector == sector && sectordata2[i].track == track)
                        {
                            if (sectordata2[i].mfmMarkerStatus == SectorMapStatus.CrcOk ||
                                sectordata2[i].mfmMarkerStatus == SectorMapStatus.SectorOKButZeroed ||
                                sectordata2[i].mfmMarkerStatus == SectorMapStatus.ErrorCorrected)
                            {
                                sectorchecked[track, sector] = 1;
                                sectorokchecksum++;
                                break;
                            }
                        }
                    }
                }
            }

            if (sectorokchecksum != sectorstotal)
            {
                tbreceived.Append("Not all sectors are represented in sectordata2. Can't continue. \r\n");
                return;
            }

            int offset = 0;
            i = 0;
            int q = 0;

            MFMData sectordataheader;
            MFMData sectordata;
            // Find all track and sector offsets and lengths
            if (processing.procsettings.platform == 0) // PC
            {
                for (track = 0; track < trackhead; track++)
                {
                    for (sector = 0; sector < sectorspertrack; sector++)
                    {
                        for (i = 0; i < sectordata2.Count; i++)
                        {
                            sectordataheader = sectordata2[i];
                            if (sectordataheader.sector == sector && sectordataheader.track == track)
                            {
                                if (sectordataheader.MarkerType == MarkerType.header || sectordataheader.MarkerType == MarkerType.headerAndData)
                                {
                                    if (sectordataheader.DataIndex != 0)
                                        sectordata = sectordata2[sectordataheader.DataIndex];
                                    else continue;
                                    if (sectordata.mfmMarkerStatus == SectorMapStatus.CrcOk ||
                                        sectordata.mfmMarkerStatus == SectorMapStatus.SectorOKButZeroed ||
                                        sectordata.mfmMarkerStatus == SectorMapStatus.ErrorCorrected)
                                    {
                                        TrackSectorOffset tso = new TrackSectorOffset();
                                        tso.offsetstart = sectordataheader.rxbufMarkerPositions;
                                        for (q = i + 1; q < sectordata2.Count; q++)
                                        {
                                            if (sectordata2[q].mfmMarkerStatus != 0 &&
                                                (sectordata2[q].MarkerType == MarkerType.header || sectordata2[q].MarkerType == MarkerType.headerAndData))
                                            {
                                                tso.offsetend = sectordata2[q].rxbufMarkerPositions;
                                                break;
                                            }
                                        }
                                        if (tso.offsetend == 0) tso.offsetend = tso.offsetstart + 10000;
                                        tso.length = tso.offsetend - tso.offsetstart;
                                        if (tso.length > 10000)
                                        {

                                            tso.offsetend = tso.offsetstart + 10000;
                                            tso.length = tso.offsetend - tso.offsetstart;
                                        }
                                        tsoffset[track, sector] = tso;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else // Amiga
            {
                for (track = 0; track < trackhead; track++)
                {
                    for (sector = 0; sector < sectorspertrack; sector++)
                    {
                        for (i = 0; i < sectordata2.Count; i++)
                        {
                            sectordata = sectordata2[i];
                            if (sectordata.sector == sector && sectordata.track == track)
                            {
                                if (sectordata.mfmMarkerStatus == SectorMapStatus.CrcOk ||
                                    sectordata.mfmMarkerStatus == SectorMapStatus.SectorOKButZeroed ||
                                    sectordata.mfmMarkerStatus == SectorMapStatus.ErrorCorrected)
                                {
                                    TrackSectorOffset tso = new TrackSectorOffset();
                                    tso.offsetstart = sectordata.rxbufMarkerPositions;
                                    for (q = i + 1; q < sectordata2.Count; q++)
                                    {
                                        if (sectordata.mfmMarkerStatus != 0 &&
                                            (sectordata.MarkerType == MarkerType.header || sectordata.MarkerType == MarkerType.headerAndData))
                                        {
                                            tso.offsetend = sectordata2[q].rxbufMarkerPositions;
                                            break;
                                        }
                                    }
                                    if (tso.offsetend == 0) tso.offsetend = tso.offsetstart + 10000;
                                    tso.length = tso.offsetend - tso.offsetstart;
                                    if (tso.length > 10000)
                                    {

                                        tso.offsetend = tso.offsetstart + 10000;
                                        tso.length = tso.offsetend - tso.offsetstart;
                                    }
                                    tsoffset[track, sector] = tso;
                                }
                            }
                        }
                    }
                }
            }

            // calculate track time
            UInt32[] trackduration = new UInt32[200];
            int[] indexpulsespertrack = new int[200];
            int val = 0;
            for (track = 0; track < trackhead; track++)
            {
                for (sector = 0; sector < sectorspertrack; sector++)
                {
                    for (i = tsoffset[track, sector].offsetstart; i < tsoffset[track, sector].offsetend; i++)
                    {
                        val = processing.rxbuf[i];
                        trackduration[track] += (UInt32)((val) << 1);
                        if (val < 4) indexpulsespertrack[track]++;
                    }
                }
            }
            for (track = 0; track < trackhead; track++)
            {
                for (sector = 0; sector < sectorspertrack; sector++)
                {
                    tbreceived.Append("T: " + track.ToString("D3") + " S" + sector + "\to1:" +
                        tsoffset[track, sector].offsetstart + "\to2:" +
                        tsoffset[track, sector].offsetend +
                    "\tlength: " + tsoffset[track, sector].length + "\t durcation: " + trackduration[track] + "\r\n");
                }
            }
            UInt32[] offsettable = new UInt32[200];

            // Create offset table, calculated without header offset
            offsettable[0] = 0;
            int perioddataoffset = 0;
            int tracklength = 0;
            int value;
            for (track = 0; track < trackhead; track++)
            {
                offsettable[track] = (UInt32)(perioddataoffset);

                for (sector = 0; sector < sectorspertrack; sector++)
                {
                    value = tsoffset[track, sector].length;
                    perioddataoffset += value * 2;
                    tracklength += value;
                }
                perioddataoffset += 0x10;
                perioddataoffset -= (indexpulsespertrack[track] * 2);
                tracklengthrxbuf[track] = tracklength - indexpulsespertrack[track];
                tracklength = 0;
            }

            // Write period data to disk in bin format
            extension = ".scp";
            string path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_" + binfilecount.ToString("D3") + extension;

            tbreceived.Append("Path:" + path + "\r\n");

            bool exists = System.IO.Directory.Exists(path);

            while (File.Exists(path))
            {
                binfilecount++;
                path = PathToRecoveredDisks + @"\" + BaseFileName + @"\" + BaseFileName + "_" + binfilecount.ToString("D3") + extension;
            }

            //Only save if there's any data to save

            //if (processing.diskformat == 3 || processing.diskformat == 4) //PC 720 KB dd or 1440KB hd
            //{
            try
            {
                writer = new BinaryWriter(new FileStream(path, FileMode.Create));
            }
            catch (IOException ex)
            {
                tbreceived.Append("IO error: " + ex.ToString());
                ioerror = 1;
            }

            if (ioerror == 0) // skip writing on io error
            {
                //Write header
                writer.Write((byte)'S');            // 0
                writer.Write((byte)'C');            // 1
                writer.Write((byte)'P');            // 2
                writer.Write((byte)0x39);           // 3 Version 3.9
                writer.Write((byte)scpformat);      // 4 SCP disk type
                writer.Write((byte)1);            // 5 Number of revolutions
                writer.Write((byte)0);            // 6 Start track
                writer.Write((byte)(trackhead - 1)); // 7 end track
                writer.Write((byte)0);            // 8 Flags (copied from sample scp files)
                writer.Write((byte)0);            // 9 Bit depth of flux period data. Using default 16 bits shorts.
                writer.Write((byte)0);             // A number of heads
                writer.Write((byte)0);            // B reserved byte
                writer.Write((UInt32)0);            // C..F Checksum (not used for now)

                UInt32 headeroffset = (UInt32)(0x10 + (trackhead * 4));

                for (track = 0; track < trackhead; track++)
                {
                    writer.Write((UInt32)((offsettable[track]) + headeroffset));
                }

                qq = 0;
                for (track = 0; track < trackhead; track++)
                {

                    writer.Write((byte)'T');            // 0
                    writer.Write((byte)'R');            // 1
                    writer.Write((byte)'K');            // 2
                    writer.Write((byte)track);          // 0..2 track

                    // We're using one single revolution
                    writer.Write((UInt32)trackduration[track]);             // 0..2 track duration in nanoseconds/25
                    writer.Write((UInt32)tracklengthrxbuf[track]);          // 0..2 track flux periods (length)
                    writer.Write((UInt32)0x10);                    // 0..2


                    byte b1 = 0;
                    byte b2 = 0;
                    //int val;
                    //Save sector status
                    for (sector = 0; sector < sectorspertrack; sector++)
                    {
                        for (i = tsoffset[track, sector].offsetstart; i < tsoffset[track, sector].offsetend; i++)
                        {
                            val = processing.rxbuf[i] + averagetimecompensation;
                            if (val < 4 + averagetimecompensation) continue;
                            b1 = (byte)((val >> 7) & 1);
                            b2 = (byte)(val << 1);
                            if (b2 == 0x1a)
                                qq = 2;
                            writer.Write((byte)b1);
                            writer.Write((byte)b2);
                        }
                    }
                }
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
            }
            else ioerror = 0;
            //}
        }
    } //Class
}
