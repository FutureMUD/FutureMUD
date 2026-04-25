using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;

namespace MudSharp.Economy.Property;

public enum HotelLicenseStatus
{
	None,
	Requested,
	Approved
}

public enum HotelLostPropertyStatus
{
	Held,
	ListedForAuction
}

public interface IHotelFurnishing
{
	long GameItemId { get; }
	IGameItem GameItem { get; }
	string Description { get; }
	decimal ReplacementValue { get; set; }
	double OriginalCondition { get; }
	double OriginalDamageCondition { get; }
	decimal CurrentDepositClaim(IHotelRoom room);
}

public interface IHotelRoomRental
{
	IHotelRoom Room { get; }
	ICharacter Guest { get; }
	long GuestId { get; }
	MudDateTime StartTime { get; }
	MudDateTime EndTime { get; }
	decimal RentalCharge { get; }
	decimal SecurityDeposit { get; }
	decimal TaxCharged { get; }
}

public interface IHotelLostProperty
{
	IProperty Property { get; }
	IHotelRoom Room { get; }
	long OwnerId { get; }
	ICharacter Owner { get; }
	IGameItem Bundle { get; }
	long BundleId { get; }
	MudDateTime StoredUntil { get; set; }
	HotelLostPropertyStatus Status { get; set; }
	long? AuctionHouseId { get; set; }
	decimal ReservePrice { get; set; }
	string Description { get; }
}

public interface IHotelPatronBalance
{
	long PatronId { get; }
	ICharacter Patron { get; }
	decimal Balance { get; set; }
}

public interface IHotelRoom : IKeyworded
{
	IProperty Property { get; }
	ICell Cell { get; }
	string Name { get; set; }
	bool Listed { get; set; }
	decimal PricePerDay { get; set; }
	decimal SecurityDeposit { get; set; }
	TimeSpan MinimumDuration { get; set; }
	TimeSpan MaximumDuration { get; set; }
	IEnumerable<IPropertyKey> Keys { get; }
	IEnumerable<IHotelFurnishing> Furnishings { get; }
	IHotelRoomRental ActiveRental { get; set; }
	void AddKey(IPropertyKey key);
	void RemoveKey(IPropertyKey key);
	void AddFurnishing(IGameItem item, decimal replacementValue);
	void RemoveFurnishing(IHotelFurnishing furnishing);
}
