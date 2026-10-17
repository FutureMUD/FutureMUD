#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.Form.Audio;
using MudSharp.Health;

namespace MudSharp.Magic;

/// <summary>Source-shaped repeated electrical strikes; all four environmental identities are supplied by the owned world.</summary>
public static class ArmageddonLeapingLightningSpellContent
{
	public const string Key = "arm.spell.leaping_lightning";
	public const string ProtectionTag = "arm.source.chain_lightning";

	public static ArmageddonCanonicalSpellContribution Create(long siltTag, long shallowsTag,
		long waterPlane, long lightningPlane)
	{
		if (siltTag <= 0 || shallowsTag <= 0 || waterPlane <= 0 || lightningPlane <= 0 ||
			siltTag == shallowsTag || waterPlane == lightningPlane)
			throw new ArgumentException("Leaping Lightning requires distinct typed Silt/Shallows tags and Water/Lightning planes.");
		var content = new ArmageddonUtilitySpellContent(Key, "Leaping Lightning",
			"Strike independently sampled eligible bodies in the caster's room with replacement, including repeated hits. " +
			"The selected external grade yields grade+1 strikes, or 2*grade+1 in tagged Silt/Shallows terrain or effective Water/Lightning plane presence. " +
			"One shared electrical damage budget is sampled inclusively from 3*grade through 5*grade before target draws. " +
			"Caster hits take one quarter damage; other hits take full damage. Charged Aegis completely absorbs a selected bolt. " +
			"Only the native area route is valid; no target-free ordinary invocation, quiet method or carrier is admitted.",
			"0", 7, "$0 call|calls leaping lightning into the surrounding space.",
			(resource, cost, filter) =>
			{
				if (resource <= 0 || cost <= 0 || filter < 0)
					throw new ArgumentOutOfRangeException(nameof(resource));
				var definition = ArmageddonUtilitySpellContent.Definition(Key, "character", resource, cost, filter, 60, 7,
					new XElement("Effect", new XAttribute("type", "damage"),
						new XElement("DamageType", (int)DamageType.Electrical),
						new XElement("DamageExpression", "0"), new XElement("Bodypart", 0), new XElement("Limb", -1),
						new XElement("IdentitySafeDamage", new XAttribute("schema", 1), new XAttribute("version", 1)),
						new XElement("InterdictionTag", new XCData(ProtectionTag))));
				definition.Element("ControlledPower")!.Add(new XElement("Area", new XAttribute("schema", 1),
					new XAttribute("scope", (int)SpellAreaScope.RoomCharacters),
					new XAttribute("selection", (int)SpellAreaSelection.RandomWithReplacement),
					new XAttribute("identity", (int)SpellAreaIdentity.PhysicalBody),
					new XAttribute("plane", (int)SpellAreaPlane.MagicReach),
					new XAttribute("targets", 256), new XAttribute("applications", 15),
					new XAttribute("applicationBase", 1), new XAttribute("applicationsPerGrade", 1),
					new XAttribute("caster", true), new XAttribute("allies", true), new XAttribute("others", true),
					new XAttribute("layer", false), new XAttribute("grounded", false), new XAttribute("staff", false),
					new XAttribute("casterDamage", 0.25), new XAttribute("otherDamage", 1),
					new XAttribute("filter", 0), new XAttribute("provenance", "S4 locked source-shaped chain lightning"),
					new XElement("Method", new XAttribute("name", "Say"), new XAttribute("volume", (int)AudioVolume.Decent),
						new XAttribute("energy", 1), new XAttribute("difficulty", 0)),
					new XElement("ChainLightning", new XAttribute("siltTag", siltTag),
						new XAttribute("shallowsTag", shallowsTag), new XAttribute("waterPlane", waterPlane),
						new XAttribute("lightningPlane", lightningPlane))));
				return definition;
			});
		return new("arm.capability.lightning", content);
	}
}
