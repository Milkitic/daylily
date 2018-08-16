using Newtonsoft.Json;

namespace Daylily.Osu.Models
{
    public class Beatmapsets
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "artist")]
        public string Artist { get; set; }
        [JsonProperty(PropertyName = "favourite_count")]
        public int FavouriteCount { get; set; }
        [JsonProperty(PropertyName = "covers")]
        public Covers Covers { get; set; }
        [JsonProperty(PropertyName = "preview_url")]
        public string PreviewUrl { get; set; }
    }

    public class Covers
    {
        [JsonProperty(PropertyName = "list@2x")]
        public string List2XUrl { get; set; }
    }
}
