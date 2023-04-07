using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class SurgicalProcedure
    {
        public SurgicalProcedure()
        {
            SurgicalProcedurePhases = new HashSet<SurgicalProcedurePhase>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string ProcedureName { get; set; }
        public int Procedure { get; set; }
        public double BaseCheckBonus { get; set; }
        public int Check { get; set; }
        public string MedicalSchool { get; set; }
        public long? KnowledgeRequiredId { get; set; }
        public long? UsabilityProgId { get; set; }
        public long? WhyCannotUseProgId { get; set; }
        public long? CompletionProgId { get; set; }
        public long? AbortProgId { get; set; }
        public string ProcedureBeginEmote { get; set; }
        public string ProcedureDescriptionEmote { get; set; }
        public string ProcedureGerund { get; set; }
        public string Definition { get; set; }
        public long? CheckTraitDefinitionId { get; set; }
        public long TargetBodyTypeId { get; set; }

        public virtual FutureProg AbortProg { get; set; }
        public virtual FutureProg CompletionProg { get; set; }
        public virtual Knowledge KnowledgeRequired { get; set; }
        public virtual FutureProg UsabilityProg { get; set; }
        public virtual FutureProg WhyCannotUseProg { get; set; }
        public virtual TraitDefinition CheckTraitDefinition { get; set; }
        public virtual BodyProto TargetBodyType { get; set; }
        public virtual ICollection<SurgicalProcedurePhase> SurgicalProcedurePhases { get; set; }
    }
}
