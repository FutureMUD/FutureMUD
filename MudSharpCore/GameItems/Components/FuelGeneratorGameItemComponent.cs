using MudSharp.GameItems;
#nullable enable annotations

using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class FuelGeneratorGameItemComponent : GameItemComponent, IProducePower, ISwitchable, ILiquidContainer,
    ILockable, IOnOff
{
    private readonly List<IConsumePower> _connectedConsumers = new();
    private readonly List<IConsumePower> _powerUsers = new();
    private FuelGeneratorGameItemComponentProto _prototype;
    public bool CanBeEmptiedWhenInRoom => true;
    private LiquidMixture _liquidMixture;

    public LiquidMixture LiquidMixture
    {
        get => _liquidMixture;
        set
        {
            _liquidMixture = value;
            Changed = true;
            CheckOn();
        }
    }

    private void AdjustLiquidQuantity(double amount, ICharacter who, string action)
    {
        if (LiquidMixture == null)
        {
            return;
        }

        LiquidMixture.AddLiquidVolume(amount);
        if (LiquidMixture.IsEmpty)
        {
            LiquidMixture = null;
        }

        Changed = true;
    }

    public void AddLiquidQuantity(double amount, ICharacter who, string action)
    {
        if (LiquidMixture == null)
        {
            return;
        }

        if (LiquidMixture.TotalVolume + amount > LiquidCapacity)
        {
            amount = LiquidCapacity - LiquidMixture.TotalVolume;
        }

        if (LiquidMixture.TotalVolume + amount < 0)
        {
            amount = -1 * LiquidMixture.TotalVolume;
        }

        AdjustLiquidQuantity(amount, who, action);
    }

    public void ReduceLiquidQuantity(double amount, ICharacter who, string action)
    {
        if (LiquidMixture == null)
        {
            return;
        }

        if (LiquidMixture.TotalVolume - amount > LiquidCapacity)
        {
            amount = (LiquidCapacity - LiquidMixture.TotalVolume) * -1;
        }

        if (LiquidMixture.TotalVolume - amount < 0)
        {
            amount = LiquidMixture.TotalVolume;
        }

        AdjustLiquidQuantity(amount * -1, who, action);
    }

    public void MergeLiquid(LiquidMixture otherMixture, ICharacter who, string action)
    {
        if (otherMixture == null)
        {
            return;
        }

        if (LiquidMixture == null)
        {
            LiquidMixture = otherMixture;
        }
        else
        {
            LiquidMixture.AddLiquid(otherMixture);
        }

        if (LiquidMixture.IsEmpty)
        {
            LiquidMixture = null;
        }

        Changed = true;
    }

    public LiquidMixture RemoveLiquidAmount(double amount, ICharacter who, string action)
    {
        if (LiquidMixture == null)
        {
            return null;
        }

        LiquidMixture newMixture = LiquidMixture.RemoveLiquidVolume(amount);
        Changed = true;
        if (LiquidMixture.IsEmpty)
        {
            LiquidMixture = null;
        }

        return newMixture;
    }

    public double LiquidVolume => LiquidMixture?.TotalVolume ?? 0.0;
    public double LiquidCapacity => _prototype.FuelCapacity;

    #region IOnOff Implementation

    private bool _switchedOn;

    public bool SwitchedOn
    {
        get => _switchedOn;
        set
        {
            _switchedOn = value;
            Changed = true;
            CheckOn();
        }
    }

    #endregion

    public override IGameItemComponentProto Prototype => _prototype;

    public override void Delete()
    {
        SuspendPower();
        base.Delete();
        foreach (ILock item in Locks.ToList())
        {
            DeleteOwnedContainedChild(item.Parent);
            _locks.Remove(item);
        }
    }

    public override void Quit()
    {
        SuspendPower();
        base.Quit();
        foreach (ILock item in Locks.ToArray())
        {
            item.Parent.Quit();
        }
    }

    public override void Login()
    {
        base.Login();
        _active = true;
        CheckOn();
        foreach (ILock item in Locks.ToArray())
        {
            item.Parent.Login();
        }
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new FuelGeneratorGameItemComponent(this, newParent, temporary);
    }

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return (type == DescriptionType.Contents && IsOpen) || type == DescriptionType.Short ||
               type == DescriptionType.Full || type == DescriptionType.Evaluate;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        switch (type)
        {
            case DescriptionType.Contents:
                if (LiquidMixture?.IsEmpty == false)
                {
                    return string.Format(voyeur, "\n\nIt is approximately {0:P0} full of {1}.",
                        Math.Round(20 * LiquidMixture.TotalVolume / LiquidCapacity) / 20,
                        LiquidMixture.ColouredLiquidDescription);
                }

                return description + "\n\nIt is currently empty.";
            case DescriptionType.Short:
                return $"{description}{(SwitchedOn ? " (on)".FluentColour(Telnet.BoldWhite, colour) : "")}";
            case DescriptionType.Full:
                StringBuilder sb = new();
                sb.Append(description);
                sb.AppendLine();
                sb.AppendLine($"It is{(SwitchedOn ? "" : " not")} switched on.".Colour(Telnet.Yellow));
                sb.AppendLine($"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}."
                    .Colour(Telnet.Yellow));
                if (Locks.Any())
                {
                    sb.AppendLine("It has the following locks:");
                    foreach (ILock thelock in Locks)
                    {
                        sb.AppendLineFormat("\t{0}", thelock.Parent.HowSeen(voyeur));
                    }
                }

                return sb.ToString();
            case DescriptionType.Evaluate:
                return
                    $"{description}\nIt can hold {Gameworld.UnitManager.DescribeMostSignificantExact(LiquidCapacity, Framework.Units.UnitType.FluidVolume, voyeur).ColourValue()} of liquid.";
        }

        throw new NotSupportedException("Invalid Decorate type in FuelGeneratorGameItemComponent.Decorate");
    }

    public override int DecorationPriority => int.MaxValue;

    public override bool PreventsMerging(IGameItemComponent component)
    {
        return true;
    }

    public bool PrimaryLoadTimePowerProducer => true;
    public bool PrimaryExternalConnectionPowerProducer => false;

    public double FuelLevel => (LiquidMixture?.TotalVolume ?? 0.0) / _prototype.FuelCapacity;

    public bool ProducingPower => _active && SwitchedOn && LiquidMixture is { IsEmpty: false } mixture &&
        mixture.CountsAs(_prototype.LiquidFuel).Truth;

    public double MaximumPowerInWatts => ProducingPower
        ? _prototype.WattageProvided * Parent.EffectsOfType<IMagicPowerOrFuelEnhancementEffect>(x =>
            x.AppliesToPoweredItem(Parent)).Aggregate(1.0, (current, effect) => current * effect.PowerProductionMultiplier)
        : 0.0;

    private double _spikeDrawdown;
    internal double RouteCheckpointSpikeDrawdown
    {
        get => _spikeDrawdown;
        set => _spikeDrawdown = value;
    }

    public bool CanBeginDrawDown(double wattage)
    {
        return SwitchedOn && ProducingPower &&
               _powerUsers.Sum(x => x.PowerConsumptionInWatts) + wattage + _spikeDrawdown <
               MaximumPowerInWatts;
    }

    public bool CanDrawdownSpike(double wattage)
    {
        return SwitchedOn &&
               _powerUsers.Sum(x => x.PowerConsumptionInWatts) + wattage + _spikeDrawdown <
               MaximumPowerInWatts && ProducingPower;
    }

    public bool DrawdownSpike(double wattage)
    {
        if (!CanDrawdownSpike(wattage))
        {
            return false;
        }

        _spikeDrawdown += wattage;
        return true;
    }

    public void BeginDrawdown(IConsumePower item)
    {
        if (!_connectedConsumers.Contains(item))
        {
            _connectedConsumers.Add(item);

            if (ProducingPower &&
                _powerUsers.Sum(x => x.PowerConsumptionInWatts) + item.PowerConsumptionInWatts <=
                MaximumPowerInWatts)
            {
                _powerUsers.Add(item);
                item.OnPowerCutIn();
            }
        }
    }

    public void EndDrawdown(IConsumePower item)
    {
        _connectedConsumers.Remove(item);
        if (_powerUsers.Remove(item)) item.OnPowerCutOut();
    }

    public override double ComponentWeight => Locks.Sum(x => x.Parent.Weight) + LiquidMixture?.TotalWeight ?? 0.0;

    public override double ComponentBuoyancy(double fluidDensity)
    {
        return Locks.Sum(x => x.Parent.Buoyancy(fluidDensity)) +
            LiquidMixture?.Instances.Sum(x => (fluidDensity - x.Liquid.Density) * x.Amount * x.Liquid.Density) ?? 0.0;
    }

    public override bool HandleDieOrMorph(IGameItem newItem, IRoom location)
    {
        ILockable newItemLockable = newItem?.GetItemType<ILockable>();
        if (newItemLockable != null)
        {
            foreach (ILock thelock in Locks)
            {
                newItemLockable.InstallLock(thelock);
            }
        }
        else
        {
            foreach (ILock thelock in Locks)
            {
                if (location != null)
                {
                    InsertAtParentSpatialLocation(thelock.Parent, location);
                    thelock.Parent.ContainedIn = null;
                }
                else
                {
                    thelock.Parent.Delete();
                }
            }
        }

        _locks.Clear();

        if (LiquidMixture == null)
        {
            return false;
        }

        ILiquidContainer newItemLiquid = newItem?.GetItemType<ILiquidContainer>();
        if (newItemLiquid != null)
        {
            newItemLiquid.LiquidMixture = LiquidMixture;
            newItemLiquid.Changed = true;
        }

        return false;
    }

    public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
    {
        if (_locks.Any(x => x.Parent == existingItem) && newItem.IsItemType<ILock>())
        {
            _locks[_locks.IndexOf(existingItem.GetItemType<ILock>())] = newItem.GetItemType<ILock>();
            existingItem.ContainedIn = null;
            newItem.ContainedIn = Parent;
            Changed = true;
            return true;
        }

        return false;
    }

    public IEnumerable<string> SwitchSettings => new[] { "on", "off" };

    public bool CanSwitch(ICharacter actor, string setting)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out _, Parent))
		{
			return false;
		}

        // TODO - more reasons why something couldn't be switched on or off
        return (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn) ||
               (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn) ||
               (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) &&
                LiquidMixture?.CountsAs(_prototype.LiquidFuel).Truth == true
               )
            ;
    }

    public string WhyCannotSwitch(ICharacter actor, string setting)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent))
		{
			return manipulationReason;
		}

        if (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn)
        {
            return $"{Parent.HowSeen(actor)} is already on.";
        }

        if (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn)
        {
            return $"{Parent.HowSeen(actor)} is already off.";
        }

        if (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) &&
            LiquidMixture?.CountsAs(_prototype.LiquidFuel).Truth != true)
        {
            return $"{Parent.HowSeen(actor)} cannot be switched on as it is not appropriately fueled.";
        }

        return $"{Parent.HowSeen(actor)} cannot be switched to {setting} at this time.";
    }

    public bool Switch(ICharacter actor, string setting)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent))
		{
			return false;
		}

        if (!CanSwitch(actor, setting))
        {
            return false;
        }

        return setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase)
            ? SwitchOn(actor)
            : SwitchOff(actor);
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (FuelGeneratorGameItemComponentProto)newProto;
        if (LiquidMixture != null && LiquidMixture.TotalVolume > LiquidCapacity)
        {
            LiquidMixture.SetLiquidVolume(LiquidCapacity);
        }
    }

    private bool SwitchOn(ICharacter actor)
    {
        SwitchedOn = true;
        _prototype.SwitchOnProg?.Execute(actor, Parent);
        return true;
    }

    private bool _active;
    private bool _heartbeatOn;

    private void SuspendPower()
    {
        _active = false;
        CheckOn();
    }

    private void CheckOn()
    {
        if (ProducingPower)
        {
            if (!_heartbeatOn)
            {
                Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManager_SecondHeartbeat;
                _heartbeatOn = true;
            }

            foreach (var item in _connectedConsumers.ToArray())
            {
                if (!ProducingPower) break;
                if (!_connectedConsumers.Contains(item) || _powerUsers.Contains(item) ||
                    _powerUsers.Sum(x => x.PowerConsumptionInWatts) + item.PowerConsumptionInWatts + _spikeDrawdown > MaximumPowerInWatts) continue;
                _powerUsers.Add(item);
                item.OnPowerCutIn();
            }
            return;
        }

        if (_heartbeatOn)
        {
            Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
            _heartbeatOn = false;
        }

        _spikeDrawdown = 0.0;
        var powered = _powerUsers.ToArray();
        _powerUsers.Clear();
        foreach (var item in powered) item.OnPowerCutOut();
    }

    private void HeartbeatManager_SecondHeartbeat()
    {
        if (!_heartbeatOn || !ProducingPower) return;
        var fuelMultiplier = Parent.EffectsOfType<IMagicPowerOrFuelEnhancementEffect>(x => x.AppliesToPoweredItem(Parent))
                                   .Aggregate(1.0, (current, effect) => current * effect.FuelUseMultiplier);
        LiquidMixture.RemoveLiquidVolume(_prototype.FuelPerSecond * fuelMultiplier);
        Changed = true;
        _spikeDrawdown = 0.0;
        if (LiquidMixture.IsEmpty)
        {
            LiquidMixture = null;
            Parent.Handle(new EmoteOutput(new Emote(_prototype.FuelExpendedEmote, Parent, Parent)));
            _prototype.FuelOutProg?.Execute(Parent);
            return;
        }

        CheckOn();
    }

    private bool SwitchOff(ICharacter actor)
    {
        _prototype.SwitchOffProg?.Execute(actor, Parent);
        SwitchedOn = false;
        return true;
    }

    #region Constructors

    public FuelGeneratorGameItemComponent(FuelGeneratorGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _liquidMixture = rhs.LiquidMixture?.Clone();
        _switchedOn = rhs.SwitchedOn;
        _isOpen = rhs.IsOpen;
    }

    public FuelGeneratorGameItemComponent(FuelGeneratorGameItemComponentProto proto, IGameItem parent,
        bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public FuelGeneratorGameItemComponent(MudSharp.Models.GameItemComponent component,
        FuelGeneratorGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    protected override string SaveToXml()
    {
        return
            new XElement("Definition",
                    new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
                    new XAttribute("On", SwitchedOn),
                    LiquidMixture?.SaveToXml() ?? new XElement("NoLiquid"),
                    new XElement("Locks", Locks.Select(itemLock => new XElement("Lock", itemLock.Parent.Id)),
                        AmbiguousLegacyChildClaims("Lock")))
                .ToString();
    }

    protected void LoadFromXml(XElement root)
    {
        XAttribute attr = root.Attribute("Open");
        if (attr != null)
        {
            _isOpen = bool.Parse(attr.Value);
        }

        attr = root.Attribute("On");
        if (attr != null)
        {
            _switchedOn = bool.Parse(attr.Value);
        }

        foreach (var reference in root.Element("Locks")?.Elements("Lock") ?? [])
        {
            if (RestoreOwnedChild(reference)?.GetItemType<ILock>() is { } itemLock) InstallLock(itemLock);
        }

        // Legacy
        attr = root.Attribute("Liquid");
        if (attr != null)
        {
            ILiquid liquid = Gameworld.Liquids.Get(long.Parse(attr.Value));
            if (liquid != null)
            {
                LiquidMixture = new LiquidMixture(new[]
                {
                    new LiquidInstance
                    {
                        Liquid = liquid,
                        Amount = double.Parse(root.Attribute("LiquidQuantity").Value)
                    }
                }, Gameworld);
            }
        }
        // Non-Legacy
        else
        {
            if (root.Element("Mix") != null)
            {
                LiquidMixture = new LiquidMixture(root.Element("Mix"), Gameworld);
            }
        }
    }

    public override void FinaliseLoad()
    {
        foreach (ILock item in Locks.ToArray())
        {
            item.Parent.FinaliseLoadTimeTasks();
        }

    }

    #endregion

    #region IOpenable Members

    private bool _isOpen = true;

    public bool IsOpen
    {
        get => _isOpen;
        protected set
        {
            _isOpen = value;
            Changed = true;
        }
    }

    public bool CanOpen(IBody opener)
    {
        return !IsOpen && Locks.All(x => !x.IsLocked) &&
               Parent.EffectsOfType<IOverrideLockEffect>().All(x => !x.Applies(opener?.Actor));
    }

    public WhyCannotOpenReason WhyCannotOpen(IBody opener)
    {
        if (IsOpen)
        {
            return WhyCannotOpenReason.AlreadyOpen;
        }

        return Locks.Any(x => x.IsLocked) ||
               Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies(opener?.Actor))
            ? WhyCannotOpenReason.Locked
            : WhyCannotOpenReason.Unknown;
    }

    public void Open()
    {
        IsOpen = true;
        OnOpen?.Invoke(this);
    }

    public bool CanClose(IBody closer)
    {
        return IsOpen;
    }

    public WhyCannotCloseReason WhyCannotClose(IBody closer)
    {
        return !IsOpen ? WhyCannotCloseReason.AlreadyClosed : WhyCannotCloseReason.Unknown;
    }

    public void Close()
    {
        IsOpen = false;
        OnClose?.Invoke(this);
    }

    public event OpenableEvent OnOpen;
    public event OpenableEvent OnClose;

    #endregion

    #region ILockable Members

    private readonly List<ILock> _locks = new();
    public IEnumerable<ILock> Locks => _locks;

    public bool InstallLock(ILock theLock, ICharacter? actor = null)
    {
        _locks.Add(theLock);
        if (_noSave)
        {
            theLock.Parent.LoadTimeSetContainedIn(Parent);
        }
        else
        {
            theLock.Parent.ContainedIn = Parent;
        }

        Changed = true;
        return true;
    }

    public bool RemoveLock(ILock theLock)
    {
        if (_locks.Contains(theLock))
        {
            theLock.Parent.ContainedIn = null;
            _locks.Remove(theLock);
            Changed = true;
            return true;
        }

        return false;
    }

    #endregion
}
