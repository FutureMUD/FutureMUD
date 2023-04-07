using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ShieldType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long BlockTraitId { get; set; }
        public double BlockBonus { get; set; }
        public double StaminaPerBlock { get; set; }
        public long? EffectiveArmourTypeId { get; set; }

        public virtual TraitDefinition BlockTrait { get; set; }
        public virtual ArmourType EffectiveArmourType { get; set; }
    }
}
