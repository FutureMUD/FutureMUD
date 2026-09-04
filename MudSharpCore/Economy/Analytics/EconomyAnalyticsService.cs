#nullable enable

using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Economy.Analytics;

public sealed class EconomyAnalyticsService : IEconomyAnalyticsService
{
	public const string SnapshotsEnabledConfiguration = "EconomyAnalyticsSnapshotsEnabled";
	public const string SnapshotIntervalConfiguration = "EconomyAnalyticsSnapshotIntervalMinutes";
	public const string RolloverSnapshotsEnabledConfiguration = "EconomyAnalyticsRolloverSnapshotsEnabled";
	public static readonly TimeSpan MinimumSnapshotInterval = TimeSpan.FromHours(1.0);

	private readonly IFuturemud _gameworld;
	private readonly object _snapshotLock = new();
	private bool _initialised;
	private bool _snapshotInProgress;
	private readonly object _pendingActivityLock = new();
	private readonly List<EconomicActivityRecordItem> _pendingActivities = new();

	public EconomyAnalyticsService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public bool SnapshotsEnabled => _gameworld.GetStaticBool(SnapshotsEnabledConfiguration);
	public TimeSpan SnapshotInterval => TimeSpan.FromMinutes(
		Math.Max(MinimumSnapshotInterval.TotalMinutes,
			_gameworld.GetStaticDouble(SnapshotIntervalConfiguration)));
	public bool RolloverSnapshotsEnabled => _gameworld.GetStaticBool(RolloverSnapshotsEnabledConfiguration);
	public DateTime? LastSnapshotUtc { get; private set; }
	public DateTime? LastPeriodicSnapshotUtc { get; private set; }
	public DateTime? NextPeriodicSnapshotUtc => !SnapshotsEnabled
		? null
		: LastPeriodicSnapshotUtc.HasValue
			? EconomyAnalyticsMath.NextPeriodicDue(LastPeriodicSnapshotUtc.Value, SnapshotInterval)
			: DateTime.UtcNow;
	public DateTime? ActivityCoverageStartUtc { get; private set; }

	public void Initialise()
	{
		if (_initialised)
		{
			return;
		}

		using (new FMDB())
		{
			LastSnapshotUtc = FMDB.Context.EconomySnapshots
				.AsNoTracking()
				.Max(x => (DateTime?)x.RealDateTime);
			LastPeriodicSnapshotUtc = FMDB.Context.EconomySnapshots
				.AsNoTracking()
				.Where(x => x.Reason == (int)EconomySnapshotReason.Periodic ||
				            x.Reason == (int)EconomySnapshotReason.Baseline)
				.Max(x => (DateTime?)x.RealDateTime);
			ActivityCoverageStartUtc = FMDB.Context.EconomicActivityRecords
				.AsNoTracking()
				.Min(x => (DateTime?)x.RealDateTime);
		}

		_gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += CheckSnapshotSchedule;
		_initialised = true;
		if (SnapshotsEnabled && LastSnapshotUtc is null)
		{
			TakeSnapshot(EconomySnapshotReason.Baseline);
		}
	}

	private void CheckSnapshotSchedule()
	{
		if (!SnapshotsEnabled || _snapshotInProgress)
		{
			return;
		}

		if (LastPeriodicSnapshotUtc is null || DateTime.UtcNow >=
		    EconomyAnalyticsMath.NextPeriodicDue(LastPeriodicSnapshotUtc.Value, SnapshotInterval))
		{
			TakeSnapshot(LastPeriodicSnapshotUtc is null
				? EconomySnapshotReason.Baseline
				: EconomySnapshotReason.Periodic);
		}
	}

	public void RecordActivity(EconomicActivityEvent activity)
	{
		if (activity.Amount == 0.0M)
		{
			return;
		}

		var currency = _gameworld.Currencies.FirstOrDefault(x => x.Id == activity.CurrencyId);
		if (currency is null)
		{
			return;
		}

		var zone = activity.EconomicZoneId.HasValue
			? _gameworld.EconomicZones.FirstOrDefault(x => x.Id == activity.EconomicZoneId.Value)
			: null;
		var mudDate = zone?.FinancialPeriodReferenceCalendar.CurrentDate;
		var now = DateTime.UtcNow;
		var dbRecord = new Models.EconomicActivityRecord
		{
			RealDateTime = now,
				EconomicZoneId = zone?.Id,
				CurrencyId = currency.Id,
				FinancialPeriodId = zone?.CurrentFinancialPeriod?.Id,
				MudCalendarId = zone?.FinancialPeriodReferenceCalendar.Id,
				MudYear = mudDate?.Year,
				MudMonth = mudDate?.Month.NominalOrder,
				MudWeek = mudDate is null ? null : mudDate.DayNumberInYear() / 7,
				MudDay = mudDate?.Day,
				MudDateTime = zone?.ZoneForTimePurposes.DateTime().GetDateTimeString(),
				ActivityType = (int)activity.ActivityType,
				VolumeClassification = (int)activity.Classification,
				Amount = Math.Abs(activity.Amount),
				GlobalBaseValue = Math.Abs(activity.Amount) * currency.BaseCurrencyToGlobalBaseCurrencyConversion,
				SourceId = activity.SourceId,
				SourceType = activity.SourceType,
				SourceControlBucket = (int)ResolveControl(activity.SourceType, activity.SourceId),
				DestinationId = activity.DestinationId,
				DestinationType = activity.DestinationType,
				DestinationControlBucket = (int)ResolveControl(activity.DestinationType, activity.DestinationId),
				ReferenceId = activity.ReferenceId,
				ReferenceType = activity.ReferenceType,
				ReferenceText = activity.ReferenceText
		};
		var pending = new EconomicActivityRecordItem(_gameworld, dbRecord);
		pending.IdRegistered += PendingActivityIdRegistered;
		lock (_pendingActivityLock)
		{
			_pendingActivities.Add(pending);
		}

		ActivityCoverageStartUtc ??= now;
	}

