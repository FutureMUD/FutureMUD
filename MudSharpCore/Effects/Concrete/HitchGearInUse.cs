using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Effects.Concrete;

public class HitchGearInUse : Effect, INoGetEffect
{
	public HitchGearInUse(IPerceivable owner, long? vehicleTowLinkId = null, long? vehicleHitchLinkId = null,
		long? sourceCharacterId = null, long? targetId = null) : base(owner)
	{
		VehicleTowLinkId = vehicleTowLinkId;
		VehicleHitchLinkId = vehicleHitchLinkId;
		SourceCharacterId = sourceCharacterId;
		TargetId = targetId;
	}

	public long? VehicleTowLinkId { get; }
	public long? VehicleHitchLinkId { get; }
	public long? SourceCharacterId { get; }
	public long? TargetId { get; }
	public bool CombatRelated => false;

	protected override string SpecificEffectType => "HitchGearInUse";

	public override string Describe(IPerceiver voyeur)
	{
		return "Being used as hitching gear.";
	}

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return "being used as hitching gear";
	}

	public bool Matches(long? vehicleTowLinkId = null, long? vehicleHitchLinkId = null, long? sourceCharacterId = null,
		long? targetId = null)
	{
		return vehicleTowLinkId is not null && VehicleTowLinkId == vehicleTowLinkId ||
		       vehicleHitchLinkId is not null && VehicleHitchLinkId == vehicleHitchLinkId ||
		       sourceCharacterId is not null && targetId is not null &&
		       SourceCharacterId == sourceCharacterId && TargetId == targetId;
	}

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		return oldItem == Owner
			? new HitchGearInUse(newItem, VehicleTowLinkId, VehicleHitchLinkId, SourceCharacterId, TargetId)
			: null!;
	}

	public override bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem)
	{
		return true;
	}
}
