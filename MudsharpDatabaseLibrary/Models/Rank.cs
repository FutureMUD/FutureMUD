using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Rank
    {
        public Rank()
        {
            AppointmentsMinimumRank = new HashSet<Appointment>();
            AppointmentsMinimumRankToAppoint = new HashSet<Appointment>();
            RanksAbbreviations = new HashSet<RanksAbbreviations>();
            RanksPaygrades = new HashSet<RanksPaygrade>();
            RanksTitles = new HashSet<RanksTitle>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? InsigniaGameItemId { get; set; }
        public int? InsigniaGameItemRevnum { get; set; }
        public long ClanId { get; set; }
        public long Privileges { get; set; }
        public string RankPath { get; set; }
        public int RankNumber { get; set; }
        public int FameType { get; set; }

        public virtual Clan Clan { get; set; }
        public virtual GameItemProto InsigniaGameItem { get; set; }
        public virtual ICollection<Appointment> AppointmentsMinimumRank { get; set; }
        public virtual ICollection<Appointment> AppointmentsMinimumRankToAppoint { get; set; }
        public virtual ICollection<RanksAbbreviations> RanksAbbreviations { get; set; }
        public virtual ICollection<RanksPaygrade> RanksPaygrades { get; set; }
        public virtual ICollection<RanksTitle> RanksTitles { get; set; }
    }
}
