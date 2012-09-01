using System;
using System.Globalization;

namespace VChat.Services.Maps
{
    public class GoogleMapService : IMapService
    {
        private const string BaseUrl = "http://maps.googleapis.com/maps/api/staticmap";

        #region IMapService

        public Uri GetPreview(double latitude, double longitude)
        {
            var builder = new UriBuilder(BaseUrl)
            {
                Query = string.Format(
                    new CultureInfo("en-US"),
                    "center={0},{1}&zoom=12&size=380x240&format=jpg&language={2}&sensor=false",
                    latitude,
                    longitude,
                    GetCurrentLanguage())
            };

            return builder.Uri;
        }

        private string GetCurrentLanguage()
        {
            return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
        }

        #endregion
    }
}