using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FloppyControlApp;
using FloppyControlApp.MyClasses;
using System.IO;
using System.Windows.Forms;

namespace FloppyControl.Test
{
    [TestClass]
    public class FDDProcessingTests
    {
        /// <summary>
        /// Tests:
        /// - Opening a bin file
        /// - Processing period data to mfm
        /// - Convert mfm data to sectors
        /// </summary>
        [TestMethod]
        public void TestProcessPCData()
        {
            //Arrange
            FileIO fileio = new FileIO();
            
            FDDProcessing processing = new FDDProcessing();
            processing = new FDDProcessing();
            processing.indexrxbuf = 0;
            processing.tbreceived = new System.Text.StringBuilder();
            processing.GetProcSettingsCallback += GetProcSettingsCallback;

            fileio.processing = processing;
            fileio.textBoxFilesLoaded = new TextBox();
            string[] file = new string[1];
            
            // This will get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;
            // or: Directory.GetCurrentDirectory() gives the same result

            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            file[0] = projectDirectory+@"\TestData\A005 Modules5_T000_T159_000.bin";
            fileio.OpenFilesPaths = file;

            fileio.openfiles();

            
            processing.procsettings.NumberOfDups = 1;
            processing.procsettings.pattern = 0;
            //tbreceived.Append("Combobox:" + PeriodBeyond8uscomboBox.SelectedIndex + "\r\n");

            processing.procsettings.offset = 0;
            processing.procsettings.min = 0;
            processing.procsettings.four = 66;
            processing.procsettings.six = 102;
            processing.procsettings.max = 140;
            processing.procsettings.SkipPeriodData = false;
            processing.procsettings.AutoRefreshSectormap = false;
            processing.procsettings.start = 0;
            processing.procsettings.end = processing.rxbuf.Length-1;
            processing.procsettings.finddupes = true;
            
            //processing.procsettings.platform = platform; // 1 = Amiga
            processing.procsettings.UseErrorCorrection = true;
            processing.procsettings.OnlyBadSectors = false;
            processing.procsettings.AddNoise = false;

            processing.procsettings.limittotrack = 0;
            processing.procsettings.limittosector = 0;
            
            processing.procsettings.LimitTSOn = false;
            processing.procsettings.IgnoreHeaderError = false;

            processing.procsettings.rateofchange = 1.0f; // Adapt rate
            processing.procsettings.AdaptOffset2 = 1.0f; // Adapt rate 2 (not labelled in gui)
            processing.procsettings.rateofchange2 = 128; // Adapt track

            processing.procsettings.addnoiselimitstart = 0;
            processing.procsettings.addnoiselimitend = processing.indexrxbuf;
            
            processing.procsettings.addnoiserndamount = 12;

            //Act
            processing.StartProcessing(Platform.PC);

            int sectorcount = 0;
            var sectorok = processing.sectormap.sectorok;
            //Count good sectors
            for ( int t = 0; t < 255; t++)
            {
                for(int s = 0; s < 255; s++)
                {
                    if (sectorok[t, s] == SectorMapStatus.CrcOk)
                        sectorcount++;
                }
            }

            //Assert
            Assert.AreEqual(1440, sectorcount);

        }

        /// <summary>
        /// Tests:
        /// - Opening a bin file
        /// - Processing period data to mfm
        /// - Convert mfm data to sectors
        /// </summary>
        [TestMethod]
        public void TestProcessAmigaData()
        {
            //Arrange
            FileIO fileio = new FileIO();

            FDDProcessing processing = new FDDProcessing();
            processing = new FDDProcessing();
            processing.indexrxbuf = 0;
            processing.tbreceived = new System.Text.StringBuilder();
            processing.GetProcSettingsCallback += GetProcSettingsCallback;

            fileio.processing = processing;
            fileio.textBoxFilesLoaded = new TextBox();
            string[] file = new string[1];

            // This will get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;
            // or: Directory.GetCurrentDirectory() gives the same result

            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            file[0] = projectDirectory + @"\TestData\S009 Premiere_T000_T166_009.bin";
            fileio.OpenFilesPaths = file;

            fileio.openfiles();
            
            processing.procsettings.NumberOfDups = 1;
            processing.procsettings.pattern = 0;
            //tbreceived.Append("Combobox:" + PeriodBeyond8uscomboBox.SelectedIndex + "\r\n");

            processing.procsettings.offset = 0;
            processing.procsettings.min = 0;
            processing.procsettings.four = 73;
            processing.procsettings.six = 109;
            processing.procsettings.max = 147;
            processing.procsettings.SkipPeriodData = false;
            processing.procsettings.AutoRefreshSectormap = false;
            processing.procsettings.start = 0;
            processing.procsettings.end = processing.rxbuf.Length - 1;
            processing.procsettings.finddupes = true;

            //processing.procsettings.platform = platform; // 1 = Amiga
            processing.procsettings.UseErrorCorrection = true;
            processing.procsettings.OnlyBadSectors = false;
            processing.procsettings.AddNoise = false;

            processing.procsettings.limittotrack = 0;
            processing.procsettings.limittosector = 0;

            processing.procsettings.LimitTSOn = false;
            processing.procsettings.IgnoreHeaderError = false;

            processing.procsettings.rateofchange = 1.0f; // Adapt rate
            processing.procsettings.AdaptOffset2 = 1.0f; // Adapt rate 2 (not labelled in gui)
            processing.procsettings.rateofchange2 = 128; // Adapt track

            processing.procsettings.addnoiselimitstart = 0;
            processing.procsettings.addnoiselimitend = processing.indexrxbuf;

            processing.procsettings.addnoiserndamount = 12;

            //Act
            processing.StartProcessing(Platform.Amiga);

            int sectorcount = 0;
            var sectorok = processing.sectormap.sectorok;
            //Count good sectors
            for (int t = 0; t < 255; t++)
            {
                for (int s = 0; s < 255; s++)
                {
                    if (sectorok[t, s] == SectorMapStatus.CrcOk || sectorok[t, s] == SectorMapStatus.SectorOKButZeroed)
                        sectorcount++;
                }
            }

            //Assert
            Assert.AreEqual(1760, sectorcount);

        }

#region helpers
        private void GetProcSettingsCallback()
        {
            return;
        }
    }
#endregion
}
