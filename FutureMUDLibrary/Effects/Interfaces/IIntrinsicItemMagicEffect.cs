#nullable enable

namespace MudSharp.Effects.Interfaces;

/// <summary>Persistent item magic with an explicit, grade-gated cleansing policy independent of a timed spell parent.</summary>
public interface IIntrinsicItemMagicEffect : IEffectSubtype
{
	int DispelPriority { get; }
	int MinimumDispelGrade { get; }
	bool IsIntrinsicMagicActive => true;
	bool StopsDispelOnInsufficientGrade => false;
	bool TryDispel(int grade);
}
