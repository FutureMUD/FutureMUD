#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>Canonical shelter content; native source and recovery identities are supplied by typed ownership.</summary>
public sealed record ArmageddonShelterSpellContent(string Key, string Name, string NativeCapabilityKey,
	string TemplateKey, SpellShelterKind Kind, double MinimumEnergy, int OpeningSkill)
{
	public ArmageddonUtilitySpellContent Bind(long autobuilderTemplateId, IReadOnlyCollection<long> allowedTerrainIds,
		long fallbackRoomId, long waterPrototypeId = 0, long waterLiquidId = 0, IReadOnlyCollection<long>? wardSchoolIds = null)
	{
		if (autobuilderTemplateId <= 0 || fallbackRoomId <= 0 || allowedTerrainIds.Count == 0 ||
			allowedTerrainIds.Any(x => x <= 0) || allowedTerrainIds.Distinct().Count() != allowedTerrainIds.Count)
			throw new ArgumentException("Shelters require exact positive autobuilder, terrain and permanent fallback identities.");
		if (Kind == SpellShelterKind.SpringHaven ? waterPrototypeId <= 0 || waterLiquidId <= 0 : waterPrototypeId != 0 || waterLiquidId != 0)
			throw new ArgumentException("Only Spring Haven binds its exact finite water prototype and liquid.");
		if (Kind == SpellShelterKind.SeveringRefuge ? wardSchoolIds is null || wardSchoolIds.Count != 7 ||
			wardSchoolIds.Any(x => x <= 0) || wardSchoolIds.Distinct().Count() != 7 : wardSchoolIds is not null)
			throw new ArgumentException("Only Severing Refuge binds exactly seven distinct elemental ward school identities.");
		var config = new SpellShelterConfiguration(Kind, 0, allowedTerrainIds.Order().ToArray(), fallbackRoomId, 300, 1,
			waterPrototypeId, waterLiquidId, Kind == SpellShelterKind.SpringHaven ? 0.5 : 0,
			Ward: wardSchoolIds is null ? null : new(wardSchoolIds.Order().ToArray(), [], MagicInterdictionCoverage.Both),
			AutobuilderTemplateId: autobuilderTemplateId, CapacityByGrade: true);
		return new(Key, Name,
			"Creates one occupied native indoor shelter for 300*grade seconds, with one occupant per selected grade including offline instances. " +
			"Native terrain admission and exact safe recovery bindings govern entry and closure; visitors and their goods are conserved. " +
			(Kind == SpellShelterKind.SpringHaven ? "Its initial finite water is 0.5*grade litres, with no refill on reload. " : "") +
			(Kind == SpellShelterKind.BurrowRefuge ? "The private refuge is one native grid level below its source. " : "") +
			(Kind == SpellShelterKind.SeveringRefuge ? "Its owned fail-mode ward covers incoming and outgoing invocations of the seven explicitly bound elemental schools; native physical hazards continue. " : ""),
			"0", MinimumEnergy, "$0 shape|shapes a sheltered space.",
			(resource, cost, filter) => ArmageddonUtilitySpellContent.Definition(Key, "room", resource, cost, filter,
				OpeningSkill, MinimumEnergy, config.SaveEffect()));
	}

	public static IReadOnlyList<ArmageddonShelterSpellContent> Contributions { get; } = Array.AsReadOnly(new[]
	{
		new ArmageddonShelterSpellContent("arm.spell.spring_haven", "Spring Haven", "arm.capability.water", "arm.roomtemplate.spring_haven", SpellShelterKind.SpringHaven, 7, 40),
		new ArmageddonShelterSpellContent("arm.spell.burrow_refuge", "Burrow Refuge", "arm.capability.earth", "arm.roomtemplate.burrow_refuge", SpellShelterKind.BurrowRefuge, 20, 40),
		new ArmageddonShelterSpellContent("arm.spell.sand_shelter", "Sand Shelter", "arm.capability.earth", "arm.roomtemplate.sand_shelter", SpellShelterKind.SandShelter, 7, 40),
		new ArmageddonShelterSpellContent("arm.spell.severing_refuge", "Severing Refuge", "arm.capability.void", "arm.roomtemplate.severing_refuge", SpellShelterKind.SeveringRefuge, 9, 30)
	});
}
