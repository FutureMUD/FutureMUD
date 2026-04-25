using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Economy
{
    public interface IEconomicZone : IEditableItem, ISaveable
    {
        IZone ZoneForTimePurposes { get; }
        ICurrency Currency { get; }
        IEnumerable<ISalesTax> SalesTaxes { get; }
        IEnumerable<IProfitTax> ProfitTaxes { get; }
        IEnumerable<IIncomeTax> IncomeTaxes { get; }
        IEnumerable<IHotelTax> HotelTaxes { get; }

        IFinancialPeriod CurrentFinancialPeriod { get; }
        RecurringInterval FinancialPeriodInterval { get; }
        IClock FinancialPeriodReferenceClock { get; }
        ICalendar FinancialPeriodReferenceCalendar { get; }
        IMudTimeZone FinancialPeriodTimezone { get; }
        IEnumerable<IFinancialPeriod> FinancialPeriods { get; }
        IClan ControllingClan { get; set; }

        void CloseCurrentFinancialPeriod();
        decimal OutstandingTaxesForShop(IShop shop);
        void PayTaxesForShop(IShop shop, decimal amount);
        void ForgiveTaxesForShop(IShop shop, decimal amount = 0.0M);
        void ReportSalesTaxCollected(IShop shop, decimal amount);
        decimal CalculateHotelTax(IProperty property, ICharacter patron, decimal rentalCharge);
        IEnumerable<(IFinancialPeriod Period, decimal TotalTaxRevenue)> HistoricalRevenues { get; }
        FinancialPeriodResult FinancialPeriodResultForShop(IShop shop, IFinancialPeriod period);
        decimal TotalRevenueHeld { get; set; }


        IEnumerable<IEstate> Estates { get; }
        void AddEstate(IEstate estate);
        void RemoveEstate(IEstate estate);
        bool EstatesEnabled { get; }
        MudTimeSpan EstateDefaultDiscoverTime { get; }
        MudTimeSpan EstateClaimPeriodLength { get; }
        IAuctionHouse EstateAuctionHouse { get; }

        IEnumerable<ICell> ConveyancingCells { get; }
        IEnumerable<ICell> JobFindingCells { get; }
        IEnumerable<ICell> ProbateOfficeCells { get; }
        ICell MorgueOfficeCell { get; }
        ICell MorgueStorageCell { get; }

        bool BuildingCommandFromClanCommand(ICharacter actor, StringStack command);
        IEconomicZone Clone(string newName);
    }
}
