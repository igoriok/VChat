using System;

namespace VChat.Services.Maps
{
    public interface IMapService
    {
        Uri GetPreview(double latitude, double longitude);
    }
}