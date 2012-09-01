using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Phone.Reactive;

using Newtonsoft.Json.Linq;

using VChat.Events.Update;
using VChat.Models;
using VChat.Services.Cache;
using VChat.Services.Token;
using VChat.Services.Vkontakte.Data;
using VChat.Services.Vkontakte.Requests;

namespace VChat.Services.Vkontakte
{
    public class VkClient : IVkClient
    {
        private const string ClientId = "3059423";
        private const string ClientSecret = "37aOgIpAQkusuN4QHHtB";

        private readonly ITokenProvider _token;
        private readonly IUserCache _cache;

        private const string AuthorizeUrl = "https://api.vk.com/oauth/token";

        public VkClient(ITokenProvider token)
        {
            _token = token;
            _cache = new UserCache(this);
        }

        public IObservable<bool> SetOnline()
        {
            return new VkApiRequest("account.setOnline", _token)
                .Select(Parser.ParseValue<bool>);
        }

        public IObservable<bool> RegisterDevice(string token)
        {
            return new VkApiRequest("account.registerDevice", _token)
                .AddParameter("token", Uri.EscapeDataString(token))
                .Select(Parser.ParseValue<bool>);
        }

        public IObservable<bool> UnregisterDevice(string token)
        {
            return new VkApiRequest("account.unregisterDevice", _token)
                .AddParameter("token", Uri.EscapeDataString(token))
                .Select(Parser.ParseValue<bool>);
        }

        public IObservable<int> GetUnreadMessagesCount()
        {
            return new VkApiRequest("messages.get", _token)
                .AddParameter("count", "0")
                .AddParameter("filters", "1")
                .Select(Enumerable.First)
                .Select(Parser.ParseValue<int>);
        }

        public IObservable<int[]> GetFriendsRequests(int count)
        {
            return new VkApiRequest("friends.getRequests", _token)
                .AddParameter("count", count.ToString())
                .Select(j => j.Children<JValue>().Select(Parser.ParseValue<int>).ToArray());
        }

        public IObservable<User[]> GetFriendsRequestsWithUsers(int count)
        {
            return Execute(string.Format(
                "var uids = API.friends.getRequests({{\"count\":{0}}})@.uid;" +
                "return API.users.get({{\"uids\":uids,\"fields\":\"sex,photo_rec,online,last_seen\"}});", count))
                .Select(Parser.ParseUsers);
        }

        public IObservable<User[]> GetFriendsSuggestions(int count)
        {
            return new VkApiRequest("friends.getSuggestions", _token)
                .AddParameter("count", count.ToString())
                .AddParameter("fields", "sex,photo_rec,online,last_seen")
                .Select(Parser.ParseUsers);
        }

        public IObservable<bool> AddFriend(int uid)
        {
            return new VkApiRequest("friends.add", _token)
                .AddParameter("uid", uid.ToString())
                .Select(Parser.ParseValue<bool>);
        }

        public IObservable<bool> DeleteFriend(int uid)
        {
            return new VkApiRequest("friends.delete", _token)
                .AddParameter("uid", uid.ToString())
                .Select(Parser.ParseValue<bool>);
        }

        public IObservable<Video> GetVideo(int ownerId, int videoId)
        {
            return new VkApiRequest("video.get", _token)
                .AddParameter("videos", string.Join("_", ownerId, videoId))
                .Select(Parser.ParseContainer)
                .Select(Enumerable.First)
                .Select(Parser.ParseVideo);
        }

        public IObservable<bool> CheckPhone(string phone)
        {
            return new VkApiRequest("auth.checkPhone")
                .AddParameter("phone", phone)
                .AddParameter("client_id", ClientId)
                .AddParameter("client_secret", ClientSecret)
                .Select(Parser.ParseValue<bool>);
        }

