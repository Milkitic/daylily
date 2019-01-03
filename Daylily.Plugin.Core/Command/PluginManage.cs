using Daylily.Bot;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daylily.Plugin.Core
{
    [Name("插件管理")]
    [Author("yf_extension")]
    [Version(0, 1, 4, PluginVersion.Alpha)]
    [Help("动态管理插件的启用状态。", "仅限当前群生效。", HelpType = PermissionLevel.Admin)]
    [Command("plugin")]
    public class PluginManage : CommandPlugin
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

        public override void OnInitialized(string[] args)
        {
            LoadDisableSettings();
        }

        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
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
                        var list = CoolQDispatcher.PrivateDisabledList[long.Parse(_cm.UserId)];
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
                        var list = CoolQDispatcher.DiscussDisabledList[long.Parse(_cm.DiscussId)];
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
                        var list = CoolQDispatcher.GroupDisabledList[long.Parse(_cm.GroupId)];
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
                        list = CoolQDispatcher.PrivateDisabledList[long.Parse(_cm.UserId)];
                        listCmd = PluginManager.CommandMap.Values.Distinct().Select(item => item.Name).ToList();
                        listApp = PluginManager.ApplicationList;
                        listSvc = PluginManager.ServiceList;
                    }

                    break;
                case MessageType.Discuss:
                    {
                        list = CoolQDispatcher.DiscussDisabledList[long.Parse(_cm.DiscussId)];
                        listCmd = PluginManager.CommandMap.Values.Distinct().Select(item => item.Name).ToList();
                        listApp = PluginManager.ApplicationList;
                        listSvc = PluginManager.ServiceList;
                    }

                    break;
                case MessageType.Group:
                default:
                    {
                        list = CoolQDispatcher.GroupDisabledList[long.Parse(_cm.GroupId)];
                        listCmd = PluginManager.CommandMap.Values.Distinct().Select(item => item.Name).ToList();
                        listApp = PluginManager.ApplicationList;
                        listSvc = PluginManager.ServiceList;
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
            SaveSettings(CoolQDispatcher.GroupDisabledList, "GroupDisabledList");
            SaveSettings(CoolQDispatcher.DiscussDisabledList, "DiscussDisabledList");
            SaveSettings(CoolQDispatcher.PrivateDisabledList, "PrivateDisabledList");
        }

        private void LoadDisableSettings()
        {
            CoolQDispatcher.GroupDisabledList =
                LoadSettings<ConcurrentDictionary<long, List<string>>>("GroupDisabledList") ??
                new ConcurrentDictionary<long, List<string>>();
            CoolQDispatcher.DiscussDisabledList =
                LoadSettings<ConcurrentDictionary<long, List<string>>>("DiscussDisabledList") ??
                new ConcurrentDictionary<long, List<string>>();
            CoolQDispatcher.PrivateDisabledList =
                LoadSettings<ConcurrentDictionary<long, List<string>>>("PrivateDisabledList") ??
                new ConcurrentDictionary<long, List<string>>();
        }

        private CommonMessageResponse ShowPluginList()
        {
            var dicPlugin = new Dictionary<string, List<PluginInfo>>();
            const string cmdKey = "命令插件", svcKey = "服务插件", appKey = "应用插件";

            var commandMap = PluginManager.CommandMapStatic.Values.Distinct();

            dicPlugin.Add(cmdKey, new List<PluginInfo>());
            dicPlugin.Add(svcKey, new List<PluginInfo>());
            dicPlugin.Add(appKey, new List<PluginInfo>());

            foreach (var item in commandMap)
                dicPlugin[cmdKey].Add(new PluginInfo(item.Name, item.GetType().Name));

            foreach (var item in PluginManager.ServiceList)
                dicPlugin[svcKey].Add(new PluginInfo(item.Name, item.GetType().Name));

            foreach (var item in PluginManager.ApplicationList)
                dicPlugin[appKey].Add(new PluginInfo(item.Name, item.GetType().Name));

            var sb = new StringBuilder();
            sb.AppendLine(CoolQDispatcher.SessionInfo[_cm.CqIdentity].Name + " 的插件情况：");
            switch (_cm.MessageType)
            {
                case MessageType.Discuss:
                    foreach (var item in CoolQDispatcher.DiscussDisabledList[_cm.CqIdentity.Id])
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
                    foreach (var item in CoolQDispatcher.PrivateDisabledList[_cm.CqIdentity.Id])
                    {
                        var pluginInfos = dicPlugin[cmdKey].Concat(dicPlugin[svcKey]).Concat(dicPlugin[appKey]);
                        foreach (var i in pluginInfos)
                            if (i.TypeName == item)
                                i.IsEnabled = false;
                    }

                    break;
                case MessageType.Group:
                    foreach (var item in CoolQDispatcher.GroupDisabledList[_cm.CqIdentity.Id])
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
            SendMessage(new CommonMessageResponse(sb.ToString().Trim('\n').Trim('\r'), new CqIdentity(_cm.UserId, MessageType.Private)));
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
