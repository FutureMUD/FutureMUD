using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character.Name;
using MudSharp.Economy.Currency;
using MudSharp.Models;
using MudSharp.RPG.Law;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	private readonly HashSet<IProsthetic> _prosthetics = new();
	private bool _inventoryChanged;

	private bool _prostheticsChanged;

	public bool ProstheticsChanged
	{
		get => _prostheticsChanged;
		set
		{
			if (!_noSave)
			{
				if (value && !_prostheticsChanged)
				{
					Changed = true;
				}

				_prostheticsChanged = value;
			}
		}
	}

	public bool InventoryChanged
	{
		get => _inventoryChanged;
		set
		{
			if (!_noSave)
			{
				if (value && !_inventoryChanged)
				{
					Changed = true;
				}

				_inventoryChanged = value;
			}

			RecalculateItemHelpers();
		}
	}

	public event InventoryChangeEvent OnInventoryChange;
	public IEnumerable<IProsthetic> Prosthetics => _prosthetics;

	public void InstallProsthetic(IProsthetic prosthetic)
	{
		_prosthetics.Add(prosthetic);
		prosthetic.InstallProsthetic(this);
		RecalculatePartsAndOrgans();
		ReevaluateLimbAndPartDamageEffects();
		RecalculateItemHelpers();
		ProstheticsChanged = true;
	}

	public void RemoveProsthetic(IProsthetic prosthetic)
	{
		_prosthetics.Remove(prosthetic);
		prosthetic.RemoveProsthetic();
		RecalculatePartsAndOrgans();
		ReevaluateLimbAndPartDamageEffects();
		RecalculateItemHelpers();
		ProstheticsChanged = true;
	}

	protected void SaveProsthetics(MudSharp.Models.Body body)
	{
		FMDB.Context.BodiesProsthetics.RemoveRange(body.BodiesProsthetics);
		foreach (var item in _prosthetics)
		{
			body.BodiesProsthetics.Add(new BodiesProsthetics { Body = body, ProstheticId = item.Parent.Id });
		}

		ProstheticsChanged = false;
	}

	private void SaveInventory(MudSharp.Models.Body body)
	{
		var order = 0;
		FMDB.Context.BodiesGameItems.RemoveRange(body.BodiesGameItems);
		foreach (var item in DirectWornItems.SelectNotNull(x => x.GetItemType<IWearable>()))
		{
			// This line is necessary because if somebody removes something, drops it, and someone else gets and wears it quickly, 
			// it could cause an out-of-order save exception relating to duplicate Bodies_GameItems entries
			var oldItem = FMDB.Context.BodiesGameItems.FirstOrDefault(x => x.GameItemId == item.Parent.Id);
			if (oldItem != null)
			{
				FMDB.Context.BodiesGameItems.Remove(oldItem);
			}

			var dbitem = new BodiesGameItems();
			var dbgameitem = FMDB.Context.GameItems.Find(item.Parent.Id);
			if (dbgameitem == null)
			{
				Console.WriteLine(
					$"Warning: no db entry for gameitem in save for character {Actor.Id:N0} ({Actor.PersonalName.GetName(NameStyle.FullName)}) - item {item.Parent.HowSeen(item.Parent, colour: false)} #{item.Parent.Id:N0}");
				Gameworld.SystemMessage(
					$"Warning: no db entry for gameitem in save for character {Actor.Id:N0} ({Actor.PersonalName.GetName(NameStyle.FullName)}) - item {item.Parent.HowSeen(item.Parent, colour: false)} #{item.Parent.Id:N0}",
					x => x.IsAdministrator(PermissionLevel.HighAdmin));
				continue;
			}

			dbitem.BodyId = body.Id;
			dbitem.GameItem = dbgameitem;
			FMDB.Context.BodiesGameItems.Add(dbitem);
			dbitem.EquippedOrder = order++;
			dbitem.WearProfile = item.CurrentProfile.Id;
		}

		foreach (var item in HeldItems)
		{
			// This line is necessary because if somebody removes something, drops it, and someone else gets and wears it quickly, 
			// it could cause an out-of-order save exception relating to duplicate Bodies_GameItems entries
			var oldItem = FMDB.Context.BodiesGameItems.FirstOrDefault(x => x.GameItemId == item.Id);
			if (oldItem != null)
			{
				FMDB.Context.BodiesGameItems.Remove(oldItem);
			}

			var dbitem = new BodiesGameItems
			{
				BodyId = body.Id
			};
			var dbgameitem = FMDB.Context.GameItems.Find(item.Id);
			if (dbgameitem == null)
			{
				Console.WriteLine(
					$"Warning: no db entry for gameitem in save for character {Actor.Id:N0} ({Actor.PersonalName.GetName(NameStyle.FullName)}) - item {item.HowSeen(item, colour: false)} #{item.Id:N0}");
				Gameworld.SystemMessage(
					$"Warning: no db entry for gameitem in save for character {Actor.Id:N0} ({Actor.PersonalName.GetName(NameStyle.FullName)}) - item {item.HowSeen(item, colour: false)} #{item.Id:N0}",
					x => x.IsAdministrator(PermissionLevel.HighAdmin));
				continue;
			}

			dbitem.GameItem = dbgameitem;
			FMDB.Context.BodiesGameItems.Add(dbitem);
			dbitem.EquippedOrder = order++;
		}

		foreach (var item in WieldedItems)
		{
			// This line is necessary because if somebody removes something, drops it, and someone else gets and wears it quickly, 
			// it could cause an out-of-order save exception relating to duplicate Bodies_GameItems entries
			var oldItem = FMDB.Context.BodiesGameItems.FirstOrDefault(x => x.GameItemId == item.Id);
			if (oldItem != null)
			{
				FMDB.Context.BodiesGameItems.Remove(oldItem);
			}

			var dbitem = new BodiesGameItems
			{
				BodyId = body.Id
			};
			var dbgameitem = FMDB.Context.GameItems.Find(item.Id);
			if (dbgameitem == null)
			{
				Console.WriteLine(
					$"Warning: no db entry for gameitem in save for character {Actor.Id:N0} ({Actor.PersonalName.GetName(NameStyle.FullName)}) - item {item.HowSeen(item, colour: false)} #{item.Id:N0}");
				Gameworld.SystemMessage(
					$"Warning: no db entry for gameitem in save for character {Actor.Id:N0} ({Actor.PersonalName.GetName(NameStyle.FullName)}) - item {item.HowSeen(item, colour: false)} #{item.Id:N0}",
					x => x.IsAdministrator(PermissionLevel.HighAdmin));
				continue;
			}

			dbitem.GameItem = dbgameitem;
			FMDB.Context.BodiesGameItems.Add(dbitem);
			dbitem.EquippedOrder = order++;
			dbitem.Wielded = 1;
		}

		_inventoryChanged = false;
		FMDB.Context.SaveChanges();
	}

	public void LoadInventory(MudSharp.Models.Body body)
	{
		var loadedItems = new List<IGameItem>();
		_noSave = true;

		foreach (var item in body.BodiesGameItems.OrderBy(x => x.EquippedOrder))
		{
			var gitem = Gameworld.TryGetItem(item.GameItemId, true);
			if (gitem == null)
			{
				continue;
			}

			if (gitem.ContainedIn != null || gitem.Location != null || gitem.InInventoryOf != null)
			{
				_noSave = false;
				InventoryChanged = true;
				_noSave = true;
				Gameworld.SystemMessage(
					$"Duplicated Item: {gitem.HowSeen(gitem, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {gitem.Id.ToString("N0")}",
					true);
				continue;
			}

			loadedItems.Add(gitem);

			gitem.Get(this);
			if (item.WearProfile.HasValue)
			{
				var profile = Gameworld.WearProfiles.Get(item.WearProfile.Value);
				if (!LoadtimeWear(gitem, profile))
				{
					gitem.Get(null);
					gitem.RoomLayer = RoomLayer;
					Location.Insert(gitem);
				}
			}
			else if (item.Wielded.HasValue && item.Wielded.Value == 1)
			{
				// TODO - save wield flags
				if (!LoadtimeWield(gitem, ItemCanWieldFlags.None))
				{
					gitem.Get(null);
					gitem.RoomLayer = RoomLayer;
					Location.Insert(gitem);
				}
			}
			else
			{
				if (!LoadtimeGet(gitem))
				{
					gitem.Get(null);
					gitem.RoomLayer = RoomLayer;
					Location.Insert(gitem);
				}
			}
		}

		foreach (var item in loadedItems)
		{
			item.FinaliseLoadTimeTasks();
		}

		_noSave = false;
		RecalculateItemHelpers();
	}

	#region IWield Implementation

	private readonly List<Tuple<IGameItem, IWield>> _wieldedItems = new();

	public IEnumerable<IGameItem> WieldedItems
	{
		get { return _wieldedItems.Select(x => x.Item1).Distinct(); }
	}

	public IEnumerable<IGameItem> ItemsInHands => WieldedItems.Concat(HeldItems);

	public IEnumerable<IGameItem> WieldedItemsFor(IBodypart proto)
	{
		return _wieldedItems.Where(x => x.Item2 == proto).Select(x => x.Item1);
	}

	public bool CanWield(IGameItem item, ItemCanWieldFlags flags = ItemCanWieldFlags.None)
	{
		var wieldable = item.GetItemType<IWieldable>();
		if (wieldable is null)
		{
			return false;
		}

		if (!wieldable.CanWield(Actor))
		{
			return false;
		}

		if (_wieldedItems.Any(x => x.Item1 == item) && !flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands))
		{
			return false;
		}

		var openWieldLocs = WieldLocs.Select(x => (Wielder: x, CanWield: x.CanWield(item, this))).Where(x =>
			x.CanWield == IWieldItemWieldResult.Success ||
			(flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands) &&
			 (x.CanWield == IWieldItemWieldResult.AlreadyWielding ||
			  x.CanWield == IWieldItemWieldResult.GrabbingWielderHoldOtherItem))).Select(x => x.Wielder).ToList();

		var locsAndHands = openWieldLocs.Select(x => (Item: x, Hands: x.Hands(item))).ToList();
		if (flags.HasFlag(ItemCanWieldFlags.RequireTwoHands) && locsAndHands.Count < 2)
		{
			return false;
		}

		if (flags.HasFlag(ItemCanWieldFlags.RequireOneHand) && locsAndHands.All(x => x.Hands > 1))
		{
			return false;
		}

		return locsAndHands.Any(x => x.Hands == 1) || locsAndHands.Count > 1;
	}

	public bool CanWield(IGameItem item, IWield specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None)
	{
		var wieldable = item.GetItemType<IWieldable>();
		if (wieldable is null)
		{
			return false;
		}

		if (!wieldable.CanWield(Actor))
		{
			return false;
		}

		if (!flags.HasFlag(ItemCanWieldFlags.RequireTwoHands))
		{
			return (specificHand?.CanWield(item, this) == IWieldItemWieldResult.Success ||
			        flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands)) && CanWield(item, flags);
		}

		if (_wieldedItems.Any(x => x.Item1 == item) && !flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands))
		{
			return false;
		}

		var openWieldLocs = WieldLocs.Where(x => x.CanWield(item, this) == IWieldItemWieldResult.Success ||
		                                         (flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands) &&
		                                          (x.CanWield(item, this) == IWieldItemWieldResult.AlreadyWielding ||
		                                           x.CanWield(item, this) ==
		                                           IWieldItemWieldResult.GrabbingWielderHoldOtherItem))).ToList();
		var locsAndHands = openWieldLocs.Select(x => (Item: x, Hands: x.Hands(item))).ToList();
		if (locsAndHands.All(x => x.Item != specificHand))
		{
			return false;
		}

		if (flags.HasFlag(ItemCanWieldFlags.RequireTwoHands) && locsAndHands.Count < 2)
		{
			return false;
		}

		return locsAndHands.Any(x => x.Hands == 1) || locsAndHands.Count > 1;
	}

	public bool CanUnwield(IGameItem item, bool ignoreFreeHands = false)
	{
		return
			_wieldedItems.Where(x => x.Item1 == item)
			             .Select(x => x.Item2)
			             .All(x => x.CanUnwield(item, this) == IWieldItemUnwieldResult.Success || ignoreFreeHands);
	}

	public string WhyCannotWield(IGameItem item, ItemCanWieldFlags flags = ItemCanWieldFlags.None)
	{
		var wieldable = item.GetItemType<IWieldable>();
		if (wieldable == null)
		{
			return $"{item.HowSeen(this, true)} is not something that you can wield.";
		}

		if (!wieldable.CanWield(Actor))
		{
			return wieldable.WhyCannotWield(Actor);
		}

		if (_wieldedItems.Any(x => x.Item1 == item) && !flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands))
		{
			return $"You are already wielding {item.HowSeen(this)}.";
		}

		var reasons = WieldLocs.Select(x => x.CanWield(item, this)).ToList();
		var openWieldLocs = WieldLocs.Where(x => x.CanWield(item, this) == IWieldItemWieldResult.Success ||
		                                         (flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands) &&
		                                          (x.CanWield(item, this) == IWieldItemWieldResult.AlreadyWielding ||
		                                           x.CanWield(item, this) ==
		                                           IWieldItemWieldResult.GrabbingWielderHoldOtherItem))).ToList();
		var locsAndHands = openWieldLocs.Select(x => (Item: x, Hands: x.Hands(item))).ToList();
		if (flags.HasFlag(ItemCanWieldFlags.RequireTwoHands) && locsAndHands.Any(x => x.Hands == 1))
		{
			return $"You could currently wield {item.HowSeen(Actor)} in one hand, but not in two.";
		}

		if (flags.HasFlag(ItemCanWieldFlags.RequireOneHand) && locsAndHands.Count(x => x.Hands > 1) > 1)
		{
			return $"You could currently wield {item.HowSeen(Actor)} in two hands, but not in one.";
		}

		if (reasons.Any(x => x == IWieldItemWieldResult.Success))
		{
			return reasons.Any(x => x == IWieldItemWieldResult.TooDamaged)
				? string.Format("You need two free, undamaged {1} to wield {0}.", item.HowSeen(this),
					WielderDescriptionPlural)
				: string.Format("You need two free {1} to wield {0}.", item.HowSeen(this),
					WielderDescriptionPlural);
		}

		if (reasons.Any(x => x == IWieldItemWieldResult.TooDamaged))
		{
			return string.Format("Your free {1} are too damaged to wield {0}.", item.HowSeen(this),
				WielderDescriptionPlural);
		}

		if (reasons.Any(
			    x =>
				    x == IWieldItemWieldResult.AlreadyWielding ||
				    x == IWieldItemWieldResult.GrabbingWielderHoldOtherItem))
		{
			return string.Format("You have no free {1} with which to wield {0}.", item.HowSeen(this),
				WielderDescriptionPlural);
		}

		return $"You cannot wield {item.HowSeen(this)}.";
	}

	public string WhyCannotUnwield(IGameItem item, bool ignoreFreeHands = false)
	{
		if (_wieldedItems.All(x => x.Item1 != item))
		{
			return $"You are not wielding {item.HowSeen(this)}.";
		}

		if (HoldLocs.WhyCannotGrab(item, this) == WhyCannotGrabReason.InventoryFull && !ignoreFreeHands)
		{
			return $"You cannot stop wielding {item.HowSeen(this)} as your inventory is full.";
		}

		return $"You cannot stop wielding {item.HowSeen(this)}";
	}

	public string WhyCannotWield(IGameItem item, IWield specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None)
	{
		if (!flags.HasFlag(ItemCanWieldFlags.RequireTwoHands) &&
		    specificHand?.CanWield(item, this) != IWieldItemWieldResult.Success &&
		    !flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands))
		{
			if (CanWield(item, flags))
			{
				return
					$"You can't wield {item.HowSeen(Actor)} specifically in your {specificHand.FullDescription()}, but could wield it otherwise.";
			}

			return
				$"You can't wield {item.HowSeen(Actor)} specifically in your {specificHand.FullDescription()} at the moment.";
		}

		var openWieldLocs = WieldLocs.Where(x => x.CanWield(item, this) == IWieldItemWieldResult.Success ||
		                                         (flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands) &&
		                                          (x.CanWield(item, this) == IWieldItemWieldResult.AlreadyWielding ||
		                                           x.CanWield(item, this) ==
		                                           IWieldItemWieldResult.GrabbingWielderHoldOtherItem))).ToList();
		var locsAndHands = openWieldLocs.Select(x => (Item: x, Hands: x.Hands(item))).ToList();

		if (locsAndHands.Any(x => x.Item == specificHand))
		{
			return
				$"You need two {WielderDescriptionPlural} to wield {item.HowSeen(Actor)}, and while {specificHand.FullDescription()} is up to the task, it is the only one.";
		}

		if (CanWield(item, flags))
		{
			return
				$"You can't wield {item.HowSeen(Actor)} specifically in your {specificHand.FullDescription()}, but could wield it otherwise.";
		}

		return
			$"You can't wield {item.HowSeen(Actor)} specifically in your {specificHand.FullDescription()} at the moment.";
	}

	public bool Wield(IGameItem item, IWield specificHand, IEmote playerEmote = null, bool silent = false,
		ItemCanWieldFlags flags = ItemCanWieldFlags.None)
	{
		if (specificHand == null)
		{
			return Wield(item, playerEmote, silent, flags);
		}

		if (!CanWield(item, specificHand, flags))
		{
			if (!silent)
			{
				OutputHandler.Send(WhyCannotWield(item, specificHand, flags));
			}

			return false;
		}

		var potentialWearLocs =
			WieldLocs.Where(x => x.CanWield(item, this) == IWieldItemWieldResult.Success).Except(specificHand);
		var hands = flags.HasFlag(ItemCanWieldFlags.RequireTwoHands) ? 2 : specificHand.Hands(item);
		_wieldedItems.Add(Tuple.Create(item, specificHand));
		if (hands == 2)
		{
			_wieldedItems.Add(Tuple.Create(item, potentialWearLocs.First()));
		}

		_heldItems.RemoveAll(x => x.Item1 == item);
		if (!silent)
		{
			OutputHandler.Handle(
				new MixedEmoteOutput(
					new Emote("@ begin|begins to wield $1 in " + WieldSuffix(item, WieldSuffixForm.WieldEcho), Actor,
						Actor, item), flags: OutputFlags.SuppressObscured).Append(playerEmote));
		}

		item.GetItemType<IWieldable>().PrimaryWieldedLocation = specificHand;
		UpdateDescriptionWielded(item);
		InventoryChanged = true;
		OnInventoryChange?.Invoke(InventoryState.Held, InventoryState.Wielded, item);
		item.InvokeInventoryChange(InventoryState.Held, InventoryState.Wielded);
		CheckConsequences();
		HandleEvent(EventType.ItemWielded, item, Actor);
		foreach (var witness in Location.EventHandlers)
		{
			HandleEvent(EventType.ItemWieldedWitness, item, Actor, witness);
		}
		return true;
	}

	public bool LoadtimeWield(IGameItem item, ItemCanWieldFlags flags)
	{
		if (!CanWield(item, flags))
		{
			return false;
		}

		var potentialWearLocs =
			WieldLocs.Where(x => x.CanWield(item, this) == IWieldItemWieldResult.Success)
			         .OrderByDescending(x => _heldItems.Any(y => y.Item1 == item && y.Item2 == x))
			         .ToList();
		var hands = flags.HasFlag(ItemCanWieldFlags.RequireTwoHands) ? 2 : potentialWearLocs.Min(x => x.Hands(item));
		if (hands == 1)
		{
			//Try dominant hand first
			var loc = potentialWearLocs.FirstOrDefault(x =>
				          x.Hands(item) == 1 && x.Alignment.LeftRightOnly() == Actor.Handedness.LeftRightOnly()) ??
			          potentialWearLocs.FirstOrDefault(x => x.Hands(item) == 1);
			_wieldedItems.Add(Tuple.Create(item, loc));
			item.GetItemType<IWieldable>().PrimaryWieldedLocation = loc;
		}
		else
		{
			var locs = potentialWearLocs.Take(2).ToList();
			item.GetItemType<IWieldable>().PrimaryWieldedLocation = locs.First();
			foreach (var loc in locs)
			{
				_wieldedItems.Add(Tuple.Create(item, loc));
			}
		}

		_carriedItems.Add(item);
		UpdateDescriptionWielded(item);
		return true;
	}

	public bool Wield(IGameItem item, IEmote playerEmote = null, bool silent = false,
		ItemCanWieldFlags flags = ItemCanWieldFlags.None)
	{
		if (!CanWield(item, flags))
		{
			if (!silent)
			{
				OutputHandler.Send(WhyCannotWield(item, flags));
			}

			return false;
		}

		var potentialWearLocs =
			WieldLocs.Where(x => x.CanWield(item, this) == IWieldItemWieldResult.Success)
			         .OrderByDescending(x => _heldItems.Any(y => y.Item1 == item && y.Item2 == x))
			         .ToList();
		var hands = flags.HasFlag(ItemCanWieldFlags.RequireTwoHands) ? 2 : potentialWearLocs.Min(x => x.Hands(item));
		if (hands == 1)
		{
			//Try dominant hand first
			var loc = potentialWearLocs.FirstOrDefault(x =>
				          x.Hands(item) == 1 && x.Alignment.LeftRightOnly() == Actor.Handedness.LeftRightOnly()) ??
			          potentialWearLocs.FirstOrDefault(x => x.Hands(item) == 1);
			_wieldedItems.Add(Tuple.Create(item, loc));
			item.GetItemType<IWieldable>().PrimaryWieldedLocation = loc;
		}
		else
		{
			var locs = potentialWearLocs.Take(2).ToList();
			item.GetItemType<IWieldable>().PrimaryWieldedLocation = locs.First();
			foreach (var loc in locs)
			{
				_wieldedItems.Add(Tuple.Create(item, loc));
			}
		}

		_heldItems.RemoveAll(x => x.Item1 == item);

		if (!silent)
		{
			OutputHandler.Handle(
				new MixedEmoteOutput(
					new Emote("@ wield|wields $1 in " + WieldSuffix(item, WieldSuffixForm.WieldEcho), Actor, Actor,
						item), flags: OutputFlags.SuppressObscured).Append(playerEmote));
		}

		UpdateDescriptionWielded(item);
		InventoryChanged = true;
		OnInventoryChange?.Invoke(InventoryState.Held, InventoryState.Wielded, item);
		item.InvokeInventoryChange(InventoryState.Held, InventoryState.Wielded);
		CheckConsequences();
		HandleEvent(EventType.ItemWielded, item, Actor);
		foreach (var witness in Location.EventHandlers)
		{
			HandleEvent(EventType.ItemWieldedWitness, item, Actor, witness);
		}
		return true;
	}

	public bool Unwield(IGameItem item, IEmote playerEmote = null, bool silent = false)
	{
		var heldLocation = _wieldedItems.FirstOrDefault(x => x.Item1 == item);
		_wieldedItems.RemoveAll(x => x.Item1 == item);
		if (heldLocation != null && heldLocation.Item2.SelfUnwielder())
		{
			_heldItems.Add(Tuple.Create(heldLocation.Item1, heldLocation.Item2 as IGrab));
		}
		else
		{
			Get(item, silent: true);
		}

		if (!silent)
		{
			OutputHandler.Handle(
				new MixedEmoteOutput(new Emote("@ stop|stops wielding $0", this, item),
					flags: OutputFlags.SuppressObscured).Append(playerEmote));
		}

		item.GetItemType<IWieldable>().PrimaryWieldedLocation = null;
		UpdateDescriptionHeld(item);
		InventoryChanged = true;
		OnInventoryChange?.Invoke(InventoryState.Wielded, InventoryState.Held, item);
		item.InvokeInventoryChange(InventoryState.Wielded, InventoryState.Held);
		CheckConsequences();
		return true;
	}

	public bool CanBeDisarmed(IGameItem item, ICharacter disarmer)
	{
		// TODO - effects that influence this
		return true;
	}

	public IWield WieldedHand(IGameItem item)
	{
		return item.GetItemType<IWieldable>().PrimaryWieldedLocation;
	}

	public int WieldedHandCount(IGameItem item)
	{
		return _wieldedItems.Count(x => x.Item1 == item);
	}

	protected List<IWield> _wieldLocs;

	public IEnumerable<IWield> WieldLocs => _wieldLocs;

	private enum WieldSuffixForm
	{
		InventoryList,
		WieldEcho
	}

	private string WieldSuffix(IGameItem item, WieldSuffixForm form = WieldSuffixForm.InventoryList)
	{
		var locs = _wieldedItems.Where(x => x.Item1 == item).Select(x => x.Item2);
		if (locs.Count() == 1)
		{
			return (form == WieldSuffixForm.WieldEcho ? "&0's " : "") +
			       locs.First().ShortDescription(false, false);
		}

		if (locs.Count() == 2 && WieldLocs.Count() == 2)
		{
			return "both " + (form == WieldSuffixForm.WieldEcho ? "&0's " : "") + WielderDescriptionPlural;
		}

		if (locs.Count() == WieldLocs.Count())
		{
			return "all " + (form == WieldSuffixForm.WieldEcho ? "&0's " : "") + WieldLocs.Count() + " " +
			       WielderDescriptionPlural;
		}

		return (form == WieldSuffixForm.WieldEcho ? "&0's " : "") + DescribeBodypartGroup(locs);
	}

	public void UpdateDescriptionWielded(IGameItem item)
	{
		item.GetItemType<IHoldable>().CurrentInventoryDescription =
			$"{"<wielded in " + WieldSuffix(item) + ">",-35}";
	}

	public bool CanDraw(IGameItem item, IWield specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None)
	{
		if (item == null)
		{
			var wielditem =
				ExternalItems
				.SelectNotNull(x => x.GetItemType<ISheath>())
				.SelectNotNull(x => x.Content)
				.FirstOrDefault();
			if (wielditem == null)
			{
				return false;
			}

			item = wielditem.Parent;
		}

		if (
			_wornItems.Where(x => x.Item == item.ContainedIn)
			          .Any(
				          x =>
					          !_wornItems.Last(
						                     y =>
							                     y.Wearloc == x.Wearloc &&
							                     (y.Item == x.Item || y.Profile.PreventsRemoval))
					                     .Equals(x)))
		{
			return false;
		}

		var sheathBeltable = item.ContainedIn?.GetItemType<IBeltable>();
		if (sheathBeltable?.ConnectedTo != null &&
		    _wornItems.Where(x => x.Item == sheathBeltable.ConnectedTo.Parent)
		              .Any(
			              x =>
				              !_wornItems.Last(
					                         y =>
						                         y.Wearloc == x.Wearloc &&
						                         (y.Item == x.Item || y.Profile.PreventsRemoval))
				                         .Equals(x)))
		{
			return false;
		}

		if (!CanGet(item, 0))
		{
			return false;
		}

		return specificHand == null ? CanWield(item, flags) : CanWield(item, specificHand, flags);
	}

	public string WhyCannotDraw(IGameItem item, IWield specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None)
	{
		if (item == null)
		{
			var wielditem =
				ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>()).SelectNotNull(x => x.Content)
				             .FirstOrDefault();
			if (wielditem == null)
			{
				return "You do not have anything that can be drawn.";
			}

			item = wielditem.Parent;
		}

		if (
			_wornItems.Where(x => x.Item == item.ContainedIn)
			          .Any(
				          x =>
					          !_wornItems.Last(
						                     y =>
							                     y.Wearloc == x.Wearloc &&
							                     (y.Item == x.Item || y.Profile.PreventsRemoval))
					                     .Equals(x)))
		{
			return "You cannot draw from a sheath that is covered by other items which prevent access and removal.";
		}

		var sheathBeltable = item.ContainedIn?.GetItemType<IBeltable>();
		if (sheathBeltable?.ConnectedTo != null &&
		    _wornItems.Where(x => x.Item == sheathBeltable.ConnectedTo.Parent)
		              .Any(
			              x =>
				              !_wornItems.Last(
					                         y =>
						                         y.Wearloc == x.Wearloc &&
						                         (y.Item == x.Item || y.Profile.PreventsRemoval))
				                         .Equals(x)))
		{
			return "You cannot draw from a sheath that is covered by other items which prevent access and removal.";
		}

		return specificHand == null ? WhyCannotWield(item, flags) : WhyCannotWield(item, specificHand, flags);
	}

	public bool Draw(IGameItem item, IWield specificHand, IEmote playerEmote = null,
		OutputFlags additionalFlags = OutputFlags.Normal, bool silent = false,
		ItemCanWieldFlags flags = ItemCanWieldFlags.None)
	{
		if (!CanDraw(item, specificHand, flags))
		{
			OutputHandler.Send(WhyCannotDraw(item, specificHand, flags));
			return false;
		}

		if (item == null)
		{
			item =
				ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>())
				             .SelectNotNull(x => x.Content)
				             .FirstOrDefault()
				             ?.Parent;
		}

		var wieldItem = item.GetItemType<IWieldable>();
		var sheathItem =
			ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>()).FirstOrDefault(x => x.Content == wieldItem);

		sheathItem.Content = null;
		sheathItem.Parent.Changed = true;
		item.Get(this);
		if (!silent)
		{
			OutputHandler.Handle(
				new MixedEmoteOutput(new Emote("@ draw|draws $0 from $1", Actor, item, sheathItem.Parent),
					flags: OutputFlags.SuppressObscured | additionalFlags).Append(playerEmote));
		}

		if (specificHand == null)
		{
			Wield(item, null, silent, flags);
		}
		else
		{
			Wield(item, specificHand, null, silent, flags);
		}

		InventoryChanged = true;
		OnInventoryChange?.Invoke(InventoryState.Sheathed, InventoryState.Wielded, item);
		item.InvokeInventoryChange(InventoryState.Sheathed, InventoryState.Wielded);
		CheckConsequences();
		return true;
	}

	public bool CanSheathe(IGameItem item, IGameItem sheath)
	{
		IWieldable targetItemWieldable = null;
		if (item == null)
		{
			targetItemWieldable =
				HeldOrWieldedItems.Where(
					                  x =>
						                  x.GetItemType<IRangedWeapon>()?.WeaponType.RangedWeaponType.IsFirearm() ??
						                  true)
				                  .SelectNotNull(x => x.GetItemType<IWieldable>())
				                  .FirstOrDefault();
			if (targetItemWieldable == null)
			{
				return false;
			}

			item = targetItemWieldable.Parent;
		}
		else
		{
			targetItemWieldable = item.GetItemType<IWieldable>();
		}

		if (targetItemWieldable == null)
		{
			return false;
		}

		bool SheathIsSuitable(ISheath isheath)
		{
			if (isheath.Content != null)
			{
				return false;
			}

			if (isheath.DesignedForGuns !=
			    (item.GetItemType<IRangedWeapon>()?.WeaponType.RangedWeaponType.IsFirearm() ??
			     false))
			{
				return false;
			}

			if (isheath.MaximumSize < item.Size)
			{
				return false;
			}

			if (
				_wornItems.Where(x => x.Item == isheath.Parent)
				          .Any(
					          x =>
						          !_wornItems.Last(
							                     y =>
								                     y.Wearloc == x.Wearloc &&
								                     (y.Item == x.Item || y.Profile.PreventsRemoval))
						                     .Equals(x)))
			{
				return false;
			}

			var sheathBeltable = isheath.Parent.GetItemType<IBeltable>();
			if (sheathBeltable?.ConnectedTo != null &&
			    _wornItems.Where(x => x.Item == sheathBeltable.ConnectedTo.Parent)
			              .Any(
				              x =>
					              !_wornItems.Last(
						                         y =>
							                         y.Wearloc == x.Wearloc &&
							                         (y.Item == x.Item || y.Profile.PreventsRemoval))
					                         .Equals(x)))
			{
				return false;
			}

			return true;
		}

		if (sheath == null)
		{
			if (!ExternalItems.Any(x => x.IsItemType<ISheath>()))
			{
				return false;
			}

			if (!ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>()).Any(SheathIsSuitable))
			{
				return false;
			}
		}
		else
		{
			return sheath.IsItemType<ISheath>() && SheathIsSuitable(sheath.GetItemType<ISheath>());
		}

		return true;
	}

	private enum WhyCannotSheatheReason
	{
		None,
		NotEmpty,
		NotRightWeaponType,
		WeaponTooLarge,
		Covered
	}

	public string WhyCannotSheathe(IGameItem item, IGameItem sheath)
	{
		IWieldable targetItemWieldable = null;
		if (item == null)
		{
			targetItemWieldable =
				HeldOrWieldedItems.Where(
					                  x =>
						                  x.GetItemType<IRangedWeapon>()?.WeaponType.RangedWeaponType.IsFirearm() ??
						                  true)
				                  .SelectNotNull(x => x.GetItemType<IWieldable>())
				                  .FirstOrDefault();
			if (targetItemWieldable == null)
			{
				return "You don't have any suitable sheathes for that item.";
			}

			item = targetItemWieldable.Parent;
		}
		else
		{
			targetItemWieldable = item.GetItemType<IWieldable>();
		}

		if (targetItemWieldable == null)
		{
			return "That is not something that can be sheathed.";
		}

		(bool Success, string ReasonText, WhyCannotSheatheReason Reason) SheathIsSuitable(ISheath isheath)
		{
			if (isheath.Content != null)
			{
				return (false, $"{isheath.Parent.HowSeen(Actor, true)} is not empty.", WhyCannotSheatheReason.NotEmpty);
			}

			if (isheath.DesignedForGuns !=
			    (item.GetItemType<IRangedWeapon>()?.WeaponType.RangedWeaponType.IsFirearm() ??
			     false))
			{
				return (false, $"{isheath.Parent.HowSeen(Actor, true)} is not designed to take that kind of weapon.",
					WhyCannotSheatheReason.NotRightWeaponType);
			}

			if (isheath.MaximumSize < item.Size)
			{
				return (false, $"{item.HowSeen(Actor, true)} is too big to fit in {isheath.Parent.HowSeen(Actor)}.",
					WhyCannotSheatheReason.WeaponTooLarge);
			}

			if (
				_wornItems.Where(x => x.Item == isheath.Parent)
				          .Any(
					          x =>
						          !_wornItems.Last(
							                     y =>
								                     y.Wearloc == x.Wearloc &&
								                     (y.Item == x.Item || y.Profile.PreventsRemoval))
						                     .Equals(x)))
			{
				return (false, $"{isheath.Parent.HowSeen(Actor, true)} is covered by other items that prevent access.",
					WhyCannotSheatheReason.Covered);
			}

			var sheathBeltable = isheath.Parent.GetItemType<IBeltable>();
			if (sheathBeltable?.ConnectedTo != null &&
			    _wornItems.Where(x => x.Item == sheathBeltable.ConnectedTo.Parent)
			              .Any(
				              x =>
					              !_wornItems.Last(
						                         y =>
							                         y.Wearloc == x.Wearloc &&
							                         (y.Item == x.Item || y.Profile.PreventsRemoval))
					                         .Equals(x)))
			{
				return (false, $"{isheath.Parent.HowSeen(Actor, true)} is covered by other items that prevent access.",
					WhyCannotSheatheReason.Covered);
			}

			return (true, "", WhyCannotSheatheReason.None);
		}

		if (sheath == null)
		{
			if (!ExternalItems.Any(x => x.IsItemType<ISheath>()))
			{
				return "You do not have any sheaths.";
			}

			var sheaths = ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>())
			                           .Select(x => (Sheath: x, Result: SheathIsSuitable(x))).ToList();
			if (sheaths.Any(x => x.Result.Reason == WhyCannotSheatheReason.NotEmpty))
			{
				return "You do not have any empty sheaths that are capable of taking that weapon.";
			}

			return "You do not have any empty sheaths that are capable of taking that weapon.";
		}

		var targetSheathComponent = sheath.GetItemType<ISheath>();
		if (targetSheathComponent == null)
		{
			return $"{sheath.HowSeen(Actor, true)} is not a sheath.";
		}

		var result = SheathIsSuitable(targetSheathComponent);
		switch (result.Reason)
		{
			case WhyCannotSheatheReason.None:
				break;
			case WhyCannotSheatheReason.NotEmpty:
				return $"{sheath.HowSeen(Actor, true)} already has something in it.";
			case WhyCannotSheatheReason.NotRightWeaponType:
				return
					$"{sheath.HowSeen(Actor, true)} is not capable of bearing {(targetSheathComponent.DesignedForGuns ? "melee weapons" : "firearms")}.";
			case WhyCannotSheatheReason.WeaponTooLarge:
				return $"{sheath.HowSeen(Actor, true)} is not a capable of bearing something as large as that.";
			case WhyCannotSheatheReason.Covered:
				return $"{sheath.HowSeen(Actor, true)} is covered by other items that prevent access to it.";
			default:
				throw new ArgumentOutOfRangeException();
		}

		throw new NotSupportedException("Got to the end of WhyCannotSheathe");
	}

	public bool Sheathe(IGameItem item, IGameItem sheath, IEmote playerEmote = null,
		OutputFlags additionalFlags = OutputFlags.Normal, bool silent = false)
	{
		if (!CanSheathe(item, sheath))
		{
			OutputHandler.Send(WhyCannotSheathe(item, sheath));
			return false;
		}

		IWieldable targetItemWieldable = null;
		if (item == null)
		{
			targetItemWieldable =
				HeldOrWieldedItems.Where(
					                  x =>
						                  x.GetItemType<IRangedWeapon>()?.WeaponType.RangedWeaponType.IsFirearm() ??
						                  true)
				                  .SelectNotNull(x => x.GetItemType<IWieldable>())
				                  .FirstOrDefault();
			item = targetItemWieldable.Parent;
		}
		else
		{
			targetItemWieldable = item.GetItemType<IWieldable>();
		}

		var wasWielded = WieldedItems.Contains(item);
		ISheath targetSheathComponent = null;
		if (sheath == null)
		{
			targetSheathComponent =
				ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>())
				             .FirstOrDefault(x => x.Content == null && x.MaximumSize >= item.Size);
			sheath = targetSheathComponent.Parent;
		}
		else
		{
			targetSheathComponent = sheath.GetItemType<ISheath>();
		}

		if (!silent)
		{
			var flags = OutputFlags.SuppressObscured | additionalFlags;
			OutputHandler.Handle(
				new MixedEmoteOutput(new Emote("@ sheathe|sheathes $0 in $1", Actor, item, sheath), flags: flags)
					.Append
						(playerEmote));
		}

		item.Drop(null);
		Take(item);
		targetSheathComponent.Content = targetItemWieldable;
		sheath.Changed = true;
		Actor.HandleEvent(EventType.CharacterSheatheItem, Actor, item, sheath);
		item.HandleEvent(EventType.ItemSheathed, Actor, item, sheath);
		sheath.HandleEvent(EventType.ItemSheathItemSheathed, Actor, item, sheath);
		foreach (var witness in Location.EventHandlers.Except(Actor))
		{
			witness.HandleEvent(EventType.CharacterSheatheItemWitness, Actor, item, sheath, witness);
		}

		foreach (var witness in ExternalItems.Except(new[] { item, sheath }))
		{
			witness.HandleEvent(EventType.CharacterSheatheItemWitness, Actor, item, sheath, witness);
		}

		InventoryChanged = true;
		OnInventoryChange?.Invoke(wasWielded ? InventoryState.Wielded : InventoryState.Held, InventoryState.Sheathed,
			item);
		item.InvokeInventoryChange(wasWielded ? InventoryState.Wielded : InventoryState.Held, InventoryState.Sheathed);
		CheckConsequences();
		return true;
	}

	#endregion

	#region IGrab Implementation

	protected List<IGrab> _holdlocs;

	public IEnumerable<IGrab> HoldLocs => _holdlocs;

	private readonly List<Tuple<IGameItem, IGrab>> _heldItems = new();

	public IEnumerable<IGameItem> HeldItems
	{
		get { return _heldItems.Select(x => x.Item1).Distinct(); }
	}

	public IEnumerable<IGameItem> HeldOrWieldedItems => HeldItems.Concat(WieldedItems).ToList();

	public IEnumerable<IGrab> FreeHands
	{
		get
		{
			//TODO - Review: Does this need to also add in WieldLocs?
			return
				HoldLocs.Where(x => _heldItems.All(y => y.Item2 != x) && _wieldedItems.All(y => y.Item2 != x))
				        .ToList();
		}
	}

	public IEnumerable<IGrab> FunctioningFreeHands
	{
		get { return FreeHands.Where(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse); }
	}

	public IEnumerable<IGameItem> HeldItemsFor(IBodypart prototype)
	{
		return _heldItems.Where(x => x.Item2 == prototype).Select(x => x.Item1).ToList();
	}

	public IEnumerable<IGameItem> HeldOrWieldedItemsFor(IBodypart prototype)
	{
		return
			_heldItems.Where(x => x.Item2 == prototype)
			          .Select(x => x.Item1)
			          .Concat(_wieldedItems.Where(x => x.Item2 == prototype).Select(x => x.Item1))
			          .ToList();
	}

	public IBodypart BodypartLocationOfInventoryItem(IGameItem item)
	{
		return _heldItems.FirstOrDefault(x => x.Item1 == item)?.Item2 ??
		       _wieldedItems.FirstOrDefault(x => x.Item1 == item)?.Item2 ??
		       _wornItems.FirstOrDefault(x => x.Item == item).Wearloc ??
		       _implants.FirstOrDefault(x => x.Parent == item)?.TargetBodypart ??
		       _prosthetics.FirstOrDefault(x => x.Parent == item)?.TargetBodypart ??
		       _wounds.FirstOrDefault(x => x.Lodged == item)?.Bodypart
			;
	}

	public IBodypart TopLevelBodypart(IBodypart part)
	{
		if (part is IOrganProto organ)
		{
			return Bodyparts.FirstOrDefault(x => x.OrganInfo.ContainsKey(organ) &&
			                                     x.OrganInfo[organ].IsPrimaryInternalLocation) ?? part;
		}

		if (part is IBone bone)
		{
			return Bodyparts.FirstOrDefault(x => x.BoneInfo.ContainsKey(bone) &&
			                                     x.BoneInfo[bone].IsPrimaryInternalLocation) ?? part;
		}

		return part;
	}

	public IBodypart HoldOrWieldLocFor(IGameItem item)
	{
		var wield = item.GetItemType<IWieldable>();
		return (IBodypart)_heldItems.FirstOrDefault(x => x.Item1 == item)?.Item2 ?? _wieldedItems
			.Where(x => x.Item1 == item).OrderByDescending(x => wield?.PrimaryWieldedLocation == x.Item2)
			.FirstOrDefault()?.Item2;
	}

	public bool CanGet(IGameItem item, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		if (item.GetItemType<IDoor>()?.InstalledExit != null)
		{
			return false;
		}

		if (item.Location?.CanGet(item, Actor) == false)
		{
			return false;
		}

		var actualItem = quantity == 0 ? item : item.PeekSplit(quantity);
		switch (actualItem.CanGet(quantity, ignoreFlags))
		{
			case ItemGetResponse.NoGetEffectCombat:
				if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreCombat))
				{
					return false;
				}

				break;
			case ItemGetResponse.NoGetEffectPlan:
				if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreInventoryPlans))
				{
					return false;
				}

				break;
			case ItemGetResponse.NoGetEffect:
			case ItemGetResponse.NotIHoldable:
			case ItemGetResponse.Unpositionable:
				return false;
		}

		if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreWeight) && !Actor.IsAdministrator())
		{
			if ((ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreLiftUseDrag)
				    ? Race.GetMaximumDragWeight(Actor)
				    : Race.GetMaximumLiftWeight(Actor)) < CarriedItems.Sum(x => x.Weight) + actualItem.Weight)
			{
				return false;
			}
		}

		if (HeldOrWieldedItems.All(x => !x.CanMerge(actualItem)))
		{
			if (HoldLocs.All(x =>
			    {
				    switch (x.CanGrab(actualItem, this))
				    {
					    case WearlocGrabResult.Success:
						    return false;
					    case WearlocGrabResult.FailNoTake:
						    return true;
					    case WearlocGrabResult.FailFull:
						    return !ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreFreeHands);
					    case WearlocGrabResult.FailDamaged:
						    return true;
					    case WearlocGrabResult.FailTooBig:
						    return !Actor.IsAdministrator();
					    case WearlocGrabResult.FailNoStackMerge:
						    return true;
				    }

				    return true;
			    }))
			{
				return false;
			}
		}

		return true;
	}

	public bool CanGet(IGameItem item, IGameItem container, int quantity,
		ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		if (item == null)
		{
			return false;
		}

		var tcontainer = container.GetItemType<IContainer>();
		return
			(container.Location?.CanGetAccess(container, Actor) ?? true) &&
			(ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreInContainer) || tcontainer?.Contents.Contains(item) == true) &&
			tcontainer.CanTake(Actor, item, quantity) && CanGet(item, quantity,
				tcontainer.Parent.InInventoryOf == this ? ignoreFlags | ItemCanGetIgnore.IgnoreWeight : ignoreFlags);
	}

	public string WhyCannotGet(IGameItem item, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		if (item.GetItemType<IDoor>()?.InstalledExit != null)
		{
			return "You cannot get installed doors directly. You must uninstall them first.";
		}

		if (item.Location?.CanGet(item, Actor) == false)
		{
			return Location.WhyCannotGet(item, Actor);
		}

		var actualItem = quantity == 0 ? item : item.PeekSplit(quantity);
		switch (actualItem.CanGet(quantity, ignoreFlags))
		{
			case ItemGetResponse.NoGetEffectCombat:
				if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreCombat))
				{
					return $"You cannot get {actualItem.HowSeen(this)} because it is involved in an ongoing combat.";
				}

				break;
			case ItemGetResponse.NoGetEffectPlan:
				if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreInventoryPlans))
				{
					var user = actualItem.EffectsOfType<IInventoryPlanItemEffect>().First().Owner;
					if (user.IsSelf(this))
					{
						return $"You cannot get {actualItem.HowSeen(this)} while you are using it in your task.";
					}

					return
						$"You cannot get {actualItem.HowSeen(this)} because {user.HowSeen(this)} is using it. You must interrupt what {user.ApparentGender(this).Subjective()} is doing if you want to force the issue.";
				}

				break;
			case ItemGetResponse.NotIHoldable:
				return actualItem.HowSeen(this, true) + " is not something that can be picked up.";
			case ItemGetResponse.Unpositionable:
				return "You cannot reposition " + actualItem.HowSeen(this) + " because it" +
				       actualItem.WhyCannotReposition();
			case ItemGetResponse.NoGetEffect:
				return $"You cannot get {actualItem.HowSeen(this)} at the moment.";
		}

		if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreWeight) && !Actor.IsAdministrator())
		{
			if ((ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreLiftUseDrag)
				    ? Race.GetMaximumDragWeight(Actor)
				    : Race.GetMaximumLiftWeight(Actor)) < CarriedItems.Sum(x => x.Weight) + actualItem.Weight)
			{
				return "You are not strong enough to lift so much.";
			}
		}

		var failingLocs = HoldLocs.Select(x => (x, x.CanGrab(actualItem, this))).Where(x =>
		{
			switch (x.Item2)
			{
				case WearlocGrabResult.Success:
					return false;
				case WearlocGrabResult.FailNoTake:
					return true;
				case WearlocGrabResult.FailFull:
					return !ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreFreeHands);
				case WearlocGrabResult.FailDamaged:
					return true;
				case WearlocGrabResult.FailTooBig:
					return !Actor.IsAdministrator();
				case WearlocGrabResult.FailNoStackMerge:
					return true;
			}

			return true;
		}).ToList();
		if (failingLocs.Any(x => x.Item2 == WearlocGrabResult.FailDamaged))
		{
			return
				$"You cannot get {actualItem.HowSeen(this)} because your free {WielderDescriptionPlural} are too damaged.";
		}

		if (failingLocs.Any(x => x.Item2 == WearlocGrabResult.FailFull))
		{
			return $"You cannot get {actualItem.HowSeen(this)} because your {WielderDescriptionPlural} are full.";
		}

		if (failingLocs.Any(x => x.Item2 == WearlocGrabResult.FailNoStackMerge))
		{
			return $"You cannot get {actualItem.HowSeen(this)} because it can't merge into other stacks.";
		}

		if (failingLocs.Any(x => x.Item2 == WearlocGrabResult.FailTooBig))
		{
			return $"You cannot get {actualItem.HowSeen(this)} because it is too big.";
		}

		return "You cannot get " + actualItem.HowSeen(this);
	}

	public string WhyCannotGet(IGameItem item, IGameItem container, int quantity,
		ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		if (item == null)
		{
			return "You don't see anything like that in such a container.";
		}

		var tcontainer = container.GetItemType<IContainer>();
		if (tcontainer == null)
		{
			return container.HowSeen(this, true) + " is not a container.";
		}

		if (!(container.Location?.CanGetAccess(container, Actor) ?? true))
		{
			return container.Location.WhyCannotGetAccess(container, Actor);
		}

		switch (tcontainer.WhyCannotTake(Actor, item))
		{
			case WhyCannotGetContainerReason.ContainerClosed:
				return "You must open " + container.HowSeen(this) + " before you can take anything out of it.";
			case WhyCannotGetContainerReason.UnlawfulAction:
				return
					$"Taking {item.HowSeen(Actor)} from {container.HowSeen(Actor)} would be a crime, and you have flagged lawful behaviour only.\n{CrimeExtensions.StandardDisableIllegalFlagText}";
			case WhyCannotGetContainerReason.NotContained:
				if (ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreInContainer))
				{
					goto default;
				}

				return "You do not see that in " + container.HowSeen(this) + ".";
			default:
				return WhyCannotGet(item, quantity,
					tcontainer.Parent.InInventoryOf == this
						? ignoreFlags | ItemCanGetIgnore.IgnoreWeight
						: ignoreFlags);
		}
	}

	private bool LoadtimeGet(IGameItem item)
	{
		if (!CanGet(item, 0, ItemCanGetIgnore.IgnoreWeight))
		{
			return false;
		}

		var grabLoc = HoldLocs.FirstOrDefault(x =>
			              (x.CanGrab(item, this) == WearlocGrabResult.Success &&
			               x.Alignment.LeftRightOnly() == Actor.Handedness.LeftRightOnly()) ||
			              (Actor.IsAdministrator() && x.CanGrab(item, this) == WearlocGrabResult.FailTooBig)) ??
		              HoldLocs.First(x => x.CanGrab(item, this) == WearlocGrabResult.Success ||
		                                  (Actor.IsAdministrator() &&
		                                   x.CanGrab(item, this) == WearlocGrabResult.FailTooBig));
		_heldItems.Add(Tuple.Create(item, grabLoc));
		_carriedItems.Add(item);
		UpdateDescriptionHeld(item);
		return true;
	}

	public void Get(IGameItem item, int quantity = 0, IEmote playerEmote = null, bool silent = false,
		ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		if (!CanGet(item, quantity, ignoreFlags))
		{
			if (!silent)
			{
				OutputHandler.Send(WhyCannotGet(item, quantity, ignoreFlags));
			}

			return;
		}

		MixedEmoteOutput output;
		IGameItem gottenItem = null;
		if (quantity == 0 || item.DropsWhole(quantity))
		{
			gottenItem = item;
			item.Get(this);

			if (!_heldItems.Any(x => x.Item1 == gottenItem))
			{
				if (HeldOrWieldedItems.Any(x => x.CanMerge(gottenItem)))
				{
					HeldOrWieldedItems.First(x => x.CanMerge(gottenItem)).Merge(gottenItem);
				}
				else
				{
					//Try to put it in our dominant hand first
					var grabLoc = HoldLocs.FirstOrDefault(x =>
						              (x.CanGrab(gottenItem, this) == WearlocGrabResult.Success &&
						               x.Alignment.LeftRightOnly() == Actor.Handedness.LeftRightOnly()) ||
						              (Actor.IsAdministrator() &&
						               x.CanGrab(gottenItem, this) == WearlocGrabResult.FailTooBig)) ??
					              HoldLocs.First(x => x.CanGrab(gottenItem, this) == WearlocGrabResult.Success ||
					                                  (Actor.IsAdministrator() && x.CanGrab(gottenItem, this) ==
						                                  WearlocGrabResult.FailTooBig));
					_heldItems.Add(Tuple.Create(gottenItem, grabLoc));
				}
			}
#if DEBUG
			else
			{
				throw new ApplicationException("Item duplication in BodyInventory.");
			}
#endif

			output = new MixedEmoteOutput(new Emote("@ get|gets $0", this, gottenItem),
				flags: OutputFlags.SuppressObscured);
			UpdateDescriptionHeld(item);
		}
		else
		{
			var newItem = item.Get(this, quantity);
			gottenItem = newItem;
			if (HeldOrWieldedItems.Any(x => x.CanMerge(gottenItem)))
			{
				HeldOrWieldedItems.First(x => x.CanMerge(gottenItem)).Merge(gottenItem);
			}
			else
			{
				//Try to put it in our dominant hand first
				var grabLoc = HoldLocs.FirstOrDefault(
					              x => (x.CanGrab(gottenItem, this) == WearlocGrabResult.Success &&
					                    x.Alignment.LeftRightOnly() == Actor.Handedness.LeftRightOnly()) ||
					                   (Actor.IsAdministrator() && x.CanGrab(gottenItem, this) ==
						                   WearlocGrabResult.FailTooBig)) ??
				              HoldLocs.First(x => x.CanGrab(gottenItem, this) == WearlocGrabResult.Success ||
				                                  (Actor.IsAdministrator() && x.CanGrab(gottenItem, this) ==
					                                  WearlocGrabResult.FailTooBig));
				_heldItems.Add(Tuple.Create(gottenItem, grabLoc));
			}

			output = new MixedEmoteOutput(new Emote("@ get|gets $0", this, gottenItem),
				flags: OutputFlags.SuppressObscured);
			UpdateDescriptionHeld(newItem);
		}

		InventoryChanged = true;
		if (!silent)
		{
			output.Append(playerEmote);
			OutputHandler.Handle(output);
		}

		OnInventoryChange?.Invoke(InventoryState.Dropped, InventoryState.Held, gottenItem);
		gottenItem.InvokeInventoryChange(InventoryState.Dropped, InventoryState.Held);
		// Handle events
		HandleEvent(EventType.CharacterGotItem, Actor, gottenItem);
		gottenItem.HandleEvent(EventType.ItemGotten, Actor, gottenItem);
		foreach (var witness in Location.EventHandlers.Except(Actor))
		{
			witness.HandleEvent(EventType.CharacterGotItemWitness, Actor, gottenItem, witness);
		}

		foreach (var witness in ExternalItems.Except(gottenItem))
		{
			witness.HandleEvent(EventType.CharacterGotItemWitness, Actor, gottenItem, witness);
		}

		CheckConsequences();
	}

	public void Get(IGameItem item, IGameItem containerItem, int quantity = 0, IEmote playerEmote = null,
		bool silent = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		if (!CanGet(item, containerItem, quantity, ignoreFlags))
		{
			if (!silent)
			{
				OutputHandler.Send(WhyCannotGet(item, containerItem, quantity, ignoreFlags));
			}

			return;
		}

		var containerComp = containerItem.GetItemType<IContainer>();

		var takenItem = containerComp.Take(Actor, item, quantity).Get(this);
		if (!_heldItems.Any(x => x.Item1 == takenItem))
		{
			if (HeldOrWieldedItems.Any(x => x.CanMerge(takenItem)))
			{
				HeldOrWieldedItems.First(x => x.CanMerge(takenItem)).Merge(takenItem);
			}
			else
			{
				_heldItems.Add(Tuple.Create(takenItem,
					HoldLocs.First(
						x =>
							x.CanGrab(takenItem, this) == WearlocGrabResult.Success ||
							(Actor.IsAdministrator() && x.CanGrab(takenItem, this) == WearlocGrabResult.FailTooBig))));
			}
		}
#if DEBUG
		else
		{
			throw new ApplicationException("Item duplication in BodyInventory.");
		}
#endif

		var output =
			new MixedEmoteOutput(new Emote("@ get|gets $0 from $1", this, takenItem, containerItem),
				flags: OutputFlags.SuppressObscured);
		UpdateDescriptionHeld(takenItem);

		output.Append(playerEmote);
		if (!silent)
		{
			OutputHandler.Handle(output);
		}

		InventoryChanged = true;
		OnInventoryChange?.Invoke(InventoryState.InContainer, InventoryState.Held, takenItem);
		takenItem.InvokeInventoryChange(InventoryState.InContainer, InventoryState.Held);
		// Handle events
		HandleEvent(EventType.CharacterGotItemContainer, Actor, takenItem, containerItem);
		takenItem.HandleEvent(EventType.ItemGottenContainer, Actor, takenItem, containerItem);
		foreach (var witness in Location.EventHandlers.Except(Actor))
		{
			witness.HandleEvent(EventType.CharacterGotItemContainerWitness, Actor, takenItem, containerItem, witness);
		}

		foreach (var witness in ExternalItems.Except(new[] { takenItem, containerItem }))
		{
			witness.HandleEvent(EventType.CharacterGotItemContainerWitness, Actor, takenItem, containerItem, witness);
		}

		CheckConsequences();
	}

	public void UpdateDescriptionHeld(IGameItem item)
	{
		var locs = _heldItems.Where(x => x.Item1 == item).Select(x => x.Item2);
		item.GetItemType<IHoldable>().CurrentInventoryDescription =
			$"{"<held in " + DescribeBodypartGroup(locs) + ">",-35}";
	}

	public bool CanPut(IGameItem item, IGameItem container, ICharacter containerOwner, int quantity,
		bool allowLesserAmounts)
	{
		var tcontainer = container.GetItemType<IContainer>();
		return
			(container.Location?.CanGetAccess(container, Actor) ?? true) &&
			tcontainer != null &&
			(tcontainer.CanPut(item.PeekSplit(quantity)) ||
			 (allowLesserAmounts && tcontainer.WhyCannotPut(item.PeekSplit(quantity)) ==
				 WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity)) && CanDrop(item, quantity) &&
			container.GetItemType<IWearable>()?.CurrentProfile?.RequireContainerIsEmpty != true &&
			!item.PreventsMovement();
	}

	public string WhyCannotPut(IGameItem item, IGameItem container, ICharacter containerOwner, int quantity,
		bool allowLesserAmounts)
	{
		var tcontainer = container.GetItemType<IContainer>();
		if (tcontainer == null)
		{
			return container.HowSeen(this, true) + " is not a container.";
		}

		if (!(container.Location?.CanGetAccess(container, Actor) ?? true))
		{
			return container.Location.WhyCannotGetAccess(container, Actor);
		}

		if (item == null)
		{
			return "You do not see such an item.";
		}

		if (item.PreventsMovement())
		{
			return
				$"You cannot put {item.HowSeen(Actor)} into {tcontainer.Parent.HowSeen(Actor)} because {item.WhyPreventsMovement(Actor)}";
		}

		if (container.GetItemType<IWearable>()?.CurrentProfile?.RequireContainerIsEmpty == true)
		{
			return $"You cannot put anything into {container.HowSeen(Actor)} while it is being worn.";
		}

		var dummy = quantity == 0 ? item : item.PeekSplit(quantity);
		switch (tcontainer.WhyCannotPut(dummy))
		{
			case WhyCannotPutReason.CantPutContainerInItself:
				return $"You cannot put a container inside itself.";
			case WhyCannotPutReason.ContainerClosed:
				return "You must open " + container.HowSeen(this) + " before you can put anything in it.";
			case WhyCannotPutReason.ContainerFull:
				return container.HowSeen(this, true) + " is too full for " + dummy.HowSeen(this) + " to fit.";
			case WhyCannotPutReason.NotCorrectItemType:
				return dummy.HowSeen(this, true) + " is not the right type of item to go in " +
				       container.HowSeen(this) + ".";
			case WhyCannotPutReason.ItemTooLarge:
				return dummy.HowSeen(this, true) + " is too large to fit in a container like " +
				       container.HowSeen(this) + ".";
			case WhyCannotPutReason.NotContainer:
				return $"{container.HowSeen(this, true)} is not a container.";
			default:
				switch (WhyCannotDrop(dummy, quantity))
				{
					default:
						return "You cannot let go of that.";
				}
		}
	}

	public void Put(IGameItem item, IGameItem container, ICharacter containerOwner, int quantity = 0,
		IEmote playerEmote = null,
		bool silent = false, bool allowLesserAmounts = true)
	{
		if (container.IsItemType<ICorpse>())
		{
			Put(item, container, "", playerEmote);
			return;
		}

		if (!CanPut(item, container, containerOwner, quantity, allowLesserAmounts))
		{
			if (!silent)
			{
				OutputHandler.Send(WhyCannotPut(item, container, containerOwner, quantity, allowLesserAmounts));
			}

			return;
		}

		var containerComp = container.GetItemType<IContainer>();
		IGameItem putItem = null;
		MixedEmoteOutput output;
		if (allowLesserAmounts && !CanPut(item, container, containerOwner, quantity, false) &&
		    containerComp.WhyCannotPut(item) == WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity)
		{
			item = item.Get(this, containerComp.CanPutAmount(item));
			quantity = 0;
		}

		if (quantity == 0 || item.DropsWhole(quantity))
		{
			putItem = item;
			_heldItems.RemoveAll(x => x.Item1 == item);
			_wieldedItems.RemoveAll(x => x.Item1 == item);
			item.Drop(null);
			containerComp.Put(Actor, item);
			if (containerOwner == null)
			{
				output = new MixedEmoteOutput(new Emote("@ put|puts $0 in $1", this, item, container),
					flags: OutputFlags.SuppressObscured);
			}
			else
			{
				output = new MixedEmoteOutput(
					new Emote("@ put|puts $0 in $2's !1", this, item, container, containerOwner),
					flags: OutputFlags.SuppressObscured);
			}
		}

		else
		{
			var newItem = item.Drop(null, quantity);
			putItem = newItem;
			containerComp.Put(Actor, newItem);
			if (containerOwner == null)
			{
				output = new MixedEmoteOutput(new Emote("@ put|puts $0 in $1", this, newItem, container),
					flags: OutputFlags.SuppressObscured);
			}
			else
			{
				output = new MixedEmoteOutput(
					new Emote("@ put|puts $0 in $2's !1", this, newItem, container, containerOwner),
					flags: OutputFlags.SuppressObscured);
			}

			UpdateDescriptionHeld(newItem);
		}

		output.Append(playerEmote);
		if (!silent)
		{
			OutputHandler.Handle(output);
		}

		InventoryChanged = true;
		OnInventoryChange?.Invoke(InventoryState.Held, InventoryState.InContainer, putItem);
		putItem.InvokeInventoryChange(InventoryState.Held, InventoryState.InContainer);
		// Handle events
		HandleEvent(EventType.CharacterPutItemContainer, Actor, putItem, container);
		putItem.HandleEvent(EventType.ItemPutContainer, Actor, putItem, container);
		foreach (var witness in Location.EventHandlers.Except(Actor))
		{
			witness.HandleEvent(EventType.CharacterPutItemContainerWitness, Actor, putItem, container, witness);
		}

		foreach (var witness in ExternalItems.Except(new[] { putItem, container }))
		{
			witness.HandleEvent(EventType.CharacterPutItemContainerWitness, Actor, putItem, container, witness);
		}

		CheckConsequences();
	}

	#region Corpse-Related Versions of Put

	public bool CanPut(IGameItem item, IGameItem container, string profile)
	{
		var containerAsCorpse = container.GetItemType<ICorpse>();
		if (containerAsCorpse == null)
		{
			return false;
		}

		if (!(container.Location?.CanGetAccess(container, Actor) ?? true))
		{
			return false;
		}

		var itemAsWearable = item.GetItemType<IWearable>();
		if (itemAsWearable == null)
		{
			return false;
		}

		if (!string.IsNullOrEmpty(profile))
		{
			var wprof =
				itemAsWearable.Profiles.FirstOrDefault(
					x => x.Name.StartsWith(profile, StringComparison.InvariantCultureIgnoreCase));
			if (wprof == null)
			{
				return false;
			}

			return CanDrop(item, 1) && containerAsCorpse.OriginalCharacter.Body.CanWear(item, wprof);
		}

		return CanDrop(item, 1) && containerAsCorpse.OriginalCharacter.Body.CanWear(item);
	}

	public string WhyCannotPut(IGameItem item, IGameItem container, string profile)
	{
		var containerAsCorpse = container.GetItemType<ICorpse>();
		if (containerAsCorpse == null)
		{
			return $"{container.HowSeen(Actor, true)} is not a corpse, and so cannot be dressed up.";
		}

		if (!(container.Location?.CanGetAccess(container, Actor) ?? true))
		{
			return container.Location.WhyCannotGetAccess(container, Actor);
		}

		var itemAsWearable = item.GetItemType<IWearable>();
		if (itemAsWearable == null)
		{
			return $"{item.HowSeen(Actor, true)} is not something that can be worn.";
		}

		var originalChar = containerAsCorpse.OriginalCharacter;

		if (!CanDrop(item, 1))
		{
			switch (originalChar.Body.HoldLocs.WhyCannotDrop(item))
			{
				case WhyCannotDropReason.Unknown:
				default:
					return $"You cannot seem to let go of {item.HowSeen(this)}";
			}
		}

		IWearProfile wprof = null;
		if (!string.IsNullOrEmpty(profile))
		{
			wprof =
				itemAsWearable.Profiles.FirstOrDefault(
					x => x.Name.StartsWith(profile, StringComparison.InvariantCultureIgnoreCase));
			if (wprof == null)
			{
				return $"{container.HowSeen(Actor, true)} does not have a wear profile named {profile}.";
			}
		}


		switch (wprof == null
			        ? containerAsCorpse.OriginalCharacter.Body.WearLocs.WhyCannotDrape(item,
				        containerAsCorpse.OriginalCharacter.Body)
			        : containerAsCorpse.OriginalCharacter.Body.WearLocs.WhyCannotDrape(item, wprof,
				        containerAsCorpse.OriginalCharacter.Body))
		{
			case WhyCannotDrapeReason.NotIDrapeable:
				return $"{item.HowSeen(Actor, true)} is not something that can be worn.";
			case WhyCannotDrapeReason.BadFit:
				return $"{item.HowSeen(this, true)} is a bad fit for {container.HowSeen(this)}.";
			case WhyCannotDrapeReason.SpecificProfileNoMatch:
				return $"{container.HowSeen(this, true)} cannot wear {item.HowSeen(this)} in that precise manner.";
			case WhyCannotDrapeReason.NotBodyType:
				return $"{container.HowSeen(this, true)} is not the right body type to wear {item.HowSeen(this)}.";
			case WhyCannotDrapeReason.TooBulky:
				return
					$"{item.HowSeen(Actor, true)} is too bulky to be worn over what {container.HowSeen(this)} is already wearing.";
			case WhyCannotDrapeReason.TooManyItems:
				return
					$"{container.HowSeen(this, true)} is already wearing too many things on the same location to wear {item.HowSeen(this)}.";
			default:
				return $"{container.HowSeen(this, true)} cannot wear {item.HowSeen(this)}.";
		}
	}

	public void Put(IGameItem item, IGameItem container, string profile, IEmote playerEmote = null,
		bool silent = false)
	{
		if (!CanPut(item, container, profile))
		{
			OutputHandler.Send(WhyCannotPut(item, container, profile));
			return;
		}

		var target = container.GetItemType<ICorpse>().OriginalCharacter;

		_heldItems.RemoveAll(x => x.Item1 == item);
		_wieldedItems.RemoveAll(x => x.Item1 == item);
		if (!silent)
		{
			OutputHandler.Handle(
				new MixedEmoteOutput(new Emote("@ put|puts $0 on $1", this, item, container),
					flags: OutputFlags.SuppressObscured).Append(playerEmote));
		}

		InventoryChanged = true;
		if (string.IsNullOrEmpty(profile))
		{
			target.Body.Wear(item, null, true);
		}
		else
		{
			target.Body.Wear(item, profile, null, true);
		}
	}

	#endregion

	public bool CanDrop(IGameItem item, int quantity)
	{
		return true;
	}

	public string WhyCannotDrop(IGameItem item, int quantity)
	{
		return "You cannot drop " + item.HowSeen(this);
	}

	public void Drop(IGameItem item, int quantity = 0, bool newStack = false, IEmote playerEmote = null,
		bool silent = false)
	{
		if (!CanDrop(item, quantity) && !silent)
		{
			OutputHandler.Send(WhyCannotDrop(item, quantity));
			return;
		}

		MixedEmoteOutput output;
		IGameItem droppedItem = null;
		var wasWielded = _heldItems.All(x => x.Item1 != item);
		if (quantity == 0 || item.DropsWhole(quantity))
		{
			droppedItem = item;
			droppedItem.RoomLayer = RoomLayer;
			item.Drop(Location);
			_heldItems.RemoveAll(x => x.Item1 == item);
			_wieldedItems.RemoveAll(x => x.Item1 == item);
			Location.Insert(item, newStack);
			output = new MixedEmoteOutput(new Emote("@ drop|drops $0", this, item),
				flags: OutputFlags.SuppressObscured);
		}
		else
		{
			var newItem = item.Drop(Location, quantity);
			droppedItem = newItem;
			droppedItem.RoomLayer = RoomLayer;
			Location.Insert(newItem, newStack);
			output = new MixedEmoteOutput(new Emote("@ drop|drops $0", this, newItem),
				flags: OutputFlags.SuppressObscured);
		}

		if (!silent)
		{
			output.Append(playerEmote);
			OutputHandler.Handle(output);
		}

		InventoryChanged = true;
		OnInventoryChange?.Invoke(wasWielded ? InventoryState.Wielded : InventoryState.Held, InventoryState.Dropped,
			droppedItem);
		droppedItem.InvokeInventoryChange(wasWielded ? InventoryState.Wielded : InventoryState.Held,
			InventoryState.Dropped);
		// Handle Events
		HandleEvent(EventType.CharacterDroppedItem, Actor, droppedItem);
		droppedItem.HandleEvent(EventType.ItemDropped, Actor, droppedItem);
		foreach (var witness in Location.EventHandlers.Except(new IHandleEvents[] { Actor, droppedItem }))
		{
			witness.HandleEvent(EventType.CharacterDroppedItemWitness, Actor, droppedItem, witness);
		}

		foreach (var witness in ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterDroppedItemWitness, Actor, droppedItem, witness);
		}

		CheckConsequences();
	}

	public bool CanGive(IGameItem item, IBody target, int quantity = 0)
	{
		return CanDrop(item, quantity) && target.CanGet(quantity == 0 ? item : item.PeekSplit(quantity), 0);
	}

	public string WhyCannotGive(IGameItem item, IBody target, int quantity = 0)
	{
		var dummy = item.PeekSplit(quantity);
		if (!CanDrop(item, quantity))
		{
			return "You cannot give that away.";
		}

		switch (target.HoldLocs.WhyCannotGrab(dummy, target))
		{
			case WhyCannotGrabReason.HandsFull:
				return target.HowSeen(this, true) + " has no free " + target.WielderDescriptionPlural +
				       " to accept " + dummy.HowSeen(this) + ".";
			case WhyCannotGrabReason.HandsTooDamaged:
				return target.HowSeen(this, true, DescriptionType.Possessive) + " " +
				       target.WielderDescriptionPlural + " are too damaged to accept " + dummy.HowSeen(this) + ".";
			case WhyCannotGrabReason.InventoryFull:
				return target.HowSeen(this, true) + " cannot hold " + dummy.HowSeen(this) + ".";
			case WhyCannotGrabReason.NoFreeUndamagedHands:
				return target.HowSeen(this, true) + " has no free, undamaged " + target.WielderDescriptionPlural +
				       " to accept " + dummy.HowSeen(this) + ".";
			default:
				return target.HowSeen(this, true) + " cannot hold " + dummy.HowSeen(this) + ".";
		}
	}

	public void Give(IGameItem item, IBody target, int quantity = 0, IEmote playerEmote = null)
	{
		if (!CanGive(item, target, quantity))
		{
			OutputHandler.Send(WhyCannotGive(item, target, quantity));
			return;
		}

		MixedEmoteOutput output;
		IGameItem givenItem = null;
		var wasWielded = _heldItems.All(x => x.Item1 != item);
		if (quantity == 0 || item.DropsWhole(quantity))
		{
			givenItem = item;
			Take(item);
			target.Get(item.Get(target), silent: true);
			output = new MixedEmoteOutput(new Emote("@ give|gives $0 to $1", this, item, target),
				flags: OutputFlags.SuppressObscured);
		}
		else
		{
			var newItem = item.Get(target, quantity);
			givenItem = newItem;
			target.Get(newItem, silent: true);
			output = new MixedEmoteOutput(new Emote("@ give|gives $0 to $1", this, newItem, target),
				flags: OutputFlags.SuppressObscured);
		}

		output.Append(playerEmote);
		OutputHandler.Handle(output);
		InventoryChanged = true;
		target.InventoryChanged = true;

		OnInventoryChange?.Invoke(wasWielded ? InventoryState.Wielded : InventoryState.Held, InventoryState.Dropped,
			givenItem);
		givenItem.InvokeInventoryChange(wasWielded ? InventoryState.Wielded : InventoryState.Held,
			InventoryState.Dropped);

		// Handle events
		HandleEvent(EventType.CharacterGiveItemGiver, Actor, target.Actor, givenItem);
		target.HandleEvent(EventType.CharacterGiveItemReceiver, Actor, target.Actor, givenItem);
		givenItem.HandleEvent(EventType.ItemGiven, Actor, target.Actor, givenItem);
		foreach (var witness in Location.EventHandlers.Except(new IHandleEvents[] { Actor, target.Actor }))
		{
			witness.HandleEvent(EventType.CharacterGiveItemWitness, Actor, target.Actor, givenItem, witness);
		}

		foreach (var witness in ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterGiveItemWitness, Actor, target.Actor, givenItem, witness);
		}
	}

	public bool CanGive(IGameItem item, ICorpse target, int quantity = 0)
	{
		return CanDrop(item, quantity) &&
		       target.OriginalCharacter.Body.CanGet(quantity == 0 ? item : item.PeekSplit(quantity), 0);
	}

	public string WhyCannotGive(IGameItem item, ICorpse target, int quantity = 0)
	{
		var dummy = item.PeekSplit(quantity);
		if (!CanDrop(item, quantity))
		{
			return "You cannot give that away.";
		}

		switch (target.OriginalCharacter.Body.HoldLocs.WhyCannotGrab(dummy, target.OriginalCharacter.Body))
		{
			case WhyCannotGrabReason.HandsFull:
				return target.Parent.HowSeen(this, true) + " has no free " +
				       target.OriginalCharacter.Body.WielderDescriptionPlural + " to accept " + dummy.HowSeen(this) +
				       ".";
			case WhyCannotGrabReason.HandsTooDamaged:
				return target.Parent.HowSeen(this, true, DescriptionType.Possessive) + " " +
				       target.OriginalCharacter.Body.WielderDescriptionPlural + " are too damaged to accept " +
				       dummy.HowSeen(this) + ".";
			case WhyCannotGrabReason.InventoryFull:
				return target.Parent.HowSeen(this, true) + " cannot hold " + dummy.HowSeen(this) + ".";
			case WhyCannotGrabReason.NoFreeUndamagedHands:
				return target.Parent.HowSeen(this, true) + " has no free, undamaged " +
				       target.OriginalCharacter.Body.WielderDescriptionPlural + " to accept " + dummy.HowSeen(this) +
				       ".";
			default:
				return target.Parent.HowSeen(this, true) + " cannot hold " + dummy.HowSeen(this) + ".";
		}
	}

	public void Give(IGameItem item, ICorpse target, int quantity = 0, IEmote playerEmote = null)
	{
		if (!CanGive(item, target, quantity))
		{
			OutputHandler.Send(WhyCannotGive(item, target, quantity));
			return;
		}

		MixedEmoteOutput output;
		IGameItem givenItem = null;
		var wasWielded = _heldItems.All(x => x.Item1 != item);
		if (quantity == 0 || item.DropsWhole(quantity))
		{
			givenItem = item;
			Take(item);
			target.OriginalCharacter.Body.Get(item.Get(target.OriginalCharacter.Body), silent: true);
			output = new MixedEmoteOutput(new Emote("@ give|gives $0 to $1", this, item, target.Parent),
				flags: OutputFlags.SuppressObscured);
		}
		else
		{
			var newItem = item.Get(target.OriginalCharacter.Body, quantity);
			givenItem = newItem;
			target.OriginalCharacter.Body.Get(newItem, silent: true);
			output = new MixedEmoteOutput(new Emote("@ give|gives $0 to $1", this, newItem, target.Parent),
				flags: OutputFlags.SuppressObscured);
		}

		output.Append(playerEmote);
		OutputHandler.Handle(output);
		InventoryChanged = true;
		target.OriginalCharacter.Body.InventoryChanged = true;
		OnInventoryChange?.Invoke(wasWielded ? InventoryState.Wielded : InventoryState.Held, InventoryState.Dropped,
			givenItem);
		givenItem.InvokeInventoryChange(wasWielded ? InventoryState.Wielded : InventoryState.Held,
			InventoryState.Dropped);

		// Handle events
		HandleEvent(EventType.CharacterGiveItemGiver, Actor, target.OriginalCharacter, givenItem);
		target.OriginalCharacter.HandleEvent(EventType.CharacterGiveItemReceiver, Actor, target.OriginalCharacter,
			givenItem);
		givenItem.HandleEvent(EventType.ItemGiven, Actor, target.OriginalCharacter, givenItem);
		foreach (var witness in Location.EventHandlers.Except(new IHandleEvents[] { Actor, target.OriginalCharacter })
		        )
		{
			witness.HandleEvent(EventType.CharacterGiveItemWitness, Actor, target.OriginalCharacter, givenItem,
				witness);
		}

		foreach (var witness in ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterGiveItemWitness, Actor, target.OriginalCharacter, givenItem,
				witness);
		}
	}

	public void Take(IGameItem item)
	{
		var oldState = InventoryState.Held;
		if (_wieldedItems.Any(x => x.Item1 == item))
		{
			oldState = InventoryState.Wielded;
		}
		else if (_wornItems.Any(x => x.Item == item))
		{
			oldState = InventoryState.Worn;
		}
		else if (_prosthetics.Any(x => x.Parent == item))
		{
			oldState = InventoryState.Prosthetic;
		}
		else if (_implants.Any(x => x.Parent == item))
		{
			oldState = InventoryState.Implanted;
		}

		_heldItems.RemoveAll(x => x.Item1 == item);
		_wieldedItems.RemoveAll(x => x.Item1 == item);
		_wornItems.RemoveAll(x => x.Item == item);
		_prosthetics.RemoveWhere(x => x.Parent == item);
		_implants.RemoveAll(x => x.Parent == item);

		var wield = item.GetItemType<IWieldable>();
		if (wield != null)
		{
			wield.PrimaryWieldedLocation = null;
		}

		item.GetItemType<IWearable>()?.UpdateWear(null, null);
		item.ContainedIn?.Take(item);
		item.Drop(null);
		item.RoomLayer = RoomLayer;
		InventoryChanged = true;
		OnInventoryChange?.Invoke(oldState, InventoryState.Dropped, item);
		RemoveEffect(item.GetItemType<IRestraint>()?.Effect);
		CheckConsequences();
	}

	private void CheckConsequences()
	{
		if (PositionState.Upright &&
		    !PositionState.In(PositionSwimming.Instance, PositionClimbing.Instance, PositionFlying.Instance) &&
		    !CanStand(false))
		{
			Actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ tumble|tumbles to the ground!", Actor)));
			Actor.PositionState = PositionSprawled.Instance;
		}
	}

	public IGameItem Take(IGameItem item, int quantity)
	{
		if (item.DropsWhole(quantity))
		{
			Take(item);
			return item;
		}

		return item.Drop(null, quantity);
	}

	/// <summary>
	/// Swaps the hands in which the first and second items are held/wielded
	/// </summary>
	/// <param name="firstItem">The first item</param>
	/// <param name="secondItem">The second item</param>
	/// <returns>True if the swap took place</returns>
	public bool Swap(IGameItem firstItem, IGameItem secondItem)
	{
		if (!HeldOrWieldedItems.Contains(firstItem) || (secondItem != null && !HeldOrWieldedItems.Contains(secondItem)))
		{
			Actor.Send("You cannot swap items that you aren't holding.");
			return false;
		}

		var item1Wielded = WieldedItems.Contains(firstItem);
		var item2Wielded = WieldedItems.Contains(secondItem);

		IBodypart FindTargetLocation(IGameItem otherItem, bool itemWielded)
		{
			if (_wieldedItems.Any(x => x.Item1 == otherItem && (!itemWielded || x.Item2.SelfUnwielder())))
			{
				return _wieldedItems.First(x => x.Item1 == otherItem && (!itemWielded || x.Item2.SelfUnwielder()))
				                    .Item2;
			}

			if (_heldItems.Any(x => x.Item1 == otherItem))
			{
				return _heldItems.First(x => x.Item1 == otherItem).Item2;
			}

			if (itemWielded && WieldLocs.Any(x => _wieldedItems.All(y => y.Item2 != x)))
			{
				return WieldLocs.First(x => _wieldedItems.All(y => y.Item2 != x));
			}

			if (HoldLocs.Any(x => _heldItems.All(y => y.Item2 != x)))
			{
				return HoldLocs.First(x => _heldItems.All(y => y.Item2 != x));
			}

			return null;
		}

		var targetloc1 = FindTargetLocation(secondItem, item1Wielded);
		var targetloc2 = FindTargetLocation(firstItem, item2Wielded);
		_heldItems.RemoveAll(x => x.Item1 == firstItem || x.Item1 == secondItem);
		_wieldedItems.RemoveAll(x => x.Item1 == firstItem || x.Item1 == secondItem);

		void SwapItem(IGameItem item, IBodypart part, IBodypart oldPart, bool wielded)
		{
			InventoryChanged = true;
			if (wielded && part is IWield w1)
			{
				if (!Wield(item, w1, null, true))
				{
					if (part is IGrab g1)
					{
						_heldItems.Add(Tuple.Create(item, g1));
					}
					else
					{
						var fallBack = HoldLocs.Except(part).Except(oldPart).OfType<IGrab>()
						                       .FirstOrDefault(x =>
							                       x.CanGrab(firstItem, this) == WearlocGrabResult.Success);
						if (fallBack == null)
						{
							Actor.Send(
								$"You set {item.HowSeen(Actor)} down as you can't figure out a way to hold it.");
							Take(item);
							item.RoomLayer = RoomLayer;
							Actor.Location.Insert(item);
							return;
						}
						else
						{
							_heldItems.Add(Tuple.Create(item, fallBack));
						}
					}
				}
			}
			else if (part is IGrab g1)
			{
				_heldItems.Add(Tuple.Create(item, g1));
			}
			else
			{
				var fallBack = HoldLocs.Except(part).Except(oldPart).OfType<IGrab>()
				                       .FirstOrDefault(x => x.CanGrab(item, this) == WearlocGrabResult.Success);
				if (fallBack == null)
				{
					Actor.Send(
						$"You set {item.HowSeen(Actor)} down as you can't figure out a way to hold it.");
					Take(item);
					item.RoomLayer = RoomLayer;
					Actor.Location.Insert(item);
					return;
				}
				else
				{
					_heldItems.Add(Tuple.Create(item, fallBack));
				}
			}

			if (!WieldedItems.Contains(item))
			{
				UpdateDescriptionHeld(item);
				if (item.IsItemType<IWieldable>())
				{
					item.GetItemType<IWieldable>().PrimaryWieldedLocation = null;
				}
			}
		}

		Actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ swap|swaps the {Prototype.WielderDescriptionPlural} #0 have|has $1$?2| and $2||$ in.", Actor, Actor,
			firstItem, secondItem)));
		SwapItem(firstItem, targetloc1, targetloc2, item1Wielded);
		if (secondItem != null)
		{
			SwapItem(secondItem, targetloc2, targetloc1, item2Wielded);
		}

		RecalculateItemHelpers();
		return true;
	}

	#endregion

	#region IWear Implementation

	protected List<IWear> _wearlocs;

	public IEnumerable<IWear> WearLocs => _wearlocs;

	public IEnumerable<IGameItem> WornItemsFor(IBodypart prototype)
	{
		return _wornItems.Where(x => prototype.CountsAs(x.Wearloc)).Select(x => x.Item);
	}

	public IEnumerable<Tuple<IGameItem, IWearlocProfile>> WornItemsProfilesFor(IBodypart proto)
	{
		return _wornItems.Where(x => proto.CountsAs(x.Wearloc)).Select(x => Tuple.Create(x.Item, x.Profile));
	}

	public ILookup<IWear, int> WornItemCounts
	{
		get
		{
			return _wearlocs.Select(x => Tuple.Create(x, _wornItems.Count(y => y.Wearloc == x)))
			                .ToLookup(x => x.Item1, x => x.Item2);
		}
	}

	public void RemoveItem(IGameItem item)
	{
#if DEBUG
		if (_wornItems.Any(x => x.Item1.Id == item.Id && x.Item1 != item))
		{
			throw new ApplicationException("Worn item had the same ID but did not reference equal on removal.");
		}
#endif
		_wornItems.RemoveAll(x => x.Item.Equals(item));
		item.GetItemType<IWearable>()?.UpdateWear(null, null);
		InventoryChanged = true;
		OnInventoryChange?.Invoke(InventoryState.Worn, InventoryState.Held, item);
		item.InvokeInventoryChange(InventoryState.Worn, InventoryState.Held);
		CheckConsequences();
	}

	public void UpdateDescriptionWorn(IGameItem item)
	{
		//var locs = _wornItems.Where(x => x.Item1 == item).Select(x => x.Item2.CoverInformation(item, this));
		var sb = new StringBuilder();
		sb.Append("<");
		sb.Append(item.GetItemType<IWearable>().CurrentProfile.WearStringInventory);
		sb.Append(" ");
		sb.Append(DescribeBodypartGroup(_wornItems.Where(x => x.Item == item).Select(x => x.Wearloc)));
		sb.Append(">");

		item.GetItemType<IHoldable>().CurrentInventoryDescription = $"{sb,-35}";
	}

	public void UpdateDescriptionAttached(IGameItem item)
	{
		item.GetItemType<IHoldable>().CurrentInventoryDescription =
			$"{new StringBuilder().Append("<").Append("attached to ").Append(item.GetItemType<IBeltable>().ConnectedTo.Parent.Name).Append(">"),-35}";
	}

	private readonly List<(IGameItem Item, IWear Wearloc, IWearlocProfile Profile)> _wornItems =
		new();

	private List<IGameItem> _directWornItems;
	public IEnumerable<IGameItem> DirectWornItems => _directWornItems;

	private List<IGameItem> _wornItemsOnly;
	public IEnumerable<IGameItem> WornItems => _wornItemsOnly;

	public bool IsOuterwear(IGameItem item)
	{
		return _outerwear.Contains(item);
	}

	private HashSet<IGameItem> _outerwear;

	/// <summary>
	/// WornItems that are the outermost item for any of their wearlocs
	/// </summary>
	public IEnumerable<IGameItem> Outerwear => _outerwear;

	private List<IGameItem> _exposedItems = new();
	public IEnumerable<IGameItem> ExposedItems => _exposedItems;

	public IEnumerable<(IGameItem Item, IWear Wearloc, IWearlocProfile Profile)> WornItemsFullInfo => _wornItems;

	public IEnumerable<IGameItem> OrderedWornItems
	{
		get
		{
			var items =
				_wornItems.GroupBy(x => x.Item, x => x)
				          .OrderBy(x => x.Average(y => y.Wearloc.DisplayOrder))
				          .Select(x => x.Key)
				          .ToList();
			foreach (
				var beltitem in
				items.SelectNotNull(x => x.GetItemType<IBelt>()).SelectMany(x => x.ConnectedItems).ToList())
			{
				items.Insert(items.IndexOf(beltitem.ConnectedTo.Parent), beltitem.Parent);
			}

			return items;
		}
	}

	public void RemoveItem(IGameItem item, IEmote playerEmote, ICharacter remover)
	{
		var obscurer = item.GetItemType<IObscureCharacteristics>();
		MixedEmoteOutput output = null;
		if (!string.IsNullOrEmpty(obscurer?.RemovalEcho))
		{
			output = new CharacteristicAwareOutput(
				new Emote("$1 remove|removes $2 from $0", Actor, Actor, remover, item), item,
				obscurer.RemovalEcho, true);
		}
		else
		{
			output = new MixedEmoteOutput(new Emote("@ remove|removes $0 from $1", remover, item, Actor));
		}

		output.Append(playerEmote);
		OutputHandler.Handle(output);
		RemoveItem(item);
	}

	public void RemoveItem(IGameItem item, IEmote playerEmote, bool silent = false,
		ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		if (!CanRemoveItem(item, ignoreFlags))
		{
			if (!silent)
			{
				OutputHandler.Send(WhyCannotRemove(item, false, ignoreFlags));
			}

			return;
		}

		var obscurer = item.GetItemType<IObscureCharacteristics>();
		if (!silent)
		{
			MixedEmoteOutput output = null;
			if (!string.IsNullOrEmpty(obscurer?.RemovalEcho))
			{
				output = new CharacteristicAwareOutput(new Emote("@ remove|removes $0", this, item), item,
					obscurer.RemovalEcho, true, flags: OutputFlags.SuppressObscured);
			}
			else
			{
				output = new MixedEmoteOutput(new Emote("@ remove|removes $0", this, item),
					flags: OutputFlags.SuppressObscured);
			}

			output.Append(playerEmote);
			OutputHandler.Handle(output);
		}

		RemoveItem(item);
		if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreFreeHands))
		{
			Get(item, silent: true, ignoreFlags: ignoreFlags);
		}

		InventoryChanged = true;
	}

	public void Wear(IGameItem item, string profile, IEmote playerEmote, bool silent = false)
	{
		if (string.IsNullOrEmpty(profile))
		{
			Wear(item, playerEmote, silent);
			return;
		}

		var wearable = item.GetItemType<IWearable>();
		if (wearable == null)
		{
			OutputHandler.Send(item.HowSeen(this, true) + " is not something that can be worn.");
			return;
		}

		var wprofile =
			wearable.Profiles.FirstOrDefault(x =>
				x.Name.ToLowerInvariant().StartsWith(profile, StringComparison.Ordinal));
		if (wprofile == null)
		{
			OutputHandler.Send("That is not a valid way to wear " + item.HowSeen(this) + ".");
			return;
		}

		Wear(item, wprofile, playerEmote, silent);
	}

	public bool CanWear(IGameItem item, string text)
	{
		var wearable = item.GetItemType<IWearable>();
		if (wearable == null)
		{
			return false;
		}

		var profile =
			wearable.Profiles.FirstOrDefault(
				x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		return CanWear(item, profile);
	}

	private static double _maximumLayerWeight;

	public static double MaximumLayerWeight
	{
		get
		{
			if (_maximumLayerWeight == 0)
			{
				_maximumLayerWeight = Futuremud.Games.First().GetStaticDouble("MaximumLayerWeight");
			}

			return _maximumLayerWeight;
		}
	}

	public bool CanWear(IGameItem item, IWearProfile profile)
	{
		if (profile is null)
		{
			return false;
		}

		var wearable = item.GetItemType<IWearable>();
		if (wearable is null)
		{
			return false;
		}

		var profiles = profile.Profile(this);
		if (profile.RequireContainerIsEmpty && item.GetItemType<IContainer>()?.Contents.Any() == true)
		{
			return false;
		}

		if (item.GetItemType<IRestraint>()?.RestraintType == RestraintType.Binding)
		{
			return false;
		}

		foreach (var availableProfile in profiles)
		{
			if (WornItemsFor(availableProfile.Key).Sum(x => x.GetItemType<IWearable>().LayerWeightConsumption) + wearable.LayerWeightConsumption > MaximumLayerWeight)
			{
				return false;
			}
		}

		if (!wearable.CanWear(this, profile))
		{
			return false;
		}

		return true;
	}

	public bool CanWear(IGameItem item)
	{
		var wearable = item.GetItemType<IWearable>();
		if (wearable == null)
		{
			return false;
		}

		return wearable.CanWear(this) && 
		       wearable.Profiles.Any(x => CanWear(item, x));
	}

	public IWearProfile WhichProfile(IGameItem item)
	{
		var wearable = item.GetItemType<IWearable>();
		if (wearable == null)
		{
			return null;
		}

		// As we haven't specified a profile, we'll try the default first
		return CanWear(item, wearable.DefaultProfile)
			? wearable.DefaultProfile
			: wearable.Profiles.Except(wearable.DefaultProfile).FirstOrDefault(profile => CanWear(item, profile));

		// Default wasn't available, try the rest
	}

	public void Wear(IGameItem item, IEmote playerEmote, bool silent = false)
	{
		if (!CanWear(item))
		{
			if (!silent)
			{
				var wearable = item.GetItemType<IWearable>();
				OutputHandler.Send(
					wearable?.Profiles.Count() == 1 ? 
						WhyCannotWear(item, wearable.Profiles.First()) : 
						WhyCannotWear(item));
			}

			return;
		}

		Wear(item, WhichProfile(item), playerEmote, silent);
	}

	public void Restrain(IGameItem item, IWearProfile profile, ICharacter restrainer, IGameItem targetItem,
		IEmote emote = null,
		bool silent = false)
	{
		if (!silent && restrainer != null)
		{
			var output =
				new MixedEmoteOutput(
					new Emote(
						$"@ {profile.WearAction1st}|{profile.WearAction3rd} {profile.WearAffix} with $2", restrainer,
						restrainer, this, item),
					flags: OutputFlags.SuppressObscured);
			output.Append(emote);
			OutputHandler.Handle(output);
		}

		restrainer.Body.Take(item);
		Take(item);
		_wornItems.AddRange(profile.Profile(this).Select(x => (item, x.Key, x.Value)));
		item.GetItemType<IWearable>().UpdateWear(this, profile);
		UpdateDescriptionWorn(item);
		InventoryChanged = true;
		var limbs = Limbs.Where(x =>
			item.GetItemType<IRestraint>().Limbs.Contains(x.LimbType) &&
			profile.AllProfiles.Any(y => x.Parts.Contains(y.Key))).ToList();
		AddEffect(new RestraintEffect(this, limbs, targetItem, item));
	}

	public bool LoadtimeWear(IGameItem item, IWearProfile profile)
	{
		if (!CanWear(item, profile))
		{
			return false;
		}

		_wornItems.AddRange(profile.Profile(this).Select(x => (item, x.Key, x.Value)));
		_carriedItems.Add(item);
		item.GetItemType<IWearable>().UpdateWear(this, profile);
		UpdateDescriptionWorn(item);
		return true;
	}

	public void Wear(IGameItem item, IWearProfile profile, IEmote playerEmote = null, bool silent = false)
	{
		if (!CanWear(item, profile))
		{
			if (!silent)
			{
				OutputHandler.Send(WhyCannotWear(item, profile));
			}

			return;
		}

		if (!silent)
		{
			var output =
				new MixedEmoteOutput(
					new Emote(
						$"@ {profile.WearAction1st}|{profile.WearAction3rd} {profile.WearAffix} $0", this, item),
					flags: OutputFlags.SuppressObscured);
			output.Append(playerEmote);
			OutputHandler.Handle(output);
		}

		var wasWielded = _heldItems.All(x => x.Item1 != item);
		_heldItems.RemoveAll(x => x.Item1 == item);
		_wieldedItems.RemoveAll(x => x.Item1 == item);
		_wornItems.AddRange(profile.Profile(this).Select(x => (item, x.Key, x.Value)));
		item.GetItemType<IWearable>().UpdateWear(this, profile);
		UpdateDescriptionWorn(item);
		InventoryChanged = true;
		OnInventoryChange?.Invoke(wasWielded ? InventoryState.Wielded : InventoryState.Held, InventoryState.Worn,
			item);
		item.InvokeInventoryChange(wasWielded ? InventoryState.Wielded : InventoryState.Held, InventoryState.Worn);
		CheckConsequences();
		HandleEvent(EventType.ItemWorn, item, Actor);
		foreach (var witness in Location.EventHandlers)
		{
			HandleEvent(EventType.ItemWornWitness, item, Actor, witness);
		}
	}

	public bool CanRemoveItem(IGameItem item, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		if (!CanGet(item, 0, ignoreFlags))
		{
			return false;
		}

		if (item.IsItemType<IBeltable>() && item.GetItemType<IBeltable>().ConnectedTo != null)
		{
			return false;
		}

		if (item.IsItemType<IRestraint>())
		{
			return false;
		}

		return _wornItems.Where(x => x.Item == item)
		                 .All(
			                 x =>
				                 _wornItems.Last(
					                           y =>
						                           y.Wearloc == x.Wearloc &&
						                           (y.Item == x.Item || y.Profile.PreventsRemoval))
				                           .Equals(x));
	}

	public bool CanBeRemoved(IGameItem item, ICharacter remover)
	{
		if (!Actor.IsTrustedAlly(remover) && Actor.EffectsOfType<BeDressedEffect>().All(x => x.Dresser != remover))
		{
			if (!Actor.State.HasFlag(CharacterState.Dead) && !Actor.State.HasFlag(CharacterState.Unconscious) &&
			    !Actor.State.HasFlag(CharacterState.Sleeping))
			{
				return false;
			}
		}

		if (item.IsItemType<IRestraint>())
		{
			return false;
		}

		// TODO - effects that prevent removal
		return
			DirectItems.Contains(item) &&
			_wornItems.Where(x => x.Item == item)
			          .All(
				          x =>
					          _wornItems.Last(
						                    y =>
							                    y.Wearloc == x.Wearloc &&
							                    (y.Item == x.Item || y.Profile.PreventsRemoval))
					                    .Equals(x));
	}

	public bool CanDress(IGameItem item, ICharacter dresser, IWearProfile profile = null)
	{
		if (!dresser.Body.CanDrop(item, 0))
		{
			return false;
		}

		if (!Actor.IsAlly(dresser) && Actor.EffectsOfType<BeDressedEffect>().All(x => x.Dresser != dresser))
		{
			if (!Actor.State.HasFlag(CharacterState.Dead) && !Actor.State.HasFlag(CharacterState.Unconscious))
			{
				return false;
			}
		}

		if (!CanWear(item, profile))
		{
			return false;
		}

		return true;
	}

	public string WhyCannotDress(IGameItem item, ICharacter dresser, IWearProfile profile = null)
	{
		if (!dresser.Body.CanDrop(item, 0))
		{
			return dresser.Body.WhyCannotDrop(item, 0);
		}

		if (!Actor.IsAlly(dresser) && Actor.EffectsOfType<BeDressedEffect>().All(x => x.Dresser != dresser))
		{
			if (!Actor.State.HasFlag(CharacterState.Dead) && !Actor.State.HasFlag(CharacterState.Unconscious))
			{
				return
					$"{Actor.HowSeen(dresser, true)} is neither knocked out or dead and does not consent to you dressing them.";
			}
		}

		if (!CanWear(item, profile))
		{
			switch (WearLocs.WhyCannotDrape(item, profile, this))
			{
				case WhyCannotDrapeReason.SpecificProfileNoMatch:
					return $"{Actor.HowSeen(dresser, true)} cannot wear {item.HowSeen(dresser)} in that way.";
				case WhyCannotDrapeReason.TooBulky:
					return
						$"{Actor.HowSeen(dresser, true)} cannot wear {item.HowSeen(dresser)} because it is too bulky to fit over what {Actor.ApparentGender(dresser).Subjective()} is already wearing.";
				case WhyCannotDrapeReason.ProgFailed:
					return item.GetItemType<IWearable>().WhyCannotWearProgText(this);
				case WhyCannotDrapeReason.TooManyItems:
					return
						$"{Actor.HowSeen(dresser, true)} cannot wear {item.HowSeen(dresser)} because {Actor.ApparentGender(dresser).Subjective()} is already wearing too many things on the same locations.";
				default:
					switch (WearLocs.WhyCannotDrape(item, this))
					{
						case WhyCannotDrapeReason.BadFit:
							return
								$"{item.HowSeen(dresser, true)} is such a bad fit for {Actor.HowSeen(dresser)} that {Actor.ApparentGender(dresser).Subjective()} cannot wear it.";
						case WhyCannotDrapeReason.NoProfilesMatch:
							return
								$"{Actor.HowSeen(dresser, true)} has no way to wear {item.HowSeen(dresser)} right now.";
						case WhyCannotDrapeReason.NotBodyType:
							return
								$"{Actor.HowSeen(dresser, true)} is not of the correct body type to wear {item.HowSeen(dresser)}";
						case WhyCannotDrapeReason.NotIDrapeable:
							return $"{item.HowSeen(dresser, true)} is not something that can be worn.";
						default:
							return $"{Actor.HowSeen(dresser, true)} cannot wear {item.HowSeen(dresser)}.";
					}
			}
		}

		throw new ApplicationException("Unknown WhyCannotDress reason");
	}

	public bool Dress(IGameItem item, ICharacter dresser, IWearProfile profile = null, IEmote emote = null)
	{
		var tempProfile = profile ?? WhichProfile(item) ?? item.GetItemType<IWearable>()?.DefaultProfile;

		//If no profile was passed in, grab the default

		if (!CanDress(item, dresser, tempProfile))
		{
			dresser.Send(WhyCannotDress(item, dresser, tempProfile));
			return false;
		}

		dresser.OutputHandler.Handle(new MixedEmoteOutput(new Emote("@ dress|dresses $0 in $1", dresser, Actor, item))
			.Append(emote));
		dresser.Body.Take(item);
		Wear(item, tempProfile, silent: true);
		return true;
	}

	#endregion

	#region IInventory Implementation

	public void RecalculateItemHelpers()
	{
		_directWornItems = _wornItems.Select(x => x.Item).Distinct().ToList();
		_wornItemsOnly = DirectWornItems.Concat(
			DirectWornItems.SelectNotNull(x => x.GetItemType<IBelt>())
			               .SelectMany(x => x.ConnectedItems, (x, y) => y.Parent)).ToList();
		_allItems = WieldedItems.Concat(HeldItems).Concat(WornItems).Concat(Prosthetics.Select(x => x.Parent))
		                        .Concat(Implants.Select(x => x.Parent)).Concat(Wounds.SelectNotNull(x => x.Lodged))
		                        .Distinct().ToList();
		_externalItems = WieldedItems.Concat(HeldItems).Concat(WornItems).Concat(Prosthetics.Select(x => x.Parent))
		                             .Concat(Implants.Where(x => x.External).Select(x => x.Parent)).Distinct().ToList();
		_carriedItems = HeldOrWieldedItems.Concat(DirectWornItems).ToList();
		_directItems = WieldedItems.Concat(HeldItems).Concat(DirectWornItems).Distinct().ToList();
		_outerwear = new HashSet<IGameItem>(_wornItems
		                                    .GroupBy(x => x.Wearloc)
		                                    .SelectNotNull(y => y
		                                                        .Reverse()
		                                                        .SkipWhile(x => !x.Profile.PreventsRemoval)
		                                                        .FirstOrDefault()
		                                                        .Item)
		                                    .Distinct());
		_itemsWornAgainstSkin = new HashSet<IGameItem>(_wornItems
		                                               .GroupBy(x => x.Wearloc)
		                                               .SelectNotNull(y => y
		                                                                   .FirstOrDefault()
		                                                                   .Item)
		                                               .Distinct());
		var parts = new HashSet<IBodypart>(Bodyparts);
		var transparentParts = new HashSet<IBodypart>(Bodyparts);
		_visiblySeveredBodyparts = SeveredRoots.ToHashSet();
		foreach (var item in DirectWornItems)
		foreach (var profile in item.GetItemType<IWearable>().CurrentProfile.AllProfiles)
		{
			if (profile.Value.HidesSeveredBodyparts)
			{
				_visiblySeveredBodyparts.Remove(profile.Key);
			}

			parts.Remove(profile.Key);
			if (!profile.Value.Transparent)
			{
				transparentParts.Remove(profile.Key);
			}
		}

		_exposedBodyparts = parts;
		_externalItemsForOtherActors = WieldedItems
		                               .Concat(HeldItems)
		                               .Concat(_outerwear)
		                               .Concat(Prosthetics
		                                       .Where(x => x.IncludedParts.Any(y => transparentParts.Contains(y)))
		                                       .Select(x => x.Parent))
		                               .Concat(Implants
		                                       .Where(x => x.External && transparentParts.Contains(x.TargetBodypart))
		                                       .Select(x => x.Parent))
		                               .Concat(Wounds.Where(x => transparentParts.Contains(x.Bodypart))
		                                             .SelectNotNull(x => x.Lodged))
		                               .Distinct().ToList();
		_exposedItems = WieldedItems
		                .Concat(HeldItems)
		                .Concat(_outerwear)
		                .Concat(Prosthetics.Where(x => x.IncludedParts.Any(y => parts.Contains(y)))
		                                   .Select(x => x.Parent))
		                .Concat(Implants.Where(x => x.External && parts.Contains(x.TargetBodypart))
		                                .Select(x => x.Parent))
		                .Concat(Wounds.Where(x => parts.Contains(x.Bodypart)).SelectNotNull(x => x.Lodged))
		                .Distinct().ToList();
	}

	private List<IGameItem> _allItems = new();

	/// <summary>
	/// All items contained within the body, including everything from ExternalItems as well as internal implants. Basically, anything that would be left if the body itself was discombobulated and only the other items were left behind.
	/// </summary>
	public IEnumerable<IGameItem> AllItems => _allItems;

	private List<IGameItem> _externalItems = new();
	public IEnumerable<IGameItem> ExternalItems => _externalItems;

	private List<IGameItem> _externalItemsForOtherActors = new();

	/// <summary>
	/// Items that would appear in ExternalItems but also are exposed enough to be targeted by other actors
	/// </summary>
	public IEnumerable<IGameItem> ExternalItemsForOtherActors => _externalItemsForOtherActors;

	private List<IGameItem> _carriedItems = new();
	public IEnumerable<IGameItem> CarriedItems => _carriedItems;

	private List<IGameItem> _directItems = new();
	public IEnumerable<IGameItem> DirectItems => _directItems;

	private HashSet<IBodypart> _exposedBodyparts;

	/// <summary>
	/// Bodyparts that have no items covering them
	/// </summary>
	public IEnumerable<IBodypart> ExposedBodyparts => _exposedBodyparts;

	private HashSet<IBodypart> _visiblySeveredBodyparts;

	public IEnumerable<IBodypart> VisiblySeveredBodyparts => _visiblySeveredBodyparts;

	private HashSet<IGameItem> _itemsWornAgainstSkin;

	/// <summary>
	/// Items that are the first-worn item for any of their slots
	/// </summary>
	public IEnumerable<IGameItem> ItemsWornAgainstSkin => _itemsWornAgainstSkin;

	/// <summary>
	/// Returns all items worn (with mandatory part), held, wielded, implanted or used as a prosthetic at or downstream from the nominated bodypart
	/// </summary>
	/// <param name="part">The bodypart in question</param>
	/// <returns>A collection of items downstream from or at the part</returns>
	public IEnumerable<IGameItem> AllItemsAtOrDownstreamOfPart(IBodypart part)
	{
		var parts = Bodyparts.Where(x => x.DownstreamOfPart(part) || x == part).ToList();
		foreach (var item in _heldItems)
		{
			if (!parts.Contains(item.Item2))
			{
				continue;
			}

			yield return item.Item1;
		}

		foreach (var item in _wieldedItems)
		{
			if (!parts.Contains(item.Item2))
			{
				continue;
			}

			yield return item.Item1;
		}

		foreach (var item in _wornItems)
		{
			if (!item.Profile.Mandatory || !parts.Contains(item.Wearloc))
			{
				continue;
			}

			yield return item.Item;
		}

		foreach (var item in _implants)
		{
			if (!parts.Contains(item.TargetBodypart))
			{
				continue;
			}

			yield return item.Parent;
		}

		foreach (var item in _prosthetics)
		{
			if (!parts.Contains(item.TargetBodypart))
			{
				continue;
			}

			yield return item.Parent;
		}
	}

	public Alignment AlignmentOf(IGameItem target)
	{
		if (WieldedItems.Any(x => x == target))
		{
			var locs = _wieldedItems.Where(x => x.Item1 == target).Select(x => x.Item2).ToList();
			if (locs.Count == 1)
			{
				return locs.First().Alignment;
			}

			int leftness = 0, frontness = 0;
			foreach (var loc in locs)
			{
				switch (loc.Alignment)
				{
					case Alignment.Left:
						leftness++;
						break;
					case Alignment.Right:
						leftness--;
						break;
					case Alignment.Front:
						frontness++;
						break;
					case Alignment.Rear:
						frontness--;
						break;
				}
			}

			if (leftness != 0)
			{
				return leftness > 0 ? Alignment.Left : Alignment.Right;
			}

			if (frontness == 0)
			{
				return Alignment.Irrelevant;
			}

			return frontness > 0 ? Alignment.Front : Alignment.Rear;
		}

		return Alignment.Irrelevant;
	}

	public Orientation OrientationOf(IGameItem target)
	{
		throw new NotImplementedException();
	}

	public string WielderDescriptionSingular => Prototype.WielderDescriptionSingular;

	public string WielderDescriptionPlural => Prototype.WielderDescriptionPlural;

	public string WhyCannotWear(IGameItem item)
	{
		if (item.GetItemType<IRestraint>()?.RestraintType == RestraintType.Binding)
		{
			return $"{item.HowSeen(this, true)} can only be used with the RESTRAIN command.";
		}

		if (item.GetItemType<IWearable>()?.Profiles.All(x => !x.RequireContainerIsEmpty) == false &&
		    item.GetItemType<IContainer>()?.Contents.Any() == true)
		{
			return $"You must first empty out the contents of  {item.HowSeen(this)} before it can be worn.";
		}

		switch (WearLocs.WhyCannotDrape(item, this))
		{
			case WhyCannotDrapeReason.BadFit:
				return $"{item.HowSeen(this, true)} is such a bad fit for you that you cannot wear it.";
			case WhyCannotDrapeReason.NoProfilesMatch:
				return $"You have no way to wear {item.HowSeen(this)} right now.";
			case WhyCannotDrapeReason.NotBodyType:
				return $"You are not of the correct body type to wear {item.HowSeen(this)}";
			case WhyCannotDrapeReason.NotIDrapeable:
				return $"{item.HowSeen(this, true)} is not something that can be worn.";
			case WhyCannotDrapeReason.ProgFailed:
				return item.GetItemType<IWearable>().WhyCannotWearProgText(this);
			default:
				return $"You cannot wear {item.HowSeen(this)}.";
		}
	}

	public string WhyCannotWear(IGameItem item, string text)
	{
		var wearable = item.GetItemType<IWearable>();
		if (wearable == null)
		{
			return $"You cannot wear {item.HowSeen(Actor)} because it is not something that can be worn.";
		}

		var profile =
			wearable.Profiles.FirstOrDefault(
				x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		return WhyCannotWear(item, profile);
	}

	public string WhyCannotWear(IGameItem item, IWearProfile profile)
	{
		if (profile == null)
		{
			return $"You have no way to wear {item.HowSeen(this)} right now.";
		}

		if (profile.RequireContainerIsEmpty && item.GetItemType<IContainer>()?.Contents.Any() == true)
		{
			return $"You must first empty out the contents of  {item.HowSeen(this)} before it can be worn.";
		}

		switch (WearLocs.WhyCannotDrape(item, profile, this))
		{
			case WhyCannotDrapeReason.SpecificProfileNoMatch:
				return $"You cannot wear {item.HowSeen(this)} in that way.";
			case WhyCannotDrapeReason.TooBulky:
				return
					$"You cannot wear {item.HowSeen(this)} because it is too bulky to fit over what you are already wearing.";
			case WhyCannotDrapeReason.ProgFailed:
				return item.GetItemType<IWearable>().WhyCannotWearProgText(this);
			case WhyCannotDrapeReason.TooManyItems:
				return
					$"You cannot wear {item.HowSeen(this)} because you are already wearing too many things on the same locations.";
		}

		return WhyCannotWear(item);
	}

	public string WhyCannotRemove(IGameItem item, bool ignoreFreeHands,
		ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		if (item.IsItemType<IBeltable>() && item.GetItemType<IBeltable>().ConnectedTo != null)
		{
			return
				$"You cannot remove {item.HowSeen(Actor)} as it is attached to {item.GetItemType<IBeltable>().ConnectedTo.Parent.HowSeen(this)}. You should unattach it instead.";
		}

		if (item.IsItemType<IRestraint>())
		{
			var restraint = item.GetItemType<IRestraint>();
			if (restraint.RestraintType == RestraintType.Binding)
			{
				return $"You cannot remove bindings directly. You must undo them instead.";
			}

			if (restraint.RestraintType == RestraintType.Shackle && item.GetItemType<ILock>()?.IsLocked == true)
			{
				return $"You must first unlock {item.HowSeen(Actor)} before you can remove it.";
			}
		}

		switch (_wornItems.WhyCannotRemove(item))
		{
			case WhyCannotRemoveReason.ItemIsAttached:
				return $"You cannot remove {item.HowSeen(this)} because it is attached to something else.";
			case WhyCannotRemoveReason.ItemIsCovered:
				return
					$"You cannot remove {item.HowSeen(this)} because there are other items covering it and preventing its removal.";
			default:
				switch (HoldLocs.WhyCannotGrab(item, this))
				{
					case WhyCannotGrabReason.InventoryFull:
						return $"You cannot remove {item.HowSeen(this)} as your inventory is full.";
					case WhyCannotGrabReason.HandsFull:
						if (ignoreFreeHands)
						{
							break;
						}

						return
							$"You cannot remove {item.HowSeen(this)} as your {WielderDescriptionPlural} are full.";
					case WhyCannotGrabReason.HandsTooDamaged:
						return
							$"You cannot remove {item.HowSeen(this)} as your {WielderDescriptionPlural} are too damaged.";
					case WhyCannotGrabReason.NoFreeUndamagedHands:
						return
							$"You cannot remove {item.HowSeen(this)} as you have no free, undamaged {WielderDescriptionPlural}.";
				}

				break;
		}

		return $"You cannot remove {item.HowSeen(this)}";
	}

	public string WhyCannotBeRemoved(IGameItem item, ICharacter remover)
	{
		if (!Actor.WillingToPermitInventoryManipulation(remover))
		{
			return "You can only take things from willing people, or corpses.";
		}

		if (item.IsItemType<IRestraint>())
		{
			var restraint = item.GetItemType<IRestraint>();
			if (restraint.RestraintType == RestraintType.Binding)
			{
				return $"You cannot remove bindings directly. You must undo them instead.";
			}

			if (restraint.RestraintType == RestraintType.Shackle && item.GetItemType<ILock>()?.IsLocked == true)
			{
				return $"You must first unlock {item.HowSeen(remover)} before you can remove it.";
			}
		}

		switch (_wornItems.WhyCannotRemove(item))
		{
			case WhyCannotRemoveReason.ItemIsCovered:
				return
					$"You cannot remove {item.HowSeen(remover)} because there are other items covering it and preventing its removal.";
		}

		return $"You cannot remove {item.HowSeen(remover)}";
	}

	public string DescribeBodypartGroup(IEnumerable<IBodypart> group)
	{
		return Prototype.DescribeBodypartGroup(group);
	}

	public string DescribeHowWorn(IGameItem item)
	{
		return Prototype.DescribeBodypartGroup(_wornItems.Where(x => x.Item == item).Select(x => x.Wearloc));
	}

	public IWearableSizeRules SizeRules => Prototype.WearRulesParameter;

	public IEnumerable<Tuple<WearableItemCoverStatus, IGameItem>> CoverInformation(IGameItem item)
	{
		return _wornItems
		       .Where(x => x.Item == item)
		       .Select(x => x.Wearloc.CoverInformation(item, this))
		       .ToList();
	}

	public Dictionary<IGameItem, WearableItemCoverStatus> GetAllItemsCoverStatus(bool useIgnoreArmour)
	{
		var itemCoverageData = new Dictionary<IGameItem, (int totalWearlocs, int coveredWearlocs)>();
		var currentlyCoveredLocations = new HashSet<IWear>();

		// Process the worn items in reverse order to account for covering items
		for (int i = _wornItems.Count - 1; i >= 0; i--)
		{
			var wornItem = _wornItems[i];
			var item = wornItem.Item;
			var wearloc = wornItem.Wearloc;
			var profile = wornItem.Profile;

			// Initialize coverage data for the item if not already present
			if (!itemCoverageData.TryGetValue(item, out var data))
			{
				data = (0, 0);
			}

			data.totalWearlocs += 1;

			// Check if the wear location is currently covered
			if (currentlyCoveredLocations.Contains(wearloc))
			{
				data.coveredWearlocs += 1;
			}

			itemCoverageData[item] = data;

			// Determine if the current item covers the wear location
			if (!(profile.NoArmour && useIgnoreArmour))
			{
				currentlyCoveredLocations.Add(wearloc);
			}
		}

		// Compile the final cover status for each item
		var result = new Dictionary<IGameItem, WearableItemCoverStatus>();
		foreach (var kvp in itemCoverageData)
		{
			var item = kvp.Key;
			var (totalWearlocs, coveredWearlocs) = kvp.Value;

			WearableItemCoverStatus status = coveredWearlocs == 0
				? WearableItemCoverStatus.Uncovered
				: coveredWearlocs == totalWearlocs
					? WearableItemCoverStatus.Covered
					: WearableItemCoverStatus.TransparentlyCovered;

			result[item] = status;
		}

		return result;
	}

	public bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
	{
		var existingWearable = existingItem.GetItemType<IWearable>();
		var newWearable = newItem.GetItemType<IWearable>();
		if (existingWearable != null && newWearable != null && _wornItems.Any(x => x.Item == existingItem))
		{
			var newProfile = newWearable.Profiles.First(x => existingWearable.CurrentProfile.CompatibleWith(x));
			var newWearProfile = newProfile.Profile(this);
			var highestIndex = _wornItems.FindLastIndex(x => x.Item == existingItem);
			foreach (var element in newWearProfile)
			{
				_wornItems.Insert(highestIndex, (newItem, element.Key, element.Value));
			}

			_wornItems.RemoveAll(x => x.Item == existingItem);
			newWearable.UpdateWear(this, newProfile);
			existingWearable.UpdateWear(null, null);
			UpdateDescriptionWorn(newItem);
			InventoryChanged = true;
			return true;
		}

		if (existingItem.IsItemType<IWieldable>() && newItem.IsItemType<IWieldable>() &&
		    _wieldedItems.Any(x => x.Item1 == existingItem))
		{
			foreach (var loc in _wieldedItems.Where(x => x.Item1 == existingItem).ToList())
			{
				_wieldedItems[_wieldedItems.IndexOf(loc)] = Tuple.Create(newItem, loc.Item2);
			}

			UpdateDescriptionWielded(newItem);
			InventoryChanged = true;
			newItem.GetItemType<IHoldable>().HeldBy = this;
			newItem.GetItemType<IWieldable>().PrimaryWieldedLocation =
				existingItem.GetItemType<IWieldable>().PrimaryWieldedLocation;
			return true;
		}

		if (existingItem.IsItemType<IHoldable>() && newItem.IsItemType<IHoldable>() && newItem.GetItemType<IHoldable>().IsHoldable &&
		    _heldItems.Any(x => x.Item1 == existingItem))
		{
			foreach (var loc in _heldItems.Where(x => x.Item1 == existingItem).ToList())
			{
				_heldItems[_heldItems.IndexOf(loc)] = Tuple.Create(newItem, loc.Item2);
			}

			UpdateDescriptionHeld(newItem);
			InventoryChanged = true;
			newItem.GetItemType<IHoldable>().HeldBy = this;
			return true;
		}

		return false;
	}

	#endregion

	#region Currency-Related Versions of Inventory Commands

	public bool CanGet(ICurrency currency, decimal amount, bool exact)
	{
		var targetCoins =
			currency.FindCurrency(
				Location.LayerGameItems(RoomLayer).SelectNotNull(x => x.GetItemType<ICurrencyPile>())
				        .Where(x => Location.CanGet(x.Parent, Actor)), amount);
		if (!targetCoins.Any())
		{
			return false;
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return false;
		}

		var tempItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))),
			true);
		return CanGet(tempItem, 0);
	}

	public bool CanGet(ICurrency currency, IGameItem container, decimal amount, bool exact)
	{
		if (!(container?.TrueLocations.FirstOrDefault()?.CanGetAccess(container, Actor) ?? true))
		{
			return false;
		}

		var targetCoins =
			currency.FindCurrency(
				container.GetItemType<IContainer>().Contents.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
				amount);
		if (!targetCoins.Any())
		{
			return false;
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return false;
		}

		var tempItem = container.GetItemType<IContainer>().Contents
		                        .FirstOrDefault(x => x.GetItemType<ICurrencyPile>() != null);

		return CanGet(tempItem, container, 0, ItemCanGetIgnore.None);
	}

	public string WhyCannotGet(ICurrency currency, decimal amount, bool exact)
	{
		var targetCoins =
			currency.FindCurrency(
				Location.LayerGameItems(RoomLayer).SelectNotNull(x => x.GetItemType<ICurrencyPile>())
				        .Where(x => Location.CanGet(x.Parent, Actor)), amount);
		var trueTargetCoins =
			currency.FindCurrency(Location.LayerGameItems(RoomLayer).SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
				amount);
		if (!targetCoins.Any())
		{
			return trueTargetCoins.Any()
				? Location.WhyCannotGet(trueTargetCoins.First().Key.Parent, Actor)
				: "There is no money at all to get.";
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return "You cannot get that exact amount. The closest amount you can get is " +
			       currency.Describe(targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)),
				       CurrencyDescriptionPatternType.Short).Colour(Telnet.Green) + ".";
		}

		var tempItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))),
			true);
		return WhyCannotGet(tempItem, 0);
	}

	public string WhyCannotGet(ICurrency currency, IGameItem container, decimal amount, bool exact)
	{
		if (!(container?.TrueLocations.FirstOrDefault()?.CanGetAccess(container, Actor) ?? true))
		{
			return container.Location.WhyCannotGetAccess(container, Actor);
		}

		var targetCoins =
			currency.FindCurrency(
				container.GetItemType<IContainer>().Contents.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
				amount);
		if (!targetCoins.Any())
		{
			return "There is no money in " + container.HowSeen(this) + " at all to get.";
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return "You cannot get that exact amount from " + container.HowSeen(this) +
			       ". The closest amount you can get is " +
			       currency.Describe(targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)),
				       CurrencyDescriptionPatternType.Short).Colour(Telnet.Green) + ".";
		}

		var tempItem = container.GetItemType<IContainer>().Contents
		                        .FirstOrDefault(x => x.GetItemType<ICurrencyPile>() != null);
		return WhyCannotGet(tempItem, container, 0, ItemCanGetIgnore.None);
	}

	public void Get(ICurrency currency, IGameItem containerItem, decimal amount, bool exact, IEmote playerEmote = null,
		bool silent = false)
	{
		if (!CanGet(currency, containerItem, amount, exact))
		{
			OutputHandler.Send(WhyCannotGet(currency, containerItem, amount, exact));
			return;
		}

		var container = containerItem.GetItemType<IContainer>();

		var targetCoins =
			currency.FindCurrency(
				container.Contents.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
				amount);
		var newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))));
		container.Put(null, newItem, false);
		foreach (var item in targetCoins)
		{
			if (!item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value))))
			{
				containerItem.GetItemType<IContainer>().Take(Actor, item.Key.Parent, 0);
				item.Key.Parent.Delete();
			}
		}

		Get(newItem, containerItem, playerEmote: playerEmote, silent: silent);
	}

	public void Get(ICurrency currency, decimal amount, bool exact, IEmote playerEmote = null, bool silent = false)
	{
		if (!CanGet(currency, amount, exact))
		{
			OutputHandler.Send(WhyCannotGet(currency, amount, exact));
			return;
		}

		var targetCoins =
			currency.FindCurrency(Location.LayerGameItems(RoomLayer).SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
				amount);
		var newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))));
		foreach (var item in targetCoins)
		{
			if (!item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value))))
			{
				item.Key.Parent.Delete();
			}
		}

		Get(newItem, playerEmote: playerEmote, silent: silent);
	}

	public bool CanPut(ICurrency currency, IGameItem container, ICharacter containerOwner, decimal amount, bool exact)
	{
		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		if (!targetCoins.Any())
		{
			return false;
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return false;
		}

		var tempItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))),
			true);
		return CanPut(tempItem, container, containerOwner, 0, false);
	}

	public string WhyCannotPut(ICurrency currency, IGameItem container, ICharacter containerOwner, decimal amount,
		bool exact)
	{
		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()), amount);
		if (!targetCoins.Any())
		{
			return "You have no money to put in " + container.HowSeen(this) + ".";
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return "You cannot put that exact amount in " + container.HowSeen(this) +
			       ". The closest amount you can get is " +
			       currency.Describe(targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)),
				       CurrencyDescriptionPatternType.Short).Colour(Telnet.Green) + ".";
		}

		var tempItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))),
			true);
		return WhyCannotPut(tempItem, container, containerOwner, 0, false);
	}

	public void Put(ICurrency currency, IGameItem container, ICharacter containerOwner, decimal amount, bool exact,
		IEmote playerEmote = null,
		bool silent = false)
	{
		if (!CanPut(currency, container, containerOwner, amount, exact))
		{
			OutputHandler.Send(WhyCannotPut(currency, container, containerOwner, amount, exact));
			return;
		}

		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		var newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))));
		foreach (var item in targetCoins)
		{
			if (!item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value))))
			{
				Take(item.Key.Parent);
				item.Key.Parent.Delete();
			}
		}

		Put(newItem, container, containerOwner, playerEmote: playerEmote, silent: silent);
	}

	public bool CanDrop(ICurrency currency, decimal amount, bool exact)
	{
		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		if (!targetCoins.Any())
		{
			return false;
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return false;
		}

		var tempItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))),
			true);
		return CanDrop(tempItem, 0);
	}

	public string WhyCannotDrop(ICurrency currency, decimal amount, bool exact)
	{
		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		if (!targetCoins.Any())
		{
			return "You have no money to drop.";
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return "You cannot drop that exact amount. The closest amount you can get is " +
			       currency.Describe(targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)),
				       CurrencyDescriptionPatternType.Short).Colour(Telnet.Green) + ".";
		}

		var tempItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))),
			true);
		return WhyCannotDrop(tempItem, 0);
	}

	public void Drop(ICurrency currency, decimal amount, bool exact, bool newStack = false, IEmote playerEmote = null,
		bool silent = false)
	{
		if (!CanDrop(currency, amount, exact))
		{
			OutputHandler.Send(WhyCannotDrop(currency, amount, exact));
			return;
		}

		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		var newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))));
		foreach (var item in targetCoins)
		{
			if (!item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value))))
			{
				Take(item.Key.Parent);
				item.Key.Parent.Delete();
			}
		}

		Drop(newItem, newStack: newStack, playerEmote: playerEmote, silent: silent);
	}

	public bool CanGive(ICurrency currency, IBody target, decimal amount, bool exact)
	{
		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		if (!targetCoins.Any())
		{
			return false;
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return false;
		}

		var tempItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))),
			true);
		return CanGive(tempItem, target);
	}

	public string WhyCannotGive(ICurrency currency, IBody target, decimal amount, bool exact)
	{
		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		if (!targetCoins.Any())
		{
			return "You have no money to give " + target.HowSeen(this) + ".";
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return "You cannot give " + target.HowSeen(this) +
			       " that exact amount. The closest amount you can get is " +
			       currency.Describe(targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)),
				       CurrencyDescriptionPatternType.Short).Colour(Telnet.Green) + ".";
		}

		var tempItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))),
			true);
		return WhyCannotGive(tempItem, target);
	}

	public void Give(ICurrency currency, IBody target, decimal amount, bool exact, IEmote playerEmote = null)
	{
		if (!CanGive(currency, target, amount, exact))
		{
			OutputHandler.Send(WhyCannotGive(currency, target, amount, exact));
			return;
		}

		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		var newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))));
		foreach (var item in targetCoins)
		{
			if (!item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value))))
			{
				Take(item.Key.Parent);
				item.Key.Parent.Delete();
			}
		}

		Give(newItem, target, playerEmote: playerEmote);
	}

	public bool CanGive(ICurrency currency, ICorpse target, decimal amount, bool exact)
	{
		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		if (!targetCoins.Any())
		{
			return false;
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return false;
		}

		var tempItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))),
			true);
		return CanGive(tempItem, target.OriginalCharacter.Body);
	}

	public string WhyCannotGive(ICurrency currency, ICorpse target, decimal amount, bool exact)
	{
		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()), amount);
		if (!targetCoins.Any())
		{
			return "You have no money to give " + target.Parent.HowSeen(this) + ".";
		}

		if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
		{
			return "You cannot give " + target.Parent.HowSeen(this) +
			       " that exact amount. The closest amount you can get is " +
			       currency.Describe(targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)),
				       CurrencyDescriptionPatternType.Short).Colour(Telnet.Green) + ".";
		}

		var tempItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))),
			true);
		return WhyCannotGive(tempItem, target.OriginalCharacter.Body);
	}

	public void Give(ICurrency currency, ICorpse target, decimal amount, bool exact, IEmote playerEmote = null)
	{
		if (!CanGive(currency, target, amount, exact))
		{
			OutputHandler.Send(WhyCannotGive(currency, target, amount, exact));
			return;
		}

		var targetCoins = currency.FindCurrency(HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		var newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			targetCoins.SelectMany(x => x.Value)
			           .Select(x => x.Key)
			           .Distinct()
			           .Select(x =>
				           Tuple.Create(x, targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))));
		foreach (var item in targetCoins)
		{
			if (!item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value))))
			{
				Take(item.Key.Parent);
				item.Key.Parent.Delete();
			}
		}

		Give(newItem, target, playerEmote: playerEmote);
	}

	#endregion

	#region Commodity-Related versions of inventory commands

	public bool CanGetByWeight(IGameItem item, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		throw new NotImplementedException();
	}

	public bool CanGetByWeight(IGameItem item, IGameItem container, double weight,
		ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		throw new NotImplementedException();
	}

	public bool WhyCannotGetByWeight(IGameItem item, double weight,
		ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		throw new NotImplementedException();
	}

	public bool WhyCannotGetByWeight(IGameItem item, IGameItem container, double weight,
		ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		throw new NotImplementedException();
	}

	public void GetByWeight(IGameItem item, double weight, IEmote playerEmote = null, bool silent = false,
		ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		throw new NotImplementedException();
	}

	public void GetByWeight(IGameItem item, IGameItem container, double weight, IEmote playerEmote = null,
		bool silent = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
	{
		throw new NotImplementedException();
	}

	#endregion
}

