using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CellOverlay
    {
        public CellOverlay()
        {
            CellOverlaysExits = new HashSet<CellOverlayExit>();
            Cells = new HashSet<Cell>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string CellName { get; set; }
        public string CellDescription { get; set; }
        public long CellOverlayPackageId { get; set; }
        public long CellId { get; set; }
        public int CellOverlayPackageRevisionNumber { get; set; }
        public long TerrainId { get; set; }
        public long? HearingProfileId { get; set; }
        public int OutdoorsType { get; set; }
        public double AmbientLightFactor { get; set; }
        public double AddedLight { get; set; }
        public long? AtmosphereId { get; set; }
        public string AtmosphereType { get; set; }
        public bool SafeQuit { get;set; }

        public virtual Cell Cell { get; set; }
        public virtual CellOverlayPackage CellOverlayPackage { get; set; }
        public virtual HearingProfile HearingProfile { get; set; }
        public virtual Terrain Terrain { get; set; }
        public virtual ICollection<CellOverlayExit> CellOverlaysExits { get; set; }
        public virtual ICollection<Cell> Cells { get; set; }
    }
}
