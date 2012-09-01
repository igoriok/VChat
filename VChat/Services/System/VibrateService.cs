using System;
using Caliburn.Micro;

using VChat.Events.Update;
using VChat.Services.Configuration;

namespace VChat.Services.System
{
    public class VibrateService : BaseUserService, IHandle<MessageAdded>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IVibrateController _vibrate;

        public VibrateService(IEventAggregator eventAggregator, IVibrateController vibrate, IConfiguration configuration)
            : base(configuration, true)
        {
            _eventAggregator = eventAggregator;
            _vibrate = vibrate;
        }

        #region BaseUserService

        public override void Start()
        {
            _eventAggregator.Subscribe(this);
        }

        public override void Stop()
        {
            _eventAggregator.Unsubscribe(this);
        }

        #endregion

        #region IHandle<MessageAdded>

        void IHandle<MessageAdded>.Handle(MessageAdded message)
        {
            _vibrate.Start(TimeSpan.FromSeconds(0.5));
        }

        #endregion
    }
}