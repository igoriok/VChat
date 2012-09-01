using System;
using System.IO;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Reactive;
using VChat.Services.Configuration;
using VChat.Services.Vkontakte;

namespace VChat.Services.System
{
    public class PushService : BaseUserService, IDisposable
    {
        public const string ChannelName = "VChat_Channel";

        private readonly IVkClient _client;

        private HttpNotificationChannel _channel;

        public PushService(IVkClient client, IConfiguration configuration)
            : base(configuration, true)
        {
            _client = client;
        }

        #region ISystemService

        public override void Disable(bool stop)
        {
            if (_channel != null && stop)
            {
                _channel.Close();
            }

            base.Disable(stop);
        }

        public override void Start()
        {
            _channel = HttpNotificationChannel.Find(ChannelName);

            if (_channel == null)
            {
                _channel = new HttpNotificationChannel(ChannelName);

                Attach(_channel);

                _channel.Open();
                _channel.BindToShellToast();
            }
            else
            {
                Attach(_channel);

                OnChannelUriUpdated(_channel.ChannelUri);
            }
        }

        public override void Stop()
        {
            if (_channel != null)
            {
                Dettach(_channel);

                _channel.Dispose();
                _channel = null;
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }
        }

        #endregion

        private void Attach(HttpNotificationChannel channel)
        {
            channel.ShellToastNotificationReceived += Channel_ShellToastNotificationReceived;
            channel.HttpNotificationReceived += Channel_HttpNotificationReceived;
            channel.ChannelUriUpdated += Channel_ChannelUriUpdated;
            channel.ErrorOccurred += Channel_ErrorOccurred;
        }

        private void Dettach(HttpNotificationChannel channel)
        {
            channel.ShellToastNotificationReceived -= Channel_ShellToastNotificationReceived;
            channel.HttpNotificationReceived -= Channel_HttpNotificationReceived;
            channel.ChannelUriUpdated -= Channel_ChannelUriUpdated;
            channel.ErrorOccurred -= Channel_ErrorOccurred;
        }

        private void Channel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            // обрабатываем коллекцию e.Collection,
            // если есть необходимость
        }

        private void Channel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            using (var reader = new StreamReader(e.Notification.Body))
            {
                var message = reader.ReadToEnd();
                // теперь в message храниться XML-строка,
                // которую необходимо разобрать
                // и представить пользователю
            }
        }

        private void Channel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            OnChannelUriUpdated(e.ChannelUri);

            // отправляем запрос к VK.API
            // с методом account.registerDevice
            // и параметром token = channelUri
        }

        private void OnChannelUriUpdated(Uri channelUri)
        {
            _client.RegisterDevice(channelUri.ToString()).Subscribe(t => { });
        }

        private void Channel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            // обрабатываем ошибку
        }
    }
}