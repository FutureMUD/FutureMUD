using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class PileGameItemComponent : GameItemComponent, IContainer
{
	protected PileGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (PileGameItemComponentProto)newProto;
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
			item.ContainedIn = null;
			Changed = true;
			CheckPileEmpty();
			return true;
		}

		return false;
	}

	public override double ComponentWeight
	{
		get { return Contents.Sum(x => x.Weight); }
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return Contents.Sum(x => x.Buoyancy(fluidDensity));
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

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return true;
	}


	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type.In(DescriptionType.Short, DescriptionType.Full);
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				if (!Contents.Any())
				{
					return "a bundle";
				}

				var things = Contents.Select(x => x.Name.ToLowerInvariant()).Distinct();
				var counts = things
				             .Select(
					             x => (Thing: x, Count: Contents.Where(y => y.Name.EqualTo(x)).Sum(y => y.Quantity)))
				             .OrderByDescending(x => x.Count)
				             .ThenBy(x => x.Thing)
				             .ToList();
				var output = counts.Select(x => x.Count == 1 ? x.Thing.A_An_RespectPlurals() : x.Thing.Pluralise())
				                   .ListToString();
				return $"a bundle of {output}";
			// return $"a bundle of {counts.Select(x => _prototype.Decorator.Describe(x.Thing, x.Thing, x.Count).Strip_A_An()).ListToString()}";
			case DescriptionType.Full:
				return
					$"This is a pile of items, bundled together. It contains:\n\n{Contents.Select(x => $"\t{x.HowSeen(voyeur)}").ListToCommaSeparatedValues("\n")}";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public override ISolid OverridenMaterial => Contents.First().Material;

	public override bool OverridesMaterial => Contents.Any();

	public override bool WarnBeforePurge => true;

	#region Constructors

	public PileGameItemComponent(PileGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
		parent, proto, temporary)
	{
		_prototype = proto;
	}

	public PileGameItemComponent(MudSharp.Models.GameItemComponent component, PileGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public PileGameItemComponent(PileGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
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

	public override void FinaliseLoad()
	{
		foreach (var item in Contents)
		{
			item.FinaliseLoadTimeTasks();
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PileGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", from content in Contents select new XElement("Contained", content.Id))
			.ToString();
	}

	#endregion

	#region IContainer Implementation

	private readonly List<IGameItem> _contents = new();
	public IEnumerable<IGameItem> Contents => _contents;

	public void SetContents(IEnumerable<IGameItem> items)
	{
		_contents.AddRange(items);
		foreach (var item in _contents)
		{
			item.ContainedIn = Parent;
		}
	}

	public string ContentsPreposition => "in";
	public bool Transparent => true;

	public bool CanPut(IGameItem item)
	{
		if (item == Parent)
		{
			return false;
		}

		return true;
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
		return WhyCannotPutReason.CantPutContainerInItself;
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
			CheckPileEmpty();
			return item;
		}

		return item.Get(null, quantity);
	}

	private bool CheckPileEmpty()
	{
		if (_contents.Count == 1)
		{
			var content = _contents.Single();
			_contents.Clear();
			content.ContainedIn = null;
			if (Parent.ContainedIn != null)
			{
				Parent.ContainedIn.SwapInPlace(Parent, content);
			}
			else if (Parent.InInventoryOf != null)
			{
				Parent.InInventoryOf.SwapInPlace(Parent, content);
			}
			else
			{
				content.RoomLayer = Parent.RoomLayer;
				Parent.Location.Insert(content);
			}

			Parent.Delete();
			return true;
		}

		return false;
	}

	public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
	{
		if (!_contents.Contains(item))
		{
			return WhyCannotGetContainerReason.NotContained;
		}

		return WhyCannotGetContainerReason.NotContainer;
	}

	public int CanPutAmount(IGameItem item)
	{
		return item.Quantity;
	}

	public void Empty(ICharacter emptier, IContainer intoContainer, IEmote emote = null)
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
						emote));
			}
			else
			{
				emptier.OutputHandler.Handle(
					new MixedEmoteOutput(new Emote($"@ empty|empties $1 {intoContainer.ContentsPreposition}to $2.",
						emptier, emptier, Parent, intoContainer.Parent)).Append(emote));
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

		Parent.Delete();
	}

	#endregion
}