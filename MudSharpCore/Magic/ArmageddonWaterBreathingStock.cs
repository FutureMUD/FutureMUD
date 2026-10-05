#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Form.Material;

namespace MudSharp.Magic;

/// <summary>Editable content; capability admission and acquisition belong to the installer.</summary>
public static class ArmageddonWaterBreathingStock
{
	public const string Key = "arm.spell.water_breathing";
	public const string Name = "Water Breathing";
	public const string PrerequisiteKey = ArmageddonDrawWineStock.Key;
	public const double PrerequisiteRaw = 80;
	public const int Opening = 30;
	public const double RawCap = 90;
	public const double BranchRaw = 80;
	public const double MinimumEnergy = 20;
	public const string LifetimeGroup = "armageddon.water_breathing";
	public const int UnitSeconds = 600;
	public const int MaximumUnits = 36;
	internal const string EligibilitySource = "return isstanding(@caster)";

	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, IReadOnlyList<ILiquid> water)
	{
		ArgumentNullException.ThrowIfNull(water);
		if (water.Count == 0 || water.Count > 128 || water.Any(x => x is null || x.Id <= 0 || world.Liquids.Get(x.Id) != x) ||
			water.Select(x => x.Id).Distinct().Count() != water.Count)
			throw new InvalidOperationException("Select one to 128 distinct existing native water liquid definitions explicitly.");
		var ids = water.Select(x => x.Id).ToArray();
		return ArmageddonUtilityStock.Create(world, school, trait, resource, Name,
			"Let the recipient breathe explicitly mapped native water while this enchantment lasts. Source duration draws inclusively from floor(grade/2) through three times grade, then clamps zero to one source hour. Each source hour is 600 real seconds; recasts add remaining normalized hours up to 36 and retain the strongest source grade with its native power. Exact native deadlines and saved remaining time are engine adaptations. Only authored native liquid identities are granted, with no CountsAs or mixture inheritance, gas immunity, organ repair or supply refill. Edit water mappings and accumulated lifetime through the normal effect builder. Ordinary character/self targeting requires a Standing caster through an editable filter; no source Nilaz, Silt, Fire or underwater-only restriction is active. There is no consumed component or opposed save. Sorcerer acquisition requires Draw Wine raw80; opening30, raw cap90, branches80, source minimum energy20. Add capability admission and acquisition separately; stored scroll delivery is unsupported.",
			"0", MinimumEnergy, "$0 weave|weaves a water breathing enchantment around $1.",
			(r, c, f) => Definition(r, c, f, ids), EligibilitySource);
	}

	internal static XElement Definition(long resource, long cost, long filter, IReadOnlyList<long> water) =>
		ArmageddonUtilityStock.Definition(Key, "character", resource, cost, filter, Opening, MinimumEnergy,
			new XElement("Effect", new XAttribute("type", "sourcewaterbreathing"),
				SpellEffects.DetectInvisibleEffect.WritePolicy(new(LifetimeGroup, UnitSeconds, MaximumUnits)),
				new XElement("WaterScope", new XAttribute("version", 1),
					water.Select(id => new XElement("Liquid", new XAttribute("id", id))))));
}
