#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Form.Audio;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

/// <summary>Bounded, one-payment local harm with an explicit physical-recipient policy.</summary>
public static class ArmageddonAreaHarmSpellContent
{
	public const string CinderRainKey = "arm.spell.cinder_rain";
	public const string ThunderclapKey = "arm.spell.thunderclap";
	public const string ScouringGustKey = "arm.spell.scouring_gust";
	public const string CausticSprayKey = "arm.spell.caustic_spray";

	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create(long resistanceTraitId)
	{
		ArmageddonPrimitiveSpellBindings.RequirePositive(resistanceTraitId, nameof(resistanceTraitId));
		return Array.AsReadOnly(new[]
	{
		Create(CinderRainKey, "Cinder Rain", "fire", DamageType.Burning, 7,
			"30*grade", "Burn each eligible local other for 3*grade initial damage and 2*grade every ten seconds.", null),
		Create(ThunderclapKey, "Thunderclap", "water", DamageType.Sonic, 7,
			"30*grade", "Strike each eligible local other for 3*grade sonic damage and resistible temporary deafness.", resistanceTraitId),
		Create(ScouringGustKey, "Scouring Gust", "wind", DamageType.Slashing, 7,
			"0", "Injure each eligible local other for 3*grade wind damage and reduce stamina by grade.", null),
		Create(CausticSprayKey, "Caustic Spray", "water", DamageType.Chemical, 8,
			"0", "Strike explicitly admitted local others for 3*grade acid-type damage.", null)
	});
	}

	private static ArmageddonCanonicalSpellContribution Create(string key, string name, string school,
		DamageType damageType, double minimum, string duration, string effectDescription, long? resistanceTraitId)
	{
		var content = new ArmageddonUtilitySpellContent(key, name,
			effectDescription + " The editable area default selects distinct physical bodies in the caster's cell and layer, " +
			"excluding the caster and staff, including allies, and refusing more than 256 eligible recipients before payment. " +
			"Only ordinary named Say casting is authored; no generic quiet or carrier admission is implied.",
			duration, minimum, "$0 unleash|unleashes " + name.ToLowerInvariant() + " around $0.",
			(resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				var definition = ArmageddonUtilitySpellContent.Definition(key, "character", resource, cost, filter,
					40, minimum, new XElement("Effect", new XAttribute("type", "damage"),
						new XElement("DamageType", (int)damageType), new XElement("DamageExpression", "3*grade"),
						new XElement("PainExpression", "0"), new XElement("StunExpression", "0"),
						new XElement("Bodypart", 0), new XElement("Limb", -1),
						new XElement("IdentitySafeDamage", new XAttribute("schema", 1), new XAttribute("version", 1))));
				definition.Element("Trigger")!.SetElementValue("CanTargetSelf", false);
				var effects = definition.Element("Effects")!;
				if (key == CinderRainKey)
					effects.Add(new XElement("Effect", new XAttribute("type", "burning"),
						new XElement("DamageType", (int)DamageType.Burning), new XElement("DamageFormula", "2*grade"),
						new XElement("PainFormula", "0"), new XElement("StunFormula", "0"),
						new XElement("ThermalFormula", "0"), new XElement("TickSeconds", 10),
						new XElement("MinimumOxidation", 0), new XElement("SelfOxidising", true),
						new XElement("SDescAddendum", "(burning)"),
						new XElement("DescAddendum", "@ is showered in magical embers."),
						new XElement("AddendumColour", "bold red")));
				if (key == ThunderclapKey)
					effects.Add(new XElement("Effect", new XAttribute("type", "deafness")));
				if (key == ScouringGustKey)
					effects.Add(new XElement("Effect", new XAttribute("type", "staminadelta"),
						new XElement("Formula", "-grade"), new XElement("DirectStaminaAdjustment", true)));
				var area = new XElement("Area", new XAttribute("schema", 1), new XAttribute("scope", 0),
					new XAttribute("selection", 0), new XAttribute("identity", 0), new XAttribute("plane", 0),
					new XAttribute("targets", 256), new XAttribute("applications", 256),
					new XAttribute("caster", false), new XAttribute("allies", true), new XAttribute("others", true),
					new XAttribute("layer", true), new XAttribute("grounded", false),
					new XAttribute("staff", true), new XAttribute("casterDamage", 1),
					new XAttribute("otherDamage", 1), new XAttribute("filter", 0),
					new XAttribute("provenance", "S4 authored per-entry local recipient policy; editable builder default"),
					new XElement("Method", new XAttribute("name", "Say"), new XAttribute("volume", (int)AudioVolume.Decent),
						new XAttribute("energy", 1), new XAttribute("difficulty", 0)));
				definition.Element("ControlledPower")!.Add(area);
				return definition;
			}, ResistingTraitId: resistanceTraitId,
			ResistingDifficulty: resistanceTraitId is null ? null : Difficulty.Normal);
		return new("arm.capability." + school, content);
	}
}
