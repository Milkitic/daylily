using DaylilyWeb.Models.CQCode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace DaylilyWeb.Assist
{
    public class CQCode
    {
        public static string CQRoot { get; set; }

        public static string EncodeAt(string id)
        {
            return $"[CQ:at,qq={Escape(id)}]";
        }
        public static string EncodeImageToBase64(Image img)
        {
            string code = ToBase64(img);
            Bitmap a = new Bitmap(ToImage(code));
            string path = Path.Combine(Environment.CurrentDirectory, "images", Guid.NewGuid() + ".png");
            a.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            return $"[CQ:image,file=file://{Escape(path)}]";
        }

        public static string Escape(string text)
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
    }
}
