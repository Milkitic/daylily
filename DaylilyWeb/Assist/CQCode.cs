using DaylilyWeb.Models.CQCode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DaylilyWeb.Assist
{
    public class CQCode
    {
        public static string CQRoot { get; set; }

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
            string base64Str;

            filestream.Read(bt, 0, bt.Length);
            base64Str = Convert.ToBase64String(bt);
            filestream.Close();
            if (abc)
                return $"[CQ:image,file=base64://{base64Str}]";
            return base64Str;
        }

        // decode
        public static string DecodeImageToText(string source)
        {
            StringBuilder sb = new StringBuilder(source);
            int index = 0;
            string str1 = "[CQ:image,";
            while ((index = sb.ToString().IndexOf(str1, index)) != -1)
            {
                int length = source.IndexOf("]", index) - index + 1;
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
            List<string> info_list = new List<string>();
            int index, index2 = 0;
            string str1 = "[CQ:at,qq=";
            while ((index = source.IndexOf(str1, index2) + str1.Length) != -1 + str1.Length)
            {
                int length = source.IndexOf("]", index) - index;
                info_list.Add(source.Substring(index, length));
                index2 = index + length;
            }
            if (info_list.Count == 0)
                return null;
            return info_list.ToArray();
        }

        public static ImageInfo[] GetImageInfo(string source)
        {
            List<ImageInfo> info_list = new List<ImageInfo>();
            int index, index2 = 0;
            while ((index = source.IndexOf("[CQ:image", index2)) != -1)
            {
                int length = source.IndexOf("]", index) - index;
                info_list.Add(new ImageInfo(source.Substring(index, length + 1)));
                index2 = index + length;
            }
            if (info_list.Count == 0)
                return null;
            return info_list.ToArray();
        }

        public static string[] GetImageUrls(string source)
        {
            List<string> url_list = new List<string>();
            int index, index2 = 0;
            while ((index = source.IndexOf("[CQ:image", index2)) != -1)
            {
                if (source.IndexOf(".gif", index) != -1)
                {
                    index2 = index2 + source.IndexOf(".gif", index);
                    continue;
                }
                int tmp_index = source.IndexOf("url=", index) + 4;
                int length = source.IndexOf("]", tmp_index) - tmp_index;
                url_list.Add(source.Substring(tmp_index, length));
                index2 = tmp_index + length;
            }
            if (url_list.Count == 0)
                return null;
            return url_list.ToArray();
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
