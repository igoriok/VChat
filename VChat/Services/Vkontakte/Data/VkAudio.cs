using System.Runtime.Serialization;

namespace VChat.Services.Vkontakte.Data
{
    [DataContract]
    public class VkAudio
    {
        [DataMember(Name = "aid")]
        public int AudioId { get; set; }

        [DataMember(Name = "owner_id")] 
        public int OwnerId { get; set; }

        [DataMember(Name = "performer")]
        public string Performer { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "duration")]
        public long Duration { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}