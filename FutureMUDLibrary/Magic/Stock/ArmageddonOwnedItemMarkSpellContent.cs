#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>Finite owner-scoped marks; the runtime verifies exact legal ownership before payment.</summary>
public static class ArmageddonOwnedItemMarkSpellContent
{
	public const string CustodianGlyphKey = "arm.spell.custodian_glyph";
	public const string AnchorRuneKey = "arm.spell.anchor_rune";

	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create() => Array.AsReadOnly(new[]
	{
		Create(CustodianGlyphKey, "Custodian Glyph", "fire", "glyph", 10, 60,
			"Mark one legally owned item so an unauthorised character cannot take it while the finite glyph lasts."),
		Create(AnchorRuneKey, "Anchor Rune", "wind", "anchor", 50, 300,
			"Register one legally owned item's exact identity and owner as a finite travel anchor. Travel spells must resolve this typed active mark; a name or number lookup does not authorize travel.")
	});

	private static ArmageddonCanonicalSpellContribution Create(string key, string name, string school,
		string mode, double minimum, int secondsPerGrade, string description)
	{
		var content = new ArmageddonUtilitySpellContent(key, name,
			description + " The item, legal owner and custody are confirmed before payment. Ownership changes invalidate the mark. " +
			"Dispel and expiry remove it. Only ordinary named Say casting is authored; quiet and carriers remain disabled.",
			$"{secondsPerGrade}*grade", minimum,
			"$0 inscribe|inscribes a faint rune upon $1.",
			(resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				return ArmageddonUtilitySpellContent.Definition(key, "item", resource, cost, filter,
					40, minimum, new XElement("Effect", new XAttribute("type", "owneditemmark"),
						new XElement("Mode", mode)));
			});
		return new("arm.capability." + school, content);
	}
}
