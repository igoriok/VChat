using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkLongPollData
    {
        [JsonProperty(PropertyName = "failed")]
        public int? Failed { get; set; }

        [JsonProperty(PropertyName = "ts")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "updates")]
        public object[] Updates { get; set; }
    }
}