using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
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

		/// <summary>
		///     Cancels the movement. Does not handle any of the echoes
		/// </summary>
		/// <returns></returns>
		bool Cancel();

		bool CancelForMoverOnly(IMove mover);

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
		bool IgnoreTerrainStamina { get; }
		MovementType MovementType { get; }
	}
}