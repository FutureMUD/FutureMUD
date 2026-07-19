#nullable enable

using MudSharp.Construction.Boundary;
using MudSharp.GameItems;

namespace MudSharp.Vehicles;

public class VehicleTowService : IVehicleTowService
{
	private readonly IVehicleHitchGraphService _graphService;

	public VehicleTowService() : this(new VehicleHitchGraphService())
	{
	}

	public VehicleTowService(IVehicleHitchGraphService graphService)
	{
		_graphService = graphService;
	}

	public IReadOnlyList<IVehicle> TowTrainFrom(IVehicle root)
	{
		if (root is null)
		{
			return [];
		}

		return _graphService.VehicleTrainFrom(root.Gameworld, root);
	}

	public IReadOnlyList<IVehicleTowLink> TowLinksFrom(IVehicle root)
	{
		if (root is null)
		{
			return [];
		}

		return _graphService.VehicleTrainLinksFrom(root.Gameworld, root)
		                    .Where(x => x.WrappedLink is IVehicleTowLink)
		                    .Select(x => (IVehicleTowLink)x.WrappedLink!)
		                    .ToList();
	}

	public double TowTrainWeight(IVehicle root)
	{
		if (root is null)
		{
			return 0.0;
		}

		return _graphService.VehicleTrainWeight(root.Gameworld, root);
	}

	public bool CanHitch(ICharacter actor, IVehicle sourceVehicle, IVehicleTowPointPrototype sourceTowPoint,
		IVehicle targetVehicle, IVehicleTowPointPrototype targetTowPoint, IGameItem? hitchItem, out string reason)
	{
		return _graphService.CanAddVehicleTowLink(actor, sourceVehicle, sourceTowPoint, targetVehicle, targetTowPoint,
			hitchItem, out reason);
	}

	public bool ValidateLink(IVehicleTowLink link, out string reason)
	{
		if (link is null)
		{
			reason = "tow link is missing";
			return false;
		}

		return _graphService.ValidateLink(new VehicleHitchGraphLink(
			$"LegacyVehicleTow:{link.Id}:{link.SourceVehicleId}:{link.TargetVehicleId}:{link.SourceTowPointPrototypeId}:{link.TargetTowPointPrototypeId}",
			VehicleHitchGraphLinkKind.LegacyVehicleTow,
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, link.SourceVehicle, null, link.SourceTowPoint),
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, link.TargetVehicle, null, link.TargetTowPoint),
			link.HitchItem,
			link.HitchItemId,
			link.IsManuallyDisabled,
			link.IsDisabled,
			link.WhyInvalid,
			link), out reason);
	}

	public bool CanMoveTowTrain(IVehicle root, ICellExit exit, out IReadOnlyList<IVehicle> towTrain, out string reason)
	{
		if (root is null)
		{
			towTrain = [];
			reason = "There is no such vehicle.";
			return false;
		}

		if (_graphService.CanMoveVehicleTrain(root.Gameworld, root, exit, out var movePlan, out reason))
		{
			towTrain = movePlan.Vehicles;
			return true;
		}

		towTrain = [];
		return false;
	}

	public bool IsTowPointInUse(IVehicle vehicle, IVehicleTowPointPrototype towPoint, IVehicleTowLink? exceptLink = null)
	{
		if (vehicle is null)
		{
			return false;
		}

		return _graphService.IsTowPointInUse(vehicle.Gameworld, vehicle, towPoint, exceptLink);
	}
}
