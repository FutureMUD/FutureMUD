using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Movement;

public class Party : PerceiverItem, IParty
{
	public Party(ICharacter leader)
	{
		Leader = leader;
		_members.Add(leader);
		Movement = null;
	}

	public override bool IdHasBeenRegistered => true;

	public override string FrameworkItemType => "Party";

	public void Join(IMove body)
	{
		if (!_members.Contains(body))
		{
			_members.Add(body);
		}

		if (Leader == null)
		{
			Leader = body as ICharacter;
		}
	}

	public bool Leave(IMove body)
	{
		if (Movement?.CharacterMovers.Contains(body) ?? false)
		{
			Movement.CancelForMoverOnly(body);
			body.Movement = null;
		}

		_members.Remove(body);
		if (Leader == body)
		{
			Leader = _members.OfType<ICharacter>().FirstOrDefault();
		}

		return _members.Count <= 1;
	}

	public void SetLeader(ICharacter leader)
	{
		Leader = leader;
	}

	public string DisplayMembers(IPerceiver voyeur, int indent = 0)
	{
		if (Leader == null)
		{
			return "There is no leader, this party is not configured correctly.";
		}

		var sb = new StringBuilder();
		if (Leader == null)
		{
			sb.AppendLine("This party has no leader.");
		}
		else
		{
			sb.AppendLine("\t" + Leader.DisplayInGroup(voyeur, indent));
		}

		foreach (var ch in Members.Except(Leader))
		{
			sb.AppendLine("\t" + ch.DisplayInGroup(voyeur, indent + 2));
		}

		return sb.ToString();
	}

	public string DisplayInGroup(IPerceiver voyeur, int indent = 0)
	{
		return DisplayMembers(voyeur, indent);
	}

	public override void Register(IOutputHandler handler)
	{
	}

	public void ExecuteMove(IMovement movement, IMoveSpeed overrideSpeed = null)
	{
		Moved(movement);
		foreach (var member in Members)
		{
			member.ExecuteMove(movement, overrideSpeed ?? CurrentSpeed);
		}
	}

	public void Disband()
	{
		if (_party != null)
		{
			_party.Leave(this);
			_party = null;
		}

		Movement?.Cancel();

		foreach (var member in _members.ToList())
		{
			member.LeaveParty(false);
		}

		_members.Clear();
		Leader = null;
	}

	public override Proximity GetProximity(IPerceivable thing)
	{
		return Proximity.Unapproximable;
	}

	public override (bool, IEmoteOutput) CanCross(ICellExit exit)
	{
		foreach (var member in Members)
		{
			var (canMove, whyNot) = member.CanCross(exit);
			if (!canMove)
			{
				return (false, whyNot);
			}
		}

		return (true, null);
	}

	public override int LineFormatLength => int.MaxValue;

	public override int InnerLineFormatLength => int.MaxValue;

	public override double DefensiveAdvantage
	{
		get => 0;
		set
		{
			// Do nothing
		}
	}

	public override double OffensiveAdvantage
	{
		get => 0;
		set
		{
			// Do nothing
		}
	}

	public override DefenseType PreferredDefenseType
	{
		get => DefenseType.None;
		set
		{
			// Do nothing
		}
	}

	public override ICombatMove ResponseToMove(ICombatMove move, IPerceiver assailant)
	{
		return null;
	}

	public override bool CheckCombatStatus()
	{
		return false;
	}

	#region IFutureProgVariable Implementation

	public override ProgVariableTypes Type => ProgVariableTypes.Error;

	#endregion

	public override object DatabaseInsert()
	{
		// Not required to do late initialisation
		return this;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		// Do nothing
	}

	#region IParty Members

	private ICharacter _leader;

	public ICharacter Leader
	{
		get => _leader ?? CharacterMembers.FirstOrDefault();
		protected set => _leader = value;
	}

	protected readonly List<IMove> _members = new();
	public IEnumerable<IMove> Members => _members;

