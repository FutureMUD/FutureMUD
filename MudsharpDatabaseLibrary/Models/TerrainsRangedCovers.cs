using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class TerrainsRangedCovers
    {
        public long TerrainId { get; set; }
        public long RangedCoverId { get; set; }

        public virtual RangedCover RangedCover { get; set; }
        public virtual Terrain Terrain { get; set; }
    }
}
