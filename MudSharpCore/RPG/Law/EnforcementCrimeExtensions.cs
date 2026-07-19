using MudSharp.Effects.Concrete;

#nullable enable

namespace MudSharp.RPG.Law;

public static class EnforcementCrimeExtensions
{
	public static bool IsLawfulEnforcementActionAgainst(this ICharacter actor, ICharacter victim, CrimeTypes crime)
	{
		if (actor is null ||
		    victim is null ||
		    CharacterInstanceIdentityComparer.SamePhysicalInstance(actor, victim))
		{
			return false;
		}

		foreach (var patrolEffect in actor.CombinedEffectsOfType<PatrolMemberEffect>() ??
		                             Enumerable.Empty<PatrolMemberEffect>())
		{
			var patrol = patrolEffect.Patrol;
			if (patrol is null || !patrol.PatrolMembers.ContainsPhysicalInstance(actor))
			{
				continue;
			}

			if (IsActiveEnforcementTarget(victim, patrol) &&
			    IsAuthorisedByEnforcementStrategy(crime, patrol.ActiveEnforcementCrime!.Law.EnforcementStrategy))
			{
				return true;
			}

			if (IsExecutionTarget(victim, patrol) && IsAuthorisedExecutionCrime(crime))
			{
				return true;
			}
		}

		return false;
	}

	private static bool IsActiveEnforcementTarget(ICharacter victim, IPatrol patrol)
	{
		return patrol.ActiveEnforcementTarget is not null &&
		       patrol.ActiveEnforcementCrime?.Law is not null &&
		       CharacterInstanceIdentityComparer.SamePhysicalInstance(victim, patrol.ActiveEnforcementTarget);
	}

	private static bool IsExecutionTarget(ICharacter victim, IPatrol patrol)
	{
		return victim.EffectsOfType<ExecutionPatrolNoQuit>(x => x.Patrol == patrol)?.Any() == true;
	}

	private static bool IsAuthorisedByEnforcementStrategy(CrimeTypes crime, EnforcementStrategy strategy)
	{
		switch (crime)
		{
			case CrimeTypes.Assault:
			case CrimeTypes.Battery:
			case CrimeTypes.GreviousBodilyHarm:
				return strategy.IsArrestable() || strategy.IsKillable();
			case CrimeTypes.AssaultWithADeadlyWeapon:
			case CrimeTypes.AttemptedMurder:
			case CrimeTypes.Manslaughter:
			case CrimeTypes.Murder:
			case CrimeTypes.Mayhem:
				return strategy.IsKillable();
			default:
				return false;
		}
	}

	private static bool IsAuthorisedExecutionCrime(CrimeTypes crime)
	{
		switch (crime)
		{
			case CrimeTypes.Assault:
			case CrimeTypes.AssaultWithADeadlyWeapon:
			case CrimeTypes.Battery:
			case CrimeTypes.AttemptedMurder:
			case CrimeTypes.Manslaughter:
			case CrimeTypes.Murder:
			case CrimeTypes.GreviousBodilyHarm:
			case CrimeTypes.Mayhem:
				return true;
			default:
				return false;
		}
	}
}
