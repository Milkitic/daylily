using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common;

namespace Daylily.Bot.Function.Application.Command
{
    [Name("插件管理")]
    [Author("yf_extension")]
    [Version(0, 1, 4, PluginVersion.Alpha)]
    [Help("动态管理插件的启用状态。", "仅限当前群生效。", HelpType = PermissionLevel.Admin)]
    [Command("plugin")]
    public class PluginManager : CommandPlugin
    {
        [Arg("list", IsSwitch = true)]
        [Help("若启用，显示插件列表。")]
        public bool List { get; set; }

        [Arg("d", IsSwitch = true)]
        [Help("若启用，显示插件列表时仅显示禁用项。")]
        public bool DisableOnly { get; set; }
        [Arg("e", IsSwitch = true)]
        [Help("若启用，显示插件列表时仅显示启用项。")]
        public bool EnableOnly { get; set; }

        [Arg("enable")]
        [Help("启用指定的插件。")]
        public string DisabledPlugin { get; set; }

        [Arg("disable")]
        [Help("禁用指定的插件。")]
        public string EnabledPlugin { get; set; }

        private CommonMessage _cm;

        public override void Initialize(string[] args)
        {
            LoadDisableSettings();
        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            _cm = messageObj;

            if (_cm.PermissionLevel == PermissionLevel.Public && _cm.MessageType == MessageType.Group)
                return new CommonMessageResponse(LoliReply.AdminOnly, _cm);
            if (List)
                return ShowPluginList();
            if (DisabledPlugin != null)
                return EnablePlugin();
            if (EnabledPlugin != null)
                return DisablePlugin();

            return new CommonMessageResponse(LoliReply.ParamError, _cm);
        }

        private CommonMessageResponse EnablePlugin()
        {
            switch (_cm.MessageType)
            {
                case MessageType.Private:
                    {
                        var list = MessageHandler.PrivateDisabledList[long.Parse(_cm.UserId)];
                        foreach (var item in list)
                        {
                            if (item != DisabledPlugin) continue;
                            list.Remove(item);
                            SaveDisableSettings();
                            return new CommonMessageResponse($"已经启用插件 \"{item}\"", _cm);
                        }
                    }

                    break;
                case MessageType.Discuss:
                    {
                        var list = MessageHandler.DiscussDisabledList[long.Parse(_cm.DiscussId)];
                        foreach (var item in list)
                        {
                            if (item != DisabledPlugin) continue;
                            list.Remove(item);
                            SaveDisableSettings();
                            return new CommonMessageResponse($"已经启用插件 \"{item}\"", _cm);
                        }
                    }

                    break;
                case MessageType.Group:
                    {
                        var list = MessageHandler.GroupDisabledList[long.Parse(_cm.GroupId)];
                        foreach (var item in list)
                        {
                            if (item != DisabledPlugin) continue;
                            list.Remove(item);
                            SaveDisableSettings();
                            return new CommonMessageResponse($"已经启用用插件 \"{item}\"", _cm);
                        }
                    }

                    break;
            }
            return new CommonMessageResponse($"找不到指定插件 \"{DisabledPlugin}\"...", _cm);
        }

        private CommonMessageResponse DisablePlugin()
        {
            List<string> list, listCmd;
            List<ApplicationPlugin> listApp;
            List<ServicePlugin> listSvc;
            switch (_cm.MessageType)
            {
                case MessageType.Private:
                    {
                        list = MessageHandler.PrivateDisabledList[long.Parse(_cm.UserId)];
                        listCmd = Bot.PluginManager.CommandMap.Values.Distinct().Select(item => item.Name).ToList();
                        listApp = Bot.PluginManager.ApplicationList;
                        listSvc = Bot.PluginManager.ServiceList;
                    }

                    break;
                case MessageType.Discuss:
                    {
                        list = MessageHandler.DiscussDisabledList[long.Parse(_cm.DiscussId)];
                        listCmd = Bot.PluginManager.CommandMap.Values.Distinct().Select(item => item.Name).ToList();
                        listApp = Bot.PluginManager.ApplicationList;
                        listSvc = Bot.PluginManager.ServiceList;
                    }

                    break;
                case MessageType.Group:
                default:
                    {
                        list = MessageHandler.GroupDisabledList[long.Parse(_cm.GroupId)];
                        listCmd = Bot.PluginManager.CommandMap.Values.Distinct().Select(item => item.Name).ToList();
                        listApp = Bot.PluginManager.ApplicationList;
                        listSvc = Bot.PluginManager.ServiceList;
                    }

                    break;
            }

            foreach (var item in listCmd)
            {
                if (item != EnabledPlugin) continue;
                if (list.Contains(item))
                    return new CommonMessageResponse("此插件已被禁用过了…", _cm);
                list.Add(item);
                SaveDisableSettings();
                return new CommonMessageResponse($"已经禁用插件 \"{item}\"", _cm);
            }

            foreach (var item in listApp)
            {
                var t = item.GetType();
                if (t.Name != EnabledPlugin) continue;
                if (list.Contains(t.Name))
                    return new CommonMessageResponse("此插件已被禁用过了…", _cm);
                list.Add(t.Name);
                SaveDisableSettings();
                return new CommonMessageResponse($"已经禁用插件 \"{t.Name}\"", _cm);
            }

            foreach (var item in listSvc)
            {
                var t = item.GetType();
                if (t.Name != EnabledPlugin) continue;
                if (list.Contains(t.Name))
                    return new CommonMessageResponse("此插件已被禁用过了…", _cm);
                list.Add(t.Name);
                SaveDisableSettings();
                return new CommonMessageResponse($"已经禁用插件 \"{t.Name}\"", _cm);
            }

            return new CommonMessageResponse($"找不到指定插件 \"{DisabledPlugin}\"...", _cm);
        }

