namespace MudSharp.Models;

public partial class ClanPayrollHistory
{
	public long Id { get; set; }
	public long ClanId { get; set; }
	public long CharacterId { get; set; }
	public long RankId { get; set; }
	public long? PaygradeId { get; set; }
	public long? AppointmentId { get; set; }
	public long? ActorId { get; set; }
	public long CurrencyId { get; set; }
	public decimal Amount { get; set; }
	public int EntryType { get; set; }
	public string DateTime { get; set; }
	public string Description { get; set; }

	public virtual Clan Clan { get; set; }
	public virtual Character Character { get; set; }
	public virtual Rank Rank { get; set; }
	public virtual Paygrade Paygrade { get; set; }
	public virtual Appointment Appointment { get; set; }
	public virtual Character Actor { get; set; }
	public virtual Currency Currency { get; set; }
}
