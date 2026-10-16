#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Health.Wounds;

namespace MudSharp.Health.Strategies;

/// <summary>Exact cellular-wound pricing shared only by compatible organic health strategies.</summary>
internal static class OrganicDirectHealthCosts
{
	internal static bool TryPlan(IHealthStrategy strategy, IHaveWounds owner, IBodypart bodypart, double damage, double pain,
		double stun, WoundSeverity maximumSeverity, out DirectHealthCostPlan plan, out string error)
	{
		plan = null!;
		error = null!;

		if (owner is not ICharacter character || character.Body is null)
		{
			error = "Only an embodied character can pay this health cost.";
			return false;
		}

		if (bodypart is null || !character.Body.Bodyparts.Contains(bodypart))
		{
			error = "The selected body part is no longer available.";
			return false;
		}

		if (maximumSeverity == WoundSeverity.None)
		{
			error = "The configured maximum wound severity does not permit a health cost.";
			return false;
		}

		if (!AreValidCosts(damage, pain, stun) || damage + pain + stun <= 0.0)
		{
			error = "The configured health cost is invalid.";
			return false;
		}

		var requestedChannels = DirectHealthCostChannels.None;
		if (damage > 0.0)
		{
			requestedChannels |= DirectHealthCostChannels.Damage;
		}

		if (pain > 0.0)
		{
			requestedChannels |= DirectHealthCostChannels.Pain;
		}

		if (stun > 0.0)
		{
			requestedChannels |= DirectHealthCostChannels.Stun;
		}

		if ((strategy.SupportedDirectHealthCostChannels & requestedChannels) != requestedChannels)
		{
			error = "This health strategy cannot safely apply all configured health-cost channels.";
			return false;
		}

		// Current organic wounds expose pain after applicable analgesic effects. The direct cost
		// contract prices visible, native units, so do not silently undercharge through a modifier
		// whose inverse is not stable across a new or existing wound.
		if (pain > 0.0 && character.Body.EffectsOfType<IPainReductionEffect>().Any(x => x.Applies()))
		{
			error = "An active pain-reduction effect prevents this health cost from being priced safely.";
			return false;
		}

		if ((pain > 0.0 || stun > 0.0) && character.Body.EffectsOfType<IConsciousnessThresholdModifierEffect>()
				.Any(x => x.Applies()))
		{
			error = "An active consciousness-threshold effect prevents this health cost from being priced safely.";
			return false;
		}

		var bodypartWounds = owner.Wounds.Where(x => x.Bodypart == bodypart).ToList();
		if (bodypartWounds.Any(x => !AreFinite(x.CurrentDamage) || !AreFinite(x.CurrentPain) || !AreFinite(x.CurrentStun)))
		{
			error = "Existing wound values on the selected body part are invalid.";
			return false;
		}

		var existingCellular = bodypartWounds.FirstOrDefault(x => x.DamageType == DamageType.Cellular);
		if (existingCellular is not null && existingCellular is not SimpleOrganicWound)
		{
			error = "The selected existing cellular wound does not support a safe direct health cost.";
			return false;
		}

		var bodypartCapacity = character.Body.HitpointsForBodypart(bodypart);
		if (!IsPositiveFinite(bodypartCapacity))
		{
			error = "The selected body part has no valid health capacity.";
			return false;
		}

		var bodypartDamage = bodypartWounds.Sum(x => x.CurrentDamage);
		var bodypartPain = bodypartWounds.Sum(x => x.CurrentPain);
		var bodypartStun = bodypartWounds.Sum(x => x.CurrentStun);
		if (!FitsWithin(bodypartDamage, damage, bodypartCapacity) ||
			!FitsWithin(bodypartPain, pain, bodypartCapacity) ||
			!FitsWithin(bodypartStun, stun, bodypartCapacity))
		{
			error = "The selected body part cannot safely absorb that health cost.";
			return false;
		}

		if (!FitsWithin(owner.Wounds.Sum(x => x.CurrentDamage), damage, strategy.MaxHP(owner)) ||
			!FitsWithin(owner.Wounds.Sum(x => x.CurrentPain), pain, strategy.MaxPain(owner)) ||
			!FitsWithin(owner.Wounds.Sum(x => x.CurrentStun), stun, strategy.MaxStun(owner)))
		{
			error = "Your current condition cannot safely absorb that health cost.";
			return false;
		}

		if (strategy.GetSeverity((existingCellular?.CurrentDamage ?? 0.0) + damage) > maximumSeverity)
		{
			error = "The health cost would exceed the configured maximum wound severity.";
			return false;
		}

		var damageInput = damage;
		var painInput = pain;
		var stunInput = stun;
		if (existingCellular is null)
		{
			if ((damage > 0.0 && !IsPositiveFinite(bodypart.DamageModifier)) ||
				(pain > 0.0 && !IsPositiveFinite(bodypart.PainModifier)) ||
				(stun > 0.0 && !IsPositiveFinite(bodypart.StunModifier)))
			{
				error = "The selected body part has an invalid health-cost modifier.";
				return false;
			}

			if (damage > 0.0)
			{
				damageInput /= bodypart.DamageModifier;
			}

			if (pain > 0.0)
			{
				painInput /= bodypart.PainModifier;
			}

			if (stun > 0.0)
			{
				stunInput /= bodypart.StunModifier;
			}
			if (!AreValidCosts(damageInput, painInput, stunInput))
			{
				error = "The selected body part cannot price this health cost safely.";
				return false;
			}
		}

		plan = new DirectHealthCostPlan(bodypart, existingCellular, damageInput, painInput, stunInput, damage, pain,
			stun);
		return true;
	}

	internal static IReadOnlyList<IWound> Apply(IHealthStrategy strategy, IHaveWounds owner, DirectHealthCostPlan plan)
	{
		if (owner is not ICharacter character)
		{
			throw new InvalidOperationException("Direct health costs require a character owner.");
		}

		var damage = new Damage
		{
			ActorOrigin = character,
			Bodypart = plan.Bodypart,
			DamageAmount = plan.DamageInput,
			DamageType = DamageType.Cellular,
			PainAmount = plan.PainInput,
			StunAmount = plan.StunInput
		};

		if (plan.ExistingWound is not null)
		{
			if (plan.ExistingWound is not SimpleOrganicWound existing || !owner.Wounds.Contains(existing) ||
				existing.Bodypart != plan.Bodypart || existing.DamageType != DamageType.Cellular)
			{
				throw new InvalidOperationException("The planned cellular wound is no longer available.");
			}

			existing.SufferAdditionalDamage(damage);
			return [existing];
		}

		return strategy.SufferDamage(owner, damage, plan.Bodypart).ToList();
	}

	private static bool AreValidCosts(params double[] costs)
	{
		return costs.All(x => AreFinite(x) && x >= 0.0);
	}

	private static bool AreFinite(double value)
	{
		return !double.IsNaN(value) && !double.IsInfinity(value);
	}

	private static bool IsPositiveFinite(double value)
	{
		return AreFinite(value) && value > 0.0;
	}

	private static bool FitsWithin(double current, double addition, double capacity)
	{
		return AreFinite(current) && AreFinite(addition) && IsPositiveFinite(capacity) &&
			current + addition <= capacity + 0.000001;
	}

}
