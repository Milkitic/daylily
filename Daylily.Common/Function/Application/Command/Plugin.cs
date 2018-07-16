using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Common.Function.Application.Command
{
    [Name("插件管理")]
    [Author("yf_extension")]
    [Version(0, 0, 2, PluginVersion.Alpha)]
    [Help("动态管理插件")]
    [Command("plugin")]
    public class Plugin : CommandApp
    {
        [Arg("l", IsSwitch = true)]
        public bool List { get; set; }
        [Arg("r")]
        public string Remove { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            if (List)
                return new CommonMessageResponse(ShowPluginList(), messageObj);
            if (Remove != null)
                return new CommonMessageResponse(Remove, messageObj);
            return null;
        }

        private string ShowPluginList()
        {
            var sb = new StringBuilder();
            var commandMap = PluginManager.CommandMapStatic.Values.Distinct();
            sb.AppendLine("命令插件：");
            foreach (var item in commandMap)
            {
                sb.Append(item.Name + "、");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine(Environment.NewLine + "服务插件：");
            foreach (var item in PluginManager.ServiceList)
            {
                sb.Append(item.Name + "、");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine(Environment.NewLine + "应用插件：");
            foreach (var item in PluginManager.ApplicationList)
            {
                sb.Append(item.Name + "、");
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}
