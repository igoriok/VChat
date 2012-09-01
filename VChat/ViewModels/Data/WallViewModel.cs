using System;
using System.Linq;
using NotifyPropertyWeaver;
using VChat.Models;

namespace VChat.ViewModels.Data
{
    public class WallViewModel : AttachmentViewModel
    {
        [NotifyProperty]
        public int Id { get; set; }

        [NotifyProperty]
        public OwnerViewModel From { get; set; }

        [NotifyProperty]
        public OwnerViewModel To { get; set; }

        [NotifyProperty]
        public DateTime Timestamp { get; set; }

        [NotifyProperty]
        public string Text { get; set; }

        [NotifyProperty]
        public GeoViewModel Geo { get; set; }

        [NotifyProperty]
        public AttachmentViewModel[] Attachments { get; set; }

        public static WallViewModel Map(Wall wall)
        {
            var viewModel = new WallViewModel();
            Map(wall, viewModel);
            return viewModel;
        }

        public static void Map(Wall wall, WallViewModel viewModel)
        {
            viewModel.Id = wall.Id;
            viewModel.From = OwnerViewModel.Map(wall.From);
            viewModel.To = OwnerViewModel.Map(wall.To);
            viewModel.Timestamp = wall.Date;
            viewModel.Text = wall.Text;

            if (wall.Geo != null)
            {
                viewModel.Geo = GeoViewModel.Map(wall.Geo);
            }

            if (wall.Attachments.Count > 0)
            {
                viewModel.Attachments = wall.Attachments.Select(Map).ToArray();
            }
        }
    }
}