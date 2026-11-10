#nullable enable

using MudSharp.Character;

namespace MudSharp.Effects.Interfaces;

/// <summary>An optional item effect invoked by an actual pickup, never by a feasibility query or scripted placement.</summary>
public interface IItemPickupAttemptEffect : IEffectSubtype
{
	/// <returns>True to continue the pickup; false to abort it.</returns>
	bool HandlePickupAttempt(ICharacter actor);
}
