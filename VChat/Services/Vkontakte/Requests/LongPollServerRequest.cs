using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Reactive;
using Newtonsoft.Json.Linq;

using VChat.Events.Update;
using VChat.Mvvm;
using VChat.Services.Token;
using VChat.Services.Vkontakte.Data;

namespace VChat.Services.Vkontakte.Requests
{
    public class LongPollServerRequest : IObservable<Update>
    {
        private readonly ITokenProvider _token;

        public LongPollServerRequest(ITokenProvider token)
        {
            _token = token;
        }

        #region IObservable<JToken>

        public IDisposable Subscribe(IObserver<Update> observer)
        {
            var request = new VkApiRequest("messages.getLongPollServer", _token);

            return request.SelectMany(Poll).Subscribe(observer);
        }

        #endregion

        private IObservable<Update> Poll(JToken token)
        {
            var response = token.ToObject<VkLongPollServer>();

            return new PollLongPollServer(response).Repeat(TimeSpan.FromSeconds(1));
        }

        private class PollLongPollServer : IObservable<Update>
        {
            private readonly VkLongPollServer _server;

            public PollLongPollServer(VkLongPollServer server)
            {
                _server = server;
            }

            public IDisposable Subscribe(IObserver<Update> observer)
            {
                var url = string.Format("http://{0}?act=a_check&key={1}&ts={2}&wait=25&mode=0", _server.Server, _server.Key, _server.Timestamp);

                return new ObservableJsonRequest(url)
                    .SelectMany(SelectMany)
                    .Subscribe(observer);
            }

            private IEnumerable<Update> SelectMany(JToken token)
            {
                var data = token.ToObject<VkLongPollData>();
                if (data.Failed.HasValue)
                {
                    throw new VkException(data.Failed.Value, "LongPollServer failed to process the request");
                }

                _server.Timestamp = data.Timestamp;

                var updates = token["updates"];
                if (updates == null)
                {
                    throw new VkException(1, "Missing 'updates' field");
                }

                return updates.Children<JArray>().Reverse().Select(ParseUpdate);
            }

            private Update ParseUpdate(JArray array)
            {
                var code = array.First.Value<int>();

                var update = ParseUpdate(code, array);

                update.Code = code;

                return update;
            }

            private Update ParseUpdate(int code, JArray array)
            {
                switch (code)
                {
                    case 0: return ParseMessageDeleted(array);
                    case 1: return ParseMessageFlagChanged(array);
                    case 2: return ParseMessageFlagAdded(array);
                    case 3: return ParseMessageFlagRemoved(array);
                    case 4: return ParseMessageAdded(array);
                    case 8: return ParseUserOnline(array);
                    case 9: return ParseUserOffline(array);
                    case 51: return ParseChatChanged(array);
                    case 61: return ParseUserTyping(array);
                    case 62: return ParseUserTypingInChat(array);
                    case 70: return ParseUserCalled(array);
                    default: return new Update();
                }
            }

            private Update ParseMessageDeleted(JArray array)
            {
                var messageId = array[1].Value<int>();

                return new MessageDeleted { MessageId = messageId };
            }

            private Update ParseMessageFlagChanged(JArray array)
            {
                var messageId = array[1].Value<int>();
                var flag = array[2].Value<MessageFlag>();

                return new MessageFlagChanged { MessageId = messageId, MessageFlag = flag };
            }

            private Update ParseMessageFlagAdded(JArray array)
            {
                var messageId = array[1].Value<int>();
                var flag = (MessageFlag)array[2].Value<int>();
                var userId = array.Count > 2 ? array[3].Value<int>() : (int?)null;

                return new MessageFlagAdded { MessageId = messageId, MessageFlag = flag, UserId = userId };
            }

            private Update ParseMessageFlagRemoved(JArray array)
            {
                var messageId = array[1].Value<int>();
                var flag = (MessageFlag)array[2].Value<int>();
                var userId = array.Count > 3 ? array[3].Value<int>() : (int?)null;

                return new MessageFlagRemoved { MessageId = messageId, MessageFlag = flag, UserId = userId };
            }

            private Update ParseMessageAdded(JArray array)
            {
                var messageId = array[1].Value<int>();

                return new MessageAdded { MessageId = messageId };
            }

            private Update ParseUserOnline(JArray array)
            {
                var userId = -array[1].Value<int>();

                return new UserOnline { UserId = userId };
            }

            private Update ParseUserOffline(JArray array)
            {
                var userId = -array[1].Value<int>();
                var isTimeout = array[2].Value<bool>();

                return new UserOffline { UserId = userId, IsTimeout = isTimeout };
            }

            private Update ParseChatChanged(JArray array)
            {
                var chatId = array[1].Value<int>();
                var isSelf = array[2].Value<bool>();

                return new ChatChanged { ChatId = chatId, IsSelf = isSelf };
            }

            private Update ParseUserTyping(JArray array)
            {
                var userId = array[1].Value<int>();

                return new UserTyping { UserId = userId };
            }

            private Update ParseUserTypingInChat(JArray array)
            {
                var userId = array[1].Value<int>();
                var chatId = array[2].Value<int>();

                return new UserTypingInChat { UserId = userId, ChatId = chatId };
            }

            private Update ParseUserCalled(JArray array)
            {
                var userId = array[1].Value<int>();
                var callId = array[2].Value<int>();

                return new UserCalled { UserId = userId, CallId = callId };
            }
        }
    }
}