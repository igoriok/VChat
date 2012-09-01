using NotifyPropertyWeaver;
using VChat.Models;

namespace VChat.ViewModels.Data
{
    public class DocumentViewModel : AttachmentViewModel
    {
        [NotifyProperty]
        public int Id { get; set; }

        [NotifyProperty]
        public string Title { get; set; }

        [NotifyProperty]
        public long Size { get; set; }

        [NotifyProperty]
        public string Extension { get; set; }

        [NotifyProperty]
        public string Url { get; set; }

        public static DocumentViewModel Map(Document document)
        {
            return new DocumentViewModel
            {
                Id = document.Id,
                Title = document.Title,
                Extension = document.Extension,
                Size = document.Size,
                Url = document.Url
            };
        }
    }
}