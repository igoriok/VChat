using System.Globalization;
using Caliburn.Micro;

using VChat.Services.System;

namespace VChat.ViewModels
{
    public class StartupViewModel : Screen
    {
        private readonly INavigationService _navigation;

        public StartupViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        protected override void OnInitialize()
        {
            Labels.Instance.Culture = new CultureInfo("ru");

            if (IoC.Get<IUserService>("UserService").IsEnabled)
            {
                _navigation.UriFor<MainViewModel>().Navigate();
            }
            else
            {
                _navigation.UriFor<SigninViewModel>().AttachTo(IoC.Get<INavigationService>()).Navigate();
            }
        }
    }
}