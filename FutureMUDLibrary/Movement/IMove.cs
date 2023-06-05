using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MudSharp.Body.Position;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Movement {
    #nullable enable
    public class MoveEventArgs : EventArgs {
        public MoveEventArgs(IMove mover, IMovement movement) {
            Mover = mover;
            Movement = movement;
        }

        public IMove Mover { get; set; }
        public IMovement Movement { get; set; }
    }
    #nullable disable

    public interface IMove : IPerceiver {
        Dictionary<IPositionState, IMoveSpeed> CurrentSpeeds { get; }
        IMoveSpeed CurrentSpeed { get; }
        IEnumerable<IMoveSpeed> Speeds { get; }

        Queue<string> QueuedMoveCommands { get; }

        IParty Party { get; }

        IMovement Movement { get; set; }

        IMove Following { get; }

        /// <summary>
        /// Checks whether any restrictions are in place with this mover moving in general
        /// </summary>
        /// <param name="ignoreBlockers">Whether to ignore action-blockers from the command framework</param>
        /// <returns>True if the mover can, generally speaking, move about</returns>
        bool CanMove(bool ignoreBlockers = false);

        /// <summary>
        /// Checks whether the mover can move through a specific exit, including general and exit-specific conditions
        /// </summary>
        /// <param name="exit">The exit to consider</param>
        /// <param name="ignoreBlockers">Whether to ignore action-blockers from the command framework</param>
        /// <returns></returns>
        bool CanMove([NotNull]ICellExit exit, bool ignoreBlockers = false, bool ignoreSafeMovement = false);

        /// <summary>
        /// Examines whether an individual is capable of moving, even if they have to change their movement speed and/or position to do so.
        /// </summary>
        /// <param name="ignoreBlockingEffects">Whether to ignore blocking effects such as ongoing delayed actions</param>
        /// <returns>A ValueTuple containing a boolean representing whether they can move, a PositionState that is the highest position state they can adopt, and a MoveSpeed that is the fastest speed that they could go</returns>
        (bool Success, IPositionState MovingState, IMoveSpeed Speed) CouldMove(bool ignoreBlockingEffects, IPositionState fixedPosition);
        (bool Success, IEmoteOutput FailureOutput) CanCross(ICellExit exit);
        bool Move(string rawInput);
        bool Move(ICellExit exit, IEmote emote = null, bool ignoreSafeMovement = false);
        bool Move(CardinalDirection direction, IEmote emote = null, bool ignoreSafeMovement = false);
        bool Move(string cmd, string target, IEmote emote = null, bool ignoreSafeMovement = false);
        void JoinParty(IParty party);
        void LeaveParty(bool echo = true);

        void ExecuteMove(IMoveSpeed overrideSpeed = null);

        string DisplayInGroup(IPerceiver voyeur, int indent = 0);
        int MoveSpeed(ICellExit exit);
        string WhyCannotMove();
        void Follow(IMove thing);
        void CeaseFollowing();
        bool CanSee(ICell thing, ICellExit exit, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);

        event EventHandler<MoveEventArgs> OnStartMove;
        event EventHandler<MoveEventArgs> OnStopMove;
        event EventHandler<MoveEventArgs> OnMoved;
        event EventHandler<MoveEventArgs> OnMovedConsensually;
        void Moved(IMovement movement);
        void StopMovement(IMovement movement);
        void StartMove(IMovement movement);

        event PerceivableResponseEvent OnWantsToMove;
    }
}