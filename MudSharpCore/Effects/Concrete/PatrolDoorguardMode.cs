using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class PatrolDoorguardMode : Effect, IDoorguardModeEffect
{
	public PatrolDoorguardMode(IPerceivable owner)
		: base(owner)
	{
	}

	protected override string SpecificEffectType => "PatrolDoorguardMode";

	public override string Describe(IPerceiver voyeur)
	{
		return "Patrol Door Guard Mode";
	}

	public override string ToString()
	{
		return "Patrol Door Guard Mode Effect";
	}
}
