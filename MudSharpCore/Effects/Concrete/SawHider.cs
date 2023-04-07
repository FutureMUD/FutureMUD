using System;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class SawHider : Effect, ISawHiderEffect
{
	public SawHider(IPerceivable owner, IPerceivable hider)
		: base(owner)
	{
		Hider = hider;
	}

	protected override string SpecificEffectType => "SawHider";
	public IPerceivable Hider { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Saw Hider Effect - {Hider.HowSeen(voyeur, colour: false)}";
	}

	public override bool Applies(object target)
	{
		return base.Applies(target) && Hider == target;
	}

	public override void ExpireEffect()
	{
		if (Hider != null && Hider.Location == Owner.Location)
		{
			Gameworld.EffectScheduler.AddSchedule(new EffectSchedule(this, TimeSpan.FromSeconds(60)));
		}
		else
		{
			Owner.RemoveEffect(this);
		}
	}

	public override string ToString()
	{
		return "Saw Hider Effect";
	}
}