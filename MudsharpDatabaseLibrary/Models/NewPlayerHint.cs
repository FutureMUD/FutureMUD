using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class NewPlayerHint
	{
		public long Id { get; set; }
		public string Text { get; set; }
		public long? FilterProgId { get; set; }
		public int Priority { get; set; }
		public bool CanRepeat { get; set; }

		public virtual FutureProg FilterProg { get; set; }
	}
}
