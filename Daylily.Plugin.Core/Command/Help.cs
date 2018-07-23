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
    [Version(0, 0, 1, PluginVersion.Beta)]
    [Help("如何使用黄花菜？")]
    [Command("help")]
    public class Help : CommandApp
    {
        [FreeArg]
        public string PluginCommand { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            //SendMessage(
            //    PluginCommand == null
            //        ? new CommonMessageResponse(ShowList().Trim('\n').Trim('\r'), messageObj.UserId)
            //        : new CommonMessageResponse(ShowDetail().Trim('\n').Trim('\r'), messageObj.UserId), null, null,
            //    MessageType.Private);

            //return messageObj.MessageType != MessageType.Private
            //    ? new CommonMessageResponse(LoliReply.PrivateOnly, messageObj)
            //    : null;
            return PluginCommand == null
                ? new CommonMessageResponse(ShowList(), messageObj)
                : new CommonMessageResponse(ShowDetail().Trim('\n').Trim('\r'), messageObj);
        }

        private static string ShowList()
        {
            CommandApp[] plugins = PluginManager.CommandMapStatic.Values.Distinct().ToArray();
            //var cmdList = new List<string>();
            //var instList = new List<string>();
            Dictionary<string, string> dictionary = plugins.ToDictionary(item => string.Join(", /", item.Commands),
                item => $"{item.Name}。{string.Join("。", item.Helps)}");

            IEnumerable<KeyValuePair<string, string>> dicSort = from objDic in dictionary orderby objDic.Key select objDic;
            //cmdList.Add(string.Join(", /", item.Commands));
            //instList.Add($"{item.Name}。{string.Join("。", item.Helps)}");


            return new FileImage(Draw(dicSort)).ToString();
        }

        private static Bitmap Draw(IEnumerable<KeyValuePair<string, string>> dictionary)
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

            Size size = MeasureSize(dictionary, fontH2, x, step, offset2);

            Bitmap bitmap = new Bitmap(size.Width, 80 + size.Height);
            using (Brush brushWhite = new SolidBrush(Color.White))
            using (Brush brushYellow = new SolidBrush(Color.FromArgb(255, 243, 82)))
            using (Brush brushGrey = new SolidBrush(Color.FromArgb(185, 185, 185)))
            using (Brush brushDarkGrey = new SolidBrush(Color.FromArgb(45, 45, 48)))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.FromArgb(30, 30, 30));
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

        private string ShowDetail()
        {
            if (!PluginManager.CommandMapStatic.Keys.Contains(PluginCommand))
                return "未找到相关资源...";

            CommandApp plugin = PluginManager.CommandMapStatic[PluginCommand];
            var sb = new StringBuilder();
            var sbArg = new StringBuilder();
            var sbFree = new StringBuilder();

            sb.AppendLine($"“{plugin.Name}”的帮助：");
            sb.AppendLine($"作者：{plugin.Author}");
            sb.AppendLine($"版本：{plugin.Version} {plugin.State.ToString()}");
            sb.AppendLine($"帮助说明：\r\n  {string.Join("\r\n  ", plugin.Helps)}");
            Type t = plugin.GetType();
            var props = t.GetProperties();

            bool used = false;
            foreach (var prop in props)
            {
                var info = prop.GetCustomAttributes(false);
                if (info.Length == 0) continue;
                string helpStr = "尚无帮助信息。", argStr = null, freeStr = null;
                foreach (var o in info)
                {
                    switch (o)
                    {
                        case ArgAttribute argAttrib:
                            argStr = $"-{argAttrib.Name}";
                            break;
                        case FreeArgAttribute freeArgAttrib:
                            freeStr = $"自由参数 ({prop.Name})";
                            break;
                        case HelpAttribute helpAttrib:
                            helpStr = string.Join("\r\n", helpAttrib.Helps);
                            break;
                    }
                }

                if (argStr != null)
                {
                    if (!used)
                    {
                        sbArg.AppendLine("参数说明：");
                        used = true;
                    }

                    sbArg.AppendLine($"  {argStr}: {helpStr}");
                }

                if (freeStr != null)
                {
                    if (!used)
                    {
                        sbFree.AppendLine("参数说明：");
                        used = true;
                    }

                    sbFree.AppendLine($"  {freeStr}: {helpStr}");
                }
            }

            return sb.ToString() + sbArg + sbFree;

        }

        private static Size MeasureSize(IEnumerable<KeyValuePair<string, string>> dictionary, Font fontH2, int x, int step, int offset2)
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
    }
}
