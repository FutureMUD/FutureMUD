using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Movement {
	public enum MovementPhase {
		OriginalRoom,
		NewRoom
	}


	[Flags]
	public enum MovementType
	{
		None = 0,
		Upright = 1 << 0,
		Crawling = 1 << 1,
		Prostrate = 1 << 2,
		Climbing = 1 << 3,
		Swimming = 1 << 4,
		Flying = 1 << 5,
		All = Upright | Crawling | Prostrate | Climbing | Swimming | Flying
	}

	public interface IMovement {
		bool Cancelled { get; }

		bool CanBeVoluntarilyCancelled { get; }
		ICellExit Exit { get; }
		MovementPhase Phase { get; }
		IEnumerable<ICharacter> CharacterMovers { get; }
		public IParty Party { get;}
		public IEnumerable<IDragging> DragEffects { get; }
		public IEnumerable<ICharacter> Draggers { get; } 
		public IEnumerable<ICharacter> Helpers { get; } 
		public IEnumerable<ICharacter> NonDraggers { get; } 
		public IEnumerable<ICharacter> NonConsensualMovers { get; } 
		public IEnumerable<ICharacter> Mounts { get; }
		public IEnumerable<IPerceivable> Targets { get; } 
		public IReadOnlyDictionary<ICharacter, ISneakMoveEffect> SneakMoveEffects { get; }
		public TimeSpan Duration { get; }

		/// <summary>
		///     Cancels the movement. Does not handle any of the echoes
		/// </summary>
		/// <returns></returns>
		bool Cancel();

		/// <summary>
		/// Cancels participation in this move for only the specified mover (and any dependent movers, like mounts).
		/// </summary>
		/// <param name="mover">The mover to cancel for</param>
		/// <param name="echo">Whether or not to echo cancellation to any dependent entities, not including the mover</param>
		/// <returns></returns>
		bool CancelForMoverOnly(IMove mover, bool echo = false);

		/// <summary>
		///     The leader/mover calls a stop to the movement voluntarily
		/// </summary>
		void StopMovement();

		bool IsMovementLeader(ICharacter character);

		bool IsConsensualMover(ICharacter character);

		string Describe(IPerceiver voyeur);
		bool SeenBy(IPerceiver voyeur, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);

		void InitialAction();
		double StaminaMultiplier { get; }
		MovementType MovementTypeForMover(ICharacter mover);
	}
}