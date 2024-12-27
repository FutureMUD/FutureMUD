using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
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

			// Do nothing
			return;
		}

		var path = actor.PathBetween(target, 10, false, false, true);
		var dirDesc = path.Select(x => x.OutboundDirection).DescribeDirection();
		var oppDirDesc = path.Select(x => x.OutboundDirection).DescribeOppositeDirection();
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

		foreach (var cell in actor.CellsUnderneathFlight(target, 10))
		{
			cell.Handle(
				new EmoteOutput(new Emote($"@ {actionDescription} from the {oppDirDesc} towards the {dirDesc}", ammo),
					style: OutputStyle.CombatMessage, flags: flags) { NoticeCheckDifficulty = Difficulty.VeryHard });
		}

		if (actor.Location != target.Location)
		{
			target.OutputHandler.Handle(new EmoteOutput(
				new Emote($"$0 {actionDescriptionTargetRoom} from the {oppDirDesc}.", target, ammo),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		}
		else if (actor.RoomLayer != target.RoomLayer)
		{
			target.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					$"$0 {actionDescriptionTargetRoom} from {(target.RoomLayer.IsHigherThan(actor.RoomLayer) ? "below" : "above")}.",
					target, ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		}

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
		var damage = new Damage
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

		var wounds = new List<IWound>();
		if (shotOutcome.IsPass() && coverOutcome.IsFail() && target.Cover != null)
		{
			// Shot would've hit if it wasn't for cover
			var strikeCover = target.Cover.Cover.CoverType == CoverType.Hard || shotOutcome == Outcome.MajorPass ||
			                  coverOutcome == Outcome.MinorFail;
			if (strikeCover)
			{
				target.OutputHandler.Handle(
					new EmoteOutput(new Emote($"The {ammo.Name.ToLowerInvariant()} strikes $?1|$1, ||$$0's cover!",
							target, target,
							target.Cover.CoverItem?.Parent), style: OutputStyle.CombatMessage,
						flags: OutputFlags.InnerWrap));
				actor.Send("You hit your target's cover instead.".Colour(Telnet.Yellow));
				wounds.AddRange(target.Cover?.CoverItem?.Parent.PassiveSufferDamage(damage) ??
				                Enumerable.Empty<IWound>());
				wounds.ProcessPassiveWounds();
				if (wounds.All(x => x.Lodged != ammo))
				{
					HandleAmmunitionAftermath(actor, target, ammo);
				}

				return;
			}
		}

		if (defenseOutcome.Outcome == OpposedOutcomeDirection.Opponent)
		{
			if (defenseEmote != null)
			{
				target.OutputHandler.Handle(defenseEmote);
			}

			target.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"$0 {(shotOutcome.IsPass() ? "narrowly misses @!" : "misses @ by a wide margin.")}",
						target, ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			if (!actor.ColocatedWith(target))
			{
				actor.Send("You missed your target.".Colour(Telnet.Red));
			}

			if (wounds.All(x => x.Lodged != ammo))
			{
				HandleAmmunitionAftermath(actor, target, ammo);
			}

			return;
		}

		foreach (var effect in target.EffectsOfType<IRangedObstructionEffect>().Where(x => x.Applies(actor)).Shuffle())
		{
			target.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"The {ammo.Name.ToLowerInvariant()} strikes $1 instead of $0!", target, target,
						effect.Obstruction), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			actor.Send($"You hit {effect.Obstruction.HowSeen(actor)} instead!");
			if (effect.Obstruction is IHaveWounds ihw)
			{
				if (ihw is IHaveABody ihab)
				{
					var oldTarget = damage.Bodypart;
					damage = new Damage(damage)
					{
						Bodypart = ihab.Body.RandomBodyPartGeometry(oldTarget?.Orientation ?? Orientation.Centre,
							Alignment.Front, Facing.Front, true)
					};
				}

				wounds.AddRange(ihw.PassiveSufferDamage(damage) ??
				                Enumerable.Empty<IWound>());
				wounds.ProcessPassiveWounds();
			}

			if (wounds.All(x => x.Lodged != ammo))
			{
				HandleAmmunitionAftermath(actor, target, ammo);
			}

			return;
		}

		if (defenseEmote != null)
		{
			target.OutputHandler.Handle(defenseEmote);
		}

		if (!target.ColocatedWith(actor))
		{
			actor.Send("You hit your target.".Colour(Telnet.BoldGreen));
		}

		var targetHB = target as IHaveABody;
		var targetMortal = (IMortalPerceiver)target;
		if (targetHB?.Body != null)
		{
			wounds.AddRange(targetHB.Body.PassiveSufferDamage(damage));
			if (!wounds.Any())
			{
				HandleAmmunitionAftermath(actor, target, ammo, true,
					new EmoteOutput(
						new Emote(
							$"$1 hits $0 on &0's {bodypart.FullDescription()} and breaks!",
							target, target, ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap),
					new EmoteOutput(
						new Emote(
							$"$1 hits $0 on &0's {bodypart.FullDescription()} but ricochets off without causing any damage!",
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
						new EmoteOutput(
							new Emote(
								$"$0 lodges in $1's {lodgedWound.Bodypart.FullDescription()}!",
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

	#endregion
}