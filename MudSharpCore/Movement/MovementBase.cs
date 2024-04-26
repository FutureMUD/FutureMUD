using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

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

	#endregion
}