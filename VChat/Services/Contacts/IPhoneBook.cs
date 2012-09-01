using System;
using Microsoft.Phone.UserData;

namespace VChat.Services.Contacts
{
    public interface IPhoneBook
    {
        IObservable<Contact> GetById(int id);
        IObservable<Contact[]> GetContacts();
        IObservable<Contact[]> SearchContacts(string filter);
    }
}