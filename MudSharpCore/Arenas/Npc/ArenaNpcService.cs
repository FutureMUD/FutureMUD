#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Framework;
using MudSharp.Effects;
using MudSharp.NPC;

namespace MudSharp.Arenas;

public class ArenaNpcService : IArenaNpcService
{
	public ArenaNpcService(IFuturemud gameworld)
	{
		_ = gameworld ?? throw new ArgumentNullException(nameof(gameworld));
	}

	public IEnumerable<ICharacter> AutoFill(IArenaEvent arenaEvent, int sideIndex, int slotsNeeded)
	{
		if (arenaEvent is null)
		{
			throw new ArgumentNullException(nameof(arenaEvent));
		}

		if (slotsNeeded <= 0)
		{
			return Enumerable.Empty<ICharacter>();
		}

		var side = arenaEvent.EventType.Sides.FirstOrDefault(x => x.Index == sideIndex);
		if (side?.NpcLoaderProg is null)
		{
			return Enumerable.Empty<ICharacter>();
		}

		return side.NpcLoaderProg.ExecuteCollection<ICharacter>(arenaEvent, sideIndex, slotsNeeded)
		                     .OfType<INPC>()
		                     .Cast<ICharacter>()
		                     .Take(slotsNeeded)
		                     .ToList();
	}

	public void PrepareNpc(ICharacter npc, IArenaEvent arenaEvent, int sideIndex, ICombatantClass combatantClass)
	{
		if (npc is null)
		{
			throw new ArgumentNullException(nameof(npc));
		}

		if (arenaEvent is null)
		{
			throw new ArgumentNullException(nameof(arenaEvent));
		}

		if (combatantClass is null)
		{
			throw new ArgumentNullException(nameof(combatantClass));
		}

		var effect = npc.CombinedEffectsOfType<ArenaNpcPreparationEffect>()
		                .FirstOrDefault(x => x.EventId == arenaEvent.Id);
                if (effect is null)
                {
                        effect = new ArenaNpcPreparationEffect(npc, arenaEvent.Id, combatantClass.ResurrectNpcOnDeath);
			npc.AddEffect(effect);
		}
		else
		{
			effect.ClearCapturedItems();
		}

		if (!arenaEvent.EventType.BringYourOwn && npc.Body is not null)
		{
			StripToArenaLoadout(npc.Body, effect);
		}

		var waitingCell = SelectArenaCell(arenaEvent.Arena.WaitingCells, sideIndex);
		if (waitingCell is not null)
		{
			npc.Teleport(waitingCell, RoomLayer.GroundLevel, false, false);
		}
	}

	public void ReturnNpc(ICharacter npc, IArenaEvent arenaEvent, bool resurrect)
	{
		if (npc is null)
		{
			throw new ArgumentNullException(nameof(npc));
		}

		if (arenaEvent is null)
		{
			throw new ArgumentNullException(nameof(arenaEvent));
		}

		var effect = npc.CombinedEffectsOfType<ArenaNpcPreparationEffect>()
		                .FirstOrDefault(x => x.EventId == arenaEvent.Id);
		if (effect is null)
		{
			return;
		}

		if (resurrect && npc.State.HasFlag(CharacterState.Dead))
		{
			npc.Body?.Resurrect(effect.OriginalLocation ?? npc.Location ?? npc.Gameworld.Cells.Get(1));
			npc.State = CharacterState.Awake;
		}

		RestoreInventory(npc.Body, effect);

		if (effect.OriginalLocation is not null)
		{
			npc.Teleport(effect.OriginalLocation, effect.OriginalRoomLayer, false, false);
		}

		npc.RemoveEffect(effect, true);
	}

	private static void StripToArenaLoadout(IBody body, ArenaNpcPreparationEffect effect)
	{
		var directItems = body.DirectItems?.OfType<IGameItem>().ToList();
		if (directItems is null || directItems.Count == 0)
		{
			return;
		}

		foreach (var item in directItems)
		{
			var state = DetermineState(body, item);
			var wearProfileId = item.GetItemType<IWearable>()?.CurrentProfile?.Id;
			var bodypartId = body.BodypartLocationOfInventoryItem(item)?.Id;
			effect.CaptureItem(item, state, wearProfileId, bodypartId);
			body.Take(item);
		}
	}

        private static void RestoreInventory(IBody? body, ArenaNpcPreparationEffect effect)
        {
                if (body is null)
                {
                        foreach (var snapshot in effect.Items)
                        {
                                if (snapshot.Item is { } orphanCandidate)
                                {
                                        HandleOrphanedItem(orphanCandidate);
                                }
                        }

                        return;
                }

                foreach (var snapshot in effect.Items)
                {
                        var item = snapshot.Item;
                        if (item is null || item.Deleted)
                        {
                                continue;
                        }

                        body.Get(item, silent: true, ignoreFlags: ItemCanGetIgnore.IgnoreWeight | ItemCanGetIgnore.IgnoreFreeHands);

                        if (HandleOrphanedItem(item))
                        {
                                continue;
                        }

                        switch (snapshot.State)
                        {
                                case InventoryState.Worn:
                                        if (!WearItem(body, item, snapshot.WearProfileId))
                                        {
                                                HandleOrphanedItem(item);
                                        }

                                        break;
                                case InventoryState.Wielded:
                                        if (!WieldItem(body, item, snapshot.BodypartId))
                                        {
                                                HandleOrphanedItem(item);
                                        }

                                        break;
                                case InventoryState.Held:
                                        HandleOrphanedItem(item);
                                        break;
                        }

                        HandleOrphanedItem(item);
                }
        }

        private static bool WearItem(IBody body, IGameItem item, long? wearProfileId)
        {
                var wearable = item.GetItemType<IWearable>();
                if (wearable is null)
                {
                        return false;
                }

                var profile = wearProfileId.HasValue
                        ? wearable.Profiles.FirstOrDefault(x => x.Id == wearProfileId.Value)
                        : wearable.CurrentProfile;

                if (profile is not null)
                {
                        body.Wear(item, profile, null, true);
                }
                else
                {
                        body.Wear(item, null, true);
                }

                return body.WornItems.Contains(item);
        }

        private static bool WieldItem(IBody body, IGameItem item, long? bodypartId)
        {
                var hand = bodypartId.HasValue
                        ? body.Bodyparts.FirstOrDefault(x => x.Id == bodypartId.Value) as IWield
                        : null;

                return body.Wield(item, hand, null, true, ItemCanWieldFlags.IgnoreFreeHands);
        }

        private static InventoryState DetermineState(IBody body, IGameItem item)
        {
                if (body.WornItems.Contains(item))
		{
			return InventoryState.Worn;
		}

		if (body.WieldedItems.Contains(item))
		{
			return InventoryState.Wielded;
                }

                return InventoryState.Held;
        }

        private static bool HandleOrphanedItem(IGameItem item)
        {
                if (item is null || item.Deleted)
                {
                        return true;
                }

                if (item.InInventoryOf is not null || item.Location is not null || item.ContainedIn is not null)
                {
                        return false;
                }

                item.Delete();
                return true;
        }

	private static ICell? SelectArenaCell(IEnumerable<ICell> cells, int sideIndex)
	{
		return cells?.ElementAtOrDefault(sideIndex) ?? cells?.FirstOrDefault();
	}
}
