using System;
using Daylily.Common.Models.Enum;

namespace Daylily.Common.Models.Attributes
{
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(params string[] commands)
        {
            Commands = commands;
        }
        public string[] Commands { get; }
    }

    public class NameAttribute : Attribute
    {
        public NameAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }

    public class AuthorAttribute : Attribute
    {
        public AuthorAttribute(string author)
        {
            Author = author;
        }
        public string Author { get; }
    }

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

    public class HelpAttribute : Attribute
    {
        public string[] Helps { get; }
        public HelpAttribute(params string[] helps)
        {
            Helps = helps;
        }
    }
}
