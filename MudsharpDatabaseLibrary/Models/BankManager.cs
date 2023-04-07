using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class BankManager
	{
		public long BankId { get; set; }
		public long CharacterId { get; set; }

		public virtual Bank Bank { get; set; }
		public virtual Character Character { get; set; }
	}
}
