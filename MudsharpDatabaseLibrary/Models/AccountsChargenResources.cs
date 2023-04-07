using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class AccountsChargenResources
    {
        public long AccountId { get; set; }
        public long ChargenResourceId { get; set; }
        public int Amount { get; set; }
        public DateTime LastAwardDate { get; set; }

        public virtual Account Account { get; set; }
        public virtual ChargenResource ChargenResource { get; set; }
    }
}
