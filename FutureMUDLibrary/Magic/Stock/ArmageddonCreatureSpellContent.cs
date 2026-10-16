#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>A pure canonical creature contribution, hydrated only from exact native grade-template bindings.</summary>
public sealed record ArmageddonCreatureSpellContent(string Key, string Name, string NativeCapabilityKey,
	string NpcFamily, string ControllerPurpose, double MinimumEnergy, int OpeningSkill,
	bool GuardCaster = false, bool CountByGrade = false, SpellNpcMissionKind Mission = SpellNpcMissionKind.None)
{
	public ArmageddonUtilitySpellContent Bind(IReadOnlyDictionary<int, long> templates)
	{
		if (templates.Count != 7 || Enumerable.Range(1, 7).Any(grade => !templates.TryGetValue(grade, out var id) || id <= 0) ||
			templates.Values.Distinct().Count() != 7)
			throw new ArgumentException("Creature stock requires seven distinct positive native template identities.", nameof(templates));
		// Copy the resolved values: later caller changes cannot rewrite an authored contribution.
		var bindings = templates.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
		return new(Key, Name,
			$"Summons {(CountByGrade ? "one creature per requested grade" : "one creature")} from the exact requested-grade native template. " +
			"The temporary creature has its own native anatomy and creator bond, lasts 120*grade seconds and conserves foreign possessions on retirement. " +
			(Mission == SpellNpcMissionKind.None ? "Its native controller supplies the configured companion behavior." :
			 $"Its {Mission.ToString().ToLowerInvariant()} mission uses explicit exact character and parcel inputs where required."),
			"0", MinimumEnergy, "$0 shape|shapes a temporary creature from magic.",
			(resource, cost, filter) =>
			{
				var definition = ArmageddonUtilitySpellContent.Definition(Key,
					Mission == SpellNpcMissionKind.None ? "room" : "roommission", resource, cost, filter, OpeningSkill, MinimumEnergy,
					new XElement("Effect", new XAttribute("type", "createnpc"), new XElement("NPCPrototypeId", bindings[1]),
						new XElement("OnLoadProg", 0),
						new XElement("GradeTemplates", new XAttribute("version", 1), bindings.Select(x =>
							new XElement("Template", new XAttribute("grade", x.Key), x.Value))),
						Mission == SpellNpcMissionKind.None ? null : new XElement("Mission", new XAttribute("version", 1), new XAttribute("kind", Mission)),
						new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", "TemporaryCleanup"),
							new XElement("Family", NpcFamily.Replace('_', '-')), new XElement("Seconds", "120*grade"),
							new XElement("Count", CountByGrade ? "grade" : "1"), new XElement("GuardCaster", GuardCaster))));
				if (Mission != SpellNpcMissionKind.None)
				{
					definition.Element("Trigger")!.SetAttributeValue("version", 1);
					definition.Element("Trigger")!.Add(new XElement("Mission", Mission));
				}
				return definition;
			});
	}

	public static IReadOnlyList<ArmageddonCreatureSpellContent> Contributions =>
	[
		new("arm.spell.earth_mount", "Earth Mount", "arm.capability.earth", "earth_mount", "MountAI", 20, 40),
		new("arm.spell.clay_sentinel", "Clay Sentinel", "arm.capability.earth", "clay_sentinel", "owned_guard", 10, 30, true),
		new("arm.spell.air_guardian", "Air Guardian", "arm.capability.wind", "air_guardian", "owned_guard", 33, 60, true, true),
		new("arm.spell.seeking_wind", "Seeking Wind", "arm.capability.wind", "seeking_wind", "owned_seek", 20, 40, Mission: SpellNpcMissionKind.Seek),
		new("arm.spell.wind_courier", "Wind Courier", "arm.capability.wind", "wind_courier", "owned_courier", 7, 40, Mission: SpellNpcMissionKind.Courier),
		new("arm.spell.haunting_shape", "Haunting Shape", "arm.capability.shadow", "haunting_shape", "owned_haunt", 25, 40, Mission: SpellNpcMissionKind.Haunt),
		new("arm.spell.call_outsider", "Call Outsider", "arm.capability.void", "outsider", "owned_outsider", 50, 40)
	];
}
