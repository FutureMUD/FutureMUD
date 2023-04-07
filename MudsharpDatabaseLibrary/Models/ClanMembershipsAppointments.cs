using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ClanMembershipsAppointments
    {
        public long ClanId { get; set; }
        public long CharacterId { get; set; }
        public long AppointmentId { get; set; }

        public virtual Appointment Appointment { get; set; }
        public virtual ClanMembership ClanMembership { get; set; }
    }
}
