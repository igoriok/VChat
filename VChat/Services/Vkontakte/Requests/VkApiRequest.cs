using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json.Linq;

using VChat.Services.Token;

namespace VChat.Services.Vkontakte.Requests
{
    public class VkApiRequest : ObservableJsonRequest
    {
        private const string ApiUrl = "https://api.vk.com/method/";

        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();

        private readonly ITokenProvider _token;

        public VkApiRequest(string methodName, ITokenProvider token = null)
            : base(ApiUrl + methodName)
        {
            _token = token;
        }

        public VkApiRequest AddParameter(string name, string value)
        {
            _parameters.Add(Uri.EscapeUriString(name), Uri.EscapeUriString(value));
            return this;
        }

        protected override Uri BuildUri()
        {
            var builder = new UriBuilder(base.BuildUri());

            var args = new Dictionary<string, string>(_parameters);

            if (_token != null)
            {
                args.Add("access_token", _token.GetToken().AccessToken);
            }

            if (args.Count > 0)
            {
                builder.Query = GetQuery(args);
            }

            return builder.Uri;
        }

        protected override JToken ReadResult(Stream stream)
        {
            var result = base.ReadResult(stream);
            var response = Parser.ParseReponse(result);

            return response;
        }

        private string GetQuery(IEnumerable<KeyValuePair<string, string>> query)
        {
            var builder = new StringBuilder();

            var isSecond = false;

            foreach (var pair in query)
            {
                if (isSecond)
                {
                    builder.Append("&");
                }
                else
                {
                    isSecond = true;
                }

                builder.AppendFormat("{0}={1}", pair.Key, pair.Value);
            }

            return builder.ToString();
        }
    }
}