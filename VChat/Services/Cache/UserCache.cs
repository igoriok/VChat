using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Reactive;
using VChat.Models;
using VChat.Services.Vkontakte;

namespace VChat.Services.Cache
{
    public class UserCache : IUserCache
    {
        private readonly IVkClient _client;

        private readonly Dictionary<int, User> _users = new Dictionary<int, User>();
        private readonly Dictionary<int, Group> _groups = new Dictionary<int, Group>();

        public UserCache(IVkClient client)
        {
            _client = client;
        }

        #region IUserCache

        public User GetUser(int id)
        {
            User user;

            if (!_users.TryGetValue(id, out user))
            {
                user = new User { Id = id };
                _users.Add(id, user);
            }

            return user;
        }

        public Group GetGroup(int id)
        {
            Group group;

            if (!_groups.TryGetValue(id, out group))
            {
                group = new Group { Id = id };
                _groups.Add(id, group);
            }

            return group;
        }

        public Owner GetOwner(int id)
        {
            var owner = new Owner { Id = id };

            if (id > 0)
            {
                owner.User = GetUser(id);
            }
            else
            {
                owner.Group = GetGroup(-id);
            }

            return owner;
        }

        public IObservable<T> Flush<T>(T result)
        {
            if (_users.Count > 0)
            {
                if (_groups.Count > 0)
                {
                    return FlusGroups().SelectMany(FlusGroups()).SelectMany(Observable.Return(result));
                }

                return FlushUsers().SelectMany(Observable.Return(result));
            }

            return Observable.Return(result);
        }

        private IObservable<User[]> FlushUsers()
        {
            if (_users.Count > 0)
            {
                return _client
                    .GetUsers(_users.Keys.ToArray())
                    .Do(UpdateUsers);
            }

            return Observable.Empty<User[]>();
        }

        private IObservable<Group[]> FlusGroups()
        {
            if (_groups.Count > 0)
            {
                return _client
                    .GetGroups(_groups.Keys.ToArray())
                    .Do(UpdateGroups);
            }

            return Observable.Empty<Group[]>();
        }

        #endregion

        private void UpdateUsers(User[] users)
        {
            foreach (var user in users)
            {
                User to;
                if (_users.TryGetValue(user.Id, out  to))
                {
                    to.FirstName = user.FirstName;
                    to.LastName = user.LastName;
                    to.Sex = user.Sex;
                    to.IsOnline = user.IsOnline;
                    to.Photo = user.Photo;
                    to.LastSeen = user.LastSeen;
                }
            }
        }

        private void UpdateGroups(Group[] groups)
        {
            foreach (var group in groups)
            {
                Group to;
                if (_groups.TryGetValue(group.Id, out to))
                {
                    to.Name = group.Name;
                    to.Photo = group.Photo;
                }
            }
        }
    }
}