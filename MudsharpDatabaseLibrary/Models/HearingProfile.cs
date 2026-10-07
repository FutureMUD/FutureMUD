using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class HearingProfile
    {
        public HearingProfile()
        {
            RoomOverlays = new HashSet<RoomOverlay>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }
        public string SurveyDescription { get; set; }

        public virtual ICollection<RoomOverlay> RoomOverlays { get; set; }
    }
}