	public IEnumerable<ICharacter> CharacterMembers
	{
		get
		{
			return
				_members.OfType<ICharacter>().Concat(_members.OfType<IParty>().SelectMany(x => x.CharacterMembers));
		}
	}

	/// <summary>
	/// Active Members are people who are present with the leader, and able and willing to move
	/// </summary>
	public IEnumerable<IMove> ActiveMembers
	{
		get
		{
			return Members.Where(x =>
				x.Location == Leader.Location && x.CanMove() && !x.EffectsOfType<IDragParticipant>().Any()).ToList();
		}
	}

	/// <summary>
	/// Active Members are people who are present with the leader, and able and willing to move
	/// </summary>
	public IEnumerable<ICharacter> ActiveCharacterMembers
	{
		get
		{
			return CharacterMembers.Where(x =>
				x.Location == Leader.Location && x.CanMove() && !x.EffectsOfType<IDragParticipant>().Any()).ToList();
		}
	}

	#endregion

	#region IMove Members

	public event EventHandler<MoveEventArgs> OnMoved;
	public event EventHandler<MoveEventArgs> OnStartMove;
	public event EventHandler<MoveEventArgs> OnStopMove;
	public event EventHandler<MoveEventArgs> OnMovedConsensually;
	public event PerceivableResponseEvent OnWantsToMove;

	public void Moved(IMovement movement)
	{
		OnMoved?.Invoke(this, new MoveEventArgs(this, movement));
		foreach (var member in ActiveMembers)
		{
			member.Moved(movement);
		}
	}

	public void StopMovement(IMovement movement)
	{
		OnStopMove?.Invoke(this, new MoveEventArgs(this, movement));
		foreach (var member in ActiveMembers)
		{
			member.StopMovement(movement);
		}
	}

	public void StartMove(IMovement movement)
	{
		OnStartMove?.Invoke(this, new MoveEventArgs(this, movement));
		foreach (var member in ActiveMembers)
		{
			member.StartMove(movement);
		}
	}

	public bool CanMove(bool ignoreBlockers = false)
	{
		if (ActiveMembers.Any(x => !x.CanMove(ignoreBlockers)))
		{
			_cannotMoveReason = ActiveMembers.Where(x => !x.CanMove(ignoreBlockers)).Select(x => x.HowSeen(Leader))
			                                 .ListToString() +
			                    " cannot move.";
			return false;
		}

		var args = new PerceivableRejectionResponse();
		OnWantsToMove?.Invoke(this, args);

		if (args.Rejected)
		{
			_cannotMoveReason = args.Reason;
			return false;
		}

		return true;
	}

	public (bool Success, IPositionState MovingState, IMoveSpeed Speed) CouldMove(bool ignoreBlockingEffects,
		IPositionState fixedPosition)
	{
		return (CanMove(ignoreBlockingEffects), null, Leader.CurrentSpeed);
	}

	public IMove Following { get; protected set; }

	public void Follow(IMove thing)
	{
		throw new NotImplementedException();
	}

	public void CeaseFollowing()
	{
		throw new NotImplementedException();
	}

	public bool CanSee(ICell thing, ICellExit exit, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return false;
	}

	public void JoinParty(IParty party)
	{
		_party = party;
		_party.Join(this);
	}

	public void LeaveParty(bool echo = true)
	{
		if (_party?.Leave(this) == true)
		{
			foreach (var ch in _party.Members.ToList())
			{
				if (echo)
				{
					ch.OutputHandler.Send("Your party is disbanded.");
				}

				ch.LeaveParty();
			}
		}

		_party = null;
	}

	public Dictionary<IPositionState, IMoveSpeed> CurrentSpeeds => Leader.CurrentSpeeds;
	public IMoveSpeed CurrentSpeed => Leader.CurrentSpeed;
	public IEnumerable<IMoveSpeed> Speeds => Leader.Speeds;

	public Queue<string> QueuedMoveCommands => Leader?.QueuedMoveCommands ?? new Queue<string>();

