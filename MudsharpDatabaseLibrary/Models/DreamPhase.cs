using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class DreamPhase
    {
        public long DreamId { get; set; }
        public int PhaseId { get; set; }
        public string DreamerText { get; set; }
        public string DreamerCommand { get; set; }
        public int WaitSeconds { get; set; }

        public virtual Dream Dream { get; set; }
    }
}
