using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Framework;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.TimeAndDate;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework.Revision;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Time;
using Object = System.Object;
using MudSharp.TimeAndDate.Date;
using MudSharp.Models;
using MudSharp.Community;
using System.Text.RegularExpressions;
using Castle.Components.DictionaryAdapter;
using MoreLinq.Extensions;
using MudSharp.Effects.Concrete;
using MudSharp.Economy.Tax;
using System.Numerics;

namespace MudSharp.Economy;

public class EconomicZone : SaveableItem, IEconomicZone
{
	private readonly All<FinancialPeriod> _financialPeriods = new();
	public ICurrency Currency { get; private set; }

	private readonly List<ISalesTax> _salesTaxes = new();
	public IEnumerable<ISalesTax> SalesTaxes => _salesTaxes;
	private readonly List<IProfitTax> _profitTaxes = new();
	public IEnumerable<IProfitTax> ProfitTaxes => _profitTaxes;
	private readonly List<IIncomeTax> _incomeTaxes = new();
	public IEnumerable<IIncomeTax> IncomeTaxes => _incomeTaxes;

	public IFinancialPeriod CurrentFinancialPeriod { get; private set; }
	public int PreviousFinancialPeriodsToKeep { get; private set; }
	public IZone ZoneForTimePurposes { get; private set; }
	public bool PermitTaxableLosses { get; private set; }
	public decimal OutstandingTaxesOwed { get; private set; }
	public decimal TotalRevenueHeld { get; private set; }
	private readonly List<(IFinancialPeriod Period, decimal TotalTaxRevenue)> _historicalRevenues = new();

	public IEnumerable<(IFinancialPeriod Period, decimal TotalTaxRevenue)> HistoricalRevenues =>
		_historicalRevenues;

	public IEnumerable<IFinancialPeriod> FinancialPeriods => _financialPeriods;

	public RecurringInterval FinancialPeriodInterval { get; private set; }
	public MudTime FinancialPeriodReferenceTime { get; private set; }
	public IClock FinancialPeriodReferenceClock { get; private set; }
	public ICalendar FinancialPeriodReferenceCalendar { get; private set; }
	public IMudTimeZone FinancialPeriodTimezone { get; private set; }

	private long? _controllingClanId;
	private IClan _controllingClan;

	public IClan ControllingClan
	{
		get
		{
			if (_controllingClan == null && _controllingClanId.HasValue)
			{
				_controllingClan = Gameworld.Clans.Get(_controllingClanId.Value);
			}

			return _controllingClan;
		}
		set
		{
			_controllingClan = value;
			_controllingClanId = value?.Id;
			Changed = true;
		}
	}

	public EconomicZone(IFuturemud gameworld, IZone zone, string name)
	{
		Gameworld = gameworld;
		_name = name;
		ZoneForTimePurposes = zone;
		FinancialPeriodInterval = new RecurringInterval
			{ IntervalAmount = 1, Modifier = 0, Type = IntervalType.Yearly };
		TotalRevenueHeld = 0.0M;
		OutstandingTaxesOwed = 0.0M;
		PermitTaxableLosses = false;
		PreviousFinancialPeriodsToKeep = 50;
		Currency = Gameworld.Currencies.Get(Gameworld.GetStaticLong("DefaultCurrencyID")) ??
		           Gameworld.Currencies.First();
		FinancialPeriodReferenceClock = zone.Clocks.First();
		FinancialPeriodReferenceCalendar = zone.Calendars.First();
		FinancialPeriodReferenceCalendar.DaysUpdated += ReferenceCalendarOnDaysUpdated;
		FinancialPeriodTimezone = zone.TimeZone(FinancialPeriodReferenceClock);
		FinancialPeriodReferenceTime =
			new MudTime(0, 0, 0, FinancialPeriodTimezone, FinancialPeriodReferenceClock, false);
		using (new FMDB())
		{
			var dbitem = new Models.EconomicZone
			{
				IntervalModifier = FinancialPeriodInterval.Modifier,
				IntervalType = (int)FinancialPeriodInterval.Type,
				IntervalAmount = FinancialPeriodInterval.IntervalAmount,
				CurrencyId = Currency.Id,
				Name = _name,
				OutstandingTaxesOwed = OutstandingTaxesOwed,
				TotalRevenueHeld = TotalRevenueHeld,
				PermitTaxableLosses = PermitTaxableLosses,
				PreviousFinancialPeriodsToKeep = PreviousFinancialPeriodsToKeep,
				ReferenceCalendarId = FinancialPeriodReferenceCalendar.Id,
				ReferenceClockId = FinancialPeriodReferenceClock.Id,
				ReferenceTime = FinancialPeriodReferenceTime.GetTimeString(),
				ZoneForTimePurposesId = ZoneForTimePurposes.Id
			};
			FMDB.Context.EconomicZones.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;

			var periodstart = new MudDateTime(
				new MudDate(FinancialPeriodReferenceCalendar, 1, FinancialPeriodReferenceCalendar.CurrentDate.Year,
					FinancialPeriodReferenceCalendar.CurrentDate.ThisYear.Months.First(),
					FinancialPeriodReferenceCalendar.CurrentDate.ThisYear, false), FinancialPeriodReferenceTime,
				FinancialPeriodTimezone);
			var nextYear =
				FinancialPeriodReferenceCalendar.CreateYear(FinancialPeriodReferenceCalendar.CurrentDate.Year + 1);
			var periodend =
				new MudDateTime(
					new MudDate(FinancialPeriodReferenceCalendar, 1,
						FinancialPeriodReferenceCalendar.CurrentDate.Year + 1, nextYear.Months.First(), nextYear,
						false), FinancialPeriodReferenceTime, FinancialPeriodTimezone);

			var dbperiod = new Models.FinancialPeriod
			{
				EconomicZone = dbitem,
				MudPeriodStart = periodstart.GetDateTimeString(),
				MudPeriodEnd = periodend.GetDateTimeString(),
				PeriodStart = DateTime.UtcNow,
				PeriodEnd = DateTime.UtcNow.AddDays(90)
			};
			dbitem.CurrentFinancialPeriod = dbperiod;
			FMDB.Context.FinancialPeriods.Add(dbperiod);
			FMDB.Context.SaveChanges();

			var period = new FinancialPeriod(dbperiod, this, Gameworld);
			_financialPeriods.Add(period);
			CurrentFinancialPeriod = period;
		}
	}

