using System;
using System.IO;
using Newtonsoft.Json.Linq;
using VChat.Events.Update;
using VChat.Models;
using VChat.Services.Vkontakte.Data;

namespace VChat.Services.Vkontakte
{
    public interface IVkClient
    {
        IObservable<bool> CheckPhone(string phone);

        IObservable<string> Signup(string phone, string firstName, string lastName);
        IObservable<int> Confirm(string phone, string code, string password);

        IObservable<OAuthToken> Authenticate(string username, string password);

        IObservable<bool> SetOnline();

        IObservable<bool> RegisterDevice(string token);
        IObservable<bool> UnregisterDevice(string token);

        IObservable<Message[]> GetMessages(int count, int offset);
        IObservable<Message> GetMessage(int messageId);
        IObservable<Chat> GetChat(int chatId);

        IObservable<Photo> UploadMessagePhoto(string name, Stream stream);

        IObservable<int> SendDialogMessage(int userId, string message);
        IObservable<int> SendChatMessage(int chatId, string message);

        IObservable<bool> DeleteDialog(int userId);
        IObservable<bool> DeleteChat(int chatId);
        IObservable<bool> DeleteMessage(int messageId);

        IObservable<Message[]> GetDialogHistory(int userId, int count, int? fromId = null);
        IObservable<Message[]> GetChatHistory(int chatId, int count, int? fromId = null);

        IObservable<Activity> GetLastActivity(int userId);

        IObservable<bool> SetDialogActivity(int userId);
        IObservable<bool> SetChatActivity(int chatId);

        IObservable<LongPollHistory> GetLongPollHistory(string timestamp);
        IObservable<Update> GetLongPollServer();

        IObservable<User[]> GetUsers(int[] uids);
        IObservable<Group[]> GetGroups(int[] gids);
        IObservable<User[]> GetFriends();

        IObservable<int> GetUnreadMessagesCount();
        IObservable<int[]> GetFriendsRequests(int count);
        IObservable<User[]> GetFriendsSuggestions(int count);

        IObservable<bool> AddFriend(int uid);
        IObservable<bool> DeleteFriend(int uid);

        IObservable<Video> GetVideo(int ownerId, int videoId);

        IObservable<JToken> Execute(string script);
    }
}