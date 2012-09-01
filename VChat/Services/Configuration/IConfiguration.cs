namespace VChat.Services.Configuration
{
    public interface IConfiguration
    {
        bool Contains(string key);
        T Get<T>(string key);
        void Set<T>(string key, T value);
    }
}