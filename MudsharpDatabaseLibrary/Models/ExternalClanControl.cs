using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ExternalClanControl
    {
        public ExternalClanControl()
        {
            ExternalClanControlsAppointments = new HashSet<ExternalClanControlsAppointment>();
        }

        public long VassalClanId { get; set; }
        public long LiegeClanId { get; set; }
        public long ControlledAppointmentId { get; set; }
        public long? ControllingAppointmentId { get; set; }
        public int NumberOfAppointments { get; set; }

        public virtual Appointment ControlledAppointment { get; set; }
        public virtual Appointment ControllingAppointment { get; set; }
        public virtual Clan LiegeClan { get; set; }
        public virtual Clan VassalClan { get; set; }
        public virtual ICollection<ExternalClanControlsAppointment> ExternalClanControlsAppointments { get; set; }
    }
}
