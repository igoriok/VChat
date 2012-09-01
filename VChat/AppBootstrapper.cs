using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using VChat.Mvvm.Logging;
using VChat.Services.Cache;
using VChat.Services.Configuration;
using VChat.Services.Contacts;
using VChat.Services.Maps;
using VChat.Services.System;
using VChat.Services.Token;
using VChat.Services.Vkontakte;
using VChat.ViewModels;

namespace VChat
{
    public class AppBootstrapper : PhoneBootstrapper
    {
        private PhoneContainer _container;

        protected override PhoneApplicationFrame CreatePhoneApplicationFrame()
        {
            return new MainFrame();
        }

        protected override void Configure()
        {
            ViewModelBinder.ApplyConventionsByDefault = false;

            _container = new PhoneContainer(RootFrame);

            _container.RegisterPerRequest(typeof(StartupViewModel), "StartupViewModel", typeof(StartupViewModel));
            _container.RegisterPerRequest(typeof(SigninViewModel), "SigninViewModel", typeof(SigninViewModel));
            _container.RegisterPerRequest(typeof(SignupViewModel), "SignupViewModel", typeof(SignupViewModel));
            _container.RegisterPerRequest(typeof(ConfirmViewModel), "ConfirmViewModel", typeof(ConfirmViewModel));
            _container.RegisterPerRequest(typeof(MainViewModel), "MainViewModel", typeof(MainViewModel));
            _container.RegisterPerRequest(typeof(FriendRequestsViewModel), "FriendRequestsViewModel", typeof(FriendRequestsViewModel));
            _container.RegisterPerRequest(typeof(DialogViewModel), "DialogViewModel", typeof(DialogViewModel));
            _container.RegisterPerRequest(typeof(ChatViewModel), "ChatViewModel", typeof(ChatViewModel));
            _container.RegisterPerRequest(typeof(ContactInfoViewModel), "ContactInfoViewModel", typeof(ContactInfoViewModel));
            _container.RegisterPerRequest(typeof(UserInfoViewModel), "UserInfoViewModel", typeof(UserInfoViewModel));
            _container.RegisterPerRequest(typeof(SettingsViewModel), "SettingsViewModel", typeof(SettingsViewModel));
            _container.RegisterPerRequest(typeof(MessagesViewModel), null, typeof(MessagesViewModel));
            _container.RegisterPerRequest(typeof(FriendsViewModel), null, typeof(FriendsViewModel));
            _container.RegisterPerRequest(typeof(ContactsViewModel), null, typeof(ContactsViewModel));

            _container.RegisterSingleton(typeof(IPhoneBook), null, typeof(PhoneBook));
            _container.RegisterSingleton(typeof(IPhotoCache), null, typeof(PhotoCache));
            _container.RegisterSingleton(typeof(IConfiguration), null, typeof(ApplicationConfiguration));
            _container.RegisterSingleton(typeof(IMapService), null, typeof(GoogleMapService));
            _container.RegisterSingleton(typeof(ITokenProvider), null, typeof(TokenProvider));
            _container.RegisterSingleton(typeof(IVkClient), null, typeof(VkClient));

            _container.RegisterSingleton(typeof(IUserService), "UserService", typeof(UserService));
            _container.RegisterSingleton(typeof(IUserService), "UserOnlineService", typeof(UserOnlineService));
            _container.RegisterSingleton(typeof(IUserService), "LongPollService", typeof(LongPollService));
            _container.RegisterSingleton(typeof(IUserService), "PushService", typeof(PushService));
            _container.RegisterSingleton(typeof(IUserService), "SoundService", typeof(SoundService));
            _container.RegisterSingleton(typeof(IUserService), "VibrateService", typeof(VibrateService));

            _container.RegisterPhoneServices();

            LogManager.GetLog = type => new DebugLog(type);
        }

        protected override void PrepareApplication()
        {
            base.PrepareApplication();
            Application.ApplicationLifetimeObjects.Add(new ApplicationService());
        }

        protected override void OnLaunch(object sender, LaunchingEventArgs e)
        {
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }
    }
}