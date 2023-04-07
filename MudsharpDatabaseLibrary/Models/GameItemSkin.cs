using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MudSharp.Models
{
    public class GameItemSkin
    {
        public long Id { get; set; }
        public int RevisionNumber { get; set; }
        public long EditableItemId { get; set; }
        public string Name { get; set; }
        public long ItemProtoId { get; set; }
        [CanBeNull] public string ItemName { get; set; }
        [CanBeNull] public string ShortDescription { get; set; }
        [CanBeNull] public string FullDescription { get; set; }
        [CanBeNull] public string LongDescription { get; set; }
        public int? Quality { get; set; }
        public bool IsPublic { get; set; }
        public long? CanUseSkinProgId { get; set; }

        public virtual EditableItem EditableItem { get; set; }
    }
}
