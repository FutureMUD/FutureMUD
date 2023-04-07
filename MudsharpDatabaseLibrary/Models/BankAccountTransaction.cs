using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class BankAccountTransaction
	{
		public long Id { get; set; }
		public long BankAccountId { get; set; }
		public int TransactionType { get; set; }
		public decimal Amount { get; set; }
		public string TransactionTime { get; set; }
		public string TransactionDescription { get; set; }
		public decimal AccountBalanceAfter { get; set; }

		public virtual BankAccount BankAccount { get; set; }
	}
}
