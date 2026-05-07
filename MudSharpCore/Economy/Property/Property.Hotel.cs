using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Economy.Property;

public partial class Property
{
	private readonly List<IHotelRoom> _hotelRooms = new();
	private readonly List<IHotelLostProperty> _hotelLostProperties = new();
	private readonly List<IHotelPatronBalance> _hotelPatronBalances = new();
	private readonly List<long> _hotelBannedPatrons = new();
	private HotelLicenseStatus _hotelLicenseStatus;
	private long? _hotelBankAccountId;
	private IBankAccount _hotelBankAccount;
	private long? _hotelCanRentProgId;
	private IFutureProg _hotelCanRentProg;
	private MudTimeSpan _hotelLostPropertyRetention = MudTimeSpan.FromDays(14);
	private decimal _hotelOutstandingTaxes;
	private bool _hotelHeartbeatRegistered;

	private void InitialiseDefaultHotelDefinition()
	{
		_hotelLicenseStatus = HotelLicenseStatus.None;
		_hotelLostPropertyRetention = MudTimeSpan.FromDays(14);
		_hotelOutstandingTaxes = 0.0M;
		EnsureHotelHeartbeat();
	}

	private void LoadHotelDefinition(string definition)
	{
		InitialiseDefaultHotelDefinition();
		if (string.IsNullOrWhiteSpace(definition))
		{
			return;
		}

		XElement root;
		try
		{
			root = XElement.Parse(definition);
		}
		catch
		{
			return;
		}

		_hotelLicenseStatus = root.Attribute("status")?.Value.TryParseEnum(out HotelLicenseStatus status) == true
			? status
			: HotelLicenseStatus.None;
		_hotelBankAccountId = long.TryParse(root.Attribute("bank")?.Value, out var bank) && bank > 0 ? bank : null;
		_hotelCanRentProgId = long.TryParse(root.Attribute("canrent")?.Value, out var prog) && prog > 0 ? prog : null;
		_hotelOutstandingTaxes = decimal.Parse(root.Attribute("taxes")?.Value ?? "0.0");
		_hotelLostPropertyRetention = !string.IsNullOrWhiteSpace(root.Attribute("lostretention")?.Value)
			? MudTimeSpan.Parse(root.Attribute("lostretention").Value)
			: MudTimeSpan.FromDays(14);

		_hotelBannedPatrons.AddRange(root.Element("Bans")?.Elements("Ban")
			.Select(x => long.Parse(x.Attribute("id")?.Value ?? "0"))
			.Where(x => x > 0) ?? Enumerable.Empty<long>());

		foreach (var item in root.Element("Rooms")?.Elements("Room") ?? Enumerable.Empty<XElement>())
		{
			_hotelRooms.Add(new HotelRoom(this, item));
		}

		foreach (var item in root.Element("LostProperties")?.Elements("LostProperty") ?? Enumerable.Empty<XElement>())
		{
			var roomId = long.Parse(item.Attribute("room")?.Value ?? "0");
			if (_hotelRooms.FirstOrDefault(x => x.Cell.Id == roomId) is not HotelRoom room)
			{
				continue;
			}

			_hotelLostProperties.Add(new HotelLostProperty(room, item));
		}

		foreach (var item in root.Element("Balances")?.Elements("Balance") ?? Enumerable.Empty<XElement>())
		{
			var balance = new HotelPatronBalance(this, item);
			if (balance.PatronId > 0 && balance.Balance != 0.0M)
			{
				_hotelPatronBalances.Add(balance);
			}
		}
	}

	private XElement SaveHotelDefinition()
	{
		return new XElement("Hotel",
			new XAttribute("status", HotelLicenseStatus),
			new XAttribute("bank", HotelBankAccount?.Id ?? _hotelBankAccountId ?? 0L),
			new XAttribute("canrent", HotelCanRentProg?.Id ?? _hotelCanRentProgId ?? 0L),
			new XAttribute("lostretention", HotelLostPropertyRetention.GetRoundTripParseText),
			new XAttribute("taxes", HotelOutstandingTaxes),
			new XElement("Bans", _hotelBannedPatrons.Select(x => new XElement("Ban", new XAttribute("id", x)))),
			new XElement("Rooms", _hotelRooms.OfType<HotelRoom>().Select(x => x.SaveToXml())),
			new XElement("LostProperties", _hotelLostProperties.OfType<HotelLostProperty>().Select(x => x.SaveToXml())),
			new XElement("Balances", _hotelPatronBalances.OfType<HotelPatronBalance>()
				.Where(x => x.Balance != 0.0M)
				.Select(x => x.SaveToXml()))
		);
	}

	internal void NoteHotelChanged()
	{
		Changed = true;
	}

