using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;

#nullable enable annotations

namespace MudSharp.Effects.Interfaces;

public interface IZeroGravityTetherEffect : IEffectSubtype
{
	IPerceivable Anchor { get; }
	IGameItem? PhysicalTether { get; }
	int MaximumRooms { get; }
	bool BlocksMovementTo(ICell destination);
}
