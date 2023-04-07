using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Exit
    {
        public Exit()
        {
            CellOverlaysExits = new HashSet<CellOverlayExit>();
        }

        public long Id { get; set; }
        public string Keywords1 { get; set; }
        public string Keywords2 { get; set; }
        public long CellId1 { get; set; }
        public long CellId2 { get; set; }
        public long? DoorId { get; set; }
        public int Direction1 { get; set; }
        public int Direction2 { get; set; }
        public double TimeMultiplier { get; set; }
        public string InboundDescription1 { get; set; }
        public string InboundDescription2 { get; set; }
        public string OutboundDescription1 { get; set; }
        public string OutboundDescription2 { get; set; }
        public string InboundTarget1 { get; set; }
        public string InboundTarget2 { get; set; }
        public string OutboundTarget1 { get; set; }
        public string OutboundTarget2 { get; set; }
        public string Verb1 { get; set; }
        public string Verb2 { get; set; }
        public string PrimaryKeyword1 { get; set; }
        public string PrimaryKeyword2 { get; set; }
        public bool AcceptsDoor { get; set; }
        public int? DoorSize { get; set; }
        public int MaximumSizeToEnter { get; set; }
        public int MaximumSizeToEnterUpright { get; set; }
        public long? FallCell { get; set; }
        public bool IsClimbExit { get; set; }
        public int ClimbDifficulty { get; set; }
        public string BlockedLayers { get; set; }

        public virtual ICollection<CellOverlayExit> CellOverlaysExits { get; set; }
    }
}