public static class DrapeableExtensionClass
{
	public static WhyCannotDrapeReason WhyCannotDrape<T>(this IEnumerable<T> wearlocs, IGameItem item, IBody body)
		where T : IWear
	{
		var wearable = item.GetItemType<IWearable>();
		if (wearable == null)
		{
			return WhyCannotDrapeReason.NotIDrapeable;
		}

		if (wearable.Profiles.All(x => !body.Prototype.CountsAs(x.DesignedBody)))
		{
			return WhyCannotDrapeReason.NotBodyType;
		}

		if (wearable.Profiles.Any() && !wearable.Profiles.Any(x => body.CanWear(item, x)))
		{
			return WhyCannotDrapeReason.NoProfilesMatch;
		}

		return wearable.WhyCannotWear(body);
	}

	public static WhyCannotDrapeReason WhyCannotDrape<T>(this IEnumerable<T> wearlocs, IGameItem item,
		IWearProfile profile, IBody body) where T : IWear
	{
		if (body.CanWear(item, profile))
		{
			return WhyCannotDrapeReason.SpecificProfileNoMatch;
		}

		var result = item.GetItemType<IWearable>()?.WhyCannotWear(body, profile) ?? WhyCannotDrapeReason.NotIDrapeable;
		if (result != WhyCannotDrapeReason.Unknown)
		{
			return result;
		}

		if (profile is not null && profile.Profile(body).Any(x =>
			    body.WornItemsFor(x.Key).Sum(y => y.GetItemType<IWearable>().LayerWeightConsumption) >
			    Body.MaximumLayerWeight) == true)
		{
			return WhyCannotDrapeReason.TooManyItems;
		}

		return wearlocs.WhyCannotDrape(item, body);
	}

	public static WhyCannotRemoveReason WhyCannotRemove(
		this ICollection<(IGameItem Item, IWear Wearloc, IWearlocProfile Profile)> wornitems, IGameItem item)
	{
		if (item.IsItemType<IBeltable>() && item.GetItemType<IBeltable>().ConnectedTo != null)
		{
			return WhyCannotRemoveReason.ItemIsAttached;
		}

		return wornitems.Where(x => x.Item == item).Any(x => !wornitems.Last(y => y.Wearloc == x.Wearloc).Equals(x))
			? WhyCannotRemoveReason.ItemIsCovered
			: WhyCannotRemoveReason.Unknown;
	}
}