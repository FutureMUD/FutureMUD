using Castle.Components.DictionaryAdapter;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.Economy.Property;
using MudSharp.Economy.Tax;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Statements;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Object = System.Object;

namespace MudSharp.Economy;

public class EconomicZone : SaveableItem, IEconomicZone
{
    private readonly All<FinancialPeriod> _financialPeriods = new();
    public ICurrency Currency { get; private set; }

    private readonly List<ISalesTax> _salesTaxes = new();
    public IEnumerable<ISalesTax> SalesTaxes => _salesTaxes;
    private readonly List<IProfitTax> _profitTaxes = new();
    public IEnumerable<IProfitTax> ProfitTaxes => _profitTaxes;
    private readonly List<IHotelTax> _hotelTaxes = new();
    public IEnumerable<IHotelTax> HotelTaxes => _hotelTaxes;
    private readonly List<IIncomeTax> _incomeTaxes = new();
    public IEnumerable<IIncomeTax> IncomeTaxes => _incomeTaxes;

    public IFinancialPeriod CurrentFinancialPeriod { get; private set; }
    public int PreviousFinancialPeriodsToKeep { get; private set; }
    public IZone ZoneForTimePurposes { get; private set; }
    public bool PermitTaxableLosses { get; private set; }
    public decimal OutstandingTaxesOwed { get; private set; }
    private decimal _totalRevenueHeld;
    public decimal TotalRevenueHeld
    {
        get => _totalRevenueHeld;
        set
        {
            _totalRevenueHeld = value;
            Changed = true;
        }
    }
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
    private long? _estateAuctionHouseId;
    private IAuctionHouse _estateAuctionHouse;

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
            MudTime.FromLocalTime(0, 0, 0, FinancialPeriodTimezone, FinancialPeriodReferenceClock);
        EstatesEnabled = true;
        EstateDefaultDiscoverTime = MudTimeSpan.FromDays(28);
        EstateClaimPeriodLength = MudTimeSpan.FromDays(14);
        using (new FMDB())
        {
            Models.EconomicZone dbitem = new()
            {
                IntervalModifier = FinancialPeriodInterval.Modifier,
                IntervalOther = FinancialPeriodInterval.SecondaryModifier,
                IntervalFallback = (int)FinancialPeriodInterval.OrdinalFallbackMode,
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
                EstateAuctionHouseId = null,
                EstatesEnabled = EstatesEnabled,
                EstateDefaultDiscoverTime = EstateDefaultDiscoverTime.GetRoundTripParseText,
                EstateClaimPeriodLength = EstateClaimPeriodLength.GetRoundTripParseText,
                MorgueOfficeLocationId = null,
                MorgueStorageLocationId = null,
                ZoneForTimePurposesId = ZoneForTimePurposes.Id
            };
            FMDB.Context.EconomicZones.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;

            MudDateTime periodstart = new(
                new MudDate(FinancialPeriodReferenceCalendar, 1, FinancialPeriodReferenceCalendar.CurrentDate.Year,
                    FinancialPeriodReferenceCalendar.CurrentDate.ThisYear.Months.First(),
                    FinancialPeriodReferenceCalendar.CurrentDate.ThisYear, false), FinancialPeriodReferenceTime,
                FinancialPeriodTimezone);
            Year nextYear =
                FinancialPeriodReferenceCalendar.CreateYear(FinancialPeriodReferenceCalendar.CurrentDate.Year + 1);
            MudDateTime periodend =
                new(
                    new MudDate(FinancialPeriodReferenceCalendar, 1,
                        FinancialPeriodReferenceCalendar.CurrentDate.Year + 1, nextYear.Months.First(), nextYear,
                        false), FinancialPeriodReferenceTime, FinancialPeriodTimezone);

            Models.FinancialPeriod dbperiod = new()
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

            FinancialPeriod period = new(dbperiod, this, Gameworld);
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
        _estateAuctionHouseId = zone.EstateAuctionHouseId;
        _morgueOfficeCell = gameworld.Cells.Get(zone.MorgueOfficeLocationId ?? 0);
        _morgueStorageCell = gameworld.Cells.Get(zone.MorgueStorageLocationId ?? 0);
        foreach (Models.FinancialPeriod period in zone.FinancialPeriods)
        {
            _financialPeriods.Add(new FinancialPeriod(period, this, gameworld));
        }

        OutstandingTaxesOwed = zone.OutstandingTaxesOwed;
        TotalRevenueHeld = zone.TotalRevenueHeld;
        PreviousFinancialPeriodsToKeep = zone.PreviousFinancialPeriodsToKeep;
        ZoneForTimePurposes = gameworld.Zones.Get(zone.ZoneForTimePurposesId);
        PermitTaxableLosses = zone.PermitTaxableLosses;
        EstatesEnabled = zone.EstatesEnabled;
        EstateDefaultDiscoverTime = !string.IsNullOrWhiteSpace(zone.EstateDefaultDiscoverTime)
            ? MudTimeSpan.Parse(zone.EstateDefaultDiscoverTime)
            : MudTimeSpan.FromDays(28);
        EstateClaimPeriodLength = !string.IsNullOrWhiteSpace(zone.EstateClaimPeriodLength)
            ? MudTimeSpan.Parse(zone.EstateClaimPeriodLength)
            : MudTimeSpan.FromDays(14);
        FinancialPeriodInterval = new RecurringInterval
        {
            IntervalAmount = zone.IntervalAmount,
            Modifier = zone.IntervalModifier,
            SecondaryModifier = zone.IntervalOther,
            OrdinalFallbackMode = (OrdinalFallbackMode)zone.IntervalFallback,
            Type = (IntervalType)zone.IntervalType
        };

        FinancialPeriodReferenceClock = gameworld.Clocks.Get(zone.ReferenceClockId);
        FinancialPeriodReferenceCalendar = gameworld.Calendars.Get(zone.ReferenceCalendarId ?? 0);
        FinancialPeriodReferenceTime = FinancialPeriodReferenceClock.GetStoredTimeOrFallback(zone.ReferenceTime,
            StoredMudTimeFallback.CurrentTime, "EconomicZone", zone.Id, zone.Name, "ReferenceTime",
            FinancialPeriodReferenceCalendar);
        FinancialPeriodTimezone = FinancialPeriodReferenceTime.Timezone;

        CurrentFinancialPeriod = _financialPeriods.Get(zone.CurrentFinancialPeriodId ?? 0) ??
                                 _financialPeriods.WhereMax(x => x.FinancialPeriodStart).FirstOrDefault();
        bool createListener = true;
        if (FinancialPeriodReferenceCalendar != null)
        {
            if (CurrentFinancialPeriod == null)
            {
                DateTime now = DateTime.UtcNow;
                MudDateTime mudnow = FinancialPeriodReferenceCalendar.CurrentDateTime;
                FinancialPeriod period = new(this, now, FinancialPeriodInterval.GetNextDateTime(now), mudnow,
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
                new object[] { }, $"End Current Financial Period EZ #{Id} {Name}"));
        }

        foreach (EconomicZoneTax item in zone.EconomicZoneTaxes)
        {
            if (TaxFactory.IsSalesTax(item))
            {
                _salesTaxes.Add(TaxFactory.LoadSalesTax(item, this));
            }
            else if (TaxFactory.IsHotelTax(item))
            {
                _hotelTaxes.Add(TaxFactory.LoadHotelTax(item, this));
            }
            else
            {
                _profitTaxes.Add(TaxFactory.LoadProfitTax(item, this));
            }
        }

        foreach (EconomicZoneShopTax item in zone.EconomicZoneShopTaxes)
        {
            _shopsOutstandingProfitTaxes[item.ShopId] = item.OutstandingProfitTaxes;
            _shopsOutstandingSalesTaxes[item.ShopId] = item.OutstandingSalesTaxes;
            _shopsTaxesInCredit[item.ShopId] = item.TaxesInCredits;
        }

        foreach (EconomicZoneRevenue item in zone.EconomicZoneRevenues)
        {
            _historicalRevenues.Add((_financialPeriods.Get(item.FinancialPeriodId), item.TotalTaxRevenue));
        }

        foreach (ShopFinancialPeriodResult item in zone.ShopFinancialPeriodResults)
        {
            _shopsPreviousFinancialPeriodResults.Add(item.ShopId,
                new FinancialPeriodResult(_financialPeriods.Get(item.FinancialPeriodId), item.GrossRevenue,
                    item.NetRevenue, item.SalesTax, item.ProfitsTax));
        }

        foreach (ConveyancingLocation location in zone.ConveyancingLocations)
        {
            ICell cell = Gameworld.Cells.Get(location.CellId);
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

        foreach (JobFindingLocation location in zone.JobFindingLocations)
        {
            ICell cell = Gameworld.Cells.Get(location.CellId);
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

        foreach (ProbateLocation location in zone.ProbateLocations)
        {
            ICell cell = Gameworld.Cells.Get(location.CellId);
#if DEBUG
            if (cell == null)
            {
                throw new ApplicationException("Cell shouldn't be null in EconomicZone constructor");
            }
#endif
            _probateOfficeCells.Add(cell);
            cell.CellRequestsDeletion -= ProbateCellRequestsDeletion;
            cell.CellRequestsDeletion += ProbateCellRequestsDeletion;
        }

        if (_morgueOfficeCell != null)
        {
            _morgueOfficeCell.CellRequestsDeletion -= MorgueOfficeCellRequestsDeletion;
            _morgueOfficeCell.CellRequestsDeletion += MorgueOfficeCellRequestsDeletion;
        }

        if (_morgueStorageCell != null)
        {
            _morgueStorageCell.CellRequestsDeletion -= MorgueStorageCellRequestsDeletion;
            _morgueStorageCell.CellRequestsDeletion += MorgueStorageCellRequestsDeletion;
        }
    }

    private void ReferenceCalendarOnDaysUpdated()
    {
        foreach (IBank bank in Gameworld.Banks.Where(x => x.EconomicZone == this))
        {
            bank.ReferenceDateOnDateChanged();
        }

        foreach (IEstate estate in Estates.ToList())
        {
            estate.CheckStatus();
        }
    }

    public IAuctionHouse EstateAuctionHouse
    {
        get
        {
            if (_estateAuctionHouse == null && _estateAuctionHouseId.HasValue)
            {
                _estateAuctionHouse = Gameworld.AuctionHouses.Get(_estateAuctionHouseId.Value);
            }

            return _estateAuctionHouse;
        }
        private set
        {
            _estateAuctionHouse = value;
            _estateAuctionHouseId = value?.Id;
            Changed = true;
        }
    }

    public IEconomicZone Clone(string newName)
    {
        newName = newName.ToLowerInvariant().TitleCase();
        using (new FMDB())
        {
            Models.EconomicZone olditem = FMDB.Context.EconomicZones.Find(Id);
            Models.EconomicZone dbitem = new()
            {
                CurrentFinancialPeriodId = CurrentFinancialPeriod?.Id,
                IntervalModifier = FinancialPeriodInterval.Modifier,
                IntervalOther = FinancialPeriodInterval.SecondaryModifier,
                IntervalFallback = (int)FinancialPeriodInterval.OrdinalFallbackMode,
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
                ReferenceTime = FinancialPeriodReferenceTime.GetTimeString(),
                EstateAuctionHouseId = EstateAuctionHouse?.Id,
                EstatesEnabled = EstatesEnabled,
                EstateDefaultDiscoverTime = EstateDefaultDiscoverTime.GetRoundTripParseText,
                EstateClaimPeriodLength = EstateClaimPeriodLength.GetRoundTripParseText,
                MorgueOfficeLocationId = MorgueOfficeCell?.Id,
                MorgueStorageLocationId = MorgueStorageCell?.Id
            };

            foreach (EconomicZoneTax tax in olditem.EconomicZoneTaxes)
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
        Models.EconomicZone dbitem = FMDB.Context.EconomicZones.Find(Id);
        dbitem.CurrentFinancialPeriodId = CurrentFinancialPeriod?.Id;
        dbitem.IntervalModifier = FinancialPeriodInterval.Modifier;
        dbitem.IntervalOther = FinancialPeriodInterval.SecondaryModifier;
        dbitem.IntervalFallback = (int)FinancialPeriodInterval.OrdinalFallbackMode;
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
        dbitem.EstateAuctionHouseId = EstateAuctionHouse?.Id;
        dbitem.EstatesEnabled = EstatesEnabled;
        dbitem.EstateDefaultDiscoverTime = EstateDefaultDiscoverTime.GetRoundTripParseText;
        dbitem.EstateClaimPeriodLength = EstateClaimPeriodLength.GetRoundTripParseText;
        dbitem.MorgueOfficeLocationId = MorgueOfficeCell?.Id;
        dbitem.MorgueStorageLocationId = MorgueStorageCell?.Id;

        FMDB.Context.EconomicZoneRevenues.RemoveRange(dbitem.EconomicZoneRevenues);
        foreach ((IFinancialPeriod Period, decimal TotalTaxRevenue) item in _historicalRevenues)
        {
            EconomicZoneRevenue newHR = new()
            {
                FinancialPeriodId = item.Period.Id,
                TotalTaxRevenue = item.TotalTaxRevenue,
                EconomicZone = dbitem
            };
            dbitem.EconomicZoneRevenues.Add(newHR);
        }

        FMDB.Context.EconomicZoneShopTaxes.RemoveRange(dbitem.EconomicZoneShopTaxes);
        foreach (long shop in _shopsOutstandingSalesTaxes.Keys.Concat(_shopsTaxesInCredit.Keys)
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
        foreach (ICell location in ConveyancingCells)
        {
            dbitem.ConveyancingLocations.Add(new ConveyancingLocation
            {
                CellId = location.Id,
                EconomicZoneId = Id
            });
        }

        FMDB.Context.JobFindingLocations.RemoveRange(dbitem.JobFindingLocations);
        foreach (ICell location in JobFindingCells)
        {
            dbitem.JobFindingLocations.Add(new JobFindingLocation
            {
                CellId = location.Id,
                EconomicZoneId = Id
            });
        }

        FMDB.Context.ProbateLocations.RemoveRange(dbitem.ProbateLocations);
        foreach (ICell location in ProbateOfficeCells)
        {
            dbitem.ProbateLocations.Add(new ProbateLocation
            {
                CellId = location.Id,
                EconomicZoneId = Id
            });
        }

        Changed = false;
    }

    public void SaveFinancialPeriodResults()
    {
        using (new FMDB())
        {
            Models.EconomicZone dbitem = FMDB.Context.EconomicZones.Find(Id);
            FMDB.Context.ShopFinancialPeriodResults.RemoveRange(dbitem.ShopFinancialPeriodResults);
            foreach ((long key, List<FinancialPeriodResult> results) in _shopsPreviousFinancialPeriodResults)
            {
                foreach (FinancialPeriodResult result in results)
                {
                    dbitem.ShopFinancialPeriodResults.Add(new ShopFinancialPeriodResult
                    {
                        EconomicZoneId = Id,
                        ShopId = key,
                        FinancialPeriodId = result.Period.Id,
                        GrossRevenue = result.GrossRevenue,
                        NetRevenue = result.NetRevenue,
                        SalesTax = result.SalesTax,
                        ProfitsTax = result.ProfitsTax
                    });
                }
            }

            FMDB.Context.SaveChanges();
        }
    }

    public override string FrameworkItemType => "EconomicZone";

    private readonly DecimalCounter<long> _shopsOutstandingProfitTaxes = new();
    private readonly DecimalCounter<long> _shopsOutstandingSalesTaxes = new();
    private readonly DecimalCounter<long> _shopsTaxesInCredit = new();

    private readonly CollectionDictionary<long, FinancialPeriodResult> _shopsPreviousFinancialPeriodResults = new();

    public FinancialPeriodResult FinancialPeriodResultForShop(IShop shop, IFinancialPeriod period)
    {
        return _shopsPreviousFinancialPeriodResults[shop.Id].FirstOrDefault(x => x.Period.Equals(period));
    }

    public void CloseCurrentFinancialPeriod()
    {
        IFinancialPeriod oldPeriod = CurrentFinancialPeriod;
        DateTime oldPeriodRealStart = oldPeriod.FinancialPeriodStart;
        DateTime oldPeriodRealEnd = DateTime.UtcNow;
        oldPeriod.FinancialPeriodEnd = oldPeriodRealEnd;
        oldPeriod.Changed = true;
        MudDateTime oldPeriodMudEnd = oldPeriod.FinancialPeriodEndMUD;

        MudDate newEnd = FinancialPeriodInterval.GetNextDateExclusive(oldPeriodMudEnd.Calendar, oldPeriodMudEnd.Date);
        DateTime newRealEnd = FinancialPeriodInterval.GetNextDateTime(oldPeriodRealEnd);

        FinancialPeriod newPeriod = new(this, oldPeriodRealEnd, newRealEnd, oldPeriodMudEnd,
            new MudDateTime(newEnd, FinancialPeriodReferenceTime, FinancialPeriodTimezone));
        _financialPeriods.Add(newPeriod);
        CurrentFinancialPeriod = newPeriod;

        decimal totalPeriodTax = 0.0M;
        foreach (IShop shop in Gameworld.Shops.Where(x => x.EconomicZone == this))
        {
            List<ITransactionRecord> transactions = shop.TransactionRecords.Where(x => x.RealDateTime >= oldPeriodRealStart).ToList();
            (decimal salesTax, decimal grossRevenue, decimal netRevenue) = transactions.Sum3(x => x.Tax,
                x => x.TransactionType == ShopTransactionType.Sale ? x.PretaxValue : 0.0M, x => x.NetValue);
            totalPeriodTax += salesTax;
            _shopsOutstandingSalesTaxes[shop.Id] += salesTax;
            decimal profitTax = 0.0M;
            foreach (IProfitTax tax in ProfitTaxes)
            {
                if (!tax.Applies(shop, grossRevenue, netRevenue))
                {
                    continue;
                }

                decimal value = tax.TaxValue(shop, grossRevenue, netRevenue);
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
            int count = _financialPeriods.Count - PreviousFinancialPeriodsToKeep;
            List<FinancialPeriod> periodsToRemove = _financialPeriods.OrderBy(x => x.FinancialPeriodStart).Take(count).ToList();
            foreach (FinancialPeriod period in periodsToRemove)
            {
                period.Delete();
                _financialPeriods.Remove(period);
                _historicalRevenues.RemoveAll(x => x.Period == period);
                _shopsPreviousFinancialPeriodResults.RemoveAll(x => x.Period == period);
            }
        }

        Changed = true;
        SaveFinancialPeriodResults();
        Gameworld.Add(new TimeAndDate.Listeners.DateListener(
            CurrentFinancialPeriod.FinancialPeriodEndMUD, 0,
            payload => { CloseCurrentFinancialPeriod(); },
            new object[] { }, $"Close Financial Period for EZ #{Id} {Name}"));
    }

    public decimal OutstandingTaxesForShop(IShop shop)
    {
        return _shopsOutstandingProfitTaxes[shop.Id] + _shopsOutstandingSalesTaxes[shop.Id] -
               _shopsTaxesInCredit[shop.Id];
    }

    public void ForgiveTaxesForShop(IShop shop, decimal amount = 0.0M)
    {
        Changed = true;
        if (amount <= 0.0M)
        {
            _shopsOutstandingSalesTaxes[shop.Id] = 0.0M;
            _shopsOutstandingProfitTaxes[shop.Id] = 0.0M;
            return;
        }

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

    public void PayTaxesForShop(IShop shop, decimal amount)
    {
        Changed = true;
        TotalRevenueHeld += amount;
        VirtualCashLedger.Credit(this, Currency, amount, null, shop, "ShopTax",
            $"Shop taxes paid by {shop.Name}",
            FinancialPeriodReferenceCalendar.CurrentDateTime, shop);
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

    public decimal CalculateHotelTax(IProperty property, ICharacter patron, decimal rentalCharge)
    {
        return HotelTaxes
               .Where(x => x.Applies(property, patron))
               .Sum(x => x.TaxValue(property, patron, rentalCharge));
    }

    #region Implementation of IEditableItem

    public string HelpInfo => @"You can use the following options with this command:

	#3name <name>#0 - renames this economic zone
	#3currency <currency>#0 - changes the currency used in this zone
	#3clock <clock>#0 - changes the clock used in this zone
	#3calendar <calendar>#0 - changes the calendar used in this zone
	#3interval <interval>#0 - sets the interval for financial periods
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
	#3hoteltax add <type> <name>#0 - adds a new hotel tax
	#3hoteltax remove <name>#0 - removes a hotel tax
	#3hoteltax <which> <...>#0 - edit properties of a particular tax
	#3realty#0 - toggles your current location as a conveyancing/realty location
	#3jobs#0 - toggles your current location as a job listing and finding location
	#3probate#0 - toggles your current location as a probate office
	#3morgueoffice <here|none>#0 - sets or clears the morgue office for this economic zone
	#3morguestorage <here|none>#0 - sets or clears the morgue storage room for this economic zone
	#3forgive <shop> <amount>#0 - forgives a certain amount of owing tax for a shop (excess gives credits)
	#3forgive <shop> all#0 - forgives all owing taxes for a shop
	#3shops#0 - lists all shops in this economic zone
	#3shop <which>#0 - shows tax information about a shop in the zone
	#3estates#0 - toggles whether this economic zone creates new estates
	#3estatediscovery <time>#0 - sets how long estates remain undiscovered before probate opens
	#3estateclaimperiod <time>#0 - sets how long claims remain open on discovered estates
	#3estateauctionhouse <which>|none#0 - sets the default auction house for estate liquidation
	#3revenue deposit|withdraw <amount>#0 - moves cash into or out of held economic-zone revenue
	#3revenue ledger [count]#0 - reviews held-revenue ledger entries
	#3taxinfo#0 - shows you information about tax revenues in this zone";

    public string ClanHelpInfo => @"You can use the following options with this command:

	#3permitloss#0 - toggles permitting taxable losses
	#3clan <clan>#0 - assigns a new clan to custody of this economic zone
	#3clan none#0 - clears clan control of this economic zone
	#3salestax add <type> <name>#0 - adds a new sales tax
	#3salestax remove <name>#0 - removes a sales tax
	#3salestax <which> <...>#0 - edit properties of a particular tax
	#3profittax add <type> <name>#0 - adds a new profit tax
	#3profittax remove <name>#0 - removes a profit tax
	#3profittax <which> <...>#0 - edit properties of a particular tax
	#3hoteltax add <type> <name>#0 - adds a new hotel tax
	#3hoteltax remove <name>#0 - removes a hotel tax
	#3hoteltax <which> <...>#0 - edit properties of a particular tax
	#3forgive <shop> <amount>#0 - forgives a certain amount of owing tax for a shop (excess gives credits)
	#3forgive <shop> all#0 - forgives all owing taxes for a shop
	#3shops#0 - lists all shops in this economic zone
	#3shop <which>#0 - shows tax information about a shop in the zone
	#3estates#0 - toggles whether this economic zone creates new estates
	#3estatediscovery <time>#0 - sets how long estates remain undiscovered before probate opens
	#3estateclaimperiod <time>#0 - sets how long claims remain open on discovered estates
	#3estateauctionhouse <which>|none#0 - sets the default auction house for estate liquidation
	#3revenue deposit|withdraw <amount>#0 - moves cash into or out of held economic-zone revenue
	#3revenue ledger [count]#0 - reviews held-revenue ledger entries
	#3taxinfo#0 - shows you information about tax revenues in this zone";

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
            case "hoteltax":
                return BuildingCommandHotelTax(actor, command, false);
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
            case "probate":
            case "probateoffice":
                return BuildingCommandProbate(actor, command);
            case "morgueoffice":
                return BuildingCommandMorgueOffice(actor, command);
            case "morguestorage":
                return BuildingCommandMorgueStorage(actor, command);
            case "forgive":
            case "forgivetaxes":
                return BuildingCommandForgiveTaxes(actor, command);
            case "endperiod":
                return BuildingCommandEndFinancialPeriod(actor, command);
            case "shop":
                return BuildingCommandShop(actor, command);
            case "estates":
            case "estate":
                return BuildingCommandEstatesEnabled(actor, command);
            case "estatediscovery":
            case "estatediscover":
            case "discovery":
                return BuildingCommandEstateDiscovery(actor, command);
            case "estateclaimperiod":
            case "estateclaims":
            case "claimperiod":
                return BuildingCommandEstateClaimPeriod(actor, command);
            case "estateauctionhouse":
            case "estateauction":
            case "probateauction":
                return BuildingCommandEstateAuctionHouse(actor, command);
            case "taxinfo":
                return BuildingCommandTaxInfo(actor, command);
            case "revenue":
            case "treasury":
                return BuildingCommandRevenue(actor, command);
            default:
                actor.OutputHandler.Send(HelpInfo.SubstituteANSIColour());
                return false;
        }
    }

    public bool BuildingCommandFromClanCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "permitloss":
                return BuildingCommandPermitLoss(actor, command, true);
            case "salestax":
                return BuildingCommandSalesTax(actor, command, true);
            case "profittax":
                return BuildingCommandProfitTax(actor, command, true);
            case "hoteltax":
                return BuildingCommandHotelTax(actor, command, true);
            case "clan":
                return BuildingCommandClan(actor, command, true);
            case "forgive":
            case "forgivetaxes":
                return BuildingCommandForgiveTaxes(actor, command);
            case "endperiod":
                return BuildingCommandEndFinancialPeriod(actor, command);
            case "shop":
                return BuildingCommandShop(actor, command);
            case "shops":
                return BuildingCommandShops(actor);
            case "estates":
            case "estate":
                return BuildingCommandEstatesEnabled(actor, command);
            case "estatediscovery":
            case "estatediscover":
            case "discovery":
                return BuildingCommandEstateDiscovery(actor, command);
            case "estateclaimperiod":
            case "estateclaims":
            case "claimperiod":
                return BuildingCommandEstateClaimPeriod(actor, command);
            case "estateauctionhouse":
            case "estateauction":
            case "probateauction":
                return BuildingCommandEstateAuctionHouse(actor, command);
            case "taxinfo":
            case "tax":
                return BuildingCommandTaxInfo(actor, command);
            case "revenue":
            case "treasury":
                return BuildingCommandRevenue(actor, command);
            default:
                actor.OutputHandler.Send(ClanHelpInfo.SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandShops(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"List of shops for economic zone {Name.TitleCase().ColourName()}");
        sb.AppendLine();
        sb.AppendLine(StringUtilities.GetTextTable(
            from shop in Gameworld.Shops
            where shop.EconomicZone == this
            let pshop = shop as IPermanentShop
            select new[]
            {
                shop.Id.ToString("N0", actor),
                shop.Name.TitleCase(),
                shop.IsTrading.ToString(actor),
                shop.EmployeeRecords.Count().ToString("N0", actor),
                shop.EmployeesOnDuty.Count().ToString("N0", actor),
                pshop?.ShopfrontCells.Select(x =>
                    x.GetFriendlyReference(actor).FluentTagMXP("send",
                        $"href='goto {x.Id}'")).FirstOrDefault() ?? "",
                shop.EconomicZone.Name,
                (shop is ITransientShop).ToColouredString()
            },
            new[]
            {
                "ID",
                "Name",
                "Open?",
                "Employs",
                "Working",
                "Storefront",
                "Economic Zone",
                "Transient?"
            },
            actor,
            colour: Telnet.Green
        ));

        actor.OutputHandler.Send(sb.ToString());
        return true;
    }

    private bool BuildingCommandTaxInfo(ICharacter actor, StringStack command)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Tax Information for {Name}".GetLineWithTitle(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
        sb.AppendLine();
        sb.AppendLine($"Revenue Held: {Currency.Describe(TotalRevenueHeld, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        decimal totalOwed =
            _shopsOutstandingProfitTaxes.Sum(x => x.Value) +
            _shopsOutstandingSalesTaxes.Sum(x => x.Value) -
            _shopsTaxesInCredit.Sum(x => x.Value);
        sb.AppendLine($"Uncollected Tax Revenue: {Currency.Describe(totalOwed, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        sb.AppendLine();
        sb.AppendLine("Previous Financial Period Results:");
        sb.AppendLine(StringUtilities.GetTextTable(
            from item in _historicalRevenues
            orderby item.Period.FinancialPeriodStartMUD descending
            select new List<string>
            {
                item.Period.FinancialPeriodStartMUD.Date.Display(CalendarDisplayMode.Short).ColourValue(),
                item.Period.FinancialPeriodEndMUD.Date.Display(CalendarDisplayMode.Short).ColourValue(),
                Currency.Describe(item.TotalTaxRevenue, CurrencyDescriptionPatternType.ShortDecimal).ColourValue(),
            },
            new List<string>
            {
                "Start",
                "End",
                "Total Tax Revenue",
            },
            actor,
            Telnet.FunctionYellow));
        actor.OutputHandler.Send(sb.ToString());
        return true;
    }

    private void EnsureRevenueLedgerBalance()
    {
        var balance = VirtualCashLedger.Balance(this, Currency);
        if (TotalRevenueHeld > balance)
        {
            VirtualCashLedger.Credit(this, Currency, TotalRevenueHeld - balance, null, this, "OpeningBalance",
                $"Opening held revenue balance for {Name}", FinancialPeriodReferenceCalendar.CurrentDateTime);
        }
    }

    private bool BuildingCommandRevenue(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "deposit":
            case "add":
                return BuildingCommandRevenueDeposit(actor, command);
            case "withdraw":
            case "remove":
                return BuildingCommandRevenueWithdraw(actor, command);
            case "ledger":
            case "history":
                return BuildingCommandRevenueLedger(actor, command);
            default:
                actor.OutputHandler.Send("Use DEPOSIT <amount>, WITHDRAW <amount>, or LEDGER [count].");
                return false;
        }
    }

    private bool BuildingCommandRevenueDeposit(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount) ||
            amount <= 0.0M)
        {
            actor.OutputHandler.Send($"How much {Currency.Name.ColourName()} do you want to deposit?");
            return false;
        }

        var payment = new OtherCashPayment(Currency, actor);
        var accessible = payment.AccessibleMoneyForPayment();
        if (accessible < amount)
        {
            actor.OutputHandler.Send(
                $"You only have {Currency.Describe(accessible, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} available.");
            return false;
        }

        payment.TakePayment(amount);
        TotalRevenueHeld += amount;
        VirtualCashLedger.Credit(this, Currency, amount, actor, actor, "Cash",
            $"Manual held revenue deposit for {Name}", FinancialPeriodReferenceCalendar.CurrentDateTime);
        actor.OutputHandler.Send(
            $"You deposit {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} into held revenue for {Name.ColourName()}.");
        return true;
    }

    private bool BuildingCommandRevenueWithdraw(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount) ||
            amount <= 0.0M)
        {
            actor.OutputHandler.Send($"How much {Currency.Name.ColourName()} do you want to withdraw?");
            return false;
        }

        if (TotalRevenueHeld < amount)
        {
            actor.OutputHandler.Send(
                $"{Name.ColourName()} only has {Currency.Describe(TotalRevenueHeld, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in held revenue.");
            return false;
        }

        EnsureRevenueLedgerBalance();
        if (!VirtualCashLedger.Debit(this, Currency, amount, actor, actor, "CashWithdrawal",
                $"Manual held revenue withdrawal for {Name}", null, FinancialPeriodReferenceCalendar.CurrentDateTime,
                out var error))
        {
            actor.OutputHandler.Send(error);
            return false;
        }

        TotalRevenueHeld -= amount;
        IGameItem cash = CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency,
            Currency.FindCoinsForAmount(amount, out _));
        if (actor.Body.CanGet(cash, 0))
        {
            actor.Body.Get(cash, silent: true);
        }
        else
        {
            cash.RoomLayer = actor.RoomLayer;
            actor.Location.Insert(cash, true);
            actor.OutputHandler.Send("You couldn't hold the money, so it is on the ground.");
        }

        actor.OutputHandler.Send(
            $"You withdraw {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} from held revenue for {Name.ColourName()}.");
        return true;
    }

