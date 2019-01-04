using System;

namespace Daylily.Bot
{
    public static class BuilderExtension
    {
        public static T Config<T>(this T middleware, Action<T> config) where T : IMiddleware
        {
            config?.Invoke(middleware);
            return middleware;
        }
    }
}