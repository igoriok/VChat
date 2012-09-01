using NotifyPropertyWeaver;
using VChat.Models;

namespace VChat.ViewModels.Data
{
    public class PhotoViewModel : AttachmentViewModel
    {
        [NotifyProperty]
        public int Id { get; set; }

        [NotifyProperty]
        public string Source { get; set; }

        [NotifyProperty]
        public string SourceBig { get; set; }

        public static PhotoViewModel Map(Photo photo)
        {
            return new PhotoViewModel
            {
                Id = photo.Id,
                Source = photo.Source,
                SourceBig = photo.SourceBig
            };
        }
    }
}