	private void PendingActivityIdRegistered(MudSharp.Framework.Save.ILateInitialisingItem item)
	{
		lock (_pendingActivityLock)
		{
			_pendingActivities.Remove((EconomicActivityRecordItem)item);
		}
	}

	public EconomicControlBucket ResolveControl(string? frameworkItemType, long? frameworkItemId)
	{
		if (string.IsNullOrWhiteSpace(frameworkItemType) || !frameworkItemId.HasValue)
		{
			return EconomicControlBucket.Unclaimed;
		}

		if (frameworkItemType.EqualTo("Character") || frameworkItemType.EqualTo("ICharacter"))
		{
			return ResolveCharacterControl(frameworkItemId.Value);
		}

		if (frameworkItemType.EqualTo("Clan") || frameworkItemType.EqualTo("IClan"))
		{
			var clan = _gameworld.Clans.FirstOrDefault(x => x.Id == frameworkItemId.Value);
			if (clan is null)
			{
				return EconomicControlBucket.Institutional;
			}

			return clan.Memberships.Any(x =>
				!x.IsArchivedMembership &&
				ResolveCharacterControl(x.MemberId) == EconomicControlBucket.DirectPc &&
				(x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageBankAccounts) ||
				 x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewTreasury) ||
				 x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty)))
				? EconomicControlBucket.SharedPcControlled
				: EconomicControlBucket.Institutional;
		}

		if (frameworkItemType.EqualTo("Shop") || frameworkItemType.EqualTo("IShop"))
		{
			var shop = _gameworld.Shops.FirstOrDefault(x => x.Id == frameworkItemId.Value);
			return shop is not null && _gameworld.Characters.Any(x =>
				ResolveCharacterControl(x.Id) == EconomicControlBucket.DirectPc &&
				(shop.IsManager(x) || shop.IsProprietor(x)))
				? EconomicControlBucket.SharedPcControlled
				: EconomicControlBucket.Institutional;
		}

		if (frameworkItemType.EqualTo("Property") || frameworkItemType.EqualTo("IProperty"))
		{
			var property = _gameworld.Properties.FirstOrDefault(x => x.Id == frameworkItemId.Value);
			if (property is null)
			{
				return EconomicControlBucket.Institutional;
			}

			var controls = property.PropertyOwners
				.Select(x => ResolveControl(x.OwnerFrameworkItemType, x.OwnerId))
				.Append(property.Lease is null
					? EconomicControlBucket.Institutional
					: ResolveControl(property.Lease.Leaseholder.FrameworkItemType, property.Lease.Leaseholder.Id));
			return controls.Any(IsPcControlled)
				? EconomicControlBucket.SharedPcControlled
				: EconomicControlBucket.Institutional;
		}

		return EconomicControlBucket.Institutional;
	}

	private EconomicControlBucket ResolveCharacterControl(long characterId)
	{
		using (new FMDB())
		{
			var character = FMDB.Context.Characters
				.AsNoTracking()
				.Where(x => x.Id == characterId)
				.Select(x => new { x.Id, x.AccountId, x.IsAdminAvatar })
				.FirstOrDefault();
			if (character is null)
			{
				return EconomicControlBucket.Ambiguous;
			}

			if (character.IsAdminAvatar)
			{
				return EconomicControlBucket.Staff;
			}
			if (FMDB.Context.Guests.AsNoTracking().Any(x => x.CharacterId == characterId))
			{
				return EconomicControlBucket.Npc;
			}

			var npc = FMDB.Context.Npcs
				.AsNoTracking()
				.FirstOrDefault(x => x.CharacterId == characterId);
			if (npc is null)
			{
				return character.AccountId.HasValue
					? EconomicControlBucket.DirectPc
					: EconomicControlBucket.Ambiguous;
			}

			if (npc.BodyguardCharacterId.HasValue &&
			    ResolveCharacterControl(npc.BodyguardCharacterId.Value) == EconomicControlBucket.DirectPc)
			{
				return EconomicControlBucket.SharedPcControlled;
			}

			return EconomicControlBucket.Npc;
		}
	}

	private static bool IsPcControlled(EconomicControlBucket bucket)
	{
		return bucket is EconomicControlBucket.DirectPc or EconomicControlBucket.SharedPcControlled;
	}

	public IReadOnlyList<EconomyHolding> GetCurrentHoldings(long? economicZoneId = null, long? currencyId = null)
	{
		var holdings = new List<EconomyHolding>();
		AddPhysicalCashHoldings(holdings);

		foreach (var account in _gameworld.BankAccounts)
		{
			var amount = account.CurrentBalance;
			var metric = amount >= 0.0M ? EconomyHoldingMetric.BankDeposits : EconomyHoldingMetric.BankDebt;
			var owner = account.AccountOwner;
			holdings.Add(CreateHolding(account.Bank.EconomicZone.Id, account.Currency, metric,
				ResolveControl(owner.FrameworkItemType, owner.Id), Math.Abs(amount), owner.Id,
				owner.FrameworkItemType, account.AccountReference));
		}

		foreach (var bank in _gameworld.Banks)
		{
			foreach (var reserve in bank.CurrencyReserves)
			{
				holdings.Add(CreateHolding(bank.EconomicZone.Id, reserve.Key, EconomyHoldingMetric.BankReserves,
					EconomicControlBucket.Institutional, reserve.Value, bank.Id, bank.FrameworkItemType, bank.Name));
			}
		}

		foreach (var shop in _gameworld.Shops.Where(x => x.CashBalance != 0.0M))
		{
			holdings.Add(CreateHolding(shop.EconomicZone.Id, shop.Currency, EconomyHoldingMetric.VirtualBalance,
				ResolveControl(shop.FrameworkItemType, shop.Id), shop.CashBalance, shop.Id,
				shop.FrameworkItemType, shop.Name));
		}

		using (new FMDB())
		{
			var virtualBalances = FMDB.Context.VirtualCashBalances
				.AsNoTracking()
				.Where(x => x.Balance != 0.0M && x.OwnerType != "EconomicZone" && x.OwnerType != "Shop")
				.ToList();
			foreach (var balance in virtualBalances)
			{
				var currency = _gameworld.Currencies.FirstOrDefault(x => x.Id == balance.CurrencyId);
				if (currency is null)
				{
					continue;
				}

				var zoneId = ResolveEconomicZone(balance.OwnerType, balance.OwnerId);
				holdings.Add(CreateHolding(zoneId, currency, EconomyHoldingMetric.VirtualBalance,
					ResolveControl(balance.OwnerType, balance.OwnerId), balance.Balance, balance.OwnerId,
					balance.OwnerType, $"{balance.OwnerType} #{balance.OwnerId:N0}"));
			}
		}

		foreach (var property in _gameworld.Properties)
		{
			var value = property.SaleOrder is { ShowForSale: true }
				? property.SaleOrder.ReservePrice
				: property.LastSaleValue;
			if (value <= 0.0M)
			{
				continue;
			}

			foreach (var owner in property.PropertyOwners)
			{
				var amount = value * owner.ShareOfOwnership;
				holdings.Add(CreateHolding(property.EconomicZone.Id, property.EconomicZone.Currency,
					EconomyHoldingMetric.PropertyEquity,
					ResolveControl(owner.OwnerFrameworkItemType, owner.OwnerId), amount, owner.OwnerId,
					owner.OwnerFrameworkItemType, property.Name));
			}
		}

		return holdings
			.Where(x => (!economicZoneId.HasValue || x.EconomicZoneId == economicZoneId) &&
			            (!currencyId.HasValue || x.CurrencyId == currencyId))
			.ToList();
	}

	private EconomyHolding CreateHolding(long? zoneId, ICurrency currency, EconomyHoldingMetric metric,
		EconomicControlBucket bucket, decimal amount, long? controllerId = null, string? controllerType = null,
		string? description = null)
	{
		return new EconomyHolding(zoneId, currency.Id, metric, bucket, amount,
			amount * currency.BaseCurrencyToGlobalBaseCurrencyConversion, controllerId, controllerType, description);
	}

	private void AddPhysicalCashHoldings(List<EconomyHolding> holdings)
	{
		using (new FMDB())
		{
			var coinValues = FMDB.Context.Coins
				.AsNoTracking()
				.ToDictionary(x => x.Id, x => (x.CurrencyId, x.Value));
			var currencyComponents = (from component in FMDB.Context.GameItemComponents.AsNoTracking()
				join prototype in FMDB.Context.GameItemComponentProtos.AsNoTracking()
					on new { Id = component.GameItemComponentProtoId, Revision = component.GameItemComponentProtoRevision }
					equals new { prototype.Id, Revision = prototype.RevisionNumber }
				join item in FMDB.Context.GameItems.AsNoTracking() on component.GameItemId equals item.Id
				where prototype.Type == "Currency"
				select new
				{
					component.GameItemId,
					component.Definition,
					item.ContainerId,
					item.OwnerId,
					item.OwnerType
				}).ToList();
			var itemParents = FMDB.Context.GameItems.AsNoTracking()
				.Where(x => x.ContainerId != null)
				.ToDictionary(x => x.Id, x => x.ContainerId);
			var itemOwners = FMDB.Context.GameItems.AsNoTracking()
				.Where(x => x.OwnerId != null && x.OwnerType != null)
				.ToDictionary(x => x.Id, x => (x.OwnerType, x.OwnerId));
			var bodyItems = FMDB.Context.BodiesGameItems.AsNoTracking()
				.ToDictionary(x => x.GameItemId, x => x.BodyId);
			var cellItems = FMDB.Context.CellsGameItems.AsNoTracking()
				.ToDictionary(x => x.GameItemId, x => x.CellId);
			var tillItems = FMDB.Context.ShopsTills.AsNoTracking()
				.ToDictionary(x => x.GameItemId, x => x.ShopId);
			var treasuryCells = FMDB.Context.ClansTreasuryCells.AsNoTracking()
				.GroupBy(x => x.CellId)
				.ToDictionary(x => x.Key, x => x.Select(y => y.ClanId).ToList());
			var propertyCells = FMDB.Context.PropertyLocations.AsNoTracking()
				.GroupBy(x => x.CellId)
				.ToDictionary(x => x.Key, x => x.Select(y => y.PropertyId).ToList());

			foreach (var component in currencyComponents)
			{
				long rootId = component.GameItemId;
				var visited = new HashSet<long>();
				while (itemParents.TryGetValue(rootId, out var parentId) && parentId.HasValue && visited.Add(rootId))
				{
					rootId = parentId.Value;
				}

				var rootOwner = itemOwners.GetValueOrDefault(rootId);
				var (ownerType, ownerId, bucket, zoneId, description) = ResolvePhysicalCustody(
					rootOwner.OwnerType ?? component.OwnerType, rootOwner.OwnerId ?? component.OwnerId,
					rootId, bodyItems, cellItems, tillItems,
					treasuryCells, propertyCells);
				var loaded = _gameworld.Items.FirstOrDefault(x => x.Id == component.GameItemId)
					?.GetItemType<ICurrencyPile>();
				if (loaded is not null)
				{
					holdings.Add(CreateHolding(zoneId, loaded.Currency, EconomyHoldingMetric.PhysicalCash,
						bucket, loaded.TotalValue, ownerId, ownerType, description));
					continue;
				}

				try
				{
					var root = XElement.Parse(component.Definition);
					var amounts = root.Descendants("Coin")
						.Select(x => new
						{
							Id = long.Parse(x.Attribute("Id")!.Value),
							Count = int.Parse(x.Attribute("Count")!.Value)
						})
						.Where(x => coinValues.ContainsKey(x.Id))
						.GroupBy(x => coinValues[x.Id].CurrencyId);
					foreach (var currencyGroup in amounts)
					{
						var currency = _gameworld.Currencies.FirstOrDefault(x => x.Id == currencyGroup.Key);
						if (currency is null)
						{
							continue;
						}

						var amount = currencyGroup.Sum(x => coinValues[x.Id].Value * x.Count);
						holdings.Add(CreateHolding(zoneId, currency, EconomyHoldingMetric.PhysicalCash,
							bucket, amount, ownerId, ownerType, description));
					}
				}
				catch (Exception)
				{
					holdings.Add(new EconomyHolding(zoneId, 0, EconomyHoldingMetric.PhysicalCash,
						EconomicControlBucket.Ambiguous, 0.0M, 0.0M, ownerId, ownerType,
						$"Malformed currency component on item #{component.GameItemId:N0}"));
				}
			}
		}
	}

	private (string? OwnerType, long? OwnerId, EconomicControlBucket Bucket, long? ZoneId, string Description)
		ResolvePhysicalCustody(string? explicitOwnerType, long? explicitOwnerId, long rootItemId,
			IReadOnlyDictionary<long, long> bodyItems, IReadOnlyDictionary<long, long> cellItems,
			IReadOnlyDictionary<long, long> tillItems, IReadOnlyDictionary<long, List<long>> treasuryCells,
			IReadOnlyDictionary<long, List<long>> propertyCells)
	{
		if (!string.IsNullOrWhiteSpace(explicitOwnerType) && explicitOwnerId.HasValue)
		{
			return (explicitOwnerType, explicitOwnerId, ResolveControl(explicitOwnerType, explicitOwnerId),
				ResolveEconomicZone(explicitOwnerType, explicitOwnerId.Value), "explicit ownership");
		}

		if (bodyItems.TryGetValue(rootItemId, out var bodyId))
		{
			using (new FMDB())
			{
				var characterId = FMDB.Context.Characters.AsNoTracking()
					.Where(x => x.BodyId == bodyId)
					.Select(x => (long?)x.Id)
					.FirstOrDefault() ?? FMDB.Context.CharacterInstances.AsNoTracking()
					.Where(x => x.BodyId == bodyId && x.IsControllable)
					.Select(x => x.PrimaryCharacterId ?? x.CharacterId)
					.Cast<long?>()
					.FirstOrDefault();
				if (characterId.HasValue)
				{
					return ("Character", characterId, ResolveCharacterControl(characterId.Value),
						ResolveCharacterZone(characterId.Value), "body custody");
				}
			}
		}

		if (tillItems.TryGetValue(rootItemId, out var shopId))
		{
			return ("Shop", shopId, ResolveControl("Shop", shopId), ResolveEconomicZone("Shop", shopId),
				"shop register");
		}

		if (!cellItems.TryGetValue(rootItemId, out var cellId))
		{
			return (null, null, EconomicControlBucket.Unclaimed, null, "no owner or location");
		}

		var claims = new List<(string Type, long Id)>();
		if (treasuryCells.TryGetValue(cellId, out var clans))
		{
			claims.AddRange(clans.Select(x => ("Clan", x)));
		}

		if (propertyCells.TryGetValue(cellId, out var properties))
		{
			claims.AddRange(properties.Select(x => ("Property", x)));
		}

		if (claims.Count > 1)
		{
			return (null, null, EconomicControlBucket.Ambiguous, ResolveCellZone(cellId),
				"conflicting treasury or property custody");
		}

		if (claims.Count == 1)
		{
			var claim = claims[0];
			return (claim.Type, claim.Id, ResolveControl(claim.Type, claim.Id),
				ResolveEconomicZone(claim.Type, claim.Id), $"{claim.Type.ToLowerInvariant()} custody");
		}

		return (null, null, EconomicControlBucket.Unclaimed, ResolveCellZone(cellId), "unclaimed cell cash");
	}

	private long? ResolveEconomicZone(string ownerType, long ownerId)
	{
		if (ownerType.EqualTo("Shop"))
		{
			return _gameworld.Shops.FirstOrDefault(x => x.Id == ownerId)?.EconomicZone.Id;
		}

		if (ownerType.EqualTo("Bank"))
		{
			return _gameworld.Banks.FirstOrDefault(x => x.Id == ownerId)?.EconomicZone.Id;
		}

		if (ownerType.EqualTo("Property"))
		{
			return _gameworld.Properties.FirstOrDefault(x => x.Id == ownerId)?.EconomicZone.Id;
		}

		if (ownerType.EqualTo("Character"))
		{
			return ResolveCharacterZone(ownerId);
		}

		return null;
	}

	private long? ResolveCharacterZone(long characterId)
	{
		using (new FMDB())
		{
			var cellId = FMDB.Context.Characters.AsNoTracking()
				.Where(x => x.Id == characterId)
				.Select(x => (long?)x.Location)
				.FirstOrDefault();
			return cellId.HasValue ? ResolveCellZone(cellId.Value) : null;
		}
	}

	private long? ResolveCellZone(long cellId)
	{
		var cell = _gameworld.Cells.FirstOrDefault(x => x.Id == cellId);
		return cell is null
			? null
			: _gameworld.EconomicZones.FirstOrDefault(x => x.ZoneForTimePurposes == cell.Zone)?.Id;
	}

	public EconomyVolumeResult GetVolume(EconomyQueryWindowKind window, long? economicZoneId = null,
		long? currencyId = null, long? financialPeriodId = null)
	{
		var now = DateTime.UtcNow;
		var start = window switch
		{
			EconomyQueryWindowKind.RealDay => now.AddDays(-1),
			EconomyQueryWindowKind.RealWeek => now.AddDays(-7),
			EconomyQueryWindowKind.RealMonth => now.AddMonths(-1),
			_ => DateTime.MinValue
		};
		using (new FMDB())
		{
			var query = FMDB.Context.EconomicActivityRecords.AsNoTracking();
			if (economicZoneId.HasValue)
			{
				query = query.Where(x => x.EconomicZoneId == economicZoneId);
			}

			if (currencyId.HasValue)
			{
				query = query.Where(x => x.CurrencyId == currencyId);
			}

			if (window is EconomyQueryWindowKind.RealDay or EconomyQueryWindowKind.RealWeek or EconomyQueryWindowKind.RealMonth)
			{
				query = query.Where(x => x.RealDateTime >= start && x.RealDateTime <= now);
			}
			else if (window == EconomyQueryWindowKind.FinancialPeriod)
			{
				query = query.Where(x => x.FinancialPeriodId == financialPeriodId);
			}
			else
			{
				var zone = economicZoneId.HasValue
					? _gameworld.EconomicZones.FirstOrDefault(x => x.Id == economicZoneId.Value)
					: null;
				var date = zone?.FinancialPeriodReferenceCalendar.CurrentDate;
				if (zone is null || date is null)
				{
					return EmptyVolume(start, now);
				}

				query = query.Where(x => x.MudCalendarId == zone.FinancialPeriodReferenceCalendar.Id &&
				                         x.MudYear == date.Year);
				query = window switch
				{
					EconomyQueryWindowKind.MudDay => query.Where(x => x.MudMonth == date.Month.NominalOrder && x.MudDay == date.Day),
					EconomyQueryWindowKind.MudWeek => query.Where(x => x.MudWeek == date.DayNumberInYear() / 7),
					EconomyQueryWindowKind.MudMonth => query.Where(x => x.MudMonth == date.Month.NominalOrder),
					_ => query
				};
			}

			var records = query.ToList();
			lock (_pendingActivityLock)
			{
				records.AddRange(_pendingActivities
					.Select(x => x.Record)
					.Where(x => (!economicZoneId.HasValue || x.EconomicZoneId == economicZoneId) &&
					            (!currencyId.HasValue || x.CurrencyId == currencyId) &&
					            (window switch
					            {
						            EconomyQueryWindowKind.RealDay or EconomyQueryWindowKind.RealWeek or EconomyQueryWindowKind.RealMonth =>
							            x.RealDateTime >= start && x.RealDateTime <= now,
						            EconomyQueryWindowKind.FinancialPeriod => x.FinancialPeriodId == financialPeriodId,
						            _ => MatchesCurrentMudWindow(x, window, economicZoneId)
					            })));
			}
			var exchange = records
				.Where(x => ((EconomicVolumeClassification)x.VolumeClassification)
					.HasFlag(EconomicVolumeClassification.Exchange))
				.Sum(x => x.GlobalBaseValue);
			var movement = records
				.Where(x => (((EconomicVolumeClassification)x.VolumeClassification) &
				             (EconomicVolumeClassification.Exchange | EconomicVolumeClassification.GeneralTransfer |
				              EconomicVolumeClassification.Source | EconomicVolumeClassification.Sink)) != 0)
				.Sum(x => x.GlobalBaseValue);
			var byActivity = records
				.GroupBy(x => (EconomicActivityType)x.ActivityType)
				.ToDictionary(x => x.Key, x => x.Sum(y => y.GlobalBaseValue));
			var byPc = records
				.GroupBy(x => IsPcControlled((EconomicControlBucket)x.SourceControlBucket) ||
				              IsPcControlled((EconomicControlBucket)x.DestinationControlBucket)
					? EconomicControlBucket.SharedPcControlled
					: EconomicControlBucket.Institutional)
				.ToDictionary(x => x.Key, x => x.Sum(y => y.GlobalBaseValue));
			return new EconomyVolumeResult(ActivityCoverageStartUtc ?? now, start, now, exchange, movement,
				byActivity, byPc, records.Count);
		}
	}

	private bool MatchesCurrentMudWindow(Models.EconomicActivityRecord record, EconomyQueryWindowKind window,
		long? economicZoneId)
	{
		var zone = economicZoneId.HasValue
			? _gameworld.EconomicZones.FirstOrDefault(x => x.Id == economicZoneId.Value)
			: null;
		var date = zone?.FinancialPeriodReferenceCalendar.CurrentDate;
		if (zone is null || date is null || record.MudCalendarId != zone.FinancialPeriodReferenceCalendar.Id ||
		    record.MudYear != date.Year)
		{
			return false;
		}

		return window switch
		{
			EconomyQueryWindowKind.MudDay => record.MudMonth == date.Month.NominalOrder && record.MudDay == date.Day,
			EconomyQueryWindowKind.MudWeek => record.MudWeek == date.DayNumberInYear() / 7,
			EconomyQueryWindowKind.MudMonth => record.MudMonth == date.Month.NominalOrder,
			_ => false
		};
	}

	private EconomyVolumeResult EmptyVolume(DateTime start, DateTime end)
	{
		return new EconomyVolumeResult(ActivityCoverageStartUtc ?? end, start, end, 0.0M, 0.0M,
			new Dictionary<EconomicActivityType, decimal>(),
			new Dictionary<EconomicControlBucket, decimal>(), 0);
	}

	public IReadOnlyList<EconomySnapshotPoint> GetTrends(EconomyHoldingMetric? metric,
		EconomicVolumeClassification? volumeClassification, long? economicZoneId = null,
		long? currencyId = null, int count = 30)
	{
		var selectedMetric = metric ?? (volumeClassification == EconomicVolumeClassification.Exchange
			? EconomyHoldingMetric.ExchangeVolume
			: EconomyHoldingMetric.GrossMovement);
		using (new FMDB())
		{
			var query = FMDB.Context.EconomySnapshotEntries
				.AsNoTracking()
				.Where(x => x.Metric == (int)selectedMetric);
			if (economicZoneId.HasValue)
			{
				query = query.Where(x => x.EconomySnapshot.EconomicZoneId == economicZoneId);
			}

			if (currencyId.HasValue)
			{
				query = query.Where(x => x.CurrencyId == currencyId);
			}

			return query
				.GroupBy(x => new
				{
					x.EconomySnapshotId,
					x.EconomySnapshot.RealDateTime,
					x.EconomySnapshot.EconomicZoneId,
					x.EconomySnapshot.FinancialPeriodId,
					x.EconomySnapshot.Reason
				})
				.Select(x => new EconomySnapshotPoint(x.Key.EconomySnapshotId, x.Key.RealDateTime,
					x.Key.EconomicZoneId, x.Key.FinancialPeriodId, (EconomySnapshotReason)x.Key.Reason,
					x.Sum(y => y.GlobalBaseValue)))
				.OrderByDescending(x => x.RealDateTimeUtc)
				.Take(Math.Clamp(count, 1, 100))
				.ToList();
		}
	}

	public IReadOnlyList<EconomyRisk> GetRisks(long? economicZoneId = null)
	{
		var risks = new List<EconomyRisk>();
		var holdings = GetCurrentHoldings(economicZoneId);
		foreach (var group in holdings.GroupBy(x => new { x.EconomicZoneId, x.CurrencyId }))
		{
			var deposits = group.Where(x => x.Metric == EconomyHoldingMetric.BankDeposits).Sum(x => x.Amount);
			var reserves = group.Where(x => x.Metric == EconomyHoldingMetric.BankReserves).Sum(x => x.Amount);
			if (deposits > 0.0M && reserves < deposits)
			{
				risks.Add(new EconomyRisk("reserve-shortfall",
					$"Bank reserves cover {reserves / deposits:P1} of positive deposits.",
					(deposits - reserves) * GetConversion(group.Key.CurrencyId), group.Key.EconomicZoneId,
					group.Key.CurrencyId));
			}

			var debt = group.Where(x => x.Metric == EconomyHoldingMetric.BankDebt).Sum(x => x.Amount);
			if (debt > 0.0M)
			{
				risks.Add(new EconomyRisk("negative-balances", "Customer accounts have outstanding overdraft debt.",
					debt * GetConversion(group.Key.CurrencyId), group.Key.EconomicZoneId, group.Key.CurrencyId));
			}
		}

		var ambiguous = holdings
			.Where(x => x.Metric == EconomyHoldingMetric.PhysicalCash &&
			            x.ControlBucket is EconomicControlBucket.Ambiguous or EconomicControlBucket.Unclaimed)
			.Sum(x => x.GlobalBaseValue);
		if (ambiguous > 0.0M)
		{
			risks.Add(new EconomyRisk("unclaimed-cash", "Physical cash has ambiguous or unclaimed custody.", ambiguous,
				economicZoneId));
		}
		var malformedCount = holdings.Count(x => x.Metric == EconomyHoldingMetric.PhysicalCash &&
			x.Description?.StartsWith("Malformed", StringComparison.InvariantCultureIgnoreCase) == true);
		if (malformedCount > 0)
		{
			risks.Add(new EconomyRisk("malformed-currency", $"{malformedCount:N0} persisted currency components could not be parsed."));
		}

		var virtualLiabilities = holdings
			.Where(x => x.Metric == EconomyHoldingMetric.VirtualBalance && x.Amount < 0.0M)
			.Sum(x => Math.Abs(x.GlobalBaseValue));
		if (virtualLiabilities > 0.0M)
		{
			risks.Add(new EconomyRisk("virtual-liabilities", "One or more virtual host balances are negative.", virtualLiabilities,
				economicZoneId));
		}

		if (!SnapshotsEnabled)
		{
			risks.Add(new EconomyRisk("snapshots-disabled", "Snapshot collection is disabled; existing history is preserved."));
		}
		else if (LastSnapshotUtc.HasValue && DateTime.UtcNow - LastSnapshotUtc.Value > SnapshotInterval + TimeSpan.FromMinutes(10))
		{
			risks.Add(new EconomyRisk("stale-snapshot", "The most recent snapshot is older than the configured interval."));
		}

		return risks;
	}

	private decimal GetConversion(long currencyId)
	{
		return _gameworld.Currencies.FirstOrDefault(x => x.Id == currencyId)
			?.BaseCurrencyToGlobalBaseCurrencyConversion ?? 0.0M;
	}

	public long? TakeSnapshot(EconomySnapshotReason reason, long? economicZoneId = null,
		long? financialPeriodId = null)
	{
		if (!SnapshotsEnabled)
		{
			return null;
		}

		lock (_snapshotLock)
		{
			if (_snapshotInProgress)
			{
				return null;
			}

			_snapshotInProgress = true;
		}

		try
		{
			using (new FMDB())
			{
				if (reason == EconomySnapshotReason.FinancialPeriodRollover && economicZoneId.HasValue &&
				    financialPeriodId.HasValue && FMDB.Context.EconomySnapshots.AsNoTracking().Any(x =>
					    x.EconomicZoneId == economicZoneId && x.FinancialPeriodId == financialPeriodId &&
					    x.Reason == (int)reason))
				{
					return null;
				}

				var now = DateTime.UtcNow;
				var zone = economicZoneId.HasValue
					? _gameworld.EconomicZones.FirstOrDefault(x => x.Id == economicZoneId.Value)
					: null;
				var snapshot = new Models.EconomySnapshot
				{
					RealDateTime = now,
					EconomicZoneId = economicZoneId,
					FinancialPeriodId = financialPeriodId,
					MudDateTime = zone?.ZoneForTimePurposes.DateTime().GetDateTimeString(),
					Reason = (int)reason
				};
				FMDB.Context.EconomySnapshots.Add(snapshot);
				var holdings = GetCurrentHoldings(economicZoneId);
				foreach (var group in holdings
				         .Where(x => x.CurrencyId > 0)
				         .GroupBy(x => new { x.CurrencyId, x.Metric, x.ControlBucket }))
				{
					snapshot.Entries.Add(new Models.EconomySnapshotEntry
					{
						CurrencyId = group.Key.CurrencyId,
						Metric = (int)group.Key.Metric,
						ControlBucket = (int)group.Key.ControlBucket,
						Amount = group.Sum(x => x.Amount),
						GlobalBaseValue = group.Sum(x => x.GlobalBaseValue),
						EntityCount = group.Count()
					});
				}

				var dayVolume = GetVolume(EconomyQueryWindowKind.RealDay, economicZoneId);
				var primaryCurrency = zone?.Currency ?? _gameworld.Currencies.FirstOrDefault();
				if (primaryCurrency is not null)
				{
					var broadSupply = holdings
						.Where(x => x.Metric is EconomyHoldingMetric.PhysicalCash or EconomyHoldingMetric.BankDeposits or
							EconomyHoldingMetric.VirtualBalance)
						.Sum(x => x.GlobalBaseValue);
					var pcWealth = holdings
						.Where(x => IsPcControlled(x.ControlBucket) &&
						            x.Metric is not EconomyHoldingMetric.BankDebt and not EconomyHoldingMetric.BankReserves)
						.Sum(x => x.GlobalBaseValue);
					var deposits = holdings.Where(x => x.Metric == EconomyHoldingMetric.BankDeposits)
						.Sum(x => x.GlobalBaseValue);
					var reserves = holdings.Where(x => x.Metric == EconomyHoldingMetric.BankReserves)
						.Sum(x => x.GlobalBaseValue);
					foreach (var aggregate in new[]
					         {
						         (EconomyHoldingMetric.BroadMoneySupply, broadSupply),
						         (EconomyHoldingMetric.PcControlledWealth, pcWealth),
						         (EconomyHoldingMetric.ReserveCoverage, deposits <= 0.0M ? 1.0M : reserves / deposits)
					         })
					{
						snapshot.Entries.Add(new Models.EconomySnapshotEntry
						{
							CurrencyId = primaryCurrency.Id,
							Metric = (int)aggregate.Item1,
							ControlBucket = (int)EconomicControlBucket.Institutional,
							GlobalBaseValue = aggregate.Item2,
							Amount = aggregate.Item2,
							EntityCount = holdings.Count
						});
					}

					snapshot.Entries.Add(new Models.EconomySnapshotEntry
					{
						CurrencyId = primaryCurrency.Id,
						Metric = (int)EconomyHoldingMetric.ExchangeVolume,
						ControlBucket = (int)EconomicControlBucket.Institutional,
						GlobalBaseValue = dayVolume.ExchangeGlobalBaseValue,
						Amount = dayVolume.ExchangeGlobalBaseValue /
						         primaryCurrency.BaseCurrencyToGlobalBaseCurrencyConversion,
						EntityCount = (int)Math.Min(int.MaxValue, dayVolume.EventCount)
					});
					snapshot.Entries.Add(new Models.EconomySnapshotEntry
					{
						CurrencyId = primaryCurrency.Id,
						Metric = (int)EconomyHoldingMetric.GrossMovement,
						ControlBucket = (int)EconomicControlBucket.Institutional,
						GlobalBaseValue = dayVolume.MovementGlobalBaseValue,
						Amount = dayVolume.MovementGlobalBaseValue /
						         primaryCurrency.BaseCurrencyToGlobalBaseCurrencyConversion,
						EntityCount = (int)Math.Min(int.MaxValue, dayVolume.EventCount)
					});
				}

				FMDB.Context.SaveChanges();
				LastSnapshotUtc = now;
				if (reason is EconomySnapshotReason.Baseline or EconomySnapshotReason.Periodic)
				{
					LastPeriodicSnapshotUtc = now;
				}

				return snapshot.Id;
			}
		}
		finally
		{
			_snapshotInProgress = false;
		}
	}

	public void NotifyFinancialPeriodClosed(long economicZoneId, long financialPeriodId)
	{
		if (SnapshotsEnabled && RolloverSnapshotsEnabled)
		{
			TakeSnapshot(EconomySnapshotReason.FinancialPeriodRollover, economicZoneId, financialPeriodId);
		}
	}

	public void SetSnapshotsEnabled(bool enabled)
	{
		SaveConfiguration(SnapshotsEnabledConfiguration, enabled.ToString().ToLowerInvariant());
		if (enabled && LastSnapshotUtc is null)
		{
			TakeSnapshot(EconomySnapshotReason.Baseline);
		}
	}

	public bool TrySetSnapshotInterval(TimeSpan interval, out string error)
	{
		if (!EconomyAnalyticsMath.IsValidSnapshotInterval(interval))
		{
			error = "The snapshot interval must be at least one hour.";
			return false;
		}

		SaveConfiguration(SnapshotIntervalConfiguration,
			interval.TotalMinutes.ToString(System.Globalization.CultureInfo.InvariantCulture));
		error = string.Empty;
		return true;
	}

	public void SetRolloverSnapshotsEnabled(bool enabled)
	{
		SaveConfiguration(RolloverSnapshotsEnabledConfiguration, enabled.ToString().ToLowerInvariant());
	}

	private void SaveConfiguration(string name, string value)
	{
		using (new FMDB())
		{
			var setting = FMDB.Context.StaticConfigurations.Find(name);
			if (setting is null)
			{
				setting = new Models.StaticConfiguration { SettingName = name };
				FMDB.Context.StaticConfigurations.Add(setting);
			}

			setting.Definition = value;
			FMDB.Context.SaveChanges();
		}

		_gameworld.UpdateStaticConfiguration(name, value);
	}
}
