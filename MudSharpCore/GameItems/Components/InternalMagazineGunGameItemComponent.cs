using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class InternalMagazineGunGameItemComponent : FirearmBaseGameItemComponent, IRangedWeapon, ISwitchable, IMeleeWeapon
{
    protected InternalMagazineGunGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (InternalMagazineGunGameItemComponentProto)newProto;
    }

    #region Constructors

    public InternalMagazineGunGameItemComponent(InternalMagazineGunGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(proto, parent, temporary)
    {
        _prototype = proto;
    }

    public InternalMagazineGunGameItemComponent(Models.GameItemComponent component,
        InternalMagazineGunGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public InternalMagazineGunGameItemComponent(InternalMagazineGunGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
		foreach (var round in rhs._roundsInMagazine)
		{
			var clonedRound = round.DeepCopy(!temporary, true);
			clonedRound.ContainedIn = newParent;
			_roundsInMagazine.Add(clonedRound);
		}

		if (rhs.ChamberedCasing is not null)
		{
			ChamberedCasing = rhs.ChamberedCasing.DeepCopy(!temporary, true);
			ChamberedCasing.ContainedIn = newParent;
		}
    }

    protected override void LoadFromXml(XElement root)
    {
        base.LoadFromXml(root);
        if (!RestoreLoadedChild(ChamberedRound?.Parent)) ChamberedRound = null;
        foreach (XElement sub in root.Element("RoundsInMagazine").Elements())
        {
            IGameItem item = Gameworld.TryGetItem(long.Parse(sub.Value), true);
            if (RestoreLoadedChild(item))
            {
                _roundsInMagazine.Add(item);
            }
        }

        ChamberedCasing = Gameworld.TryGetItem(long.Parse(root.Element("ChamberedCasing")?.Value ?? "0"), true);
        if (!RestoreLoadedChild(ChamberedCasing)) ChamberedCasing = null;
    }

    private bool RestoreLoadedChild(IGameItem item)
    {
        if (item is null || item.Deleted || item.Destroyed) return false;
        var alreadyContained = ReferenceEquals(item.ContainedIn, Parent);
        if (!alreadyContained && item is GameItem native && native.LoadedFromDatabase &&
            native.ContainerIdAtLoad != Parent.Id) return false;
        if (alreadyContained ? !ComponentUnloadCompletion.OwnedBy(item, Parent) : !ComponentItemTransfer.IsDetached(item))
            return false;
        item.LoadTimeSetContainedIn(Parent);
        return true;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new InternalMagazineGunGameItemComponent(this, newParent, temporary);
    }

    #endregion

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("RoundsInMagazine",
                from item in _roundsInMagazine
                select new XElement("Round", item.Id)
            ),
            new XElement("ChamberedRound", ChamberedRound?.Parent.Id ?? 0),
            new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
            new XElement("Safety", Safety ? "true" : "false"),
            new XElement("ChamberedCasing", ChamberedCasing?.Id ?? 0),
            SaveFirearmState()
        ).ToString();
    }

    #endregion

    #region IRangedWeapon Implementation

    public IGameItem ChamberedCasing { get; set; } //The casing waiting to be ejected for when you ready

    private readonly List<IGameItem> _roundsInMagazine = new();

    public override IEnumerable<IGameItem> MagazineContents => _roundsInMagazine;
    public override IEnumerable<IGameItem> AllContainedItems => MagazineContents.Concat([ChamberedCasing, ChamberedRound?.Parent]).SelectNotNull(x => x);

    public override bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return false;


        if (!ItemManipulationGuard.CanManipulate(loader, out var manipulationReason, Parent))
        {
            return false;
        }

        if (_roundsInMagazine.Sum(x => x.Quantity) >= _prototype.InternalMagazineCapacity)
        {
            return false;
        }

        IInventoryPlan plan = ignoreEmpty
            ? _prototype.LoadTemplateIgnoreEmpty.CreatePlan(loader)
            : _prototype.LoadTemplate.CreatePlan(loader);
        if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
        {
            return false;
        }

        return true;
    }

    public override string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return string.Empty;


        if (!ItemManipulationGuard.CanManipulate(loader, out var manipulationReason, Parent))
        {
            return manipulationReason;
        }

        if (_roundsInMagazine.Sum(x => x.Quantity) >= _prototype.InternalMagazineCapacity)
        {
            return
                $"There is no more space in the magazine of {Parent.HowSeen(loader)}.";
        }

        IInventoryPlan plan = ignoreEmpty
            ? _prototype.LoadTemplateIgnoreEmpty.CreatePlan(loader)
            : _prototype.LoadTemplate.CreatePlan(loader);
        if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
        {
            switch (plan.PlanIsFeasible())
            {
                case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
                case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
                    return
                        $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
                case InventoryPlanFeasibility.NotFeasibleMissingItems:
                    return $"You don't have any suitable rounds of ammunition to load {Parent.HowSeen(loader)}.";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        throw new ApplicationException(
            "Unknown WhyCannotLoad reason in InternalMagazineGunGameItemComponent.WhyCannotLoad");
    }

    protected override bool ChamberRound(ICharacter loader)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return false;

        var accepted = false;
        bool AcceptStep()
        {
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return false;
            MudSharp.NPC.AI.CommandExecutionScope.MarkCommitted(loader);
            accepted = true;
            Changed = true;
            return true;
        }

        if (ChamberedRound is { } oldRound)
        {
            loader.OutputHandler.Handle(new EmoteOutput(
                new Emote("$1 is ejected from $0 by the action.", loader, Parent,
                    oldRound.Parent), flags: OutputFlags.Insigificant));
            if (!ReferenceEquals(ChamberedRound, oldRound) || !AcceptStep()) return accepted;
            ChamberedRound = null;
            oldRound.Parent.ContainedIn = null;
            if (ComponentItemTransfer.IsDetached(oldRound.Parent))
            {
                oldRound.Parent.RoomLayer = loader.RoomLayer;
                if (ComponentItemTransfer.IsDetached(oldRound.Parent)) oldRound.Parent.InsertAtSource(loader);
            }
            if (ChamberedRound is not null) return accepted;
        }

        if (ChamberedCasing is { } oldCasing)
        {
            loader.OutputHandler.Handle(new EmoteOutput(
                new Emote("@ tumble|tumbles to the ground.", oldCasing), flags: OutputFlags.Insigificant));
            if (!ReferenceEquals(ChamberedCasing, oldCasing) || !AcceptStep()) return accepted;
            ChamberedCasing = null;
            oldCasing.ContainedIn = null;
            if (ComponentItemTransfer.IsDetached(oldCasing))
            {
                oldCasing.RoomLayer = loader.RoomLayer;
                if (ComponentItemTransfer.IsDetached(oldCasing)) oldCasing.InsertAtSource(loader);
            }
            if (ChamberedRound is not null || ChamberedCasing is not null) return accepted;
        }

        if (!AcceptStep()) return accepted;
        if (_roundsInMagazine.Any())
        {
            IGameItem first = _roundsInMagazine.First();
            if (first.Quantity > 1)
            {
                ChamberedRound = first.Get(null, 1).GetItemType<IAmmo>();
            }
            else
            {
                ChamberedRound = first.GetItemType<IAmmo>();
                _roundsInMagazine.Remove(first);
            }
        }
        else
        {
            ChamberedRound = null;
            loader.HandleEvent(EventType.ReadyGunEmpty, loader, this);
        }

        Changed = true;
        return accepted;
    }

    public override void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return;


        if (!ItemManipulationGuard.CanManipulate(loader, out var manipulationReason, Parent))
        {
            loader?.OutputHandler.Send(manipulationReason);
            return;
        }

        if (!CanLoad(loader))
        {
            loader.Send(WhyCannotLoad(loader));
            return;
        }

        IInventoryPlan plan = ignoreEmpty
            ? _prototype.LoadTemplateIgnoreEmpty.CreatePlan(loader)
            : _prototype.LoadTemplate.CreatePlan(loader);

        IEnumerable<InventoryPlanActionResult> results = plan.ExecuteWholePlan();
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return;
        var loadResult = results.FirstOrDefault(x => x.OriginalReference is string reference && reference == "loaditem");
        IAmmo ammo = loadResult?.PrimaryTarget?.GetItemType<IAmmo>();
        // Feasibility/scouting does not guarantee that the plan acquired its target.
        // Taking a still-floor item clears its location without extracting cell membership.
        if (loadResult?.ActionState != DesiredItemState.Held || ammo is null ||
            !loader.Body.HeldItems.Contains(ammo.Parent) ||
            !ReferenceEquals(ammo.Parent.GetItemType<IHoldable>()?.HeldBy, loader.Body) ||
            ammo.Parent.ContainedIn is not null || ComponentItemTransfer.DirectLocationOf(ammo.Parent) is not null)
        {
            plan.FinalisePlan();
            return;
        }
        var source = ammo.Parent;
        var sourceAmmo = ammo;
        var sourceQuantity = source.Quantity;
        var title = source.OwnershipReference;
        var sourceHolder = source.GetItemType<IHoldable>();
        var sourceStack = source.GetItemType<IStackable>();
        var ammoPrototype = sourceAmmo.Prototype;
        var holderPrototype = sourceHolder?.Prototype;
        var stackPrototype = sourceStack?.Prototype;
        var ammoClass = sourceAmmo.GetType();
        var holderClass = sourceHolder?.GetType();
        var stackClass = sourceStack?.GetType();
        var receiver = loader.Body;
        var receiverActor = receiver.Actor;
        var prototype = _prototype;
        var capacity = prototype.InternalMagazineCapacity;
        var gunComponent = Parent.GetItemType<InternalMagazineGunGameItemComponent>();
        var gunBody = Parent.InInventoryOf;
        var gunContainer = Parent.ContainedIn;
        var gunCell = Parent.Location;
        var gunLayer = Parent.RoomLayer;
        var gunPosition = Parent.RoutePositionMetres;
        var gunTitle = Parent.OwnershipReference;
        var chamber = ChamberedRound;
        var casing = ChamberedCasing;
        var magazine = _roundsInMagazine.Select(x => (Item: x, Quantity: x.Quantity, Title: x.OwnershipReference,
            Ammo: x.GetItemType<IAmmo>(), Stack: x.GetItemType<IStackable>(),
            AmmoPrototype: x.GetItemType<IAmmo>()?.Prototype, StackPrototype: x.GetItemType<IStackable>()?.Prototype)).ToArray();
        bool MagazineUnchanged() => !Parent.Deleted && !Parent.Destroyed && ReferenceEquals(_prototype, prototype) &&
            prototype.InternalMagazineCapacity == capacity && ReferenceEquals(Parent.GetItemType<InternalMagazineGunGameItemComponent>(), gunComponent) &&
            ReferenceEquals(Parent.InInventoryOf, gunBody) && ReferenceEquals(Parent.ContainedIn, gunContainer) &&
            ReferenceEquals(Parent.Location, gunCell) && Parent.RoomLayer == gunLayer &&
            Parent.RoutePositionMetres == gunPosition && Parent.OwnershipReference == gunTitle &&
            ReferenceEquals(loader.Body, receiver) && ReferenceEquals(receiver.Actor, receiverActor) && ReferenceEquals(ChamberedRound, chamber) &&
            ReferenceEquals(ChamberedCasing, casing) && _roundsInMagazine.Count == magazine.Length &&
            magazine.Select((x, i) => ReferenceEquals(_roundsInMagazine[i], x.Item) &&
                x.Item.Quantity == x.Quantity && x.Item.OwnershipReference == x.Title &&
                ReferenceEquals(x.Item.GetItemType<IAmmo>(), x.Ammo) && ReferenceEquals(x.Item.GetItemType<IStackable>(), x.Stack) &&
                ReferenceEquals(x.Ammo?.Prototype, x.AmmoPrototype) && ReferenceEquals(x.Stack?.Prototype, x.StackPrototype) &&
                ComponentUnloadCompletion.OwnedBy(x.Item, Parent)).All(x => x);
        bool HeldSource(int quantity) => !source.Deleted && !source.Destroyed && source.Quantity == quantity &&
            source.OwnershipReference == title && ReferenceEquals(source.GetItemType<IAmmo>(), sourceAmmo) &&
            ReferenceEquals(sourceAmmo.Prototype, ammoPrototype) && ReferenceEquals(sourceHolder?.Prototype, holderPrototype) &&
            ReferenceEquals(sourceStack?.Prototype, stackPrototype) &&
            ReferenceEquals(source.GetItemType<IHoldable>(), sourceHolder) && ReferenceEquals(source.GetItemType<IStackable>(), sourceStack) &&
            ReferenceEquals(sourceHolder?.HeldBy, receiver) && receiver.HeldItems.Any(x => ReferenceEquals(x, source)) &&
            source.ContainedIn is null && ComponentItemTransfer.DirectLocationOf(source) is null;
        var space = capacity - magazine.Sum(x => x.Quantity);
        if (sourceQuantity <= 0 || space <= 0)
        {
            plan.FinalisePlan();
            return;
        }
        var floor = ComponentUnloadCompletion.PrepareFloorDestination(loader, loader);
        if (floor is null || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader) || !MagazineUnchanged() || !HeldSource(sourceQuantity))
        {
            plan.FinalisePlan();
            return;
        }
        var whole = sourceQuantity <= space;
        var quantity = Math.Min(sourceQuantity, space);
        var participant = source;
        var participantAmmo = sourceAmmo;
        var participantHolder = sourceHolder;
        var participantStack = sourceStack;
        bool ExactParticipant() => MagazineUnchanged() && participant.Quantity == quantity &&
            participant.OwnershipReference == title && ReferenceEquals(participant.GetItemType<IAmmo>(), participantAmmo) &&
            ReferenceEquals(participant.GetItemType<IHoldable>(), participantHolder) &&
            ReferenceEquals(participant.GetItemType<IStackable>(), participantStack) &&
            ReferenceEquals(participantAmmo.Prototype, ammoPrototype) && ReferenceEquals(participantHolder?.Prototype, holderPrototype) &&
            ReferenceEquals(participantStack?.Prototype, stackPrototype) &&
            (whole || HeldSource(sourceQuantity - quantity));
        bool Ready() => MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader) &&
            ComponentItemTransfer.IsDetached(participant) && ExactParticipant();
        static bool MatchesClone(IGameItemComponent actual, Type originalClass, IGameItemComponentProto originalPrototype) => originalClass is null
            ? actual is null : actual is not null && actual.GetType() == originalClass && ReferenceEquals(actual.Prototype, originalPrototype);
        List<IGameItem> exemptions = new();
        if (whole) exemptions.Add(source);
        var mergeStarted = false;
        try
        {
            if (whole) receiver.Take(source);
            else
            {
                participant = source.Get(null, quantity);
                var splitAmmo = participant.GetItemType<IAmmo>();
                var splitHolder = participant.GetItemType<IHoldable>();
                var splitStack = participant.GetItemType<IStackable>();
                if (!MatchesClone(splitAmmo, ammoClass, ammoPrototype) || !MatchesClone(splitHolder, holderClass, holderPrototype) ||
                    !MatchesClone(splitStack, stackClass, stackPrototype)) return;
                participantAmmo = splitAmmo; participantHolder = splitHolder; participantStack = splitStack;
            }
            if (!Ready()) return;
            loader.OutputHandler.Handle(new EmoteOutput(
                new Emote(prototype.LoadEmote, loader, loader, Parent, participant), flags: OutputFlags.InnerWrap));
            if (!Ready()) return;
            IGameItem mergeTarget = null;
            foreach (var candidate in magazine)
            {
                var matches = candidate.Item.CanMerge(participant);
                if (!Ready()) return;
                if (matches) { mergeTarget = candidate.Item; break; }
            }
            if (mergeTarget is not null)
            {
                // Composite ammunition merge cleanup retains its separate qualification gate.
                mergeStarted = true;
                mergeTarget.Merge(participant);
                participant.Delete();
                Changed = true;
            }
            else
            {
                ComponentItemTransfer.ContainPrepared(participant, Parent, ExactParticipant,
                    () => { _roundsInMagazine.Add(participant); Changed = true; },
                    () => { _roundsInMagazine.RemoveAll(x => ReferenceEquals(x, participant)); Changed = true; });
            }
        }
        finally
        {
            try { plan.FinalisePlanWithExemptions(exemptions); }
            finally { if (!mergeStarted && ComponentItemTransfer.IsDetached(participant)) floor(participant); }
        }
    }

    public override bool CanUnload(ICharacter loader)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return false;


        if (!ItemManipulationGuard.CanManipulate(loader, out var manipulationReason, Parent))
        {
            return false;
        }

        return _roundsInMagazine.Any();
    }

    public override string WhyCannotUnload(ICharacter loader)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return string.Empty;


        if (!ItemManipulationGuard.CanManipulate(loader, out var manipulationReason, Parent))
        {
            return manipulationReason;
        }

        if (!_roundsInMagazine.Any())
        {
            return $"{Parent.HowSeen(loader, true)} is already unloaded.";
        }

        throw new ApplicationException("Unknown reason in GunGameItemComponent.WhyCannotUnload");
    }

    public override IEnumerable<IGameItem> Unload(ICharacter loader)
    {
		using var execution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
		if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return [];
		var canUnload = CanUnload(loader);
		if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return [];
		if (!canUnload)
		{
			var reason = WhyCannotUnload(loader);
			if (MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) loader.Send(reason);
			return [];
		}
		var snapshot = _roundsInMagazine.ToArray();
		var results = new List<IGameItem>();
		var announced = false;
		foreach (var item in snapshot)
		{
			if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) break;
			if (!_roundsInMagazine.Any(x => ReferenceEquals(x, item)) || !ComponentUnloadCompletion.OwnedBy(item, Parent)) continue;
			var completion = ComponentUnloadCompletion.PrepareReceive(loader, item);
			if (completion is null || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) break;
			if (!announced)
			{
				var perceivable = snapshot.Length > 1 ? (IPerceivable)new PerceivableGroup(snapshot) : item;
				loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnloadEmote, loader, loader, Parent, perceivable)));
				announced = true;
			}
			if (!_roundsInMagazine.Any(x => ReferenceEquals(x, item)) || !ComponentUnloadCompletion.Detach(loader, item, Parent,
				() => _roundsInMagazine.RemoveAll(x => ReferenceEquals(x, item)),
				() => _roundsInMagazine.Any(x => ReferenceEquals(x, item)))) break;
			Changed = true;
			results.Add(item);
			completion();
		}
		return results;
    }

    public override bool CanFire(ICharacter actor, IPerceivable target)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return false;


        if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent))
        {
            return false;
        }

        return true;
    }

    public override string WhyCannotFire(ICharacter actor, IPerceivable target)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return string.Empty;


        if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent))
        {
            return manipulationReason;
        }

        throw new ApplicationException("Guns should always be able to fire.");
    }

    #region Overrides of FirearmBaseGameItemComponent

    /// <inheritdoc />
    protected override void HandleShellCasingOnFire(ICharacter actor, SpatialLocation originalLocation, IGameItem casing)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return;


        if (casing != null)
        {
            if (_prototype.EjectOnFire)
            {
                originalLocation.Cell.Handle(new EmoteOutput(new Emote("@ tumble|tumbles to the ground.", casing),
                    flags: OutputFlags.Insigificant));
				casing.InsertAtSpatialLocation(originalLocation);
            }
            else
            {
                ChamberedCasing = casing;
                casing.ContainedIn = Parent;
            }
        }
    }

    #endregion

    public override bool IsLoaded => _roundsInMagazine.Any();

    public override bool Unready(ICharacter readier)
    {
        using var execution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(readier);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(readier) || !CanUnready(readier)) return false;
        var round = ChamberedRound;
        var casing = ChamberedCasing;
        var item = round?.Parent ?? casing;
        if (item is null)
        {
            readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnreadyEmoteNoChamberedRound, readier, readier, Parent)));
            return MudSharp.NPC.AI.CommandExecutionScope.TryContinue(readier);
        }

        bool ExactSlot() => round is not null ? ReferenceEquals(ChamberedRound, round) : ReferenceEquals(ChamberedCasing, casing);
        var completion = ComponentUnloadCompletion.PrepareReceive(readier, item);
        if (completion is null || !ExactSlot() || !ComponentUnloadCompletion.OwnedBy(item, Parent)) return false;
        readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnreadyEmote, readier, readier, Parent, item)));
        if (!ComponentUnloadCompletion.Detach(readier, item, Parent, () =>
            {
                if (!ExactSlot()) return;
                if (round is not null) ChamberedRound = null;
                else ChamberedCasing = null;
                Changed = true;
            }, ExactSlot)) return false;
        completion();
        return true;
    }

    #endregion

    #region IGameItemComponent Overrides

    public override double ComponentWeight =>
        MagazineContents.Sum(x => x.Weight) +
        (ChamberedRound?.Parent.Weight ?? 0.0) +
        (ChamberedCasing?.Weight ?? 0.0) +
        AttachedItemsWeight;


    public override double ComponentBuoyancy(double fluidDensity)
    {
        return MagazineContents.Sum(x => x.Buoyancy(fluidDensity)) +
               (ChamberedRound?.Parent.Buoyancy(fluidDensity) ?? 0.0) +
               (ChamberedCasing?.Buoyancy(fluidDensity) ?? 0.0) +
               AttachedItemsBuoyancy(fluidDensity);
    }

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Full || type == DescriptionType.Evaluate;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        if (type == DescriptionType.Full)
        {
            StringBuilder sb = new();
            sb.AppendLine(description);
            sb.AppendLine();
            sb.AppendLine(_roundsInMagazine.Any()
                ? $"It has {_roundsInMagazine.Select(x => x.HowSeen(voyeur)).ListToString()} in the magazine."
                : "It does not currently have any ammunition in the magazine.");
            sb.AppendLine($"The safety is currently {(Safety ? "on" : "off")}.");
            sb.AppendLine($"It is set to {CurrentFireMode.Type.DescribeEnum()} fire.");
            sb.AppendLine(InstalledAttachments.Any()
                ? $"It has {InstalledAttachments.Select(x => $"{x.Value.Parent.HowSeen(voyeur)} in its {x.Key} slot").ListToString()}."
                : "It has no firearm attachments installed.");
            return sb.ToString();
        }

        if (type == DescriptionType.Evaluate)
        {
            IMeleeWeapon mw = (IMeleeWeapon)this;
            return
                $"This is a {CycleType.DescribeEnum(true)} internal-magazine modular firearm of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt supports {FireModes.Select(x => x.Type.DescribeEnum().ColourName()).ListToString()} fire.\nIt uses the {WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill for firing.\nIt takes ammunition of type {WeaponType.SpecificAmmunitionGrade.Colour(Telnet.Green)}.\n This is also a melee weapon of type {mw.WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {mw.WeaponType.AttackTrait.Name.Colour(Telnet.Green)} skill for attack and {(mw.WeaponType.ParryTrait == mw.WeaponType.AttackTrait ? "defense" : $"the {mw.WeaponType.ParryTrait.Name.Colour(Telnet.Green)} skill for defense")}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
        }

        return base.Decorate(voyeur, name, description, type, colour, flags);
    }

    public override void Quit()
    {
        base.Quit();
        ChamberedRound?.Parent.Quit();
        ChamberedCasing?.Quit();
        foreach (IGameItem item in _roundsInMagazine.ToList())
        {
            item.Quit();
        }
    }

    public override void Delete()
    {
        base.Delete();
        ChamberedRound?.Parent.ContainedIn = null;
        ChamberedRound?.Parent.Delete();

        ChamberedCasing?.ContainedIn = null;
        ChamberedCasing?.Delete();


        foreach (IGameItem item in _roundsInMagazine.ToList())
        {
            item.ContainedIn = null;
            item.Delete();
        }
    }

    public override void Login()
    {
        ChamberedRound?.Parent.Login();
        ChamberedCasing?.Login();
        foreach (IGameItem item in _roundsInMagazine.ToList())
        {
            item.Login();
        }
    }

    #endregion

    #region Overrides of FirearmBaseGameItemComponent

    /// <inheritdoc />
    #endregion
}
