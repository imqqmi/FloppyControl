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
    class FileIO
    {
        public bool AddData { get; set; }
        public string[] OpenFilesPaths { get; set; }
        public string BaseFileName { get; set; }
        public string PathToRecoveredDisks { get; set; }
        public FDDProcessing processing { get; set; }
        public TextBox textBoxFilesLoaded { get; set; }
        public RichTextBox rtbSectorMap { get; set; }
        public StringBuilder tbreceived { get;set;}

        public Action FilesAvailableCallback { get; set; }
        public Action resetinput { get; set; }

        private BinaryReader reader;
        private BinaryWriter writer;
        private OpenFileDialog openFileDialog1;
        private int binfilecount = 0; // Keep saving each capture under a different filename as to keep all captured data


        public FileIO()
        {
            BaseFileName         = (string)Properties.Settings.Default["BaseFileName"];
            PathToRecoveredDisks = (string)Properties.Settings.Default["PathToRecoveredDisks"];
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "Bin files (*.bin)|*.bin|Kryoflux (*.raw)|*.raw|All files (*.*)|*.*";
            openFileDialog1.FileOk += openFileDialog1_FileOk;
            openFileDialog1.AddExtension = true;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.ValidateNames = true;
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

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            OpenFilesPaths = openFileDialog1.FileNames;
            FilesAvailableCallback();
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

            byte[] extra = new byte[processing.indexrxbuf];
            tempbuf.Add(extra);
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

    } //Class
}
