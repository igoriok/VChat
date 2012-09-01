using System.IO;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.UserData;
using NotifyPropertyWeaver;

using VChat.Mvvm;
using VChat.Services.Contacts;
using VChat.ViewModels.Data;

namespace VChat.ViewModels
{
    public class ContactsViewModel : MainTabItem
    {
        private readonly IPhoneBook _phoneBook;

        [NotifyProperty]
        public UserViewModel ActiveContact { get; set; }

        public BindableCollection<Group<string, UserViewModel>> Contacts { get; private set; }

        public DelegateCommand SynchronizeCommand { get; private set; }

        public ContactsViewModel(IPhoneBook phoneBook)
        {
            _phoneBook = phoneBook;

            Contacts = new BindableCollection<Group<string, UserViewModel>>();
            SynchronizeCommand = new DelegateCommand(Synchronize);

            DisplayName = "contacts";
        }

        protected override void OnInitialize()
        {
            this.StartBusy("loading...");

            _phoneBook
                .GetContacts()
                .Select(contacts => contacts
                    .Select(MapContact)
                    .GroupBy(u => u.FirstName.Substring(0, 1))
                    .Select(g => new Group<string, UserViewModel>(g.Key, g.ToList()))
                    .ToList())
                .ObserveOnDispatcher()
                .Finally(this.StopBusy)
                .Subscribe(viewModels => Contacts.AddRange(viewModels));
        }

        private UserViewModel MapContact(Contact contact)
        {
            var user = UserViewModel.Map(contact);

            using (var picture = contact.GetPicture())
            {
                if (picture != null)
                {
                    picture.CopyTo(user.PhotoStream = new MemoryStream());
                }
            }

            return user;
        }

        private void Synchronize()
        {
        }
    }
}