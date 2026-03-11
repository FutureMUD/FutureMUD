using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.Health.Wounds;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Health.Strategies;

public class ComplexLivingHealthStrategy : BaseHealthStrategy
{
	private static readonly TraitExpressionBuilderField<ComplexLivingHealthStrategy>[] TraitExpressionFields =
	[
		new("MaximumHitPointsExpression", ["maxhp", "maximumhitpointsexpression"], "Maximum Hit Points Expression",
			x => x.MaximumHitPointsExpression, (x, value) => x.MaximumHitPointsExpression = value),
		new("MaximumStunExpression", ["maxstun", "maximumstunexpression"], "Maximum Stun Expression",
			x => x.MaximumStunExpression, (x, value) => x.MaximumStunExpression = value),
		new("MaximumPainExpression", ["maxpain", "maximumpainexpression"], "Maximum Pain Expression",
			x => x.MaximumPainExpression, (x, value) => x.MaximumPainExpression = value),
		new("HealingTickDamageExpression", ["healdamage", "healingtickdamageexpression"], "Healing Tick Damage Expression",
			x => x.HealingTickDamageExpression, (x, value) => x.HealingTickDamageExpression = value),
		new("HealingTickStunExpression", ["healstun", "healingtickstunexpression"], "Healing Tick Stun Expression",
			x => x.HealingTickStunExpression, (x, value) => x.HealingTickStunExpression = value),
		new("HealingTickPainExpression", ["healpain", "healingtickpainexpression"], "Healing Tick Pain Expression",
			x => x.HealingTickPainExpression, (x, value) => x.HealingTickPainExpression = value)
	];

	private static readonly DoubleBuilderField<ComplexLivingHealthStrategy>[] DoubleFields =
	[
		PercentageField<ComplexLivingHealthStrategy>("PercentageHealthPerPenalty", ["percentagehealthperpenalty", "healthpenalty"], "Percentage Health Per Penalty",
			x => x.PercentageHealthPerPenalty, (x, value) => x.PercentageHealthPerPenalty = value),
		PercentageField<ComplexLivingHealthStrategy>("PercentageStunPerPenalty", ["percentagestunperpenalty", "stunpenalty"], "Percentage Stun Per Penalty",
			x => x.PercentageStunPerPenalty, (x, value) => x.PercentageStunPerPenalty = value),
		PercentageField<ComplexLivingHealthStrategy>("PercentagePainPerPenalty", ["percentagepainperpenalty", "painpenalty"], "Percentage Pain Per Penalty",
			x => x.PercentagePainPerPenalty, (x, value) => x.PercentagePainPerPenalty = value),
		UnitField<ComplexLivingHealthStrategy>("InternalBleedingRecoveryFlatReduction", ["ibrecoveryflat", "internalbleedingrecoveryflatreduction"], "Internal Bleeding Recovery Flat Reduction",
			UnitType.FluidVolume,
			x => x.InternalBleedingRecoveryFlatReduction, (x, value) => x.InternalBleedingRecoveryFlatReduction = value),
		PercentageField<ComplexLivingHealthStrategy>("InternalBleedingRecoveryMultiplier", ["ibrecoverymultiplier", "internalbleedingrecoverymultiplier"], "Internal Bleeding Recovery Multiplier",
			x => x.InternalBleedingRecoveryMultiplier, (x, value) => x.InternalBleedingRecoveryMultiplier = value),
		UnitField<ComplexLivingHealthStrategy>("InternalBleedingTotalDecayFlatReduction", ["ibdecayflat", "internalbleedingtotaldecayflatreduction"], "Internal Bleeding Total Decay Flat Reduction",
			UnitType.FluidVolume,
			x => x.InternalBleedingTotalDecayFlatReduction, (x, value) => x.InternalBleedingTotalDecayFlatReduction = value),
		PercentageField<ComplexLivingHealthStrategy>("InternalBleedingTotalDecayMultiplier", ["ibdecaymultiplier", "internalbleedingtotaldecaymultiplier"], "Internal Bleeding Total Decay Multiplier",
			x => x.InternalBleedingTotalDecayMultiplier, (x, value) => x.InternalBleedingTotalDecayMultiplier = value),
		UnitField<ComplexLivingHealthStrategy>("AirwayBloodMinorThreshold", ["airwayminor", "airwaybloodminorthreshold"], "Airway Blood Minor Threshold",
			UnitType.FluidVolume,
			x => x.AirwayBloodMinorThreshold, (x, value) => x.AirwayBloodMinorThreshold = value),
		UnitField<ComplexLivingHealthStrategy>("AirwayBloodModerateThreshold", ["airwaymoderate", "airwaybloodmoderatethreshold"], "Airway Blood Moderate Threshold",
			UnitType.FluidVolume,
			x => x.AirwayBloodModerateThreshold, (x, value) => x.AirwayBloodModerateThreshold = value),
		UnitField<ComplexLivingHealthStrategy>("AirwayBloodSevereThreshold", ["airwaysevere", "airwaybloodseverethreshold"], "Airway Blood Severe Threshold",
			UnitType.FluidVolume,
			x => x.AirwayBloodSevereThreshold, (x, value) => x.AirwayBloodSevereThreshold = value),
		UnitField<ComplexLivingHealthStrategy>("DigestiveBloodMinorThreshold", ["digestiveminor", "digestivebloodminorthreshold"], "Digestive Blood Minor Threshold",
			UnitType.FluidVolume,
			x => x.DigestiveBloodMinorThreshold, (x, value) => x.DigestiveBloodMinorThreshold = value),
		UnitField<ComplexLivingHealthStrategy>("DigestiveBloodModerateThreshold", ["digestivemoderate", "digestivebloodmoderatethreshold"], "Digestive Blood Moderate Threshold",
			UnitType.FluidVolume,
			x => x.DigestiveBloodModerateThreshold, (x, value) => x.DigestiveBloodModerateThreshold = value),
		UnitField<ComplexLivingHealthStrategy>("DigestiveBloodSevereThreshold", ["digestivesevere", "digestivebloodseverethreshold"], "Digestive Blood Severe Threshold",
			UnitType.FluidVolume,
			x => x.DigestiveBloodSevereThreshold, (x, value) => x.DigestiveBloodSevereThreshold = value),
		PercentageField<ComplexLivingHealthStrategy>("HeartAttackThreshold", ["heartattackthreshold"], "Heart Attack Threshold",
			x => x.HeartAttackThreshold, (x, value) => x.HeartAttackThreshold = value),
		PercentageField<ComplexLivingHealthStrategy>("BloodLossHypoxiaThreshold", ["bloodlosshypoxiathreshold"], "Blood Loss Hypoxia Threshold",
			x => x.BloodLossHypoxiaThreshold, (x, value) => x.BloodLossHypoxiaThreshold = value),
		PercentageField<ComplexLivingHealthStrategy>("BloodReplacementHypoxiaThreshold", ["bloodreplacementhypoxiathreshold"], "Blood Replacement Hypoxia Threshold",
			x => x.BloodReplacementHypoxiaThreshold, (x, value) => x.BloodReplacementHypoxiaThreshold = value),
		PercentageField<ComplexLivingHealthStrategy>("HydratingHypoxiaThreshold", ["hydratinghypoxiathreshold"], "Hydrating Hypoxia Threshold",
			x => x.HydratingHypoxiaThreshold, (x, value) => x.HydratingHypoxiaThreshold = value),
		PercentageField<ComplexLivingHealthStrategy>("CprMinimumBloodRatio", ["cprminimumbloodratio"], "CPR Minimum Blood Ratio",
			x => x.CprMinimumBloodRatio, (x, value) => x.CprMinimumBloodRatio = value),
		PercentageField<ComplexLivingHealthStrategy>("CprHypoxiaMultiplier", ["cprhypoxiamultiplier"], "CPR Hypoxia Multiplier",
			x => x.CprHypoxiaMultiplier, (x, value) => x.CprHypoxiaMultiplier = value),
		PercentageField<ComplexLivingHealthStrategy>("TissueDieOffHarmfulMultiplier", ["tissuedieoffharmfulmultiplier"], "Tissue Die Off Harmful Multiplier",
			x => x.TissueDieOffHarmfulMultiplier, (x, value) => x.TissueDieOffHarmfulMultiplier = value),
		PercentageField<ComplexLivingHealthStrategy>("TissueDieOffDeadlyMultiplier", ["tissuedieoffdeadlymultiplier"], "Tissue Die Off Deadly Multiplier",
			x => x.TissueDieOffDeadlyMultiplier, (x, value) => x.TissueDieOffDeadlyMultiplier = value),
		UnitField<ComplexLivingHealthStrategy>("TissueDieOffKidneyWasteThreshold", ["tissuedieoffkidneywastethreshold"], "Tissue Die Off Kidney Waste Threshold",
			UnitType.FluidVolume,
			x => x.TissueDieOffKidneyWasteThreshold, (x, value) => x.TissueDieOffKidneyWasteThreshold = value),
		PercentageField<ComplexLivingHealthStrategy>("TissueDieOffKidneyWasteMultiplier", ["tissuedieoffkidneywastemultiplier"], "Tissue Die Off Kidney Waste Multiplier",
			x => x.TissueDieOffKidneyWasteMultiplier, (x, value) => x.TissueDieOffKidneyWasteMultiplier = value),
		PercentageField<ComplexLivingHealthStrategy>("CriticalInjuryBloodRatioThreshold", ["criticalinjurybloodratiothreshold"], "Critical Injury Blood Ratio Threshold",
			x => x.CriticalInjuryBloodRatioThreshold, (x, value) => x.CriticalInjuryBloodRatioThreshold = value),
		new("CriticalInjuryVitalOrganDamageThreshold", ["criticalinjuryvitalorgandamagethreshold"], "Critical Injury Vital Organ Damage Threshold",
			x => x.CriticalInjuryVitalOrganDamageThreshold, (x, value) => x.CriticalInjuryVitalOrganDamageThreshold = value),
		PercentageField<ComplexLivingHealthStrategy>("PainPassOutThreshold", ["painpassoutthreshold"], "Pain Pass Out Threshold",
			x => x.PainPassOutThreshold, (x, value) => x.PainPassOutThreshold = value),
		new("AnesthesiaUnconsciousThreshold", ["anesthesiaunconsciousthreshold"], "Anesthesia Unconscious Threshold",
			x => x.AnesthesiaUnconsciousThreshold, (x, value) => x.AnesthesiaUnconsciousThreshold = value),
		PercentageField<ComplexLivingHealthStrategy>("ContaminantHydratingRemovalFraction", ["contaminanthydratingremovalfraction"], "Contaminant Hydrating Removal Fraction",
			x => x.ContaminantHydratingRemovalFraction, (x, value) => x.ContaminantHydratingRemovalFraction = value),
		PercentageField<ComplexLivingHealthStrategy>("ContaminantBloodRemovalFraction", ["contaminantbloodremovalfraction"], "Contaminant Blood Removal Fraction",
			x => x.ContaminantBloodRemovalFraction, (x, value) => x.ContaminantBloodRemovalFraction = value),
		PercentageField<ComplexLivingHealthStrategy>("MinimumBloodRatioForRegeneration", ["minimumbloodratioforregeneration"], "Minimum Blood Ratio For Regeneration",
			x => x.MinimumBloodRatioForRegeneration, (x, value) => x.MinimumBloodRatioForRegeneration = value),
		PercentageField<ComplexLivingHealthStrategy>("BloodCleanseFraction", ["bloodcleansefraction"], "Blood Cleanse Fraction",
			x => x.BloodCleanseFraction, (x, value) => x.BloodCleanseFraction = value),
		PercentageField<ComplexLivingHealthStrategy>("BloodRegenerationFraction", ["bloodregenerationfraction"], "Blood Regeneration Fraction",
			x => x.BloodRegenerationFraction, (x, value) => x.BloodRegenerationFraction = value),
		PercentageField<ComplexLivingHealthStrategy>("SleepingBloodRegenerationMultiplier", ["sleepingbloodregenerationmultiplier"], "Sleeping Blood Regeneration Multiplier",
			x => x.SleepingBloodRegenerationMultiplier, (x, value) => x.SleepingBloodRegenerationMultiplier = value),
		new("BloodGainThirstPointsPerTick", ["bloodgainthirstpointspertick"], "Blood Gain Thirst Points Per Tick",
			x => x.BloodGainThirstPointsPerTick, (x, value) => x.BloodGainThirstPointsPerTick = value),
		UnitField<ComplexLivingHealthStrategy>("BloodGainWaterLitresPerBloodChange", ["bloodgainwaterlitresperbloodchange"], "Blood Gain Water Litres Per Blood Change",
			UnitType.FluidVolume,
			x => x.BloodGainWaterLitresPerBloodChange, (x, value) => x.BloodGainWaterLitresPerBloodChange = value),
		PercentageField<ComplexLivingHealthStrategy>("KidneyWasteAccumulationThreshold", ["kidneywasteaccumulationthreshold"], "Kidney Waste Accumulation Threshold",
			x => x.KidneyWasteAccumulationThreshold, (x, value) => x.KidneyWasteAccumulationThreshold = value),
		PercentageField<ComplexLivingHealthStrategy>("KidneyWasteAccumulationMultiplier", ["kidneywasteaccumulationmultiplier"], "Kidney Waste Accumulation Multiplier",
			x => x.KidneyWasteAccumulationMultiplier, (x, value) => x.KidneyWasteAccumulationMultiplier = value),
		PercentageField<ComplexLivingHealthStrategy>("SpleenFunctionThreshold", ["spleenfunctionthreshold"], "Spleen Function Threshold",
			x => x.SpleenFunctionThreshold, (x, value) => x.SpleenFunctionThreshold = value),
		PercentageField<ComplexLivingHealthStrategy>("SpleenBloodReplacementCleanseMultiplier", ["spleenbloodreplacementcleansemultiplier"], "Spleen Blood Replacement Cleanse Multiplier",
			x => x.SpleenBloodReplacementCleanseMultiplier, (x, value) => x.SpleenBloodReplacementCleanseMultiplier = value),
		PercentageField<ComplexLivingHealthStrategy>("SpleenHarmfulCleanseMultiplier", ["spleenharmfulcleansemultiplier"], "Spleen Harmful Cleanse Multiplier",
			x => x.SpleenHarmfulCleanseMultiplier, (x, value) => x.SpleenHarmfulCleanseMultiplier = value)
	];

