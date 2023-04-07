using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class BatteryPoweredGameItemComponent : GameItemComponent, IContainer, IOpenable, IProducePower
{
	protected BatteryPoweredGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override void Delete()
	{
		base.Delete();
		foreach (var item in Contents.ToList())
		{
			_contents.Remove(item);
			item.Delete();
		}

		foreach (var item in Locks.ToList())
		{
			_locks.Remove(item);
			item.Parent.Delete();
		}

		_batteries.Clear();
	}

	public override void Quit()
	{
		foreach (var item in Contents)
		{
			item.Quit();
		}

		foreach (var item in Locks)
		{
			item.Quit();
		}
	}

	public override void Login()
	{
		foreach (var item in Contents)
		{
			item.Login();
		}

		foreach (var item in Locks)
		{
			item.Login();
		}
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

		var newItemContainer = newItem?.GetItemType<IContainer>();
		if (newItemContainer != null)
		{
			var newItemOpenable = newItem.GetItemType<IOpenable>();
			if (newItemOpenable != null)
			{
				if (IsOpen)
				{
					newItemOpenable.Open();
				}
				else
				{
					newItemOpenable.Close();
				}
			}

			if (Contents.Any())
			{
				foreach (var item in Contents.ToList())
				{
					if (newItemContainer.CanPut(item))
					{
						newItemContainer.Put(null, item);
					}
					else if (location != null)
					{
						location.Insert(item);
						item.ContainedIn = null;
					}
					else
					{
						item.Delete();
					}
				}

				_contents.Clear();
			}
		}
		else
		{
			foreach (var item in Contents.ToList())
			{
				if (location != null)
				{
					location.Insert(item);
					item.ContainedIn = null;
				}
				else
				{
					item.Delete();
				}
			}

			_contents.Clear();
		}

		return false;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return (IsOpen && type == DescriptionType.Contents) || type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Contents:
				if (_contents.Any())
				{
					return description + "\n\nIt contains the following batteries:\n" +
					       (from item in _contents
					        select "\t" + item.HowSeen(voyeur)).ListToString(separator: "\n", conjunction: "",
						       twoItemJoiner: "\n");
				}

				return description + "\n\nIt does not currently contain any batteries.";
			case DescriptionType.Full:
				var sb = new StringBuilder();
				sb.Append(description);
				sb.AppendLine();
				sb.AppendLine(
					$"It accepts {_prototype.BatteryQuantity} batteries of type {_prototype.BatteryType.Colour(Telnet.Cyan)}.");
				if ((voyeur as ICharacter)?.IsAdministrator() ?? false)
				{
					sb.AppendLine(
						$"{"[Admin Info]:".Colour(Telnet.BoldWhite)} The batteries are in the following state:");

					foreach (var battery in _batteries)
					{
						sb.AppendLine(
							$"\t{battery.Parent.HowSeen(voyeur)}: {battery.WattHoursRemaining.ToString("N4", voyeur).ColourValue()} watt-hours of {battery.TotalWattHours.ToString("N4", voyeur).ColourValue()} total.");
					}
				}

				sb.AppendLine($"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}."
					.Colour(Telnet.Yellow));
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
		}

		return description;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BatteryPoweredGameItemComponentProto)newProto;
	}

	#region Saving

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
				new XElement("Locks", from thelock in Locks select new XElement("Lock", thelock.Parent.Id)),
				from content in Contents select new XElement("Contained", content.Id)).ToString();
	}

	#endregion

	#region Constructors

	public BatteryPoweredGameItemComponent(BatteryPoweredGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public BatteryPoweredGameItemComponent(MudSharp.Models.GameItemComponent component,
		BatteryPoweredGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public BatteryPoweredGameItemComponent(BatteryPoweredGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("Open");
		if (attr != null)
		{
			_isOpen = attr.Value == "true";
		}

		var lockelem = root.Element("Locks");
		if (lockelem != null)
		{
			foreach (
				var item in
				lockelem.Elements("Lock")
				        .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
				        .Where(item => item?.IsItemType<ILock>() == true))
			{
				if (item.ContainedIn != null || item.Location != null || item.InInventoryOf != null)
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

		foreach (
			var item in
			root.Elements("Contained")
			    .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
			    .Where(item => item != null))
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
			_contents.Add(item);
			_batteries.Add(item.GetItemType<IBattery>());
			item.LoadTimeSetContainedIn(Parent);
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BatteryPoweredGameItemComponent(this, newParent, temporary);
	}

	public override void FinaliseLoad()
	{
		foreach (var item in _contents)
		{
			item.FinaliseLoadTimeTasks();
		}
	}

	#endregion

	#region IProducePower Implementation

	private bool _heartbeatOn;

	private void CheckHeartbeat()
	{
		if (_heartbeatOn && (!_connectedConsumers.Any() || !_powerUsers.Any() || !ProducingPower))
		{
			EndHeartbeat();
			return;
		}

		if (!_heartbeatOn && _connectedConsumers.Any() && ProducingPower)
		{
			StartHeartbeat();
		}
	}

	private void StartHeartbeat()
	{
		if (!_heartbeatOn)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManagerOnSecondHeartbeat;
			_heartbeatOn = true;
			foreach (var user in _connectedConsumers)
			{
				if (!_powerUsers.Contains(user))
				{
					_powerUsers.Add(user);
					user.OnPowerCutIn();
				}
			}
		}
	}

	private void HeartbeatManagerOnSecondHeartbeat()
	{
		var powerUsage = _powerUsers.Sum(x => x.PowerConsumptionInWatts) / 3600.0 /
		                 (_prototype.BatteriesInSeries ? 1.0 : _prototype.BatteryQuantity);
#if DEBUG
		if (powerUsage < 0)
		{
			Console.WriteLine("Negative power usage!");
		}
#endif
		foreach (var battery in _batteries)
		{
			battery.WattHoursRemaining -= powerUsage;
		}

		CheckHeartbeat();
	}

	private void EndHeartbeat()
	{
		if (_heartbeatOn)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
			_heartbeatOn = false;
			foreach (var user in _powerUsers.ToList())
			{
				_powerUsers.Remove(user);
				user.OnPowerCutOut();
			}
		}
	}

	public bool PrimaryLoadTimePowerProducer => true;
	public bool PrimaryExternalConnectionPowerProducer => false;

	public double MaximumPowerInWatts => ProducingPower ? _powerUsers.Sum(x => x.PowerConsumptionInWatts) : 0.0;

	public double FuelLevel => _batteries.Any()
		? _batteries.Sum(x => x.WattHoursRemaining / x.TotalWattHours) / _batteries.Count
		: 0.0;

	public bool ProducingPower
		=> _batteries.Sum(x => x.Parent.GetItemType<IStackable>()?.Quantity ?? 1) >= _prototype.BatteryQuantity &&
		   _batteries.All(x => x.WattHoursRemaining > 0);

	private readonly List<IConsumePower> _connectedConsumers = new();
	private readonly List<IConsumePower> _powerUsers = new();

	public void BeginDrawdown(IConsumePower item)
	{
		_connectedConsumers.Add(item);

		if (ProducingPower)
		{
			_powerUsers.Add(item);
			item.OnPowerCutIn();
		}

		CheckHeartbeat();
	}

	public void EndDrawdown(IConsumePower item)
	{
		_connectedConsumers.Remove(item);
		if (_powerUsers.Contains(item))
		{
			item.OnPowerCutOut();
		}

		_powerUsers.Remove(item);
		CheckHeartbeat();
	}

	public bool CanBeginDrawDown(double wattage)
	{
		return CanDrawdownSpike(wattage);
	}

	public bool CanDrawdownSpike(double wattage)
	{
		if (!ProducingPower)
		{
			return false;
		}

		var watthours = wattage / 3600.0;
		if (_prototype.BatteriesInSeries)
		{
			return _batteries.All(x => x.WattHoursRemaining >= watthours);
		}

		return _batteries.All(x => x.WattHoursRemaining >= watthours / _batteries.Count);
	}

	public bool DrawdownSpike(double wattage)
	{
		if (!CanDrawdownSpike(wattage))
		{
			return false;
		}

		if (_prototype.BatteriesInSeries)
		{
			foreach (var battery in _batteries)
			{
				battery.WattHoursRemaining -= wattage / 3600.0;
			}
		}
		else
		{
			foreach (var battery in _batteries)
			{
				battery.WattHoursRemaining -= wattage / (_prototype.BatteryQuantity * 3600.0);
			}
		}

		CheckHeartbeat();
		return true;
	}

	#endregion

	#region IContainer Implementation

	private readonly List<IBattery> _batteries = new();
	private readonly List<IGameItem> _contents = new();
	public IEnumerable<IGameItem> Contents => _contents;

	public string ContentsPreposition => _prototype.ContentsPreposition;

	public bool Transparent => _prototype.Transparent;

	public override double ComponentWeight
	{
		get { return Contents.Sum(x => x.Weight); }
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return Contents.Sum(x => x.Buoyancy(fluidDensity));
	}

	public bool CanPut(IGameItem item)
	{
		return
			IsOpen &&
			(item.GetItemType<IBattery>()?.BatteryType.EqualTo(_prototype.BatteryType) ?? false) &&
			_contents.Sum(x => x.Quantity) + item.Quantity <= _prototype.BatteryQuantity;
	}

	public int CanPutAmount(IGameItem item)
	{
		return _prototype.BatteryQuantity - _contents.Sum(x => x.Quantity);
	}

	public void Put(ICharacter putter, IGameItem item, bool allowMerge = true)
	{
		if (_contents.Contains(item))
		{
#if DEBUG
			throw new ApplicationException("Item duplication in container.");
#endif
			return;
		}

		if (allowMerge)
		{
			var mergeTarget = _contents.FirstOrDefault(x => x.CanMerge(item));
			if (mergeTarget != null)
			{
				mergeTarget.Merge(item);
				item.Delete();
				return;
			}
		}

		_contents.Add(item);
		_batteries.Add(item.GetItemType<IBattery>());
		item.ContainedIn = Parent;
		CheckHeartbeat();
		Changed = true;
	}

	public WhyCannotPutReason WhyCannotPut(IGameItem item)
	{
		if (!IsOpen)
		{
			return WhyCannotPutReason.ContainerClosed;
		}

		if (!item.GetItemType<IBattery>()?.BatteryType.EqualTo(_prototype.BatteryType) ?? true)
		{
			return WhyCannotPutReason.NotCorrectItemType;
		}

		if (_contents.Sum(x => x.Quantity) + item.Quantity > _prototype.BatteryQuantity)
		{
			var capacity = _prototype.BatteryQuantity - _contents.Sum(x => x.Quantity);
			if (item.Quantity <= 1 || capacity <= 0)
			{
				return WhyCannotPutReason.ContainerFull;
			}

			return WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity;
		}

		return WhyCannotPutReason.NotContainer;
	}

	public bool CanTake(ICharacter taker, IGameItem item, int quantity)
	{
		return IsOpen && _contents.Contains(item) && item.CanGet(quantity).AsBool();
	}

	public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
	{
		Changed = true;
		if (quantity == 0 || item.DropsWhole(quantity))
		{
			_contents.Remove(item);
			_batteries.Remove(item.GetItemType<IBattery>());
			item.ContainedIn = null;
			CheckHeartbeat();
			return item;
		}

		return item.Get(null, quantity);
	}

	public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
	{
		if (!IsOpen)
		{
			return WhyCannotGetContainerReason.ContainerClosed;
		}

		return !_contents.Contains(item)
			? WhyCannotGetContainerReason.NotContained
			: WhyCannotGetContainerReason.NotContainer;
	}

	public override bool Take(IGameItem item)
	{
		if (Contents.Contains(item))
		{
			_contents.Remove(item);
			_batteries.RemoveAll(x => x.Parent == item);
			CheckHeartbeat();
			Changed = true;
			return true;
		}

		return false;
	}

	public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
	{
		if (_contents.Contains(existingItem))
		{
			_contents[_contents.IndexOf(existingItem)] = newItem;
			_batteries.RemoveAll(x => x.Parent == existingItem);
			if (newItem.IsItemType<IBattery>())
			{
				_batteries.Add(newItem.GetItemType<IBattery>());
			}

			CheckHeartbeat();
			newItem.ContainedIn = Parent;
			Changed = true;
			existingItem.ContainedIn = null;
			return true;
		}

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

	public void Empty(ICharacter emptier, IContainer intoContainer, IEmote playerEmote = null)
	{
		var location = emptier?.Location ?? Parent.TrueLocations.FirstOrDefault();
		var contents = Contents.ToList();
		_contents.Clear();
		if (emptier is not null)
		{
			if (intoContainer == null)
			{
				emptier.OutputHandler.Handle(
					new MixedEmoteOutput(new Emote("@ empty|empties $0 onto the ground.", emptier, Parent)).Append(
						playerEmote));
			}
			else
			{
				emptier.OutputHandler.Handle(
					new MixedEmoteOutput(new Emote($"@ empty|empties $1 {intoContainer.ContentsPreposition}to $2.",
						emptier, emptier, Parent, intoContainer.Parent)).Append(playerEmote));
			}
		}

		foreach (var item in contents)
		{
			item.ContainedIn = null;
			if (intoContainer != null)
			{
				if (intoContainer.CanPut(item))
				{
					intoContainer.Put(emptier, item);
				}
				else if (location != null)
				{
					location.Insert(item);
					if (emptier != null)
					{
						emptier.OutputHandler.Handle(new EmoteOutput(new Emote(
							"@ cannot put $1 into $2, so #0 set|sets it down on the ground.", emptier, emptier, item,
							intoContainer.Parent)));
					}
				}
				else
				{
					item.Delete();
				}

				continue;
			}

			if (location != null)
			{
				location.Insert(item);
			}
			else
			{
				item.Delete();
			}
		}

		CheckHeartbeat();
		Changed = true;
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
		return !IsOpen && Locks.All(x => !x.IsLocked);
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
		if (IsOpen)
		{
			return WhyCannotOpenReason.AlreadyOpen;
		}

		return Locks.Any(x => x.IsLocked) ? WhyCannotOpenReason.Locked : WhyCannotOpenReason.NotOpenable;
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
		return !IsOpen ? WhyCannotCloseReason.AlreadyClosed : WhyCannotCloseReason.NotOpenable;
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

	public bool InstallLock(ILock theLock)
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