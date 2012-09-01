using System;
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
    public class ChatViewModel : ConversationBaseViewModel,
        IHandle<MessageAdded>,
        IHandle<ChatChanged>,
        IHandle<UserTypingInChat>
    {
        private readonly INavigationService _navigation;
        private readonly IVkClient _client;

        private IDisposable _loading = Disposable.Empty;

        private IDisposable _chatInfo = Disposable.Empty;
        private IDisposable _meTyping = Disposable.Empty;

        private bool _full;

        [NotifyProperty]
        public int ChatId { get; set; }

        [NotifyProperty]
        public ConversationViewModel Chat { get; set; }

        [NotifyProperty]
        public UserViewModel[] Users { get; set; }

        [NotifyProperty]
        public BindableCollection<UserTyping> Typing { get; private set; }

        public DelegateCommand LoadCommand { get; private set; }

        public DelegateCommand SendMessageCommand { get; private set; }

        public DelegateCommand DeleteConversationCommand { get; private set; }

        public ChatViewModel(INavigationService navigation, IEventAggregator eventAggregator, IVkClient client)
            : base(navigation, eventAggregator, client)
        {
            _navigation = navigation;
            _client = client;

            Typing = new BindableCollection<UserTyping>();

            LoadCommand = new DelegateCommand(Load, CanLoad);
            SendMessageCommand = new DelegateCommand(SendMessage, CanSendMessage);
            DeleteConversationCommand = new DelegateCommand(DeleteConversation, CanDeleteConversation);

            PropertyChanged += ViewModel_PropertyChanged;
        }

        private bool CanLoad()
        {
            return !IsBusy && !_full;
        }

        private void Load()
        {
            this.StartBusy("loading...");

            _loading = _client
                .GetChatHistory(ChatId, 20, Messages.Count == 0 ? (int?)null : Messages.Last().Id)
                .ObserveOnDispatcher()
                .Finally(this.StopBusy)
                .Subscribe(messages =>
                {
                    Messages.AddRange(messages.Select(MessageViewModel.Map));

                    UpdateMessageUsers();

                    if (messages.Length < 20)
                    {
                        _full = true;
                    }
                });
        }

        public void OpenChatInfo()
        {
            _navigation
                .UriFor<ChatInfoViewModel>()
                .WithParam(vm => vm.ChatId, ChatId)
                .Navigate();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            UpdateChatInfo();

            Load();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            _meTyping = this.ObservablePropertyChanged("Text")
                .Where(_ => !string.IsNullOrEmpty(Text))
                .Throttle(TimeSpan.FromSeconds(1))
                .SelectMany(_client.SetChatActivity(ChatId))
                .Subscribe();
        }

        protected override void OnDeactivate(bool close)
        {
            _loading.Dispose();
            _meTyping.Dispose();
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

        private void UpdateChatInfo()
        {
            _chatInfo.Dispose();

            _chatInfo = _client
                .GetChat(ChatId)
                .ObserveOnDispatcher()
                .Subscribe(chat =>
                {
                    Chat = ConversationViewModel.Map(chat);
                    UpdateMessageUsers();
                });
        }

        private void UpdateMessageUsers()
        {
            if (Chat != null && Messages.Count > 0)
            {
                foreach (var message in Messages)
                {
                    foreach (var user in Chat.Users)
                    {
                        if (user.Id == message.User.Id)
                        {
                            message.User = user;
                            break;
                        }
                    }
                }
            }
        }

        private bool CanSendMessage()
        {
            return !IsBusy && !string.IsNullOrEmpty(Text);
        }

        private void SendMessage()
        {
            this.StartBusy("sending...");

            _client
                .SendChatMessage(ChatId, Text)
                .ObserveOnDispatcher()
                .Finally(this.StopBusy)
                .Subscribe(mid =>
                {
                    var message = new MessageViewModel { Id = mid, Body = Text, IsOut = true };
                    Messages.Insert(0, message);
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
                            .DeleteChat(ChatId)
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
            if (message.Message.ChatId.HasValue)
            {
                // TODO: ...
            }
        }

        public void Handle(ChatChanged message)
        {
            if (message.ChatId == ChatId)
            {
                UpdateChatInfo();
            }
        }

        public void Handle(UserTypingInChat message)
        {
            if (message.ChatId == ChatId)
            {
                var user = Chat == null ? null : Chat.Users.FirstOrDefault(u => u.Id == message.UserId);
                if (user != null)
                {
                    var typing = Typing.FirstOrDefault(u => u.User.Id == message.UserId);
                    if (typing == null)
                    {
                        typing = new UserTyping { User = user };

                        Typing.Add(typing);
                    }
                    else
                    {
                        typing.Typing.Dispose();
                    }

                    typing.Typing = Observable
                        .Timer(TimeSpan.FromSeconds(5))
                        .ObserveOnDispatcher()
                        .Subscribe(_ =>
                        {
                            for (var index = 0; index < Typing.Count; index++)
                            {
                                if (Typing[index].User.Id == message.UserId)
                                {
                                    Typing.RemoveAt(index);
                                    break;
                                }
                            }
                        });
                }
            }
        }

        #endregion

        public class UserTyping
        {
            public UserViewModel User { get; set; }
            public IDisposable Typing { get; set; }
        }
    }
}