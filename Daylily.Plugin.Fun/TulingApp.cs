using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using Daylily.TuLing;
using System;
using System.Linq;

namespace Daylily.Plugin.Fun
{
    public class TuLingApp : CoolQApplicationPlugin
    {
        public override Guid Guid { get; } = new Guid("14e60bec-dc11-45da-8654-baed42588745");

        private static readonly TuLingClient Client = new TuLingClient();

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var rnd = StaticRandom.NextDouble();
            if (rnd >= 100f / 30000) return null;

            var routeMsg = scope.RouteMessage;
            var response = Client.SendText(routeMsg.RawMessage, routeMsg.UserId, routeMsg.GroupId);
            if (response.Intent.Code != 10004)
                return null;
            var result = response.Results.FirstOrDefault();
            return
                result != null
                    ? routeMsg.ToSource(result.Values.Text)
                    : null;
        }
    }
}
