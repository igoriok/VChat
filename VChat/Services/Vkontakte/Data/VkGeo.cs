using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkGeo
    {
        [JsonProperty(PropertyName = "type")]
        public VkGeoType Type { get; set; }

        [JsonProperty(PropertyName = "coordinates")]
        public string Coordinates { get; set; }
    }
}