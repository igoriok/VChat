using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    [Index(Columns = "UserId")]
    [Index(Columns = "ChatId")]
    public class Message
    {
        private readonly EntitySet<MessageAttachment> _attachments;
        private readonly EntitySet<ForwardMessage> _forwardMessages;

        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column]
        public DateTime Timestamp { get; set; }

        [Column]
        public int UserId { get; set; }

        [Association(ThisKey = "UserId", OtherKey = "Id")]
        public User User { get; set; }

        [Column]
        public int? ChatId { get; set; }

        [Association(ThisKey = "ChatId", OtherKey = "Id")]
        public Chat Chat { get; set; }

        [Column]
        public bool IsRead { get; set; }

        [Column]
        public bool IsOut { get; set; }

        [Column]
        public string Title { get; set; }

        [Column(DbType = "NTEXT", UpdateCheck = UpdateCheck.Never)]
        public string Body { get; set; }

        [Association(ThisKey = "Id", OtherKey = "MessageId")]
        public Geo Geo { get; set; }

        public IList<ForwardMessage> ForwardMessages
        {
            get { return _forwardMessages; }
            set { _forwardMessages.Assign(value); }
        }

        [Association(ThisKey = "Id", OtherKey = "MessageId")]
        public IList<MessageAttachment> Attachments
        {
            get { return _attachments; }
            set { _attachments.Assign(value); }
        }

        public Message()
        {
            _forwardMessages = new EntitySet<ForwardMessage>();
            _attachments = new EntitySet<MessageAttachment>();
        }
    }
}