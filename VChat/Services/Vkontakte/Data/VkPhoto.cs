using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkPhoto
    {
        [JsonProperty(PropertyName = "pid")]
        public int PictureId { get; set; }

        [JsonProperty(PropertyName = "owner_id")]
        public int OwnerId { get; set; }

        [JsonProperty(PropertyName = "src")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "src_big")]
        public string SourceBig { get; set; }

        [JsonProperty(PropertyName = "src_small")]
        public string SourceSmall { get; set; }

        [JsonProperty(PropertyName = "created")]
        public long Created { get; set; }
    }
}