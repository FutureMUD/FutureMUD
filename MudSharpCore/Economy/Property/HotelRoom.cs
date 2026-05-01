using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Economy.Property;

public class HotelFurnishing : IHotelFurnishing
{
	private readonly HotelRoom _room;

	public HotelFurnishing(HotelRoom room, XElement root)
	{
		_room = room;
		GameItemId = long.Parse(root.Attribute("item")?.Value ?? "0");
		Description = root.Attribute("description")?.Value ?? "a furnishing";
		ReplacementValue = decimal.Parse(root.Attribute("value")?.Value ?? "0.0");
		OriginalCondition = double.Parse(root.Attribute("condition")?.Value ?? "1.0");
		OriginalDamageCondition = double.Parse(root.Attribute("damage")?.Value ?? "1.0");
	}

	public HotelFurnishing(HotelRoom room, IGameItem item, decimal replacementValue)
	{
		_room = room;
		GameItemId = item.Id;
		Description = item.HowSeen(item, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings);
		ReplacementValue = replacementValue;
		OriginalCondition = item.Condition;
		OriginalDamageCondition = item.DamageCondition;
	}

	public long GameItemId { get; }
	public IGameItem GameItem => _room.Property.Gameworld.TryGetItem(GameItemId, true);
	public string Description { get; }
	public decimal ReplacementValue { get; set; }
	public double OriginalCondition { get; }
	public double OriginalDamageCondition { get; }

	public decimal CurrentDepositClaim(IHotelRoom room)
	{
		var item = GameItem;
		if (item is null || item.Deleted || item.Destroyed || !room.Cell.GameItems.SelectMany(x => x.DeepItems).Contains(item))
		{
			return ReplacementValue;
		}

		var conditionLoss = Math.Max(0.0, OriginalCondition - item.Condition);
		var damageLoss = Math.Max(0.0, OriginalDamageCondition - item.DamageCondition);
		var claimRatio = Math.Min(1.0, Math.Max(conditionLoss, damageLoss));
		return ReplacementValue * (decimal)claimRatio;
	}

	public XElement SaveToXml()
	{
		return new XElement("Furnishing",
			new XAttribute("item", GameItemId),
			new XAttribute("description", Description),
			new XAttribute("value", ReplacementValue),
			new XAttribute("condition", OriginalCondition),
			new XAttribute("damage", OriginalDamageCondition)
		);
	}
}

public class HotelRoomRental : IHotelRoomRental
{
	private readonly HotelRoom _room;
	private ICharacter _guest;

	public HotelRoomRental(HotelRoom room, XElement root)
	{
		_room = room;
		GuestId = long.Parse(root.Attribute("guest")?.Value ?? "0");
		StartTime = MudDateTime.FromStoredStringOrFallback(root.Attribute("start")?.Value ?? "Never", room.Property.Gameworld,
			StoredMudDateTimeFallback.CurrentDateTime, "HotelRoomRental", null, room.Name, "StartTime");
		EndTime = MudDateTime.FromStoredStringOrFallback(root.Attribute("end")?.Value ?? "Never", room.Property.Gameworld,
			StoredMudDateTimeFallback.Never, "HotelRoomRental", null, room.Name, "EndTime");
		RentalCharge = decimal.Parse(root.Attribute("charge")?.Value ?? "0.0");
		SecurityDeposit = decimal.Parse(root.Attribute("deposit")?.Value ?? "0.0");
		TaxCharged = decimal.Parse(root.Attribute("tax")?.Value ?? "0.0");
	}

	public HotelRoomRental(HotelRoom room, ICharacter guest, MudDateTime start, MudDateTime end, decimal rentalCharge,
		decimal securityDeposit, decimal taxCharged)
	{
		_room = room;
		_guest = guest;
		GuestId = guest.Id;
		StartTime = start;
		EndTime = end;
		RentalCharge = rentalCharge;
		SecurityDeposit = securityDeposit;
		TaxCharged = taxCharged;
	}

	public IHotelRoom Room => _room;
	public ICharacter Guest => _guest ??= _room.Property.Gameworld.TryGetCharacter(GuestId, true);
	public long GuestId { get; }
	public MudDateTime StartTime { get; }
	public MudDateTime EndTime { get; }
	public decimal RentalCharge { get; }
	public decimal SecurityDeposit { get; }
	public decimal TaxCharged { get; }

	public XElement SaveToXml()
	{
		return new XElement("Rental",
			new XAttribute("guest", GuestId),
			new XAttribute("start", StartTime.GetDateTimeString()),
			new XAttribute("end", EndTime.GetDateTimeString()),
			new XAttribute("charge", RentalCharge),
			new XAttribute("deposit", SecurityDeposit),
			new XAttribute("tax", TaxCharged)
		);
	}
}

