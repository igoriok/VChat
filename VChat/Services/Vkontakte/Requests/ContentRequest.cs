using System;
using System.IO;

namespace VChat.Services.Vkontakte.Requests
{
    public class ContentRequest : ObservableWebRequest<string>
    {
        private readonly string _url;

        public ContentRequest(string url)
        {
            _url = url;
        }

        #region ObservableWebRequest<string>

        protected override Uri BuildUri()
        {
            return new Uri(_url);
        }

        protected override string ReadResult(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion
    }
}