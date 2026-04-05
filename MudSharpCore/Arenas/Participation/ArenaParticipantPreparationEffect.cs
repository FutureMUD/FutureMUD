#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Arenas;

public sealed class ArenaParticipantPreparationEffect : Effect
{
    private readonly List<ArenaParticipantItemSnapshot> _items = new();
    private IArenaEvent? _arenaEvent;

    public ArenaParticipantPreparationEffect(ICharacter owner, long eventId)
        : base(owner)
    {
        EventId = eventId;
    }

    public ArenaParticipantPreparationEffect(XElement definition, IPerceivable owner)
        : base(definition, owner)
    {
        XElement element = definition.Element("Effect") ?? throw new ArgumentException("Invalid arena participant preparation effect.");
        EventId = long.Parse(element.Attribute("EventId")?.Value ?? "0");

        XElement? items = element.Element("Items");
        if (items is null)
        {
            return;
        }

        foreach (XElement itemElement in items.Elements("Item"))
        {
            long itemId = long.Parse(itemElement.Attribute("Id")?.Value ?? "0");
            IGameItem? item = owner.Gameworld.TryGetItem(itemId, true);
            if (item is null)
            {
                continue;
            }

            InventoryState state = (InventoryState)int.Parse(itemElement.Attribute("State")?.Value ?? "0");
            long wearProfileId = long.Parse(itemElement.Attribute("WearProfileId")?.Value ?? "0");
            long bodypartId = long.Parse(itemElement.Attribute("BodypartId")?.Value ?? "0");
            _items.Add(new ArenaParticipantItemSnapshot(item, state,
                wearProfileId > 0 ? wearProfileId : null,
                bodypartId > 0 ? bodypartId : null));
        }
    }

    public long EventId { get; }

    public IEnumerable<ArenaParticipantItemSnapshot> Items => _items;

    protected override string SpecificEffectType => "ArenaParticipantPreparation";

    public override bool SavingEffect => true;

    public override string Describe(IPerceiver voyeur)
    {
        string eventName = DescribeEventName();
        return $"Arena participant inventory snapshot for {eventName}.";
    }

    public void CaptureItem(IGameItem item, InventoryState state, long? wearProfileId, long? bodypartId)
    {
        if (item is null)
        {
            return;
        }

        _items.Add(new ArenaParticipantItemSnapshot(item, state, wearProfileId, bodypartId));
    }

    public void ClearCapturedItems()
    {
        _items.Clear();
    }

    public static void InitialiseEffectType()
    {
        RegisterFactory("ArenaParticipantPreparation", (effect, owner) => new ArenaParticipantPreparationEffect(effect, owner));
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XAttribute("EventId", EventId),
            new XElement("Items",
                from item in _items
                select new XElement("Item",
                    new XAttribute("Id", item.Item.Id),
                    new XAttribute("State", (int)item.State),
                    new XAttribute("WearProfileId", item.WearProfileId ?? 0),
                    new XAttribute("BodypartId", item.BodypartId ?? 0))));
    }

    private IArenaEvent? ResolveEvent()
    {
        if (_arenaEvent is not null)
        {
            return _arenaEvent;
        }

        _arenaEvent = Gameworld?.CombatArenas.SelectMany(x => x.ActiveEvents)
            .FirstOrDefault(x => x.Id == EventId);
        return _arenaEvent;
    }

    private string DescribeEventName()
    {
        IArenaEvent? arenaEvent = ResolveEvent();
        if (arenaEvent is null)
        {
            return $"arena event #{EventId}";
        }

        return arenaEvent.Name;
    }

    public readonly record struct ArenaParticipantItemSnapshot(
        IGameItem Item,
        InventoryState State,
        long? WearProfileId,
        long? BodypartId);
}
