using System;
using VChat.Models;

namespace VChat.Services.Cache
{
    public interface IUserCache
    {
        User GetUser(int id);
        Group GetGroup(int id);
        Owner GetOwner(int id);

        IObservable<T> Flush<T>(T result);
    }
}