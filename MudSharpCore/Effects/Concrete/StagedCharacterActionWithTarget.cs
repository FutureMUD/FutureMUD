using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class StagedCharacterActionWithTarget : CharacterActionWithTarget
{
	public StagedCharacterActionWithTarget(ICharacter owner, IPerceivable target, string actionDescription,
		string cancelEmote, string cannotMoveEmote, IEnumerable<string> blocks, string ldescAddendum,
		IEnumerable<Action<IPerceivable>> actions,
		int fireOnCount, IEnumerable<TimeSpan> timesBetweenTicks, Action<IPerceivable> onStopAction = null)
		: base(owner, target, null, actionDescription, cancelEmote, cannotMoveEmote, blocks, ldescAddendum,
			onStopAction)
	{
		CharacterOwner = owner;
		ActionDescription = actionDescription;
		OnStopAction = onStopAction;
		CancelEmoteString = cancelEmote;
		WhyCannotMoveEmoteString = cannotMoveEmote;
		ActionQueue = new Queue<Action<IPerceivable>>(actions);
		FireOnCount = fireOnCount;
		TimesBetweenTicks = new Queue<TimeSpan>(timesBetweenTicks);
	}

	public StagedCharacterActionWithTarget(ICharacter owner, IPerceivable target, string actionDescription,
		string cancelEmote, string cannotMoveEmote, IEnumerable<string> blocks, string ldescAddendum,
		IEnumerable<Action<IPerceivable>> actions,
		int fireOnCount, TimeSpan timeBetweenTicks, Action<IPerceivable> onStopAction = null)
		: this(
			owner, target, actionDescription, cancelEmote, cannotMoveEmote, blocks, ldescAddendum, actions, fireOnCount,
			Enumerable.Repeat(timeBetweenTicks, actions.Count() - 1), onStopAction)
	{
	}

	protected StagedCharacterActionWithTarget(ICharacter owner, IPerceivable target) : base(owner, target)
	{
	}

	public int CurrentCount { get; set; }
	public int FireOnCount { get; init; }
	public Queue<TimeSpan> TimesBetweenTicks { get; init; }
	public Queue<Action<IPerceivable>> ActionQueue { get; init; }
	public new Action<IPerceivable> Action => ActionQueue?.Dequeue();

	protected override string SpecificEffectType => "StagedCharacterActionWithTarget";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Character Action with Multiple Stages for {Target.HowSeen(voyeur)} - {ActionDescription}";
	}

	public override void ExpireEffect()
	{
		if (ApplicabilityProg?.ExecuteBool(Owner, null, null) ?? true)
		{
			Action(Owner);
		}

		if (Owner.Effects.All(x => x != this))
		{
			ReleaseEventHandlers();
			return;
		}

		if (++CurrentCount == FireOnCount)
		{
			Owner.RemoveEffect(this, true);
		}
		else
		{
			Owner.Gameworld.EffectScheduler.AddSchedule(new EffectSchedule(this, TimesBetweenTicks.Dequeue()));
		}
	}
}