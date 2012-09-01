using System;
using VChat.Models;

namespace VChat.Services.Videos
{
    public interface IVideoProvider
    {
        Uri GetPlayUri(Video video);
    }
}