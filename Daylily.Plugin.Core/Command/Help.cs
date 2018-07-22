using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daylily.Common.Function;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    [Name("黄花菜帮助")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Beta)]
    [Help("如何使用黄花菜？")]
    [Command("help")]
    public class Help : CommandApp
    {
        [FreeArg]
        public string PluginCommand { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            SendMessage(
                PluginCommand == null
                    ? new CommonMessageResponse(ShowList().Trim('\n').Trim('\r'), messageObj.UserId)
                    : new CommonMessageResponse(ShowDetail().Trim('\n').Trim('\r'), messageObj.UserId), null, null,
                MessageType.Private);

            return messageObj.MessageType != MessageType.Private
                ? new CommonMessageResponse(LoliReply.PrivateOnly, messageObj)
                : null;
        }

        private static string ShowList()
        {
            var sb = new StringBuilder();
            CommandApp[] plugins = PluginManager.CommandMapStatic.Values.Distinct().ToArray();
            sb.AppendLine("黄花菜帮助：");
            sb.AppendLine("（使用/help [command]查看详细帮助）");
            foreach (var item in plugins)
                sb.AppendLine($"{item.Name} ({string.Join(", ", item.Commands)}): {string.Join("。", item.Helps)}");

            return sb.ToString();
        }

        private string ShowDetail()
        {
            if (!PluginManager.CommandMapStatic.Keys.Contains(PluginCommand))
                return "未找到相关资源...";

            CommandApp plugin = PluginManager.CommandMapStatic[PluginCommand];
            var sb = new StringBuilder();
            var sbArg = new StringBuilder();
            var sbFree = new StringBuilder();

            sb.AppendLine($"“{plugin.Name}”的帮助：");
            sb.AppendLine($"作者：{plugin.Author}");
            sb.AppendLine($"版本：{plugin.Version} {plugin.State.ToString()}");
            sb.AppendLine($"帮助说明：\r\n  {string.Join("  \r\n", plugin.Helps)}");
            Type t = plugin.GetType();
            var props = t.GetProperties();

            bool used = false;
            foreach (var prop in props)
            {
                var info = prop.GetCustomAttributes(false);
                if (info.Length == 0) continue;
                string helpStr = "尚无帮助信息。", argStr = null, freeStr = null;
                foreach (var o in info)
                {
                    switch (o)
                    {
                        case ArgAttribute argAttrib:
                            argStr = $"-{argAttrib.Name}";
                            break;
                        case FreeArgAttribute freeArgAttrib:
                            freeStr = $"自由参数 ({prop.Name})";
                            break;
                        case HelpAttribute helpAttrib:
                            helpStr = string.Join("\r\n", helpAttrib.Helps);
                            break;
                    }
                }

                if (argStr != null)
                {
                    if (!used)
                    {
                        sbArg.AppendLine("参数说明：");
                        used = true;
                    }

                    sbArg.AppendLine(argStr + ": " + helpStr);
                }

                if (freeStr != null)
                {
                    if (!used)
                    {
                        sbFree.AppendLine("参数说明：");
                        used = true;
                    }

                    sbFree.AppendLine(freeStr + ": " + helpStr);
                }
            }

            return sb.ToString() + sbArg + sbFree;

        }
    }
}
