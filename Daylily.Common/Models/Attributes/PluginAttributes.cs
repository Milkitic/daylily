using System;
using Daylily.Common.Models.Enum;

namespace Daylily.Common.Models.Attributes
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
        public AuthorAttribute(string author)
        {
            Author = author;
        }
        public string Author { get; }
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
        public PermissionLevel HelpType { get; set; } = PermissionLevel.Public;
        public HelpAttribute(params string[] helps)
        {
            Helps = helps;
        }
    }
}
