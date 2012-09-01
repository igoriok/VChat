using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

using VChat.Models;
using VChat.Services.Cache;

namespace VChat.Services.Vkontakte
{
    public static class Parser
    {
        private static readonly CultureInfo DefaultCulture = new CultureInfo("en-US");

        public static Uri ParseVideoUrl(string content)
        {
            var host = Regex.Match(content, @"var video_host = '([0-9a-z\.\/:]*)'").Groups[1].Value;
            var uid = Regex.Match(content, @"var video_uid = '(\d*)'").Groups[1].Value;
            var vtag = Regex.Match(content, @"var video_vtag = '([\da-zA-Z]*)'").Groups[1].Value;
            var noFlv = Regex.Match(content, @"var video_no_flv = (\d)").Groups[1].Value;

            if (!host.StartsWith("http"))
            {
                host = "http://cs" + host + ".vk.com";
            }

            var ext = "flv";

            if (noFlv == "1")
            {
                //var maxHd = Regex.Match(content, @"var video_max_hd = '(\d)'").Groups[1].Value;
                ext = "240.mp4";
            }

            var url = string.Format("{0}u{1}/video/{2}.{3}", host, uid, vtag, ext);

            return new Uri(url);
        }

        public static JToken ParseReponse(JToken token)
        {
            var response = token["response"];
            if (response == null)
            {
                var error = token["error"];
                if (error == null)
                {
                    throw new VkException(-1, "Response has unknown format");
                }

                var code = error["error_code"].Value<int>();
                var message = error["error_msg"].Value<string>();

                throw new VkException(code, message);
            }

            return response;
        }

        public static T ParseValue<T>(JToken token)
        {
            return token.Value<T>();
        }

        public static T ValueOrDefault<T>(JToken token)
        {
            return token == null ? default(T) : token.Value<T>();
        }

        public static int Count(JToken token)
        {
            return token.Count();
        }

        public static OAuthToken ParseToken(JToken token)
        {
            var error = token["error"];
            if (error == null)
            {
                return token.ToObject<OAuthToken>();
            }

            var description = token["error_description"].Value<string>();

            switch (error.Value<string>())
            {
                case "invalid_client":

                    throw new VkException(5, description);

                case "need_captcha":

                    var sid = token["captcha_sid"].Value<string>();
                    var img = token["captcha_img"].Value<string>();

                    throw new VkCaptchaException(14, sid, img, description);
            }

            throw new VkException(1, description);
        }

        public static DateTime Convert(long secs)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(secs);
        }

        public static Activity ParseActivity(JToken token)
        {
            return new Activity();
        }

        public static Chat ParseChat(JToken token, IUserCache cache)
        {
            var chat = new Chat
            {
                Id = ValueOrDefault<int>(token["chat_id"]),
                Title = ValueOrDefault<string>(token["title"]),
                Admin = cache.GetUser(ValueOrDefault<int>(token["admin_id"]))
            };

            var uids = token["users"].ToObject<int[]>();

            foreach (var uid in uids)
            {
                chat.Users.Add(new ChatUser { User = cache.GetUser(uid) });
            }

            return chat;
        }

        public static Chat ParseMessageChat(JToken token, IUserCache cache)
        {
            var chatId = ValueOrDefault<int?>(token["chat_id"]);
            if (chatId.HasValue)
            {
                var chat = new Chat
                {
                    Id = chatId.Value,
                    Title = ValueOrDefault<string>(token["title"]),
                    Admin = cache.GetUser(ValueOrDefault<int>(token["admin_id"]))
                };

                var active = ValueOrDefault<string>(token["chat_active"]);
                if (!string.IsNullOrEmpty(active))
                {
                    var uids = active.Split(',');
                    foreach (var uid in uids)
                    {
                        int userId;
                        if (int.TryParse(uid, out userId))
                        {
                            chat.Users.Add(new ChatUser
                            {
                                Chat = chat,
                                User = cache.GetUser(userId)
                            });
                        }
                    }
                }

                return chat;
            }

            return null;
        }

        public static User ParseUser(JToken token)
        {
            return new User
            {
                Id = ValueOrDefault<int>(token["uid"]),
                FirstName = ValueOrDefault<string>(token["first_name"]),
                LastName = ValueOrDefault<string>(token["last_name"]),
                Sex = (Sex)ValueOrDefault<int>(token["sex"]),
                Photo = ValueOrDefault<string>(token["photo_rec"]),
                IsOnline = ValueOrDefault<bool>(token["online"]),
                LastSeen = ParseLastSeen(token["last_seen"])
            };
        }

