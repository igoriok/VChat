using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.GamerServices;
using NotifyPropertyWeaver;

using VChat.Events.Update;
using VChat.Models;
using VChat.Mvvm;
using VChat.Services.Videos;
using VChat.Services.Vkontakte;
using VChat.ViewModels.Data;

namespace VChat.ViewModels
{
    public class ConversationBaseViewModel : Screen, IBusyState,
        IHandle<TaskCompleted<PhotoResult>>,
        IHandle<MessageDeleted>,
        IHandle<MessageFlagAdded>,
        IHandle<MessageFlagRemoved>,
        IHandle<MessageFlagChanged>
    {
        private readonly INavigationService _navigation;
        private readonly IEventAggregator _eventAggregator;
        private readonly IVkClient _client;

        [NotifyProperty]
        public bool IsBusy { get; set; }

        [NotifyProperty]
        public string Status { get; set; }

        [NotifyProperty]
        public MessageViewModel ActiveMessage { get; set; }

        [NotifyProperty]
        public string Text { get; set; }

        public bool CanDelete { get { return !IsBusy; } }

        public bool CanForward { get { return !IsBusy; } }

        public bool CanCopy { get { return !IsBusy; } }

        public BindableCollection<MessageViewModel> Messages { get; private set; }

        public BindableCollection<AttachmentViewModel> Attachments { get; private set; }

        public GeoViewModel Geo { get; set; }

        public DelegateCommand AddPictureCommand { get; private set; }

        public DelegateCommand AddLocationCommand { get; private set; }

        public ConversationBaseViewModel(INavigationService navigation, IEventAggregator eventAggregator, IVkClient client)
        {
            _navigation = navigation;
            _eventAggregator = eventAggregator;
            _client = client;

            Messages = new BindableCollection<MessageViewModel>();

            Attachments = new BindableCollection<AttachmentViewModel>();

            AddPictureCommand = new DelegateCommand(AddPicture);
            AddLocationCommand = new DelegateCommand(AddLocation);

            PropertyChanged += ViewModel_PropertyChanged;
        }

        private void AddPicture()
        {
            _eventAggregator.RequestTask<PhotoChooserTask>(null);
        }

        private void AddLocation()
        {
        }

        public void OpenMessageInfo(MessageViewModel message)
        {
            if (message.Attachments != null && message.Attachments.Length > 0)
            {
                _navigation
                    .UriFor<MessageInfoViewModel>()
                    .WithParam(vm => vm.MessageId, message.Id)
                    .Navigate();
            }
        }

        public void OpenDocument(DocumentViewModel document)
        {
        }

        public void TooggleAudioPlay(AudioViewModel audio)
        {
            if (audio.IsPlaying)
            {
                BackgroundAudioPlayer.Instance.Stop();
            }
            else
            {
                var track = new AudioTrack(
                    new Uri(audio.Url),
                    audio.Title,
                    audio.Performer,
                    null, null,
                    audio.Id.ToString(),
                    EnabledPlayerControls.Pause | EnabledPlayerControls.Rewind | EnabledPlayerControls.FastForward);

                BackgroundAudioPlayer.Instance.Track = track;
                BackgroundAudioPlayer.Instance.Play();
            }
        }

        public void OpenVideo(VideoViewModel viewModel)
        {
            _client
                .GetVideo(viewModel.Owner.Id, viewModel.Id)
                .ObserveOnDispatcher()
                .Subscribe(video =>
                {
                    var uri = video.Files.Video240 ?? video.Files.External;

                    if (video.Files.External != null)
                    {
                        var launcher = new WebBrowserTask
                        {
                            Uri = new Uri(video.Files.External)
                        };

                        launcher.Show();    
                    }
                    else
                    {
                        var launcher = new MediaPlayerLauncher
                        {
                            Location = MediaLocationType.None,
                            Controls = MediaPlaybackControls.All,
                            Media = new Uri(uri)
                        };

                        launcher.Show();
                    }
                });
        }

        public void OpenGeo(MessageViewModel message)
        {
            if (message.Geo != null)
            {
                var task = new BingMapsTask
                {
                    Center = message.Geo.Coordinate,
                    ZoomLevel = 12
                };

                task.Show();
            }
        }

        public void Delete(MessageViewModel message)
        {
            MessageBox
                .Show("Delete message?", "Message will be deleted.", new[] { "delete", "cancel" }, 0, MessageBoxIcon.None)
                .ObserveOnDispatcher()
                .Subscribe(index =>
                {
                    if (index == 0)
                    {
                        this.StartBusy("deleting...");

                        _client
                            .DeleteMessage(message.Id)
                            .ObserveOnDispatcher()
                            .Finally(this.StopBusy)
                            .Subscribe(result =>
                            {
                                if (result)
                                {
                                    Messages.Remove(message);

                                    if (Messages.Count == 0)
                                    {
                                        _navigation.GoBack();
                                    }
                                }
                            });
                    }
                });
        }

        public void Forward(MessageViewModel message)
        {
        }

        public void Copy(MessageViewModel message)
        {
        }

        protected override void OnInitialize()
        {
            _eventAggregator.Subscribe(this);
        }

        protected override void OnActivate()
        {
            ActiveMessage = null;
            BackgroundAudioPlayer.Instance.PlayStateChanged += Instance_PlayStateChanged;
        }

        protected override void OnDeactivate(bool close)
        {
            BackgroundAudioPlayer.Instance.PlayStateChanged -= Instance_PlayStateChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ActiveMessage":

                    if (ActiveMessage != null)
                        OpenMessageInfo(ActiveMessage);
                    break;

                case "IsBusy":

                    NotifyOfPropertyChange("CanDelete");
                    NotifyOfPropertyChange("CanForward");
                    NotifyOfPropertyChange("CanCopy");
                    break;
            }
        }

        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            var instance = (BackgroundAudioPlayer)sender;
            var track = instance.Track;

