using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkDocument
    {
        [JsonProperty(PropertyName = "did")]
        public int DocumentId { get; set; }

        [JsonProperty(PropertyName = "owner_id")]
        public int OwnerId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        [JsonProperty(PropertyName = "ext")]
        public string Extension { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}