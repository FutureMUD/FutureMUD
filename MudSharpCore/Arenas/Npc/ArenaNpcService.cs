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
		if (side is null)
		{
			return Enumerable.Empty<ICharacter>();
		}

		var candidates = new List<ICharacter>();
		var activeNpcIds = arenaEvent.Arena.ActiveEvents
			.SelectMany(x => x.Participants)
			.Select(x => x.Character?.Id)
			.Where(x => x.HasValue)
			.Select(x => x.Value)
			.ToHashSet();

		var stableCells = arenaEvent.Arena.NpcStablesCells?.ToList() ?? [];
		if (stableCells.Count > 0)
		{
			var stableNpcs = stableCells
				.SelectMany(cell => cell.Characters)
				.OfType<ICharacter>()
				.Where(npc => !npc.IsPlayerCharacter)
				.Where(npc => npc is INPC)
				.Where(npc => npc.State.IsAble())
				.Where(npc => !activeNpcIds.Contains(npc.Id))
				.Where(npc => IsEligibleForSide(npc, side));

			foreach (var npc in stableNpcs)
			{
				candidates.Add(npc);
				if (candidates.Count >= slotsNeeded)
				{
					return candidates;
				}
			}
		}

		if (side.NpcLoaderProg is null)
		{
			return candidates;
		}

		var remaining = slotsNeeded - candidates.Count;
		if (remaining <= 0)
		{
			return candidates;
		}

		var generated = side.NpcLoaderProg.ExecuteCollection<ICharacter>(
				ArenaProgParameters.BuildNpcLoaderArguments(arenaEvent, sideIndex, remaining))
			.OfType<INPC>()
			.Cast<ICharacter>()
			.Where(npc => npc.State.IsAble())
			.Where(npc => !activeNpcIds.Contains(npc.Id))
			.Where(npc => IsEligibleForSide(npc, side))
			.ToList();

		foreach (var npc in generated)
		{
			if (candidates.Count >= slotsNeeded)
			{
				break;
			}

			if (stableCells.Count > 0)
			{
				var stableCell = SelectArenaCell(stableCells, sideIndex);
				if (stableCell is not null)
				{
					npc.Teleport(stableCell, RoomLayer.GroundLevel, false, false);
				}
			}

			candidates.Add(npc);
		}

		return candidates;
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
		effect.MarkPreparing();

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

	public void ReturnNpc(ICharacter npc, IArenaEvent arenaEvent, bool resurrect, bool fullRestoreBeforeInventory)
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

		var returnLocation = effect.OriginalLocation ?? npc.Location ?? npc.Gameworld.Cells.Get(1);
		if (resurrect && npc.State.HasFlag(CharacterState.Dead))
		{
			npc.Resurrect(returnLocation);
		}

		if (npc.State.HasFlag(CharacterState.Dead))
		{
			npc.RemoveEffect(effect, true);
			return;
		}

		if (fullRestoreBeforeInventory && npc.Body is { } restoreBody)
		{
			RestoreBody(restoreBody);
		}

		RestoreInventory(npc, effect);

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

	private static void RestoreInventory(ICharacter npc, ArenaNpcPreparationEffect effect)
	{
		var body = npc.Body;
		if (body is null)
		{
			foreach (var snapshot in effect.Items)
			{
				if (snapshot.Item is { } item && !item.Deleted)
				{
					PlaceItemNearCharacter(item, npc);
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

			try
			{
				body.Get(item, silent: true, ignoreFlags: ItemCanGetIgnore.IgnoreWeight);
			}
			catch
			{
				PlaceItemNearCharacter(item, npc);
				continue;
			}

			if (!ReferenceEquals(item.InInventoryOf, body))
			{
				PlaceItemNearCharacter(item, npc);
				continue;
			}

			switch (snapshot.State)
			{
				case InventoryState.Worn:
					if (!WearItem(body, item, snapshot.WearProfileId))
					{
						PlaceItemNearCharacter(item, npc);
					}

					break;
				case InventoryState.Wielded:
					if (!WieldItem(body, item, snapshot.BodypartId))
					{
						PlaceItemNearCharacter(item, npc);
					}

					break;
				case InventoryState.Held:
					break;
			}

			if (!ItemAnchored(item))
			{
				PlaceItemNearCharacter(item, npc);
			}
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

	private static void RestoreBody(IBody body)
	{
		body.HeldBreathTime = TimeSpan.Zero;
		body.RestoreAllBodypartsOrgansAndBones();
		body.Sober();
		body.CureAllWounds();
		body.CurrentStamina = body.MaximumStamina;
		body.CurrentBloodVolumeLitres = body.TotalBloodVolumeLitres;
		body.EndHealthTick();
	}

	private static bool ItemAnchored(IGameItem item)
	{
		return item.InInventoryOf is not null || item.Location is not null || item.ContainedIn is not null;
	}

	private static void PlaceItemNearCharacter(IGameItem item, ICharacter character)
	{
		item.ContainedIn?.Take(item);
		item.InInventoryOf?.Take(item);
		item.Location?.Extract(item);

		if (character.Location is not { } location)
		{
			return;
		}

		item.RoomLayer = character.RoomLayer;
		location.Insert(item, true);
	}

	private static ICell? SelectArenaCell(IEnumerable<ICell> cells, int sideIndex)
	{
		return cells?.ElementAtOrDefault(sideIndex) ?? cells?.FirstOrDefault();
	}

	private static bool IsEligibleForSide(ICharacter npc, IArenaEventTypeSide side)
	{
		foreach (var combatantClass in side.EligibleClasses)
		{
			try
			{
				if (combatantClass.EligibilityProg.Execute<bool?>(npc) != false)
				{
					return true;
				}
			}
			catch
			{
			}
		}

		return false;
	}
}
