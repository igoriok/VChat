using System;
using Microsoft.Phone.BackgroundAudio;
using VChat.Models;

namespace VChat.Services.AudioPlayer
{
    public interface IAudioPlayer
    {
        PlayState PlayState { get; }
        int? CurrentAudioId { get; }

        IObservable<PlayState> PlayStateChanged { get; }

        void Play(Audio audio);
        void Pause();
    }
}