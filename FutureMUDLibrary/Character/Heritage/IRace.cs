using System;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Health.Breathing;
using MudSharp.NPC.Templates;
using MudSharp.Strategies.BodyStratagies;
using MudSharp.Work.Butchering;

namespace MudSharp.Character.Heritage {
	public interface IRace : IEditableItem, IProgVariable, IHaveContextualSizeCategory, IHavePositionalSizes {
		string Description { get; }

		/// <summary>
		///     Additional Bodyparts to be connected to the IBody when this IRace is used
		/// </summary>
		IEnumerable<IBodypart> BodypartAdditions { get; }

		/// <summary>
		/// Bodyparts to be removed if present from the base model
		/// </summary>
		IEnumerable<IBodypart> BodypartRemovals { get; }
			/// <summary>
		///     Additional Bodyparts to be connected to the IBody when this IRace is used only for females
		/// </summary>
		IEnumerable<IBodypart> FemaleOnlyAdditions { get; }

		/// <summary>
		///     Additional Bodyparts to be connected to the IBody when this IRace is used only for males
		/// </summary>
		IEnumerable<IBodypart> MaleOnlyAdditions { get; }

		/// <summary>
		///     The body upon which this race is based
		/// </summary>
		IBodyPrototype BaseBody { get; }

		IEnumerable<Gender> AllowedGenders { get; }

		IEnumerable<IAttributeDefinition> Attributes { get; }
		void AddAttributeFromPromotion(IAttributeDefinition definition);
		void AddAttributeFromDemotion(IAttributeDefinition definition);
		void RemoveAttribute(IAttributeDefinition definition);

		IFutureProg AttributeBonusProg { get; }

		string DiceExpression { get; }

		int AttributeTotalCap { get; }

		int IndividualAttributeCap { get; }

		/// <summary>
		///     The multiplier (base = 1.0) relative to human perception of the light perceiving abilities of this race
		/// </summary>
		double IlluminationPerceptionMultiplier { get; }

		PerceptionTypes NaturalPerceptionTypes { get; }

		ICorpseModel CorpseModel { get; }

		IHealthStrategy DefaultHealthStrategy { get; }

		RacialCombatSettings CombatSettings { get; }

		IEnumerable<INaturalAttack> NaturalWeaponAttacks { get; }

		void AddNaturalAttack(INaturalAttack attack);
		void RemoveNaturalAttack(INaturalAttack attack);
		void RemoveNaturalAttacksAssociatedWith(IWeaponAttack attack);
		
		IEnumerable<INaturalAttack> UsableNaturalWeaponAttacks(ICharacter character, IPerceiver target,
			bool ignorePosition,
			params BuiltInCombatMoveType[] type);

		IEnumerable<IAuxiliaryCombatAction> UsableAuxiliaryMoves(ICharacter character, IPerceiver target,
			bool ignorePosition);
		IEnumerable<IAuxiliaryCombatAction> AuxiliaryActions { get; }
		bool AddAuxiliaryAction(IAuxiliaryCombatAction action);
		bool RemoveAuxiliaryAction(IAuxiliaryCombatAction action);

		IArmourType NaturalArmourType { get; }

		ItemQuality NaturalArmourQuality { get; }

		IMaterial NaturalArmourMaterial { get; }

		ILiquid BloodLiquid { get; }

		ILiquid SweatLiquid { get; }

		double SweatRateInLitresPerMinute { get; }

		IEnumerable<ITraitDefinition> HealthTraits { get; }

		IEnumerable<IFluid> BreathableFluids { get; }

		(bool Truth, double RateMultiplier) CanBreatheFluid(IFluid fluid);

		bool NeedsToBreathe { get; }

		double BreathingRate(IBody character, IFluid fluid);
		TimeSpan HoldBreathLength(IBody character);
		IBreathingStrategy BreathingStrategy { get; }
		IRace ParentRace { get; }

		/// <summary>
		///     Checks if the two races are equivalent
		/// </summary>
		/// <param name="race"></param>
		/// <returns></returns>
		bool SameRace(IRace race);

		IEnumerable<ICharacteristicDefinition> Characteristics(Gender gender);
		IEnumerable<(Gender Gender, ICharacteristicDefinition Definition)> GenderedCharacteristics { get; }
		void PromoteCharacteristicFromChildren(ICharacteristicDefinition definition, Gender gender);
		void DemoteCharacteristicFromParent(ICharacteristicDefinition definition, Gender gender);
		void RemoveCharacteristicDueToPromotion(ICharacteristicDefinition definition);

