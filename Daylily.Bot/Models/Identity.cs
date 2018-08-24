using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Bot.Enum;

namespace Daylily.Bot.Models
{
    public struct Identity
    {
        public long Id { get; }
        public MessageType Type { get; }
        public Identity(long id, MessageType type) : this()
        {
            Id = id;
            Type = type;
        }

        public Identity(string id, MessageType type) : this()
        {
            Id = long.Parse(id);
            Type = type;
        }
    }
}
