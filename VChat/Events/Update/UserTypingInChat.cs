namespace VChat.Events.Update
{
    public class UserTypingInChat : Update
    {
        public int UserId { get; set; }
        public int ChatId { get; set; }
    }
}