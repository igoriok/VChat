using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    public class Group
    {
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public string Photo { get; set; }
    }
}