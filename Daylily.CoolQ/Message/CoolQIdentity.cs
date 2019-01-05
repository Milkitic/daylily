using Daylily.Bot.Message;
using Daylily.Bot.Session;
using Newtonsoft.Json;

namespace Daylily.CoolQ.Message
{
    public struct CoolQIdentity : ISessionIdentity
    {
        [JsonProperty("id")]
        public long Id
        {
            get => long.Parse(Identity);
            set => Identity = value.ToString();
        }

        [JsonProperty("type")]
        public MessageType Type
        {
            get => System.Enum.Parse<MessageType>(SessionType);
            set => SessionType = value.ToString();
        }

        [JsonIgnore]
        public string Identity { get; set; }
        [JsonIgnore]
        public string SessionType { get; set; }

        public CoolQIdentity(long id, MessageType type) : this()
        {
            Id = id;
            Type = type;
        }

        public CoolQIdentity(string id, MessageType type) : this()
        {
            Id = long.Parse(id);
            Type = type;
        }

        public bool Equals(CoolQIdentity other) => Id == other.Id && Type == other.Type;

        public override bool Equals(object obj) => !(obj is null) && obj is CoolQIdentity identity && Equals(identity);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ (int)Type;
            }
        }

        public static bool operator !=(CoolQIdentity i1, CoolQIdentity i2) => !i1.Equals(i2);

        public static bool operator ==(CoolQIdentity i1, CoolQIdentity i2) => i1.Equals(i2);
    }
}
