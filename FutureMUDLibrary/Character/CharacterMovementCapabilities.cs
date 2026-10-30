#nullable enable

using System.Linq;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Character;

public static class CharacterMovementCapabilities
{
	/// <summary>Whether race or an applicable effect permits ordinary climbing.</summary>
	public static bool HasClimbingAbility(this ICharacter character) => character.Race?.CanClimb == true ||
		character.CombinedEffectsOfType<IClimbingEffect>()?.Any(x => x.Applies()) == true;
}
