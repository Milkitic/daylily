using System;
using Daylily.Bot.Messaging;

namespace Daylily.Bot.Backend
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NameAttribute : Attribute
    {
        public NameAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorAttribute : Attribute
    {
        public AuthorAttribute(params string[] author)
        {
            Author = author;
        }
        public string[] Author { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class VersionAttribute : Attribute
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public PluginVersion PluginVersion { get; }

        public VersionAttribute(int major, int minor, int patch, PluginVersion pluginVersion)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            PluginVersion = pluginVersion;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class HelpAttribute : Attribute
    {
        public string[] Helps { get; }
        public Authority Authority { get; set; } = Authority.Public;
        public HelpAttribute(params string[] helps)
        {
            Helps = helps;
        }
    }
}
