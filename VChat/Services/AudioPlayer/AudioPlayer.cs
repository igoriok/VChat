using System;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Reactive;

using VChat.Models;

namespace VChat.Services.AudioPlayer
{
    public class AudioPlayer : IAudioPlayer
    {
        private readonly BackgroundAudioPlayer _audioPlayer;
        private readonly IObservable<PlayState> _playStateChanged;

        public PlayState PlayState
        {
            get { return _audioPlayer.PlayerState; }
        }

        public int? CurrentAudioId
        {
            get
            {
                var track = _audioPlayer.Track;
                int audioId;

                if (track != null && !string.IsNullOrEmpty(track.Tag) && int.TryParse(track.Tag, out audioId))
                {
                    return audioId;
                }

                return null;
            }
        }

        public IObservable<PlayState> PlayStateChanged
        {
            get { return _playStateChanged; }
        }

        public AudioPlayer()
        {
            _audioPlayer = BackgroundAudioPlayer.Instance;

            _playStateChanged = Observable
                .FromEvent<EventHandler, EventArgs>(
                    handler => (s, e) => handler(s, e),
                    handler => _audioPlayer.PlayStateChanged += handler,
                    handler => _audioPlayer.PlayStateChanged -= handler)
                .Select(e => _audioPlayer.PlayerState).Publish().RefCount();
        }

        #region IAudioPlayer

        public void Play(Audio audio)
        {
            _audioPlayer.Play();
        }

        public void Pause()
        {
            _audioPlayer.Pause();
        }

        #endregion
    }
}