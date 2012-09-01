using System.Data.Linq.Mapping;

namespace VChat.Models
{
    public class MessageWallAttachment : MessageAttachment
    {
        [Column]
        public int WallId { get; set; }

        [Association]
        public Wall Wall { get; set; }
    }
}