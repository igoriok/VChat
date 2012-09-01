using System;
using Microsoft.Phone.Reactive;
using VChat.Models;
using VChat.Services.Cache;
using VChat.Services.Vkontakte;

namespace VChat.Services.Data
{
    public class DataService : IDataService
    {
        private readonly IDataCache _cache;
        private readonly IVkClient _client;

        #region IDataService

        public DataService(IDataCache cache, IVkClient client)
        {
            _cache = cache;
            _client = client;
        }

        public IObservable<Message[]> GetMessages()
        {
            return Observable
                .Defer(Observable.ToAsync(() => _cache.GetMessages()))
                .Concat(_client.GetMessages(1, 0).Select(_cache.UpdateMessages));
        }

        #endregion
    }
}