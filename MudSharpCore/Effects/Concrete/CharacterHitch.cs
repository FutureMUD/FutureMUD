using MudSharp.Framework;
using MudSharp.Effects.Interfaces;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

public class CharacterHitch : Effect, INoQuitEffect, INoTimeOutEffect
{
	public CharacterHitch(IPerceivable owner, IPerceivable target, double pullMultiplier, long? targetTowPointId = null,
		long? vehicleHitchLinkId = null) : base(owner)
	{
		Target = target;
		PullMultiplier = pullMultiplier;
		TargetTowPointId = targetTowPointId;
		VehicleHitchLinkId = vehicleHitchLinkId;
	}

	public IPerceivable Target { get; }
	public double PullMultiplier { get; }
	public long? TargetTowPointId { get; }
	public long? VehicleHitchLinkId { get; }

	public string NoQuitReason => $"You cannot quit while you are hitched to pull {Target.HowSeen(Owner as IPerceiver)}.";
	public string NoTimeOutReason => $"You cannot be timed out while you are hitched to pull {Target.HowSeen(Owner as IPerceiver)}.";

	protected override string SpecificEffectType => "CharacterHitch";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Hitched to pull {Target.HowSeen(voyeur)}.";
	}
}
