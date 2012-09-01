using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkPostSource
    {
        [JsonProperty(PropertyName = "type")]
        public PostSourceType Type { get; set; }

        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }
    }
}