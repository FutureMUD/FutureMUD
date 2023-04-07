using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class TraitExpression
    {
        public TraitExpression()
        {
            Checks = new HashSet<Check>();
            TraitDefinitions = new HashSet<TraitDefinition>();
            TraitExpressionParameters = new HashSet<TraitExpressionParameters>();
            WeaponAttacksDamageExpression = new HashSet<WeaponAttack>();
            WeaponAttacksPainExpression = new HashSet<WeaponAttack>();
            WeaponAttacksStunExpression = new HashSet<WeaponAttack>();
        }

        public long Id { get; set; }
        public string Expression { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Check> Checks { get; set; }
        public virtual ICollection<TraitDefinition> TraitDefinitions { get; set; }
        public virtual ICollection<TraitExpressionParameters> TraitExpressionParameters { get; set; }
        public virtual ICollection<WeaponAttack> WeaponAttacksDamageExpression { get; set; }
        public virtual ICollection<WeaponAttack> WeaponAttacksPainExpression { get; set; }
        public virtual ICollection<WeaponAttack> WeaponAttacksStunExpression { get; set; }
    }
}
