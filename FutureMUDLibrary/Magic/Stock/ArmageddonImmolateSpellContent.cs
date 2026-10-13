#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.Health;

namespace MudSharp.Magic;

/// <summary>Approved direct fire strike followed by a spell-owned burning effect.</summary>
public static class ArmageddonImmolateSpellContent
{
	public const string Key = "arm.spell.immolate";

	public static ArmageddonCanonicalSpellContribution Create() => new("arm.capability.fire",
		new ArmageddonUtilitySpellContent(Key, "Immolate",
			"Strike one visible local character for 3*external grade native fire damage, then burn for 2*grade damage " +
			"every 10 seconds during a 30*grade second spell lifetime. The strike uses the current native body and " +
			"armour path; the sustained fire is spell-owned and survives an ordinary cold load for its remaining lifetime. " +
			"The target, damage type, formulas, tick interval and lifetime are editable builder defaults.",
			"30*grade", 20, "$0 set|sets $1 ablaze with magical fire.",
			(resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				var definition = ArmageddonUtilitySpellContent.Definition(Key, "character", resource, cost, filter,
					40, 20, new XElement("Effect", new XAttribute("type", "damage"),
						new XElement("DamageType", (int)DamageType.Burning),
						new XElement("DamageExpression", "3*grade"), new XElement("PainExpression", "0"),
						new XElement("StunExpression", "0"), new XElement("Bodypart", 0), new XElement("Limb", -1),
						new XElement("IdentitySafeDamage", new XAttribute("schema", 1), new XAttribute("version", 1))));
				definition.Element("Trigger")!.SetElementValue("CanTargetSelf", false);
				definition.Element("Effects")!.Add(new XElement("Effect", new XAttribute("type", "burning"),
					new XElement("DamageType", (int)DamageType.Burning),
					new XElement("DamageFormula", "2*grade"), new XElement("PainFormula", "0"),
					new XElement("StunFormula", "0"), new XElement("ThermalFormula", "0"),
					new XElement("TickSeconds", 10), new XElement("MinimumOxidation", 0),
					new XElement("SelfOxidising", true),
					new XElement("SDescAddendum", "(burning)"),
					new XElement("DescAddendum", "@ is wreathed in magical fire."),
					new XElement("AddendumColour", "bold red")));
				return definition;
			}));
}
