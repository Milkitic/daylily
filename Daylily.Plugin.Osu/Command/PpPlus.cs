using Daylily.Bot.Enum;
using Daylily.Bot.Message;
using Daylily.Common;
using Daylily.Common.Utils.HtmlUtils;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.RequestUtils;
using Daylily.Common.Utils.StringUtils;
using Daylily.Osu.Database.BLL;
using Daylily.Osu.Database.Model;
using Daylily.Osu.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using Daylily.Bot.Backend;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;

namespace Daylily.Plugin.Osu
{
    [Name("PP+查询")]
    [Author("yf_extension")]
    [Version(2, 0, 3, PluginVersion.Stable)]
    [Help("获取发送者的PP+信息，并生成相应六维图。")]
    [Command("pp")]
    public class PpPlus : CoolQCommandPlugin
    {
        [FreeArg]
        [Help("查询指定的osu用户名。若带空格，请使用引号。")]
        public string OsuId { get; set; }

        public override void OnInitialized(string[] args)
        {

        }

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            string userName, userId;
            if (OsuId == null)
            {
                BllUserRole bllUserRole = new BllUserRole();
                List<TblUserRole> userInfo = bllUserRole.GetUserRoleByQq(long.Parse(routeMsg.UserId));
                if (userInfo.Count == 0)
                    return routeMsg.ToSource(LoliReply.IdNotBound, true);
                userId = userInfo[0].UserId.ToString();
                userName = userInfo[0].CurrentUname;
            }
            else
            {
                int userNum = OldSiteApi.GetUser(OsuId, out var userObj);
                if (userNum == 0)
                    return routeMsg.ToSource(LoliReply.IdNotFound, true);
                if (userNum > 1)
                {
                    // ignored
                }

                userId = userObj.user_id;
                userName = userObj.username;
            }

            var jsonString = HttpClientUtil.HttpGet("https://syrin.me/pp+/u/" + userId);
            if (jsonString == null)
                return routeMsg.ToSource("PP+获取超时，请重试…", true);

            StringFinder sf = new StringFinder(jsonString);
            sf.FindNext("<div class=\"performance-table\">", false);
            sf.FindNext("</div>");

            string innerText = sf.Cut().Trim('\n').Trim('\r');
            HtmlTable htmlTable = new HtmlTable(innerText);

            string[,] array = htmlTable.GetArray();

            Dictionary<string, int> dValue = new Dictionary<string, int>();
            for (var i = 0; i < array.GetLength(0); i++)
            {
                for (var j = 0; j < array.GetLength(1); j += 2)
                {
                    string key = array[i, j].Trim(':').Replace("Aim (", "").Replace(")", "");
                    int value = int.Parse(array[i, j + 1].Replace(",", "").Trim('p'));
                    dValue.Add(key, value);
                }
            }

            var cqImg = new FileImage(Draw(userName, dValue)).ToString();
            return routeMsg.ToSource(cqImg);
        }

