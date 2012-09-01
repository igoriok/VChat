using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;

namespace VChat.Mvvm
{
    public static class Extensions
    {
        public static IObservable<T> Queue<T>(this IObservable<Unit> source, IObservable<T> observable)
        {
            return Observable.CreateWithDisposable<T>(observer =>
            {
                var @event = new AutoResetEvent(false);

                var composite = new CompositeDisposable(
                    Repeat(observable, @event)
                        .Concat()
                        .SubscribeOn(Scheduler.ThreadPool)
                        .Subscribe(observer),
                    source.Subscribe(
                        _ => @event.Set(),
                        e =>
                        {
                            @event.Close();
                            observer.OnError(e);
                        },
                        () =>
                        {
                            @event.Close();
                            observer.OnCompleted();
                        }),
                    @event);

                return composite;
            });
        }

        private static IEnumerable<IObservable<T>> Repeat<T>(IObservable<T> source, AutoResetEvent @event)
        {
            while (@event.WaitOne())
            {
                yield return source;
            }
        }

        public static IObservable<string> ObservablePropertyChanged(this INotifyPropertyChanged notify)
        {
            return Observable
                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => (s, e) => handler(s, e),
                    handler => notify.PropertyChanged += handler,
                    handler => notify.PropertyChanged -= handler)
                .Select(e => e.EventArgs.PropertyName);
        }

        public static IObservable<string> ObservablePropertyChanged(this INotifyPropertyChanged notify, string propertyName)
        {
            return notify.ObservablePropertyChanged() .Where(p => p == propertyName);
        }

        public static IObservable<T> Retry<T>(this IObservable<T> source, TimeSpan delay)
        {
            return RepeatWithDelay(source, delay).Catch();
        }

        public static IObservable<T> Repeat<T>(this IObservable<T> source, TimeSpan delay)
        {
            return RepeatWithDelay(source, delay).Concat();
        }

        private static IEnumerable<IObservable<T>> RepeatWithDelay<T>(IObservable<T> source, TimeSpan delay)
        {
            yield return source;

            while (true)
            {
                yield return source.Delay(delay);
            }
        }

        public static IObservable<T> FromEvent<T>(this IEventAggregator eventAggregator)
        {
            return Observable.CreateWithDisposable<T>(observer =>
            {
                var subscription = new Handler<T>(observer);

                eventAggregator.Subscribe(subscription);

                return Disposable.Create(() => eventAggregator.Unsubscribe(subscription));
            });
        }

        private class Handler<T> : IHandle<T>
        {
            private readonly IObserver<T> _observer;

            public Handler(IObserver<T> observer)
            {
                _observer = observer;
            }

            #region IHandle<T>

            public void Handle(T message)
            {
                _observer.OnNext(message);
            }

            #endregion
        }

        public static void StartBusy(this IBusyState state, string status = null)
        {
            state.IsBusy = true;
            state.Status = status;
        }

        public static void StopBusy(this IBusyState state)
        {
            state.IsBusy = false;
            state.Status = null;
        }

        public static void Syncronize<T>(this IList<T> source, IList<T> list) where T : class
        {
            if (list.Count == 0)
            {
                if (source.Count > 0)
                {
                    source.Clear();
                }
            }
            else
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var item = list[index];

                    if (source.Count == index)
                    {
                        source.Add(item);
                    }
                    else if (!ReferenceEquals(source[index], item))
                    {
                        source.RemoveAt(index);
                    }
                }

                while (source.Count > list.Count)
                {
                    source.RemoveAt(list.Count);
                }
            }
        }

        public static void ClearHistory(this INavigationService navigation)
        {
            while (navigation.RemoveBackEntry() != null) { }
        }
    }
}