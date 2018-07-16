using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Common.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(params string[] commands)
        {
            Commands = commands;
        }
        public string[] Commands { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ArgAttribute : Attribute
    {
        public ArgAttribute(string name)
        {
            Name = name;
        }
        public char Abbreviate { get; set; }
        public string Name { get; set; }
        public bool IsSwitch { get; set; }
        public object Default { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FreeArgAttribute : Attribute
    {
        public object Default { get; set; }
    }
}
