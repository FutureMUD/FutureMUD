using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace MudSharp.GameItems.Components;

public class AmmoClipGameItemComponent : GameItemComponent, IAmmoClip
{
	protected AmmoClipGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	#region IContainer Implementation

	public override double ComponentWeight
	{
		get { return Contents.Sum(x => x.Weight); }
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return Contents.Sum(x => x.Buoyancy(fluidDensity));
	}

	public string ClipType => _prototype.ClipType;

	public int Capacity => _prototype.Capacity;

	public string SpecificAmmoGrade => _prototype.SpecificAmmoGrade;

	private readonly List<IGameItem> _contents = new();
	public IEnumerable<IGameItem> Contents => _contents;

	public string ContentsPreposition => "in";

	public bool Transparent => true;

	public bool CanPut(IGameItem item)
	{
		var ammo = item.GetItemType<IAmmo>();

		if (ammo?.AmmoType.SpecificType.EqualTo(SpecificAmmoGrade) != true)
		{
			return false;
		}

		if (_contents.Sum(x => x.Quantity) + item.Quantity > Capacity)
		{
			return false;
		}

		return true;
	}

	public int CanPutAmount(IGameItem item)
	{
		return Capacity - _contents.Sum(x => x.Quantity);
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
		var ammo = item.GetItemType<IAmmo>();

		if (ammo?.AmmoType.SpecificType.EqualTo(SpecificAmmoGrade) != true)
		{
			return WhyCannotPutReason.NotCorrectItemType;
		}

		if (_contents.Sum(x => x.Quantity) + item.Quantity > Capacity)
		{
			var remaining = Capacity - _contents.Sum(x => x.Quantity);
			if (item.Quantity <= 1 || remaining <= 0)
			{
				return WhyCannotPutReason.ContainerFull;
			}

			return WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity;
		}

		throw new ApplicationException("Unknown WhyCannotPutReason in AmmoClipGameItemComponent.WhyCannotPut.");
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
			Changed = true;
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
					item.RoomLayer = emptier.RoomLayer;
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
				item.RoomLayer = emptier.RoomLayer;
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

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (AmmoClipGameItemComponentProto)newProto;
	}

	#region Constructors

	public AmmoClipGameItemComponent(AmmoClipGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
		base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public AmmoClipGameItemComponent(MudSharp.Models.GameItemComponent component, AmmoClipGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public AmmoClipGameItemComponent(AmmoClipGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		foreach (var item in root.Element("Contents").Elements("Item"))
		{
			var content = Gameworld.TryGetItem(long.Parse(item.Value), true);
			if (content != null)
			{
				content.Get(null);
				_contents.Add(content);
				content.LoadTimeSetContainedIn(Parent);
			}
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new AmmoClipGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Contents",
				from item in _contents
				select new XElement("Item", item.Id)
			)
		).ToString();
	}

	#endregion

	#region IGameItemComponent Overrides

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Contents || type == DescriptionType.Full || type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
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
				sb.AppendLine("\n");
				sb.AppendLine(
					$"It holds ammunition for firearms of type {SpecificAmmoGrade.Colour(Telnet.Cyan)} and fits weapons designed to take a magazine of type {ClipType.Colour(Telnet.Cyan)}."
						.Wrap(voyeur.InnerLineFormatLength));
				return sb.ToString();
			case DescriptionType.Evaluate:
				return
					$"It holds {_prototype.Capacity.ToString("N0", voyeur).Colour(Telnet.Green)} rounds of {SpecificAmmoGrade.Colour(Telnet.Cyan)}, and fits into weapons with a magazine type of {ClipType.Colour(Telnet.Cyan)}"
						.Wrap(voyeur.InnerLineFormatLength);
		}

		return description;
	}

	public override void FinaliseLoad()
	{
		foreach (var item in _contents)
		{
			item.FinaliseLoadTimeTasks();
		}
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

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
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
			item.ContainedIn = null;
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

	#endregion
}