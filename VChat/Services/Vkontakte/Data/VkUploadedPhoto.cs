using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkUploadedPhoto : VkPhoto
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }
}