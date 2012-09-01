using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace VChat.Models
{
    [Table]
    public class Wall
    {
        private readonly EntitySet<WallAttachment> _attachments;

        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column]
        public int FromId { get; set; }

        [Association(ThisKey = "FromId", OtherKey = "Id")]
        public Owner From { get; set; }

        [Column]
        public int ToId { get; set; }

        [Association(ThisKey = "ToId", OtherKey = "Id")]
        public Owner To { get; set; }

        [Column]
        public DateTime Date { get; set; }

        [Column]
        public string Text { get; set; }

        [Column]
        public int GeoId { get; set; }

        [Association]
        public Geo Geo { get; set; }

        [Association(ThisKey = "Id", OtherKey = "WallId")]
        public IList<WallAttachment> Attachments
        {
            get { return _attachments; }
            set { _attachments.Assign(value); }
        }

        [Column]
        public int PostSourceId { get; set; }

        public PostSource PostSource { get; set; }

        public string SignerId { get; set; }

        public string CopyOwnerId { get; set; }

        public string CopyPostId { get; set; }

        public string CopyText { get; set; }

        public Wall()
        {
            _attachments = new EntitySet<WallAttachment>();
        }
    }
}