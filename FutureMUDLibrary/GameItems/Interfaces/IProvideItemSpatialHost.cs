namespace MudSharp.GameItems.Interfaces;

/// <summary>
/// Identifies an item component whose parent is a logical projection of another item rather than an
/// independently placed world object. The parent inherits its effective cell, layer and RouteCell
/// coordinate from the spatial host.
/// </summary>
public interface IProvideItemSpatialHost : IGameItemComponent
{
	IGameItem SpatialHost { get; }
}
