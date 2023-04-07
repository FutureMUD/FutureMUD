using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ClanMembership
    {
        public ClanMembership()
        {
            ClanMembershipsAppointments = new HashSet<ClanMembershipsAppointments>();
            ClanMembershipsBackpay = new HashSet<ClanMembershipBackpay>();
            ExternalClanControlsAppointments = new HashSet<ExternalClanControlsAppointment>();
        }

        public long ClanId { get; set; }
        public long CharacterId { get; set; }
        public long RankId { get; set; }
        public long? PaygradeId { get; set; }
        public string JoinDate { get; set; }
        public long? ManagerId { get; set; }
        public string PersonalName { get; set; }
        public bool ArchivedMembership { get; set; }

        public virtual Character Character { get; set; }
        public virtual Clan Clan { get; set; }
        public virtual Character Manager { get; set; }
        public virtual ICollection<ClanMembershipsAppointments> ClanMembershipsAppointments { get; set; }
        public virtual ICollection<ClanMembershipBackpay> ClanMembershipsBackpay { get; set; }
        public virtual ICollection<ExternalClanControlsAppointment> ExternalClanControlsAppointments { get; set; }
    }
}
