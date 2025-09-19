using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Combat;

/// <summary>
///     Represents the outcome of a ranged scatter calculation, including the cell that receives the scattered shot,
///     the intended room layer for any physical aftermath and an optional target that was struck.
/// </summary>
public sealed record RangedScatterResult(ICell Cell, RoomLayer RoomLayer, CardinalDirection DirectionFromTarget,
        int DistanceFromTarget, IPerceiver? Target);
