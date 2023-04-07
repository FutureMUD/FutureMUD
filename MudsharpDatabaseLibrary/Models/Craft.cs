using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Craft
    {
        public Craft()
        {
            CraftInputs = new HashSet<CraftInput>();
            CraftPhases = new HashSet<CraftPhase>();
            CraftProducts = new HashSet<CraftProduct>();
            CraftTools = new HashSet<CraftTool>();
        }

        public long Id { get; set; }
        public int RevisionNumber { get; set; }
        public long EditableItemId { get; set; }
        public string Name { get; set; }
        public string Blurb { get; set; }
        public string ActionDescription { get; set; }
        public string Category { get; set; }
        public bool Interruptable { get; set; }
        public double ToolQualityWeighting { get; set; }
        public double InputQualityWeighting { get; set; }
        public double CheckQualityWeighting { get; set; }
        public int FreeSkillChecks { get; set; }
        public int FailThreshold { get; set; }
        public long? CheckTraitId { get; set; }
        public int CheckDifficulty { get; set; }
        public int FailPhase { get; set; }
        public string QualityFormula { get; set; }
        public long? AppearInCraftsListProgId { get; set; }
        public long? CanUseProgId { get; set; }
        public long? WhyCannotUseProgId { get; set; }
        public long? OnUseProgStartId { get; set; }
        public long? OnUseProgCompleteId { get; set; }
        public long? OnUseProgCancelId { get; set; }
        public string ActiveCraftItemSdesc { get; set; }
        public bool IsPracticalCheck { get; set; }

        public virtual FutureProg AppearInCraftsListProg { get; set; }
        public virtual FutureProg CanUseProg { get; set; }
        public virtual TraitDefinition CheckTrait { get; set; }
        public virtual EditableItem EditableItem { get; set; }
        public virtual FutureProg OnUseProgCancel { get; set; }
        public virtual FutureProg OnUseProgComplete { get; set; }
        public virtual FutureProg OnUseProgStart { get; set; }
        public virtual FutureProg WhyCannotUseProg { get; set; }
        public virtual ICollection<CraftInput> CraftInputs { get; set; }
        public virtual ICollection<CraftPhase> CraftPhases { get; set; }
        public virtual ICollection<CraftProduct> CraftProducts { get; set; }
        public virtual ICollection<CraftTool> CraftTools { get; set; }
    }
}