	private static readonly IntBuilderField<ComplexLivingHealthStrategy>[] IntFields =
	[
		new("InternalBleedingRecoveryRollSides", ["ibrecoveryrollsides", "internalbleedingrecoveryrollsides"], "Internal Bleeding Recovery Roll Sides",
			x => x.InternalBleedingRecoveryRollSides, (x, value) => x.InternalBleedingRecoveryRollSides = value,
			Validator: (_, value) => value > 0
				? (true, string.Empty, value)
				: (false, "The internal bleeding recovery roll sides must be at least 1.", value)),
		new("ExistingFractureAggravationChanceDivisor", ["existingfractureaggravationchancedivisor", "fractureaggravationdivisor"], "Existing Fracture Aggravation Chance Divisor",
			x => x.ExistingFractureAggravationChanceDivisor, (x, value) => x.ExistingFractureAggravationChanceDivisor = value,
			Validator: (_, value) => value > 0
				? (true, string.Empty, value)
				: (false, "The existing fracture aggravation chance divisor must be at least 1.", value))
	];

	private static readonly TimeSpanBuilderField<ComplexLivingHealthStrategy>[] TimeSpanFields =
	[
		new("BleedMessageCooldown", ["bleedcooldown", "bleedmessagecooldown"], "Bleed Message Cooldown",
			x => x.BleedMessageCooldown, (x, value) => x.BleedMessageCooldown = value)
	];

	private const string TypeBlurb =
		"A detailed organic character model with bone fractures, organ failure, temperature, blood chemistry, CPR, and recovery tuning.";

	private static readonly string TypeHelp = BuildTypeHelp(TypeBlurb,
		GetBuilderFieldHelpText(TraitExpressionFields)
			.Concat(GetBuilderFieldHelpText(DoubleFields))
			.Concat(GetBuilderFieldHelpText(IntFields))
			.Concat(GetBuilderFieldHelpText(TimeSpanFields)));

	private ComplexLivingHealthStrategy(HealthStrategy strategy, IFuturemud gameworld)
		: base(strategy, gameworld)
	{
		LoadDefinition(XElement.Parse(strategy.Definition), gameworld);
	}

	private ComplexLivingHealthStrategy(IFuturemud gameworld, string name)
		: base(gameworld, name)
	{
		SetComplexDefaultSeverityRanges();
		MaximumHitPointsExpression = CreateDefaultExpression(gameworld, $"{name} Max HP", "100");
		MaximumStunExpression = CreateDefaultExpression(gameworld, $"{name} Max Stun", "100");
		MaximumPainExpression = CreateDefaultExpression(gameworld, $"{name} Max Pain", "100");
		HealingTickDamageExpression = CreateDefaultExpression(gameworld, $"{name} Damage Heal", "1");
		HealingTickStunExpression = CreateDefaultExpression(gameworld, $"{name} Stun Heal", "1");
		HealingTickPainExpression = CreateDefaultExpression(gameworld, $"{name} Pain Heal", "1");
		PercentageHealthPerPenalty = 1.0;
		PercentageStunPerPenalty = 1.0;
		PercentagePainPerPenalty = 1.0;
		BleedMessageCooldown = TimeSpan.FromSeconds(15);
		InternalBleedingRecoveryRollSides = 30;
		InternalBleedingRecoveryFlatReduction = 0.01;
		InternalBleedingRecoveryMultiplier = 0.66;
		InternalBleedingTotalDecayFlatReduction = 1.0;
		InternalBleedingTotalDecayMultiplier = 0.98;
		AirwayBloodMinorThreshold = 0.005;
		AirwayBloodModerateThreshold = 0.01;
		AirwayBloodSevereThreshold = 0.03;
		DigestiveBloodMinorThreshold = 0.1;
		DigestiveBloodModerateThreshold = 0.3;
		DigestiveBloodSevereThreshold = 0.4;
		HeartAttackThreshold = 0.3;
		BloodLossHypoxiaThreshold = 0.6;
		BloodReplacementHypoxiaThreshold = 0.5;
		HydratingHypoxiaThreshold = 0.66;
		CprMinimumBloodRatio = 0.4;
		CprHypoxiaMultiplier = 0.01;
		TissueDieOffHarmfulMultiplier = 1.0;
		TissueDieOffDeadlyMultiplier = 5.0;
		TissueDieOffKidneyWasteThreshold = 1000.0;
		TissueDieOffKidneyWasteMultiplier = 0.5;
		CriticalInjuryBloodRatioThreshold = 0.75;
		CriticalInjuryVitalOrganDamageThreshold = 10.0;
		PainPassOutThreshold = 1.0;
		AnesthesiaUnconsciousThreshold = 1.0;
		ContaminantHydratingRemovalFraction = 0.001;
		ContaminantBloodRemovalFraction = 0.002;
		MinimumBloodRatioForRegeneration = 0.2;
		BloodCleanseFraction = 0.00025;
		BloodRegenerationFraction = 0.0005;
		SleepingBloodRegenerationMultiplier = 1.05;
		BloodGainThirstPointsPerTick = -0.01;
		BloodGainWaterLitresPerBloodChange = -0.85;
		KidneyWasteAccumulationThreshold = 0.3;
		KidneyWasteAccumulationMultiplier = 1.0;
		SpleenFunctionThreshold = 0.3;
		SpleenBloodReplacementCleanseMultiplier = 2.0;
		SpleenHarmfulCleanseMultiplier = 50.0;
		ExistingFractureAggravationChanceDivisor = 3;
		DoDatabaseInsert(HealthStrategyType);
	}

