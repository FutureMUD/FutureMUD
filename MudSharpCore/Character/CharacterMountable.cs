using System.Collections.Generic;
using System.Linq;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Construction.Boundary;
using MudSharp.Body.Position;
using MudSharp.Character.Heritage;
using MudSharp.Framework;

namespace MudSharp.Character;

public partial class Character
{
#nullable enable
	private ICharacter? _ridingMount;
	public ICharacter? RidingMount
	{
		get => _ridingMount;
		set
		{
			_ridingMount = value;
			Changed = true;
		}
	}
#nullable restore
	public bool CanEverBeMounted(ICharacter rider)
	{
		var ai = (this as INPC)?.AIs.OfType<IMountableAI>().FirstOrDefault();
		if (ai is null)
		{
			return false;
		}

		if (ai.PermitRider(this, rider))
		{
			return true;
		}

		return false;
	}

	public bool CanBeMountedBy(ICharacter rider)
	{
		var ai = (this as INPC)?.AIs.OfType<IMountableAI>().FirstOrDefault();
		if (ai is null)
		{
			return false;
		}

		if (!ai.PermitRider(this, rider))
		{
			return false;
		}

		if (_riders.Contains(rider))
		{
			return false;
		}

		if (rider.CurrentContextualSize(SizeContext.RidingMount) > CurrentContextualSize(SizeContext.BeingRiddenAsMount))
		{
			return false;
		}

		if (_riders.Count >= ai.MaximumNumberOfRiders)
		{
			return false;
		}

		if (_riders.Sum(x => ai.RiderSlotsOccupiedBy(x)) + ai.RiderSlotsOccupiedBy(rider) > ai.RiderSlots)
		{
			return false;
		}

		return true;
	}

	public string WhyCannotBeMountedBy(ICharacter rider)
	{
		var ai = (this as INPC)?.AIs.OfType<IMountableAI>().FirstOrDefault();
		if (ai is null)
		{
			return $"{HowSeen(rider, true)} is not something that can be mounted.";
		}

		if (!ai.PermitRider(this, rider))
		{
			return ai.WhyCannotPermitRider(this, rider);
		}

		if (_riders.Contains(rider))
		{
			return $"You are already riding {HowSeen(rider)}.";
		}

		if (rider.CurrentContextualSize(SizeContext.RidingMount) > CurrentContextualSize(SizeContext.BeingRiddenAsMount))
		{
			return $"You are too big to ride {HowSeen(rider)}.";
		}

		if (_riders.Count >= ai.MaximumNumberOfRiders)
		{
			return $"{HowSeen(rider, true)} already has the maximum number of riders.";
		}

		if (_riders.Sum(x => ai.RiderSlotsOccupiedBy(x)) + ai.RiderSlotsOccupiedBy(rider) > ai.RiderSlots)
		{
			return $"{HowSeen(rider, true)} does not have enough rider room to permit you as a rider.";
		}

		return $"{HowSeen(rider, true)} is not something that can be mounted.";
	}

	private readonly List<ICharacter> _riders = new();

	public IEnumerable<ICharacter> Riders => _riders;

	public bool Mount(ICharacter rider)
	{
		if (!CanBeMountedBy(rider))
		{
			rider.OutputHandler.Send(new EmoteOutput(new Emote(WhyCannotBeMountedBy(rider), this, this, rider)));
			return false;
		}

		var ai = (this as INPC)?.AIs.OfType<IMountableAI>().FirstOrDefault();
		if (ai is null)
		{
			rider.OutputHandler.Send(new EmoteOutput(new Emote("@ is not something that you can mount.", this, this)));
			return false;
		}

		_riders.Add(rider);
		rider.RidingMount = this;
		rider.OutputHandler.Handle(new EmoteOutput(new Emote(ai.MountEmote(this, rider), this, this, rider)));
		return false;
	}

	public void Dismount(ICharacter rider)
	{
		var ai = (this as INPC)?.AIs.OfType<IMountableAI>().FirstOrDefault();
		if (ai is null)
		{
			rider.OutputHandler.Handle(new EmoteOutput(new Emote("$1 $1|dismount|dismounts $0.", this, this, rider)));
			return;
		}

		rider.OutputHandler.Handle(new EmoteOutput(new Emote(ai.DismountEmote(this, rider), this, this, rider)));
		rider.RidingMount = null;
		_riders.Remove(rider);
	}

	public void RemoveRider(ICharacter rider)
	{
		_riders.Remove(rider);
	}

	public Difficulty ControlMountDifficulty(ICharacter rider)
	{
		var ai = (this as INPC)?.AIs.OfType<IMountableAI>().FirstOrDefault();
		if (ai is null)
		{
			return Difficulty.Impossible;
		}

		return ai.ControlDifficulty(this, rider);
	}

