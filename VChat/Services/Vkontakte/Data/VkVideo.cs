using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkVideo
    {
        [JsonProperty(PropertyName = "vid")]
        public int VideoId { get; set; }

        [JsonProperty(PropertyName = "owner_id")]
        public int OwnerId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public long Duration { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "image_big")]
        public string ImageBig { get; set; }

        [JsonProperty(PropertyName = "image_small")]
        public string ImageSmall { get; set; }

        [JsonProperty(PropertyName = "views")]
        public int Views { get; set; }

        [JsonProperty(PropertyName = "date")]
        public long Date { get; set; }
    }
}