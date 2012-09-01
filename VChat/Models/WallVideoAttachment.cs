using System.Data.Linq.Mapping;

namespace VChat.Models
{
    public class WallVideoAttachment : WallAttachment
    {
        [Column]
        public int VideoId { get; set; }

        [Association]
        public Video Video { get; set; }
    }
}