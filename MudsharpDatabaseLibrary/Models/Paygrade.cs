using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Paygrade
    {
        public Paygrade()
        {
            Appointments = new HashSet<Appointment>();
            RanksPaygrades = new HashSet<RanksPaygrade>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public long CurrencyId { get; set; }
        public decimal PayAmount { get; set; }
        public long ClanId { get; set; }

        public virtual Clan Clan { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<RanksPaygrade> RanksPaygrades { get; set; }
    }
}
