using System;
using FloppyControlApp;
using FloppyControlApp.MyClasses;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace FloppyControl.Test.net7
{
	[TestClass]
	public class ProcessingTests
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
			FileIO fileio = new();

			FDDProcessing processing = new();
			processing = new FDDProcessing
			{
				Indexrxbuf = 0,
				TBReceived = new System.Text.StringBuilder()
			};
			processing.GetProcSettingsCallback += GetProcSettingsCallback;

			fileio.processing = processing;
			fileio.textBoxFilesLoaded = new TextBox();
			string[] file = new string[1];

			// This will get the current WORKING directory (i.e. \bin\Debug)
			string workingDirectory = Environment.CurrentDirectory;
			// or: Directory.GetCurrentDirectory() gives the same result

			// This will get the current PROJECT directory
			if (!IsNotNull( Directory.GetParent(workingDirectory) ) ) return;

			var projectDirectory = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName;
			
			file[0] = projectDirectory + @"\TestData\A005 Modules5_T000_T159_000.bin";
			fileio.OpenFilesPaths = file;

			fileio.openfiles();


			processing.ProcSettings.NumberOfDups = 1;
			processing.ProcSettings.pattern = 0;
			//tbreceived.Append("Combobox:" + PeriodBeyond8uscomboBox.SelectedIndex + "\r\n");

			processing.ProcSettings.offset = 0;
			processing.ProcSettings.min = 0;
			processing.ProcSettings.four = 66;
			processing.ProcSettings.six = 102;
			processing.ProcSettings.max = 140;
			processing.ProcSettings.SkipPeriodData = false;
			processing.ProcSettings.AutoRefreshSectormap = false;
			processing.ProcSettings.start = 0;
			processing.ProcSettings.end = processing.RxBbuf.Length - 1;
			processing.ProcSettings.finddupes = false;

			//processing.procsettings.platform = platform; // 1 = Amiga
			processing.ProcSettings.UseErrorCorrection = true;
			processing.ProcSettings.OnlyBadSectors = false;
			processing.ProcSettings.AddNoise = false;

			processing.ProcSettings.limittotrack = 0;
			processing.ProcSettings.limittosector = 0;

			processing.ProcSettings.LimitTSOn = false;
			processing.ProcSettings.IgnoreHeaderError = false;

			processing.ProcSettings.rateofchange = 1.0f; // Adapt rate
			processing.ProcSettings.AdaptOffset2 = 1.0f; // Adapt rate 2 (not labelled in gui)
			processing.ProcSettings.rateofchange2 = 1300; // Adapt track

			processing.ProcSettings.addnoiselimitstart = 0;
			processing.ProcSettings.addnoiselimitend =0;

			processing.ProcSettings.addnoiserndamount = 12;

			//Act
			processing.StartProcessing(Platform.PC);

			int sectorcount = 0;
			var sectorok = processing.SectorMap.sectorok;
			//Count good sectors
			for (int t = 0; t < 255; t++)
			{
				for (int s = 0; s < 255; s++)
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
            FileIO fileio = new();

            FDDProcessing processing = new();
            processing = new FDDProcessing
            {
                Indexrxbuf = 0,
                TBReceived = new System.Text.StringBuilder()
            };
            processing.GetProcSettingsCallback += GetProcSettingsCallback;

            fileio.processing = processing;
            fileio.textBoxFilesLoaded = new TextBox();
            string[] file = new string[1];

            // This will get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;
            // or: Directory.GetCurrentDirectory() gives the same result

            // This will get the current PROJECT directory
            var projectDirectory = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName;
            file[0] = projectDirectory + @"\TestData\S009 Premiere_T000_T166_009.bin";
            fileio.OpenFilesPaths = file;

            fileio.openfiles();
            
            processing.ProcSettings.NumberOfDups = 1;
            processing.ProcSettings.pattern = 0;
            //tbreceived.Append("Combobox:" + PeriodBeyond8uscomboBox.SelectedIndex + "\r\n");

            processing.ProcSettings.offset = 0;
            processing.ProcSettings.min = 0;
            processing.ProcSettings.four = 69;
            processing.ProcSettings.six = 106;
            processing.ProcSettings.max = 146;
            processing.ProcSettings.SkipPeriodData = false;
            processing.ProcSettings.AutoRefreshSectormap = false;
            processing.ProcSettings.start = 0;
            processing.ProcSettings.end = processing.RxBbuf.Length - 1;
            processing.ProcSettings.finddupes = false;

            //processing.procsettings.platform = platform; // 1 = Amiga
            processing.ProcSettings.UseErrorCorrection = true;
            processing.ProcSettings.OnlyBadSectors = false;
            processing.ProcSettings.AddNoise = false;

            processing.ProcSettings.limittotrack = 0;
            processing.ProcSettings.limittosector = 0;

            processing.ProcSettings.LimitTSOn = false;
            processing.ProcSettings.IgnoreHeaderError = false;

            processing.ProcSettings.rateofchange = 1.0f; // Adapt rate
            processing.ProcSettings.AdaptOffset2 = 1.0f; // Adapt rate 2 (not labelled in gui)
            processing.ProcSettings.rateofchange2 = 1300; // Adapt track

            processing.ProcSettings.addnoiselimitstart = 0;
            processing.ProcSettings.addnoiselimitend = 0;

            processing.ProcSettings.addnoiserndamount = 12;

            //Act
            processing.StartProcessing(Platform.Amiga);

            int sectorcount = 0;
            var sectorok = processing.SectorMap.sectorok;
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

		private static bool IsNotNull([NotNullWhen(true)] object? obj) => obj != null;
		#endregion
	}
}