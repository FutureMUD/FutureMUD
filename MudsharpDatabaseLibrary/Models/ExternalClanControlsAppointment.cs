using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ExternalClanControlsAppointment
    {
        public long VassalClanId { get; set; }
        public long LiegeClanId { get; set; }
        public long ControlledAppointmentId { get; set; }
        public long CharacterId { get; set; }

        public virtual ClanMembership ClanMemberships { get; set; }
        public virtual ExternalClanControl ExternalClanControls { get; set; }
    }
}
