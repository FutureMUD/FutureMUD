#nullable enable

using System;
using MudSharp.Construction.Boundary;
using MudSharp.GameItems;

namespace MudSharp.Effects.Interfaces;

/// <summary>Optional native item contact for an admitted exit traversal. Eligibility queries are pure.</summary>
public interface IExitBarrierItemTraversalEffect : IExitBarrierTraversalEffect
{
	bool OccupiesLayerAndPlane(IGameItem item);
	void Contact(IGameItem item, IRoomExit exit, Func<bool> continuation);
}
