namespace VChat.Events.Update
{
    public class UserCalled : Update
    {
        public int UserId { get; set; }
        public int CallId { get; set; }
    }
}