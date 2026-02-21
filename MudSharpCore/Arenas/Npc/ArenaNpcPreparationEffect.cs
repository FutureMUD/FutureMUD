#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.Framework;

namespace MudSharp.Arenas;

public sealed class ArenaNpcPreparationEffect : Effect
{
	private readonly List<ArenaNpcItemSnapshot> _items = new();

	public ArenaNpcPreparationEffect(ICharacter owner, long eventId, bool resurrectOnReturn)
		: base(owner)
	{
		EventId = eventId;
		OriginalLocation = owner.Location;
		OriginalRoomLayer = owner.RoomLayer;
		ResurrectOnReturn = resurrectOnReturn;
		IsParticipating = false;
	}

	public ArenaNpcPreparationEffect(XElement definition, IPerceivable owner)
		: base(definition, owner)
	{
		var element = definition.Element("Effect") ?? throw new ArgumentException("Invalid arena NPC preparation effect.");
		EventId = long.Parse(element.Attribute("EventId")?.Value ?? "0");
		var locationId = long.Parse(element.Attribute("LocationId")?.Value ?? "0");
		OriginalLocation = locationId > 0 ? owner.Gameworld.Cells.Get(locationId) : null;
		OriginalRoomLayer = (RoomLayer)int.Parse(element.Attribute("RoomLayer")?.Value ?? "0");
		ResurrectOnReturn = bool.Parse(element.Attribute("ResurrectOnReturn")?.Value ?? bool.FalseString);
		IsParticipating = bool.Parse(element.Attribute("IsParticipating")?.Value ?? bool.FalseString);

		var items = element.Element("Items");
		if (items is null)
		{
			return;
		}

		foreach (var itemElement in items.Elements("Item"))
		{
			var itemId = long.Parse(itemElement.Attribute("Id")?.Value ?? "0");
			var item = owner.Gameworld.TryGetItem(itemId, true);
			if (item is null)
			{
				continue;
			}

			var state = (InventoryState)int.Parse(itemElement.Attribute("State")?.Value ?? "0");
			var wearProfileId = long.Parse(itemElement.Attribute("WearProfileId")?.Value ?? "0");
			var bodypartId = long.Parse(itemElement.Attribute("BodypartId")?.Value ?? "0");
			_items.Add(new ArenaNpcItemSnapshot(item, state,
				wearProfileId > 0 ? wearProfileId : null,
				bodypartId > 0 ? bodypartId : null));
		}
	}

	public long EventId { get; }
	public ICell? OriginalLocation { get; }
	public RoomLayer OriginalRoomLayer { get; }
	public bool ResurrectOnReturn { get; }
	public bool IsParticipating { get; private set; }

	public IEnumerable<ArenaNpcItemSnapshot> Items => _items;

	protected override string SpecificEffectType => IsParticipating ? "ArenaNpcParticipation" : "ArenaNpcPreparation";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return IsParticipating ? "Arena NPC Participation" : "Arena NPC Preparation";
	}

	public void MarkParticipating()
	{
		IsParticipating = true;
	}

	public void MarkPreparing()
	{
		IsParticipating = false;
	}

	public void CaptureItem(IGameItem item, InventoryState state, long? wearProfileId, long? bodypartId)
	{
		if (item is null)
		{
			return;
		}

		_items.Add(new ArenaNpcItemSnapshot(item, state, wearProfileId, bodypartId));
	}

	public void ClearCapturedItems()
	{
		_items.Clear();
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("ArenaNpcPreparation", (effect, owner) => new ArenaNpcPreparationEffect(effect, owner));
		RegisterFactory("ArenaNpcParticipation", (effect, owner) => new ArenaNpcPreparationEffect(effect, owner));
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XAttribute("EventId", EventId),
			new XAttribute("LocationId", OriginalLocation?.Id ?? 0),
			new XAttribute("RoomLayer", (int)OriginalRoomLayer),
			new XAttribute("ResurrectOnReturn", ResurrectOnReturn),
			new XAttribute("IsParticipating", IsParticipating),
			new XElement("Items",
				from item in _items
				select new XElement("Item",
					new XAttribute("Id", item.Item.Id),
					new XAttribute("State", (int)item.State),
					new XAttribute("WearProfileId", item.WearProfileId ?? 0),
					new XAttribute("BodypartId", item.BodypartId ?? 0))));
	}

	public readonly record struct ArenaNpcItemSnapshot(
		IGameItem Item,
		InventoryState State,
		long? WearProfileId,
		long? BodypartId);
}
