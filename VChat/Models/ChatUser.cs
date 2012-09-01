using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    public class ChatUser
    {
        [Column]
        public int ChatId { get; set; }

        [Association(ThisKey = "ChatId", OtherKey = "Id")]
        public Chat Chat { get; set; }

        [Column]
        public int UserId { get; set; }

        [Association(ThisKey = "UserId", OtherKey = "Id")]
        public User User { get; set; }
    }
}