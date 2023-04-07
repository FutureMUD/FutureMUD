using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class BankCurrencyReserve
	{
		public long BankId { get; set; }
		public long CurrencyId { get; set; }
		public decimal Amount { get; set; }

		public virtual Bank Bank { get; set; }
		public virtual Currency Currency { get; set; }
	}
}
