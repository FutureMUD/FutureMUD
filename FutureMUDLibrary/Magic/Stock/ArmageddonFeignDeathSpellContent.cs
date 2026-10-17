#nullable enable

using System;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>A living-body death illusion with the locked Void duration and energy minimum.</summary>
public static class ArmageddonFeignDeathSpellContent
{
	public const string Key = "arm.spell.feign_death";

	public static ArmageddonCanonicalSpellContribution Create() => new("arm.capability.void",
		new ArmageddonUtilitySpellContent(Key, "Feign Death",
			"Make one living character appear dead to ordinary visual and vitals observers for 60 seconds per selected grade. " +
			"The target remains physiologically alive; voluntary action breaks the appearance.",
			"60*grade", 20, "$0 draw|draws a deathly stillness over $1.",
			(resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				return ArmageddonUtilitySpellContent.Definition(Key, "character", resource, cost, filter, 60, 20,
					new XElement("Effect", new XAttribute("type", "feigndeath"), new XAttribute("schema", 1)));
			}));
}
