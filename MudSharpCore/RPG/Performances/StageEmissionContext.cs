#nullable enable

using System.Threading;

namespace MudSharp.RPG.Performances;

/// <summary>Tracks listeners who received the stage rendition of one instrument emission.</summary>
internal sealed class StageEmissionContext : IDisposable
{
	private static readonly AsyncLocal<StageEmissionContext?> Current = new();
	private readonly HashSet<ICharacter> _audience = new(ReferenceEqualityComparer.Instance);
	private readonly StageEmissionContext? _previous = Current.Value;

	private StageEmissionContext() => Current.Value = this;

	public static StageEmissionContext BeginEmission() => new();
	public static void StageDelivered(ICharacter listener) => Current.Value?._audience.Add(listener);
	public static bool AlreadyHeard(ICharacter listener) => Current.Value?._audience.Contains(listener) == true;
	public void Dispose() => Current.Value = _previous;
}
