using System.Threading;
using MudSharp.Body.Traits;

#nullable enable
namespace MudSharp.RPG.Checks;

/// <summary>Suppress native branching/improvement only while this actor's configured cast check is evaluated.</summary>
internal sealed class CheckImprovementScope : IDisposable
{
	private static readonly AsyncLocal<CheckImprovementScope?> Current = new();
	private readonly CheckImprovementScope? _previous;
	private readonly ICharacter _actor;
	public CheckImprovementScope(ICharacter actor) { _actor = actor; _previous = Current.Value; Current.Value = this; }
	public static bool Suppresses(IPerceivableHaveTraits actor, CheckType type) =>
		type == CheckType.CastSpellCheck && ReferenceEquals(Current.Value?._actor, actor);
	public void Dispose() => Current.Value = _previous;
}
