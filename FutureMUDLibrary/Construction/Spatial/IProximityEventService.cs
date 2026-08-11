#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Construction;

/// <summary>
/// Identifies the world-state mutation that changed a proximity relationship.
/// </summary>
public enum ProximityChangeCause
{
	Movement,
	RoutePosition,
	Layer,
	Positioning,
	Party,
	Containment
}

/// <summary>
/// A receiver's opt-in registration for proximity-change events.
/// </summary>
public interface IProximityEventRegistration : IDisposable
{
	IPerceivable Receiver { get; }
	Proximity MaximumObservedProximity { get; }
}

/// <summary>
/// Captures the before state for one atomic spatial or relationship update and publishes its changes when completed.
/// </summary>
public interface IProximityChangeBatch : IDisposable
{
	void Track(IPerceivable perceivable);
	void TrackPair(IPerceivable receiver, IPerceivable counterpart);
	void TrackParty(IEnumerable<ICharacter> members);
	void Complete();
}

/// <summary>
/// Delivers directional, opt-in proximity-change events without broadcasting a movement update to every perceivable.
/// </summary>
public interface IProximityEventService
{
	IProximityEventRegistration Register(IPerceivable receiver,
		Proximity maximumObservedProximity = Proximity.VeryDistant);

	IProximityChangeBatch BeginChange(ProximityChangeCause cause, params IPerceivable[] affected);
}
