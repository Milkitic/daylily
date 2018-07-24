using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daylily.Common.Function;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Utils;

namespace Daylily.Plugin.Core.Command
{
    [Name("黄花菜帮助")]
    [Author("yf_extension")]
    [Version(0, 1, 2, PluginVersion.Beta)]
    [Help("如何使用黄花菜？")]
    [Command("help")]
    public class Help : CommandApp
    {
        private static string _versionInfo;
        [FreeArg]
        public string PluginCommand { get; set; }

        public override void Initialize(string[] args)
        {
            _versionInfo = args[0];
        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            return PluginCommand == null
                ? new CommonMessageResponse(ShowList(), messageObj)
                : new CommonMessageResponse(ShowDetail().Trim('\n').Trim('\r'), messageObj);
        }

        private static string ShowList()
        {
            CommandApp[] plugins = PluginManager.CommandMapStatic.Values.Distinct().ToArray();
            Dictionary<string, string> dictionary = plugins.ToDictionary(item => string.Join(", /", item.Commands),
                item => $"{item.Name}。{string.Join("。", item.Helps)}");

            IEnumerable<KeyValuePair<string, string>> dicSort = from objDic in dictionary orderby objDic.Key select objDic;

            return new FileImage(DrawList(dicSort)).ToString();
        }

        private string ShowDetail()
        {
            if (!PluginManager.CommandMapStatic.Keys.Contains(PluginCommand))
                return "未找到相关资源...";
            CommandApp plugin = PluginManager.CommandMapStatic[PluginCommand];
            Custom custom = new Custom
            {
                Title = plugin.Name,
                Helps = plugin.Helps,
                Author = plugin.Author,
                Version = plugin.Version,
                State = plugin.State,
                Arg = new Dictionary<string, string>(),
                FreeArg = new Dictionary<string, string>()
            };

            var sbArg = new StringBuilder();
            var sbFree = new StringBuilder();

            Type t = plugin.GetType();
            var props = t.GetProperties();

            foreach (var prop in props)
            {
                var info = prop.GetCustomAttributes(false);
                if (info.Length == 0) continue;
                string helpStr = "尚无帮助信息。", argStr = null, freeStr = null, argName = null;
                bool isSwitch = false;
                foreach (var o in info)
                {
                    switch (o)
                    {
                        case ArgAttribute argAttrib:
                            argStr = $"-{argAttrib.Name}";
                            argName = prop.Name;
                            isSwitch = argAttrib.IsSwitch;
                            break;
                        case FreeArgAttribute freeArgAttrib:
                            freeStr = prop.Name;
                            break;
                        case HelpAttribute helpAttrib:
                            helpStr = string.Join(" ", helpAttrib.Helps);
                            break;
                    }
                }

                if (argStr != null)
                {
                    sbArg.Append($" [{argStr}{(isSwitch ? "" : " " + StringUtils.GetUnderLineString(argName))}]");
                    custom.Arg.Add(argStr, helpStr);
                }

                if (freeStr != null)
                {
                    sbFree.Append($" [{StringUtils.GetUnderLineString(freeStr)}]");
                    custom.FreeArg.Add(StringUtils.GetUnderLineString(freeStr), helpStr);
                }
            }

            custom.Usage = $"/{PluginCommand}{sbArg}{sbFree}";
            return new FileImage(DrawDetail(custom)).ToString();
        }

        private static Bitmap DrawList(IEnumerable<KeyValuePair<string, string>> dictionary)
        {
            const string title = "黄花菜Help";
            const string sub = "（输入 \"/help [command]\" 查找某个命令的详细信息。）";

            Point pointTitle = new Point(30, 30);
            Point pointSub = new Point(20, 57);
            const int x = 9, y = 90;
            const int step = 25, offset1 = 25, offset2 = 160;

            Font fontH1 = new Font("等线", 14);
            Font fontH1B = new Font("等线", 14, FontStyle.Bold);
            Font fontH2 = new Font("等线", 12);
            Font fontH3 = new Font("等线", 11);
            Font fontH4 = new Font("等线", 10);

            Size size = MeasureListSize(dictionary, fontH2, x, step, offset2);

            Bitmap bitmap = new Bitmap(size.Width, 80 + size.Height);
            using (Brush brushWhite = new SolidBrush(Color.White))
            using (Brush brushYellow = new SolidBrush(Color.FromArgb(255, 243, 82)))
            using (Brush brushGrey = new SolidBrush(Color.FromArgb(185, 185, 185)))
            using (Brush brushDarkGrey = new SolidBrush(Color.FromArgb(45, 45, 48)))
            using (Brush brushLDarkGrey = new SolidBrush(Color.FromArgb(64, 64, 64)))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.FromArgb(30, 30, 30));

