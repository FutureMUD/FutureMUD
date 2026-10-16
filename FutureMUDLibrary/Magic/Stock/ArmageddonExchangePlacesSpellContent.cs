#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic;

public static class ArmageddonExchangePlacesSpellContent
{
	public const string Key = "arm.spell.exchange_places";
	public static ArmageddonCanonicalSpellContribution Create(long restrictedTag, long populatedTag,
		IReadOnlyList<long> teleportationRanks, IReadOnlyList<long> alterationRanks)
	{
		if (restrictedTag <= 0 || populatedTag <= 0 || teleportationRanks.Count != 6 || alterationRanks.Count != 6 ||
			teleportationRanks.Concat(alterationRanks).Any(x => x <= 0) || teleportationRanks.Concat(alterationRanks).Distinct().Count() != 12)
			throw new ArgumentException("Exchange Places requires two room tags and both six-rank component families.");
		var retained = teleportationRanks.ToArray(); var consumed = alterationRanks.ToArray();
		var content = new ArmageddonUtilitySpellContent(Key, "Exchange Places",
			"Exchange the exact caster and one active outbound mind-contact traveller once. Both permanent endpoints must remain on the same plane; " +
			"the target must be outdoors, non-staff and outside the explicit populated destination tag. The restricted-room tag and native magic wards protect both ends. " +
			"Retain a directly carried Teleportation component or one accessible immediately inside an open carried container; consume one separate directly carried Alteration unit, each rank max(grade-2,0). Higher descendants qualify. " +
			"The two people exchange room and layer; mounts, riders, temporary/vehicle/route rooms and identical positions refuse before payment. " +
			"A failed second native move attempts to restore only the exact pair; uncertain outcomes are reported without mastery. " +
			"The ordinary casting roll replaces the source's additional random fizzle. Setting-specific city barriers use builder room tags and native wards; no hardcoded criminal diversion is installed. " +
			"Only named Say casting is authored; quiet, area and carriers remain disabled.",
			"0", 33, "$0 call|calls upon the wind to exchange places with $1.",
			(resource, cost, filter) =>
			{
				var definition = ArmageddonUtilitySpellContent.Definition(Key, "mindcontactcharacter", resource, cost, filter, 30, 33,
					new XElement("Effect", new XAttribute("type", "exchangeplaces"), new XAttribute("version", 1),
						new XElement("RestrictedRoomTag", restrictedTag), new XElement("PopulatedRoomTag", populatedTag),
						new XElement("ConsumedRootTag", consumed[0]), new XElement("RetainedRanks", retained.Select((id, rank) =>
							new XElement("Rank", new XAttribute("number", rank), new XAttribute("tag", id))))));
				definition.Element("Plan")!.Add(new XElement("Phase", new XElement("Action", new XAttribute("state", "consumed"),
					new XAttribute("tag", consumed[0]), new XAttribute("secondtag", 0), new XAttribute("quantity", 1),
					new XAttribute("carriedonly", true), new XAttribute("originalreference", "armageddon.exchange.alteration"),
					new XElement("GradeRank", new XAttribute("offset", -2), consumed.Select((id, rank) =>
						new XElement("Rank", new XAttribute("minimum", rank), new XAttribute("tag", id)))))));
				return definition;
			});
		return new("arm.capability.wind", content);
	}
}
