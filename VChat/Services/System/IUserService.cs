namespace VChat.Services.System
{
    public interface IUserService : ISystemService
    {
        bool IsEnabled { get; }

        void Enable(bool start);
        void Disable(bool stop);
    }
}