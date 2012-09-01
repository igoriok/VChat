using Caliburn.Micro;
using NotifyPropertyWeaver;

using VChat.ViewModels.Data;

namespace VChat.ViewModels
{
    public class MessageInfoViewModel : Screen
    {
        [NotifyProperty]
        public int MessageId { get; set; }

        [NotifyProperty]
        public MessageViewModel Message { get; set; }
    }
}