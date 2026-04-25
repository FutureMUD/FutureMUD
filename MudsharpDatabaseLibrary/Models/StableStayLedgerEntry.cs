namespace MudSharp.Models;

public class StableStayLedgerEntry
{
	public long Id { get; set; }
	public long StableStayId { get; set; }
	public int EntryType { get; set; }
	public string MudDateTime { get; set; }
	public long? ActorId { get; set; }
	public string ActorName { get; set; }
	public decimal Amount { get; set; }
	public string Note { get; set; }

	public virtual StableStay StableStay { get; set; }
	public virtual Character Actor { get; set; }
}
