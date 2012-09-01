using System;
using Microsoft.Phone.Reactive;

namespace VChat.ViewModels
{
    public static class ViewModelExtensions
    {
         public static IObservable<T> Attach<T>(this IObservable<T> source, IndicatorViewModel indicator, string text)
         {
             indicator.Begin(text);

             return source.Finally(indicator.End);
         }
    }
}