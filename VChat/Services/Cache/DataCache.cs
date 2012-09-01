using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using VChat.Models;

namespace VChat.Services.Cache
{
    public class DataCache : DataContext, IDataCache
    {
        private readonly object _lock = new object();

        private Table<User> _users;
        private Table<Message> _messages;
        private Table<Association> _associations;
        private Table<Chat> _chats;


        public DataCache()
            : base("Data Source=isostore:/DataCache.sdf")
        {
            DeferredLoadingEnabled = true;
            ObjectTrackingEnabled = false;

            LoadOptions.LoadWith<MessagePhotoAttachment>(att => att.Photo);
            LoadOptions.LoadWith<MessageAudioAttachment>(att => att.Audio);
            LoadOptions.LoadWith<MessageVideoAttachment>(att => att.Video);
            LoadOptions.LoadWith<MessageDocumentAttachment>(att => att.Document);
            LoadOptions.LoadWith<MessageWallAttachment>(att => att.Wall);
        }

        public Table<User> Users
        {
            get { return _users ?? (_users = GetTable<User>()); }
        }

        public Table<Association> Associations
        {
            get { return _associations ?? (_associations = GetTable<Association>()); }
        }

        public Table<Message> Messages
        {
            get { return _messages ?? (_messages = GetTable<Message>()); }
        }

        public Table<Chat> Chats
        {
            get { return _chats ?? (_chats = GetTable<Chat>()); }
        }

        #region IDataCache

        public Message[] GetMessages()
        {
            lock (_lock)
            {
                var dialogs = Messages.Where(m => m.ChatId == null).GroupBy(m => m.UserId);
                var chats = Messages.Where(m => m.ChatId != null).GroupBy(m => m.ChatId);

                return dialogs
                    .Select(g => g.OrderByDescending(m => m.Timestamp).First())
                    .Concat(chats.Select(g => g.OrderByDescending(m => m.Timestamp).First()))
                    .OrderByDescending(m => m.Timestamp)
                    .ToArray();
            }
        }

        public Message[] UpdateMessages(Message[] messages)
        {
            lock (_lock)
            {
                var dialogs = SyncDialogs(messages);
                var chats = SyncChats(messages);

                SubmitChanges();

                return dialogs.Concat(chats).OrderByDescending(m => m.Timestamp).ToArray();
            }
        }

        private IEnumerable<Message> SyncDialogs(IEnumerable<Message> messages)
        {
            var dialogs = messages.Where(m => m.ChatId == null).GroupBy(m => m.UserId).ToList();
            var cached = Messages.Where(m => m.ChatId == null).GroupBy(m => m.UserId).ToList();

            foreach (var dialog in cached)
            {
                if (dialogs.Any(m => m.Key == dialog.Key))
                {
                    var message = dialog.OrderByDescending(m => m.Timestamp).First();

                    Messages.Attach(message, true);
                }
                else
                {
                    Messages.DeleteAllOnSubmit(dialog);
                }
            }

            foreach (var dialog in dialogs.Where(m => !cached.Any(g => g.Key == m.Key)))
            {
                Messages.InsertOnSubmit(dialog.First());
            }

            return dialogs.Select(g => g.First());
        }

        private IEnumerable<Message> SyncChats(IEnumerable<Message> messages)
        {
            var chats = messages.Where(m => m.ChatId != null).GroupBy(m => m.ChatId).ToList();
            var cached = Messages.Where(m => m.ChatId != null).GroupBy(m => m.ChatId).ToList();

            foreach (var chat in cached)
            {
                if (chats.Any(m => m.Key == chat.Key))
                {
                    var message = chat.OrderByDescending(m => m.Timestamp).First();

                    Messages.Attach(message);
                }
                else
                {
                    Messages.DeleteAllOnSubmit(chat);
                    Chats.DeleteOnSubmit(chat.First().Chat);
                }
            }

            foreach (var message in chats.Where(m => !cached.Any(g => g.Key == m.Key)))
            {
                Chats.InsertOnSubmit(message.First().Chat);
                Messages.InsertOnSubmit(message.First());
            }

            return chats.Select(c => c.First());
        }

        #endregion
    }
}