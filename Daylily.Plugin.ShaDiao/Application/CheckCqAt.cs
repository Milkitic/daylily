using System.IO;
using System.Linq;
using System.Threading;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.Common;
using Daylily.CoolQ.Message;

namespace Daylily.Plugin.ShaDiao.Application
{
    [Name("@检测")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("当自己被at时回击at对方")]
    public class CheckCqAt : ApplicationPlugin
    {
        private static readonly string PandaDir = Path.Combine(Domain.ResourcePath, "panda");

        public override CommonMessageResponse OnMessageReceived(CoolQNavigableMessage navigableMessageObj)
        {
            if (navigableMessageObj.MessageType == MessageType.Private)
                return null;

            string[] ids = CqCode.GetAt(navigableMessageObj.RawMessage);
            if (ids == null || !ids.Contains("2181697779") && !ids.Contains("3421735167")) return null;
            Thread.Sleep(StaticRandom.Next(200, 300));
            if (StaticRandom.NextDouble() < 0.9)
                return new CommonMessageResponse("", navigableMessageObj, true);
            else
            {
                var cqImg = new FileImage(Path.Combine(PandaDir, "at.jpg"));
                return new CommonMessageResponse(cqImg.ToString(), navigableMessageObj);
            }
        }
    }
}
