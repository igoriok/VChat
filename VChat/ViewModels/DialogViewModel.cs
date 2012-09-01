using System;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using NotifyPropertyWeaver;

using VChat.Events.Update;
using VChat.Models;
using VChat.Mvvm;
using VChat.Services.Vkontakte;
using VChat.ViewModels.Data;

namespace VChat.ViewModels
{
    public class DialogViewModel : ConversationBaseViewModel,
        IHandle<MessageAdded>,
        IHandle<UserOnline>,
        IHandle<UserOffline>,
        IHandle<UserTyping>
    {
        private readonly INavigationService _navigation;
        private readonly IVkClient _client;

        private IDisposable _loading = Disposable.Empty;

        private IDisposable _pollUser = Disposable.Empty;
        private IDisposable _userTyping = Disposable.Empty;
        private IDisposable _meTyping = Disposable.Empty;

        private bool _full;

        [NotifyProperty]
        public int UserId { get; set; }

        [NotifyProperty]
        public UserViewModel User { get; set; }

        [DependsOn("User")]
        public bool IsOnline
        {
            get
            {
                if (User == null)
                    return false;

                if (User.LastSeen == null)
                    return false;

                return User.LastSeen.Value.AddMinutes(5) > DateTime.UtcNow;
            }
        }

        [DependsOn("IsTyping", "User")]
        public string UserStatus
        {
            get
            {
                if (IsTyping)
                    return "typing...";

                if (IsOnline)
                    return "online";

                if (User == null || !User.LastSeen.HasValue)
                    return string.Empty;

                return User.LastSeen.Value.ToLocalTime().ToString();
            }
        }

        [NotifyProperty]
        public bool IsLocked { get; set; }

        [NotifyProperty]
        public bool IsTyping { get; set; }

        public DelegateCommand LoadCommand { get; private set; }

        public DelegateCommand SendMessageCommand { get; private set; }

        public DelegateCommand DeleteConversationCommand { get; private set; }

        public DialogViewModel(INavigationService navigation, IEventAggregator eventAggregator, IVkClient client)
            : base(navigation, eventAggregator, client)
        {
            _navigation = navigation;
            _client = client;

            LoadCommand = new DelegateCommand(Load, CanLoad);
            SendMessageCommand = new DelegateCommand(SendMessage, CanSendMessage);
            DeleteConversationCommand = new DelegateCommand(DeleteConversation, CanDeleteConversation);

            PropertyChanged += ViewModel_PropertyChanged;
        }

        public void OpenContactInfo()
        {
            _navigation
                .UriFor<ContactInfoViewModel>()
                .WithParam(vm => vm.ContactId, UserId)
                .Navigate();
        }

        protected override void OnInitialize()
        {
            Load();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            _pollUser = _client
                .GetUsers(new[] { UserId })
                .Retry(TimeSpan.FromSeconds(5))
                .Repeat(TimeSpan.FromSeconds(5))
                .ObserveOnDispatcher()
                .Subscribe(users => { User = UserViewModel.Map(users[0]); });

            _meTyping = this.ObservablePropertyChanged("Text")
                .Where(_ => !string.IsNullOrEmpty(Text))
                .Throttle(TimeSpan.FromSeconds(1))
                .SelectMany(_client.SetDialogActivity(UserId))
                .Subscribe();
        }

        protected override void OnDeactivate(bool close)
        {
            _loading.Dispose();
            _pollUser.Dispose();
            _meTyping.Dispose();

            base.OnDeactivate(close);
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsBusy":

                    SendMessageCommand.NotifyCanExecuteChanged();
                    DeleteConversationCommand.NotifyCanExecuteChanged();
                    break;

                case "Text":

                    SendMessageCommand.NotifyCanExecuteChanged();
                    break;
            }
        }

        private bool CanLoad()
        {
            return !IsBusy && !_full;
        }

        private void Load()
        {
            this.StartBusy("loading...");

            _loading = _client
                .GetDialogHistory(UserId, 20, Messages.Count == 0 ? (int?)null : Messages.Last().Id)
                .ObserveOnDispatcher()
                .Finally(this.StopBusy)
                .Subscribe(messages =>
                {
                    foreach (var message in messages)
                    {
                        Messages.Add(MessageViewModel.Map(message));
                    }

                    if (messages.Length < 20)
                    {
                        _full = true;
                    }
                });
        }

        private bool CanSendMessage()
        {
            return !IsBusy && !string.IsNullOrEmpty(Text);
        }

        private void SendMessage()
        {
            this.StartBusy("sending...");

            _client
                .SendDialogMessage(UserId, Text)
                .ObserveOnDispatcher()
                .Finally(this.StopBusy)
                .Subscribe(mid =>
                {
                    Messages.Insert(0, new MessageViewModel { Id = mid, Body = Text, IsOut = true });
                    Text = null;
                });
        }

        private bool CanDeleteConversation()
        {
            return !IsBusy;
        }

        private void DeleteConversation()
        {
            MessageBox
                .Show("Delete conversation?", "Conversation will be deleted.", new[] { "delete", "cancel" }, 0)
                .ObserveOnDispatcher()
                .Subscribe(index =>
                {
                    if (index == 0)
                    {
                        this.StartBusy("deleting...");

                        _client
                            .DeleteDialog(UserId)
                            .ObserveOnDispatcher()
                            .Finally(this.StopBusy)
                            .Subscribe(result =>
                            {
                                if (result)
                                {
                                    _navigation.GoBack();
                                }
                            });
                    }
                });
        }

        #region IHandle

        public void Handle(MessageAdded message)
        {
        }

        public void Handle(UserOnline message)
        {
            if (User == null)
                return;

            if (message.UserId == User.Id)
            {
                User.IsOnline = true;
                NotifyOfPropertyChange("UserStatus");
            }
        }

        public void Handle(UserOffline message)
        {
            if (User == null)
                return;

            if (message.UserId == User.Id)
            {
                User.IsOnline = false;
                NotifyOfPropertyChange("UserStatus");
            }
        }

        public void Handle(UserTyping message)
        {
            if (UserId == message.UserId)
            {
                _userTyping.Dispose();

                IsTyping = true;

                _userTyping = Observable
                    .Timer(TimeSpan.FromSeconds(5))
                    .ObserveOnDispatcher()
                    .Subscribe(_ => IsTyping = false);
            }
        }

        #endregion
    }
}