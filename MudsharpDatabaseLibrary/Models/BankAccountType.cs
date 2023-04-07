using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class BankAccountType
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string CustomerDescription { get; set; }
		public decimal MaximumOverdrawAmount { get; set; }
		public decimal WithdrawalFleeFlat { get; set; }
		public decimal WithdrawalFleeRate { get; set; }
		public decimal DepositFeeFlat { get; set; }
		public decimal DepositFeeRate { get; set; }
		public decimal TransferFeeFlat { get; set; }
		public decimal TransferFeeRate { get; set; }
		public decimal TransferFeeOtherBankFlat { get; set; }
		public decimal TransferFeeOtherBankRate { get; set; }
		public decimal DailyFee { get; set; }
		public decimal DailyInterestRate { get; set; }
		public decimal OverdrawFeeFlat { get; set; }
		public decimal OverdrawFeeRate { get;set; }
		public decimal DailyOverdrawnFee { get; set; }
		public decimal DailyOverdrawnInterestRate { get; set; }

		public long BankId { get; set; }
		public long? CanOpenAccountProgCharacterId { get; set; }
		public long? CanOpenAccountProgClanId { get; set; }
		public long? CanOpenAccountProgShopId { get; set; }
		public long? CanCloseAccountProgId { get; set; }

		public int NumberOfPermittedPaymentItems { get; set; }
		public long? PaymentItemPrototypeId { get; set; }

		public virtual Bank Bank { get; set; }
		public virtual FutureProg CanOpenAccountProgCharacter { get; set; }
		public virtual FutureProg CanOpenAccountProgClan { get; set; }
		public virtual FutureProg CanOpenAccountProgShop { get; set; }
		public virtual FutureProg CanCloseAccountProg { get; set; }
	}
}
