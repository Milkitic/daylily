using System;
using System.IO;
using Daylily.Assist.Interface;
using Daylily.Common.IO;

namespace Daylily.CoolQ.Models
{
    public class ImageInfo
    {
        public string Url { get; set; }

        public string Md5 { get; }
        public int Width { get; }
        public int Height { get; }
        public long Size { get; }
        public DateTime Addtime { get; }

        public string Extension
        {
            get
            {
                string newStr = FileInfo.Name.Replace(".cqimg", "");
                int index = newStr.LastIndexOf(".", StringComparison.Ordinal);
                return newStr.Substring(index, newStr.Length - index);
            }
        }

        public FileInfo FileInfo { get; private set; }
        public ImageInfo(string source)
        {
            // TODO 这里还没考虑 file=base64:// 的情况
            const string file = "file=";
            var index = source.IndexOf(file, StringComparison.InvariantCulture) + file.Length;
            var index2 = source.IndexOf("]", index, StringComparison.InvariantCulture);
            var index3 = source.IndexOf(",", index, StringComparison.InvariantCulture);
            if ((index3 < index2) & (index3 != -1))
                index2 = index3;
            string fileName = source.Substring(index, index2 - index) + ".cqimg";
            string fullPath = Path.Combine(CqCode.CqPath, "data", "image", fileName);
            FileInfo = new FileInfo(fullPath);

            string[] settings;
            if (!FileInfo.Exists)
            {
                string tmp = AssistApi.GetImgFile(fileName);
                settings = tmp.Replace("\r", "").Trim('\n').Split('\n');
            }
            else
            {
                settings = ConcurrentFile.ReadAllLines(fullPath);
            }

            foreach (var line in settings)
            {
                string[] result = line.Split('=');
                if (result.Length < 2) continue;

                var key = result[0];
                var value = "";
                for (int i = 1; i < result.Length; i++)
                    value += result[i] + "=";
                value = value.Remove(value.Length - 1);
                switch (key)
                {
                    case "md5":
                        Md5 = value;
                        break;
                    case "width":
                        Width = int.Parse(value);
                        break;
                    case "height":
                        Height = int.Parse(value);
                        break;
                    case "size":
                        Size = long.Parse(value);
                        break;
                    case "url":
                        Url = value;
                        break;
                    case "addtime":
                        Addtime = new DateTime(1970, 1, 1).ToLocalTime().AddSeconds(int.Parse(value));
                        break;
                    default:
                       throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
