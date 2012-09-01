namespace VChat.Mvvm
{
    public interface IBusyState
    {
        bool IsBusy { get; set; }
        string Status { get; set; }
    }
}