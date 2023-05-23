using FloppyControlApp.MyClasses;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace FloppyControlApp.MyClasses.Utility
{
    public static class SubArrayHelper
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }

	public enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F, NonZero3 = 0x84CF }

    public class Crc16Ccitt
    {
        const ushort poly = 0x1021;
        ushort[] table = new ushort[256];
        ushort initialValue = 0;
        ushort good_crc = 0xffff;

        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = initialValue;
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)(crc << 8 ^ table[crc >> 8 ^ 0xff & bytes[i]]);
            }

            //crc = (ushort)~crc;
            //data = crc;
            //crc = (ushort)((crc << 8) | (data >> 8 & 0xff));

            return crc;
        }
        // See http://srecord.sourceforge.net/crc16-ccitt.html
        public ushort ComputeGoodChecksum(byte[] bytes)
        {
            good_crc = initialValue;


            for (int i = 0; i < bytes.Length; ++i)
            {
                update_good_crc(bytes[i]);
            }
            augment_message_for_good_crc();
            //crc = (ushort)~crc;
            //data = crc;
            //crc = (ushort)((crc << 8) | (data >> 8 & 0xff));

            return good_crc;

        }
        private void update_good_crc(ushort ch)
        {
            ushort i, v, xor_flag;

            /*
            Align test bit with leftmost bit of the message byte.
            */
            v = 0x80;

            for (i = 0; i < 8; i++)
            {
                if ((good_crc & 0x8000) != 0)
                {
                    xor_flag = 1;
                }
                else
                {
                    xor_flag = 0;
                }
                good_crc = (ushort)(good_crc << 1);

                if ((ch & v) != 0)
                {
                    /*
                    Append next bit of message to end of CRC if it is not zero.
                    The zero bit placed there by the shift above need not be
                    changed if the next bit of the message is zero.
                    */
                    good_crc = (ushort)(good_crc + 1);
                }

                if (xor_flag != 0)
                {
                    good_crc = (ushort)(good_crc ^ poly);
                }

                /*
                Align test bit with next bit of the message byte.
                */
                v = (ushort)(v >> 1);
            }
        }

        private void augment_message_for_good_crc()
        {
            ushort i, xor_flag;

            for (i = 0; i < 16; i++)
            {
                if ((good_crc & 0x8000) != 0)
                {
                    xor_flag = 1;
                }
                else
                {
                    xor_flag = 0;
                }
                good_crc = (ushort)(good_crc << 1);

                if (xor_flag != 0)
                {
                    good_crc = (ushort)(good_crc ^ poly);
                }
            }
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }

        public Crc16Ccitt(InitialCrcValue initialValue)
        {
            this.initialValue = (ushort)initialValue;
            ushort temp, a;
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)(temp << 1 ^ poly);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                table[i] = temp;
            }
        }

    }
}