using Daylily.Common.Models.CQCode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Daylily.Common.Assist
{
    public class CqCode
    {
        public static string CqRoot { get; set; }

        // Encode
        public static string EncodeAt(string id)
        {
            return $"[CQ:at,qq={Escape(id)}]";
        }
        public static string EncodeImageToBase64(Image img)
        {
            //string code = ToBase64(img);
            //Bitmap a = new Bitmap(ToImage(code));
            string path = Path.Combine(Environment.CurrentDirectory, "images", Guid.NewGuid() + ".png");
            img.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            return $"[CQ:image,file=base64://{EncodeFileToBase64(path, false)}]";
        }

        public static string EncodeFileToBase64(string path, bool abc = true)
        {
            FileStream filestream = new FileStream(path, FileMode.Open);
            byte[] bt = new byte[filestream.Length];

            filestream.Read(bt, 0, bt.Length);
            string base64Str = Convert.ToBase64String(bt);
            filestream.Close();
            if (abc)
                return $"[CQ:image,file=base64://{base64Str}]";
            return base64Str;
        }

        // decode
        public static string Decode(string source)
        {
            source = DecodeImageToText(source);
            return source;
        }

        private static string DecodeImageToText(string source)
        {
            StringBuilder sb = new StringBuilder(source);
            int index = 0;
            string str1 = "[CQ:image,";
            while ((index = sb.ToString().IndexOf(str1, index, StringComparison.Ordinal)) != -1)
            {
                int length = source.IndexOf("]", index, StringComparison.Ordinal) - index + 1;
                sb.Remove(index, length);
                sb.Insert(index, "[图片]");
            }
            return sb.ToString();
        }

        // Get

        /// <summary>
        /// 返回一个string数组表示被at的qq号
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string[] GetAt(string source)
        {
            List<string> infoList = new List<string>();
            int index, index2 = 0;
            const string str1 = "[CQ:at,qq=";

            while ((index = source.IndexOf(str1, index2, StringComparison.Ordinal) + str1.Length) != -1 + str1.Length)
            {
                int length = source.IndexOf("]", index, StringComparison.Ordinal) - index;
                infoList.Add(source.Substring(index, length));
                index2 = index + length;
            }
            return infoList.Count == 0 ? null : infoList.ToArray();
        }

        public static ImageInfo[] GetImageInfo(string source)
        {
            List<ImageInfo> infoList = new List<ImageInfo>();
            int index, index2 = 0;
            while ((index = source.IndexOf("[CQ:image", index2, StringComparison.Ordinal)) != -1)
            {
                int length = source.IndexOf("]", index, StringComparison.Ordinal) - index;
                infoList.Add(new ImageInfo(source.Substring(index, length + 1)));
                index2 = index + length;
            }
            return infoList.Count == 0 ? null : infoList.ToArray();
        }

        public static string[] GetImageUrls(string source)
        {
            List<string> urlList = new List<string>();
            int index, index2 = 0;
            while ((index = source.IndexOf("[CQ:image", index2, StringComparison.Ordinal)) != -1)
            {
                if (source.IndexOf(".gif", index, StringComparison.Ordinal) != -1)
                {
                    index2 = index2 + source.IndexOf(".gif", index, StringComparison.Ordinal);
                    continue;
                }
                int tmpIndex = source.IndexOf("url=", index, StringComparison.Ordinal) + 4;
                int length = source.IndexOf("]", tmpIndex, StringComparison.Ordinal) - tmpIndex;
                urlList.Add(source.Substring(tmpIndex, length));
                index2 = tmpIndex + length;
            }
            return urlList.Count == 0 ? null : urlList.ToArray();
        }

        private static string Escape(string text)
        {
            return text.Replace("&", "&amp").Replace("[", "&#91").Replace("]", "&#93").Replace(",", "&#44");
        }

        private static string ToBase64(Image img)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            binFormatter.Serialize(memStream, img);
            byte[] bytes = memStream.GetBuffer();
            return Convert.ToBase64String(bytes);
        }

        private static Image ToImage(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream memStream = new MemoryStream(bytes);
            BinaryFormatter binFormatter = new BinaryFormatter();
            return (Image)binFormatter.Deserialize(memStream);
        }
    }
}
