using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class PropertyLocation
	{
		public long PropertyId { get; set; }
		public long CellId { get; set; }

		public virtual Property Property { get; set; }
		public virtual Cell Cell { get; set; }
	}
}
