using System;
using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    public class Audio
    {
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column]
        public int OwnerId { get; set; }

        [Column]
        public string Performer { get; set; }

        [Column]
        public string Title { get; set; }

        [Column]
        public TimeSpan Duration { get; set; }

        [Column]
        public string Url { get; set; }
    }
}