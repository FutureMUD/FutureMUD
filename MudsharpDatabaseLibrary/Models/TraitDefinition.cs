using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class TraitDefinition
    {
        public TraitDefinition()
        {
            ChargenRolesTraits = new HashSet<ChargenRolesTrait>();
            Crafts = new HashSet<Craft>();
            Languages = new HashSet<Language>();
            RaceButcheryProfilesBreakdownChecks = new HashSet<RaceButcheryProfilesBreakdownChecks>();
            RacesAttributes = new HashSet<RacesAttributes>();
            RangedWeaponTypesFireTrait = new HashSet<RangedWeaponTypes>();
            RangedWeaponTypesOperateTrait = new HashSet<RangedWeaponTypes>();
            ShieldTypes = new HashSet<ShieldType>();
            TraitDefinitionsChargenResources = new HashSet<TraitDefinitionsChargenResources>();
            TraitExpressionParameters = new HashSet<TraitExpressionParameters>();
            Traits = new HashSet<Trait>();
            WeaponTypesAttackTrait = new HashSet<WeaponType>();
            WeaponTypesParryTrait = new HashSet<WeaponType>();
            WearableSizeParameterRule = new HashSet<WearableSizeParameterRule>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public long DecoratorId { get; set; }
        public string TraitGroup { get; set; }
        public int DerivedType { get; set; }
        public long? ExpressionId { get; set; }
        public long? ImproverId { get; set; }
        public bool? Hidden { get; set; }
        public string ChargenBlurb { get; set; }
        public double BranchMultiplier { get; set; }
        public string Alias { get; set; }
        public long? AvailabilityProgId { get; set; }
        public long? TeachableProgId { get; set; }
        public long? LearnableProgId { get; set; }
        public int TeachDifficulty { get; set; }
        public int LearnDifficulty { get; set; }
        public string ValueExpression { get; set; }
        public int DisplayOrder { get; set; }
        public bool DisplayAsSubAttribute { get; set;}
        public bool ShowInScoreCommand { get;set; }
        public bool ShowInAttributeCommand { get;set; }

        public virtual FutureProg AvailabilityProg { get; set; }
        public virtual TraitExpression Expression { get; set; }
        public virtual FutureProg LearnableProg { get; set; }
        public virtual FutureProg TeachableProg { get; set; }
        public virtual ICollection<ChargenRolesTrait> ChargenRolesTraits { get; set; }
        public virtual ICollection<Craft> Crafts { get; set; }
        public virtual ICollection<Language> Languages { get; set; }
        public virtual ICollection<RaceButcheryProfilesBreakdownChecks> RaceButcheryProfilesBreakdownChecks { get; set; }
        public virtual ICollection<RacesAttributes> RacesAttributes { get; set; }
        public virtual ICollection<RangedWeaponTypes> RangedWeaponTypesFireTrait { get; set; }
        public virtual ICollection<RangedWeaponTypes> RangedWeaponTypesOperateTrait { get; set; }
        public virtual ICollection<ShieldType> ShieldTypes { get; set; }
        public virtual ICollection<TraitDefinitionsChargenResources> TraitDefinitionsChargenResources { get; set; }
        public virtual ICollection<TraitExpressionParameters> TraitExpressionParameters { get; set; }
        public virtual ICollection<Trait> Traits { get; set; }
        public virtual ICollection<WeaponType> WeaponTypesAttackTrait { get; set; }
        public virtual ICollection<WeaponType> WeaponTypesParryTrait { get; set; }
        public virtual ICollection<WearableSizeParameterRule> WearableSizeParameterRule { get; set; }
    }
}
