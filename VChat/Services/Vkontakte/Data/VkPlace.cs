using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkPlace
    {
        [JsonProperty(PropertyName = "place_id")]
        public string PlaceId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "country_id")]
        public int CountryId { get; set; }

        [JsonProperty(PropertyName = "city_id")]
        public int CityId { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }
    }
}