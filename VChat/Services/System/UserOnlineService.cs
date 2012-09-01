using System;
using Microsoft.Phone.Reactive;

using VChat.Mvvm;
using VChat.Services.Configuration;
using VChat.Services.Vkontakte;

namespace VChat.Services.System
{
    public class UserOnlineService : BaseUserService
    {
        private readonly IVkClient _client;

        private IDisposable _disposable = Disposable.Empty;

        public UserOnlineService(IVkClient client, IConfiguration configuration)
            : base(configuration, true)
        {
            _client = client;
        }

        #region BaseUserService

        public override void Start()
        {
            _disposable.Dispose();

            _disposable = _client.SetOnline().Repeat(TimeSpan.FromSeconds(5)).Subscribe();
        }

        public override void Stop()
        {
            _disposable.Dispose();
        }

        #endregion
    }
}