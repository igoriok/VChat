using System.Windows;
using Caliburn.Micro;

namespace VChat.Services.System
{
    public class ApplicationService : IApplicationService
    {
        #region IApplicationService

        public void StartService(ApplicationServiceContext context)
        {
            IoC.Get<IUserService>("UserService").Start();
        }

        public void StopService()
        {
            IoC.Get<IUserService>("UserService").Stop();
        }

        #endregion
    }
}