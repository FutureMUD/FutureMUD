#nullable enable

namespace MudSharp.Effects.Concrete;

/// <summary>Recovery for explicitly authored bonus manoeuvres, separate from the ordinary turn schedule.</summary>
public sealed class AdditionalCombatActionRecovery(ICharacter owner) : Effect(owner)
{
	protected override string SpecificEffectType => "AdditionalCombatActionRecovery";
	public override string Describe(IPerceiver voyeur) => "Recovering from an additional combat manoeuvre.";
}
