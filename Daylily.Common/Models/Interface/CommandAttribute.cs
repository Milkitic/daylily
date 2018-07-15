using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Common.Models.Interface
{
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string command)
        {
            Command = command;

        }
        public string Command { get; }
    }
}
