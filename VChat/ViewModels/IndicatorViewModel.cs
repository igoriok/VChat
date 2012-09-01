using Caliburn.Micro;
using NotifyPropertyWeaver;
using VChat.Mvvm;

namespace VChat.ViewModels
{
    public class IndicatorViewModel : PropertyChangedBase, IBusyState
    {
        [NotifyProperty]
        public bool IsBusy { get; set; }

        [NotifyProperty]
        public string Status { get; set; }

        public void Begin(string text)
        {
            Status = text;
            IsBusy = true;
        }

        public void End()
        {
            Status = null;
            IsBusy = false;
        }
    }
}