using System;
using System.ComponentModel;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using NotifyPropertyWeaver;

using VChat.Mvvm;
using VChat.Properties;
using VChat.Services.System;
using VChat.Services.Token;
using VChat.Services.Vkontakte;

namespace VChat.ViewModels
{
    public class SigninViewModel : Screen, IBusyState
    {
        private readonly INavigationService _navigation;
        private readonly IVkClient _client;
        private readonly ITokenProvider _token;

        private IDisposable _processing = Disposable.Empty;

        [NotifyProperty]
        public string Username { get; set; }

        [NotifyProperty]
        public string Password { get; set; }

        [NotifyProperty]
        public bool IsBusy { get; set; }

        [NotifyProperty]
        public string Status { get; set; }

        public bool CanLogin
        {
            get { return !IsBusy && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password); }
        }

        public SigninViewModel(INavigationService navigation, IVkClient client, ITokenProvider token)
        {
            _navigation = navigation;
            _client = client;
            _token = token;

            PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsBusy":
                case "Username":
                case "Password":

                    NotifyOfPropertyChange("CanLogin");
                    break;
            }
        }

        protected override void OnInitialize()
        {
            _navigation.ClearHistory();
        }

        protected override void OnDeactivate(bool close)
        {
            _processing.Dispose();
        }

        public void Login()
        {
            var username = Username;
            var password = Password;

            this.StartBusy(Resources.Authenticating.ToLower());

            _processing = _client
                .Authenticate(username, password)
                .Do(_token.SetToken)
                .ObserveOnDispatcher()
                .Finally(this.StopBusy)
                .Subscribe(token =>
                {
                    IoC.Get<IUserService>("UserService").Enable(true);
                    _navigation.UriFor<MainViewModel>().Navigate();
                }, ExceptionHelper.HandleException);
        }

        public void Register()
        {
            _navigation.UriFor<SignupViewModel>().Navigate();
        }
    }
}