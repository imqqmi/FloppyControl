using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloppyControlApp
{
    public partial class FloppyControl : Form
    {
        private void ConvertToMFMBtn_Click(object sender, EventArgs e)
        {
            int i;
            StringBuilder tbt = new StringBuilder();
            StringBuilder txt = new StringBuilder();

            // Convert string of hex encoded ascii to byte array
            byte[] bytes = FDDProcessing.HexToBytes(tbBIN.Text);

            if (ANPCRadio.Checked)
            {
                // Convert byte array to MFM
                byte[] mfmbytes = processing.BIN2MFMbits(ref bytes, bytes.Count(), 0, false);
                byte[] bytebuf = new byte[tbBIN.Text.Length];

                // Convert mfm to string
                tbMFM.Text = Encoding.ASCII.GetString(processing.BIN2MFMbits(ref bytes, bytes.Count(), 0, true));

                for (i = 0; i < mfmbytes.Length / 16; i++)
                {
                    bytebuf[i] = processing.MFMBits2BINbyte(ref mfmbytes, (i * 16));
                    tbt.Append(bytebuf[i].ToString("X2") + " ");
                    if (bytebuf[i] > ' ' && bytebuf[i] < 127) txt.Append((char)bytebuf[i]);
                    else txt.Append(".");

                }
            }
            else
            if (ANAmigaRadio.Checked)
            {
                byte[] mfmbytes = new byte[bytes.Length * 8];
                int j;

                // Convert byte array to MFM
                for (i = 0; i < bytes.Length; i++)
                {
                    for (j = 0; j < 8; j++)
                    {
                        mfmbytes[i * 8 + j] = (byte)(bytes[i] >> (7 - j) & 1);
                    }
                }
                //byte[] mfmbytes = BIN2MFMbits(ref bytes, bytes.Count(), 0, false);
                byte[] bytebuf = new byte[tbBIN.Text.Length];

                // Convert mfm to string
                tbMFM.Text = Encoding.ASCII.GetString(processing.BIN2MFMbits(ref bytes, bytes.Count(), 0, true));

                bytebuf = processing.AmigaMfmDecodeBytes(mfmbytes, 0, mfmbytes.Length); // This doesn't convert sector properly yet

                // Convert mfm back to bytes
                for (i = 0; i < bytebuf.Length; i++)
                {
                    //bytebuf[i] = MFMBits2BINbyte(ref mfmbytes, (i * 16));
                    tbt.Append(bytebuf[i].ToString("X2") + " ");
                    if (bytebuf[i] > 31 && bytebuf[i] < 127) txt.Append((char)bytebuf[i]);
                    else txt.Append(".");
                    if (i % 16 == 15) tbt.Append("\r\n");
                    if (i % 32 == 31) txt.Append("\r\n");
                }
            }
            else
            if (AmigaMFMRadio.Checked)
            {
                byte[] mfmbytes;

                // Convert bytes to Amiga mfm
                mfmbytes = processing.AmigaMfmEncodeBytes(bytes, 0, bytes.Length);
                byte[] bytebuf = new byte[tbBIN.Text.Length];

                // Convert mfm to string
                tbMFM.Text = Encoding.ASCII.GetString(processing.BIN2MFMbits(ref bytes, bytes.Count(), 0, true));

                bytebuf = processing.AmigaMfmDecodeBytes(mfmbytes, 0, mfmbytes.Length); // This doesn't convert sector properly yet

                // Convert mfm back to bytes
                for (i = 0; i < bytebuf.Length; i++)
                {
                    //bytebuf[i] = MFMBits2BINbyte(ref mfmbytes, (i * 16));
                    tbt.Append(bytebuf[i].ToString("X2") + " ");
                    if (bytebuf[i] > 31 && bytebuf[i] < 127) txt.Append((char)bytebuf[i]);
                    else txt.Append(".");
                    if (i % 16 == 15) tbt.Append("\r\n");
                    if (i % 32 == 31) txt.Append("\r\n");
                }
            }

            tbTest.Clear();
            AntxtBox.Clear();
            tbTest.AppendText(tbt.ToString());
            AntxtBox.AppendText(txt.ToString());
        }

        private void AsciiCrcCheckBtn_Click(object sender, EventArgs e)
        {
            byte[] bytebuf = Encoding.ASCII.GetBytes(tbBIN.Text);

            ushort datacrcchk;
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
            datacrcchk = crc.ComputeChecksum(bytebuf);
            tbTest.AppendText("CRC: " + datacrcchk.ToString("X4") + "\r\n");
        }

        private void button23_Click(object sender, EventArgs e)
        {
            byte[] bytes = FDDProcessing.HexToBytes(tbBIN.Text);
            ushort datacrcchk;
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
            datacrcchk = crc.ComputeChecksum(bytes);
            tbTest.AppendText("CRC: " + datacrcchk.ToString("X4") + "\r\n");
        }

        private void button26_Click(object sender, EventArgs e)
        {
            byte[] bytebuf = Encoding.ASCII.GetBytes(tbBIN.Text);

            ushort datacrcchk;
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero3);
            datacrcchk = crc.ComputeGoodChecksum(bytebuf);
            tbTest.AppendText("CRC: " + datacrcchk.ToString("X4") + "\r\n");
        }

        private void button25_Click(object sender, EventArgs e)
        {
            byte[] bytes = FDDProcessing.HexToBytes(tbBIN.Text);
            ushort datacrcchk;
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero3);
            datacrcchk = crc.ComputeGoodChecksum(bytes);
            tbTest.AppendText("CRC: " + datacrcchk.ToString("X4") + "\r\n");
        }

        private void button20_Click_1(object sender, EventArgs e)
        {
            //int i;
            StringBuilder tbt = new StringBuilder();
            StringBuilder txt = new StringBuilder();

            // Convert string of hex encoded ascii to byte array
            byte[] bytes = FDDProcessing.HexToBytes(tbBIN.Text);

            byte[] mfmbytes;

            // Convert bytes to Amiga mfm
            mfmbytes = processing.AmigaMfmEncodeBytes(bytes, 0, bytes.Length);

            byte[] checksum;

            checksum = processing.AmigaChecksum(mfmbytes, 0, mfmbytes.Length);

            tbt.Append("Checksum:" + checksum[0].ToString("X2") + checksum[1].ToString("X2") + checksum[2].ToString("X2") + checksum[3].ToString("X2"));

            tbTest.Clear();
            AntxtBox.Clear();
            tbTest.AppendText(tbt.ToString());
            AntxtBox.AppendText(txt.ToString());
        }

        // Convert MFM in text to MFM in bytes to decoded bytes to hex
        private void button2_Click(object sender, EventArgs e)
        {
            int i;

            //byte[] bytes = new byte[] { 65, 66, 67, 68 };//Encoding.ASCII.GetBytes(tbBIN.Text);

            //byte[] bytes = HexToBytes(tbBIN.Text);
            //byte[] mfmbytes = BIN2MFMbits(ref bytes, bytes.Count(), 0, false);
            byte[] bytebuf = new byte[tbMFM.Text.Length / 8];

            byte[] mfmbytes = new byte[tbMFM.Text.Length];
            byte[] mfmbytes2 = new byte[tbMFM.Text.Length];

            //tbMFM.Text = Encoding.ASCII.GetString(BIN2MFMbits(ref bytes, bytes.Count(), 0, true));

            mfmbytes = Encoding.ASCII.GetBytes(tbMFM.Text);

            int cnt = 0;

            for (i = 0; i < mfmbytes.Length; i++)
            {
                if (mfmbytes[i] == 48 || mfmbytes[i] == 49)
                    mfmbytes2[cnt++] = (byte)(mfmbytes[i] - 48); // from ascii to byte
            }

            StringBuilder tbt = new StringBuilder();
            StringBuilder txt = new StringBuilder();
            for (i = 0; i < mfmbytes2.Length / 16; i++)
            {
                bytebuf[i] = processing.MFMBits2BINbyte(ref mfmbytes2, (i * 16));
                tbt.Append(bytebuf[i].ToString("X2"));
                if (bytebuf[i] > ' ' && bytebuf[i] < 127) txt.Append((char)bytebuf[i]);
                else txt.Append(".");

            }
            tbTest.Clear();
            AntxtBox.Clear();
            tbTest.AppendText(tbt.ToString());
            AntxtBox.AppendText(txt.ToString());
        }
    } // Class
}
