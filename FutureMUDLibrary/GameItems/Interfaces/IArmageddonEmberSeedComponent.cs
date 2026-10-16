#nullable enable

using System;
using MudSharp.Magic;

namespace MudSharp.GameItems.Interfaces;

/// <summary>Typed, persisted spell-created Ember Seed binding exposed to the S4 invocation path.</summary>
public interface IArmageddonEmberSeedComponent : IGameItemComponent
{
	ArmageddonEmberSeedBinding? Binding { get; }
	bool Spent { get; }
	void Bind(MudSharp.Magic.SpellLifecycleOrigin origin, MudSharp.Character.ICharacter creator, double damagePerGrade);
}

public sealed record ArmageddonEmberSeedBinding(string CatalogueKey, Guid LifecycleId, long SpellId, SpellLifecycleMode LifecycleMode,
	int Grade, long CreatorId, long CreatorInstanceId, DateTime ExpiresAtUtc, double DamagePerGrade)
{
	public bool IsValid => CatalogueKey == "arm.item.ember_seed" && SpellId > 0 && LifecycleId != Guid.Empty &&
											LifecycleMode == SpellLifecycleMode.TemporaryCleanup && Grade is >= 1 and <= 7 &&
													CreatorId > 0 && CreatorInstanceId > 0 && ExpiresAtUtc.Kind == DateTimeKind.Utc &&
													double.IsFinite(DamagePerGrade) && DamagePerGrade is > 0.0 and <= 100.0;
	public double DamagePerEligibleTarget => IsValid ? DamagePerGrade * Grade : 0.0;
	public bool CanActivate(bool spent, DateTime nowUtc) => IsValid && !spent && nowUtc.Kind == DateTimeKind.Utc && nowUtc < ExpiresAtUtc;
}