	public EconomicZone(MudSharp.Models.EconomicZone zone, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = zone.Id;
		_name = zone.Name;
		Currency = Gameworld.Currencies.Get(zone.CurrencyId);
		_controllingClanId = zone.ControllingClanId;
		foreach (var period in zone.FinancialPeriods)
		{
			_financialPeriods.Add(new FinancialPeriod(period, this, gameworld));
		}

		OutstandingTaxesOwed = zone.OutstandingTaxesOwed;
		TotalRevenueHeld = zone.TotalRevenueHeld;
		PreviousFinancialPeriodsToKeep = zone.PreviousFinancialPeriodsToKeep;
		ZoneForTimePurposes = gameworld.Zones.Get(zone.ZoneForTimePurposesId);
		PermitTaxableLosses = zone.PermitTaxableLosses;
		FinancialPeriodInterval = new RecurringInterval
		{
			IntervalAmount = zone.IntervalAmount,
			Modifier = zone.IntervalModifier,
			Type = (IntervalType)zone.IntervalType
		};

		FinancialPeriodReferenceClock = gameworld.Clocks.Get(zone.ReferenceClockId);
		FinancialPeriodReferenceCalendar = gameworld.Calendars.Get(zone.ReferenceCalendarId ?? 0);
		FinancialPeriodReferenceTime = new MudTime(zone.ReferenceTime, FinancialPeriodReferenceClock);
		FinancialPeriodTimezone = FinancialPeriodReferenceTime.Timezone;

		CurrentFinancialPeriod = _financialPeriods.Get(zone.CurrentFinancialPeriodId ?? 0) ??
		                         _financialPeriods.WhereMax(x => x.FinancialPeriodStart).FirstOrDefault();
		var createListener = true;
		if (FinancialPeriodReferenceCalendar != null)
		{
			if (CurrentFinancialPeriod == null)
			{
				var now = DateTime.UtcNow;
				var mudnow = FinancialPeriodReferenceCalendar.CurrentDateTime;
				var period = new FinancialPeriod(this, now, FinancialPeriodInterval.GetNextDateTime(now), mudnow,
					FinancialPeriodInterval.GetNextDateTime(mudnow));
				_financialPeriods.Add(period);
				CurrentFinancialPeriod = period;
			}

			if (CurrentFinancialPeriod != null && CurrentFinancialPeriod.FinancialPeriodEndMUD <=
			    FinancialPeriodReferenceCalendar.CurrentDateTime)
			{
				CloseCurrentFinancialPeriod();
				createListener = false;
			}


			FinancialPeriodReferenceCalendar.DaysUpdated += ReferenceCalendarOnDaysUpdated;
		}

		if (CurrentFinancialPeriod != null && createListener)
		{
			Gameworld.Add(new TimeAndDate.Listeners.DateListener(
				CurrentFinancialPeriod.FinancialPeriodEndMUD, 0,
				payload => { CloseCurrentFinancialPeriod(); },
				new object[] { }));
		}

		foreach (var item in zone.EconomicZoneTaxes)
		{
			if (TaxFactory.IsSalesTax(item))
			{
				_salesTaxes.Add(TaxFactory.LoadSalesTax(item, this));
			}
			else
			{
				_profitTaxes.Add(TaxFactory.LoadProfitTax(item, this));
			}
		}

		foreach (var item in zone.EconomicZoneShopTaxes)
		{
			_shopsOutstandingProfitTaxes[item.ShopId] = item.OutstandingProfitTaxes;
			_shopsOutstandingSalesTaxes[item.ShopId] = item.OutstandingSalesTaxes;
			_shopsTaxesInCredit[item.ShopId] = item.TaxesInCredits;
		}

		foreach (var item in zone.EconomicZoneRevenues)
		{
			_historicalRevenues.Add((_financialPeriods.Get(item.FinancialPeriodId), item.TotalTaxRevenue));
		}

		foreach (var item in zone.ShopFinancialPeriodResults)
		{
			_shopsPreviousFinancialPeriodResults.Add(item.ShopId,
				new FinancialPeriodResult(_financialPeriods.Get(item.FinancialPeriodId), item.GrossRevenue,
					item.NetRevenue, item.SalesTax, item.ProfitsTax));
		}

		foreach (var location in zone.ConveyancingLocations)
		{
			var cell = Gameworld.Cells.Get(location.CellId);
#if DEBUG
			if (cell == null)
			{
				throw new ApplicationException("Cell shouldn't be null in EconomicZone constructor");
			}
#endif
			_conveyancingCells.Add(cell);
			cell.CellRequestsDeletion -= ConveyancingCellRequestsDeletion;
			cell.CellRequestsDeletion += ConveyancingCellRequestsDeletion;
		}

		foreach (var location in zone.JobFindingLocations)
		{
			var cell = Gameworld.Cells.Get(location.CellId);
#if DEBUG
			if (cell == null)
			{
				throw new ApplicationException("Cell shouldn't be null in EconomicZone constructor");
			}
#endif
			_jobFindingCells.Add(cell); 
			cell.CellRequestsDeletion -= JobCellRequestsDeletion;
			cell.CellRequestsDeletion += JobCellRequestsDeletion;
		}
	}

