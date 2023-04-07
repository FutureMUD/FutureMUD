using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class AppointmentsAbbreviations
    {
        public long AppointmentId { get; set; }
        public string Abbreviation { get; set; }
        public long? FutureProgId { get; set; }
        public int Order { get; set; }

        public virtual Appointment Appointment { get; set; }
        public virtual FutureProg FutureProg { get; set; }
    }
}
