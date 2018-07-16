using Daylily.Common.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Utils.LogUtils;

namespace Daylily.Service
{
    [Name("后台计时")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("后台计时")]
    public class Test : ServiceApp
    {
        public override void RunTask(string[] args)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine(DateTime.Now);
                }
            });
        }
    }
}
