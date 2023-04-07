using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenRolesClanMembershipsAppointments
    {
        public long ChargenRoleId { get; set; }
        public long ClanId { get; set; }
        public long AppointmentId { get; set; }

        public virtual ChargenRolesClanMemberships ChargenRolesClanMembership { get; set; }
    }
}
