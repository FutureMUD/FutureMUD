#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public static class ArmageddonWeightedTravelSpellContent
{
	public const string WaystepKey = "arm.spell.waystep";
	public const string BanishKey = "arm.spell.banish_traveller";
	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create(long restricted, long lethal, long urban,
		long wilderness, long elemental, IReadOnlyList<long> elementalPlanes, long resistance)
	{
		if (new[] { restricted, lethal, urban, wilderness, elemental, resistance }.Any(x => x <= 0) ||
			elementalPlanes.Count == 0 || elementalPlanes.Any(x => x <= 0)) throw new ArgumentException("Weighted travel requires exact native destination policy bindings.");
		return new[] { CreateOne(false), CreateOne(true) };
		ArmageddonCanonicalSpellContribution CreateOne(bool banish)
		{
			var key = banish ? BanishKey : WaystepKey; var name = banish ? "Banish Traveller" : "Waystep";
			return new("arm.capability.wind", new(key, name,
				(banish ? "After native resistance, displace one local physical traveller to a permitted weighted destination. " :
					"Displace the caster to a permitted weighted destination, or the live room of one exact legally owned Anchor Rune. ") +
				"Material-world pools use authored urban/wilderness group weights 10/90, then uniform native zones and rooms within the selected group. " +
				"The separate elemental route uses only its explicit same-plane pool. Builders map their world using the owned pool tags; unconfigured or empty pools refuse before payment. " +
				"Restricted, lethal, indoor, temporary, vehicle, route, cross-plane and warded endpoints refuse. The selected native layer is preserved. " +
				"Pool selection draws only once after payment, never during quotes or retries. " +
				(banish ? "The source's rune/no-save exception is refused because no native save-waiver authority is bound. " : "A carried rune cannot stand in for a remote destination. ") +
				"Setting-specific city diversion uses configured refusal tags and wards. Only named Say casting is authored; quiet, area and carriers remain disabled.",
				"0", 7, "$0 call|calls upon the wind to carry $1 away.", (resource, cost, filter) =>
					ArmageddonUtilitySpellContent.Definition(key, banish ? "banishtraveller" : "waystepdestination", resource, cost, filter, 40, 7,
						new XElement("Effect", new XAttribute("type", banish ? "banishtraveller" : "waystep"), new XAttribute("version", 1),
							new XElement("RestrictedRoomTag", restricted), new XElement("LethalRoomTag", lethal),
							new XElement("UrbanPoolTag", urban), new XElement("WildPoolTag", wilderness), new XElement("ElementalPoolTag", elemental),
							new XElement("UrbanWeight", 10), new XElement("WildWeight", 90),
							new XElement("ElementalPlanes", elementalPlanes.Select(id => new XElement("Plane", id))))),
				ResistingTraitId: banish ? resistance : null, ResistingDifficulty: banish ? Difficulty.Normal : null));
		}
	}
}
