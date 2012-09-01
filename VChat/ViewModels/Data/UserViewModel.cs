using System;
using System.IO;
using System.Windows.Media;
using Caliburn.Micro;
using Microsoft.Phone.UserData;
using NotifyPropertyWeaver;
using VChat.Models;

namespace VChat.ViewModels.Data
{
    public class UserViewModel : PropertyChangedBase
    {
        [NotifyProperty]
        public int Id { get; set; }

        [NotifyProperty]
        public string FirstName { get; set; }

        [NotifyProperty]
        public string LastName { get; set; }

        [NotifyProperty]
        public string Photo { get; set; }

        public Stream PhotoStream { get; set; }

        [NotifyProperty]
        public bool IsOnline { get; set; }

        [NotifyProperty]
        public DateTime? LastSeen { get; set; }

        public static UserViewModel Map(ChatUser user)
        {
            return Map(user.User);
        }

        public static UserViewModel Map(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Photo = user.Photo,
                IsOnline = user.IsOnline,
                LastSeen = user.LastSeen
            };
        }

        public static UserViewModel Map(Contact contact)
        {
            return new UserViewModel
            {
                Id = contact.GetHashCode(),
                FirstName = contact.CompleteName.FirstName,
                LastName = contact.CompleteName.LastName
            };
        }
    }
}