#nullable enable

using MudSharp.Construction;
using MudSharp.Events;

namespace MudSharp.Effects.Concrete;

/// <summary>
/// Defines the movement-event boundary for traps anchored in a cell. Trap effects must resolve when a mover
/// enters the cell, rather than waiting for the optional completion witness that is not raised for every move.
/// </summary>
internal static class TrapEventRouting
{
	internal static bool IsCellArrivalWitness(EventType eventType)
	{
		return eventType == EventType.CharacterEnterCellWitness;
	}

}