	private ComplexLivingHealthStrategy(ComplexLivingHealthStrategy rhs, string name)
		: base(rhs, name)
	{
		MaximumHitPointsExpression = CloneExpression(rhs.MaximumHitPointsExpression, Gameworld);
		MaximumStunExpression = CloneExpression(rhs.MaximumStunExpression, Gameworld);
		MaximumPainExpression = CloneExpression(rhs.MaximumPainExpression, Gameworld);
		HealingTickDamageExpression = CloneExpression(rhs.HealingTickDamageExpression, Gameworld);
		HealingTickStunExpression = CloneExpression(rhs.HealingTickStunExpression, Gameworld);
		HealingTickPainExpression = CloneExpression(rhs.HealingTickPainExpression, Gameworld);
		PercentageHealthPerPenalty = rhs.PercentageHealthPerPenalty;
		PercentageStunPerPenalty = rhs.PercentageStunPerPenalty;
		PercentagePainPerPenalty = rhs.PercentagePainPerPenalty;
		BleedMessageCooldown = rhs.BleedMessageCooldown;
		InternalBleedingRecoveryRollSides = rhs.InternalBleedingRecoveryRollSides;
		InternalBleedingRecoveryFlatReduction = rhs.InternalBleedingRecoveryFlatReduction;
		InternalBleedingRecoveryMultiplier = rhs.InternalBleedingRecoveryMultiplier;
		InternalBleedingTotalDecayFlatReduction = rhs.InternalBleedingTotalDecayFlatReduction;
		InternalBleedingTotalDecayMultiplier = rhs.InternalBleedingTotalDecayMultiplier;
		AirwayBloodMinorThreshold = rhs.AirwayBloodMinorThreshold;
		AirwayBloodModerateThreshold = rhs.AirwayBloodModerateThreshold;
		AirwayBloodSevereThreshold = rhs.AirwayBloodSevereThreshold;
		DigestiveBloodMinorThreshold = rhs.DigestiveBloodMinorThreshold;
		DigestiveBloodModerateThreshold = rhs.DigestiveBloodModerateThreshold;
		DigestiveBloodSevereThreshold = rhs.DigestiveBloodSevereThreshold;
		HeartAttackThreshold = rhs.HeartAttackThreshold;
		BloodLossHypoxiaThreshold = rhs.BloodLossHypoxiaThreshold;
		BloodReplacementHypoxiaThreshold = rhs.BloodReplacementHypoxiaThreshold;
		HydratingHypoxiaThreshold = rhs.HydratingHypoxiaThreshold;
		CprMinimumBloodRatio = rhs.CprMinimumBloodRatio;
		CprHypoxiaMultiplier = rhs.CprHypoxiaMultiplier;
		TissueDieOffHarmfulMultiplier = rhs.TissueDieOffHarmfulMultiplier;
		TissueDieOffDeadlyMultiplier = rhs.TissueDieOffDeadlyMultiplier;
		TissueDieOffKidneyWasteThreshold = rhs.TissueDieOffKidneyWasteThreshold;
		TissueDieOffKidneyWasteMultiplier = rhs.TissueDieOffKidneyWasteMultiplier;
		CriticalInjuryBloodRatioThreshold = rhs.CriticalInjuryBloodRatioThreshold;
		CriticalInjuryVitalOrganDamageThreshold = rhs.CriticalInjuryVitalOrganDamageThreshold;
		PainPassOutThreshold = rhs.PainPassOutThreshold;
		AnesthesiaUnconsciousThreshold = rhs.AnesthesiaUnconsciousThreshold;
		ContaminantHydratingRemovalFraction = rhs.ContaminantHydratingRemovalFraction;
		ContaminantBloodRemovalFraction = rhs.ContaminantBloodRemovalFraction;
		MinimumBloodRatioForRegeneration = rhs.MinimumBloodRatioForRegeneration;
		BloodCleanseFraction = rhs.BloodCleanseFraction;
		BloodRegenerationFraction = rhs.BloodRegenerationFraction;
		SleepingBloodRegenerationMultiplier = rhs.SleepingBloodRegenerationMultiplier;
		BloodGainThirstPointsPerTick = rhs.BloodGainThirstPointsPerTick;
		BloodGainWaterLitresPerBloodChange = rhs.BloodGainWaterLitresPerBloodChange;
		KidneyWasteAccumulationThreshold = rhs.KidneyWasteAccumulationThreshold;
		KidneyWasteAccumulationMultiplier = rhs.KidneyWasteAccumulationMultiplier;
		SpleenFunctionThreshold = rhs.SpleenFunctionThreshold;
		SpleenBloodReplacementCleanseMultiplier = rhs.SpleenBloodReplacementCleanseMultiplier;
		SpleenHarmfulCleanseMultiplier = rhs.SpleenHarmfulCleanseMultiplier;
		ExistingFractureAggravationChanceDivisor = rhs.ExistingFractureAggravationChanceDivisor;
		DoDatabaseInsert(HealthStrategyType);
	}

	public ITraitExpression HealingTickDamageExpression { get; set; }
	public ITraitExpression HealingTickStunExpression { get; set; }
	public ITraitExpression HealingTickPainExpression { get; set; }
	public ITraitExpression MaximumHitPointsExpression { get; set; }
	public ITraitExpression MaximumStunExpression { get; set; }
	public ITraitExpression MaximumPainExpression { get; set; }

	public double PercentageHealthPerPenalty { get; set; }
	public double PercentageStunPerPenalty { get; set; }
	public double PercentagePainPerPenalty { get; set; }
	public TimeSpan BleedMessageCooldown { get; set; }
	public int InternalBleedingRecoveryRollSides { get; set; }
	public double InternalBleedingRecoveryFlatReduction { get; set; }
	public double InternalBleedingRecoveryMultiplier { get; set; }
	public double InternalBleedingTotalDecayFlatReduction { get; set; }
	public double InternalBleedingTotalDecayMultiplier { get; set; }
	public double AirwayBloodMinorThreshold { get; set; }
	public double AirwayBloodModerateThreshold { get; set; }
	public double AirwayBloodSevereThreshold { get; set; }
	public double DigestiveBloodMinorThreshold { get; set; }
	public double DigestiveBloodModerateThreshold { get; set; }
	public double DigestiveBloodSevereThreshold { get; set; }
	public double HeartAttackThreshold { get; set; }
	public double BloodLossHypoxiaThreshold { get; set; }
	public double BloodReplacementHypoxiaThreshold { get; set; }
	public double HydratingHypoxiaThreshold { get; set; }
	public double CprMinimumBloodRatio { get; set; }
	public double CprHypoxiaMultiplier { get; set; }
	public double TissueDieOffHarmfulMultiplier { get; set; }
	public double TissueDieOffDeadlyMultiplier { get; set; }
	public double TissueDieOffKidneyWasteThreshold { get; set; }
	public double TissueDieOffKidneyWasteMultiplier { get; set; }
	public double CriticalInjuryBloodRatioThreshold { get; set; }
	public double CriticalInjuryVitalOrganDamageThreshold { get; set; }
	public double PainPassOutThreshold { get; set; }
	public double AnesthesiaUnconsciousThreshold { get; set; }
	public double ContaminantHydratingRemovalFraction { get; set; }
	public double ContaminantBloodRemovalFraction { get; set; }
	public double MinimumBloodRatioForRegeneration { get; set; }
	public double BloodCleanseFraction { get; set; }
	public double BloodRegenerationFraction { get; set; }
	public double SleepingBloodRegenerationMultiplier { get; set; }
	public double BloodGainThirstPointsPerTick { get; set; }
	public double BloodGainWaterLitresPerBloodChange { get; set; }
	public double KidneyWasteAccumulationThreshold { get; set; }
	public double KidneyWasteAccumulationMultiplier { get; set; }
	public double SpleenFunctionThreshold { get; set; }
	public double SpleenBloodReplacementCleanseMultiplier { get; set; }
	public double SpleenHarmfulCleanseMultiplier { get; set; }
	public int ExistingFractureAggravationChanceDivisor { get; set; }

	public override string HealthStrategyType => "ComplexLiving";
	public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;

	protected override IEnumerable<string> SubtypeBuilderHelpText =>
		GetBuilderFieldHelpText(TraitExpressionFields)
			.Concat(GetBuilderFieldHelpText(DoubleFields))
			.Concat(GetBuilderFieldHelpText(IntFields))
			.Concat(GetBuilderFieldHelpText(TimeSpanFields));

	public static void RegisterHealthStrategyLoader()
	{
		RegisterHealthStrategy("ComplexLiving",
			(strategy, game) => new ComplexLivingHealthStrategy(strategy, game),
			(game, name) => new ComplexLivingHealthStrategy(game, name),
			TypeHelp,
			TypeBlurb);
	}

	private void LoadDefinition(XElement root, IFuturemud gameworld)
	{
		var element = root.Element("MaximumHitPointsExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a MaximumHitPointsExpression element.");
		}

