using System;
using System.IO;
using System.Linq;
using System.Threading;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common;
using Daylily.CoolQ;

namespace Daylily.Plugin.ShaDiao.Application
{
    [Name("@检测")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("当自己被at时回击at对方")]
    public class CheckCqAt : ApplicationPlugin
    {
        private static readonly string PandaDir = Path.Combine(Domain.CurrentDirectory, "panda");

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            if (messageObj.MessageType == MessageType.Private)
                return null;

            string[] ids = CqCode.GetAt(messageObj.Message);
            if (ids == null || !ids.Contains("2181697779") && !ids.Contains("3421735167")) return null;
            Thread.Sleep(Rnd.Next(200, 300));
            if (Rnd.NextDouble() < 0.9)
                return new CommonMessageResponse("", messageObj, true);
            else
            {
                var cqImg = new FileImage(Path.Combine(PandaDir, "at.jpg"));
                return new CommonMessageResponse(cqImg.ToString(), messageObj);
            }
        }
    }
}
