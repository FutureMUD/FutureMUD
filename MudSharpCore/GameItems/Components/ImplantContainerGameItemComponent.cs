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
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ImplantContainerGameItemComponent : ImplantBaseGameItemComponent, IContainer, IOpenable, ILockable,
	IImplantReportStatus
{
	protected ImplantContainerGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ImplantContainerGameItemComponentProto)newProto;
		if (!_prototype.Closable && !IsOpen)
		{
			IsOpen = true;
		}

		base.UpdateComponentNewPrototype(newProto);
	}

	#region Constructors

	public ImplantContainerGameItemComponent(ImplantContainerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
		IsOpen = true;
	}

	public ImplantContainerGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImplantContainerGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ImplantContainerGameItemComponent(ImplantContainerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_contents = new List<IGameItem>();
		_isOpen = rhs._isOpen;
	}

	protected override void LoadFromXml(XElement root)
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

			_contents.Add(item);
			item.Get(null);
			item.LoadTimeSetContainedIn(Parent);
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ImplantContainerGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		var basevalue = SaveToXmlNoTextConversion();
		basevalue.Add(new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
			new XElement("Locks", from thelock in Locks select new XElement("Lock", thelock.Parent.Id)),
			from content in Contents select new XElement("Contained", content.Id));
		return basevalue.ToString();
	}

	#endregion

	#region IContainer Members

	protected readonly List<IGameItem> _contents = new();
	public IEnumerable<IGameItem> Contents => _contents;

	public bool CanPut(IGameItem item)
	{
		return
			item != Parent &&
			IsOpen &&
			(item.Size <= _prototype.MaximumContentsSize || item.IsItemType<ICommodity>()) &&
			_contents.Sum(x => x.Weight) + item.Weight <= _prototype.WeightLimit;
	}

	public int CanPutAmount(IGameItem item)
	{
		return (int)((_prototype.WeightLimit - _contents.Sum(x => x.Weight)) / (item.Weight / item.Quantity));
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
		item.ContainedIn = Parent;
		Changed = true;
	}

	public WhyCannotPutReason WhyCannotPut(IGameItem item)
	{
		if (item == Parent)
		{
			return WhyCannotPutReason.CantPutContainerInItself;
		}

		if (!IsOpen)
		{
			return WhyCannotPutReason.ContainerClosed;
		}

		if (item.Size > _prototype.MaximumContentsSize)
		{
			return WhyCannotPutReason.ItemTooLarge;
		}

		if (_contents.Sum(x => x.Weight) + item.Weight > _prototype.WeightLimit)
		{
			var capacity =
				(int)((_prototype.WeightLimit - _contents.Sum(x => x.Weight)) / (item.Weight / item.Quantity));
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

	public bool Transparent => _prototype.Transparent;

	public string ContentsPreposition => _prototype.ContentsPreposition;

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
		return _prototype.Closable && IsOpen;
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

		return WhyCannotCloseReason.Unknown;
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

	#region GameItemComponent Overrides

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return (IsOpen && type == DescriptionType.Contents && type == DescriptionType.Evaluate) ||
		       type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		var sb = new StringBuilder();
		switch (type)
		{
			case DescriptionType.Evaluate:
				sb.AppendLine(
					$"It can hold {Gameworld.UnitManager.DescribeMostSignificantExact(_prototype.WeightLimit, Framework.Units.UnitType.Mass, voyeur).Colour(Telnet.Green)} of items up to {_prototype.MaximumContentsSize.Describe().ColourValue()} size.");
				sb.AppendLine($"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}."
					.Colour(Telnet.Yellow));
				return sb.ToString();
			case DescriptionType.Contents:
				if (_contents.Any())
				{
					return description + "\n\nIt has the following contents:\n" +
					       (from item in _contents
					        select "\t" + item.HowSeen(voyeur)).ListToString(separator: "\n", conjunction: "",
						       twoItemJoiner: "\n");
				}

				return description + "\n\nIt is currently empty.";
			case DescriptionType.Full:
				sb.Append(description);
				if (_prototype.Closable)
				{
					sb.AppendLine();
					sb.AppendLine(
						$"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}.".Colour(
							Telnet.Yellow));
				}

				if (IsOpen || Transparent)
				{
					sb.AppendLine(
						$"It is {(_contents.Sum(x => x.Weight) / _prototype.WeightLimit).ToString("P2", voyeur).Colour(Telnet.Green)} full.");
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

	public override double ComponentWeight
	{
		get { return Contents.Sum(x => x.Weight) + Locks.Sum(x => x.Parent.Weight); }
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return Contents.Sum(x => x.Buoyancy(fluidDensity)) + Locks.Sum(x => x.Parent.Buoyancy(fluidDensity));
	}

	public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
	{
		if (_contents.Contains(existingItem))
		{
			_contents[_contents.IndexOf(existingItem)] = newItem;
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

	public override bool Take(IGameItem item)
	{
		if (Contents.Contains(item))
		{
			_contents.Remove(item);
			Changed = true;
			return true;
		}

		return false;
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
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

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var truth = false;
		foreach (var content in Contents)
		{
			truth = truth || content.HandleEvent(type, arguments);
		}

		return truth;
	}

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

	#region IImplantReportStatus

	public string ReportStatus()
	{
		if (!_powered)
		{
			return "\t* Implant is unpowered and non-functional.";
		}

		return
			$"Implant is a container, designed to hold {_prototype.MaximumContentsSize.Describe().ToLowerInvariant()} items.\n\t\t* {(Contents.Any() ? "It is currently empty" : $"It is currently filled to {(Contents.Sum(x => x.Weight) / _prototype.WeightLimit).ToString("P2", InstalledBody.Actor)} capacity.")}";
	}

	#endregion
}