        public IObservable<string> Signup(string phone, string firstName, string lastName)
        {
            return new VkApiRequest("auth.signup")
                .AddParameter("phone", Uri.EscapeUriString(phone))
                .AddParameter("first_name", Uri.EscapeUriString(firstName))
                .AddParameter("last_name", Uri.EscapeUriString(lastName))
                .AddParameter("client_id", ClientId)
                .AddParameter("client_secret", ClientSecret)
                //.AddParameter("test_mode", "1")
                .Select(j => j["sid"].Value<string>());
        }

        public IObservable<int> Confirm(string phone, string code, string password)
        {
            return new VkApiRequest("auth.confirm")
                .AddParameter("phone", Uri.EscapeUriString(phone))
                .AddParameter("code", Uri.EscapeUriString(code))
                .AddParameter("password", Uri.EscapeUriString(password))
                .AddParameter("client_id", ClientId)
                .AddParameter("client_secret", ClientSecret)
                //.AddParameter("test_mode", "1")
                .Select(j => j["uid"].Value<int>());
        }

        public IObservable<OAuthToken> Authenticate(string username, string password)
        {
            var url = string.Format(
                AuthorizeUrl +
                "?grant_type=password" +
                "&client_id={0}" +
                "&client_secret={1}" +
                "&username={2}" +
                "&password={3}" +
                "&scope=friends,photos,video,groups,messages",
                ClientId, ClientSecret, username, password);

            return new ObservableJsonRequest(url)
                .Select(Parser.ParseToken);
        }

        public IObservable<Message[]> GetMessages(int count, int offset)
        {
            return new VkApiRequest("messages.getDialogs", _token)
                .AddParameter("count", count.ToString())
                .AddParameter("offset", offset.ToString())
                .Select(Parser.ParseContainer)
                .Select(ParseMessages)
                .SelectMany(_cache.Flush);
        }

        public IObservable<Message> GetMessage(int messageId)
        {
            return new VkApiRequest("messages.getById", _token)
                .AddParameter("mid", messageId.ToString())
                .Select(Parser.ParseContainer)
                .Select(ParseMessages)
                .Select(Enumerable.First)
                .SelectMany(_cache.Flush);
        }

        public IObservable<Chat> GetChat(int chatId)
        {
            return new VkApiRequest("messages.getChat", _token)
                .AddParameter("chat_id", chatId.ToString())
                .Select(ParseChat)
                .SelectMany(_cache.Flush);
        }

        public IObservable<Photo> UploadMessagePhoto(string name, Stream stream)
        {
            return new VkApiRequest("photos.getMessagesUploadServer", _token)
                .SelectMany(j => new ObservableJsonRequest(j["upload_url"].Value<string>())
                    .AddFile("photo", name, stream))
                .SelectMany(j => new VkApiRequest("photos.saveMessagesPhoto", _token)
                    .AddParameter("server", j["server"].Value<string>())
                    .AddParameter("photo", j["photo"].Value<string>())
                    .AddParameter("hash", j["hash"].Value<string>()))
                .Select(Enumerable.First)
                .Select(ParsePhoto);
        }

        public IObservable<int> SendDialogMessage(int userId, string message)
        {
            return new VkApiRequest("messages.send", _token)
                .AddField("uid", userId.ToString())
                .AddField("message", message)
                .Select(Parser.ParseValue<int>);
        }

        public IObservable<int> SendChatMessage(int chatId, string message)
        {
            return new VkApiRequest("messages.send", _token)
                .AddField("chat_id", chatId.ToString())
                .AddField("message", message)
                .Select(Parser.ParseValue<int>);
        }

        public IObservable<bool> DeleteDialog(int userId)
        {
            return new VkApiRequest("messages.deleteDialog", _token)
                .AddParameter("uid", userId.ToString())
                .Select(Parser.ParseValue<bool>);
        }

        public IObservable<bool> DeleteChat(int chatId)
        {
            return new VkApiRequest("messages.deleteDialog", _token)
                .AddParameter("chat_id", chatId.ToString())
                .Select(Parser.ParseValue<bool>);
        }

