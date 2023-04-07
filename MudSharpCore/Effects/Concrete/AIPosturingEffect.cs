using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class AIPosturingEffect : Effect, IEffectSubtype, IRemoveOnCombatStart, IRemoveOnMeleeCombat,
	IRemoveOnMovementEffect, IRemoveOnStateChange
{
	public double ThreatLevel { get; set; }
	public List<ICharacter> PosturingTargets { get; } = new();

	public Func<double, IEnumerable<ICharacter>, (double Threat, bool StillPosturing, TimeSpan PostureLength)>
		OnExpireFunction { get; }

	public AIPosturingEffect(ICharacter owner, IEnumerable<ICharacter> targets,
		Func<double, IEnumerable<ICharacter>, (double Threat, bool StillPosturing, TimeSpan PostureLength)> expireFunc)
		: base(owner)
	{
		PosturingTargets.AddRange(targets);
		OnExpireFunction = expireFunc;
	}

	protected override string SpecificEffectType => "AIPosturingEffect";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Posturing against {PosturingTargets.Select(x => x.HowSeen(voyeur)).ListToString()} at threat level {ThreatLevel.ToString("N2", voyeur).Colour(Telnet.BoldRed)}.";
	}

	public override void ExpireEffect()
	{
		var (threat, stillposturing, posturelength) = OnExpireFunction(ThreatLevel, PosturingTargets);
		if (stillposturing)
		{
			Owner.Reschedule(this, posturelength);
			ThreatLevel += threat;
		}
		else
		{
			base.ExpireEffect();
		}
	}

	public bool ShouldRemove(CharacterState newState)
	{
		return newState.HasFlag(CharacterState.Dead) || !CharacterState.Able.HasFlag(newState);
	}

	bool IRemoveOnMovementEffect.ShouldRemove()
	{
		return true;
	}
}