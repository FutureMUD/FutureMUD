using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class PhotocopierGameItemComponent : PoweredMachineBaseGameItemComponent, IContainer, IOpenable, ILockable
{
	private PhotocopierGameItemComponentProto _prototype;

	public PhotocopierGameItemComponent(PhotocopierGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
		CurrentInkLevels = _prototype.MaximumCharactersPrintedPerCartridge;
	}

	public PhotocopierGameItemComponent(MudSharp.Models.GameItemComponent component,
		PhotocopierGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
	}

	public PhotocopierGameItemComponent(PhotocopierGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_contents = new List<IGameItem>();
		_isOpen = rhs._isOpen;
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		_currentInkLevels = int.Parse(root.Element("CurrentInkLevels")?.Value ?? "0");
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

	protected override XElement SaveToXml(XElement root)
	{
		root.Add(new XAttribute("Open", IsOpen));
		root.Add(new XElement("Locks",
			from thelock in Locks select new XElement("Lock", thelock.Parent.Id)
		));
		foreach (var item in Contents)
		{
			root.Add(new XElement("Contained", item.Id));
		}

		root.Add(new XElement("CurrentInkLevels", CurrentInkLevels));
		return root;
	}

	protected override void OnPowerCutInAction()
	{
		// Do nothing
	}

	protected override void OnPowerCutOutAction()
	{
		// Do nothing
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

		base.FinaliseLoad();
	}

	public override void Quit()
	{
		base.Quit();
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

		base.Login();
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

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return (IsOpen && (type == DescriptionType.Contents || type == DescriptionType.Evaluate)) ||
		       type == DescriptionType.Full || type == DescriptionType.Short;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Evaluate:
				return
					$"It can hold {Gameworld.UnitManager.DescribeMostSignificantExact(_prototype.PaperWeightCapacity, Framework.Units.UnitType.Mass, voyeur).Colour(Telnet.Green)} of paper.";
			case DescriptionType.Short:
				return $"{description}{(_onAndPowered ? " (on)".FluentColour(Telnet.BoldWhite, colour) : "")}";
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
				var sb = new StringBuilder();
				sb.Append(description);
				sb.AppendLine($"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}."
					.Colour(Telnet.Yellow));
				if (IsOpen || Transparent)
				{
					sb.AppendLine(
						$"It is {(_contents.Sum(x => x.Weight) / _prototype.PaperWeightCapacity).ToString("P2", voyeur).Colour(Telnet.Green)} full of paper.");
				}

				sb.AppendLine(
					$"It is {((double)CurrentInkLevels / _prototype.MaximumCharactersPrintedPerCartridge).ToString("P2", voyeur).ColourValue()} full of ink.");
				if (Locks.Any())
				{
					sb.AppendLine();
					sb.AppendLine("It has the following locks:");
					foreach (var thelock in Locks)
					{
						sb.AppendLineFormat("\t{0}", thelock.Parent.HowSeen(voyeur));
					}
				}

				sb.AppendLine(
					$"You can use the {"refill photocopier".ColourCommand()} command to refill its ink cartridge.");
				sb.AppendLine(
					$"You can use the {"photocopy <paper>".ColourCommand()} command to photocopy a sheet of paper.");
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

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PhotocopierGameItemComponent(this, newParent, temporary);
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

	public int CurrentInkLevels
	{
		get => _currentInkLevels;
		protected set
		{
			_currentInkLevels = value;
			if (_currentInkLevels < 0)
			{
				_currentInkLevels = 0;
			}

			Changed = true;
		}
	}

	#region IContainer Implementation

	private readonly List<IGameItem> _contents = new();
	public IEnumerable<IGameItem> Contents => _contents;
	public string ContentsPreposition => "in";
	public bool Transparent => false;

	public bool CanPut(IGameItem item)
	{
		return
			IsOpen &&
			item.IsItemType<PaperSheetGameItemComponent>() &&
			_contents.Sum(x => x.Weight) + item.Weight <= _prototype.PaperWeightCapacity;
	}

	public int CanPutAmount(IGameItem item)
	{
		return (int)((_prototype.PaperWeightCapacity - _contents.Sum(x => x.Weight)) / (item.Weight / item.Quantity));
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
		if (!IsOpen)
		{
			return WhyCannotPutReason.ContainerClosed;
		}

		if (!item.IsItemType<PaperSheetGameItemComponent>())
		{
			return WhyCannotPutReason.NotCorrectItemType;
		}

		if (_contents.Sum(x => x.Weight) + item.Weight > _prototype.PaperWeightCapacity)
		{
			var capacity = _prototype.PaperWeightCapacity - _contents.Sum(x => x.Weight);
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
	private int _currentInkLevels;

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

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.CommandInput && ((string)arguments[2]).Equals("photocopy"))
		{
			var ch = (ICharacter)arguments[0];
			var ss = (StringStack)arguments[3];
			if (ss.IsFinished)
			{
				ch.OutputHandler.Send("What do you want to photocopy?");
				return true;
			}

			var target = ch.TargetHeldItem(ss.PopSpeech());
			if (target == null)
			{
				ch.OutputHandler.Send("You aren't holding anything like that.");
				return true;
			}

			if (!(target.GetItemType<IReadable>() is IReadable readable))
			{
				ch.OutputHandler.Send($"{target.HowSeen(ch, true)} is not something that can be photocopied.");
				return true;
			}

			if (!_onAndPowered)
			{
				ch.OutputHandler.Send($"{Parent.HowSeen(ch, true)} is not currently powered on.");
				return true;
			}

			var paperItem = _contents.FirstOrDefault();
			if (paperItem == null)
			{
				ch.OutputHandler.Handle(new EmoteOutput(new Emote(
					"@ attempt|attempts to photocopy $1 in $2, but it is out of paper.", ch, ch, target, Parent)));
				return true;
			}

			if (paperItem.IsItemType<IStackable>() && paperItem.Quantity > 1)
			{
				paperItem = paperItem.Get(null, 1);
			}

			ch.OutputHandler.Handle(new EmoteOutput(new Emote("@ photocopy|photocopies $1 in $2.", ch, ch, target,
				Parent)));
			var papers = new List<IGameItem>();
			papers.Add(paperItem);
			Take(paperItem);
			var copiedEverything = true;
			foreach (var item in readable.Readables)
			{
				while (paperItem != null)
				{
					var paper = paperItem.GetItemType<IWriteable>();
					if (paper == null)
					{
						paperItem = _contents.FirstOrDefault();
						if (paperItem.IsItemType<IStackable>() && paperItem.Quantity > 1)
						{
							paperItem = paperItem.Get(null, 1);
						}

						if (paperItem == null)
						{
							break;
						}

						papers.Add(paperItem);
						Take(paperItem);
						continue;
					}

					if (item is IWriting writing)
					{
						if (!paper.CanAddWriting(writing))
						{
							paperItem = _contents.FirstOrDefault();
							if (paperItem.IsItemType<IStackable>() && paperItem.Quantity > 1)
							{
								paperItem = paperItem.Get(null, 1);
							}

							if (paperItem == null)
							{
								break;
							}

							papers.Add(paperItem);
							Take(paperItem);
							continue;
						}

						paper.AddWriting(writing);
					}
					else if (item is IDrawing drawing)
					{
						if (!paper.CanAddDrawing(drawing))
						{
							paperItem = _contents.FirstOrDefault();
							if (paperItem.IsItemType<IStackable>() && paperItem.Quantity > 1)
							{
								paperItem = paperItem.Get(null, 1);
							}

							if (paperItem == null)
							{
								break;
							}

							papers.Add(paperItem);
							Take(paperItem);
							continue;
						}

						paper.AddDrawing(drawing);
					}

					CurrentInkLevels -= item.DocumentLength;
					if (CurrentInkLevels <= 0)
					{
						paperItem = null;
					}

					break;
				}

				if (paperItem == null)
				{
					copiedEverything = false;
					break;
				}
			}


			Parent.OutputHandler.Handle(new EmoteOutput(new Emote("@ spit|spits out $1.", Parent, Parent,
				new PerceivableGroup(papers))));
			if (!copiedEverything)
			{
				if (CurrentInkLevels <= 0)
				{
					Parent.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ could not copy the entire document because it ran out of ink.", Parent)));
				}
				else
				{
					Parent.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ could not copy the entire document due to a lack of paper.", Parent)));
				}
			}

			foreach (var item in papers)
			{
				if (ch.Body.CanGet(item, 0))
				{
					ch.Body.Get(item, silent: true);
				}
				else
				{
					ch.Location.Insert(item, true);
				}
			}

			return true;
		}

		if (type == EventType.CommandInput && ((string)arguments[2]).Equals("refill"))
		{
			var ch = (ICharacter)arguments[0];
			var ss = (StringStack)arguments[3];
			if (ss.IsFinished || !ss.Peek().EqualTo("photocopier"))
			{
				return false;
			}

			var ink = ch.Body.HeldOrWieldedItems.FirstOrDefault(x =>
				x.Prototype.Id == _prototype.InkCartridgePrototypeId);
			if (ink == null)
			{
				ch.OutputHandler.Send(
					$"You are not holding an ink cartridge suitable for use in {Parent.HowSeen(ch)}.");
				return true;
			}

			ch.OutputHandler.Handle(new EmoteOutput(new Emote("@ refill|refills $1 with $2.", ch, ch, Parent,
				ink.PeekSplit(1))));
			if (ink.DropsWhole(1))
			{
				ink.Delete();
			}
			else
			{
				ink.GetItemType<IStackable>().Quantity -= 1;
			}

			if (_prototype.SpentInkCartridgePrototypeId != 0)
			{
				var spent = Gameworld.ItemProtos.Get(_prototype.SpentInkCartridgePrototypeId)?.CreateNew(ch);
				if (spent != null)
				{
					Gameworld.Add(spent);
					if (ch.Body.CanGet(spent, 0))
					{
						ch.Body.Get(spent);
					}
					else
					{
						spent.RoomLayer = ch.RoomLayer;
						ch.Location.Insert(spent);
					}

					spent.Login();
					spent.HandleEvent(EventType.ItemFinishedLoading, spent);
					
				}
			}

			CurrentInkLevels = _prototype.MaximumCharactersPrintedPerCartridge;
			Changed = true;
			return true;
		}

		var truth = false;
		foreach (var content in Contents)
		{
			truth = truth || content.HandleEvent(type, arguments);
		}

		return truth;
	}
}