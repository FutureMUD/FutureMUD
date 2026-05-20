namespace MudSharp.Models;

public class VehicleAccessState
{
	public long Id { get; set; }
	public long VehicleId { get; set; }
	public long? CharacterId { get; set; }
	public string AccessTag { get; set; }
	public int AccessLevel { get; set; }

	public virtual Vehicle Vehicle { get; set; }
	public virtual Character Character { get; set; }
}
