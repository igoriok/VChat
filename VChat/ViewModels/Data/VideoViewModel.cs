using System;
using NotifyPropertyWeaver;
using VChat.Models;

namespace VChat.ViewModels.Data
{
    public class VideoViewModel : AttachmentViewModel
    {
        [NotifyProperty]
        public int Id { get; set; }

        [NotifyProperty]
        public OwnerViewModel Owner { get; set; }

        [NotifyProperty]
        public string Title { get; set; }

        [NotifyProperty]
        public string Description { get; set; }

        [NotifyProperty]
        public TimeSpan Duration { get; set; }

        [NotifyProperty]
        public string Image { get; set; }

        [NotifyProperty]
        public string ImageBig { get; set; }

        [NotifyProperty]
        public string ImageSmall { get; set; }

        [NotifyProperty]
        public int Views { get; set; }

        [NotifyProperty]
        public DateTime Timestamp { get; set; }

        public static VideoViewModel Map(Video video)
        {
            return new VideoViewModel
            {
                Id = video.Id,
                Owner = new OwnerViewModel { Id = video.OwnerId },
                Title = video.Title,
                Description = video.Description,
                Duration = video.Duration,
                Timestamp = video.Timestamp,
                Views = video.Views,
                Image = video.Image,
                ImageBig = video.ImageBig,
                ImageSmall = video.ImageSmall
            };
        }
    }
}