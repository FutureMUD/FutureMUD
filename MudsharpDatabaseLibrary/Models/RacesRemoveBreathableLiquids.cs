namespace MudSharp.Models;

public partial class RacesRemoveBreathableLiquids
{
	public long RaceId { get; set; }
	public long LiquidId { get; set; }

	public virtual Liquid Liquid { get; set; }
	public virtual Race Race { get; set; }
}