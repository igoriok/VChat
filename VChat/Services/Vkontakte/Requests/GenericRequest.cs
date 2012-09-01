using System;
using System.IO;

namespace VChat.Services.Vkontakte.Requests
{
    public class GenericRequest<T> : ObservableWebRequest<T>
    {
        private readonly string _url;
        private readonly Func<Stream, T> _selector;

        public GenericRequest(string url, Func<Stream, T> selector)
        {
            _url = url;
            _selector = selector;
        }

        #region ObservableWebRequest<string>

        protected override Uri BuildUri()
        {
            return new Uri(_url);
        }

        protected override T ReadResult(Stream stream)
        {
            return _selector(stream);
        }

        #endregion
    }
}