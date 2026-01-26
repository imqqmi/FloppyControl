using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DskX.Core;
using FloppyControlApp.MyClasses.Capture.Models;

namespace FloppyControlApp.MyClasses.Processing
{
    public static class RichTextBoxExtensions
    {
        private const int WM_USER = 0x0400;
        private const int EM_SETEVENTMASK = WM_USER + 69;
        private const int WM_SETREDRAW = 0x0b;
        private static nint OldEventMask;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern nint SendMessage(nint hWnd, int msg, nint wParam, nint lParam);

        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        public static void AppendText(this RichTextBox box, string text, Color fcolor, Color bcolor)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionBackColor = bcolor;
            box.SelectionColor = fcolor;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            box.SelectionBackColor = box.BackColor;
        }

        public static void BeginUpdate(this RichTextBox box)
        {
            SendMessage(box.Handle, WM_SETREDRAW, nint.Zero, nint.Zero);
            OldEventMask = SendMessage(box.Handle, EM_SETEVENTMASK, nint.Zero, nint.Zero);
        }

        public static void EndUpdate(this RichTextBox box)
        {
            SendMessage(box.Handle, WM_SETREDRAW, 1, nint.Zero);
            SendMessage(box.Handle, EM_SETEVENTMASK, nint.Zero, OldEventMask);
        }
    }

    public class SectorMap
    {
        public FDDProcessing processing { get; set; }
        public RichTextBox rtbSectorMap { get; set; }

        public int recoveredsectorcount { get; set; }
        public int RecoveredSectorWithErrorsCount { get; set; }
        public int badsectorcnt;

        public SectorMapStatus[,] sectorok = new SectorMapStatus[256, 256]; // track, sector5
        public SectorMapStatus[,] sectorokLatestScan = new SectorMapStatus[256, 256]; // track, sector
        //public byte[,] duplicateSectors = new byte[200, 255]; // track, sector

        public Action SectorMapUpdateGUICallback { get; set; }

        public float s1, s2, s3;
        public int WindowWidth { get; set; }

        public SectorMap(RichTextBox richtextbox, FDDProcessing proc)
        {
            rtbSectorMap = richtextbox;
            processing = proc;

        }

        public void RefreshSectorMap()
        {
            int sector, track;


            //resettimems();
            Color fcol, bcol;

            //resettimems();

            fcol = rtbSectorMap.ForeColor;
            bcol = rtbSectorMap.BackColor;

            //tbSectorMap.ForeColor = colBlack;
            rtbSectorMap.BeginUpdate();
            //tbreceived.Append("1 Sectormap: " + relativetime() + "ms\r\n");
            rtbSectorMap.Clear();
            //tbreceived.Append("2 Sectormap: " + relativetime() + "ms\r\n");
            recoveredsectorcount = 0;
            RecoveredSectorWithErrorsCount = 0;
            //SectorInfo.Clear();
            //tbreceived.Append("3 Sectormap: " + relativetime() + "ms\r\n");
            rtbSectorMap.Font = new Font(rtbSectorMap.Font.FontFamily, 9);
            rtbSectorMap.ZoomFactor = 1.0f;
            WindowWidth = 1620;

            if (processing.diskformat == DiskFormat.amigados) //AmigaDOS
            {
                processing.sectorspertrack = 11;
                WindowWidth = 1630;
            }
            else if (processing.diskformat == DiskFormat.diskspare) // Amiga DiskSpare
            {
                processing.sectorspertrack = 12;
                WindowWidth = 1680;
            }
            else if (processing.diskformat == DiskFormat.pcdd) // PC/MSX
            {
                processing.sectorspertrack = 9;
            }
            else if (processing.diskformat == DiskFormat.pchd || processing.diskformat == DiskFormat.pc2m) // PCHD
            {
                processing.sectorspertrack = 18;
                rtbSectorMap.Font = new Font(rtbSectorMap.Font.FontFamily, 6);
                rtbSectorMap.ZoomFactor = 1.1f;
            }
            //tbreceived.Append("Sectormap: " + relativetime() + "ms\r\n");
            for (track = 0; track < 166; track++)
            {
                Application.DoEvents();
                //tbSectorMap.ForeColor = colBlack;
                rtbSectorMap.AppendText("T" + track.ToString("D3") + " ");
                for (sector = 0; sector < processing.sectorspertrack; sector++)
                {
                    int blue = 0;
                    if (sectorokLatestScan[track, sector] >= SectorMapStatus.CrcOk)
                    {
                        blue = 255;
                        fcol = Color.FromArgb(255, 0, 0, blue);
                        bcol = Color.FromArgb(255, 0xf8, 0xf8, 0xf0);
                    }
                    else
                    {
                        fcol = rtbSectorMap.ForeColor;
                        bcol = rtbSectorMap.BackColor;
                    }
                    sectorokLatestScan[track, sector] = SectorMapStatus.empty;

                    if (sectorok[track, sector] == SectorMapStatus.CrcOk) // Sector good
                    {
                        rtbSectorMap.AppendText(".", fcol, bcol);
                        recoveredsectorcount++;
                    }
                    else if (sectorok[track, sector] == SectorMapStatus.HeadOkDataBad) // Sector contains error
                    {
                        rtbSectorMap.AppendText("X", fcol, bcol);
                        RecoveredSectorWithErrorsCount++;
                    }
                    else if (sectorok[track, sector] == SectorMapStatus.BadSectorUnique) // The only sector found, no duplicates
                        rtbSectorMap.AppendText("_");
                    else if (sectorok[track, sector] == SectorMapStatus.DuplicatesFound) // Duplicates found
                        rtbSectorMap.AppendText("+");
                    else if (sectorok[track, sector] == SectorMapStatus.SectorOKButZeroed) // Sector checksum = ok, but sector is zeroed
                    {
                        rtbSectorMap.AppendText("*");
                        recoveredsectorcount++;
                    }
                    else if (sectorok[track, sector] == SectorMapStatus.ErrorCorrected) // Sector error corrected
                    {
                        rtbSectorMap.AppendText("c");
                        recoveredsectorcount++;
                    }
                    else if (sectorok[track, sector] == SectorMapStatus.ErrorCorrected2) // Sector error corrected
                    {
                        rtbSectorMap.AppendText("b");
                        recoveredsectorcount++;
                    }
                    else rtbSectorMap.AppendText("0"); // No Data
                }
                rtbSectorMap.AppendText(" ");
            }
            //tbreceived.Append("Sectormap: " + relativetime() + "ms\r\n");
            rtbSectorMap.AppendText("\r\nLegend: . = OK | 0 = empty | X = Header ok, BAD data | * = OK, sector zeroed | + = Bad header, good sector, recovered | c = error corrected\r\n");
            rtbSectorMap.AppendText("Total Sector counts: PCDOS DD: 1440 | PCDOS HD: 2880 | ADOS: 880K: 1760 | DiskSpare 960KB: 1920 984KB: 1968");

            //tbreceived.Append("Sectormap: " + relativetime() + "ms\r\n");
            // Calculate total found bad sector markers

            int i;

            int total = processing.stat4us + processing.stat6us + processing.stat8us;
            s1 = processing.stat4us / (float)total;
            s2 = processing.stat6us / (float)total;
            s3 = processing.stat8us / (float)total;

            MFMData sectordata;
            badsectorcnt = 0;
            for (i = 0; i < processing.sectordata2.Count; i++)
            {
                sectordata = processing.sectordata2[i];
                if (sectordata.Status == SectorMapStatus.HeadOkDataBad)
                    badsectorcnt++;
            }

            //tbSectorMap.Text = tbtxt.ToString();
            //tbreceived.Append("Sectormap: " + relativetime() + "ms\r\n");

            SectorMapUpdateGUICallback();

            rtbSectorMap.EndUpdate();

            //tbreceived.Append("Sectormap: " + relativetime() + "ms\r\n");
            //rtbSectorMap.Text = SectorInfo.ToString();
            //tbreceived.Append( relativetime() +"ms refreshsectormap\r\n");
        }

        /// <summary>
        /// Highlights the specified sectors with a background color.
        /// This method will highlight sectors in the RichTextBox sectormap display
        /// to visually show which sectors belong to a selected file.
        /// </summary>
        /// <param name="sectors">List of sector locations to highlight.</param>
        /// <param name="backgroundColor">The background color to use for highlighting.</param>
        public void HighlightSectors(List<SectorLocation> sectors, Color backgroundColor)
        {
            if (sectors == null || sectors.Count == 0)
            {
                return;
            }

            rtbSectorMap.BeginUpdate();
            
            try
            {
                foreach (var sector in sectors)
                {
                    // Calculate character index for this sector
                    int charIndex = GetSectorCharIndex(sector.Track, sector.Sector);
                    
                    if (charIndex >= 0 && charIndex < rtbSectorMap.TextLength)
                    {
                        // Select the character at this position
                        rtbSectorMap.SelectionStart = charIndex;
                        rtbSectorMap.SelectionLength = 1;
                        
                        // Apply background color
                        rtbSectorMap.SelectionBackColor = backgroundColor;
                    }
                }
                
                // Reset selection
                rtbSectorMap.SelectionStart = 0;
                rtbSectorMap.SelectionLength = 0;
            }
            finally
            {
                rtbSectorMap.EndUpdate();
            }
        }

        /// <summary>
        /// Clears all sector highlighting, restoring default colors.
        /// This is done by refreshing the entire sectormap.
        /// </summary>
        public void ClearHighlighting()
        {
            // The easiest way to clear highlighting is to refresh the entire sectormap
            // which will redraw all sectors with their original colors
            RefreshSectorMap();
        }

        /// <summary>
        /// Gets the character index in the RichTextBox for a given track/sector.
        /// The sectormap format is: "T000 ......... T001 ......... ..."
        /// Each track has: "Tnnn " (5 chars) + sector characters + space
        /// </summary>
        /// <param name="track">The track number (0-based).</param>
        /// <param name="sector">The sector number within the track (0-based).</param>
        /// <returns>Character index in the RichTextBox, or -1 if invalid.</returns>
        private int GetSectorCharIndex(int track, int sector)
        {
            // Validate inputs
            if (track < 0 || track >= 166 || sector < 0 || sector >= processing.sectorspertrack)
            {
                return -1;
            }

            // Calculate character position
            // Each track block width = 5 (for "Tnnn ") + sectorspertrack + 1 (space)
            int trackBlockWidth = 5 + processing.sectorspertrack + 1;
            
            // Character index = (track * block_width) + 5 (skip "Tnnn ") + sector
            int charIndex = (track * trackBlockWidth) + 5 + sector;
            
            return charIndex;
        }

    } // End class sectormap
}
