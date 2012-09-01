using Caliburn.Micro;
using VChat.ViewModels;

namespace VChat.Storages
{
    public class MainViewModelStorage : StorageHandler<MainViewModel>
    {
        #region StorageHandler<MainViewModel>

        public override void Configure()
        {
            this.ActiveItemIndex().InPhoneState().RestoreAfterActivation();
        }

        #endregion
    }
}