	private void ReferenceCalendarOnDaysUpdated()
	{
		foreach (var bank in Gameworld.Banks.Where(x => x.EconomicZone == this))
		{
			bank.ReferenceDateOnDateChanged();
		}
	}

	public IEconomicZone Clone(string newName)
	{
		newName = newName.ToLowerInvariant().TitleCase();
		using (new FMDB())
		{
			var olditem = FMDB.Context.EconomicZones.Find(Id);
			var dbitem = new Models.EconomicZone
			{
				CurrentFinancialPeriodId = CurrentFinancialPeriod?.Id,
				IntervalModifier = FinancialPeriodInterval.Modifier,
				IntervalType = (int)FinancialPeriodInterval.Type,
				IntervalAmount = FinancialPeriodInterval.IntervalAmount,
				CurrencyId = Currency.Id,
				ControllingClanId = ControllingClan?.Id,
				Name = newName,
				OutstandingTaxesOwed = 0.0M,
				TotalRevenueHeld = 0.0M,
				PermitTaxableLosses = PermitTaxableLosses,
				PreviousFinancialPeriodsToKeep = PreviousFinancialPeriodsToKeep,
				ReferenceCalendarId = FinancialPeriodReferenceCalendar.Id,
				ReferenceClockId = FinancialPeriodReferenceClock.Id,
				ReferenceTime = FinancialPeriodReferenceTime.GetTimeString()
			};

			foreach (var tax in olditem.EconomicZoneTaxes)
			{
				dbitem.EconomicZoneTaxes.Add(new EconomicZoneTax
				{
					EconomicZone = dbitem,
					MerchandiseFilterProg = tax.MerchandiseFilterProg,
					TaxType = tax.TaxType,
					Definition = tax.Definition,
					MerchantDescription = tax.MerchantDescription,
					Name = tax.Name
				});
			}

			FMDB.Context.EconomicZones.Add(dbitem);
			FMDB.Context.SaveChanges();
			return new EconomicZone(dbitem, Gameworld);
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.EconomicZones.Find(Id);
		dbitem.CurrentFinancialPeriodId = CurrentFinancialPeriod?.Id;
		dbitem.IntervalModifier = FinancialPeriodInterval.Modifier;
		dbitem.IntervalType = (int)FinancialPeriodInterval.Type;
		dbitem.IntervalAmount = FinancialPeriodInterval.IntervalAmount;
		dbitem.CurrencyId = Currency.Id;
		dbitem.ControllingClanId = ControllingClan?.Id;
		dbitem.Name = _name;
		dbitem.OutstandingTaxesOwed = OutstandingTaxesOwed;
		dbitem.TotalRevenueHeld = TotalRevenueHeld;
		dbitem.PermitTaxableLosses = PermitTaxableLosses;
		dbitem.PreviousFinancialPeriodsToKeep = PreviousFinancialPeriodsToKeep;
		dbitem.ReferenceCalendarId = FinancialPeriodReferenceCalendar.Id;
		dbitem.ReferenceClockId = FinancialPeriodReferenceClock.Id;
		dbitem.ReferenceTime = FinancialPeriodReferenceTime.GetTimeString();

		FMDB.Context.EconomicZoneRevenues.RemoveRange(dbitem.EconomicZoneRevenues);
		foreach (var item in _historicalRevenues)
		{
			var newHR = new EconomicZoneRevenue
			{
				FinancialPeriodId = item.Period.Id,
				TotalTaxRevenue = item.TotalTaxRevenue,
				EconomicZone = dbitem
			};
			dbitem.EconomicZoneRevenues.Add(newHR);
		}

		FMDB.Context.EconomicZoneShopTaxes.RemoveRange(dbitem.EconomicZoneShopTaxes);
		foreach (var shop in _shopsOutstandingSalesTaxes.Keys.Concat(_shopsTaxesInCredit.Keys)
		                                                .Concat(_shopsOutstandingProfitTaxes.Keys).Distinct())
		{
			dbitem.EconomicZoneShopTaxes.Add(new EconomicZoneShopTax
			{
				EconomicZone = dbitem,
				ShopId = shop,
				OutstandingProfitTaxes = _shopsOutstandingProfitTaxes[shop],
				OutstandingSalesTaxes = _shopsOutstandingSalesTaxes[shop],
				TaxesInCredits = _shopsTaxesInCredit[shop]
			});
		}

		FMDB.Context.ConveyancingLocations.RemoveRange(dbitem.ConveyancingLocations);
		foreach (var location in ConveyancingCells)
		{
			dbitem.ConveyancingLocations.Add(new ConveyancingLocation
			{
				CellId = location.Id,
				EconomicZoneId = Id
			});
		}

		FMDB.Context.JobFindingLocations.RemoveRange(dbitem.JobFindingLocations);
		foreach (var location in JobFindingCells)
		{
			dbitem.JobFindingLocations.Add(new JobFindingLocation
			{
				CellId = location.Id,
				EconomicZoneId = Id
			});
		}

		Changed = false;
	}

	public override string FrameworkItemType => "EconomicZone";

	private readonly DecimalCounter<long> _shopsOutstandingProfitTaxes = new();
	private readonly DecimalCounter<long> _shopsOutstandingSalesTaxes = new();
	private readonly DecimalCounter<long> _shopsTaxesInCredit = new();

	private readonly CollectionDictionary<long, FinancialPeriodResult> _shopsPreviousFinancialPeriodResults = new();

	public void CloseCurrentFinancialPeriod()
	{
		var oldPeriod = CurrentFinancialPeriod;
		var oldPeriodRealStart = oldPeriod.FinancialPeriodStart;
		var oldPeriodRealEnd = DateTime.UtcNow;
		oldPeriod.FinancialPeriodEnd = oldPeriodRealEnd;
		oldPeriod.Changed = true;
		var oldPeriodMudEnd = oldPeriod.FinancialPeriodEndMUD;

		var newEnd = FinancialPeriodInterval.GetNextDateExclusive(oldPeriodMudEnd.Calendar, oldPeriodMudEnd.Date);
		var newRealEnd = FinancialPeriodInterval.GetNextDateTime(oldPeriodRealEnd);

		var newPeriod = new FinancialPeriod(this, oldPeriodRealEnd, newRealEnd, oldPeriodMudEnd,
			new MudDateTime(newEnd, FinancialPeriodReferenceTime, FinancialPeriodTimezone));
		_financialPeriods.Add(newPeriod);
		CurrentFinancialPeriod = newPeriod;

		var totalPeriodTax = 0.0M;
		foreach (var shop in Gameworld.Shops.Where(x => x.EconomicZone == this))
		{
			var transactions = shop.TransactionRecords.Where(x => x.RealDateTime >= oldPeriodRealStart).ToList();
			var (salesTax, grossRevenue, netRevenue) = transactions.Sum3(x => x.Tax,
				x => x.TransactionType == ShopTransactionType.Sale ? x.PretaxValue : 0.0M, x => x.NetValue);
			totalPeriodTax += salesTax;
			_shopsOutstandingSalesTaxes[shop.Id] += salesTax;
			var profitTax = 0.0M;
			foreach (var tax in ProfitTaxes)
			{
				if (!tax.Applies(shop, grossRevenue, netRevenue))
				{
					continue;
				}

				var value = tax.TaxValue(shop, grossRevenue, netRevenue);
				if (value < 0)
				{
					if (PermitTaxableLosses)
					{
						totalPeriodTax += value;
						_shopsTaxesInCredit[shop.Id] += -1 * value;
					}

					continue;
				}

				profitTax += value;
			}

			_shopsOutstandingProfitTaxes[shop.Id] += profitTax;
			totalPeriodTax += profitTax;
			if (_shopsTaxesInCredit[shop.Id] > 0.0M)
			{
				if (_shopsTaxesInCredit[shop.Id] <= _shopsOutstandingProfitTaxes[shop.Id])
				{
					_shopsOutstandingProfitTaxes[shop.Id] -= _shopsTaxesInCredit[shop.Id];
					_shopsTaxesInCredit[shop.Id] = 0.0M;
				}
				else
				{
					_shopsTaxesInCredit[shop.Id] -= _shopsOutstandingProfitTaxes[shop.Id];
					_shopsOutstandingProfitTaxes[shop.Id] = 0.0M;
				}
			}

			_shopsPreviousFinancialPeriodResults.Add(shop.Id,
				new FinancialPeriodResult(newPeriod, grossRevenue, netRevenue, salesTax, profitTax));
		}

		OutstandingTaxesOwed += totalPeriodTax;
		_historicalRevenues.Add((newPeriod, totalPeriodTax));

		if (PreviousFinancialPeriodsToKeep < _financialPeriods.Count)
		{
			var count = _financialPeriods.Count - PreviousFinancialPeriodsToKeep;
			var periodsToRemove = _financialPeriods.OrderBy(x => x.FinancialPeriodStart).Take(count).ToList();
			foreach (var period in periodsToRemove)
			{
				period.Delete();
				_financialPeriods.Remove(period);
				_historicalRevenues.RemoveAll(x => x.Period == period);
				_shopsPreviousFinancialPeriodResults.RemoveAll(x => x.Period == period);
			}
		}

		Changed = true;
		Gameworld.Add(new TimeAndDate.Listeners.DateListener(
			CurrentFinancialPeriod.FinancialPeriodEndMUD, 0,
			payload => { CloseCurrentFinancialPeriod(); },
			new object[] { }));
	}

	public decimal OutstandingTaxesForShop(IShop shop)
	{
		return _shopsOutstandingProfitTaxes[shop.Id] + _shopsOutstandingSalesTaxes[shop.Id] -
		       _shopsTaxesInCredit[shop.Id];
	}

	public void PayTaxesForShop(IShop shop, decimal amount)
	{
		Changed = true;
		TotalRevenueHeld += amount;
		if (_shopsOutstandingSalesTaxes[shop.Id] >= amount)
		{
			_shopsOutstandingSalesTaxes[shop.Id] -= amount;
			return;
		}

		amount -= _shopsOutstandingSalesTaxes[shop.Id];
		_shopsOutstandingSalesTaxes[shop.Id] = 0.0M;

		if (_shopsOutstandingProfitTaxes[shop.Id] >= amount)
		{
			_shopsOutstandingProfitTaxes[shop.Id] -= amount;
			return;
		}

		amount -= _shopsOutstandingProfitTaxes[shop.Id];
		_shopsOutstandingProfitTaxes[shop.Id] = 0.0M;

		if (amount > 0.0M)
		{
			_shopsTaxesInCredit[shop.Id] += amount;
		}
	}

	public void ReportSalesTaxCollected(IShop shop, decimal amount)
	{
		Changed = true;
		if (_shopsTaxesInCredit[shop.Id] > 0.0M)
		{
			if (_shopsTaxesInCredit[shop.Id] > amount)
			{
				_shopsTaxesInCredit[shop.Id] -= amount;
				return;
			}

			amount -= _shopsTaxesInCredit[shop.Id];
			_shopsTaxesInCredit[shop.Id] = 0.0M;
		}

		_shopsOutstandingSalesTaxes[shop.Id] += amount;
	}

	#region Implementation of IEditableItem

	public string HelpInfo => @"You can use the following options with this command:

	#3name <name>#0 - renames this economic zone
	#3currency <currency>#0 - changes the currency used in this zone
	#3clock <clock>#0 - changes the clock used in this zone
	#3calendar <calendar>#0 - changes the calendar used in this zone
	#3interval <type> <amount> <offset>#0 - sets the interval for financial periods
	#3time <time>#0 - sets the reference time for financial periods
	#3timezone <tz>#0 - sets the reference timezone for this zone
	#3zone <zone>#0 - sets the physical zone used as a reference for current time
	#3previous <amount>#0 - sets the number of previous financial periods to keep records for
	#3permitloss#0 - toggles permitting taxable losses
	#3clan <clan>#0 - assigns a new clan to custody of this economic zone
	#3clan none#0 - clears clan control of this economic zone
	#3salestax add <type> <name>#0 - adds a new sales tax
	#3salestax remove <name>#0 - removes a sales tax
	#3salestax <which> <...>#0 - edit properties of a particular tax
	#3profittax add <type> <name>#0 - adds a new profit tax
	#3profittax remove <name>#0 - removes a profit tax
	#3profittax <which> <...>#0 - edit properties of a particular tax
	#3realty#0 - toggles your current location as a conveyancing/realty location
	#3jobs#0 - toggles your current location as a job listing and finding location";

	public string ClanHelpInfo => @"You can use the following options with this command:

	#3permitloss#0 - toggles permitting taxable losses
	#3clan <clan>#0 - assigns a new clan to custody of this economic zone
	#3clan none#0 - clears clan control of this economic zone
	#3salestax add <type> <name>#0 - adds a new sales tax
	#3salestax remove <name>#0 - removes a sales tax
	#3salestax <which> <...>#0 - edit properties of a particular tax
	#3profittax add <type> <name>#0 - adds a new profit tax
	#3profittax remove <name>#0 - removes a profit tax
	#3profittax <which> <...>#0 - edit properties of a particular tax";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "currency":
				return BuildingCommandCurrency(actor, command);
			case "calendar":
				return BuildingCommandCalendar(actor, command);
			case "interval":
				return BuildingCommandInterval(actor, command);
			case "time":
				return BuildingCommandTime(actor, command);
			case "zone":
				return BuildingCommandZone(actor, command);
			case "permitloss":
				return BuildingCommandPermitLoss(actor, command, false);
			case "previous":
				return BuildingCommandPrevious(actor, command);
			case "salestax":
				return BuildingCommandSalesTax(actor, command, false);
			case "profittax":
				return BuildingCommandProfitTax(actor, command, false);
			case "clan":
				return BuildingCommandClan(actor, command, false);
			case "conveyance":
			case "conveyancing":
			case "realestate":
			case "realty":
				return BuildingCommandConveyance(actor, command);
			case "job":
			case "jobs":
				return BuildingCommandJobs(actor, command);
			default:
				actor.OutputHandler.Send(HelpInfo.SubstituteANSIColour());
				return false;
		}
	}

	public bool BuildingCommandFromClanCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech())
		{
			case "permitloss":
				return BuildingCommandPermitLoss(actor, command, true);
			case "salestax":
				return BuildingCommandSalesTax(actor, command, true);
			case "profittax":
				return BuildingCommandProfitTax(actor, command, true);
			case "clan":
				return BuildingCommandClan(actor, command, true);
			default:
				actor.OutputHandler.Send(ClanHelpInfo.SubstituteANSIColour());
				return false;
		}
	}

	public bool BuildingCommandCalendar(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which calendar would you like to use as the reference for financial periods and economic activity in this zone?");
			return false;
		}

		var calendar = actor.Gameworld.Calendars.GetByIdOrName(command.SafeRemainingArgument);
		if (calendar is null)
		{
			actor.OutputHandler.Send("There is no such calendar.");
			return false;
		}

		if (calendar == FinancialPeriodReferenceCalendar)
		{
			actor.OutputHandler.Send(
				$"The {calendar.Name.ColourName()} calendar is already being used as a reference for this zone.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Warning: Changing the calendar will delete all the existing financial periods and their data. This is completely unrecoverable. Are you sure that this is what you want to do?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "Changing the calendar for an economic zone",
			AcceptAction = text =>
			{
				FinancialPeriodReferenceCalendar.DaysUpdated -= ReferenceCalendarOnDaysUpdated;
				FinancialPeriodReferenceCalendar = calendar;
				calendar.DaysUpdated += ReferenceCalendarOnDaysUpdated;
				foreach (var period in _financialPeriods.ToList())
				{
					period.Delete();
				}

				_financialPeriods.Clear();
				_shopsPreviousFinancialPeriodResults.Clear();
				_historicalRevenues.Clear();
				var now = DateTime.UtcNow;
				var mudnow = FinancialPeriodReferenceCalendar.CurrentDateTime;
				var newPeriod = new FinancialPeriod(this, now, FinancialPeriodInterval.GetNextDateTime(now), mudnow,
					FinancialPeriodInterval.GetNextDateTime(mudnow));
				_financialPeriods.Add(newPeriod);
				CurrentFinancialPeriod = newPeriod;
				Changed = true;
				Gameworld.Add(new TimeAndDate.Listeners.DateListener(
					CurrentFinancialPeriod.FinancialPeriodEndMUD, 0,
					payload => { CloseCurrentFinancialPeriod(); },
					new object[] { }));
				actor.OutputHandler.Send(
					$"You set this economic zone to now use the {calendar.Name.ColourName()} calendar.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send($"You decide not to change the calendar for this economic zone.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send($"You decide not to change the calendar for this economic zone.");
			},
			Keywords = new List<string> { "calendar" }
		}), TimeSpan.FromSeconds(120));


		throw new NotImplementedException();
	}

	private bool BuildingCommandClan(ICharacter actor, StringStack command, bool fromClanCommand)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a clan to control taxation in this economic zone, or use 'none' to clear an existing controlling clan.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "delete", "remove", "del"))
		{
			ControllingClan = null;
			Changed = true;
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} economic zone will no longer be controlled by any clan.");
			return true;
		}

		var clan = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Clans.Get(value)
			: Gameworld.Clans.GetByName(command.Last) ??
			  Gameworld.Clans.FirstOrDefault(x => x.Alias.EqualTo(command.Last));
		if (clan == null)
		{
			actor.OutputHandler.Send("There is no such clan.");
			return false;
		}

		ControllingClan = clan;
		actor.OutputHandler.Send(
			$"The setting and collection of taxes in the {Name.ColourName()} economic zone will now be done by the {clan.FullName.ColourName()} clan.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this economic zone?");
			return false;
		}

		var name = command.PopSpeech().ToLowerInvariant().TitleCase();
		if (Gameworld.EconomicZones.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already an economic zone with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the {Name.ColourName()} economic zone to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandCurrency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What currency do you want to set as the currency for this economic zone?");
			return false;
		}

		var currency = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Currencies.Get(value)
			: Gameworld.Currencies.GetByName(command.Last);
		if (currency == null)
		{
			actor.OutputHandler.Send("There is no such currency.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What conversion rate do you want to use for outstanding balances between the old currency and the new currency?");
			return false;
		}

		if (!decimal.TryParse(command.SafeRemainingArgument, out var dvalue))
		{
			actor.OutputHandler.Send("That is not a valid multiplier.");
			return false;
		}

		Currency = currency;
		Changed = true;
		foreach (var revenue in _historicalRevenues.ToList())
		{
			_historicalRevenues[_historicalRevenues.IndexOf(revenue)] =
				(revenue.Period, revenue.TotalTaxRevenue * dvalue);
		}

		TotalRevenueHeld *= dvalue;
		OutstandingTaxesOwed *= dvalue;

		foreach (var tax in _shopsOutstandingProfitTaxes.ToList())
		{
			_shopsOutstandingProfitTaxes[tax.Key] = tax.Value * dvalue;
		}

		foreach (var tax in _shopsOutstandingSalesTaxes.ToList())
		{
			_shopsOutstandingSalesTaxes[tax.Key] = tax.Value * dvalue;
		}

		foreach (var tax in _shopsTaxesInCredit.ToList())
		{
			_shopsTaxesInCredit[tax.Key] = tax.Value * dvalue;
		}

		foreach (var item in _shopsPreviousFinancialPeriodResults.ToList())
		{
			_shopsPreviousFinancialPeriodResults[item.Key] = item.Value.Select(x =>
				new FinancialPeriodResult(x.Period, x.GrossRevenue * dvalue, x.NetRevenue * dvalue, x.SalesTax * dvalue,
					x.ProfitsTax * dvalue)).ToList();
		}

		actor.OutputHandler.Send(
			$"You change the currency of the {Name.ColourName()} economic zone to {Currency.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandInterval(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What do you want to set the financial period interval to?\n{"Use the following form: every <x> hours|days|weekdays|weeks|months|years <offset>".ColourCommand()}");
			return false;
		}

		if (!RecurringInterval.TryParse(command.SafeRemainingArgument, out var interval))
		{
			actor.OutputHandler.Send(
				$"That is not a valid financial period interval.\n{"Use the following form: every <x> hours|days|weekdays|weeks|months|years <offset>".ColourCommand()}");
			return false;
		}

		FinancialPeriodInterval = interval;
		Changed = true;
		CloseCurrentFinancialPeriod();
		actor.OutputHandler.Send(
			$"You change the interval for financial periods to {interval.Describe(FinancialPeriodReferenceCalendar).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTime(ICharacter actor, StringStack command)
	{
		var regex = new Regex(@"(?<hours>\d+):(?<minutes>\d+):(?<seconds>\d+)");
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What time do you want to set as the reference time for new financial periods? Enter the time in the format hh:mm:ss as appropriate for the in-game clock.");
			return false;
		}

		if (!regex.IsMatch(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send("The time must be in the format hh:mm:ss, as appropriate for the in-game clock.");
			return false;
		}

		var match = regex.Match(command.SafeRemainingArgument);
		FinancialPeriodReferenceTime = new MudTime(int.Parse(match.Groups["seconds"].Value),
			int.Parse(match.Groups["minutes"].Value), int.Parse(match.Groups["hours"].Value),
			ZoneForTimePurposes.TimeZone(FinancialPeriodReferenceClock), FinancialPeriodReferenceClock, false);
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} economic zone will now use {FinancialPeriodReferenceTime.Display(TimeDisplayTypes.Short).ColourValue()} as its reference time.");
		return true;
	}

	private bool BuildingCommandZone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which physical zone do you want this economic zone to use as its reference for things like current time?");
			return false;
		}

		var zone = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Zones.Get(value)
			: Gameworld.Zones.GetByName(command.SafeRemainingArgument);
		if (zone == null)
		{
			actor.OutputHandler.Send("There is no such zone.");
			return false;
		}

		ZoneForTimePurposes = zone;
		FinancialPeriodReferenceCalendar = zone.Calendars.First();
		FinancialPeriodReferenceClock = zone.Clocks.First();
		FinancialPeriodReferenceTime = new MudTime(FinancialPeriodReferenceTime.Seconds,
			FinancialPeriodReferenceTime.Minutes, FinancialPeriodReferenceTime.Hours,
			zone.TimeZone(FinancialPeriodReferenceClock), FinancialPeriodReferenceClock, false);
		// TODO - should we be redoing listeners here?
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} economic zone will now link with the {zone.Name.ColourName()} physical zone.");
		return true;
	}

	private bool BuildingCommandPermitLoss(ICharacter actor, StringStack command, bool fromClanCommand)
	{
		PermitTaxableLosses = !PermitTaxableLosses;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} economic zone will {(PermitTaxableLosses ? "now" : "no longer")} permit taxable losses.");
		return true;
	}

	private bool BuildingCommandPrevious(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How many previous financial periods of information do you want this economic zone to retain?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid number greater than zero.");
			return false;
		}

		PreviousFinancialPeriodsToKeep = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} economic zone will now keep {value.ToString("N0", actor).ColourValue()} previous financial periods of information.");
		return true;
	}

	private bool BuildingCommandSalesTax(ICharacter actor, StringStack command, bool fromClanCommand)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandSalesTaxCreate(actor, command, fromClanCommand);
			case "delete":
			case "del":
			case "remove":
			case "rem":
				return BuildingCommandSalesTaxRemove(actor, command, fromClanCommand);
		}

		var tax = _salesTaxes.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
		          _salesTaxes.FirstOrDefault(x =>
			          x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (tax == null)
		{
			actor.OutputHandler.Send(
				fromClanCommand
					? $"This economic zone has no such sales tax. You can either use CLAN ECONOMICZONE {Id.ToString("F0", actor)} SALESTAX ADD <name> <type> to create one, CLAN ECONOMICZONE {Id.ToString("F0", actor)} SALESTAX DELETE <name> to delete one or CLAN ECONOMICZONE {Id.ToString("F0", actor)} SALESTAX <name> <other commands> to edit one."
					: $"This economic zone has no such sales tax. You can either use ECONOMICZONE SET SALESTAX ADD <name> <type> to create one, ECONOMICZONE SET SALESTAX DELETE <name> to delete one or ECONOMICZONE SET SALESTAX <name> <other commands> to edit one.");
			return false;
		}

		return tax.BuildingCommand(actor, command);
	}

	private bool BuildingCommandSalesTaxRemove(ICharacter actor, StringStack command, bool fromClanCommand)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which sales tax do you want to remove?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		var tax = _salesTaxes.FirstOrDefault(x => x.Name.EqualTo(name)) ??
		          _salesTaxes.FirstOrDefault(x => x.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
		if (tax == null)
		{
			actor.OutputHandler.Send("There is no such sales tax.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to delete the {tax.Name.ColourName()} sales tax? This action is irreversible.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send($"You delete the {tax.Name.ColourName()} sales tax.");
				_salesTaxes.Remove(tax);
				tax.Delete();
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send($"You decide not to delete the {tax.Name.ColourName()} sales tax.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send($"You decide not to delete the {tax.Name.ColourName()} sales tax.");
			},
			Keywords = new List<string> { "tax", "delete" },
			DescriptionString = "Deleting a sales tax"
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandSalesTaxCreate(ICharacter actor, StringStack command, bool fromClanCommand)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new tax?");
			return false;
		}

		var name = command.PopSpeech();
		if (_salesTaxes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a sales tax with that name. Names must be unique.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which type of sales tax do you want to create? The options are {TaxFactory.SalesTaxes.Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		var type = command.PopSpeech();
		var tax = TaxFactory.CreateSalesTax(type, name, this);
		if (tax == null)
		{
			actor.OutputHandler.Send(
				$"That is not a valid type of sales tax. The options are {TaxFactory.SalesTaxes.Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		_salesTaxes.Add(tax);
		actor.OutputHandler.Send(
			$"You create a new sales tax of type {type.ColourValue()} called {name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandProfitTax(ICharacter actor, StringStack command, bool fromClanCommand)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandProfitTaxCreate(actor, command, fromClanCommand);
			case "delete":
			case "del":
			case "remove":
			case "rem":
				return BuildingCommandProfitTaxRemove(actor, command, fromClanCommand);
		}

		var tax = _profitTaxes.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
		          _profitTaxes.FirstOrDefault(x =>
			          x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (tax == null)
		{
			actor.OutputHandler.Send(fromClanCommand
				? $"This economic zone has no such profit tax. You can either use CLAN ECONOMICZONE {Id.ToString("F0", actor)} PROFITTAX ADD <name> <type> to create one, CLAN ECONOMICZONE {Id.ToString("F0", actor)} PROFITTAX DELETE <name> to delete one or CLAN ECONOMICZONE {Id.ToString("F0", actor)} PROFITTAX <name> <other commands> to edit one."
				: $"This economic zone has no such profit tax. You can either use ECONOMICZONE SET PROFITTAX ADD <name> <type> to create one, ECONOMICZONE SET PROFITTAX DELETE <name> to delete one or ECONOMICZONE SET PROFITTAX <name> <other commands> to edit one.");
			return false;
		}

		return tax.BuildingCommand(actor, command);
	}

	private bool BuildingCommandProfitTaxRemove(ICharacter actor, StringStack command, bool fromClanCommand)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which profit tax do you want to remove?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		var tax = _profitTaxes.FirstOrDefault(x => x.Name.EqualTo(name)) ??
		          _profitTaxes.FirstOrDefault(x =>
			          x.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
		if (tax == null)
		{
			actor.OutputHandler.Send("There is no such profit tax.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to delete the {tax.Name.ColourName()} profit tax? This action is irreversible.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send($"You delete the {tax.Name.ColourName()} profit tax.");
				_profitTaxes.Remove(tax);
				tax.Delete();
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send($"You decide not to delete the {tax.Name.ColourName()} profit tax.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send($"You decide not to delete the {tax.Name.ColourName()} profit tax.");
			},
			Keywords = new List<string> { "tax", "delete" },
			DescriptionString = "Deleting a profit tax"
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandProfitTaxCreate(ICharacter actor, StringStack command, bool fromClanCommand)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new tax?");
			return false;
		}

		var name = command.PopSpeech();
		if (_profitTaxes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a profit tax with that name. Names must be unique.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which type of profit tax do you want to create? The options are {TaxFactory.ProfitTaxes.Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		var type = command.PopSpeech();
		var tax = TaxFactory.CreateProfitTax(type, name, this);
		if (tax == null)
		{
			actor.OutputHandler.Send(
				$"That is not a valid type of profit tax. The options are {TaxFactory.ProfitTaxes.Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		_profitTaxes.Add(tax);
		actor.OutputHandler.Send(
			$"You create a new profit tax of type {type.ColourValue()} called {name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandConveyance(ICharacter actor, StringStack command)
	{
		if (_conveyancingCells.Contains(actor.Location))
		{
			actor.OutputHandler.Send(
				"Your current location is no longer a location for conveyancing property in this economic zone.");
			_conveyancingCells.Remove(actor.Location);
			actor.Location.CellRequestsDeletion -= ConveyancingCellRequestsDeletion;
		}
		else
		{
			actor.OutputHandler.Send(
				"Your current location is now a location for conveyancing property in this economic zone.");
			_conveyancingCells.Add(actor.Location);
			actor.Location.CellRequestsDeletion -= ConveyancingCellRequestsDeletion;
			actor.Location.CellRequestsDeletion += ConveyancingCellRequestsDeletion;
		}

		Changed = true;
		return true;
	}

	private void ConveyancingCellRequestsDeletion(object sender, EventArgs e)
	{
		_conveyancingCells.Remove((ICell)sender);
	}

	private void JobCellRequestsDeletion(object sender, EventArgs e)
	{
		_jobFindingCells.Remove((ICell)sender);
	}

	private bool BuildingCommandJobs(ICharacter actor, StringStack command)
	{
		if (_jobFindingCells.Contains(actor.Location))
		{
			actor.OutputHandler.Send(
				"Your current location is no longer a location for listing and finding jobs in this economic zone.");
			_jobFindingCells.Remove(actor.Location);
			actor.Location.CellRequestsDeletion -= JobCellRequestsDeletion;
		}
		else
		{
			actor.OutputHandler.Send(
				"Your current location is now a location for listing and finding jobs in this economic zone.");
			_jobFindingCells.Add(actor.Location);
			actor.Location.CellRequestsDeletion -= JobCellRequestsDeletion;
			actor.Location.CellRequestsDeletion += JobCellRequestsDeletion;
		}

		Changed = true;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Economic Zone {Name.ColourName()} (#{Id.ToString("N0", actor)})");
		sb.AppendLine($"Controlling Clan: {ControllingClan?.FullName.ColourName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Currency: {Currency.Name.ColourValue()}");
		sb.AppendLine($"Zone: {ZoneForTimePurposes.Name.ColourValue()}");
		sb.AppendLine($"Clock: {FinancialPeriodReferenceClock.Name.ColourValue()}");
		sb.AppendLine($"Calendar: {FinancialPeriodReferenceCalendar.Name.ColourValue()}");
		sb.AppendLine(
			$"Financial Period Interval: {FinancialPeriodInterval.Describe(FinancialPeriodReferenceCalendar).ColourValue()}");
		sb.AppendLine($"Taxable Losses Permitted: {PermitTaxableLosses.ToColouredString()}");
		sb.AppendLine(
			$"# Financial Periods to Keep: {PreviousFinancialPeriodsToKeep.ToString("N0", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine(
			$"Current Financial Period: {CurrentFinancialPeriod.FinancialPeriodStartMUD.Date.Display(CalendarDisplayMode.Short).ColourName()} to {CurrentFinancialPeriod.FinancialPeriodEndMUD.Date.Display(CalendarDisplayMode.Short).ColourName()}");
		sb.AppendLine(
			$"CFP Revenue: {Currency.Describe(_historicalRevenues.FirstOrDefault(x => x.Period == CurrentFinancialPeriod).TotalTaxRevenue, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Total Held Revenue: {Currency.Describe(TotalRevenueHeld, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Taxes Owed: {Currency.Describe(_shopsOutstandingProfitTaxes.Sum(x => x.Value) + _shopsOutstandingSalesTaxes.Sum(x => x.Value) - _shopsTaxesInCredit.Sum(x => x.Value), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Sales Taxes:");
		foreach (var tax in _salesTaxes)
		{
			sb.AppendLine($"\t[#{tax.Id.ToString("N0", actor)}] {tax.Name.ColourName()} - {tax.MerchantDescription}");
		}

		sb.AppendLine();
		sb.AppendLine("Profit Taxes:");
		foreach (var tax in _profitTaxes)
		{
			sb.AppendLine($"\t[#{tax.Id.ToString("N0", actor)}] {tax.Name.ColourName()} - {tax.MerchantDescription}");
		}

		sb.AppendLine();
		sb.AppendLine("Income Taxes:");
		foreach (var tax in _profitTaxes)
		{
			sb.AppendLine($"\t[#{tax.Id.ToString("N0", actor)}] {tax.Name.ColourName()} - {tax.MerchantDescription}");
		}

		sb.AppendLine();
		sb.AppendLine("Conveyancing Locations:");
		foreach (var location in ConveyancingCells)
		{
			sb.AppendLine($"\t{location.GetFriendlyReference(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("Job Finding Locations:");
		foreach (var location in JobFindingCells)
		{
			sb.AppendLine($"\t{location.GetFriendlyReference(actor)}");
		}

		return sb.ToString();
	}

	#endregion

	#region Estates

	private readonly List<IEstate> _estates = new();
	public IEnumerable<IEstate> Estates => _estates;

	public void AddEstate(IEstate estate)
	{
		_estates.Add(estate);
	}

	public void RemoveEstate(IEstate estate)
	{
		_estates.Remove(estate);
	}

	public MudTimeSpan EstateDefaultDiscoverTime { get; set; } = MudTimeSpan.FromDays(28);
	public MudTimeSpan EstateClaimPeriodLength { get; set; } = MudTimeSpan.FromDays(14);

	#endregion

	#region Property

	private readonly List<ICell> _conveyancingCells = new();
	public IEnumerable<ICell> ConveyancingCells => _conveyancingCells;

	#endregion

	private readonly List<ICell> _jobFindingCells = new();
	public IEnumerable<ICell> JobFindingCells => _jobFindingCells;
}