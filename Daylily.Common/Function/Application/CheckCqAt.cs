using System;
using System.IO;
using System.Linq;
using System.Threading;
using Daylily.Common.Assist;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Common.Function.Application
{
    public class CheckCqAt : AppConstruct
    {
        public override string Name => "嗅探at";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Stable;
        public override string VersionNumber => "1.0";
        public override string Description => "当自己被at时回击at对方";
        public override string Command => null;
        public override AppType AppType => AppType.Application;

        private static readonly string PandaDir = Path.Combine(Environment.CurrentDirectory, "panda");

        public override void OnLoad(string[] args)
        {
            //throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(CommonMessage messageObj)
        {
            if (messageObj.MessageType == MessageType.Private) return null;
            if (messageObj.Group != null && messageObj.GroupId == "133605766" &&
                DateTime.Now.Hour < 22 && DateTime.Now.Hour > 6)
                return null;
            string[] ids = CqCode.GetAt(messageObj.Message);
            if (ids == null || !ids.Contains("2181697779") && !ids.Contains("3421735167")) return null;
            Thread.Sleep(Rnd.Next(200, 300));
            return Rnd.NextDouble() < 0.9
                ? new CommonMessageResponse("", messageObj, true)
                : new CommonMessageResponse(CqCode.EncodeFileToBase64(Path.Combine(PandaDir, "at.jpg")), messageObj);

        }
    }
}
