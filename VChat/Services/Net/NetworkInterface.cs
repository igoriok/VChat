using System;
using System.Net.NetworkInformation;
using Microsoft.Phone.Reactive;

using SystemNetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace VChat.Services.Net
{
    public class NetworkInterface : INetworkInterface
    {
        private readonly IObservable<bool> _observable;

        public NetworkInterface()
        {
            _observable = Observable
                .FromEvent<NetworkAddressChangedEventHandler, EventArgs>(
                    handler => (s, e) => handler(s, e),
                    handler => NetworkChange.NetworkAddressChanged += handler,
                    handler => NetworkChange.NetworkAddressChanged -= handler)
                .Select(e => SystemNetworkInterface.GetIsNetworkAvailable());
        }

        public IObservable<bool> IsNetworkAvailable
        {
            get
            {
                return _observable
                    .Publish().RefCount()
                    .Merge(Observable.Start<bool>(SystemNetworkInterface.GetIsNetworkAvailable))
                    .DistinctUntilChanged();
            }
        }
    }
}