        private void SaveDisableSettings()
        {
            SaveSettings(MessageHandler.GroupDisabledList, "GroupDisabledList");
            SaveSettings(MessageHandler.DiscussDisabledList, "DiscussDisabledList");
            SaveSettings(MessageHandler.PrivateDisabledList, "PrivateDisabledList");
        }

        private void LoadDisableSettings()
        {
            MessageHandler.GroupDisabledList =
                LoadSettings<ConcurrentDictionary<long, List<string>>>("GroupDisabledList");
            MessageHandler.DiscussDisabledList =
                LoadSettings<ConcurrentDictionary<long, List<string>>>("DiscussDisabledList");
            MessageHandler.PrivateDisabledList =
                LoadSettings<ConcurrentDictionary<long, List<string>>>("PrivateDisabledList");
        }

        private CommonMessageResponse ShowPluginList()
        {
            var dicPlugin = new Dictionary<string, List<PluginInfo>>();
            const string cmdKey = "命令插件", svcKey = "服务插件", appKey = "应用插件";

            var commandMap = Bot.PluginManager.CommandMapStatic.Values.Distinct();

            dicPlugin.Add(cmdKey, new List<PluginInfo>());
            dicPlugin.Add(svcKey, new List<PluginInfo>());
            dicPlugin.Add(appKey, new List<PluginInfo>());

            foreach (var item in commandMap)
                dicPlugin[cmdKey].Add(new PluginInfo(item.Name, item.GetType().Name));

            foreach (var item in Bot.PluginManager.ServiceList)
                dicPlugin[svcKey].Add(new PluginInfo(item.Name, item.GetType().Name));

            foreach (var item in Bot.PluginManager.ApplicationList)
                dicPlugin[appKey].Add(new PluginInfo(item.Name, item.GetType().Name));

            var sb = new StringBuilder();
            switch (_cm.MessageType)
            {
                case MessageType.Discuss:
                    sb.AppendLine(MessageHandler.DiscussInfo[long.Parse(_cm.IdentityId)].Name + " 的插件情况：");
                    foreach (var item in MessageHandler.DiscussDisabledList[long.Parse(_cm.IdentityId)])
                    {
                        var pluginInfos = dicPlugin[cmdKey].Concat(dicPlugin[svcKey]).Concat(dicPlugin[appKey]);
                        foreach (var i in pluginInfos)
                        {
                            if (i.TypeName != item) continue;
                            i.IsEnabled = false;
                            break;
                        }
                    }

                    break;
                case MessageType.Private:
                    sb.AppendLine(_cm.IdentityId + " 的插件情况：");
                    foreach (var item in MessageHandler.PrivateDisabledList[long.Parse(_cm.IdentityId)])
                    {
                        var pluginInfos = dicPlugin[cmdKey].Concat(dicPlugin[svcKey]).Concat(dicPlugin[appKey]);
                        foreach (var i in pluginInfos)
                            if (i.TypeName == item)
                                i.IsEnabled = false;
                    }

                    break;
                case MessageType.Group:
                    sb.AppendLine(MessageHandler.GroupInfo[long.Parse(_cm.IdentityId)].Info.GroupName + " 的插件情况：");
                    foreach (var item in MessageHandler.GroupDisabledList[long.Parse(_cm.IdentityId)])
                    {
                        var pluginInfos = dicPlugin[cmdKey].Concat(dicPlugin[svcKey]).Concat(dicPlugin[appKey]);
                        foreach (var i in pluginInfos)
                            if (i.TypeName == item)
                                i.IsEnabled = false;
                    }

                    break;
            }

            string tmpKey = "";
            foreach (var item in dicPlugin)
            {
                if (tmpKey != item.Key)
                {
                    tmpKey = item.Key;
                    sb.AppendLine(tmpKey + ": ");
                }

                IEnumerable<PluginInfo> items;
                if (DisableOnly)
                    items = item.Value.Where(c => !c.IsEnabled);
                else if (EnableOnly)
                    items = item.Value.Where(c => c.IsEnabled);
                else
                    items = item.Value;

                var words = items.Select(c => $"  {c.FriendlyName} ({c.TypeName}){(c.IsEnabled ? "" : " (已禁用)")}");

                sb.AppendLine(string.Join("\r\n", words));
            }

            sb.Append("（为避免消息过长，本条消息为私聊发送）");
            SendMessage(new CommonMessageResponse(sb.ToString().Trim('\n').Trim('\r'), _cm.UserId), null, null, MessageType.Private);
            return null;
        }

        private class PluginInfo
        {
            public string FriendlyName { get; set; }
            public string TypeName { get; set; }
            public bool IsEnabled { get; set; } = true;

            public PluginInfo(string friendlyName, string typeName)
            {
                FriendlyName = friendlyName;
                TypeName = typeName;
            }
        }
    }
}
