using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    public class Document
    {
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column]
        public int OwnerId { get; set; }

        [Column]
        public string Title { get; set; }

        [Column]
        public long Size { get; set; }

        [Column]
        public string Extension { get; set; }

        [Column]
        public string Url { get; set; }
    }
}