using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RoomOverlayPackage
    {
        public RoomOverlayPackage()
        {
            RoomOverlays = new HashSet<RoomOverlay>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long EditableItemId { get; set; }
        public int RevisionNumber { get; set; }

        public virtual EditableItem EditableItem { get; set; }
        public virtual ICollection<RoomOverlay> RoomOverlays { get; set; }
    }
}
