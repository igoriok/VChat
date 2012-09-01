using System;

namespace VChat.Services.Vkontakte
{
    public class VkException : Exception
    {
        private readonly int _code;

        public int Code
        {
            get { return _code; }
        }

        public VkException(int code, string message)
            : base(message)
        {
            _code = code;
        }
    }
}