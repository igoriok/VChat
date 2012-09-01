using System;
using System.Windows;
using Microsoft.Phone.BackgroundAudio;

namespace VkAudioPlaybackAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// AudioPlayer instances can share the same process. 
        /// Static fields can be used to share state between AudioPlayer instances
        /// or to communicate with the Audio Streaming agent.
        /// </remarks>
        public AudioPlayer()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;

                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += AudioPlayer_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void AudioPlayer_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        protected override void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
            switch (action)
            {
                case UserAction.Play:
                    if (player.PlayerState != PlayState.Playing)
                    {
                        player.Play();
                    }
                    break;

                case UserAction.Stop:
                    if (player.PlayerState == PlayState.Playing)
                    {
                        player.Stop();
                    }
                    break;

                case UserAction.Pause:
                    player.Pause();
                    break;

                case UserAction.FastForward:
                    player.FastForward();
                    break;

                case UserAction.Rewind:
                    player.Rewind();
                    break;

                case UserAction.Seek:
                    player.Position = (TimeSpan)param;
                    break;

                case UserAction.SkipNext:
                    break;
                case UserAction.SkipPrevious:
                    break;
            }

            NotifyComplete();
        }
    }
}
