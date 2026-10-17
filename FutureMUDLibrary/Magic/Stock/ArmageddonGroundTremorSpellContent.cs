#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic;

public static class ArmageddonGroundTremorSpellContent
{
	public const string Key = "arm.spell.ground_tremor";
	public static ArmageddonCanonicalSpellContribution Create(long stoneSkin, long agility, long shadow,
		long wind, long water, long empty, long silt)
	{
		if (new[] { stoneSkin, agility, shadow, wind, water, empty, silt }.Any(x => x <= 0))
			throw new ArgumentException("Ground Tremor requires its exact native spell, attribute, plane and terrain bindings.");
		return new("arm.capability.earth", new(Key, "Ground Tremor",
			"Shake the ground for 3*grade local Crushing damage, with one third for the caster. " +
			"Staff, airborne, ethereal, Shadow-plane and active Stone Skin bodies are protected. " +
			"Separately propagate through admitted native room edges up to grade-1 edges, sending nearby messages and an ordinary Agility check against falling. " +
			"Wind-plane casting refuses; nearby propagation stops at Silt terrain, Wind, Water or Void planes and native wards. " +
			"At most 256 bodies and 512 rooms are admitted before payment. Only named Say casting is authored; no quiet or carrier routes.",
			"0", 7, "$0 gesture|gestures as the ground begins to shake.", (resource, cost, filter) =>
				ArmageddonUtilitySpellContent.Definition(Key, "room", resource, cost, filter, 40, 7,
					new XElement("Effect", new XAttribute("type", "groundtremor"), new XAttribute("version", 1),
						new XElement("StoneSkinSpell", stoneSkin), new XElement("AgilityTrait", agility),
						new XElement("ShadowPlane", shadow), new XElement("WindPlane", wind),
						new XElement("WaterPlane", water), new XElement("VoidPlane", empty), new XElement("SiltTag", silt)))));
	}
}
