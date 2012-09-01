using System.Device.Location;
using Caliburn.Micro;
using NotifyPropertyWeaver;
using VChat.Models;

namespace VChat.ViewModels.Data
{
    public class GeoViewModel : PropertyChangedBase
    {
        [NotifyProperty]
        public GeoCoordinate Coordinate { get; set; }

        [NotifyProperty]
        public string Image { get; set; }

        public static GeoViewModel Map(Geo entity)
        {
            return new GeoViewModel
            {
                Coordinate = new GeoCoordinate(entity.Latitude, entity.Longtitude)
            };
        }
    }
}