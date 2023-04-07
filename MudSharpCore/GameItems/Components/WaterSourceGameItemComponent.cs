using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class WaterSourceGameItemComponent : GameItemComponent, ILiquidContainer, ILockable, IOnOff, ISwitchable
{
	protected WaterSourceGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	#region Constructors

	public WaterSourceGameItemComponent(WaterSourceGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		LiquidMixture = new LiquidMixture(_prototype.DefaultLiquid, _prototype.LiquidCapacity, Gameworld);
		SwitchedOn = true;
	}

	public WaterSourceGameItemComponent(Models.GameItemComponent component, WaterSourceGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public WaterSourceGameItemComponent(WaterSourceGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_isOpen = rhs._isOpen;
		if (rhs.LiquidMixture != null)
		{
			LiquidMixture = new LiquidMixture(rhs.LiquidMixture);
		}

		_switchedOn = rhs._switchedOn;
	}

	protected void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("Open");
		_isOpen = bool.Parse(attr?.Value ?? "true");

		var lockelem = root.Element("Locks");
		if (lockelem != null)
		{
			foreach (
				var item in
				lockelem.Elements("Lock")
				        .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
				        .Where(item => item != null && item.IsItemType<ILock>()))
			{
				item.Get(null);
				InstallLock(item.GetItemType<ILock>());
			}
		}

		if (root.Element("Mix") != null)
		{
			LiquidMixture = new LiquidMixture(root.Element("Mix"), Gameworld);
		}

		_switchedOn = bool.Parse(root.Attribute("On")?.Value ?? "true");
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new WaterSourceGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
					new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
					new XAttribute("On", SwitchedOn.ToString().ToLowerInvariant()),
					LiquidMixture?.SaveToXml() ?? new XElement("NoLiquid"),
					new XElement("Locks",
						from thelock in Locks select new XElement("Lock", thelock.Parent.Id)
					)
				)
				.ToString();
	}

	#endregion

	#region ILiquidContainer

	private LiquidMixture _liquidMixture;

	public LiquidMixture LiquidMixture
	{
		get => _liquidMixture;
		set
		{
			_liquidMixture = value;
			Changed = true;
			CheckHeartbeat();
		}
	}

	public bool CanBeEmptiedWhenInRoom => _prototype.CanBeEmptiedWhenInRoom;
	private bool _liquidHeartbeatSubscribed;

	private void LiquidHeartbeat()
	{
		CheckHeartbeat();
		if (!_liquidHeartbeatSubscribed)
		{
			return;
		}

		if (LiquidMixture == null)
		{
			LiquidMixture = new LiquidMixture(_prototype.DefaultLiquid, _prototype.RefillRate, Gameworld);
			Changed = true;
			return;
		}

		var amount = Math.Min(_prototype.LiquidCapacity - LiquidMixture.TotalVolume, _prototype.RefillRate);
		LiquidMixture.AddLiquid(new LiquidMixture(_prototype.DefaultLiquid, amount, Gameworld));
		Changed = true;
		CheckHeartbeat();
	}

	private void CheckOnOff()
	{
		if (_prototype.UseOnOffForRefill)
		{
			return;
		}

		SwitchedOn = _prototype.RefillingProg?.Execute<bool?>(Parent) ?? true;
	}

	private void CheckHeartbeat()
	{
		CheckOnOff();

		if (_liquidHeartbeatSubscribed)
		{
			if ((LiquidMixture?.TotalVolume ?? 0.0) >= _prototype.LiquidCapacity || !SwitchedOn)
			{
				EndHeartbeat();
				return;
			}
		}

		if ((LiquidMixture?.TotalVolume ?? 0.0) < _prototype.LiquidCapacity && SwitchedOn)
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += LiquidHeartbeat;
			_liquidHeartbeatSubscribed = true;
		}
	}

	private void EndHeartbeat()
	{
		if (!_liquidHeartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= LiquidHeartbeat;
		_liquidHeartbeatSubscribed = false;
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
		CheckHeartbeat();
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
		CheckHeartbeat();
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
		CheckHeartbeat();
	}

	public void MergeLiquid(LiquidMixture otherMixture, ICharacter who, string action)
	{
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
		CheckHeartbeat();
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

		CheckHeartbeat();
		return newMixture;
	}

	public double LiquidVolume => LiquidMixture?.TotalVolume ?? 0.0;

	public double LiquidCapacity => _prototype.LiquidCapacity;

	#endregion

	public override int DecorationPriority => -1;

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
		foreach (var item in Locks)
		{
			item.Quit();
		}

		EndHeartbeat();
	}

	public override void Login()
	{
		foreach (var item in Locks)
		{
			item.Login();
		}

		CheckHeartbeat();
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return ((IsOpen || _prototype.Transparent) &&
		        (type == DescriptionType.Contents || type == DescriptionType.Short)) ||
		       type == DescriptionType.Full || type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		var sb = new StringBuilder();
		switch (type)
		{
			case DescriptionType.Short:
				if (LiquidMixture?.IsEmpty == false && !flags.HasFlag(PerceiveIgnoreFlags.IgnoreLiquidsAndFlags))
				{
					return $"{description} filled with {LiquidMixture.ColouredLiquidDescription}";
				}

				return description;

			case DescriptionType.Contents:
				if (LiquidMixture?.IsEmpty == false)
				{
					if (voyeur is ICharacter ch)
					{
						return
							$"It is {(LiquidMixture.TotalVolume / LiquidCapacity).ToString("P2", ch)} full of {LiquidMixture.ColouredLiquidLongDescription}";
					}

					return string.Format(voyeur, "\n\nIt is approximately {0:P0} full of {1}.",
						Math.Round(20 * LiquidMixture.TotalVolume / LiquidCapacity) / 20,
						LiquidMixture.ColouredLiquidLongDescription);
				}

				return description + "\n\nIt is currently empty.";
			case DescriptionType.Full:
				sb.Append(description);
				sb.AppendLine();

				if (_prototype.UseOnOffForRefill)
				{
					sb.AppendLine($"It is currently switched {(SwitchedOn ? "on" : "off")}.");
				}

				if (Locks.Any())
				{
					sb.AppendLine();
					sb.AppendLine("It has the following locks:");
					foreach (var thelock in Locks)
					{
						sb.AppendLineFormat("\t{0}", thelock.Parent.HowSeen(voyeur));
					}
				}

				return sb.ToString();
			case DescriptionType.Evaluate:
				sb.AppendLine(description);
				sb.AppendLine(
					$"It can hold {Gameworld.UnitManager.DescribeMostSignificantExact(LiquidCapacity, UnitType.FluidVolume, voyeur).ColourValue()} of liquid.");
				sb.AppendLine(
					$"It refills at a rate of about {Gameworld.UnitManager.DescribeMostSignificant(_prototype.RefillRate * 12.0, UnitType.FluidVolume, voyeur).ColourValue()} per minute.");
				if (_prototype.UseOnOffForRefill)
				{
					sb.AppendLine(
						$"It can be switched on and off to control refilling and is currently {(SwitchedOn ? "on" : "off")}.");
				}

				return sb.ToString();
		}

		return description;
	}

	public override double ComponentWeight
	{
		get { return Locks.Sum(x => x.Parent.Weight) + (LiquidMixture?.TotalWeight ?? 0.0); }
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
			foreach (var thelock in Locks.ToList())
			{
				newItemLockable.InstallLock(thelock);
			}
		}
		else
		{
			foreach (var thelock in Locks.ToList())
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

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (WaterSourceGameItemComponentProto)newProto;
		if (LiquidMixture != null && LiquidMixture.TotalVolume > LiquidCapacity)
		{
			LiquidMixture.SetLiquidVolume(LiquidCapacity);
		}

		if (_prototype.Closable && IsOpen)
		{
			IsOpen = false;
		}
	}

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
		return _prototype.Closable && !IsOpen && Locks.All(x => !x.IsLocked) &&
		       Parent.EffectsOfType<IOverrideLockEffect>().All(x => !x.Applies(opener?.Actor));
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
		if (!_prototype.Closable)
		{
			return WhyCannotOpenReason.NotOpenable;
		}

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
		return _prototype.Closable && !_prototype.OnceOnly && IsOpen;
	}

	public WhyCannotCloseReason WhyCannotClose(IBody closer)
	{
		if (!_prototype.Closable)
		{
			return WhyCannotCloseReason.NotOpenable;
		}

		if (!IsOpen)
		{
			return WhyCannotCloseReason.AlreadyClosed;
		}

		return _prototype.OnceOnly ? WhyCannotCloseReason.SingleUse : WhyCannotCloseReason.Unknown;
	}

	public void Close()
	{
		IsOpen = false;
		OnClose?.Invoke(this);
	}

	public event OpenableEvent OnOpen;
	public event OpenableEvent OnClose;

	#endregion

	#region IOnOff Implementation

	private bool _switchedOn;

	public bool SwitchedOn
	{
		get => _switchedOn;
		set
		{
			_switchedOn = value;
			Changed = true;
		}
	}

	#endregion

	#region Implementation of ISwitchable

	public IEnumerable<string> SwitchSettings =>
		_prototype.UseOnOffForRefill ? new[] { "on", "off" } : new string[] { };

	public bool CanSwitch(ICharacter actor, string setting)
	{
		if (!_prototype.UseOnOffForRefill)
		{
			return false;
		}

		// TODO - more reasons why something couldn't be switched on or off
		return (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn) ||
		       (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn)
			;
	}

	public string WhyCannotSwitch(ICharacter actor, string setting)
	{
		if (!_prototype.UseOnOffForRefill)
		{
			return $"{Parent.HowSeen(actor)} is not something that can be switched.";
		}

		if (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn)
		{
			return $"{Parent.HowSeen(actor)} is already on.";
		}

		if (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn)
		{
			return $"{Parent.HowSeen(actor)} is already off.";
		}

		return $"{Parent.HowSeen(actor)} cannot be switched to {setting} at this time.";
	}

	private bool SwitchOn(ICharacter actor)
	{
		SwitchedOn = true;
		Changed = true;
		CheckHeartbeat();
		return true;
	}

	private bool SwitchOff(ICharacter actor)
	{
		Changed = true;
		SwitchedOn = false;
		CheckHeartbeat();
		return true;
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

	#endregion
}