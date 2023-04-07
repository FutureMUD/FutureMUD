using System;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class DelayedAction : Effect, IActionEffect
{
	public DelayedAction(IPerceivable owner, Action<IPerceivable> action, string actionDescription)
		: base(owner)
	{
		Action = action;
		ActionDescription = actionDescription;
	}

	protected override string SpecificEffectType => "DelayedAction";

	public string ActionDescription { get; set; }
	public Action<IPerceivable> Action { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Delayed Action - {ActionDescription}";
	}

	public override void ExpireEffect()
	{
		Owner.RemoveEffect(this);
		if ((bool?)ApplicabilityProg?.Execute(Owner, null, null) ?? true)
		{
			Action(Owner);
		}
	}

	public override string ToString()
	{
		return $"DelayedAction Effect ({ActionDescription})";
	}
}