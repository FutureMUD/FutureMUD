#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public sealed record ArmageddonCanonicalUtilityBindings(long WaterLiquidId, long WineLiquidId, long WaterPlaneId,
	long HoveringLightPrototypeId, IReadOnlyList<long> FoodPrototypeIds, long CharacterResistanceTraitId,
	string PerceptionEligibilitySource, string TargetEligibilitySource, string LiquidEligibilitySource, string MagicalEligibilitySource);

/// <summary>Canonical content with caller-owned typed dependencies; legacy reviewed contributions remain independent.</summary>
public static class ArmageddonCanonicalUtilityContent
{
	public const string SustainMealTargetException = "The locked defining result delivers food into the caster's room. The canonical room trigger preserves that native placement despite the inventory's broad character target label.";
	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Contributions(ArmageddonCanonicalUtilityBindings bindings)
	{
		Validate(bindings);
		var foods = bindings.FoodPrototypeIds.ToArray();
		var sense = ArmageddonReviewedUtilityContent.SenseEnchantment() with
		{
			Description = "Reveals perceptible native magical effects to the recipient for 3000*grade seconds with exclusive refresh. The explicit owned perception predicate refuses Silt.",
			EligibilitySource = bindings.PerceptionEligibilitySource
		};
		var mend = ArmageddonReviewedUtilityContent.MendFlesh() with
		{
			Description = "Heals eligible native wounds worst first with overflow within 2*grade*grade damage. The explicit owned target/caster predicate enforces undead and Void restrictions, including the configured Defiler exception.",
			EligibilitySource = bindings.TargetEligibilitySource
		};
		var water = ArmageddonReviewedUtilityContent.DrawWater(bindings.WaterLiquidId, bindings.WaterPlaneId) with
		{
			Description = "Adds 0.5*grade litres of native clean water to an accessible compatible open drink container, clamped to remaining capacity. The selected Water plane doubles volume; the explicit owned liquid predicate refuses Fire and Silt. Water persists through ordinary fluid custody.",
			EligibilitySource = bindings.LiquidEligibilitySource
		};
		var hover = ArmageddonReviewedUtilityContent.HoveringLight(bindings.HoveringLightPrototypeId) with
		{
			Description = "Creates one real wearable native light in the recipient's available configured wear profile for 1800*grade seconds. Native illumination, custody and cleanup apply; it supplies no free held focus."
		};
		var wine = ArmageddonReviewedProvisionContent.DrawWine(bindings.WineLiquidId, bindings.WaterPlaneId) with
		{
			Description = "Adds 0.5*grade litres of the configured native fallback wine to an accessible compatible open drink container, clamped to remaining capacity. The selected Water plane doubles volume; the explicit owned liquid predicate refuses Fire and Silt. Ordinary fluid persistence applies without expiry.",
			EligibilitySource = bindings.LiquidEligibilitySource
		};
		var meal = ArmageddonReviewedProvisionContent.SustainMeal(foods) with
		{
			Description = "Creates grade independent finite edible items in the caster's room from three authored native food profiles. Each output expires after 1350*grade seconds and provides real bites and nutrition; consumed units are never recreated. " + SustainMealTargetException
		};
		var pierce = ArmageddonReviewedPierceContent.PierceConcealment();
		var pierceDefinition = pierce.BuildDefinition;
		pierce = pierce with
		{
			Description = "Permits the recipient to perceive eligible invisible entities for 60*grade seconds through the native binary detection effect. The explicit owned perception predicate refuses Silt; planar and other native perception restrictions still apply.",
			DurationFormula = "60*grade", EligibilitySource = bindings.PerceptionEligibilitySource,
			BuildDefinition = (resource, cost, filter) =>
			{
				var definition = pierceDefinition(resource, cost, filter);
				definition.Descendants("LifetimePolicy").Single().Remove(); return definition;
			}
		};
		var bane = ArmageddonBaneContent.Create(bindings.CharacterResistanceTraitId, Difficulty.Normal, 8, 56, DamageType.Arcane) with
		{
			EligibilitySource = bindings.MagicalEligibilitySource
		};
		return
		[
			new("arm.capability.fire", Strict(sense)), new("arm.capability.water", Strict(mend)),
			new("arm.capability.water", Strict(water)), new("arm.capability.fire", Strict(hover)),
			new("arm.capability.water", Strict(wine)), new("arm.capability.earth", Strict(meal)),
			new("arm.capability.wind", Strict(pierce)), new("arm.capability.void", Strict(bane))
		];
	}
	private static ArmageddonUtilitySpellContent Strict(ArmageddonUtilitySpellContent content)
	{
		var build = content.BuildDefinition;
		return content with { BuildDefinition = (resource, cost, filter) =>
		{
			if (resource <= 0 || cost <= 0 || filter < 0 || content.EligibilitySource is not null && filter == 0)
				throw new ArgumentException("Resolve positive native resource/cost and the required owned eligibility prog before emitting canonical utility content.");
			return build(resource, cost, filter);
		} };
	}
	private static void Validate(ArmageddonCanonicalUtilityBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings); ArgumentNullException.ThrowIfNull(bindings.FoodPrototypeIds);
		if (bindings.WaterLiquidId <= 0 || bindings.WineLiquidId <= 0 || bindings.WaterLiquidId == bindings.WineLiquidId ||
			bindings.WaterPlaneId <= 0 || bindings.HoveringLightPrototypeId <= 0 || bindings.CharacterResistanceTraitId <= 0 ||
			bindings.FoodPrototypeIds.Count != 3 || bindings.FoodPrototypeIds.Any(x => x <= 0 || x == bindings.HoveringLightPrototypeId) ||
			bindings.FoodPrototypeIds.Distinct().Count() != 3 ||
			new[] { bindings.PerceptionEligibilitySource, bindings.TargetEligibilitySource, bindings.LiquidEligibilitySource, bindings.MagicalEligibilitySource }.Any(string.IsNullOrWhiteSpace))
			throw new ArgumentException("Canonical utilities require distinct native water/wine, a Water plane, wearable-light prototype, three distinct food prototypes, native resistance trait and four exact owned predicate sources.");
	}
}
