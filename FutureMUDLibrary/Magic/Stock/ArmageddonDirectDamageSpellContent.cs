#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

/// <summary>Resolved native world policy for the single opposed burst; no historical IDs.</summary>
public sealed record ArmageddonDirectDamageSpellBindings(long ResistanceTraitId, Difficulty ResistanceDifficulty,
	DamageType PsychicDamageType)
{
	public void Validate()
	{
		ArmageddonPrimitiveSpellBindings.RequirePositive(ResistanceTraitId, nameof(ResistanceTraitId));
		if (!Enum.IsDefined(ResistanceDifficulty)) throw new ArgumentOutOfRangeException(nameof(ResistanceDifficulty));
		if (!Enum.IsDefined(PsychicDamageType)) throw new ArgumentOutOfRangeException(nameof(PsychicDamageType));
	}
}

/// <summary>Locked S4 single-target burst definitions. No admissions or delivery qualification.</summary>
public static class ArmageddonDirectDamageSpellContent
{
	public static ArmageddonCanonicalSpellContribution ConsumingFlame(long necromancyComponentTag)
	{
		ArmageddonPrimitiveSpellBindings.RequirePositive(necromancyComponentTag, nameof(necromancyComponentTag));
		return Burst("fire", "consuming_flame", "Consuming Flame", DamageType.Burning,
			33, 40, 8, 3, componentTag: necromancyComponentTag);
	}

	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create(ArmageddonDirectDamageSpellBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings);
		bindings.Validate();
		return Array.AsReadOnly(new[]
		{
			Burst("fire", "ember_lance", "Ember Lance", DamageType.Burning, 20, 40, 5, 2),
			Burst("fire", "pillar_of_flame", "Pillar of Flame", DamageType.Burning, 7, 60, 8, 3),
			Burst("lightning", "lightning_lance", "Lightning Lance", DamageType.Electrical, 20, 60, 5, 2),
			Burst("void", "mind_scour", "Mind Scour", bindings.PsychicDamageType, 20, 40, 5, 2,
				bindings.ResistanceTraitId, bindings.ResistanceDifficulty)
		});
	}

	private static ArmageddonCanonicalSpellContribution Burst(string school, string suffix, string name,
		DamageType type, double minimum, int opening, int damage, int pain, long? resistance = null,
		Difficulty? difficulty = null, long? componentTag = null)
	{
		var key = "arm.spell." + suffix;
		var content = new ArmageddonUtilitySpellContent(key, name,
			$"Strike one visible local target for {damage}*external grade native damage, {pain}*grade pain and zero stun. " +
			$"The editable native damage choice is {type}; all values are approved authored stock defaults. " +
			"Native armour and prosthetics remain in the damage path. Frozen current identity, body, location and custody prevent callback redirection. " +
			(componentTag is null ? "" : "One directly carried Necromancy component is consumed before the strike. ") +
			(resistance is null ? "This stock strike is unopposed." : "The exact supplied native resistance trait and difficulty oppose the strike."),
			"0", minimum, "$0 shape|shapes a striking enchantment against $1.", (resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				var definition = ArmageddonUtilitySpellContent.Definition(key, "character", resource, cost, filter,
					opening, minimum, new XElement("Effect", new XAttribute("type", "damage"),
						new XElement("DamageType", (int)type), new XElement("DamageExpression", $"{damage}*grade"),
						new XElement("PainExpression", $"{pain}*grade"), new XElement("StunExpression", "0"),
						new XElement("Bodypart", 0), new XElement("Limb", -1),
						new XElement("IdentitySafeDamage", new XAttribute("schema", 1), new XAttribute("version", 1))));
				definition.Element("Trigger")!.SetElementValue("CanTargetSelf", false);
				if (componentTag is { } tag)
					definition.Element("Plan")!.Add(new XElement("Phase", new XElement("Action",
						new XAttribute("state", "consumed"), new XAttribute("tag", tag),
						new XAttribute("secondtag", 0), new XAttribute("quantity", 1),
						new XAttribute("carriedonly", true))));
				return definition;
			}, ResistingTraitId: resistance, ResistingDifficulty: difficulty);
		return new("arm.capability." + school, content);
	}
}
