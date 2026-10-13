#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic;

public sealed record ArmageddonShadowBladeBindings(IReadOnlyList<long> TemporarySwords, IReadOnlyList<long> PermanentStaffs,
	long CreationRankSixTagId, long EligibilityProgId, long LifetimeMultiplierProgId);
public sealed record ArmageddonSparkLashBindings(IReadOnlyList<long> TemporaryWhips, IReadOnlyList<long> PermanentStaffs,
	long CreationRankSixTagId, long EligibilityProgId);

/// <summary>Locked native weapon and room-water contributions with caller-resolved dependencies.</summary>
public static class ArmageddonGradedWeaponSpellContent
{
	public const string ChampionLifetime = "300*grade";
	public const string SourceWeaponLifetime = "1125*(grade+1)";
	public const string PuddleLitres = "0.25*grade";
	public static IReadOnlyList<string> Keys => ["arm.spell.shadow_blade", "arm.spell.champion_blade", "arm.spell.spark_lash", "arm.spell.gather_puddle"];
	public static ArmageddonUtilitySpellContent ShadowBlade(ArmageddonShadowBladeBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings);
		if (bindings.LifetimeMultiplierProgId <= 0) throw new ArgumentException("Shadow Blade requires an explicit native lifetime multiplier prog for its Shadow-sector doubling.");
		return SourceWeapon("shadow_blade", "Shadow Blade", bindings.TemporarySwords, bindings.PermanentStaffs,
			bindings.CreationRankSixTagId, bindings.EligibilityProgId, bindings.LifetimeMultiplierProgId,
			"Grades one through six create one temporary shadow sword for 1125*(grade+1) seconds, doubled by the explicit Shadow-sector multiplier. Grade seven consumes one directly carried Creation rank-six descendant and samples one of eight permanent staves. The configured native eligibility gate preserves source environment restrictions.");
	}
	public static ArmageddonUtilitySpellContent SparkLash(ArmageddonSparkLashBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings);
		return SourceWeapon("spark_lash", "Spark Lash", bindings.TemporaryWhips, bindings.PermanentStaffs,
			bindings.CreationRankSixTagId, bindings.EligibilityProgId, 0,
			"Grades one through six create one temporary lightning whip for 1125*(grade+1) seconds. Grade seven consumes one directly carried Creation rank-six descendant and samples one of eight permanent staves. The configured native eligibility gate preserves source environment restrictions.");
	}
	private static ArmageddonUtilitySpellContent SourceWeapon(string suffix, string name, IReadOnlyList<long> temporary,
		IReadOnlyList<long> permanent, long componentTag, long eligibility, long multiplier, string description)
	{
		ArgumentNullException.ThrowIfNull(temporary); ArgumentNullException.ThrowIfNull(permanent);
		if (temporary.Count != 6 || permanent.Count != 8 || temporary.Concat(permanent).Any(x => x <= 0) ||
			temporary.Concat(permanent).Distinct().Count() != 14 || componentTag <= 0 || eligibility <= 0)
			throw new ArgumentException("Source weapon creation requires six distinct temporary weapons, eight distinct permanent staves, a Creation rank-six tag and a native eligibility prog.");
		var weapons = temporary.ToArray(); var staffs = permanent.ToArray(); var key = "arm.spell." + suffix;
		return new(key, name, description, SourceWeaponLifetime, 7, "$0 shape|shapes a magical weapon.", (resource, cost, _) =>
		{
			var effect = ItemEffect(suffix.Replace('_', '-'), SourceWeaponLifetime, weapons.Select((id, index) => new KeyValuePair<int, long>(index + 1, id)).ToDictionary(x => x.Key, x => x.Value));
			var life = effect.Element("Lifecycle")!;
			var gradeOutputs = life.Element("GradeOutputs")!; gradeOutputs.Remove();
			life.Add(new XElement("PermanentOutput", new XAttribute("grade", 7), staffs[0]), new XElement("EligibilityProg", eligibility),
				multiplier == 0 ? null : new XElement("LifetimeMultiplierProg", multiplier));
			gradeOutputs.Add(new XElement("Output", new XAttribute("grade", 7), staffs.Select(x => new XElement("Prototype", x)))); life.Add(gradeOutputs);
			var definition = ArmageddonUtilitySpellContent.Definition(key, "character", resource, cost, 0, 30, 7, effect);
			definition.Element("Plan")!.Add(new XElement("Phase", new XElement("Action", new XAttribute("state", "consumed"),
				new XAttribute("tag", componentTag), new XAttribute("secondtag", 0), new XAttribute("quantity", 1),
				new XAttribute("carriedonly", true), new XAttribute("grade", 7))));
			return definition;
		});
	}
	public static ArmageddonUtilitySpellContent ChampionBlade(IReadOnlyDictionary<int, long> prototypes)
	{
		var outputs = GradeItems(prototypes);
		return new("arm.spell.champion_blade", "Champion Blade",
			"Creates one usable temporary enchanted sword of the selected grade for 300*grade seconds. Ordinary combat and placement apply; no summoned hero is linked.",
			ChampionLifetime, 10, "$0 shape|shapes an enchanted sword.", (resource, cost, _) =>
				ArmageddonUtilitySpellContent.Definition("arm.spell.champion_blade", "character", resource, cost, 0, 30, 10,
					ItemEffect("champion-blade", ChampionLifetime, outputs)));
	}
	public static ArmageddonUtilitySpellContent GatherPuddle(long waterLiquidId)
	{
		if (waterLiquidId <= 0) throw new ArgumentOutOfRangeException(nameof(waterLiquidId));
		return new("arm.spell.gather_puddle", "Gather Puddle",
			"Creates or tops up native clean-water surface custody at your current room, layer and route point with 0.25*grade litres clamped to native puddle capacity.",
			"0", 3, "$0 gather|gathers water into a puddle.", (resource, cost, _) =>
				ArmageddonUtilitySpellContent.Definition("arm.spell.gather_puddle", "room", resource, cost, 0, 30, 3,
					new XElement("Effect", new XAttribute("type", "createliquid"), new XElement("LiquidId", waterLiquidId),
						new XElement("AmountFormula", "0"), new XElement("GatherPuddle", new XAttribute("version", 1), new XElement("Litres", PuddleLitres)))));
	}
	private static Dictionary<int, long> GradeItems(IReadOnlyDictionary<int, long> prototypes)
	{
		ArgumentNullException.ThrowIfNull(prototypes);
		if (prototypes.Count != 7 || Enumerable.Range(1, 7).Any(x => !prototypes.TryGetValue(x, out var id) || id <= 0) || prototypes.Values.Distinct().Count() != 7)
			throw new ArgumentException("Select seven distinct positive native weapon prototypes by external requested grade.");
		return prototypes.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
	}
	private static XElement ItemEffect(string family, string lifetime, Dictionary<int, long> outputs) =>
		new("Effect", new XAttribute("type", "createitem"), new XElement("ItemQuality", "base"), new XElement("ItemPrototypeId", outputs[1]),
			new XElement("ItemSkinId", 0), new XElement("Quantity", 1), new XElement("LoadString", ""),
			new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", "TemporaryCleanup"),
				new XElement("Family", family), new XElement("Seconds", lifetime),
				new XElement("GradeOutputs", outputs.Select(x => new XElement("Output", new XAttribute("grade", x.Key), new XElement("Prototype", x.Value))))));
}
