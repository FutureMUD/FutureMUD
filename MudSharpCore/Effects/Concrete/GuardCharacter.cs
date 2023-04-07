using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class GuardCharacter : Effect, IGuardCharacterEffect, IAffectProximity
{
	private readonly List<ICharacter> _targets;

	public GuardCharacter(ICharacter owner, params ICharacter[] targets) : base(owner)
	{
		_targets = targets.ToList();
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (_targets.Contains(thing))
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}

	public bool Interdicting { get; set; }

	protected override string SpecificEffectType => "Guard Character";
	public IEnumerable<ICharacter> Targets => _targets;

	public void AddTarget(ICharacter target)
	{
		if (!_targets.Contains(target))
		{
			_targets.Add(target);
			target.OnQuit += Target_OnQuit;
			target.OnDeath += Target_OnDeath;
			target.OnDeleted += Target_OnDeleted;
		}
	}

	public void RemoveTarget(ICharacter target)
	{
		ReleaseEvents(target);
	}

	public override void RemovalEffect()
	{
		foreach (var target in Targets.ToList())
		{
			ReleaseEvents(target);
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Guarding {Targets.Select(x => x.HowSeen(voyeur)).ListToString()}{(Interdicting ? " and interposing" : "")}.";
	}

	private void ReleaseEvents(ICharacter target)
	{
		_targets.Remove(target);
		target.OnQuit -= Target_OnQuit;
		target.OnDeleted -= Target_OnDeleted;
		target.OnDeath -= Target_OnDeath;

		if (!_targets.Any())
		{
			Owner.RemoveEffect(this);
		}
	}

	private void Target_OnDeleted(IPerceivable owner)
	{
		ReleaseEvents((ICharacter)owner);
	}

	private void Target_OnDeath(IPerceivable owner)
	{
		ReleaseEvents((ICharacter)owner);
	}

	private void Target_OnQuit(IPerceivable owner)
	{
		ReleaseEvents((ICharacter)owner);
	}

	public bool ShouldRemove(IAffectedByChangeInGuarding newEffect)
	{
		if (newEffect == this)
		{
			return false;
		}

		return true;
	}
}