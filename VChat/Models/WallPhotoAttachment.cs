using System.Data.Linq.Mapping;

namespace VChat.Models
{
    public class WallPhotoAttachment : WallAttachment
    {
        [Column]
        public int PhotoId { get; set; }

        [Association]
        public Photo Photo { get; set; }
    }
}