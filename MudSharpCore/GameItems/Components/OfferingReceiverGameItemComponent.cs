using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Components;

public class OfferingReceiverGameItemComponent : GameItemComponent, IOfferingReceiver
{
	private OfferingReceiverGameItemComponentProto _prototype;
	private readonly List<IGameItem> _contents = [];

	public OfferingReceiverGameItemComponent(OfferingReceiverGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public OfferingReceiverGameItemComponent(MudSharp.Models.GameItemComponent component,
		OfferingReceiverGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public OfferingReceiverGameItemComponent(OfferingReceiverGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public OfferingConsumptionMode ConsumptionMode => _prototype.ConsumptionMode;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new OfferingReceiverGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (OfferingReceiverGameItemComponentProto)newProto;
	}

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
		foreach (var item in _contents)
		{
			item.FinaliseLoadTimeTasks();
		}
	}

	public override void Login()
	{
		base.Login();
		foreach (var item in _contents)
		{
			item.Login();
		}
	}

	public override void Quit()
	{
		foreach (var item in _contents)
		{
			item.Quit();
		}

		base.Quit();
	}

	public override void Delete()
	{
		foreach (var item in _contents.ToList())
		{
			_contents.Remove(item);
			item.ContainedIn = null;
			item.Delete();
		}

		base.Delete();
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			from item in _contents
			select new XElement("Contained", item.Id)).ToString();
	}

