using VChat.Models;

namespace VChat.Events.Update
{
    public class MessageAdded : Update
    {
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}