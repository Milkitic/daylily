using Daylily.Common.Models;
using System;
using System.Threading;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Service
{
    public class Test : AppConstruct
    {
        public override string Name => "后台计时";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Alpha;
        public override string VersionNumber => "1.0";
        public override string Description => "后台计时";
        public override string Command => null;
        public override AppType AppType => AppType.Service;

        public override void OnLoad(string[] args)
        {
            Thread t = new Thread(Async);
            t.Start();
        }

        public override CommonMessageResponse OnExecute(CommonMessage messageObj) => null;

        private static void Async()
        {
            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine(DateTime.Now);
            }
        }
    }
}
