using VChat.Models;

namespace VChat.Services.Token
{
    public interface ITokenProvider
    {
        OAuthToken GetToken();
        void SetToken(OAuthToken token);
    }
}