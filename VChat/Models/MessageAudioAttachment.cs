using System.Data.Linq.Mapping;

namespace VChat.Models
{
    public class MessageAudioAttachment : MessageAttachment
    {
        [Column]
        public int AudioId { get; set; }

        [Association]
        public Audio Audio { get; set; }
    }
}