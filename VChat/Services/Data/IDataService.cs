using System;
using VChat.Models;

namespace VChat.Services.Data
{
    public interface IDataService
    {
        IObservable<Message[]> GetMessages();
    }
}