using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class BankManagerAuditLog
	{
		public long Id { get; set; }
		public long BankId { get; set; }
		public long CharacterId { get; set; }
		public string DateTime { get; set; }
		public string Detail { get; set; }

		public virtual Bank Bank { get; set; }
		public virtual Character Character { get; set; }
	}
}
