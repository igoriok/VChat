using Caliburn.Micro;
using NotifyPropertyWeaver;

using VChat.ViewModels.Data;

namespace VChat.ViewModels
{
    public class ChatInfoViewModel : Screen
    {
        [NotifyProperty]
        public int ChatId { get; set; }

        [NotifyProperty]
        public ConversationViewModel Chat { get; set; }
    }
}