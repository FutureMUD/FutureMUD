using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class PropertyKey
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public long GameItemId { get; set; }
		public long PropertyId { get; set; }
		public string AddedToPropertyOnDate { get; set; }
		public decimal CostToReplace { get; set; }
		public bool IsReturned { get; set; }

		public virtual GameItem GameItem { get; set; }
		public virtual Property Property { get; set; }
	}
}