		if (!long.TryParse(element.Value, out var value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a MaximumHitPointsExpression element that did not contain an ID.");
		}

		MaximumHitPointsExpression = gameworld.TraitExpressions.Get(value);

		PercentageHealthPerPenalty = LoadDouble(root, "PercentageHealthPerPenalty", 1.0);
		PercentageStunPerPenalty = LoadDouble(root, "PercentageStunPerPenalty", 1.0);
		PercentagePainPerPenalty = LoadDouble(root, "PercentagePainPerPenalty", 1.0);
		BleedMessageCooldown = LoadTimeSpanFromSeconds(root, "BleedMessageCooldown", 15);
		InternalBleedingRecoveryRollSides = LoadInt(root, "InternalBleedingRecoveryRollSides", 30);
		InternalBleedingRecoveryFlatReduction = LoadDouble(root, "InternalBleedingRecoveryFlatReduction", 0.01);
		InternalBleedingRecoveryMultiplier = LoadDouble(root, "InternalBleedingRecoveryMultiplier", 0.66);
		InternalBleedingTotalDecayFlatReduction = LoadDouble(root, "InternalBleedingTotalDecayFlatReduction", 1.0);
		InternalBleedingTotalDecayMultiplier = LoadDouble(root, "InternalBleedingTotalDecayMultiplier", 0.98);
		AirwayBloodMinorThreshold = LoadDouble(root, "AirwayBloodMinorThreshold", 0.005);
		AirwayBloodModerateThreshold = LoadDouble(root, "AirwayBloodModerateThreshold", 0.01);
		AirwayBloodSevereThreshold = LoadDouble(root, "AirwayBloodSevereThreshold", 0.03);
		DigestiveBloodMinorThreshold = LoadDouble(root, "DigestiveBloodMinorThreshold", 0.1);
		DigestiveBloodModerateThreshold = LoadDouble(root, "DigestiveBloodModerateThreshold", 0.3);
		DigestiveBloodSevereThreshold = LoadDouble(root, "DigestiveBloodSevereThreshold", 0.4);
		HeartAttackThreshold = LoadDouble(root, "HeartAttackThreshold", 0.3);
		BloodLossHypoxiaThreshold = LoadDouble(root, "BloodLossHypoxiaThreshold", 0.6);
		BloodReplacementHypoxiaThreshold = LoadDouble(root, "BloodReplacementHypoxiaThreshold", 0.5);
		HydratingHypoxiaThreshold = LoadDouble(root, "HydratingHypoxiaThreshold", 0.66);
		CprMinimumBloodRatio = LoadDouble(root, "CprMinimumBloodRatio", 0.4);
		CprHypoxiaMultiplier = LoadDouble(root, "CprHypoxiaMultiplier", 0.01);
		TissueDieOffHarmfulMultiplier = LoadDouble(root, "TissueDieOffHarmfulMultiplier", 1.0);
		TissueDieOffDeadlyMultiplier = LoadDouble(root, "TissueDieOffDeadlyMultiplier", 5.0);
		TissueDieOffKidneyWasteThreshold = LoadDouble(root, "TissueDieOffKidneyWasteThreshold", 1000.0);
		TissueDieOffKidneyWasteMultiplier = LoadDouble(root, "TissueDieOffKidneyWasteMultiplier", 0.5);
		CriticalInjuryBloodRatioThreshold = LoadDouble(root, "CriticalInjuryBloodRatioThreshold", 0.75);
		CriticalInjuryVitalOrganDamageThreshold =
			LoadDouble(root, "CriticalInjuryVitalOrganDamageThreshold", 10.0);
		PainPassOutThreshold = LoadDouble(root, "PainPassOutThreshold", 1.0);
		AnesthesiaUnconsciousThreshold = LoadDouble(root, "AnesthesiaUnconsciousThreshold", 1.0);
		ContaminantHydratingRemovalFraction = LoadDouble(root, "ContaminantHydratingRemovalFraction", 0.001);
		ContaminantBloodRemovalFraction = LoadDouble(root, "ContaminantBloodRemovalFraction", 0.002);
		MinimumBloodRatioForRegeneration = LoadDouble(root, "MinimumBloodRatioForRegeneration", 0.2);
		BloodCleanseFraction = LoadDouble(root, "BloodCleanseFraction", 0.00025);
		BloodRegenerationFraction = LoadDouble(root, "BloodRegenerationFraction", 0.0005);
		SleepingBloodRegenerationMultiplier = LoadDouble(root, "SleepingBloodRegenerationMultiplier", 1.05);
		BloodGainThirstPointsPerTick = LoadDouble(root, "BloodGainThirstPointsPerTick", -0.01);
		BloodGainWaterLitresPerBloodChange = LoadDouble(root, "BloodGainWaterLitresPerBloodChange", -0.85);
		KidneyWasteAccumulationThreshold = LoadDouble(root, "KidneyWasteAccumulationThreshold", 0.3);
		KidneyWasteAccumulationMultiplier = LoadDouble(root, "KidneyWasteAccumulationMultiplier", 1.0);
		SpleenFunctionThreshold = LoadDouble(root, "SpleenFunctionThreshold", 0.3);
		SpleenBloodReplacementCleanseMultiplier =
			LoadDouble(root, "SpleenBloodReplacementCleanseMultiplier", 2.0);
		SpleenHarmfulCleanseMultiplier = LoadDouble(root, "SpleenHarmfulCleanseMultiplier", 50.0);
		ExistingFractureAggravationChanceDivisor =
			Math.Max(1, LoadInt(root, "ExistingFractureAggravationChanceDivisor", 3));

		element = root.Element("MaximumStunExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a MaximumStunExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a MaximumStunExpression element that did not contain an ID.");
		}

		MaximumStunExpression = gameworld.TraitExpressions.Get(value);