                g.DrawString(_versionInfo, fontH4, brushLDarkGrey, 2, 2);
                g.DrawString(title, fontH1, brushWhite, pointTitle);
                g.DrawString(sub, fontH3, brushGrey, pointSub);

                int i = 0;
                foreach (var item in dictionary)
                {
                    float width = g.MeasureString(item.Value, fontH2).Width;
                    Rectangle rec = new Rectangle(x + offset1 - 3, y + i * step - 3,
                        offset2 - offset1 + (int)width + 6, step - 3);
                    FillRoundRectangle(g, brushDarkGrey, rec, 10);

                    g.DrawString(">", fontH1B, brushWhite, x, y + i * step);
                    g.DrawString(item.Key, fontH2, brushYellow, x + offset1, y + i * step);
                    g.DrawString(item.Value, fontH2, brushWhite, x + offset2, y + i * step);

                    i++;
                }
            }

            fontH1.Dispose();
            fontH1B.Dispose();
            fontH2.Dispose();
            fontH3.Dispose();

            return bitmap;
        }

        private static Bitmap DrawDetail(Custom custom)
        {
            Bitmap bitmap = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(bitmap);
            Font fontH1 = new Font("等线", 14);
            Font fontH1B = new Font("等线", 14, FontStyle.Bold);
            Font fontH2 = new Font("等线", 12);
            Font fontH3 = new Font("等线", 11);
            Font fontH4 = new Font("等线", 10);

            Point pointTitle = new Point(30, 30);
            Point pointHelp = new Point(45, 54);
            string help = string.Join(Environment.NewLine, custom.Helps);
            Size sizeHelp = GetStrSize(g, help, fontH3);

            int topAuthor = 54 + sizeHelp.Height + 3;
            Point pointAuthor = new Point(45, topAuthor);
            Point pointVer = new Point(205, topAuthor);
            Point pointState = new Point(285, topAuthor);
            Size sizeAuthor = GetStrSize(g, custom.Author, fontH4);

            int topUsage = topAuthor + sizeAuthor.Height + 40;
            Point pointUsage = new Point(30, topUsage);
            Size sizeUsage = GetStrSize(g, custom.Usage, fontH3);

            int topUsage2 = topUsage + 22;
            Point pointUsage2 = new Point(45, topUsage2);

            int topArg = topUsage2 + 24;
            Point pointArg = new Point(30, topArg);
            int topArg2 = topArg + 22, leftArg2 = 45;
            if (custom.Arg.Count == 0)
                topArg2 = topArg;
            const int stepArg = 22, offsetArg = 125;
            GetRightBottom(offsetArg, topArg2, stepArg, custom.Arg, g, fontH3, out int rightArg, out int bottomArg);

            int topFree = bottomArg + 2;
            if (custom.FreeArg.Count == 0)
                topFree = bottomArg;
            Point pointFree = new Point(30, topFree);
            int topFree2 = topFree + 22, leftFree2 = 45;
            if (custom.FreeArg.Count == 0)
                topFree2 = topFree;
            const int stepFree = 22, offsetFree = 125;
            GetRightBottom(offsetFree, topFree2, stepFree, custom.FreeArg, g, fontH3, out int rightFree, out int bottomFree);


            List<int> rightList = new List<int>
            {
                pointHelp.X + sizeHelp.Width + 20,
                360,
                rightArg + 70,
                rightFree + 70,
                pointUsage.X + sizeUsage.Width + 40
            };
            Size size = new Size(rightList.Max(), bottomFree + 20);

            bitmap = new Bitmap(size.Width, size.Height);
            g = Graphics.FromImage(bitmap);
            using (Brush brushWhite = new SolidBrush(Color.White))
            using (Brush brushYellow = new SolidBrush(Color.FromArgb(255, 243, 82)))
            using (Brush brushGrey = new SolidBrush(Color.FromArgb(185, 185, 185)))
            using (Brush brushLDarkGrey = new SolidBrush(Color.FromArgb(64, 64, 64)))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.FromArgb(30, 30, 30));

                g.DrawString(_versionInfo, fontH4, brushLDarkGrey, 2, 2);
                g.DrawString(custom.Title, fontH1, brushWhite, pointTitle);
                g.DrawString(help, fontH3, brushGrey, pointHelp);
                g.DrawString($"作者：{custom.Author}", fontH4, brushWhite, pointAuthor);
                g.DrawString($"版本：{custom.Version}", fontH4, brushWhite, pointVer);
                g.DrawString(custom.State.ToString(), fontH4, brushWhite, pointState);
                g.DrawString("用法：", fontH2, brushWhite, pointUsage);
                g.DrawString(custom.Usage, fontH3, brushGrey, pointUsage2);
                if (custom.Arg.Count != 0) g.DrawString("选项：", fontH2, brushWhite, pointArg);
                int i = 0;
                foreach (var arg in custom.Arg)
                {
                    g.DrawString(arg.Key, fontH3, brushYellow, leftArg2, topArg2 + stepArg * i);
                    g.DrawString(arg.Value, fontH3, brushGrey, leftArg2 + offsetArg, topArg2 + stepArg * i);
                    i++;
                }
                if (custom.FreeArg.Count != 0) g.DrawString("自由参数：", fontH2, brushWhite, pointFree);
                i = 0;
                foreach (var free in custom.FreeArg)
                {
                    g.DrawString(free.Key, fontH3, brushYellow, leftFree2, topFree2 + stepFree * i);
                    g.DrawString(free.Value, fontH3, brushGrey, leftFree2 + offsetFree, topFree2 + stepFree * i);
                    i++;
                }
            }

            fontH1.Dispose();
            fontH1B.Dispose();
            fontH2.Dispose();
            fontH3.Dispose();
            g.Dispose();

            return bitmap;
        }

        private static void GetRightBottom(int offsetDisp, int top, int step, Dictionary<string, string> keyValues, Graphics g,
            Font f, out int right, out int bottom)
        {
            bottom = top + step * keyValues.Count;
            right = 0;
            foreach (var item in keyValues)
            {
                Size size = GetStrSize(g, item.Value, f);
                if (offsetDisp + size.Width > right) right = offsetDisp + size.Width;
            }
        }

        private static Size GetStrSize(Graphics g, string str, Font font)
        {
            var ok = g.MeasureString(str, font);
            return new Size((int)ok.Width, (int)ok.Height);
        }

        private static Size MeasureListSize(IEnumerable<KeyValuePair<string, string>> dictionary, Font fontH2, int x, int step, int offset2)
        {
            int maxW = 0, maxH = 0;
            using (Bitmap bitmap = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                foreach (var t in dictionary)
                {
                    var item = t.Value;
                    SizeF sInst = g.MeasureString(item, fontH2);
                    float width = x + offset2 + sInst.Width + x;
                    if (maxW < width) maxW = (int)width;
                    maxH += step;
                }
            }

            return new Size(maxW, maxH + 20);
        }

        private static void DrawRoundRectangle(Graphics g, Pen pen, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.DrawPath(pen, path);
            }
        }

        private static void FillRoundRectangle(Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.FillPath(brush, path);
            }
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }

        private class Custom
        {
            public string Title { get; set; }
            public string[] Helps { get; set; }
            public string Author { get; set; }
            public string Version { get; set; }
            public PluginVersion State { get; set; }
            public string Usage { get; set; }
            public Dictionary<string, string> Arg { get; set; }
            public Dictionary<string, string> FreeArg { get; set; }

        }
    }
}
