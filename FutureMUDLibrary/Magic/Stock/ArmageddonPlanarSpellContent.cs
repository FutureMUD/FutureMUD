#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Planes;

namespace MudSharp.Magic;

/// <summary>Fixed, editable native planar defaults. The caller resolves the owned plane identity.</summary>
public static class ArmageddonPlanarSpellContent
{
	public const string ShadowPassageKey = "arm.spell.shadow_passage";
	public const string CrossTheVeilKey = "arm.spell.cross_the_veil";

	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create(long shadowPlaneId)
	{
		if (shadowPlaneId <= 0) throw new ArgumentOutOfRangeException(nameof(shadowPlaneId));
		return
		[
			new("arm.capability.shadow", Content(ShadowPassageKey, "Shadow Passage", "planarstate", 20,
				"An ethereal presence on the authored Shadow plane, with physical contact and inventory suspended.",
				NonCorporeal(shadowPlaneId))),
			new("arm.capability.void", Content(CrossTheVeilKey, "Cross the Veil", "planeshift", 10,
				"A manifested presence on the authored Shadow plane, with native planar interaction rules.",
				Manifested(shadowPlaneId)))
		];
	}

	private static ArmageddonUtilitySpellContent Content(string key, string name, string type, double minimum,
		string policy, PlanarPresenceDefinition presence) => new(key, name,
			$"{policy} The one selected overlay lasts 60*external grade seconds. It grants no magical-barrier " +
			"bypass or unrestricted plane choice. Its native plane and state remain builder editable; the parent owns expiry and saved remaining time.",
			"60*grade", minimum, "$0 shape|shapes a passage through the veil around $1.",
			(resource, cost, filter) => ArmageddonUtilitySpellContent.Definition(key, "character", resource, cost, filter,
				40, minimum, new XElement("Effect", new XAttribute("type", type), presence.SaveToXml())));

	private static PlanarPresenceDefinition NonCorporeal(long planeId) => Presence(planeId, false);
	private static PlanarPresenceDefinition Manifested(long planeId) => Presence(planeId, true);

	private static PlanarPresenceDefinition Presence(long planeId, bool manifested)
	{
		var plane = new[] { planeId };
		var interactions = Enum.GetValues<PlanarInteractionKind>().ToDictionary(kind => kind,
			kind => (IEnumerable<long>)(manifested || kind is PlanarInteractionKind.Observe or
				PlanarInteractionKind.Hear or PlanarInteractionKind.Speak or PlanarInteractionKind.Magic
					? plane : Array.Empty<long>()));
		return new PlanarPresenceDefinition(plane, plane, plane, interactions,
			suspendsPhysicalContact: !manifested, propagatesInventory: manifested,
			canCrossClosedDoors: false, canCrossMagicalBarriers: false,
			visibleToEtherealPerception: !manifested, playerCanManifest: false, playerCanDissipate: false,
			transitionProfile: manifested ? "manifested" : "noncorporeal");
	}
}
