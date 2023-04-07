using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Movement {
    public enum MovementPhase {
        OriginalRoom,
        NewRoom
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
    }
}