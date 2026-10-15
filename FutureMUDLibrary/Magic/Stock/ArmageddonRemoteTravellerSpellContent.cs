#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public static class ArmageddonRemoteTravellerSpellContent
{
	public const string JoinKey = "arm.spell.join_traveller";
	public const string DrawKey = "arm.spell.draw_traveller";
	public static ArmageddonCanonicalSpellContribution Join(long restrictedTag, IReadOnlyList<long> teleportationRanks) =>
		Create(false, restrictedTag, teleportationRanks.Select((tag, rank) => (rank, tag)).ToArray(), null);
	public static ArmageddonCanonicalSpellContribution Draw(long restrictedTag, IReadOnlyList<(int Rank, long Tag)> conjurationRanks, long resistanceTrait) =>
		Create(true, restrictedTag, conjurationRanks, resistanceTrait);
	private static ArmageddonCanonicalSpellContribution Create(bool draw, long restrictedTag,
		IReadOnlyList<(int Rank, long Tag)> ranks, long? resistance)
	{
		if (restrictedTag <= 0 || ranks.Count < 1 || ranks.Any(x => x.Tag <= 0 || x.Rank is < 0 or > 6) || resistance is <= 0)
			throw new ArgumentException("Remote travel requires exact room restrictions, material ranks and an optional resistance binding.");
		var key = draw ? DrawKey : JoinKey; var name = draw ? "Draw Traveller" : "Join Traveller"; var minimum = draw ? 50 : 7;
		return new("arm.capability.wind", new(key, name,
			(draw ? "Resisted native displacement of one recognised remote traveller to the caster's exact current room and layer. " :
				"Move the caster to one recognised remote traveller's exact current room and layer. ") +
			"Use one verified personal identity dub for a currently visible primary physical traveller; consent is not required. " +
			"Both ends must be on the same plane. The target's room must be outdoors and both endpoints must pass native wards and the explicit restricted-room tag. " +
			(draw ? "Grades one through six require a permitted native path of at most 20*grade edges; grade seven retains the source range exception. " :
				"Grades one through six require a permitted native path shorter than 20*grade edges; grade seven retains the source range exception. ") +
			"Consume one directly carried " + (draw ? "Conjuration" : "Teleportation") + " component of rank at least max(grade-2,0). " +
			"Setting-specific criminal diversion is represented by configured refusal tags and native wards. Riders, mounts and temporary/vehicle/route endpoints refuse. " +
			"Only named Say casting is authored; quiet, area and carriers remain disabled.", "0", minimum,
			"$0 call|calls upon the wind to reach $1.", (resource, cost, filter) =>
			{
				var definition = ArmageddonUtilitySpellContent.Definition(key, "recognisedcharacter", resource, cost, filter, 40, minimum,
					new XElement("Effect", new XAttribute("type", draw ? "drawtraveller" : "jointraveller"), new XAttribute("version", 1),
						new XElement("RestrictedRoomTag", restrictedTag)));
				var rankDefinition = new XElement("GradeRank", new XAttribute("offset", -2),
					draw ? new XAttribute("selection", "explicit") : null,
					ranks.Select(x => new XElement("Rank", new XAttribute("minimum", x.Rank), new XAttribute("tag", x.Tag))));
				definition.Element("Plan")!.Add(new XElement("Phase", new XElement("Action", new XAttribute("state", "consumed"),
					new XAttribute("tag", draw ? 0 : ranks[0].Tag), new XAttribute("secondtag", 0), new XAttribute("quantity", 1),
					new XAttribute("carriedonly", true), new XAttribute("originalreference", draw ? "armageddon.draw.conjuration" : "armageddon.join.teleportation"), rankDefinition)));
				return definition;
			}, ResistingTraitId: resistance, ResistingDifficulty: resistance is null ? null : Difficulty.Normal));
	}
}