public class HotelPatronBalance : IHotelPatronBalance
{
	private readonly IProperty _property;
	private ICharacter _patron;
	private decimal _balance;

	public HotelPatronBalance(IProperty property, XElement root)
	{
		_property = property;
		PatronId = long.Parse(root.Attribute("patron")?.Value ?? "0");
		_balance = decimal.Parse(root.Attribute("balance")?.Value ?? "0.0");
	}

	public HotelPatronBalance(IProperty property, ICharacter patron, decimal balance)
	{
		_property = property;
		_patron = patron;
		PatronId = patron.Id;
		_balance = balance;
	}

	public long PatronId { get; }
	public ICharacter Patron => _patron ??= _property.Gameworld.TryGetCharacter(PatronId, true);

	public decimal Balance
	{
		get => _balance;
		set => _balance = value;
	}

	public XElement SaveToXml()
	{
		return new XElement("Balance",
			new XAttribute("patron", PatronId),
			new XAttribute("balance", Balance)
		);
	}
}

public class HotelLostProperty : IHotelLostProperty
{
	private readonly HotelRoom _room;
	private ICharacter _owner;
	private IGameItem _bundle;

	public HotelLostProperty(HotelRoom room, XElement root)
	{
		_room = room;
		OwnerId = long.Parse(root.Attribute("owner")?.Value ?? "0");
		BundleId = long.Parse(root.Attribute("bundle")?.Value ?? "0");
		StoredUntil = MudDateTime.FromStoredStringOrFallback(root.Attribute("until")?.Value ?? "Never", room.Property.Gameworld,
			StoredMudDateTimeFallback.Never, "HotelLostProperty", null, room.Name, "StoredUntil");
		Status = root.Attribute("status")?.Value.TryParseEnum(out HotelLostPropertyStatus status) == true
			? status
			: HotelLostPropertyStatus.Held;
		AuctionHouseId = long.TryParse(root.Attribute("auction")?.Value, out var auction) && auction > 0 ? auction : null;
		ReservePrice = decimal.Parse(root.Attribute("reserve")?.Value ?? "0.0");
		Description = root.Attribute("description")?.Value ?? "lost property";
	}

	public HotelLostProperty(HotelRoom room, ICharacter owner, IGameItem bundle, MudDateTime storedUntil, decimal reservePrice)
	{
		_room = room;
		_owner = owner;
		_bundle = bundle;
		OwnerId = owner.Id;
		BundleId = bundle.Id;
		StoredUntil = storedUntil;
		Status = HotelLostPropertyStatus.Held;
		ReservePrice = reservePrice;
		Description = bundle.HowSeen(bundle, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings);
	}

	public IProperty Property => _room.Property;
	public IHotelRoom Room => _room;
	public long OwnerId { get; }
	public ICharacter Owner => _owner ??= Property.Gameworld.TryGetCharacter(OwnerId, true);
	public IGameItem Bundle => _bundle ??= Property.Gameworld.TryGetItem(BundleId, true);
	public long BundleId { get; }
	public MudDateTime StoredUntil { get; set; }
	public HotelLostPropertyStatus Status { get; set; }
	public long? AuctionHouseId { get; set; }
	public decimal ReservePrice { get; set; }
	public string Description { get; }

	public XElement SaveToXml()
	{
		return new XElement("LostProperty",
			new XAttribute("owner", OwnerId),
			new XAttribute("bundle", BundleId),
			new XAttribute("room", _room.Cell.Id),
			new XAttribute("until", StoredUntil.GetDateTimeString()),
			new XAttribute("status", Status),
			new XAttribute("auction", AuctionHouseId ?? 0L),
			new XAttribute("reserve", ReservePrice),
			new XAttribute("description", Description)
		);
	}
}

public class HotelRoom : IHotelRoom
{
	private readonly Property _property;
	private readonly List<long> _keyIds = new();
	private readonly List<IHotelFurnishing> _furnishings = new();
	private long _cellId;
	private ICell _cell;
	private string _name;
	private bool _listed;
	private decimal _pricePerDay;
	private decimal _securityDeposit;
	private TimeSpan _minimumDuration;
	private TimeSpan _maximumDuration;
	private IHotelRoomRental _activeRental;

