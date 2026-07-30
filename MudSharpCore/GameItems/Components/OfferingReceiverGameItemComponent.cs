using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.GameItems.Components;

public class OfferingReceiverGameItemComponent : GameItemComponent, IOfferingReceiver
{
	private OfferingReceiverGameItemComponentProto _prototype;
	private readonly List<IGameItem> _contents = [];
	private long _liquidOfferingCount;
	private double _totalOfferedLiquidVolume;
	private long? _lastOffererId;
	private string? _lastOffererName;
	private string? _lastOfferedLiquid;
	private double _lastOfferedLiquidVolume;
	private DateTime? _lastLiquidOfferingUtc;

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
	public bool AcceptsLiquidOfferings => _prototype.AcceptsLiquidOfferings;
	public long LiquidOfferingCount => _liquidOfferingCount;
	public double TotalOfferedLiquidVolume => _totalOfferedLiquidVolume;
	public long? LastOffererId => _lastOffererId;
	public string? LastOffererName => _lastOffererName;
	public string? LastOfferedLiquid => _lastOfferedLiquid;
	public double LastOfferedLiquidVolume => _lastOfferedLiquidVolume;
	public DateTime? LastLiquidOfferingUtc => _lastLiquidOfferingUtc;

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
			new XElement("LiquidOfferingCount", _liquidOfferingCount),
			new XElement("TotalOfferedLiquidVolume", _totalOfferedLiquidVolume),
			new XElement("LastOffererId", _lastOffererId ?? 0),
			new XElement("LastOffererName", new XCData(_lastOffererName ?? string.Empty)),
			new XElement("LastOfferedLiquid", new XCData(_lastOfferedLiquid ?? string.Empty)),
			new XElement("LastOfferedLiquidVolume", _lastOfferedLiquidVolume),
			new XElement("LastLiquidOfferingUtc",
				_lastLiquidOfferingUtc?.ToString("O", System.Globalization.CultureInfo.InvariantCulture) ??
				string.Empty),
			from item in _contents
			select new XElement("Contained", item.Id)).ToString();
	}

	private void LoadFromXml(XElement root)
	{
		_liquidOfferingCount = long.TryParse(root.Element("LiquidOfferingCount")?.Value, out var count)
			? count
			: 0;
		_totalOfferedLiquidVolume = double.TryParse(root.Element("TotalOfferedLiquidVolume")?.Value,
			System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture,
			out var totalVolume)
			? totalVolume
			: 0.0;
		var lastOffererId = long.TryParse(root.Element("LastOffererId")?.Value, out var offererId)
			? offererId
			: 0;
		_lastOffererId = lastOffererId > 0 ? lastOffererId : null;
		var lastOffererName = root.Element("LastOffererName")?.Value;
		_lastOffererName = string.IsNullOrEmpty(lastOffererName) ? null : lastOffererName;
		var lastOfferedLiquid = root.Element("LastOfferedLiquid")?.Value;
		_lastOfferedLiquid = string.IsNullOrEmpty(lastOfferedLiquid) ? null : lastOfferedLiquid;
		_lastOfferedLiquidVolume = double.TryParse(root.Element("LastOfferedLiquidVolume")?.Value,
			System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture,
			out var lastVolume)
			? lastVolume
			: 0.0;
		_lastLiquidOfferingUtc = DateTime.TryParse(root.Element("LastLiquidOfferingUtc")?.Value,
			System.Globalization.CultureInfo.InvariantCulture,
			System.Globalization.DateTimeStyles.RoundtripKind, out var offeredAt)
			? offeredAt
			: null;
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
				var liquidSummary = !_prototype.AcceptsLiquidOfferings
					? string.Empty
					: _liquidOfferingCount == 0
						? " It has no recorded liquid libations."
						: $" It has received {_liquidOfferingCount.ToString("N0", voyeur).ColourValue()} liquid libation{(_liquidOfferingCount == 1 ? string.Empty : "s")}, most recently {_lastOfferedLiquidVolume.ToString("N3", voyeur).ColourValue()} of {_lastOfferedLiquid?.ColourName() ?? "an unknown liquid".ColourName()} from {(_lastOffererName ?? "an unknown offerer").ColourName()}.";
				return
					$"{description}\n\nIt can receive offerings up to {_prototype.MaximumItemSize.Describe().ColourName()} size and currently bears {_contents.Count.ToString("N0", voyeur).ColourValue()} offering{(_contents.Count == 1 ? string.Empty : "s")}.{liquidSummary}";
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
				InsertAtParentSpatialLocation(item, location);
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

			item.InsertAtSource(emptier is null ? Parent.LocationLevelPerceivable : emptier);
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

	public bool CanOfferLiquid(ICharacter actor, IGameItem source, double amount)
	{
		if (!_prototype.AcceptsLiquidOfferings || source == Parent || amount <= 0.0)
		{
			return false;
		}

		var container = source.GetItemType<ILiquidContainer>();
		if (container is null || !container.IsOpen || container.LiquidMixture?.IsEmpty != false ||
		    amount > container.LiquidVolume ||
		    amount < _prototype.MinimumLiquidOfferingVolume ||
		    (_prototype.MaximumLiquidOfferingVolume > 0.0 && amount > _prototype.MaximumLiquidOfferingVolume))
		{
			return false;
		}

		var mixture = new LiquidMixture(container.LiquidMixture, amount);
		return LiquidTagRulesAccept(mixture) &&
		       (_prototype.CanOfferLiquidProg?.ExecuteBool(false, actor, Parent, source, mixture, amount) ?? true);
	}

	public string WhyCannotOfferLiquid(ICharacter actor, IGameItem source, double amount)
	{
		if (!_prototype.AcceptsLiquidOfferings)
		{
			return $"{Parent.HowSeen(actor, true)} does not accept liquid libations.";
		}

		if (source == Parent)
		{
			return "You cannot pour a libation from the offering focus into itself.";
		}

		var container = source.GetItemType<ILiquidContainer>();
		if (container is null)
		{
			return "You must pour the libation from a liquid container.";
		}

		if (!container.IsOpen)
		{
			return $"{source.HowSeen(actor, true)} is not open.";
		}

		if (container.LiquidMixture?.IsEmpty != false)
		{
			return $"{source.HowSeen(actor, true)} contains no liquid to offer.";
		}

		if (amount <= 0.0)
		{
			return "You must offer a positive quantity of liquid.";
		}

		if (amount > container.LiquidVolume)
		{
			return $"{source.HowSeen(actor, true)} does not contain that much liquid.";
		}

		if (amount < _prototype.MinimumLiquidOfferingVolume)
		{
			return
				$"The smallest libation accepted by {Parent.HowSeen(actor)} is {Gameworld.UnitManager.DescribeExact(_prototype.MinimumLiquidOfferingVolume, Framework.Units.UnitType.FluidVolume, actor).ColourValue()}.";
		}

		if (_prototype.MaximumLiquidOfferingVolume > 0.0 && amount > _prototype.MaximumLiquidOfferingVolume)
		{
			return
				$"The largest libation accepted by {Parent.HowSeen(actor)} is {Gameworld.UnitManager.DescribeExact(_prototype.MaximumLiquidOfferingVolume, Framework.Units.UnitType.FluidVolume, actor).ColourValue()}.";
		}

		var mixture = new LiquidMixture(container.LiquidMixture, amount);
		if (!LiquidTagRulesAccept(mixture))
		{
			return $"{mixture.ColouredLiquidDescription} is not an acceptable libation for {Parent.HowSeen(actor)}.";
		}

		if (_prototype.CanOfferLiquidProg?.ExecuteBool(false, actor, Parent, source, mixture, amount) == false)
		{
			return _prototype.WhyCannotOfferLiquidProg?.ExecuteString(actor, Parent, source, mixture, amount) ??
			       $"{Parent.HowSeen(actor, true)} rejects that libation.";
		}

		return $"You cannot offer that liquid at {Parent.HowSeen(actor)}.";
	}

	public bool OfferLiquid(ICharacter actor, IGameItem source, double amount, IEmote? playerEmote)
	{
		if (!CanOfferLiquid(actor, source, amount))
		{
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote(_prototype.LiquidRejectEcho, actor, actor, source, Parent)));
			actor.OutputHandler.Send(WhyCannotOfferLiquid(actor, source, amount));
			return false;
		}

		var container = source.GetItemType<ILiquidContainer>();
		var offered = container.RemoveLiquidAmount(amount, actor, "libate");
		if (offered?.IsEmpty != false)
		{
			return false;
		}

		_liquidOfferingCount++;
		_totalOfferedLiquidVolume += offered.TotalVolume;
		_lastOffererId = CharacterInstanceIdentityComparer.IdentityId(actor);
		_lastOffererName = actor.Name;
		_lastOfferedLiquid = offered.LiquidDescription;
		_lastOfferedLiquidVolume = offered.TotalVolume;
		_lastLiquidOfferingUtc = DateTime.UtcNow;
		Changed = true;

		var echo = string.Format(System.Globalization.CultureInfo.InvariantCulture, _prototype.LiquidAcceptEcho,
			offered.ColouredLiquidDescription);
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(echo, actor, actor, source, Parent)).Append(playerEmote));
		_prototype.OnOfferLiquidProg?.Execute(actor, Parent, source, offered, offered.TotalVolume);
		var oracleResponse =
			_prototype.OracleResponseProg?.ExecuteString(actor, Parent, source, offered, offered.TotalVolume);
		if (!string.IsNullOrWhiteSpace(oracleResponse))
		{
			actor.OutputHandler.Send(oracleResponse);
		}

		HandleLiquidOfferingEvent(actor, source, offered);
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

	private bool LiquidTagRulesAccept(LiquidMixture mixture)
	{
		var liquids = mixture.Instances
			.Select(x => x.Liquid)
			.Distinct()
			.ToList();
		if (liquids.Any(liquid => _prototype.BlockedLiquidTags.Any(liquid.IsA)))
		{
			return false;
		}

		return !_prototype.AllowedLiquidTags.Any() ||
		       liquids.All(liquid => _prototype.AllowedLiquidTags.Any(liquid.IsA));
	}

	private void HandleOfferingEvent(EventType itemEvent, EventType witnessEvent, ICharacter actor, IGameItem offering)
	{
		Parent.HandleEvent(itemEvent, Parent, actor, offering);
		foreach (var witness in Parent.TrueLocations.SelectMany(x => x.EventHandlersFor(Parent.LocationLevelPerceivable)))
		{
			witness.HandleEvent(witnessEvent, Parent, actor, offering, witness);
		}
	}

	private void HandleLiquidOfferingEvent(ICharacter actor, IGameItem source, LiquidMixture liquid)
	{
		Parent.HandleEvent(EventType.LiquidOfferingReceived, Parent, actor, source, liquid, liquid.TotalVolume);
		foreach (var witness in Parent.TrueLocations.SelectMany(x => x.EventHandlersFor(Parent.LocationLevelPerceivable)))
		{
			witness.HandleEvent(EventType.LiquidOfferingReceivedWitness, Parent, actor, source, liquid,
				liquid.TotalVolume, witness);
		}
	}
}
