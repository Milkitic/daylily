using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using Daylily.Common.Assist;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    public class PpPlus : AppConstruct
    {
        public override string Name => "PP+查询";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Beta;
        public override string VersionNumber => "1.0";
        public override string Description => "获取自己的PP+信息，并生成六维图";
        public override string Command => "pp";
        public override AppType AppType => AppType.Command;

        public override void OnLoad(string[] args)
        {
        }

        public override CommonMessageResponse OnExecute(in CommonMessage messageObj)
        {
            string userId = messageObj.Parameter;

            Logger.DefaultLine("Sent request.");
            var response = WebRequestHelper.CreatePostHttpResponse("https://syrin.me/pp+/u/" + userId);
            Logger.DefaultLine("Received request.");
            if (response == null)
                return null;

            string jsonString = WebRequestHelper.GetResponseString(response);
            if (jsonString.IndexOf("Oops!", StringComparison.Ordinal) != -1)
                throw new Exception(LoliReply.IdNotFound);
            int index = jsonString.IndexOf("<div class=\"performance-table\">", StringComparison.Ordinal);
            int length = jsonString.IndexOf("</div>", index, StringComparison.Ordinal) - index;
            string innerText = jsonString.Substring(index, length);

            Dictionary<string, int> dIndex = new Dictionary<string, int>
            {
                {"Performance", innerText.IndexOf("Performance", StringComparison.Ordinal)},
                {"Total", innerText.IndexOf("Total", StringComparison.Ordinal)},
                {"Jump", innerText.IndexOf("Jump", StringComparison.Ordinal)},
                {"Flow", innerText.IndexOf("Flow", StringComparison.Ordinal)},
                {"Precision", innerText.IndexOf("Precision", StringComparison.Ordinal)},
                {"Speed", innerText.IndexOf("Speed", StringComparison.Ordinal)},
                {"Stamina", innerText.IndexOf("Stamina", StringComparison.Ordinal)},
                {"Accuracy", innerText.IndexOf("Accuracy", StringComparison.Ordinal)}
            };

            Dictionary<string, int> dValue = new Dictionary<string, int>();
            foreach (var kvp in dIndex)
            {
                if (kvp.Key == "Performance")
                {
                    int tmp = innerText.IndexOf("<th>", kvp.Value, StringComparison.Ordinal) + 4;
                    var str = innerText.Substring(tmp,
                        innerText.IndexOf("pp", kvp.Value, StringComparison.Ordinal) - tmp);
                    int pp = int.Parse(str.Replace(",", ""));
                    dValue.Add(kvp.Key, pp);
                }
                else
                {
                    int tmp = innerText.IndexOf("<td>", kvp.Value, StringComparison.Ordinal) + 4;
                    var str = innerText.Substring(tmp,
                        innerText.IndexOf("pp", kvp.Value, StringComparison.Ordinal) - tmp);
                    int pp = int.Parse(str.Replace(",", ""));
                    dValue.Add(kvp.Key, pp);
                }
            }

            var resp = CqCode.EncodeImageToBase64(Draw(userId, dValue));
            //Logger.InfoLine(resp);
            return new CommonMessageResponse(resp, messageObj);
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
                        Logger.DebugLine("[BACK POLYGON] OK");
                    }
                }

                // 画网络
                foreach (var item in pointBorder)
                {
                    //using (Brush brush = new LinearGradientBrush(pointCentre, item, NET_COLOR, BORDER_COLOR)) // 线性渐变笔刷在linux平台Out of memory
                    using (Pen pen = new Pen(borderColor, 1))
                    {
                        g.DrawLine(pen, pointCentre, item);
                        Logger.DebugLine("[NET FRAME] OK");
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
                    Logger.DebugLine("[FORE POLYGON] OK");
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
                        Logger.DebugLine("[FORE BORDER] OK");
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
                        Logger.DebugLine("[STRING] OK");
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
                    Logger.DebugLine("[STRING] OK");
                }

                g.DrawImage(Image.FromFile(Path.Combine(Environment.CurrentDirectory, "static", "ForeLayer.png")),
                    new Rectangle(0, 0, bmp.Width, bmp.Height));
                Logger.DebugLine("[IMAGE] OK");
            }

            return bmp;
        }
    }
}