		int ResourceCost(IChargenResource resource);
		int ResourceRequirement(IChargenResource resource);

		bool ChargenAvailable(ICharacterTemplate template);

		IBodyCommunicationStrategy CommunicationStrategy { get; }
		
		Alignment DefaultHandedness { get; }
		IEnumerable<Alignment> HandednessOptions { get; }

		double GetMaximumDragWeight(ICharacter actor);

		double GetMaximumLiftWeight(ICharacter actor);

		IRaceButcheryProfile ButcheryProfile { get; }

		SizeCategory ModifiedSize(IBodypart part);
		double ModifiedHitpoints(IBodypart part);
		double ModifiedSeverthreshold(IBodypart part);

		double DamageToleranceModifier { get; }
		double PainToleranceModifier { get; }
		double StunToleranceModifier { get; }

		IBloodModel BloodModel { get; }

		IEnumerable<IChargenAdvice> ChargenAdvices { get; }
		bool ToggleAdvice(IChargenAdvice advice);
		bool RaceUsesStamina { get; }

		/// <summary>
		/// Whether or not this race can eat corpses or bodyparts directly
		/// </summary>
		bool CanEatCorpses { get; }

		/// <summary>
		/// Whether or not food of a specified material can be eaten by this race
		/// </summary>
		/// <param name="material">The material of the food item</param>
		/// <returns>True if it can be eaten</returns>
		bool CanEatFoodMaterial(IMaterial material);

		bool CanEatCorpseMaterial(IMaterial material);

		/// <summary>
		/// Whether or not a particular foragable yield of the location can be consumed directly
		/// </summary>
		/// <param name="yieldType">The yield type being queried</param>
		/// <returns>True if it can be eaten</returns>
		bool CanEatForagableYield(string yieldType);

		IEnumerable<EdibleForagableYield> EdibleForagableYields { get; }
		IEnumerable<EdibleMaterial> EdibleMaterials { get; }

		/// <summary>
		/// The weight of material eaten per bite of a corpse or bodypart
		/// </summary>
		double BiteWeight { get; }
		double BiteYield(string yieldType);
		string EatCorpseEmoteText { get; }

		(string Emote, INeedFulfiller Fulfiller, double YieldBite) EatYield(string yieldType, double bites);
		INeedFulfiller GetCorpseNeedFulfill(IMaterial material, double bites);

		double TemperatureRangeFloor { get; }
		double TemperatureRangeCeiling { get; }

		IPositionState MinimumSleepingPosition { get; }
		bool CanClimb { get; }
		bool CanSwim { get; }
		AgeCategory AgeCategory(int ageInYears);
		AgeCategory AgeCategory(ICharacter character);
		AgeCategory AgeCategory(ICharacterTemplate template);
		int MinimumAgeForCategory(AgeCategory category);
		string ConsiderString { get; }
		IHeightWeightModel DefaultHeightWeightModel(Gender gender);
		ITraitExpression BreathingVolumeExpression { get; }
		ITraitExpression HoldBreathLengthExpression { get; }
		ITraitExpression MaximumDragWeightExpression { get; }
		ITraitExpression MaximumLiftWeightExpression { get; }
		int BodypartSizeModifier { get; }
		double BodypartDamageMultiplier { get; }
		bool OptInMaterialEdibility { get; }
		IRace Clone(string newName);
		double HungerRate { get; }
		double ThirstRate { get; }
		double TrackIntensityVisual { get; }
		double TrackIntensityOlfactory { get; }
		double TrackingAbilityVisual { get; }
		double TrackingAbilityOlfactory { get; }
	}

	public enum SizeContext {
		None,
		CellExit,
		RangedTarget,
		ExplosiveDamage,
		Scan,
		GrappleAttack,
		GrappleDefense,
		RainfallExposure,
		RidingMount,
		BeingRiddenAsMount
	}

	public enum AgeCategory
	{
		Baby,
		Child,
		Youth,
		YoungAdult,
		Adult,
		Elder,
		Venerable
	}

	public interface IHavePositionalSizes
	{
		SizeCategory SizeStanding { get; }
		SizeCategory SizeProne { get; }
		SizeCategory SizeSitting { get; }
	}

	public interface IHaveContextualSizeCategory {
		SizeCategory CurrentContextualSize(SizeContext context);
	}
}