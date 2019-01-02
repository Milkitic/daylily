using Daylily.Bot.Interface;
using System;

namespace Daylily.Bot
{
    public static class DispatcherBuilderExtension
    {
        public static IDispatcher Config(this IDispatcher dispatcher, Action<IDispatcher> config)
        {
            config?.Invoke(dispatcher);
            return dispatcher;
        }

        public static IFrontend Config(this IFrontend dispatcher, Action<IFrontend> config)
        {
            config?.Invoke(dispatcher);
            return dispatcher;
        }

    }
}