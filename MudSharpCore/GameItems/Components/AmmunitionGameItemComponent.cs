using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Combat.ScatterStrategies;

namespace MudSharp.GameItems.Components;

public class AmmunitionGameItemComponent : GameItemComponent, IAmmo
{
	protected AmmunitionGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new AmmunitionGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (AmmunitionGameItemComponentProto)newProto;
	}

	#region Constructors

	public AmmunitionGameItemComponent(AmmunitionGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public AmmunitionGameItemComponent(MudSharp.Models.GameItemComponent component,
		AmmunitionGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	public AmmunitionGameItemComponent(AmmunitionGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	#endregion

	#region IAmmo Implementation

	public IAmmunitionType AmmoType => _prototype.AmmoType;

	public IGameItem GetFiredItem
	{
		get
		{
			if (_prototype.BulletProto == null)
			{
				return null;
			}

			return new GameItem(_prototype.BulletProto, quality: Parent.Quality);
		}
	}

	public IGameItem GetFiredWasteItem
	{
		get
		{
			if (_prototype.CasingProto == null)
			{
				return null;
			}

			return new GameItem(_prototype.CasingProto, quality: Parent.Quality);
		}
	}

	private void HandleAmmunitionAftermath(ICharacter actor, IPerceiver target, IGameItem ammo, bool hit = false,
			IEmoteOutput emoteOnBreak = null, IEmoteOutput emoteOnFallToGround = null)
	{
		if (target == null)
		{
			return;
		}

		ammo.RoomLayer = target.RoomLayer;

		if (RandomUtilities.Roll(1.0, hit ? AmmoType.BreakChanceOnHit : AmmoType.BreakChanceOnMiss))
		{
			target.Location.Insert(ammo, true);
			if (emoteOnBreak != null)
			{
				target.OutputHandler.Handle(emoteOnBreak);
			}

			var result = ammo.Die();
			if (result != null)
			{
				result.RoomLayer = target.RoomLayer;
				target.Location.Insert(result);
				if (!result.Deleted)
				{
					result.AddEffect(new CombatNoGetEffect(result, actor.Combat), TimeSpan.FromSeconds(20));
					result.PositionTarget = target;
				}
			}

			return;
		}

		target.Location.Insert(ammo);
		ammo.PositionTarget = target;
		if (emoteOnFallToGround != null)
		{
			target.OutputHandler.Handle(emoteOnFallToGround);
		}


		// The call to insert the item into the location can result in a stack merge, in which case the ammo item is deleted and we should not further alter it
		if (actor.Combat != null && !ammo.Deleted)
		{
			ammo.AddEffect(new CombatNoGetEffect(ammo, actor.Combat), TimeSpan.FromSeconds(20));
		}
	}

	private void HandleAmmunitionScatterToCell(ICharacter actor, ICell cell, RoomLayer roomLayer, IGameItem ammo,
		IEmoteOutput emoteOnBreak = null, IEmoteOutput emoteOnFallToGround = null)
	{
		ammo.RoomLayer = roomLayer;

		if (RandomUtilities.Roll(1.0, AmmoType.BreakChanceOnMiss))
		{
			cell.Insert(ammo, true);
			if (emoteOnBreak != null)
			{
				cell.Handle(emoteOnBreak);
			}

			var result = ammo.Die();
			if (result != null)
			{
				result.RoomLayer = roomLayer;
				cell.Insert(result);
				if (!result.Deleted && actor.Combat != null)
				{
					result.AddEffect(new CombatNoGetEffect(result, actor.Combat), TimeSpan.FromSeconds(20));
				}
			}

			return;
		}

		cell.Insert(ammo);
		ammo.PositionTarget = null;
		if (emoteOnFallToGround != null)
		{
			cell.Handle(emoteOnFallToGround);
		}

		if (actor.Combat != null && !ammo.Deleted)
		{
			ammo.AddEffect(new CombatNoGetEffect(ammo, actor.Combat), TimeSpan.FromSeconds(20));
		}
	}

	private void BroadcastProjectileFlight(ICharacter actor, IPerceivable destination, IGameItem ammo,
		IReadOnlyList<ICellExit> precomputedPath = null)
	{
		if (actor?.Location == null || destination?.Location == null)
		{
			return;
		}

		var path = precomputedPath ?? actor.PathBetween(destination, 10, false, false, true)?.ToList() ??
				   new List<ICellExit>();
		var directions = path.Select(x => x.OutboundDirection).ToList();
		var dirDesc = directions.DescribeDirection();
		var oppDirDesc = directions.DescribeOppositeDirection();
		var actionDescription = "fly|flies overhead";
		var actionDescriptionTargetRoom = "fly|flies in";
		var flags = OutputFlags.InnerWrap;
		switch (AmmoType.EchoType)
		{
			case AmmunitionEchoType.Arcing:
				flags |= OutputFlags.NoticeCheckRequired;
				break;
			case AmmunitionEchoType.Laser:
				actionDescription = "flash|flashes through the area";
				actionDescriptionTargetRoom = "flash|flashes in";
				break;
			case AmmunitionEchoType.Subsonic:
				actionDescription = "fly|flies through the area";
				break;
			case AmmunitionEchoType.Supersonic:
				actionDescription = "whizz|whizzes past";
				actionDescriptionTargetRoom = "whizz|whizzes in";
				flags |= OutputFlags.PurelyAudible;
				break;
		}

		foreach (var cell in actor.CellsUnderneathFlight(destination, 10))
		{
			cell.Handle(
				new EmoteOutput(
					new Emote($"@ {actionDescription} from the {oppDirDesc} towards the {dirDesc}", ammo),
					style: OutputStyle.CombatMessage, flags: flags)
				{ NoticeCheckDifficulty = Difficulty.VeryHard });
		}

		if (destination is not IPerceiver destinationPerceiver)
		{
			return;
		}

		if (!Equals(actor.Location, destinationPerceiver.Location))
		{
			destinationPerceiver.OutputHandler.Handle(new EmoteOutput(
				new Emote($"$0 {actionDescriptionTargetRoom} from the {oppDirDesc}.", destinationPerceiver, ammo),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			return;
		}

		if (actor.RoomLayer == destinationPerceiver.RoomLayer)
		{
			return;
		}

		var relativeDirection = destinationPerceiver.RoomLayer.IsHigherThan(actor.RoomLayer) ? "below" : "above";
		destinationPerceiver.OutputHandler.Handle(new EmoteOutput(
			new Emote($"$0 {actionDescriptionTargetRoom} from {relativeDirection}.", destinationPerceiver, ammo),
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
		IGameItem ammo, IReadOnlyList<ICellExit> path, string context)
	{
		var scatterResult = RangedScatterStrategyFactory.GetStrategy(weaponType)
			.GetScatterTarget(actor, originalTarget, path ?? Array.Empty<ICellExit>());

		if (scatterResult == null)
		{
			Gameworld.DebugMessage(
				$"[Scatter:{context}] {actor.HowSeen(actor)} had no valid ricochet destinations from {originalTarget?.HowSeen(actor) ?? "unknown target"}.");
			return false;
		}

		ResolveScatterResult(actor, weaponType, ammo, scatterResult, context);
		return true;
	}

	private void ResolveScatterResult(ICharacter actor, IRangedWeaponType weaponType, IGameItem ammo,
		RangedScatterResult scatterResult, string context)
	{
		if (scatterResult.Target != null)
		{
			var scatterPath = actor.PathBetween(scatterResult.Target, 10, false, false, true)?.ToList() ??
			                  new List<ICellExit>();
			BroadcastProjectileFlight(actor, scatterResult.Target, ammo, scatterPath);
			var bodypart = (scatterResult.Target as IHaveABody)?.Body?.RandomBodyPartGeometry(Orientation.Centre,
				Alignment.Front, Facing.Front);
			Hit(actor, scatterResult.Target, Outcome.Pass, Outcome.Pass,
				new OpposedOutcome(OpposedOutcomeDirection.Proponent, OpposedOutcomeDegree.Marginal), bodypart, ammo,
				weaponType, null);
			Gameworld.DebugMessage(
				$"[Scatter:{context}] Ricochet struck {scatterResult.Target.HowSeen(actor)} in {scatterResult.Cell.HowSeen(actor)} after deviating{ScatterStrategyUtilities.DescribeFromDirection(scatterResult.DirectionFromTarget)} (distance {scatterResult.DistanceFromTarget:N0}).");
			return;
		}

		var dummy = new DummyPerceiver(location: scatterResult.Cell)
		{
			RoomLayer = scatterResult.RoomLayer
		};
		var scatterCellPath = actor.PathBetween(dummy, 10, false, false, true)?.ToList() ?? new List<ICellExit>();
		BroadcastProjectileFlight(actor, dummy, ammo, scatterCellPath);
		var directionText = ScatterStrategyUtilities.DescribeFromDirection(scatterResult.DirectionFromTarget);
		var breakOutput = new EmoteOutput(
			new Emote($"$0 ricochets{directionText} and shatters!", dummy, ammo),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap);
		var fallOutput = new EmoteOutput(
			new Emote($"$0 ricochets{directionText} and falls to the ground.", dummy, ammo),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap);
		HandleAmmunitionScatterToCell(actor, scatterResult.Cell, scatterResult.RoomLayer, ammo, breakOutput,
			fallOutput);
		Gameworld.DebugMessage(
			$"[Scatter:{context}] Ricochet landed in {scatterResult.Cell.HowSeen(actor)}{directionText} without hitting a new target.");
	}

	private bool TryResolveCoverInterception(ICharacter actor, IPerceiver target, Outcome shotOutcome,
		Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IGameItem ammo,
		IRangedWeaponType weaponType, IReadOnlyList<ICellExit> path)
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

		BroadcastProjectileFlight(actor, target, ammo, path);
		var coverItem = target.Cover.CoverItem?.Parent;
		var actorText = actor.HowSeen(actor);
		var targetText = target.HowSeen(actor);
		var coverText = coverItem?.HowSeen(actor) ?? "environmental cover";
		Gameworld.DebugMessage(
			$"[Ranged] Cover interception: {actorText} vs {targetText} (shot {shotOutcome}, cover {coverOutcome}) stopped by {coverText}.");
		target.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"The {ammo.Name.ToLowerInvariant()} strikes $?1|$1, ||$$0's cover!", target, target,
					coverItem), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		actor.Send("You hit your target's cover instead.".Colour(Telnet.Yellow));

		var damage = BuildDamage(actor, target, bodypart, ammo, weaponType, defenseOutcome);
		var wounds = new List<IWound>();
		wounds.AddRange(coverItem?.PassiveSufferDamage(damage) ?? Enumerable.Empty<IWound>());
		wounds.ProcessPassiveWounds();

		if (wounds.Any(x => x.Lodged == ammo))
		{
			return true;
		}

		var scatterContext = $"cover {actorText}->{targetText}";
		if (ShouldAttemptScatter(shotOutcome, actor, scatterContext) &&
		    TryResolveScatter(actor, target, weaponType, ammo, path, scatterContext))
		{
			return true;
		}

		HandleAmmunitionAftermath(actor, target, ammo);
		Gameworld.DebugMessage(
			$"[Ranged] Cover fully absorbed the shot from {actorText} to {targetText}; ammunition resolved at the target.");
		return true;
	}

	private bool TryResolveMiss(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
		OpposedOutcome defenseOutcome, IGameItem ammo, IRangedWeaponType weaponType, IEmoteOutput defenseEmote,
		IReadOnlyList<ICellExit> path)
	{
		if (shotOutcome.IsPass() && defenseOutcome.Outcome != OpposedOutcomeDirection.Opponent)
		{
			return false;
		}

		BroadcastProjectileFlight(actor, target, ammo, path);

		if (defenseEmote != null)
		{
			target.OutputHandler.Handle(defenseEmote);
		}

		target.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"$0 {(shotOutcome.IsPass() ? "narrowly misses @!" : "misses @ by a wide margin.")}", target,
					ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

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
			    TryResolveScatter(actor, target, weaponType, ammo, path, scatterContext))
			{
				return true;
			}
		}

		HandleAmmunitionAftermath(actor, target, ammo);
		Gameworld.DebugMessage($"[Ranged] Miss resolved with ammunition falling near {targetText}.");
		return true;
	}

	private bool TryResolveObstruction(ICharacter actor, IPerceiver target, IGameItem ammo,
		IRangedWeaponType weaponType, OpposedOutcome defenseOutcome, IBodypart bodypart, IReadOnlyList<ICellExit> path)
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
			$"[Ranged] Obstruction: {actorText}'s shot at {targetText} intercepted by {obstructionText}.");
		if (obstructionPerceiver != null)
		{
			var obstructionPath = actor.PathBetween(obstructionPerceiver, 10, false, false, true)?.ToList() ??
								  new List<ICellExit>();
			BroadcastProjectileFlight(actor, obstructionPerceiver, ammo, obstructionPath);
		}
		else
		{
			BroadcastProjectileFlight(actor, target, ammo, path);
		}

		target.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"The {ammo.Name.ToLowerInvariant()} strikes $1 instead of $0!", target, target, obstruction),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		actor.Send($"You hit {obstruction.HowSeen(actor)} instead!");

		var damage = BuildDamage(actor, target, bodypart, ammo, weaponType, defenseOutcome);
		var wounds = new List<IWound>();

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

			wounds.AddRange(ihw.PassiveSufferDamage(damage) ?? Enumerable.Empty<IWound>());
			wounds.ProcessPassiveWounds();
		}

		if (wounds.All(x => x.Lodged != ammo))
		{
			if (obstructionPerceiver != null)
			{
				HandleAmmunitionAftermath(actor, obstructionPerceiver, ammo);
			}
			else
			{
				HandleAmmunitionAftermath(actor, target, ammo);
			}
			Gameworld.DebugMessage(
				$"[Ranged] Obstruction aftermath: ammunition resolved at {(obstructionPerceiver != null ? obstructionText : targetText)}.");
		}

		return true;
	}

	private Damage BuildDamage(ICharacter actor, IPerceiver target, IBodypart bodypart,
		IGameItem ammo, IRangedWeaponType weaponType, OpposedOutcome defenseOutcome)
	{
		AmmoType.DamageProfile.DamageExpression.Formula.Parameters["quality"] = (int)ammo.Quality;
		AmmoType.DamageProfile.DamageExpression.Formula.Parameters["degree"] = (int)defenseOutcome.Degree;
		AmmoType.DamageProfile.DamageExpression.Formula.Parameters["pointblank"] = actor == target ? 1 : 0;
		AmmoType.DamageProfile.DamageExpression.Formula.Parameters["inmelee"] = actor.MeleeRange ? 1 : 0;
		AmmoType.DamageProfile.DamageExpression.Formula.Parameters["range"] = target.DistanceBetween(actor, 10);
		AmmoType.DamageProfile.PainExpression.Formula.Parameters["quality"] = (int)ammo.Quality;
		AmmoType.DamageProfile.PainExpression.Formula.Parameters["degree"] = (int)defenseOutcome.Degree;
		AmmoType.DamageProfile.PainExpression.Formula.Parameters["pointblank"] = actor == target ? 1 : 0;
		AmmoType.DamageProfile.PainExpression.Formula.Parameters["inmelee"] = actor.MeleeRange ? 1 : 0;
		AmmoType.DamageProfile.PainExpression.Formula.Parameters["range"] = target.DistanceBetween(actor, 10);
		AmmoType.DamageProfile.StunExpression.Formula.Parameters["quality"] = (int)ammo.Quality;
		AmmoType.DamageProfile.StunExpression.Formula.Parameters["degree"] = (int)defenseOutcome.Degree;
		AmmoType.DamageProfile.StunExpression.Formula.Parameters["pointblank"] = actor == target ? 1 : 0;
		AmmoType.DamageProfile.StunExpression.Formula.Parameters["inmelee"] = actor.MeleeRange ? 1 : 0;
		AmmoType.DamageProfile.StunExpression.Formula.Parameters["range"] = target.DistanceBetween(actor, 10);

		weaponType.DamageBonusExpression.Formula.Parameters["range"] = target.DistanceBetween(actor, 10);
		weaponType.DamageBonusExpression.Formula.Parameters["quality"] = (int)Parent.Quality;
		weaponType.DamageBonusExpression.Formula.Parameters["degree"] = (int)defenseOutcome.Degree;
		weaponType.DamageBonusExpression.Formula.Parameters["pointblank"] = actor == target ? 1 : 0;
		weaponType.DamageBonusExpression.Formula.Parameters["inmelee"] = actor.MeleeRange ? 1 : 0;

		var finalDamage = AmmoType.DamageProfile.DamageExpression.Evaluate(actor) +
						  weaponType.DamageBonusExpression.Evaluate(actor, weaponType.FireTrait);
		var finalPain = AmmoType.DamageProfile.PainExpression.Evaluate(actor);
		var finalStun = AmmoType.DamageProfile.StunExpression.Evaluate(actor);
		return new Damage
		{
			ActorOrigin = actor,
			ToolOrigin = Parent,
			Bodypart = bodypart,
			DamageAmount = finalDamage,
			DamageType = AmmoType.DamageProfile.DamageType,
			PainAmount = finalPain,
			StunAmount = finalStun,
			LodgableItem = ammo
		};
	}

	protected virtual void Hit(ICharacter actor, IPerceiver target, Outcome shotOutcome,
		Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IGameItem ammo,
		IRangedWeaponType weaponType, IEmoteOutput defenseEmote)
	{
		var damage = BuildDamage(actor, target, bodypart, ammo, weaponType, defenseOutcome);
		var wounds = new List<IWound>();
		var actorText = actor.HowSeen(actor);
		var targetText = target.HowSeen(actor);
		var locationText = target.Location?.HowSeen(actor) ?? "unknown location";
		Gameworld.DebugMessage(
			$"[Ranged] Hit resolved: {actorText} struck {targetText} at {locationText} (shot {shotOutcome}, cover {coverOutcome}, defense {defenseOutcome.Outcome}:{defenseOutcome.Degree}).");

		if (defenseEmote != null)
		{
			target.OutputHandler.Handle(defenseEmote);
		}

		if (!target.ColocatedWith(actor))
		{
			actor.Send("You hit your target.".Colour(Telnet.BoldGreen));
		}

		var targetHB = target as IHaveABody;
		if (targetHB?.Body != null)
		{
			wounds.AddRange(targetHB.Body.PassiveSufferDamage(damage));
			if (!wounds.Any())
			{
				HandleAmmunitionAftermath(actor, target, ammo, true,
						new EmoteOutput(new Emote($"$1 hits $0 on &0's {bodypart.FullDescription()} and breaks!",
								target, target, ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap),
						new EmoteOutput(new Emote($"$1 hits $0 on &0's {bodypart.FullDescription()} but ricochets off without causing any damage!",
								target, target, ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap)
				);
				return;
			}

			if (wounds.Any(x => x.Lodged == ammo))
			{
				var lodgedWound = wounds.First(x => x.Lodged == ammo);
				if (lodgedWound.Parent == target)
				{
					target.OutputHandler.Handle(
							new EmoteOutput(new Emote($"$0 lodges in $1's {lodgedWound.Bodypart.FullDescription()}!",
									target, ammo, target)));
				}
				else
				{
					target.OutputHandler.Handle(
							new EmoteOutput(new Emote($"$0 lodges in $1's !2!", target, ammo, target,
									lodgedWound.Parent)));
				}

				wounds.ProcessPassiveWounds();
				return;
			}

			HandleAmmunitionAftermath(actor, target, ammo, true,
					new EmoteOutput(new Emote($"$0 strikes $1's {bodypart.FullDescription()}, and then breaks!",
							target, ammo, target)),
					new EmoteOutput(new Emote($"$0 strikes $1's {bodypart.FullDescription()}, but falls to the ground!",
							target, ammo, target)));
			wounds.ProcessPassiveWounds();
			return;
		}

		if (target is IGameItem targetItem)
		{
			wounds.AddRange(targetItem.PassiveSufferDamage(damage));
			if (!wounds.Any())
			{
				HandleAmmunitionAftermath(actor, target, ammo, true,
						new EmoteOutput(new Emote(
								"$1 hit|hits $0 but ricochets off without causing any damage and shatters!",
								targetItem, targetItem, ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap),
						new EmoteOutput(new Emote("$1 hit|hits $0 but ricochets off without causing any damage!",
								targetItem, targetItem, ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
				return;
			}

			if (wounds.Any(x => x.Lodged == ammo))
			{
				target.OutputHandler.Handle(
						new EmoteOutput(new Emote($"$0 lodges in $1!", actor, ammo, targetItem)));
				wounds.ProcessPassiveWounds();
				return;
			}

			HandleAmmunitionAftermath(actor, target, ammo, true,
					new EmoteOutput(new Emote($"$0 strikes $1, and then breaks!",
							target, ammo, target)),
					new EmoteOutput(new Emote($"$0 strikes $1, but falls to the ground!",
							target, ammo, target)));
			wounds.ProcessPassiveWounds();
			return;
		}

		throw new NotImplementedException("Unknown target type in Fire.");
	}

	public virtual void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
		OpposedOutcome defenseOutcome, IBodypart bodypart, IGameItem ammo, IRangedWeaponType weaponType,
		IEmoteOutput defenseEmote)
	{
		// Ammunition that is just created and lodges can cause a crash if we don't flush now
		Gameworld.SaveManager.Flush();

		if (target == null)
		{
			// Fired at sky
			if (actor.Location.CurrentOverlay.OutdoorsType != CellOutdoorsType.Outdoors)
			{
				HandleAmmunitionAftermath(actor, actor, ammo, true,
					new EmoteOutput(new Emote("$1 $1|hit|hits the ceiling and shatters!", actor, actor, ammo)),
					new EmoteOutput(
						new Emote("$1 $1|hit|hits the ceiling and drops to the ground!", actor, actor, ammo))
				);
			}

			return;
		}

		var pathToTarget = actor.PathBetween(target, 10, false, false, true)?.ToList() ?? new List<ICellExit>();

		// Cover checks run first – they may absorb the shot or trigger a ricochet.
		if (TryResolveCoverInterception(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo,
				weaponType, pathToTarget))
		{
			return;
		}

		// Handle outright misses (bad shot or a successful defense), including scatter fall-through.
		if (TryResolveMiss(actor, target, shotOutcome, coverOutcome, defenseOutcome, ammo, weaponType, defenseEmote,
				pathToTarget))
		{
			return;
		}

		// Interposing effects redirect successful shots before they land on the original target.
		if (TryResolveObstruction(actor, target, ammo, weaponType, defenseOutcome, bodypart, pathToTarget))
		{
			return;
		}

		BroadcastProjectileFlight(actor, target, ammo, pathToTarget);
		Hit(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo, weaponType, defenseEmote);
	}

	#endregion
}
