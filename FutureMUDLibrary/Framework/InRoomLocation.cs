using MudSharp.Construction;

namespace MudSharp.Framework;

public record InRoomLocation
{
    public IRoom Location { get; init; }
    public RoomLayer RoomLayer { get; init; }
}