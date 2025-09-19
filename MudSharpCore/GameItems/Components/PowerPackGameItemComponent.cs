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

		var path = actor.PathBetween(target, 10, false, false, true).ToList();
		var dirDesc = path.Select(x => x.OutboundDirection).DescribeDirection();
		var oppDirDesc = path.Select(x => x.OutboundDirection).DescribeOppositeDirection();
		var flags = OutputFlags.Normal;
		foreach (var cell in actor.CellsUnderneathFlight(target, 10))
		{
			cell.Handle(new EmoteOutput(
				new Emote($"A laser beam flashes through the area from the {oppDirDesc} towards the {dirDesc}", actor),
				flags: flags));
		}

		if (actor.Location != target.Location)
		{
			target.OutputHandler.Handle(new EmoteOutput(new Emote($"A laser beam flashes in from the {oppDirDesc}.",
				target)));
		}
		else if (actor.RoomLayer != target.RoomLayer)
		{
			target.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					$"A laser beam flashes in from {(target.RoomLayer.IsHigherThan(actor.RoomLayer) ? "below" : "above")}.",
					target), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		}

                var damage = BuildDamage(actor, target, bodypart, weaponType, defenseOutcome, painMultiplier, stunMultiplier);

		var wounds = new List<IWound>();
		if (shotOutcome.IsPass() && coverOutcome.IsFail() && target.Cover != null)
		{
			// Shot would've hit if it wasn't for cover
			var strikeCover = target.Cover.Cover.CoverType == CoverType.Hard || shotOutcome == Outcome.MajorPass ||
			                  coverOutcome == Outcome.MinorFail;
			if (strikeCover)
			{
				target.OutputHandler.Handle(
					new EmoteOutput(new Emote($"The beam strikes $?1|$1, ||$$0's cover!", target, target,
						target.Cover.CoverItem?.Parent)));
				actor.Send("You hit your target's cover instead.".Colour(Telnet.Yellow));
				wounds.AddRange(target.Cover?.CoverItem?.Parent.PassiveSufferDamage(damage) ??
				                Enumerable.Empty<IWound>());
				wounds.ProcessPassiveWounds();
				defenseOutcome = new OpposedOutcome(OpposedOutcomeDirection.Opponent, OpposedOutcomeDegree.Total);
				return;
			}
		}

		if (defenseOutcome.Outcome == OpposedOutcomeDirection.Opponent)
		{
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

                        var chance = 10 * (8 - (int)shotOutcome);
                        var mult = actor.Merits.OfType<IScatterChanceMerit>().Aggregate(1.0, (x, y) => x * y.ScatterMultiplier);
                        if (Dice.Roll(1, 100) <= chance * mult)
                        {
                                var scatterResult = RangedScatterStrategyFactory.GetStrategy(weaponType)
                                        .GetScatterTarget(actor, target, path);
                                if (scatterResult != null)
                                {
                                        if (scatterResult.Target != null)
                                        {
                                                Hit(actor, scatterResult.Target,
                                                        (scatterResult.Target as IHaveABody)?.Body.RandomBodyPartGeometry(Orientation.Centre, Alignment.Front, Facing.Front),
                                                        weaponType, new OpposedOutcome(OpposedOutcomeDirection.Proponent, OpposedOutcomeDegree.Marginal),
                                                        painMultiplier, stunMultiplier, null);
                                                return;
                                        }

                                        HandleBeamScatterToCell(scatterResult);
                                        return;
                                }
                        }

                        return;
                }

		foreach (var effect in target.EffectsOfType<IRangedObstructionEffect>().Where(x => x.Applies(actor)).Shuffle())
		{
			target.OutputHandler.Handle(
				new EmoteOutput(new Emote("The beam strikes $1 instead of $0!", target, target, effect.Obstruction),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
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

	#endregion
}