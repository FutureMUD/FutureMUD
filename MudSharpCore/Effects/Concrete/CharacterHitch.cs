using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

public class CharacterHitch : Effect
{
	public CharacterHitch(IPerceivable owner, IPerceivable target, double pullMultiplier, long? targetTowPointId = null) : base(owner)
	{
		Target = target;
		PullMultiplier = pullMultiplier;
		TargetTowPointId = targetTowPointId;
	}

	public IPerceivable Target { get; }
	public double PullMultiplier { get; }
	public long? TargetTowPointId { get; }

	protected override string SpecificEffectType => "CharacterHitch";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Hitched to pull {Target.HowSeen(voyeur)}.";
	}
}
