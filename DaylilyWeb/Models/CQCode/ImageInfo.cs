using DaylilyWeb.Interface.DaylilyAssist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Models.CQCode
{
    public class ImageInfo
    {
        public string Url { get; set; }

        public string Md5 { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public long Size { get; private set; }
        public DateTime Addtime { get; private set; }

        public string Extension
        {
            get
            {
                var new_str = FileInfo.Name.Replace(".cqimg", "");
                int index = new_str.LastIndexOf(".");
                return new_str.Substring(index, new_str.Length - index);
            }
        }

        public FileInfo FileInfo { get; private set; }
        public ImageInfo(string source)
        {
            const string FILE = "file=";
            int index, index2;
            index = source.IndexOf(FILE) + FILE.Length;
            index2 = source.IndexOf(",", index);
            string file_name = source.Substring(index, index2 - index) + ".cqimg";
            string file = Path.Combine(Assist.CQCode.CQRoot, "data", "image", file_name);
            FileInfo = new FileInfo(file);

            string[] settings = new string[0];
            if (!FileInfo.Exists)
            {
                string tmp = AssistApi.GetImgFile(file_name);
                settings = tmp.Replace("\r", "").Trim('\n').Split('\n');
            }
            else
            {
                settings = File.ReadAllLines(file);
            }

            foreach (var line in settings)
            {
                string key, value;
                string[] result = line.Split('=');
                if (result.Length >= 2)
                {
                    key = result[0];
                    value = "";
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
                    }
                }
            }
        }
    }
}
