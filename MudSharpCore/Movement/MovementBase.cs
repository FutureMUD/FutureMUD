using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Health;

namespace MudSharp.Movement;

public abstract class MovementBase : IMovement
{
	protected MovementBase(ICellExit exit, TimeSpan duration)
	{
		Exit = exit;
		Duration = duration;
		Phase = MovementPhase.OriginalRoom;
		Cancelled = false;
	}

	public virtual double StaminaMultiplier => 1.0;
	public virtual bool IgnoreTerrainStamina => false;
	public TimeSpan Duration { get; protected init; }

	public bool Cancelled { get; protected set; }
	public MovementPhase Phase { get; protected set; }
	public bool CanBeVoluntarilyCancelled { get; set; } = true;

	public abstract IEnumerable<ICharacter> CharacterMovers { get; }

	public ICellExit Exit { get; protected set; }

	#region IMovement Members

	public abstract bool Cancel();

	public virtual bool CancelForMoverOnly(IMove mover)
	{
		return Cancel();
	}

	public abstract bool IsMovementLeader(ICharacter character);

	public abstract void StopMovement();

	public virtual bool IsConsensualMover(ICharacter character)
	{
		return true;
	}

	public abstract string Describe(IPerceiver voyeur);
	public abstract bool SeenBy(IPerceiver voyeur, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);
	public abstract void InitialAction();

	protected void TurnaroundTrack(ICharacter mover)
	{
		var track = DepartureTracks[mover];
		if (track is null)
		{
			return;
		}

		track.TurnedAround = true;
		track.Changed = true;
	}

	protected void TurnaroundTracks()
	{
		foreach (var mover in CharacterMovers)
		{
			TurnaroundTrack(mover);
		}
	}

	protected DictionaryWithDefault<ICharacter, ITrack> DepartureTracks { get; } = new();

	protected static TrackCircumstances ApplyTrackCircumstances(ICharacter actor, TrackCircumstances circumstance)
	{
		if (actor.Body.Wounds.Any(x => x.BleedStatus == BleedStatus.Bleeding))
		{
			circumstance |= TrackCircumstances.Bleeding;
		}

		if (actor.Combat is not null && actor.CombatStrategyMode == CombatStrategyMode.Flee)
		{
			circumstance |= TrackCircumstances.Fleeing;
		}

		return circumstance;
	}

	protected void CreateDepartureTracks(ICharacter actor, TrackCircumstances circumstance)
	{
		if (!actor.Gameworld.GetStaticBool("TrackingEnabled"))
		{
			return;
		}
		var location = actor.Location;
		if (!location.Terrain(actor).CanHaveTracks)
		{
			return;
		}

		if (actor.IsAdministrator())
		{
			return;
		}

		circumstance = ApplyTrackCircumstances(actor, circumstance);
		if (GetTrackIntensities(actor, location, out var visual, out var olfactory))
		{
			return;
		}

		var track = new Movement.Track(actor.Gameworld, actor, Exit, circumstance, true, visual, olfactory);
		actor.Gameworld.Add(track);
		DepartureTracks[actor] = track;
		location.AddTrack(track);
	}

	private static bool GetTrackIntensities(ICharacter actor, ICell location, out double visual, out double olfactory)
	{
		visual = 1.0 * location.Terrain(actor).TrackIntensityMultiplierVisual * actor.Race.TrackIntensityVisual; ;
		olfactory = 1.0 * location.Terrain(actor).TrackIntensityMultiplierOlfactory * actor.Race.TrackIntensityOlfactory; ;
		if (location.IsSwimmingLayer(actor.RoomLayer) || actor.RoomLayer.In(RoomLayer.InAir, RoomLayer.HighInAir))
		{
			visual = 0.0;
		}

		if (visual <= 0.0 && olfactory <= 0.0)
		{
			return true;
		}

		return false;
	}

	protected void CreateArrivalTracks(ICharacter actor, TrackCircumstances circumstance)
	{
		if (!actor.Gameworld.GetStaticBool("TrackingEnabled"))
		{
			return;
		}
		var location = actor.Location;
		if (!location.Terrain(actor).CanHaveTracks)
		{
			return;
		}

		if (actor.IsAdministrator())
		{
			return;
		}

		if (GetTrackIntensities(actor, location, out var visual, out var olfactory))
		{
			return;
		}

		circumstance = ApplyTrackCircumstances(actor, circumstance);
		var track = new Movement.Track(actor.Gameworld, actor, Exit.Opposite, circumstance, false, visual, olfactory);
		actor.Gameworld.Add(track);
		location.AddTrack(track);
	}

	#endregion
}