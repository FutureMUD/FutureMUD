using System.Collections.Generic;

namespace MudSharp.Models;

public class Stable
{
	public Stable()
	{
		Stays = new HashSet<StableStay>();
		StableAccounts = new HashSet<StableAccount>();
	}

	public long Id { get; set; }
	public string Name { get; set; }
	public long EconomicZoneId { get; set; }
	public long CellId { get; set; }
	public long? BankAccountId { get; set; }
	public bool IsTrading { get; set; }
	public decimal LodgeFee { get; set; }
	public decimal DailyFee { get; set; }
	public long? LodgeFeeProgId { get; set; }
	public long? DailyFeeProgId { get; set; }
	public long? CanStableProgId { get; set; }
	public long? WhyCannotStableProgId { get; set; }
	public string EmployeeRecords { get; set; }

	public virtual EconomicZone EconomicZone { get; set; }
	public virtual Cell Cell { get; set; }
	public virtual BankAccount BankAccount { get; set; }
	public virtual FutureProg LodgeFeeProg { get; set; }
	public virtual FutureProg DailyFeeProg { get; set; }
	public virtual FutureProg CanStableProg { get; set; }
	public virtual FutureProg WhyCannotStableProg { get; set; }
	public virtual ICollection<StableStay> Stays { get; set; }
	public virtual ICollection<StableAccount> StableAccounts { get; set; }
}
