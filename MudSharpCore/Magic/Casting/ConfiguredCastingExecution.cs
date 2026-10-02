using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic.Casting;

internal sealed record CastingPayment(ICharacter Holder, IMagicResource Resource, double Amount);
internal sealed record ConfiguredAreaApplication(ICharacter Target, double DamageMultiplier, Func<bool> StillEligible);

/// <summary>Explicit invocation-local engine state; never attached to the catalogue spell.</summary>
internal sealed class ConfiguredCastingExecution(IReadOnlyList<CastingPayment> payments)
{
	public IReadOnlyList<CastingPayment> Payments { get; } = payments;
	public Action? BeforeMaterials { get; init; }
	public Action? AfterCommit { get; init; }
	public CheckOutcome? CheckResult { get; set; }
	public bool AppliedIntendedOperation { get; set; }
	public IReadOnlyList<ConfiguredAreaApplication>? AreaApplications { get; set; }
}
