using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenRolesClanMemberships
    {
        public ChargenRolesClanMemberships()
        {
            ChargenRolesClanMembershipsAppointments = new HashSet<ChargenRolesClanMembershipsAppointments>();
        }

        public long ChargenRoleId { get; set; }
        public long ClanId { get; set; }
        public long RankId { get; set; }
        public long? PaygradeId { get; set; }

        public virtual ChargenRole ChargenRole { get; set; }
        public virtual Clan Clan { get; set; }
        public virtual Rank Rank { get; set; }
        public virtual Paygrade Paygrade { get; set; }
        public virtual ICollection<ChargenRolesClanMembershipsAppointments> ChargenRolesClanMembershipsAppointments { get; set; }
    }
}
