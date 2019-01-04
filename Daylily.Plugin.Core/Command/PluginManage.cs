using Daylily.Bot.Backend;
using Daylily.Bot.Backend.Plugins;
using Daylily.Bot.Enum;
using Daylily.Bot.Message;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daylily.Plugin.Core
{
    [Name("插件管理")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Alpha)]
    [Help("动态管理插件的启用状态。", "仅限当前群生效。", Authority = Authority.Admin)]
    [Command("plugin")]
    public class PluginManage : CoolQCommandPlugin
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

        private CoolQRouteMessage _routeMsg;

        public override void OnInitialized(string[] args)
        {
            LoadDisableSettings();
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            _routeMsg = routeMsg;

            if (_routeMsg.Authority == Authority.Public && _routeMsg.MessageType == MessageType.Group)
                return _routeMsg.ToSource(LoliReply.AdminOnly);
            if (List)
                return ShowPluginList();
            if (DisabledPlugin != null)
                return EnablePlugin();
            if (EnabledPlugin != null)
                return DisablePlugin();

            return _routeMsg.ToSource(LoliReply.ParamError);
        }

        private CoolQRouteMessage EnablePlugin()
        {
            switch (_routeMsg.MessageType)
            {
                case MessageType.Private:
                    {
                        var list = CoolQDispatcher.Current.PrivateDisabledList[long.Parse(_routeMsg.UserId)];
                        foreach (var item in list)
                        {
                            if (item != DisabledPlugin) continue;
                            list.Remove(item);
                            SaveDisableSettings();
                            return _routeMsg.ToSource($"已经启用插件 \"{item}\"");
                        }
                    }

                    break;
                case MessageType.Discuss:
                    {
                        var list = CoolQDispatcher.Current.DiscussDisabledList[long.Parse(_routeMsg.DiscussId)];
                        foreach (var item in list)
                        {
                            if (item != DisabledPlugin) continue;
                            list.Remove(item);
                            SaveDisableSettings();
                            return _routeMsg.ToSource($"已经启用插件 \"{item}\"");
                        }
                    }

                    break;
                case MessageType.Group:
                    {
                        var list = CoolQDispatcher.Current.GroupDisabledList[long.Parse(_routeMsg.GroupId)];
                        foreach (var item in list)
                        {
                            if (item != DisabledPlugin) continue;
                            list.Remove(item);
                            SaveDisableSettings();
                            return _routeMsg.ToSource($"已经启用用插件 \"{item}\"");
                        }
                    }

                    break;
            }
            return _routeMsg.ToSource($"找不到指定插件 \"{DisabledPlugin}\"...");
        }

        private CoolQRouteMessage DisablePlugin()
        {
            List<string> list, listCmd;
            List<ApplicationPlugin> listApp;
            List<ServicePlugin> listSvc;
            switch (_routeMsg.MessageType)
            {
                case MessageType.Private:
                    {
                        list = CoolQDispatcher.Current.PrivateDisabledList[long.Parse(_routeMsg.UserId)];
                        listCmd = PluginManager.CommandMap.Values.Distinct().Select(item => item.Name).ToList();
                        listApp = PluginManager.ApplicationList;
                        listSvc = PluginManager.ServiceList;
                    }

                    break;
                case MessageType.Discuss:
                    {
                        list = CoolQDispatcher.Current.DiscussDisabledList[long.Parse(_routeMsg.DiscussId)];
                        listCmd = PluginManager.CommandMap.Values.Distinct().Select(item => item.Name).ToList();
                        listApp = PluginManager.ApplicationList;
                        listSvc = PluginManager.ServiceList;
                    }

                    break;
                case MessageType.Group:
                default:
                    {
                        list = CoolQDispatcher.Current.GroupDisabledList[long.Parse(_routeMsg.GroupId)];
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
                    return _routeMsg.ToSource("此插件已被禁用过了…");
                list.Add(item);
                SaveDisableSettings();
                return _routeMsg.ToSource($"已经禁用插件 \"{item}\"");
            }

            foreach (var item in listApp)
            {
                var t = item.GetType();
                if (t.Name != EnabledPlugin) continue;
                if (list.Contains(t.Name))
                    return _routeMsg.ToSource("此插件已被禁用过了…");
                list.Add(t.Name);
                SaveDisableSettings();
                return _routeMsg.ToSource($"已经禁用插件 \"{t.Name}\"");
            }

            foreach (var item in listSvc)
            {
                var t = item.GetType();
                if (t.Name != EnabledPlugin) continue;
                if (list.Contains(t.Name))
                    return _routeMsg.ToSource("此插件已被禁用过了…");
                list.Add(t.Name);
                SaveDisableSettings();
                return _routeMsg.ToSource($"已经禁用插件 \"{t.Name}\"");
            }

            return _routeMsg.ToSource($"找不到指定插件 \"{DisabledPlugin}\"...");
        }

        private void SaveDisableSettings()
        {
            SaveSettings(CoolQDispatcher.Current.GroupDisabledList, "GroupDisabledList");
            SaveSettings(CoolQDispatcher.Current.DiscussDisabledList, "DiscussDisabledList");
            SaveSettings(CoolQDispatcher.Current.PrivateDisabledList, "PrivateDisabledList");
        }

        private void LoadDisableSettings()
        {
            CoolQDispatcher.Current.GroupDisabledList =
                LoadSettings<ConcurrentDictionary<long, List<string>>>("GroupDisabledList") ??
                new ConcurrentDictionary<long, List<string>>();
            CoolQDispatcher.Current.DiscussDisabledList =
                LoadSettings<ConcurrentDictionary<long, List<string>>>("DiscussDisabledList") ??
                new ConcurrentDictionary<long, List<string>>();
            CoolQDispatcher.Current.PrivateDisabledList =
                LoadSettings<ConcurrentDictionary<long, List<string>>>("PrivateDisabledList") ??
                new ConcurrentDictionary<long, List<string>>();
        }

        private CoolQRouteMessage ShowPluginList()
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
            sb.AppendLine(CoolQDispatcher.Current.SessionInfo[(CqIdentity)_routeMsg.Identity].Name + " 的插件情况：");
            switch (_routeMsg.MessageType)
            {
                case MessageType.Discuss:
                    foreach (var item in CoolQDispatcher.Current.DiscussDisabledList[((CqIdentity)_routeMsg.Identity).Id])
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
                    foreach (var item in CoolQDispatcher.Current.PrivateDisabledList[((CqIdentity)_routeMsg.Identity).Id])
                    {
                        var pluginInfos = dicPlugin[cmdKey].Concat(dicPlugin[svcKey]).Concat(dicPlugin[appKey]);
                        foreach (var i in pluginInfos)
                            if (i.TypeName == item)
                                i.IsEnabled = false;
                    }

                    break;
                case MessageType.Group:
                    foreach (var item in CoolQDispatcher.Current.GroupDisabledList[((CqIdentity)_routeMsg.Identity).Id])
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
            SendMessage(new CoolQRouteMessage(sb.ToString().Trim('\n').Trim('\r'), new CqIdentity(_routeMsg.UserId, MessageType.Private)));
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
