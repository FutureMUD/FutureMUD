using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class WeaponType
    {
        public WeaponType()
        {
            WeaponAttacks = new HashSet<WeaponAttack>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int Classification { get; set; }
        public long? AttackTraitId { get; set; }
        public long? ParryTraitId { get; set; }
        public double ParryBonus { get; set; }
        public int Reach { get; set; }
        public double StaminaPerParry { get; set; }

        public virtual TraitDefinition AttackTrait { get; set; }
        public virtual TraitDefinition ParryTrait { get; set; }
        public virtual ICollection<WeaponAttack> WeaponAttacks { get; set; }
    }
}