	public bool IsPrimaryRider(ICharacter rider)
	{
		return _riders.FirstOrDefault() == rider;
	}

	public bool BuckRider()
	{
		return false;
	}

	public bool PermitControl(ICharacter rider)
	{
		var ai = (this as INPC)?.AIs.OfType<IMountableAI>().FirstOrDefault();
		return ai?.PermitControl(this, rider) ?? true;
	}

	public void HandleControlDenied(ICharacter rider)
	{
		var ai = (this as INPC)?.AIs.OfType<IMountableAI>().FirstOrDefault();
		ai?.HandleDeniedControl(this, rider);
	}

	public bool RiderMove(ICellExit exit, ICharacter rider, IEmote emote = null, bool ignoreSafeMovement = false)
	{
		if (!IsPrimaryRider(rider))
		{
			rider.OutputHandler.Send("You are not in control of this mount.");
			return false;
		}

		if (!PermitControl(rider))
		{
			HandleControlDenied(rider);
			return false;
		}

		var result = Move(exit, emote, ignoreSafeMovement);
		if (!result)
		{
			rider.OutputHandler.Send(WhyCannotMove());
		}

		return result;
	}

	public bool RiderFly(ICharacter rider, IEmote emote = null)
	{
		if (!IsPrimaryRider(rider))
		{
			rider.OutputHandler.Send("You are not in control of this mount.");
			return false;
		}

		if (!PermitControl(rider))
		{
			HandleControlDenied(rider);
			return false;
		}

		var check = CanFly();
		if (!check.Truth)
		{
			rider.OutputHandler.Send(check.Error);
			return false;
		}

		Fly(emote);
		return true;
	}

	public bool RiderAscend(ICharacter rider, IEmote emote = null)
	{
		if (!IsPrimaryRider(rider))
		{
			rider.OutputHandler.Send("You are not in control of this mount.");
			return false;
		}

		if (!PermitControl(rider))
		{
			HandleControlDenied(rider);
			return false;
		}

		var check = ((IFly)this).CanAscend();
		if (!check.Truth)
		{
			rider.OutputHandler.Send(check.Error);
			return false;
		}

		((IFly)this).Ascend(emote);
		return true;
	}

	public bool RiderDive(ICharacter rider, IEmote emote = null)
	{
		if (!IsPrimaryRider(rider))
		{
			rider.OutputHandler.Send("You are not in control of this mount.");
			return false;
		}

		if (!PermitControl(rider))
		{
			HandleControlDenied(rider);
			return false;
		}

		var check = ((IFly)this).CanDive();
		if (!check.Truth)
		{
			rider.OutputHandler.Send(check.Error);
			return false;
		}

		((IFly)this).Dive(emote);
		return true;
	}

	public bool RiderClimbUp(ICharacter rider, IEmote emote = null)
	{
		if (!IsPrimaryRider(rider))
		{
			rider.OutputHandler.Send("You are not in control of this mount.");
			return false;
		}

		if (!PermitControl(rider))
		{
			HandleControlDenied(rider);
			return false;
		}

		var check = CanClimbUp();
		if (!check.Truth)
		{
			rider.OutputHandler.Send(check.Error);
			return false;
		}

		ClimbUp(emote);
		return true;
	}

	public bool RiderClimbDown(ICharacter rider, IEmote emote = null)
	{
		if (!IsPrimaryRider(rider))
		{
			rider.OutputHandler.Send("You are not in control of this mount.");
			return false;
		}

		if (!PermitControl(rider))
		{
			HandleControlDenied(rider);
			return false;
		}

		var check = CanClimbDown();
		if (!check.Truth)
		{
			rider.OutputHandler.Send(check.Error);
			return false;
		}

		ClimbDown(emote);
		return true;
	}

	public bool RiderMovePosition(IPositionState whichPosition, PositionModifier whichModifier, IPerceivable target,
			ICharacter rider, IEmote playerEmote = null, IEmote playerPmote = null,
			bool ignoreMovementRestrictions = false, bool ignoreMovement = false)
	{
		if (!IsPrimaryRider(rider))
		{
			rider.OutputHandler.Send("You are not in control of this mount.");
			return false;
		}

		if (!PermitControl(rider))
		{
			HandleControlDenied(rider);
			return false;
		}

		if (!CanMovePosition(whichPosition, whichModifier, target, ignoreMovementRestrictions, ignoreMovement))
		{
			rider.OutputHandler.Send(WhyCannotMovePosition(whichPosition, whichModifier, target, ignoreMovementRestrictions));
			return false;
		}

		MovePosition(whichPosition, whichModifier, target, playerEmote, playerPmote, ignoreMovementRestrictions, ignoreMovement);
		return true;
	}
}