using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MudSharp.Combat;

public static class WeaponPoisonDeliveryHelper
{
	public const string ApplyDefaultCapacityFraction = "WeaponPoisonApplyDefaultCapacityFraction";
	public const string DipCapacityFraction = "WeaponPoisonDipCapacityFraction";
	public const string DosePerHitCapacityFraction = "WeaponPoisonDosePerHitCapacityFraction";
	public const string DeliveryMinimumChance = "WeaponPoisonDeliveryMinimumChance";
	public const string DeliveryMaximumChance = "WeaponPoisonDeliveryMaximumChance";
	public const string DipDifficultyStepsEasier = "WeaponPoisonDipDifficultyStepsEasier";
	public const string ContactDamageMultiplier = "WeaponPoisonContactDamageMultiplier";
	public const string ExternalBleedingWoundMultiplier = "WeaponPoisonExternalBleedingWoundMultiplier";
	public const string ExternalNonBleedingWoundMultiplier = "WeaponPoisonExternalNonBleedingWoundMultiplier";
	public const string InternalWoundMultiplier = "WeaponPoisonInternalWoundMultiplier";

	public static string InjectedDamageMultiplierConfiguration(DamageType type)
	{
		return $"WeaponPoisonInjectedDamageMultiplier{type.DescribeEnum()}";
	}

	public static string SeverityMultiplierConfiguration(WoundSeverity severity)
	{
		return $"WeaponPoisonSeverityMultiplier{severity.DescribeEnum()}";
	}

	public static bool IsValidPoisonableItem(IGameItem item)
	{
		return item.IsItemType<IMeleeWeapon>() || item.IsItemType<IAmmo>();
	}

	public static bool HasDeliverableDrug(LiquidMixture mixture)
	{
		return mixture?.Instances.Any(x =>
			x.Liquid.Drug is not null &&
			x.Liquid.DrugGramsPerUnitVolume > 0.0 &&
			(x.Liquid.Drug.DrugVectors.HasFlag(DrugVector.Touched) || x.Liquid.Drug.DrugVectors.HasFlag(DrugVector.Injected))) == true;
	}

	public static double CoatingCapacity(IGameItem item)
	{
		return item.LiquidAbsorbtionAmounts.Coating * Math.Max(1, item.Quantity);
	}

	public static double ExistingPoisonVolume(IGameItem item)
	{
		return item.EffectsOfType<IWeaponPoisonCoatingEffect>().Sum(x => x.ContaminatingLiquid.TotalVolume);
	}

	public static double RemainingPoisonCapacity(IGameItem item, double capacityFraction)
	{
		return Math.Max(0.0, CoatingCapacity(item) * capacityFraction - ExistingPoisonVolume(item));
	}

	public static void AddPoisonCoating(IGameItem item, LiquidMixture mixture)
	{
		if (mixture is null || mixture.IsEmpty)
		{
			return;
		}

		var effect = item.EffectsOfType<IWeaponPoisonCoatingEffect>()
		                 .FirstOrDefault(x => x.ContaminatingLiquid.CanMerge(mixture));
		if (effect is null)
		{
			effect = new WeaponPoisonCoating(item, LiquidMixture.CreateEmpty(item.Gameworld));
			effect.AddLiquid(mixture);
			item.AddEffect(effect, LiquidContamination.EffectDuration(effect.ContaminatingLiquid));
			return;
		}

		effect.AddLiquid(mixture);
		item.Reschedule(effect, LiquidContamination.EffectDuration(effect.ContaminatingLiquid));
	}

	public static void CopyPoisonCoating(IGameItem source, IGameItem target)
	{
		if (source == target)
		{
			return;
		}

		foreach (var coating in source.EffectsOfType<IWeaponPoisonCoatingEffect>().ToList())
		{
			var copied = new WeaponPoisonCoating(target, coating.ContaminatingLiquid.Clone());
			var duration = source.ScheduledDuration(coating);
			target.AddEffect(copied, duration > TimeSpan.Zero ? duration : LiquidContamination.EffectDuration(copied.ContaminatingLiquid));
		}
	}

	public static void DeliverFromWeapon(ICharacter attacker, IGameItem coatedItem, IEnumerable<IWound> wounds,
		bool requireOriginMatch = true)
	{
		var coatings = coatedItem.EffectsOfType<IWeaponPoisonCoatingEffect>().ToList();
		if (!coatings.Any())
		{
			return;
		}

		var causedWounds = requireOriginMatch
			? wounds.Where(x =>
				x.ToolOrigin == coatedItem ||
				x.Lodged == coatedItem ||
				(x.ToolOrigin is not null && x.ToolOrigin.Id == coatedItem.Id)).ToList()
			: wounds.ToList();
		if (!causedWounds.Any())
		{
			return;
		}

		foreach (var coating in coatings)
		{
			if (coating.ContaminatingLiquid is null || coating.ContaminatingLiquid.IsEmpty)
			{
				continue;
			}

			DeliverOneCoating(attacker, coatedItem, coating, causedWounds);
		}
	}

