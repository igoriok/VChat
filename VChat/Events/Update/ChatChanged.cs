namespace VChat.Events.Update
{
    public class ChatChanged : Update
    {
        public int ChatId { get; set; }
        public bool IsSelf { get; set; }
    }
}