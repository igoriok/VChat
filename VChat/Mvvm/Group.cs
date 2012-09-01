using System.Collections;
using System.Collections.Generic;

namespace VChat.Mvvm
{
    public class Group<TKey, TItem> : IEnumerable<TItem>
    {
        private readonly TKey _key;
        private readonly IList<TItem> _items;

        public TKey Key
        {
            get { return _key; }
        }

        public Group(TKey key, IList<TItem> items)
        {
            _key = key;
            _items = items;
        }

        #region IEnumerable

        public IEnumerator<TItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}