using System.Data.Linq.Mapping;

namespace VChat.Models
{
    public class WallAudioAttachment : WallAttachment
    {
        [Column]
        public int AudioId { get; set; }

        [Association]
        public Audio Audio { get; set; }
    }
}