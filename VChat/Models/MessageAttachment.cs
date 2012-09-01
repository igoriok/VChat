using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    [InheritanceMapping(Code = null, Type = typeof(MessageAttachment), IsDefault = true)]
    [InheritanceMapping(Code = "photo", Type = typeof(MessagePhotoAttachment))]
    [InheritanceMapping(Code = "video", Type = typeof(MessageVideoAttachment))]
    [InheritanceMapping(Code = "audio", Type = typeof(MessageAudioAttachment))]
    [InheritanceMapping(Code = "doc", Type = typeof(MessageDocumentAttachment))]
    [InheritanceMapping(Code = "wall", Type = typeof(MessageWallAttachment))]
    public class MessageAttachment
    {
        [Column]
        public int MessageId { get; set; }

        [Association(ThisKey = "MessageId", OtherKey = "Id")]
        public Message Message { get; set; }

        [Column(IsDiscriminator = true)]
        public string Type { get; set; }
    }
}