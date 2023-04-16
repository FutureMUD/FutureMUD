using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health.Wounds;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;
using System.Collections.Generic;

namespace MudSharp.Health.Strategies;

public class ConstructHealthStrategy : BaseHealthStrategy
{
	private ConstructHealthStrategy(HealthStrategy strategy, IFuturemud gameworld)
		: base(strategy)
	{
		LoadDefinition(XElement.Parse(strategy.Definition), gameworld);
	}

	public override string HealthStrategyType => "Construct";

	public ITraitExpression MaximumHitPointsExpression { get; set; }

	public override bool RequiresSpinalCord => false;

	public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;

	public static void RegisterHealthStrategyLoader()
	{
		RegisterHealthStrategy("Construct", (strategy, game) => new ConstructHealthStrategy(strategy, game));
	}

	private void LoadDefinition(XElement root, IFuturemud gameworld)
	{
		var element = root.Element("MaximumHitPointsExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ConstructHealthStrategy ID {Id} did not contain a MaximumHitPointsExpression element.");
		}

		if (!long.TryParse(element.Value, out var value))
		{
			throw new ApplicationException(
				$"ConstructHealthStrategy ID {Id} had a MaximumHitPointsExpression element that did not contain an ID.");
		}

		MaximumHitPointsExpression = gameworld.TraitExpressions.Get(value);
	}

	#region Overrides of BaseHealthStrategy

	public override double MaxHP(IHaveWounds owner)
	{
		return MaximumHitPointsExpression.Evaluate(owner as IPerceivableHaveTraits);
	}

	#endregion

	public override void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture)
	{
		// Do nothing
	}

	public override IEnumerable<IWound> SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart)
	{
		if (damage.DamageType == DamageType.Hypoxia || damage.DamageType == DamageType.Cellular)
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
			new SimpleWound(owner.Gameworld, owner, damage.DamageAmount, damage.DamageType, damage.Bodypart,
				lodgedItem, damage.ToolOrigin, damage.ActorOrigin)
		};
	}

	public override HealthTickResult PerformHealthTick(IHaveWounds thing)
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

		return MaximumHitPointsExpression.Evaluate(character) > thing.Wounds.Sum(x => x.CurrentDamage)
			? HealthTickResult.None
			: HealthTickResult.Dead;
	}

	#region Overrides of BaseHealthStrategy

	public override HealthTickResult EvaluateStatus(IHaveWounds thing)
	{
		return PerformHealthTick(thing);
	}

	public override bool IsCriticallyInjured(IHaveWounds owner)
	{
		return owner.Wounds.Sum(x => x.CurrentDamage * x.Bodypart?.DamageModifier) /
		       MaximumHitPointsExpression.Evaluate((ICharacter)owner) > 0.9 && owner is ICharacter ch &&
		       ch.State.HasFlag(CharacterState.Unconscious);
	}

	#endregion

	public override string ReportConditionPrompt(IHaveWounds owner, PromptType type)
	{
		if (owner is not ICharacter character)
		{
			return "Fine";
		}

		var totalWounds = MaximumHitPointsExpression.Evaluate(character);
		return string.Format(character, "Hp: {0:N0}/{1:N0}",
			totalWounds - owner.Wounds.Sum(x => x.CurrentDamage * x.Bodypart?.DamageModifier), totalWounds);
	}
}