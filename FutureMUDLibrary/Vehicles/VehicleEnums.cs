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

public enum VehicleMovementEnvironment
{
	Unrestricted = 0,
	SurfaceWater = 1
}

public enum VehiclePropulsionType
{
	None = 0,
	SelfPowered = 1,
	Rowed = 2,
	Sail = 3,
	OutboardMotor = 4
}

public enum OutboardMotorEnergySource
{
	Fuelled = 0,
	Electric = 1
}

public enum VehicleRangedCoverDirection
{
	SameLevel = 0,
	Above = 1,
	Below = 2
}

public enum VehicleCombatDisplacementType
{
	Unbalance = 0,
	Knockdown = 1,
	Push = 2,
	Pull = 3,
	Throw = 4,
	AquaticVehicleAttack = 5
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

public enum VehicleHitchEndpointType
{
	Vehicle = 0,
	Character = 1
}

public enum RouteVehiclePropulsionMode
{
	Powered = 0,
	ExternallyPulled = 1
}

public enum VehicleRouteStepType
{
	LinearRoute = 0,
	CellExit = 1
}

public enum VehicleServiceOperatorMode
{
	Automatic = 0,
	Onboard = 1
}

public enum VehicleJourneyState
{
	Scheduled = 0,
	Boarding = 1,
	Held = 2,
	Departing = 3,
	EnRoute = 4,
	Dwelling = 5,
	Arrived = 6,
	Cancelled = 7,
	Faulted = 8
}

public enum VehicleJourneyEventType
{
	Scheduled = 0,
	BoardingOpened = 1,
	Held = 2,
	Departed = 3,
	Checkpointed = 4,
	StopArrived = 5,
	Dwelling = 6,
	BoardingClosed = 7,
	Completed = 8,
	DelayChanged = 9,
	Cancelled = 10,
	Faulted = 11
}

public enum VehicleDockingState
{
	DockedClosed = 0,
	BoardingOpen = 1
}