	private void EnsureHotelHeartbeat()
	{
		if (_hotelHeartbeatRegistered || Gameworld is null)
		{
			return;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HotelHeartbeat;
		_hotelHeartbeatRegistered = true;
	}

	private void HotelHeartbeat()
	{
		if (_hotelLicenseStatus == HotelLicenseStatus.None &&
			_hotelRooms.All(x => x.ActiveRental is null) &&
			_hotelLostProperties.Count == 0)
		{
			return;
		}

		CheckHotelLostProperty();
	}

	public HotelLicenseStatus HotelLicenseStatus
	{
		get => _hotelLicenseStatus;
		set
		{
			_hotelLicenseStatus = value;
			Changed = true;
		}
	}

	public IBankAccount HotelBankAccount
	{
		get
		{
			if (_hotelBankAccount is null && _hotelBankAccountId.HasValue)
			{
				_hotelBankAccount = Gameworld.BankAccounts.Get(_hotelBankAccountId.Value);
			}

			return _hotelBankAccount;
		}
		set
		{
			_hotelBankAccount = value;
			_hotelBankAccountId = value?.Id;
			Changed = true;
		}
	}

	public decimal HotelCashBalance => VirtualCashLedger.Balance(this, EconomicZone.Currency);
	public decimal HotelAvailableFunds => VirtualCashLedger.AvailableFunds(this, EconomicZone.Currency, HotelBankAccount);

	public IFutureProg HotelCanRentProg
	{
		get
		{
			if (_hotelCanRentProg is null && _hotelCanRentProgId.HasValue)
			{
				_hotelCanRentProg = Gameworld.FutureProgs.Get(_hotelCanRentProgId.Value);
			}

			return _hotelCanRentProg;
		}
		set
		{
			_hotelCanRentProg = value;
			_hotelCanRentProgId = value?.Id;
			Changed = true;
		}
	}

	public MudTimeSpan HotelLostPropertyRetention
	{
		get => _hotelLostPropertyRetention;
		set
		{
			_hotelLostPropertyRetention = value;
			Changed = true;
		}
	}

	public decimal HotelOutstandingTaxes
	{
		get => _hotelOutstandingTaxes;
		set
		{
			_hotelOutstandingTaxes = value;
			Changed = true;
		}
	}

	public IEnumerable<IHotelRoom> HotelRooms => _hotelRooms;
	public IEnumerable<IHotelLostProperty> HotelLostProperties => _hotelLostProperties;
	public IEnumerable<IHotelPatronBalance> HotelPatronBalances => _hotelPatronBalances;
	public IEnumerable<long> HotelBannedPatronIds => _hotelBannedPatrons;
	public bool IsApprovedHotel => HotelLicenseStatus == HotelLicenseStatus.Approved;

	public bool IsBannedFromHotel(ICharacter patron)
	{
		return patron is not null && _hotelBannedPatrons.Contains(patron.Id);
	}

	public void BanFromHotel(ICharacter patron)
	{
		if (patron is null || _hotelBannedPatrons.Contains(patron.Id))
		{
			return;
		}

		_hotelBannedPatrons.Add(patron.Id);
		Changed = true;
	}

	public void UnbanFromHotel(ICharacter patron)
	{
		if (patron is null)
		{
			return;
		}

		if (_hotelBannedPatrons.Remove(patron.Id))
		{
			Changed = true;
		}
	}

	public bool HasHotelBalance(ICharacter patron)
	{
		return HotelBalanceFor(patron) != 0.0M;
	}

	public decimal HotelBalanceFor(ICharacter patron)
	{
		if (patron is null)
		{
			return 0.0M;
		}

		return _hotelPatronBalances.FirstOrDefault(x => x.PatronId == patron.Id)?.Balance ?? 0.0M;
	}

	public void AdjustHotelBalance(ICharacter patron, decimal amount)
	{
		if (patron is null || amount == 0.0M)
		{
			return;
		}

		var balance = _hotelPatronBalances.FirstOrDefault(x => x.PatronId == patron.Id);
		if (balance is null)
		{
			_hotelPatronBalances.Add(new HotelPatronBalance(this, patron, amount));
			Changed = true;
			return;
		}

		balance.Balance += amount;
		if (balance.Balance == 0.0M)
		{
			_hotelPatronBalances.Remove(balance);
		}

		Changed = true;
	}

	public IHotelRoom AddHotelRoom(ICell cell, string name, decimal pricePerDay, decimal securityDeposit,
		TimeSpan minimumDuration, TimeSpan maximumDuration)
	{
		var room = new HotelRoom(this, cell, name, pricePerDay, securityDeposit, minimumDuration, maximumDuration);
		_hotelRooms.Add(room);
		Changed = true;
		return room;
	}

	public void RemoveHotelRoom(IHotelRoom room)
	{
		if (room is null)
		{
			return;
		}

		_hotelRooms.Remove(room);
		Changed = true;
	}

	public IHotelRoom HotelRoomForCell(ICell cell)
	{
		return cell is null ? null : _hotelRooms.FirstOrDefault(x => x.Cell == cell);
	}

	public bool CanRentHotelRoom(ICharacter patron, IHotelRoom room, TimeSpan duration, out string reason)
	{
		if (!IsApprovedHotel)
		{
			reason = "This property is not currently approved as a hotel.";
			return false;
		}

		if (room is null || !_hotelRooms.Contains(room))
		{
			reason = "That is not a room in this hotel.";
			return false;
		}

		if (!room.Listed)
		{
			reason = "That room is not currently being offered for rent.";
			return false;
		}

		if (room.ActiveRental is not null)
		{
			reason = "That room is already rented.";
			return false;
		}

		if (duration < room.MinimumDuration || duration > room.MaximumDuration)
		{
			reason = $"That room can only be rented for between {room.MinimumDuration.Describe(patron)} and {room.MaximumDuration.Describe(patron)}.";
			return false;
		}

		if (IsBannedFromHotel(patron))
		{
			reason = "You are banned from staying at this hotel.";
			return false;
		}

		if (HotelBalanceFor(patron) < 0.0M)
		{
			reason = "You have an outstanding balance with this hotel.";
			return false;
		}

		var prog = HotelCanRentProg;
		if (prog is not null)
		{
			var canRent = prog.MatchesParameters(new[] { ProgVariableTypes.Character })
				? prog.ExecuteBool(false, patron)
				: prog.ExecuteBool(false);
			if (!canRent)
			{
				reason = "This hotel declines to rent you a room.";
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	public IHotelRoomRental RentHotelRoom(ICharacter patron, IHotelRoom room, TimeSpan duration, decimal rentalCharge,
		decimal taxCharge)
	{
		var now = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		var rental = new HotelRoomRental((HotelRoom)room, patron, now, now + duration, rentalCharge, room.SecurityDeposit, taxCharge);
		room.ActiveRental = rental;
		HotelOutstandingTaxes += taxCharge;
		foreach (var key in room.Keys)
		{
			key.IsReturned = false;
			key.GameItem.Login();
		}

		Changed = true;
		return rental;
	}

	public decimal CompleteHotelStay(IHotelRoom iroom, ICharacter actor, bool force)
	{
		if (iroom is not HotelRoom room || room.ActiveRental is not HotelRoomRental rental)
		{
			return 0.0M;
		}

		if (!force && actor is not null && actor.Id != rental.GuestId)
		{
			return 0.0M;
		}

		var guest = rental.Guest ?? actor;
		var furnishingClaims = room.Furnishings.Sum(x => x.CurrentDepositClaim(room));
		var keyClaims = 0.0M;
		foreach (var key in room.Keys.Where(x => !x.IsReturned).ToList())
		{
			keyClaims += key.CostToReplace;
			key.GameItem = key.GameItem.DeepCopy(true, false);
			key.IsReturned = true;
			key.GameItem.Quit();
		}

		var refund = rental.SecurityDeposit - furnishingClaims - keyClaims;
		AdjustHotelBalance(guest, refund);
		CollectLostProperty(room, guest);
		room.ActiveRental = null;
		Changed = true;
		return refund;
	}

	public void ClaimHotelLostProperty(IHotelLostProperty property)
	{
		if (property is null)
		{
			return;
		}

		if (_hotelLostProperties.Remove(property))
		{
			Changed = true;
		}
	}

	public void CheckHotelLostProperty()
	{
		var now = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		foreach (var room in _hotelRooms.Where(x => x.ActiveRental?.EndTime <= now).ToList())
		{
			CompleteHotelStay(room, null, true);
		}

		foreach (var lost in _hotelLostProperties.OfType<HotelLostProperty>().ToList())
		{
			if (lost.Bundle is null || lost.Bundle.Deleted || lost.Bundle.Destroyed)
			{
				_hotelLostProperties.Remove(lost);
				Changed = true;
				continue;
			}

			switch (lost.Status)
			{
				case HotelLostPropertyStatus.Held when lost.StoredUntil <= now:
					ListLostPropertyForAuctionOrLiquidate(lost);
					break;
				case HotelLostPropertyStatus.ListedForAuction:
					ResolveAuctionedLostProperty(lost);
					break;
			}
		}
	}

	private void CollectLostProperty(HotelRoom room, ICharacter guest)
	{
		if (guest is null)
		{
			return;
		}

		var furnishingIds = room.Furnishings.Select(x => x.GameItemId).ToHashSet();
		bool HasNonFurnishingAncestor(IGameItem item)
		{
			var parent = item.ContainedIn;
			while (parent is not null)
			{
				if (!furnishingIds.Contains(parent.Id))
				{
					return true;
				}

				parent = parent.ContainedIn;
			}

			return false;
		}

		var items = room.Cell.GameItems
			.SelectMany(x => x.DeepItems)
			.Distinct()
			.Where(x => !furnishingIds.Contains(x.Id))
			.Where(x => !HasNonFurnishingAncestor(x))
			.ToList();

		if (!items.Any())
		{
			return;
		}

		IGameItem bundle;
		if (items.Count == 1)
		{
			bundle = items.Single();
			bundle.ContainedIn?.Take(bundle);
			bundle.InInventoryOf?.Take(bundle);
			bundle.Location?.Extract(bundle);
		}
		else
		{
			bundle = PileGameItemComponentProto.CreateNewBundle(items);
			Gameworld.Add(bundle);
		}

		bundle.SetOwner(guest);
		var reserve = items.Sum(HotelItemValue);
		var lost = new HotelLostProperty(room, guest, bundle,
			EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime + (TimeSpan)HotelLostPropertyRetention,
			reserve);
		_hotelLostProperties.Add(lost);
		bundle.Quit();
		Changed = true;
	}

	private decimal HotelItemValue(IGameItem item)
	{
		if (item is null)
		{
			return 0.0M;
		}

		if (item.Prototype.CostInBaseCurrency > 0.0M)
		{
			return item.Prototype.CostInBaseCurrency / EconomicZone.Currency.BaseCurrencyToGlobalBaseCurrencyConversion;
		}

		return item.DeepItems
			.Where(x => x != item)
			.Distinct()
			.Sum(HotelItemValue);
	}

	private IAuctionHouse DefaultHotelAuctionHouse()
	{
		return EconomicZone.EstateAuctionHouse ??
		       Gameworld.AuctionHouses.FirstOrDefault(x => x.EconomicZone == EconomicZone);
	}

	private void ListLostPropertyForAuctionOrLiquidate(HotelLostProperty lost)
	{
		var auctionHouse = DefaultHotelAuctionHouse();
		if (auctionHouse is null)
		{
			LiquidateLostProperty(lost);
			return;
		}

		auctionHouse.AddAuctionItem(new AuctionItem
		{
			Asset = lost.Bundle,
			Seller = this,
			PayoutTarget = this,
			ListingDateTime = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime,
			FinishingDateTime = new MudDateTime(EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime) + auctionHouse.DefaultListingTime,
			MinimumPrice = lost.ReservePrice,
			BuyoutPrice = null
		});
		lost.Status = HotelLostPropertyStatus.ListedForAuction;
		lost.AuctionHouseId = auctionHouse.Id;
		Changed = true;
	}

	private void ResolveAuctionedLostProperty(HotelLostProperty lost)
	{
		var auctionHouse = lost.AuctionHouseId.HasValue ? Gameworld.AuctionHouses.Get(lost.AuctionHouseId.Value) : null;
		if (auctionHouse is null)
		{
			LiquidateLostProperty(lost);
			return;
		}

		if (auctionHouse.ActiveAuctionItems.Any(x => x.Asset.FrameworkItemEquals(lost.BundleId, "GameItem")))
		{
			return;
		}

		var unclaimed = auctionHouse.UnclaimedItems.FirstOrDefault(x =>
			x.AuctionItem.Asset.FrameworkItemEquals(lost.BundleId, "GameItem") &&
			x.AuctionItem.Seller.FrameworkItemEquals(Id, FrameworkItemType));
		if (unclaimed is null)
		{
			_hotelLostProperties.Remove(lost);
			Changed = true;
			return;
		}

		if (unclaimed.WinningBid is not null)
		{
			_hotelLostProperties.Remove(lost);
			Changed = true;
			return;
		}

		auctionHouse.ClaimItem(unclaimed.AuctionItem);
		LiquidateLostProperty(lost);
	}

	private void LiquidateLostProperty(HotelLostProperty lost)
	{
		var bundle = lost.Bundle;
		var value = Math.Max(lost.ReservePrice, HotelItemValue(bundle));
		if (value > 0.0M)
		{
			VirtualCashLedger.CreditBankOrVirtual(this, EconomicZone.Currency, value, null, this, "Liquidation",
				$"Liquidated lost property from {Name}", HotelBankAccount,
				EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime, lost.Bundle, lost.Bundle?.Name);
		}

		bundle?.Delete();
		_hotelLostProperties.Remove(lost);
		Changed = true;
	}
}
