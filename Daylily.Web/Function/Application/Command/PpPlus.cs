using Daylily.Common.Assist;
using Daylily.Common.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Web.Function.Application.Command
{
    public class PpPlus : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage commonMessage)
        {
            string @params = commonMessage.Parameter.Replace("@me", "yf_bmp");
            string json_string = null;

            Logger.DefaultLine("Sent request.");
            var response = WebRequestHelper.CreatePostHttpResponse("https://syrin.me/pp+/u/" + @params);
            Logger.DefaultLine("Received request.");
            if (response != null)
            {
                json_string = WebRequestHelper.GetResponseString(response);
                if (json_string.IndexOf("Oops!") != -1)
                    throw new Exception("不存在的");
                int index = json_string.IndexOf("<div class=\"performance-table\">");
                int length = json_string.IndexOf("</div>", index) - index;
                string inner_text = json_string.Substring(index, length);

                Dictionary<string, int> d_index = new Dictionary<string, int>
                {
                    { "Performance", inner_text.IndexOf("Performance") },
                    { "Total", inner_text.IndexOf("Total") },
                    { "Jump", inner_text.IndexOf("Jump") },
                    { "Flow", inner_text.IndexOf("Flow") },
                    { "Precision", inner_text.IndexOf("Precision") },
                    { "Speed", inner_text.IndexOf("Speed") },
                    { "Stamina", inner_text.IndexOf("Stamina") },
                    { "Accuracy", inner_text.IndexOf("Accuracy") }
                };

                Dictionary<string, int> d_value = new Dictionary<string, int>();
                foreach (var kvp in d_index)
                {
                    if (kvp.Key == "Performance")
                    {
                        int tmp = inner_text.IndexOf("<th>", kvp.Value) + 4;
                        var str = inner_text.Substring(tmp, inner_text.IndexOf("pp", kvp.Value) - tmp);
                        int pp = int.Parse(str.Replace(",", ""));
                        d_value.Add(kvp.Key, pp);
                    }
                    else
                    {
                        int tmp = inner_text.IndexOf("<td>", kvp.Value) + 4;
                        var str = inner_text.Substring(tmp, inner_text.IndexOf("pp", kvp.Value) - tmp);
                        int pp = int.Parse(str.Replace(",", ""));
                        d_value.Add(kvp.Key, pp);
                    }
                }
                var resp = CQCode.EncodeImageToBase64(Draw(@params, d_value));
                //Logger.InfoLine(resp);
                return new CommonMessageResponse(resp, commonMessage);
            }
            return null;
        }

        private Bitmap Draw(string user, Dictionary<string, int> d_pfmance)
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
                Color NET_COLOR = Color.FromArgb(204, 213, 240);
                Color BORDER_COLOR = Color.FromArgb(169, 173, 225);

                int rMax = 100;
                int rStep = 25;
                for (int i = 0; i < 4; i++)
                {
                    int r = rMax - i * rStep;
                    PointF[] points = new PointF[6];
                    float deg = 0;

                    using (GraphicsPath gp = new GraphicsPath())
                    using (Brush brush = new SolidBrush(Color.FromArgb(100 + i * 20, 79, 103, 175)))
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            deg = (float)(j * 60d / 180d * Math.PI);
                            points[j] = new PointF((float)(pointCentre.X + Math.Sin(deg) * r), pointCentre.Y + (float)(Math.Cos(deg) * r));
                            if (i == 0)
                                pointBorder[j] = new PointF((float)(pointCentre.X + Math.Sin(deg) * r), pointCentre.Y + (float)(Math.Cos(deg) * r));
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
                    using (Pen pen = new Pen(BORDER_COLOR, 1))
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
                PointF[] point_static = new PointF[6];
                double[] _MAX_PPS = { 7500, 5500, 3500, 4500, 4500, 4500 };
                double _MAX_PP = 15500;
                int ii = 0;
                foreach (var item in d_pfmance)
                {
                    if (item.Key == "Performance" || item.Key == "Total") continue;
                    double pcent = item.Value / _MAX_PPS[ii];
                    float r_current = (float)(rMax * pcent);

                    float deg = (float)(ii * 60d / 180d * Math.PI);
                    point_static[ii] = new PointF((float)(pointCentre.X + Math.Sin(deg) * r_current), pointCentre.Y + (float)(Math.Cos(deg) * r_current));

                    ii++;
                }

                using (GraphicsPath gp = new GraphicsPath())
                using (Brush brush = new SolidBrush(Color.FromArgb(128,
                    171 + (int)((244 - 171) * d_pfmance["Performance"] / _MAX_PP),
                    210 + (int)((166 - 210) * d_pfmance["Performance"] / _MAX_PP),
                    124 + (int)((98 - 124) * d_pfmance["Performance"] / _MAX_PP))))
                {
                    gp.AddPolygon(point_static);
                    gp.CloseFigure();
                    // gp.FillPath(new SolidBrush(Color.FromArgb(128, 171, 210, 124)), gp2);
                    g.FillPath(brush, gp);
                    Logger.DebugLine("[FORE POLYGON] OK");
                }

                // 画数据边框
                Color c_bder_static = Color.FromArgb(90 + (int)((241 - 90) * d_pfmance["Performance"] / _MAX_PP),
                                            180 + (int)((139 - 180) * d_pfmance["Performance"] / _MAX_PP),
                                             49 + (int)((37 - 49) * d_pfmance["Performance"] / _MAX_PP));

                using (Brush brush = new SolidBrush(c_bder_static))
                using (Pen pen = new Pen(c_bder_static, 2))
                {
                    for (int i = 0; i < point_static.Length; i++)
                    {
                        g.FillEllipse(brush, new RectangleF(point_static[i].X - 3, point_static[i].Y - 3, 6, 6));

                        if (i == point_static.Length - 1)
                            g.DrawLine(pen, point_static[i], point_static[0]);
                        else
                            g.DrawLine(pen, point_static[i], point_static[i + 1]);
                        Logger.DebugLine("[FORE BORDER] OK");
                    }
                }

                // 写字
                ii = 0;
                using (Font fontMSYH = new Font("微软雅黑", 8))
                using (Brush blackBrush = new SolidBrush(Color.Black))
                {
                    foreach (var item in d_pfmance)
                    {
                        if (item.Key == "Performance" || item.Key == "Total") continue;
                        double pcent = item.Value / _MAX_PPS[ii];
                        float r_current = (float)(rMax * pcent) + (pcent > 0.6 ? -20 : 15);
                        string str = item.Value.ToString();

                        SizeF ok = g.MeasureString(str, fontMSYH);

                        float deg = (float)(ii * 60d / 180d * Math.PI);
                        var temp_point = new PointF((float)(pointCentre.X + Math.Sin(deg) * r_current) - ok.Width / 2f, pointCentre.Y + (float)(Math.Cos(deg) * r_current) - ok.Height / 2f);

                        g.DrawString(item.Value.ToString(), fontMSYH, blackBrush, temp_point);
                        Logger.DebugLine("[STRING] OK");
                        //g.DrawString(item.Key.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), point_bder[ii]);
                        ii++;
                    }
                }

                string str_title = user.ToUpper();
                string str_total = d_pfmance["Performance"].ToString("N0") + "pp";

                using (Font font_title = new Font("Arial", 10, FontStyle.Bold))
                using (Font font_total = new Font("Arial", 35))
                using (Brush brushTotal = new SolidBrush(Color.FromArgb(70, c_bder_static.R, c_bder_static.G, c_bder_static.B)))
                using (Brush brushTitle = new SolidBrush(Color.FromArgb(200, 79, 103, 175)))
                {
                    SizeF size_str_title = g.MeasureString(str_title, font_title);
                    SizeF size_str_total = g.MeasureString(str_total, font_total);

                    g.DrawString(str_total, font_total, brushTotal, bmp.Width - size_str_total.Width + 5, 5);
                    g.DrawString(str_title, font_title, brushTitle, 4, 4);
                    Logger.DebugLine("[STRING] OK");
                }
                g.DrawImage(Image.FromFile(Path.Combine(Environment.CurrentDirectory, "static", "ForeLayer.png")), new Rectangle(0, 0, bmp.Width, bmp.Height));
                Logger.DebugLine("[IMAGE] OK");
            }
            return bmp;
        }
    }
}
