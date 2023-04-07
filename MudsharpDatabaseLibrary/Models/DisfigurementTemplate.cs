using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class DisfigurementTemplate
    {
        public long Id { get; set; }
        public int RevisionNumber { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long EditableItemId { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string Definition { get; set; }

        public virtual EditableItem EditableItem { get; set; }
    }
}
