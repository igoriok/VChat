using System;
using VChat.Models;

namespace VChat.Services.Videos
{
    public class VideoProvider : IVideoProvider
    {
        #region Implementation of IVideoProvider

        public Uri GetPlayUri(Video video)
        {
            var uri = video.Files.Video240 ?? video.Files.External;

            return new Uri(uri);
        }

        #endregion
    }
}