    private bool BuildingCommandRevenueLedger(ICharacter actor, StringStack command)
    {
        EnsureRevenueLedgerBalance();
        var count = 25;
        if (!command.IsFinished && (!int.TryParse(command.SafeRemainingArgument, out count) || count <= 0))
        {
            actor.OutputHandler.Send("How many ledger entries do you want to review?");
            return false;
        }

        var entries = VirtualCashLedger.LedgerEntries(this, count).ToList();
        if (!entries.Any())
        {
            actor.OutputHandler.Send($"{Name.ColourName()} does not have any held revenue ledger entries.");
            return false;
        }

        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            entries.Select(x => new List<string>
            {
                x.RealDateTime.ToString("g", actor),
                x.ActorName ?? string.Empty,
                Currency.Describe(x.Amount, CurrencyDescriptionPatternType.ShortDecimal),
                Currency.Describe(x.BalanceAfter, CurrencyDescriptionPatternType.ShortDecimal),
                $"{x.SourceKind}->{x.DestinationKind}",
                x.Reason
            }),
            new List<string> { "When", "Actor", "Amount", "Balance", "Route", "Reason" },
            actor.LineFormatLength,
            colour: Telnet.Green,
            unicodeTable: actor.Account.UseUnicode));
        return true;
    }

    private bool BuildingCommandEstateDiscovery(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How long should estates in this economic zone remain undiscovered before probate opens?");
            return false;
        }

        if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out MudTimeSpan value) || (TimeSpan)value <= TimeSpan.Zero)
        {
            actor.OutputHandler.Send("That is not a valid positive amount of time.");
            return false;
        }

        EstateDefaultDiscoverTime = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"Estates in the {Name.ColourName()} economic zone will now remain undiscovered for {value.Describe(actor).ColourValue()} before claims open.");
        return true;
    }

    private bool BuildingCommandEstatesEnabled(ICharacter actor, StringStack command)
    {
        EstatesEnabled = !EstatesEnabled;
        Changed = true;
        actor.OutputHandler.Send(
            $"The {Name.ColourName()} economic zone will {(EstatesEnabled ? "now" : "no longer")} create new estates when characters die.");
        return true;
    }

    private bool BuildingCommandEstateClaimPeriod(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How long should claims remain open for discovered estates in this economic zone?");
            return false;
        }

        if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out MudTimeSpan value) || (TimeSpan)value <= TimeSpan.Zero)
        {
            actor.OutputHandler.Send("That is not a valid positive amount of time.");
            return false;
        }

        EstateClaimPeriodLength = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"Discovered estates in the {Name.ColourName()} economic zone will now remain open to claims for {value.Describe(actor).ColourValue()}.");
        return true;
    }

    private bool BuildingCommandEstateAuctionHouse(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which auction house should this economic zone use for estate liquidation?");
            return false;
        }

        if (command.SafeRemainingArgument.EqualToAny("none", "clear"))
        {
            EstateAuctionHouse = null;
            actor.OutputHandler.Send(
                $"The {Name.ColourName()} economic zone will no longer use any default auction house for estate liquidation.");
            return true;
        }

        IAuctionHouse house = long.TryParse(command.SafeRemainingArgument, out long value)
            ? Gameworld.AuctionHouses.Get(value)
            : Gameworld.AuctionHouses.GetByName(command.SafeRemainingArgument);
        if (house == null)
        {
            actor.OutputHandler.Send("There is no such auction house.");
            return false;
        }

        if (house.EconomicZone != this)
        {
            actor.OutputHandler.Send("That auction house does not belong to this economic zone.");
            return false;
        }

        EstateAuctionHouse = house;
        actor.OutputHandler.Send(
            $"The {Name.ColourName()} economic zone will now use {house.Name.ColourName()} for estate liquidation.");
        return true;
    }

    private bool BuildingCommandShop(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which shop do you want to view economic information about?");
            return false;
        }

        IShop shop = Gameworld.Shops.Where(x => x.EconomicZone == this).GetByIdOrName(command.SafeRemainingArgument);
        if (shop is null)
        {
            actor.OutputHandler.Send("This economic zone has no such shop.");
            return false;
        }

        StringBuilder sb = new();
        sb.AppendLine($"Shop #{shop.Id.ToString("N0", actor)} ({shop.Name.TitleCase()})".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
        sb.AppendLine();
        sb.AppendLine($"Tax Credits: {Currency.Describe(_shopsTaxesInCredit[shop.Id], CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        sb.AppendLine($"Profit Tax Owed: {Currency.Describe(_shopsOutstandingProfitTaxes[shop.Id], CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        sb.AppendLine($"Sales Tax Owed: {Currency.Describe(_shopsOutstandingSalesTaxes[shop.Id], CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        sb.AppendLine();
        sb.AppendLine("Financial Period Results:");
        sb.AppendLine();
        sb.AppendLine(StringUtilities.GetTextTable(
            from item in _shopsPreviousFinancialPeriodResults[shop.Id]
            orderby item.Period.FinancialPeriodStartMUD descending
            select new List<string>
            {
                item.Period.FinancialPeriodStartMUD.Date.Display(CalendarDisplayMode.Short).ColourValue(),
                item.Period.FinancialPeriodEndMUD.Date.Display(CalendarDisplayMode.Short).ColourValue(),
                Currency.Describe(item.GrossRevenue, CurrencyDescriptionPatternType.ShortDecimal).ColourValue(),
                Currency.Describe(item.NetRevenue, CurrencyDescriptionPatternType.ShortDecimal).ColourValue(),
                Currency.Describe(item.ProfitsTax, CurrencyDescriptionPatternType.ShortDecimal).ColourValue(),
                Currency.Describe(item.SalesTax, CurrencyDescriptionPatternType.ShortDecimal).ColourValue(),
            },
            new List<string>
            {
                "Start",
                "End",
                "Gross Revenue",
                "Net Revenue",
                "Profit Tax",
                "Sales Tax"
            },
            actor,
            Telnet.FunctionYellow
        ));
        actor.OutputHandler.Send(sb.ToString());
        return true;
    }

    private bool BuildingCommandEndFinancialPeriod(ICharacter actor, StringStack command)
    {
        CloseCurrentFinancialPeriod();
        actor.OutputHandler.Send("You manually end the current financial period.");
        return true;
    }

    private bool BuildingCommandForgiveTaxes(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which shop do you want to forgive taxes for?");
            return false;
        }

        IShop shop = Gameworld.Shops.Where(x => x.EconomicZone == this).GetByIdOrName(command.PopSpeech());
        if (shop is null)
        {
            actor.OutputHandler.Send($"This economic zone does not have any such shop.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"How much of the outstanding taxes from {shop.Name.TitleCase().ColourName()} do you want to forgive?");
            return false;
        }

        decimal outstanding = OutstandingTaxesForShop(shop);
        if (command.SafeRemainingArgument.EqualTo("all"))
        {
            ForgiveTaxesForShop(shop);
            actor.OutputHandler.Send($"You forgive all of the taxes that {shop.Name.TitleCase().ColourName()} owes, totalling {Currency.Describe(outstanding, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
            return true;
        }

        if (!Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out decimal amount))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of {Currency.Name.ColourValue()}.");
            return false;
        }

        ForgiveTaxesForShop(shop, amount);
        decimal remaining = OutstandingTaxesForShop(shop);
        actor.OutputHandler.Send($"You forgive {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} of the taxes owed by {shop.Name.TitleCase().ColourName()}, taking their total from {Currency.Describe(outstanding, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} to {Currency.Describe(remaining, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
        return true;
    }

    public bool BuildingCommandCalendar(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                "Which calendar would you like to use as the reference for financial periods and economic activity in this zone?");
            return false;
        }

        ICalendar calendar = actor.Gameworld.Calendars.GetByIdOrNames(command.SafeRemainingArgument);
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
                foreach (FinancialPeriod period in _financialPeriods.ToList())
                {
                    period.Delete();
                }

                _financialPeriods.Clear();
                _shopsPreviousFinancialPeriodResults.Clear();
                _historicalRevenues.Clear();
                DateTime now = DateTime.UtcNow;
                MudDateTime mudnow = FinancialPeriodReferenceCalendar.CurrentDateTime;
                FinancialPeriod newPeriod = new(this, now, FinancialPeriodInterval.GetNextDateTime(now), mudnow,
                    FinancialPeriodInterval.GetNextDateTime(mudnow));
                _financialPeriods.Add(newPeriod);
                CurrentFinancialPeriod = newPeriod;
                Changed = true;
                Gameworld.Add(new TimeAndDate.Listeners.DateListener(
                    CurrentFinancialPeriod.FinancialPeriodEndMUD, 0,
                    payload => { CloseCurrentFinancialPeriod(); },
                    new object[] { }, $"Close Financial Period for EZ #{Id} {Name}"));
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

		return true;
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

        IClan clan = long.TryParse(command.PopSpeech(), out long value)
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

        string name = command.PopSpeech().ToLowerInvariant().TitleCase();
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

        ICurrency currency = long.TryParse(command.PopSpeech(), out long value)
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

        if (!decimal.TryParse(command.SafeRemainingArgument, out decimal dvalue))
        {
            actor.OutputHandler.Send("That is not a valid multiplier.");
            return false;
        }

        Currency = currency;
        Changed = true;
        foreach ((IFinancialPeriod Period, decimal TotalTaxRevenue) revenue in _historicalRevenues.ToList())
        {
            _historicalRevenues[_historicalRevenues.IndexOf(revenue)] =
                (revenue.Period, revenue.TotalTaxRevenue * dvalue);
        }

        TotalRevenueHeld *= dvalue;
        OutstandingTaxesOwed *= dvalue;

        foreach (KeyValuePair<long, decimal> tax in _shopsOutstandingProfitTaxes.ToList())
        {
            _shopsOutstandingProfitTaxes[tax.Key] = tax.Value * dvalue;
        }

        foreach (KeyValuePair<long, decimal> tax in _shopsOutstandingSalesTaxes.ToList())
        {
            _shopsOutstandingSalesTaxes[tax.Key] = tax.Value * dvalue;
        }

        foreach (KeyValuePair<long, decimal> tax in _shopsTaxesInCredit.ToList())
        {
            _shopsTaxesInCredit[tax.Key] = tax.Value * dvalue;
        }

        foreach (KeyValuePair<long, List<FinancialPeriodResult>> item in _shopsPreviousFinancialPeriodResults.ToList())
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
                $"What do you want to set the financial period interval to?\n{"Use forms like: every <x> hours|days|weekdays|weeks|months|years <offset>, every month on day 15, or every month on the 5th or last Wednesday".ColourCommand()}");
            return false;
        }

        if (!RecurringInterval.TryParse(command.SafeRemainingArgument, FinancialPeriodReferenceCalendar, out RecurringInterval interval, out string intervalError))
        {
            actor.OutputHandler.Send(
                $"That is not a valid financial period interval: {intervalError}\n{"Use forms like: every <x> hours|days|weekdays|weeks|months|years <offset>, every month on day 15, or every month on the 5th or last Wednesday".ColourCommand()}");
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
        Regex regex = new(@"(?<hours>\d+):(?<minutes>\d+):(?<seconds>\d+)");
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

        Match match = regex.Match(command.SafeRemainingArgument);
        FinancialPeriodReferenceTime = MudTime.FromLocalTime(int.Parse(match.Groups["seconds"].Value),
            int.Parse(match.Groups["minutes"].Value), int.Parse(match.Groups["hours"].Value),
            ZoneForTimePurposes.TimeZone(FinancialPeriodReferenceClock), FinancialPeriodReferenceClock);
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

        IZone zone = long.TryParse(command.SafeRemainingArgument, out long value)
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
        FinancialPeriodReferenceTime = MudTime.FromLocalTime(FinancialPeriodReferenceTime.Seconds,
            FinancialPeriodReferenceTime.Minutes, FinancialPeriodReferenceTime.Hours,
            zone.TimeZone(FinancialPeriodReferenceClock), FinancialPeriodReferenceClock);
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

        if (!int.TryParse(command.SafeRemainingArgument, out int value) || value <= 0)
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

        ISalesTax tax = _salesTaxes.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
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

        string name = command.SafeRemainingArgument;
        ISalesTax tax = _salesTaxes.FirstOrDefault(x => x.Name.EqualTo(name)) ??
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

        string name = command.PopSpeech();
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

        string type = command.PopSpeech();
        ISalesTax tax = TaxFactory.CreateSalesTax(type, name, this);
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

        IProfitTax tax = _profitTaxes.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
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

        string name = command.SafeRemainingArgument;
        IProfitTax tax = _profitTaxes.FirstOrDefault(x => x.Name.EqualTo(name)) ??
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

        string name = command.PopSpeech();
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

        string type = command.PopSpeech();
        IProfitTax tax = TaxFactory.CreateProfitTax(type, name, this);
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

    private bool BuildingCommandHotelTax(ICharacter actor, StringStack command, bool fromClanCommand)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "add":
            case "new":
            case "create":
                return BuildingCommandHotelTaxCreate(actor, command, fromClanCommand);
            case "delete":
            case "del":
            case "remove":
            case "rem":
                return BuildingCommandHotelTaxRemove(actor, command, fromClanCommand);
        }

        IHotelTax tax = _hotelTaxes.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
                  _hotelTaxes.FirstOrDefault(x =>
                      x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
        if (tax == null)
        {
            actor.OutputHandler.Send(fromClanCommand
                ? $"This economic zone has no such hotel tax. You can either use CLAN ECONOMICZONE {Id.ToString("F0", actor)} HOTELTAX ADD <name> <type> to create one, CLAN ECONOMICZONE {Id.ToString("F0", actor)} HOTELTAX DELETE <name> to delete one or CLAN ECONOMICZONE {Id.ToString("F0", actor)} HOTELTAX <name> <other commands> to edit one."
                : $"This economic zone has no such hotel tax. You can either use ECONOMICZONE SET HOTELTAX ADD <name> <type> to create one, ECONOMICZONE SET HOTELTAX DELETE <name> to delete one or ECONOMICZONE SET HOTELTAX <name> <other commands> to edit one.");
            return false;
        }

        return tax.BuildingCommand(actor, command);
    }

    private bool BuildingCommandHotelTaxRemove(ICharacter actor, StringStack command, bool fromClanCommand)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which hotel tax do you want to remove?");
            return false;
        }

        string name = command.SafeRemainingArgument;
        IHotelTax tax = _hotelTaxes.FirstOrDefault(x => x.Name.EqualTo(name)) ??
                  _hotelTaxes.FirstOrDefault(x =>
                      x.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
        if (tax == null)
        {
            actor.OutputHandler.Send("There is no such hotel tax.");
            return false;
        }

        actor.OutputHandler.Send(
            $"Are you sure you want to delete the {tax.Name.ColourName()} hotel tax? This action is irreversible.\n{Accept.StandardAcceptPhrasing}");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                actor.OutputHandler.Send($"You delete the {tax.Name.ColourName()} hotel tax.");
                _hotelTaxes.Remove(tax);
                tax.Delete();
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send($"You decide not to delete the {tax.Name.ColourName()} hotel tax.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send($"You decide not to delete the {tax.Name.ColourName()} hotel tax.");
            },
            Keywords = new List<string> { "tax", "delete" },
            DescriptionString = "Deleting a hotel tax"
        }), TimeSpan.FromSeconds(120));
        return true;
    }

    private bool BuildingCommandHotelTaxCreate(ICharacter actor, StringStack command, bool fromClanCommand)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to your new tax?");
            return false;
        }

        string name = command.PopSpeech();
        if (_hotelTaxes.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send("There is already a hotel tax with that name. Names must be unique.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which type of hotel tax do you want to create? The options are {TaxFactory.HotelTaxes.Select(x => x.ColourValue()).ListToString()}.");
            return false;
        }

        string type = command.PopSpeech();
        IHotelTax tax = TaxFactory.CreateHotelTax(type, name, this);
        if (tax == null)
        {
            actor.OutputHandler.Send(
                $"That is not a valid type of hotel tax. The options are {TaxFactory.HotelTaxes.Select(x => x.ColourValue()).ListToString()}.");
            return false;
        }

        _hotelTaxes.Add(tax);
        actor.OutputHandler.Send(
            $"You create a new hotel tax of type {type.ColourValue()} called {name.ColourName()}.");
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

    private void ProbateCellRequestsDeletion(object sender, EventArgs e)
    {
        _probateOfficeCells.Remove((ICell)sender);
    }

    private void MorgueOfficeCellRequestsDeletion(object sender, EventArgs e)
    {
        _morgueOfficeCell = null;
        Changed = true;
    }

    private void MorgueStorageCellRequestsDeletion(object sender, EventArgs e)
    {
        _morgueStorageCell = null;
        Changed = true;
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

    private bool BuildingCommandProbate(ICharacter actor, StringStack command)
    {
        if (_probateOfficeCells.Contains(actor.Location))
        {
            actor.OutputHandler.Send(
                "Your current location is no longer a probate office in this economic zone.");
            _probateOfficeCells.Remove(actor.Location);
            actor.Location.CellRequestsDeletion -= ProbateCellRequestsDeletion;
        }
        else
        {
            actor.OutputHandler.Send(
                "Your current location is now a probate office in this economic zone.");
            _probateOfficeCells.Add(actor.Location);
            actor.Location.CellRequestsDeletion -= ProbateCellRequestsDeletion;
            actor.Location.CellRequestsDeletion += ProbateCellRequestsDeletion;
        }

        Changed = true;
        return true;
    }

    private bool BuildingCommandMorgueOffice(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || command.PeekSpeech().EqualTo("here"))
        {
            MorgueOfficeCell = actor.Location;
            actor.OutputHandler.Send(
                $"{actor.Location.GetFriendlyReference(actor)} is now the morgue office for this economic zone.");
            return true;
        }

        if (!command.PeekSpeech().EqualTo("none") && !command.PeekSpeech().EqualTo("clear"))
        {
            actor.OutputHandler.Send("You must specify either HERE or NONE.");
            return false;
        }

        MorgueOfficeCell = null;
        actor.OutputHandler.Send("This economic zone no longer has a morgue office configured.");
        return true;
    }

    private bool BuildingCommandMorgueStorage(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || command.PeekSpeech().EqualTo("here"))
        {
            MorgueStorageCell = actor.Location;
            actor.OutputHandler.Send(
                $"{actor.Location.GetFriendlyReference(actor)} is now the morgue storage room for this economic zone.");
            return true;
        }

        if (!command.PeekSpeech().EqualTo("none") && !command.PeekSpeech().EqualTo("clear"))
        {
            actor.OutputHandler.Send("You must specify either HERE or NONE.");
            return false;
        }

        MorgueStorageCell = null;
        actor.OutputHandler.Send("This economic zone no longer has a morgue storage room configured.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Economic Zone {Name.ColourName()} (#{Id.ToString("N0", actor)})");
        sb.AppendLine($"Controlling Clan: {ControllingClan?.FullName.ColourName() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine($"Currency: {Currency.Name.ColourValue()}");
        sb.AppendLine($"Zone: {ZoneForTimePurposes.Name.ColourValue()}");
        sb.AppendLine($"Clock: {FinancialPeriodReferenceClock.Name.ColourValue()}");
        sb.AppendLine($"Calendar: {FinancialPeriodReferenceCalendar.Name.ColourValue()}");
        sb.AppendLine(
            $"Financial Period Interval: {FinancialPeriodInterval.Describe(FinancialPeriodReferenceCalendar).ColourValue()}");
        sb.AppendLine($"Estates Enabled: {EstatesEnabled.ToColouredString()}");
        sb.AppendLine($"Estate Discovery Delay: {EstateDefaultDiscoverTime.Describe(actor).ColourValue()}");
        sb.AppendLine($"Estate Claim Period: {EstateClaimPeriodLength.Describe(actor).ColourValue()}");
        sb.AppendLine(
            $"Estate Auction House: {EstateAuctionHouse?.Name.ColourName() ?? "None".ColourError()}");
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
        foreach (ISalesTax tax in _salesTaxes)
        {
            sb.AppendLine($"\t[#{tax.Id.ToString("N0", actor)}] {tax.Name.ColourName()} - {tax.MerchantDescription}");
        }

        sb.AppendLine();
        sb.AppendLine("Profit Taxes:");
        foreach (IProfitTax tax in _profitTaxes)
        {
            sb.AppendLine($"\t[#{tax.Id.ToString("N0", actor)}] {tax.Name.ColourName()} - {tax.MerchantDescription}");
        }

        sb.AppendLine();
        sb.AppendLine("Hotel Taxes:");
        foreach (IHotelTax tax in _hotelTaxes)
        {
            sb.AppendLine($"\t[#{tax.Id.ToString("N0", actor)}] {tax.Name.ColourName()} - {tax.MerchantDescription}");
        }

        sb.AppendLine();
        sb.AppendLine("Income Taxes:");
        foreach (IProfitTax tax in _profitTaxes)
        {
            sb.AppendLine($"\t[#{tax.Id.ToString("N0", actor)}] {tax.Name.ColourName()} - {tax.MerchantDescription}");
        }

        sb.AppendLine();
        sb.AppendLine("Conveyancing Locations:");
        foreach (ICell location in ConveyancingCells)
        {
            sb.AppendLine($"\t{location.GetFriendlyReference(actor)}");
        }

        sb.AppendLine();
        sb.AppendLine("Job Finding Locations:");
        foreach (ICell location in JobFindingCells)
        {
            sb.AppendLine($"\t{location.GetFriendlyReference(actor)}");
        }

        sb.AppendLine();
        sb.AppendLine("Probate Office Locations:");
        foreach (ICell location in ProbateOfficeCells)
        {
            sb.AppendLine($"\t{location.GetFriendlyReference(actor)}");
        }

        sb.AppendLine();
        sb.AppendLine($"Morgue Office: {MorgueOfficeCell?.GetFriendlyReference(actor) ?? "None".ColourError()}");
        sb.AppendLine($"Morgue Storage: {MorgueStorageCell?.GetFriendlyReference(actor) ?? "None".ColourError()}");

        return sb.ToString();
    }

    #endregion

    #region Estates

    private readonly List<IEstate> _estates = new();
    public IEnumerable<IEstate> Estates => _estates;
    public bool EstatesEnabled { get; private set; }

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

    private readonly List<ICell> _probateOfficeCells = new();
    public IEnumerable<ICell> ProbateOfficeCells => _probateOfficeCells;

    private ICell _morgueOfficeCell;
    public ICell MorgueOfficeCell
    {
        get => _morgueOfficeCell;
        private set
        {
            _morgueOfficeCell?.CellRequestsDeletion -= MorgueOfficeCellRequestsDeletion;

            _morgueOfficeCell = value;

            if (_morgueOfficeCell != null)
            {
                _morgueOfficeCell.CellRequestsDeletion -= MorgueOfficeCellRequestsDeletion;
                _morgueOfficeCell.CellRequestsDeletion += MorgueOfficeCellRequestsDeletion;
            }

            Changed = true;
        }
    }

    private ICell _morgueStorageCell;
    public ICell MorgueStorageCell
    {
        get => _morgueStorageCell;
        private set
        {
            _morgueStorageCell?.CellRequestsDeletion -= MorgueStorageCellRequestsDeletion;

            _morgueStorageCell = value;

            if (_morgueStorageCell != null)
            {
                _morgueStorageCell.CellRequestsDeletion -= MorgueStorageCellRequestsDeletion;
                _morgueStorageCell.CellRequestsDeletion += MorgueStorageCellRequestsDeletion;
            }

            Changed = true;
        }
    }
}
