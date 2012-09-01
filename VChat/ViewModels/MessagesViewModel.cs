using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using NotifyPropertyWeaver;
using VChat.Events.Update;
using VChat.Mvvm;
using VChat.Services.Vkontakte;
using VChat.ViewModels.Data;

namespace VChat.ViewModels
{
    public class MessagesViewModel : MainTabItem,
        IHandle<UserOnline>,
        IHandle<UserOffline>,
        IHandle<MessageFlagAdded>,
        IHandle<MessageFlagRemoved>,
        IHandle<MessageFlagChanged>,
        IHandle<MessageAdded>,
        IHandle<MessageDeleted>
    {
        private readonly INavigationService _navigation;
        private readonly IVkClient _client;
        private readonly IEventAggregator _eventAggregator;

        private IDisposable _loading = Disposable.Empty;
        private IDisposable _deleting = Disposable.Empty;

        private bool _full;

        public bool CanDelete
        {
            get { return !IsBusy; }
        }

        [NotifyProperty]
        public ConversationViewModel ActiveConversation { get; set; }

        public BindableCollection<ConversationViewModel> Conversations { get; private set; }

        public ICollectionView ConversationsView { get; private set; }

        public DelegateCommand LoadCommand { get; private set; }

        public DelegateCommand AddCommand { get; private set; }

        public DelegateCommand UpdateCommand { get; private set; }

        public MessagesViewModel(INavigationService navigation, IVkClient client, IEventAggregator eventAggregator)
        {
            _navigation = navigation;
            _client = client;
            _eventAggregator = eventAggregator;

            DisplayName = "messages"; // Resources.Messages;

            Conversations = new BindableCollection<ConversationViewModel>();
            ConversationsView = new CollectionViewSource { Source = Conversations }.View;

            ConversationsView.Filter = ConversationFilter;

            LoadCommand = new DelegateCommand(Load, CanLoad);
            AddCommand = new DelegateCommand(Add);
            UpdateCommand = new DelegateCommand(Update);

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
                .GetMessages(20, Conversations.Count)
                .ObserveOnDispatcher()
                .Finally(this.StopBusy)
                .Subscribe(response =>
                {
                    Conversations.AddRange(response.Select(ConversationViewModel.Map));

                    if (response.Length == 0)
                    {
                        _full = true;
                    }

                }, HandleError);
        }

        public void Add()
        {
        }

        public void Delete(ConversationViewModel conversation)
        {
            this.StartBusy("deleting...");

            if (conversation.ChatId.HasValue)
            {
                _deleting = _client
                    .DeleteChat(conversation.ChatId.Value)
                    .ObserveOnDispatcher()
                    .Finally(this.StopBusy)
                    .Subscribe(result =>
                    {
                        if (result)
                        {
                            Conversations.Remove(conversation);
                        }
                    });
            }
            else
            {
                _deleting = _client
                    .DeleteDialog(conversation.Users[0].Id)
                    .ObserveOnDispatcher()
                    .Finally(this.StopBusy)
                    .Subscribe(result =>
                    {
                        if (result)
                        {
                            Conversations.Remove(conversation);
                        }
                    });
            }

            Conversations.Remove(conversation);
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ActiveConversation":

                    if (ActiveConversation != null)
                        OpenConversation(ActiveConversation);
                    break;

                case "IsBusy":

                    NotifyOfPropertyChange("CanDelete");
                    UpdateCommand.NotifyCanExecuteChanged();
                    break;
            }
        }

        protected override void OnActivate()
        {
            ActiveConversation = null;
        }

        protected override void OnDeactivate(bool close)
        {
            _loading.Dispose();
            _deleting.Dispose();
        }

        protected override void OnInitialize()
        {
            _eventAggregator.Subscribe(this);

            Update();
        }

        private bool ConversationFilter(object obj)
        {
            return true;
        }

        private void Update()
        {
            if (IsBusy)
            {
                _loading.Dispose();
                _deleting.Dispose();
            }
            else
            {
                _full = false;
                Conversations.Clear();

                Load();
            }
        }

