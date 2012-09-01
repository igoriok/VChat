namespace VChat.Events.Update
{
    public class MessageFlagChanged : Update
    {
        public int MessageId { get; set; }
        public MessageFlag MessageFlag { get; set; }
    }
}