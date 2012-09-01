using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkLastSeen
    {
        [JsonProperty(PropertyName = "time")]
        public long Time { get; set; }
    }
}