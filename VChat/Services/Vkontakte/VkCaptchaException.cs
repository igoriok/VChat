namespace VChat.Services.Vkontakte
{
    public class VkCaptchaException : VkException
    {
        private readonly string _sid;
        private readonly string _image;

        public string Sid
        {
            get { return _sid; }
        }

        public string Image
        {
            get { return _image; }
        }

        public VkCaptchaException(int code, string sid, string image, string message)
            : base(code, message)
        {
            _sid = sid;
            _image = image;
        }
    }
}