        public static Models.Group ParseGroup(JToken token)
        {
            return new Models.Group
            {
                Id = ValueOrDefault<int>(token["gid"]),
                Name = ValueOrDefault<string>(token["name"]),
                Photo = ValueOrDefault<string>(token["photo"])
            };
        }

        public static Models.Group[] ParseGroups(JToken token)
        {
            return token.Select(ParseGroup).ToArray();
        }

        private static DateTime? ParseLastSeen(JToken token)
        {
            if (token == null)
                return null;

            return Convert(ValueOrDefault<long>(token["time"]));
        }

        public static User[] ParseUsers(JToken token)
        {
            return token.Select(ParseUser).ToArray();
        }

        public static IEnumerable<JToken> ParseContainer(JToken token)
        {
            if (token is JArray && token.HasValues && token[0] is JValue)
            {
                return token.Skip(1);
            }

            throw new ArgumentException("Invalid json format");
        }

        public static Message[] ParseMessages(IEnumerable<JToken> tokens, IUserCache cache)
        {
            if (tokens == null)
                return new Message[0];

            return tokens.Select(token => ParseMessage(token, cache)).ToArray();
        }

        public static Message ParseMessage(JToken token, IUserCache cache)
        {
            var message = new Message
            {
                Id = ValueOrDefault<int>(token["mid"]),
                Timestamp = Convert(ValueOrDefault<long>(token["date"])),
                User = cache.GetUser(ValueOrDefault<int>(token["uid"])),
                IsRead = ValueOrDefault<bool>(token["read_state"]),
                IsOut = ValueOrDefault<bool>(token["out"]),
                Title = ValueOrDefault<string>(token["title"]),
                Body = ValueOrDefault<string>(token["body"]),
                Geo = ParseGeo(token["geo"]),
                Attachments = ParseMessageAttachments(token["attachments"], cache),
                ForwardMessages = ParseForwardMessages(token["fwd_messages"], cache),
                Chat = ParseMessageChat(token, cache),
            };

            return message;
        }

        public static ForwardMessage[] ParseForwardMessages(IEnumerable<JToken> token, IUserCache cache)
        {
            if (token == null)
                return new ForwardMessage[0];

            return token.Select(t => ParseForwardMessage(t, cache)).ToArray();
        }

        public static ForwardMessage ParseForwardMessage(JToken token, IUserCache cache)
        {
            return new ForwardMessage
            {
                User = cache.GetUser(ValueOrDefault<int>(token["uid"])),
                Body = ValueOrDefault<string>(token["body"]),
                Timestamp = Convert(ValueOrDefault<long>(token["date"]))
            };
        }

        public static Photo ParsePhoto(JToken token, IUserCache cache)
        {
            var photo = new Photo
            {
                Id = ValueOrDefault<int>(token["pid"]),
                Owner = cache.GetOwner(ValueOrDefault<int>(token["owner_id"])),
                Source = ValueOrDefault<string>(token["src"]),
                SourceBig = ValueOrDefault<string>(token["src_big"])
            };

            return photo;
        }

        public static Video ParseVideo(JToken token)
        {
            var video = new Video
            {
                Id = ValueOrDefault<int>(token["vid"]),
                OwnerId = ValueOrDefault<int>(token["owner_id"]),
                Title = ValueOrDefault<string>(token["title"]),
                Description = ValueOrDefault<string>(token["description"]),
                Duration = TimeSpan.FromSeconds(ValueOrDefault<long>(token["duration"])),
                Image = ValueOrDefault<string>(token["image"]),
                ImageBig = ValueOrDefault<string>(token["image_big"]),
                ImageSmall = ValueOrDefault<string>(token["image_small"]),
                Views = ValueOrDefault<int>(token["views"]),
                Timestamp = Convert(ValueOrDefault<long>(token["date"]))
            };

            var files = token["files"];
            if (files != null)
            {
                video.Files = ParseVideoFiles(files);
            }

            return video;
        }

        public static VideoFiles ParseVideoFiles(JToken token)
        {
            return new VideoFiles
            {
                Video240 = ValueOrDefault<string>(token["mp4_240"]),
                Video360 = ValueOrDefault<string>(token["mp4_360"]),
                Video480 = ValueOrDefault<string>(token["mp4_480"]),
                Video720 = ValueOrDefault<string>(token["mp4_720"]),
                External = ValueOrDefault<string>(token["external"]),
            };
        }

