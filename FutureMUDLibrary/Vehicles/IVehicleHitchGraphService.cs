#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Vehicles;

public enum VehicleHitchGraphNodeType
{
	Vehicle,
	Character
}

public enum VehicleHitchGraphLinkKind
{
	LegacyVehicleTow,
	PersistentHitch,
	TransientCharacterHitch,
	TransientDrag
}

public sealed record VehicleHitchGraphEndpoint(
	VehicleHitchGraphNodeType NodeType,
	IVehicle? Vehicle,
	ICharacter? Character,
	IVehicleTowPointPrototype? TowPoint)
{
	public IPerceivable? Perceivable => NodeType switch
	{
		VehicleHitchGraphNodeType.Vehicle => Vehicle?.ExteriorItem,
		VehicleHitchGraphNodeType.Character => Character,
		_ => null
	};
}

public sealed record VehicleHitchGraphLink(
	string Key,
	VehicleHitchGraphLinkKind Kind,
	VehicleHitchGraphEndpoint Source,
	VehicleHitchGraphEndpoint Target,
	IGameItem? HitchItem,
	long? HitchItemId,
	bool IsManuallyDisabled,
	bool IsDisabled,
	string InvalidReason,
	IFrameworkItem? WrappedLink);

public sealed record VehicleHitchGraphTrainMember(
	IVehicle Vehicle,
	int Depth,
	VehicleHitchGraphLink? IncomingLink);

public sealed record VehicleHitchGraphTowStress(
	VehicleHitchGraphLink Link,
	IVehicle? TargetVehicle,
	double EffectiveWeight,
	double Capacity,
	double StressRatio,
	bool IsWarning,
	bool CanFail,
	double FailureChance,
	string Reason);

public sealed record VehicleHitchGraphMovePlan(
	IVehicle RootVehicle,
	IReadOnlyList<VehicleHitchGraphTrainMember> Members,
	IReadOnlyList<VehicleHitchGraphLink> Links,
	IReadOnlyList<IGameItem> HitchItems,
	double TotalWeight)
{
	public IReadOnlyList<IVehicle> Vehicles { get; } = Members.Select(x => x.Vehicle).ToList();
}

public interface IVehicleHitchGraphService
{
	IReadOnlyList<VehicleHitchGraphLink> LinksFrom(IFuturemud? gameworld, IVehicle source);
	IReadOnlyList<VehicleHitchGraphLink> LinksInvolving(IFuturemud? gameworld, IPerceivable perceivable);
	IReadOnlyList<IVehicle> VehicleTrainFrom(IFuturemud? gameworld, IVehicle root);
	IReadOnlyList<VehicleHitchGraphLink> VehicleTrainLinksFrom(IFuturemud? gameworld, IVehicle root);
	bool TryBuildVehicleTrain(IFuturemud? gameworld, IVehicle root, out VehicleHitchGraphMovePlan movePlan,
		out string reason, bool allowRootIncoming = false);
	double VehicleTrainWeight(IFuturemud? gameworld, IVehicle root);
	bool ValidateLink(VehicleHitchGraphLink link, out string reason);
	bool CanAddVehicleTowLink(ICharacter actor, IVehicle sourceVehicle, IVehicleTowPointPrototype sourceTowPoint,
		IVehicle targetVehicle, IVehicleTowPointPrototype targetTowPoint, IGameItem? hitchItem, out string reason);
	bool CanAddCharacterVehicleHitch(ICharacter actor, ICharacter source, IVehicle targetVehicle,
		IVehicleTowPointPrototype targetTowPoint, IGameItem? hitchItem, IDragAid? dragAid, out string reason);
	bool CanMoveVehicleTrain(IFuturemud? gameworld, IVehicle root, ICellExit exit,
		out VehicleHitchGraphMovePlan movePlan, out string reason);
	bool CanDragVehicleTrain(IFuturemud? gameworld, IVehicle root, ICellExit exit,
		IEnumerable<ICharacter> allowedPullers, out VehicleHitchGraphMovePlan movePlan, out string reason);
	bool IsTowPointInUse(IFuturemud? gameworld, IVehicle vehicle, IVehicleTowPointPrototype towPoint,
		IVehicleTowLink? exceptLegacyLink = null);
	IReadOnlyList<VehicleHitchGraphTowStress> EvaluateTowStress(VehicleHitchGraphMovePlan movePlan,
		double warningRatio = 0.90, double failureStartRatio = 0.95, double maximumFailureChance = 0.25);
	void CompleteVehicleTrainMove(VehicleHitchGraphMovePlan movePlan, ICell destination, RoomLayer layer,
		ICellExit exit, IMovement? movement = null, IVehicle? alreadyMovedVehicle = null);
}
