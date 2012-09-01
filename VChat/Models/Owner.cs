namespace VChat.Models
{
    public class Owner
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Group Group { get; set; }
    }
}