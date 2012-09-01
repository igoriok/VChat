using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    public class Chat
    {
        private readonly EntitySet<ChatUser> _users;
        private readonly EntitySet<Message> _messages;

        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column]
        public string Title { get; set; }

        [Association(ThisKey = "Id", OtherKey = "ChatId")]
        public IList<ChatUser> Users
        {
            get { return _users; }
            set { _users.Assign(value); }
        }

        public IList<Message> Messages
        {
            get { return _messages; }
            set { _messages.Assign(value); }
        }

        [Column]
        public int AdminId { get; set; }

        [Association(ThisKey = "OwnerId", OtherKey = "Id")]
        public User Admin { get; set; }

        public Chat()
        {
            _users = new EntitySet<ChatUser>();
            _messages = new EntitySet<Message>();
        }
    }
}