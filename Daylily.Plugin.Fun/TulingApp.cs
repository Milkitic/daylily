using System;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;

namespace Daylily.Plugin.Fun
{
    class TuLingApp:CoolQApplicationPlugin
    {
        public override Guid Guid { get; } = new Guid("14e60bec-dc11-45da-8654-baed42588745");

        private const string TuLingApiUri = "http://openapi.tuling123.com/openapi/api/v2";

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            throw new NotImplementedException();
        }
    }
}
