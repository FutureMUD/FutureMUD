#nullable enable

using System;
using MudSharp.Character;
using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Interfaces;

/// <summary>Optional contact action for an exit barrier. Queries must never call Contact.</summary>
public interface IExitBarrierTraversalEffect : IExitBarrierEffect
{
	bool IsActive { get; }
	bool OccupiesLayerAndPlane(ICharacter actor);
	void Contact(ICharacter actor, IRoomExit exit, Func<bool> continuation);
}
