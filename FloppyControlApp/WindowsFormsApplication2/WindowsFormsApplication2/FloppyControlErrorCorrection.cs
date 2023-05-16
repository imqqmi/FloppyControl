using FloppyControlApp.MyClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloppyControlApp
{
    public partial class FloppyControl : Form
    {
        private void BadSectorByteDraw()
        {
            int i; //, datapoints, start, end, scrollbarcurrentpos;
            //decimal posx;
            int indexS1 = -1, indexS2 = -1;
            int offset = 4, diskoffset;
            int track, sector;
            byte[] sectors = new byte[1050];
            //int qq;
            int sectorlength = 512;
            int threadid;

            switch ((int)processing.diskformat)
            {
                case 0:
                    return;
                case 1:
                    offset = 0;
                    break;
                case 2:
                    offset = 0;
                    break;
                case 3:
                    offset = 4;
                    break;
                case 4:
                    offset = 4;
                    break;
                case 5:
                    offset = 4;
                    break;
            }

            badsectorkeyval badsector1;
            //textBoxReceived.Text += "";
            foreach (int q in BadSectorListBox.SelectedIndices)
            {
                badsector1 = (badsectorkeyval)BadSectorListBox.Items[q];
            }

            if (BadSectorListBox.SelectedIndices.Count == 1)
            {
                indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;
                indexS2 = -1;

                if (BSBlueSectormapRadio.Checked)
                {
                    track = processing.sectordata2[indexS1].track;
                    sector = processing.sectordata2[indexS1].sector;

                    diskoffset = track * sectorlength * processing.sectorspertrack + sector * sectorlength;
                    Array.Copy(processing.disk, diskoffset, sectors, 0, sectorlength);
                    offset = 0;
                }
                if (BSBlueFromListRadio.Checked)
                {
                    sectorlength = processing.sectordata2[indexS1].sectorlength;
                    Array.Copy(processing.sectordata2[indexS1].sectorbytes, 0, sectors, 0, sectorlength);

                    //offset = 4;
                }
                if (BlueTempRadio.Checked)
                {
                    Array.Copy(TempSector, 0, sectors, 0, sectorlength + 6);
                    //offset = 4;
                }

                BlueCrcCheckLabel.Text = "Crc: " + processing.sectordata2[indexS1].crc.ToString("X2");

                if (indexS2 != -1)
                    RedCrcCheckLabel.Text = "Crc: " + processing.sectordata2[indexS2].crc.ToString("X2");
                else RedCrcCheckLabel.Text = "Crc:";
            }
            else if (BadSectorListBox.SelectedIndices.Count >= 2)
            {
                indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                indexS2 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[1]]).id;
                threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;
                sectorlength = processing.sectordata2[indexS1].sectorlength;

                if (BSBlueFromListRadio.Checked)
                {
                    Array.Copy(processing.sectordata2[indexS1].sectorbytes, 0, sectors, 0, processing.sectordata2[indexS1].sectorbytes.Length);
                    //offset = 4;
                }

                BlueCrcCheckLabel.Text = "Crc: " + processing.sectordata2[indexS1].crc.ToString("X2");

                if (indexS2 != -1)
                    RedCrcCheckLabel.Text = "Crc: " + processing.sectordata2[indexS2].crc.ToString("X2");
                else RedCrcCheckLabel.Text = "Crc:";
            }
            else if (BSBlueSectormapRadio.Checked)
            {
                track = (int)Track1UpDown.Value;
                sector = (int)Sector1UpDown.Value;

                diskoffset = track * sectorlength * processing.sectorspertrack + sector * sectorlength;
                Array.Copy(processing.disk, diskoffset, sectors, 0, sectorlength);
                offset = 0;
            }
            else return; // nothing selected, nothing to do

            System.Drawing.Pen BlackPen;
            BlackPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 128, 128, 128));
            System.Drawing.Graphics formGraphics = BadSectorPanel.CreateGraphics();
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.White);


            ECHisto.DoHistogram(sectors, offset, sectorlength);
            HistScalingLabel.Text = "Scale: " + ECHisto.getScaling().ToString(); ;

            if (!BSBlueSectormapRadio.Checked) // there's no relevant data when this radio button is checked
            {
                int scatoffset = processing.sectordata2[indexS1].rxbufMarkerPositions + (int)ScatterMinTrackBar.Value + (int)ScatterOffsetTrackBar.Value;
                int scatlength = processing.sectordata2[indexS1].rxbufMarkerPositions + (int)ScatterMaxTrackBar.Value + (int)ScatterOffsetTrackBar.Value - scatoffset;

                scatterplot.AnScatViewlargeoffset = scatoffset;
                scatterplot.AnScatViewoffset = 0;
                scatterplot.AnScatViewlength = scatlength;
                scatterplot.UpdateScatterPlot();
            }
            using (var bmp1 = new System.Drawing.Bitmap(512, 256))
            {
                LockBitmap lockBitmap = new LockBitmap(bmp1);
                lockBitmap.LockBits();

                byte value1 = 0, value2 = 0, colorR = 0, colorB = 0;
                int y = 0;
                int y2, q;

                float f = 512.0f / sectorlength;

                //if (f == 0.5f)
                //{
                //    int qq = 2;
                //}

                for (i = 0; i < sectorlength; i++)
                {
                    value1 = sectors[i + offset];
                    if (indexS2 == -1)
                    {
                        colorB = value1;
                        value2 = 0;
                        colorR = 0;
                    }
                    else
                    {
                        value2 = processing.sectordata2[indexS2].sectorbytes[i + offset];
                        if (value1 == value2)
                        {
                            colorR = 0;
                            colorB = value1;
                        }
                        else
                        {
                            colorR = (byte)(128 + (value2 / 2));
                            colorB = value1;
                        }
                    }

                    y2 = 0;
                    for (q = 0; q < 256; q++)
                    {
                        lockBitmap.SetPixel((i % 32) * 16 + (q % 16), (int)(y * 16 * f + (y2 * f)), Color.FromArgb(255, colorR, 0, colorB));
                        if (q % 16 == 15) y2++;
                    }
                    if (i % 32 == 31) y++;
                }

                lockBitmap.UnlockBits();
                formGraphics.DrawImage(bmp1, 0, 0);
            }
            BlackPen.Dispose();
            formGraphics.Dispose();
            myBrush.Dispose();
        }
        private void BadMFMSectorDraw()
        {
            //decimal posx;
            int indexS1 = -1, indexS2 = -1;
            int offset = 4, diskoffset;
            int track, sector;
            int offsetmfm;
            int offsetmfm2;
            int lengthmfm = 0;
            byte[] sectors = new byte[1050];
            byte[] sectors2 = new byte[1050];
            //int qq;
            int sectorlength = 512;
            int threadid = 0;

            switch (processing.diskformat)
            {
                case DiskFormat.unknown:
                    textBoxReceived.AppendText("\r\nMissing disk format definition, can't draw map. See method BadMFMSectorDraw().\r\n");
                    return;
                case DiskFormat.amigados: //AmigaDos
                    offset = 0;
                    lengthmfm = 8704;
                    break;
                case DiskFormat.diskspare://diskspare
                    offset = 0;
                    lengthmfm = 8320;
                    break;
                case DiskFormat.pcdd://pc2m
                case DiskFormat.pc360kb525in:// PC360KB 5.25"
                    offset = -704;
                    lengthmfm = 10464;
                    break;
                case DiskFormat.pchd://pcdd
                    offset = -704;
                    lengthmfm = 10464;
                    break;
                case DiskFormat.pc2m://pchd
                    offset = -704;
                    lengthmfm = 10464;
                    break;
            }

            badsectorkeyval badsector1;
            //textBoxReceived.Text += "";
            foreach (int q in BadSectorListBox.SelectedIndices)
            {
                badsector1 = (badsectorkeyval)BadSectorListBox.Items[q];
            }

            if (BadSectorListBox.SelectedIndices.Count == 1)
            {
                indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;
                indexS2 = -1;

                if (BSBlueSectormapRadio.Checked)
                {
                    track = processing.sectordata2[indexS1].track;
                    sector = processing.sectordata2[indexS1].sector;

                    diskoffset = track * sectorlength * processing.sectorspertrack + sector * sectorlength;
                    Array.Copy(processing.disk, diskoffset, sectors, 0, sectorlength);
                    offset = 0;
                }
                if (BSBlueFromListRadio.Checked)
                {
                    threadid = processing.sectordata2[indexS1].threadid;

                    sectorlength = processing.sectordata2[indexS1].sectorlength;
                    //Array.Copy(processing.sectordata2[indexS1].sectorbytes, 0, sectors, 0, sectorlength);
                    offsetmfm = processing.sectordata2[indexS1].MarkerPositions;
                    sectors = processing.MFM2ByteArray(processing.mfms[threadid], offsetmfm + offset, lengthmfm);

                    //offset = 4;
                }
                if (BlueTempRadio.Checked)
                {
                    Array.Copy(TempSector, 0, sectors, 0, sectorlength + 6);
                    //offset = 4;
                }

                BlueCrcCheckLabel.Text = "Crc: " + processing.sectordata2[indexS1].crc.ToString("X2");

                if (indexS2 != -1)
                    RedCrcCheckLabel.Text = "Crc: " + processing.sectordata2[indexS2].crc.ToString("X2");
                else RedCrcCheckLabel.Text = "Crc:";
            }
            else if (BadSectorListBox.SelectedIndices.Count >= 2)
            {
                indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                indexS2 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[1]]).id;
                //threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;

                threadid = processing.sectordata2[indexS1].threadid;
                sectorlength = processing.sectordata2[indexS1].sectorlength;

                offsetmfm = processing.sectordata2[indexS1].MarkerPositions;
                sectors = processing.MFM2ByteArray(processing.mfms[threadid], offsetmfm + offset, lengthmfm);

                threadid = processing.sectordata2[indexS2].threadid;
                offsetmfm2 = processing.sectordata2[indexS2].MarkerPositions;
                sectors2 = processing.MFM2ByteArray(processing.mfms[threadid], offsetmfm2 + offset, lengthmfm);

                BlueCrcCheckLabel.Text = "Crc: " + processing.sectordata2[indexS1].crc.ToString("X2");

                if (indexS2 != -1)
                    RedCrcCheckLabel.Text = "Crc: " + processing.sectordata2[indexS2].crc.ToString("X2");
                else RedCrcCheckLabel.Text = "Crc:";
            }
            else if (BSBlueSectormapRadio.Checked)
            {
                track = (int)Track1UpDown.Value;
                sector = (int)Sector1UpDown.Value;

                diskoffset = track * sectorlength * processing.sectorspertrack + sector * sectorlength;
                Array.Copy(processing.disk, diskoffset, sectors, 0, sectorlength);
                offset = 0;
            }
            else return; // nothing selected, nothing to do

            System.Drawing.Graphics formGraphics = BadSectorPanel.CreateGraphics();

            //ECHisto.DoHistogram(sectors, offset, sectorlength);
            //HistScalingLabel.Text = "Scale: " + ECHisto.getScaling().ToString(); ;

            if (!BSBlueSectormapRadio.Checked) // there's no relevant data when this radio button is checked
            {
                int scatoffset = processing.sectordata2[indexS1].rxbufMarkerPositions + (int)ScatterMinTrackBar.Value + (int)ScatterOffsetTrackBar.Value;
                int scatlength = processing.sectordata2[indexS1].rxbufMarkerPositions + (int)ScatterMaxTrackBar.Value + (int)ScatterOffsetTrackBar.Value - scatoffset;

                scatterplot.AnScatViewlargeoffset = scatoffset;
                scatterplot.AnScatViewoffset = 0;
                scatterplot.AnScatViewlength = scatlength;
                scatterplot.UpdateScatterPlot();
            }
            //StringBuilder mfmbyteEnc = new StringBuilder();

            using (var bmp1 = new System.Drawing.Bitmap(520, 256))
            {
                LockBitmap lockBitmap = new LockBitmap(bmp1);
                lockBitmap.LockBits();

                //lockBitmap.filledsquare(10, 10, 16, 16, Color.FromArgb(255, 0, 0, 255));


                byte value1 = 0, value2 = 0, colorR = 0, colorB = 0;

                //int y2;
                //int width = 40;

                int height = 32; //sectors.Length / width;
                int sectorsindex = 0;
                int w, h, x, y;
                w = 13;
                h = (256 / height);

                float f = 512.0f / sectorlength;

                //if (f == 0.5f)
                //{
                //    int qq = 2;
                //}

                if (sectors.Length > 0)

                    for (y = 0; y < 256; y += h)
                    {
                        for (x = 0; x < 520; x += w)
                        {
                            //Thread.Sleep(250);
                            //lockBitmap.UnlockBits();
                            //formGraphics.DrawImage(bmp1, 0, 0);
                            //lockBitmap.LockBits();

                            value1 = sectors[sectorsindex];
                            mfmbyteenc[value1]++;
                            //mfmbyteEnc.Append(value1.ToString("X2")+" ");
                            if (indexS2 == -1)
                            {
                                colorB = value1;
                                value2 = 0;
                                colorR = 0;
                            }
                            else
                            {
                                value2 = sectors2[sectorsindex];
                                if (value1 == value2)
                                {
                                    colorR = 0;
                                    colorB = value1;
                                }
                                else
                                {
                                    colorR = (byte)(128 + (value2 / 2));
                                    colorB = value1;
                                }
                            }

                            sectorsindex++;
                            if (sectorsindex >= sectors.Length)
                                break;

                            lockBitmap.filledsquare(x, y, w, h, Color.FromArgb(255, colorR, 0, colorB));

                        }
                        if (sectorsindex >= sectors.Length)
                            break;
                    }

                lockBitmap.UnlockBits();
                formGraphics.DrawImage(bmp1, 0, 0);
            }
            formGraphics.Dispose();
            //tbSectorMap.AppendText(mfmbyteEnc.ToString());
        }
        public void BadSectorPanelClick()
        {
            int indexS1;//, indexS2;
            int offset = 4;
            int diskoffset;
            int x, y;
            int bsbyte;
            int track, sectornr;
            int datacrc;

            int i;
            int threadid;

            BadSectorTooltipPos = BadSectorPanel.PointToClient(Cursor.Position);

            if (BadSectorListBox.SelectedIndices.Count >= 1)
            {
                indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;

                int sectorlength = processing.sectordata2[indexS1].sectorlength;
                if (sectorlength < 512)
                {
                    tbreceived.Append("sector length is less than 512 bytes!!");
                    return;
                }

                if (ECMFMcheckBox.Checked)
                {
                    int w = 13;
                    int h = 8;
                    //int lengthmfm;
                    int mfmoffset2 = 0;

                    switch ((int)processing.diskformat)
                    {
                        case 0:
                            return;
                        case 1: //AmigaDos
                            offset = -48;
                            //lengthmfm = 8704;
                            break;
                        case 2://diskspare
                            offset = -16;
                            //lengthmfm = 8320;
                            break;
                        case 3://pcdd
                            offset = -712;
                            //lengthmfm = 10464;
                            mfmoffset2 = -712;
                            break;
                        case 4://pchd
                            offset = -712;
                            //lengthmfm = 10464;
                            mfmoffset2 = -712;
                            break;
                        case 5://pc2m
                            offset = -712;
                            //lengthmfm = 10464;
                            mfmoffset2 = -712;
                            break;
                    }
                    //if (f == 0.0f) f = 1;
                    x = ((BadSectorTooltipPos.X) / w);
                    y = (int)(BadSectorTooltipPos.Y / h);
                    //int offset;


                    int mfmoffset = processing.sectordata2[indexS1].MarkerPositions;
                    bsbyte = ((y * 40 + x) * 8) + offset;
                    MFMByteStartUpDown.Value = ((y * 40 + x) * 8) + mfmoffset2;
                    int indexcnt = 0;
                    if (bsbyte > 0)
                    {
                        for (i = 0; i < bsbyte; i++)
                        {
                            if (processing.mfms[processing.sectordata2[indexS1].threadid][i + mfmoffset] == 1)
                            {
                                indexcnt++;
                            }
                        }
                    }
                    else
                    {
                        for (i = bsbyte; i < 0; i++)
                        {
                            if (processing.mfms[processing.sectordata2[indexS1].threadid][i + mfmoffset] == 1)
                            {
                                indexcnt--;
                            }
                        }
                    }
                    tbreceived.Append("index:" + indexcnt + "\r\n");
                    ScatterMinTrackBar.Value = indexcnt;
                    ScatterMaxTrackBar.Value = indexcnt + 14;
                    updateECInterface();
                }
                else
                {
                    int f = sectorlength / 512;

                    x = ((BadSectorTooltipPos.X) / 16);
                    y = (int)((BadSectorTooltipPos.Y) / (16 / f));

                    bsbyte = y * 32 + x;
                    // Temporary decouple event handler
                    byteinsector = bsbyte;
                    BSEditByteLabel.Text = "Byte: " + bsbyte;

                    // Zoom in scatterplot
                    int indexcnt = 0;
                    int mfmoffset = processing.sectordata2[indexS1].MarkerPositions;
                    // First find the period index
                    for (i = 0; i < (bsbyte + 4) * 16; i++)
                    {
                        if (processing.mfms[processing.sectordata2[indexS1].threadid][i + mfmoffset] == 1)
                        {
                            indexcnt++;
                        }
                    }
                    tbreceived.Append("index:" + indexcnt + "\r\n");
                    ScatterMinTrackBar.Value = indexcnt;
                    ScatterMaxTrackBar.Value = indexcnt + 14;
                    updateECInterface();
                    if ((int)BluetoRedByteCopyToolBtn.Tag == 1)
                    {
                        // Copy single byte from BadSectors to disk array
                        if (BSBlueSectormapRadio.Checked)
                        {
                            textBoxReceived.AppendText("Copy byte to disk array.");
                            track = processing.sectordata2[indexS1].track;
                            sectornr = processing.sectordata2[indexS1].sector;
                            datacrc = processing.sectordata2[indexS1].crc;

                            processing.sectorspertrack = 9;

                            //(tracknr * processing.sectorspertrack * 512 * 2) + (headnr * processing.sectorspertrack * 512) + (sectornr * 512);
                            diskoffset = track * processing.sectorspertrack * 512 + sectornr * 512;
                            processing.disk[diskoffset] = processing.sectordata2[indexS1].sectorbytes[bsbyte + offset];
                        }

                        //Copy byte from BadSectors to TempSector
                        if (BlueTempRadio.Checked)
                        {
                            textBoxReceived.AppendText("Copy byte to Temp.");
                            track = processing.sectordata2[indexS1].track;
                            sectornr = processing.sectordata2[indexS1].sector;
                            datacrc = processing.sectordata2[indexS1].crc;

                            processing.sectorspertrack = 9;

                            //(tracknr * processing.sectorspertrack * 512 * 2) + (headnr * processing.sectorspertrack * 512) + (sectornr * 512);
                            diskoffset = track * processing.sectorspertrack * 512 + sectornr * 512;
                            TempSector[bsbyte + offset] = processing.sectordata2[indexS1].sectorbytes[bsbyte + offset];
                        }

                        //Check crc
                        ushort datacrcchk;
                        Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
                        datacrcchk = crc.ComputeChecksum(TempSector);
                        BlueCrcCheckLabel.Text = "Crc: " + datacrcchk.ToString("X2");

                        processing.sectordata2[indexS1].crc = datacrcchk;
                    }
                }
            }
        }

        public void CopySectorToBlue()
        {
            int i;
            int indexS1; //indexS2;
            int offset = 4;
            int diskoffset;
            //int x, y;
            //int bsbyte;
            int track, sectornr;
            int datacrc;
            //int processing.sectorspertrack;
            int threadid;

            switch ((int)processing.diskformat)
            {
                case 0:
                    return;
                case 1:
                    offset = 0;
                    break;
                case 2:
                    offset = 0;
                    break;
                case 3:
                    offset = 4;
                    break;
                case 4:
                    offset = 4;
                    break;
                case 5:
                    offset = 4;
                    break;
            }

            if (BadSectorListBox.SelectedIndices.Count == 1)
            {
                indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;

                // Copy sector from BadSectors to disk array
                if (BSBlueSectormapRadio.Checked && BSRedFromlistRadio.Checked)
                {
                    textBoxReceived.AppendText("Copy single sector to disk array.");
                    track = processing.sectordata2[indexS1].track;
                    sectornr = processing.sectordata2[indexS1].sector;
                    datacrc = processing.sectordata2[indexS1].crc;
                    //processing.sectorspertrack = 9;

                    //(tracknr * processing.sectorspertrack * 512 * 2) + (headnr * processing.sectorspertrack * 512) + (sectornr * 512);
                    diskoffset = track * processing.sectorspertrack * 512 + sectornr * 512;

                    for (i = 0; i < 512; i++)
                    {
                        processing.disk[diskoffset + i] = processing.sectordata2[indexS1].sectorbytes[i + offset];
                    }
                }

                // Copy sector from BadSectors to temporary sector buffer
                if (BlueTempRadio.Checked && BSRedFromlistRadio.Checked)
                {
                    textBoxReceived.AppendText("Copy full sector to Temp.");
                    track = processing.sectordata2[indexS1].track;
                    sectornr = processing.sectordata2[indexS1].sector;
                    datacrc = processing.sectordata2[indexS1].crc;

                    //processing.sectorspertrack = 9;

                    // I combined tracks and head to simplify stuff
                    // My track = tracks * 2 + headnr
                    // track 10 head 1 is 21
                    diskoffset = track * processing.sectorspertrack * 512 + sectornr * 512;

                    for (i = 0; i < 518; i++)
                    {
                        TempSector[i] = processing.sectordata2[indexS1].sectorbytes[i];
                    }

                    //Check crc
                    ushort datacrcchk;
                    Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
                    datacrcchk = crc.ComputeChecksum(TempSector);
                    BlueCrcCheckLabel.Text = "Crc: " + datacrcchk.ToString("X2");

                    processing.sectordata2[indexS1].crc = datacrcchk;
                }

                // Copy sector from temporary sector buffer to disk array
                if (BlueTempRadio.Checked && BSRedTempRadio.Checked)
                {
                    textBoxReceived.AppendText("Copy full sector to Temp.");
                    track = processing.sectordata2[indexS1].track;
                    sectornr = processing.sectordata2[indexS1].sector;
                    datacrc = processing.sectordata2[indexS1].crc;

                    //processing.sectorspertrack = 9;

                    //(tracknr * processing.sectorspertrack * 512 * 2) + (headnr * processing.sectorspertrack * 512) + (sectornr * 512);
                    diskoffset = track * processing.sectorspertrack * 512 + sectornr * 512;

                    for (i = 0; i < 512; i++)
                    {
                        processing.disk[diskoffset + i] = TempSector[i + offset];
                    }

                    //Do crc check
                    ushort datacrcchk;
                    Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
                    datacrcchk = crc.ComputeChecksum(TempSector);
                    BlueCrcCheckLabel.Text = "Crc: " + datacrcchk.ToString("X2");

                    processing.sectordata2[indexS1].crc = datacrcchk;
                }
            }
        }

        public void ECSectorOverlay()
        {
            int i, track1, sector1, track2, sector2;
            //int offset = 4;
            //uint databyte;
            StringBuilder bytesstring = new StringBuilder();
            StringBuilder txtstring = new StringBuilder();
            StringBuilder badsecttext = new StringBuilder();
            string key;

            BadSectorListBox.DisplayMember = "name";
            BadSectorListBox.ValueMember = "id";

            track1 = (int)Track1UpDown.Value;
            sector1 = (int)Sector1UpDown.Value;

            track2 = (int)Track2UpDown.Value;
            sector2 = (int)Sector2UpDown.Value;

            BadSectorListBox.Items.Clear();
            JumpTocomboBox.Items.Clear();

            bool goodsectors = GoodSectorsCheckBox.Checked;
            bool badsectors = BadSectorsCheckBox.Checked;

            // First determine if there's bad sectors with the same track and sector
            //int threadid;
            MFMData sectordata;

            for (i = 0; i < processing.sectordata2.Count; i++)
            {
                sectordata = processing.sectordata2[i];
                if (sectordata.track >= track1 && sectordata.track <= track2 &&
                    sectordata.sector >= sector1 && sectordata.sector <= sector2)
                {
                    if ((sectordata.mfmMarkerStatus == SectorMapStatus.HeadOkDataBad) && badsectors)
                    {

                        key = "i: " + i + " B T: " + sectordata.track + " S: " + sectordata.sector;

                        BadSectorListBox.Items.Add(new badsectorkeyval
                        {
                            name = key,
                            id = i,
                            threadid = 0
                        });

                        JumpTocomboBox.Items.Add(new ComboboxItem
                        {
                            Text = key,
                            id = i,
                        });
                    }
                    if ((sectordata.mfmMarkerStatus == SectorMapStatus.CrcOk) && goodsectors)
                    {
                        key = "i: " + i + " G T: " + sectordata.track + " S: " + sectordata.sector;

                        BadSectorListBox.Items.Add(new badsectorkeyval
                        {
                            name = key,
                            id = i,
                            threadid = 0
                        });
                        JumpTocomboBox.Items.Add(new ComboboxItem
                        {
                            Text = key,
                            id = i,
                        });
                    }
                }

                txtstring.Clear();
                bytesstring.Clear();
            }
        }

        public void BadSectorToolTip()
        {
            int x, y, bsbyte, indexS1 = 0, indexS2;
            int offset = 4;
            int sectorlength;
            int threadid = 0;

            switch ((int)processing.diskformat)
            {
                case 0:
                    return;
                case 1:
                    offset = 0;
                    break;
                case 2:
                    offset = 0;
                    break;
                case 3:
                    offset = 4;
                    break;
                case 4:
                    offset = 4;
                    break;
                case 5:
                    offset = 4;
                    break;
            }

            if (ECMFMcheckBox.Checked)
            {
                //if( processing.diskformat == DiskFormat.amigados || processing.diskformat == DiskFormat.diskspare || processing.diskformat == )
                if (BadSectorListBox.SelectedIndices.Count == 1)
                {
                    indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                    indexS2 = -1;
                    threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;
                }
                else if (BadSectorListBox.SelectedIndices.Count >= 2)
                {
                    indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                    indexS2 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[1]]).id;
                    threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;
                }
                else return;
                if (processing.sectordata2 == null) return;
                if (processing.sectordata2.Count == 0) return;
                sectorlength = processing.sectordata2[indexS1].sectorlength;
                BadSectorTooltipPos = BadSectorPanel.PointToClient(Cursor.Position);
                //int f = sectorlength / 512;
                int w = 13;
                int h = 8;
                //int lengthmfm;
                switch ((int)processing.diskformat)
                {
                    case 0:
                        return;
                    case 1: //AmigaDos
                        offset = 0;
                        //lengthmfm = 8704;
                        break;
                    case 2://diskspare
                        offset = 0;
                        //lengthmfm = 8320;
                        break;
                    case 3://pc2m
                        offset = -704;
                        //lengthmfm = 10464;
                        break;
                    case 4://pcdd
                        offset = -704;
                        //lengthmfm = 10464;
                        break;
                    case 5://pchd
                        offset = -704;
                        //lengthmfm = 10464;
                        break;
                }
                //if (f == 0.0f) f = 1;
                x = ((BadSectorTooltipPos.X) / w);
                y = (int)(BadSectorTooltipPos.Y / h);
                bsbyte = (y * 40 + x);

                //if (bsbyte > sectorlength - 1) return;

                if (BadSectorTooltipPos.X < 350)
                    BadSectorTooltipPos.X += 30;
                else BadSectorTooltipPos.X -= 150;
                int mfmoffset = bsbyte * 8 + offset;
                if (mfmoffset < offset) return;
                int mfmmarkerposition = processing.sectordata2[indexS1].MarkerPositions;
                threadid = processing.sectordata2[indexS1].threadid;
                byte[] mfm = processing.MFM2ByteArray(processing.mfms[threadid], mfmmarkerposition + mfmoffset, 256);
                BadSectorTooltip.Text = " Offset: " + (mfmoffset) + " = " + mfm[0].ToString("X2"); ;
                BadSectorTooltip.Show();

            }
            else
            {

                //if( processing.diskformat == DiskFormat.amigados || processing.diskformat == DiskFormat.diskspare || processing.diskformat == )
                if (BadSectorListBox.SelectedIndices.Count == 1)
                {
                    indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                    indexS2 = -1;
                    threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;
                }
                else if (BadSectorListBox.SelectedIndices.Count >= 2)
                {
                    indexS1 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).id;
                    indexS2 = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[1]]).id;
                    threadid = ((badsectorkeyval)BadSectorListBox.Items[BadSectorListBox.SelectedIndices[0]]).threadid;
                }
                else return;
                if (processing.sectordata2 == null) return;
                if (processing.sectordata2.Count == 0) return;
                sectorlength = processing.sectordata2[indexS1].sectorlength;
                BadSectorTooltipPos = BadSectorPanel.PointToClient(Cursor.Position);
                int f = sectorlength / 512;
                if (f == 0.0f) f = 1;
                x = ((BadSectorTooltipPos.X) / 16);
                y = (int)((BadSectorTooltipPos.Y) / (16 / f));
                bsbyte = y * 32 + x;

                if (bsbyte > sectorlength - 1) return;

                if (BadSectorTooltipPos.X < 350)
                    BadSectorTooltipPos.X += 30;
                else BadSectorTooltipPos.X -= 150;

                //BadSectors[indexS1][i + offset];
                //BadSectorTooltip.Text = "X: " + x + " Y:" + y + " byte: " + bsbyte;
                if (bsbyte >= 0 && bsbyte <= (sectorlength + 6) - 4)
                {
                    BadSectorTooltip.Text = " byte: " + bsbyte + " = " + processing.sectordata2[indexS1].sectorbytes[bsbyte + offset].ToString("X2");
                    BadSectorTooltip.Show();
                }
            }
        }
    } //Class
}
