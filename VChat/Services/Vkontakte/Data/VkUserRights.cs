using System;

namespace VChat.Services.Vkontakte.Data
{
    [Flags]
    public enum VkUserRights
    {
        Alerts = 1,
        Friends = 2,
        Fotos = 4,
        Audios = 8,
        Videos = 16,
        Applications = 32,
        Questions = 64,
        Wiki = 128,
        ApplicationLink = 256,
        WallLink = 512,
        Statuses = 1024,
        Notes = 2048,
        Messages = 4096,
        Wall = 8192,
        Ads = 32768,
        Documents = 131072,
        Groups = 262144,
        Notifications = 524288,
        Statistics = 1048576
    }
}