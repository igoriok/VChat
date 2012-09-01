using System;
using System.ComponentModel;
using Microsoft.Phone.Reactive;

namespace VChat.ViewModels
{
    public static class Extensions
    {
        public static IObservable<PropertyChangedEventArgs> FromPropertyChangedEvent(this INotifyPropertyChanged source)
        {
            return Observable
                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => (s, e) => handler(s, e),
                    handler => source.PropertyChanged += handler,
                    handler => source.PropertyChanged -= handler)
                .Select(e => e.EventArgs);
        }
    }
}