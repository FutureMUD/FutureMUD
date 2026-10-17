#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Magic;

public enum SpellShelterKind { SpringHaven, BurrowRefuge, SandShelter, SeveringRefuge }

/// <summary>Explicit native bindings. Durations are real seconds; quantities are litres.</summary>
public sealed record SpellShelterConfiguration(SpellShelterKind Kind, long TemplateRoomId,
	IReadOnlyCollection<long> AllowedTerrainIds, long FallbackRoomId, double SecondsPerGrade,
	int MaximumOccupants, long WaterPrototypeId = 0, long LiquidId = 0, double LitresPerGrade = 0, int UndergroundDepth = 1,
	SpellShelterWardConfiguration? Ward = null, long AutobuilderTemplateId = 0, bool CapacityByGrade = false)
{
	public int CapacityForGrade(int grade) => CapacityByGrade ? MaximumOccupants * grade : MaximumOccupants;

	public XElement SaveEffect() => new("Effect", new XAttribute("type", "createshelter"), new XAttribute("version", 1),
		new XElement("Kind", Kind), new XElement("TemplateRoom", TemplateRoomId), new XElement("FallbackRoom", FallbackRoomId),
		new XElement("SecondsPerGrade", SecondsPerGrade), new XElement("MaximumOccupants", MaximumOccupants),
		AllowedTerrainIds.Order().Select(x => new XElement("Terrain", x)), new XElement("WaterPrototype", WaterPrototypeId),
		new XElement("Liquid", LiquidId), new XElement("LitresPerGrade", LitresPerGrade),
		new XElement("UndergroundDepth", UndergroundDepth), Ward?.Save(),
		AutobuilderTemplateId == 0 ? null : new XElement("AutobuilderTemplate", AutobuilderTemplateId),
		CapacityByGrade ? new XElement("CapacityByGrade", true) : null);
}

public interface ISpellOwnedShelterService
{
	string? AdmissionError(ICharacter caster, IRoom anchor, SpellShelterConfiguration configuration, int grade);
	IRoom Create(ICharacter caster, IRoom anchor, SpellShelterConfiguration configuration, SpellLifecycleOrigin origin);
	/// <summary>Retain the callback-free paid frame through native admission and private topology staging.</summary>
	IRoom CreatePrepared(ICharacter caster, IRoom anchor, SpellShelterConfiguration configuration,
		SpellLifecycleOrigin origin, Func<bool> inputsCurrent) =>
		throw new InvalidOperationException("This shelter service does not support prepared physical frames.");
	/// <summary>Only declared shelter rooms are inspected; ordinary movement does not query persistence.</summary>
	bool CanEnter(IRoom room, IPerceiver entrant);
	/// <summary>Native login alone can restore an exact persisted resident of a closed shelter.</summary>
	bool CanReconnect(IRoom room, ICharacter resident);
	bool OwnsRoom(long roomId);
	bool OwnsExit(long exitId);
	bool ReturnRejectedEntrant(IRoom room, ICharacter entrant);
	bool RequestRetirement(long roomId, SpellRetirementReason reason);
	int ReconcileRetirements(DateTime nowUtc, int limit = 100);
}