	public static double CalculateDeliveryChance(IFuturemud gameworld, IWound wound, DrugVector vector)
	{
		var severity = StaticDouble(gameworld, SeverityMultiplierConfiguration(wound.Severity));
		var damage = vector == DrugVector.Injected
			? StaticDouble(gameworld, InjectedDamageMultiplierConfiguration(wound.DamageType))
			: StaticDouble(gameworld, ContactDamageMultiplier);
		if (vector == DrugVector.Injected && damage <= 0.0)
		{
			return 0.0;
		}

		var woundNature = wound.Internal
			? StaticDouble(gameworld, InternalWoundMultiplier)
			: wound.BleedStatus == BleedStatus.Bleeding
				? StaticDouble(gameworld, ExternalBleedingWoundMultiplier)
				: StaticDouble(gameworld, ExternalNonBleedingWoundMultiplier);
		var minimum = StaticDouble(gameworld, DeliveryMinimumChance);
		var maximum = StaticDouble(gameworld, DeliveryMaximumChance);
		return Math.Clamp(severity * damage * woundNature, minimum, maximum);
	}

	public static void DoseContact(IBody body, LiquidMixture mixture, object originator)
	{
		foreach (var liquid in mixture.Instances)
		{
			var drug = liquid.Liquid.Drug;
			if (drug is null || !drug.DrugVectors.HasFlag(DrugVector.Touched))
			{
				continue;
			}

			var grams = liquid.Amount * liquid.Liquid.DrugGramsPerUnitVolume;
			if (grams > 0.0)
			{
				body.Dose(drug, DrugVector.Touched, grams, originator);
			}
		}
	}

	public static Difficulty DipDifficulty(IFuturemud gameworld, Difficulty baseDifficulty)
	{
		return baseDifficulty.StageDown((int)StaticDouble(gameworld, DipDifficultyStepsEasier));
	}

	public static double StaticDouble(IFuturemud gameworld, string key)
	{
		try
		{
			return gameworld.GetStaticDouble(key);
		}
		catch
		{
			return DefaultStaticSettings.DefaultStaticConfigurations.TryGetValue(key, out var value) &&
			       double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
				? result
				: 0.0;
		}
	}

	private static void DeliverOneCoating(ICharacter attacker, IGameItem coatedItem,
		IWeaponPoisonCoatingEffect coating, IReadOnlyCollection<IWound> causedWounds)
	{
		var mixture = coating.ContaminatingLiquid;
		var hasInjected = mixture.Instances.Any(x =>
			x.Liquid.Drug is not null &&
			x.Liquid.DrugGramsPerUnitVolume > 0.0 &&
			x.Liquid.Drug.DrugVectors.HasFlag(DrugVector.Injected));
		var hasTouched = mixture.Instances.Any(x =>
			x.Liquid.Drug is not null &&
			x.Liquid.DrugGramsPerUnitVolume > 0.0 &&
			x.Liquid.Drug.DrugVectors.HasFlag(DrugVector.Touched));

		if (!hasInjected && !hasTouched)
		{
			return;
		}

		var injectedWound = hasInjected
			? causedWounds
				.Where(x => BodyForWound(x) is not null)
				.Select(x => (Wound: x, Chance: CalculateDeliveryChance(coatedItem.Gameworld, x, DrugVector.Injected)))
				.Where(x => x.Chance > 0.0)
				.OrderByDescending(x => x.Chance)
				.FirstOrDefault()
			: default;

		var vector = injectedWound.Wound is not null ? DrugVector.Injected : DrugVector.Touched;
		var selectedWound = injectedWound.Wound;
		var chance = injectedWound.Chance;
		if (selectedWound is null)
		{
			if (!hasTouched)
			{
				return;
			}

			var contactWound = causedWounds
				.Where(x => BodyForWound(x) is not null)
				.Select(x => (Wound: x, Chance: CalculateDeliveryChance(coatedItem.Gameworld, x, DrugVector.Touched)))
				.OrderByDescending(x => x.Chance)
				.FirstOrDefault();
			selectedWound = contactWound.Wound;
			chance = contactWound.Chance;
		}

		if (selectedWound is null)
		{
			return;
		}

		var doseVolume = Math.Min(
			coating.ContaminatingLiquid.TotalVolume,
			Math.Max(0.0, CoatingCapacity(coatedItem) * StaticDouble(coatedItem.Gameworld, DosePerHitCapacityFraction)));
		if (doseVolume <= 0.0)
		{
			return;
		}

		var doseMixture = coating.RemovePoisonVolume(doseVolume);
		if (doseMixture is null || doseMixture.IsEmpty || !RandomUtilities.Roll(1.0, chance))
		{
			return;
		}

		var body = BodyForWound(selectedWound);
		if (body is null)
		{
			return;
		}

		if (vector == DrugVector.Injected)
		{
			body.HealthStrategy.InjectedLiquid(body, FilterMixture(doseMixture, DrugVector.Injected));
			return;
		}

		DoseContact(body, FilterMixture(doseMixture, DrugVector.Touched), attacker is not null ? attacker : coatedItem);
	}

	private static LiquidMixture FilterMixture(LiquidMixture mixture, DrugVector vector)
	{
		var instances = mixture.Instances
			.Where(x => x.Liquid.Drug?.DrugVectors.HasFlag(vector) == true)
			.Select(x => x.Copy())
			.ToList();
		return instances.Any()
			? new LiquidMixture(instances, mixture.Gameworld)
			: LiquidMixture.CreateEmpty(mixture.Gameworld);
	}

	private static IBody BodyForWound(IWound wound)
	{
		return wound.Parent as IBody ?? (wound.Parent as IHaveABody)?.Body;
	}
}
