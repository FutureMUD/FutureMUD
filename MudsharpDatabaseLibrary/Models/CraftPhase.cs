using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CraftPhase
    {
        public long CraftPhaseId { get; set; }
        public int CraftPhaseRevisionNumber { get; set; }
        public int PhaseNumber { get; set; }
        public double PhaseLengthInSeconds { get; set; }
        public string Echo { get; set; }
        public string FailEcho { get; set; }
        public int ExertionLevel { get; set; }
        public double StaminaUsage { get; set; }

        public virtual Craft Craft { get; set; }
    }
}
