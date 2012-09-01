using VChat.Services.Configuration;

namespace VChat.Services.System
{
    public abstract class BaseUserService : IUserService, ISystemService
    {
        private readonly IConfiguration _configuration;
        private readonly bool _enabled;
        private readonly string _key;

        protected BaseUserService(IConfiguration configuration, bool enabled)
        {
            _configuration = configuration;
            _enabled = enabled;
            _key = GetType().Name + "Enabled";
        }

        #region ISystemService

        public bool IsEnabled
        {
            get { return _configuration.Contains(_key) ? _configuration.Get<bool>(_key) : _enabled; }
        }

        public virtual void Enable(bool start)
        {
            start &= !IsEnabled;

            _configuration.Set(_key, true);

            if (start)
            {
                Start();
            }
        }

        public virtual void Disable(bool stop)
        {
            stop &= IsEnabled;

            _configuration.Set(_key, false);

            if (stop)
            {
                Stop();
            }
        }

        void ISystemService.Start()
        {
            if (IsEnabled)
            {
                Start();
            }
        }

        void ISystemService.Stop()
        {
            if (IsEnabled)
            {
                Stop();
            }
        }

        #endregion

        public abstract void Start();
        public abstract void Stop();
    }
}