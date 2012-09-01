using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    public class Photo
    {
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column]
        public int OwnerId { get; set; }

        [Association]
        public Owner Owner { get; set; }

        [Column]
        public string Source { get; set; }

        [Column]
        public string SourceBig { get; set; }
    }
}