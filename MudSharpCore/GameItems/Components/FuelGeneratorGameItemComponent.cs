using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

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
			if (_liquidMixture != null && _liquidMixture.CountsAs(_prototype.LiquidFuel).Truth && SwitchedOn)
			{
				CheckOn();
			}
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

		var newMixture = LiquidMixture.RemoveLiquidVolume(amount);
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
			if (value)
			{
				CheckOn();
			}
			else
			{
				foreach (var item in _powerUsers.ToList())
				{
					item.OnPowerCutOut();
				}

				_spikeDrawdown = 0.0;
				_powerUsers.Clear();
				Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
				_heartbeatOn = false;
			}
		}
	}

	#endregion

	public override IGameItemComponentProto Prototype => _prototype;

	public override void Delete()
	{
		base.Delete();
		foreach (var item in Locks.ToList())
		{
			_locks.Remove(item);
			item.Parent.Delete();
		}
	}

	public override void Quit()
	{
		base.Quit();
		foreach (var item in Locks)
		{
			item.Quit();
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
				var sb = new StringBuilder();
				sb.Append(description);
				sb.AppendLine();
				sb.AppendLine($"It is{(SwitchedOn ? "" : " not")} switched on.".Colour(Telnet.Yellow));
				sb.AppendLine($"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}."
					.Colour(Telnet.Yellow));
				if (Locks.Any())
				{
					sb.AppendLine("It has the following locks:");
					foreach (var thelock in Locks)
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

	public bool ProducingPower => SwitchedOn && LiquidMixture?.CountsAs(_prototype.LiquidFuel).Truth == true;

	public double MaximumPowerInWatts => ProducingPower ? _prototype.WattageProvided : 0.0;

	private double _spikeDrawdown;

	public bool CanBeginDrawDown(double wattage)
	{
		return SwitchedOn && ProducingPower &&
		       _powerUsers.Sum(x => x.PowerConsumptionInWatts) + wattage + _spikeDrawdown <
		       _prototype.WattageProvided;
	}

	public bool CanDrawdownSpike(double wattage)
	{
		return SwitchedOn &&
		       _powerUsers.Sum(x => x.PowerConsumptionInWatts) + wattage + _spikeDrawdown <
		       _prototype.WattageProvided && ProducingPower;
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

			if (SwitchedOn &&
			    _powerUsers.Sum(x => x.PowerConsumptionInWatts) + item.PowerConsumptionInWatts <=
			    _prototype.WattageProvided)
			{
				_powerUsers.Add(item);
				item.OnPowerCutIn();
			}
		}
	}

	public void EndDrawdown(IConsumePower item)
	{
		_connectedConsumers.Remove(item);
		if (_powerUsers.Contains(item))
		{
			item.OnPowerCutOut();
		}

		_powerUsers.Remove(item);
	}

	public override double ComponentWeight
	{
		get { return Locks.Sum(x => x.Parent.Weight) + LiquidMixture?.TotalWeight ?? 0.0; }
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return Locks.Sum(x => x.Parent.Buoyancy(fluidDensity)) +
			LiquidMixture?.Instances.Sum(x => (fluidDensity - x.Liquid.Density) * x.Amount * x.Liquid.Density) ?? 0.0;
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemLockable = newItem?.GetItemType<ILockable>();
		if (newItemLockable != null)
		{
			foreach (var thelock in Locks)
			{
				newItemLockable.InstallLock(thelock);
			}
		}
		else
		{
			foreach (var thelock in Locks)
			{
				if (location != null)
				{
					location.Insert(thelock.Parent);
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

		var newItemLiquid = newItem?.GetItemType<ILiquidContainer>();
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

	private bool _heartbeatOn;

	private void CheckOn()
	{
		if (!_heartbeatOn && SwitchedOn && LiquidMixture?.CountsAs(_prototype.LiquidFuel).Truth == true)
		{
			_spikeDrawdown = 0.0;
			var cumulativeDraw = 0.0;
			foreach (var item in _connectedConsumers)
			{
				if (_prototype.WattageProvided - cumulativeDraw >= item.PowerConsumptionInWatts)
				{
					_powerUsers.Add(item);
					item.OnPowerCutIn();
					cumulativeDraw += item.PowerConsumptionInWatts;
				}
			}

			Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManager_SecondHeartbeat;
			_heartbeatOn = true;
		}
	}

	private void HeartbeatManager_SecondHeartbeat()
	{
		LiquidMixture?.RemoveLiquidVolume(_prototype.FuelPerSecond);
		_spikeDrawdown = 0.0;
		if ((LiquidMixture?.TotalWeight ?? 0.0) <= 0)
		{
			LiquidMixture = null;
			Parent.Handle(new EmoteOutput(new Emote(_prototype.FuelExpendedEmote, Parent, Parent)));
			_prototype.FuelOutProg?.Execute(Parent);
			Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
			_heartbeatOn = false;
			Changed = true;
			foreach (var item in _powerUsers)
			{
				item.OnPowerCutOut();
			}

			_powerUsers.Clear();
		}
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
		SwitchedOn = rhs.SwitchedOn;
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
					new XElement("Locks", from thelock in Locks select new XElement("Lock", thelock.Parent.Id)))
				.ToString();
	}

	protected void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("Open");
		if (attr != null)
		{
			_isOpen = bool.Parse(attr.Value);
		}

		attr = root.Attribute("On");
		if (attr != null)
		{
			SwitchedOn = bool.Parse(attr.Value);
		}

		var lockelem = root.Element("Locks");
		if (lockelem != null)
		{
			foreach (
				var item in
				lockelem.Elements("Lock")
				        .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
				        .Where(item => item != null && item.IsItemType<ILock>()))
			{
				if ((item.ContainedIn != null && item.ContainedIn != Parent) || item.Location != null ||
				    item.InInventoryOf != null)
				{
					Changed = true;
					Gameworld.SystemMessage(
						$"Duplicated Item: {item.HowSeen(item, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {item.Id.ToString("N0")}",
						true);
					continue;
				}

				item.Get(null);
				InstallLock(item.GetItemType<ILock>());
			}
		}

		// Legacy
		attr = root.Attribute("Liquid");
		if (attr != null)
		{
			var liquid = Gameworld.Liquids.Get(long.Parse(attr.Value));
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
		foreach (var item in Locks)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}

		if (SwitchedOn)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManager_SecondHeartbeat;
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

	public bool InstallLock(ILock theLock, ICharacter actor = null)
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