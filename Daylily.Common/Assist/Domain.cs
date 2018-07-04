using System;

namespace Daylily.Common.Assist
{
    public static class Domain
    {
        public static string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory;
    }
}
