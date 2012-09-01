using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VChat.Services.Vkontakte.Requests
{
    public class ObservableJsonRequest : ObservableWebRequest<JToken>
    {
        private readonly string _url;

        public ObservableJsonRequest(string url)
        {
            _url = url;
        }

        protected override Uri BuildUri()
        {
            return new Uri(_url);
        }

        protected override JToken ReadResult(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                using (var json = new JsonTextReader(sr))
                {
                    return JToken.ReadFrom(json);
                }
            }
        }
    }
}