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
		return false;
	}

	public bool CanBeMountedBy(ICharacter rider)
	{
		return false;
	}

	public string WhyCannotBeMountedBy(ICharacter rider)
	{
		return $"{HowSeen(rider, true)} is not something that can be mounted.";
	}

	private readonly List<ICharacter> _riders = new();

	public IEnumerable<ICharacter> Riders => _riders;

	public bool Mount(ICharacter rider)
	{
		if (!CanBeMountedBy(rider))
		{
			rider.OutputHandler.Send(new EmoteOutput(new Emote(WhyCannotBeMountedBy(rider), rider, rider, this)));
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
		rider.OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("DefaultCannotMountError"), rider, rider, this)));
		return false;
	}

	public void Dismount(ICharacter rider)
	{
		rider.OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("DefaultDismountMessage"), rider, rider, this)));
		_riders.Remove(rider);
	}

	public void RemoveRider(ICharacter rider)
	{
		_riders.Remove(rider);
	}

	public Difficulty ControlMountDifficulty(ICharacter rider)
	{
		return Difficulty.Impossible;
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