        public IObservable<bool> DeleteMessage(int messageId)
        {
            return new VkApiRequest("messages.delete", _token)
                .AddParameter("mids", messageId.ToString())
                .Select(j => j[messageId.ToString()].Value<bool>());
        }

        public IObservable<Message[]> GetDialogHistory(int userId, int count, int? fromId)
        {
            return new VkApiRequest("messages.getHistory", _token)
                .AddParameter("uid", userId.ToString())
                .AddParameter("count", (fromId.HasValue ? count + 1 : count).ToString())
                .AddParameter("rev", fromId.HasValue ? "1" : "0")
                .AddParameter("start_mid", fromId.ToString())
                .Select(Parser.ParseContainer)
                .Select(ParseMessages)
                .SelectMany(_cache.Flush)
                .Select(messages => fromId.HasValue ? messages.Reverse().Skip(1).ToArray() : messages);
        }

        public IObservable<Message[]> GetChatHistory(int chatId, int count, int? fromId)
        {
            return new VkApiRequest("messages.getHistory", _token)
                .AddParameter("chat_id", chatId.ToString())
                .AddParameter("count", (fromId.HasValue ? count + 1 : count).ToString())
                .AddParameter("rev", fromId.HasValue ? "1" : "0")
                .AddParameter("start_mid", fromId.ToString())
                .Select(Parser.ParseContainer)
                .Select(ParseMessages)
                .SelectMany(_cache.Flush)
                .Select(messages => fromId.HasValue ? messages.Reverse().Skip(1).ToArray() : messages);
        }

        public IObservable<Activity> GetLastActivity(int userId)
        {
            return new VkApiRequest("messages.getLastActivity", _token)
                .AddParameter("uid", userId.ToString())
                .Select(Parser.ParseActivity);
        }

        public IObservable<bool> SetDialogActivity(int userId)
        {
            return new VkApiRequest("messages.setActivity", _token)
                .AddParameter("uid", userId.ToString())
                .AddParameter("type", "typing")
                .Select(Parser.ParseValue<bool>);
        }

        public IObservable<bool> SetChatActivity(int chatId)
        {
            return new VkApiRequest("messages.setActivity", _token)
                .AddParameter("chat_id", chatId.ToString())
                .AddParameter("type", "typing")
                .Select(Parser.ParseValue<bool>);
        }

        public IObservable<User[]> GetUsers(int[] uids)
        {
            return new VkApiRequest("users.get", _token)
                .AddParameter("uids", string.Join(",", uids))
                .AddParameter("fields", "sex,photo_rec,online,last_seen")
                .Select(Parser.ParseUsers);
        }

        public IObservable<Group[]> GetGroups(int[] gids)
        {
            return new VkApiRequest("groups.getById", _token)
                .AddParameter("gids", string.Join(",", gids))
                .Select(Parser.ParseGroups);
        }

        public IObservable<LongPollHistory> GetLongPollHistory(string timestamp)
        {
            return new VkApiRequest("messages.getLongPollHistory", _token)
                .Select(LongPollHistory.Parse);
        }

        public IObservable<Update> GetLongPollServer()
        {
            return new LongPollServerRequest(_token);
        }

        public IObservable<User[]> GetFriends()
        {
            return new VkApiRequest("friends.get", _token)
                .AddParameter("fields", "sex,photo_rec,online,last_seen")
                .AddParameter("order", "name")
                .Select(Parser.ParseUsers);
        }

        public IObservable<JToken> Execute(string script)
        {
            return new VkApiRequest("execute", _token)
                .AddField("code", Uri.EscapeUriString(script));
        }

        private Message[] ParseMessages(IEnumerable<JToken> tokens)
        {
            return Parser.ParseMessages(tokens, _cache);
        }

        private Chat ParseChat(JToken token)
        {
            return Parser.ParseChat(token, _cache);
        }

        private Photo ParsePhoto(JToken token)
        {
            return Parser.ParsePhoto(token, _cache);
        }
    }
}