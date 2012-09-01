using Caliburn.Micro;
using Microsoft.Phone.Reactive;

using NotifyPropertyWeaver;

using VChat.Mvvm;
using VChat.Services.Vkontakte;

namespace VChat.ViewModels
{
    public class SignupViewModel : Screen, IBusyState
    {
        private readonly INavigationService _navigation;
        private readonly IVkClient _client;

        [NotifyProperty]
        public string PhoneNumber { get; set; }

        [NotifyProperty]
        public string FirstName { get; set; }

        [NotifyProperty]
        public string LastName { get; set; }

        [NotifyProperty]
        public bool IsBusy { get; set; }

        [NotifyProperty]
        public string Status { get; set; }

        [DependsOn("IsBusy", "PhoneNumber", "FirstName", "LastName")]
        public bool CanSignup
        {
            get
            {
                return
                    !IsBusy &&
                    !string.IsNullOrWhiteSpace(PhoneNumber) &&
                    !string.IsNullOrWhiteSpace(FirstName) &&
                    !string.IsNullOrWhiteSpace(LastName);
            }
        }

        public SignupViewModel(INavigationService navigation, IVkClient client)
        {
            _navigation = navigation;
            _client = client;
        }

        public void Signup()
        {
            var phone = PhoneNumber;
            var firstName = FirstName;
            var lastName = LastName;

            this.StartBusy("registering...");

            _client
                .Signup(phone, firstName, lastName)
                .ObserveOnDispatcher()
                .Finally(this.StopBusy)
                .Subscribe(
                    sid => _navigation.UriFor<ConfirmViewModel>()
                        .WithParam(vm => vm.PhoneNumber, phone)
                        .WithParam(vm => vm.Sid, sid)
                        .Navigate(),
                    ExceptionHelper.HandleException);
        }
    }
}