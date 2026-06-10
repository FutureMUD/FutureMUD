using MudSharp.Framework;
using MudSharp.Effects.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.Vehicles;

namespace MudSharp.Effects.Concrete;

public class CharacterHitch : Effect, INoQuitEffect, INoTimeOutEffect
{
	public CharacterHitch(IPerceivable owner, IPerceivable target, double pullMultiplier, long? targetTowPointId = null,
		long? vehicleHitchLinkId = null, long? hitchItemId = null) : base(owner)
	{
		Target = target;
		PullMultiplier = pullMultiplier;
		TargetTowPointId = targetTowPointId;
		VehicleHitchLinkId = vehicleHitchLinkId;
		HitchItemId = hitchItemId;
	}

	public IPerceivable Target { get; }
	public double PullMultiplier { get; }
	public long? TargetTowPointId { get; }
	public long? VehicleHitchLinkId { get; }
	public long? HitchItemId { get; }

	public string NoQuitReason => $"You cannot quit while you are hitched to pull {Target.HowSeen(Owner as IPerceiver)}.";
	public string NoTimeOutReason => $"You cannot be timed out while you are hitched to pull {Target.HowSeen(Owner as IPerceiver)}.";

	protected override string SpecificEffectType => "CharacterHitch";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Hitched to pull {Target.HowSeen(voyeur)}.";
	}

	public override void RemovalEffect()
	{
		base.RemovalEffect();
		if (HitchItemId is null)
		{
			return;
		}

		var item = Gameworld.Items.Get(HitchItemId.Value);
		HitchGearRules.Release(item, vehicleHitchLinkId: VehicleHitchLinkId, sourceCharacterId: Owner.Id,
			targetId: Target.Id);
	}
}
