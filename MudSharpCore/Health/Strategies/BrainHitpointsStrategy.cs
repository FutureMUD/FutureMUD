using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using MudSharp.Form.Material;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Health.Wounds;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.RPG.Checks;
using MudSharp.Body.Needs;

namespace MudSharp.Health.Strategies;

public class BrainHitpointsStrategy : BaseHealthStrategy
{
	private BrainHitpointsStrategy(Models.HealthStrategy strategy, IFuturemud gameworld)
		: base(strategy)
	{
		LoadDefinition(XElement.Parse(strategy.Definition), gameworld);
	}

	public override string HealthStrategyType => "BrainHitpoints";

	public ITraitExpression MaximumHitPointsExpression { get; set; }
	public ITraitExpression HealingTickDamageExpression { get; set; }
	public double PercentageHealthPerPenalty { get; set; }

	public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;

	public bool CheckPowerCore { get; set; }

	public bool CheckHeart { get; set; }

	public bool UseHypoxiaDamage { get; set; }

	public bool KnockoutOnCritical { get; set; }

	public TimeSpan KnockoutDuration { get; set; }

	public override bool RequiresSpinalCord => false;

	public static void RegisterHealthStrategyLoader()
	{
		RegisterHealthStrategy("BrainHitpoints", (strategy, game) => new BrainHitpointsStrategy(strategy, game));
	}

	private void LoadDefinition(XElement root, IFuturemud gameworld)
	{
		var element = root.Element("MaximumHitPointsExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"BrainHPHealthStrategy ID {Id} did not contain a MaximumHitPointsExpression element.");
		}

		if (!long.TryParse(element.Value, out var value))
		{
			throw new ApplicationException(
				$"BrainHPHealthStrategy ID {Id} had a MaximumHitPointsExpression element that did not contain an ID.");
		}

		MaximumHitPointsExpression = gameworld.TraitExpressions.Get(value);

		element = root.Element("HealingTickDamageExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"BrainHPHealthStrategy ID {Id} did not contain a HealingTickDamageExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"BrainHPHealthStrategy ID {Id} had a HealingTickDamageExpression element that did not contain an ID.");
		}

		PercentageHealthPerPenalty = double.Parse(root.Element("PercentageHealthPerPenalty")?.Value ?? "1.0");
		HealingTickDamageExpression = gameworld.TraitExpressions.Get(value);

