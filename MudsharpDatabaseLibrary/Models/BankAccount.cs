using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class BankAccount
	{
		public BankAccount()
		{
			BankAccountTransactions = new HashSet<BankAccountTransaction>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public int AccountNumber { get; set; }
		public long BankId { get; set; }
		public long BankAccountTypeId { get; set; }
		public decimal CurrentBalance { get; set; }
		public long? AccountOwnerCharacterId { get; set; }
		public long? AccountOwnerClanId { get; set; }
		public long? AccountOwnerShopId { get; set; }
		public long? NominatedBenefactorAccountId { get; set; }
		public string AccountCreationDate { get; set; }
		public int AccountStatus { get; set; }
		public decimal CurrentMonthInterest { get; set; }
		public decimal CurrentMonthFees { get; set; }
		public string AuthorisedBankPaymentItems { get; set; }

		public virtual Bank Bank { get; set; }
		public virtual BankAccountType BankAccountType { get; set; }
		public virtual Character AccountOwnerCharacter { get; set; }
		public virtual Clan AccountOwnerClan { get; set; }
		public virtual Shop AccountOwnerShop { get; set; }
		public virtual BankAccount NominatedBenefactorAccount { get; set; }

		public virtual ICollection<BankAccountTransaction> BankAccountTransactions { get; set; }
	}
}
