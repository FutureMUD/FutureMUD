using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Economy.Stables;

public class StableStay : SaveableItem, IStableStay
{
	private readonly List<IStableLedgerEntry> _ledgerEntries = new();
	private ICharacter? _mount;
	private ICharacter? _originalOwner;
	private MudDateTime? _closedDateTime;
	private StableStayStatus _status;
	private long? _ticketItemId;
	private string _ticketToken;
	private decimal _amountOwing;
	private MudDateTime _lastDailyFeeDateTime;

	public StableStay(IStable stable, ICharacter mount, ICharacter originalOwner)
	{
		Gameworld = stable.Gameworld;
		Stable = stable;
		MountId = mount.Id;
		_mount = mount;
		OriginalOwnerId = originalOwner.Id;
		_originalOwner = originalOwner;
		OriginalOwnerName = originalOwner.CurrentName;
		LodgedDateTime = new MudDateTime(stable.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime);
		_lastDailyFeeDateTime = new MudDateTime(LodgedDateTime);
		_status = StableStayStatus.Active;
		_ticketToken = GenerateTicketToken();

		using (new FMDB())
		{
			MudSharp.Models.StableStay dbitem = new()
			{
				StableId = stable.Id,
				MountId = mount.Id,
				OriginalOwnerId = originalOwner.Id,
				OriginalOwnerName = originalOwner.CurrentName.SaveToXml().ToString(),
				LodgedDateTime = LodgedDateTime.GetDateTimeString(),
				LastDailyFeeDateTime = LastDailyFeeDateTime.GetDateTimeString(),
				Status = (int)StableStayStatus.Active,
				TicketToken = _ticketToken,
				AmountOwing = 0.0M
			};
			FMDB.Context.StableStays.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public StableStay(MudSharp.Models.StableStay stay, IStable stable)
	{
		Gameworld = stable.Gameworld;
		_id = stay.Id;
		Stable = stable;
		MountId = stay.MountId;
		OriginalOwnerId = stay.OriginalOwnerId;
		OriginalOwnerName = string.IsNullOrEmpty(stay.OriginalOwnerName)
			? null
			: new PersonalName(XElement.Parse(stay.OriginalOwnerName), Gameworld);
		LodgedDateTime = new MudDateTime(stay.LodgedDateTime, Gameworld);
		_lastDailyFeeDateTime = new MudDateTime(stay.LastDailyFeeDateTime, Gameworld);
		_closedDateTime = string.IsNullOrEmpty(stay.ClosedDateTime)
			? null
			: new MudDateTime(stay.ClosedDateTime, Gameworld);
		_status = (StableStayStatus)stay.Status;
		_ticketItemId = stay.TicketItemId;
		_ticketToken = stay.TicketToken;
		_amountOwing = stay.AmountOwing;

		foreach (var entry in stay.LedgerEntries.OrderBy(x => x.Id))
		{
			_ledgerEntries.Add(new StableLedgerEntry(entry, this));
		}
	}

	public override string FrameworkItemType => "StableStay";

	public override void Save()
	{
		var dbitem = FMDB.Context.StableStays.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.LastDailyFeeDateTime = LastDailyFeeDateTime.GetDateTimeString();
		dbitem.ClosedDateTime = ClosedDateTime?.GetDateTimeString();
		dbitem.Status = (int)Status;
		dbitem.TicketItemId = TicketItemId;
		dbitem.TicketToken = TicketToken;
		dbitem.AmountOwing = AmountOwing;
		Changed = false;
	}

	public IStable Stable { get; }
	public ICharacter? Mount => _mount ??= Gameworld.TryGetCharacter(MountId, true);
	public long MountId { get; }
	public ICharacter? OriginalOwner => _originalOwner ??= Gameworld.TryGetCharacter(OriginalOwnerId, true);
	public long OriginalOwnerId { get; }
	public IPersonalName? OriginalOwnerName { get; }
	public MudDateTime LodgedDateTime { get; }

	public MudDateTime LastDailyFeeDateTime
	{
		get => _lastDailyFeeDateTime;
		set
		{
			_lastDailyFeeDateTime = value;
			Changed = true;
		}
	}

	public MudDateTime? ClosedDateTime => _closedDateTime;
	public StableStayStatus Status => _status;
	public bool IsActive => Status == StableStayStatus.Active;
	public long? TicketItemId => _ticketItemId;
	public string TicketToken => _ticketToken;
	public decimal AmountOwing => _amountOwing;
	public IEnumerable<IStableLedgerEntry> LedgerEntries => _ledgerEntries;

	public void RegisterTicket(long ticketItemId, string ticketToken)
	{
		_ticketItemId = ticketItemId;
		_ticketToken = ticketToken;
		Changed = true;
	}

	public void AddLedgerEntry(StableLedgerEntryType entryType, decimal amount, ICharacter? actor, string note)
	{
		var entry = new StableLedgerEntry(this, entryType,
			Stable.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime, actor, amount, note);
		using (new FMDB())
		{
			MudSharp.Models.StableStayLedgerEntry dbitem = new()
			{
				StableStayId = Id,
				EntryType = (int)entryType,
				MudDateTime = entry.MudDateTime.GetDateTimeString(),
				ActorId = actor?.Id,
				ActorName = actor?.PersonalName.GetName(NameStyle.FullName),
				Amount = amount,
				Note = note
			};
			FMDB.Context.StableStayLedgerEntries.Add(dbitem);
			FMDB.Context.SaveChanges();
			entry.Id = dbitem.Id;
		}
		_ledgerEntries.Add(entry);
	}

	public void AddCharge(StableLedgerEntryType entryType, decimal amount, ICharacter? actor, string note)
	{
		if (amount <= 0.0M)
		{
			return;
		}

		_amountOwing += amount;
		AddLedgerEntry(entryType, amount, actor, note);
		Changed = true;
	}

	public void AddPayment(decimal amount, ICharacter? actor, string note)
	{
		if (amount <= 0.0M)
		{
			return;
		}

		_amountOwing = Math.Max(0.0M, _amountOwing - amount);
		AddLedgerEntry(StableLedgerEntryType.Payment, -amount, actor, note);
		Changed = true;
	}

	public void WaiveOutstanding(ICharacter? actor, string note)
	{
		if (AmountOwing <= 0.0M)
		{
			return;
		}

		var waived = AmountOwing;
		_amountOwing = 0.0M;
		AddLedgerEntry(StableLedgerEntryType.Waiver, -waived, actor, note);
		Changed = true;
	}

	public void Close(StableStayStatus status, ICharacter? actor, string note)
	{
		_status = status;
		_closedDateTime = Stable.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		_ticketToken = $"{TicketToken}:closed:{Guid.NewGuid():N}";
		AddLedgerEntry(status == StableStayStatus.Redeemed ? StableLedgerEntryType.Redeemed : StableLedgerEntryType.ManagerRelease,
			0.0M, actor, note);
		Changed = true;
	}

	public bool TicketMatches(IGameItem item, string token)
	{
		return IsActive &&
		       TicketItemId == item.Id &&
		       TicketToken.EqualTo(token);
	}

	private static string GenerateTicketToken()
	{
		return Convert.ToHexString(RandomNumberGenerator.GetBytes(24));
	}
}
