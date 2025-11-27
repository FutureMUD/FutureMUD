using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Form.Material;

namespace MudSharp.Framework;

public abstract class PerceiverItem : PerceivedItem, IPerceiver
{
	protected PerceiverItem()
	{
	}

	protected PerceiverItem(long id)
		: base(id)
	{
	}

	protected override void ReleaseEvents()
	{
		base.ReleaseEvents();
		OnEngagedInMelee = null;
		OnJoinCombat = null;
		OnLeaveCombat = null;
	}

	public abstract bool CanHear(IPerceivable thing);
	public abstract bool CanSense(IPerceivable thing, bool ignoreFuzzy = false);
	public abstract bool CanSee(IPerceivable thing, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);
	public abstract bool CanSmell(IPerceivable thing);
	public virtual double VisionPercentage => 1.0;

	public override RoomLayer RoomLayer
	{
		get => _roomLayer;
		set
		{
			base.RoomLayer = value;
			Combat?.ReevaluateMeleeRange(this);
		}
	}

	public override void MoveTo(ICell location, RoomLayer layer, ICellExit exit = null, bool noSave = false)
	{
		base.MoveTo(location, layer, exit, noSave);
		RoomLayer = layer;
		Combat?.ReevaluateMeleeRange(this);
	}

	public virtual ICellOverlayPackage CurrentOverlayPackage { get; set; }

	public abstract int LineFormatLength { get; }

	public abstract int InnerLineFormatLength { get; }

	#region IFormatProvider Members

	public virtual object GetFormat(Type formatType)
	{
		return CultureInfo.InvariantCulture.GetFormat(formatType);
	}

	#endregion

	#region IHaveAccount Members

	public virtual IAccount Account
	{
		get => DummyAccount.Instance;
		set { }
	}

	#endregion

	#region IHaveDubs Members

	public virtual IList<IDub> Dubs => new List<IDub>();

	public virtual bool HasDubFor(IKeyworded target, IEnumerable<string> keywords)
	{
		return false;
	}

	public virtual bool HasDubFor(IKeyworded target, string keyword)
	{
		return false;
	}

	#endregion

	#region IPerceiver Members

	public abstract PerceptionTypes NaturalPerceptionTypes { get; }

	public bool BriefCombatMode { get; set; } // TODO - save this

	public bool IsPersonOfInterest(IPerceivable thing)
	{
		return (thing?.IsSelf(this) ?? false) ||
		       thing == CombatTarget ||
		       (thing as ICombatant)?.CombatTarget == this
			;
	}

	#endregion

	#region Implementation of ICombatant

	private ICombat _combat;

	public ICombat Combat
	{
		get => _combat;
		set
		{
			if (_combat != null && value == null)
			{
				PerceiverLeaveCombat();
			}

			_combat = value;
			if (_combat != null)
			{
				PerceiverJoinCombat();
			}
		}
	}

	private IPerceiver _combatTarget;

	public virtual IPerceiver CombatTarget
	{
		get => _combatTarget;
		set
		{
			if (_combatTarget != value)
			{
				//Aim?.ReleaseEvents();
				//Aim = null;
				RemoveAllEffects(x => x.IsEffectType<ICombatEffectRemovedOnTargetChange>());
				if (TargettedBodypart != null &&
				    !(value is ICharacter tch && tch.Body.Bodyparts.Contains(TargettedBodypart)))
				{
					TargettedBodypart = null;
				}
			}

			_combatTarget = value;
		}
	}

	public virtual double DefensiveAdvantage { get; set; }
	public virtual double OffensiveAdvantage { get; set; }

	private IAimInformation _aim;

	public IAimInformation Aim
	{
		get => _aim;
		set
		{
			if (_aim != null)
			{
				_aim.AimInvalidated -= Aim_AimInvalidated;
				_aim.ReleaseEvents();
			}

			_aim = value;
			if (_aim != null)
			{
				_aim.AimInvalidated -= Aim_AimInvalidated;
				_aim.AimInvalidated += Aim_AimInvalidated;
			}
		}
	}

	private void Aim_AimInvalidated(object sender, EventArgs e)
	{
		var aim = (IAimInformation)sender;
		HandleEvent(EventType.LostAim, this, aim.Target, aim.Weapon.Parent);
		Aim = null;
	}

	public IBodypart TargettedBodypart { get; set; }

	public abstract DefenseType PreferredDefenseType { get; set; }
	public virtual CombatStrategyMode CombatStrategyMode { get; set; }
	public abstract ICombatMove ResponseToMove(ICombatMove move, IPerceiver assailant);
	public abstract bool CheckCombatStatus();

