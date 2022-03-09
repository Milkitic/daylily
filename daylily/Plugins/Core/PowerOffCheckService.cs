using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Configuration;
using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Core;

[PluginIdentifier("3d5d466c-e52c-4888-950c-5bcc36ac8294")]
public class PowerOffCheckService : ServicePlugin
{
    private readonly ShuntDownConfig _config;

    public PowerOffCheckService(IConfiguration<ShuntDownConfig> configuration)
    {
        _config = configuration.Instance;
    }

    public override async Task<bool> BeforeSend(PluginInfo pluginInfo, IResponse response)
    {
        var context = response.MessageContext;
        var messageIdentity = context?.MessageIdentity;
        if (messageIdentity == null)
        {
            return true;
        }

        DateTimeOffset? expireTime;
        using (await _config.AsyncLock.LockAsync())
        {
            if (!_config.ExpireTimeDictionary!.TryGetValue(messageIdentity, out expireTime))
            {
                return true;
            }
        }

        if (expireTime <= DateTimeOffset.Now)
        {
            return true;
        }

        if (context!.Authority > MessageAuthority.Public)
        {
            return true;
        }

        response.Handled();
        return false;
    }
}