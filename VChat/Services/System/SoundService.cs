using System;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using VChat.Events.Update;
using VChat.Services.Configuration;

namespace VChat.Services.System
{
    public class SoundService : BaseUserService, IHandle<MessageAdded>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISoundEffectPlayer _soundEffect;

        public SoundService(IEventAggregator eventAggregator, ISoundEffectPlayer soundEffect, IConfiguration configuration)
            : base(configuration, true)
        {
            _eventAggregator = eventAggregator;
            _soundEffect = soundEffect;
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
            _soundEffect.Play(new Uri("/VChat;component/Resources/Sound/message.wav", UriKind.Relative));
        }

        #endregion
    }
}