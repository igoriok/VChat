using System;
using System.IO;
using Microsoft.Phone.UserData;

namespace VChat.Services.Cache
{
    public interface IPhotoCache
    {
        Stream GetContactPhoto(Contact contact);
        Stream SaveContactPhoto(Contact contact);

        Stream GetUserPhoto(int id);
        IObservable<Stream> SaveUserPhoto(int id, string uri);
    }
}