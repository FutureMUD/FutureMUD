#nullable enable

using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Health;

namespace MudSharp.Magic;

/// <summary>
/// An invocation-local measurement capability carried by a native damage packet and its copies.
/// It is never persisted. Only the native health mutation boundary may complete a measurement.
/// Requested damage, item damage, pain and stun are not evidence of delivered vitality.
/// </summary>
public interface IMagicVitalTransferDamage
{
	IMagicVitalTransferMeasurement? BeginMeasurement(IBody body, IDamage damage);
}

/// <summary>A single native wound mutation, captured before wound-event callbacks.</summary>
public interface IMagicVitalTransferMeasurement
{
	void Complete(IEnumerable<IWound> wounds);
}