		CheckPowerCore = bool.Parse(root.Element("CheckPowerCore")?.Value ?? "false");
		CheckHeart = bool.Parse(root.Element("CheckHeart")?.Value ?? "false");
		UseHypoxiaDamage = bool.Parse(root.Element("UseHypoxiaDamage")?.Value ?? "false");
		KnockoutOnCritical = bool.Parse(root.Element("KnockoutOnCritical")?.Value ?? "false");
		KnockoutDuration = TimeSpan.FromSeconds(double.Parse(root.Element("KnockoutDuration")?.Value ?? "240"));
	}

	public override double MaxHP(IHaveWounds owner)
	{
		return MaximumHitPointsExpression.Evaluate(owner as IPerceivableHaveTraits);
	}


	public override void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture)
	{
		var cOwner = (ICharacter)owner;
		foreach (var liquid in mixture.Instances)
		{
			if (liquid.Liquid.Drug != null)
			{
				cOwner.Body.Dose(liquid.Liquid.Drug, DrugVector.Injected,
					liquid.Amount * liquid.Liquid.DrugGramsPerUnitVolume);
			}

			if (liquid.Liquid.InjectionConsequence == LiquidInjectionConsequence.Benign)
			{
				continue;
			}

			switch (liquid.Liquid.InjectionConsequence)
			{
				case LiquidInjectionConsequence.Hydrating:
					cOwner.Body.FulfilNeeds(new NeedFulfiller
					{
						ThirstPoints = liquid.Liquid.DrinkSatiatedHoursPerLitre /
						               (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres),
						WaterLitres = liquid.Liquid.WaterLitresPerLitre /
						              (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres),
						Calories = liquid.Liquid.CaloriesPerLitre /
						           (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres),
						AlcoholLitres = liquid.Liquid.AlcoholLitresPerLitre /
						                (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres),
						SatiationPoints = liquid.Liquid.FoodSatiatedHoursPerLitre /
						                  (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres)
					});
					continue;
				case LiquidInjectionConsequence.BloodReplacement:
					if (!(liquid is BloodLiquidInstance blood))
					{
						continue;
					}

					cOwner.Body.CurrentBloodVolumeLitres +=
						liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres;

					if (cOwner.Body.Bloodtype?.IsCompatibleWithDonorBlood(blood.BloodType) != false)
					{
						continue;
					}

					break;
			}
		}
	}

	public override IEnumerable<IWound> SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart)
	{
		if (!UseHypoxiaDamage && damage.DamageType == DamageType.Hypoxia)
		{
			return Enumerable.Empty<IWound>();
		}

		if (!CheckHeart && damage.DamageType == DamageType.Cellular)
		{
			return Enumerable.Empty<IWound>();
		}

		IGameItem lodgedItem = null;
		LodgeDamageExpression.Parameters["damage"] = damage.DamageAmount;
		LodgeDamageExpression.Parameters["type"] = (int)damage.DamageType;
		if (damage.DamageType.CanLodge() && Dice.Roll(0, 100) < Convert.ToDouble(LodgeDamageExpression.Evaluate()))
		{
			lodgedItem = damage.LodgableItem;
		}

		return new[]
		{
			new HealingSimpleWound(owner.Gameworld, owner, damage.DamageAmount, damage.DamageType, damage.Bodypart,
				lodgedItem, damage.ToolOrigin, damage.ActorOrigin)
		};
	}

	public override HealthTickResult PerformHealthTick(IHaveWounds thing)
	{
		if (thing is not ICharacter character)
		{
			return HealthTickResult.None;
		}

		if (CheckHeart && character.Body.OrganFunction<HeartProto>() <= 0.0 && character.State.IsUnconscious())
		{
			var damagePart = character.Body.Organs.FirstOrDefault() ?? character.Body.Bodyparts.GetRandomElement();
			SufferDamage(thing, new Damage
			{
				DamageAmount = character.Gameworld.GetStaticDouble("BrainConstructHeartAttackDamagePerTick"),
				DamageType = UseHypoxiaDamage ? DamageType.Hypoxia : DamageType.Cellular
			}, damagePart);
		}

		return EvaluateStatus(thing);
	}

	public override HealthTickResult EvaluateStatus(IHaveWounds thing)
	{
		if (thing is not ICharacter character)
		{
			return HealthTickResult.None;
		}

		if (character.State.HasFlag(CharacterState.Dead))
		{
			character.EndHealthTick();
			return HealthTickResult.Dead;
		}

		if (character.Body.OrganFunction<BrainProto>() <= 0.0)
		{
			return HealthTickResult.Dead;
		}

		if (MaximumHitPointsExpression.Evaluate(character) <= thing.Wounds.Sum(x => x.CurrentDamage))
		{
			return HealthTickResult.Dead;
		}

		if (CheckHeart && character.Body.OrganFunction<HeartProto>() <= 0.0)
		{
			return HealthTickResult.Unconscious;
		}

		if (KnockoutOnCritical)
		{
			if (character.Wounds.Sum(x => x.CurrentDamage) / MaximumHitPointsExpression.Evaluate(character) > 0.9)
			{
				if (!character.AffectedBy<CriticalInjureKnockout>())
				{
					character.AddEffect(new CriticalInjureKnockout(character, DateTime.UtcNow + KnockoutDuration));
					return HealthTickResult.PassOut;
				}

				return character.EffectsOfType<CriticalInjureKnockout>().First().WakeupTime > DateTime.UtcNow
					? HealthTickResult.PassOut
					: HealthTickResult.None;
			}
			else
			{
				character.RemoveAllEffects<CriticalInjureKnockout>(fireRemovalAction: true);
			}
		}

		return HealthTickResult.None;
	}

	public override bool IsCriticallyInjured(IHaveWounds owner)
	{
		return owner.Wounds.Sum(x => x.CurrentDamage) /
		       MaximumHitPointsExpression.Evaluate((ICharacter)owner) > 0.9 &&
		       owner is ICharacter ch &&
		       ch.State.HasFlag(CharacterState.Unconscious);
	}

	public override string ReportConditionPrompt(IHaveWounds owner, PromptType type)
	{
		if (owner is not ICharacter character)
		{
			return "Fine";
		}

		var statusString = "";
		if (CheckHeart && character.Body.OrganFunction<HeartProto>() <= 0.0)
		{
			statusString = $" - {"Cardiac Arrest".Colour(Telnet.BoldRed)}";
		}

		var totalWounds = MaximumHitPointsExpression.Evaluate(character);
		return string.Format(character, "Hp: {0:N0}/{1:N0}{2}",
			totalWounds - owner.Wounds.Sum(x => x.CurrentDamage), totalWounds, statusString);
	}

	public override double GetHealingTickAmount(IWound wound, Outcome outcome, HealthDamageType type)
	{
		ITraitExpression whichExpression;
		switch (type)
		{
			case HealthDamageType.Damage:
				whichExpression = HealingTickDamageExpression;
				break;
			case HealthDamageType.Pain:
			case HealthDamageType.Shock:
			case HealthDamageType.Stun:
			default:
				return 0;
		}

		whichExpression.Formula.Parameters["originaldamage"] = wound.OriginalDamage;
		whichExpression.Formula.Parameters["damage"] = wound.CurrentDamage;
		whichExpression.Formula.Parameters["outcome"] = outcome.SuccessDegrees();
		return whichExpression.Evaluate((ICharacter)wound.Parent);
	}

	public override double WoundPenaltyFor(IHaveWounds owner)
	{
		if (owner is not ICharacter charOwner)
		{
			return 0;
		}

		var penalty =
			charOwner.Wounds.Sum(x => x.CurrentDamage / PercentageHealthPerPenalty) /
			MaximumHitPointsExpression.Evaluate(charOwner);
		return -1 * penalty;
	}
}