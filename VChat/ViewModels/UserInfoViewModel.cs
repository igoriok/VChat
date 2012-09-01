using System;
using System.ComponentModel;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using NotifyPropertyWeaver;

using VChat.Mvvm;
using VChat.Services.Vkontakte;
using VChat.ViewModels.Data;

namespace VChat.ViewModels
{
    public class UserInfoViewModel : Screen, IBusyState
    {
        private readonly IVkClient _client;

        private IDisposable _loading = Disposable.Empty;
        private IDisposable _invite = Disposable.Empty;
        private IDisposable _delete = Disposable.Empty;

        [NotifyProperty]
        public bool IsBusy { get; set; }

        [NotifyProperty]
        public string Status { get; set; }

        [NotifyProperty]
        public int UserId { get; set; }

        [NotifyProperty]
        public bool IsRequest { get; set; }

        [NotifyProperty]
        public UserViewModel User { get; set; }

        public DelegateCommand InviteCommand { get; private set; }

        public DelegateCommand DeleteCommand { get; private set; }

        public UserInfoViewModel(IVkClient client)
        {
            _client = client;

            InviteCommand = new DelegateCommand(Invite, CanInvite);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);

            PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsBusy":

                    InviteCommand.NotifyCanExecuteChanged();
                    DeleteCommand.NotifyCanExecuteChanged();
                    break;
            }
        }

        protected override void OnInitialize()
        {
            this.StartBusy("loading...");

            _loading = _client
                .GetUsers(new[] { UserId })
                .ObserveOnDispatcher()
                .Finally(this.StopBusy)
                .Subscribe(users => User = UserViewModel.Map(users[0]));
        }

        protected override void OnDeactivate(bool close)
        {
            _loading.Dispose();
            _invite.Dispose();
            _delete.Dispose();
        }

        private bool CanInvite()
        {
            return !IsBusy;
        }

        private void Invite()
        {
            _invite = _client
                .AddFriend(UserId)
                .Subscribe(result =>
                {
                    IsRequest = false;
                });
        }

        private bool CanDelete()
        {
            return !IsBusy;
        }

        private void Delete()
        {
            _delete = _client
                .DeleteFriend(UserId)
                .Subscribe(result =>
                {
                    IsRequest = true;
                });
        }
    }
}