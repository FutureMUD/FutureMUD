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

public class BatteryChargerGameItemComponent : GameItemComponent, IContainer, IOpenable, IConsumePower, ILockable
{
	private bool _drawingDown;
	protected BatteryChargerGameItemComponentProto _prototype;

	public double PowerConsumptionInWatts => _drawingDown ? _prototype.Wattage / _prototype.Efficiency : 0.0;

	public void OnPowerCutIn()
	{
		StartDrawDown();
	}

	public void OnPowerCutOut()
	{
		EndDrawDown();
	}

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
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
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

		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
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

		Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
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
				if (_drawingDown)
				{
					sb.AppendLine("It is currently charging some batteries.");
				}
				else if (_batteries.Any(x => x.Rechargable && x.WattHoursRemaining < x.TotalWattHours))
				{
					sb.AppendLine("It is unpowered, and not charging its batteries.");
				}
				else if (_batteries.Any())
				{
					sb.AppendLine(
						"It is not charging the batteries it contains, either because they are full or not rechargable.");
				}
				else
				{
					sb.AppendLine("It could charge some batteries if they were put inside of it.");
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
		}

		return description;
	}

	private void CheckDrawDown()
	{
		if (!_drawingDown && _batteries.Any(x => x.Rechargable && x.WattHoursRemaining < x.TotalWattHours))
		{
			StartDrawDown();
		}

		if (_drawingDown && _batteries.All(x => !x.Rechargable || x.WattHoursRemaining >= x.TotalWattHours))
		{
			EndDrawDown();
		}
	}

	private void StartDrawDown()
	{
		_drawingDown = true;
		Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManager_SecondHeartbeat;
	}

	private void HeartbeatManager_SecondHeartbeat()
	{
		var charging = _batteries.Where(x => x.Rechargable && x.WattHoursRemaining < x.TotalWattHours).ToList();
		foreach (var battery in charging)
		{
			battery.WattHoursRemaining = Math.Min(battery.TotalWattHours,
				battery.WattHoursRemaining + _prototype.Wattage *
				(battery.Parent.GetItemType<IStackable>()?.Quantity ?? 1) /
				charging.Sum(x => x.Parent.GetItemType<IStackable>()?.Quantity ?? 1));
		}

		CheckDrawDown();
	}

	private void EndDrawDown()
	{
		_drawingDown = false;
		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BatteryChargerGameItemComponentProto)newProto;
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
			(_contents.Sum(x => x.GetItemType<IStackable>()?.Quantity ?? 1) +
				item.GetItemType<IStackable>()?.Quantity ?? 1) <= _prototype.BatteryQuantity;
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
		Changed = true;
		CheckDrawDown();
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
			item.ContainedIn = null;
			CheckDrawDown();
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
			CheckDrawDown();
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

			newItem.ContainedIn = Parent;
			Changed = true;
			existingItem.ContainedIn = null;
			CheckDrawDown();
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

	public bool InstallLock(ILock theLock, ICharacter actor)
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

	#region Constructors

	public BatteryChargerGameItemComponent(BatteryChargerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public BatteryChargerGameItemComponent(MudSharp.Models.GameItemComponent component,
		BatteryChargerGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public BatteryChargerGameItemComponent(BatteryChargerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	#region Overrides of GameItemComponent

	public override void FinaliseLoad()
	{
		foreach (var item in Locks)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}

		foreach (var item in Contents)
		{
			item.FinaliseLoadTimeTasks();
		}
	}

	#endregion

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
				        .Where(item => item != null && item.IsItemType<ILock>()))
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
				InstallLock(item.GetItemType<ILock>(), null);
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
		return new BatteryChargerGameItemComponent(this, newParent, temporary);
	}

	#endregion
}