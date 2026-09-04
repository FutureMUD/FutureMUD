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
	internal sealed class EconomyVolumeAggregateRow
	{
		public int ActivityType { get; init; }
		public bool PcInvolved { get; init; }
		public long EventCount { get; init; }
		public decimal ExchangeGlobalBaseValue { get; init; }
		public decimal MovementGlobalBaseValue { get; init; }
		public decimal TotalGlobalBaseValue { get; init; }
	}

	private sealed record CharacterControlRecord(long Id, long? AccountId, bool IsAdminAvatar, bool IsGuest,
		bool IsNpc, long? BodyguardCharacterId);

	public const string SnapshotsEnabledConfiguration = "EconomyAnalyticsSnapshotsEnabled";
	public const string SnapshotIntervalConfiguration = "EconomyAnalyticsSnapshotIntervalMinutes";
	public const string RolloverSnapshotsEnabledConfiguration = "EconomyAnalyticsRolloverSnapshotsEnabled";
	public const string GlobalDisplayCurrencyConfiguration = "EconomyAnalyticsGlobalDisplayCurrencyId";
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
	public ICurrency GlobalDisplayCurrency
	{
		get
		{
			var firstCurrency = _gameworld.Currencies.FirstOrDefault() ??
			                    throw new InvalidOperationException(
				                    "Economy analytics requires at least one currency.");
			var configuredCurrency = _gameworld.Currencies.Get(
				_gameworld.GetStaticLong(GlobalDisplayCurrencyConfiguration));
			if (configuredCurrency?.BaseCurrencyToGlobalBaseCurrencyConversion > 0.0M)
			{
				return configuredCurrency;
			}

			if (firstCurrency.BaseCurrencyToGlobalBaseCurrencyConversion > 0.0M)
			{
				return firstCurrency;
			}

			return _gameworld.Currencies.FirstOrDefault(x =>
			           x.BaseCurrencyToGlobalBaseCurrencyConversion > 0.0M) ??
			       throw new InvalidOperationException(
				       "Economy analytics requires at least one currency with a positive global-base conversion factor.");
		}
	}
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

		var currency = _gameworld.Currencies.Get(activity.CurrencyId);
		if (currency is null)
		{
			return;
		}

		var zone = activity.EconomicZoneId.HasValue
			? _gameworld.EconomicZones.Get(activity.EconomicZoneId.Value)
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
		return ResolveControl(frameworkItemType, frameworkItemId, new Dictionary<long, EconomicControlBucket>());
	}

	private EconomicControlBucket ResolveControl(string? frameworkItemType, long? frameworkItemId,
		Dictionary<long, EconomicControlBucket> characterControlCache)
	{
		if (string.IsNullOrWhiteSpace(frameworkItemType) || !frameworkItemId.HasValue)
		{
			return EconomicControlBucket.Unclaimed;
		}

		if (frameworkItemType.EqualTo("Character") || frameworkItemType.EqualTo("ICharacter"))
		{
			return ResolveCharacterControl(frameworkItemId.Value, characterControlCache);
		}

		if (frameworkItemType.EqualTo("Clan") || frameworkItemType.EqualTo("IClan"))
		{
			var clan = _gameworld.Clans.Get(frameworkItemId.Value);
			if (clan is null)
			{
				return EconomicControlBucket.Institutional;
			}

			return clan.Memberships.Any(x =>
				!x.IsArchivedMembership &&
				(x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageBankAccounts) ||
				 x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewTreasury) ||
				 x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty)) &&
				ResolveCharacterControl(x.MemberId, characterControlCache) == EconomicControlBucket.DirectPc)
				? EconomicControlBucket.SharedPcControlled
				: EconomicControlBucket.Institutional;
		}

		if (frameworkItemType.EqualTo("Shop") || frameworkItemType.EqualTo("IShop"))
		{
			var shop = _gameworld.Shops.Get(frameworkItemId.Value);
			return shop is not null && _gameworld.Characters
				.Where(x => shop.IsManager(x) || shop.IsProprietor(x))
				.Any(x => ResolveCharacterControl(x.Id, characterControlCache) == EconomicControlBucket.DirectPc)
				? EconomicControlBucket.SharedPcControlled
				: EconomicControlBucket.Institutional;
		}

		if (frameworkItemType.EqualTo("Property") || frameworkItemType.EqualTo("IProperty"))
		{
			var property = _gameworld.Properties.Get(frameworkItemId.Value);
			if (property is null)
			{
				return EconomicControlBucket.Institutional;
			}

			var controls = property.PropertyOwners
				.Select(x => ResolveControl(x.OwnerFrameworkItemType, x.OwnerId, characterControlCache))
				.Append(property.Lease is null
					? EconomicControlBucket.Institutional
					: ResolveControl(property.Lease.Leaseholder.FrameworkItemType, property.Lease.Leaseholder.Id,
						characterControlCache));
			return controls.Any(IsPcControlled)
				? EconomicControlBucket.SharedPcControlled
				: EconomicControlBucket.Institutional;
		}

		return EconomicControlBucket.Institutional;
	}

	private EconomicControlBucket ResolveCharacterControl(long characterId,
		Dictionary<long, EconomicControlBucket> characterControlCache)
	{
		if (!characterControlCache.ContainsKey(characterId))
		{
			PreloadCharacterControls([characterId], characterControlCache);
		}

		return characterControlCache.GetValueOrDefault(characterId, EconomicControlBucket.Ambiguous);
	}

	private void PreloadCharacterControls(IEnumerable<long> characterIds,
		Dictionary<long, EconomicControlBucket> characterControlCache)
	{
		var requestedIds = characterIds
			.Where(x => x > 0 && !characterControlCache.ContainsKey(x))
			.Distinct()
			.ToList();
		if (requestedIds.Count == 0)
		{
			return;
		}

		var records = new Dictionary<long, CharacterControlRecord>();
		var pendingIds = requestedIds.ToHashSet();
		using (new FMDB())
		{
			while (pendingIds.Count > 0)
			{
				var batchIds = pendingIds.Take(1000).ToList();
				pendingIds.ExceptWith(batchIds);
				var characters = FMDB.Context.Characters
					.AsNoTracking()
					.Where(x => batchIds.Contains(x.Id))
					.Select(x => new { x.Id, x.AccountId, x.IsAdminAvatar })
					.ToList();
				var guestIds = FMDB.Context.Guests
					.AsNoTracking()
					.Where(x => batchIds.Contains(x.CharacterId))
					.Select(x => x.CharacterId)
					.ToHashSet();
				var npcs = FMDB.Context.Npcs
					.AsNoTracking()
					.Where(x => batchIds.Contains(x.CharacterId))
					.Select(x => new { x.CharacterId, x.BodyguardCharacterId })
					.ToDictionary(x => x.CharacterId);
				foreach (var character in characters)
				{
					var npc = npcs.GetValueOrDefault(character.Id);
					var record = new CharacterControlRecord(character.Id, character.AccountId,
						character.IsAdminAvatar, guestIds.Contains(character.Id), npc is not null,
						npc?.BodyguardCharacterId);
					records[record.Id] = record;
					if (record.BodyguardCharacterId is { } bodyguardId &&
					    !characterControlCache.ContainsKey(bodyguardId) && !records.ContainsKey(bodyguardId))
					{
						pendingIds.Add(bodyguardId);
					}
				}
			}
		}

		foreach (var characterId in requestedIds)
		{
			ResolveCharacterControlRecord(characterId, records, characterControlCache, []);
		}
	}

	private static EconomicControlBucket ResolveCharacterControlRecord(long characterId,
		IReadOnlyDictionary<long, CharacterControlRecord> records,
		Dictionary<long, EconomicControlBucket> characterControlCache, HashSet<long> resolving)
	{
		if (characterControlCache.TryGetValue(characterId, out var cached))
		{
			return cached;
		}

		if (!records.TryGetValue(characterId, out var character))
		{
			return characterControlCache[characterId] = EconomicControlBucket.Ambiguous;
		}

		if (!resolving.Add(characterId))
		{
			return EconomicControlBucket.Npc;
		}

		var result = character.IsAdminAvatar
			? EconomicControlBucket.Staff
			: character.IsGuest
				? EconomicControlBucket.Npc
				: !character.IsNpc
					? character.AccountId.HasValue
						? EconomicControlBucket.DirectPc
						: EconomicControlBucket.Ambiguous
					: character.BodyguardCharacterId is { } bodyguardId &&
					  ResolveCharacterControlRecord(bodyguardId, records, characterControlCache, resolving) ==
					  EconomicControlBucket.DirectPc
						? EconomicControlBucket.SharedPcControlled
						: EconomicControlBucket.Npc;
		resolving.Remove(characterId);
		return characterControlCache[characterId] = result;
	}

	private static bool IsPcControlled(EconomicControlBucket bucket)
	{
		return bucket is EconomicControlBucket.DirectPc or EconomicControlBucket.SharedPcControlled;
	}

	public IReadOnlyList<EconomyHolding> GetCurrentHoldings(long? economicZoneId = null, long? currencyId = null)
	{
		var holdings = new List<EconomyHolding>();
		var bankAccounts = _gameworld.BankAccounts
			.Where(x => x.CurrentBalance != 0.0M)
			.Where(x => !economicZoneId.HasValue || x.Bank.EconomicZone.Id == economicZoneId)
			.Where(x => !currencyId.HasValue || x.Currency.Id == currencyId)
			.ToList();
		var characterControlCache = new Dictionary<long, EconomicControlBucket>();
		PreloadCharacterControls(bankAccounts
			.Where(x => x.AccountOwnerFrameworkItemType.EqualTo("Character") ||
			            x.AccountOwnerFrameworkItemType.EqualTo("ICharacter"))
			.Select(x => x.AccountOwnerId), characterControlCache);
		AddPhysicalCashHoldings(holdings, characterControlCache);

		foreach (var account in bankAccounts)
		{
			var amount = account.CurrentBalance;
			var metric = amount >= 0.0M ? EconomyHoldingMetric.BankDeposits : EconomyHoldingMetric.BankDebt;
			holdings.Add(CreateHolding(account.Bank.EconomicZone.Id, account.Currency, metric,
				ResolveControl(account.AccountOwnerFrameworkItemType, account.AccountOwnerId, characterControlCache),
				Math.Abs(amount), account.AccountOwnerId, account.AccountOwnerFrameworkItemType,
				account.AccountReference));
		}

		foreach (var bank in _gameworld.Banks
		         .Where(x => !economicZoneId.HasValue || x.EconomicZone.Id == economicZoneId))
		{
			foreach (var reserve in bank.CurrencyReserves
			         .Where(x => !currencyId.HasValue || x.Key.Id == currencyId))
			{
				holdings.Add(CreateHolding(bank.EconomicZone.Id, reserve.Key, EconomyHoldingMetric.BankReserves,
					EconomicControlBucket.Institutional, reserve.Value, bank.Id, bank.FrameworkItemType, bank.Name));
			}
		}

		foreach (var shop in _gameworld.Shops
		         .Where(x => x.CashBalance != 0.0M)
		         .Where(x => !economicZoneId.HasValue || x.EconomicZone.Id == economicZoneId)
		         .Where(x => !currencyId.HasValue || x.Currency.Id == currencyId))
		{
			holdings.Add(CreateHolding(shop.EconomicZone.Id, shop.Currency, EconomyHoldingMetric.VirtualBalance,
				ResolveControl(shop.FrameworkItemType, shop.Id, characterControlCache), shop.CashBalance, shop.Id,
				shop.FrameworkItemType, shop.Name));
		}

		using (new FMDB())
		{
			var virtualBalanceQuery = FMDB.Context.VirtualCashBalances
				.AsNoTracking()
				.Where(x => x.Balance != 0.0M && x.OwnerType != "EconomicZone" && x.OwnerType != "Shop");
			if (currencyId.HasValue)
			{
				virtualBalanceQuery = virtualBalanceQuery.Where(x => x.CurrencyId == currencyId);
			}

			var virtualBalances = virtualBalanceQuery.ToList();
			foreach (var balance in virtualBalances)
			{
				var currency = _gameworld.Currencies.Get(balance.CurrencyId);
				if (currency is null)
				{
					continue;
				}

				var zoneId = ResolveEconomicZone(balance.OwnerType, balance.OwnerId);
				holdings.Add(CreateHolding(zoneId, currency, EconomyHoldingMetric.VirtualBalance,
					ResolveControl(balance.OwnerType, balance.OwnerId, characterControlCache), balance.Balance, balance.OwnerId,
					balance.OwnerType, $"{balance.OwnerType} #{balance.OwnerId:N0}"));
			}
		}

		foreach (var property in _gameworld.Properties
		         .Where(x => !economicZoneId.HasValue || x.EconomicZone.Id == economicZoneId)
		         .Where(x => !currencyId.HasValue || x.EconomicZone.Currency.Id == currencyId))
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
					ResolveControl(owner.OwnerFrameworkItemType, owner.OwnerId, characterControlCache), amount, owner.OwnerId,
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

	private void AddPhysicalCashHoldings(List<EconomyHolding> holdings,
		Dictionary<long, EconomicControlBucket> characterControlCache)
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
			var itemCustody = currencyComponents.ToDictionary(x => x.GameItemId,
				x => (x.ContainerId, x.OwnerId, OwnerType: (string?)x.OwnerType));
			var parentIds = itemCustody.Values
				.Select(x => x.ContainerId)
				.Where(x => x.HasValue)
				.Select(x => x!.Value)
				.Where(x => !itemCustody.ContainsKey(x))
				.ToHashSet();
			while (parentIds.Count > 0)
			{
				var nextParentIds = new HashSet<long>();
				foreach (var batch in parentIds.Chunk(1000))
				{
					var ids = batch.ToList();
					var parents = FMDB.Context.GameItems
						.AsNoTracking()
						.Where(x => ids.Contains(x.Id))
						.Select(x => new { x.Id, x.ContainerId, x.OwnerId, x.OwnerType })
						.ToList();
					foreach (var parent in parents)
					{
						itemCustody[parent.Id] = (parent.ContainerId, parent.OwnerId, parent.OwnerType);
						if (parent.ContainerId is { } nextParentId && !itemCustody.ContainsKey(nextParentId))
						{
							nextParentIds.Add(nextParentId);
						}
					}
				}

				parentIds = nextParentIds;
			}

			var rootIds = currencyComponents
				.Select(x => RootItemId(x.GameItemId, itemCustody))
				.ToHashSet();
			var bodyItems = new Dictionary<long, long>();
			var cellItems = new Dictionary<long, long>();
			var tillItems = new Dictionary<long, long>();
			foreach (var batch in rootIds.Chunk(1000))
			{
				var ids = batch.ToList();
				foreach (var row in FMDB.Context.BodiesGameItems.AsNoTracking()
					         .Where(x => ids.Contains(x.GameItemId))
					         .Select(x => new { x.GameItemId, x.BodyId }))
				{
					bodyItems[row.GameItemId] = row.BodyId;
				}

				foreach (var row in FMDB.Context.CellsGameItems.AsNoTracking()
					         .Where(x => ids.Contains(x.GameItemId))
					         .Select(x => new { x.GameItemId, x.CellId }))
				{
					cellItems[row.GameItemId] = row.CellId;
				}

				foreach (var row in FMDB.Context.ShopsTills.AsNoTracking()
					         .Where(x => ids.Contains(x.GameItemId))
					         .Select(x => new { x.GameItemId, x.ShopId }))
				{
					tillItems[row.GameItemId] = row.ShopId;
				}
			}

			var bodyCustodians = new Dictionary<long, (long CharacterId, long? CellId)>();
			var bodyIds = bodyItems.Values.ToHashSet();
			foreach (var batch in bodyIds.Chunk(1000))
			{
				var ids = batch.ToList();
				foreach (var character in FMDB.Context.Characters.AsNoTracking()
					         .Where(x => ids.Contains(x.BodyId))
					         .Select(x => new { x.BodyId, x.Id, x.Location }))
				{
					bodyCustodians.TryAdd(character.BodyId, (character.Id, character.Location));
				}

				var missingIds = ids.Where(x => !bodyCustodians.ContainsKey(x)).ToList();
				foreach (var instance in FMDB.Context.CharacterInstances.AsNoTracking()
					         .Where(x => missingIds.Contains(x.BodyId) && x.IsControllable)
					         .Select(x => new
					         {
						         x.BodyId,
						         CharacterId = x.PrimaryCharacterId ?? x.CharacterId,
						         x.LocationId
					         }))
				{
					bodyCustodians.TryAdd(instance.BodyId, (instance.CharacterId, instance.LocationId));
				}
			}

			PreloadCharacterControls(bodyCustodians.Values.Select(x => x.CharacterId), characterControlCache);
			var relevantCellIds = cellItems.Values.ToHashSet();
			var treasuryCells = new Dictionary<long, List<long>>();
			var propertyCells = new Dictionary<long, List<long>>();
			foreach (var batch in relevantCellIds.Chunk(1000))
			{
				var ids = batch.ToList();
				foreach (var row in FMDB.Context.ClansTreasuryCells.AsNoTracking()
					         .Where(x => ids.Contains(x.CellId))
					         .Select(x => new { x.CellId, x.ClanId }))
				{
					if (!treasuryCells.TryGetValue(row.CellId, out var clanIds))
					{
						clanIds = [];
						treasuryCells[row.CellId] = clanIds;
					}

					clanIds.Add(row.ClanId);
				}

				foreach (var row in FMDB.Context.PropertyLocations.AsNoTracking()
					         .Where(x => ids.Contains(x.CellId))
					         .Select(x => new { x.CellId, x.PropertyId }))
				{
					if (!propertyCells.TryGetValue(row.CellId, out var propertyIds))
					{
						propertyIds = [];
						propertyCells[row.CellId] = propertyIds;
					}

					propertyIds.Add(row.PropertyId);
				}
			}

			foreach (var component in currencyComponents)
			{
				var rootId = RootItemId(component.GameItemId, itemCustody);
				var rootOwner = itemCustody.GetValueOrDefault(rootId);
				var (ownerType, ownerId, bucket, zoneId, description) = ResolvePhysicalCustody(
					rootOwner.OwnerType ?? component.OwnerType, rootOwner.OwnerId ?? component.OwnerId,
					rootId, bodyItems, bodyCustodians, cellItems, tillItems,
					treasuryCells, propertyCells, characterControlCache);
				var loaded = _gameworld.Items.Get(component.GameItemId)
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
						var currency = _gameworld.Currencies.Get(currencyGroup.Key);
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
			IReadOnlyDictionary<long, long> bodyItems,
			IReadOnlyDictionary<long, (long CharacterId, long? CellId)> bodyCustodians,
			IReadOnlyDictionary<long, long> cellItems,
			IReadOnlyDictionary<long, long> tillItems, IReadOnlyDictionary<long, List<long>> treasuryCells,
			IReadOnlyDictionary<long, List<long>> propertyCells,
			Dictionary<long, EconomicControlBucket> characterControlCache)
	{
		if (!string.IsNullOrWhiteSpace(explicitOwnerType) && explicitOwnerId.HasValue)
		{
			return (explicitOwnerType, explicitOwnerId,
				ResolveControl(explicitOwnerType, explicitOwnerId, characterControlCache),
				ResolveEconomicZone(explicitOwnerType, explicitOwnerId.Value), "explicit ownership");
		}

		if (bodyItems.TryGetValue(rootItemId, out var bodyId) &&
		    bodyCustodians.TryGetValue(bodyId, out var custodian))
		{
			return ("Character", custodian.CharacterId,
				ResolveCharacterControl(custodian.CharacterId, characterControlCache),
				custodian.CellId.HasValue ? ResolveCellZone(custodian.CellId.Value) : null, "body custody");
		}

		if (tillItems.TryGetValue(rootItemId, out var shopId))
		{
			return ("Shop", shopId, ResolveControl("Shop", shopId, characterControlCache),
				ResolveEconomicZone("Shop", shopId),
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
			return (claim.Type, claim.Id, ResolveControl(claim.Type, claim.Id, characterControlCache),
				ResolveEconomicZone(claim.Type, claim.Id), $"{claim.Type.ToLowerInvariant()} custody");
		}

		return (null, null, EconomicControlBucket.Unclaimed, ResolveCellZone(cellId), "unclaimed cell cash");
	}

	private long? ResolveEconomicZone(string ownerType, long ownerId)
	{
		if (ownerType.EqualTo("Shop"))
		{
			return _gameworld.Shops.Get(ownerId)?.EconomicZone.Id;
		}

		if (ownerType.EqualTo("Bank"))
		{
			return _gameworld.Banks.Get(ownerId)?.EconomicZone.Id;
		}

		if (ownerType.EqualTo("Property"))
		{
			return _gameworld.Properties.Get(ownerId)?.EconomicZone.Id;
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
		var cell = _gameworld.Cells.Get(cellId);
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
					? _gameworld.EconomicZones.Get(economicZoneId.Value)
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

			var aggregates = BuildVolumeAggregateQuery(query).ToList();
			List<Models.EconomicActivityRecord> pendingRecords;
			lock (_pendingActivityLock)
			{
				pendingRecords = _pendingActivities
					.Select(x => x.Record)
					.Where(x => (!economicZoneId.HasValue || x.EconomicZoneId == economicZoneId) &&
					            (!currencyId.HasValue || x.CurrencyId == currencyId) &&
					            (window switch
					            {
						            EconomyQueryWindowKind.RealDay or EconomyQueryWindowKind.RealWeek or EconomyQueryWindowKind.RealMonth =>
							            x.RealDateTime >= start && x.RealDateTime <= now,
						            EconomyQueryWindowKind.FinancialPeriod => x.FinancialPeriodId == financialPeriodId,
						            _ => MatchesCurrentMudWindow(x, window, economicZoneId)
					            }))
					.ToList();
			}

			var exchange = aggregates.Sum(x => x.ExchangeGlobalBaseValue) + pendingRecords
				.Where(x => ((EconomicVolumeClassification)x.VolumeClassification)
					.HasFlag(EconomicVolumeClassification.Exchange))
				.Sum(x => x.GlobalBaseValue);
			var movement = aggregates.Sum(x => x.MovementGlobalBaseValue) + pendingRecords
				.Where(x => (((EconomicVolumeClassification)x.VolumeClassification) &
				             (EconomicVolumeClassification.Exchange | EconomicVolumeClassification.GeneralTransfer |
				              EconomicVolumeClassification.Source | EconomicVolumeClassification.Sink)) != 0)
				.Sum(x => x.GlobalBaseValue);
			var byActivity = aggregates
				.GroupBy(x => (EconomicActivityType)x.ActivityType)
				.ToDictionary(x => x.Key, x => x.Sum(y => y.TotalGlobalBaseValue));
			foreach (var group in pendingRecords
				.GroupBy(x => (EconomicActivityType)x.ActivityType)
				.Select(x => (x.Key, Value: x.Sum(y => y.GlobalBaseValue))))
			{
				byActivity[group.Key] = byActivity.GetValueOrDefault(group.Key) + group.Value;
			}

			var byPc = aggregates
				.GroupBy(x => x.PcInvolved
					? EconomicControlBucket.SharedPcControlled
					: EconomicControlBucket.Institutional)
				.ToDictionary(x => x.Key, x => x.Sum(y => y.TotalGlobalBaseValue));
			foreach (var group in pendingRecords
				.GroupBy(x => IsPcControlled((EconomicControlBucket)x.SourceControlBucket) ||
				              IsPcControlled((EconomicControlBucket)x.DestinationControlBucket)
					? EconomicControlBucket.SharedPcControlled
					: EconomicControlBucket.Institutional)
				.Select(x => (x.Key, Value: x.Sum(y => y.GlobalBaseValue))))
			{
				byPc[group.Key] = byPc.GetValueOrDefault(group.Key) + group.Value;
			}

			return new EconomyVolumeResult(ActivityCoverageStartUtc ?? now, start, now, exchange, movement,
				byActivity, byPc, aggregates.Sum(x => x.EventCount) + pendingRecords.Count);
		}
	}

	private static long RootItemId(long itemId,
		IReadOnlyDictionary<long, (long? ContainerId, long? OwnerId, string? OwnerType)> itemCustody)
	{
		var rootId = itemId;
		var visited = new HashSet<long>();
		while (itemCustody.TryGetValue(rootId, out var custody) && custody.ContainerId is { } parentId &&
		       visited.Add(rootId))
		{
			rootId = parentId;
		}

		return rootId;
	}

	internal static IQueryable<EconomyVolumeAggregateRow> BuildVolumeAggregateQuery(
		IQueryable<Models.EconomicActivityRecord> query)
	{
		const int exchangeMask = (int)EconomicVolumeClassification.Exchange;
		const int movementMask = (int)(EconomicVolumeClassification.Exchange |
		                               EconomicVolumeClassification.GeneralTransfer |
		                               EconomicVolumeClassification.Source |
		                               EconomicVolumeClassification.Sink);
		return query
			.GroupBy(x => new
			{
				x.ActivityType,
				PcInvolved = x.SourceControlBucket == (int)EconomicControlBucket.DirectPc ||
				             x.SourceControlBucket == (int)EconomicControlBucket.SharedPcControlled ||
				             x.DestinationControlBucket == (int)EconomicControlBucket.DirectPc ||
				             x.DestinationControlBucket == (int)EconomicControlBucket.SharedPcControlled
			})
			.Select(x => new EconomyVolumeAggregateRow
			{
				ActivityType = x.Key.ActivityType,
				PcInvolved = x.Key.PcInvolved,
				EventCount = x.LongCount(),
				ExchangeGlobalBaseValue = x.Sum(y => (y.VolumeClassification & exchangeMask) != 0
					? y.GlobalBaseValue
					: 0.0M),
				MovementGlobalBaseValue = x.Sum(y => (y.VolumeClassification & movementMask) != 0
					? y.GlobalBaseValue
					: 0.0M),
				TotalGlobalBaseValue = x.Sum(y => y.GlobalBaseValue)
			});
	}

	private bool MatchesCurrentMudWindow(Models.EconomicActivityRecord record, EconomyQueryWindowKind window,
		long? economicZoneId)
	{
		var zone = economicZoneId.HasValue
			? _gameworld.EconomicZones.Get(economicZoneId.Value)
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

			return BuildTrendQuery(query, count).ToList();
		}
	}

	internal static IQueryable<EconomySnapshotPoint> BuildTrendQuery(
		IQueryable<Models.EconomySnapshotEntry> query, int count)
	{
		return query
				.GroupBy(x => new
				{
					x.EconomySnapshotId,
					x.EconomySnapshot.RealDateTime,
					x.EconomySnapshot.EconomicZoneId,
					x.EconomySnapshot.FinancialPeriodId,
					x.EconomySnapshot.Reason
				})
				.OrderByDescending(x => x.Key.RealDateTime)
				.Take(Math.Clamp(count, 1, 100))
				.Select(x => new EconomySnapshotPoint(x.Key.EconomySnapshotId, x.Key.RealDateTime,
					x.Key.EconomicZoneId, x.Key.FinancialPeriodId, (EconomySnapshotReason)x.Key.Reason,
					x.Sum(y => y.GlobalBaseValue)));
	}

	public IReadOnlyList<EconomyRisk> GetRisks(long? economicZoneId = null)
	{
		return GetRisks(GetCurrentHoldings(economicZoneId), economicZoneId);
	}

	public IReadOnlyList<EconomyRisk> GetRisks(IReadOnlyList<EconomyHolding> currentHoldings,
		long? economicZoneId = null)
	{
		var risks = new List<EconomyRisk>();
		foreach (var group in currentHoldings.GroupBy(x => new { x.EconomicZoneId, x.CurrencyId }))
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

		var ambiguous = currentHoldings
			.Where(x => x.Metric == EconomyHoldingMetric.PhysicalCash &&
			            x.ControlBucket is EconomicControlBucket.Ambiguous or EconomicControlBucket.Unclaimed)
			.Sum(x => x.GlobalBaseValue);
		if (ambiguous > 0.0M)
		{
			risks.Add(new EconomyRisk("unclaimed-cash", "Physical cash has ambiguous or unclaimed custody.", ambiguous,
				economicZoneId));
		}
		var malformedCount = currentHoldings.Count(x => x.Metric == EconomyHoldingMetric.PhysicalCash &&
			x.Description?.StartsWith("Malformed", StringComparison.InvariantCultureIgnoreCase) == true);
		if (malformedCount > 0)
		{
			risks.Add(new EconomyRisk("malformed-currency", $"{malformedCount:N0} persisted currency components could not be parsed."));
		}

		var virtualLiabilities = currentHoldings
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
		return _gameworld.Currencies.Get(currencyId)
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
					? _gameworld.EconomicZones.Get(economicZoneId.Value)
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

	public bool TrySetGlobalDisplayCurrency(ICurrency currency, out string error)
	{
		if (currency.BaseCurrencyToGlobalBaseCurrencyConversion <= 0.0M)
		{
			error = $"{currency.Name} cannot display global values because its global-base conversion factor is zero.";
			return false;
		}

		SaveConfiguration(GlobalDisplayCurrencyConfiguration,
			currency.Id.ToString(System.Globalization.CultureInfo.InvariantCulture));
		error = string.Empty;
		return true;
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
