using System;
using Caliburn.Micro;
using NotifyPropertyWeaver;
using VChat.Models;

namespace VChat.ViewModels.Data
{
    public class ForwardMessageViewModel : PropertyChangedBase
    {
        [NotifyProperty]
        public UserViewModel User { get; set; }

        [NotifyProperty]
        public DateTime Timestamp { get; set; }

        [NotifyProperty]
        public string Body { get; set; }

        public static ForwardMessageViewModel Map(ForwardMessage message)
        {
            return new ForwardMessageViewModel
            {
                User = UserViewModel.Map(message.User ?? new User { Id = message.UserId }),
                Timestamp = message.Timestamp,
                Body = MessageViewModel.ConvertMessageBody(message.Body)
            };
        }
    }
}