using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RanksPaygrade
    {
        public long RankId { get; set; }
        public long PaygradeId { get; set; }
        public int Order { get; set; }

        public virtual Paygrade Paygrade { get; set; }
        public virtual Rank Rank { get; set; }
    }
}
