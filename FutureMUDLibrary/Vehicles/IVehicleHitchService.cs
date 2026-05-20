#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using System.Collections.Generic;

namespace MudSharp.Vehicles;

public interface IVehicleHitchService
{
	IReadOnlyList<IVehicleHitchLink> LinksFrom(IFuturemud gameworld, IPerceivable source);
	IReadOnlyList<IVehicleHitchLink> LinksInvolving(IFuturemud gameworld, IPerceivable perceivable);
	bool CanPersistCharacterHitch(ICharacter source, IPerceivable target, out string reason);
	IVehicleHitchLink? CreatePersistentCharacterHitch(ICharacter actor, ICharacter source, IPerceivable target,
		IVehicle? targetVehicle, IVehicleTowPointPrototype? targetTowPoint, IGameItem? hitchItem, out string reason);
	void DeletePersistentLink(IFuturemud gameworld, long linkId);
	void RecoverPersistentLinks(IFuturemud gameworld);
}
