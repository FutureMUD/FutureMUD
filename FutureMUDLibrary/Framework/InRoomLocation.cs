using MudSharp.Construction;

namespace MudSharp.Framework;

public record InRoomLocation
{
	public ICell Location { get; init; }
	public RoomLayer RoomLayer { get; init; }
}