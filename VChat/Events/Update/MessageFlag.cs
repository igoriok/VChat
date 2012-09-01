using System;

namespace VChat.Events.Update
{
    [Flags]
    public enum MessageFlag
    {
        None = 0,
        Unread = 1,
        Outbox = 2,
        Replied = 4,
        Important = 8,
        Chat = 16,
        Friends = 32,
        Spam = 64,
        Deleted = 128,
        Fixed = 256,
        Media = 512
    }
}