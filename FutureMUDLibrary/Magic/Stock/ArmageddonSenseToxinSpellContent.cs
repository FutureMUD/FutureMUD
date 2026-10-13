#nullable enable

using System;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>Retained, binary permission to inspect native active and latent drug dosages.</summary>
public static class ArmageddonSenseToxinSpellContent
{
	public const string Key = "arm.spell.sense_toxin";

	public static ArmageddonCanonicalSpellContribution Create() => new("arm.capability.water",
		new ArmageddonUtilitySpellContent(Key, "Sense Toxin",
			"For 60 seconds per selected grade, the recipient may inspect detectable active and latent native drug poison on nearby characters using inspect toxin. This grants no treatment, trait bonus, or ability to inspect a character who is not locally targetable.",
			"60*grade", 7, "$0 open|opens $1's senses to hidden toxins.",
			(resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				return ArmageddonUtilitySpellContent.Definition(Key, "character", resource, cost, filter, 60, 7,
					new XElement("Effect", new XAttribute("type", "detectpoison"), new XElement("RetainedPermission", true)));
			}));
}
