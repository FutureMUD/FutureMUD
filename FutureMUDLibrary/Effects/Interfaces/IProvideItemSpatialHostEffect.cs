using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces;

/// <summary>
/// Gives an otherwise detached game item the effective spatial context of another perceivable.
/// </summary>
public interface IProvideItemSpatialHostEffect : IEffectSubtype
{
	IPerceivable SpatialHost { get; }
	RoomLayer SpatialLayer { get; }
	double? SpatialRoutePositionMetres { get; }
}
