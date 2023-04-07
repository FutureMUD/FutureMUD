using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public class LineOfCreditAccountUser
    {
        public long LineOfCreditAccountId { get; set; }
        public virtual LineOfCreditAccount LineOfCreditAccount { get; set; }

        public long AccountUserId { get; set; }
        public virtual Character AccountUser { get; set; }
        public string AccountUserName { get; set; }

        public decimal? SpendingLimit { get; set; }
    }
}
