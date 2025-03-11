using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.CommunicationStrategies;
using MudSharp.Body.Needs;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Health.Breathing;
using MudSharp.Models;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;
using MudSharp.Strategies.BodyStratagies;
using MudSharp.Work.Butchering;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp.Character.Heritage;

public partial class Race : SaveableItem, IRace
{
	private static readonly Regex AllowedGendersRegex = new(@"(\d+)");

	private readonly List<Gender> _allowedGenders = new();

	private readonly List<ICharacteristicDefinition> _baseCharacteristics = new();

	private readonly List<IBodypart> _bodypartAdditions = new();
	private readonly List<IBodypart> _bodypartRemovals = new();

	private readonly List<IBodypart> _femaleOnlyAdditions = new();

	private readonly List<ICharacteristicDefinition> _femaleOnlyCharacteristics =
		new();

	private readonly List<IBodypart> _maleOnlyAdditions = new();

	private readonly List<ICharacteristicDefinition> _maleOnlyCharacteristics =
		new();

	private readonly List<ICharacteristicDefinition> _maleAndFemaleCharacteristics =
		new();

	public Alignment DefaultHandedness { get; set; }
	public IEnumerable<Alignment> HandednessOptions { get; set; }

	public IBloodModel BloodModel { get; set; }

	private readonly List<IChargenAdvice> _chargenAdvices = new();

	public IEnumerable<IChargenAdvice> ChargenAdvices => _chargenAdvices;

	public bool ToggleAdvice(IChargenAdvice advice)
	{
		Changed = true;
		if (_chargenAdvices.Contains(advice))
		{
			_chargenAdvices.Remove(advice);
			return false;
		}

		_chargenAdvices.Add(advice);
		return true;
	}

	public IRace Clone(string newName)
	{
		return new Race(this, newName);
	}

