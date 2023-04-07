using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class BankExchangeRate
	{
		public long BankId { get; set; }
		public long FromCurrencyId { get; set; }
		public long ToCurrencyId { get; set; }
		public decimal ExchangeRate { get;set; }

		public virtual Bank Bank { get; set; }
		public virtual Currency FromCurrency {get; set; }
		public virtual Currency ToCurrency { get; set; }
	}
}
