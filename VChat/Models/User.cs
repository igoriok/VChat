using System;
using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    public class User
    {
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column]
        public string FirstName { get; set; }

        [Column]
        public string LastName { get; set; }

        [Column]
        public Sex Sex { get; set; }

        [Column]
        public string Photo { get; set; }

        [Column]
        public bool IsOnline { get; set; }

        [Column]
        public DateTime? LastSeen { get; set; }
    }
}