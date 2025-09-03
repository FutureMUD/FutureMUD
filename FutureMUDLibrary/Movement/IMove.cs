using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using JetBrains.Annotations;
using MudSharp.Body.Position;
using MudSharp.Character;
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

	public record CanMoveResponse
	{
		public static CanMoveResponse True => new CanMoveResponse
		{
			Result = true,
			ErrorMessage = string.Empty,
			WouldBeAbleToCross = null,
			HighestMovingPositionState = null,
			FastestMoveSpeed = null
		};

		public required bool Result { get; init; }
		public required string ErrorMessage { get; init; }
		public bool? WouldBeAbleToCross { get; init; }
		public IPositionState? HighestMovingPositionState { get; init; }
		public IMoveSpeed? FastestMoveSpeed { get; init; }

		public static implicit operator bool(CanMoveResponse source)
		{
			return source.Result;
		}
	}
#nullable disable

	[Flags]
	public enum CanMoveFlags
	{
		None,
		IgnoreMount = 1,
		IgnoreCancellableActionBlockers = 2,
		IgnoreWhetherExitCanBeCrossed = 4,
		IgnoreSafeMovement = 8
	}

	public interface IMove : IPerceiver {
		Dictionary<IPositionState, IMoveSpeed> CurrentSpeeds { get; }
		IMoveSpeed CurrentSpeed { get; }
		IEnumerable<IMoveSpeed> Speeds { get; }

		Queue<string> QueuedMoveCommands { get; }

		IParty Party { get; }

		IMovement Movement { get; set; }

		IMove Following { get; }

		CanMoveResponse CanMove(CanMoveFlags flags);
		CanMoveResponse CanMove(ICellExit exit, CanMoveFlags flags = CanMoveFlags.None);

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

		void ExecuteMove(IMovement movement, IMoveSpeed overrideSpeed = null);

		string DisplayInGroup(IPerceiver voyeur, int indent = 0);
		double MoveSpeed(ICellExit exit);
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