using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    [InheritanceMapping(Code = null, Type = typeof(WallAttachment), IsDefault = true)]
    [InheritanceMapping(Code = "photo", Type = typeof(MessagePhotoAttachment))]
    [InheritanceMapping(Code = "video", Type = typeof(MessageVideoAttachment))]
    [InheritanceMapping(Code = "audio", Type = typeof(MessageAudioAttachment))]
    [InheritanceMapping(Code = "doc", Type = typeof(MessageDocumentAttachment))]
    [InheritanceMapping(Code = "wall", Type = typeof(MessageWallAttachment))]
    public class WallAttachment
    {
        [Column]
        public int WallId { get; set; }

        [Association(ThisKey = "WallId", OtherKey = "Id")]
        public Wall Wall { get; set; }

        [Column(IsDiscriminator = true)]
        public string Type { get; set; }
    }
}