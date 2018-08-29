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

        public bool Equals(Identity other) => Id == other.Id && Type == other.Type;

        public override bool Equals(object obj) => !(obj is null) && obj is Identity identity && Equals(identity);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ (int)Type;
            }
        }

        public static bool operator !=(Identity i1, Identity i2) => !i1.Equals(i2);

        public static bool operator ==(Identity i1, Identity i2) => i1.Equals(i2);
    }
}
