using Daylily.Bot;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using Daylily.TuLing;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Daylily.Plugin.Fun
{
    public class TuLingApp : CoolQApplicationPlugin
    {
        public override Guid Guid { get; } = new Guid("14e60bec-dc11-45da-8654-baed42588745");

        private static readonly TuLingClient Client = new TuLingClient();

        private ConcurrentDictionary<string, int> _apiTimes;

        public override void OnInitialized(StartupConfig startup)
        {
            _apiTimes = LoadSettings<ConcurrentDictionary<string, int>>() ??
                    new ConcurrentDictionary<string, int>(TuLingSecret.ApiKeys.ToDictionary(k => k, k => 0));
            foreach (var key in TuLingSecret.ApiKeys)
            {
                if (_apiTimes.ContainsKey(key)) continue;
                _apiTimes.TryAdd(key, 0);
            }

            Task.Factory.StartNew(() =>
            {
                var now = DateTime.Now;
                var next = DateTime.Now.AddDays(1);
                var dateTime = new DateTime(next.Year, next.Month, next.Day);
                bool ok = false;
                while (true)
                {
                    if (!ok)
                    {
                        Thread.Sleep(dateTime - now);
                        ok = true;
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromDays(1));
                    }

                    foreach (var apiTime in _apiTimes)
                    {
                        _apiTimes[apiTime.Key] = 0;
                    }

                    SaveConfig();
                }
            }, TaskCreationOptions.LongRunning);

            SaveConfig();
        }

        private void SaveConfig()
        {
            SaveSettings(_apiTimes);
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var rnd = StaticRandom.NextDouble();
            if (rnd >= 100f * 5 / 30000) return null;

            var routeMsg = scope.RouteMessage;
            var apis = _apiTimes.Where(k => k.Value < 100).Select(k => k.Key).ToArray();
            var api = apis[StaticRandom.Next(apis.Length)];
            var response = Client.SendText(api, routeMsg.RawMessage, routeMsg.UserId, routeMsg.GroupId);
            _apiTimes[api]++;
            if (response.Intent.Code == 4003)
                _apiTimes[api] = 100;
            SaveConfig();
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
