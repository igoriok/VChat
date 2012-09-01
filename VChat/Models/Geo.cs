using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    public class Geo
    {
        [Column]
        public int MessageId { get; set; }

        [Association(ThisKey = "MessageId", OtherKey = "Id")]
        public Message Message { get; set; }

        [Column]
        public double Latitude { get; set; }

        [Column]
        public double Longtitude { get; set; }
    }
}