        private static Bitmap Draw(string user, IReadOnlyDictionary<string, int> dPfmance)
        {
            Bitmap bmp = new Bitmap(230, 256, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.FromArgb(244, 244, 244));

                PointF[] pointBorder = new PointF[6];
                PointF pointCentre = new PointF(bmp.Width / 2f, bmp.Height / 2f + 15);

                // 预置颜色
                // Color netColor = Color.FromArgb(204, 213, 240);
                Color borderColor = Color.FromArgb(169, 173, 225);

                int rMax = 100;
                int rStep = 25;
                for (int i = 0; i < 4; i++)
                {
                    int r = rMax - i * rStep;
                    PointF[] points = new PointF[6];

                    using (GraphicsPath gp = new GraphicsPath())
                    using (Brush brush = new SolidBrush(Color.FromArgb(100 + i * 20, 79, 103, 175)))
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            float deg = (float)(j * 60d / 180d * Math.PI);
                            points[j] = new PointF((float)(pointCentre.X + Math.Sin(deg) * r),
                                pointCentre.Y + (float)(Math.Cos(deg) * r));
                            if (i == 0)
                                pointBorder[j] = new PointF((float)(pointCentre.X + Math.Sin(deg) * r),
                                    pointCentre.Y + (float)(Math.Cos(deg) * r));
                        }

                        gp.AddPolygon(points);
                        gp.CloseFigure();
                        g.FillPath(brush, gp);
                        Logger.Debug("[BACK POLYGON] OK");
                    }
                }

                // 画网络
                foreach (var item in pointBorder)
                {
                    //using (Brush brush = new LinearGradientBrush(pointCentre, item, NET_COLOR, BORDER_COLOR)) // 线性渐变笔刷在linux平台Out of memory
                    using (Pen pen = new Pen(borderColor, 1))
                    {
                        g.DrawLine(pen, pointCentre, item);
                        Logger.Debug("[NET FRAME] OK");
                    }
                }

                //// 画边框
                //using (Pen pen = new Pen(BORDER_COLOR, 1))
                //{
                //    for (int i = 0; i < pointBorder.Length; i++)
                //    {
                //        if (i == pointBorder.Length - 1)
                //            g.DrawLine(pen, pointBorder[i], pointBorder[0]);
                //        else
                //            g.DrawLine(pen, pointBorder[i], pointBorder[i + 1]);
                //    }
                //}

                // 画数据
                PointF[] pointStatic = new PointF[6];
                double[] maxPps = { 7500, 5500, 3500, 4500, 4500, 4500 };
                double maxPp = 15500;
                int ii = 0;
                foreach (var item in dPfmance)
                {
                    if (item.Key == "Performance" || item.Key == "Total") continue;
                    double pcent = item.Value / maxPps[ii];
                    float rCurrent = (float)(rMax * pcent);

                    float deg = (float)(ii * 60d / 180d * Math.PI);
                    pointStatic[ii] = new PointF((float)(pointCentre.X + Math.Sin(deg) * rCurrent),
                        pointCentre.Y + (float)(Math.Cos(deg) * rCurrent));

                    ii++;
                }

                using (GraphicsPath gp = new GraphicsPath())
                using (Brush brush = new SolidBrush(Color.FromArgb(128,
                    171 + (int)((244 - 171) * dPfmance["Performance"] / maxPp),
                    210 + (int)((166 - 210) * dPfmance["Performance"] / maxPp),
                    124 + (int)((98 - 124) * dPfmance["Performance"] / maxPp))))
                {
                    gp.AddPolygon(pointStatic);
                    gp.CloseFigure();
                    // gp.FillPath(new SolidBrush(Color.FromArgb(128, 171, 210, 124)), gp2);
                    g.FillPath(brush, gp);
                    Logger.Debug("[FORE POLYGON] OK");
                }

                // 画数据边框
                Color cBorderStatic = Color.FromArgb(90 + (int)((241 - 90) * dPfmance["Performance"] / maxPp),
                    180 + (int)((139 - 180) * dPfmance["Performance"] / maxPp),
                    49 + (int)((37 - 49) * dPfmance["Performance"] / maxPp));

                using (Brush brush = new SolidBrush(cBorderStatic))
                using (Pen pen = new Pen(cBorderStatic, 2))
                {
                    for (int i = 0; i < pointStatic.Length; i++)
                    {
                        g.FillEllipse(brush, new RectangleF(pointStatic[i].X - 3, pointStatic[i].Y - 3, 6, 6));
                        g.DrawLine(pen, pointStatic[i],
                            i == pointStatic.Length - 1 ? pointStatic[0] : pointStatic[i + 1]);
                        Logger.Debug("[FORE BORDER] OK");
                    }
                }

                // 写字
                ii = 0;
                using (Font fontMsyh = new Font("微软雅黑", 8))
                using (Brush blackBrush = new SolidBrush(Color.Black))
                {
                    foreach (var item in dPfmance)
                    {
                        if (item.Key == "Performance" || item.Key == "Total") continue;
                        double pcent = item.Value / maxPps[ii];
                        float rCurrent = (float)(rMax * pcent) + (pcent > 0.6 ? -20 : 15);
                        string str = item.Value.ToString();

                        SizeF ok = g.MeasureString(str, fontMsyh);

                        float deg = (float)(ii * 60d / 180d * Math.PI);
                        var tempPoint = new PointF((float)(pointCentre.X + Math.Sin(deg) * rCurrent) - ok.Width / 2f,
                            pointCentre.Y + (float)(Math.Cos(deg) * rCurrent) - ok.Height / 2f);

                        g.DrawString(item.Value.ToString(), fontMsyh, blackBrush, tempPoint);
                        Logger.Debug("[STRING] OK");
                        //g.DrawString(item.Key.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), point_bder[ii]);
                        ii++;
                    }
                }

                string strTitle = user.ToUpper();
                string strTotal = dPfmance["Performance"].ToString("N0") + "pp";

                using (Font fontTitle = new Font("Arial", 10, FontStyle.Bold))
                using (Font fontTotal = new Font("Arial", 35))
                using (Brush brushTotal =
                    new SolidBrush(Color.FromArgb(70, cBorderStatic.R, cBorderStatic.G, cBorderStatic.B)))
                using (Brush brushTitle = new SolidBrush(Color.FromArgb(200, 79, 103, 175)))
                {
                    //SizeF sizeStrTitle = g.MeasureString(strTitle, fontTitle);
                    SizeF sizeStrTotal = g.MeasureString(strTotal, fontTotal);

                    g.DrawString(strTotal, fontTotal, brushTotal, bmp.Width - sizeStrTotal.Width + 5, 5);
                    g.DrawString(strTitle, fontTitle, brushTitle, 4, 4);
                    Logger.Debug("[STRING] OK");
                }

                g.DrawImage(Image.FromFile(Path.Combine(Domain.ResourcePath, "ppPlus", "ForeLayer.png")),
                    new Rectangle(0, 0, bmp.Width, bmp.Height));
                Logger.Debug("[IMAGE] OK");
            }

            return bmp;
        }
    }
}
