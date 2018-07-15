using System;
using System.Linq;
using System.Text;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Common.Function.Application.Command
{
    public class Plugin : CommandApp
    {
        public override string Name => "插件管理";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Alpha;
        public override string VersionNumber => "1.0";
        public override string Description => "动态管理插件";
        public override string Command => "plugin";

        public override void OnLoad(string[] args)
        {

        }

        public override CommonMessageResponse OnExecute(in CommonMessage messageObj) // 必要方法
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