        private void OpenConversation(ConversationViewModel conversation)
        {
            if (conversation.ChatId.HasValue)
            {
                _navigation.UriFor<ChatViewModel>()
                    .WithParam(vm => vm.ChatId, conversation.ChatId.Value)
                    .Navigate();
            }
            else
            {
                _navigation.UriFor<DialogViewModel>()
                    .WithParam(vm => vm.UserId, conversation.Users[0].Id)
                    .Navigate();
            }
        }

        private void HandleError(Exception exception)
        {
        }

        private void Synchronize()
        {
            Conversations.Syncronize(Conversations.OrderByDescending(c => c.Message.Timestamp).ToList());
        }

        #region IHandle

        void IHandle<UserOnline>.Handle(UserOnline message)
        {
            foreach (var chat in Conversations)
            {
                foreach (var user in chat.Users)
                {
                    if (user.Id == message.UserId)
                    {
                        user.IsOnline = true;
                    }
                }
            }
        }

        void IHandle<UserOffline>.Handle(UserOffline message)
        {
            foreach (var chat in Conversations)
            {
                foreach (var user in chat.Users)
                {
                    if (user.Id == message.UserId)
                    {
                        user.IsOnline = false;
                    }
                }
            }
        }

        void IHandle<MessageFlagAdded>.Handle(MessageFlagAdded message)
        {
            foreach (var conversation in Conversations)
            {
                if (conversation.Message.Id == message.MessageId)
                {
                    switch (message.MessageFlag)
                    {
                        case MessageFlag.Unread:

                            conversation.Message.IsRead = false;
                            break;
                    }
                }
            }
        }

        void IHandle<MessageFlagRemoved>.Handle(MessageFlagRemoved message)
        {
            foreach (var conversation in Conversations)
            {
                if (conversation.Message.Id == message.MessageId)
                {
                    switch (message.MessageFlag)
                    {
                        case MessageFlag.Unread:

                            conversation.Message.IsRead = true;
                            break;
                    }
                }
            }
        }

        void IHandle<MessageFlagChanged>.Handle(MessageFlagChanged message)
        {
            foreach (var conversation in Conversations)
            {
                if (conversation.Message.Id == message.MessageId)
                {
                    break;
                }
            }
        }

        void IHandle<MessageAdded>.Handle(MessageAdded message)
        {
            var founded = false;

            if (message.Message.ChatId.HasValue)
            {
                foreach (var conversation in Conversations)
                {
                    if (conversation.ChatId.HasValue && conversation.ChatId == message.Message.ChatId)
                    {
                        conversation.Message = MessageViewModel.Map(message.Message);
                        founded = true;
                        break;
                    }
                }
            }
            else
            {
                foreach (var conversation in Conversations)
                {
                    if (conversation.UserId.HasValue && conversation.UserId == message.Message.UserId)
                    {
                        conversation.Message = MessageViewModel.Map(message.Message);
                        founded = true;
                        break;
                    }
                }
            }

            if (!founded)
            {
                //var conversation = Mapper.MapToCoversation(message.Message);

                //Conversations.Insert(0, conversation);
            }

            Synchronize();
        }

        void IHandle<MessageDeleted>.Handle(MessageDeleted message)
        {
            for (int index = 0; index < Conversations.Count; index++)
            {
                var conversation = Conversations[index];

                if (conversation.Message.Id == message.MessageId)
                {
                    conversation.Message = null;
                    break;
                }
            }
        }

        private void UpdateConversation(ConversationViewModel conversation)
        {
            if (conversation.ChatId.HasValue)
            {
                _client
                    .GetChatHistory(conversation.ChatId.Value, 1)
                    .ObserveOnDispatcher()
                    .Subscribe(messages =>
                    {
                        var index = Conversations.IndexOf(conversation);
                        if (index < 0)
                            return;

                        if (messages.Length == 0)
                        {
                            Conversations.RemoveAt(index);
                        }
                        else
                        {
                            conversation.Message = MessageViewModel.Map(messages[0]);
                        }

                        Synchronize();
                    });
            }
        }

        #endregion


    }
}