	public bool CanMove(ICellExit exit, bool ignoreBlockers = false, bool ignoreSafeMovement = false)
	{
		if (Members.All(x => x.CanMove(exit, ignoreBlockers, ignoreSafeMovement)))
		{
			return true;
		}

		_cannotMoveReason = Members.Where(x => !x.CanMove(exit, ignoreBlockers, ignoreSafeMovement))
		                           .Select(x => x.HowSeen(Leader)).ListToString() +
		                    " cannot move.";
		return false;
	}

	public bool Move(string rawString)
	{
		return Leader.Move(rawString);
	}

	public bool Move(ICellExit exit, IEmote emote = null, bool ignoreSafeMovement = false)
	{
		if (CanMove(exit, ignoreSafeMovement: ignoreSafeMovement))
		{
			foreach (var member in ActiveCharacterMembers)
			{
				if (member.PositionState.TransitionOnMovement != null)
				{
					member.SetState(member.PositionState.TransitionOnMovement);
				}

				member.SetModifier(PositionModifier.None);
				member.SetTarget(null);
			}

			var timespan = TimeSpan.Zero;
			if (!Leader.AffectedBy<IImmwalkEffect>())
			{
				timespan = TimeSpan.FromMilliseconds(MoveSpeed(exit));
			}

			IMovement movement;
			if (ActiveCharacterMembers.Any(x => x.AffectedBy<IDragParticipant>()))
			{
				var effects = ActiveCharacterMembers
				              .SelectMany(x => x.EffectsOfType<Dragging>())
				              .Distinct()
				              .Select(x => (x, x.CharacterOwner, x.CharacterDraggers.Except(x.CharacterOwner),
					              x.Target))
				              .ToList();
				movement = new GroupDragMovement(this, effects, exit, timespan);
			}
			else
			{
				movement = new GroupMovement(exit, this, timespan);
			}

			movement.InitialAction();
			return true;
		}

		return false;
	}

	public bool Move(CardinalDirection direction, IEmote emote = null, bool ignoreSafeMovement = false)
	{
		var exit = Leader.Location.GetExit(direction, this);
		if (exit == null || !Leader.CanSee(Leader.Location, exit))
		{
			_cannotMoveReason = "You cannot move in that direction.";
			return false;
		}

		return Move(exit, emote, ignoreSafeMovement);
	}

	public bool Move(string cmd, string target, IEmote emote = null, bool ignoreSafeMovement = false)
	{
		var exit = Leader.Location.GetExit(cmd, target, Leader);
		if (exit == null || !Leader.CanSee(Leader.Location, exit))
		{
			_cannotMoveReason = "You cannot move in that direction.";
			return false;
		}

		return Move(exit, emote, ignoreSafeMovement);
	}

	protected IParty _party;
	IParty IMove.Party => _party;

	public IMovement Movement { get; set; }

	public int MoveSpeed(ICellExit exit)
	{
		return Members.Max(x => x.MoveSpeed(exit));
	}

	public IMoveSpeed SlowestSpeed(ICellExit exit)
	{
		var partyMembers = Members.OrderByDescending(x => x.MoveSpeed(exit));
		var slowest = partyMembers.FirstOrDefault();
		return slowest?.CurrentSpeed;
	}

	private string _cannotMoveReason;

	public string WhyCannotMove()
	{
		return _cannotMoveReason ??= "You cannot move in that direction.";
	}

	#endregion

	#region IPerceiver Members

	public override PerceptionTypes NaturalPerceptionTypes => throw new NotImplementedException();

	public override bool CanHear(IPerceivable thing)
	{
		throw new NotImplementedException();
	}

	public override bool CanSense(IPerceivable thing, bool ignoreFuzzy = false)
	{
		throw new NotImplementedException();
	}

	public override bool CanSee(IPerceivable thing, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		throw new NotImplementedException();
	}

	public override bool CanSmell(IPerceivable thing)
	{
		throw new NotImplementedException();
	}

	public void Look()
	{
		throw new NotImplementedException();
	}

	public void Look(IPerceivable thing)
	{
		throw new NotImplementedException();
	}

	public void LookIn(IPerceivable thing)
	{
		throw new NotImplementedException();
	}

	#endregion
}