using System;

namespace Daylily.Common
{
    public static class Domain
    {
        public static string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory;
    }
}
