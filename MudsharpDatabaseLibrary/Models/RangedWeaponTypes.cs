using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RangedWeaponTypes
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Classification { get; set; }
        public long FireTraitId { get; set; }
        public long OperateTraitId { get; set; }
        public bool FireableInMelee { get; set; }
        public int DefaultRangeInRooms { get; set; }
        public string AccuracyBonusExpression { get; set; }
        public string DamageBonusExpression { get; set; }
        public int AmmunitionLoadType { get; set; }
        public string SpecificAmmunitionGrade { get; set; }
        public int AmmunitionCapacity { get; set; }
        public int RangedWeaponType { get; set; }
        public double StaminaToFire { get; set; }
        public double StaminaPerLoadStage { get; set; }
        public double CoverBonus { get; set; }
        public int BaseAimDifficulty { get; set; }
        public double LoadDelay { get; set; }
        public double ReadyDelay { get; set; }
        public double FireDelay { get; set; }
        public double AimBonusLostPerShot { get; set; }
        public bool RequiresFreeHandToReady { get; set; }
        public bool AlwaysRequiresTwoHandsToWield { get; set; }

        public virtual TraitDefinition FireTrait { get; set; }
        public virtual TraitDefinition OperateTrait { get; set; }
    }
}