	public Race(IFuturemud gameworld, string name, IBodyPrototype body, IRace parentRace)
	{
		Gameworld = gameworld;
		_name = name;
		BaseBody = body;
		ParentRace = parentRace;
		Description = "An undescribed race.";
		AvailabilityProg = Gameworld.FutureProgs.GetByName("AlwaysFalse");
		if (ParentRace is not null)
		{
			DiceExpression = ParentRace.DiceExpression;
			_allowedGenders.AddRange(ParentRace.AllowedGenders);
			AttributeBonusProg = ParentRace.AttributeBonusProg;
			AttributeTotalCap = ParentRace.AttributeTotalCap;
			IndividualAttributeCap = ParentRace.IndividualAttributeCap;
			IlluminationPerceptionMultiplier = ParentRace.IlluminationPerceptionMultiplier;
			DefaultHealthStrategy = ParentRace.DefaultHealthStrategy;
			DefaultHandedness = ParentRace.DefaultHandedness;
			HandednessOptions = ParentRace.HandednessOptions.ToList();
			ButcheryProfile = ParentRace.ButcheryProfile;
			CorpseModel = ParentRace.CorpseModel;
			HungerRate = ParentRace.HungerRate;
			ThirstRate = ParentRace.ThirstRate;
			TrackIntensityOlfactory = ParentRace.TrackIntensityOlfactory;
			TrackIntensityVisual = ParentRace.TrackIntensityVisual;
			TrackingAbilityVisual = ParentRace.TrackingAbilityVisual;
			TrackingAbilityOlfactory = ParentRace.TrackingAbilityOlfactory;
			CombatSettings = new RacialCombatSettings
			{
				CanAttack = ParentRace.CombatSettings.CanAttack,
				CanDefend = ParentRace.CombatSettings.CanDefend,
				CanUseWeapons = ParentRace.CombatSettings.CanUseWeapons
			};

			NaturalArmourType = ParentRace.NaturalArmourType;
			NaturalArmourQuality = ParentRace.NaturalArmourQuality;
			NaturalArmourMaterial = ParentRace.NaturalArmourMaterial;
			_bloodLiquidId = ParentRace.BloodLiquid?.Id ?? 0;
			NeedsToBreathe = ParentRace.NeedsToBreathe;
			_breathingStrategy = ParentRace.BreathingStrategy;

			BreathingVolumeExpression =
				new TraitExpression(ParentRace.BreathingVolumeExpression.OriginalFormulaText, Gameworld);
			HoldBreathLengthExpression =
				new TraitExpression(ParentRace.HoldBreathLengthExpression.OriginalFormulaText, Gameworld);

			SweatRateInLitresPerMinute = ParentRace.SweatRateInLitresPerMinute;
			_sweatLiquidId = ParentRace.SweatLiquid?.Id ?? 0;

			SizeProne = ParentRace.SizeProne;
			SizeStanding = ParentRace.SizeStanding;
			SizeSitting = ParentRace.SizeSitting;
			CommunicationStrategy = ParentRace.CommunicationStrategy;
			_maximumDragWeightExpression =
				new TraitExpression(ParentRace.MaximumDragWeightExpression.OriginalFormulaText, Gameworld);
			_maximumLiftWeightExpression =
				new TraitExpression(ParentRace.MaximumLiftWeightExpression.OriginalFormulaText, Gameworld);
			
			RaceUsesStamina = ParentRace.RaceUsesStamina;
			_optInMaterialEdibility = ParentRace.OptInMaterialEdibility;
			EatCorpseEmoteText = ParentRace.EatCorpseEmoteText;
			CanEatCorpses = ParentRace.CanEatCorpses;
			BiteWeight = ParentRace.BiteWeight;
			TemperatureRangeCeiling = ParentRace.TemperatureRangeCeiling;
			TemperatureRangeFloor = ParentRace.TemperatureRangeFloor;
			_bodypartSizeModifier = ParentRace.BodypartSizeModifier;
			_bodypartDamageMultiplier = ParentRace.BodypartDamageMultiplier;
			CanSwim = ParentRace.CanSwim;
			CanClimb = ParentRace.CanClimb;
			MinimumSleepingPosition = ParentRace.MinimumSleepingPosition;
			Ages.Add(Heritage.AgeCategory.Baby, double.MinValue,
				ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.Child));
			Ages.Add(Heritage.AgeCategory.Child, ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.Child),
				ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.Youth));
			Ages.Add(Heritage.AgeCategory.Youth, ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.Youth),
				ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.YoungAdult));
			Ages.Add(Heritage.AgeCategory.YoungAdult, ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.YoungAdult),
				ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.Adult));
			Ages.Add(Heritage.AgeCategory.Adult, ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.Adult),
				ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.Elder));
			Ages.Add(Heritage.AgeCategory.Elder, ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.Elder),
				ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.Venerable));
			Ages.Add(Heritage.AgeCategory.Venerable, ParentRace.MinimumAgeForCategory(Heritage.AgeCategory.Venerable),
				double.MaxValue);
			IHeightWeightModel hwmodel;
			if ((hwmodel = ParentRace.DefaultHeightWeightModel(Gender.Male)) is not null)
			{
				_defaultHeightWeightModels[Gender.Male] = hwmodel;
			}

			if ((hwmodel = ParentRace.DefaultHeightWeightModel(Gender.Female)) is not null)
			{
				_defaultHeightWeightModels[Gender.Female] = hwmodel;
			}

			if ((hwmodel = ParentRace.DefaultHeightWeightModel(Gender.Neuter)) is not null)
			{
				_defaultHeightWeightModels[Gender.Neuter] = hwmodel;
			}

			if ((hwmodel = ParentRace.DefaultHeightWeightModel(Gender.NonBinary)) is not null)
			{
				_defaultHeightWeightModels[Gender.NonBinary] = hwmodel;
			}
		}
		else
		{
			DiceExpression = "3d6";
			_allowedGenders.Add(Gender.Male);
			_allowedGenders.Add(Gender.Female);
			AttributeBonusProg = Gameworld.FutureProgs.GetByName("AlwaysZero");
			_attributes.AddRange(Gameworld.Traits.OfType<IAttributeDefinition>());
			AttributeTotalCap = _attributes.Count * 13;
			IndividualAttributeCap = 18;
			IlluminationPerceptionMultiplier = 1.0;
			DefaultHealthStrategy =
				Gameworld.HealthStrategies.First(x => x.OwnerType == HealthStrategyOwnerType.Character);
			DefaultHandedness = Alignment.Right;
			HandednessOptions = new List<Alignment> { Alignment.Right, Alignment.Left };
			CorpseModel = Gameworld.CorpseModels.First();
			HungerRate = 1.0;
			ThirstRate = 1.0;
			TrackIntensityOlfactory = 1.0;
			TrackIntensityVisual = 1.0;
			TrackingAbilityVisual = 1.0;
			TrackingAbilityOlfactory = 0.0;
			CombatSettings = new RacialCombatSettings
			{
				CanAttack = true,
				CanDefend = true,
				CanUseWeapons = false
			};

			NaturalArmourQuality = ItemQuality.Standard;
			NeedsToBreathe = true;
			_breathingStrategy = new LungBreather();

			BreathingVolumeExpression =
				new TraitExpression(Gameworld.GetStaticConfiguration("DefaultBreatheVolumeExpression"), Gameworld);
			HoldBreathLengthExpression =
				new TraitExpression(Gameworld.GetStaticConfiguration("DefaultHoldBreathExpression"), Gameworld);
			SweatRateInLitresPerMinute = 0.8;

			SizeProne = SizeCategory.Normal;
			SizeStanding = SizeCategory.Normal;
			SizeSitting = SizeCategory.Normal;
			CommunicationStrategy = HumanoidCommunicationStrategy.Instance;
			_maximumDragWeightExpression =
				new TraitExpression(Gameworld.GetStaticConfiguration("DefaultDragWeightExpression"), Gameworld);
			_maximumLiftWeightExpression =
				new TraitExpression(Gameworld.GetStaticConfiguration("DefaultLiftWeightExpression"), Gameworld);
			RaceUsesStamina = true;
			_optInMaterialEdibility = false;
			EatCorpseEmoteText = "@ eat|eats {0}$1";
			CanEatCorpses = false;
			BiteWeight = 1000;
			TemperatureRangeCeiling = 40;
			TemperatureRangeFloor = 0;
			_bodypartSizeModifier = 0;
			_bodypartDamageMultiplier = 1.0;
			CanSwim = true;
			CanClimb = false;
			MinimumSleepingPosition = PositionLounging.Instance;
			Ages.Add(Heritage.AgeCategory.Baby, double.MinValue, 1);
			Ages.Add(Heritage.AgeCategory.Child, 1, 2);
			Ages.Add(Heritage.AgeCategory.Youth, 2, 3);
			Ages.Add(Heritage.AgeCategory.YoungAdult, 3, 4);
			Ages.Add(Heritage.AgeCategory.Adult, 4, 40);
			Ages.Add(Heritage.AgeCategory.Elder, 40, 75);
			Ages.Add(Heritage.AgeCategory.Venerable, 75, double.MaxValue);
		}

		using (new FMDB())
		{
			var dbitem = new Models.Race
			{
				Name = _name,
				Description = Description,
				BaseBodyId = BaseBody.Id,
				AllowedGenders = _allowedGenders.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues(" "),
				ParentRaceId = ParentRace?.Id,
				AttributeBonusProgId = AttributeBonusProg.Id,
				AttributeTotalCap = AttributeTotalCap,
				IndividualAttributeCap = IndividualAttributeCap,
				DiceExpression = DiceExpression,
				IlluminationPerceptionMultiplier = IlluminationPerceptionMultiplier,
				AvailabilityProgId = AvailabilityProg?.Id,
				CorpseModelId = CorpseModel.Id,
				DefaultHealthStrategyId = DefaultHealthStrategy.Id,
				CanUseWeapons = CombatSettings.CanUseWeapons,
				CanAttack = CombatSettings.CanAttack,
				CanDefend = CombatSettings.CanDefend,
				NaturalArmourTypeId = NaturalArmourType?.Id,
				NaturalArmourQuality = (int)NaturalArmourQuality,
				NaturalArmourMaterialId = NaturalArmourMaterial?.Id,
				BloodLiquidId = BloodLiquid?.Id,
				NeedsToBreathe = NeedsToBreathe,
				BreathingModel = BreathingStrategy.Name,
				SweatLiquidId = SweatLiquid?.Id,
				SweatRateInLitresPerMinute = SweatRateInLitresPerMinute,
				SizeStanding = (int)SizeStanding,
				SizeProne = (int)SizeProne,
				SizeSitting = (int)SizeSitting,
				HungerRate = HungerRate,
				ThirstRate = ThirstRate,
				TrackIntensityOlfactory = TrackIntensityOlfactory,
				TrackIntensityVisual = TrackIntensityVisual,
				TrackingAbilityOlfactory = TrackingAbilityOlfactory,
				TrackingAbilityVisual = TrackingAbilityVisual,
				CommunicationStrategyType = CommunicationStrategy.Name,
				DefaultHandedness = (int)DefaultHandedness,
				HandednessOptions = HandednessOptions.Select(x => ((int)x).ToString("F0"))
				                                     .ListToCommaSeparatedValues(" "),
				MaximumDragWeightExpression = MaximumDragWeightExpression.OriginalFormulaText,
				MaximumLiftWeightExpression = MaximumLiftWeightExpression.OriginalFormulaText,
				RaceButcheryProfileId = ButcheryProfile?.Id,
				BloodModelId = BloodModel?.Id,
				RaceUsesStamina = RaceUsesStamina,
				CanEatCorpses = CanEatCorpses,
				BiteWeight = BiteWeight,
				EatCorpseEmoteText = EatCorpseEmoteText,
				CanEatMaterialsOptIn = OptInMaterialEdibility,
				TemperatureRangeFloor = TemperatureRangeFloor,
				TemperatureRangeCeiling = TemperatureRangeCeiling,
				BodypartSizeModifier = BodypartSizeModifier,
				BodypartHealthMultiplier = BodypartDamageMultiplier,
				BreathingVolumeExpression = BreathingVolumeExpression.OriginalFormulaText,
				HoldBreathLengthExpression = HoldBreathLengthExpression.OriginalFormulaText,
				CanClimb = CanClimb,
				CanSwim = CanSwim,
				MinimumSleepingPosition = (int)MinimumSleepingPosition.Id,
				ChildAge = MinimumAgeForCategory(Heritage.AgeCategory.Child),
				YouthAge = MinimumAgeForCategory(Heritage.AgeCategory.Youth),
				YoungAdultAge = MinimumAgeForCategory(Heritage.AgeCategory.YoungAdult),
				AdultAge = MinimumAgeForCategory(Heritage.AgeCategory.Adult),
				ElderAge = MinimumAgeForCategory(Heritage.AgeCategory.Elder),
				VenerableAge = MinimumAgeForCategory(Heritage.AgeCategory.Venerable),
				DefaultHeightWeightModelMaleId = _defaultHeightWeightModels.ValueOrDefault(Gender.Male, null)?.Id,
				DefaultHeightWeightModelFemaleId = _defaultHeightWeightModels.ValueOrDefault(Gender.Female, null)?.Id,
				DefaultHeightWeightModelNeuterId = _defaultHeightWeightModels.ValueOrDefault(Gender.Neuter, null)?.Id,
				DefaultHeightWeightModelNonBinaryId =
					_defaultHeightWeightModels.ValueOrDefault(Gender.NonBinary, null)?.Id
			};
			FMDB.Context.Races.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Race(MudSharp.Models.Race race, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = race.Id;
		_name = race.Name;
		Description = race.Description;
		BaseBody = gameworld.BodyPrototypes.Get(race.BaseBodyId);
		DiceExpression = race.DiceExpression;
		AttributeTotalCap = race.AttributeTotalCap;
		IndividualAttributeCap = race.IndividualAttributeCap;
		AttributeBonusProg = gameworld.FutureProgs.Get(race.AttributeBonusProgId);
		IlluminationPerceptionMultiplier = race.IlluminationPerceptionMultiplier;
		AvailabilityProg = gameworld.FutureProgs.Get(race.AvailabilityProgId ?? 0);
		CorpseModel = gameworld.CorpseModels.Get(race.CorpseModelId);
		DefaultHealthStrategy = gameworld.HealthStrategies.Get(race.DefaultHealthStrategyId);
		DefaultHandedness = (Alignment)race.DefaultHandedness;
		HandednessOptions = race.HandednessOptions.Split(' ').Select(x => (Alignment)int.Parse((string)x)).ToList();
		ButcheryProfile = gameworld.RaceButcheryProfiles.Get(race.RaceButcheryProfileId ?? 0L);
		HungerRate = race.HungerRate;
		ThirstRate = race.ThirstRate;
		TrackIntensityOlfactory = race.TrackIntensityOlfactory;
		TrackIntensityVisual = race.TrackIntensityVisual;
		TrackingAbilityOlfactory = race.TrackingAbilityOlfactory;
		TrackingAbilityVisual = race.TrackingAbilityVisual;

		foreach (var item in race.ChargenAdvicesRaces)
		{
			_chargenAdvices.Add(Gameworld.ChargenAdvices.Get(item.ChargenAdviceId));
		}

		foreach (var item in race.RacesChargenResources)
		{
			_costs.Add(new ChargenResourceCost
			{
				Amount = item.Amount,
				RequirementOnly = item.RequirementOnly,
				Resource = gameworld.ChargenResources.Get(item.ChargenResourceId)
			});
		}

		foreach (Match match in AllowedGendersRegex.Matches(race.AllowedGenders))
		{
			if (Enum.IsDefined(typeof(Gender), Convert.ToInt16(match.Value)))
			{
				_allowedGenders.Add((Gender)Convert.ToInt16(match.Value));
			}
		}

		foreach (var bodypart in race.RacesAdditionalBodyparts)
		{
			var bp = BaseBody?.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Id == bodypart.BodypartId) ??
			         BodypartPrototype.LoadFromDatabase(bodypart.Bodypart, Gameworld);
			switch (bodypart.Usage)
			{
				case "male":
					_maleOnlyAdditions.Add(bp);
					break;
				case "female":
					_femaleOnlyAdditions.Add(bp);
					break;
				case "general":
					_bodypartAdditions.Add(bp);
					break;
				case "remove":
					_bodypartRemovals.Add(bp);
					break;
			}
		}

		foreach (var item in race.RacesAdditionalCharacteristics)
		{
			var characteristic = gameworld.Characteristics.Get(item.CharacteristicDefinitionId);
			switch (item.Usage)
			{
				case "base":
					_baseCharacteristics.Add(characteristic);
					break;
				case "male":
					_maleOnlyCharacteristics.Add(characteristic);
					_maleAndFemaleCharacteristics.Add(characteristic);
					break;
				case "female":
					_femaleOnlyCharacteristics.Add(characteristic);
					_maleAndFemaleCharacteristics.Add(characteristic);
					break;
			}
		}

		foreach (var item in race.RacesAttributes)
		{
			var attribute = (IAttributeDefinition)gameworld.Traits.Get(item.AttributeId);
			_attributes.Add(attribute);
			if (item.IsHealthAttribute)
			{
				_healthTraits.Add(attribute);
			}
		}

		foreach (var item in race.RacesWeaponAttacks)
		{
			_naturalWeaponAttacks.Add(new NaturalAttack
			{
				Attack = Gameworld.WeaponAttacks.Get(item.WeaponAttackId),
				Bodypart = Gameworld.BodypartPrototypes.Get(item.BodypartId),
				Quality = (ItemQuality)item.Quality
			});
		}

		foreach (var item in race.RacesCombatActions)
		{
			_auxiliaryCombatActions.Add(new AuxiliaryCombatAction(item.CombatAction, Gameworld));
		}

		CombatSettings = new RacialCombatSettings
		{
			CanAttack = race.CanAttack,
			CanDefend = race.CanDefend,
			CanUseWeapons = race.CanUseWeapons
		};

		NaturalArmourType = Gameworld.ArmourTypes.Get(race.NaturalArmourTypeId ?? 0);
		NaturalArmourQuality = (ItemQuality)race.NaturalArmourQuality;
		NaturalArmourMaterial = Gameworld.Materials.Get(race.NaturalArmourMaterialId ?? 0);
		_bloodLiquidId = race.BloodLiquidId ?? 0;
		BloodModel = Gameworld.BloodModels.Get(race.BloodModelId ?? 0);
		NeedsToBreathe = race.NeedsToBreathe;
		_breathingStrategy = GetBreathingStrategy(race.BreathingModel);
		foreach (var item in race.RacesBreathableLiquids)
		{
			_breathableFluids.Add(Gameworld.Liquids.Get(item.LiquidId));
			_fluidBreathingMultipliers.Add(Gameworld.Liquids.Get(item.LiquidId), item.Multiplier);
		}

		foreach (var item in race.RacesBreathableGases)
		{
			_breathableFluids.Add(Gameworld.Gases.Get(item.GasId));
			_fluidBreathingMultipliers.Add(Gameworld.Gases.Get(item.GasId), item.Multiplier);
		}
		
		foreach (var item in race.RacesRemoveBreathableLiquids)
		{
			_removeBreathableFluids.Add(Gameworld.Liquids.Get(item.LiquidId));
		}

		foreach (var item in race.RacesRemoveBreathableGases)
		{
			_removeBreathableFluids.Add(Gameworld.Gases.Get(item.GasId));
		}

		BreathingVolumeExpression = new TraitExpression(race.BreathingVolumeExpression, Gameworld);
		HoldBreathLengthExpression = new TraitExpression(race.HoldBreathLengthExpression, Gameworld);

		SweatRateInLitresPerMinute = race.SweatRateInLitresPerMinute;
		_sweatLiquidId = race.SweatLiquidId ?? 0;

		SizeProne = (SizeCategory)race.SizeProne;
		SizeStanding = (SizeCategory)race.SizeStanding;
		SizeSitting = (SizeCategory)race.SizeSitting;
		switch (race.CommunicationStrategyType)
		{
			case "humanoid":
				CommunicationStrategy = HumanoidCommunicationStrategy.Instance;
				break;
			case "non communicator":
				CommunicationStrategy = NonCommunicatorCommunicationStrategy.Instance;
				break;
			case "mouth only":
				CommunicationStrategy = MouthOnlyCommunicationStrategy.Instance;
				break;
			case "robot":
				CommunicationStrategy = RobotCommunicationStrategy.Instance;
				break;
			default:
				CommunicationStrategy = HumanoidCommunicationStrategy.Instance;
				break;
		}

		_maximumDragWeightExpression = new TraitExpression(race.MaximumDragWeightExpression, Gameworld);
		_maximumLiftWeightExpression = new TraitExpression(race.MaximumLiftWeightExpression, Gameworld);

		RaceUsesStamina = race.RaceUsesStamina;
		foreach (var yield in race.RaceEdibleForagableYields)
		{
			_edibleForagableYields.Add(new EdibleForagableYield
			{
				YieldType = yield.YieldType.ToLowerInvariant(),
				YieldPerBite = yield.BiteYield,
				CaloriesPerYield = yield.CaloriesPerYield,
				HungerPerYield = yield.HungerPerYield,
				ThirstPerYield = yield.ThirstPerYield,
				WaterPerYield = yield.WaterPerYield,
				AlcoholPerYield = yield.AlcoholPerYield,
				EmoteText = yield.EatEmote
			});
		}

		_optInMaterialEdibility = race.CanEatMaterialsOptIn;
		foreach (var material in race.RacesEdibleMaterials)
		{
			_edibleMaterials.Add(new EdibleMaterial
			{
				Material = Gameworld.Materials.Get(material.MaterialId),
				HungerPerKilogram = material.HungerPerKilogram,
				ThirstPerKilogram = material.ThirstPerKilogram,
				CaloriesPerKilogram = material.CaloriesPerKilogram,
				WaterPerKilogram = material.WaterPerKilogram,
				AlcoholPerKilogram = material.AlcoholPerKilogram
			});
		}

		EatCorpseEmoteText = race.EatCorpseEmoteText;
		CanEatCorpses = race.CanEatCorpses;
		BiteWeight = race.BiteWeight;
		TemperatureRangeCeiling = race.TemperatureRangeCeiling;
		TemperatureRangeFloor = race.TemperatureRangeFloor;
		_bodypartSizeModifier = race.BodypartSizeModifier;
		_bodypartDamageMultiplier = race.BodypartHealthMultiplier;
		CanSwim = race.CanSwim;
		CanClimb = race.CanClimb;
		MinimumSleepingPosition = PositionState.GetState(race.MinimumSleepingPosition);
		Ages.Add(Heritage.AgeCategory.Baby, double.MinValue, race.ChildAge);
		Ages.Add(Heritage.AgeCategory.Child, race.ChildAge, race.YouthAge);
		Ages.Add(Heritage.AgeCategory.Youth, race.YouthAge, race.YoungAdultAge);
		Ages.Add(Heritage.AgeCategory.YoungAdult, race.YoungAdultAge, race.AdultAge);
		Ages.Add(Heritage.AgeCategory.Adult, race.AdultAge, race.ElderAge);
		Ages.Add(Heritage.AgeCategory.Elder, race.ElderAge, race.VenerableAge);
		Ages.Add(Heritage.AgeCategory.Venerable, race.VenerableAge, double.MaxValue);
		IHeightWeightModel hwmodel;
		if ((hwmodel = Gameworld.HeightWeightModels.Get(race.DefaultHeightWeightModelMaleId ?? 0)) is not null)
		{
			_defaultHeightWeightModels[Gender.Male] = hwmodel;
		}

		if ((hwmodel = Gameworld.HeightWeightModels.Get(race.DefaultHeightWeightModelFemaleId ?? 0)) is not null)
		{
			_defaultHeightWeightModels[Gender.Female] = hwmodel;
		}

		if ((hwmodel = Gameworld.HeightWeightModels.Get(race.DefaultHeightWeightModelNeuterId ?? 0)) is not null)
		{
			_defaultHeightWeightModels[Gender.Neuter] = hwmodel;
		}

		if ((hwmodel = Gameworld.HeightWeightModels.Get(race.DefaultHeightWeightModelNonBinaryId ?? 0)) is not null)
		{
			_defaultHeightWeightModels[Gender.NonBinary] = hwmodel;
		}
	}

	public Race(Race rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		BaseBody = rhs.BaseBody;
		ParentRace = rhs.ParentRace;
		AvailabilityProg = rhs.AvailabilityProg;
		Description = rhs.Description;
		DiceExpression = rhs.DiceExpression;
		AttributeBonusProg = rhs.AttributeBonusProg;
		AttributeTotalCap = rhs.AttributeTotalCap;
		IndividualAttributeCap = rhs.IndividualAttributeCap;
		IlluminationPerceptionMultiplier = rhs.IlluminationPerceptionMultiplier;
		DefaultHealthStrategy = rhs.DefaultHealthStrategy;
		DefaultHandedness = rhs.DefaultHandedness;
		HandednessOptions = rhs.HandednessOptions.ToList();
		ButcheryProfile = rhs.ButcheryProfile;
		CorpseModel = rhs.CorpseModel;
		HungerRate = rhs.HungerRate;
		ThirstRate = rhs.ThirstRate;
		TrackIntensityOlfactory = rhs.TrackIntensityOlfactory;
		TrackIntensityVisual = rhs.TrackIntensityVisual;
		TrackingAbilityOlfactory = rhs.TrackingAbilityOlfactory;
		TrackingAbilityVisual = rhs.TrackingAbilityVisual;
		CombatSettings = new RacialCombatSettings
		{
			CanAttack = rhs.CombatSettings.CanAttack,
			CanDefend = rhs.CombatSettings.CanDefend,
			CanUseWeapons = rhs.CombatSettings.CanUseWeapons
		};

		NaturalArmourType = rhs.NaturalArmourType;
		NaturalArmourQuality = rhs.NaturalArmourQuality;
		NaturalArmourMaterial = rhs.NaturalArmourMaterial;
		_bloodLiquidId = rhs.BloodLiquid?.Id ?? 0;
		NeedsToBreathe = rhs.NeedsToBreathe;
		_breathingStrategy = rhs.BreathingStrategy;

		BreathingVolumeExpression =
			new TraitExpression(rhs.BreathingVolumeExpression.OriginalFormulaText, Gameworld);
		HoldBreathLengthExpression =
			new TraitExpression(rhs.HoldBreathLengthExpression.OriginalFormulaText, Gameworld);

		SweatRateInLitresPerMinute = rhs.SweatRateInLitresPerMinute;
		_sweatLiquidId = rhs.SweatLiquid?.Id ?? 0;

		SizeProne = rhs.SizeProne;
		SizeStanding = rhs.SizeStanding;
		SizeSitting = rhs.SizeSitting;
		CommunicationStrategy = rhs.CommunicationStrategy;
		_maximumDragWeightExpression =
			new TraitExpression(rhs.MaximumDragWeightExpression.OriginalFormulaText, Gameworld);
		_maximumLiftWeightExpression =
			new TraitExpression(rhs.MaximumLiftWeightExpression.OriginalFormulaText, Gameworld);

		RaceUsesStamina = rhs.RaceUsesStamina;
		_optInMaterialEdibility = rhs.OptInMaterialEdibility;
		EatCorpseEmoteText = rhs.EatCorpseEmoteText;
		CanEatCorpses = rhs.CanEatCorpses;
		BiteWeight = rhs.BiteWeight;
		TemperatureRangeCeiling = rhs.TemperatureRangeCeiling;
		TemperatureRangeFloor = rhs.TemperatureRangeFloor;
		_bodypartSizeModifier = rhs.BodypartSizeModifier;
		_bodypartDamageMultiplier = rhs.BodypartDamageMultiplier;
		CanSwim = rhs.CanSwim;
		CanClimb = rhs.CanClimb;
		MinimumSleepingPosition = rhs.MinimumSleepingPosition;
		Ages.Add(Heritage.AgeCategory.Baby, double.MinValue, rhs.MinimumAgeForCategory(Heritage.AgeCategory.Child));
		Ages.Add(Heritage.AgeCategory.Child, rhs.MinimumAgeForCategory(Heritage.AgeCategory.Child),
			rhs.MinimumAgeForCategory(Heritage.AgeCategory.Youth));
		Ages.Add(Heritage.AgeCategory.Youth, rhs.MinimumAgeForCategory(Heritage.AgeCategory.Youth),
			rhs.MinimumAgeForCategory(Heritage.AgeCategory.YoungAdult));
		Ages.Add(Heritage.AgeCategory.YoungAdult, rhs.MinimumAgeForCategory(Heritage.AgeCategory.YoungAdult),
			rhs.MinimumAgeForCategory(Heritage.AgeCategory.Adult));
		Ages.Add(Heritage.AgeCategory.Adult, rhs.MinimumAgeForCategory(Heritage.AgeCategory.Adult),
			rhs.MinimumAgeForCategory(Heritage.AgeCategory.Elder));
		Ages.Add(Heritage.AgeCategory.Elder, rhs.MinimumAgeForCategory(Heritage.AgeCategory.Elder),
			rhs.MinimumAgeForCategory(Heritage.AgeCategory.Venerable));
		Ages.Add(Heritage.AgeCategory.Venerable, rhs.MinimumAgeForCategory(Heritage.AgeCategory.Venerable),
			double.MaxValue);
		IHeightWeightModel hwmodel;
		if ((hwmodel = rhs.DefaultHeightWeightModel(Gender.Male)) is not null)
		{
			_defaultHeightWeightModels[Gender.Male] = hwmodel;
		}

		if ((hwmodel = rhs.DefaultHeightWeightModel(Gender.Female)) is not null)
		{
			_defaultHeightWeightModels[Gender.Female] = hwmodel;
		}

		if ((hwmodel = rhs.DefaultHeightWeightModel(Gender.Neuter)) is not null)
		{
			_defaultHeightWeightModels[Gender.Neuter] = hwmodel;
		}

		if ((hwmodel = rhs.DefaultHeightWeightModel(Gender.NonBinary)) is not null)
		{
			_defaultHeightWeightModels[Gender.NonBinary] = hwmodel;
		}

		_attributes.AddRange(rhs._attributes);
		_baseCharacteristics.AddRange(rhs._baseCharacteristics);
		_maleOnlyAdditions.AddRange(rhs._maleOnlyAdditions);
		_femaleOnlyAdditions.AddRange(rhs._femaleOnlyAdditions);
		_maleAndFemaleCharacteristics.AddRange(rhs._maleAndFemaleCharacteristics);
		_maleOnlyCharacteristics.AddRange(rhs._maleOnlyCharacteristics);
		_femaleOnlyCharacteristics.AddRange(rhs._femaleOnlyCharacteristics);
		_bodypartAdditions.AddRange(rhs._bodypartAdditions);
		_bodypartRemovals.AddRange(rhs._bodypartRemovals);
		_allowedGenders.AddRange(rhs._allowedGenders);
		_naturalWeaponAttacks.AddRange(rhs._naturalWeaponAttacks);
		_auxiliaryCombatActions.AddRange(rhs._auxiliaryCombatActions);
		_breathableFluids.AddRange(rhs._breathableFluids);
		_fluidBreathingMultipliers = rhs._fluidBreathingMultipliers.ToDictionary();
		_removeBreathableFluids.AddRange(rhs._removeBreathableFluids);
		_edibleForagableYields.AddRange(rhs._edibleForagableYields);
		_costs.AddRange(rhs._costs);
		_edibleMaterials.AddRange(rhs._edibleMaterials);
		_healthTraits.AddRange(rhs._healthTraits);

		using (new FMDB())
		{
			var dbitem = new Models.Race
			{
				Name = _name,
				Description = Description,
				BaseBodyId = BaseBody.Id,
				AllowedGenders = _allowedGenders.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues(" "),
				ParentRaceId = ParentRace?.Id,
				AttributeBonusProgId = AttributeBonusProg.Id,
				AttributeTotalCap = AttributeTotalCap,
				IndividualAttributeCap = IndividualAttributeCap,
				DiceExpression = DiceExpression,
				IlluminationPerceptionMultiplier = IlluminationPerceptionMultiplier,
				AvailabilityProgId = AvailabilityProg?.Id,
				CorpseModelId = CorpseModel.Id,
				DefaultHealthStrategyId = DefaultHealthStrategy.Id,
				CanUseWeapons = CombatSettings.CanUseWeapons,
				CanAttack = CombatSettings.CanAttack,
				CanDefend = CombatSettings.CanDefend,
				NaturalArmourTypeId = NaturalArmourType?.Id,
				NaturalArmourQuality = (int)NaturalArmourQuality,
				NaturalArmourMaterialId = NaturalArmourMaterial?.Id,
				BloodLiquidId = BloodLiquid?.Id,
				NeedsToBreathe = NeedsToBreathe,
				BreathingModel = BreathingStrategy.Name,
				SweatLiquidId = SweatLiquid?.Id,
				SweatRateInLitresPerMinute = SweatRateInLitresPerMinute,
				SizeStanding = (int)SizeStanding,
				SizeProne = (int)SizeProne,
				SizeSitting = (int)SizeSitting,
				HungerRate = HungerRate,
				ThirstRate = ThirstRate,
				CommunicationStrategyType = CommunicationStrategy.Name,
				DefaultHandedness = (int)DefaultHandedness,
				HandednessOptions = HandednessOptions.Select(x => ((int)x).ToString("F0"))
				                                     .ListToCommaSeparatedValues(" "),
				MaximumDragWeightExpression = MaximumDragWeightExpression.OriginalFormulaText,
				MaximumLiftWeightExpression = MaximumLiftWeightExpression.OriginalFormulaText,
				RaceButcheryProfileId = ButcheryProfile?.Id,
				BloodModelId = BloodModel?.Id,
				RaceUsesStamina = RaceUsesStamina,
				CanEatCorpses = CanEatCorpses,
				BiteWeight = BiteWeight,
				EatCorpseEmoteText = EatCorpseEmoteText,
				CanEatMaterialsOptIn = OptInMaterialEdibility,
				TemperatureRangeFloor = TemperatureRangeFloor,
				TemperatureRangeCeiling = TemperatureRangeCeiling,
				BodypartSizeModifier = BodypartSizeModifier,
				BodypartHealthMultiplier = BodypartDamageMultiplier,
				BreathingVolumeExpression = BreathingVolumeExpression.OriginalFormulaText,
				HoldBreathLengthExpression = HoldBreathLengthExpression.OriginalFormulaText,
				CanClimb = CanClimb,
				CanSwim = CanSwim,
				TrackIntensityOlfactory = TrackIntensityOlfactory,
				TrackIntensityVisual = TrackIntensityVisual,
				TrackingAbilityOlfactory = TrackingAbilityOlfactory,
				TrackingAbilityVisual = TrackingAbilityVisual,
				MinimumSleepingPosition = (int)MinimumSleepingPosition.Id,
				ChildAge = MinimumAgeForCategory(Heritage.AgeCategory.Child),
				YouthAge = MinimumAgeForCategory(Heritage.AgeCategory.Youth),
				YoungAdultAge = MinimumAgeForCategory(Heritage.AgeCategory.YoungAdult),
				AdultAge = MinimumAgeForCategory(Heritage.AgeCategory.Adult),
				ElderAge = MinimumAgeForCategory(Heritage.AgeCategory.Elder),
				VenerableAge = MinimumAgeForCategory(Heritage.AgeCategory.Venerable),
				DefaultHeightWeightModelMaleId = _defaultHeightWeightModels.ValueOrDefault(Gender.Male, null)?.Id,
				DefaultHeightWeightModelFemaleId = _defaultHeightWeightModels.ValueOrDefault(Gender.Female, null)?.Id,
				DefaultHeightWeightModelNeuterId = _defaultHeightWeightModels.ValueOrDefault(Gender.Neuter, null)?.Id,
				DefaultHeightWeightModelNonBinaryId =
					_defaultHeightWeightModels.ValueOrDefault(Gender.NonBinary, null)?.Id
			};
			FMDB.Context.Races.Add(dbitem);
			foreach (var part in _bodypartAdditions)
			{
				dbitem.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
				{
					Race = dbitem,
					BodypartId = part.Id,
					Usage = "general"
				});
			}

			foreach (var part in _maleOnlyAdditions)
			{
				dbitem.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
				{
					Race = dbitem,
					BodypartId = part.Id,
					Usage = "male"
				});
			}

			foreach (var part in _femaleOnlyAdditions)
			{
				dbitem.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
				{
					Race = dbitem,
					BodypartId = part.Id,
					Usage = "female"
				});
			}

			foreach (var part in _bodypartRemovals)
			{
				dbitem.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
				{
					Race = dbitem,
					BodypartId = part.Id,
					Usage = "remove"
				});
			}

			foreach (var item in _baseCharacteristics)
			{
				dbitem.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
				{
					Race = dbitem,
					CharacteristicDefinitionId = item.Id,
					Usage = "base"
				});
			}

			foreach (var item in _maleOnlyCharacteristics)
			{
				dbitem.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
				{
					Race = dbitem,
					CharacteristicDefinitionId = item.Id,
					Usage = "male"
				});
			}

			foreach (var item in _femaleOnlyCharacteristics)
			{
				dbitem.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
				{
					Race = dbitem,
					CharacteristicDefinitionId = item.Id,
					Usage = "female"
				});
			}

			foreach (var item in _attributes)
			{
				dbitem.RacesAttributes.Add(new RacesAttributes
				{
					Race = dbitem,
					AttributeId = item.Id,
					IsHealthAttribute = _healthTraits.Contains(item)
				});
			}

			foreach (var item in _breathableFluids)
			{
				if (item is ILiquid)
				{
					dbitem.RacesBreathableLiquids.Add(new RacesBreathableLiquids
					{
						Race = dbitem,
						Multiplier = _fluidBreathingMultipliers[item],
						LiquidId = item.Id
					});
					continue;
				}

				dbitem.RacesBreathableGases.Add(new RacesBreathableGases
				{
					Race = dbitem,
					Multiplier = _fluidBreathingMultipliers[item],
					GasId = item.Id
				});
			}

			foreach (var item in _removeBreathableFluids)
			{
				if (item is ILiquid)
				{
					dbitem.RacesRemoveBreathableLiquids.Add(new RacesRemoveBreathableLiquids
					{
						Race = dbitem,
						LiquidId = item.Id
					});
					continue;
				}

				dbitem.RacesRemoveBreathableGases.Add(new RacesRemoveBreathableGases
				{
					Race = dbitem,
					GasId = item.Id
				});
			}

			foreach (var item in _edibleMaterials)
			{
				dbitem.RacesEdibleMaterials.Add(new RacesEdibleMaterials
				{
					Race = dbitem,
					MaterialId = item.Material.Id,
					CaloriesPerKilogram = item.CaloriesPerKilogram,
					ThirstPerKilogram = item.ThirstPerKilogram,
					HungerPerKilogram = item.HungerPerKilogram,
					WaterPerKilogram = item.WaterPerKilogram,
					AlcoholPerKilogram = item.AlcoholPerKilogram
				});
			}

			foreach (var item in _costs)
			{
				dbitem.RacesChargenResources.Add(new RacesChargenResources
				{
					Race = dbitem,
					ChargenResourceId = item.Resource.Id,
					Amount = item.Amount,
					RequirementOnly = item.RequirementOnly
				});
			}

			foreach (var attack in _naturalWeaponAttacks)
			{
				var dbattack = new RacesWeaponAttacks();
				FMDB.Context.RacesWeaponAttacks.Add(dbattack);
				dbattack.BodypartId = attack.Bodypart.Id;
				dbattack.Quality = (int)attack.Quality;
				dbattack.Race = dbitem;
				dbattack.WeaponAttackId = attack.Attack.Id;
			}

			foreach (var action in _auxiliaryCombatActions)
			{
				FMDB.Context.RacesCombatActions.Add(new RacesCombatActions
				{
					CombatActionId = action.Id,
					Race = dbitem
				});
			}

			foreach (var item in _edibleForagableYields)
			{
				dbitem.RaceEdibleForagableYields.Add(new RaceEdibleForagableYields
				{
					Race = dbitem,
					YieldType = item.YieldType.ToLowerInvariant(),
					BiteYield = item.YieldPerBite,
					AlcoholPerYield = item.AlcoholPerYield,
					CaloriesPerYield = item.CaloriesPerYield,
					EatEmote = item.EmoteText,
					HungerPerYield = item.HungerPerYield,
					ThirstPerYield = item.ThirstPerYield,
					WaterPerYield = item.WaterPerYield
				});
			}

			foreach (var item in _chargenAdvices)
			{
				dbitem.ChargenAdvicesRaces.Add(new ChargenAdvicesRaces { ChargenAdviceId = item.Id, Race = dbitem });
			}

			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private IBreathingStrategy GetBreathingStrategy(string breathingModel)
	{
		if (!NeedsToBreathe)
		{
			return new NonBreather();
		}

		switch (breathingModel)
		{
			case "simple":
				return new LungBreather();
			case "gills":
				return new GillBreather();
			case "blowhole":
				return new BlowholeBreather();
			case "partless":
				return new PartlessBreather();
		}

		throw new ArgumentOutOfRangeException("Invalid value for breathingModel: " + breathingModel);
	}

	public IRace ParentRace { get; protected set; }
	public override string FrameworkItemType => "Race";

	public bool SameRace(IRace race)
	{
		return race != null && (race == this || ParentRace?.SameRace(race) == true);
	}

	public IBodyPrototype BaseBody { get; protected set; }
	public IEnumerable<IBodypart> BodypartAdditions => _bodypartAdditions;
	public IEnumerable<IBodypart> MaleOnlyAdditions => _maleOnlyAdditions;
	public IEnumerable<IBodypart> FemaleOnlyAdditions => _femaleOnlyAdditions;
	public IEnumerable<IBodypart> BodypartRemovals => _bodypartRemovals;

	public IEnumerable<ICharacteristicDefinition> Characteristics(Gender gender)
	{
		var returnVar = new List<ICharacteristicDefinition>(_baseCharacteristics);
		if (ParentRace != null)
		{
			returnVar.AddRange(ParentRace.Characteristics(gender));
		}

		if (gender == Gender.Male)
		{
			returnVar.AddRange(_maleOnlyCharacteristics);
		}

		if (gender == Gender.Female)
		{
			returnVar.AddRange(_femaleOnlyCharacteristics);
		}

		if (gender == Gender.NonBinary || gender == Gender.Indeterminate)
		{
			returnVar.AddRange(_maleAndFemaleCharacteristics);
		}

		return returnVar;
	}

	public IEnumerable<(Gender Gender, ICharacteristicDefinition Definition)> GenderedCharacteristics
	{
		get
		{
			var returnVar = new List<(Gender, ICharacteristicDefinition)>();
			if (ParentRace is not null)
			{
				returnVar.AddRange(ParentRace.GenderedCharacteristics);
			}

			foreach (var item in _baseCharacteristics)
			{
				returnVar.Add((Gender.Indeterminate, item));
			}

			foreach (var item in _maleOnlyCharacteristics)
			{
				returnVar.Add((Gender.Male, item));
			}

			foreach (var item in _femaleOnlyCharacteristics)
			{
				returnVar.Add((Gender.Female, item));
			}

			return returnVar;
		}
	}

	public void PromoteCharacteristicFromChildren(ICharacteristicDefinition definition, Gender gender)
	{
		var children = Gameworld.Races.Where(x => x.SameRace(this)).Except(this).ToList();
		foreach (var child in children)
		{
			child.RemoveCharacteristicDueToPromotion(definition);
		}

		switch (gender)
		{
			case Gender.Indeterminate:
				_baseCharacteristics.Add(definition);
				break;
			case Gender.Male:
				_maleOnlyCharacteristics.Add(definition);
				_maleAndFemaleCharacteristics.Add(definition);
				break;
			case Gender.Female:
				_femaleOnlyCharacteristics.Add(definition);
				_maleAndFemaleCharacteristics.Add(definition);
				break;
		}

		Changed = true;
		RecalculateCharactersBecauseOfRaceChange();
	}

	public void DemoteCharacteristicFromParent(ICharacteristicDefinition definition, Gender gender)
	{
		switch (gender)
		{
			case Gender.Indeterminate:
				_baseCharacteristics.Add(definition);
				break;
			case Gender.Male:
				_maleOnlyCharacteristics.Add(definition);
				_maleAndFemaleCharacteristics.Add(definition);
				break;
			case Gender.Female:
				_femaleOnlyCharacteristics.Add(definition);
				_maleAndFemaleCharacteristics.Add(definition);
				break;
		}

		Changed = true;
	}

	public void RemoveCharacteristicDueToPromotion(ICharacteristicDefinition definition)
	{
		_baseCharacteristics.Remove(definition);
		_femaleOnlyCharacteristics.Remove(definition);
		_maleOnlyCharacteristics.Remove(definition);
		_maleAndFemaleCharacteristics.Remove(definition);
		Changed = true;
	}

	public string Description { get; protected set; }

	public IEnumerable<Gender> AllowedGenders => _allowedGenders;

	public override string ToString()
	{
		return $"Race ID={Id} Name={Name}";
	}

	public void LinkParents(MudSharp.Models.Race race)
	{
		if (race.ParentRaceId.HasValue)
		{
			ParentRace = Gameworld.Races.Get(race.ParentRaceId.Value);
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Races.Find(Id);
		dbitem.Name = _name;
		dbitem.Description = Description;
		dbitem.BaseBodyId = BaseBody.Id;
		dbitem.AllowedGenders = _allowedGenders.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues(" ");
		dbitem.ParentRaceId = ParentRace?.Id;
		dbitem.AttributeBonusProgId = AttributeBonusProg.Id;
		dbitem.AttributeTotalCap = AttributeTotalCap;
		dbitem.IndividualAttributeCap = IndividualAttributeCap;
		dbitem.DiceExpression = DiceExpression;
		dbitem.IlluminationPerceptionMultiplier = IlluminationPerceptionMultiplier;
		dbitem.AvailabilityProgId = AvailabilityProg?.Id;
		dbitem.CorpseModelId = CorpseModel.Id;
		dbitem.DefaultHealthStrategyId = DefaultHealthStrategy.Id;
		dbitem.CanUseWeapons = CombatSettings.CanUseWeapons;
		dbitem.CanAttack = CombatSettings.CanAttack;
		dbitem.CanDefend = CombatSettings.CanDefend;
		dbitem.NaturalArmourTypeId = NaturalArmourType?.Id;
		dbitem.NaturalArmourQuality = (int)NaturalArmourQuality;
		dbitem.NaturalArmourMaterialId = NaturalArmourMaterial?.Id;
		dbitem.HungerRate = HungerRate;
		dbitem.ThirstRate = ThirstRate;
		dbitem.TrackIntensityOlfactory = TrackIntensityOlfactory;
		dbitem.TrackIntensityVisual = TrackIntensityVisual;
		dbitem.TrackingAbilityOlfactory = TrackingAbilityOlfactory;
		dbitem.TrackingAbilityVisual = TrackingAbilityVisual;
		dbitem.BloodLiquidId = BloodLiquid?.Id;
		dbitem.NeedsToBreathe = NeedsToBreathe;
		dbitem.BreathingModel = BreathingStrategy.Name;
		dbitem.SweatLiquidId = SweatLiquid?.Id;
		dbitem.SweatRateInLitresPerMinute = SweatRateInLitresPerMinute;
		dbitem.SizeStanding = (int)SizeStanding;
		dbitem.SizeProne = (int)SizeProne;
		dbitem.SizeSitting = (int)SizeSitting;
		dbitem.CommunicationStrategyType = CommunicationStrategy.Name;
		dbitem.DefaultHandedness = (int)DefaultHandedness;
		dbitem.HandednessOptions =
			HandednessOptions.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues(" ");
		dbitem.MaximumDragWeightExpression = MaximumDragWeightExpression.OriginalFormulaText;
		dbitem.MaximumLiftWeightExpression = MaximumLiftWeightExpression.OriginalFormulaText;
		dbitem.RaceButcheryProfileId = ButcheryProfile?.Id;
		dbitem.BloodModelId = BloodModel?.Id;
		dbitem.RaceUsesStamina = RaceUsesStamina;
		dbitem.CanEatCorpses = CanEatCorpses;
		dbitem.BiteWeight = BiteWeight;
		dbitem.EatCorpseEmoteText = EatCorpseEmoteText;
		dbitem.CanEatMaterialsOptIn = OptInMaterialEdibility;
		dbitem.TemperatureRangeFloor = TemperatureRangeFloor;
		dbitem.TemperatureRangeCeiling = TemperatureRangeCeiling;
		dbitem.BodypartSizeModifier = BodypartSizeModifier;
		dbitem.BodypartHealthMultiplier = BodypartDamageMultiplier;
		dbitem.BreathingVolumeExpression = BreathingVolumeExpression.OriginalFormulaText;
		dbitem.HoldBreathLengthExpression = HoldBreathLengthExpression.OriginalFormulaText;
		dbitem.CanClimb = CanClimb;
		dbitem.CanSwim = CanSwim;
		dbitem.MinimumSleepingPosition = (int)MinimumSleepingPosition.Id;
		dbitem.ChildAge = MinimumAgeForCategory(Heritage.AgeCategory.Child);
		dbitem.YouthAge = MinimumAgeForCategory(Heritage.AgeCategory.Youth);
		dbitem.YoungAdultAge = MinimumAgeForCategory(Heritage.AgeCategory.YoungAdult);
		dbitem.AdultAge = MinimumAgeForCategory(Heritage.AgeCategory.Adult);
		dbitem.ElderAge = MinimumAgeForCategory(Heritage.AgeCategory.Elder);
		dbitem.VenerableAge = MinimumAgeForCategory(Heritage.AgeCategory.Venerable);
		dbitem.DefaultHeightWeightModelMaleId = _defaultHeightWeightModels.ValueOrDefault(Gender.Male, null)?.Id;
		dbitem.DefaultHeightWeightModelFemaleId = _defaultHeightWeightModels.ValueOrDefault(Gender.Female, null)?.Id;
		dbitem.DefaultHeightWeightModelNeuterId = _defaultHeightWeightModels.ValueOrDefault(Gender.Neuter, null)?.Id;
		dbitem.DefaultHeightWeightModelNonBinaryId =
			_defaultHeightWeightModels.ValueOrDefault(Gender.NonBinary, null)?.Id;

		FMDB.Context.RacesAdditionalBodyparts.RemoveRange(dbitem.RacesAdditionalBodyparts);
		foreach (var part in _bodypartAdditions)
		{
			dbitem.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
			{
				Race = dbitem,
				BodypartId = part.Id,
				Usage = "general"
			});
		}

		foreach (var part in _maleOnlyAdditions)
		{
			dbitem.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
			{
				Race = dbitem,
				BodypartId = part.Id,
				Usage = "male"
			});
		}

		foreach (var part in _femaleOnlyAdditions)
		{
			dbitem.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
			{
				Race = dbitem,
				BodypartId = part.Id,
				Usage = "female"
			});
		}

		foreach (var part in _bodypartRemovals)
		{
			dbitem.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
			{
				Race = dbitem,
				BodypartId = part.Id,
				Usage = "remove"
			});
		}

		FMDB.Context.RacesAdditionalCharacteristics.RemoveRange(dbitem.RacesAdditionalCharacteristics);
		foreach (var item in _baseCharacteristics)
		{
			dbitem.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{
				Race = dbitem,
				CharacteristicDefinitionId = item.Id,
				Usage = "base"
			});
		}

		foreach (var item in _maleOnlyCharacteristics)
		{
			dbitem.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{
				Race = dbitem,
				CharacteristicDefinitionId = item.Id,
				Usage = "male"
			});
		}

		foreach (var item in _femaleOnlyCharacteristics)
		{
			dbitem.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{
				Race = dbitem,
				CharacteristicDefinitionId = item.Id,
				Usage = "female"
			});
		}

		FMDB.Context.RacesAttributes.RemoveRange(dbitem.RacesAttributes);
		foreach (var item in _attributes)
		{
			dbitem.RacesAttributes.Add(new RacesAttributes
			{
				Race = dbitem,
				AttributeId = item.Id,
				IsHealthAttribute = _healthTraits.Contains(item)
			});
		}

		FMDB.Context.RacesBreathableGases.RemoveRange(dbitem.RacesBreathableGases);
		FMDB.Context.RacesBreathableLiquids.RemoveRange(dbitem.RacesBreathableLiquids);
		foreach (var item in _breathableFluids)
		{
			if (item is ILiquid)
			{
				dbitem.RacesBreathableLiquids.Add(new RacesBreathableLiquids
				{
					Race = dbitem,
					Multiplier = _fluidBreathingMultipliers[item],
					LiquidId = item.Id
				});
				continue;
			}

			dbitem.RacesBreathableGases.Add(new RacesBreathableGases
			{
				Race = dbitem,
				Multiplier = _fluidBreathingMultipliers[item],
				GasId = item.Id
			});
		}

		FMDB.Context.RacesRemoveBreathableGases.RemoveRange(dbitem.RacesRemoveBreathableGases);
		FMDB.Context.RacesRemoveBreathableLiquids.RemoveRange(dbitem.RacesRemoveBreathableLiquids);
		foreach (var item in _removeBreathableFluids)
		{
			if (item is ILiquid)
			{
				dbitem.RacesRemoveBreathableLiquids.Add(new RacesRemoveBreathableLiquids
				{
					Race = dbitem,
					LiquidId = item.Id
				});
				continue;
			}

			dbitem.RacesRemoveBreathableGases.Add(new RacesRemoveBreathableGases
			{
				Race = dbitem,
				GasId = item.Id
			});
		}

		FMDB.Context.RacesEdibleMaterials.RemoveRange(dbitem.RacesEdibleMaterials);
		foreach (var item in _edibleMaterials)
		{
			dbitem.RacesEdibleMaterials.Add(new RacesEdibleMaterials
			{
				Race = dbitem,
				MaterialId = item.Material.Id,
				CaloriesPerKilogram = item.CaloriesPerKilogram,
				ThirstPerKilogram = item.ThirstPerKilogram,
				HungerPerKilogram = item.HungerPerKilogram,
				WaterPerKilogram = item.WaterPerKilogram,
				AlcoholPerKilogram = item.AlcoholPerKilogram
			});
		}

		FMDB.Context.RacesChargenResources.RemoveRange(dbitem.RacesChargenResources);
		foreach (var item in _costs)
		{
			dbitem.RacesChargenResources.Add(new RacesChargenResources
			{
				Race = dbitem,
				ChargenResourceId = item.Resource.Id,
				Amount = item.Amount,
				RequirementOnly = item.RequirementOnly
			});
		}

		FMDB.Context.RacesWeaponAttacks.RemoveRange(dbitem.RacesWeaponAttacks);
		foreach (var attack in _naturalWeaponAttacks)
		{
			var dbattack = new RacesWeaponAttacks();
			FMDB.Context.RacesWeaponAttacks.Add(dbattack);
			dbattack.BodypartId = attack.Bodypart.Id;
			dbattack.Quality = (int)attack.Quality;
			dbattack.RaceId = Id;
			dbattack.WeaponAttackId = attack.Attack.Id;
		}

		FMDB.Context.RacesCombatActions.RemoveRange(dbitem.RacesCombatActions);
		foreach (var action in _auxiliaryCombatActions)
		{
			var dbaction = new RacesCombatActions
			{
				CombatActionId = action.Id,
				RaceId = Id
			};
			FMDB.Context.RacesCombatActions.Add(dbaction);
		}

		FMDB.Context.RaceEdibleForagableYields.RemoveRange(dbitem.RaceEdibleForagableYields);
		foreach (var item in _edibleForagableYields)
		{
			dbitem.RaceEdibleForagableYields.Add(new RaceEdibleForagableYields
			{
				Race = dbitem,
				YieldType = item.YieldType.ToLowerInvariant(),
				BiteYield = item.YieldPerBite,
				AlcoholPerYield = item.AlcoholPerYield,
				CaloriesPerYield = item.CaloriesPerYield,
				EatEmote = item.EmoteText,
				HungerPerYield = item.HungerPerYield,
				ThirstPerYield = item.ThirstPerYield,
				WaterPerYield = item.WaterPerYield
			});
		}

		FMDB.Context.ChargenAdvicesRaces.RemoveRange(dbitem.ChargenAdvicesRaces);
		foreach (var item in _chargenAdvices)
		{
			dbitem.ChargenAdvicesRaces.Add(new ChargenAdvicesRaces { ChargenAdviceId = item.Id, Race = dbitem });
		}

		Changed = false;
	}

	#region IFutureProgVariable Members

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "childage", ProgVariableTypes.Number },
			{ "youthage", ProgVariableTypes.Number },
			{ "youngadultage", ProgVariableTypes.Number },
			{ "adultage", ProgVariableTypes.Number },
			{ "elderage", ProgVariableTypes.Number },
			{ "venerableage", ProgVariableTypes.Number }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The Id of the race" },
			{ "name", "The name of the race" },
			{ "childage", "The minimum age at which this race is considered a child" },
			{ "youthage", "The minimum age at which this race is considered a youth" },
			{ "youngadultage", "The minimum age at which this race is considered a young adult" },
			{ "adultage", "The minimum age at which this race is considered an adult" },
			{ "elderage", "The minimum age at which this race is considered an elder" },
			{ "venerableage", "The minimum age at which this race is considered a venerable elder" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Race, DotReferenceHandler(),
			DotReferenceHelp());
	}


	public IProgVariable GetProperty(string property)
	{
		IProgVariable returnVar = null;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = new NumberVariable(Id);
				break;
			case "name":
				returnVar = new TextVariable(Name);
				break;
			case "childage":
				returnVar = new NumberVariable(MinimumAgeForCategory(Heritage.AgeCategory.Child));
				break;
			case "youthage":
				returnVar = new NumberVariable(MinimumAgeForCategory(Heritage.AgeCategory.Youth));
				break;
			case "youngadultage":
				returnVar = new NumberVariable(MinimumAgeForCategory(Heritage.AgeCategory.YoungAdult));
				break;
			case "adultage":
				returnVar = new NumberVariable(MinimumAgeForCategory(Heritage.AgeCategory.Adult));
				break;
			case "elderage":
				returnVar = new NumberVariable(MinimumAgeForCategory(Heritage.AgeCategory.Elder));
				break;
			case "venerableage":
				returnVar = new NumberVariable(MinimumAgeForCategory(Heritage.AgeCategory.Venerable));
				break;
		}

		return returnVar;
	}

	public ProgVariableTypes Type => ProgVariableTypes.Race;

	public object GetObject => this;

	#endregion

	#region IRace Members

	private int _bodypartSizeModifier;
	private double _bodypartDamageMultiplier;

	public int BodypartSizeModifier => _bodypartSizeModifier;
	public double BodypartDamageMultiplier => _bodypartDamageMultiplier;

	public SizeCategory ModifiedSize(IBodypart part)
	{
		return part.Size.ChangeSize(_bodypartSizeModifier);
	}

	public double ModifiedHitpoints(IBodypart part)
	{
		return part.MaxLife * _bodypartDamageMultiplier;
	}

	public double ModifiedSeverthreshold(IBodypart part)
	{
		return part.SeveredThreshold * _bodypartDamageMultiplier;
	}

	// TODO - have separate damage/pain/tolerance and global/local modifiers
	public double DamageToleranceModifier => _bodypartDamageMultiplier;
	public double PainToleranceModifier => _bodypartDamageMultiplier;
	public double StunToleranceModifier => _bodypartDamageMultiplier;

	public IPositionState MinimumSleepingPosition { get; } = PositionLounging.Instance;

	public double TemperatureRangeFloor { get; protected set; }
	public double TemperatureRangeCeiling { get; protected set; }

	public bool CanClimb { get; }
	public bool CanSwim { get; }

	private readonly List<IAttributeDefinition> _attributes = new();

	public IEnumerable<IAttributeDefinition> Attributes =>
		ParentRace?.Attributes.Concat(_attributes).Distinct() ?? _attributes;

	public IFutureProg AttributeBonusProg { get; protected set; }

	public int AttributeTotalCap { get; protected set; }

	public int IndividualAttributeCap { get; protected set; }

	public string DiceExpression { get; protected set; }

	public double IlluminationPerceptionMultiplier { get; protected set; }

	public PerceptionTypes NaturalPerceptionTypes
		=>
			PerceptionTypes.VisualLight | PerceptionTypes.Audible | PerceptionTypes.Smell | PerceptionTypes.Taste |
			PerceptionTypes.Touch;

	private readonly List<ChargenResourceCost> _costs = new();

	public int ResourceCost(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => !x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public int ResourceRequirement(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public IFutureProg AvailabilityProg { get; protected set; }

	public bool ChargenAvailable(ICharacterTemplate template)
	{
		return _costs.Where(x => x.RequirementOnly)
		             .All(x => template.Account.AccountResources[x.Resource] >= x.Amount) &&
		       (AvailabilityProg?.ExecuteBool(template) ?? true);
	}

	public ICorpseModel CorpseModel { get; set; }

	public IHealthStrategy DefaultHealthStrategy { get; set; }

	public RacialCombatSettings CombatSettings { get; set; }

	private readonly List<INaturalAttack> _naturalWeaponAttacks = new();

	public IEnumerable<INaturalAttack> NaturalWeaponAttacks =>
		ParentRace?.NaturalWeaponAttacks.Concat(_naturalWeaponAttacks) ?? _naturalWeaponAttacks;

	public void AddNaturalAttack(INaturalAttack attack)
	{
		if (!_naturalWeaponAttacks.Contains(attack))
		{
			_naturalWeaponAttacks.Add(attack);
			Changed = true;
		}
	}

	public void RemoveNaturalAttack(INaturalAttack attack)
	{
		if (_naturalWeaponAttacks.Contains(attack))
		{
			_naturalWeaponAttacks.Remove(attack);
			Changed = true;
		}
	}

	public void RemoveNaturalAttacksAssociatedWith(IWeaponAttack attack)
	{
		if (_naturalWeaponAttacks.RemoveAll(x => x.Attack == attack) > 0)
		{
			Changed = true;
		}
	}

	public IEnumerable<INaturalAttack> UsableNaturalWeaponAttacks(ICharacter character, IPerceiver target,
		bool ignorePosition,
		params BuiltInCombatMoveType[] type)
	{
		return NaturalWeaponAttacks.Where(x => x.UsableAttack(character, target, ignorePosition, type)).ToList();
	}

	private readonly List<IAuxiliaryCombatAction> _auxiliaryCombatActions = new();

	public IEnumerable<IAuxiliaryCombatAction> UsableAuxiliaryMoves(ICharacter character, IPerceiver target,
		bool ignorePosition)
	{
		return AuxiliaryActions.Where(x => x.UsableMove(character, target, ignorePosition)).ToList();
	}
	public IEnumerable<IAuxiliaryCombatAction> AuxiliaryActions => ParentRace?.AuxiliaryActions.Concat(_auxiliaryCombatActions) ?? _auxiliaryCombatActions;

	public bool AddAuxiliaryAction(IAuxiliaryCombatAction action)
	{
		if (!_auxiliaryCombatActions.Contains(action))
		{
			_auxiliaryCombatActions.Add(action);
			Changed = true;
			return true;
		}

		return false;
	}

	public bool RemoveAuxiliaryAction(IAuxiliaryCombatAction action)
	{
		if (_auxiliaryCombatActions.Contains(action))
		{
			_auxiliaryCombatActions.Remove(action);
			Changed = true;
			return true;
		}

		return false;
	}

	public IArmourType NaturalArmourType { get; set; }

	public ItemQuality NaturalArmourQuality { get; set; }

	public IMaterial NaturalArmourMaterial { get; set; }

	private readonly ITraitExpression _maximumDragWeightExpression;
	public ITraitExpression MaximumDragWeightExpression => _maximumDragWeightExpression;

	public double GetMaximumDragWeight(ICharacter actor)
	{
		return _maximumDragWeightExpression.Evaluate(actor, context: TraitBonusContext.DraggingCapacity);
	}

	private readonly ITraitExpression _maximumLiftWeightExpression;
	public ITraitExpression MaximumLiftWeightExpression => _maximumLiftWeightExpression;

	public double GetMaximumLiftWeight(ICharacter actor)
	{
		return _maximumLiftWeightExpression.Evaluate(actor, context: TraitBonusContext.CarryingCapacity);
	}

	private long _bloodLiquidId;
	private ILiquid _bloodLiquid;

	public ILiquid BloodLiquid
	{
		get
		{
			if (_bloodLiquidId != 0)
			{
				_bloodLiquid = Gameworld.Liquids.Get(_bloodLiquidId);
				_bloodLiquidId = 0;
			}

			return _bloodLiquid;
		}
	}

	private long _sweatLiquidId;
	private ILiquid _sweatLiquid;

	public ILiquid SweatLiquid
	{
		get
		{
			if (_sweatLiquidId != 0)
			{
				_sweatLiquid = Gameworld.Liquids.Get(_sweatLiquidId);
				_sweatLiquidId = 0;
			}

			return _sweatLiquid;
		}
	}

	public double SweatRateInLitresPerMinute { get; set; }

	private readonly List<ITraitDefinition> _healthTraits = new();

	public IEnumerable<ITraitDefinition> HealthTraits =>
		ParentRace?.HealthTraits.Concat(_healthTraits).Distinct() ?? _healthTraits;

	private readonly List<IFluid> _breathableFluids = new();

	private readonly List<IFluid> _removeBreathableFluids = new();

	public IEnumerable<IFluid> BreathableFluids =>
		ParentRace?
			.BreathableFluids
			.Except(_removeBreathableFluids)
			.Concat(_breathableFluids)
			.Distinct() 
		?? _breathableFluids;

	private readonly Dictionary<IFluid, double> _fluidBreathingMultipliers = new();

	public (bool Truth, double RateMultiplier) CanBreatheFluid(IFluid fluid)
	{
		var best = _fluidBreathingMultipliers
			.Where(x => !_removeBreathableFluids.Contains(x.Key))
			.Where(x => fluid.CountsAs(x.Key) && fluid.CountAsQuality(x.Key) != ItemQuality.Terrible)
			.FirstMax(x => x.Value / fluid.CountsAsMultiplier(x.Key));
		if (best.Value <= 0.0)
		{
			return (false, 0.0);
		}

		return (true, best.Value);
	}

	public ITraitExpression BreathingVolumeExpression { get; set; }
	public ITraitExpression HoldBreathLengthExpression { get; set; }

	public double BreathingRate(IBody character, IFluid fluid)
	{
		var (_, rate) = CanBreatheFluid(fluid);
		BreathingVolumeExpression.Formula.Parameters["exertion"] = (int)character.CurrentExertion;
		return BreathingVolumeExpression.Evaluate(character, context: TraitBonusContext.BreathingRate) *
		       rate;
	}

	public TimeSpan HoldBreathLength(IBody character)
	{
		return TimeSpan.FromSeconds(
			HoldBreathLengthExpression.Evaluate(character, context: TraitBonusContext.HoldBreathLength));
	}

	private IBreathingStrategy _breathingStrategy;

	public IBreathingStrategy BreathingStrategy
	{
		get => _breathingStrategy;
		set
		{
			_breathingStrategy = value;
			Changed = true;
		}
	}

	public bool NeedsToBreathe { get; set; }
	public SizeCategory SizeStanding { get; set; } = SizeCategory.Normal;
	public SizeCategory SizeProne { get; set; } = SizeCategory.Normal;
	public SizeCategory SizeSitting { get; set; } = SizeCategory.Small;

	public double HungerRate { get; set; }
	public double ThirstRate { get; set; }
	public double TrackIntensityVisual { get; set; }
	public double TrackIntensityOlfactory { get; set; }
	public double TrackingAbilityVisual { get; set; }
	public double TrackingAbilityOlfactory { get; set; }

	public SizeCategory CurrentContextualSize(SizeContext context)
	{
		return SizeStanding;
	}

	public IBodyCommunicationStrategy CommunicationStrategy { get; set; }

	public IRaceButcheryProfile ButcheryProfile { get; set; }

	public bool RaceUsesStamina { get; protected set; }

	// Foragable Yields
	private readonly List<EdibleForagableYield> _edibleForagableYields = new();

	public IEnumerable<EdibleForagableYield> EdibleForagableYields =>
		ParentRace?.EdibleForagableYields.Concat(_edibleForagableYields) ?? _edibleForagableYields;

	public bool CanEatCorpses { get; protected set; }

	public bool CanEatFoodMaterial(IMaterial material)
	{
		return !_optInMaterialEdibility ||
		       _edibleMaterials.Any(x => x.Material == material) ||
		       (ParentRace?.CanEatFoodMaterial(material) ?? false);
	}

	public bool CanEatCorpseMaterial(IMaterial material)
	{
		return
			_edibleMaterials.Any(x => x.Material == material) ||
			(ParentRace?.CanEatCorpseMaterial(material) ?? false);
	}

	public bool CanEatForagableYield(string yieldType)
	{
		return
			_edibleForagableYields.Any(x => x.YieldType.EqualTo(yieldType)) ||
			(ParentRace?.CanEatForagableYield(yieldType) ?? false);
	}

	/// <summary>
	/// Whether for food items only the material is opt-in (i.e. requires an edible material listed) or unrestricted
	/// </summary>
	private bool _optInMaterialEdibility;

	public bool OptInMaterialEdibility => _optInMaterialEdibility;
	private readonly List<EdibleMaterial> _edibleMaterials = new();

	public IEnumerable<EdibleMaterial> EdibleMaterials =>
		ParentRace?.EdibleMaterials.Concat(_edibleMaterials) ?? _edibleMaterials;

	public double BiteWeight { get; protected set; }

	public double BiteYield(string yieldType)
	{
		return _edibleForagableYields.First(x => x.YieldType.EqualTo(yieldType)).YieldPerBite;
	}

	public string EatCorpseEmoteText { get; set; }

	public (string Emote, INeedFulfiller Fulfiller, double YieldBite) EatYield(string yieldType, double bites)
	{
		var efy = EdibleForagableYields.First(x => x.YieldType.EqualTo(yieldType));
		var totalYield = efy.YieldPerBite * bites;
		return (efy.EmoteText,
			new NeedFulfiller
			{
				Calories = efy.CaloriesPerYield * totalYield,
				SatiationPoints = efy.HungerPerYield * totalYield,
				ThirstPoints = efy.ThirstPerYield * totalYield,
				WaterLitres = efy.WaterPerYield * totalYield,
				AlcoholLitres = efy.AlcoholPerYield * totalYield
			},
			totalYield);
	}

	public INeedFulfiller GetCorpseNeedFulfill(IMaterial material, double bites)
	{
		var em = EdibleMaterials.First(x => x.Material == material);
		var totalKGs = BiteWeight * bites;
		return new NeedFulfiller
		{
			Calories = em.CaloriesPerKilogram * totalKGs,
			SatiationPoints = em.HungerPerKilogram * totalKGs,
			ThirstPoints = em.ThirstPerKilogram * totalKGs,
			WaterLitres = em.WaterPerKilogram * totalKGs,
			AlcoholLitres = em.AlcoholPerKilogram * totalKGs
		};
	}

	public RankedRange<AgeCategory> Ages { get; } = new();

	public AgeCategory AgeCategory(int ageInYears)
	{
		return Ages.Find(ageInYears);
	}

	public AgeCategory AgeCategory(ICharacter character)
	{
		var age = character.Birthday.Calendar.CurrentDate.YearsDifference(character.Birthday);
		return AgeCategory(age);
	}

	public AgeCategory AgeCategory(ICharacterTemplate template)
	{
		var age = template.SelectedBirthday.Calendar.CurrentDate.YearsDifference(template.SelectedBirthday);
		return AgeCategory(age);
	}

	public int MinimumAgeForCategory(AgeCategory category)
	{
		return Math.Max(0, (int)Ages.Ranges.First(x => x.Value == category).LowerBound);
	}

	private readonly Dictionary<Gender, IHeightWeightModel> _defaultHeightWeightModels = new();

	public IHeightWeightModel DefaultHeightWeightModel(Gender gender)
	{
		if (_defaultHeightWeightModels.ContainsKey(gender))
		{
			return _defaultHeightWeightModels[gender];
		}

		return ParentRace?.DefaultHeightWeightModel(gender);
	}

	public void AddAttributeFromPromotion(IAttributeDefinition definition)
	{
		_attributes.Add(definition);
		Changed = true;
		RecalculateCharactersBecauseOfRaceChange();
	}

	public void AddAttributeFromDemotion(IAttributeDefinition definition)
	{
		_attributes.Add(definition);
		Changed = true;
	}

	public void RemoveAttribute(IAttributeDefinition definition)
	{
		_attributes.Remove(definition);
		Changed = true;
	}

	public string ConsiderString { get; } = string.Empty; // TODO - load

	#endregion
}