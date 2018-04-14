using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Daylily.Assist
{
    public class CQCode
    {
        public static string EncodeAt(string id)
        {
            return $"[CQ:at,qq={Escape(id)}]";
        }
        public static string EncodeImageToFile(Bitmap bmp)
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "images")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "images"));
            string path = Path.Combine(Environment.CurrentDirectory, "images", Guid.NewGuid() + ".png");
            bmp.Save(path, ImageFormat.Png);
            return $"[CQ:image,file=file://{Escape(path)}]";
        }
        public static string EncodeImageToBase64(Bitmap bmp)
        {
            var guid = Guid.NewGuid();
            string png_file = guid + ".png";
            string tmp_file = guid + ".txt";

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "images")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "images"));
            string path = Path.Combine(Environment.CurrentDirectory, "images", png_file);
            bmp.Save(path, ImageFormat.Png);

            string base64Str = EncodeFileToBase64(path);

            return $"[CQ:image,file=base64://{base64Str}]";
        }

        private static string EncodeFileToBase64(string path)
        {
            FileStream filestream = new FileStream(path, FileMode.Open);
            byte[] bt = new byte[filestream.Length];
            string base64Str;

            filestream.Read(bt, 0, bt.Length);
            base64Str = Convert.ToBase64String(bt);
            filestream.Close();
            return base64Str;
        }

        private static void EncodeBase64ToFile(string base64Str, string outPath)
        {
            var contents = Convert.FromBase64String(base64Str);
            using (var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(contents, 0, contents.Length);
                fs.Flush();
            }
        }

        public static string Escape(string text)
        {
            return text.Replace("&", "&amp").Replace("[", "&#91").Replace("]", "&#93").Replace(",", "&#44");
        }

        private static string ImageToBase64(Image img)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            binFormatter.Serialize(memStream, img);
            byte[] bytes = memStream.GetBuffer();
            return Convert.ToBase64String(bytes);
        }

        private static Image Base64ToImage(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream memStream = new MemoryStream(bytes);
            BinaryFormatter binFormatter = new BinaryFormatter();
            return (Image)binFormatter.Deserialize(memStream);
        }
    }
}
