namespace VChat.Models
{
    public class VideoFiles
    {
        public int VideoId { get; set; }

        public Video Video { get; set; }

        public string Video240 { get; set; }

        public string Video360 { get; set; }

        public string Video480 { get; set; }

        public string Video720 { get; set; }

        public string External { get; set; }
    }
}