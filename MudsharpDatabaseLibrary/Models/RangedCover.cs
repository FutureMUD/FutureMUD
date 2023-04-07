using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RangedCover
    {
        public RangedCover()
        {
            CellsRangedCovers = new HashSet<CellsRangedCovers>();
            TerrainsRangedCovers = new HashSet<TerrainsRangedCovers>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int CoverType { get; set; }
        public int CoverExtent { get; set; }
        public int HighestPositionState { get; set; }
        public string DescriptionString { get; set; }
        public string ActionDescriptionString { get; set; }
        public int MaximumSimultaneousCovers { get; set; }
        public bool CoverStaysWhileMoving { get; set; }

        public virtual ICollection<CellsRangedCovers> CellsRangedCovers { get; set; }
        public virtual ICollection<TerrainsRangedCovers> TerrainsRangedCovers { get; set; }
    }
}
