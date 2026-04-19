#nullable enable

using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Events;

internal static class MovementEventUtilities
{
	public static bool ShouldSuppressMovementEvents(ICharacter mover)
	{
		return mover.CombinedEffectsOfType<IImmwalkEffect>().Any() ||
		       mover.RidingMount?.CombinedEffectsOfType<IImmwalkEffect>().Any() == true;
	}
}
