using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VChat.Mvvm
{
    public class FilteredCollection<T> : ReadOnlyObservableCollection<T> where T : class
    {
        private readonly IList<T> _source;
        private readonly Predicate<T> _filter;

        public FilteredCollection(IList<T> source, Predicate<T> filter)
            : base(new ObservableCollection<T>())
        {
            _source = source;
            _filter = filter;

            Sync();
        }

        public void Sync()
        {
            if (_source.Count == 0)
            {
                if (Items.Count > 0)
                {
                    Items.Clear();
                }
            }
            else
            {
                var innerIndex = 0;

                for (var index = 0; index < _source.Count; index++)
                {
                    var item = _source[index];

                    if (_filter(item))
                    {
                        if (Items.Count == innerIndex || !ReferenceEquals(Items[innerIndex], item))
                        {
                            Items.Insert(innerIndex, item);
                        }

                        innerIndex++;
                    }
                    else
                    {
                        if (Items.Count > innerIndex && ReferenceEquals(Items[innerIndex], item))
                        {
                            Items.RemoveAt(innerIndex);
                        }
                    }
                }
            }
        }
    }
}