using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public partial class PatrolRouteTimeOfDay
    {
        public PatrolRouteTimeOfDay()
        {

        }

        public long PatrolRouteId { get; set; }
        public int TimeOfDay { get; set; }

        public virtual PatrolRoute PatrolRoute { get; set; }
    }
}
