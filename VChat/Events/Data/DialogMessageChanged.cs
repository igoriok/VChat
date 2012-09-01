using VChat.Models;

namespace VChat.Events.Data
{
    public class DialogMessageChanged
    {
        public int UserId { get; set; }

        public Message Old { get; set; }
        public Message New { get; set; }
    }
}