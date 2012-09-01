using System.Data.Linq.Mapping;

namespace VChat.Models
{
    public class MessagePhotoAttachment : MessageAttachment
    {
        [Column]
        public int PhotoId { get; set; }

        [Association]
        public Photo Photo { get; set; }
    }
}