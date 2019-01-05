using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Backend.Plugins;
using Daylily.Bot.Message;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daylily.Plugin.Basic
{
    [Name("插件管理")]
    [Author("yf_extension")]
    [Version(2, 1, 1, PluginVersion.Alpha)]
    [Help("动态管理插件的启用状态。", "仅限当前群生效。", Authority = Authority.Admin)]
    [Command("plugin")]
    public class PluginManage : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("1888139a-860d-41f6-8684-639b2b6923e9");

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
        private CoolQIdentity _identity;
        private IEnumerable<MessagePlugin> _plugins;
        private List<Guid> _disabled;

        public static ConcurrentDictionary<CoolQIdentity, List<Guid>> DisabledList { get; set; } =
            new ConcurrentDictionary<CoolQIdentity, List<Guid>>();

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            Priority = int.MaxValue
        };

        public override bool CanDisabled => false;

        public override void OnInitialized(string[] args)
        {
            LoadDisableSettings();
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            _identity = (CoolQIdentity)_routeMsg.Identity;
            _routeMsg = routeMsg;
            _plugins = ((IEnumerable<MessagePlugin>)PluginManager.Current.Plugins.Where(k => k is MessagePlugin))
                .Where(k => k.CanDisabled);
            _disabled = DisabledList[_identity];

            if (_routeMsg.Authority == Authority.Public && _routeMsg.MessageType == MessageType.Group)
                return _routeMsg.ToSource(DefaultReply.AdminOnly);
            if (List)
                return ShowPluginList();
            if (DisabledPlugin != null)
                return EnablePlugin();
            if (EnabledPlugin != null)
                return DisablePlugin();

            return _routeMsg.ToSource(DefaultReply.ParamError);
        }

        private CoolQRouteMessage EnablePlugin()
        {
            var plugin = _plugins.FirstOrDefault(k => k.Name == DisabledPlugin);
            if (plugin == null)
            {
                return _routeMsg.ToSource($"指定插件 \"{DisabledPlugin}\" 不存在.");
            }

            foreach (var item in _disabled)
            {
                if (item != plugin.Guid) continue;
                _disabled.Remove(item);
                SaveDisableSettings();
                return _routeMsg.ToSource($"已经启用插件 \"{item}\".");
            }

            return _routeMsg.ToSource($"指定插件 \"{DisabledPlugin}\" 未被禁用.");
        }

        private CoolQRouteMessage DisablePlugin()
        {
            var disabled = DisabledList[_identity];

            foreach (var plugin in _plugins)
            {
                if (plugin.Name != EnabledPlugin) continue;
                if (disabled.Contains(plugin.Guid))
                    return _routeMsg.ToSource($"指定插件 \"{EnabledPlugin}\" 已被禁用.");
                disabled.Add(plugin.Guid);
                SaveDisableSettings();
                return _routeMsg.ToSource($"已经禁用插件 \"{plugin.Name}\".");
            }

            return _routeMsg.ToSource($"指定插件 \"{EnabledPlugin}\" 不存在.");
        }

        private void SaveDisableSettings()
        {
            SaveSettings(DisabledList, "DisabledList");
        }

        private void LoadDisableSettings()
        {
            DisabledList =
                LoadSettings<ConcurrentDictionary<CoolQIdentity, List<Guid>>>("DisabledList") ??
                new ConcurrentDictionary<CoolQIdentity, List<Guid>>();

        }

        private CoolQRouteMessage ShowPluginList()
        {
            var sb = new StringBuilder();
            sb.AppendLine(CoolQDispatcher.Current.SessionInfo[(CoolQIdentity)_routeMsg.Identity].Name + " 的插件情况：");
            var pluginGroup = _plugins.GroupBy(k => k.PluginType);

            var disabled = _plugins.Where(k => _disabled.Contains(k.Guid)).ToArray();

            foreach (var plugins in pluginGroup)
            {
                sb.AppendLine($"{plugins.Key.ToBotString()}：");
                foreach (var plugin in plugins)
                {
                    if (DisableOnly && !disabled.Contains(plugin) ||
                        EnableOnly && disabled.Contains(plugin))
                        continue;
                    sb.AppendLine(string.Format("  {0} {1}", plugin.Name, disabled.Contains(plugin) ? " (已禁用)" : ""));
                }
            }

            sb.Append("（为避免消息过长，本条消息为私聊发送）");
            SendMessage(new CoolQRouteMessage(sb.ToString(), new CoolQIdentity(_routeMsg.UserId, MessageType.Private)));
            return null;
        }

    }
}
