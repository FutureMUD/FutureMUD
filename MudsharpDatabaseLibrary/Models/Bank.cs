using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class Bank
	{
		public Bank()
		{
			BankAccounts = new HashSet<BankAccount>();
			BankAccountTypes = new HashSet<BankAccountType>();
			BankCurrencyReserves = new HashSet<BankCurrencyReserve>();
			BankExchangeRates = new HashSet<BankExchangeRate>();
			BankBranches = new HashSet<BankBranch>();
			BankManagers = new HashSet<BankManager>();
			BankManagerAuditLogs = new HashSet<BankManagerAuditLog>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public long EconomicZoneId { get; set; }
		public long PrimaryCurrencyId { get; set; }
		public int MaximumBankAccountsPerCustomer { get; set; }

		public virtual EconomicZone EconomicZone { get; set; }
		public virtual Currency PrimaryCurrency { get; set; }

		public virtual ICollection<BankAccount> BankAccounts { get; set; }
		public virtual ICollection<BankAccountType> BankAccountTypes { get; set; }
		public virtual ICollection<BankCurrencyReserve> BankCurrencyReserves { get; set; }
		public virtual ICollection<BankExchangeRate> BankExchangeRates { get; set; }
		public virtual ICollection<BankBranch> BankBranches { get; set; }
		public virtual ICollection<BankManager> BankManagers { get; set; }
		public virtual ICollection<BankManagerAuditLog> BankManagerAuditLogs { get; set; }
	}
}
