#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.Vehicles;

/// <summary>
/// A revisioned operational itinerary. This is deliberately distinct from a linear RouteCell.
/// Approved revisions pin every traversed RouteCell topology version and every concrete step.
/// </summary>
public interface IVehicleRoute : IEditableRevisableItem, IProgVariable
{
	IReadOnlyList<IVehicleRouteStop> Stops { get; }
	IReadOnlyList<IVehicleRouteLeg> Legs { get; }
	IReadOnlyCollection<IVehicleRouteTopologyPin> TopologyPins { get; }
}

public interface IVehicleRouteStop : IFrameworkItem
{
	IVehicleRoute Route { get; }
	int Sequence { get; }
	SpatialLocation Location { get; }
	TimeSpan DwellDuration { get; }
	IReadOnlyCollection<IVehicleRoutePlatformBinding> PlatformBindings { get; }
}

public interface IVehicleRoutePlatformBinding : IFrameworkItem
{
	IVehicleRouteStop Stop { get; }
	ICell PlatformCell { get; }
	IVehicleAccessPointPrototype AccessPoint { get; }
	double DockingToleranceMetres { get; }
}

public interface IVehicleRouteTopologyPin
{
	ICell RouteCell { get; }
	long TopologyVersion { get; }
}

public interface IVehicleRouteLeg : IFrameworkItem
{
	IVehicleRoute Route { get; }
	int Sequence { get; }
	IVehicleRouteStop OriginStop { get; }
	IVehicleRouteStop DestinationStop { get; }
	IReadOnlyList<IVehicleRouteStep> Steps { get; }
	double RouteDistanceMetres { get; }
	double RoomEquivalentCost { get; }
}

public interface IVehicleRouteStep : IFrameworkItem, ISpatialPathStep
{
	IVehicleRouteLeg Leg { get; }
	int Sequence { get; }
	VehicleRouteStepType StepType { get; }
	long? OriginTopologyVersion { get; }
	long? DestinationTopologyVersion { get; }
}

public interface IVehicleRouteLinearStep : IVehicleRouteStep, ILinearRoutePathStep
{
	long PinnedTopologyVersion { get; }
}

public interface IVehicleRouteExitStep : IVehicleRouteStep, IExitTraversalPathStep
{
}

/// <summary>
/// The recurring in-world departure schedule for a vehicle service.
/// </summary>
public interface IVehicleServiceSchedule
{
	MudDateTime ReferenceDeparture { get; }
	RecurringInterval Recurrence { get; }
	MudDateTime NextDeparture { get; }
}

public interface IVehicleService : IFrameworkItem, IKeyworded, IHaveFuturemud, ISaveable, IProgVariable
{
	IVehicleRoute Route { get; }
	IVehicle Vehicle { get; }
	IVehicleServiceSchedule Schedule { get; }
	VehicleServiceOperatorMode OperatorMode { get; }
	TimeSpan RetryInterval { get; }
	TimeSpan MaximumHold { get; }
	bool Enabled { get; }
	IVehicleJourney? ActiveJourney { get; }
	bool IsReady { get; }
	string ReadinessReason { get; }
}

public delegate void VehicleJourneyStateChangedEvent(
	IVehicleJourney journey,
	VehicleJourneyState previousState,
	VehicleJourneyState currentState,
	IVehicleJourneyEvent journeyEvent);

public interface IVehicleJourney : IFrameworkItem, IHaveFuturemud, ISaveable, IProgVariable
{
	Guid OperationId { get; }
	IVehicleService Service { get; }
	IVehicleRoute Route { get; }
	IVehicle Vehicle { get; }
	VehicleJourneyState State { get; }
	IVehicleRouteStop? CurrentStop { get; }
	IVehicleRouteStop? NextStop { get; }
	MudDateTime ScheduledDeparture { get; }
	MudDateTime ExpectedDeparture { get; }
	TimeSpan Delay { get; }
	DateTimeOffset LastCheckpointUtc { get; }
	bool BoardingOpen { get; }
	string StatusReason { get; }
	IReadOnlyList<IVehicleJourneyEvent> Events { get; }
	event VehicleJourneyStateChangedEvent StateChanged;
}

/// <summary>
/// The result returned by the physical movement layer when a compiled route leg completes.
/// Operational journeys deliberately depend on this seam rather than on a concrete movement
/// implementation so longitudinal RouteCell and room-scale vehicle movement can evolve independently.
/// </summary>
public sealed record VehicleJourneyLegResult(bool Succeeded, string Reason)
{
	public static VehicleJourneyLegResult Success { get; } = new(true, string.Empty);
}

/// <summary>
/// Physical boarding and movement operations consumed by the journey state machine. Implementations
/// must invoke the supplied completion callback exactly once for every successfully-started leg.
/// </summary>
public interface IVehicleJourneyOperations
{
	bool TryOpenBoarding(IVehicleJourney journey, out string reason);
	bool TryCloseBoarding(IVehicleJourney journey, out string reason);
	bool TryBeginLeg(
		IVehicleJourney journey,
		IVehicleRouteLeg leg,
		Action<VehicleJourneyLegResult> completion,
		out string reason);
	void Cancel(IVehicleJourney journey);
}

/// <summary>
/// Narrow physical executor implemented by a vehicle route movement strategy. Boarding and
/// scheduling remain owned by the operational coordinator.
/// </summary>
public interface IVehicleRouteLegExecutor
{
	bool TryBeginLeg(
		IVehicleJourney journey,
		IVehicleRouteLeg leg,
		Action<VehicleJourneyLegResult> completion,
		out string reason);
	void Cancel(IVehicleJourney journey);
}

public interface IVehicleJourneyCoordinator
{
	IVehicleJourney CreateJourney(IVehicleService service, MudDateTime scheduledDeparture);
	void StartOrResume(IVehicleJourney journey);
	void Cancel(IVehicleJourney journey, string reason);
	void Resume(IVehicleJourney journey);
}

public interface IVehicleJourneyEvent : IFrameworkItem
{
	IVehicleJourney Journey { get; }
	long Sequence { get; }
	VehicleJourneyEventType EventType { get; }
	VehicleJourneyState State { get; }
	DateTimeOffset OccurredAtUtc { get; }
	MudDateTime? WorldTime { get; }
	string Message { get; }
}

public interface IVehicleDocking : IFrameworkItem
{
	IVehicle Vehicle { get; }
	IVehicleAccessPoint AccessPoint { get; }
	IVehicleCompartment Compartment { get; }
	ICell ExteriorCell { get; }
	RoomLayer ExteriorLayer { get; }
	IVehicleRouteStop? Stop { get; }
	VehicleDockingState State { get; }
	IExit TransientExit { get; }
}

public interface IVehicleDockingService
{
	IReadOnlyCollection<IVehicleDocking> ActiveDockings(IVehicle vehicle);
	bool CanDock(
		IVehicle vehicle,
		IVehicleAccessPoint accessPoint,
		ICell exteriorCell,
		RoomLayer exteriorLayer,
		IVehicleRouteStop? stop,
		out string reason);
	IVehicleDocking Dock(
		IVehicle vehicle,
		IVehicleAccessPoint accessPoint,
		ICell exteriorCell,
		RoomLayer exteriorLayer,
		IVehicleRouteStop? stop = null);
	void SetBoardingOpen(IVehicleDocking docking, bool open);
	void Undock(IVehicleDocking docking);
	void RebuildDockings(IVehicle vehicle);
}
