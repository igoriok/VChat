using System;
using System.Linq;
using Caliburn.Micro;
using NotifyPropertyWeaver;
using VChat.Models;

using Message = VChat.Models.Message;

namespace VChat.ViewModels.Data
{
    public class ConversationViewModel : PropertyChangedBase
    {
        [NotifyProperty]
        public int? ChatId { get; set; }

        [NotifyProperty]
        public int? UserId { get; set; }

        [NotifyProperty]
        public string Title { get; set; }

        [NotifyProperty]
        public UserViewModel[] Users { get; set; }

        [NotifyProperty]
        public MessageViewModel Message { get; set; }

        public static ConversationViewModel Map(Chat chat)
        {
            return new ConversationViewModel
            {
                ChatId = chat.Id,
                Title = chat.Title,
                Users = chat.Users.Select(UserViewModel.Map).ToArray()
            };
        }

        public static ConversationViewModel Map(Message message)
        {
            var chat = message.Chat;

            var viewModel = new ConversationViewModel
            {
                ChatId = chat == null ? (int?)null : chat.Id,
                UserId = chat == null ? message.UserId : (int?)null,
                Title = chat == null ? string.Format("{0} {1}", message.User.FirstName, message.User.LastName) : chat.Title,
                Message = MessageViewModel.Map(message),
                Users = chat == null ? new[] { UserViewModel.Map(message.User) } : chat.Users.Select(UserViewModel.Map).ToArray()
            };

            viewModel.Message.Body = CropMessageBody(viewModel.Message.Body);

            if (message.Attachments.Count > 0)
            {
                viewModel.Message.Body = message.Attachments.Count > 1
                    ? string.Format("{0} attachment(s)", message.Attachments.Count)
                    : GetAttachmentTitle(message.Attachments[0].Type);
            }

            return viewModel;
        }

        private static string CropMessageBody(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                return body;
            }

            return body.Split(Environment.NewLine.ToCharArray()).FirstOrDefault();
        }

        private static string GetAttachmentTitle(string type)
        {
            switch (type)
            {
                case "audio": return "Audio";
                case "photo": return "Photo";
                case "video": return "Video";
                case "wall": return "Wall";

                default: return "Attachment";
            }
        }
    }
}