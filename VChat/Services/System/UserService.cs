using Caliburn.Micro;
using VChat.Services.Configuration;

namespace VChat.Services.System
{
    public class UserService : BaseUserService
    {
        private readonly IUserService _online;
        private readonly IUserService _longPoll;
        private readonly IUserService _sound;
        private readonly IUserService _vibrate;
        private readonly IUserService _push;

        public UserService(IConfiguration configuration)
            : base(configuration, false)
        {
            _online = IoC.Get<IUserService>("UserOnlineService");
            _longPoll = IoC.Get<IUserService>("LongPollService");
            _sound = IoC.Get<IUserService>("SoundService");
            _vibrate = IoC.Get<IUserService>("VibrateService");
            _push = IoC.Get<IUserService>("PushService");
        }

        #region BaseUserService

        public override void Start()
        {
            _online.Start();
            _longPoll.Start();
            _sound.Start();
            _vibrate.Start();
            _push.Start();
        }

        public override void Stop()
        {
            _online.Stop();
            _longPoll.Stop();
            _sound.Stop();
            _vibrate.Stop();
            _push.Stop();
        }

        #endregion
    }
}