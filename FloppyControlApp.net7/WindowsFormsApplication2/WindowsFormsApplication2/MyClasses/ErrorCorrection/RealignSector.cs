using FloppyControlApp.MyClasses.Capture.Models;
using FloppyControlApp.MyClasses.Processing;
using FloppyControlApp.MyClasses.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloppyControlApp.MyClasses.ErrorCorrection
{
	public class RealignSector
	{
		public void ErrorCorrectRealign4E(ListBox BadSectorListBox,
											TextBox textBoxSector,
											int ScatterMin,
											int ScatterMax,
											ref FDDProcessing processing,
											FloppyControl floppycontrol)
		{
			int indexS1, listlength, threadid;
			var selected = BadSectorListBox.SelectedIndices;
			listlength = selected.Count;

			ECSettings ecSettings = new ECSettings();
			ECResult sectorresult;
			ecSettings.sectortextbox = textBoxSector;

			if (ScatterMax - ScatterMin > 50)
			{
				processing.TBReceived.Append("Error: selection can't be larger than 50!\r\n");
				return;
			}

			if (listlength >= 1)
			{
				for (int i = 0; i < listlength; i++)
				{
					if (processing.stop == 1)
						break;
					indexS1 = ((FloppyControl.Badsectorkeyval)BadSectorListBox.Items[selected[i]]).Id;
					threadid = ((FloppyControl.Badsectorkeyval)BadSectorListBox.Items[selected[i]]).Threadid;
					ecSettings.indexS1 = indexS1;
					ecSettings.periodSelectionStart = (int)ScatterMin;
					ecSettings.periodSelectionEnd = (int)ScatterMax;
					ecSettings.threadid = threadid;
					if ((int)processing.diskformat > 2)
					{
						sectorresult = ProcessRealign4E(ecSettings, ref processing);
						if (sectorresult != null)
						{
							floppycontrol.AddRealignedToLists(sectorresult);
						}
					}
					else
					{
						sectorresult = processing.ProcessRealignAmiga(ecSettings);
						if (sectorresult != null)
						{
							floppycontrol.AddRealignedToLists(sectorresult);
						}
					}
				}
			}
			else
			{
				processing.TBReceived.Append("Error, no data selected.");
				return;
			}
		}

		// Some bad sectors only contain a cluster of bad periods
		// These can easily be reprocessed brute force wise if they
		// only cover like 20 periods. 
		public ECResult ProcessRealign4E(  ECSettings ecSettings,
										   ref FDDProcessing processing
										   )
		{
			int i;
			byte[] _4EMarker = { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0 };// 3x 4E
																																												  //byte[] _4489EMarker = { 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1 };// 44894489
																																												  //01000100100010010100010010001001
			int markerindex;
			byte[] crcinsectordatadecoded = new byte[100];
			int crcinsectordata;
			int bitshifted;
			byte[] lasttwosectorbytes = new byte[100];
			int periodSelectionStart, mfmSelectionStart = 0;
			int periodSelectionEnd, mfmSelectionEnd = 0;
			int bytestart, byteend;
			int mfmAlignedStart, mfmAlignedEnd;

			mfmAlignedStart = ecSettings.MFMByteStart;
			mfmAlignedEnd = mfmAlignedStart + (ecSettings.MFMByteLength * 8);

			// User selected part to be brute forced:
			periodSelectionStart = ecSettings.periodSelectionStart;
			periodSelectionEnd = ecSettings.periodSelectionEnd;
			int indexS1 = ecSettings.indexS1;
			int threadid = ecSettings.threadid;

			// Stop if selection is too large, taking too long.
			if (periodSelectionEnd - periodSelectionStart > 50)
			{
				processing.TBReceived.Append("Selection too large, please make it smaller, 50 max.\r\n");
				return null;
			}
			var sectordata2 = processing.sectordata2;
			var TBReceived = processing.TBReceived;
			var mfms = processing.mfms;
			// Copy mfm data from mfms
			int sectorlength = sectordata2[indexS1].sectorlength;

			byte[] mfmbuf = mfms[sectordata2[indexS1].threadid]
							.SubArray(sectordata2[indexS1].MarkerPositions, (sectorlength + 100) * 16);
			byte[] bytebuf = new byte[sectorlength + 6];

			int cntperiods = 0;
			//Find where in the mfm data the periodSelectionStart is
			for (i = 0; i < (sectorlength + 6) * 16; i++)
			{
				if (mfmbuf[i] == 1)
					cntperiods++;
				if (cntperiods == periodSelectionStart) mfmSelectionStart = i;
				if (cntperiods == periodSelectionEnd)
				{
					mfmSelectionEnd = i;
					break;
				}
			}

			TBReceived.Append("Selection: period start:" + periodSelectionStart + " period end: " + periodSelectionEnd + "\r\n");
			TBReceived.Append("mfm start: " + mfmSelectionStart + " mfm end:" + mfmSelectionEnd + "\r\n");

			bytestart = mfmSelectionStart / 16;
			byteend = mfmSelectionEnd / 16;

			//mfmAlignedStart = bytestart * 16;
			//mfmAlignedEnd = (byteend + 1) * 16;

			TBReceived.Append("bytestart: " + bytestart + " byte end: " + byteend + "\r\n");
			TBReceived.Append("mfmAlignedstart: " + mfmAlignedStart + " mfmAlignedEnd: " + mfmAlignedEnd + "\r\n");

			// Find 4E right after the crc bytes at the end of the sector
			// 4E bytes are padding bytes between header and data. 
			// When the 4E markers are found it will increase the chance of 
			// getting a proper crc, even if it's bit shifted caused by corrupt data

			// Is processing.diskformat amiga or pc?
			// The number of bits shifted with regards to where the 4E padding should or expected to be
			markerindex = processing.FindMarker(ref mfmbuf, mfmbuf.Length, (sectorlength + 4) * 16, ref _4EMarker);
			bitshifted = markerindex - ((sectorlength + 4 + 3) * 16)+16;


			//bitshifted = markerindex - ((sectorlength + 7) * 16);

			// Skip processing if bitshift is too large
			if (bitshifted > 32 || bitshifted < -32)
			{
				TBReceived.Append("ScanTemp: i: " + indexS1 + " Bitshift is too large. (" + bitshifted + ")\r\n");
				return null;
			}

			int markeroffset = sectordata2[indexS1].MarkerPositions;
			byte[] mfmsdest = mfms[sectordata2[indexS1].threadid];

			//Copy the bitshift correct mfm data back to the large mfms array
			for (i = mfmAlignedStart; i < mfmbuf.Length; i++)
			{
				mfmsdest[markeroffset + i] = mfmbuf[i + bitshifted];
			}

			// If no 4E markers are found, exit
			if (markerindex == -1 || markerindex == -2)
			{
				TBReceived.Append("ScanTemp: No marker found.\r\n");
				return null;
			}



			if ((int)processing.diskformat > 2) // PC processing
			{
				// get the sector crc
				int crcindex = markerindex - 48;
				for (i = 0; i < 2; i++)
				{
					crcinsectordatadecoded[i] = processing.MFMBits2BINbyte(ref mfmbuf, crcindex + (i * 16));
					//tbreceived.Append(lasttwosectorbytes[i].ToString("X2") );
				}
				crcinsectordata = crcinsectordatadecoded[0] << 8 | crcinsectordatadecoded[1];

				for (i = 0; i < 4; i++)
				{
					lasttwosectorbytes[i] = processing.MFMBits2BINbyte(ref mfmbuf, crcindex + (i * 16) - 32);
					//tbreceived.Append(lasttwosectorbytes[i].ToString("X2"));
				}
				int track = sectordata2[indexS1].trackhead;
				int sector = sectordata2[indexS1].sector;
				//tbreceived.Append("\r\n");
				TBReceived.Append("ProcessRealign4E: T" + track.ToString("d3") + "S" + sector + " Found 4E aligned CRC: i:" + indexS1 + " " + crcinsectordata.ToString("X4") + ". Last two bytes: " + lasttwosectorbytes[0].ToString("X2") +
				 lasttwosectorbytes[1].ToString("X2") + " Bitshifted: " + bitshifted + "\r\n");

				// Convert first part up to mfmAlignedStart
				// Convert it back to binary
				for (i = 0; i < bytestart; i++)
				{
					bytebuf[i] = processing.MFMBits2BINbyte(ref mfmbuf, (i * 16));
				}

				int start = i - 1;
				//int cnt = start+1;
				int mfmoffset = crcindex - ((sectorlength + 4) * 16);

				// Convert last part with realignedment based on the 4E 'marker':
				for (i = start + 1; i < (sectorlength + 6); i++)
				{
					bytebuf[i] = processing.MFMBits2BINbyte(ref mfmbuf, mfmoffset + (i * 16));

				}

				byte databyte;
				StringBuilder bytesstring = new StringBuilder();
				StringBuilder txtstring = new StringBuilder();
				// Convert to text and ascii encoded hex
				for (i = 0; i < (sectorlength + 6); i++)
				{
					databyte = bytebuf[i];
					bytesstring.Append(databyte.ToString("X2"));
					if (databyte > 32 && databyte < 127)
						txtstring.Append((char)databyte);
					else txtstring.Append(".");
					if (i == bytestart || i == byteend) bytesstring.Append(" ");
					if (i % 32 == 31)
					{
						txtstring.Append("\r\n");
						bytesstring.Append("\r\n");
					}
				}
				ecSettings.sectortextbox.Text = txtstring.ToString() + "\r\n\r\n";
				ecSettings.sectortextbox.Text += bytesstring.ToString() + "\r\n";


				// Now we've got the sector sans corrupted data

				// Find the incoming period
				/*for (i = 1; i < 4; i++)
                {
                    if (mfmbuf[mfmAlignedStart - i] == 1)
                        break;
                }*/

				// Add the newly created aligned bad sector to the bad sector list
				// First clone all the data

				int badsectorold = indexS1;
				
				MFMData sectordata = new MFMData
				{
					threadid = threadid,
					MarkerPositions = sectordata2[badsectorold].MarkerPositions,
					rxbufMarkerPositions = sectordata2[badsectorold].rxbufMarkerPositions,
					Status = sectordata2[badsectorold].Status, // 2 = bad sector data
					trackhead = sectordata2[badsectorold].trackhead,
					track = sectordata2[badsectorold].track,
					sector = sectordata2[badsectorold].sector,
					sectorlength = sectordata2[badsectorold].sectorlength,
					crc = sectordata2[badsectorold].crc,
					sectorbytes = bytebuf
				};

				sectordata2.TryAdd(sectordata2.Count, sectordata);
				//sectordata2[badsectorcnt2].sectorbytes = bytebuf;

				//return sectordata;
				ECResult result = new ECResult
				{
					index = sectordata2.Count - 1,
					sectordata = sectordata
				};

				return result;

			} // End PC processing
			return null;
		} // End ProcessRealign4E()
	}
}
