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
    [Version(0, 0, 1, PluginVersion.Alpha)]
    [Help("动态管理插件")]
    [Command("plugin")]
    public class Plugin : CommandApp
    {
        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            string[] args = messageObj.Parameter.Split(' ');

            for (int i = 0; i < args.Length; i++)
            {
                string cmd = args[i]; //, par = args[i + 1];
                switch (cmd)
                {
                    case "-l":
                    case "--list":
                        return new CommonMessageResponse(ShowPluginList(), messageObj);
                    case "-r":
                    case "--remove":
                        string par = args[i + 1];
                        return new CommonMessageResponse(par, messageObj);
                    default:
                        return new CommonMessageResponse("未知的参数: " + cmd + "...", messageObj);
                }
            }

            return null;
        }

        private string ShowPluginList()
        {
            var sb = new StringBuilder();
            var commandMap = PluginManager.CommandMap.Values.Distinct();
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
