using System;
using System.Linq;
using Caliburn.Micro;
using NotifyPropertyWeaver;
using VChat.Models;

using Message = VChat.Models.Message;

namespace VChat.ViewModels.Data
{
    public class MessageViewModel : PropertyChangedBase
    {
        [NotifyProperty]
        public int Id { get; set; }

        [NotifyProperty]
        public DateTime Timestamp { get; set; }

        [NotifyProperty]
        public UserViewModel User { get; set; }

        [NotifyProperty]
        public string Title { get; set; }

        [NotifyProperty]
        public string Body { get; set; }

        [NotifyProperty]
        public bool IsRead { get; set; }

        [NotifyProperty]
        public bool IsOut { get; set; }

        [NotifyProperty]
        public GeoViewModel Geo { get; set; }

        [NotifyProperty]
        public ForwardMessageViewModel[] ForwardMessages { get; set; }

        [NotifyProperty]
        public AttachmentViewModel[] Attachments { get; set; }

        public static MessageViewModel Map(Message message)
        {
            var viewModel = new MessageViewModel
            {
                Id = message.Id,
                Timestamp = message.Timestamp,
                User = UserViewModel.Map(message.User),
                IsRead = message.IsRead,
                IsOut = message.IsOut,
                Title = message.Title,
                Body = ConvertMessageBody(message.Body)
            };

            if (message.Geo != null)
            {
                viewModel.Geo = GeoViewModel.Map(message.Geo);
            }

            if (message.ForwardMessages.Count > 0)
            {
                viewModel.ForwardMessages = message.ForwardMessages.Select(ForwardMessageViewModel.Map).ToArray();
            }

            if (message.Attachments.Count > 0)
            {
                viewModel.Attachments = message.Attachments.Select(AttachmentViewModel.Map).ToArray();
            }

            return viewModel;
        }

        public static string ConvertMessageBody(string body)
        {
            return body
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("<br>", Environment.NewLine);
        }
    }
}