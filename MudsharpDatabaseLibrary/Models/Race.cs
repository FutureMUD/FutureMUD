using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Race
    {
        public Race()
        {
            Bodies = new HashSet<Body>();
            ChargenAdvicesRaces = new HashSet<ChargenAdvicesRaces>();
            Ethnicities = new HashSet<Ethnicity>();
            InverseParentRace = new HashSet<Race>();
            RaceEdibleForagableYields = new HashSet<RaceEdibleForagableYields>();
            RacesAdditionalBodyparts = new HashSet<RacesAdditionalBodyparts>();
            RacesAdditionalCharacteristics = new HashSet<RacesAdditionalCharacteristics>();
            RacesAttributes = new HashSet<RacesAttributes>();
            RacesBreathableGases = new HashSet<RacesBreathableGases>();
            RacesBreathableLiquids = new HashSet<RacesBreathableLiquids>();
            RacesChargenResources = new HashSet<RacesChargenResources>();
            RacesEdibleMaterials = new HashSet<RacesEdibleMaterials>();
            RacesWeaponAttacks = new HashSet<RacesWeaponAttacks>();
            RacesCombatActions = new HashSet<RacesCombatActions>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long BaseBodyId { get; set; }
        public string AllowedGenders { get; set; }
        public long? ParentRaceId { get; set; }
        public long AttributeBonusProgId { get; set; }
        public int AttributeTotalCap { get; set; }
        public int IndividualAttributeCap { get; set; }
        public string DiceExpression { get; set; }
        public double IlluminationPerceptionMultiplier { get; set; }
        public long? AvailabilityProgId { get; set; }
        public long CorpseModelId { get; set; }
        public long DefaultHealthStrategyId { get; set; }
        public bool CanUseWeapons { get; set; }
        public bool CanAttack { get; set; }
        public bool CanDefend { get; set; }
        public long? NaturalArmourTypeId { get; set; }
        public long NaturalArmourQuality { get; set; }
        public long? NaturalArmourMaterialId { get; set; }
        public long? BloodLiquidId { get; set; }
        public bool NeedsToBreathe { get; set; }
        public string BreathingModel {get;set;}
        public long? SweatLiquidId { get; set; }
        public double SweatRateInLitresPerMinute { get; set; }
        public int SizeStanding { get; set; }
        public int SizeProne { get; set; }
        public int SizeSitting { get; set; }
        public string CommunicationStrategyType { get; set; }
        public int DefaultHandedness { get; set; }
        public string HandednessOptions { get; set; }
        public string MaximumDragWeightExpression { get; set; }
        public string MaximumLiftWeightExpression { get; set; }
        public long? RaceButcheryProfileId { get; set; }
        public long? BloodModelId { get; set; }
        public bool RaceUsesStamina { get; set; }
        public bool CanEatCorpses { get; set; }
        public double BiteWeight { get; set; }
        public string EatCorpseEmoteText { get; set; }
        public bool CanEatMaterialsOptIn { get; set; }
        public double TemperatureRangeFloor { get; set; }
        public double TemperatureRangeCeiling { get; set; }
        public int BodypartSizeModifier { get; set; }
        public double BodypartHealthMultiplier { get; set; }
        public string BreathingVolumeExpression { get; set; }
        public string HoldBreathLengthExpression { get; set; }
        public bool CanClimb { get; set; }
        public bool CanSwim { get; set; }
        public int MinimumSleepingPosition { get; set; }
        public int ChildAge { get; set; }
        public int YouthAge { get; set; }
        public int YoungAdultAge { get; set; }
        public int AdultAge { get; set; }
        public int ElderAge { get; set; }
        public int VenerableAge { get; set; }
        public long? DefaultHeightWeightModelMaleId;
        public long? DefaultHeightWeightModelFemaleId;
        public long? DefaultHeightWeightModelNeuterId;
        public long? DefaultHeightWeightModelNonBinaryId;

        public double HungerRate { get; set; }
        public double ThirstRate { get; set; }
        public double TrackIntensityVisual { get; set; }
        public double TrackIntensityOlfactory { get; set; }
        public double TrackingAbilityVisual { get; set; }
        public double TrackingAbilityOlfactory { get; set; }

		public virtual HeightWeightModel DefaultHeightWeightModelMale { get; set; }
        public virtual HeightWeightModel DefaultHeightWeightModelFemale { get; set; }
        public virtual HeightWeightModel DefaultHeightWeightModelNeuter { get; set; }
        public virtual HeightWeightModel DefaultHeightWeightModelNonBinary { get; set; }
        public virtual FutureProg AttributeBonusProg { get; set; }
        public virtual FutureProg AvailabilityProg { get; set; }
        public virtual BodyProto BaseBody { get; set; }
        public virtual Liquid BloodLiquid { get; set; }
        public virtual BloodModel BloodModel { get; set; }
        public virtual CorpseModel CorpseModel { get; set; }
        public virtual HealthStrategy DefaultHealthStrategy { get; set; }
        public virtual Material NaturalArmourMaterial { get; set; }
        public virtual ArmourType NaturalArmourType { get; set; }
        public virtual Race ParentRace { get; set; }
        public virtual RaceButcheryProfile RaceButcheryProfile { get; set; }
        public virtual Liquid SweatLiquid { get; set; }
        public virtual ICollection<Body> Bodies { get; set; }
        public virtual ICollection<ChargenAdvicesRaces> ChargenAdvicesRaces { get; set; }
        public virtual ICollection<Ethnicity> Ethnicities { get; set; }
        public virtual ICollection<Race> InverseParentRace { get; set; }
        public virtual ICollection<RaceEdibleForagableYields> RaceEdibleForagableYields { get; set; }
        public virtual ICollection<RacesAdditionalBodyparts> RacesAdditionalBodyparts { get; set; }
        public virtual ICollection<RacesAdditionalCharacteristics> RacesAdditionalCharacteristics { get; set; }
        public virtual ICollection<RacesAttributes> RacesAttributes { get; set; }
        public virtual ICollection<RacesBreathableGases> RacesBreathableGases { get; set; }
        public virtual ICollection<RacesBreathableLiquids> RacesBreathableLiquids { get; set; }
        public virtual ICollection<RacesChargenResources> RacesChargenResources { get; set; }
        public virtual ICollection<RacesEdibleMaterials> RacesEdibleMaterials { get; set; }
        public virtual ICollection<RacesWeaponAttacks> RacesWeaponAttacks { get; set; }
        public virtual ICollection<RacesCombatActions> RacesCombatActions { get; set; }
    }
}
