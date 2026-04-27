using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EconomicZone
    {
        public EconomicZone()
        {
            EconomicZoneRevenues = new HashSet<EconomicZoneRevenue>();
            EconomicZoneShopTaxes = new HashSet<EconomicZoneShopTax>();
            FinancialPeriods = new HashSet<FinancialPeriod>();
            ShopFinancialPeriodResults = new HashSet<ShopFinancialPeriodResult>();
            Shops = new HashSet<Shop>();
            EconomicZoneTaxes = new HashSet<EconomicZoneTax>();
            Properties = new HashSet<Property>();
            ConveyancingLocations = new HashSet<ConveyancingLocation>();
            Estates = new HashSet<Estate>();
            JobListings = new HashSet<JobListing>();
            JobFindingLocations = new HashSet<JobFindingLocation>();
            ProbateLocations = new HashSet<ProbateLocation>();
            Shoppers = new HashSet<Shopper>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int PreviousFinancialPeriodsToKeep { get; set; }
        public long ZoneForTimePurposesId { get; set; }
        public bool PermitTaxableLosses { get; set; }
        public decimal OutstandingTaxesOwed { get; set; }
        public long CurrencyId { get; set; }
        public long? CurrentFinancialPeriodId { get; set; }
        public long? ReferenceCalendarId { get; set; }
        public long ReferenceClockId { get; set; }
        public string ReferenceTime { get; set; }
        public int IntervalType { get; set; }
        public int IntervalModifier { get; set; }
        public int IntervalOther { get; set; }
        public int IntervalFallback { get; set; }
        public int IntervalAmount { get; set; }
        public decimal TotalRevenueHeld { get; set; }
        public long? ControllingClanId { get; set; }
        public long? EstateAuctionHouseId { get; set; }
        public bool EstatesEnabled { get; set; }
        public string EstateDefaultDiscoverTime { get; set; }
        public string EstateClaimPeriodLength { get; set; }
        public long? MorgueOfficeLocationId { get; set; }
        public long? MorgueStorageLocationId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual FinancialPeriod CurrentFinancialPeriod { get; set; }
        public virtual Calendar ReferenceCalendar { get; set; }
        public virtual Clock ReferenceClock { get; set; }
        public virtual Clan ControllingClan { get; set; }
        public virtual AuctionHouse EstateAuctionHouse { get; set; }
        public virtual Cell MorgueOfficeLocation { get; set; }
        public virtual Cell MorgueStorageLocation { get; set; }
        public virtual Timezone ReferenceClockNavigation { get; set; }
        public virtual ICollection<EconomicZoneRevenue> EconomicZoneRevenues { get; set; }
        public virtual ICollection<EconomicZoneShopTax> EconomicZoneShopTaxes { get; set; }
        public virtual ICollection<FinancialPeriod> FinancialPeriods { get; set; }
        public virtual ICollection<ShopFinancialPeriodResult> ShopFinancialPeriodResults { get; set; }
        public virtual ICollection<Shop> Shops { get; set; }
        public virtual ICollection<EconomicZoneTax> EconomicZoneTaxes { get; set; }
        public virtual ICollection<Estate> Estates { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<ConveyancingLocation> ConveyancingLocations { get; set; }
        public virtual ICollection<JobFindingLocation> JobFindingLocations { get; set; }
        public virtual ICollection<ProbateLocation> ProbateLocations { get; set; }
        public virtual ICollection<JobListing> JobListings { get; set; }
        public virtual ICollection<Shopper> Shoppers { get; set; }
    }
}
