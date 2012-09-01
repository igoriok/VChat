using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkAttachment
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "photo")]
        public VkPhoto Photo { get; set; }

        [JsonProperty(PropertyName = "video")]
        public VkVideo Video { get; set; }

        [JsonProperty(PropertyName = "audio")]
        public VkAudio Audio { get; set; }

        [JsonProperty(PropertyName = "doc")]
        public VkDocument Document { get; set; }

        [JsonProperty(PropertyName = "wall")]
        public VkWall Wall { get; set; }
    }
}