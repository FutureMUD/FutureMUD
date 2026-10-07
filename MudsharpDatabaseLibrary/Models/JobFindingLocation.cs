using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class JobFindingLocation
    {
        public long EconomicZoneId { get; set; }
        public long RoomId { get; set; }

        public virtual EconomicZone EconomicZone { get; set; }
        public virtual Room Room { get; set; }
    }
}
