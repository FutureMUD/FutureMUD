#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat;

public enum MountedCombatDomain
{
	Ground = 0,
	Aerial = 1,
	Aquatic = 2,
	GroundVehicle = 3,
	AquaticVehicle = 4
}

public sealed record MountedCombatContext(
	ICharacter Operator,
	IPerceivable Conveyance,
	MountedCombatDomain Domain,
	SizeCategory EffectiveSize,
	double Momentum,
	ICharacter? Mount = null,
	IVehicle? Vehicle = null);

public interface IMountedCombatService
{
	MountedCombatContext? ResolveContext(ICharacter combatant);
	BuiltInCombatMoveType ChargeMessageType(MountedCombatContext context);
	CheckType ChargeCheckType(MountedCombatContext context);
	void ResolveMountSprawl(ICharacter mount, int knockdownSuccessDegrees);
}
