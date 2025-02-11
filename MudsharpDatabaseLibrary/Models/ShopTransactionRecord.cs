using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ShopTransactionRecord
    {
        public long Id { get; set; }
        public long CurrencyId { get; set; }
        public decimal PretaxValue { get; set; }
        public decimal Tax { get; set; }
        public int TransactionType { get; set; }
        public long ShopId { get; set; }
        public long? ThirdPartyId { get; set; }
        public DateTime RealDateTime { get; set; }
        public string MudDateTime { get; set; }
        public long? MerchandiseId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual Merchandise Merchandise { get; set; }
    }
}