	private void LoadFromXml(XElement root)
	{
		foreach (var item in root.Elements("Contained")
		                         .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
		                         .Where(item => item is not null))
		{
			_contents.Add(item!);
			item!.Get(null);
			item.LoadTimeSetContainedIn(Parent);
		}
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Contents or DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Contents:
				return _contents.Any()
					? description + "\n\nIt bears the following offerings:\n" + _contents.Select(x => "\t" + x.HowSeen(voyeur)).ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
					: description + "\n\nIt bears no offerings.";
			case DescriptionType.Full:
				return
					$"{description}\n\nIt can receive offerings up to {_prototype.MaximumItemSize.Describe().ColourName()} size and currently bears {_contents.Count.ToString("N0", voyeur).ColourValue()} offering{(_contents.Count == 1 ? string.Empty : "s")}.";
			default:
				return description;
		}
	}

	public override bool Take(IGameItem item)
	{
		if (_contents.Remove(item))
		{
			item.ContainedIn = null;
			Changed = true;
			return true;
		}

		return false;
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		var newContainer = newItem?.GetItemType<IContainer>();
		foreach (var item in _contents.ToList())
		{
			if (newContainer is not null && newContainer.CanPut(item))
			{
				newContainer.Put(null, item);
			}
			else if (location is not null)
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

	public IEnumerable<IGameItem> Contents => _contents;
	public string ContentsPreposition => "on";
	public bool Transparent => true;

	public bool CanPut(IGameItem item)
	{
		return CanAcceptItem(null, item);
	}

	public void Put(ICharacter? putter, IGameItem item, bool allowMerge = true)
	{
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

		if (item.Size > _prototype.MaximumItemSize && !item.IsItemType<ICommodity>())
		{
			return WhyCannotPutReason.ItemTooLarge;
		}

		if (!TagRulesAccept(item))
		{
			return WhyCannotPutReason.NotCorrectItemType;
		}

		return _contents.Sum(x => x.Weight) + item.Weight > _prototype.MaximumContentsWeight
			? WhyCannotPutReason.ContainerFull
			: WhyCannotPutReason.NotContainer;
	}

	public bool CanTake(ICharacter taker, IGameItem item, int quantity)
	{
		return _contents.Contains(item) && item.CanGet(quantity).AsBool();
	}

	public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
	{
		if (!CanTake(taker, item, quantity))
		{
			return null!;
		}

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
		return _contents.Contains(item)
			? WhyCannotGetContainerReason.NotContainer
			: WhyCannotGetContainerReason.NotContained;
	}

	public int CanPutAmount(IGameItem item)
	{
		return (int)((_prototype.MaximumContentsWeight - _contents.Sum(x => x.Weight)) / (item.Weight / item.Quantity));
	}

	public void Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)
	{
		foreach (var item in _contents.ToList())
		{
			_contents.Remove(item);
			item.ContainedIn = null;
			if (intoContainer is not null && intoContainer.CanPut(item))
			{
				intoContainer.Put(emptier, item);
				continue;
			}

			(emptier?.Location ?? Parent.TrueLocations.FirstOrDefault())?.Insert(item);
		}

		Changed = true;
	}

	public bool CanOffer(ICharacter actor, IGameItem offering)
	{
		return CanAcceptItem(actor, offering);
	}

	public string WhyCannotOffer(ICharacter actor, IGameItem offering)
	{
		if (offering is null)
		{
			return "You do not have such an item to offer.";
		}

		if (!CanAcceptItem(actor, offering))
		{
			return WhyCannotPut(offering) switch
			{
				WhyCannotPutReason.CantPutContainerInItself => "You cannot offer the focus to itself.",
				WhyCannotPutReason.ContainerFull => $"{Parent.HowSeen(actor, true)} cannot hold {offering.HowSeen(actor)}.",
				WhyCannotPutReason.ItemTooLarge => $"{offering.HowSeen(actor, true)} is too large to offer at {Parent.HowSeen(actor)}.",
				WhyCannotPutReason.NotCorrectItemType => $"{offering.HowSeen(actor, true)} is not an acceptable offering for {Parent.HowSeen(actor)}.",
				_ => $"{Parent.HowSeen(actor, true)} will not accept {offering.HowSeen(actor)}."
			};
		}

		return string.Empty;
	}

	public bool Offer(ICharacter actor, IGameItem offering, IEmote? playerEmote)
	{
		if (!CanOffer(actor, offering))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.RejectEcho, actor, actor, offering, Parent)));
			actor.OutputHandler.Send(WhyCannotOffer(actor, offering));
			return false;
		}

		actor.Body.Put(offering, Parent, null, 0, null, true, false);
		if (!_contents.Contains(offering))
		{
			return false;
		}

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.AcceptEcho, actor, actor, offering, Parent)).Append(playerEmote));
		_prototype.OnOfferProg?.Execute(actor, Parent, offering);
		HandleOfferingEvent(EventType.OfferingReceived, EventType.OfferingReceivedWitness, actor, offering);

		if (_prototype.ConsumptionMode == OfferingConsumptionMode.BurnOnOffer)
		{
			BurnOffering(actor, offering, null);
		}

		return true;
	}

	public bool CanBurnOffering(ICharacter actor, IGameItem offering)
	{
		return _prototype.ConsumptionMode != OfferingConsumptionMode.RecordOnly &&
		       offering is not null &&
		       _contents.Contains(offering) &&
		       (Parent.Location?.CanGetAccess(Parent, actor) ?? true);
	}

	public string WhyCannotBurnOffering(ICharacter actor, IGameItem offering)
	{
		if (_prototype.ConsumptionMode == OfferingConsumptionMode.RecordOnly)
		{
			return $"{Parent.HowSeen(actor, true)} records offerings but does not burn them.";
		}

		if (offering is null || !_contents.Contains(offering))
		{
			return $"That is not an offering on {Parent.HowSeen(actor)}.";
		}

		if (!(Parent.Location?.CanGetAccess(Parent, actor) ?? true))
		{
			return Parent.Location.WhyCannotGetAccess(Parent, actor);
		}

		return $"You cannot burn {offering.HowSeen(actor)} at {Parent.HowSeen(actor)}.";
	}

	public bool BurnOffering(ICharacter actor, IGameItem offering, IEmote? playerEmote)
	{
		if (!CanBurnOffering(actor, offering))
		{
			actor.OutputHandler.Send(WhyCannotBurnOffering(actor, offering));
			return false;
		}

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.BurnEcho, actor, actor, offering, Parent)).Append(playerEmote));
		_prototype.OnBurnProg?.Execute(actor, Parent, offering);
		HandleOfferingEvent(EventType.OfferingBurned, EventType.OfferingBurnedWitness, actor, offering);

		_contents.Remove(offering);
		offering.ContainedIn = null;
		if (_prototype.ResidueItemProto is null)
		{
			offering.Delete();
		}
		else
		{
			var residue = _prototype.ResidueItemProto.CreateNew(actor);
			residue.RoomLayer = Parent.RoomLayer;
			residue.ContainedIn = Parent;
			_contents.Add(residue);
			residue.Login();
			offering.Delete();
		}

		Changed = true;
		return true;
	}

	private bool CanAcceptItem(ICharacter? actor, IGameItem item)
	{
		return item != Parent &&
		       (item.Size <= _prototype.MaximumItemSize || item.IsItemType<ICommodity>()) &&
		       _contents.Sum(x => x.Weight) + item.Weight <= _prototype.MaximumContentsWeight &&
		       TagRulesAccept(item) &&
		       (actor is null || (_prototype.CanOfferProg?.ExecuteBool(false, actor, Parent, item) ?? true));
	}

	private bool TagRulesAccept(IGameItem item)
	{
		if (_prototype.BlockedTags.Any(tag => item.IsA(tag)))
		{
			return false;
		}

		return !_prototype.AllowedTags.Any() || _prototype.AllowedTags.Any(tag => item.IsA(tag));
	}

	private void HandleOfferingEvent(EventType itemEvent, EventType witnessEvent, ICharacter actor, IGameItem offering)
	{
		Parent.HandleEvent(itemEvent, Parent, actor, offering);
		foreach (var witness in Parent.TrueLocations.SelectMany(x => x.EventHandlers))
		{
			witness.HandleEvent(witnessEvent, Parent, actor, offering, witness);
		}
	}
}
