using System;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class SawHiddenItem : Effect, ISawHiddenItemEffect
{
	public SawHiddenItem(IPerceivable owner, IPerceivable item)
		: base(owner)
	{
		Item = item;
	}

	protected override string SpecificEffectType => "SawHiddenItem";
	public IPerceivable Item { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Saw Hidden Item Effect - {Item.HowSeen(voyeur, colour: false)}";
	}

	public override bool Applies(object target)
	{
		return base.Applies(target) && Item == target;
	}

	public override void ExpireEffect()
	{
		if (Item != null && Item.Location == Owner.Location)
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
		return "Saw Hidden Item Effect";
	}
}