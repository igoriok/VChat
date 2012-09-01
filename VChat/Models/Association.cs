using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    [Index(Columns = "UserId")]
    public class Association
    {
        [Column]
        public int UserId { get; set; }

        [Column]
        public int ContactId { get; set; }
    }
}