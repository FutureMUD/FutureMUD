using System;
using System.Xml.Linq;
using MudSharp.Health;

#nullable enable

namespace MudSharp.Magic;

/// <summary>One configured native hit linked to at most one admitted Wind exit step.</summary>
public static class ArmageddonWindHammerSpellContent
{
	public const string Key = "arm.spell.wind_hammer";

	public static ArmageddonCanonicalSpellContribution Create() => new("arm.capability.wind",
		new ArmageddonUtilitySpellContent(Key, "Wind Hammer",
			"Strike one exact local character for 5*external grade shockwave damage with no added pain or stun, " +
			"then force at most one legal non-fall step through the explicitly selected exit only when native harm is applied. " +
			"The damage body and exit are selected and reconfirmed before payment; native armour and movement gates remain authoritative.",
			"0", 33, "$0 launch|launches a hammering gust against $1.",
			(resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				var definition = ArmageddonUtilitySpellContent.Definition(Key, "characterexit", resource, cost,
					filter, 74, 33, new XElement("Effect", new XAttribute("type", "windhammer"),
						new XAttribute("schema", 1), new XElement("Damage", new XElement("Effect",
							new XAttribute("type", "damage"),
							new XElement("DamageType", (int)DamageType.Shockwave),
							new XElement("DamageExpression", "5*grade"),
							new XElement("PainExpression", "0"), new XElement("StunExpression", "0"),
							new XElement("Bodypart", 0), new XElement("Limb", -1),
							new XElement("IdentitySafeDamage", new XAttribute("schema", 1),
								new XAttribute("version", 1))))));
				definition.Element("Trigger")!.SetElementValue("CanTargetSelf", false);
				return definition;
			}));
}
