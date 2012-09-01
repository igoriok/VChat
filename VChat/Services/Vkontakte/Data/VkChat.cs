using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkChat
    {
        [JsonProperty(PropertyName = "chat_id")]
        public int ChatId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "admin_id")]
        public int AdminId { get; set; }

        [JsonProperty("users")]
        public int[] Users { get; set; }
    }
}