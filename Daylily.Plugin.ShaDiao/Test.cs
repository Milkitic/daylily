using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Daylily.Bot;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common;
using Daylily.Common.Utils.RequestUtils;
using Daylily.CoolQ;

namespace Daylily.Plugin.ShaDiao
{
    class Test : ApplicationPlugin
    {
        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            if (!messageObj.Message.Equals("/转"))
                return null;
            using (Session session = new Session(1000 * 60, messageObj.Identity, messageObj.UserId))
            {
                SendMessage(new CommonMessageResponse("请发送图片，5张以内，1分钟内有效。", messageObj, true));
                try
                {
                    CommonMessage cm = session.GetMessage();
                    var infoList = CqCode.GetImageInfo(cm.Message);
                    if (infoList == null) return new CommonMessageResponse("你发送的消息没有包含图片。", cm);
                    if (infoList.Length > 5) return new CommonMessageResponse("你发送的图片过多。", cm);

                    List<Image> imgList = infoList.Select(imgInfo => HttpClientUtil.GetImageFromUrl(imgInfo.Url))
                        .ToList();

                    var sendList = HandleImage(imgList);

                    return new CommonMessageResponse(string.Join("\r\n", sendList), messageObj);
                }
                catch (TimeoutException)
                {
                    return null;
                }
            }
        }

        private static IEnumerable<FileImage> HandleImage(IEnumerable<Image> imgList)
        {
            List<FileImage> sendList = new List<FileImage>();
            foreach (Image img in imgList)
            {
                List<Bitmap> animationList = new List<Bitmap>();
                int width = img.Width, height = img.Height;
                int fixedW = (int)Math.Ceiling(Math.Max(width, height) * Math.Pow(2, 0.5));
                int divide = 15;

                for (int i = 0; i < divide; i++)
                {
                    float deg = i * 360f / divide;
                    Bitmap bitmap = new Bitmap(fixedW, fixedW);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.White);
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.TranslateTransform(fixedW / 2f, fixedW / 2f);
                        g.RotateTransform(deg);
                        g.TranslateTransform(-fixedW / 2f, -fixedW / 2f);
                        g.DrawImage(img, fixedW / 2f - img.Width / 2f, fixedW / 2f - img.Height / 2f, img.Width, img.Height);
                    }

                    animationList.Add(bitmap);
                }

                var fullPath = GenerateAnimation(animationList, 10);
                sendList.Add(new FileImage(fullPath));
            }

            return sendList;
        }

        private static string GenerateAnimation(IReadOnlyCollection<Bitmap> bitmaps, double refreshDelay)
        {
            const int propertyTagFrameDelay = 0x5100;
            const int propertyTagLoopCount = 0x5101;
            const short propertyTagTypeLong = 4;
            const short propertyTagTypeShort = 3;

            const int uintBytes = 4;

            var gifEncoder = GetEncoder(ImageFormat.Gif);
            // Params of the first frame.
            var encoderParams1 = new EncoderParameters(1)
            {
                Param = { [0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame) }
            };
            // Params of other frames.
            var encoderParamsN = new EncoderParameters(1)
            {
                Param = { [0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionTime) }
            };
            // Params for the finalizing call.
            var encoderParamsFlush = new EncoderParameters(1)
            {
                Param = { [0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush) }
            };

            // PropertyItem for the frame delay (apparently, no other way to create a fresh instance).
            var frameDelay = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
            frameDelay.Id = propertyTagFrameDelay;
            frameDelay.Type = propertyTagTypeLong;
            // Length of the value in bytes.
            frameDelay.Len = bitmaps.Count * uintBytes;
            // The value is an array of 4-byte entries: one per frame.
            // Every entry is the frame delay in 1/100-s of a second, in little endian.
            frameDelay.Value = new byte[bitmaps.Count * uintBytes];
            // E.g., here, we're setting the delay of every frame to 1 second.
            var frameDelayBytes = BitConverter.GetBytes((uint)(refreshDelay / 10));
            for (int j = 0; j < bitmaps.Count; ++j)
                Array.Copy(frameDelayBytes, 0, frameDelay.Value, j * uintBytes, uintBytes);

            // PropertyItem for the number of animation loops.
            var loopPropertyItem = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
            loopPropertyItem.Id = propertyTagLoopCount;
            loopPropertyItem.Type = propertyTagTypeShort;
            loopPropertyItem.Len = 1;
            // 0 means to animate forever.
            loopPropertyItem.Value = BitConverter.GetBytes((ushort)0);

            string fileName = Path.Combine(Domain.CacheImagePath, Guid.NewGuid() + ".gif");
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                bool first = true;
                Bitmap firstBitmap = null;
                // Bitmaps is a collection of Bitmap instances that'll become gif frames.
                foreach (var bitmap in bitmaps)
                {
                    if (first)
                    {
                        firstBitmap = bitmap;
                        firstBitmap.SetPropertyItem(frameDelay);
                        firstBitmap.SetPropertyItem(loopPropertyItem);
                        firstBitmap.Save(stream, gifEncoder, encoderParams1);
                        first = false;
                    }
                    else
                    {
                        firstBitmap.SaveAdd(bitmap, encoderParamsN);
                    }
                }

                firstBitmap.SaveAdd(encoderParamsFlush);
            }

            return fileName;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }
}
