using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using NotifyPropertyWeaver;

using VChat.Events.Update;
using VChat.Mvvm;
using VChat.Services.Vkontakte;
using VChat.ViewModels.Data;

namespace VChat.ViewModels
{
    public class FriendsViewModel : MainTabItem, IHandle<UserOnline>, IHandle<UserOffline>
    {
        private readonly IVkClient _client;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigation;

        [NotifyProperty]
        public bool IsOnline { get; set; }

        [NotifyProperty]
        public UserViewModel ActiveFriend { get; set; }

        public BindableCollection<UserViewModel> FriendsSource { get; private set; }

        public FilteredCollection<UserViewModel> Friends { get; private set; }

        public DelegateCommand OnlineStatusCommand { get; private set; }

        public FriendsViewModel(IVkClient client, IEventAggregator eventAggregator, INavigationService navigation)
        {
            _client = client;
            _eventAggregator = eventAggregator;
            _navigation = navigation;

            DisplayName = "friends";

            FriendsSource = new BindableCollection<UserViewModel>();
            Friends = new FilteredCollection<UserViewModel>(FriendsSource, FriendFilter);

            OnlineStatusCommand = new DelegateCommand(ToogleOnlineStatus);

            PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ActiveFriend":
                    if (ActiveFriend != null)
                    {
                        OpenConversation(ActiveFriend);
                    }
                    break;

                case "SearchPhrase":

                    Friends.Sync();
                    break;
            }
        }



        protected override void OnInitialize()
        {
            _eventAggregator.Subscribe(this);

            _client
                .GetFriends()
                .ObserveOnDispatcher()
                .Subscribe(users =>
                {
                    FriendsSource.Clear();
                    FriendsSource.AddRange(users.Select(UserViewModel.Map));

                    Friends.Sync();
                });
        }

        protected override void OnActivate()
        {
            ActiveFriend = null;
        }

        private void OpenConversation(UserViewModel user)
        {
            new UriBuilder<DialogViewModel>()
                .WithParam(vm => vm.UserId, user.Id)
                .AttachTo(_navigation).Navigate();
        }

        private bool FriendFilter(UserViewModel user)
        {
            if (user == null)
                return false;

            if (IsOnline && !user.IsOnline)
                return false;

            if (string.IsNullOrEmpty(SearchPhrase) ||
                StringContains(user.FirstName, SearchPhrase) ||
                StringContains(user.LastName, SearchPhrase))
                return true;

            return false;
        }

        private bool StringContains(string source, string part)
        {
            if (string.IsNullOrEmpty(part))
                return true;

            if (string.IsNullOrEmpty(source))
                return false;

            return source.ToUpper().Contains(part.ToUpper());
        }

        private void ToogleOnlineStatus()
        {
            IsOnline = !IsOnline;
            Friends.Sync();
        }

        void IHandle<UserOnline>.Handle(UserOnline message)
        {
            foreach (var friend in FriendsSource)
            {
                if (friend.Id == message.UserId)
                {
                    friend.IsOnline = true;
                }
            }

            if (IsOnline)
            {
                Friends.Sync();
            }
        }

        void IHandle<UserOffline>.Handle(UserOffline message)
        {
            foreach (var friend in FriendsSource)
            {
                if (friend.Id == message.UserId)
                {
                    friend.IsOnline = false;
                }
            }

            if (IsOnline)
            {
                Friends.Sync();
            }
        }
    }
}