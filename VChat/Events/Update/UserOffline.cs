namespace VChat.Events.Update
{
    public class UserOffline : Update
    {
        public int UserId { get; set; }
        public bool IsTimeout { get; set; }
    }
}