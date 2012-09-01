using System;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.UserData;
using VChat.Services.Vkontakte.Requests;

namespace VChat.Services.Cache
{
    public class PhotoCache : IPhotoCache
    {
        private const string ContactsPath = @"Cache\Photo\Contacts";
        private const string UsersPath = @"Cache\Photo\Users";

        #region IPhotoCache

        public Stream GetContactPhoto(Contact contact)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var filePath = Path.Combine(ContactsPath, contact.GetHashCode() + ".jpg");

                if (store.FileExists(filePath))
                {
                    return GetStream(store, filePath);
                }

                return null;
            }
        }

        public Stream SaveContactPhoto(Contact contact)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var filePath = Path.Combine(ContactsPath, contact.GetHashCode() + ".jpg");

                EnsurePath(store, ContactsPath);

                using (var picture = contact.GetPicture())
                {
                    if (picture != null)
                    {
                        using (var fileStream = store.OpenFile(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            picture.CopyTo(fileStream);
                        }

                        picture.Position = 0;

                        return GetStream(picture);
                    }
                }

                return null;
            }
        }

        public Stream GetUserPhoto(int id)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var files = store.GetFileNames(string.Format(@"{0}\{1}.", UsersPath, id));

                if (files.Length > 0)
                {
                    return GetStream(store, files[0]);
                }

                return null;
            }
        }

        public IObservable<Stream> SaveUserPhoto(int id, string uri)
        {
            return new GenericRequest<Stream>(uri, stream =>
            {
                var fileExt = Path.GetExtension(uri);
                var filePath = Path.Combine(UsersPath, id.ToString() + fileExt);

                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    EnsurePath(store, UsersPath);

                    using (var fileStream = store.OpenFile(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }

                    return GetStream(store, filePath);
                }
            });
        }

        private void EnsurePath(IsolatedStorageFile store, string path)
        {
            if (!store.DirectoryExists(path))
            {
                store.CreateDirectory(path);
            }
        }

        private Stream GetStream(Stream stream)
        {
            var memory = new MemoryStream();
            stream.CopyTo(memory);
            return memory;
        }

        private Stream GetStream(IsolatedStorageFile store, string filePath)
        {
            using (var file = store.OpenFile(filePath, FileMode.Open, FileAccess.Read))
            {
                var stream = new MemoryStream();

                file.CopyTo(stream);

                return stream;
            }
        }

        #endregion
    }
}