using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class MultiStageBlockingDelayedAction : Effect, IActionEffect
{
	protected readonly List<string> _blocks = new();

	public MultiStageBlockingDelayedAction(IPerceivable owner, IEnumerable<Action<IPerceivable>> actions,
		string actionDescription, string block, int fireOnCount, TimeSpan timeBetweenTicks)
		: base(owner)
	{
		ActionQueue = new Queue<Action<IPerceivable>>(actions);
		ActionDescription = actionDescription;
		_blocks.Add(block);
		FireOnCount = fireOnCount;
		CurrentCount = 0;
		TimesBetweenTicks = new Queue<TimeSpan>(Enumerable.Repeat(timeBetweenTicks, actions.Count() - 1));
	}

	public MultiStageBlockingDelayedAction(IPerceivable owner, IEnumerable<Action<IPerceivable>> actions,
		string actionDescription, string block, int fireOnCount, IEnumerable<TimeSpan> timesBetweenTicks)
		: base(owner)
	{
		ActionQueue = new Queue<Action<IPerceivable>>(actions);
		ActionDescription = actionDescription;
		FireOnCount = fireOnCount;
		_blocks.Add(block);
		TimesBetweenTicks = new Queue<TimeSpan>(timesBetweenTicks);
		CurrentCount = 0;
	}

	public MultiStageBlockingDelayedAction(IPerceivable owner, IEnumerable<Action<IPerceivable>> actions,
		string actionDescription, IEnumerable<string> blocks, int fireOnCount, TimeSpan timeBetweenTicks)
		: base(owner)
	{
		ActionQueue = new Queue<Action<IPerceivable>>(actions);
		ActionDescription = actionDescription;
		_blocks.AddRange(blocks);
		FireOnCount = fireOnCount;
		CurrentCount = 0;
		TimesBetweenTicks = new Queue<TimeSpan>(Enumerable.Repeat(timeBetweenTicks, actions.Count() - 1));
	}

	public int CurrentCount { get; set; }
	public int FireOnCount { get; set; }
	public Queue<TimeSpan> TimesBetweenTicks { get; set; }
	public Queue<Action<IPerceivable>> ActionQueue { get; set; }

	protected override string SpecificEffectType => "MultiStageBlockingDelayedAction";
	public string ActionDescription { get; set; }
	public Action<IPerceivable> Action => ActionQueue?.Dequeue();

	public override bool CanBeStoppedByPlayer => true;

	public override string Describe(IPerceiver voyeur)
	{
		return $"Multi-Stage Blocking Delayed Action - {ActionDescription}";
	}

	public override void ExpireEffect()
	{
		if ((bool?)ApplicabilityProg?.Execute(Owner, null, null) ?? true)
		{
			Action(Owner);
		}

		if (Owner.Effects.All(x => x != this))
		{
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

	public override IEnumerable<string> Blocks => _blocks;

	public override bool IsBlockingEffect(string blockingType)
	{
		return string.IsNullOrEmpty(blockingType) || _blocks.Contains(blockingType);
	}

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return ActionDescription;
	}

	public override string ToString()
	{
		return $"MultiStageBlockingDelayedAction Effect ({ActionDescription})";
	}
}