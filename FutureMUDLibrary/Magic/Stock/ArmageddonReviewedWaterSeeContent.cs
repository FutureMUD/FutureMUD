#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>Pure reviewed definitions; native catalogue and ancestry validation belong to callers.</summary>
public static class ArmageddonReviewedWaterSeeContent
{
	// These unordered native posture identities preserve the reviewed eligibility text exactly.
	public const string WaterBreathingEligibilitySource = "var posture = positionid(@caster)\nreturn @caster.incombat == false and (@posture == 1 or @posture == 9 or @posture == 10 or @posture == 11 or @posture == 14 or @posture == 15 or @posture == 13 or @posture == 16 or @posture == 17 or @posture == 18 or @posture == 19 or @posture == 20)";
	public const string SeeTheUnbodiedEligibilitySource = "if (@target != @caster)\nreturn false\nend if\nvar posture = positionid(@caster)\nreturn @caster.incombat == false and (@posture == 1 or @posture == 9 or @posture == 10 or @posture == 11 or @posture == 14 or @posture == 15 or @posture == 13 or @posture == 16 or @posture == 17 or @posture == 18 or @posture == 19 or @posture == 20)";

	public static ArmageddonUtilitySpellContent WaterBreathing(IReadOnlyList<long> waterLiquids)
	{
		ArgumentNullException.ThrowIfNull(waterLiquids);
		var ids = waterLiquids.ToArray();
		if (ids.Length is < 1 or > 128 || ids.Any(id => id <= 0) || ids.Distinct().Count() != ids.Length)
			throw new ArgumentException("Select one to 128 distinct positive native water liquid IDs.", nameof(waterLiquids));
		return new("arm.spell.water_breathing", "Water Breathing", "Let the recipient breathe explicitly mapped native water while this enchantment lasts. Source duration draws inclusively from floor(grade/2) through three times grade, then clamps zero to one source hour. Each source hour is 600 real seconds; recasts add remaining normalized hours up to 36 and retain the strongest source grade with its native power. Exact native deadlines and saved remaining time are engine adaptations. Only authored native liquid identities are granted, with no CountsAs or mixture inheritance, gas immunity, organ repair or supply refill. Edit water mappings and accumulated lifetime through the normal effect builder. Ordinary character/self targeting uses an editable minimum-Standing filter: standing variants and active movement postures, including flying, swimming and riding, qualify; resting postures and combat do not. This is an explicit native posture mapping of the source minimum, not a literal standing-only rule. No source Nilaz, Silt, Fire or underwater-only restriction is active. There is no consumed component or opposed save. Sorcerer acquisition requires Draw Wine raw80; opening 30, raw cap 90, branches 80, source minimum energy 20. Add capability admission and acquisition separately; stored scroll delivery is unsupported.",
			"0", 20, "$0 weave|weaves a water breathing enchantment around $1.",
			(resource, cost, filter) => WaterDefinition(resource, cost, filter, ids), WaterBreathingEligibilitySource);
	}

	public static ArmageddonUtilitySpellContent SeeTheUnbodied(long siltTerrain, long shadowTerrain,
		IReadOnlyList<long> divinationRanks)
	{
		ArgumentNullException.ThrowIfNull(divinationRanks);
		if (siltTerrain <= 0) throw new ArgumentOutOfRangeException(nameof(siltTerrain));
		if (shadowTerrain <= 0) throw new ArgumentOutOfRangeException(nameof(shadowTerrain));
		var ranks = divinationRanks.ToArray();
		if (ranks.Length != 5 || ranks.Any(id => id <= 0) || ranks.Distinct().Count() != 5)
			throw new ArgumentException("Select five distinct positive native Divination rank IDs.", nameof(divinationRanks));
		return new("arm.spell.see_the_unbodied", "See the Unbodied", "Open your senses to eligible ethereal entities through native perception. Historical Detect Ethereal is self-only, refuses mapped Silt before mapped Shadow's component exemption, and adds three source hours per grade to remaining whole source hours up to 36 while retaining the strongest source grade. Each source hour is 600 real seconds; exact deadlines and saved remaining time offline are native adaptations. Outside mapped Shadow consume one directly carried Divination quantity unit of rank at least grade minus three, floored at zero, including grades one to three. Native quantity-unit consumption adapts historical whole-object extraction; higher descendants qualify. Edit the source terrain IDs, dedicated component reference, rank hierarchy and accumulated lifetime through the normal spell builders. The editable eligibility prog admits only self and the existing minimum-Standing mapping: standing variants and active movement including swimming, flying and riding; resting postures and combat refuse. Native character-target syntax requires explicit self; historical omitted-target self is not provided by this filter-bearing trigger. No opposed save, duration draw, blindness removal or unrestricted planar sight is supplied. Source setting guild/race component bypasses are not inferred. Pierce Concealment raw80 opens this spell at30, raw cap90, branches80, printed minimum energy7. Capability admission and acquisition remain separate; charged and area delivery are unsupported.",
			"1800*grade", 7, "$0 open|opens $0's senses to the unbodied.",
			(resource, cost, filter) => SeeDefinition(resource, cost, filter, siltTerrain, shadowTerrain, ranks),
			SeeTheUnbodiedEligibilitySource);
	}

	private static XElement WaterDefinition(long resource, long cost, long filter, IReadOnlyList<long> water) =>
		ArmageddonUtilitySpellContent.Definition("arm.spell.water_breathing", "character", resource, cost, filter, 30, 20,
			new XElement("Effect", new XAttribute("type", "sourcewaterbreathing"),
				LifetimePolicy("armageddon.water_breathing"),
				new XElement("WaterScope", new XAttribute("version", 1),
					water.Select(id => new XElement("Liquid", new XAttribute("id", id))))));

	private static XElement SeeDefinition(long resource, long cost, long filter, long silt, long shadow,
		IReadOnlyList<long> ranks)
	{
		var definition = ArmageddonUtilitySpellContent.Definition("arm.spell.see_the_unbodied", "character",
			resource, cost, filter, 30, 7,
			new XElement("Effect", new XAttribute("type", "detectethereal"),
				LifetimePolicy("armageddon.detect_ethereal"),
				new XElement("SourceScope", new XAttribute("version", 1), new XAttribute("silt", silt),
					new XAttribute("shadow", shadow), new XAttribute("component", "armageddon.see_unbodied.divination"))));
		definition.Element("Plan")!.Add(new XElement("Phase", new XElement("Action", new XAttribute("state", "consumed"),
			new XAttribute("tag", ranks[0]), new XAttribute("secondtag", 0), new XAttribute("quantity", 1),
			new XAttribute("carriedonly", true), new XAttribute("originalreference", "armageddon.see_unbodied.divination"),
			new XElement("GradeRank", new XAttribute("offset", -3), ranks.Select((tag, rank) =>
				new XElement("Rank", new XAttribute("minimum", rank), new XAttribute("tag", tag)))))));
		return definition;
	}

	private static XElement LifetimePolicy(string group) => new("LifetimePolicy", new XAttribute("version", 1),
		new XAttribute("mode", "accumulate"), new XAttribute("group", group), new XAttribute("unitSeconds", 600),
		new XAttribute("maximumUnits", 36), new XAttribute("retainStrongestGrade", true));
}
