#nullable enable

using System;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Vehicles;

public interface IVehicleHitchLink : IFrameworkItem, IHaveFuturemud
{
	VehicleHitchEndpointType SourceType { get; }
	long? SourceVehicleId { get; }
	long? SourceCharacterId { get; }
	long? SourceCharacterInstanceId { get; }
	long? SourceTowPointPrototypeId { get; }
	VehicleHitchEndpointType TargetType { get; }
	long? TargetVehicleId { get; }
	long? TargetCharacterId { get; }
	long? TargetCharacterInstanceId { get; }
	long? TargetTowPointPrototypeId { get; }
	long? HitchItemId { get; }
	bool IsManuallyDisabled { get; }
	bool IsDisabled { get; }
	DateTime CreatedDateTime { get; }
	IVehicle? SourceVehicle { get; }
	ICharacter? SourceCharacter { get; }
	IVehicleTowPointPrototype? SourceTowPoint { get; }
	IVehicle? TargetVehicle { get; }
	ICharacter? TargetCharacter { get; }
	IVehicleTowPointPrototype? TargetTowPoint { get; }
	IGameItem? HitchItem { get; }
	bool IsBroken { get; }
	string WhyInvalid { get; }
}
