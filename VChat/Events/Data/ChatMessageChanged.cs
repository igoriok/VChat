using VChat.Models;

namespace VChat.Events.Data
{
    public class ChatMessageChanged
    {
        public int ChatId { get; set; }

        public Message Old { get; set; }
        public Message New { get; set; }
    }
}