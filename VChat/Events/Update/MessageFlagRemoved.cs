namespace VChat.Events.Update
{
    public class MessageFlagRemoved : Update
    {
        public int MessageId { get; set; }
        public MessageFlag MessageFlag { get; set; }
        public int? UserId { get; set; }
    }
}