using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Combat;
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
		var location = actor.Location;
		if (!location.Terrain(actor).CanHaveTracks)
		{
			return;
		}

		circumstance = ApplyTrackCircumstances(actor, circumstance);

		var track = new Movement.Track(actor.Gameworld, actor, Exit, circumstance, true);
		actor.Gameworld.Add(track);
		DepartureTracks[actor] = track;
		location.AddTrack(track);
	}

	protected void CreateArrivalTracks(ICharacter actor, TrackCircumstances circumstance)
	{
		var location = actor.Location;
		if (!location.Terrain(actor).CanHaveTracks)
		{
			return;
		}
		circumstance = ApplyTrackCircumstances(actor, circumstance);
		var track = new Movement.Track(actor.Gameworld, actor, Exit.Opposite, circumstance, false);
		actor.Gameworld.Add(track);
		location.AddTrack(track);
	}

	#endregion
}