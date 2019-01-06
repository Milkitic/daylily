using Daylily.Bot.Message;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Daylily.Bot;
using Daylily.Bot.Backend;

namespace Daylily.Plugin.Kernel
{
    public class PluginSwitch : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("20e7a3e1-fdc3-4b3a-bff1-ecf396a63311");

        public ConcurrentDictionary<long, List<string>> GroupDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();
        public ConcurrentDictionary<long, List<string>> DiscussDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();
        public ConcurrentDictionary<long, List<string>> PrivateDisabledList { get; set; } =
            new ConcurrentDictionary<long, List<string>>();

        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig
        {
            CanDisabled = false
        };

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMessageObj)
        {
            return null;
        }

        public bool ValidateDisabled(CoolQRouteMessage cm, MemberInfo t)
        {
            switch (cm.MessageType)
            {
                case MessageType.Group:
                    if (!GroupDisabledList.Keys.Contains(long.Parse(cm.GroupId)))
                    {
                        GroupDisabledList.TryAdd(long.Parse(cm.GroupId), new List<string>());
                    }

                    if (GroupDisabledList[long.Parse(cm.GroupId)].Contains(t.Name))
                        return true;
                    break;
                case MessageType.Private:
                    if (!PrivateDisabledList.Keys.Contains(long.Parse(cm.UserId)))
                    {
                        PrivateDisabledList.TryAdd(long.Parse(cm.UserId), new List<string>());
                    }

                    if (PrivateDisabledList[long.Parse(cm.UserId)].Contains(t.Name))
                        return true;
                    break;
                case MessageType.Discuss:
                    if (!DiscussDisabledList.Keys.Contains(long.Parse(cm.DiscussId)))
                    {
                        DiscussDisabledList.TryAdd(long.Parse(cm.DiscussId), new List<string>());
                    }

                    if (DiscussDisabledList[long.Parse(cm.DiscussId)].Contains(t.Name))
                        return true;
                    break;
            }

            return false;
        }

    }
}