        public static Video[] ParseVideos(JToken token)
        {
            return token.Select(ParseVideo).ToArray();
        }

        public static Document ParseDocument(JToken token)
        {
            return new Document
            {
                Id = ValueOrDefault<int>(token["did"]),
                OwnerId = ValueOrDefault<int>(token["owner_id"]),
                Title = ValueOrDefault<string>(token["title"]),
                Extension = ValueOrDefault<string>(token["ext"]),
                Size = ValueOrDefault<long>(token["size"]),
                Url = ValueOrDefault<string>(token["url"])
            };
        }

        public static Geo ParseGeo(JToken token)
        {
            if (token == null)
                return null;

            double latitude, longtitude;

            var parts = token["coordinates"].Value<string>().Split(' ');

            if (parts.Length == 2 &&
                double.TryParse(parts[0], NumberStyles.Float, DefaultCulture, out latitude) &&
                double.TryParse(parts[1], NumberStyles.Float, DefaultCulture, out longtitude))
            {
                return new Geo
                {
                    Latitude = latitude,
                    Longtitude = longtitude
                };
            }

            return null;
        }

        public static Wall ParseWall(JToken token, IUserCache cache)
        {
            var attachment = new Wall
            {
                Id = ValueOrDefault<int>(token["id"]),
                From = cache.GetOwner(ValueOrDefault<int>(token["from_id"])),
                To = cache.GetOwner(ValueOrDefault<int>(token["to_id"])),
                Date = Convert(ValueOrDefault<long>(token["date"])),
                Text = ValueOrDefault<string>(token["text"]),
                Geo = ParseGeo(token["geo"]),
                Attachments = ParseWallAttachments(token["attachments"], cache)
            };

            return attachment;
        }

        public static Audio ParseAudio(JToken token)
        {
            return new Audio
            {
                Id = ValueOrDefault<int>(token["aid"]),
                OwnerId = ValueOrDefault<int>(token["owner_id"]),
                Duration = TimeSpan.FromSeconds(ValueOrDefault<long>(token["duration"])),
                Performer = ValueOrDefault<string>(token["performer"]),
                Title = ValueOrDefault<string>(token["title"]),
                Url = ValueOrDefault<string>(token["url"])
            };
        }

        public static WallAttachment[] ParseWallAttachments(IEnumerable<JToken> token, IUserCache cache)
        {
            if (token == null)
                return new WallAttachment[0];

            return token.Select(t => ParseWallAttachment(t, cache)).ToArray();
        }

        public static WallAttachment ParseWallAttachment(JToken token, IUserCache cache)
        {
            var type = ValueOrDefault<string>(token["type"]);

            switch (type)
            {
                case "photo": return new WallPhotoAttachment { Photo = ParsePhoto(token["photo"], cache) };
                case "video": return new WallVideoAttachment { Video = ParseVideo(token["video"]) };
                case "audio": return new WallAudioAttachment { Audio = ParseAudio(token["audio"]) };
                case "doc": return new WallDocumentAttachment { Document = ParseDocument(token["doc"]) };
                case "wall": return new WallWallAttachment { WallWall = ParseWall(token["wall"], cache) };
                default: return new WallAttachment();
            }
        }

        public static MessageAttachment[] ParseMessageAttachments(IEnumerable<JToken> tokens, IUserCache cache)
        {
            if (tokens == null)
                return new MessageAttachment[0];

            return tokens.Select(t => ParseMessageAttachment(t, cache)).ToArray();
        }

        public static MessageAttachment ParseMessageAttachment(JToken token, IUserCache cache)
        {
            var type = ValueOrDefault<string>(token["type"]);

            switch (type)
            {
                case "photo": return new MessagePhotoAttachment { Type = type, Photo = ParsePhoto(token["photo"], cache) };
                case "video": return new MessageVideoAttachment { Type = type, Video = ParseVideo(token["video"]) };
                case "audio": return new MessageAudioAttachment { Type = type, Audio = ParseAudio(token["audio"]) };
                case "doc": return new MessageDocumentAttachment { Type = type, Document = ParseDocument(token["doc"]) };
                case "wall": return new MessageWallAttachment { Type = type, Wall = ParseWall(token["wall"], cache) };
                default: return new MessageAttachment { Type = type };
            }
        }
    }
}