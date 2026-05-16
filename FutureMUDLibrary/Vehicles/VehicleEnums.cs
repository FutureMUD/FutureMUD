namespace MudSharp.Vehicles;

public enum VehicleScale
{
	ItemScale = 0,
	RoomContainer = 1,
	RoomScale = 2
}

public enum VehicleLocationType
{
	Nowhere = 0,
	Cell = 1,
	CellExitTransit = 2,
	Route = 3,
	Coordinate2D = 4,
	Coordinate3D = 5
}

public enum VehicleOccupantSlotType
{
	Passenger = 0,
	Driver = 1,
	Crew = 2
}

public enum VehicleMovementProfileType
{
	CellExit = 0,
	Route = 1,
	Coordinate2D = 2,
	Coordinate3D = 3
}

public enum VehicleMovementStatus
{
	Stationary = 0,
	Moving = 1,
	Disabled = 2,
	Destroyed = 3
}

public enum VehicleAccessPointType
{
	Door = 0,
	Hatch = 1,
	Ramp = 2,
	Canopy = 3,
	ServicePanel = 4
}

public enum VehicleSystemStatus
{
	Functional = 0,
	Disabled = 1,
	Destroyed = 2
}

public enum VehicleDamageEffectTargetType
{
	WholeVehicleMovement = 0,
	MovementProfile = 1,
	AccessPoint = 2,
	CargoSpace = 3,
	InstallationPoint = 4,
	TowPoint = 5
}
