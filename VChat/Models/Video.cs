using System;
using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    public class Video
    {
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column]
        public int OwnerId { get; set; }

        [Column]
        public string Title { get; set; }

        [Column]
        public string Description { get; set; }

        [Column]
        public TimeSpan Duration { get; set; }

        [Column]
        public string Image { get; set; }

        [Column]
        public string ImageBig { get; set; }

        [Column]
        public string ImageSmall { get; set; }

        [Column]
        public int Views { get; set; }

        [Column]
        public DateTime Timestamp { get; set; }

        [Association(ThisKey = "Id", OtherKey = "VideoId")]
        public VideoFiles Files { get; set; }
    }
}