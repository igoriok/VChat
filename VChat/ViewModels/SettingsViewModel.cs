using Caliburn.Micro;
using NotifyPropertyWeaver;
using VChat.Services.System;

namespace VChat.ViewModels
{
    public class SettingsViewModel : Screen
    {
        [NotifyProperty]
        public bool IsSoundEnabled { get; set; }

        [NotifyProperty]
        public bool IsVibrateEnabled { get; set; }

        [NotifyProperty]
        public bool IsPushEnabled { get; set; }

        [NotifyProperty]
        public bool IsHiddenEnabled { get; set; }

        public SettingsViewModel()
        {
            PropertyChanged += SettingsViewModel_PropertyChanged;
        }

        private void SettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (IsInitialized)
            {
                switch (e.PropertyName)
                {
                    case "IsSoundEnabled":

                        OnSoundEnabledChanged();
                        break;

                    case "IsVibrateEnabled":

                        OnVibrateEnabledChanged();
                        break;

                    case "IsPushEnabled":

                        OnPushEnabledChanged();
                        break;


                    case "IsHiddenEnabled":

                        OnHiddenEnabledChanged();
                        break;
                }
            }
        }

        protected override void OnInitialize()
        {
            IsSoundEnabled = IoC.Get<IUserService>("SoundService").IsEnabled;
            IsVibrateEnabled = IoC.Get<IUserService>("VibrateService").IsEnabled;
            IsPushEnabled = IoC.Get<IUserService>("PushService").IsEnabled;
            IsHiddenEnabled = !IoC.Get<IUserService>("UserOnlineService").IsEnabled;
        }

        private void OnSoundEnabledChanged()
        {
            if (IsSoundEnabled)
            {
                IoC.Get<IUserService>("SoundService").Enable(true);
            }
            else
            {
                IoC.Get<IUserService>("SoundService").Disable(true);
            }
        }

        private void OnVibrateEnabledChanged()
        {
            if (IsVibrateEnabled)
            {
                IoC.Get<IUserService>("VibrateService").Enable(true);
            }
            else
            {
                IoC.Get<IUserService>("VibrateService").Disable(true);
            }
        }

        private void OnPushEnabledChanged()
        {
            if (IsPushEnabled)
            {
                IoC.Get<IUserService>("PushService").Enable(true);
            }
            else
            {
                IoC.Get<IUserService>("PushService").Disable(true);
            }
        }

        private void OnHiddenEnabledChanged()
        {
            if (IsHiddenEnabled)
            {
                IoC.Get<IUserService>("UserOnlineService").Disable(true);
            }
            else
            {
                IoC.Get<IUserService>("UserOnlineService").Enable(true);
            }
        }
    }
}