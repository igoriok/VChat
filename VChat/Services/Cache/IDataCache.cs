using VChat.Models;

namespace VChat.Services.Cache
{
    public interface IDataCache
    {
        Message[] GetMessages();
        Message[] UpdateMessages(Message[] messages);
    }
}