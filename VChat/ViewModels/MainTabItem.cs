using Caliburn.Micro;
using NotifyPropertyWeaver;
using VChat.Mvvm;

namespace VChat.ViewModels
{
    public class MainTabItem : Screen, IBusyState
    {
        [NotifyProperty]
        public string SearchPhrase { get; set; }

        [NotifyProperty]
        public bool IsBusy { get; set; }

        [NotifyProperty]
        public string Status { get; set; }
    }
}