	public HotelRoom(Property property, XElement root)
	{
		_property = property;
		_cellId = long.Parse(root.Attribute("cell")?.Value ?? "0");
		_name = root.Attribute("name")?.Value ?? $"Room {_cellId:N0}";
		_listed = bool.Parse(root.Attribute("listed")?.Value ?? "false");
		_pricePerDay = decimal.Parse(root.Attribute("price")?.Value ?? "0.0");
		_securityDeposit = decimal.Parse(root.Attribute("deposit")?.Value ?? "0.0");
		_minimumDuration = TimeSpan.FromTicks(long.Parse(root.Attribute("min")?.Value ?? TimeSpan.FromDays(1).Ticks.ToString()));
		_maximumDuration = TimeSpan.FromTicks(long.Parse(root.Attribute("max")?.Value ?? TimeSpan.FromDays(7).Ticks.ToString()));
		_keyIds.AddRange(root.Element("Keys")?.Elements("Key").Select(x => long.Parse(x.Attribute("id")?.Value ?? "0")) ?? Enumerable.Empty<long>());
		foreach (var item in root.Element("Furnishings")?.Elements("Furnishing") ?? Enumerable.Empty<XElement>())
		{
			_furnishings.Add(new HotelFurnishing(this, item));
		}

		if (root.Element("Rental") is XElement rental)
		{
			_activeRental = new HotelRoomRental(this, rental);
		}
	}

	public HotelRoom(Property property, ICell cell, string name, decimal pricePerDay, decimal securityDeposit,
		TimeSpan minimumDuration, TimeSpan maximumDuration)
	{
		_property = property;
		_cell = cell;
		_cellId = cell.Id;
		_name = name;
		_listed = true;
		_pricePerDay = pricePerDay;
		_securityDeposit = securityDeposit;
		_minimumDuration = minimumDuration;
		_maximumDuration = maximumDuration;
	}

	public IProperty Property => _property;
	public ICell Cell => _cell ??= Property.Gameworld.Cells.Get(_cellId);

	public string Name
	{
		get => _name;
		set
		{
			_name = value;
			_property.NoteHotelChanged();
		}
	}

	public bool Listed
	{
		get => _listed;
		set
		{
			_listed = value;
			_property.NoteHotelChanged();
		}
	}

	public decimal PricePerDay
	{
		get => _pricePerDay;
		set
		{
			_pricePerDay = value;
			_property.NoteHotelChanged();
		}
	}

	public decimal SecurityDeposit
	{
		get => _securityDeposit;
		set
		{
			_securityDeposit = value;
			_property.NoteHotelChanged();
		}
	}

	public TimeSpan MinimumDuration
	{
		get => _minimumDuration;
		set
		{
			_minimumDuration = value;
			_property.NoteHotelChanged();
		}
	}

	public TimeSpan MaximumDuration
	{
		get => _maximumDuration;
		set
		{
			_maximumDuration = value;
			_property.NoteHotelChanged();
		}
	}

	public IEnumerable<IPropertyKey> Keys => _keyIds
		.Select(x => Property.PropertyKeys.FirstOrDefault(y => y.Id == x))
		.Where(x => x is not null);

	public IEnumerable<IHotelFurnishing> Furnishings => _furnishings;

	public IHotelRoomRental ActiveRental
	{
		get => _activeRental;
		set
		{
			_activeRental = value;
			_property.NoteHotelChanged();
		}
	}

	public void AddKey(IPropertyKey key)
	{
		if (_keyIds.Contains(key.Id))
		{
			return;
		}

		_keyIds.Add(key.Id);
		_property.NoteHotelChanged();
	}

	public void RemoveKey(IPropertyKey key)
	{
		if (_keyIds.Remove(key.Id))
		{
			_property.NoteHotelChanged();
		}
	}

	public void AddFurnishing(IGameItem item, decimal replacementValue)
	{
		if (_furnishings.Any(x => x.GameItemId == item.Id))
		{
			return;
		}

		_furnishings.Add(new HotelFurnishing(this, item, replacementValue));
		_property.NoteHotelChanged();
	}

	public void RemoveFurnishing(IHotelFurnishing furnishing)
	{
		if (_furnishings.Remove(furnishing))
		{
			_property.NoteHotelChanged();
		}
	}

	public XElement SaveToXml()
	{
		return new XElement("Room",
			new XAttribute("cell", Cell.Id),
			new XAttribute("name", Name),
			new XAttribute("listed", Listed),
			new XAttribute("price", PricePerDay),
			new XAttribute("deposit", SecurityDeposit),
			new XAttribute("min", MinimumDuration.Ticks),
			new XAttribute("max", MaximumDuration.Ticks),
			new XElement("Keys", _keyIds.Select(x => new XElement("Key", new XAttribute("id", x)))),
			new XElement("Furnishings", _furnishings.OfType<HotelFurnishing>().Select(x => x.SaveToXml())),
			(_activeRental as HotelRoomRental)?.SaveToXml()
		);
	}

	public IEnumerable<string> Keywords => new ExplodedString(Name).Words.Append(Cell.Id.ToString());
}
