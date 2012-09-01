using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace VChat.Services.Vkontakte.Data
{
    [DataContract]
    public class VkWall
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "to_id")]
        public int ToId { get; set; }

        [JsonProperty(PropertyName = "from_id")]
        public int FromId { get; set; }

        [JsonProperty(PropertyName = "date")]
        public long Date { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "comments")]
        public VkComments Comments { get; set; }

        [JsonProperty(PropertyName = "likes")]
        public VkLikes Likes { get; set; }

        [JsonProperty(PropertyName = "reposts")]
        public VkReposts Reposts { get; set; }

        [JsonProperty(PropertyName =  "attachments")]
        public VkAttachment[] Attachments { get; set; }

        [JsonProperty(PropertyName = "geo")]
        public VkGeo Geo { get; set; }

        [JsonProperty(PropertyName = "post_source")]
        public VkPostSource PostSource { get; set; }

        [JsonProperty(PropertyName = "signer_id")]
        public string SignerId { get; set; }

        [JsonProperty(PropertyName = "copy_owner_id")]
        public string CopyOwnerId { get; set; }

        [JsonProperty(PropertyName = "copy_post_id")]
        public string CopyPostId { get; set; }

        [JsonProperty(PropertyName = "copy_text")]
        public string CopyText { get; set; }
    }
}