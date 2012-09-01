using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using NotifyPropertyWeaver;

using VChat.Mvvm;
using VChat.Services.Contacts;
using VChat.ViewModels.Data;

namespace VChat.ViewModels
{
    public class ContactInfoViewModel : Screen, IBusyState
    {
        private readonly IPhoneBook _phoneBook;

        [NotifyProperty]
        public bool IsBusy { get; set; }

        [NotifyProperty]
        public string Status { get; set; }

        [NotifyProperty]
        public int ContactId { get; set; }

        [NotifyProperty]
        public UserViewModel Contact { get; set; }

        public ContactInfoViewModel(IPhoneBook phoneBook)
        {
            _phoneBook = phoneBook;
        }

        protected override void OnInitialize()
        {
            _phoneBook
                .GetById(ContactId)
                .ObserveOnDispatcher()
                .Subscribe(contact => Contact = UserViewModel.Map(contact));
        }
    }
}