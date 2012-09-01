using System;
using NotifyPropertyWeaver;
using VChat.Models;

namespace VChat.ViewModels.Data
{
    public class AudioViewModel : AttachmentViewModel
    {
        [NotifyProperty]
        public int Id { get; set; }

        [NotifyProperty]
        public string Performer { get; set; }

        [NotifyProperty]
        public string Title { get; set; }

        [NotifyProperty]
        public TimeSpan Duration { get; set; }

        [NotifyProperty]
        public bool IsPlaying { get; set; }

        [NotifyProperty]
        public string Url { get; set; }

        public static AudioViewModel Map(Audio audio)
        {
            return new AudioViewModel
            {
                Id = audio.Id,
                Performer = audio.Performer,
                Title = audio.Title,
                Duration = audio.Duration,
                Url = audio.Url
            };
        }
    }
}