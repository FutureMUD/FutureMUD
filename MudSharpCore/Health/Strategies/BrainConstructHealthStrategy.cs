using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health.Wounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;

namespace MudSharp.Health.Strategies;

public class BrainConstructHealthStrategy : BaseHealthStrategy
{
	private BrainConstructHealthStrategy(Models.HealthStrategy strategy, IFuturemud gameworld)
		: base(strategy)
	{
		LoadDefinition(XElement.Parse(strategy.Definition), gameworld);
	}

	public override string HealthStrategyType => "BrainConstruct";

	public ITraitExpression MaximumHitPointsExpression { get; set; }

	public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;

	public bool CheckPowerCore { get; set; }

	public bool CheckHeart { get; set; }

	public bool UseHypoxiaDamage { get; set; }

	public override bool RequiresSpinalCord => false;

	public static void RegisterHealthStrategyLoader()
	{
		RegisterHealthStrategy("BrainConstruct", (strategy, game) => new BrainConstructHealthStrategy(strategy, game));
	}

	private void LoadDefinition(XElement root, IFuturemud gameworld)
	{
		var element = root.Element("MaximumHitPointsExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"BrainConstructHealthStrategy ID {Id} did not contain a MaximumHitPointsExpression element.");
		}

		if (!long.TryParse(element.Value, out var value))
		{
			throw new ApplicationException(
				$"BrainConstructHealthStrategy ID {Id} had a MaximumHitPointsExpression element that did not contain an ID.");
		}

		MaximumHitPointsExpression = gameworld.TraitExpressions.Get(value);

		CheckPowerCore = bool.Parse(root.Element("CheckPowerCore")?.Value ?? "false");
		CheckHeart = bool.Parse(root.Element("CheckHeart")?.Value ?? "false");
		UseHypoxiaDamage = bool.Parse(root.Element("UseHypoxiaDamage")?.Value ?? "false");
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

	public override IWound SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart)
	{
		if (!UseHypoxiaDamage && damage.DamageType == DamageType.Hypoxia)
		{
			return null;
		}

		if (!CheckHeart && damage.DamageType == DamageType.Cellular)
		{
			return null;
		}

		IGameItem lodgedItem = null;
		LodgeDamageExpression.Parameters["damage"] = damage.DamageAmount;
		LodgeDamageExpression.Parameters["type"] = (int)damage.DamageType;
		if (damage.DamageType.CanLodge() && Dice.Roll(0, 100) < Convert.ToDouble(LodgeDamageExpression.Evaluate()))
		{
			lodgedItem = damage.LodgableItem;
		}

		return new SimpleWound(owner.Gameworld, owner, damage.DamageAmount, damage.DamageType, damage.Bodypart,
			lodgedItem, damage.ToolOrigin, damage.ActorOrigin);
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

	#region Overrides of BaseHealthStrategy

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

		if (character.Body.OrganFunction<BrainProto>() <= 0.0 && character.Body.OrganFunction<PositronicBrain>() <= 0.0)
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

		if (CheckPowerCore && character.Body.OrganFunction<PowerCore>() <= 0.0)
		{
			return HealthTickResult.Unconscious;
		}

		return HealthTickResult.None;
	}

	public override bool IsCriticallyInjured(IHaveWounds owner)
	{
		return owner.Wounds.Sum(x => x.CurrentDamage * x.Bodypart?.DamageModifier) /
		       MaximumHitPointsExpression.Evaluate((ICharacter)owner) > 0.9 &&
		       owner is ICharacter ch &&
		       ch.State.HasFlag(CharacterState.Unconscious);
	}

	#endregion

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

		if (CheckPowerCore && character.Body.OrganFunction<PowerCore>() <= 0.0)
		{
			statusString = $" - {"Power Core Failure".Colour(Telnet.BoldMagenta)}";
		}

		var totalWounds = MaximumHitPointsExpression.Evaluate(character);
		return string.Format(character, "Hp: {0:N0}/{1:N0}{2}",
			totalWounds - owner.Wounds.Sum(x => x.CurrentDamage * x.Bodypart?.DamageModifier), totalWounds,
			statusString);
	}
}