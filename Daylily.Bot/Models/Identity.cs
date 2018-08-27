using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Bot.Enum;
using Newtonsoft.Json;

namespace Daylily.Bot.Models
{
    public struct Identity
    {
        [JsonProperty("id")] public long Id { get; set; }
        [JsonProperty("type")] public MessageType Type { get; set; }

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
