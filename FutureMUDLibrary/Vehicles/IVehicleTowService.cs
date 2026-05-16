#nullable enable

using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.GameItems;
using System.Collections.Generic;

namespace MudSharp.Vehicles;

public interface IVehicleTowService
{
	IReadOnlyList<IVehicle> TowTrainFrom(IVehicle root);
	IReadOnlyList<IVehicleTowLink> TowLinksFrom(IVehicle root);
	double TowTrainWeight(IVehicle root);
	bool CanHitch(ICharacter actor, IVehicle sourceVehicle, IVehicleTowPointPrototype sourceTowPoint,
		IVehicle targetVehicle, IVehicleTowPointPrototype targetTowPoint, IGameItem? hitchItem, out string reason);
	bool ValidateLink(IVehicleTowLink link, out string reason);
	bool CanMoveTowTrain(IVehicle root, ICellExit exit, out IReadOnlyList<IVehicle> towTrain, out string reason);
	bool IsTowPointInUse(IVehicle vehicle, IVehicleTowPointPrototype towPoint, IVehicleTowLink? exceptLink = null);
}