	public virtual void AcquireTarget()
	{
		// Do nothing
	}

	public virtual ICombatMove ChooseMove()
	{
		return null;
	}

	public virtual ItemQuality NaturalWeaponQuality(INaturalAttack attack)
	{
		return ItemQuality.Standard;
	}

	public virtual bool CanTruce()
	{
		return true;
	}

	public virtual string WhyCannotTruce()
	{
		throw new ApplicationException(
			"Perceiver without overriden version of WhyCannotTruce asked to provide a reason why they could not truce.");
	}

	public virtual Facing GetFacingFor(ICombatant opponent, bool reset = false)
	{
		return Facing.Front;
	}

	public virtual bool MeleeRange { get; set; }

	/// <summary>
	/// This property calculates whether a combatant is in melee range with their target or is themselves engaged in melee by another
	/// </summary>
	public bool IsEngagedInMelee
	{
		get
		{
			if (Combat is null)
			{
				return false;
			}

			return MeleeRange ||
			       (Combat.Combatants.Except(this).Any(x => x.CombatTarget == this && x.MeleeRange));
		}
	}

	public virtual bool CanEngage(IPerceiver target)
	{
		return false;
	}

	public virtual string WhyCannotEngage(IPerceiver target)
	{
		return $"You cannot engage {target.HowSeen(this)} in combat.";
	}

	public virtual bool Engage(IPerceiver target, bool ranged)
	{
		return false;
	}

	public event PerceivableEvent OnJoinCombat;

	protected void PerceiverJoinCombat()
	{
		OnJoinCombat?.Invoke(this);
	}

	public event PerceivableEvent OnEngagedInMelee;

	protected void PerceiverEngagedInMelee()
	{
		OnEngagedInMelee?.Invoke(this);
	}

	public event PerceivableEvent OnLeaveCombat;

	protected void PerceiverLeaveCombat()
	{
		OnLeaveCombat?.Invoke(this);
	}

	public ICombatantCover Cover { get; set; }

	public virtual bool TakeOrQueueCombatAction(ISelectedCombatAction action)
	{
		return false;
	}

	public virtual ICharacterCombatSettings CombatSettings
	{
		get => null;
		set { }
	}

	private static ExpressionEngine.Expression _meleeTargetingExpression;

	public static ExpressionEngine.Expression MeleeTargetingExpression
	{
		get
		{
			if (_meleeTargetingExpression is null)
			{
				_meleeTargetingExpression =
					new ExpressionEngine.Expression(Futuremud.Games.First()
					                                         .GetStaticConfiguration("MeleeTargetingBonusExpression"));
			}

			return _meleeTargetingExpression;
		}
	}

	public virtual double GetBonusForDefendersFromTargeting()
	{
		if (TargettedBodypart is null)
		{
			return 0.0;
		}

		if (CombatTarget is not ICharacter tch)
		{
			return 0.0;
		}

		var (min, max) = tch.Body.Bodyparts.Select(x => x.RelativeHitChance).MinMax(x => x > 0.0);

		return Convert.ToDouble(MeleeTargetingExpression.EvaluateDoubleWith(
			("minchance", min),
			("maxchance", max),
			("hitchance", TargettedBodypart.RelativeHitChance)
		));
	}

	public virtual double GetDefensiveAdvantagePenaltyFromTargeting()
	{
		if (TargettedBodypart is null)
		{
			return 0.0;
		}

		return Gameworld.GetStaticDouble("MeleeTargetingDefensiveAdvantage");
	}

	#endregion

	public virtual bool ShouldFall()
	{
		if (PositionState?.SafeFromFalling == true)
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

		return true;
	}

	public void FallToGround()
	{
		if (this is ICharacter ch)
		{
			ch.Movement?.CancelForMoverOnly(ch);
		}

		var dragTarget = EffectsOfType<Dragging>().FirstOrDefault()?.Target as IPerceiver;
		RemoveAllEffects(x => x.IsEffectType<IActionEffect>());
		var fallDistance = 0.0;
		while (true)
		{
			var result = FallOneLayer(ref fallDistance);
			if (dragTarget != null)
			{
				var otherFallDistance = fallDistance;
				dragTarget.FallOneLayer(ref otherFallDistance);
			}

			if (result)
			{
				break;
			}
		}
	}

	public virtual bool CouldTransitionToLayer(RoomLayer otherLayer)
	{
		return false;
	}

