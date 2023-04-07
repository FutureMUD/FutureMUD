using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class AmmunitionTypes
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SpecificType { get; set; }
        public string RangedWeaponTypes { get; set; }
        public double BaseAccuracy { get; set; }
        public int Loudness { get; set; }
        public double BreakChanceOnHit { get; set; }
        public double BreakChanceOnMiss { get; set; }
        public int BaseBlockDifficulty { get; set; }
        public int BaseDodgeDifficulty { get; set; }
        public string DamageExpression { get; set; }
        public string StunExpression { get; set; }
        public string PainExpression { get; set; }
        public int DamageType { get; set; }
    }
}
