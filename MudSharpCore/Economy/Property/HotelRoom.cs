using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using DbHotelLostProperty = MudSharp.Models.HotelLostProperty;
using DbHotelPatronBalance = MudSharp.Models.HotelPatronBalance;
using DbHotelRoom = MudSharp.Models.HotelRoom;
using DbHotelRoomFurnishing = MudSharp.Models.HotelRoomFurnishing;
using DbHotelRoomRental = MudSharp.Models.HotelRoomRental;

namespace MudSharp.Economy.Property;

public class HotelFurnishing : IHotelFurnishing
{
	private readonly HotelRoom _room;

	public HotelFurnishing(HotelRoom room, DbHotelRoomFurnishing record)
	{
		_room = room;
		GameItemId = record.GameItemId;
		Description = record.Description;
		ReplacementValue = record.ReplacementValue;
		OriginalCondition = record.OriginalCondition;
		OriginalDamageCondition = record.OriginalDamageCondition;
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

}

public class HotelRoomRental : IHotelRoomRental
{
	private readonly HotelRoom _room;
	private ICharacter _guest;

	public HotelRoomRental(HotelRoom room, DbHotelRoomRental record)
	{
		_room = room;
		GuestId = record.GuestId;
		StartTime = MudDateTime.FromStoredStringOrFallback(record.StartTime, room.Property.Gameworld,
			StoredMudDateTimeFallback.CurrentDateTime, "HotelRoomRental", null, room.Name, "StartTime");
		EndTime = MudDateTime.FromStoredStringOrFallback(record.EndTime, room.Property.Gameworld,
			StoredMudDateTimeFallback.Never, "HotelRoomRental", null, room.Name, "EndTime");
		RentalCharge = record.RentalCharge;
		SecurityDeposit = record.SecurityDeposit;
		TaxCharged = record.TaxCharged;
	}

	public HotelRoomRental(HotelRoom room, ICharacter guest, MudDateTime start, MudDateTime end, decimal rentalCharge,
		decimal securityDeposit, decimal taxCharged)
	{
		_room = room;
		_guest = guest;
		GuestId = CharacterInstanceIdentityComparer.IdentityId(guest);
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

}

public class HotelPatronBalance : IHotelPatronBalance
{
	private readonly IProperty _property;
	private ICharacter _patron;
	private decimal _balance;

	public HotelPatronBalance(IProperty property, DbHotelPatronBalance record)
	{
		_property = property;
		PatronId = record.PatronId;
		_balance = record.Balance;
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

}

public class HotelLostProperty : IHotelLostProperty
{
	private readonly HotelRoom _room;
	private ICharacter _owner;
	private IGameItem _bundle;

	public HotelLostProperty(HotelRoom room, DbHotelLostProperty record)
	{
		_room = room;
		OwnerId = record.OwnerId;
		BundleId = record.BundleId;
		StoredUntil = MudDateTime.FromStoredStringOrFallback(record.StoredUntil, room.Property.Gameworld,
			StoredMudDateTimeFallback.Never, "HotelLostProperty", null, room.Name, "StoredUntil");
		Status = Enum.IsDefined(typeof(HotelLostPropertyStatus), record.Status)
			? (HotelLostPropertyStatus)record.Status
			: HotelLostPropertyStatus.Held;
		AuctionHouseId = record.AuctionHouseId;
		ReservePrice = record.ReservePrice;
		Description = record.Description;
	}

	public HotelLostProperty(HotelRoom room, ICharacter owner, IGameItem bundle, MudDateTime storedUntil, decimal reservePrice)
	{
		_room = room;
		_owner = owner;
		_bundle = bundle;
		OwnerId = CharacterInstanceIdentityComparer.IdentityId(owner);
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

}

public class HotelRoom : IHotelRoom
{
	public const int MaximumNameLength = 200;

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
	internal long DatabaseId { get; }

	public HotelRoom(Property property, DbHotelRoom record)
	{
		_property = property;
		DatabaseId = record.Id;
		_cellId = record.CellId;
		_name = NormaliseName(record.Name);
		_listed = record.Listed;
		_pricePerDay = Math.Max(0.0M, record.PricePerDay);
		_securityDeposit = Math.Max(0.0M, record.SecurityDeposit);
		_minimumDuration = TimeSpan.FromTicks(record.MinimumDurationTicks);
		_maximumDuration = TimeSpan.FromTicks(record.MaximumDurationTicks);
		_keyIds.AddRange(record.Keys.Select(x => x.PropertyKeyId));
		foreach (var item in record.Furnishings)
		{
			_furnishings.Add(new HotelFurnishing(this, item));
		}

		if (record.ActiveRental is not null)
		{
			_activeRental = new HotelRoomRental(this, record.ActiveRental);
		}
	}

	public HotelRoom(Property property, ICell cell, string name, decimal pricePerDay, decimal securityDeposit,
		TimeSpan minimumDuration, TimeSpan maximumDuration)
	{
		_property = property;
		_cell = cell;
		_cellId = cell.Id;
		_name = NormaliseName(name);
		_listed = true;
		_pricePerDay = Math.Max(0.0M, pricePerDay);
		_securityDeposit = Math.Max(0.0M, securityDeposit);
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
			_name = NormaliseName(value);
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
			_pricePerDay = Math.Max(0.0M, value);
			_property.NoteHotelChanged();
		}
	}

	public decimal SecurityDeposit
	{
		get => _securityDeposit;
		set
		{
			_securityDeposit = Math.Max(0.0M, value);
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

	public IEnumerable<string> Keywords => new ExplodedString(Name).Words.Append(Cell.Id.ToString());

	private static string NormaliseName(string name)
	{
		name = name?.Trim() ?? string.Empty;
		return name.Length <= MaximumNameLength ? name : name.Substring(0, MaximumNameLength);
	}
}
