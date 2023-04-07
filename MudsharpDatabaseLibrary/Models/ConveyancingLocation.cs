using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class ConveyancingLocation
	{
		public long EconomicZoneId { get; set; }
		public long CellId { get; set; }

		public virtual EconomicZone EconomicZone { get; set; }
		public virtual Cell Cell { get; set; }
	}
}