            int audioId;

            if (track != null && int.TryParse(track.Tag, out audioId))
            {
                var isPlaying = instance.PlayerState == PlayState.Playing;

                foreach (var message in Messages)
                {
                    if (message.Attachments != null)
                    {
                        foreach (var audio in message.Attachments.OfType<AudioViewModel>())
                        {
                            if (audio.Id == audioId)
                            {
                                audio.IsPlaying = isPlaying;
                            }
                        }
                    }
                }
            }
        }

        #region IHandle

        public void Handle(TaskCompleted<PhotoResult> message)
        {
            if (message.Result.TaskResult == TaskResult.OK)
            {
                var file = System.IO.Path.GetFileName(message.Result.OriginalFileName);

                _client
                    .UploadMessagePhoto(file, message.Result.ChosenPhoto)
                    .Subscribe(att => Attachments.Add(PhotoViewModel.Map(att)));
            }
        }

        public void Handle(MessageDeleted message)
        {
            for (var index = 0; index < Messages.Count; index++)
            {
                var viewModel = Messages[index];

                if (message.MessageId == viewModel.Id)
                {
                    Messages.RemoveAt(index);
                    index--;
                }
            }
        }

        public void Handle(MessageFlagAdded message)
        {
            foreach (var viewModel in Messages)
            {
                if (message.MessageId == viewModel.Id)
                {
                    switch (message.MessageFlag)
                    {
                        case MessageFlag.Unread:

                            viewModel.IsRead = false;
                            break;

                        // TODO: ...
                    }
                }
            }
        }

        public void Handle(MessageFlagRemoved message)
        {
            foreach (var viewModel in Messages)
            {
                if (message.MessageId == viewModel.Id)
                {
                    switch (message.MessageFlag)
                    {
                        case MessageFlag.Unread:

                            viewModel.IsRead = true;
                            break;

                        // TODO: ...
                    }
                }
            }
        }

        public void Handle(MessageFlagChanged message)
        {
            foreach (var viewModel in Messages)
            {

            }

            // TODO: ...
        }

        #endregion
    }
}