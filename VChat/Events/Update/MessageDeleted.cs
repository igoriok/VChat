using VChat.Models;

namespace VChat.Events.Update
{
    public class MessageDeleted : Update
    {
        public int MessageId { get; set; }
    }
}