		element = root.Element("MaximumPainExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a MaximumPainExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a MaximumPainExpression element that did not contain an ID.");
		}

		MaximumPainExpression = gameworld.TraitExpressions.Get(value);

		element = root.Element("HealingTickDamageExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a HealingTickDamageExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a HealingTickDamageExpression element that did not contain an ID.");
		}

		HealingTickDamageExpression = gameworld.TraitExpressions.Get(value);

		element = root.Element("HealingTickStunExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a HealingTickStunExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a HealingTickStunExpression element that did not contain an ID.");
		}

		HealingTickStunExpression = gameworld.TraitExpressions.Get(value);

		element = root.Element("HealingTickPainExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a HealingTickPainExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a HealingTickPainExpression element that did not contain an ID.");
		}

		HealingTickPainExpression = gameworld.TraitExpressions.Get(value);
	}

	protected override void SaveSubtypeDefinition(XElement root)
	{
		SaveBuilderFields(root, this, TraitExpressionFields);
		SaveBuilderFields(root, this, DoubleFields);
		SaveBuilderFields(root, this, IntFields);
		SaveBuilderFields(root, this, TimeSpanFields);
	}

	protected override void AppendSubtypeShow(StringBuilder sb, ICharacter actor)
	{
		sb.AppendLine();
		sb.AppendLine("Expressions".GetLineWithTitleInner(actor, Telnet.Yellow, Telnet.BoldWhite));
		AppendBuilderFieldShow(sb, actor, this, TraitExpressionFields);
		sb.AppendLine();
		sb.AppendLine("Penalties and Recovery".GetLineWithTitleInner(actor, Telnet.Yellow, Telnet.BoldWhite));
		AppendBuilderFieldShow(sb, actor, this,
			DoubleFields.Where(x => x.ElementName is
				"PercentageHealthPerPenalty" or
				"PercentageStunPerPenalty" or
				"PercentagePainPerPenalty" or
				"InternalBleedingRecoveryFlatReduction" or
				"InternalBleedingRecoveryMultiplier" or
				"InternalBleedingTotalDecayFlatReduction" or
				"InternalBleedingTotalDecayMultiplier"));
		AppendBuilderFieldShow(sb, actor, this,
			IntFields.Where(x => x.ElementName is
				"InternalBleedingRecoveryRollSides" or
				"ExistingFractureAggravationChanceDivisor"));
		AppendBuilderFieldShow(sb, actor, this,
			TimeSpanFields.Where(x => x.ElementName is "BleedMessageCooldown"));
		sb.AppendLine();
		sb.AppendLine("Bleeding Symptoms".GetLineWithTitleInner(actor, Telnet.Yellow, Telnet.BoldWhite));
		AppendBuilderFieldShow(sb, actor, this,
			DoubleFields.Where(x => x.ElementName is
				"AirwayBloodMinorThreshold" or
				"AirwayBloodModerateThreshold" or
				"AirwayBloodSevereThreshold" or
				"DigestiveBloodMinorThreshold" or
				"DigestiveBloodModerateThreshold" or
				"DigestiveBloodSevereThreshold"));
		sb.AppendLine();
		sb.AppendLine("Hypoxia and Critical Injury".GetLineWithTitleInner(actor, Telnet.Yellow, Telnet.BoldWhite));
		AppendBuilderFieldShow(sb, actor, this,
			DoubleFields.Where(x => x.ElementName is
				"HeartAttackThreshold" or
				"BloodLossHypoxiaThreshold" or
				"BloodReplacementHypoxiaThreshold" or
				"HydratingHypoxiaThreshold" or
				"CprMinimumBloodRatio" or
				"CprHypoxiaMultiplier" or
				"CriticalInjuryBloodRatioThreshold" or
				"CriticalInjuryVitalOrganDamageThreshold" or
				"PainPassOutThreshold" or
				"AnesthesiaUnconsciousThreshold"));
		sb.AppendLine();
		sb.AppendLine("Contaminants and Regeneration".GetLineWithTitleInner(actor, Telnet.Yellow, Telnet.BoldWhite));
		AppendBuilderFieldShow(sb, actor, this,
			DoubleFields.Where(x => x.ElementName is
				"TissueDieOffHarmfulMultiplier" or
				"TissueDieOffDeadlyMultiplier" or
				"TissueDieOffKidneyWasteThreshold" or
				"TissueDieOffKidneyWasteMultiplier" or
				"ContaminantHydratingRemovalFraction" or
				"ContaminantBloodRemovalFraction" or
				"MinimumBloodRatioForRegeneration" or
				"BloodCleanseFraction" or
				"BloodRegenerationFraction" or
				"SleepingBloodRegenerationMultiplier" or
				"BloodGainThirstPointsPerTick" or
				"BloodGainWaterLitresPerBloodChange" or
				"KidneyWasteAccumulationThreshold" or
				"KidneyWasteAccumulationMultiplier" or
				"SpleenFunctionThreshold" or
				"SpleenBloodReplacementCleanseMultiplier" or
				"SpleenHarmfulCleanseMultiplier"));
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (TryBuildingCommand(actor, command.GetUndo(), TraitExpressionFields))
		{
			return true;
		}

		if (TryBuildingCommand(actor, command.GetUndo(), DoubleFields))
		{
			return true;
		}

		if (TryBuildingCommand(actor, command.GetUndo(), IntFields))
		{
			return true;
		}

		if (TryBuildingCommand(actor, command.GetUndo(), TimeSpanFields))
		{
			return true;
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	public override IHealthStrategy Clone(string name)
	{
		return new ComplexLivingHealthStrategy(this, name);
	}

	public override IEnumerable<IWound> SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart)
	{
		if (bodypart == null)
		{
			return Enumerable.Empty<IWound>();
		}

#if DEBUG
		if (double.IsInfinity(damage.DamageAmount) || double.IsInfinity(damage.PainAmount) ||
		    double.IsInfinity(damage.StunAmount) ||
		    double.IsNaN(damage.DamageAmount) || double.IsNaN(damage.PainAmount) || double.IsNaN(damage.StunAmount))
		{
			throw new ApplicationException("Invalid damage/pain/stun in SufferDamage.");
		}
#endif
		var chOwner = (ICharacter)owner;
		IGameItem lodgedItem = CheckDamageLodges(damage) ? damage.LodgableItem : null;
		
		if (bodypart is IBone bone)
		{
			var wounds = new List<IWound>();
			var (ordinaryDamageAmount, boneDamageAmount) = bone.ShouldBeBoneBreak(damage);
			if (boneDamageAmount > 0.0)
			{
				var boneDamage = new Damage(damage) { DamageAmount = boneDamageAmount };
				if (RandomUtilities.Random(1, ExistingFractureAggravationChanceDivisor) == 1)
				{
					var existing =
						owner.Wounds.Where(x => x.Bodypart == bodypart).OfType<BoneFracture>().GetRandomElement();
					if (existing != null)
					{
						existing.SufferAdditionalDamage(boneDamage);
						wounds.Add(existing);
					}
				}
				else
				{
					wounds.Add(new BoneFracture(owner.Gameworld, owner, boneDamageAmount, boneDamageAmount,
						0, DamageType.Crushing, damage.Bodypart, damage.ToolOrigin, damage.ActorOrigin));
				}
			}
			
			if (ordinaryDamageAmount > 0)
			{
				damage = new Damage(damage) { DamageAmount = ordinaryDamageAmount };
				wounds.Add(new SimpleOrganicWound(owner.Gameworld, chOwner, damage.DamageAmount, damage.PainAmount,
					damage.StunAmount, damage.DamageType, damage.Bodypart, lodgedItem, damage.ToolOrigin,
					damage.ActorOrigin));
			}

			return wounds;
		}

		return new[]
		{
			new SimpleOrganicWound(owner.Gameworld, chOwner, damage.DamageAmount, damage.PainAmount,
				damage.StunAmount, damage.DamageType, damage.Bodypart, lodgedItem, damage.ToolOrigin,
				damage.ActorOrigin)
		};
	}

	private string WoundCountDesc(int count)
	{
		switch (count)
		{
			case 1:
				return "a wound";
			case 2:
				return "a couple of wounds";
			case 3:
			case 4:
				return "a few wounds";
			case 5:
			case 6:
			case 7:
			case 8:
				return "several wounds";
			default:
				return "many wounds";
		}
	}

	private void DoTemperatureTick(ICharacter character)
	{
		if (character.IsAdministrator())
		{
			return;
		}

		var (floor, ceiling) = character.Body.TolerableTemperatures(true);
		var temperature = character.Location.CurrentTemperature(character);
		var subjective =
			TemperatureExtensions.SubjectiveTemperature(temperature, floor, ceiling);
		var effect = character.CombinedEffectsOfType<ThermalImbalance>().FirstOrDefault();
		if (effect != null && subjective == Temperature.Temperate)
		{
			return;
		}

		if (effect == null)
		{
			effect = new ThermalImbalance(character);
			character.AddEffect(effect);
		}

		effect.TenSecondProgress(subjective);
	}

	public override HealthTickResult PerformHealthTick(IHaveWounds thing)
	{
		var charOwner = (ICharacter)thing;
		if (charOwner.State.HasFlag(CharacterState.Dead) && charOwner.Corpse == null)
		{
			charOwner.Quit();
			return HealthTickResult.Dead;
		}

		DoTemperatureTick(charOwner);

		var totalBlood = charOwner.Body.TotalBloodVolumeLitres;
		var isBleeding = false;
		if (charOwner.Body.CurrentBloodVolumeLitres > 0 && charOwner.LongtermExertion != ExertionLevel.Stasis)
		{
			var bleedingWounds =
				thing.Wounds.Select(
					     x => x.Bleed(charOwner.Body.CurrentBloodVolumeLitres, charOwner.Body.CurrentExertion,
						     charOwner.Body.TotalBloodVolumeLitres))
				     .Where(x => x.BloodAmount > 0)
				     .ToList();
			var bleeding = bleedingWounds.Sum(x => x.BloodAmount);
			if (bleeding != 0)
			{
				charOwner.Body.CurrentBloodVolumeLitres -= bleeding;
			}

			if (bleeding > 0)
			{
				charOwner.HandleEvent(EventType.BleedTick, charOwner, bleeding);
				isBleeding = true;
			}

			var visibleBleedingWounds = bleedingWounds.Where(x => x.Visible).ToList();
			if (visibleBleedingWounds.Any())
			{
				if (!charOwner.Body.EffectsOfType<ISuppressBleedMessage>().Any())
				{
					var messages = new List<string>();
					var perceivables = new List<IPerceivable> { charOwner };
					if (visibleBleedingWounds.Any(x => x.CoverItem == null))
					{
						var wounds = visibleBleedingWounds.Where(x => x.CoverItem == null).ToList();
						var countDesc = WoundCountDesc(wounds.Count);
						messages.Add(
							$"{countDesc} on &0's {wounds.Select(x => x.Bodypart).Distinct().Select(x => x.FullDescription()).ListToString()}");
					}

					if (visibleBleedingWounds.Any(x => x.CoverItem != null))
					{
						var wounds = visibleBleedingWounds.Where(x => x.CoverItem != null).ToList();
						var countDesc = WoundCountDesc(wounds.Count);
						var index = 1;
						messages.Add(
							$"{countDesc} underneath {wounds.Select(x => x.CoverItem).Distinct().Select(x => $"${index++}").ListToString()}");
						perceivables.AddRange(wounds.Select(x => x.CoverItem).Distinct());
					}

					charOwner.OutputHandler.Handle(
						new EmoteOutput(
							new Emote($"$0 bleed|bleeds from {messages.ListToString()}", charOwner,
								perceivables.ToArray()), flags: OutputFlags.InnerWrap | OutputFlags.SuppressObscured));
					if (!charOwner.Body.EffectsOfType<ISuppressBleedMessage>().Any())
					{
						charOwner.Body.AddEffect(new SuppressBleedMessage(charOwner.Body, null),
							BleedMessageCooldown);
					}
				}

				foreach (var witness in charOwner.Location?.Perceivables ?? Enumerable.Empty<IPerceivable>())
				{
					witness.HandleEvent(EventType.WitnessBleedTick, charOwner, bleeding, witness);
				}
			}


			var internalBleeders = charOwner.Body.EffectsOfType<IInternalBleedingEffect>().ToList();
			var internalBleeding = internalBleeders.Sum(x => x.BloodlossPerTick);
			if (internalBleeding > 0)
			{
				isBleeding = true;
				var message = !thing.EffectsOfType<ISuppressBleedMessage>().Any();
				var airwayBlood = 0.0;
				var digestiveBlood = 0.0;
				charOwner.Body.CurrentBloodVolumeLitres -= internalBleeding;
				foreach (var effect in internalBleeders)
				{
					if (Dice.Roll(1, InternalBleedingRecoveryRollSides) == 1)
					{
						effect.BloodlossPerTick = Math.Max(Math.Min(
							effect.BloodlossPerTick - InternalBleedingRecoveryFlatReduction,
							effect.BloodlossPerTick * InternalBleedingRecoveryMultiplier), 0);
					}

					if (effect.BloodlossPerTick <= 0.0)
					{
						effect.BloodlossTotal = Math.Max(0,
							Math.Min(effect.BloodlossTotal - InternalBleedingTotalDecayFlatReduction,
								effect.BloodlossTotal * InternalBleedingTotalDecayMultiplier));
					}

					if (effect.BloodlossPerTick <= 0.0 && effect.BloodlossTotal <= 0.0)
					{
						charOwner.Body.RemoveEffect(effect);
						continue;
					}

					effect.BloodlossTotal += effect.BloodlossPerTick;
					if (effect.Organ is LungProto)
					{
						airwayBlood += effect.BloodlossTotal;
					}
					else if (effect.Organ is TracheaProto)
					{
						airwayBlood += effect.BloodlossTotal;
					}
					else if (effect.Organ is StomachProto)
					{
						digestiveBlood += effect.BloodlossTotal;
					}
					else if (effect.Organ is EsophagusProto)
					{
						digestiveBlood += effect.BloodlossTotal;
					}
				}

				if (airwayBlood > AirwayBloodMinorThreshold && message)
				{
					var amount = "a small amount of";
					var action = "";
					if (airwayBlood > AirwayBloodModerateThreshold)
					{
						amount = "some";
					}
					else if (airwayBlood > AirwayBloodSevereThreshold)
					{
						amount = "a large amount of";
						action = "violently ";
					}

					if (charOwner.IsBreathing)
					{
						charOwner.OutputHandler.Handle(
							new EmoteOutput(
								new Emote(
									$"@ {action}cough|coughs up {amount} {charOwner.Race.BloodLiquid.Name.ToLowerInvariant()}.",
									charOwner)));
					}
					else
					{
						charOwner.OutputHandler.Handle(
							new EmoteOutput(
								new Emote(
									$"{amount} {charOwner.Race.BloodLiquid.Name.ToLowerInvariant()} runs out of @'s mouth.",
									charOwner)));
					}

					foreach (var effect in internalBleeders)
					{
						if (effect.Organ is LungProto || effect.Organ is TracheaProto)
						{
							effect.BloodlossTotal = 0;
						}
					}
				}

				if (digestiveBlood > DigestiveBloodMinorThreshold && message)
				{
					var amount = "a small amount of";
					var action = "spit|spits up";
					if (digestiveBlood > DigestiveBloodModerateThreshold)
					{
						amount = "some";
						action = "vomit|vomits up";
					}
					else if (digestiveBlood > DigestiveBloodSevereThreshold)
					{
						amount = "a large amount of";
						action = "violently vomit|vomits out";
					}

					if (charOwner.IsBreathing)
					{
						charOwner.OutputHandler.Handle(
							new EmoteOutput(
								new Emote(
									$"@ {action} {amount} {charOwner.Race.BloodLiquid.Name.ToLowerInvariant()}.",
									charOwner)));
					}
					else
					{
						charOwner.OutputHandler.Handle(
							new EmoteOutput(
								new Emote(
									$"{amount} {charOwner.Race.BloodLiquid.Name.ToLowerInvariant()} runs out of @'s mouth.",
									charOwner)));
					}

					foreach (var effect in internalBleeders)
					{
						if (effect.Organ is StomachProto || effect.Organ is EsophagusProto)
						{
							effect.BloodlossTotal = 0;
						}
					}
				}
			}
		}

		if (charOwner.State.HasFlag(CharacterState.Dead))
		{
			// Only process bleeding for dead people
			if (!isBleeding)
			{
				charOwner.EndHealthTick();
			}

			return HealthTickResult.Dead;
		}

		var hypoxia = 0.0;
		// Hypoxia from Cardiac Arrest
		var heartAttack = false;
		var heartFactor = charOwner.Body.OrganFunction<HeartProto>();

		if (heartFactor < HeartAttackThreshold)
		{
			heartAttack = true;
			charOwner.OutputHandler.Send(heartFactor <= 0
				? $"You feel an all-encompassing sensation of crushing in your {charOwner.Body.Bodyparts.Where(x => x.Organs.Any(y => y is HeartProto)).Select(x => x.FullDescription()).ListToString()}!"
				: $"You feel a tightness in your {charOwner.Body.Bodyparts.Where(x => x.Organs.Any(y => y is HeartProto)).Select(x => x.FullDescription()).ListToString()}{(charOwner.CanBreathe ? " and you find yourself short of breath" : "")}!");
			hypoxia += charOwner.Gameworld.GetStaticDouble("HypoxiaPerHeartFactor") *
			           (1.0 - heartFactor / HeartAttackThreshold);
		}

		// Hypoxia from Blood Loss
		var bloodRatio = charOwner.Body.CurrentBloodVolumeLitres / totalBlood;
		var oxygenHypoxia = 0.0;
		if (bloodRatio < BloodLossHypoxiaThreshold)
		{
			oxygenHypoxia =
				Math.Pow(BloodLossHypoxiaThreshold - bloodRatio,
					charOwner.Gameworld.GetStaticDouble("HypoxiaPowerFromBloodLoss")) *
				charOwner.Gameworld.GetStaticDouble("HypoxiaDamageFromBloodLoss");
		}

		// Hypoxia from blood replacements
		var bloodReplacementToBloodRatio =
			(charOwner.Body.EffectsOfType<IHarmfulBloodAdditiveEffect>()
			          .FirstOrDefault(x => x.Applies() && x.Consequence == LiquidInjectionConsequence.BloodVolume)?
			          .Volume ?? 0) / totalBlood;
		var hydrateToBloodRatio =
			(charOwner.Body.EffectsOfType<IHarmfulBloodAdditiveEffect>()
			          .FirstOrDefault(x => x.Applies() && x.Consequence == LiquidInjectionConsequence.Hydrating)?
			          .Volume ?? 0) / totalBlood;

		if (bloodReplacementToBloodRatio > BloodReplacementHypoxiaThreshold)
		{
			hypoxia += (bloodReplacementToBloodRatio - BloodReplacementHypoxiaThreshold) *
			           charOwner.Gameworld.GetStaticDouble("HypoxiaFromBloodReplacementPerVolume");
		}

		if (hydrateToBloodRatio > HydratingHypoxiaThreshold)
		{
			hypoxia += (hydrateToBloodRatio - HydratingHypoxiaThreshold) *
			           charOwner.Gameworld.GetStaticDouble("HypoxiaFromHydratePerVolume");
		}

		if (charOwner.NeedsToBreathe && !charOwner.CanBreathe && charOwner.HeldBreathPercentage <= 0.0)
		{
			if (!charOwner.Body.Implants
			              .SelectNotNull(x => x.Parent.GetItemType<ICannula>())
			              .Any(x => x.ConnectedItems.Any(y =>
				              y.Item2.Parent.GetItemType<IExternalBloodOxygenator>()?.ProvidingOxygenation == true)))
			{
				if (!heartAttack)
				{
					charOwner.OutputHandler.Send("You can't breathe! You are suffocating!");
				}

				oxygenHypoxia += charOwner.Gameworld.GetStaticDouble("HypoxiaDamagePerTick");
			}
		}

		var multiplier =
			charOwner.Merits.OfType<IHypoxiaReducingMerit>().Where(x => x.Applies(charOwner))
			         .Aggregate(1.0, (accum, merit) => accum * merit.HypoxiaReductionFactor) *
			(charOwner.Body.EffectsOfType<CPRTarget>().Any() && bloodRatio > CprMinimumBloodRatio
				? CprHypoxiaMultiplier
				: 1.0);

		if (multiplier > 0.0 && (oxygenHypoxia > 0 || hypoxia > 0))
		{
			foreach (var organ in charOwner.Body.Organs)
			{
				charOwner.SufferDamage(new Damage
				{
					ActorOrigin = charOwner,
					DamageAmount = multiplier * (organ.HypoxiaDamagePerTick * oxygenHypoxia + hypoxia),
					DamageType = DamageType.Hypoxia,
					Bodypart = organ,
					PainAmount = multiplier * (organ.HypoxiaDamagePerTick * oxygenHypoxia + hypoxia),
					PenetrationOutcome = Outcome.NotTested
				});
			}
		}

		var dieoffFactor =
			charOwner.Gameworld.GetStaticDouble("TissueDieOffMultiplierPerVolumeContaminant");
		var tissueDieOff =
			charOwner.Body.EffectsOfType<IHarmfulBloodAdditiveEffect>().Where(x => x.Applies()).Select(x =>
			{
				switch (x.Consequence)
				{
					case LiquidInjectionConsequence.BloodReplacement:
					case LiquidInjectionConsequence.Harmful:
						return TissueDieOffHarmfulMultiplier * x.Volume * dieoffFactor;
					case LiquidInjectionConsequence.Deadly:
						return TissueDieOffDeadlyMultiplier * x.Volume * dieoffFactor;
					case LiquidInjectionConsequence.KidneyWaste:
						return x.Volume > TissueDieOffKidneyWasteThreshold
							? TissueDieOffKidneyWasteMultiplier * (x.Volume - TissueDieOffKidneyWasteThreshold) *
							  dieoffFactor
							: 0.0;
					default:
						return 0.0;
				}
			}).DefaultIfEmpty(0).Sum();

		if (tissueDieOff > 0)
		{
			foreach (var organ in charOwner.Body.Organs)
			{
				if (organ is BrainProto)
				{
					continue;
				}

				charOwner.Body.SufferDamage(new Damage
				{
					ActorOrigin = charOwner,
					DamageAmount = tissueDieOff,
					DamageType = DamageType.Cellular,
					Bodypart = organ,
					PainAmount = tissueDieOff,
					PenetrationOutcome = Outcome.NotTested
				});
			}
		}

		return EvaluateStatus(thing);
	}

	private static string ColourRatio(string text, double ratio, string replacement)
	{
		ANSIColour colour;
		var boldColour = false;
		if (ratio >= 1.0)
		{
			colour = Telnet.Green;
		}
		else if (ratio > 0.8)
		{
			colour = Telnet.Green;
			boldColour = true;
		}
		else if (ratio > 0.4)
		{
			colour = Telnet.Yellow;
			boldColour = true;
		}
		else if (ratio > 0.2)
		{
			colour = Telnet.Red;
			boldColour = true;
		}
		else
		{
			colour = Telnet.Red;
		}

		return string.Format(text,
			boldColour ? colour.Bold + replacement + Telnet.RESETALL : colour.Colour + replacement + Telnet.RESETALL);
	}

	public override string ReportConditionPrompt(IHaveWounds owner, PromptType type)
	{
		var charOwner = (ICharacter)owner;
		var stunRatio = owner.Wounds.Sum(x => x.CurrentStun) /
		                MaximumStunExpression.Evaluate(charOwner);
		var painRatio = owner.Wounds.Sum(x => x.CurrentPain) /
		                MaximumPainExpression.Evaluate(charOwner);
		var bloodlossRatio = charOwner.Body.CurrentBloodVolumeLitres / charOwner.Body.TotalBloodVolumeLitres;
		var totalBreath = charOwner.Body.HeldBreathPercentage;

		switch (type)
		{
			case PromptType.Classic:
				return ReportClassicPrompt(charOwner, stunRatio, painRatio, bloodlossRatio, totalBreath);
			case PromptType.Full:
				return ReportFullPrompt(charOwner, stunRatio, painRatio, bloodlossRatio, totalBreath, false);
			case PromptType.FullBrief:
				return ReportFullPrompt(charOwner, stunRatio, painRatio, bloodlossRatio, totalBreath, true);
		}

		return ">";
	}

	private string ReportFullPrompt(ICharacter charOwner, double stunRatio, double painRatio, double bloodlossRatio,
		double breathRatio, bool brief)
	{
		var sb = new StringBuilder();
		sb.Append("<");
		var heartFactor =
			charOwner.Body.OrganFunction<HeartProto>();
		if (heartFactor <= 0)
		{
			sb.Append("*** YOU ARE IN CARDIAC ARREST! ***".Colour(Telnet.BoldRed));
			sb.Append(">\n<");
		}
		else if (heartFactor < HeartAttackThreshold)
		{
			sb.Append("*** YOU ARE HAVING A HEART ATTACK! ***".Colour(Telnet.BoldRed));
			sb.Append(">\n<");
		}
		else
		{
			if (charOwner.NeedsToBreathe && !charOwner.CanBreathe)
			{
				if (breathRatio > 0.0 && breathRatio < 1.0)
				{
					sb.Append("*** YOU ARE HOLDING YOUR BREATH! ***".Colour(Telnet.KeywordBlue));
					sb.Append(">\n<");
				}
				else if (charOwner.BreathingFluid is ILiquid)
				{
					sb.Append("*** YOU ARE DROWNING! ***".Colour(Telnet.BoldCyan));
					sb.Append(">\n<");
				}
				else
				{
					sb.Append("*** YOU ARE SUFFOCATING! ***".Colour(Telnet.BoldYellow));
					sb.Append(">\n<");
				}
			}
		}

		if (painRatio < 0.01)
		{
			if (!brief)
			{
				sb.Append($"You are currently in {"no pain".Colour(Telnet.Green)}");
			}
		}
		else if (painRatio < 0.2)
		{
			sb.Append($"You are currently in {"mild pain".Colour(Telnet.Yellow)}");
		}
		else if (painRatio < 0.4)
		{
			sb.Append($"You are currently in {"moderate pain".Colour(Telnet.Yellow)}");
		}
		else if (painRatio < 0.6)
		{
			sb.Append($"You are currently in {"severe pain".Colour(Telnet.Red)}");
		}
		else if (painRatio < 0.8)
		{
			sb.Append($"You are currently in {"very severe pain".Colour(Telnet.Red)}");
		}
		else if (painRatio < 1.0)
		{
			sb.Append($"You are currently in {"agonising pain".Colour(Telnet.BoldRed)}");
		}
		else
		{
			sb.Append($"You are currently in {"overwhelming pain".Colour(Telnet.BoldRed)}");
		}

		if (stunRatio > 0.05)
		{
			if (stunRatio < 0.2)
			{
				sb.Append($", {"dizzy".Colour(Telnet.BoldCyan)}");
			}
			else if (stunRatio < 0.4)
			{
				sb.Append($", {"dazed".Colour(Telnet.BoldCyan)}");
			}
			else if (stunRatio < 0.6)
			{
				sb.Append($", {"stunned".Colour(Telnet.BoldBlue)}");
			}
			else if (stunRatio < 0.8)
			{
				sb.Append($", {"severly stunned".Colour(Telnet.BoldBlue)}");
			}
			else if (stunRatio < 1.0)
			{
				sb.Append($", {"practically catatonic".Colour(Telnet.BoldMagenta)}");
			}
			else
			{
				sb.Append($", {"knocked out".Colour(Telnet.BoldMagenta)}");
			}
		}

		if (bloodlossRatio <= 0.98 && sb.Length != 0)
		{
			sb.Append(" and have ");
		}

		switch (bloodlossRatio)
		{
			case > 0.98:
				break;
			case >= 0.94:
				sb.Append($"{"very minor blood loss".Colour(Telnet.Yellow)}");
				break;
			case >= 0.90:
				sb.Append($"{"minor blood loss".Colour(Telnet.Yellow)}");
				break;
			case >= 0.825:
				sb.Append($"{"moderate blood loss".Colour(Telnet.Red)}");
				break;
			case >= 0.75:
				sb.Append($"{"major blood loss".Colour(Telnet.Red)}");
				break;
			case >= 0.675:
				sb.Append($"{"severe blood loss".Colour(Telnet.Red)}");
				break;
			case >= 0.6:
				sb.Append($"{"very severe blood loss".Colour(Telnet.Red)}");
				break;
			case >= 0.5:
				sb.Append($"{"critical blood loss".Colour(Telnet.Red)}");
				break;
			default:
				sb.Append($"{"life-threatening blood loss".Colour(Telnet.Red)}");
				break;
		}

		sb.Append(">");

		return sb.ToString();
	}

	private string ReportClassicPrompt(ICharacter charOwner, double stunRatio, double painRatio, double bloodlossRatio,
		double breathRatio)
	{
		var sb = new StringBuilder();
		sb.Append("Pain: ");
		switch (painRatio)
		{
			case <= 0.0:
				sb.Append($"{Telnet.Red.Colour}**{Telnet.Yellow.Colour}**{Telnet.Green.Colour}**{Telnet.RESET}");
				break;
			case <= 0.1667:
				sb.Append($"{Telnet.Red.Colour}**{Telnet.Yellow.Colour}**{Telnet.Green.Colour}* {Telnet.RESET}");
				break;
			case <= 0.3333:
				sb.Append($"{Telnet.Red.Colour}**{Telnet.Yellow.Colour}**  {Telnet.RESET}");
				break;
			case <= 0.5:
				sb.Append($"{Telnet.Red.Colour}**{Telnet.Yellow.Colour}*   {Telnet.RESET}");
				break;
			case <= 0.6667:
				sb.Append($"{Telnet.Red.Colour}**    {Telnet.RESET}");
				break;
			case <= 0.8335:
				sb.Append($"{Telnet.Red.Colour}*     {Telnet.RESET}");
				break;
			case >= 1.0:
				sb.Append($"      ");
				break;
		}

		sb.Append(" / Stun: ");
		switch (stunRatio)
		{
			case <= 0.0:
				sb.Append($"{Telnet.Blue.Colour}**{Telnet.Cyan.Colour}**{Telnet.BoldCyan.Colour}**{Telnet.RESETALL}");
				break;
			case <= 0.1667:
				sb.Append($"{Telnet.Blue.Colour}**{Telnet.Cyan.Colour}**{Telnet.BoldCyan.Colour}* {Telnet.RESETALL}");
				break;
			case <= 0.3333:
				sb.Append($"{Telnet.Blue.Colour}**{Telnet.Cyan.Colour}**  {Telnet.RESET}");
				break;
			case <= 0.5:
				sb.Append($"{Telnet.Blue.Colour}**{Telnet.Cyan.Colour}*   {Telnet.RESET}");
				break;
			case <= 0.6667:
				sb.Append($"{Telnet.Blue.Colour}**    {Telnet.RESET}");
				break;
			case <= 0.8335:
				sb.Append($"{Telnet.Blue.Colour}*     {Telnet.RESET}");
				break;
			case >= 1.0:
				sb.Append($"      ");
				break;
		}

		var stamRatio = 1.0 - charOwner.CurrentStamina / charOwner.MaximumStamina;
		sb.Append(" / Stam: ");
		switch (stamRatio)
		{
			case <= 0.0:
				sb.Append($"{Telnet.Red.Colour}||{Telnet.Yellow.Colour}||{Telnet.Green.Colour}||{Telnet.RESET}");
				break;
			case <= 0.1667:
				sb.Append($"{Telnet.Red.Colour}||{Telnet.Yellow.Colour}||{Telnet.Green.Colour}| {Telnet.RESET}");
				break;
			case <= 0.3333:
				sb.Append($"{Telnet.Red.Colour}||{Telnet.Yellow.Colour}||  {Telnet.RESET}");
				break;
			case <= 0.5:
				sb.Append($"{Telnet.Red.Colour}||{Telnet.Yellow.Colour}|   {Telnet.RESET}");
				break;
			case <= 0.6667:
				sb.Append($"{Telnet.Red.Colour}||    {Telnet.RESET}");
				break;
			case <= 0.8335:
				sb.Append($"{Telnet.Red.Colour}|     {Telnet.RESET}");
				break;
			case >= 1.0:
				sb.Append($"      ");
				break;
		}

		sb.Append(" / Blood: ");
		switch (bloodlossRatio)
		{
			case >= 0.94:
				sb.Append($"{Telnet.Red.Colour}**{Telnet.Red.Bold}**{Telnet.White.Bold}**{Telnet.RESETALL}");
				break;
			case >= 0.9:
				sb.Append($"{Telnet.Red.Colour}**{Telnet.Red.Colour}**{Telnet.White.Colour}* {Telnet.RESETALL}");
				break;
			case >= 0.825:
				sb.Append($"{Telnet.Red.Colour}**{Telnet.Red.Colour}**  {Telnet.RESETALL}");
				break;
			case >= 0.75:
				sb.Append($"{Telnet.Red.Colour}**{Telnet.Red.Colour}*   {Telnet.RESETALL}");
				break;
			case >= 0.675:
				sb.Append($"{Telnet.Red.Colour}**    {Telnet.RESET}");
				break;
			case >= 0.6:
				sb.Append($"{Telnet.Red.Colour}*     {Telnet.RESET}");
				break;
			default:
				sb.Append($"      ");
				break;
		}

		if (breathRatio > 1.0)
		{
			var breathRatioReversed = 1.0 - breathRatio;
			sb.Append(" / Breath: ");
			switch (breathRatioReversed)
			{
				case <= 0.0:
					sb.Append($"      ");
					break;
				case <= 0.1667:
					sb.Append($"{Telnet.Blue.Colour}|     {Telnet.RESET}");
					break;
				case <= 0.3333:
					sb.Append($"{Telnet.Blue.Colour}||    {Telnet.RESET}");
					break;
				case <= 0.5:
					sb.Append($"{Telnet.Blue.Colour}||{Telnet.Cyan.Colour}|   {Telnet.RESET}");
					break;
				case <= 0.6667:
					sb.Append($"{Telnet.Blue.Colour}||{Telnet.Cyan.Colour}||  {Telnet.RESET}");
					break;
				case <= 0.8335:
					sb.Append($"{Telnet.Blue.Colour}||{Telnet.Cyan.Colour}||{Telnet.BoldCyan.Colour}| {Telnet.RESET}");
					break;
				default:
					sb.Append($"{Telnet.Blue.Colour}||{Telnet.Cyan.Colour}||{Telnet.BoldCyan.Colour}||{Telnet.RESET}");
					break;
			}
		}

		return sb.ToString();
	}

	public override double GetHealingTickAmount(IWound wound, Outcome outcome, HealthDamageType type)
	{
		ITraitExpression whichExpression;
		switch (type)
		{
			case HealthDamageType.Damage:
				whichExpression = HealingTickDamageExpression;
				break;
			case HealthDamageType.Pain:
				whichExpression = HealingTickPainExpression;
				break;
			case HealthDamageType.Shock:
				return 0;
			case HealthDamageType.Stun:
				whichExpression = HealingTickStunExpression;
				break;
			default:
				return 0;
		}

		whichExpression.Formula.Parameters["originaldamage"] = wound.OriginalDamage;
		whichExpression.Formula.Parameters["damage"] = wound.CurrentDamage;
		whichExpression.Formula.Parameters["pain"] = wound.CurrentPain;
		whichExpression.Formula.Parameters["stun"] = wound.CurrentStun;
		whichExpression.Formula.Parameters["shock"] = wound.CurrentShock;
		whichExpression.Formula.Parameters["outcome"] = outcome.SuccessDegrees();
		return whichExpression.Evaluate((ICharacter)wound.Parent);
	}

	#region Overrides of BaseHealthStrategy

	public override BodyTemperatureStatus CurrentTemperatureStatus(IHaveWounds owner)
	{
		var charOwner = (ICharacter)owner;
		return charOwner.CombinedEffectsOfType<ThermalImbalance>().FirstOrDefault()?.TemperatureStatus ??
		       BodyTemperatureStatus.NormalTemperature;
	}

	public override double WoundPenaltyFor(IHaveWounds owner)
	{
		if (owner is not ICharacter charOwner)
		{
			return 0;
		}

		var penalty =
			charOwner.Wounds.Sum(
				x =>
					x.CurrentDamage / PercentageHealthPerPenalty + x.CurrentPain / PercentagePainPerPenalty +
					x.CurrentStun / PercentageStunPerPenalty) /
			(MaximumHitPointsExpression.Evaluate(charOwner) + MaximumStunExpression.Evaluate(charOwner) +
			 MaximumPainExpression.Evaluate(charOwner));
		return -1 * penalty;
	}

	public override void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture)
	{
		var cOwner = (ICharacter)owner;
		foreach (var liquid in mixture.Instances)
		{
			if (liquid.Liquid.Drug != null)
			{
				cOwner.Body.Dose(liquid.Liquid.Drug, DrugVector.Injected,
					liquid.Amount * liquid.Liquid.DrugGramsPerUnitVolume);
			}

			if (liquid.Liquid.InjectionConsequence == LiquidInjectionConsequence.Benign)
			{
				continue;
			}

			switch (liquid.Liquid.InjectionConsequence)
			{
				case LiquidInjectionConsequence.Hydrating:
					cOwner.Body.FulfilNeeds(new NeedFulfiller
					{
						ThirstPoints = liquid.Liquid.DrinkSatiatedHoursPerLitre /
						               (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres),
						WaterLitres = liquid.Liquid.WaterLitresPerLitre /
						              (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres),
						AlcoholLitres = liquid.Liquid.AlcoholLitresPerLitre /
						                (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres),
						SatiationPoints = liquid.Liquid.FoodSatiatedHoursPerLitre /
						                  (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres)
					});
					continue;
				case LiquidInjectionConsequence.BloodReplacement:
					if (!(liquid is BloodLiquidInstance blood))
					{
						continue;
					}

					cOwner.Body.CurrentBloodVolumeLitres +=
						liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres;

					if (cOwner.Body.Bloodtype?.IsCompatibleWithDonorBlood(blood.BloodType) != false)
					{
						continue;
					}

					break;
			}

			var effect =
				cOwner.Body
				      .EffectsOfType<IHarmfulBloodAdditiveEffect>()
				      .FirstOrDefault(x => x.Consequence == liquid.Liquid.InjectionConsequence);
			if (effect == null)
			{
				effect = new HarmfulBloodAdditive(cOwner.Body, liquid.Liquid.InjectionConsequence, 0);
				cOwner.Body.AddEffect(effect);
			}

			effect.Volume += liquid.Amount;
		}
	}

	public override bool IsCriticallyInjured(IHaveWounds owner)
	{
		var ch = (ICharacter)owner;
		if (!ch.CanBreathe && ch.NeedsToBreathe)
		{
			return true;
		}

		if (ch.Body.CurrentBloodVolumeLitres / ch.Body.TotalBloodVolumeLitres < CriticalInjuryBloodRatioThreshold)
		{
			return true;
		}

		if (ch.Body.Wounds.Any(x =>
			    x.Bodypart is IOrganProto op &&
			    op.IsVital &&
			    x.CurrentDamage > CriticalInjuryVitalOrganDamageThreshold))
		{
			return true;
		}

		// TODO - how best to determine this in a lightweight fashion with more criteria? Should it be an effect or a flag maybe?

		return false;
	}

	public override bool KidneyFunctionActive => true;

	public override double MaxHP(IHaveWounds owner)
	{
		return owner is not ICharacter charOwner ? 0.0 : MaximumHitPointsExpression.Evaluate(charOwner);
	}

	public override double MaxStun(IHaveWounds owner)
	{
		return owner is not ICharacter charOwner ? 0.0 : MaximumStunExpression.Evaluate(charOwner);
	}

	public override double MaxPain(IHaveWounds owner)
	{
		return owner is not ICharacter charOwner ? 0.0 : MaximumPainExpression.Evaluate(charOwner);
	}

	public override HealthTickResult EvaluateStatus(IHaveWounds thing)
	{
		var charOwner = (ICharacter)thing;
		if (charOwner.State.HasFlag(CharacterState.Dead))
		{
			return HealthTickResult.Dead;
		}

		var brainActivity = charOwner.Body.OrganFunction<BrainProto>();
		if (brainActivity <= 0)
		{
			return HealthTickResult.Dead;
		}

		if (charOwner.Body.EffectsOfType<ILossOfConsciousnessEffect>().Any(x => x.Applies()))
		{
			return charOwner.Body.EffectsOfType<ILossOfConsciousnessEffect>().First(x => x.Applies()).UnconType;
		}

		if (brainActivity < thing.Gameworld.GetStaticDouble("BrainDamageUnconsciousnessThreshold"))
		{
			return HealthTickResult.Unconscious;
		}

		if (
			charOwner.Body.OrganFunction<HeartProto>() <= 0.0)
		{
			return HealthTickResult.Unconscious;
		}

		var maxStun = MaximumStunExpression.Evaluate(charOwner);
		var painRatio = charOwner.Wounds.Sum(x => x.CurrentPain) /
		                MaximumPainExpression.Evaluate(charOwner);
		if (painRatio >= PainPassOutThreshold && !charOwner.Body.EffectsOfType<IPreventPassOut>().Any(x => x.Applies()))
		{
			return HealthTickResult.PassOut;
		}

		if (thing.Wounds.Sum(x => x.CurrentStun) >= maxStun &&
		    !charOwner.Body.EffectsOfType<IPreventPassOut>().Any(x => x.Applies()))
		{
			return HealthTickResult.Unconscious;
		}

		var anasthesia =
			charOwner.Body.EffectsOfType<Anesthesia>().Select(x => x.IntensityPerGramMass).DefaultIfEmpty(0).Sum();

		if (anasthesia >= AnesthesiaUnconsciousThreshold)
		{
			return HealthTickResult.Unconscious;
		}

		return HealthTickResult.None;
	}

	private void RemoveContaminants(IBody body, List<IHarmfulBloodAdditiveEffect> contaminants)
	{
		// try hydrating contaminants first
		var hydrate = contaminants.FirstOrDefault(x => x.Consequence == LiquidInjectionConsequence.Hydrating);
		if (hydrate != null)
		{
			hydrate.Volume -= body.TotalBloodVolumeLitres * ContaminantHydratingRemovalFraction;
			if (hydrate.Volume <= 0)
			{
				body.RemoveEffect(hydrate);
			}

			return;
		}

		var blood = contaminants.FirstOrDefault(x => x.Consequence == LiquidInjectionConsequence.BloodVolume);
		if (blood != null)
		{
			blood.Volume -= body.TotalBloodVolumeLitres * ContaminantBloodRemovalFraction;
			if (blood.Volume <= 0)
			{
				body.RemoveEffect(blood);
			}
		}
	}

	public override void PerformBloodGain(IHaveWounds owner)
	{
		// Blood gain requires that the patient be well hydrated, and also has an impact on hydration
		var cOwner = (ICharacter)owner;
		var body = cOwner.Body;
		var totalBlood = body.TotalBloodVolumeLitres;
		if (body.CurrentBloodVolumeLitres / totalBlood < MinimumBloodRatioForRegeneration)
		{
			return;
		}

		var contaminants =
			body.EffectsOfType<IHarmfulBloodAdditiveEffect>()
			    .Where(
				    x =>
					    x.Applies() &&
					    (x.Consequence == LiquidInjectionConsequence.BloodVolume ||
					     x.Consequence == LiquidInjectionConsequence.Hydrating))
			    .ToList();

		if (body.CurrentBloodVolumeLitres < totalBlood || contaminants.Any())
		{
			double bloodChange = 0;
			if (!body.NeedsModel.Status.HasFlag(NeedsResult.Parched) &&
			    !body.NeedsModel.Status.HasFlag(NeedsResult.Thirsty))
			{
				if (body.CurrentBloodVolumeLitres >= totalBlood)
				{
					RemoveContaminants(body, contaminants);
					bloodChange = totalBlood * BloodCleanseFraction;
				}
				else
				{
					var recoveryAmount = BloodRegenerationFraction *
					                     (cOwner.State.HasFlag(CharacterState.Sleeping)
						                     ? SleepingBloodRegenerationMultiplier
						                     : 1.0);

					var newBlood = Math.Min(body.CurrentBloodVolumeLitres + recoveryAmount * totalBlood, totalBlood);
					bloodChange = newBlood - body.CurrentBloodVolumeLitres;
					body.CurrentBloodVolumeLitres = newBlood;
				}
			}

			body.FulfilNeeds(new NeedFulfiller
			{
				ThirstPoints = BloodGainThirstPointsPerTick,
				WaterLitres = BloodGainWaterLitresPerBloodChange * bloodChange
			});
		}
	}

	public override void PerformKidneyFunction(IBody owner)
	{
		if (KidneyFunctionActive)
		{
			return;
		}

		var contaminant =
			owner.EffectsOfType<IHarmfulBloodAdditiveEffect>()
			     .FirstOrDefault(x => x.Consequence == LiquidInjectionConsequence.KidneyWaste);
		var kidneyFunction = owner.OrganFunction<KidneyProto>();
		if (kidneyFunction < KidneyWasteAccumulationThreshold)
		{
			var accumulation = KidneyWasteAccumulationMultiplier *
			                   (1.0 - kidneyFunction / KidneyWasteAccumulationThreshold);
			if (contaminant == null)
			{
				contaminant = new HarmfulBloodAdditive(owner, LiquidInjectionConsequence.KidneyWaste,
					accumulation);
				owner.AddEffect(contaminant);
				return;
			}

			contaminant.Volume += accumulation;
			return;
		}

		if (contaminant != null)
		{
			owner.RemoveEffect(contaminant);
		}
	}

	public override void PerformLiverFunction(IBody owner)
	{
		var contaminant =
			owner.EffectsOfType<IHarmfulBloodAdditiveEffect>()
			     .Where(x => x.Consequence.In(LiquidInjectionConsequence.Harmful, LiquidInjectionConsequence.Deadly,
				     LiquidInjectionConsequence.BloodReplacement))
			     .FirstMax(x => x.Volume);
		if (contaminant != null)
		{
			contaminant.Volume -= owner.LiverAlcoholRemovalKilogramsPerHour * 60000.0 / 3600.0;
			if (contaminant.Volume <= 0)
			{
				owner.RemoveEffect(contaminant);
			}
		}
	}

	public override void PerformSpleenFunction(IBody owner)
	{
		var spleenFunction = owner.OrganFunction<SpleenProto>();
		if (spleenFunction < SpleenFunctionThreshold)
		{
			return;
		}

		var contaminant =
			owner.EffectsOfType<IHarmfulBloodAdditiveEffect>()
			     .Where(x => x.Consequence.In(LiquidInjectionConsequence.Harmful, LiquidInjectionConsequence.Deadly,
				     LiquidInjectionConsequence.BloodReplacement))
			     .FirstMax(x => x.Volume);
		if (contaminant != null)
		{
			var multiplier = contaminant.Consequence == LiquidInjectionConsequence.BloodReplacement
				? SpleenBloodReplacementCleanseMultiplier
				: SpleenHarmfulCleanseMultiplier;
			contaminant.Volume -= owner.CurrentBloodVolumeLitres / owner.Gameworld.UnitManager.BaseFluidToLitres /
			                      (1440 * multiplier);
			if (contaminant.Volume <= 0)
			{
				owner.RemoveEffect(contaminant);
			}
		}
	}

	#endregion
}
