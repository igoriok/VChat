using System;

namespace VChat.Services.Net
{
    public interface INetworkInterface
    {
        IObservable<bool> IsNetworkAvailable { get; }
    }
}