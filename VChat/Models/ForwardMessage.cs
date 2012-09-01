using System;
using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    public class ForwardMessage
    {
        [Column]
        public int MessageId { get; set; }

        [Association(ThisKey = "MessageId", OtherKey = "Id")]
        public Message Message { get; set; }

        [Column]
        public int UserId { get; set; }

        [Association(ThisKey = "UserId", OtherKey = "Id")]
        public User User { get; set; }

        [Column]
        public DateTime Timestamp { get; set; }

        [Column(DbType = "NTEXT", UpdateCheck = UpdateCheck.Never)]
        public string Body { get; set; }
    }
}