#nullable enable

using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.GameItems;
using MudSharp.Planes;

namespace MudSharp.GameItems.Components;

/// <summary>
/// A deliberately narrow perceiver used by cameras and microphones. It is not an omniscient dummy: it can only
/// observe the owning item’s current cell, plane and visible layer, respects ordinary hiding effects, and applies
/// the camera's configured illumination threshold.
/// </summary>
internal sealed class MediaSensorPerceiver : DummyPerceiver
{
	private readonly IGameItem _owner;
	private readonly double _minimumIllumination;

	public MediaSensorPerceiver(IGameItem owner, ICell location, double minimumIllumination)
		: base("a media sensor", "it is a media sensor", location)
	{
		_owner = owner;
		_minimumIllumination = Math.Max(0.0, minimumIllumination);
		RoomLayer = owner.RoomLayer;
	}

	public override bool CanHear(IPerceivable thing)
	{
		return InSameObservableLocation(thing) && _owner.SharesPlaneWith(thing);
	}

	public override bool CanSee(IPerceivable thing, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (flags.HasFlag(PerceiveIgnoreFlags.IgnoreCanSee))
		{
			return true;
		}

		// Cells do not have a containing Location of their own. Cell descriptions ask the
		// perceiver whether it can see the cell itself before rendering the scene, so handle
		// the sensor's current cell explicitly while still respecting its light threshold.
		if (ReferenceEquals(thing, Location))
		{
			return flags.HasFlag(PerceiveIgnoreFlags.IgnoreDark) ||
			       Location.CurrentIllumination(this) >= _minimumIllumination;
		}

		if (!InSameObservableLocation(thing) || !_owner.SharesPlaneWith(thing) ||
		    !thing.RoomLayer.CanBeSeenFromLayer(RoomLayer))
		{
			return false;
		}

		if (!flags.HasFlag(PerceiveIgnoreFlags.IgnoreDark) &&
		    Location.CurrentIllumination(this) < _minimumIllumination)
		{
			return false;
		}

		return thing.HiddenFromPerception(this, PerceptionTypes.AllVisual, flags);
	}

	public override bool CanSense(IPerceivable thing, bool ignoreFuzzy = false)
	{
		return CanSee(thing) || CanHear(thing);
	}

	public override bool CanSmell(IPerceivable thing)
	{
		return false;
	}

	private bool InSameObservableLocation(IPerceivable thing)
	{
		if (thing is null)
		{
			return false;
		}

		if (ReferenceEquals(thing, _owner))
		{
			return true;
		}

		return ReferenceEquals(thing.Location, Location);
	}
}