	public virtual (bool Success, IEmoteOutput FailureOutput) CanCross(ICellExit exit)
	{
		if (exit.Exit.Door?.IsOpen == false)
		{
			return (false,
				new EmoteOutput(new Emote("@ stop|stops at $0 because it is closed.", this, exit.Exit.Door.Parent),
					flags: OutputFlags.SuppressObscured));
		}

		return (true, null);
	}

	public ICellExit GetFirstFallExit()
	{
		return Location.ExitsFor(this)
		               .Where(x => (x.IsFallExit || x.IsClimbExit) && x.OutboundDirection == CardinalDirection.Down)
		               .FirstOrDefault(x => CanCross(x).Success);
	}

	public virtual void DoFallDamage(double fallDistance)
	{
		// Do nothing
	}

	/// <summary>
	/// Falls down a single layer and accumulates any falling distance, handles any side effects, and handles all echoes
	/// </summary>
	/// <param name="cumulativeFallDistance">The cumulative falling distance in "total rooms"</param>
	/// <returns>True if the fall should stop, false if the fall should continue</returns>
	public bool FallOneLayer(ref double cumulativeFallDistance)
	{
		var ch = this as ICharacter;
		var item = this as IGameItem;

		if (ch == null && item == null)
		{
			throw new ApplicationException("FallOneLayer was called for a non Character, non GameItem type.");
		}

		switch (RoomLayer)
		{
			case RoomLayer.GroundLevel:
				// Figure out if this is water or not
				if (Location.Terrain(this).TerrainLayers.Any(x => x.IsUnderwater()))
				{
					var effectiveDistance =
						cumulativeFallDistance - Gameworld.GetStaticDouble("FallIntoWaterRoomsGrace");
					OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("FallEmoteHitWater"), this, this)));
					DoFallDamage(effectiveDistance);
					if (ch != null)
					{
						SetPosition(PositionSwimming.Instance, PositionModifier.None, null, null);
					}
					else
					{
						SetPosition(PositionFloatingInWater.Instance, PositionModifier.None, null, null);
					}

					cumulativeFallDistance = effectiveDistance;
					if (cumulativeFallDistance >= Gameworld.GetStaticDouble("FallDeeperIntoWaterDistance"))
					{
						cumulativeFallDistance -= Gameworld.GetStaticDouble("FallDeeperIntoWaterDistance");
						RoomLayer = RoomLayer.Underwater;
						OutputHandler.Handle(new EmoteOutput(
							new Emote(Gameworld.GetStaticString("FallEnterRoomEmote"), this, this), flags: OutputFlags.SuppressSource));
						return false;
					}

