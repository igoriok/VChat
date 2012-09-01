using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [JsonObject]
    public class VkMessage
    {
        [JsonProperty(PropertyName = "mid")]
        public int MessageId { get; set; }

        [JsonProperty(PropertyName = "date")]
        public long Date { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "from_id")]
        public int? FromId { get; set; }

        [JsonProperty(PropertyName = "read_state")]
        public bool IsRead { get; set; }

        [JsonProperty(PropertyName = "out")]
        public bool IsOut { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }

        [JsonProperty(PropertyName = "geo")]
        public VkGeo Geo { get; set; }

        [JsonProperty(PropertyName = "attachment")]
        public VkAttachment Attachment { get; set; }

        [JsonProperty(PropertyName = "attachments")]
        public VkAttachment[] Attachments { get; set; }

        [JsonProperty(PropertyName = "fwd_messages")]
        public VkMessage[] ForwardMessages { get; set; }

        [JsonProperty(PropertyName = "chat_id")]
        public int? ChatId { get; set; }

        [JsonProperty(PropertyName = "chat_active")]
        public string ChatActive { get; set; }

        [JsonProperty(PropertyName = "users_count")]
        public int? UsersCount { get; set; }

        [JsonProperty(PropertyName = "admin_id")]
        public int? AdminId { get; set; }

        [JsonProperty(PropertyName = "deleted")]
        public bool IsDeleted { get; set; }
    }
}