using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkLikes
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "user_likes")]
        public bool IsUserLikes { get; set; }

        [JsonProperty(PropertyName = "can_like")]
        public bool CanLike { get; set; }

        [JsonProperty(PropertyName = "can_publish")]
        public bool CanPublish { get; set; }
    }
}