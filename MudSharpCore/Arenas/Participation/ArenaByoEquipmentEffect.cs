#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Arenas;

public sealed class ArenaByoEquipmentEffect : Effect
{
	public ArenaByoEquipmentEffect(IGameItem owner, long eventId, long ownerCharacterId)
		: base(owner)
	{
		EventId = eventId;
		OwnerCharacterId = ownerCharacterId;
	}

	public ArenaByoEquipmentEffect(XElement definition, IPerceivable owner)
		: base(definition, owner)
	{
		var element = definition.Element("Effect")
		              ?? throw new ArgumentException("Invalid arena BYO equipment effect.");
		EventId = long.Parse(element.Attribute("EventId")?.Value ?? "0");
		OwnerCharacterId = long.Parse(element.Attribute("OwnerCharacterId")?.Value ?? "0");
	}

	public long EventId { get; }
	public long OwnerCharacterId { get; }

	protected override string SpecificEffectType => "ArenaByoEquipment";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Arena BYO equipment for character #{OwnerCharacterId.ToString("N0", voyeur)} in event #{EventId.ToString("N0", voyeur)}.";
	}

	public bool Matches(IArenaEvent arenaEvent)
	{
		return arenaEvent is not null && arenaEvent.Id == EventId;
	}

	public bool Matches(IArenaEvent arenaEvent, ICharacter owner)
	{
		return arenaEvent is not null &&
		       owner is not null &&
		       arenaEvent.Id == EventId &&
		       owner.Id == OwnerCharacterId;
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("ArenaByoEquipment", (effect, owner) => new ArenaByoEquipmentEffect(effect, owner));
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XAttribute("EventId", EventId),
			new XAttribute("OwnerCharacterId", OwnerCharacterId));
	}
}
