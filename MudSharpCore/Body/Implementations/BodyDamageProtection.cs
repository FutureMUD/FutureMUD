#nullable enable

using MudSharp.Effects.Interfaces;
using MudSharp.Health;
using MudSharp.NPC.AI;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	/// <summary>One post-armour protection decision for an incoming packet and its inward damage.</summary>
	internal bool TryApplyDamageProtection(ref IDamage? damage, ref IDamage? inward)
	{
		if (damage is null || (damage.DamageAmount <= 0 && (inward?.DamageAmount ?? 0) <= 0)) return true;
		foreach (var protection in CombinedEffectsOfType<IDamageProtectionEffect>()
			.Distinct<IDamageProtectionEffect>(ReferenceEqualityComparer.Instance).ToArray())
		{
			if (!CommandExecutionScope.TryContinue() || !ReferenceEquals(Actor.Body, this)) return false;
			var applies = protection.Applies();
			if (!CommandExecutionScope.TryContinue() || !ReferenceEquals(Actor.Body, this)) return false;
			if (!applies || !Attached()) continue;
			var multiplier = protection.DamageMultiplier;
			if (!double.IsFinite(multiplier) || multiplier is < 0 or > 1)
				throw new InvalidOperationException("A damage-protection multiplier must be finite and within [0,1].");
			if (multiplier == 1) continue;
			damage.MutationGuard?.Validate(this, damage);
			inward?.MutationGuard?.Validate(this, inward);
			if (!CommandExecutionScope.TryContinue() || !ReferenceEquals(Actor.Body, this)) return false;
			if (!Attached()) continue;
			CommandExecutionScope.MarkCommitted();
			if (!protection.TryCommitProtection()) continue;
			if (!CommandExecutionScope.TryContinue() || !ReferenceEquals(Actor.Body, this)) return false;
			damage = multiplier == 0 ? null : new Damage(damage, multiplier);
			inward = inward is null || multiplier == 0 ? null : new Damage(inward, multiplier);
			if (damage is null) return true;
			bool Attached() => CombinedEffectsOfType<IDamageProtectionEffect>().Any(x => ReferenceEquals(x, protection));
		}
		return true;
	}
}
