using System.Data.Linq.Mapping;

namespace VChat.Models
{
    public class WallWallAttachment : WallAttachment
    {
        [Column]
        public int WallWallId { get; set; }

        [Association]
        public Wall WallWall { get; set; }
    }
}