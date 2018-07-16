using System;
using System.Threading.Tasks;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    [Name("帮助")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Alpha)]
    [Help("按一定条件触发复读")]
    [Command("help")]
    public class Help : CommandApp
    {
        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            return new CommonMessageResponse("太多了哇..都在这里：https://www.zybuluo.com/milkitic/note/1130078", messageObj);
        }
    }
}
