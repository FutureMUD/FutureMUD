using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenRolesCurrency
    {
        public long ChargenRoleId { get; set; }
        public long CurrencyId { get; set; }
        public decimal Amount { get; set; }

        public virtual ChargenRole ChargenRole { get; set; }
        public virtual Currency Currency { get; set; }
    }
}
