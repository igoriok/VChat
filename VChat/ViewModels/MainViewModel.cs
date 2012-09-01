using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using NotifyPropertyWeaver;
using VChat.Mvvm;
using VChat.Services.System;
using VChat.Services.Vkontakte;

namespace VChat.ViewModels
{
    public class MainViewModel : Conductor<MainTabItem>.Collection.OneActive
    {
        private readonly INavigationService _navigation;
        private readonly IVkClient _client;

        [NotifyProperty]
        public bool IsSearching { get; set; }

        [NotifyProperty]
        public bool IsSearchActive { get; set; }

        [NotifyProperty]
        public int? NewMessages { get; set; }

        [NotifyProperty]
        public int? NewFriends { get; set; }

        public DelegateCommand SearchCommand { get; private set; }

        public DelegateCommand SettingsCommand { get; private set; }

        public DelegateCommand SignoutCommand { get; private set; }

        public MessagesViewModel Messages { get; private set; }

        public FriendsViewModel Friends { get; private set; }

        public ContactsViewModel Contacts { get; private set; }

        public MainViewModel(INavigationService navigation, IVkClient client)
        {
            _navigation = navigation;
            _client = client;

            SearchCommand = new DelegateCommand(Search);
            SettingsCommand = new DelegateCommand(Settings);
            SignoutCommand = new DelegateCommand(Signout);

            Messages = IoC.Get<MessagesViewModel>();
            Friends = IoC.Get<FriendsViewModel>();
            Contacts = IoC.Get<ContactsViewModel>();

            Items.Add(Messages);
            Items.Add(Friends);
            Items.Add(Contacts);

            PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsSearchActive":

                    if (!IsSearchActive && string.IsNullOrEmpty(ActiveItem.SearchPhrase))
                    {
                        CancelSearch();
                    }
                    break;
            }
        }

        protected override void OnInitialize()
        {
            _navigation.ClearHistory();

            ActivateItem(Items[0]);

            _client
                .GetUnreadMessagesCount()
                .ObserveOnDispatcher()
                .Subscribe(count => NewMessages = count == 0 ? (int?)null : count);

            _client
                .GetFriendsRequests(100)
                .ObserveOnDispatcher()
                .Subscribe(uids => NewFriends = uids.Length == 0 ? (int?)null : uids.Length);
        }

        protected override void OnDeactivate(bool close)
        {
            CancelSearch();
            base.OnDeactivate(close);
        }

        public void Search()
        {
            IsSearching = true;
        }

        public void CancelSearch()
        {
            IsSearching = false;
            if (ActiveItem != null)
            {
                ActiveItem.SearchPhrase = null;
            }
        }

        public void OpenDialogs()
        {
            ActivateItem(Messages);
        }

        public void OpenFriends()
        {
            _navigation.UriFor<FriendRequestsViewModel>().Navigate();
        }

        private void Settings()
        {
            new UriBuilder<SettingsViewModel>()
                .AttachTo(_navigation)
                .Navigate();
        }

        private void Signout()
        {
            IoC.Get<IUserService>("UserService").Disable(true);
            _navigation.UriFor<SigninViewModel>().Navigate();
        }
    }
}