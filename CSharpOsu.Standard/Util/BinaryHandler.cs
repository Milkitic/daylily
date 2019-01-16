using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CSharpOsu.Standard.Util
{
    internal class BinHandler
    {
        /// <summary>
        /// Binary String
        /// </summary>
        /// <param name="binWriter">The main BinaryWriter.</param>
        /// <param name="str">A string.</param>
        public void writeString(BinaryWriter binWriter, string str)
        {
            binWriter.Write((byte)0x0B);
            binWriter.Write(str);
        }

        /// <summary>
        /// Binary Byte
        /// </summary>
        /// <param name="binWriter">The main BinaryWriter.</param>
        /// <param name="bt">A string.</param>
        public void writeByte(BinaryWriter binWriter, string bt)
        {
            binWriter.Write(byte.Parse(bt));
        }

        /// <summary>
        /// Binary Short
        /// </summary>
        /// <param name="binWriter">The main BinaryWriter.</param>
        /// <param name="srt">A string.</param>
        public void writeShort(BinaryWriter binWriter, string srt)
        {
            binWriter.Write(Convert.ToUInt16(srt));
        }

        /// <summary>
        /// Binary Int
        /// </summary>
        /// <param name="binWriter">The main BinaryWriter.</param>
        /// <param name="i">A string.</param>
        public void writeInteger(BinaryWriter binWriter, string i)
        {
            binWriter.Write(Convert.ToUInt32(i));
        }

        /// <summary>
        /// Binary Database Timestamp
        /// </summary>
        /// <param name="binWriter">The main BinaryWriter.</param>
        /// <param name="datestr">A date parsed as a string.</param>
        public void writeDate(BinaryWriter binWriter, string datestr)
        {
            // Weird string and timestamp manipulation. I have no clue how it works.
            // The credits for this one goes to omkelderman(https://github.com/omkelderman/osu-replay-downloader).

            var constant = 429.4967296;
            DateTime date = DateTime.Parse(datestr.Replace(' ', 'T') + "+08:00");

            var temp1 = (GetTime(date) / 1000) + 62135596800;
            var temp2 = temp1 / constant;
            var high = Math.Round(temp2);
            var low = (temp2 - high) * constant * 10000000;
            byte[] toBytes2 = BitConverter.GetBytes(Convert.ToInt32(high));
            byte[] toBytes1 = BitConverter.GetBytes(Convert.ToInt32(low));


            binWriter.Write(toBytes1, 0, 4);
            binWriter.Write(toBytes2, 0, 4);
        }

        /// <summary>
        /// Unix Timestamp
        /// </summary>
        /// <param name="date">A date from DateTime.</param>
        /// <returns></returns>
        private Int64 GetTime(DateTime date)
        {
            Int64 retval = 0;
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (date.ToUniversalTime() - st);
            retval = (Int64)(t.TotalMilliseconds);
            return retval;
        }

        /// <summary>
        /// Calculate MD5Hash
        /// </summary>
        /// <param name="input">Decoded MD5 string.</param>
        /// <returns></returns>
        public string MD5Hash(string input)

        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            StringBuilder sb = new StringBuilder();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            for (int i = 0; i < hash.Length; i++)
            { sb.Append(hash[i].ToString("X2")); }

            return sb.ToString().ToLower();

        }
    }
}
