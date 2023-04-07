using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ClanMembershipBackpay
    {
        public long ClanId { get; set; }
        public long CharacterId { get; set; }
        public long CurrencyId { get; set; }
        public decimal Amount { get; set; }

        public virtual ClanMembership C { get; set; }
        public virtual Currency Currency { get; set; }
    }
}
