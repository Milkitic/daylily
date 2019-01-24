using Daylily.Bot;
using Daylily.Bot.Backend.Plugins;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daylily.Plugin.Kernel
{
    public class MessageSendingFilterSvc : ServicePlugin
    {
        public override Guid Guid { get; } = new Guid("365d4d8b-e5f3-4815-9503-a9416fc28af5");

        public override void OnInitialized(StartupConfig startup)
        {
            CoolQDispatcher.Current.OnMessageSending += Dispatcher_OnMessageSending;
        }

        private readonly CoolQIdentityDictionary<List<(string message, DateTime sentTime)>>
            _identityDictionary = new CoolQIdentityDictionary<List<(string message, DateTime sentTime)>>();

        private void Dispatcher_OnMessageSending(CoolQRouteMessage routeMsg)
        {
            var id = routeMsg.CoolQIdentity;
            if (!_identityDictionary.ContainsKey(id))
                _identityDictionary.Add(id, new List<(string, DateTime)>());
            else
            {
                _identityDictionary[id].RemoveAll(k => k.sentTime.AddMinutes(2) < DateTime.Now);
                if (_identityDictionary[id].Any(k => k.message == routeMsg.RawMessage))
                {
                    routeMsg.Canceled = true;
                }
                else
                {
                    _identityDictionary[id].Add((routeMsg.RawMessage, DateTime.Now));
                }
            }

        }
    }
}
