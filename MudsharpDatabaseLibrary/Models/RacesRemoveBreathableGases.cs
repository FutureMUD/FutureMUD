namespace MudSharp.Models;

public partial class RacesRemoveBreathableGases
{
	public long RaceId { get; set; }
	public long GasId { get; set; }

	public virtual Gas Gas { get; set; }
	public virtual Race Race { get; set; }
}