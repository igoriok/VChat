using System;
using System.Linq;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.UserData;

namespace VChat.Services.Contacts
{
    public class PhoneBook : IPhoneBook, IDisposable
    {
        private readonly Microsoft.Phone.UserData.Contacts _contacts;

        #region IPhoneBook

        public PhoneBook()
        {
            _contacts = new Microsoft.Phone.UserData.Contacts();
            _contacts.SearchCompleted += Contacts_SearchCompleted;
        }

        public IObservable<Contact> GetById(int id)
        {
            return GetContacts()
                .SelectMany(contacts => contacts)
                .Where(contact => contact.GetHashCode() == id);
        }

        public IObservable<Contact[]> GetContacts()
        {
            return Observable.CreateWithDisposable<Contact[]>(observer =>
            {
                _contacts.SearchAsync(string.Empty, FilterKind.None, observer);

                return Disposable.Empty;
            });
        }

        public IObservable<Contact[]> SearchContacts(string filter)
        {
            return Observable.CreateWithDisposable<Contact[]>(observer =>
            {
                _contacts.SearchAsync(filter, FilterKind.DisplayName, observer);

                return Disposable.Empty;
            });
        }

        #endregion

        public void Dispose()
        {
            _contacts.SearchCompleted -= Contacts_SearchCompleted;
        }

        private void Contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            var observer = e.State as IObserver<Contact[]>;
            if (observer != null)
            {
                try
                {
                    observer.OnNext(e.Results.ToArray());
                }
                catch (Exception exception)
                {
                    observer.OnError(exception);
                }

                observer.OnCompleted();
            }
        }
    }
}