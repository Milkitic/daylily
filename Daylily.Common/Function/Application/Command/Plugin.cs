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
    [Version(0, 1, 0, PluginVersion.Alpha)]
    [Help("动态管理插件的启用状态。", HelpType = PermissionLevel.Root)]
    [Command("plugin")]
    public class Plugin : CommandApp
    {
        [Arg("l", IsSwitch = true)]
        [Help("若启用，显示当前启用的插件列表。")]
        public bool List { get; set; }
        [Arg("d")]
        [Help("禁用指定的插件。")]
        public string PluginDisable { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            if (List)
                return new CommonMessageResponse(ShowPluginList(), messageObj);
            if (PluginDisable != null)
                return new CommonMessageResponse(PluginDisable, messageObj);
            return null;
        }

        private static string ShowPluginList()
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
