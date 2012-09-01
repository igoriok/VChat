using System;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using VChat.Events.Update;
using VChat.Services.Configuration;
using VChat.Services.Vkontakte;

namespace VChat.Services.System
{
    public class LongPollService : BaseUserService, IObserver<Update>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IVkClient _client;
        private readonly IPhoneService _phone;

        private IDisposable _disposable = Disposable.Empty;

        public LongPollService(IEventAggregator eventAggregator, IVkClient client, IPhoneService phone, IConfiguration configuration)
            : base(configuration, true)
        {
            _eventAggregator = eventAggregator;
            _client = client;
            _phone = phone;
        }

        #region ISystemService

        public override void Start()
        {
            _phone.Deactivated += Phone_Deactivated;
            _phone.Continued += Phone_Continued;

            _disposable = _client.GetLongPollServer().SubscribeOn(Scheduler.ThreadPool).Subscribe(this);
        }

        public override void Stop()
        {
            _phone.Deactivated -= Phone_Deactivated;
            _phone.Continued -= Phone_Continued;

            _disposable.Dispose();
        }

        #endregion

        private void Phone_Continued()
        {
            if (IsEnabled)
            {
                Start();
            }
        }

        private void Phone_Deactivated(object sender, DeactivatedEventArgs e)
        {
            if (IsEnabled)
            {
                Stop();
            }
        }

        #region IObserver<Update>

        void IObserver<Update>.OnNext(Update update)
        {
            var messageAdded = update as MessageAdded;
            if (messageAdded != null)
            {
                LoadMessage(messageAdded);
            }

            _eventAggregator.Publish(update);
        }

        private void LoadMessage(MessageAdded update)
        {
            update.Message = _client.GetMessage(update.MessageId).Single();
        }

        void IObserver<Update>.OnError(Exception exception)
        {
        }

        void IObserver<Update>.OnCompleted()
        {
        }

        #endregion
    }
}