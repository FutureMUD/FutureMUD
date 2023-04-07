using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class SurgicalProcedurePhase
    {
        public long SurgicalProcedureId { get; set; }
        public int PhaseNumber { get; set; }
        public double BaseLengthInSeconds { get; set; }
        public string PhaseEmote { get; set; }
        public string PhaseSpecialEffects { get; set; }
        public long? OnPhaseProgId { get; set; }
        public string InventoryActionPlan { get; set; }

        public virtual FutureProg OnPhaseProg { get; set; }
        public virtual SurgicalProcedure SurgicalProcedure { get; set; }
    }
}
