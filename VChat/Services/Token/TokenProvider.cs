using VChat.Models;
using VChat.Services.Configuration;

namespace VChat.Services.Token
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IConfiguration _configuration;
        private OAuthToken _token;

        public TokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region ITokenProvider

        public OAuthToken GetToken()
        {
            return _token ?? (_token = _configuration.Get<OAuthToken>("OAuthToken"));
        }

        public void SetToken(OAuthToken token)
        {
            _configuration.Set("OAuthToken", _token = token);
        }

        #endregion
    }
}