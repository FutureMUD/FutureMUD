#nullable enable

namespace MudSharp.Models;

public partial class RouteRoomLandmark
{
	public long Id { get; set; }
	public long RouteRoomId { get; set; }
	public string Name { get; set; } = null!;
	public string Keywords { get; set; } = null!;
	public string Description { get; set; } = null!;
	public decimal PositionMetres { get; set; }
	public int DisplayOrder { get; set; }

	public virtual RouteRoom RouteRoom { get; set; } = null!;
}
