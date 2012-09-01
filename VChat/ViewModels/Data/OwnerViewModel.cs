using Caliburn.Micro;
using NotifyPropertyWeaver;
using VChat.Models;

namespace VChat.ViewModels.Data
{
    public class OwnerViewModel : PropertyChangedBase
    {
        [NotifyProperty]
        public int Id { get; set; }

        [NotifyProperty]
        public string Name { get; set; }

        [NotifyProperty]
        public string Photo { get; set; }

        public static OwnerViewModel Map(Owner owner)
        {
            var viewModel = new OwnerViewModel
            {
                Id = owner.Id
            };

            if (owner.User != null)
            {
                viewModel.Name = string.Format("{0} {1}", owner.User.FirstName, owner.User.LastName);
                viewModel.Photo = owner.User.Photo;
            }
            else if (owner.Group != null)
            {
                viewModel.Name = owner.Group.Name;
                viewModel.Photo = owner.Group.Photo;
            }

            return viewModel;
        }
    }
}