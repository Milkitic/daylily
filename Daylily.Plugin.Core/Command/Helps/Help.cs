using Daylily.Bot;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Bot.Sessions;
using Daylily.Common;
using Daylily.Common.IO;
using Daylily.Common.Utils.StringUtils;
using Daylily.CoolQ;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;

namespace Daylily.Plugin.Core
{
    [Name("黄花菜帮助")]
    [Author("yf_extension")]
    [Version(0, 1, 2, PluginVersion.Beta)]
    [Help("查看此帮助信息。")]
    [Command("help")]
    public class Help : CommandPlugin
    {
        [FreeArg]
        public string CommandName { get; set; }

        [Arg("list", Abbreviate = 'l', Default = false, IsSwitch = true)]
        public bool UseList { get; set; }

        private static string _versionInfo;
        private CommonMessage _cm;
        private static readonly string HelpDir = Path.Combine(Domain.ResourcePath, "help");
        private static readonly string StaticDir = Path.Combine(HelpDir, "static");

        public override void OnInitialized(string[] args)
        {
            _versionInfo = args[0];
        }

        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {
            _cm = messageObj;
            if (UseList)
                return new CommonMessageResponse(ShowList(), _cm);
            if (CommandName == null)
            {
                using (Session session = new Session(20000, _cm.Identity, _cm.UserId))
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>
                    {
                        {"你是谁", "你是谁。"},
                        {"基础帮助", "查看通用的基础使用方法。(/help -list)"},
                        {"查列表", "查看所有可用的命令和应用列表。"}
                    };

                    string[] sb = dic.Select(k => $"【{k.Key}】 {k.Value}").ToArray();

                    string msg = "被召唤啦！请选择你要查看的帮助类型：\r\n" + string.Join("\r\n", sb);
                    SendMessage(new CommonMessageResponse(msg, _cm));
                    try
                    {
                        var a = dic.Select(k => k.Key).ToArray();

                        CommonMessage cm;
                        do
                        {
                            cm = session.GetMessage();
                            if (cm.RawMessage.Contains("你是谁"))
                                return new CommonMessageResponse(
                                    new FileImage(Path.Combine(StaticDir, "help.jpg")).ToString(), _cm);
                            if (cm.RawMessage.Contains("基础帮助"))
                            {
                                if (cm.MessageType == MessageType.Private)
                                    return new CommonMessageResponse(
                                        ConcurrentFile.ReadAllText(Path.Combine(StaticDir, "common.txt")), _cm);
                                SendMessage(new CommonMessageResponse("已发送至私聊，请查看。", _cm, true));
                                SendMessage(new CommonMessageResponse(
                                    ConcurrentFile.ReadAllText(Path.Combine(StaticDir, "common.txt")),
                                    new Identity(_cm.UserId, MessageType.Private)));
                                return null;
                            }

                            if (cm.RawMessage.Contains("查列表"))
                                return new CommonMessageResponse(ShowList(), _cm);
                            SendMessage(new CommonMessageResponse("请回复大括号内的文字。", _cm));

                        } while (!a.Contains(cm.RawMessage));

                        return new CommonMessageResponse(ShowList(), _cm);
                    }
                    catch (TimeoutException)
                    {
                        return new CommonMessageResponse("没人鸟我，走了.jpg", _cm);
                    }

                }
            }
            else
                return new CommonMessageResponse(ShowDetail().Trim('\n').Trim('\r'), _cm);
        }

