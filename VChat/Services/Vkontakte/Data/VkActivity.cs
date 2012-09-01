using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkActivity
    {
        [JsonProperty(PropertyName = "online")]
        public bool IsOnline { get; set; }

        [JsonProperty(PropertyName = "time")]
        public long DateTime { get; set; }
    }
}