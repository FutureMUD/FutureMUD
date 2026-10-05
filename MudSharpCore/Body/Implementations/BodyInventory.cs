using MudSharp.Accounts;
using MudSharp.NPC.AI;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.RPG.Law;

#nullable enable annotations

namespace MudSharp.Body.Implementations;

public partial class Body
{
    private readonly HashSet<IProsthetic> _prosthetics = new();
    private bool _inventoryChanged;
    private bool _inventoryLoaded;

    public bool InventoryLoaded => _inventoryLoaded;

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
                // A borrowed body's initialisation can leave an existing dirty flag
                // after its pending save was cancelled. Every later mutation must
                // queue it again; Changed already prevents duplicate queue entries.
                if (value)
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
        foreach (IProsthetic item in _prosthetics)
        {
            body.BodiesProsthetics.Add(new BodiesProsthetics { Body = body, ProstheticId = item.Parent.Id });
        }

        ProstheticsChanged = false;
    }

    private void SaveInventory(MudSharp.Models.Body body)
    {
        int order = 0;
        FMDB.Context.BodiesGameItems.RemoveRange(body.BodiesGameItems);
        foreach (IWearable item in DirectWornItems.SelectNotNull(x => x.GetItemType<IWearable>()))
        {
            // This line is necessary because if somebody removes something, drops it, and someone else gets and wears it quickly, 
            // it could cause an out-of-order save exception relating to duplicate Bodies_GameItems entries
            BodiesGameItems oldItem = FMDB.Context.BodiesGameItems.FirstOrDefault(x => x.GameItemId == item.Parent.Id);
            if (oldItem != null)
            {
                FMDB.Context.BodiesGameItems.Remove(oldItem);
            }

            BodiesGameItems dbitem = new();
            Models.GameItem dbgameitem = FMDB.Context.GameItems.Find(item.Parent.Id);
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

        foreach (IGameItem item in HeldItems)
        {
            // This line is necessary because if somebody removes something, drops it, and someone else gets and wears it quickly, 
            // it could cause an out-of-order save exception relating to duplicate Bodies_GameItems entries
            BodiesGameItems oldItem = FMDB.Context.BodiesGameItems.FirstOrDefault(x => x.GameItemId == item.Id);
            if (oldItem != null)
            {
                FMDB.Context.BodiesGameItems.Remove(oldItem);
            }

            BodiesGameItems dbitem = new()
            {
                BodyId = body.Id
            };
            Models.GameItem dbgameitem = FMDB.Context.GameItems.Find(item.Id);
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

        foreach (IGameItem item in WieldedItems)
        {
            // This line is necessary because if somebody removes something, drops it, and someone else gets and wears it quickly, 
            // it could cause an out-of-order save exception relating to duplicate Bodies_GameItems entries
            BodiesGameItems oldItem = FMDB.Context.BodiesGameItems.FirstOrDefault(x => x.GameItemId == item.Id);
            if (oldItem != null)
            {
                FMDB.Context.BodiesGameItems.Remove(oldItem);
            }

            BodiesGameItems dbitem = new()
            {
                BodyId = body.Id
            };
            Models.GameItem dbgameitem = FMDB.Context.GameItems.Find(item.Id);
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
        if (_inventoryLoaded)
        {
            RecalculateItemHelpers();
            return;
        }

        List<IGameItem> loadedItems = new();
        _noSave = true;

        foreach (BodiesGameItems item in body.BodiesGameItems.OrderBy(x => x.EquippedOrder))
        {
            IGameItem gitem = Gameworld.TryGetItem(item.GameItemId, true);
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
                    $"Duplicated Item: {gitem.HowSeen(gitem, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {gitem.Id:N0}",
                    true);
                continue;
            }

            loadedItems.Add(gitem);
            var originalSaveStates = gitem.Components
                                         .Cast<ISaveable>()
                                         .Append(gitem)
                                         .Distinct()
                                         .Select(x => (Item: x, WasChanged: x.Changed))
                                         .ToList();

            gitem.Get(this);
            if (item.WearProfile.HasValue)
            {
                IWearProfile profile = Gameworld.WearProfiles.Get(item.WearProfile.Value);
                if (!LoadtimeWear(gitem, profile))
                {
                    gitem.Get(null);
                    gitem.RoomLayer = RoomLayer;
                    gitem.InsertAtSource(Actor);
                }
            }
            else if (item.Wielded.HasValue && item.Wielded.Value == 1)
            {
                // TODO - save wield flags
                if (!LoadtimeWield(gitem, ItemCanWieldFlags.None))
                {
                    gitem.Get(null);
                    gitem.RoomLayer = RoomLayer;
                    gitem.InsertAtSource(Actor);
                }
                else
                {
                    foreach (var (saveable, wasChanged) in originalSaveStates)
                    {
                        saveable.Changed = wasChanged;
                        if (!wasChanged)
                        {
                            Gameworld.SaveManager.Abort(saveable);
                        }
                    }
                }
            }
            else
            {
                if (!LoadtimeGet(gitem))
                {
                    gitem.Get(null);
                    gitem.RoomLayer = RoomLayer;
                    gitem.InsertAtSource(Actor);
                }
            }
        }

        foreach (IGameItem item in loadedItems)
        {
            item.FinaliseLoadTimeTasks();
        }

        _noSave = false;
        _inventoryLoaded = true;
        RecalculateItemHelpers();
    }

    #region IWield Implementation

    private readonly List<Tuple<IGameItem, IWield>> _wieldedItems = new();

    public IEnumerable<IGameItem> WieldedItems => _wieldedItems.Select(x => x.Item1).Distinct();

    public IEnumerable<IGameItem> ItemsInHands => WieldedItems.Concat(HeldItems);

    public IEnumerable<IGameItem> WieldedItemsFor(IBodypart proto)
    {
        return _wieldedItems.Where(x => x.Item2 == proto).Select(x => x.Item1);
    }

    public bool CanWield(IGameItem item, ItemCanWieldFlags flags = ItemCanWieldFlags.None)
    {
        IWieldable wieldable = item.GetItemType<IWieldable>();
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

        List<IWield> openWieldLocs = WieldLocs.Where(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse).Select(x => (Wielder: x, CanWield: x.CanWield(item, this))).Where(x =>
            x.CanWield == IWieldItemWieldResult.Success ||
            (flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands) &&
             (x.CanWield == IWieldItemWieldResult.AlreadyWielding ||
              x.CanWield == IWieldItemWieldResult.GrabbingWielderHoldOtherItem))).Select(x => x.Wielder).ToList();

        List<(IWield Item, int Hands)> locsAndHands = openWieldLocs.Select(x => (Item: x, Hands: x.Hands(item))).ToList();
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

    public bool CanWield(IGameItem item, IWield? specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None)
    {
        if (specificHand is not null && (!WieldLocs.Contains(specificHand) || CanUseBodypart(specificHand) != CanUseBodypartResult.CanUse))
        {
            return false;
        }

        IWieldable wieldable = item.GetItemType<IWieldable>();
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

        List<IWield> openWieldLocs = WieldLocs.Where(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse).Where(x => x.CanWield(item, this) == IWieldItemWieldResult.Success ||
                                                 (flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands) &&
                                                  (x.CanWield(item, this) == IWieldItemWieldResult.AlreadyWielding ||
                                                   x.CanWield(item, this) ==
                                                   IWieldItemWieldResult.GrabbingWielderHoldOtherItem))).ToList();
        List<(IWield Item, int Hands)> locsAndHands = openWieldLocs.Select(x => (Item: x, Hands: x.Hands(item))).ToList();
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
        IWieldable wieldable = item.GetItemType<IWieldable>();
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

        List<IWieldItemWieldResult> reasons = WieldLocs.Select(x => x.CanWield(item, this)).ToList();
        List<IWield> openWieldLocs = WieldLocs.Where(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse).Where(x => x.CanWield(item, this) == IWieldItemWieldResult.Success ||
                                                 (flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands) &&
                                                  (x.CanWield(item, this) == IWieldItemWieldResult.AlreadyWielding ||
                                                   x.CanWield(item, this) ==
                                                   IWieldItemWieldResult.GrabbingWielderHoldOtherItem))).ToList();
        List<(IWield Item, int Hands)> locsAndHands = openWieldLocs.Select(x => (Item: x, Hands: x.Hands(item))).ToList();
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

    public string WhyCannotWield(IGameItem item, IWield? specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None)
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

        List<IWield> openWieldLocs = WieldLocs.Where(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse).Where(x => x.CanWield(item, this) == IWieldItemWieldResult.Success ||
                                                 (flags.HasFlag(ItemCanWieldFlags.IgnoreFreeHands) &&
                                                  (x.CanWield(item, this) == IWieldItemWieldResult.AlreadyWielding ||
                                                   x.CanWield(item, this) ==
                                                   IWieldItemWieldResult.GrabbingWielderHoldOtherItem))).ToList();
        List<(IWield Item, int Hands)> locsAndHands = openWieldLocs.Select(x => (Item: x, Hands: x.Hands(item))).ToList();

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

	private sealed record PreparedWield(IWield Primary, IReadOnlyList<IWield> Hands);

	private PreparedWield? PrepareWieldPlacement(IGameItem item, IWield? specificHand, ItemCanWieldFlags flags)
	{
		var executor = Actor;
		var available = WieldLocs.ToArray()
			.Where(x => x.CanWield(item, this) == IWieldItemWieldResult.Success)
			.OrderByDescending(x => _heldItems.Any(y => y.Item1 == item && y.Item2 == x))
			.Select(x => (Hand: x, Count: x.Hands(item))).ToList();
		if (!ReferenceEquals(Actor, executor) || !CommandExecutionScope.TryContinue()) return null;
		if (available.Count == 0) return null;
		var hands = flags.HasFlag(ItemCanWieldFlags.RequireTwoHands) ? 2 :
			specificHand is null ? available.Min(x => x.Count) : specificHand.Hands(item);
		if (!ReferenceEquals(Actor, executor) || !CommandExecutionScope.TryContinue()) return null;
		var primary = specificHand ?? (hands == 1 ?
			available.FirstOrDefault(x => x.Count == 1 && x.Hand.Alignment.LeftRightOnly() == executor.Handedness.LeftRightOnly()).Hand ??
			available.FirstOrDefault(x => x.Count == 1).Hand : available[0].Hand);
		if (primary is null) return null;
		var chosen = new List<IWield> { primary };
		if (hands == 2)
		{
			var secondary = available.FirstOrDefault(x => !ReferenceEquals(x.Hand, primary)).Hand;
			if (secondary is null) return null;
			chosen.Add(secondary);
		}
		return new PreparedWield(primary, chosen);
	}

	private bool CompleteWieldPlacement(IGameItem item, PreparedWield placement, MudSharp.Character.ICharacter executor,
		IEmote? playerEmote, bool silent)
	{
		if (!ReferenceEquals(Actor, executor) || item.Deleted || item.Destroyed || ComponentItemTransfer.DirectLocationOf(item) is not null ||
			item.ContainedIn is not null || !ReferenceEquals(item.GetItemType<IHoldable>()?.HeldBy, this)) return false;
		// A committed draw finishes only its prepared item. Callback relocation is preserved.
		if (placement.Hands.Any(hand => _heldItems.Any(x => x.Item1 != item && ReferenceEquals(x.Item2, hand)) ||
			_wieldedItems.Any(x => x.Item1 != item && ReferenceEquals(x.Item2, hand)))) return false;
		_wieldedItems.RemoveAll(x => x.Item1 == item);
		_wieldedItems.AddRange(placement.Hands.Select(x => Tuple.Create(item, x)));
		_heldItems.RemoveAll(x => x.Item1 == item);
		item.GetItemType<IWieldable>().PrimaryWieldedLocation = placement.Primary;
		UpdateDescriptionWielded(item);
		InventoryChanged = true;
		if (!silent) OutputHandler.Handle(new MixedEmoteOutput(
			new Emote("@ wield|wields $1 in " + WieldSuffix(item, WieldSuffixForm.WieldEcho), executor, executor, item),
			flags: OutputFlags.SuppressObscured).Append(playerEmote));
		OnInventoryChange?.Invoke(InventoryState.Held, InventoryState.Wielded, item);
		item.InvokeInventoryChange(InventoryState.Held, InventoryState.Wielded);
		CheckConsequences();
		item.HandleEvent(EventType.ItemWielded, item, executor);
		foreach (var witness in Location.EventHandlersFor(executor).ToArray())
			witness.HandleEvent(EventType.ItemWieldedWitness, item, executor, witness);
		return true;
	}

    public bool Wield(IGameItem item, IWield? specificHand, IEmote? playerEmote = null, bool silent = false,
        ItemCanWieldFlags flags = ItemCanWieldFlags.None)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		bool CanContinue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
		if (!CanContinue()) return false;
		ForeignCustodyTransferContext.EnsureBody(this, item);
		var canWield = specificHand is null ? CanWield(item, flags) : CanWield(item, specificHand, flags);
		if (!CanContinue()) return false;
		if (!canWield)
		{
			if (!silent) OutputHandler.Send(specificHand is null ? WhyCannotWield(item, flags) : WhyCannotWield(item, specificHand, flags));
			return false;
		}
		var placement = PrepareWieldPlacement(item, specificHand, flags);
		if (placement is null || !CanContinue()) return false;
		CommandExecutionScope.MarkCommitted(executor);
		return CompleteWieldPlacement(item, placement, executor, playerEmote, silent);
    }

    public bool LoadtimeWield(IGameItem item, ItemCanWieldFlags flags)
    {
        if (!CanWield(item, flags))
        {
            return false;
        }

        List<IWield> potentialWearLocs =
            WieldLocs.Where(x => x.CanWield(item, this) == IWieldItemWieldResult.Success)
                     .OrderByDescending(x => _heldItems.Any(y => y.Item1 == item && y.Item2 == x))
                     .ToList();
        int hands = flags.HasFlag(ItemCanWieldFlags.RequireTwoHands) ? 2 : potentialWearLocs.Min(x => x.Hands(item));
        if (hands == 1)
        {
            //Try dominant hand first
            IWield loc = potentialWearLocs.FirstOrDefault(x =>
                          x.Hands(item) == 1 && x.Alignment.LeftRightOnly() == Actor.Handedness.LeftRightOnly()) ??
                      potentialWearLocs.FirstOrDefault(x => x.Hands(item) == 1);
            _wieldedItems.Add(Tuple.Create(item, loc));
            item.GetItemType<IWieldable>().PrimaryWieldedLocation = loc;
        }
        else
        {
            List<IWield> locs = potentialWearLocs.Take(2).ToList();
            item.GetItemType<IWieldable>().PrimaryWieldedLocation = locs.First();
            foreach (IWield loc in locs)
            {
                _wieldedItems.Add(Tuple.Create(item, loc));
            }
        }

        _carriedItems.Add(item);
        UpdateDescriptionWielded(item);
        return true;
    }

    public bool Wield(IGameItem item, IEmote? playerEmote = null, bool silent = false,
        ItemCanWieldFlags flags = ItemCanWieldFlags.None) => Wield(item, (IWield?)null, playerEmote, silent, flags);

    public bool Unwield(IGameItem item, IEmote? playerEmote = null, bool silent = false)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		bool CanContinue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
		if (!CanContinue()) return false;
		var canUnwield = CanUnwield(item);
		if (!CanContinue() || !canUnwield) return false;
        Tuple<IGameItem, IWield> heldLocation = _wieldedItems.FirstOrDefault(x => x.Item1 == item);
        if (heldLocation is null) return false;
		var selfUnwielder = heldLocation.Item2.SelfUnwielder();
		var placement = selfUnwielder ? null : PrepareGetPlacement(item, true);
		if (!CanContinue() || (!selfUnwielder && placement is null)) return false;
		CommandExecutionScope.MarkCommitted(executor);
        _wieldedItems.RemoveAll(x => x.Item1 == item);
        if (selfUnwielder)
        {
            _heldItems.Add(Tuple.Create(heldLocation.Item1, heldLocation.Item2 as IGrab));
        }
        else
        {
            item.Get(this);
            if (!CompleteGetPlacement(item, placement!)) return false;
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
        HandleCharacterItemEvent(EventType.CharacterUnwieldedItem, EventType.ItemUnwielded,
            EventType.CharacterUnwieldedItemWitness, item);
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
        IEnumerable<IWield> locs = _wieldedItems.Where(x => x.Item1 == item).Select(x => x.Item2);
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
            IWieldable wielditem =
                ExternalItems
                .SelectNotNull(x => x.GetItemType<ISheath>())
                .SelectMany(x => x is IMultiSlotSheath multi ? multi.WieldableContents : x.Content is null ? [] : [x.Content])
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

        IBeltable sheathBeltable = item.ContainedIn?.GetItemType<IBeltable>();
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
            IWieldable wielditem = ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>())
                                             .SelectMany(x => x is IMultiSlotSheath multi ? multi.WieldableContents : x.Content is null ? [] : [x.Content])
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

        IBeltable sheathBeltable = item.ContainedIn?.GetItemType<IBeltable>();
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

    public bool Draw(IGameItem item, IWield specificHand, IEmote? playerEmote = null,
        OutputFlags additionalFlags = OutputFlags.Normal, bool silent = false,
        ItemCanWieldFlags flags = ItemCanWieldFlags.None)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		bool CanContinue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
		if (!CanContinue()) return false;
		ForeignCustodyTransferContext.EnsureBody(this, item);
        if (!CanDraw(item, specificHand, flags))
        {
            OutputHandler.Send(WhyCannotDraw(item, specificHand, flags));
            return false;
        }

        if (item == null)
        {
            item =
                ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>())
                             .SelectMany(x => x is IMultiSlotSheath multi ? multi.WieldableContents : x.Content is null ? [] : [x.Content])
                             .FirstOrDefault()
                             ?.Parent;
        }

        if (!CanContinue()) return false;
		var placement = PrepareWieldPlacement(item, specificHand, flags);
		var fallback = PrepareGetPlacement(item, false);
		if (placement is null || fallback is null || !CanContinue()) return false;
        IWieldable wieldItem = item.GetItemType<IWieldable>();
        ISheath sheathItem =
            ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>()).FirstOrDefault(x =>
                x is IMultiSlotSheath multi ? multi.WieldableContents.Contains(wieldItem) : x.Content == wieldItem);

        if (sheathItem is null || !CanContinue()) return false;
		CommandExecutionScope.MarkCommitted(executor);
        if (sheathItem is IMultiSlotSheath concreteSheath)
        {
            concreteSheath.TryRemove(wieldItem);
        }
        else
        {
            sheathItem.Content = null;
        }
        sheathItem.Parent.Changed = true;
        if (item.Deleted || item.Destroyed || item.ContainedIn is not null || item.Location is not null || item.InInventoryOf is not null) return false;
		item.Get(this);
        if (!silent)
        {
            OutputHandler.Handle(
                new MixedEmoteOutput(new Emote("@ draw|draws $0 from $1", Actor, item, sheathItem.Parent),
                    flags: OutputFlags.SuppressObscured | additionalFlags).Append(playerEmote));
        }

        if (!CompleteWieldPlacement(item, placement, executor, null, true))
		{
			CompleteGetPlacement(item, fallback);
			return false;
		}

		InventoryChanged = true;
        OnInventoryChange?.Invoke(InventoryState.Sheathed, InventoryState.Wielded, item);
        item.InvokeInventoryChange(InventoryState.Sheathed, InventoryState.Wielded);
        CheckConsequences();
        return true;
    }

    public bool CanSheathe(IGameItem item, IGameItem sheath)
    {
        return this.CanPerformManualAction(out _) && CanSheatheExternally(item, sheath);
    }

    public bool CanSheatheExternally(IGameItem item, IGameItem sheath)
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
            if (!isheath.CanSheath(item))
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

            IBeltable sheathBeltable = isheath.Parent.GetItemType<IBeltable>();
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
        if (!this.CanPerformManualAction(out var manualReason))
        {
            return manualReason;
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
            if (!isheath.CanSheath(item))
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

            IBeltable sheathBeltable = isheath.Parent.GetItemType<IBeltable>();
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

            List<(ISheath Sheath, (bool Success, string ReasonText, WhyCannotSheatheReason Reason) Result)> sheaths = ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>())
                                       .Select(x => (Sheath: x, Result: SheathIsSuitable(x))).ToList();
            if (sheaths.Any(x => x.Result.Reason == WhyCannotSheatheReason.NotEmpty))
            {
                return "You do not have any empty sheaths that are capable of taking that weapon.";
            }

            return "You do not have any empty sheaths that are capable of taking that weapon.";
        }

        ISheath targetSheathComponent = sheath.GetItemType<ISheath>();
        if (targetSheathComponent == null)
        {
            return $"{sheath.HowSeen(Actor, true)} is not a sheath.";
        }

        (bool Success, string ReasonText, WhyCannotSheatheReason Reason) result = SheathIsSuitable(targetSheathComponent);
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

    public bool Sheathe(IGameItem item, IGameItem sheath, IEmote? playerEmote = null,
        OutputFlags additionalFlags = OutputFlags.Normal, bool silent = false)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		if (!CommandExecutionScope.TryContinue(executor)) return false;
		ForeignCustodyTransferContext.EnsureBody(this, item);
		ForeignCustodyTransferContext.EnsureBody(this, sheath);
        if (!CanSheathe(item, sheath))
        {
            OutputHandler.Send(WhyCannotSheathe(item, sheath));
            return false;
        }

		return ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor) &&
			SheatheInternal(item, sheath, playerEmote, additionalFlags, silent);
    }

    public bool SheatheExternally(IGameItem item, IGameItem sheath)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
        return CommandExecutionScope.TryContinue(executor) && CanSheatheExternally(item, sheath) &&
			ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor) &&
               SheatheInternal(item, sheath, null, OutputFlags.Normal, true);
    }

    private bool SheatheInternal(IGameItem item, IGameItem sheath, IEmote? playerEmote,
        OutputFlags additionalFlags, bool silent)
    {
		var executor = Actor;
		bool CanContinue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
		if (!CanContinue()) return false;
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
			if (targetItemWieldable is null || !CanContinue()) return false;
            item = targetItemWieldable.Parent;
        }
        else
        {
            targetItemWieldable = item.GetItemType<IWieldable>();
        }

        bool wasWielded = WieldedItems.Contains(item);
        ISheath targetSheathComponent = null;
        if (sheath == null)
        {
            targetSheathComponent =
                ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>())
                              .FirstOrDefault(x => x.CanSheath(item));
			if (targetSheathComponent is null || !CanContinue()) return false;
            sheath = targetSheathComponent.Parent;
        }
        else
        {
            targetSheathComponent = sheath.GetItemType<ISheath>();
        }

		if (!CanContinue() || targetItemWieldable is null || targetSheathComponent is null ||
			item.Deleted || item.Destroyed || sheath.Deleted || sheath.Destroyed) return false;
		var returnItem = PrepareDetachedItemReturn(item);
		if (returnItem is null || !CanContinue()) return false;

        if (!silent)
        {
            OutputFlags flags = OutputFlags.SuppressObscured | additionalFlags;
            OutputHandler.Handle(
                new MixedEmoteOutput(new Emote("@ sheathe|sheathes $0 in $1", Actor, item, sheath), flags: flags)
                    .Append
                        (playerEmote));
        }

		if (!CanContinue() || item.Deleted || item.Destroyed ||
			(!ReferenceEquals(item.InInventoryOf, this) && !ComponentItemTransfer.IsDetached(item))) return false;
		CommandExecutionScope.MarkCommitted(executor);
		if (ReferenceEquals(item.InInventoryOf, this)) TakeInternal(item);
		if (!ComponentItemTransfer.IsDetached(item)) return false;
		if (!CanContinue() || sheath.Deleted || sheath.Destroyed) { returnItem(); return false; }
        if (targetSheathComponent is IMultiSlotSheath concreteSheath)
        {
			if (!concreteSheath.TryAdd(targetItemWieldable)) { returnItem(); return false; }
        }
        else
        {
            targetSheathComponent.Content = targetItemWieldable;
        }
		if (!ReferenceEquals(item.ContainedIn, sheath)) { returnItem(); return false; }
        sheath.Changed = true;
        Actor.HandleEvent(EventType.CharacterSheatheItem, Actor, item, sheath);
        item.HandleEvent(EventType.ItemSheathed, Actor, item, sheath);
        sheath.HandleEvent(EventType.ItemSheathItemSheathed, Actor, item, sheath);
        foreach (IHandleEvents witness in Location.EventHandlersFor(Actor).Except(Actor))
        {
            witness.HandleEvent(EventType.CharacterSheatheItemWitness, Actor, item, sheath, witness);
        }

        foreach (IGameItem witness in ExternalItems.Except(new[] { item, sheath }))
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

    public IEnumerable<IGameItem> HeldItems => _heldItems.Select(x => x.Item1).Distinct();

    public IEnumerable<IGameItem> HeldOrWieldedItems => HeldItems.Concat(WieldedItems).ToList();

    public IEnumerable<IGrab> FreeHands =>
        //TODO - Review: Does this need to also add in WieldLocs?
        HoldLocs.Where(x => _heldItems.All(y => y.Item2 != x) && _wieldedItems.All(y => y.Item2 != x))
                        .ToList();

    public IEnumerable<IGrab> FunctioningFreeHands => FreeHands.Where(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse);

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
        IWieldable wield = item.GetItemType<IWieldable>();
        return (IBodypart)_heldItems.FirstOrDefault(x => x.Item1 == item)?.Item2 ?? _wieldedItems
            .Where(x => x.Item1 == item).OrderByDescending(x => wield?.PrimaryWieldedLocation == x.Item2)
            .FirstOrDefault()?.Item2;
    }

    public bool CanGet(IGameItem item, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
        if (item is not null && (item.Location is not null || item.ContainedIn is not null || item.InInventoryOf is not null))
        {
            var reach = Actor.CanReachItem(item, requireInventoryPermission: false);
            if (!reach.Truth)
            {
                return false;
            }
        }

        if (!this.CanPerformManualAction(out _))
        {
            return false;
        }

		if (item?.Location?.RouteDefinition is not null && Actor.GetProximity(item) > Proximity.Immediate)
		{
			return false;
		}

        if (item.GetItemType<IDoor>()?.InstalledExit != null)
        {
            return false;
        }

        if (!Actor.MountedCanRetrieve(item, out _))
        {
            return false;
        }

        if (item.Location?.CanGet(item, Actor) == false)
        {
            return false;
        }

        IGameItem actualItem = quantity == 0 ? item : item.PeekSplit(quantity);
        switch (item.CanGet(quantity, ignoreFlags))
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
        var manipulation = Actor.CanReachItem(container, requireInventoryPermission: false);
        if (!manipulation.Truth)
        {
            return false;
        }

        if (item == null)
        {
            return false;
        }

        IContainer tcontainer = container.GetItemType<IContainer>();
        if (!Actor.MountedCanManipulate(container, out _))
        {
            return false;
        }
        if (container.Location?.CanGetAccess(container, Actor) == false)
        {
            return false;
        }

        if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreInContainer))
        {
            if (tcontainer is null || !tcontainer.Contents.Contains(item) || !tcontainer.CanTake(Actor, item, quantity))
            {
                return false;
            }
        }

        if (!CanGet(item, quantity, tcontainer?.Parent.InInventoryOf == this ? ignoreFlags | ItemCanGetIgnore.IgnoreWeight : ignoreFlags))
        {
            return false;
        }

        return true;
    }

    public string WhyCannotGet(IGameItem item, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
        if (item is not null && (item.Location is not null || item.ContainedIn is not null || item.InInventoryOf is not null))
        {
            var reach = Actor.CanReachItem(item, requireInventoryPermission: false);
            if (!reach.Truth)
            {
                return reach.Message;
            }
        }

        if (!this.CanPerformManualAction(out var manualReason))
        {
            return manualReason;
        }

		if (item?.Location?.RouteDefinition is not null && Actor.GetProximity(item) > Proximity.Immediate)
		{
			return $"You are too far away from {item.HowSeen(Actor)} to pick it up.";
		}

        if (item.GetItemType<IDoor>()?.InstalledExit != null)
        {
            return "You cannot get installed doors directly. You must uninstall them first.";
        }

        if (!Actor.MountedCanRetrieve(item, out string mountMessage))
        {
            return mountMessage;
        }

        if (item.Location?.CanGet(item, Actor) == false)
        {
            return Location.WhyCannotGet(item, Actor);
        }

        IGameItem actualItem = quantity == 0 ? item : item.PeekSplit(quantity);
        switch (item.CanGet(quantity, ignoreFlags))
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
                    IPerceivable user = actualItem.EffectsOfType<IInventoryPlanItemEffect>().First().Owner;
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

        List<(IGrab x, WearlocGrabResult)> failingLocs = HoldLocs.Select(x => (x, x.CanGrab(actualItem, this))).Where(x =>
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
        var manipulation = Actor.CanReachItem(container, requireInventoryPermission: false);
        if (!manipulation.Truth)
        {
            return manipulation.Message;
        }

        if (item == null)
        {
            return "You don't see anything like that in such a container.";
        }

        IContainer tcontainer = container.GetItemType<IContainer>();
        if (tcontainer == null)
        {
            return container.HowSeen(this, true) + " is not a container.";
        }

        if (!Actor.MountedCanManipulate(container, out string mountMessage))
        {
            return mountMessage;
        }

        if (!(container.Location?.CanGetAccess(container, Actor) ?? true))
        {
            return container.Location.WhyCannotGetAccess(container, Actor);
        }

        if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreInContainer))
        {
            if (!tcontainer.Contents.Contains(item))
            {
                return $"You do not see that in {container.HowSeen(this)}.";
            }

            if (!tcontainer.CanTake(Actor, item, quantity))
            {
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
                            break;
                        }

                        return "You do not see that in " + container.HowSeen(this) + ".";
                }
            }
        }

        return WhyCannotGet(item, quantity,
                    tcontainer.Parent.InInventoryOf == this
                        ? ignoreFlags | ItemCanGetIgnore.IgnoreWeight
                        : ignoreFlags);

    }

    private bool LoadtimeGet(IGameItem item)
    {
        if (!CanGet(item, 0, ItemCanGetIgnore.IgnoreWeight))
        {
            return false;
        }

        IGrab grabLoc = HoldLocs.FirstOrDefault(x =>
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

    private IEnumerable<IHandleEvents> FilterWitnessHandlers(IEnumerable<IHandleEvents> explicitWitnessHandlers,
        params IHandleEvents[] excluded)
    {
        return explicitWitnessHandlers is null
            ? Location.EventHandlersFor(Actor).Except(excluded)
            : explicitWitnessHandlers.Except(excluded);
    }

    private IEnumerable<IGameItem> FilterExternalItemWitnesses(IEnumerable<IHandleEvents> explicitWitnessHandlers,
        params IGameItem[] excluded)
    {
        return explicitWitnessHandlers is null
            ? ExternalItems.Except(excluded)
            : explicitWitnessHandlers.OfType<IGameItem>().Except(excluded);
    }

    public void Get(IGameItem item, int quantity = 0, IEmote? playerEmote = null, bool silent = false,
        ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
		GetInternal(item, quantity, playerEmote, silent, ignoreFlags, null, true);
    }

    public IGameItem? Get(IGameItem item, int quantity, IEmote? playerEmote, bool silent,
		ItemCanGetIgnore ignoreFlags, IEnumerable<IHandleEvents> witnessHandlers)
	{
		return GetInternal(item, quantity, playerEmote, silent, ignoreFlags, witnessHandlers, true);
	}

	public IGameItem? GetWithoutMerge(IGameItem item, bool silent = true, bool triggerEvents = true)
	{
		return GetInternal(item, 0, null, silent, ItemCanGetIgnore.None, null, false, triggerEvents);
	}

	private sealed record PreparedGet(IGrab? Hand, IGameItem? Merge, MudSharp.Character.ICharacter Executor, ICell? Fallback, RoomLayer Layer)
	{
		public double? RoutePosition { get; init; }
	}

	// A failed transfer may return only its exact already-detached item, without
	// authorising another public operation from an expired command callback.
	internal Action? PrepareDetachedItemReturn(IGameItem item)
	{
		var executor = Actor;
		if (!CommandExecutionScope.TryContinue()) return null;
		var hand = _heldItems.FirstOrDefault(x => ReferenceEquals(x.Item1, item))?.Item2 ??
			_wieldedItems.FirstOrDefault(x => ReferenceEquals(x.Item1, item))?.Item2 as IGrab;
		var placement = new PreparedGet(hand, null, executor, Location, RoomLayer);
		return () =>
		{
			if (!ComponentItemTransfer.IsDetached(item)) return;
			item.Get(this);
			if (CompleteGetPlacement(item, placement)) NotifyGotItem(item, executor);
		};
	}

	// Prepare while the component still owns this item. The returned completion handles this
	// exact detached item only; public reentrant body operations retain their normal gates.
	internal Action? PrepareComponentUnload(IGameItem item)
	{
		var executor = Actor;
		if (!CommandExecutionScope.TryContinue()) return null;
		var canReceive = CanGet(item, 0);
		if (!ReferenceEquals(Actor, executor) || !CommandExecutionScope.TryContinue()) return null;
		var placement = canReceive ? PrepareGetPlacement(item, true) : null;
		if (!ReferenceEquals(Actor, executor) || !CommandExecutionScope.TryContinue()) return null;
		placement ??= new PreparedGet(null, null, executor, Location, RoomLayer);
		return () =>
		{
			if (!ComponentItemTransfer.IsDetached(item)) return;
			item.Get(this);
			if (CompleteGetPlacement(item, placement)) NotifyGotItem(item, executor);
		};
	}

	private PreparedGet? PrepareGetPlacement(IGameItem item, bool allowMerge)
	{
		var executor = Actor;
		var merge = allowMerge ? HeldOrWieldedItems.ToArray().FirstOrDefault(x => x.CanMerge(item)) : null;
		if (!ReferenceEquals(Actor, executor) || !CommandExecutionScope.TryContinue()) return null;
		if (merge is not null) return new PreparedGet(null, merge, executor, Location, RoomLayer) { RoutePosition = RoutePositionMetres };
		var hands = HoldLocs.Where(hand => !_heldItems.Any(x => ReferenceEquals(x.Item2, hand)) &&
			!_wieldedItems.Any(x => ReferenceEquals(x.Item2, hand))).ToArray();
		var hand = hands.FirstOrDefault(x =>
			(x.CanGrab(item, this) == WearlocGrabResult.Success &&
			 x.Alignment.LeftRightOnly() == executor.Handedness.LeftRightOnly()) ||
			(executor.IsAdministrator() && x.CanGrab(item, this) == WearlocGrabResult.FailTooBig)) ??
			hands.FirstOrDefault(x => x.CanGrab(item, this) == WearlocGrabResult.Success ||
				(executor.IsAdministrator() && x.CanGrab(item, this) == WearlocGrabResult.FailTooBig));
		if (!ReferenceEquals(Actor, executor) || !CommandExecutionScope.TryContinue()) return null;
		return merge is null && hand is null ? null : new PreparedGet(hand, merge, executor, Location, RoomLayer) { RoutePosition = RoutePositionMetres };
	}

	// This private completion is only for the one item whose custody operation has already committed.
	// Reentrant public inventory operations still pass their own normal authority checks.
	private bool CompleteGetPlacement(IGameItem item, PreparedGet placement) => CompleteGetPlacementWithResult(item, placement, out _);

	private bool CompleteGetPlacementWithResult(IGameItem item, PreparedGet placement, out IGameItem acquired, bool consumeNativeStack = false)
	{
		acquired = item;
		bool SourceReady() => !item.Deleted && !item.Destroyed && ComponentItemTransfer.DirectLocationOf(item) is null &&
			item.ContainedIn is null && ReferenceEquals(item.GetItemType<IHoldable>()?.HeldBy, this);
		if (!SourceReady()) return false;
		if (HeldOrWieldedItems.Any(x => ReferenceEquals(x, item))) return true;
		if (ReferenceEquals(Actor, placement.Executor))
		{
			if (placement.Merge is { Deleted: false, Destroyed: false } merge && HeldOrWieldedItems.Any(x => ReferenceEquals(x, merge)))
			{
				// Get/removal callbacks may change the prepared survivor's title or custody.
				// CanMerge itself invokes effects/components, so check its inputs and custody again afterwards.
				var sourceOwner = item.OwnershipReference;
				var mergeOwner = merge.OwnershipReference;
				var sourceDescription = (item.Prototype, item.OverrideSdesc, item.OverrideDesc);
				var mergeDescription = (merge.Prototype, merge.OverrideSdesc, merge.OverrideDesc);
				var canMerge = merge.CanMerge(item);
				if (!SourceReady()) return false;
				if (HeldOrWieldedItems.Any(x => ReferenceEquals(x, item))) return true;
				if (canMerge && ReferenceEquals(Actor, placement.Executor) && !merge.Deleted && !merge.Destroyed &&
					HeldOrWieldedItems.Any(x => ReferenceEquals(x, merge)) &&
					ReferenceEquals(merge.GetItemType<IHoldable>()?.HeldBy, this) && merge.ContainedIn is null &&
					ComponentItemTransfer.DirectLocationOf(merge) is null &&
					item.OwnershipReference == sourceOwner && merge.OwnershipReference == mergeOwner && sourceOwner == mergeOwner &&
					(item.Prototype, item.OverrideSdesc, item.OverrideDesc) == sourceDescription &&
					(merge.Prototype, merge.OverrideSdesc, merge.OverrideDesc) == mergeDescription)
				{
					if (consumeNativeStack && merge is MudSharp.GameItems.GameItem nativeMerge && item is MudSharp.GameItems.GameItem nativeSource &&
						nativeMerge.GetItemType<StackableGameItemComponent>() is not null &&
						nativeSource.GetItemType<StackableGameItemComponent>() is not null)
					{
						acquired = merge;
						try { nativeMerge.MergeCommittedStackForGet(nativeSource, this); }
						finally
						{
							// Observers may refill or retitle the zero source. Preserve their value at the
							// captured floor if they did not establish a real inventory/spatial claim.
							if (SourceReady() && !HeldOrWieldedItems.Any(x => ReferenceEquals(x, item)))
							{
								if (placement.Fallback is { } floor) nativeSource.TryDropPrepared(new SpatialLocation(floor, placement.Layer, placement.RoutePosition));
								else item.Drop(null);
								if (!item.Deleted && !item.Destroyed && ReferenceEquals(item.Location, placement.Fallback) &&
									item.InInventoryOf is null && item.ContainedIn is null) placement.Fallback?.Insert(item);
							}
						}
					}
					else merge.Merge(item);
					return true;
				}
			}
			if (ReferenceEquals(Actor, placement.Executor) && placement.Hand is not null && !_heldItems.Any(x => ReferenceEquals(x.Item2, placement.Hand)) &&
				!_wieldedItems.Any(x => ReferenceEquals(x.Item2, placement.Hand)))
			{
				_heldItems.Add(Tuple.Create(item, placement.Hand));
				UpdateDescriptionHeld(item);
				InventoryChanged = true;
				return true;
			}
		}
		// A callback may invalidate the prepared destination. Preserve actual relocation/deletion,
		// otherwise leave this item safely at the captured cell rather than inventing another hand.
		item.RoomLayer = placement.Layer;
		item.Drop(placement.Fallback);
		if (!item.Deleted && !item.Destroyed && ReferenceEquals(item.Location, placement.Fallback) &&
			item.InInventoryOf is null && item.ContainedIn is null) placement.Fallback?.Insert(item);
		return false;
	}

	private IGameItem? GetInternal(IGameItem item, int quantity, IEmote? playerEmote, bool silent,
		ItemCanGetIgnore ignoreFlags, IEnumerable<IHandleEvents> witnessHandlers, bool allowMerge,
		bool triggerEvents = true)
	{
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		bool CanContinue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
		if (!CanContinue()) return null;
		ForeignCustodyTransferContext.EnsureBody(this, item);
		var canGet = CanGet(item, quantity, ignoreFlags);
		if (!CanContinue()) return null;
		if (!canGet)
		{
			if (!silent) OutputHandler.Send(WhyCannotGet(item, quantity, ignoreFlags));
			return null;
		}
		var whole = quantity == 0 || item.DropsWhole(quantity);
		var placement = PrepareGetPlacement(whole ? item : item.PeekSplit(quantity), allowMerge);
		if (placement is null || !CanContinue()) return null;
		CommandExecutionScope.MarkCommitted(executor);
		var gottenItem = whole ? item.Get(this) : item.Get(this, quantity);
		if (!CompleteGetPlacementWithResult(gottenItem, placement, out gottenItem, consumeNativeStack: true)) return null;
		if (gottenItem.Deleted || gottenItem.Destroyed) return null;
		var output = new MixedEmoteOutput(new Emote("@ get|gets $0", this, gottenItem), flags: OutputFlags.SuppressObscured);
        InventoryChanged = true;
        if (!silent)
        {
            output.Append(playerEmote);
            OutputHandler.Handle(output);
        }

        if (triggerEvents) NotifyGotItem(gottenItem, executor, witnessHandlers);

        return gottenItem;
    }

	private void NotifyGotItem(IGameItem item, MudSharp.Character.ICharacter executor, IEnumerable<IHandleEvents>? witnessHandlers = null)
	{
		OnInventoryChange?.Invoke(InventoryState.Dropped, InventoryState.Held, item);
		item.InvokeInventoryChange(InventoryState.Dropped, InventoryState.Held);
		HandleEvent(EventType.CharacterGotItem, executor, item);
		item.HandleEvent(EventType.ItemGotten, executor, item);
		foreach (var witness in FilterWitnessHandlers(witnessHandlers, executor).ToArray())
			witness.HandleEvent(EventType.CharacterGotItemWitness, executor, item, witness);
		foreach (var witness in FilterExternalItemWitnesses(witnessHandlers, item).ToArray())
			witness.HandleEvent(EventType.CharacterGotItemWitness, executor, item, witness);
		CheckConsequences();
	}

	// Preserve native reach facts across the final executable authority callback.
	private static Func<bool>? PrepareItemReachSnapshot(IGameItem item)
	{
		var seen = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		var edges = new List<(IGameItem Item, IGameItem? Parent)>();
		for (var current = item; current is not null; current = current.ContainedIn)
		{
			if (!seen.Add(current)) return null;
			edges.Add((current, current.ContainedIn));
		}
		var nodes = new List<(IGameItem Item, IGameItem? Parent, IBody? Body, MudSharp.Character.ICharacter? Actor,
			ICell? Cell, RoomLayer Layer, double? Position, IOpenable? Openable, bool? Open)>();
		foreach (var (current, parent) in edges)
		{
			var body = current.InInventoryOf;
			var openable = current.GetItemType<IOpenable>();
			nodes.Add((current, parent, body, body?.Actor, current.Location, current.RoomLayer,
				current.RoutePositionMetres, openable, openable?.IsOpen));
		}
		return () => edges.All(x => ReferenceEquals(x.Item.ContainedIn, x.Parent)) &&
			nodes.All(x => !x.Item.Deleted && !x.Item.Destroyed && ReferenceEquals(x.Item.InInventoryOf, x.Body) &&
			ReferenceEquals(x.Body?.Actor, x.Actor) && ReferenceEquals(x.Item.Location, x.Cell) &&
			x.Item.RoomLayer == x.Layer && x.Item.RoutePositionMetres == x.Position &&
			ReferenceEquals(x.Item.GetItemType<IOpenable>(), x.Openable) && x.Openable?.IsOpen == x.Open);
	}

	private IGameItem? GivePhysicalItem(IGameItem item, IBody receiver, int quantity, MudSharp.Character.ICharacter executor,
		int expectedQuantity, IHoldable? expectedHolder, bool whole, MudSharp.Character.ICharacter receiverExecutor, Func<bool> canGive, Func<bool> receiverUnchanged)
	{
		bool OriginalSource() => !item.Deleted && !item.Destroyed && item.Quantity == expectedQuantity &&
			ReferenceEquals(item.GetItemType<IHoldable>(), expectedHolder) && ReferenceEquals(expectedHolder?.HeldBy, this) &&
			(_heldItems.Any(x => ReferenceEquals(x.Item1, item)) || _wieldedItems.Any(x => ReferenceEquals(x.Item1, item))) &&
			item.ContainedIn is null && ComponentItemTransfer.DirectLocationOf(item) is null;
		if (!OriginalSource()) return null;
		if (receiver is not Body destination) return null;
		var placement = destination.PrepareGetPlacement(whole ? item : item.PeekSplit(quantity), true);
		if (placement is null || !ReferenceEquals(receiver.Actor, receiverExecutor) || !canGive() ||
			!CommandExecutionScope.TryContinue(executor) || !ReferenceEquals(Actor, executor) ||
			!ReferenceEquals(receiver.Actor, receiverExecutor) || !receiverUnchanged() || !OriginalSource()) return null;
		CommandExecutionScope.MarkCommitted(executor);
		IGameItem transferred;
		if (whole)
		{
			TakeInternal(item);
			if (item.Deleted || item.Destroyed || item.ContainedIn is not null || item.InInventoryOf is not null || item.Location is not null) return null;
			transferred = item.Get(destination);
		}
		else transferred = item.Get(destination, quantity);
		if (!destination.CompleteGetPlacementWithResult(transferred, placement, out transferred, consumeNativeStack: true) ||
			transferred.Deleted || transferred.Destroyed) return null;
		destination.NotifyGotItem(transferred, placement.Executor);
		return transferred;
	}

    public void Get(IGameItem item, IGameItem containerItem, int quantity = 0, IEmote? playerEmote = null,
        bool silent = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
        Get(item, containerItem, quantity, playerEmote, silent, ignoreFlags, null);
    }

    public IGameItem? Get(IGameItem item, IGameItem containerItem, int quantity, IEmote? playerEmote, bool silent,
        ItemCanGetIgnore ignoreFlags, IEnumerable<IHandleEvents> witnessHandlers)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		bool CanContinue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
		if (!CanContinue()) return null;
		ForeignCustodyTransferContext.EnsureBody(this, item);
		ForeignCustodyTransferContext.EnsurePair(containerItem, item);
		var sourceQuantity = item.Quantity;
		var sourceHolder = item.GetItemType<IHoldable>();
		var containerComp = containerItem.GetItemType<IContainer>();
		var whole = quantity == 0 || item.DropsWhole(quantity);
		var reachUnchanged = PrepareItemReachSnapshot(containerItem);
		if (reachUnchanged is null) return null;
		bool OriginalSource() => ReferenceEquals(Actor, executor) && reachUnchanged() &&
			item.Quantity == sourceQuantity && ReferenceEquals(item.GetItemType<IHoldable>(), sourceHolder) &&
			!containerItem.Deleted && !containerItem.Destroyed && ReferenceEquals(containerItem.GetItemType<IContainer>(), containerComp) &&
			containerComp?.Contents.Any(x => ReferenceEquals(x, item)) == true && ComponentUnloadCompletion.OwnedBy(item, containerItem);
        if (!CanGet(item, containerItem, quantity, ignoreFlags))
        {
            if (!silent)
            {
                OutputHandler.Send(WhyCannotGet(item, containerItem, quantity, ignoreFlags));
            }

            return null;
        }

        if (!CanContinue()) return null;
		var placement = PrepareGetPlacement(whole ? item : item.PeekSplit(quantity), true);
		if (placement is null || !CanGet(item, containerItem, quantity, ignoreFlags) || !CanContinue() || !OriginalSource()) return null;

        CommandExecutionScope.MarkCommitted(executor);
		var takenItem = containerComp.Take(executor, item, quantity);
		if (takenItem is null || takenItem.Deleted || takenItem.Destroyed || takenItem.ContainedIn is not null ||
			takenItem.InInventoryOf is not null || takenItem.Location is not null) return null;
		takenItem.Get(this);
		if (!CompleteGetPlacementWithResult(takenItem, placement, out takenItem, consumeNativeStack: true) ||
			takenItem.Deleted || takenItem.Destroyed) return null;

        MixedEmoteOutput output =
            new(new Emote("@ get|gets $0 from $1", this, takenItem, containerItem),
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
        foreach (IHandleEvents witness in FilterWitnessHandlers(witnessHandlers, Actor))
        {
            witness.HandleEvent(EventType.CharacterGotItemContainerWitness, Actor, takenItem, containerItem, witness);
        }

        foreach (IGameItem witness in FilterExternalItemWitnesses(witnessHandlers, takenItem, containerItem))
        {
            witness.HandleEvent(EventType.CharacterGotItemContainerWitness, Actor, takenItem, containerItem, witness);
        }

        CheckConsequences();
        return takenItem;
    }

    public void UpdateDescriptionHeld(IGameItem item)
    {
        IEnumerable<IGrab> locs = _heldItems.Where(x => x.Item1 == item).Select(x => x.Item2);
        item.GetItemType<IHoldable>().CurrentInventoryDescription =
            $"{"<held in " + DescribeBodypartGroup(locs) + ">",-35}";
    }

    public bool CanPut(IGameItem item, IGameItem container, ICharacter? containerOwner, int quantity,
        bool allowLesserAmounts)
    {
        var manipulation = Actor.CanReachItem(container, requireInventoryPermission: false);
        if (!manipulation.Truth)
        {
            return false;
        }

        if (!this.CanPerformManualAction(out _))
        {
            return false;
        }

		if (container?.Location?.RouteDefinition is not null && Actor.GetProximity(container) > Proximity.Immediate)
		{
			return false;
		}

        if (Actor.RidingMount != null)
        {
            return false;
        }

        IContainer tcontainer = container.GetItemType<IContainer>();
        return
            (container.Location?.CanGetAccess(container, Actor) ?? true) &&
            tcontainer != null &&
            (tcontainer.CanPut(item.PeekSplit(quantity)) ||
             (allowLesserAmounts && tcontainer.WhyCannotPut(item.PeekSplit(quantity)) ==
                 WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity)) && CanDrop(item, quantity) &&
            container.GetItemType<IWearable>()?.CurrentProfile?.RequireContainerIsEmpty != true &&
            !item.PreventsMovement();
    }

    public string WhyCannotPut(IGameItem item, IGameItem container, ICharacter? containerOwner, int quantity,
        bool allowLesserAmounts)
    {
        var manipulation = Actor.CanReachItem(container, requireInventoryPermission: false);
        if (!manipulation.Truth)
        {
            return manipulation.Message;
        }

        if (!this.CanPerformManualAction(out var manualReason))
        {
            return manualReason;
        }

		if (container?.Location?.RouteDefinition is not null && Actor.GetProximity(container) > Proximity.Immediate)
		{
			return $"You are too far away from {container.HowSeen(Actor)} to put anything into it.";
		}

        if (Actor.RidingMount != null)
        {
            return new QuickEmote("@ cannot put items into containers while riding $0.", Actor, Actor.RidingMount);
        }

        IContainer tcontainer = container.GetItemType<IContainer>();
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

        IGameItem dummy = quantity == 0 ? item : item.PeekSplit(quantity);
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

    public void Put(IGameItem item, IGameItem container, ICharacter? containerOwner, int quantity = 0,
        IEmote? playerEmote = null,
        bool silent = false, bool allowLesserAmounts = true)
    {
        Put(item, container, containerOwner, quantity, playerEmote, silent, allowLesserAmounts, null);
    }

    public IGameItem? Put(IGameItem item, IGameItem container, ICharacter? containerOwner, int quantity,
        IEmote? playerEmote, bool silent, bool allowLesserAmounts, IEnumerable<IHandleEvents> witnessHandlers)
    {
        var executor = Actor;
        using var execution = CommandExecutionScope.EnterBodyOperation(executor);
        bool Continue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
        if (!Continue()) return null;
        if (container.IsItemType<ICorpse>()) { Put(item, container, "", playerEmote, silent); return null; }
        var allowed = CanPut(item, container, containerOwner, quantity, allowLesserAmounts);
        if (!Continue()) return null;
        if (!allowed)
        {
            if (!silent) OutputHandler.Send(WhyCannotPut(item, container, containerOwner, quantity, allowLesserAmounts));
            return null;
        }
        var component = container.GetItemType<IContainer>();
        if (component is null || container.Deleted || container.Destroyed) return null;
		var targetLocation = container.Location;
		var targetLayer = container.RoomLayer;
		var targetBody = container.InInventoryOf;
		var targetContainer = container.ContainedIn;
		bool SameDestination() => !container.Deleted && !container.Destroyed &&
			ReferenceEquals(container.GetItemType<IContainer>(), component) && ReferenceEquals(container.Location, targetLocation) &&
			container.RoomLayer == targetLayer && ReferenceEquals(container.InInventoryOf, targetBody) &&
			ReferenceEquals(container.ContainedIn, targetContainer);
        if (allowLesserAmounts)
        {
            var entire = CanPut(item, container, containerOwner, quantity, false);
            if (!Continue()) return null;
            if (!entire)
            {
                var reason = component.WhyCannotPut(item);
                if (!Continue()) return null;
                if (reason == WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity)
                {
                    quantity = component.CanPutAmount(item);
                    if (!Continue() || quantity <= 0) return null;
                }
            }
        }
        var whole = quantity == 0 || item.DropsWhole(quantity);
        var returnHand = _heldItems.FirstOrDefault(x => ReferenceEquals(x.Item1, item))?.Item2 ??
            _wieldedItems.FirstOrDefault(x => ReferenceEquals(x.Item1, item))?.Item2 as IGrab;
        var placement = new PreparedGet(returnHand, null, executor, Location, RoomLayer);
        if (!Continue()) return null;
        if (!silent)
        {
            var emote = containerOwner is null
                ? new Emote("@ put|puts $0 in $1", this, item, container)
                : new Emote("@ put|puts $0 in $2's !1", this, item, container, containerOwner);
            OutputHandler.Handle(new MixedEmoteOutput(emote, flags: OutputFlags.SuppressObscured).Append(playerEmote));
        }
        if (!Continue() || !SameDestination() || !ReferenceEquals(item.InInventoryOf, this) ||
            !HeldOrWieldedItems.Any(x => ReferenceEquals(x, item))) return null;
        CommandExecutionScope.MarkCommitted(executor);
        IGameItem putItem;
        if (whole) { TakeInternal(item); putItem = item; }
        else putItem = item.Drop(null, quantity);
        if (!ComponentItemTransfer.IsDetached(putItem)) return null;
        void Restore()
        {
            if (!ComponentItemTransfer.IsDetached(putItem)) return;
            putItem.Get(this);
            if (CompleteGetPlacement(putItem, placement)) NotifyGotItem(putItem, executor);
        }
        if (!Continue() || !SameDestination()) { Restore(); return null; }
        var canAdopt = component.CanPut(putItem);
        if (!Continue() || !SameDestination() || !canAdopt || !ComponentItemTransfer.IsDetached(putItem)) { Restore(); return null; }
        component.Put(executor, putItem);
        if (ComponentItemTransfer.IsDetached(putItem)) { Restore(); return null; }
        if (putItem.Deleted || putItem.Destroyed) return putItem;
        if (!ReferenceEquals(putItem.ContainedIn, container) || !component.Contents.Any(x => ReferenceEquals(x, putItem))) return null;
        InventoryChanged = true;
        OnInventoryChange?.Invoke(InventoryState.Held, InventoryState.InContainer, putItem);
        putItem.InvokeInventoryChange(InventoryState.Held, InventoryState.InContainer);
        HandleEvent(EventType.CharacterPutItemContainer, executor, putItem, container);
        putItem.HandleEvent(EventType.ItemPutContainer, executor, putItem, container);
        foreach (var witness in FilterWitnessHandlers(witnessHandlers, executor).ToArray())
            witness.HandleEvent(EventType.CharacterPutItemContainerWitness, executor, putItem, container, witness);
        foreach (var witness in FilterExternalItemWitnesses(witnessHandlers, putItem, container).ToArray())
            witness.HandleEvent(EventType.CharacterPutItemContainerWitness, executor, putItem, container, witness);
        if (Continue()) CheckConsequences();
        return putItem;
    }

    #region Corpse-Related Versions of Put

    public bool CanPut(IGameItem item, IGameItem container, string profile)
    {
        var manipulation = Actor.CanReachItem(container, requireInventoryPermission: false);
        if (!manipulation.Truth)
        {
            return false;
        }

        if (!this.CanPerformManualAction(out _))
        {
            return false;
        }

        ICorpse containerAsCorpse = container.GetItemType<ICorpse>();
        if (containerAsCorpse == null)
        {
            return false;
        }

        if (!(container.Location?.CanGetAccess(container, Actor) ?? true))
        {
            return false;
        }

        IWearable itemAsWearable = item.GetItemType<IWearable>();
        if (itemAsWearable == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(profile))
        {
            IWearProfile wprof =
                itemAsWearable.Profiles.FirstOrDefault(
                    x => x.Name.StartsWith(profile, StringComparison.InvariantCultureIgnoreCase));
            if (wprof == null)
            {
                return false;
            }

            return CanDrop(item, 1) && containerAsCorpse.Body.CanWear(item, wprof);
        }

        return CanDrop(item, 1) && containerAsCorpse.Body.CanWear(item);
    }

    public string WhyCannotPut(IGameItem item, IGameItem container, string profile)
    {
        var manipulation = Actor.CanReachItem(container, requireInventoryPermission: false);
        if (!manipulation.Truth)
        {
            return manipulation.Message;
        }

        if (!this.CanPerformManualAction(out var manualReason))
        {
            return manualReason;
        }

        ICorpse containerAsCorpse = container.GetItemType<ICorpse>();
        if (containerAsCorpse == null)
        {
            return $"{container.HowSeen(Actor, true)} is not a corpse, and so cannot be dressed up.";
        }

        if (!(container.Location?.CanGetAccess(container, Actor) ?? true))
        {
            return container.Location.WhyCannotGetAccess(container, Actor);
        }

        IWearable itemAsWearable = item.GetItemType<IWearable>();
        if (itemAsWearable == null)
        {
            return $"{item.HowSeen(Actor, true)} is not something that can be worn.";
        }

        IBody corpseBody = containerAsCorpse.Body;

        if (!CanDrop(item, 1))
        {
            switch (corpseBody.HoldLocs.WhyCannotDrop(item))
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
                    ? containerAsCorpse.Body.WearLocs.WhyCannotDrape(item,
                        containerAsCorpse.Body)
                    : containerAsCorpse.Body.WearLocs.WhyCannotDrape(item, wprof,
                        containerAsCorpse.Body))
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

    public void Put(IGameItem item, IGameItem container, string profile, IEmote? playerEmote = null,
        bool silent = false)
    {
        var executor = Actor;
        using var execution = CommandExecutionScope.EnterBodyOperation(executor);
        bool Continue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
        if (!Continue()) return;
        var allowed = CanPut(item, container, profile);
        if (!Continue()) return;
        if (!allowed) { if (!silent) OutputHandler.Send(WhyCannotPut(item, container, profile)); return; }
        var targetBody = container.GetItemType<ICorpse>()?.Body;
        if (targetBody is null) return;
        var targetActor = targetBody.Actor;
        var wearable = item.GetItemType<IWearable>();
        var wearProfile = string.IsNullOrEmpty(profile) ? null : wearable?.Profiles.FirstOrDefault(x => x.Name.StartsWith(profile, StringComparison.InvariantCultureIgnoreCase));
        if (!Continue() || (profile.Length > 0 && wearProfile is null)) return;
        var restore = PrepareDetachedItemReturn(item);
        if (restore is null || !Continue()) return;
        if (!silent)
            OutputHandler.Handle(new MixedEmoteOutput(new Emote("@ put|puts $0 on $1", this, item, container),
                flags: OutputFlags.SuppressObscured).Append(playerEmote));
        if (!Continue() || !ReferenceEquals(item.InInventoryOf, this) ||
            !HeldOrWieldedItems.Any(x => ReferenceEquals(x, item))) return;
        CommandExecutionScope.MarkCommitted(executor);
        TakeInternal(item);
        if (!ComponentItemTransfer.IsDetached(item)) return;
        if (!Continue() || container.Deleted || container.Destroyed ||
            !ReferenceEquals(container.GetItemType<ICorpse>()?.Body, targetBody) || !ReferenceEquals(targetBody.Actor, targetActor))
        { restore(); return; }
        CommandExecutionScope.InvokeOwned(targetActor, () =>
        {
            if (wearProfile is null) targetBody.WearExternally(item);
            else targetBody.WearExternally(item, wearProfile);
			return 0;
        });
        if (ComponentItemTransfer.IsDetached(item)) restore();
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

    public void Drop(IGameItem item, int quantity = 0, bool newStack = false, IEmote? playerEmote = null,
        bool silent = false)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		bool CanContinue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
		if (!CanContinue()) return;
		var destination = Location;
		var layer = RoomLayer;
		ForeignCustodyTransferContext.EnsureBody(this, item);
        if (!CanDrop(item, quantity) && !silent)
        {
            OutputHandler.Send(WhyCannotDrop(item, quantity));
            return;
        }

        if (!CanContinue()) return;
        MixedEmoteOutput output;
        IGameItem droppedItem = null;
        bool wasWielded = _heldItems.All(x => x.Item1 != item);
        var whole = quantity == 0 || item.DropsWhole(quantity);
		if (!CanContinue()) return;
		CommandExecutionScope.MarkCommitted(executor);
        if (whole)
        {
            droppedItem = item;
            droppedItem.RoomLayer = layer;
            item.Drop(destination);
            _heldItems.RemoveAll(x => x.Item1 == item);
            _wieldedItems.RemoveAll(x => x.Item1 == item);
            if (droppedItem.Deleted || droppedItem.Destroyed || droppedItem.ContainedIn is not null ||
				droppedItem.InInventoryOf is not null || !ReferenceEquals(droppedItem.Location, destination)) return;
            droppedItem.InsertAtSource(executor, newStack);
            output = new MixedEmoteOutput(new Emote("@ drop|drops $0", this, item),
                flags: OutputFlags.SuppressObscured);
        }
        else
        {
            IGameItem newItem = item.Drop(destination, quantity);
            droppedItem = newItem;
            droppedItem.RoomLayer = layer;
            if (newItem.Deleted || newItem.Destroyed || newItem.ContainedIn is not null ||
				newItem.InInventoryOf is not null || !ReferenceEquals(newItem.Location, destination)) return;
            newItem.InsertAtSource(executor, newStack);
            output = new MixedEmoteOutput(new Emote("@ drop|drops $0", this, newItem),
                flags: OutputFlags.SuppressObscured);
        }

        if (!silent)
        {
            output.Append(playerEmote);
            OutputHandler.Handle(output);
        }

		var retainingCarrier = WornItems
			.Concat(HeldOrWieldedItems)
			.SelectNotNull(x => x.GetItemType<IWeaponCarrierAttachment>())
			.FirstOrDefault(x => x.AttachedWeapon == droppedItem && x.TryRetain(droppedItem, Actor));
		if (retainingCarrier is not null)
		{
			InventoryChanged = true;
			OnInventoryChange?.Invoke(wasWielded ? InventoryState.Wielded : InventoryState.Held,
				InventoryState.Hanging, droppedItem);
			droppedItem.InvokeInventoryChange(wasWielded ? InventoryState.Wielded : InventoryState.Held,
				InventoryState.Hanging);
			if (!silent)
			{
				OutputHandler.Handle(new EmoteOutput(new Emote("$0 is caught by $1.", Actor, droppedItem, retainingCarrier.Parent)));
			}
			CheckConsequences();
			return;
		}

        InventoryChanged = true;
        OnInventoryChange?.Invoke(wasWielded ? InventoryState.Wielded : InventoryState.Held, InventoryState.Dropped,
            droppedItem);
        droppedItem.InvokeInventoryChange(wasWielded ? InventoryState.Wielded : InventoryState.Held,
            InventoryState.Dropped);
        // Handle Events
        HandleEvent(EventType.CharacterDroppedItem, Actor, droppedItem);
        droppedItem.HandleEvent(EventType.ItemDropped, Actor, droppedItem);
        foreach (IHandleEvents witness in Location.EventHandlersFor(Actor).Except(new IHandleEvents[] { Actor, droppedItem }))
        {
            witness.HandleEvent(EventType.CharacterDroppedItemWitness, Actor, droppedItem, witness);
        }

        foreach (IGameItem witness in ExternalItems)
        {
            witness.HandleEvent(EventType.CharacterDroppedItemWitness, Actor, droppedItem, witness);
        }

        CheckConsequences();
    }

    public bool CanGive(IGameItem item, IBody target, int quantity = 0)
    {
        if (!Actor.ColocatedWith(target.Actor))
        {
            return false;
        }

        if (!this.CanPerformManualAction(out _))
        {
            return false;
        }

        return CanDrop(item, quantity) && target.CanGet(quantity == 0 ? item : item.PeekSplit(quantity), 0);
    }

    public string WhyCannotGive(IGameItem item, IBody target, int quantity = 0)
    {
        if (!Actor.ColocatedWith(target.Actor))
        {
            return "They are too far away for you to give anything to them.";
        }

        if (!this.CanPerformManualAction(out var manualReason))
        {
            return manualReason;
        }

        IGameItem dummy = item.PeekSplit(quantity);
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

    public void Give(IGameItem item, IBody target, int quantity = 0, IEmote? playerEmote = null)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		if (!CommandExecutionScope.TryContinue(executor)) return;
		ForeignCustodyTransferContext.EnsureBody(this, item);
		var sourceQuantity = item.Quantity;
		var sourceHolder = item.GetItemType<IHoldable>();
		var receiver = target;
		var receiverExecutor = receiver.Actor;
		var receiverCurrentBody = receiverExecutor.Body;
		var receiverCell = receiverExecutor.Location;
		var receiverLayer = receiverExecutor.RoomLayer;
		var receiverPosition = receiverExecutor.RoutePositionMetres;
		bool ReceiverUnchanged() => ReferenceEquals(receiverExecutor.Body, receiverCurrentBody) && ReferenceEquals(receiverExecutor.Location, receiverCell) &&
			receiverExecutor.RoomLayer == receiverLayer && receiverExecutor.RoutePositionMetres == receiverPosition;
		var whole = quantity == 0 || item.DropsWhole(quantity);
        if (!CanGive(item, target, quantity))
        {
            OutputHandler.Send(WhyCannotGive(item, target, quantity));
            return;
        }

        if (!ReferenceEquals(Actor, executor) || !CommandExecutionScope.TryContinue(executor)) return;
		var wasWielded = _heldItems.All(x => x.Item1 != item);
		var givenItem = GivePhysicalItem(item, receiver, quantity, executor, sourceQuantity, sourceHolder, whole, receiverExecutor, () => CanGive(item, target, quantity), ReceiverUnchanged);
		if (givenItem is null) return;
		var output = new MixedEmoteOutput(new Emote("@ give|gives $0 to $1", this, givenItem, target), flags: OutputFlags.SuppressObscured);

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
        foreach (IHandleEvents witness in Location.EventHandlersFor(Actor).Except(new IHandleEvents[] { Actor, target.Actor }))
        {
            witness.HandleEvent(EventType.CharacterGiveItemWitness, Actor, target.Actor, givenItem, witness);
        }

        foreach (IGameItem witness in ExternalItems)
        {
            witness.HandleEvent(EventType.CharacterGiveItemWitness, Actor, target.Actor, givenItem, witness);
        }
    }

    public bool CanGive(IGameItem item, ICorpse target, int quantity = 0)
    {
		if (target.Body is null)
		{
			return false;
		}

        var manipulation = Actor.CanManipulateItem(target.Parent);
        if (!manipulation.Truth)
        {
            return false;
        }

        if (!this.CanPerformManualAction(out _))
        {
            return false;
        }

        return CanDrop(item, quantity) &&
               target.Body.CanGet(quantity == 0 ? item : item.PeekSplit(quantity), 0);
    }

    public string WhyCannotGive(IGameItem item, ICorpse target, int quantity = 0)
    {
		if (target.Body is null)
		{
			return "You cannot give anything to these remains because their original body can no longer be identified.";
		}

        var manipulation = Actor.CanManipulateItem(target.Parent);
        if (!manipulation.Truth)
        {
            return manipulation.Message;
        }

        if (!this.CanPerformManualAction(out var manualReason))
        {
            return manualReason;
        }

        IGameItem dummy = item.PeekSplit(quantity);
        if (!CanDrop(item, quantity))
        {
            return "You cannot give that away.";
        }

        switch (target.Body.HoldLocs.WhyCannotGrab(dummy, target.Body))
        {
            case WhyCannotGrabReason.HandsFull:
                return target.Parent.HowSeen(this, true) + " has no free " +
                       target.Body.WielderDescriptionPlural + " to accept " + dummy.HowSeen(this) +
                       ".";
            case WhyCannotGrabReason.HandsTooDamaged:
                return target.Parent.HowSeen(this, true, DescriptionType.Possessive) + " " +
                       target.Body.WielderDescriptionPlural + " are too damaged to accept " +
                       dummy.HowSeen(this) + ".";
            case WhyCannotGrabReason.InventoryFull:
                return target.Parent.HowSeen(this, true) + " cannot hold " + dummy.HowSeen(this) + ".";
            case WhyCannotGrabReason.NoFreeUndamagedHands:
                return target.Parent.HowSeen(this, true) + " has no free, undamaged " +
                       target.Body.WielderDescriptionPlural + " to accept " + dummy.HowSeen(this) +
                       ".";
            default:
                return target.Parent.HowSeen(this, true) + " cannot hold " + dummy.HowSeen(this) + ".";
        }
    }

    public void Give(IGameItem item, ICorpse target, int quantity = 0, IEmote? playerEmote = null)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		if (!CommandExecutionScope.TryContinue(executor)) return;
		ForeignCustodyTransferContext.EnsureBody(this, item);
		var sourceQuantity = item.Quantity;
		var sourceHolder = item.GetItemType<IHoldable>();
		var receiver = target.Body;
		if (receiver is null) return;
		var receiverExecutor = receiver.Actor;
		var corpse = target.Parent;
		var reachUnchanged = PrepareItemReachSnapshot(corpse);
		if (reachUnchanged is null) return;
		var corpseCell = corpse.Location;
		var corpseLayer = corpse.RoomLayer;
		var corpsePosition = corpse.RoutePositionMetres;
		var receiverCell = receiverExecutor.Location;
		var receiverLayer = receiverExecutor.RoomLayer;
		var receiverPosition = receiverExecutor.RoutePositionMetres;
		bool ReceiverUnchanged() => reachUnchanged() && ReferenceEquals(target.Body, receiver) && ReferenceEquals(target.Parent, corpse) &&
			ReferenceEquals(corpse.Location, corpseCell) && corpse.RoomLayer == corpseLayer && corpse.RoutePositionMetres == corpsePosition &&
			ReferenceEquals(receiverExecutor.Location, receiverCell) && receiverExecutor.RoomLayer == receiverLayer &&
			receiverExecutor.RoutePositionMetres == receiverPosition;
		var whole = quantity == 0 || item.DropsWhole(quantity);
        if (!CanGive(item, target, quantity))
        {
            OutputHandler.Send(WhyCannotGive(item, target, quantity));
            return;
        }

        if (!ReferenceEquals(Actor, executor) || !CommandExecutionScope.TryContinue(executor)) return;
		var wasWielded = _heldItems.All(x => x.Item1 != item);
		var givenItem = ReferenceEquals(target.Body, receiver)
			? GivePhysicalItem(item, receiver, quantity, executor, sourceQuantity, sourceHolder, whole, receiverExecutor,
				() => ReferenceEquals(target.Body, receiver) && CanGive(item, target, quantity), ReceiverUnchanged)
			: null;
		if (givenItem is null) return;
		var output = new MixedEmoteOutput(new Emote("@ give|gives $0 to $1", this, givenItem, target.Parent), flags: OutputFlags.SuppressObscured);

        output.Append(playerEmote);
        OutputHandler.Handle(output);
        InventoryChanged = true;
        target.Body.InventoryChanged = true;
        OnInventoryChange?.Invoke(wasWielded ? InventoryState.Wielded : InventoryState.Held, InventoryState.Dropped,
            givenItem);
        givenItem.InvokeInventoryChange(wasWielded ? InventoryState.Wielded : InventoryState.Held,
            InventoryState.Dropped);

		// Character receiver events require the corpse's exact body to still be the final-death owner's body.
		// Other resolved remains accept physical gifts without dispatching those events against a survivor.
        if (target.GetOriginalCharacterWithMatchingBody() is not null)
        {
            HandleEvent(EventType.CharacterGiveItemGiver, Actor, target.OriginalCharacter, givenItem);
            target.OriginalCharacter.HandleEvent(EventType.CharacterGiveItemReceiver, Actor, target.OriginalCharacter,
                givenItem);
            givenItem.HandleEvent(EventType.ItemGiven, Actor, target.OriginalCharacter, givenItem);
            foreach (IHandleEvents witness in Location.EventHandlersFor(Actor).Except(new IHandleEvents[] { Actor, target.OriginalCharacter })
                    )
            {
                witness.HandleEvent(EventType.CharacterGiveItemWitness, Actor, target.OriginalCharacter, givenItem,
                    witness);
            }

            foreach (IGameItem witness in ExternalItems)
            {
                witness.HandleEvent(EventType.CharacterGiveItemWitness, Actor, target.OriginalCharacter, givenItem,
                    witness);
            }
        }
    }

    public void Take(IGameItem item)
	{
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		if (!CommandExecutionScope.TryContinue(executor)) return;
		CommandExecutionScope.MarkCommitted(executor);
		TakeInternal(item);
	}

	internal void TakeForNativeDeletion(IGameItem item)
	{
		// Only this deletion's item bypasses preflight. Its callbacks retain ordered reentrancy checks.
		TakeInternal(item);
	}

	private void TakeInternal(IGameItem item)
    {
		ForeignCustodyTransferContext.EnsureBody(this, item);
		using var exposureChange = MudSharp.Form.Material.EnvironmentalExposureService.For(Gameworld).Change(this);
        InventoryState oldState = InventoryState.Held;
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

        IWieldable wield = item.GetItemType<IWieldable>();
        wield?.PrimaryWieldedLocation = null;

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
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		bool CanContinue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
		if (!CanContinue()) return null;
		ForeignCustodyTransferContext.EnsureBody(this, item);
		var whole = item.DropsWhole(quantity);
		if (!CanContinue()) return null;
		CommandExecutionScope.MarkCommitted(executor);
        if (whole)
        {
			TakeInternal(item);
			return ComponentItemTransfer.IsDetached(item) ? item : null;
        }

		var split = item.Drop(null, quantity);
		return ComponentItemTransfer.IsDetached(split) ? split : null;
    }

    /// <summary>
    /// Swaps the hands in which the first and second items are held/wielded
    /// </summary>
    /// <param name="firstItem">The first item</param>
    /// <param name="secondItem">The second item</param>
    /// <returns>True if the swap took place</returns>
    public bool Swap(IGameItem firstItem, IGameItem secondItem)
    {
		ForeignCustodyTransferContext.EnsureBody(this, firstItem);
		ForeignCustodyTransferContext.EnsureBody(this, secondItem);
        if (!this.CanPerformManualAction(out var manualReason))
        {
            Actor.Send(manualReason);
            return false;
        }

        if (!HeldOrWieldedItems.Contains(firstItem) || (secondItem != null && !HeldOrWieldedItems.Contains(secondItem)))
        {
            Actor.Send("You cannot swap items that you aren't holding.");
            return false;
        }

        bool item1Wielded = WieldedItems.Contains(firstItem);
        bool item2Wielded = WieldedItems.Contains(secondItem);

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

            if (itemWielded && WieldLocs.Any(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse && _wieldedItems.All(y => y.Item2 != x)))
            {
                return WieldLocs.First(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse && _wieldedItems.All(y => y.Item2 != x));
            }

            if (HoldLocs.Any(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse && _heldItems.All(y => y.Item2 != x)))
            {
                return HoldLocs.First(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse && _heldItems.All(y => y.Item2 != x));
            }

            return null;
        }

        IBodypart targetloc1 = FindTargetLocation(secondItem, item1Wielded);
        IBodypart targetloc2 = FindTargetLocation(firstItem, item2Wielded);
        if (targetloc1 is null || CanUseBodypart(targetloc1) != CanUseBodypartResult.CanUse ||
            secondItem is not null && (targetloc2 is null || CanUseBodypart(targetloc2) != CanUseBodypartResult.CanUse))
        {
            Actor.Send("You do not have functioning locations available to swap those items.");
            return false;
        }

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
                        IGrab fallBack = HoldLocs.Except(part).Except(oldPart).OfType<IGrab>()
                                               .FirstOrDefault(x =>
                                                   x.CanGrab(firstItem, this) == WearlocGrabResult.Success);
                        if (fallBack == null)
                        {
                            Actor.Send(
                                $"You set {item.HowSeen(Actor)} down as you can't figure out a way to hold it.");
                            Take(item);
                            item.RoomLayer = RoomLayer;
                            item.InsertAtSource(Actor);
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
                IGrab fallBack = HoldLocs.Except(part).Except(oldPart).OfType<IGrab>()
                                       .FirstOrDefault(x => x.CanGrab(item, this) == WearlocGrabResult.Success);
                if (fallBack == null)
                {
                    Actor.Send(
                        $"You set {item.HowSeen(Actor)} down as you can't figure out a way to hold it.");
                    Take(item);
                    item.RoomLayer = RoomLayer;
                    item.InsertAtSource(Actor);
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

    public ILookup<IWear, int> WornItemCounts => _wearlocs.Select(x => Tuple.Create(x, _wornItems.Count(y => y.Wearloc == x)))
                            .ToLookup(x => x.Item1, x => x.Item2);

    public void RemoveItem(IGameItem item)
	{
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		if (!CommandExecutionScope.TryContinue(executor)) return;
		CommandExecutionScope.MarkCommitted(executor);
		RemoveItemInternal(item);
	}

	private void RemoveItemInternal(IGameItem item)
    {
		using var exposureChange = MudSharp.Form.Material.EnvironmentalExposureService.For(Gameworld).Change(this);
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
        StringBuilder sb = new();
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

    private List<IGameItem> _directWornItems = new();
    public IEnumerable<IGameItem> DirectWornItems => _directWornItems;

    private List<IGameItem> _wornItemsOnly = new();
    public IEnumerable<IGameItem> WornItems => _wornItemsOnly;

    public bool IsOuterwear(IGameItem item)
    {
        return _outerwear.Contains(item);
    }

    private HashSet<IGameItem> _outerwear = new();

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
            List<IGameItem> items =
                _wornItems.GroupBy(x => x.Item, x => x)
                          .OrderBy(x => x.Average(y => y.Wearloc.DisplayOrder))
                          .Select(x => x.Key)
                          .ToList();
            foreach (
                IBeltable beltitem in
                items.SelectNotNull(x => x.GetItemType<IBelt>()).SelectMany(x => x.ConnectedItems).ToList())
            {
                items.Insert(items.IndexOf(beltitem.ConnectedTo.Parent), beltitem.Parent);
            }

            return items;
        }
    }

    public void RemoveItem(IGameItem item, IEmote playerEmote, ICharacter remover)
    {
        var targetActor = Actor;
        var removerBody = remover.Body;
        using var execution = CommandExecutionScope.EnterBodyOperation(remover);
        bool CanContinue() => ReferenceEquals(Actor, targetActor) && ReferenceEquals(remover.Body, removerBody) && CommandExecutionScope.TryContinue(remover);
        if (!CanContinue()) return;
        if (!CanBeRemoved(item, remover))
        {
            remover.Send(WhyCannotBeRemoved(item, remover));
            return;
        }

        if (!CanContinue()) return;
        IObscureCharacteristics obscurer = item.GetItemType<IObscureCharacteristics>();
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
        if (!CanContinue()) return;
        CommandExecutionScope.InvokeOwned(targetActor, () => { RemoveItem(item); return true; });
        if (_wornItems.Any(x => ReferenceEquals(x.Item, item))) return;
        HandleWornItemRemovedEvent(item, remover);
    }

    private void HandleWornItemRemovedEvent(IGameItem item, ICharacter remover)
    {
        Actor.HandleEvent(EventType.CharacterWornItemRemoved, Actor, remover, item);
        item.HandleEvent(EventType.ItemRemovedFromWear, Actor, remover, item);
        foreach (IHandleEvents witness in Location.EventHandlersFor(Actor))
        {
            witness.HandleEvent(EventType.CharacterWornItemRemovedWitness, Actor, remover, item, witness);
        }
    }

    public void RemoveItem(IGameItem item, IEmote playerEmote, bool silent = false,
        ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		bool CanContinue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
		if (!CanContinue()) return;
        if (!CanRemoveItem(item, ignoreFlags))
        {
            if (!silent)
            {
                OutputHandler.Send(WhyCannotRemove(item, false, ignoreFlags));
            }

            return;
        }

        if (!CanContinue()) return;
		var placement = ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreFreeHands) ? null : PrepareGetPlacement(item, true);
		if (!CanContinue() || (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreFreeHands) && placement is null)) return;
        IObscureCharacteristics obscurer = item.GetItemType<IObscureCharacteristics>();
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

        if (!CanContinue()) return;
		CommandExecutionScope.MarkCommitted(executor);
        RemoveItemInternal(item);
        if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreFreeHands))
        {
            if (item.Deleted || item.Destroyed || item.ContainedIn is not null || ComponentItemTransfer.DirectLocationOf(item) is not null ||
				!ReferenceEquals(item.InInventoryOf, this)) return;
			item.Get(this);
			if (!CompleteGetPlacement(item, placement!)) return;
			NotifyGotItem(item, executor);
        }

        HandleWornItemRemovedEvent(item, Actor);
        InventoryChanged = true;
    }

    public void Wear(IGameItem item, string profile, IEmote playerEmote, bool silent = false)
    {
		using var execution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Actor);
		ForeignCustodyTransferContext.EnsureBody(this, item);
        if (string.IsNullOrEmpty(profile))
        {
            Wear(item, playerEmote, silent);
            return;
        }

        IWearable wearable = item.GetItemType<IWearable>();
        if (wearable == null)
        {
            OutputHandler.Send(item.HowSeen(this, true) + " is not something that can be worn.");
            return;
        }

        IWearProfile wprofile =
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
        IWearable wearable = item.GetItemType<IWearable>();
        if (wearable == null)
        {
            return false;
        }

        IWearProfile profile =
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

        IWearable wearable = item.GetItemType<IWearable>();
        if (wearable is null)
        {
            return false;
        }

        Dictionary<IWear, IWearlocProfile> profiles = profile.Profile(this);
        if (profile.RequireContainerIsEmpty && item.GetItemType<IContainer>()?.Contents.Any() == true)
        {
            return false;
        }

        if (item.GetItemType<IRestraint>()?.RestraintType == RestraintType.Binding)
        {
            return false;
        }

        foreach (KeyValuePair<IWear, IWearlocProfile> availableProfile in profiles)
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
        IWearable wearable = item.GetItemType<IWearable>();
        if (wearable == null)
        {
            return false;
        }

        return wearable.CanWear(this) &&
               wearable.Profiles.Any(x => CanWear(item, x));
    }

    public IWearProfile WhichProfile(IGameItem item)
    {
        IWearable wearable = item.GetItemType<IWearable>();
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
		var executor = Actor;
		using var execution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(executor);
		ForeignCustodyTransferContext.EnsureBody(this, item);
        var canWear = CanWear(item);
		if (!ReferenceEquals(Actor, executor) || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(executor)) return;
        if (!canWear)
        {
            if (!silent)
            {
                IWearable wearable = item.GetItemType<IWearable>();
                OutputHandler.Send(
                    wearable?.Profiles.Count() == 1 ?
                        WhyCannotWear(item, wearable.Profiles.First()) :
                        WhyCannotWear(item));
            }

            return;
        }

		var profile = WhichProfile(item);
		if (!ReferenceEquals(Actor, executor) || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(executor)) return;
        Wear(item, profile, playerEmote, silent);
    }

    public void Restrain(IGameItem item, IWearProfile profile, ICharacter restrainer, IGameItem targetItem,
        IEmote? emote = null,
        bool silent = false)
    {
        var targetActor = Actor;
        var sourceBody = restrainer.Body;
        using var execution = CommandExecutionScope.EnterBodyOperation(restrainer);
        bool CanContinue() => ReferenceEquals(Actor, targetActor) && ReferenceEquals(restrainer.Body, sourceBody) && CommandExecutionScope.TryContinue(restrainer);
        if (!CanContinue()) return;
        if (!restrainer.CanPerformManualAction(out var manualReason))
        {
            restrainer.Send(manualReason);
            return;
        }

        if (!restrainer.ColocatedWith(Actor))
        {
            restrainer.Send("They are too far away for you to restrain.");
            return;
        }

        if (!CanContinue()) return;
        var wearable = item.GetItemType<IWearable>();
        var restraint = item.GetItemType<IRestraint>();
        if (wearable is null || restraint is null || !ReferenceEquals(item.InInventoryOf, sourceBody)) return;
        var locations = profile.Profile(this).Select(x => (item, x.Key, x.Value)).ToArray();
        var limbs = Limbs.Where(x => restraint.Limbs.Contains(x.LimbType) && profile.AllProfiles.Any(y => x.Parts.Contains(y.Key))).ToList();
        var returnItem = (sourceBody as Body)?.PrepareDetachedItemReturn(item);
        if (returnItem is null || !CanContinue()) return;

        if (!silent && restrainer != null)
        {
            MixedEmoteOutput output =
                new(
                    new Emote(
                        $"@ {profile.WearAction1st}|{profile.WearAction3rd} {profile.WearAffix} with $2", restrainer,
                        restrainer, this, item),
                    flags: OutputFlags.SuppressObscured);
            output.Append(emote);
            OutputHandler.Handle(output);
        }

        if (!CanContinue()) return;
        sourceBody.Take(item);
        if (!ComponentItemTransfer.IsDetached(item)) return;
        if (!CanContinue()) { returnItem(); return; }
        CommandExecutionScope.MarkCommitted(restrainer);
        _wornItems.AddRange(locations);
        wearable.UpdateWear(this, profile);
        UpdateDescriptionWorn(item);
        InventoryChanged = true;
        if (!CanContinue() || !ReferenceEquals(wearable.WornBy, this)) return;
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

    public void WearExternally(IGameItem item, IWearProfile? profile = null)
    {
		var executor = Actor;
		using var execution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(executor);
		profile ??= WhichProfile(item);
		if (!ReferenceEquals(Actor, executor) || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(executor)) return;
        WearInternal(item, profile, null, true);
    }

    public void Wear(IGameItem item, IWearProfile profile, IEmote? playerEmote = null, bool silent = false)
    {
		var executor = Actor;
		using var execution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(executor);
		ForeignCustodyTransferContext.EnsureBody(this, item);
        if (!this.CanPerformManualAction(out var reason))
        {
            if (!silent) OutputHandler.Send(reason);
            return;
        }

		if (!ReferenceEquals(Actor, executor) || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(executor)) return;
        WearInternal(item, profile, playerEmote, silent);
    }

    private void WearInternal(IGameItem item, IWearProfile profile, IEmote? playerEmote, bool silent)
    {
		var executor = Actor;
		bool CanContinue() => ReferenceEquals(Actor, executor) && MudSharp.NPC.AI.CommandExecutionScope.TryContinue(executor);
        if (!CanContinue()) return;
        using var exposureChange = MudSharp.Form.Material.EnvironmentalExposureService.For(Gameworld).Change(this);
        var canWear = CanWear(item, profile);
        if (!CanContinue()) return;
        if (!canWear)
        {
            if (!silent)
            {
                OutputHandler.Send(WhyCannotWear(item, profile));
            }

            return;
        }

        if (!CanContinue()) return;
        var wearLocations = profile.Profile(this).Select(x => (item, x.Key, x.Value)).ToList();
        if (!CanContinue()) return;

        if (!silent)
        {
            MixedEmoteOutput output =
                new(
                    new Emote(
                        $"@ {profile.WearAction1st}|{profile.WearAction3rd} {profile.WearAffix} $0", this, item),
                    flags: OutputFlags.SuppressObscured);
            output.Append(playerEmote);
            OutputHandler.Handle(output);
        }

        if (!CanContinue()) return;
        MudSharp.NPC.AI.CommandExecutionScope.MarkCommitted(executor);
        bool wasWielded = _heldItems.All(x => x.Item1 != item);
        _heldItems.RemoveAll(x => x.Item1 == item);
        _wieldedItems.RemoveAll(x => x.Item1 == item);
        _wornItems.AddRange(wearLocations);
        item.GetItemType<IWearable>().UpdateWear(this, profile);
        UpdateDescriptionWorn(item);
        InventoryChanged = true;
        OnInventoryChange?.Invoke(wasWielded ? InventoryState.Wielded : InventoryState.Held, InventoryState.Worn,
            item);
        item.InvokeInventoryChange(wasWielded ? InventoryState.Wielded : InventoryState.Held, InventoryState.Worn);
        CheckConsequences();
        item.HandleEvent(EventType.ItemWorn, item, Actor);
        foreach (IHandleEvents witness in Location.EventHandlersFor(Actor).ToList())
        {
            witness.HandleEvent(EventType.ItemWornWitness, item, Actor, witness);
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
        if (!remover.CanPerformManualAction(out var manualReason))
        {
            return false;
        }

        var reach = remover.CanReachItem(item, requireInventoryPermission: false);
        if (!reach.Truth)
        {
            return false;
        }

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
        if (!dresser.CanPerformManualAction(out var manualReason))
        {
            return false;
        }

        if (!dresser.ColocatedWith(Actor))
        {
            return false;
        }

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
        if (!dresser.CanPerformManualAction(out var manualReason))
        {
            return manualReason;
        }

        if (!dresser.ColocatedWith(Actor))
        {
            return "They are too far away for you to dress.";
        }

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

    public bool Dress(IGameItem item, ICharacter dresser, IWearProfile profile = null, IEmote? emote = null)
    {
        IWearProfile tempProfile = profile ?? WhichProfile(item) ?? item.GetItemType<IWearable>()?.DefaultProfile;

        //If no profile was passed in, grab the default

        if (!CanDress(item, dresser, tempProfile))
        {
            dresser.Send(WhyCannotDress(item, dresser, tempProfile));
            return false;
        }

        dresser.OutputHandler.Handle(new MixedEmoteOutput(new Emote("@ dress|dresses $0 in $1", dresser, Actor, item))
            .Append(emote));
        dresser.Body.Take(item);
        WearExternally(item, tempProfile);
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
        HashSet<IBodypart> parts = new(Bodyparts);
        HashSet<IBodypart> transparentParts = new(Bodyparts);
        _visiblySeveredBodyparts = SeveredRoots.ToHashSet();
        foreach (IGameItem item in DirectWornItems)
        {
            foreach (KeyValuePair<IWear, IWearlocProfile> profile in item.GetItemType<IWearable>().CurrentProfile.AllProfiles)
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
		if (Actor?.Gameworld is { } world) world.MagicCasting?.NotifyCapacityChange(Actor);
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

    private HashSet<IBodypart> _exposedBodyparts = new();

    /// <summary>
    /// Bodyparts that have no items covering them
    /// </summary>
    public IEnumerable<IBodypart> ExposedBodyparts => _exposedBodyparts;

    private HashSet<IBodypart> _visiblySeveredBodyparts = new();

    public IEnumerable<IBodypart> VisiblySeveredBodyparts => _visiblySeveredBodyparts;

    private HashSet<IGameItem> _itemsWornAgainstSkin = new();

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
        List<IBodypart> parts = Bodyparts.Where(x => x.DownstreamOfPart(part) || x == part).ToList();
        foreach (Tuple<IGameItem, IGrab> item in _heldItems)
        {
            if (!parts.Contains(item.Item2))
            {
                continue;
            }

            yield return item.Item1;
        }

        foreach (Tuple<IGameItem, IWield> item in _wieldedItems)
        {
            if (!parts.Contains(item.Item2))
            {
                continue;
            }

            yield return item.Item1;
        }

        foreach ((IGameItem Item, IWear Wearloc, IWearlocProfile Profile) item in _wornItems)
        {
            if (!item.Profile.Mandatory || !parts.Contains(item.Wearloc))
            {
                continue;
            }

            yield return item.Item;
        }

        foreach (IImplant item in _implants)
        {
            if (!parts.Contains(item.TargetBodypart))
            {
                continue;
            }

            yield return item.Parent;
        }

        foreach (IProsthetic item in _prosthetics)
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
            List<IWield> locs = _wieldedItems.Where(x => x.Item1 == target).Select(x => x.Item2).ToList();
            if (locs.Count == 1)
            {
                return locs.First().Alignment;
            }

            int leftness = 0, frontness = 0;
            foreach (IWield loc in locs)
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
        IWearable wearable = item.GetItemType<IWearable>();
        if (wearable == null)
        {
            return $"You cannot wear {item.HowSeen(Actor)} because it is not something that can be worn.";
        }

        IWearProfile profile =
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
            IRestraint restraint = item.GetItemType<IRestraint>();
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
        if (!remover.CanPerformManualAction(out var manualReason))
        {
            return manualReason;
        }

        var reach = remover.CanReachItem(item, requireInventoryPermission: false);
        if (!reach.Truth)
        {
            return reach.Message;
        }

        if (!Actor.WillingToPermitInventoryManipulation(remover))
        {
            return "You can only take things from willing people, or corpses.";
        }

        if (item.IsItemType<IRestraint>())
        {
            IRestraint restraint = item.GetItemType<IRestraint>();
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
        Dictionary<IGameItem, (int totalWearlocs, int coveredWearlocs)> itemCoverageData = new();
        HashSet<IWear> currentlyCoveredLocations = new();

        // Process the worn items in reverse order to account for covering items
        for (int i = _wornItems.Count - 1; i >= 0; i--)
        {
            (IGameItem Item, IWear Wearloc, IWearlocProfile Profile) wornItem = _wornItems[i];
            IGameItem item = wornItem.Item;
            IWear wearloc = wornItem.Wearloc;
            IWearlocProfile profile = wornItem.Profile;

            // Initialize coverage data for the item if not already present
            if (!itemCoverageData.TryGetValue(item, out (int totalWearlocs, int coveredWearlocs) data))
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
        Dictionary<IGameItem, WearableItemCoverStatus> result = new();
        foreach (KeyValuePair<IGameItem, (int totalWearlocs, int coveredWearlocs)> kvp in itemCoverageData)
        {
            IGameItem item = kvp.Key;
            (int totalWearlocs, int coveredWearlocs) = kvp.Value;

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
        IWearable existingWearable = existingItem.GetItemType<IWearable>();
        IWearable newWearable = newItem.GetItemType<IWearable>();
        if (existingWearable != null && newWearable != null && _wornItems.Any(x => x.Item == existingItem))
        {
            IWearProfile newProfile = newWearable.Profiles.First(x => existingWearable.CurrentProfile.CompatibleWith(x));
            Dictionary<IWear, IWearlocProfile> newWearProfile = newProfile.Profile(this);
            int highestIndex = _wornItems.FindLastIndex(x => x.Item == existingItem);
            foreach (KeyValuePair<IWear, IWearlocProfile> element in newWearProfile)
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
            foreach (Tuple<IGameItem, IWield> loc in _wieldedItems.Where(x => x.Item1 == existingItem).ToList())
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
            foreach (Tuple<IGameItem, IGrab> loc in _heldItems.Where(x => x.Item1 == existingItem).ToList())
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

    internal static Dictionary<ICurrencyPile, Dictionary<ICoin, int>> FindCurrencyPreservingOwnership(
        ICurrency currency, IEnumerable<ICurrencyPile> targetPiles, decimal amount)
    {
        return targetPiles
               .GroupBy(x => x.Parent.OwnershipReference)
               .Select(x => currency.FindCurrency(x, amount))
               .Where(x => x.Count > 0)
               .Select(x => new
               {
                   Coins = x,
                   Total = x.Sum(y => y.Value.Sum(z => z.Key.Value * z.Value))
               })
               .OrderBy(x => x.Total == amount ? 0 : 1)
               .ThenBy(x => Math.Abs(x.Total - amount))
               .Select(x => x.Coins)
               .FirstOrDefault() ?? [];
    }

    private static IGameItem CreateCurrencyPileFromSelection(ICurrency currency,
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins, bool temporary = false)
    {
        var newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
            targetCoins.SelectMany(x => x.Value)
                       .Select(x => x.Key)
                       .Distinct()
                       .Select(x => Tuple.Create(x,
                           targetCoins.Sum(y => y.Value.Where(z => z.Key == x).Sum(z => z.Value)))),
            temporary);
		if (temporary) ((MudSharp.GameItems.GameItem)newItem).CopyCurrencyPreviewOwner(targetCoins.First().Key.Parent);
		else newItem.CopyOwnerFrom(targetCoins.First().Key.Parent);
        return newItem;
    }

    private IEnumerable<ICurrencyPile> AccessibleRoomCurrencyPiles()
    {
        return Location.LayerGameItems(RoomLayer)
            .SelectNotNull(x => x.GetItemType<ICurrencyPile>())
            .Where(x => CanGet(x.Parent, 0, ItemCanGetIgnore.IgnoreWeight));
    }

    private IEnumerable<ICurrencyPile> AccessibleContainerCurrencyPiles(IGameItem container)
    {
        var component = container.GetItemType<IContainer>();
        return component?.Contents
            .SelectNotNull(x => x.GetItemType<ICurrencyPile>())
            .Where(x => CanGet(x.Parent, container, 0, ItemCanGetIgnore.IgnoreWeight)) ?? Enumerable.Empty<ICurrencyPile>();
    }

    public bool CanGet(ICurrency currency, decimal amount, bool exact)
    {
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins =
            FindCurrencyPreservingOwnership(currency,
                AccessibleRoomCurrencyPiles(), amount);
        if (!targetCoins.Any())
        {
            return false;
        }

        if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
        {
            return false;
        }

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);
        return CanGet(tempItem, 0);
    }

    public bool CanGet(ICurrency currency, IGameItem container, decimal amount, bool exact)
    {
        if (container?.GetItemType<IContainer>() is null || !Actor.CanReachItem(container, requireInventoryPermission: false).Truth)
        {
            return false;
        }

        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins =
            FindCurrencyPreservingOwnership(currency,
                AccessibleContainerCurrencyPiles(container),
                amount);
        if (!targetCoins.Any())
        {
            return false;
        }

        if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
        {
            return false;
        }

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);

        return CanGet(tempItem, container, 0, ItemCanGetIgnore.IgnoreInContainer);
    }

    public string WhyCannotGet(ICurrency currency, decimal amount, bool exact)
    {
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins =
            FindCurrencyPreservingOwnership(currency,
                AccessibleRoomCurrencyPiles(), amount);
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> trueTargetCoins =
            currency.FindCurrency(Location.LayerGameItems(RoomLayer).SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
                amount);
        if (!targetCoins.Any())
        {
            var mountBlockedPile = trueTargetCoins.Keys.FirstOrDefault(x => !Actor.MountedCanRetrieve(x.Parent, out _));
            if (mountBlockedPile is not null && !Actor.MountedCanRetrieve(mountBlockedPile.Parent, out var mountMessage))
            {
                return mountMessage;
            }

            if (trueTargetCoins.Any())
            {
                var pile = trueTargetCoins.First().Key.Parent;
                return WhyCannotGet(pile, 0, ItemCanGetIgnore.IgnoreWeight);
            }

            return "There is no money at all to get.";
        }

        if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
        {
            return "You cannot get that exact amount. The closest amount you can get is " +
                   currency.Describe(targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)),
                       CurrencyDescriptionPatternType.Short).Colour(Telnet.Green) + ".";
        }

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);
        return WhyCannotGet(tempItem, 0);
    }

    public string WhyCannotGet(ICurrency currency, IGameItem container, decimal amount, bool exact)
    {
        if (container?.GetItemType<IContainer>() is null)
        {
            return "That is not a container.";
        }

        var manipulation = Actor.CanReachItem(container, requireInventoryPermission: false);
        if (!manipulation.Truth)
        {
            return manipulation.Message;
        }

        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins =
            FindCurrencyPreservingOwnership(currency,
                AccessibleContainerCurrencyPiles(container),
                amount);
        if (!targetCoins.Any())
        {
            var inaccessible = container.GetItemType<IContainer>().Contents.FirstOrDefault(x => x.IsItemType<ICurrencyPile>());
            return inaccessible is not null
                ? WhyCannotGet(inaccessible, container, 0)
                : "There is no money in " + container.HowSeen(this) + " at all to get.";
        }

        if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
        {
            return "You cannot get that exact amount from " + container.HowSeen(this) +
                   ". The closest amount you can get is " +
                   currency.Describe(targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)),
                       CurrencyDescriptionPatternType.Short).Colour(Telnet.Green) + ".";
        }

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);
        return WhyCannotGet(tempItem, container, 0, ItemCanGetIgnore.IgnoreInContainer);
    }

    public void Get(ICurrency currency, IGameItem containerItem, decimal amount, bool exact, IEmote? playerEmote = null,
        bool silent = false)
    {
        Get(currency, containerItem, amount, exact, playerEmote, silent, null);
    }

    public IGameItem? Get(ICurrency currency, IGameItem containerItem, decimal amount, bool exact,
        IEmote? playerEmote, bool silent, IEnumerable<IHandleEvents> witnessHandlers)
    {
		return TransferPreparedCurrency(currency, amount, exact, CurrencyTransferKind.GetContainer, containerItem: containerItem, playerEmote: playerEmote, silent: silent, witnessHandlers: witnessHandlers);
	}

    public void Get(ICurrency currency, decimal amount, bool exact, IEmote? playerEmote = null, bool silent = false)
    {
        Get(currency, amount, exact, playerEmote, silent, null);
    }

    public IGameItem? Get(ICurrency currency, decimal amount, bool exact, IEmote? playerEmote, bool silent,
        IEnumerable<IHandleEvents> witnessHandlers)
    {
		return TransferPreparedCurrency(currency, amount, exact, CurrencyTransferKind.GetRoom, playerEmote: playerEmote, silent: silent, witnessHandlers: witnessHandlers);
	}

    public bool CanPut(ICurrency currency, IGameItem container, ICharacter? containerOwner, decimal amount, bool exact)
    {
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = FindCurrencyPreservingOwnership(currency, HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
            amount);
        if (!targetCoins.Any())
        {
            return false;
        }

        if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
        {
            return false;
        }

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);
        return CanPut(tempItem, container, containerOwner, 0, false);
    }

    public string WhyCannotPut(ICurrency currency, IGameItem container, ICharacter? containerOwner, decimal amount,
        bool exact)
    {
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = FindCurrencyPreservingOwnership(currency, HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()), amount);
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

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);
        return WhyCannotPut(tempItem, container, containerOwner, 0, false);
    }

    public void Put(ICurrency currency, IGameItem container, ICharacter? containerOwner, decimal amount, bool exact,
        IEmote? playerEmote = null,
        bool silent = false)
    {
        Put(currency, container, containerOwner, amount, exact, playerEmote, silent, null);
    }

    public IGameItem? Put(ICurrency currency, IGameItem container, ICharacter? containerOwner, decimal amount, bool exact,
        IEmote? playerEmote, bool silent, IEnumerable<IHandleEvents> witnessHandlers)
    {
		return TransferPreparedCurrency(currency, amount, exact, CurrencyTransferKind.Put, containerItem: container, containerOwner: containerOwner, playerEmote: playerEmote, silent: silent, witnessHandlers: witnessHandlers);
	}

    public bool CanDrop(ICurrency currency, decimal amount, bool exact)
    {
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = FindCurrencyPreservingOwnership(currency, HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
            amount);
        if (!targetCoins.Any())
        {
            return false;
        }

        if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
        {
            return false;
        }

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);
        return CanDrop(tempItem, 0);
    }

    public string WhyCannotDrop(ICurrency currency, decimal amount, bool exact)
    {
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = FindCurrencyPreservingOwnership(currency, HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
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

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);
        return WhyCannotDrop(tempItem, 0);
    }

    public void Drop(ICurrency currency, decimal amount, bool exact, bool newStack = false, IEmote? playerEmote = null,
        bool silent = false)
    {
		TransferPreparedCurrency(currency, amount, exact, CurrencyTransferKind.Drop, newStack: newStack, playerEmote: playerEmote, silent: silent);
	}

    public bool CanGive(ICurrency currency, IBody target, decimal amount, bool exact)
    {
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = FindCurrencyPreservingOwnership(currency, HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
            amount);
        if (!targetCoins.Any())
        {
            return false;
        }

        if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
        {
            return false;
        }

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);
        return CanGive(tempItem, target);
    }

    public string WhyCannotGive(ICurrency currency, IBody target, decimal amount, bool exact)
    {
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = FindCurrencyPreservingOwnership(currency, HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
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

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);
        return WhyCannotGive(tempItem, target);
    }

    public void Give(ICurrency currency, IBody target, decimal amount, bool exact, IEmote? playerEmote = null)
    {
		TransferPreparedCurrency(currency, amount, exact, CurrencyTransferKind.Give, recipient: target, playerEmote: playerEmote);
	}

    public bool CanGive(ICurrency currency, ICorpse target, decimal amount, bool exact)
    {
		if (target.Body is null)
		{
			return false;
		}

        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = FindCurrencyPreservingOwnership(currency, HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
            amount);
        if (!targetCoins.Any())
        {
            return false;
        }

        if (exact && targetCoins.Sum(x => x.Value.Sum(y => y.Key.Value * y.Value)) != amount)
        {
            return false;
        }

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);
        return CanGive(tempItem, target.Body);
    }

    public string WhyCannotGive(ICurrency currency, ICorpse target, decimal amount, bool exact)
    {
		if (target.Body is null)
		{
			return "You cannot give anything to these remains because their original body can no longer be identified.";
		}

        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = FindCurrencyPreservingOwnership(currency, HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()), amount);
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

        IGameItem tempItem = CreateCurrencyPileFromSelection(currency, targetCoins, true);
        return WhyCannotGive(tempItem, target.Body);
    }

    public void Give(ICurrency currency, ICorpse target, decimal amount, bool exact, IEmote? playerEmote = null)
    {
		if (target.Body is null)
		{
			OutputHandler.Send(WhyCannotGive(currency, target, amount, exact));
			return;
		}
		TransferPreparedCurrency(currency, amount, exact, CurrencyTransferKind.Give, recipient: target.Body, corpse: target, playerEmote: playerEmote);
	}

    #endregion

    #region Commodity-Related versions of inventory commands

    public bool CanGetByWeight(IGameItem item, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
        return double.IsFinite(weight) && weight > 0.0 &&
               CanGet(item, 0, ignoreFlags | ItemCanGetIgnore.IgnoreWeight) &&
               CanGet(item.PeekSplitByWeight(weight), 0, ignoreFlags);
    }

    public bool CanGetByWeight(IGameItem item, IGameItem container, double weight,
        ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
        return double.IsFinite(weight) && weight > 0.0 &&
               CanGet(item, container, 0, ignoreFlags | ItemCanGetIgnore.IgnoreWeight) &&
               CanGet(item.PeekSplitByWeight(weight), 0,
                   container.InInventoryOf == this ? ignoreFlags | ItemCanGetIgnore.IgnoreWeight : ignoreFlags);
    }

    public string WhyCannotGetByWeight(IGameItem item, double weight,
        ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
        if (!double.IsFinite(weight) || weight <= 0.0) return "You must specify a finite positive weight.";
        return !CanGet(item, 0, ignoreFlags | ItemCanGetIgnore.IgnoreWeight)
            ? WhyCannotGet(item, 0, ignoreFlags | ItemCanGetIgnore.IgnoreWeight)
            : WhyCannotGet(item.PeekSplitByWeight(weight), 0, ignoreFlags);
    }

    public string WhyCannotGetByWeight(IGameItem item, IGameItem container, double weight,
        ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
        if (!double.IsFinite(weight) || weight <= 0.0) return "You must specify a finite positive weight.";
        return !CanGet(item, container, 0, ignoreFlags | ItemCanGetIgnore.IgnoreWeight)
            ? WhyCannotGet(item, container, 0, ignoreFlags | ItemCanGetIgnore.IgnoreWeight)
            : WhyCannotGet(item.PeekSplitByWeight(weight), 0,
                container.InInventoryOf == this ? ignoreFlags | ItemCanGetIgnore.IgnoreWeight : ignoreFlags);
    }

    public void GetByWeight(IGameItem item, double weight, IEmote? playerEmote = null, bool silent = false,
        ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		bool CanContinue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
		if (!CanContinue()) return;
        if (!CanGetByWeight(item, weight, ignoreFlags))
        {
            if (!silent) OutputHandler.Send(WhyCannotGetByWeight(item, weight, ignoreFlags));
            return;
        }

		if (!CanContinue()) return;
		var whole = item.DropsWholeByWeight(weight);
		if (!CanContinue()) return;
		if (whole) { Get(item, 0, playerEmote, silent, ignoreFlags); return; }
		GetPreparedWeightSplit(item, weight, playerEmote, silent, executor);
    }

    public void GetByWeight(IGameItem item, IGameItem container, double weight, IEmote? playerEmote = null,
        bool silent = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		bool CanContinue() => ReferenceEquals(Actor, executor) && CommandExecutionScope.TryContinue(executor);
		if (!CanContinue()) return;
        if (!CanGetByWeight(item, container, weight, ignoreFlags))
        {
            if (!silent) OutputHandler.Send(WhyCannotGetByWeight(item, container, weight, ignoreFlags));
            return;
        }

		if (!CanContinue()) return;
		var whole = item.DropsWholeByWeight(weight);
		if (!CanContinue()) return;
        if (whole)
        {
            Get(item, container, 0, playerEmote, silent, ignoreFlags);
            return;
        }

		GetPreparedWeightSplit(item, weight, playerEmote, silent, executor, container);
    }

	private void GetPreparedWeightSplit(IGameItem item, double weight, IEmote? playerEmote, bool silent,
		ICharacter executor, IGameItem? container = null)
	{
		var placement = PrepareGetPlacement(item.PeekSplitByWeight(weight), true);
		if (placement is null || !ReferenceEquals(Actor, executor) || !CommandExecutionScope.TryContinue(executor)) return;
		CommandExecutionScope.MarkCommitted(executor);
		var split = item.GetByWeight(null, weight);
		if (!ComponentItemTransfer.IsDetached(split)) return;
		split.Get(this);
		if (!CompleteGetPlacement(split, placement)) return;
		if (!silent)
		{
			var text = container is null ? "@ get|gets $0" : "@ get|gets $0 from $1";
			OutputHandler.Handle(new MixedEmoteOutput(new Emote(text, this, split, container), flags: OutputFlags.SuppressObscured).Append(playerEmote));
		}
		NotifyGotItem(split, executor);
	}

    #endregion
}

public static class DrapeableExtensionClass
{
    public static WhyCannotDrapeReason WhyCannotDrape<T>(this IEnumerable<T> wearlocs, IGameItem item, IBody body)
        where T : IWear
    {
        IWearable wearable = item.GetItemType<IWearable>();
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

        WhyCannotDrapeReason result = item.GetItemType<IWearable>()?.WhyCannotWear(body, profile) ?? WhyCannotDrapeReason.NotIDrapeable;
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
