using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RoomOverlay
    {
        public RoomOverlay()
        {
            RoomOverlaysExits = new HashSet<RoomOverlayExit>();
            Rooms = new HashSet<Room>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string RoomName { get; set; }
        public string RoomDescription { get; set; }
        public long RoomOverlayPackageId { get; set; }
        public long RoomId { get; set; }
        public int RoomOverlayPackageRevisionNumber { get; set; }
        public long TerrainId { get; set; }
        public long? HearingProfileId { get; set; }
        public int OutdoorsType { get; set; }
        public double AmbientLightFactor { get; set; }
        public double AddedLight { get; set; }
        public long? AtmosphereId { get; set; }
        public string AtmosphereType { get; set; }
        public bool SafeQuit { get; set; }

        public virtual Room Room { get; set; }
        public virtual RoomOverlayPackage RoomOverlayPackage { get; set; }
        public virtual HearingProfile HearingProfile { get; set; }
        public virtual Terrain Terrain { get; set; }
        public virtual ICollection<RoomOverlayExit> RoomOverlaysExits { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
