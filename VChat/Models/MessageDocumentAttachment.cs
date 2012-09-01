using System.Data.Linq.Mapping;

namespace VChat.Models
{
    public class MessageDocumentAttachment : MessageAttachment
    {
        [Column]
        public int DocumentId { get; set; }

        [Association]
        public Document Document { get; set; }
    }
}