        private string ShowList()
        {
            CommandPlugin[] plugins = PluginManager.CommandMapStatic.Values.Distinct().ToArray();
            ApplicationPlugin[] apps = PluginManager.ApplicationList.ToArray();
            var groupCmd = plugins.Where(plugin => plugin.HelpType <= _cm.PermissionLevel)
                .GroupBy(k => k.GetType().Namespace);
            Dictionary<string, Dictionary<string, string>> dicNs = new Dictionary<string, Dictionary<string, string>>();
            foreach (IGrouping<string, CommandPlugin> group in groupCmd.OrderBy(k => k.Key))
            {
                var key = group.Key.Replace("Daylily.Bot.PluginBase", "扩展插件");
                dicNs.Add(key, new Dictionary<string, string>());
                var sb = group.OrderBy(o => o.Commands[0]);
                foreach (var plugin in sb)
                {
                    var cmd = $"/{string.Join(", /", plugin.Commands)}";
                    const int maxL = 23;
                    dicNs[key].Add(cmd.Length > maxL ? cmd.Substring(0, maxL - 3) + "..." : cmd,
                        $"{plugin.Name}。{plugin.Helps[0]}");
                }
            }
            Dictionary<string, string> dicApp = apps.Where(plugin => plugin.HelpType <= _cm.PermissionLevel)
                .OrderBy(k => k.Name).ToDictionary(plugin => plugin.Name, plugin => plugin.Helps[0]);

            string[] hot = CoolQDispatcher.CommandHot.OrderByDescending(k => k.Value)
                  .Take(5)
                  .Where(k => k.Value > 50)
                  .Select(k => "/" + k.Key).ToArray();
            return new FileImage(DrawList(dicNs.OrderBy(k => k.Key).ToDictionary(k => k.Key, k => k.Value), dicApp, hot), 95).ToString();
        }

