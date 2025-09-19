using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionEngine;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character.Heritage;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.Movement;
using MudSharp.NPC.AI.Groups;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Character;

public partial class Character
{
	public double MaximumDragWeight => Race.GetMaximumDragWeight(this);

	public void TransferTo(ICell target, RoomLayer layer)
	{
		OutputHandler.Handle(
			new EmoteOutput(
				new Emote("$0 opens a swirling vortex of energy and disappears as #0 steps through.", this, this),
				flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
		OutputHandler.Send(
			"You open a swirling vortex of magical energies, stepping through and emerging somewhere new.");

		Teleport(target, layer, true, false);

		OutputHandler.Handle(
			new EmoteOutput(new Emote("A swirling vortex of energy opens up briefly, and @ steps through.", this),
				flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
	}

	public void Teleport(ICell target, RoomLayer layer, bool includeFollowers, bool echo,
		string playerEchoLeave = "@ leaves the area.",
		string playerEchoArrive = "@ enters the area.",
		string playerEchoSelf = "",
		string followerEchoLeave = "@ leaves the area.",
		string followerEchoArrive = "@ enters the area.",
		string followerEchoSelf = "")
	{
		Movement?.CancelForMoverOnly(this);
		RemoveAllEffects(x => x.IsEffectType<IActionEffect>(), true);
		RemoveAllEffects<IRemoveOnMovementEffect>(fireRemovalAction: true);
		RemoveAllEffects<Dragging.DragTarget>(fireRemovalAction: true);
		var otherMovers = new List<ICharacter>();
		var otherItems = new List<IGameItem>();
		var drag = EffectHandler.EffectsOfType<IDragging>().FirstOrDefault();
		if (includeFollowers)
		{
			if (drag is not null)
			{
				if (drag.Target is ICharacter ch)
				{
					otherMovers.Add(ch);
				}
				else if (drag.Target is IGameItem item)
				{
					otherItems.Add(item);
				}
				otherMovers.AddRange(drag.CharacterDraggers.Except(this).Where(x => x.ColocatedWith(this)));
			}

			if (Party is not null && Party.Leader == this)
			{
				otherMovers.AddRange(Party.CharacterMembers.Except(this).Except(otherMovers).Where(x => x.ColocatedWith(this)));
			}
		}
		else
		{
			if (drag is not null)
			{
				RemoveEffect(drag, true);
			}
		}

		if (RidingMount is not null)
		{
			otherMovers.Add(RidingMount);
		}

		otherMovers.AddRange(_riders);
		otherMovers = otherMovers.Distinct().ToList();

		foreach (var mover in otherMovers)
		{
			Movement?.CancelForMoverOnly(mover);
			mover.RemoveAllEffects(x => x.IsEffectType<IActionEffect>(), true);
			mover.RemoveAllEffects<IRemoveOnMovementEffect>(fireRemovalAction: true);
		}

		if (echo)
		{
			OutputHandler.Handle(new EmoteOutput(new Emote(playerEchoLeave, this, this), flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
			foreach (var mover in otherMovers)
			{
				mover.OutputHandler.Handle(new EmoteOutput(new Emote(followerEchoLeave, mover, mover, this), flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
				if (!string.IsNullOrEmpty(followerEchoSelf))
				{
					mover.OutputHandler.Send(new EmoteOutput(new Emote(followerEchoSelf, mover, mover, this)));
				}
			}

			if (!string.IsNullOrEmpty(playerEchoSelf))
			{
				OutputHandler.Send(new EmoteOutput(new Emote(playerEchoSelf, this, this)));
			}
		}

		SetPosition(PositionState, PositionModifier.None, null, null);
		Location.Leave(this);
		foreach (var item in otherItems)
		{
			Location.Extract(item);
		}
		RoomLayer = layer;

		foreach (var mover in otherMovers)
		{
			mover.SetPosition(mover.PositionState, PositionModifier.None, null, null);
			mover.Location.Leave(mover);
			mover.RoomLayer = layer;
		}

		target.Enter(this);
		foreach (var mover in otherMovers)
		{
			target.Enter(mover);
		}

		foreach (var item in otherItems)
		{
			item.RoomLayer = layer;
			target.Insert(item, true);
		}

		if (echo)
		{
			OutputHandler.Handle(new EmoteOutput(new Emote(playerEchoArrive, this), flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
			foreach (var mover in otherMovers)
			{
				mover.OutputHandler.Handle(new EmoteOutput(new Emote(followerEchoArrive, mover), flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
			}
		}

		Body.Look();
		foreach (var mover in otherMovers)
		{
			mover.Body.Look();
		}
	}

	public override (bool Success, IEmoteOutput FailureOutput) CanCross(ICellExit exit)
	{
		if (EffectHandler.AffectedBy<IImmwalkEffect>() || RidingMount?.AffectedBy<IImmwalkEffect>() == true)
		{
			return (true, null);
		}

		if (exit.Exit.Door?.IsOpen == false)
		{
			return (false,
				new EmoteOutput(new Emote("@ stop|stops at $0 because it is closed.", this, exit.Exit.Door.Parent),
					flags: OutputFlags.SuppressObscured));
		}

		ICharacter guardCharacter = null;
		foreach (var ch in exit.Origin.LayerCharacters(RoomLayer))
		{
			if (ch == this || ch == RidingMount || _riders.Contains(ch))
			{
				continue;
			}

			if (ch.EffectsOfType<IGuardExitEffect>()
				  .Any(x =>
					  !x.PermittedToCross(this, exit) ||
					  !x.PermittedToCross(RidingMount, exit) ||
					  _riders.Any(y => !x.PermittedToCross(y, exit))
					))
			{
				guardCharacter = ch;
				break;
			}
		}
		if (guardCharacter != null)
		{
			return (false,
				new EmoteOutput(
					new Emote(
						$"@ stop|stops at the exit to {exit.OutboundDirectionDescription} because $1 $1|are|is guarding it.",
						this, this, guardCharacter), flags: OutputFlags.SuppressObscured));
		}

		return (true, null);
	}

	public Dictionary<IPositionState, IMoveSpeed> CurrentSpeeds => Body.CurrentSpeeds;

	public IMoveSpeed CurrentSpeed => CurrentSpeeds.ContainsKey(PositionState) ? CurrentSpeeds[PositionState] : null;

	public IEnumerable<IMoveSpeed> Speeds => Body.Speeds;

	public bool CanSee(ICell thing, ICellExit exit, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return Body.CanSee(thing, exit, flags);
	}

	public IPositionState MostUprightMobilePosition(bool ignoreCouldMove = false)
	{
		if (Location.ExitsFor(this).Any(x => x.IsFallExit) && PositionState.SafeFromFalling)
		{
			return PositionState;
		}

		if ((CanMovePosition(PositionStanding.Instance, ignoreCouldMove) ||
			 PositionState == PositionStanding.Instance) &&
			(ignoreCouldMove || CouldMove(true, PositionStanding.Instance).Success))
		{
			return PositionStanding.Instance;
		}

		if ((CanMovePosition(PositionProstrate.Instance, ignoreCouldMove) ||
			 PositionState == PositionProstrate.Instance) &&
			(ignoreCouldMove || CouldMove(true, PositionProstrate.Instance).Success))
		{
			return PositionProstrate.Instance;
		}

		if ((CanMovePosition(PositionProne.Instance, ignoreCouldMove) || PositionState == PositionProne.Instance) &&
			(ignoreCouldMove || CouldMove(true, PositionProne.Instance).Success))
		{
			return PositionProne.Instance;
		}

		return null;
	}

	private static ITraitExpression _fallDamageExpression;

	public static ITraitExpression FallDamageExpression
	{
		get
		{
			if (_fallDamageExpression == null)
			{
				_fallDamageExpression =
					new Body.Traits.TraitExpression(Futuremud.Games.First().GetStaticConfiguration("CharacterFallDamageExpression"),
						Futuremud.Games.First());
			}

			return _fallDamageExpression;
		}
	}

	public override void DoFallDamage(double fallDistance)
	{
		var check = Gameworld.GetCheck(CheckType.FallingImpactCheck);
		var result = check.Check(this, Difficulty.Normal.StageUp((int)fallDistance));
		FallDamageExpression.Formula.Parameters["rooms"] = fallDistance;
		FallDamageExpression.Formula.Parameters["weight"] = Weight;
		FallDamageExpression.Formula.Parameters["check"] = result.CheckDegrees();
		FallDamageExpression.Formula.Parameters["success"] = result.SuccessDegrees();
		FallDamageExpression.Formula.Parameters["failure"] = result.FailureDegrees();
		var damageAmount = FallDamageExpression.Evaluate(this);
		var damage = new Damage
		{
			DamageType = DamageType.Falling,
			DamageAmount = damageAmount,
			PainAmount = damageAmount
		};

		var targetLimbs = new List<LimbType>();
		switch (result.Outcome)
		{
			case Outcome.MajorFail:
				if (Dice.Roll(1, 2) == 1)
				{
					targetLimbs.Add(LimbType.Head);
				}

				targetLimbs.Add(LimbType.Torso);
				targetLimbs.Add(LimbType.Torso);
				targetLimbs.Add(LimbType.Appendage);
				targetLimbs.Add(LimbType.Arm);
				targetLimbs.Add(LimbType.Leg);
				targetLimbs.Add(LimbType.Wing);
				targetLimbs.Add(LimbType.Genitals);
				break;
			case Outcome.Fail:
				if (Dice.Roll(1, 3) == 1)
				{
					targetLimbs.Add(LimbType.Torso);
					targetLimbs.Add(LimbType.Torso);
					targetLimbs.Add(LimbType.Appendage);
					targetLimbs.Add(LimbType.Leg);
				}
				else
				{
					targetLimbs.Add(LimbType.Torso);
					targetLimbs.Add(LimbType.Appendage);
					targetLimbs.Add(LimbType.Appendage);
					targetLimbs.Add(LimbType.Arm);
					targetLimbs.Add(LimbType.Arm);
					targetLimbs.Add(LimbType.Wing);
					targetLimbs.Add(LimbType.Wing);
					break;
				}

				break;
			case Outcome.MinorFail:
				if (Dice.Roll(1, 3) == 1)
				{
					targetLimbs.Add(LimbType.Appendage);
					targetLimbs.Add(LimbType.Leg);
				}
				else
				{
					targetLimbs.Add(LimbType.Appendage);
					targetLimbs.Add(LimbType.Arm);
					targetLimbs.Add(LimbType.Wing);
					break;
				}

				break;
			default:
				targetLimbs.Add(LimbType.Leg);
				break;
		}

		var wounds = new List<IWound>();
		foreach (var target in targetLimbs)
		{
			var limb = Body.Limbs.Where(x => x.LimbType == target).GetRandomElement();
			if (limb == null)
			{
				continue;
			}

			var bodypart = Body.BodypartsForLimb(limb).GetWeightedRandom(x => x.RelativeHitChance);
			var partDamage = new Damage(damage) { Bodypart = bodypart };
			wounds.AddRange(Body.PassiveSufferDamage(partDamage));
		}

		wounds.ProcessPassiveWounds();
	}

	public override bool CouldTransitionToLayer(RoomLayer otherLayer)
	{
		switch (otherLayer)
		{
			case RoomLayer.GroundLevel:
				return true;
			case RoomLayer.Underwater:
			case RoomLayer.DeepUnderwater:
			case RoomLayer.VeryDeepUnderwater:
				return Race.CanSwim;
			case RoomLayer.InTrees:
			case RoomLayer.HighInTrees:
			case RoomLayer.OnRooftops:
				return Race.CanClimb || CanFly().Truth;
			case RoomLayer.InAir:
			case RoomLayer.HighInAir:
				return CanFly().Truth;
		}

		return base.CouldTransitionToLayer(otherLayer);
	}

	#region IMove Members

	public event EventHandler<MoveEventArgs> OnMoved;
	public event EventHandler<MoveEventArgs> OnMovedConsensually;
	public event EventHandler<MoveEventArgs> OnStartMove;
	public event EventHandler<MoveEventArgs> OnStopMove;
	public event PerceivableResponseEvent OnWantsToMove;

	public void Moved(IMovement movement)
	{
		OnMoved?.Invoke(this, new MoveEventArgs(this, movement));
		if (movement.IsConsensualMover(this))
		{
			OnMovedConsensually?.Invoke(this, new MoveEventArgs(this, movement));
		}
	}

	public void StopMovement(IMovement movement)
	{
		OnStopMove?.Invoke(this, new MoveEventArgs(this, movement));
	}

	public void StartMove(IMovement movement)
	{
		OnStartMove?.Invoke(this, new MoveEventArgs(this, movement));
		EffectHandler.RemoveAllEffects(x => x.IsEffectType<IRemoveOnStartMovement>(), true);
		Body.RemoveAllEffects(x => x.IsEffectType<IRemoveOnStartMovement>(), true);
		if (PositionState.TransitionOnMovement != null)
		{
			SetState(PositionState.TransitionOnMovement);
		}

		SetModifier(PositionModifier.None);
		SetTarget(null);
	}

	public void JoinParty(IParty party)
	{
		Party = party;
		Party.Join(this);
	}

	public void LeaveParty(bool echo = true)
	{
		if (Party == null)
		{
			return;
		}

		if (Party.Leave(this) && echo)
		{
			foreach (var ch in Party.Members.ToList())
			{
				ch.OutputHandler.Send("Your party is disbanded.");
				ch.LeaveParty();
			}
		}

		Party = null;
	}

	public IParty Party { get; protected set; }

	public Queue<string> QueuedMoveCommands { get; protected set; } = new();

	public void ConvertGrapplesToDrags()
	{
		var grappling = CombinedEffectsOfType<IGrappling>().FirstOrDefault();
		if (grappling?.TargetEffect.UnderControl == true && !IsEngagedInMelee)
		{
			var target = grappling.Target;
			RemoveEffect(grappling, true);
			var drag = new Dragging(this, null, target);
			RemoveAllEffects<Dragging>(fireRemovalAction: true);
			AddEffect(drag);
		}
	}

	public void DoCombatKnockdown()
	{
		if (RidingMount is not null)
		{
			DoFallOffHorse();
		}

		// TODO - check sprawled is a valid position and maybe give a chance (skill based?) to land in another position
		SetPosition(PositionSprawled.Instance, PositionModifier.None, PositionTarget, null);
	}

	public void DoFallOffHorse()
	{
		if (RidingMount is null)
		{
			return;
		}

		OutputHandler.Handle(new EmoteOutput(new Emote("@ fall|falls off $1!", this, this, RidingMount)));
		RidingMount.RemoveRider(this);
		RidingMount = null;
		DoFallDamage(0.5);
	}

	public bool Move(ICellExit exit, IEmote emote = null, bool ignoreSafeMovement = false)
	{
		if (Movement != null)
		{
			return false;
		}

		ConvertGrapplesToDrags();

		var flags = CanMoveFlags.IgnoreWhetherExitCanBeCrossed | CanMoveFlags.IgnoreCancellableActionBlockers;
		if (ignoreSafeMovement)
		{
			flags |= CanMoveFlags.IgnoreSafeMovement;
		}

		var response = CanMove(flags);
		if (!response.Result)
		{
			OutputHandler.Send(response.ErrorMessage);
			HandleEvent(EventType.CharacterCannotMove, this, Location, exit);
			return false;
		}

		var movement = MudSharp.Movement.Movement.CreateMovement(this, exit, emote, ignoreSafeMovement);
		if (movement is null)
		{
			return false;
		}

		foreach (var mover in movement.CharacterMovers)
		{
			mover.Movement = movement;
		}
		
		movement.InitialAction();
		return true;
	}

	protected double StaminaForMovement(IPositionState movingPosition, IMoveSpeed speed, double staminaMultiplier,
		bool ignoreTerrainStamina)
	{
		if (movingPosition == PositionFlying.Instance)
		{
			return
				FlyStaminaMultiplier() *
				staminaMultiplier *
				speed?.StaminaMultiplier ?? 1.0 *
				(1 + EncumbrancePercentage * Gameworld.GetStaticDouble("StaminaMultiplierPerEncumbrancePercentage"));
		}

		return
			(1 + EncumbrancePercentage * Gameworld.GetStaticDouble("StaminaMultiplierPerEncumbrancePercentage")) *
			(ignoreTerrainStamina ? 1.0 : Location.Terrain(this)?.StaminaCost ?? 0) *
			staminaMultiplier *
			speed?.StaminaMultiplier ?? 1.0;
	}



	public CanMoveResponse CanMove(CanMoveFlags flags)
	{
		if (EffectsOfType<IImmwalkEffect>().Any())
		{
			return CanMoveResponse.True;
		}

		if (!flags.HasFlag(CanMoveFlags.IgnoreCancellableActionBlockers) && Effects.Any(x => x.IsBlockingEffect("movement") && !x.CanBeStoppedByPlayer))
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = $"You cannot move because you are {Effects.First(x => x.IsBlockingEffect("movement") && !x.CanBeStoppedByPlayer).BlockingDescription("movement", this)}."
			};
		}

		if (PositionState.MoveRestrictions == MovementAbility.Restricted ||
			(PositionState.MoveRestrictions == MovementAbility.FreeIfNotInOn &&
			 (PositionModifier == PositionModifier.On || PositionModifier == PositionModifier.In)))
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = "Your position prevents you from moving. You should stand up first."
			};
		}

		var dragging = CombinedEffectsOfType<Dragging>().Any();
		var staminaMultiplier = 1.0;
		var ignoreTerrainStamina = false;

		IPositionState movingPosition;
		switch (PositionState)
		{
			case PositionSwimming _:
				movingPosition = PositionSwimming.Instance;
				staminaMultiplier = dragging ? 2.5 : 1.5;
				break;
			case PositionClimbing _:
				movingPosition = PositionClimbing.Instance;
				staminaMultiplier = dragging ? 2.5 : 1.5;
				break;
			case PositionFlying _:
				movingPosition = PositionFlying.Instance;
				ignoreTerrainStamina = true;
				goto default;
			default:
				movingPosition = PositionState.TransitionOnMovement ?? PositionState;
				staminaMultiplier = dragging ? 2.0 : 1.0;
				break;
		}

		if (!CurrentSpeeds.ContainsKey(movingPosition))
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = "Your position prevents you from moving. You should stand up first."
			};
		}

		var staminaCost = StaminaForMovement(movingPosition, CurrentSpeeds[movingPosition], staminaMultiplier,
			ignoreTerrainStamina);
		if (RidingMount is null && !EffectHandler.AffectedBy<IImmwalkEffect>() &&
		Race.RaceUsesStamina &&
		!CanSpendStamina(staminaCost))
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = Gameworld.GetStaticBool("ShowStaminaCostInExhaustedMessage")
					? $"You are too exhausted to move ({staminaCost.ToString("N2", this).ColourValue()} stamina required)."
					: "You are too exhausted to move."
			};
		}

		if (Body.ExternalItems.Any(x => x.PreventsMovement()))
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = $"You cannot move because {Body.ExternalItems.First(x => x.PreventsMovement()).WhyPreventsMovement(this)}"
			};
		}

		if ((PositionState == PositionProne.Instance ||
			 movingPosition.TransitionOnMovement == PositionProne.Instance) &&
			!Body.Limbs.Any(x =>
				x.LimbType.In(LimbType.Leg, LimbType.Arm, LimbType.Appendage, LimbType.Wing) &&
				Body.CanUseLimb(x) == CanUseLimbResult.CanUse)
		   )
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = $"You need at least one working arm, leg, limb or other appendage to crawl."
			};
		}

		if ((PositionState == PositionProstrate.Instance ||
			 movingPosition.TransitionOnMovement == PositionProstrate.Instance) &&
			Body.Limbs.Count(x => x.LimbType == LimbType.Leg && Body.CanUseLimb(x) == CanUseLimbResult.CanUse) <
			Body.Prototype.MinimumLegsToStand)
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = $"All of your {Body.Prototype.LegDescriptionPlural} need to be working in order for you to shuffle."
			};
		}

		if (movingPosition == PositionClimbing.Instance && !Body.Limbs.Any(x =>
				x.LimbType.In(LimbType.Leg, LimbType.Arm, LimbType.Appendage, LimbType.Wing) &&
				Body.CanUseLimb(x) == CanUseLimbResult.CanUse)
		   )
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = "You need at least one working arm, leg, limb or other appendage to climb."
			};
		}

		var args = new PerceivableRejectionResponse();
		OnWantsToMove?.Invoke(this, args);

		if (args.Rejected)
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = args.Reason
			};
		}

		return CanMoveResponse.True;
	}

	public CanMoveResponse CanMove(ICellExit exit, CanMoveFlags flags)
	{
		var response = CanMove(flags);
		if (!response.Result)
		{
			return response;
		}

		if (exit.Exit.MaximumSizeToEnter < Body.CurrentContextualSize(SizeContext.CellExit))
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = $"Only something of size {exit.Exit.MaximumSizeToEnter.Describe().Colour(Telnet.Green)} or smaller can use that exit, and you are size {Body.CurrentContextualSize(SizeContext.CellExit).Describe().Colour(Telnet.Green)}."
			};
		}

		if (exit.Exit.MaximumSizeToEnterUpright < Body.CurrentContextualSize(SizeContext.CellExit) &&
			PositionState.Upright && PositionState != PositionFlying.Instance &&
			PositionState != PositionSwimming.Instance && PositionState != PositionClimbing.Instance)
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = $"Only something of size {exit.Exit.MaximumSizeToEnterUpright.Describe().Colour(Telnet.Green)} or smaller can use that exit while standing up, and you are size {Body.CurrentContextualSize(SizeContext.CellExit).Describe().Colour(Telnet.Green)}. Consider crawling."
			};
		}

		if (exit.IsFlyExit && PositionState != PositionFlying.Instance)
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = "You cannot move in that direction unless you can fly."
			};
		}

		if (exit.IsClimbExit && !Race.CanClimb)
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = "Your kind are not built for climbing."
			};
		}

		if (!flags.HasFlag(CanMoveFlags.IgnoreSafeMovement))
		{
			switch (exit.MovementTransition(this).TransitionType)
			{
				case CellMovementTransition.TreesToTrees:
					break;
				case CellMovementTransition.FlyOnly:
					if (PositionState == PositionFlying.Instance)
					{
						break;
					}

					if (exit.IsClimbExit)
					{
						if (PositionState == PositionClimbing.Instance)
						{
							break;
						}
						return new CanMoveResponse
						{
							Result = false,
							ErrorMessage = "You must climb to use that exit."
						};
					}

					return new CanMoveResponse
					{
						Result = false,
						ErrorMessage = "You must be able to fly to move in that direction."
					};

				case CellMovementTransition.SwimOnly:
					if (PositionState != PositionSwimming.Instance)
					{
						return new CanMoveResponse
						{
							Result = false,
							ErrorMessage = Gameworld.GetCheck(CheckType.SwimStayAfloatCheck).WouldBeAbjectFailure(this) ?
								$"You would begin swimming if you moved in that direction.\nYou must append ! to your movement command to move anyway due to your safe movement settings.\n{"WARNING: You are unable to swim. If you swim into the water, you will probably drown.".Colour(Telnet.BoldRed).Blink()}" :
								"You would begin swimming if you moved in that direction.\nYou must append ! to your movement command to move anyway due to your safe movement settings."
						};
					}

					break;
				case CellMovementTransition.FallExit:
					break;
				case CellMovementTransition.SwimToLand:
					break;
			}
		}

		return CanMoveResponse.True;
	}

	public (bool Success, IPositionState MovingState, IMoveSpeed Speed) CouldMove(bool ignoreBlockingEffects,
		IPositionState fixedPosition)
	{
		if (!ignoreBlockingEffects && Effects.Any(x => x.IsBlockingEffect("movement")))
		{
			_cannotMoveReason =
				$"You cannot move because you are {Effects.First(x => x.IsBlockingEffect("movement")).BlockingDescription("movement", this)}.";
			return (false, null, null);
		}

		var args = new PerceivableRejectionResponse();
		OnWantsToMove?.Invoke(this, args);

		if (args.Rejected)
		{
			_cannotMoveReason = args.Reason;
			return (false, null, null);
		}

		if (Body.ExternalItems.Any(x => x.PreventsMovement()))
		{
			_cannotMoveReason =
				$"You cannot move because {Body.ExternalItems.First(x => x.PreventsMovement()).WhyPreventsMovement(this)}";
			return (false, null, null);
		}

		IPositionState movingPosition;
		if (fixedPosition != null)
		{
			movingPosition = fixedPosition;
		}
		else
		{
			switch (PositionState)
			{
				case PositionSwimming _:
				case PositionClimbing _:
				case PositionFlying _:
					movingPosition = PositionState;
					break;
				default:
					movingPosition = PositionState.TransitionOnMovement ?? PositionState;
					break;
			}
		}

		if (!CurrentSpeeds.ContainsKey(movingPosition))
		{
			_cannotMoveReason = "Your position prevents you from moving. You should stand up first.";
			return (false, null, null);
		}

		(bool Success, IMoveSpeed FastestMoveSpeed) CanMovePositionGeneralLogic(IPositionState state)
		{
			if (EffectHandler.AffectedBy<IImmwalkEffect>())
			{
				return (true, CurrentSpeeds[state]);
			}

			if (Race.RaceUsesStamina)
			{
				var speed = Body.Speeds.Where(x => x.Position == state)
								.Where(x => CanSpendStamina(StaminaForMovement(state, x, 1.0, false)))
								.OrderBy(x => x.Multiplier)
								.FirstOrDefault();
				if (speed == null)
				{
					return (false, null);
				}

				return (true, speed);
			}

			var cspeed = Body.Speeds.Where(x => x.Position == state).OrderByDescending(x => x.Multiplier)
							 .FirstOrDefault();
			if (cspeed == null)
			{
				return (false, null);
			}

			return (true, cspeed);
		}

		(bool Success, IMoveSpeed FastestMoveSpeed) CanMoveStanding()
		{
			if ((PositionState.TransitionOnMovement ?? PositionState) != PositionStanding.Instance &&
				!CanMovePosition(PositionStanding.Instance, PositionModifier.None, null, true))
			{
				return (false, null);
			}

			return CanMovePositionGeneralLogic(PositionStanding.Instance);
		}

		(bool Success, IMoveSpeed FastestMoveSpeed) CanMoveProne()
		{
			var general = CanMovePositionGeneralLogic(PositionProne.Instance);
			if (!general.Success)
			{
				return (false, null);
			}

			if (!Body.Limbs.Any(x =>
					x.LimbType.In(LimbType.Arm, LimbType.Appendage, LimbType.Leg, LimbType.Wing) &&
					Body.CanUseLimb(x) == CanUseLimbResult.CanUse))
			{
				_cannotMoveReason = $"You need at least one working arm or leg to crawl.";
				return (false, null);
			}

			if ((PositionState.TransitionOnMovement ?? PositionState) != PositionProne.Instance &&
				!CanMovePosition(PositionProne.Instance, PositionModifier.None, null, true))
			{
				return (false, null);
			}

			return general;
		}

		(bool Success, IMoveSpeed FastestMoveSpeed) CanMoveProstrate()
		{
			var general = CanMovePositionGeneralLogic(PositionProne.Instance);
			if (!general.Success)
			{
				return (false, null);
			}

			if (Body.Limbs.Count(x => x.LimbType.In(LimbType.Leg) && Body.CanUseLimb(x) == CanUseLimbResult.CanUse) <
				Body.Prototype.MinimumLegsToStand)
			{
				return (false, null);
			}

			if ((PositionState.TransitionOnMovement ?? PositionState) != PositionProstrate.Instance &&
				!CanMovePosition(PositionProstrate.Instance, PositionModifier.None, null, true))
			{
				return (false, null);
			}

			return general;
		}

		IMoveSpeed fastestspeed;
		bool success;
		switch (movingPosition)
		{
			case PositionStanding _:
				(success, fastestspeed) = CanMoveStanding();
				return (success, PositionStanding.Instance, fastestspeed);
			case PositionProstrate _:
				(success, fastestspeed) = CanMoveProstrate();
				return (success, PositionStanding.Instance, fastestspeed);
			case PositionProne _:
				(success, fastestspeed) = CanMoveProne();
				return (success, PositionStanding.Instance, fastestspeed);
			default:
				return (false, null, null);
		}
	}

	private IMovement _movement;

	public IMovement Movement
	{
		get => _movement;
		set
		{
			var old = _movement;
			_movement = value;
			if (old is not null && old != value)
			{
				old.CancelForMoverOnly(this, false);
			}
			if (value != null)
			{
				SetEmote(null);
			}
		}
	}

	public double MoveSpeed(ICellExit exit)
	{
		var (transition, _) = exit?.MovementTransition(this) ??
							  (CellMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
		IMoveSpeed speed;
		switch (transition)
		{
			case CellMovementTransition.SwimOnly:
			case CellMovementTransition.SwimToLand:
				speed = Body.CurrentSpeeds[PositionSwimming.Instance];
				break;
			default:
				if (!Body.CurrentSpeeds.ContainsKey(PositionState))
				{
					return -1;
				}

				speed = Body.CurrentSpeeds[PositionState];
				break;
		}

		if (speed == null)
		{
			return -1;
		}

		var sightMultiplier = 1.0;
		switch (IlluminationSightDifficulty())
		{
			case Difficulty.Impossible:
				sightMultiplier = 3.0;
				break;
			case Difficulty.ExtremelyHard:
				sightMultiplier = 2.0;
				break;
			case Difficulty.VeryHard:
				sightMultiplier = 1.6;
				break;
			case Difficulty.Hard:
				sightMultiplier = 1.2;
				break;
		}

		BaseSpeedExpression.Formula.Parameters["encumbrance"] = (int)Encumbrance;
		BaseSpeedExpression.Formula.Parameters["encpercent"] = EncumbrancePercentage;
		var baseSpeed = BaseSpeedExpression.Evaluate(this);
		var woundPenalty = WoundPenaltyToMoveSpeedPenalty * Body.HealthStrategy.WoundPenaltyFor(this);

		var aidePenalty = 1.0;
		if (PositionState.Upright && !Body.CanStand(true))
		{
			var armourUseDifficulty = ArmourUseCheckDifficulty(ArmourUseDifficultyContext.AidedWalking);
			Gameworld.GetCheck(CheckType.ArmourUseCheck).Check(this, armourUseDifficulty);
			var check = Gameworld.GetCheck(CheckType.CrutchWalking);
			var crutchResult = check.Check(this, armourUseDifficulty);
			switch (crutchResult.Outcome)
			{
				case Outcome.MajorFail:
					aidePenalty = 2.8;
					break;
				case Outcome.Fail:
					aidePenalty = 2.3;
					break;
				case Outcome.MinorFail:
					aidePenalty = 1.9;
					break;
				case Outcome.MinorPass:
					aidePenalty = 1.6;
					break;
				case Outcome.Pass:
					aidePenalty = 1.4;
					break;
				case Outcome.MajorPass:
					aidePenalty = 1.25;
					break;
			}
		}

		baseSpeed += woundPenalty;
		baseSpeed += Dice.Roll(1, 100); // Inject a little bit of randomness to break up close ties

		var meritMultiplier =
			Merits.OfType<IMovementSpeedMerit>()
				  .Where(x => x.Applies(this))
				  .Select(x => x.SpeedMultiplier(speed))
				  .Aggregate(1.0, (x, y) => x * y);

		var multiplier = (exit?.Exit.TimeMultiplier ?? 1.0) +
		                 meritMultiplier +
		                 speed.Multiplier +
		                 sightMultiplier +
		                 Location.Terrain(this).MovementRate +
		                 aidePenalty -
		                 5.0;
		if (multiplier < 0)
		{
			multiplier = 0;
		}

		var result = baseSpeed * multiplier;
		return result;
	}

	public bool Move(string rawInput)
	{
		var ss = new StringStack(rawInput);
		var direction = ss.Pop();
		var force = false;
		if (ss.Peek().EqualTo("!"))
		{
			force = true;
			ss.Pop();
		}

		var target = ss.PopSafe();

		var emote = new PlayerEmote(ss.PopParentheses(), this);

		if (!emote.Valid)
		{
			OutputHandler.Send(emote.ErrorMessage);
			QueuedMoveCommands.Clear();
			return false;
		}

		// Check non-cardinal exits.
		if (Move(direction, target, emote, force))
		{
			return true;
		}

		// Check cardinal exits next
		if (Constants.CardinalDirectionStringToDirection.ContainsKey(direction))
		{
			var targetDirection = Constants.CardinalDirectionStringToDirection[direction];
			if (Move(targetDirection, emote, force))
			{
				return true;
			}
		}

		OutputHandler.Send(WhyCannotMove());
		QueuedMoveCommands.Clear();
		return false;
	}

	public bool Move(CardinalDirection direction, IEmote emote = null, bool ignoreSafeMovement = false)
	{
		var exit = Location.GetExit(direction, this);
		if (exit == null || !Body.CanSee(Location, exit))
		{
			_cannotMoveReason = "You cannot move in that direction.";
			return false;
		}

		return Move(exit, emote, ignoreSafeMovement);
	}

	public bool Move(string cmd, string target, IEmote emote = null, bool ignoreSafeMovement = false)
	{
		var exit = Location.GetExit(cmd, target, this);
		if (exit == null || !Body.CanSee(Location, exit))
		{
			_cannotMoveReason = "You cannot move in that direction.";
			return false;
		}

		return Move(exit, emote, ignoreSafeMovement);
	}

	private string _cannotMoveReason = "You cannot move.";

	public string WhyCannotMove()
	{
		return CanMove(CanMoveFlags.None).ErrorMessage;
	}

	protected void TargetMoved(object sender, MoveEventArgs args)
	{
		if (ColocatedWith(Following) && CanSee(Following) && Body.CanSee(Location, args.Movement.Exit))
		{
			// TODO - check trespassing
			if (Movement != null && Movement.Exit == args.Movement.Exit)
			{
				return;
			}

			Move(args.Movement.Exit);
		}
	}

	protected void TargetStopped(object sender, MoveEventArgs args)
	{
		if (Movement?.IsMovementLeader(this) != true)
		{
			return;
		}

		if (Movement.Phase == MovementPhase.NewRoom && !QueuedMoveCommands.Any())
		{
			return;
		}

		if (Movement.Phase == MovementPhase.NewRoom)
		{
			QueuedMoveCommands.Clear();
			return;
		}

		Movement.StopMovement();
		QueuedMoveCommands.Clear();
	}

	public IMove Following { get; protected set; }

	public void Follow(IMove thing)
	{
		CeaseFollowing();
		Following = thing;
		if (thing is not null)
		{
			thing.OnStartMove += TargetMoved;
			thing.OnStopMove += TargetStopped;
			thing.OnQuit += FollowingThingDisappeared;
		}
	}

	public void CeaseFollowing()
	{
		if (Following != null)
		{
			Following.OnStartMove -= TargetMoved;
			Following.OnStopMove -= TargetStopped;
			Following.OnQuit -= FollowingThingDisappeared;
			Following = null;
		}
	}

	private void FollowingThingDisappeared(IPerceivable thing)
	{
		CeaseFollowing();
	}

	public string DisplayInGroup(IPerceiver voyeur, int indent = 0)
	{
		return new string(' ', indent) + HowSeen(voyeur);
	}

	public override bool ShouldFall()
	{
		if (RidingMount != null)
		{
			return false;
		}

		if (EffectHandler.AffectedBy<IImmwalkEffect>())
		{
			return false;
		}

		if (!RoomLayer.IsHigherThan(RoomLayer.GroundLevel))
		{
			return false;
		}

		if (EffectsOfType<Dragging.DragTarget>().Any())
		{
			return false;
		}

		if (PositionState?.SafeFromFalling == true)
		{
			return false;
		}

		if (!State.IsAble())
		{
			return true;
		}

		return true;
	}

	public void ExecuteMove(IMovement movement, IMoveSpeed speedOverride = null)
	{
		if (RidingMount is null && !EffectHandler.AffectedBy<IImmwalkEffect>())
		{
			SpendStamina(StaminaForMovement(PositionState, speedOverride ?? CurrentSpeed, movement.StaminaMultiplier, PositionState.IgnoreTerrainStaminaCostsForMovement));
		}

		movement?.Exit?.Origin?.Leave(this);
		var originalLayer = RoomLayer;
		var (transition, targetLayer) = movement?.Exit?.MovementTransition(this) ??
										(CellMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
		var destinationTerrain = movement?.Exit?.Destination.Terrain(this);
		if (destinationTerrain?.TerrainLayers.Contains(targetLayer) == false)
		{
			if (destinationTerrain.TerrainLayers.LowestLayer().IsHigherThan(RoomLayer))
			{
				targetLayer = destinationTerrain.TerrainLayers.LowestLayer();
			}
			else
			{
				targetLayer = destinationTerrain.TerrainLayers.Where(x => x.IsLowerThan(RoomLayer)).HighestLayer();
			}
		}

		Moved(movement);
		movement?.Exit?.Destination?.Enter(this, movement.Exit, roomLayer: targetLayer);

		foreach (var rider in Riders)
		{
			movement?.Exit?.Origin?.Leave(rider);
			rider.RoomLayer = targetLayer;
			movement?.Exit?.Destination?.Enter(rider, movement.Exit, roomLayer: targetLayer);
			rider.Moved(movement);
		}

		switch (transition)
		{
			case CellMovementTransition.TreesToTrees:
				if (AffectedBy<Immwalk>())
				{
					break;
				}
				var check = Gameworld.GetCheck(CheckType.ClimbTreetoTreeCheck);
				var weather = Location.CurrentWeather(this);
				var result = check.CheckAgainstAllDifficulties(this, Difficulty.VeryEasy, null,
					externalBonus:
					(weather?.Precipitation.PrecipitationClimbingBonus() ?? 0.0) +
					(weather?.Wind.WindClimbingBonus() ?? 0.0));
				if (result[Difficulty.VeryEasy].Outcome.IsFail())
				{
					OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("FallFromTreesWhenMovingRoomToRoom"), this, this)));
					RemoveAllEffects<HideInvis>(fireRemovalAction: true);
					FallToGround();
					Body.Look(true);
				}
				break;
			case CellMovementTransition.FlyOnly:
				if (!PositionState.SafeFromFalling && CanFly().Truth)
				{
					MovePosition(PositionFlying.Instance, PositionModifier.None, null, null);
					RemoveAllEffects<HideInvis>(fireRemovalAction: true);
				}

				break;
			case CellMovementTransition.SwimOnly:
				if (PositionState != PositionSwimming.Instance)
				{
					SetPosition(PositionSwimming.Instance, PositionModifier.None, null, null);
				}

				break;
			case CellMovementTransition.FallExit:
				if (!PositionState.SafeFromFalling && CanFly().Truth)
				{
					MovePosition(PositionFlying.Instance, PositionModifier.None, null, null);
					RemoveAllEffects<HideInvis>(fireRemovalAction: true);
				}

				break;
			case CellMovementTransition.SwimToLand:
				SetPosition(PositionSwimming.Instance, PositionModifier.None, null, null);
				break;
		}

		foreach (var character in movement?.Exit?.Origin?.Characters.Where(x => x.Movement != movement) ??
								  Enumerable.Empty<ICharacter>())
		{
			character.RemoveAllEffects(x => x.GetSubtype<ISawHiderEffect>()?.Hider == this);
		}

		InvalidatePositionTargets();
		EffectHandler.RemoveAllEffects(x => x.IsEffectType<ISawHiderEffect>());
		var hideEffect = EffectsOfType<HideInvis>().FirstOrDefault();
		if (hideEffect != null)
		{
			hideEffect.EffectiveHideSkill = Gameworld.GetCheck(CheckType.HideCheck)
													 .Check(this, Location.Terrain(this).HideDifficulty).TargetNumber;
		}

		AcquireTarget();
		ForceAcquireTargetCheckForOpponents();
		Changed = true;
	}

	public Difficulty ArmourUseCheckDifficulty(ArmourUseDifficultyContext context)
	{
		var baseDifficulty = Difficulty.Automatic;
		switch (Encumbrance)
		{
			case EncumbranceLevel.LightlyEncumbered:
				baseDifficulty = Difficulty.ExtremelyEasy;
				break;
			case EncumbranceLevel.ModeratelyEncumbered:
				baseDifficulty = Difficulty.VeryEasy;
				break;
			case EncumbranceLevel.HeavilyEncumbered:
				baseDifficulty = Difficulty.Easy;
				break;
			case EncumbranceLevel.CriticallyEncumbered:
				baseDifficulty = Difficulty.Normal;
				break;
		}

		switch (context)
		{
			case ArmourUseDifficultyContext.Flying:
			case ArmourUseDifficultyContext.Climbing:
			case ArmourUseDifficultyContext.Swimming:
				baseDifficulty = baseDifficulty.StageUp(2);
				break;
			case ArmourUseDifficultyContext.AidedWalking:
				baseDifficulty = baseDifficulty.StageUp(1);
				break;
		}

		return baseDifficulty;
	}

	#endregion

	#region IHaveStamina Members

	public double MaximumStamina => Body.MaximumStamina;

	public double CurrentStamina
	{
		get => Body.CurrentStamina;
		set => Body.CurrentStamina = value;
	}

	public ExertionLevel CurrentExertion => Body.CurrentExertion;

	public ExertionLevel LongtermExertion => Body.LongtermExertion;

	public void StaminaTenSecondHeartbeat()
	{
		Body.StaminaTenSecondHeartbeat();
	}

	public void StaminaMinuteHeartbeat()
	{
		Body.StaminaMinuteHeartbeat();
	}

	public bool CanSpendStamina(double amount)
	{
		return Body.CanSpendStamina(amount);
	}

	public void GainStamina(double amount)
	{
		Body.GainStamina(amount);
	}

	public void SpendStamina(double amount)
	{
		Body.SpendStamina(amount);
	}

	public EncumbranceLevel Encumbrance => Body.Encumbrance;

	public double EncumbrancePercentage => Body.EncumbrancePercentage;

	public void SetExertion(ExertionLevel level)
	{
		Body.SetExertion(level);
	}

	public void SetExertionToMinimumLevel(ExertionLevel level)
	{
		Body.SetExertionToMinimumLevel(level);
	}

	public void InitialiseStamina()
	{
		Body.InitialiseStamina();
	}

	public event ExertionEvent OnExertionChanged
	{
		add => Body.OnExertionChanged += value;
		remove => Body.OnExertionChanged -= value;
	}

	#endregion

	private (bool Truth, string Error) CheckGeneralMovementRestrictions(string actionVerb)
	{
		if (!CharacterState.Able.HasFlag(State))
		{
			return (false, $"You cannot {actionVerb} because you are {State.Describe()}.");
		}

		if (IsEngagedInMelee)
		{
			return (false, $"You must first escape melee combat before you can {actionVerb}.");
		}

		if (Movement != null)
		{
			return (false, $"You must finish moving before you can {actionVerb}.");
		}

		var blockers = CombinedEffectsOfType<IEffect>().Where(x => x.IsBlockingEffect("move")).ToList();
		if (blockers.Any())
		{
			return (false,
				$"You must first stop {blockers.Select(x => x.BlockingDescription("move", this)).ListToString()} before you can {actionVerb}.");
		}

		var itemBlockers = Body.AllItems.Where(x => x.PreventsMovement()).Select(x => x.WhyPreventsMovement(this))
							   .ToList();
		if (itemBlockers.Any())
		{
			return (false, $"You cannot {actionVerb} because {itemBlockers.ListToString()}.");
		}

		return (true, string.Empty);
	}

	#region IFly Members

	public TimeSpan FlyDelay => TimeSpan.FromSeconds(5); // TODO

	public (bool Truth, string Error) CanFly()
	{
		var (truth, error) = CheckGeneralMovementRestrictions("fly");
		if (!truth)
		{
			return (false, error);
		}

		if (Location.IsUnderwaterLayer(RoomLayer))
		{
			return (false, "You cannot fly when you are underwater. That's called swimming.");
		}

		if (PositionState == PositionFlying.Instance)
		{
			return (false, "You are already flying.");
		}

		if (CombinedEffectsOfType<IImmwalkEffect>().Any())
		{
			return (true, string.Empty);
		}

		var wings = Body.Bodyparts.Where(x => x is WingProto).ToList();
		var workingWings = wings.Where(x => Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse)
								.Select(x => Body.GetLimbFor(x)).Distinct().Count();
		if (Body.Prototype.MinimumWingsToFly <= workingWings)
		{
			return (true, string.Empty);
		}

		// TODO - more ways people could fly

		if (wings.Any())
		{
			return (false, $"You need {Body.Prototype.MinimumWingsToFly} working wings to fly.");
		}


		return (false, "You weren't made to fly.");
	}

	public void Fly(IEmote actionEmote = null)
	{
		if (RidingMount is not null && RidingMount.IsPrimaryRider(this))
		{
			RidingMount.RiderFly(this, actionEmote);
			return;
		}

		var (truth, error) = CanFly();
		if (!truth)
		{
			OutputHandler.Send(error);
			return;
		}

		MovePosition(PositionFlying.Instance, PositionModifier.None, null, actionEmote, null);
		if (!AffectedBy<Immwalk>())
		{
			AddEffect(new BlockLayerChange(this), FlyDelay);
		}

		RemoveAllEffects<HideInvis>(fireRemovalAction: true);
	}

	public (bool Truth, string Error) CanLand()
	{
		if (PositionState != PositionFlying.Instance)
		{
			return (false, "You are not flying.");
		}

		var (truth, error) = CheckGeneralMovementRestrictions("land");
		if (!truth)
		{
			return (false, error);
		}

		return (true, string.Empty);
	}

	public void Land(IEmote actionEmote = null)
	{
		if (RidingMount is not null && RidingMount.IsPrimaryRider(this))
		{
			RidingMount.RiderMovePosition(Location.IsSwimmingLayer(RoomLayer) ? PositionSwimming.Instance : PositionStanding.Instance,
					PositionModifier.None, null, this, actionEmote, null);
			return;
		}

		var (truth, error) = CanLand();
		if (!truth)
		{
			OutputHandler.Send(error);
			return;
		}

		if (Location.IsSwimmingLayer(RoomLayer))
		{
			MovePosition(PositionSwimming.Instance, PositionModifier.None, null, actionEmote, null);
			return;
		}

		MovePosition(PositionStanding.Instance, PositionModifier.None, null, actionEmote, null);
		if (!AffectedBy<Immwalk>())
		{
			AddEffect(new BlockLayerChange(this), FlyDelay);
		}
	}

	public double FlyStaminaMultiplier()
	{
		var multiplier = 1.0;
		var check = Gameworld.GetCheck(CheckType.FlyCheck);
		Difficulty difficulty;
		switch (Encumbrance)
		{
			case EncumbranceLevel.LightlyEncumbered:
				difficulty = Difficulty.ExtremelyEasy;
				break;
			case EncumbranceLevel.ModeratelyEncumbered:
				difficulty = Difficulty.VeryEasy;
				break;
			case EncumbranceLevel.HeavilyEncumbered:
				difficulty = Difficulty.Easy;
				break;
			case EncumbranceLevel.CriticallyEncumbered:
				difficulty = Difficulty.Normal;
				break;
			default:
				difficulty = Difficulty.Trivial;
				break;
		}

		switch (Location.CurrentWeather(this)?.Wind ?? WindLevel.None)
		{
			case WindLevel.Still:
			case WindLevel.OccasionalBreeze:
				difficulty = difficulty.StageUp(1);
				break;
			case WindLevel.Breeze:
			case WindLevel.Wind:
				difficulty = difficulty.StageUp(2);
				break;
			case WindLevel.StrongWind:
				difficulty = difficulty.StageUp(3);
				break;
			case WindLevel.GaleWind:
				difficulty = difficulty.StageUp(4);
				break;
			case WindLevel.HurricaneWind:
				difficulty = difficulty.StageUp(5);
				break;
			case WindLevel.MaelstromWind:
				difficulty = difficulty.StageUp(6);
				break;
		}

		var outcome = check.Check(this, difficulty);
		if (outcome.TargetNumber <= 0)
		{
			return 1000;
		}

		multiplier /= (outcome.TargetNumber / 100.0);
		return multiplier;
	}

	public double FlyStaminaCost()
	{
		return
			FlyStaminaMultiplier() *
			Merits.OfType<FlyingStaminaMerit>().Aggregate(1.0, (prev, merit) => prev * merit.Multiplier) *
			Gameworld.GetStaticDouble("FlyStaminaCost") *
			(
				EffectsOfType<Dragging>().Any()
				? 1.0 + ((IHaveWeight)EffectsOfType<Dragging>().First().Target).Weight / Weight
				: 1.0
			);
	}

	public double FlyStaminaCostPerTick()
	{
		return FlyStaminaMultiplier() *
			   Merits.OfType<FlyingStaminaMerit>().Aggregate(1.0, (prev, merit) => prev * merit.Multiplier) *
			   Gameworld.GetStaticDouble("FlyStaminaCostPerTick") *
			   (
				   EffectsOfType<Dragging>().Any()
				   ? 1.0 + ((IHaveWeight)EffectsOfType<Dragging>().First().Target).Weight / Weight
				   : 1.0
			   );
	}

	public void CheckCanFly()
	{
		if (PositionState != PositionFlying.Instance)
		{
			return;
		}

		if (!CanFly().Truth)
		{
			OutputHandler.Handle(new EmoteOutput(new Emote("@ can no longer fly!", this),
				flags: OutputFlags.SuppressObscured));
			PositionState = PositionSprawled.Instance;
			return;
		}
	}

	public void DoFlyHeartbeat()
	{
		if (CombinedEffectsOfType<IImmwalkEffect>().Any())
		{
			return;
		}

		if (PositionState != PositionFlying.Instance)
		{
			return;
		}

		var staminaCost = FlyStaminaCostPerTick();

		if (CanSpendStamina(staminaCost))
		{
			SpendStamina(staminaCost);
			OutputHandler.Send("", false);
			return;
		}

		SpendStamina(staminaCost);
		OutputHandler.Handle(new EmoteOutput(new Emote("Exhausted, @ can fly no more.", this, this)));
		SetPosition(PositionSprawled.Instance, PositionModifier.None, null, null);
	}

	(bool Truth, string Error) IFly.CanAscend()
	{
		var (truth, error) = CheckGeneralMovementRestrictions("ascend");
		if (!truth)
		{
			return (false, error);
		}

		var terrain = Location.Terrain(this);
		if (terrain.TerrainLayers.HighestLayer() == RoomLayer)
		{
			if (!Location.ExitsFor(this).Any(x => x.OutboundDirection == CardinalDirection.Up))
			{
				return (false, "There is nowhere higher to ascend; you're as high as you can go.");
			}

			return (true, string.Empty);
		}

		var dragging = CombinedEffectsOfType<Dragging>().FirstOrDefault();
		if (dragging != null &&
			MaximumDragWeight * (dragging?.Draggers.First(x => x.Character == this).Aid?.EffortMultiplier ?? 1.0) -
			Body.ExternalItems.Sum(x => x.Weight) - ((IHaveWeight)dragging.Target).Weight < 0.0)
		{
			return (false, $"You aren't strong enough to lift {dragging.Target.HowSeen(this)} with you.");
		}

		if (!CanSpendStamina(FlyStaminaCost()))
		{
			return (false,
				Gameworld.GetStaticBool("ShowStaminaCostInExhaustedMessage")
					? $"You are too exhausted to ascend ({FlyStaminaCost().ToString("N2", this).ColourValue()} stamina required)."
					: "You are too exhausted to ascend.");
		}

		return (true, string.Empty);
	}

	(bool Truth, string Error) IFly.CanDive()
	{
		var (truth, error) = CheckGeneralMovementRestrictions("dive");
		if (!truth)
		{
			return (false, error);
		}

		var terrain = Location.Terrain(this);
		if (terrain.TerrainLayers.LowestLayer() == RoomLayer)
		{
			if (!Location.ExitsFor(this).Any(x => x.OutboundDirection == CardinalDirection.Down))
			{
				return (false, "There is nowhere lower to dive; you've reached the bottom.");
			}

			return (true, string.Empty);
		}

		if (Location.IsSwimmingLayer(RoomLayer))
		{
			return (false,
				"You would descend into the water if you dive any lower; you must land if you want to do so.");
		}

		return (true, string.Empty);
	}

	void IFly.Ascend(IEmote actionEmote)
	{
		if (RidingMount is not null && RidingMount.IsPrimaryRider(this))
		{
			RidingMount.RiderAscend(this, actionEmote);
			return;
		}

		ConvertGrapplesToDrags();
		var (can, error) = ((IFly)this).CanAscend();
		if (!can)
		{
			OutputHandler.Send(error);
			return;
		}

		var terrain = Location.Terrain(this);
		if (terrain.TerrainLayers.HighestLayer() == RoomLayer)
		{
			Move(CardinalDirection.Up, actionEmote, true);
			return;
		}

		SpendStamina(FlyStaminaCost());
		var higherLayers = terrain.TerrainLayers.Where(x => x.IsHigherThan(RoomLayer)).ToList();
		var desired = higherLayers.LowestLayer();
		var emote = new Emote(Gameworld.GetStaticString("AscendFlyEmote"), this, this);
		var breaching = RoomLayer.In(RoomLayer.InTrees, RoomLayer.HighInTrees) &&
						desired.In(RoomLayer.InAir, RoomLayer.HighInAir);
		if (breaching)
		{
			emote = new Emote(Gameworld.GetStaticString("AscendFlyEmoteLeaveTrees"), this, this);
		}
		else if (RoomLayer.In(RoomLayer.GroundLevel, RoomLayer.OnRooftops) &&
				 desired.In(RoomLayer.InTrees, RoomLayer.HighInTrees))
		{
			emote = new Emote(Gameworld.GetStaticString("AscendFlyEmoteEnterTrees"), this, this);
		}

		var targetEmote =
			new Emote(
				Gameworld.GetStaticString(breaching
					? "AscendFlyEmoteTargetLocationLeaveTrees"
					: "AscendFlyEmoteTargetLocation"), this, this);
		OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.SuppressObscured));
				RoomLayer = desired;
			   if (EffectsOfType<Dragging>().FirstOrDefault()?.Target is IPerceiver dragTarget)
			   {
					   dragTarget.RoomLayer = RoomLayer;
					   (dragTarget as ICharacter)?.Body.Look(true);
			   }


			   foreach (var rider in Riders)
			   {
					   rider.RoomLayer = RoomLayer;
					   rider.Body.Look(true);
			   }

		OutputHandler.Handle(new EmoteOutput(targetEmote,
			flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
		if (!AffectedBy<Immwalk>())
		{
			AddEffect(new BlockLayerChange(this), FlyDelay);
		}

		Body.Look(true);
	}

	void IFly.Dive(IEmote actionEmote)
	{
		if (RidingMount is not null && RidingMount.IsPrimaryRider(this))
		{
			RidingMount.RiderDive(this, actionEmote);
			return;
		}

		ConvertGrapplesToDrags();
		var (can, error) = ((IFly)this).CanDive();
		if (!can)
		{
			OutputHandler.Send(error);
			return;
		}

		var terrain = Location.Terrain(this);
		if (terrain.TerrainLayers.LowestLayer() == RoomLayer)
		{
			Move(CardinalDirection.Down, actionEmote, true);
			return;
		}

		var lowerLayers = terrain.TerrainLayers.Where(x => x.IsLowerThan(RoomLayer)).ToList();
		var desired = lowerLayers.HighestLayer();
		var emote = new Emote(Gameworld.GetStaticString("DiveFlyEmote"), this, this);
		var leavingTrees = RoomLayer.In(RoomLayer.InTrees, RoomLayer.HighInTrees) &&
						   desired.In(RoomLayer.GroundLevel, RoomLayer.OnRooftops);
		var enteringTrees = RoomLayer.In(RoomLayer.InAir, RoomLayer.HighInAir) &&
							desired.In(RoomLayer.InTrees, RoomLayer.HighInTrees);
		if (leavingTrees)
		{
			emote = new Emote(Gameworld.GetStaticString("DiveFlyEmoteLeaveTrees"), this, this);
		}
		else if (enteringTrees)
		{
			emote = new Emote(Gameworld.GetStaticString("DiveFlyEmoteEnterTrees"), this, this);
		}

		var targetEmote =
			new Emote(
				Gameworld.GetStaticString(leavingTrees ? "DiveFlyEmoteTargetLocationLeaveTrees" :
					enteringTrees ? "DiveFlyEmoteTargetLocationEnterTrees" : "DiveFlyEmoteTargetLocation"), this, this);
		OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.SuppressObscured));
		RoomLayer = desired;
		if (EffectsOfType<Dragging>().FirstOrDefault()?.Target is IPerceiver dragTarget)
		{
			dragTarget.RoomLayer = RoomLayer;
			(dragTarget as ICharacter)?.Body.Look(true);
		}

		OutputHandler.Handle(new EmoteOutput(targetEmote,
			flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
		if (!AffectedBy<Immwalk>())
		{
			AddEffect(new BlockLayerChange(this), FlyDelay);
		}

		Body.Look(true);
	}

	#endregion

	#region ISwim Members

	public TimeSpan DiveDelay => TimeSpan.FromSeconds(10);
	public double SwimSpeedMultiplier => 1.0; // TODO - based on skill

	public (bool Truth, TimeSpan Delay) DoSurfaceCheck()
	{
		var staminaCost = Gameworld.GetStaticDouble("SwimSurfaceStamina");
		var check = Gameworld.GetCheck(CheckType.SwimmingCheck);
		var terrain = Location.Terrain(this);
		var inventoryBuoyancy = Body.AllItems.Sum(x => x.Buoyancy(terrain.WaterFluid.Density));
		var difficulty =
			Difficulty.ExtremelyEasy.StageDown((int)(inventoryBuoyancy /
													 Gameworld.GetStaticDouble("FloatBuoyancyPerDifficulty")));
		var outcome = check.Check(this, difficulty);
		switch (outcome.Outcome)
		{
			case Outcome.MajorFail:
				staminaCost *= 4;
				break;
			case Outcome.Fail:
				staminaCost *= 2.5;
				break;
			case Outcome.MinorFail:
				staminaCost *= 1.75;
				break;
			case Outcome.MinorPass:
				break;
			case Outcome.Pass:
				staminaCost *= 0.8;
				break;
			case Outcome.MajorPass:
				staminaCost *= 0.6;
				break;
		}

		if (outcome.IsAbjectFailure)
		{
			SpendStamina(staminaCost);
			OutputHandler.Handle(new EmoteOutput(
				new Emote("@ thrash|thrashes about, trying in vain to ascend towards the surface.", this, this)));
			return (false, TimeSpan.FromSeconds(5));
		}

		if (CanSpendStamina(staminaCost))
		{
			SpendStamina(staminaCost);
			return (true, TimeSpan.FromSeconds(5));
		}

		SpendStamina(staminaCost);
		OutputHandler.Handle(new EmoteOutput(
			new Emote("Exhausted, @ try|tries to swim towards the surface but can't make it.", this, this)));
		return (false, TimeSpan.FromSeconds(5));
	}

	public void DoSwimHeartbeat()
	{
		var terrain = Location.Terrain(this);
		if (!Location.IsSwimmingLayer(RoomLayer))
		{
			return;
		}

		if (!terrain.TerrainLayers.Any(x => x.IsLowerThan(RoomLayer)))
		{
			return;
		}

		if (CombinedEffectsOfType<IImmwalkEffect>().Any())
		{
			return;
		}

		if (!CharacterState.Able.HasFlag(State))
		{
			PositionState = PositionSprawled.Instance;
		}

		var dragTarget = EffectsOfType<Dragging>().FirstOrDefault()?.Target as IPerceiver;
		if (PositionState != PositionSwimming.Instance)
		{
			if (terrain.TerrainLayers.Any(x => x.IsLowerThan(RoomLayer)))
			{
				var emoteInsert = RoomLayer == RoomLayer.GroundLevel ? "below the surface" : "deeper underwater";
				OutputHandler.Handle(
					new EmoteOutput(new Emote($"@ sink|sinks {emoteInsert}, not even attempting to swim.", this,
						this)));
				RoomLayer = terrain.TerrainLayers.Where(x => x.IsLowerThan(RoomLayer)).FirstMax(x => x.LayerHeight());
				if (dragTarget != null)
				{
					dragTarget.RoomLayer = RoomLayer;
					(dragTarget as ICharacter)?.Body.Look(true);
				}

				OutputHandler.Handle(new EmoteOutput(
					new Emote($"@ sink|sinks {emoteInsert} from above, not even attempting to swim.", this, this),
					flags: OutputFlags.SuppressSource));
				Body.Look(true);
			}

			return;
		}

		if (Location.IsUnderwaterLayer(RoomLayer))
		{
			return;
		}

		var staminaCost = Gameworld.GetStaticDouble("SwimStayAfloatStamina");
		var check = Gameworld.GetCheck(CheckType.SwimStayAfloatCheck);
		var inventoryBuoyancy = Body.AllItems.Sum(x => x.Buoyancy(terrain.WaterFluid.Density));
		var difficulty =
			Difficulty.ExtremelyEasy.StageDown((int)(inventoryBuoyancy /
													 Gameworld.GetStaticDouble("FloatBuoyancyPerDifficulty")));
		// TODO - weather condition affecting this check
		var outcome = check.Check(this, difficulty);
		switch (outcome.Outcome)
		{
			case Outcome.MajorFail:
				staminaCost *= 4;
				break;
			case Outcome.Fail:
				staminaCost *= 2.5;
				break;
			case Outcome.MinorFail:
				staminaCost *= 1.75;
				break;
			case Outcome.MinorPass:
				break;
			case Outcome.Pass:
				staminaCost *= 0.8;
				break;
			case Outcome.MajorPass:
				staminaCost *= 0.6;
				break;
		}

		if (outcome.IsAbjectFailure)
		{
			SpendStamina(staminaCost);
			OutputHandler.Handle(new EmoteOutput(new Emote("@ sink|sinks below the surface, clearly unable to swim.",
				this, this)));
			RoomLayer = terrain.TerrainLayers.Where(x => x.IsLowerThan(RoomLayer)).HighestLayer();
			if (dragTarget != null)
			{
				dragTarget.RoomLayer = RoomLayer;
				(dragTarget as ICharacter)?.Body.Look(true);
			}

			OutputHandler.Handle(new EmoteOutput(
				new Emote("@ sink|sinks below the surface from above, clearly unable to swim.", this, this),
				flags: OutputFlags.SuppressSource));
			Body.Look(true);
			return;
		}

		if (difficulty > Difficulty.Normal)
		{
			OutputHandler.Send(
				"You are having a hard time keeping your head above the water due to the weight of your inventory.");
		}

		if (CanSpendStamina(staminaCost))
		{
			SpendStamina(staminaCost);
			OutputHandler.Send("", false);
			return;
		}

		SpendStamina(staminaCost);
		OutputHandler.Handle(new EmoteOutput(new Emote("Exhausted, @ sink|sinks below the surface.", this, this)));
		RoomLayer = terrain.TerrainLayers.Where(x => x.IsLowerThan(RoomLayer)).HighestLayer();
		if (dragTarget != null)
		{
			dragTarget.RoomLayer = RoomLayer;
			(dragTarget as ICharacter)?.Body.Look(true);
		}

		OutputHandler.Handle(new EmoteOutput(
			new Emote("Exhausted, @ sink|sinks below the surface from above.", this, this),
			flags: OutputFlags.SuppressSource));
		Body.Look(true);
	}

	public double SwimStaminaCost()
	{
		return Gameworld.GetStaticDouble("SwimStaminaCost") *
			   Merits.OfType<SwimmingStaminaMerit>().Aggregate(1.0, (prev, merit) => prev * merit.Multiplier) *
			   (EffectsOfType<Dragging>().Any()
				   ? 1.0 + ((IHaveWeight)EffectsOfType<Dragging>().First().Target).Weight / Weight
				   : 1.0);
		;
	}

	(bool Truth, string Error) ISwim.CanAscend()
	{
		if (!CharacterState.Able.HasFlag(State))
		{
			return (false, $"You cannot ascend because you are {State.Describe()}.");
		}

		if (IsEngagedInMelee)
		{
			return (false, "You must first escape melee combat before you can ascend.");
		}

		if (Movement != null)
		{
			return (false, "You must finish moving before you can ascend.");
		}

		var terrain = Location.Terrain(this);
		var higherLayers = terrain.TerrainLayers.Where(x => x.IsHigherThan(RoomLayer)).ToList();
		var desired = higherLayers.LowestLayer();
		if (!Location.IsUnderwaterLayer(RoomLayer))
		{
			return (false, "You are already at the surface, there is nowhere further to rise.");
		}

		if (!higherLayers.Any())
		{
			if (!Location.ExitsFor(this).Any(x => x.OutboundDirection == CardinalDirection.Up))
			{
				return (false, "There is nowhere higher to ascend; you've reached the top.");
			}

			return (true, string.Empty);
		}

		var dragging = CombinedEffectsOfType<Dragging>().FirstOrDefault();
		if (dragging != null &&
			(MaximumDragWeight * dragging?.Draggers.First(x => x.Character == this).Aid?.EffortMultiplier ?? 1.0) -
			Body.ExternalItems.Sum(x => x.Weight) - ((IHaveWeight)dragging.Target).Weight < 0.0)
		{
			return (false, $"You aren't strong enough to lift {dragging.Target.HowSeen(this)} with you.");
		}

		if (!CanSpendStamina(SwimStaminaCost()))
		{
			return (false,
				Gameworld.GetStaticBool("ShowStaminaCostInExhaustedMessage")
					? $"You are too exhausted to ascend ({SwimStaminaCost().ToString("N2", this).ColourValue()} stamina required)."
					: "You are too exhausted to ascend.");
		}

		var blockers = CombinedEffectsOfType<IEffect>()
					   .Where(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("move")).ToList();
		if (blockers.Any())
		{
			return (false,
				$"You must first stop {blockers.Select(x => x.IsBlockingEffect("general") ? x.BlockingDescription("general", this) : x.BlockingDescription("move", this)).ListToString()} before you can ascend.");
		}

		return (true, string.Empty);
	}

	(bool Truth, string Error) ISwim.CanDive()
	{
		if (!CharacterState.Able.HasFlag(State))
		{
			return (false, $"You cannot dive because you are {State.Describe()}.");
		}

		if (IsEngagedInMelee)
		{
			return (false, "You must first escape melee combat before you can dive.");
		}

		if (Movement != null)
		{
			return (false, "You must finish moving before you can dive.");
		}

		var terrain = Location.Terrain(this);
		if (RoomLayer == terrain.TerrainLayers.LowestLayer())
		{
			if (!Location.ExitsFor(this).Any(x => x.OutboundDirection == CardinalDirection.Down))
			{
				return (false, "There is nowhere lower to dive; you've reached the bottom.");
			}

			return (true, string.Empty);
		}

		if (!CanSpendStamina(SwimStaminaCost()))
		{
			return (false,
				Gameworld.GetStaticBool("ShowStaminaCostInExhaustedMessage")
					? $"You are too exhausted to dive ({SwimStaminaCost().ToString("N2", this).ColourValue()} stamina required)."
					: "You are too exhausted to dive.");
		}

		var blockers = CombinedEffectsOfType<IEffect>()
					   .Where(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("move")).ToList();
		if (blockers.Any())
		{
			return (false,
				$"You must first stop {blockers.Select(x => x.IsBlockingEffect("general") ? x.BlockingDescription("general", this) : x.BlockingDescription("move", this)).ListToString()} before you can dive.");
		}

		return (true, string.Empty);
	}

	void ISwim.Ascend(IEmote actionEmote)
	{
		if (RidingMount is not null && RidingMount.IsPrimaryRider(this))
		{
			RidingMount.RiderAscend(this, actionEmote);
			return;
		}

		ConvertGrapplesToDrags();
		var (can, error) = ((ISwim)this).CanAscend();
		if (!can)
		{
			OutputHandler.Send(error);
			return;
		}

		var terrain = Location.Terrain(this);
		if (terrain.TerrainLayers.HighestLayer() == RoomLayer)
		{
			Move(CardinalDirection.Up, actionEmote, true);
			return;
		}

		var (truth, delay) = DoSurfaceCheck();
		if (!AffectedBy<Immwalk>())
		{
			AddEffect(new BlockLayerChange(this), delay);
		}

		if (!truth)
		{
			return;
		}

		var higherLayers = terrain.TerrainLayers.Where(x => x.IsHigherThan(RoomLayer)).ToList();
		var desired = higherLayers.LowestLayer();
		var emote = new Emote(Gameworld.GetStaticString("AscendEmote"), this, this);
		var breaching = !Location.IsUnderwaterLayer(desired);
		if (breaching)
		{
			emote = new Emote(Gameworld.GetStaticString("AscendEmoteFromUnderwater"), this, this);
		}

		var targetEmote =
			new Emote(
				Gameworld.GetStaticString(breaching
					? "AscendEmoteTargetLocationOutOfWater"
					: "AscendEmoteTargetLocation"), this, this);
		OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.SuppressObscured));
		RoomLayer = desired;
		if (EffectsOfType<Dragging>().FirstOrDefault()?.Target is IPerceiver dragTarget)
		{
			dragTarget.RoomLayer = RoomLayer;
			(dragTarget as ICharacter)?.Body.Look(true);
		}

		OutputHandler.Handle(new EmoteOutput(targetEmote,
			flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
		Body.Look(true);
	}

	void ISwim.Dive(IEmote actionEmote)
	{
		if (RidingMount is not null && RidingMount.IsPrimaryRider(this))
		{
			RidingMount.RiderDive(this, actionEmote);
			return;
		}

		ConvertGrapplesToDrags();
		var (truth, error) = ((ISwim)this).CanDive();
		if (!truth)
		{
			OutputHandler.Send(error);
			return;
		}

		var terrain = Location.Terrain(this);
		if (terrain.TerrainLayers.LowestLayer() == RoomLayer)
		{
			Move(CardinalDirection.Down, actionEmote, true);
			return;
		}

		var desired = terrain.TerrainLayers.Where(x => x.IsLowerThan(RoomLayer)).HighestLayer();

		var alreadyUnderwater = Location.IsUnderwaterLayer(RoomLayer);
		var emote = alreadyUnderwater
			? new Emote(Gameworld.GetStaticString("DiveEmoteAlreadyUnderwater"), this, this)
			: new Emote(Gameworld.GetStaticString("DiveEmote"), this, this);
		;

		var targetEmote =
			new Emote(
				Gameworld.GetStaticString(alreadyUnderwater
					? "DiveEmoteTargetLocationAlreadyUnderwater"
					: "DiveEmoteTargetLocation"), this, this);
		OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.SuppressObscured));
		RoomLayer = desired;
		if (EffectsOfType<Dragging>().FirstOrDefault()?.Target is IPerceiver dragTarget)
		{
			dragTarget.RoomLayer = RoomLayer;
			(dragTarget as ICharacter)?.Body.Look(true);
		}

		OutputHandler.Handle(new EmoteOutput(targetEmote,
			flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
		if (!AffectedBy<Immwalk>())
		{
			AddEffect(new BlockLayerChange(this), DiveDelay);
		}

		Body.Look(true);
	}

	#endregion

	#region IClimb Members

	public TimeSpan ClimbDelay => TimeSpan.FromSeconds(10); // TODO

	public bool DoClimbMovementCheck(Difficulty difficulty)
	{
		if (AffectedBy<Immwalk>())
		{
			return true;
		}

		var staminaCost = Gameworld.GetStaticDouble("ClimbingTickStaminaUsage") * (EffectsOfType<Dragging>().Any()
			? 1.0 + ((IHaveWeight)EffectsOfType<Dragging>().First().Target).Weight / Weight
			: 1.0);
		if (!CanSpendStamina(staminaCost))
		{
			if (Gameworld.GetStaticBool("ShowStaminaCostInExhaustedMessage"))
			{
				OutputHandler.Send(
					$"You are too exhausted to continue climbing; instead you simply cling on ({staminaCost.ToString("N2", this).ColourValue()} stamina required).");
			}
			else
			{
				OutputHandler.Send("You are too exhausted to continue climbing; instead you simply cling on.");
			}

			return false;
		}

		SpendStamina(staminaCost);
		var check = Gameworld.GetCheck(CheckType.ClimbCheck);
		var weather = Location.CurrentWeather(this);
		var allResults = check.CheckAgainstAllDifficulties(this, difficulty, null,
			externalBonus: (weather?.Precipitation.PrecipitationClimbingBonus() ?? 0.0) +
						   (weather?.Wind.WindClimbingBonus() ?? 0.0));
		var result = allResults[difficulty];

		if (result.Outcome.IsPass())
		{
			return true;
		}

		if (result == Outcome.MajorFail && difficulty > Difficulty.VeryEasy)
		{
			var compare = difficulty.StageDown(3);
			var compareOutcome = allResults[compare];
			if (compareOutcome == Outcome.MajorFail || difficulty == Difficulty.Impossible)
			{
				OutputHandler.Handle(new EmoteOutput(new Emote("@ lose|loses &0's grip and fall|falls!", this, this),
					style: OutputStyle.NoNewLine, flags: OutputFlags.SuppressObscured));
				FallToGround();
				Body.Look(true);
				return false;
			}
		}

		OutputHandler.Handle(new EmoteOutput(
			new Emote("@ do|does not make any climbing progress, but manage|manages to keep hold.", this),
			flags: OutputFlags.SuppressObscured));
		return false;
	}

	public (bool Truth, string Error) CanClimbUp()
	{
		var (truth, error) = CheckGeneralMovementRestrictions("climb");
		if (!truth)
		{
			return (false, error);
		}

		if (Location.IsUnderwaterLayer(RoomLayer))
		{
			return (false, "You cannot climb when you are underwater. That's called swimming.");
		}

		if (!Race.CanClimb)
		{
			return (false, "Your kind are not built for climbing.");
		}

		var terrain = Location.Terrain(this);
		var higherLayers = terrain.TerrainLayers.Where(x => x.IsHigherThan(RoomLayer)).ToList();
		var climbExit = Location.ExitsFor(this)
								.FirstOrDefault(x => x.OutboundDirection == CardinalDirection.Up && x.IsClimbExit);
		if (!higherLayers.Any())
		{
			if (climbExit != null)
			{
				return (true, string.Empty);
			}

			return (false, "You are already as high as you can climb.");
		}

		if (climbExit == null &&
			!higherLayers.Any(x => x.In(RoomLayer.HighInTrees, RoomLayer.InTrees, RoomLayer.OnRooftops)))
		{
			return (false, "There is nowhere for you to climb here. You would need to fly to ascend any higher.");
		}

		var dragging = CombinedEffectsOfType<Dragging>().FirstOrDefault();
		if (dragging != null &&
			(MaximumDragWeight * dragging?.Draggers.First(x => x.Character == this).Aid?.EffortMultiplier ?? 1.0) -
			Body.ExternalItems.Sum(x => x.Weight) - ((IHaveWeight)dragging.Target).Weight < 0.0)
		{
			return (false, $"You aren't strong enough to lift {dragging.Target.HowSeen(this)} with you.");
		}

		return (true, string.Empty);
	}

	public (bool Truth, string Error) CanClimbDown()
	{
		var (truth, error) = CheckGeneralMovementRestrictions("climb");
		if (!truth)
		{
			return (false, error);
		}

		if (Location.IsUnderwaterLayer(RoomLayer))
		{
			return (false, "You cannot climb when you are underwater. That's called swimming.");
		}

		if (!Race.CanClimb)
		{
			return (false, "Your kind are not built for climbing.");
		}

		var terrain = Location.Terrain(this);
		var lowerLayers = terrain.TerrainLayers
								 .Where(x => x.IsLowerThan(RoomLayer) && !x.IsLowerThan(RoomLayer.GroundLevel))
								 .ToList();
		var climbExit = Location.ExitsFor(this)
								.FirstOrDefault(x => x.OutboundDirection == CardinalDirection.Down && x.IsClimbExit);
		if (!lowerLayers.Any())
		{
			if (climbExit != null)
			{
				return (true, string.Empty);
			}

			return (false, "You are already as low as you can climb.");
		}

		if (climbExit == null && !lowerLayers.Any(x =>
				x.In(RoomLayer.HighInTrees, RoomLayer.InTrees, RoomLayer.OnRooftops, RoomLayer.GroundLevel)))
		{
			return (false, "There is nowhere for you to climb down to.");
		}

		var dragging = CombinedEffectsOfType<Dragging>().FirstOrDefault();
		if (dragging != null &&
			(MaximumDragWeight * dragging?.Draggers.First(x => x.Character == this).Aid?.EffortMultiplier ?? 1.0) -
			Body.ExternalItems.Sum(x => x.Weight) - ((IHaveWeight)dragging.Target).Weight < 0.0)
		{
			return (false, $"You aren't strong enough to drag {dragging.Target.HowSeen(this)} with you.");
		}

		return (true, string.Empty);
	}

	public void ClimbUp(IEmote actionEmote = null)
	{
		if (RidingMount is not null && RidingMount.IsPrimaryRider(this))
		{
			RidingMount.RiderClimbUp(this, actionEmote);
			return;
		}

		ConvertGrapplesToDrags();
		var (truth, error) = CanClimbUp();
		if (!truth)
		{
			OutputHandler.Send(error);
			return;
		}

		var terrain = Location.Terrain(this);
		var higherLayers = terrain.TerrainLayers.Where(x => x.IsHigherThan(RoomLayer)).ToList();
		var climbExit = Location.ExitsFor(this)
								.FirstOrDefault(x => x.OutboundDirection == CardinalDirection.Up && x.IsClimbExit);
		if (!higherLayers.Any())
		{
			Move(CardinalDirection.Up, actionEmote, true);
			return;
		}

		var result = DoClimbMovementCheck(climbExit != null ? climbExit.ClimbDifficulty : Difficulty.ExtremelyEasy);
		if (!AffectedBy<Immwalk>())
		{
			AddEffect(new BlockLayerChange(this), ClimbDelay);
		}

		if (!result)
		{
			return;
		}

		var desiredLayer = higherLayers.LowestLayer();
		OutputHandler.Handle(new MixedEmoteOutput(new Emote(Gameworld.GetStaticString("ClimbUpEmote"), this, this),
			flags: OutputFlags.SuppressObscured).Append(actionEmote));
		RoomLayer = desiredLayer;
		if (EffectsOfType<Dragging>().FirstOrDefault()?.Target is IPerceiver dragTarget)
		{
			dragTarget.RoomLayer = RoomLayer;
			(dragTarget as ICharacter)?.Body.Look(true);
		}

		OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(Gameworld.GetStaticString("ClimbUpEmoteTargetLocation"), this, this),
				flags: OutputFlags.SuppressSource | OutputFlags.SuppressObscured).Append(actionEmote));
		if (RoomLayer == RoomLayer.OnRooftops)
		{
			var highestUpright = MostUprightMobilePosition();
			if (highestUpright != null)
			{
				SetPosition(highestUpright, PositionModifier.None, null, null);
			}
			else
			{
				SetPosition(PositionClimbing.Instance, PositionModifier.None, null, null);
			}
		}
		else
		{
			SetPosition(PositionClimbing.Instance, PositionModifier.None, null, null);
		}

		Body.Look(true);
	}

	public void ClimbDown(IEmote actionEmote = null)
	{
		if (RidingMount is not null && RidingMount.IsPrimaryRider(this))
		{
			RidingMount.RiderClimbDown(this, actionEmote);
			return;
		}

		ConvertGrapplesToDrags();
		var (truth, error) = CanClimbDown();
		if (!truth)
		{
			OutputHandler.Send(error);
			return;
		}

		var terrain = Location.Terrain(this);
		var lowerLayers = terrain.TerrainLayers.Where(x => x.IsLowerThan(RoomLayer)).ToList();
		var climbExit = Location.ExitsFor(this)
								.FirstOrDefault(x => x.OutboundDirection == CardinalDirection.Down && x.IsClimbExit);
		if (!lowerLayers.Any())
		{
			Move(CardinalDirection.Down, actionEmote, true);
			return;
		}

		var result = DoClimbMovementCheck(climbExit != null ? climbExit.ClimbDifficulty : Difficulty.Trivial);
		if (!AffectedBy<Immwalk>())
		{
			AddEffect(new BlockLayerChange(this), ClimbDelay);
		}

		if (!result)
		{
			return;
		}

		var desiredLayer = lowerLayers.HighestLayer();
		OutputHandler.Handle(new MixedEmoteOutput(new Emote(Gameworld.GetStaticString("ClimbDownEmote"), this, this),
			flags: OutputFlags.SuppressObscured).Append(actionEmote));
		RoomLayer = desiredLayer;
		if (EffectsOfType<Dragging>().FirstOrDefault()?.Target is IPerceiver dragTarget)
		{
			dragTarget.RoomLayer = RoomLayer;
			(dragTarget as ICharacter)?.Body.Look(true);
		}

		OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(Gameworld.GetStaticString("ClimbDownEmoteTargetLocation"), this, this),
				flags: OutputFlags.SuppressSource | OutputFlags.SuppressObscured).Append(actionEmote));
		if (Location.IsSwimmingLayer(RoomLayer))
		{
			SetPosition(PositionSwimming.Instance, PositionModifier.None, null, null);
		}
		else if (RoomLayer == RoomLayer.GroundLevel)
		{
			var highestUpright = MostUprightMobilePosition();
			if (highestUpright != null)
			{
				SetPosition(highestUpright, PositionModifier.None, null, null);
			}
			else
			{
				SetPosition(PositionClimbing.Instance, PositionModifier.None, null, null);
			}
		}
		else
		{
			SetPosition(PositionClimbing.Instance, PositionModifier.None, null, null);
		}

		Body.Look(true);
	}

	#endregion
}