using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class HearingProfile
    {
        public HearingProfile()
        {
            CellOverlays = new HashSet<CellOverlay>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }
        public string SurveyDescription { get; set; }

        public virtual ICollection<CellOverlay> CellOverlays { get; set; }
    }
}
