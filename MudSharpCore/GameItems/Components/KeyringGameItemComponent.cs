using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Construction;
using MudSharp.Form.Shape;

namespace MudSharp.GameItems.Components;

public class KeyringGameItemComponent : GameItemComponent, IKeyring, IContainer
{
	protected KeyringGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (KeyringGameItemComponentProto)newProto;
	}

	#region Constructors
	public KeyringGameItemComponent(KeyringGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public KeyringGameItemComponent(Models.GameItemComponent component, KeyringGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public KeyringGameItemComponent(KeyringGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		foreach (
			var item in
			root.Elements("Contained")
			    .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
			    .Where(item => item != null))
		{
			if (item.ContainedIn != null || item.Location != null || item.InInventoryOf != null)
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
		return new KeyringGameItemComponent(this, newParent, temporary);
	}
	#endregion

	#region Saving
	protected override string SaveToXml()
	{
		return new XElement("Definition",
				from content in Contents select new XElement("Contained", content.Id)
			).ToString();
	}
	#endregion

	public override void FinaliseLoad()
	{
		foreach (var item in Contents)
		{
			item.FinaliseLoadTimeTasks();
		}
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

	public override void Delete()
	{
		base.Delete();
		foreach (var item in Contents.ToList())
		{
			_contents.Remove(item);
			item.Delete();
		}
	}

	public override void Quit()
	{
		foreach (var item in Contents)
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
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return Contents.Any();
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Contents || type == DescriptionType.Evaluate ||
		       type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		var sb = new StringBuilder();
		switch (type)
		{
			case DescriptionType.Evaluate:
				sb.AppendLine("It can be used in the inventory code as any key that it contains.");
				sb.AppendLine($"It can hold {_prototype.MaximumNumberOfKeys.ToString("N0", voyeur).ColourValue()} {"key".Pluralise(_prototype.MaximumNumberOfKeys != 1)}.");
				return sb.ToString();
			case DescriptionType.Contents:
				if (_contents.Any())
				{
					return description + "\n\nIt has the following keys:\n" +
					       (from item in _contents
					        select "\t" + item.HowSeen(voyeur)).ListToString(separator: "\n", conjunction: "",
						       twoItemJoiner: "\n");
				}

				return description + "\n\nIt is currently empty.";
			case DescriptionType.Full:

				sb.Append(description);
				sb.AppendLine($"It contains {_contents.Count.ToString("N0", voyeur).Colour(Telnet.Green)} of maximum {_prototype.MaximumNumberOfKeys.ToString("N0", voyeur).ColourValue()} keys.");

				return sb.ToString();
		}

		return description;
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

		return false;
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemContainer = newItem?.GetItemType<IContainer>();
		if (newItemContainer != null)
		{
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

	#region Implementation of IKey

	/// <inheritdoc />
	public string LockType => string.Empty;

	/// <inheritdoc />
	public int Pattern { get; set; }

	/// <inheritdoc />
	public bool Unlocks(string type, int pattern)
	{
		return Contents.SelectNotNull(x => x.GetItemType<IKey>()).Any(x => x.Unlocks(type, pattern));
	}

	/// <inheritdoc />
	public string Inspect(ICharacter actor, string description)
	{
		return string.Empty;
	}

	#endregion

	#region Implementation of IContainer

	private readonly List<IGameItem> _contents = new();
	public IEnumerable<IGameItem> Contents => _contents;

	public string ContentsPreposition => "on";

	public bool Transparent => true;

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
			(item.GetItemType<IKey>() is not null && item.GetItemType<IKeyring>() is null) &&
			(_contents.Sum(x => x.GetItemType<IStackable>()?.Quantity ?? 1) +
				item.GetItemType<IStackable>()?.Quantity ?? 1) <= _prototype.MaximumNumberOfKeys;
	}

	public int CanPutAmount(IGameItem item)
	{
		return _prototype.MaximumNumberOfKeys - _contents.Sum(x => x.Quantity);
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
		if (item.GetItemType<IKey>() is null || item.GetItemType<IKeyring>() is not null)
		{
			return WhyCannotPutReason.NotCorrectItemType;
		}

		if (_contents.Sum(x => x.Quantity) + item.Quantity > _prototype.MaximumNumberOfKeys)
		{
			var capacity = _prototype.MaximumNumberOfKeys - _contents.Sum(x => x.Quantity);
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
		return _contents.Contains(item) && item.CanGet(quantity).AsBool();
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
}