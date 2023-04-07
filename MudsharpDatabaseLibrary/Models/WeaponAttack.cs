using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class WeaponAttack
    {
        public WeaponAttack()
        {
            CombatMessagesWeaponAttacks = new HashSet<CombatMessagesWeaponAttacks>();
            RacesWeaponAttacks = new HashSet<RacesWeaponAttacks>();
        }

        public long Id { get; set; }
        public long? WeaponTypeId { get; set; }
        public int Verb { get; set; }
        public long? FutureProgId { get; set; }
        public int BaseAttackerDifficulty { get; set; }
        public int BaseBlockDifficulty { get; set; }
        public int BaseDodgeDifficulty { get; set; }
        public int BaseParryDifficulty { get; set; }
        public double BaseAngleOfIncidence { get; set; }
        public int RecoveryDifficultySuccess { get; set; }
        public int RecoveryDifficultyFailure { get; set; }
        public int MoveType { get; set; }
        public long Intentions { get; set; }
        public int ExertionLevel { get; set; }
        public int DamageType { get; set; }
        public long DamageExpressionId { get; set; }
        public long StunExpressionId { get; set; }
        public long PainExpressionId { get; set; }
        public double Weighting { get; set; }
        public long? BodypartShapeId { get; set; }
        public double StaminaCost { get; set; }
        public double BaseDelay { get; set; }
        public string Name { get; set; }
        public int Orientation { get; set; }
        public int Alignment { get; set; }
        public string AdditionalInfo { get; set; }
        public int HandednessOptions { get; set; }
        public string RequiredPositionStateIds { get; set; }

        public virtual TraitExpression DamageExpression { get; set; }
        public virtual FutureProg FutureProg { get; set; }
        public virtual TraitExpression PainExpression { get; set; }
        public virtual TraitExpression StunExpression { get; set; }
        public virtual WeaponType WeaponType { get; set; }
        public virtual ICollection<CombatMessagesWeaponAttacks> CombatMessagesWeaponAttacks { get; set; }
        public virtual ICollection<RacesWeaponAttacks> RacesWeaponAttacks { get; set; }
    }
}
