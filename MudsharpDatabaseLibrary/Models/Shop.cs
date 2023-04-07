using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Shop
    {
        public Shop()
        {
            EconomicZoneShopTaxes = new HashSet<EconomicZoneShopTax>();
            Merchandises = new HashSet<Merchandise>();
            ShopFinancialPeriodResults = new HashSet<ShopFinancialPeriodResult>();
            ShopTransactionRecords = new HashSet<ShopTransactionRecord>();
            ShopsStoreroomCells = new HashSet<ShopsStoreroomCell>();
            ShopsTills = new HashSet<ShopsTill>();
            LineOfCreditAccounts = new HashSet<LineOfCreditAccount>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? WorkshopCellId { get; set; }
        public long? StockroomCellId { get; set; }
        public long? CanShopProgId { get; set; }
        public long? WhyCannotShopProgId { get; set; }
        public long CurrencyId { get; set; }
        public bool IsTrading { get; set; }
        public long EconomicZoneId { get; set; }
        public string EmployeeRecords { get; set; } 
        public decimal CashBalance { get; set; }
        public long? BankAccountId { get; set; }

        public virtual FutureProg CanShopProg { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual EconomicZone EconomicZone { get; set; }
        public virtual Cell StockroomCell { get; set; }
        public virtual FutureProg WhyCannotShopProg { get; set; }
        public virtual Cell WorkshopCell { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual ICollection<EconomicZoneShopTax> EconomicZoneShopTaxes { get; set; }
        public virtual ICollection<Merchandise> Merchandises { get; set; }
        public virtual ICollection<ShopFinancialPeriodResult> ShopFinancialPeriodResults { get; set; }
        public virtual ICollection<ShopTransactionRecord> ShopTransactionRecords { get; set; }
        public virtual ICollection<ShopsStoreroomCell> ShopsStoreroomCells { get; set; }
        public virtual ICollection<ShopsTill> ShopsTills { get; set; }
        public virtual ICollection<LineOfCreditAccount> LineOfCreditAccounts { get; set; }
    }
}
