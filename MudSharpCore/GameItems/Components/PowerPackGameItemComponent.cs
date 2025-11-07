using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Health;
using MudSharp.Combat.ScatterStrategies;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.GameItems.Components;

public class PowerPackGameItemComponent : GameItemComponent, ILaserPowerPack
{
	protected PowerPackGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;


	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (PowerPackGameItemComponentProto)newProto;
	}

	#region Constructors

	public PowerPackGameItemComponent(PowerPackGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
		base(parent, proto, temporary)
	{
		_prototype = proto;
		CurrentWattSeconds = proto.WattSecondCapacity + (int)parent.Quality * proto.WattSecondBonusPerQuality;
	}

	public PowerPackGameItemComponent(MudSharp.Models.GameItemComponent component,
		PowerPackGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public PowerPackGameItemComponent(PowerPackGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		CurrentWattSeconds = double.Parse(root.Element("CurrentWattSeconds").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PowerPackGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("CurrentWattSeconds", CurrentWattSeconds)
		).ToString();
	}

	#endregion

	#region ILaserPowerPack Implementation

	public string ClipType => _prototype.ClipType;

	public double CurrentWattSeconds { get; protected set; }

	public double PowerLevel => CurrentWattSeconds /
								(_prototype.WattSecondCapacity +
								 (int)Parent.Quality * _prototype.WattSecondBonusPerQuality);

	public bool CanDraw(double watts)
	{
		return watts <= CurrentWattSeconds;
	}

	public void Draw(double watts)
	{
		CurrentWattSeconds -= watts;
		Changed = true;
	}

	private IPerceiver CheckFriendlyFire(ICharacter actor, IPerceiver target, Outcome shotOutcome,
		IRangedWeaponType weaponType, double painMultiplier, double stunMultiplier, IEnumerable<ICell> path)
	{
		// TODO
		if (Dice.Roll(1, 100) > 10 * (8 - (int)shotOutcome))
		{
			return target;
		}

		var range = path.Count();
		if (target.IsEngagedInMelee)
		{
			var proximity = target.Combat.MeleeProximityOfCombatant(target);
		}

		bool canScatterMelee = true,
			canScatterSameCover = false,
			canScatterSameRoom = false,
			canScatterDifferentRoom = false;
		switch (shotOutcome)
		{
			case Outcome.MajorPass:
				// Major Passes can only scatter in melee
				break;
			case Outcome.Pass:
				// Passes add in the possibility of hitting someone else in the same cover
				canScatterSameCover = true;
				break;
			case Outcome.MinorPass:
				canScatterSameCover = true;
				canScatterSameRoom = true;
				break;
			default:
				canScatterSameCover = true;
				canScatterSameRoom = true;
				canScatterDifferentRoom = true;
				break;
		}

				return target;
		}

		private Damage BuildDamage(ICharacter actor, IPerceiver target, IBodypart bodypart,
				IRangedWeaponType weaponType, OpposedOutcome defenseOutcome, double painMultiplier, double stunMultiplier)
		{
				weaponType.DamageBonusExpression.Formula.Parameters["range"] = target.DistanceBetween(actor, 10);
				weaponType.DamageBonusExpression.Formula.Parameters["quality"] = (int)Parent.Quality;
				weaponType.DamageBonusExpression.Formula.Parameters["degrees"] = (int)defenseOutcome.Degree;

				var finalDamage = weaponType.DamageBonusExpression.Evaluate(actor);
				return new Damage
				{
						ActorOrigin = actor,
						ToolOrigin = Parent,
						Bodypart = bodypart,
						DamageAmount = finalDamage,
						DamageType = DamageType.Burning,
						PainAmount = finalDamage * painMultiplier,
						StunAmount = finalDamage * stunMultiplier
				};
		}

		protected virtual void Hit(ICharacter actor, IPerceiver target, IBodypart bodypart,
				IRangedWeaponType weaponType, OpposedOutcome defenseOutcome, double painMultiplier, double stunMultiplier,
				IEmoteOutput defenseEmote)
		{
				var damage = BuildDamage(actor, target, bodypart, weaponType, defenseOutcome, painMultiplier, stunMultiplier);
				var wounds = new List<IWound>();
				var actorText = actor.HowSeen(actor);
				var targetText = target.HowSeen(actor);
				var locationText = target.Location?.HowSeen(actor) ?? "unknown location";
				Gameworld.DebugMessage(
						$"[Ranged] Beam hit resolved: {actorText} struck {targetText} at {locationText} (defense {defenseOutcome.Outcome}:{defenseOutcome.Degree}).");

				if (defenseEmote != null)
				{
						target.OutputHandler.Handle(defenseEmote);
				}

				if (!target.ColocatedWith(actor))
				{
						actor.Send("You hit your target.".Colour(Telnet.BoldGreen));
				}

				if (target is ICharacter targetChar)
				{
						wounds.AddRange(targetChar.Body.PassiveSufferDamage(damage));
						target.OutputHandler.Handle(
								new EmoteOutput(
										new Emote($"The beam hits $0 on &0's {bodypart.FullDescription()}!", target, target)));
						wounds.ProcessPassiveWounds();
						return;
				}

				if (target is IGameItem targetItem)
				{
						wounds.AddRange(targetItem.PassiveSufferDamage(damage));
						if (!wounds.Any())
						{
								target.OutputHandler.Handle(
										new EmoteOutput(new Emote("The beam hits $0 but ricochets off without causing any damage!",
												targetItem, targetItem)));
								return;
						}

						target.OutputHandler.Handle(
								new EmoteOutput(new Emote($"The beam hits $0!", actor, targetItem)));
						wounds.ProcessPassiveWounds();
						return;
				}

				throw new NotImplementedException("Unknown target type in Fire.");
		}

	private void HandleBeamScatterToCell(RangedScatterResult scatterResult)
	{
		var dummy = new DummyPerceiver(location: scatterResult.Cell)
		{
			RoomLayer = scatterResult.RoomLayer
				};

		var directionText = ScatterStrategyUtilities.DescribeFromDirection(scatterResult.DirectionFromTarget);
		var emote = new Emote($"A stray beam of energy flashes{directionText} and scorches the surroundings.", dummy);
		scatterResult.Cell.Handle(new EmoteOutput(emote, style: OutputStyle.CombatMessage,
			flags: OutputFlags.InnerWrap));
	}

	private void BroadcastBeamFlight(ICharacter actor, IPerceivable destination,
		IReadOnlyList<ICellExit> precomputedPath = null)
	{
		if (actor?.Location == null || destination?.Location == null)
		{
			return;
		}

		var path = precomputedPath ?? actor.PathBetween(destination, 10, false, false, true)?.ToList() ??
				   new List<ICellExit>();
		var dirDesc = path.Select(x => x.OutboundDirection).DescribeDirection();
		var oppDirDesc = path.Select(x => x.OutboundDirection).DescribeOppositeDirection();
		foreach (var cell in actor.CellsUnderneathFlight(destination, 10))
		{
			cell.Handle(new EmoteOutput(
				new Emote($"A laser beam flashes through the area from the {oppDirDesc} towards the {dirDesc}", actor)));
		}

		if (destination is not IPerceiver destinationPerceiver)
		{
			return;
		}

		if (!Equals(actor.Location, destinationPerceiver.Location))
		{
			destinationPerceiver.OutputHandler.Handle(
				new EmoteOutput(new Emote($"A laser beam flashes in from the {oppDirDesc}.", destinationPerceiver)));
			return;
		}

		if (actor.RoomLayer == destinationPerceiver.RoomLayer)
		{
			return;
		}

		var relativeDirection = destinationPerceiver.RoomLayer.IsHigherThan(actor.RoomLayer) ? "below" : "above";
		destinationPerceiver.OutputHandler.Handle(new EmoteOutput(
			new Emote($"A laser beam flashes in from {relativeDirection}.", destinationPerceiver),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
	}

private bool ShouldAttemptScatter(Outcome shotOutcome, ICharacter actor, string context)
{
	var baseChance = Math.Max(0, 10 * (8 - (int)shotOutcome));
	var multiplier = actor.Merits.OfType<IScatterChanceMerit>()
	                     .Aggregate(1.0, (current, merit) => current * merit.ScatterMultiplier);
	var finalChance = baseChance * multiplier;
	if (finalChance <= 0)
	{
		Gameworld.DebugMessage(
			$"[Scatter:{context}] No scatter possible for {actor.HowSeen(actor)} - final chance {finalChance:F2}% (base {baseChance}% * mult {multiplier:F2}).");
		return false;
	}

	var roll = Dice.Roll(1, 100);
	var success = roll <= finalChance;
	Gameworld.DebugMessage(
		$"[Scatter:{context}] {actor.HowSeen(actor)} rolled {roll:N0} vs {finalChance:F2}% (base {baseChance}% * mult {multiplier:F2}) -> {(success ? "scatter triggered" : "no scatter")}.");
	return success;
}

private bool TryResolveScatter(ICharacter actor, IPerceiver originalTarget, IRangedWeaponType weaponType,
	double painMultiplier, double stunMultiplier, IReadOnlyList<ICellExit> path, string context)
{
	var scatterResult = RangedScatterStrategyFactory.GetStrategy(weaponType)
		.GetScatterTarget(actor, originalTarget, path ?? Array.Empty<ICellExit>());

	if (scatterResult == null)
	{
		Gameworld.DebugMessage(
			$"[Scatter:{context}] {actor.HowSeen(actor)} had no valid ricochet destinations from {originalTarget?.HowSeen(actor) ?? "unknown target"}.");
		return false;
	}

	ResolveScatterResult(actor, weaponType, painMultiplier, stunMultiplier, scatterResult, context);
	return true;
}

private void ResolveScatterResult(ICharacter actor, IRangedWeaponType weaponType, double painMultiplier,
	double stunMultiplier, RangedScatterResult scatterResult, string context)
{
	if (scatterResult.Target != null)
	{
		var scatterPath = actor.PathBetween(scatterResult.Target, 10, false, false, true)?.ToList() ??
		                  new List<ICellExit>();
			BroadcastBeamFlight(actor, scatterResult.Target, scatterPath);
		var bodypart = (scatterResult.Target as IHaveABody)?.Body?.RandomBodyPartGeometry(Orientation.Centre,
			Alignment.Front, Facing.Front);
		Hit(actor, scatterResult.Target, bodypart, weaponType,
			new OpposedOutcome(OpposedOutcomeDirection.Proponent, OpposedOutcomeDegree.Marginal), painMultiplier,
			stunMultiplier, null);
		Gameworld.DebugMessage(
			$"[Scatter:{context}] Ricochet beam struck {scatterResult.Target.HowSeen(actor)} in {scatterResult.Cell.HowSeen(actor)} after deviating{ScatterStrategyUtilities.DescribeFromDirection(scatterResult.DirectionFromTarget)} (distance {scatterResult.DistanceFromTarget:N0}).");
		return;
	}

	var dummy = new DummyPerceiver(location: scatterResult.Cell)
	{
			RoomLayer = scatterResult.RoomLayer
		};
	var scatterCellPath = actor.PathBetween(dummy, 10, false, false, true)?.ToList() ?? new List<ICellExit>();
	BroadcastBeamFlight(actor, dummy, scatterCellPath);
	HandleBeamScatterToCell(scatterResult);
	Gameworld.DebugMessage(
		$"[Scatter:{context}] Ricochet beam impacted {scatterResult.Cell.HowSeen(actor)}{ScatterStrategyUtilities.DescribeFromDirection(scatterResult.DirectionFromTarget)} without striking a new target.");
}

	private bool TryResolveCoverInterception(ICharacter actor, IPerceiver target, Outcome shotOutcome,
		Outcome coverOutcome, IRangedWeaponType weaponType, IBodypart bodypart, OpposedOutcome defenseOutcome,
		double painMultiplier, double stunMultiplier, IReadOnlyList<ICellExit> path)
	{
		if (!shotOutcome.IsPass() || coverOutcome.IsPass() || target?.Cover == null)
		{
			return false;
		}

		var strikeCover = target.Cover.Cover.CoverType == CoverType.Hard || shotOutcome == Outcome.MajorPass ||
						  coverOutcome == Outcome.MinorFail;
		if (!strikeCover)
		{
			return false;
		}

	BroadcastBeamFlight(actor, target, path);
	var coverItem = target.Cover.CoverItem?.Parent;
	var actorText = actor.HowSeen(actor);
	var targetText = target.HowSeen(actor);
	var coverText = coverItem?.HowSeen(actor) ?? "environmental cover";
	Gameworld.DebugMessage(
		$"[Ranged] Cover interception: {actorText} vs {targetText} (shot {shotOutcome}, cover {coverOutcome}) stopped by {coverText}.");
	target.OutputHandler.Handle(
		new EmoteOutput(new Emote($"The beam strikes $?1|$1, ||$$0's cover!", target, target,
			coverItem)));
	actor.Send("You hit your target's cover instead.".Colour(Telnet.Yellow));

	var damage = BuildDamage(actor, target, bodypart, weaponType, defenseOutcome, painMultiplier, stunMultiplier);
	var coverWounds = coverItem?.PassiveSufferDamage(damage)?.ToList() ?? new List<IWound>();
	coverWounds.ProcessPassiveWounds();

	var scatterContext = $"cover {actorText}->{targetText}";
	if (ShouldAttemptScatter(shotOutcome, actor, scatterContext) &&
	    TryResolveScatter(actor, target, weaponType, painMultiplier, stunMultiplier, path, scatterContext))
	{
		return true;
	}

	Gameworld.DebugMessage(
		$"[Ranged] Cover fully absorbed the beam from {actorText} to {targetText}; no scatter occurred.");
	return true;
}

	private bool TryResolveMiss(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
		OpposedOutcome defenseOutcome, IRangedWeaponType weaponType, double painMultiplier, double stunMultiplier,
		IEmoteOutput defenseEmote, IReadOnlyList<ICellExit> path)
	{
		if (shotOutcome.IsPass() && defenseOutcome.Outcome != OpposedOutcomeDirection.Opponent)
		{
			return false;
		}

		BroadcastBeamFlight(actor, target, path);

		if (defenseEmote != null)
		{
			target.OutputHandler.Handle(defenseEmote);
		}

		target.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"The beam {(shotOutcome.IsPass() ? "narrowly misses @!" : "misses @ by a wide margin.")}", target)));

	if (!actor.ColocatedWith(target))
	{
		actor.Send("You missed your target.".Colour(Telnet.Red));
	}

	var actorText = actor.HowSeen(actor);
	var targetText = target.HowSeen(actor);
	var missReason = shotOutcome.IsPass()
		? $"defended ({defenseOutcome.Outcome}:{defenseOutcome.Degree})"
		: "wild shot";
	Gameworld.DebugMessage($"[Ranged] Miss: {actorText} vs {targetText} - {missReason} (shot {shotOutcome}, cover {coverOutcome}).");

	if (!shotOutcome.IsPass() && !coverOutcome.IsPass())
	{
		var scatterContext = $"miss {actorText}->{targetText}";
		if (ShouldAttemptScatter(shotOutcome, actor, scatterContext) &&
		    TryResolveScatter(actor, target, weaponType, painMultiplier, stunMultiplier, path, scatterContext))
		{
			return true;
		}
	}

	Gameworld.DebugMessage($"[Ranged] Miss resolved with no scatter for beam from {actorText} to {targetText}.");
	return true;
}

	private bool TryResolveObstruction(ICharacter actor, IPerceiver target, IRangedWeaponType weaponType,
		OpposedOutcome defenseOutcome, IBodypart bodypart, double painMultiplier, double stunMultiplier,
		IReadOnlyList<ICellExit> path)
	{
		var obstructionEffect = target.EffectsOfType<IRangedObstructionEffect>().Where(x => x.Applies(actor)).Shuffle()
										 .FirstOrDefault();
		if (obstructionEffect == null)
		{
			return false;
		}

	var obstruction = obstructionEffect.Obstruction;
	var obstructionPerceiver = obstruction as IPerceiver;
	var actorText = actor.HowSeen(actor);
	var targetText = target.HowSeen(actor);
	var obstructionText = obstruction.HowSeen(actor);
	Gameworld.DebugMessage(
		$"[Ranged] Obstruction: {actorText}'s beam at {targetText} intercepted by {obstructionText}.");
	if (obstructionPerceiver != null)
	{
		var obstructionPath = actor.PathBetween(obstructionPerceiver, 10, false, false, true)?.ToList() ??
							  new List<ICellExit>();
		BroadcastBeamFlight(actor, obstructionPerceiver, obstructionPath);
		}
		else
		{
			BroadcastBeamFlight(actor, target, path);
		}

		target.OutputHandler.Handle(
			new EmoteOutput(new Emote("The beam strikes $1 instead of $0!", target, target, obstruction),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		actor.Send($"You hit {obstruction.HowSeen(actor)} instead!");

		var damage = BuildDamage(actor, target, bodypart, weaponType, defenseOutcome, painMultiplier, stunMultiplier);
	if (obstruction is IHaveWounds ihw)
	{
		if (obstruction is IHaveABody ihab)
		{
			var oldTarget = damage.Bodypart;
				damage = new Damage(damage)
				{
					Bodypart = ihab.Body.RandomBodyPartGeometry(oldTarget?.Orientation ?? Orientation.Centre,
						Alignment.Front, Facing.Front, true)
				};
			}

		var obstructionWounds = ihw.PassiveSufferDamage(damage)?.ToList() ?? new List<IWound>();
		obstructionWounds.ProcessPassiveWounds();
	}

	Gameworld.DebugMessage(
		$"[Ranged] Obstruction aftermath: beam concluded at {(obstructionPerceiver != null ? obstructionText : targetText)}.");
	return true;
}

		public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
				OpposedOutcome defenseOutcome, IBodypart bodypart, IRangedWeaponType weaponType, double painMultiplier,
				double stunMultiplier, IEmoteOutput defenseEmote)
		{
		// Fired at sky
		if (target == null)
		{
			if (actor.Location.CurrentOverlay.OutdoorsType != CellOutdoorsType.Outdoors)
			{
				actor.OutputHandler.Handle(new EmoteOutput(new Emote("The laser beam hits the ceiling!", actor)));
			}

			return;
		}

		var pathToTarget = actor.PathBetween(target, 10, false, false, true)?.ToList() ?? new List<ICellExit>();

		// Resolve cover interactions before anything else so ricochets happen from the correct origin.
		if (TryResolveCoverInterception(actor, target, shotOutcome, coverOutcome, weaponType, bodypart, defenseOutcome,
				painMultiplier, stunMultiplier, pathToTarget))
		{
			return;
		}

		// Simple misses (aim failure or evasive defence) can still scatter to fresh targets.
		if (TryResolveMiss(actor, target, shotOutcome, coverOutcome, defenseOutcome, weaponType, painMultiplier,
				stunMultiplier, defenseEmote, pathToTarget))
		{
			return;
		}

		// Anything that explicitly obstructs the shot takes the hit instead of the intended target.
		if (TryResolveObstruction(actor, target, weaponType, defenseOutcome, bodypart, painMultiplier, stunMultiplier,
				pathToTarget))
		{
			return;
		}

		BroadcastBeamFlight(actor, target, pathToTarget);
		Hit(actor, target, bodypart, weaponType, defenseOutcome, painMultiplier, stunMultiplier, defenseEmote);
	}

	#endregion
}
