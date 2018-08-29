using System;
using System.IO;

namespace Daylily.Common
{
    public static class Domain
    {
        public static string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory;
        public static string CacheDirectory => Path.Combine(CurrentDirectory, "_cache");
        public static string CacheImageDirectory => Path.Combine(CacheDirectory, "_images");
    }
}
