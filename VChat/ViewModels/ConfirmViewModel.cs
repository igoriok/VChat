using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using NotifyPropertyWeaver;

using VChat.Mvvm;
using VChat.Services.Token;
using VChat.Services.Vkontakte;

namespace VChat.ViewModels
{
    public class ConfirmViewModel : Screen, IBusyState
    {
        private readonly INavigationService _navigation;
        private readonly IVkClient _client;
        private readonly ITokenProvider _token;

        [NotifyProperty]
        public string PhoneNumber { get; set; }

        [NotifyProperty]
        public string Sid { get; set; }

        [NotifyProperty]
        public string Code { get; set; }

        [NotifyProperty]
        public string Password { get; set; }

        [NotifyProperty]
        public bool IsBusy { get; set; }

        [NotifyProperty]
        public string Status { get; set; }

        [DependsOn("PhoneNumber", "Code", "Password")]
        public bool CanConfirm
        {
            get
            {
                return
                    !string.IsNullOrWhiteSpace(Code) &&
                    !string.IsNullOrWhiteSpace(Password) &&
                    Password.Length > 5;
            }
        }

        public ConfirmViewModel(INavigationService navigation, IVkClient client, ITokenProvider token)
        {
            _navigation = navigation;
            _client = client;
            _token = token;
        }

        public void Confirm()
        {
            var phone = PhoneNumber;
            var code = Code;
            var password = Password;

            this.StartBusy("registering...");

            _client
                .Confirm(phone, code, password)
                .ObserveOnDispatcher()
                .Finally(this.StopBusy)
                .Subscribe(uid =>
                {
                    this.StartBusy("authorizing...");

                    _client
                        .Authenticate(phone, password)
                        .Do(_token.SetToken)
                        .ObserveOnDispatcher()
                        .Finally(this.StopBusy)
                        .Subscribe(token => new UriBuilder<SigninViewModel>()
                            .AttachTo(_navigation)
                            .Navigate(), ExceptionHelper.HandleException);
                }, ExceptionHelper.HandleException);
        }
    }
}