					return true;
				}

				// Figure out if there is a fall exit
				var fallExit = GetFirstFallExit();
				if (fallExit != null)
				{
					cumulativeFallDistance += 0.5;
					OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("FallLeaveRoomEmote"), this, this)));
					if (ch != null)
					{
						Location.Leave(ch);
						fallExit.Destination.Enter(ch, fallExit,
							roomLayer: fallExit.Destination.Terrain(this).TerrainLayers.HighestLayer());
					}
					else
					{
						Location.Extract(item);
						RoomLayer = fallExit.Destination.Terrain(this).TerrainLayers.HighestLayer();
						fallExit.Destination.Insert(item, true);
					}

					OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("FallEnterRoomEmote"), this, this), flags: OutputFlags.SuppressSource));
					return false;
				}

				OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("FallEmoteHitGround"), this, this)));
				cumulativeFallDistance += 0.1;
				DoFallDamage(cumulativeFallDistance);
				if (ch != null)
				{
					SetPosition(PositionSprawled.Instance, PositionModifier.None, null, null);
				}
				else
				{
					SetPosition(PositionUndefined.Instance, PositionModifier.None, null, null);
				}

				return true;
			case RoomLayer.Underwater:
				if (cumulativeFallDistance >= Gameworld.GetStaticDouble("FallDeeperIntoWaterDistance"))
				{
					cumulativeFallDistance -= Gameworld.GetStaticDouble("FallDeeperIntoWaterDistance");
					if (!Location.Terrain(this).TerrainLayers.Any(x => x.IsLowerThan(RoomLayer.Underwater)))
					{
						// Figure out if there is a fall exit
						fallExit = GetFirstFallExit();
						if (fallExit != null)
						{
							cumulativeFallDistance += 0.5;
							OutputHandler.Handle(new EmoteOutput(
								new Emote(Gameworld.GetStaticString("FallLeaveRoomEmote"), this, this)));
							if (ch != null)
							{
								Location.Leave(ch);
								fallExit.Destination.Enter(ch, fallExit,
									roomLayer: fallExit.Destination.Terrain(this).TerrainLayers.HighestLayer());
							}
							else
							{
								Location.Extract(item);
								RoomLayer = fallExit.Destination.Terrain(this).TerrainLayers.HighestLayer();
								fallExit.Destination.Insert(item, true);
							}

							OutputHandler.Handle(new EmoteOutput(
								new Emote(Gameworld.GetStaticString("FallEnterRoomEmote"), this, this), flags: OutputFlags.SuppressSource));
							return false;
						}

						OutputHandler.Handle(new EmoteOutput(
							new Emote(Gameworld.GetStaticString("FallEmoteHitWaterFloor"), this, this)));
						if (item != null)
						{
							item.PositionState = PositionUndefined.Instance;
						}

						DoFallDamage(cumulativeFallDistance);
						return true;
					}

					OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("FallLeaveRoomEmoteUnderwater"), this, this)));
					RoomLayer = RoomLayer.DeepUnderwater;
					OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("FallEnterRoomEmoteUnderwater"), this, this), flags: OutputFlags.SuppressSource));
					return false;
				}

				return true;
			case RoomLayer.DeepUnderwater:
				if (cumulativeFallDistance >= Gameworld.GetStaticDouble("FallDeeperIntoWaterDistance"))
				{
					cumulativeFallDistance -= Gameworld.GetStaticDouble("FallDeeperIntoWaterDistance");
					if (!Location.Terrain(this).TerrainLayers.Any(x => x.IsLowerThan(RoomLayer.DeepUnderwater)))
					{
						// Figure out if there is a fall exit
						fallExit = GetFirstFallExit();
						if (fallExit != null)
						{
							cumulativeFallDistance += 0.5;
							OutputHandler.Handle(new EmoteOutput(
								new Emote(Gameworld.GetStaticString("FallLeaveRoomEmote"), this, this)));
							if (ch != null)
							{
								Location.Leave(ch);
								fallExit.Destination.Enter(ch, fallExit,
									roomLayer: fallExit.Destination.Terrain(this).TerrainLayers.HighestLayer());
							}
							else
							{
								Location.Extract(item);
								RoomLayer = fallExit.Destination.Terrain(this).TerrainLayers.HighestLayer();
								fallExit.Destination.Insert(item, true);
							}

							OutputHandler.Handle(new EmoteOutput(
								new Emote(Gameworld.GetStaticString("FallEnterRoomEmote"), this, this), flags: OutputFlags.SuppressSource));
							return false;
						}

						OutputHandler.Handle(new EmoteOutput(
							new Emote(Gameworld.GetStaticString("FallEmoteHitWaterFloor"), this, this)));
						if (item != null)
						{
							item.PositionState = PositionUndefined.Instance;
						}

						DoFallDamage(cumulativeFallDistance);
						return true;
					}

					OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("FallLeaveRoomEmoteUnderwater"), this, this)));
					RoomLayer = RoomLayer.VeryDeepUnderwater;
					OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("FallEnterRoomEmoteUnderwater"), this, this), flags: OutputFlags.SuppressSource));
					return false;
				}

				return true;
			case RoomLayer.VeryDeepUnderwater:
				if (cumulativeFallDistance >= Gameworld.GetStaticDouble("FallDeeperIntoWaterDistance"))
				{
					// Figure out if there is a fall exit
					fallExit = GetFirstFallExit();
					if (fallExit != null)
					{
						cumulativeFallDistance += 0.5;
						OutputHandler.Handle(new EmoteOutput(
							new Emote(Gameworld.GetStaticString("FallLeaveRoomEmote"), this, this)));
						if (ch != null)
						{
							Location.Leave(ch);
							fallExit.Destination.Enter(ch, fallExit,
								roomLayer: fallExit.Destination.Terrain(this).TerrainLayers.HighestLayer());
						}
						else
						{
							Location.Extract(item);
							RoomLayer = fallExit.Destination.Terrain(this).TerrainLayers.HighestLayer();
							fallExit.Destination.Insert(item, true);
						}

						OutputHandler.Handle(new EmoteOutput(
							new Emote(Gameworld.GetStaticString("FallEnterRoomEmote"), this, this), flags: OutputFlags.SuppressSource));
						return false;
					}

					cumulativeFallDistance -= Gameworld.GetStaticDouble("FallDeeperIntoWaterDistance");
					OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("FallEmoteHitWaterFloor"), this, this)));
					if (item != null)
					{
						item.PositionState = PositionUndefined.Instance;
					}

					DoFallDamage(cumulativeFallDistance);
				}

				return true;
			case RoomLayer.InTrees:
				if (cumulativeFallDistance > 0.0 && RandomUtilities.DoubleRandom(0.0, 1.0) <=
				    Gameworld.GetStaticDouble("FallTreesDamageChance"))
				{
					var impactPercent = RandomUtilities.DoubleRandom(
						Gameworld.GetStaticDouble("FallTreesDamageMultiplierMinimum"),
						Gameworld.GetStaticDouble("FallTreesDamageMultiplierMaximum"));
					var treeFall = cumulativeFallDistance * impactPercent;
					OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("FallEmoteHitTrees"), this, this)));
					DoFallDamage(treeFall);
					cumulativeFallDistance -= treeFall;
				}

				OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("FallLeaveRoomEmoteTrees"), this, this)));
				RoomLayer = RoomLayer.GroundLevel;
				OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("FallEnterRoomEmoteFromTrees"), this, this), flags: OutputFlags.SuppressSource));
				cumulativeFallDistance += 0.25;
				return false;
			case RoomLayer.HighInTrees:
				if (cumulativeFallDistance > 0.0 && RandomUtilities.DoubleRandom(0.0, 1.0) <=
				    Gameworld.GetStaticDouble("FallTreesDamageChance"))
				{
					var impactPercent = RandomUtilities.DoubleRandom(
						Gameworld.GetStaticDouble("FallTreesDamageMultiplierMinimum"),
						Gameworld.GetStaticDouble("FallTreesDamageMultiplierMaximum"));
					var treeFall = cumulativeFallDistance * impactPercent;
					OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("FallEmoteHitTrees"), this, this)));
					DoFallDamage(treeFall);
					cumulativeFallDistance -= treeFall;
				}

				OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("FallLeaveRoomEmoteTrees"), this, this)));
				RoomLayer = RoomLayer.InTrees;
				OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("FallEnterRoomEmoteFromTrees"), this, this), flags: OutputFlags.SuppressSource));
				cumulativeFallDistance += 0.25;
				return false;
			case RoomLayer.OnRooftops:
				if (cumulativeFallDistance > 0.0 && RandomUtilities.DoubleRandom(0.0, 1.0) <=
				    Gameworld.GetStaticDouble("FallRooftopsDamageChance"))
				{
					OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("FallEmoteHitRooftop"), this, this)));
					DoFallDamage(cumulativeFallDistance);
					return true;
				}

				OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("FallLeaveRoomEmoteRooftops"), this, this)));
				RoomLayer = RoomLayer.GroundLevel;
				OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("FallEnterRoomEmote"), this, this), flags: OutputFlags.SuppressSource));
				cumulativeFallDistance += 0.25;
				return false;
			case RoomLayer.InAir:
				if (!Location.Terrain(this).TerrainLayers.Any(x => x.IsLowerThan(RoomLayer.InAir)))
				{
					fallExit = GetFirstFallExit();
					if (fallExit != null)
					{
						cumulativeFallDistance += 0.5;
						OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("FallLeaveRoomEmote"),
							this, this)));
						if (ch != null)
						{
							Location.Leave(ch);
							fallExit.Destination.Enter(ch, fallExit,
								roomLayer: fallExit.Destination.Terrain(this).TerrainLayers.HighestLayer());
						}
						else
						{
							Location.Extract(item);
							RoomLayer = fallExit.Destination.Terrain(this).TerrainLayers.HighestLayer();
							Location.Insert(item, true);
						}

						OutputHandler.Handle(new EmoteOutput(
							new Emote(Gameworld.GetStaticString("FallEnterRoomEmote"), this, this), flags: OutputFlags.SuppressSource));
						return false;
					}

					return true;
				}

				OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("FallLeaveRoomEmote"), this, this)));
				RoomLayer = Location.Terrain(this).TerrainLayers.Where(x => x.IsLowerThan(RoomLayer.InAir))
				                    .FirstMax(x => x.LayerHeight());
				OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("FallEnterRoomEmote"), this, this), flags: OutputFlags.SuppressSource));
				cumulativeFallDistance += 0.25;
				return false;
			case RoomLayer.HighInAir:

				OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("FallLeaveRoomEmote"), this, this)));
				RoomLayer = RoomLayer.InAir;
				OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("FallEnterRoomEmote"), this, this), flags: OutputFlags.SuppressSource));
				cumulativeFallDistance += 0.25;
				return false;
		}

		return true;
	}
}