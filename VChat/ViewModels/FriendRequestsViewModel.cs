using System;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using NotifyPropertyWeaver;

using VChat.Models;
using VChat.Mvvm;
using VChat.Services.Vkontakte;
using VChat.ViewModels.Data;

namespace VChat.ViewModels
{
    public class FriendRequestsViewModel : Screen, IBusyState
    {
        private readonly INavigationService _navigation;
        private readonly IVkClient _client;

        [NotifyProperty]
        public bool IsBusy { get; set; }

        [NotifyProperty]
        public string Status { get; set; }

        [NotifyProperty]
        public int? Count { get; set; }

        [NotifyProperty]
        public UserViewModel Selected { get; set; }

        public BindableCollection<Group<string, UserViewModel>> Users { get; private set; }

        public FriendRequestsViewModel(INavigationService navigation, IVkClient client)
        {
            _navigation = navigation;
            _client = client;

            Users = new BindableCollection<Group<string, UserViewModel>>();

            DisplayName = "friend requests";

            PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Selected":

                    OpenUserInfo(Selected);
                    break;
            }
        }

        protected override void OnInitialize()
        {
            this.StartBusy("loading...");

            GetFriendsRequests()
                .ForkJoin(GetFriendsSuggestions(), Tuple.Create)
                .ObserveOnDispatcher()
                .Finally(this.StopBusy)
                .Subscribe(tuple =>
                {
                    Count = tuple.Item1.Count();

                    Users.Add(tuple.Item1);
                    Users.Add(tuple.Item2);
                });
        }

        protected override void OnActivate()
        {
            Selected = null;
        }

        private void OpenUserInfo(UserViewModel user)
        {
            if (user != null)
            {
                _navigation
                .UriFor<UserInfoViewModel>()
                .WithParam(vm => vm.UserId, user.Id)
                .WithParam(vm => vm.IsRequest, true)
                .Navigate();
            }
        }

        private IObservable<Group<string, UserViewModel>> GetFriendsRequests()
        {
            return _client
                .GetFriendsRequests(100)
                .SelectMany(uids => uids.Length > 0 ? _client.GetUsers(uids) : Observable.Return(new User[0]))
                .Select(users => new Group<string, UserViewModel>("REQUESTS", users.Select(UserViewModel.Map).ToArray()));
        }

        private IObservable<Group<string, UserViewModel>> GetFriendsSuggestions()
        {
            return _client
                .GetFriendsSuggestions(10)
                .Select(users => new Group<string, UserViewModel>("SUGGESTIONS", users.Select(UserViewModel.Map).ToArray()));
        }
    }
}