        private string ShowDetail()
        {
            Custom custom;
            Bot.PluginBase.Plugin plugin;
            if (PluginManager.CommandMapStatic.Keys.Contains(CommandName))
            {
                plugin = PluginManager.CommandMapStatic[CommandName];
                custom = new Custom
                {
                    Title = plugin.Name,
                    Helps = plugin.Helps,
                    Author = plugin.Author,
                    Version = plugin.Version,
                    State = plugin.State,
                    Arg = new Dictionary<string, string>(),
                    FreeArg = new Dictionary<string, string>()
                };
            }
            else if (PluginManager.ApplicationList.Select(k => k.Name).Contains(CommandName))
            {
                plugin = PluginManager.ApplicationList.First(k => k.Name == CommandName);
                custom = new Custom
                {
                    Title = plugin.Name,
                    Helps = plugin.Helps,
                    Author = plugin.Author,
                    Version = plugin.Version,
                    State = plugin.State,
                    Arg = new Dictionary<string, string>(),
                    FreeArg = new Dictionary<string, string>()
                };
            }
            else
                return "未找到相关资源...";

            var sbArg = new StringBuilder();
            var sbFree = new StringBuilder();
            var sbSw = new StringBuilder();

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
                        case FreeArgAttribute _:
                            freeStr = prop.Name;
                            break;
                        case HelpAttribute helpAttrib:
                            helpStr = string.Join(" ", helpAttrib.Helps);
                            break;
                    }
                }

                if (argStr != null)
                {
                    //sbArg.Append($" [{argStr}{(isSwitch ? "" : " " + StringConvert.ToUnderLineStyle(argName))}]");
                    if (isSwitch)
                    {
                        sbSw.Append($" [{argStr}]");
                    }
                    else
                        sbArg.Append($" [{argStr} {argName.ToUnderLineStyle()}]");

                    if (!custom.Arg.ContainsKey(argStr))
                    {
                        custom.Arg.Add(argStr, helpStr);
                    }
                    else
                    {
                        int i = 0;
                        while (custom.Arg.ContainsKey(argStr + " (" + i + ")"))
                            i++;
                        custom.Arg.Add(argStr + " (" + i + ")", helpStr);
                    }
                }

                if (freeStr != null)
                {
                    sbFree.Append($" [{freeStr.ToUnderLineStyle()}]");
                    custom.FreeArg.Add(freeStr.ToUnderLineStyle(), helpStr);
                }
            }

            custom.Usage = plugin is ApplicationPlugin ? "自动触发。" : $"/{CommandName}{sbArg}{sbFree}{sbSw}";
            return new FileImage(DrawDetail(custom)).ToString();
        }

        private Bitmap DrawList(Dictionary<string, Dictionary<string, string>> dicNs, Dictionary<string, string> dicApp, string[] hot)
        {
            string title = "黄花菜Help";
            string sub = "（输入 \"/help [command]\" 查找某个命令的详细信息。）" + Environment.NewLine +
                         "（例：/help sub。）";
            //const string sub2 = "无需命令自动激活：";
            const int offsetH = 30;
            Point pointTitle = new Point(30, 30);
            Point pointSub = new Point(20, 57);
            const int step = 25, offset1 = 25, offset2 = 190;

            Font fontH1 = new Font("等线", 14);
            Font fontH1B = new Font("等线", 14, FontStyle.Bold);
            Font fontH2 = new Font("等线", 12);
            Font fontH2B = new Font("等线", 12, FontStyle.Bold);
            Font fontH3 = new Font("等线", 11);
            Font fontH4 = new Font("等线", 10);

            const int x = 9;
            Size size = MeasureListSize(dicNs, dicApp, fontH2, x, step, offset2);
            size = new Size(size.Width, 80 + size.Height + offsetH * dicNs.Count + 7);

            DirectoryInfo di = InitDirectory(HelpDir);
            FileInfo[] pics = di.GetFiles();
            string picPath = pics[Rnd.Next(pics.Length)].FullName;

            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            using (Image backImg = Image.FromFile(picPath))
            using (Image coverImg = Image.FromFile(Path.Combine(StaticDir, "damnae.png")))
            using (Image hotImg = Image.FromFile(Path.Combine(StaticDir, "hot.png")))
            using (Brush brushWhite = new SolidBrush(Color.White))
            using (Brush brushBlue = new SolidBrush(Color.FromArgb(43, 170, 235)))
            using (Brush brushYellow = new SolidBrush(Color.FromArgb(255, 243, 82)))
            using (Brush brushGrey = new SolidBrush(Color.FromArgb(185, 185, 185)))
            using (Brush brushDarkGrey = new SolidBrush(Color.FromArgb(128, 45, 45, 48)))
            using (Brush brushLightGrey = new SolidBrush(Color.FromArgb(48, 45, 45, 48)))
            using (Brush brushLDarkGrey = new SolidBrush(Color.FromArgb(64, 64, 64)))
            using (Brush brushBack = new SolidBrush(Color.FromArgb(210, 30, 30, 30)))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.InitialHighQuality();
                g.FillImage(backImg, size);
                g.FillRectangle(brushBack, -1, -1, size.Width + 2, size.Height + 2);
                g.DrawImage(coverImg, 0, 0, size.Width, size.Height);

                g.DrawString(_versionInfo, fontH4, brushLDarkGrey, 2, 2);
                g.DrawString(title, fontH1, brushWhite, pointTitle);
                g.DrawString(sub, fontH3, brushGrey, pointSub);

                int i = 0, baseY = 0;
                var sizeSub = g.MeasureString(sub, fontH3);
                int y = (int)(pointSub.Y + sizeSub.Height + 10);
                //bool app = false;
                foreach (var dic in dicNs)
                {
                    float widthsb = g.MeasureString(dic.Key, fontH2B).Width;
                    Rectangle recsb = new Rectangle(x + offset1 - 3 + 30, y + baseY + i * step - 2,
                        (int)widthsb + 6, step - 3);

                    g.FillRectangle(brushLightGrey, recsb);
                    g.DrawString(dic.Key, fontH2B, brushBlue, x + offset1 + 30, y + baseY + i * step + 1);
                    y += offsetH;
                    foreach (var pair in dic.Value)
                    {
                        float width = g.MeasureString(pair.Value, fontH2).Width;
                        //if (!pair.Key.StartsWith('/') && !app)
                        //{
                        //    app = true;
                        //    g.DrawString(sub2, fontH3, brushWhite, x + offset1, y + i * step + 3);
                        //    y += offsetH;
                        //}

                        Rectangle rec = new Rectangle(x + offset1 - 3, y + i * step - 3,
                            offset2 - offset1 + (int)width + 6, step - 3);

                        //if (!app)
                        //{
                        g.FillRoundRectangle(brushDarkGrey, rec, 10);
                        if (hot.Contains(pair.Key))
                            g.DrawImage(hotImg, x, y + i * step, 16, 16);
                        //g.DrawString(">", fontH1B, brushWhite, x, y + i * step);
                        g.DrawString(pair.Key, fontH2, brushYellow, x + offset1, y + i * step);
                        g.DrawString(pair.Value, fontH2, brushWhite, x + offset2, y + i * step);
                        //}
                        //else
                        //{
                        //    g.FillRectangle(brushDarkGrey, rec);
                        //    g.DrawString(pair.Key, fontH2, brushBlue, x + offset1, y + i * step);
                        //    g.DrawString(pair.Value, fontH2, brushWhite, x + offset2, y + i * step);
                        //}

                        i++;
                    }
                }
            }

            fontH1.Dispose();
            fontH1B.Dispose();
            fontH2.Dispose();
            fontH3.Dispose();

            return bitmap;
        }

        private static DirectoryInfo InitDirectory(string path)
        {
            var di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                Directory.CreateDirectory(di.FullName);
                using (Bitmap bi = new Bitmap(200, 200))
                using (Graphics g = Graphics.FromImage(bi))
                {
                    g.Clear(Color.FromArgb(30, 30, 30));
                    bi.Save(Path.Combine(di.FullName, "b.png"), ImageFormat.Png);
                }

                di = new DirectoryInfo(HelpDir);
            }

            return di;
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
            Size sizeHelp = g.MeasureStringSize(help, fontH3);

            int topAuthor = 54 + sizeHelp.Height + 3;
            Point pointAuthor = new Point(45, topAuthor);
            Point pointVer = new Point(205, topAuthor);
            Point pointState = new Point(285, topAuthor);
            string author = string.Join(", ", custom.Author);
            Size sizeAuthor = g.MeasureStringSize(author, fontH4);

            int topUsage = topAuthor + sizeAuthor.Height + 40;
            Point pointUsage = new Point(30, topUsage);
            Size sizeUsage = g.MeasureStringSize(custom.Usage, fontH3);

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

            DirectoryInfo di = InitDirectory(HelpDir);
            FileInfo[] pics = di.GetFiles();
            string picPath = pics[Rnd.Next(pics.Length)].FullName;

            bitmap = new Bitmap(size.Width, size.Height);
            g = Graphics.FromImage(bitmap);
            using (Image backImg = Image.FromFile(picPath))
            using (Image coverImg = Image.FromFile(Path.Combine(StaticDir, "damnae.png")))
            using (Brush brushWhite = new SolidBrush(Color.White))
            using (Brush brushYellow = new SolidBrush(Color.FromArgb(255, 243, 82)))
            using (Brush brushGrey = new SolidBrush(Color.FromArgb(185, 185, 185)))
            using (Brush brushBack = new SolidBrush(Color.FromArgb(210, 30, 30, 30)))
            using (Brush brushLDarkGrey = new SolidBrush(Color.FromArgb(64, 64, 64)))
            {
                g.InitialHighQuality();
                g.FillImage(backImg, size);
                g.FillRectangle(brushBack, -1, -1, size.Width + 2, size.Height + 2);
                g.DrawImage(coverImg, 0, 0, size.Width, size.Height);

                g.DrawString(_versionInfo, fontH4, brushLDarkGrey, 2, 2);
                g.DrawString(custom.Title, fontH1, brushWhite, pointTitle);
                g.DrawString(help, fontH3, brushGrey, pointHelp);
                g.DrawString($"作者：{author}", fontH4, brushWhite, pointAuthor);
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
                Size size = g.MeasureStringSize(item.Value, f);
                if (offsetDisp + size.Width > right) right = offsetDisp + size.Width;
            }
        }

        private static Size MeasureListSize(Dictionary<string, Dictionary<string, string>> dicNs,
            Dictionary<string, string> dicApp, Font fontH2, int x, int step, int offset2)
        {
            int maxW = 0, maxH = 0;
            using (Bitmap bitmap = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                foreach (var dic in dicNs)
                {
                    foreach (var t in dic.Value)
                    {
                        var item = t.Value;
                        SizeF sInst = g.MeasureString(item, fontH2);
                        float width = x + offset2 + sInst.Width + x;
                        if (maxW < width) maxW = (int)width;
                        maxH += step;
                    }
                }
            }

            return new Size(maxW, maxH + 20);
        }

        private class Custom
        {
            public string Title { get; set; }
            public string[] Helps { get; set; }
            public string[] Author { get; set; }
            public string Version { get; set; }
            public PluginVersion State { get; set; }
            public string Usage { get; set; }
            public Dictionary<string, string> Arg { get; set; }
            public Dictionary<string, string> FreeArg { get; set; }

        }
    }
}
