using System.IO.IsolatedStorage;

namespace VChat.Services.Configuration
{
    public class ApplicationConfiguration : IConfiguration
    {
        #region IConfiguration

        public bool Contains(string key)
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(key);
        }

        public T Get<T>(string key)
        {
            T value;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value);
            return value;
        }

        public void Set<T>(string key, T value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        #endregion
    }
}