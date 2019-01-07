using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Backend.Plugins;
using Daylily.Bot.Message;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daylily.Plugin.Kernel
{
    [Name("插件管理")]
    [Author("yf_extension")]
    [Version(2, 1, 3, PluginVersion.Alpha)]
    [Help("动态管理插件的启用状态。", "仅限当前群生效。", Authority = Authority.Admin)]
    [Command("plugin")]
    public class PluginSwitcher : CoolQCommandPlugin
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

        public static CoolQIdentityDictionary<List<Guid>> DisabledList { get; private set; } =
            new CoolQIdentityDictionary<List<Guid>>();

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            Priority = int.MaxValue,
            CanDisabled = false
        };

        public override void OnInitialized(string[] args)
        {
            LoadDisableSettings();
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            _routeMsg = routeMsg;
            _identity = (CoolQIdentity)_routeMsg.Identity;
            _plugins = PluginManager.Current.Plugins.OfType<MessagePlugin>()
                .Where(k => (k.MiddlewareConfig as BackendConfig)?.CanDisabled == true)
                .Where(k => k.TargetAuthority != Authority.Root);
            if (!DisabledList.ContainsKey(_identity))
                DisabledList.Add(_identity, new List<Guid>());

            _disabled = DisabledList[_identity];

            if (_routeMsg.CurrentAuthority == Authority.Public && _routeMsg.MessageType == MessageType.Group)
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
            var inputs = DisabledPlugin.Split(',');
            if (inputs.Length == 1)
            {
                MessagePlugin plugin = InnerGetPlugin(DisabledPlugin);
                if (plugin == null)
                    return _routeMsg.ToSource($"指定插件 \"{DisabledPlugin}\" 不存在.");

                if (!_disabled.Contains(plugin.Guid))
                    return _routeMsg.ToSource($"指定插件 \"{GetPluginString(plugin)}\" 未被禁用.");

                _disabled.Remove(_disabled.First(k => k == plugin.Guid));
                SaveDisableSettings();
                return _routeMsg.ToSource($"已经启用插件 \"{GetPluginString(plugin)}\".");
            }

            var sb = new StringBuilder();
            var plugins = inputs.Select(k => (k, InnerGetPlugin(k)));
            foreach (var (input, plugin) in plugins)
            {
                sb.Append(input + ": ");
                if (plugin == null)
                {
                    sb.AppendLine($"指定插件 \"{input}\" 不存在.");
                    continue;
                }
                if (!_disabled.Contains(plugin.Guid))
                {
                    sb.AppendLine($"指定插件 \"{GetPluginString(plugin)}\" 未被禁用.");
                    continue;
                }
                _disabled.Remove(_disabled.First(k => k == plugin.Guid));
                sb.AppendLine($"已经启用插件 \"{GetPluginString(plugin)}\".");
            }

            SaveDisableSettings();
            return _routeMsg.ToSource(sb.ToString().Trim('\n', '\r'));
        }

        private CoolQRouteMessage DisablePlugin()
        {
            var inputs = DisabledPlugin.Split(',');
            if (inputs.Length == 1)
            {
                MessagePlugin plugin = InnerGetPlugin(EnabledPlugin);
                if (plugin == null)
                    return _routeMsg.ToSource($"指定插件 \"{EnabledPlugin}\" 不存在.");

                if (_disabled.Contains(plugin.Guid))
                    return _routeMsg.ToSource($"指定插件 \"{GetPluginString(plugin)}\" 已被禁用.");

                _disabled.Add(plugin.Guid);
                SaveDisableSettings();
                return _routeMsg.ToSource($"已经禁用插件 \"{GetPluginString(plugin)}\".");
            }

            var sb = new StringBuilder();
            var plugins = inputs.Select(k => (k, InnerGetPlugin(k)));
            foreach (var (input, plugin) in plugins)
            {
                sb.Append(input + ": ");
                if (plugin == null)
                {
                    sb.AppendLine($"指定插件 \"{input}\" 不存在.");
                    continue;
                }
                if (!_disabled.Contains(plugin.Guid))
                {
                    sb.AppendLine($"指定插件 \"{GetPluginString(plugin)}\" 已被禁用.");
                    continue;
                }
                _disabled.Add(plugin.Guid);
                sb.AppendLine($"已经禁用插件 \"{GetPluginString(plugin)}\".");
            }

            SaveDisableSettings();
            return _routeMsg.ToSource(sb.ToString().Trim('\n', '\r'));
        }

        private void SaveDisableSettings()
        {
            SaveSettings(DisabledList, "DisabledList");
        }

        private void LoadDisableSettings()
        {
            DisabledList =
                LoadSettings<CoolQIdentityDictionary<List<Guid>>>("DisabledList") ??
                new CoolQIdentityDictionary<List<Guid>>();
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
                    sb.AppendLine(string.Format("  {0} {1}", GetPluginString(plugin), disabled.Contains(plugin) ? " (已禁用)" : ""));
                }
            }

            sb.Append("（为避免消息过长，本条消息为私聊发送）");
            SendMessage(new CoolQRouteMessage(sb.ToString(), new CoolQIdentity(_routeMsg.UserId, MessageType.Private)));
            return null;
        }

        private MessagePlugin InnerGetPlugin(string key)
        {
            return _plugins
                .FirstOrDefault(k =>
                    k.Name == key
                    || k.GetType().Name == key
                    || k is CommandPlugin cmd && cmd.Commands.Contains(key)
                );
        }

        private string GetPluginString(MessagePlugin plugin)
        {
            return
                string.Format("{0} ({1}){2}",
                    plugin.Name,
                    plugin.GetType().Name,
                    plugin is CommandPlugin cmd ? $" ({string.Join(", ", cmd.Commands)})" : "");
        }
    }
}
