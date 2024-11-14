using System;
using System.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health.Wounds;
using System.Collections.Generic;

namespace MudSharp.Health.Strategies;

public class GameItemHealthStrategy : BaseHealthStrategy
{
	private GameItemHealthStrategy(HealthStrategy strategy)
		: base(strategy)
	{
	}

	public static void RegisterHealthStrategyLoader()
	{
		RegisterHealthStrategy("GameItem", (strategy, game) => new GameItemHealthStrategy(strategy));
	}

	#region IHealthStrategy Members

	public override string HealthStrategyType => "GameItem";

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

		if (lodgedItem != null)
		{
			return new[]
			{
				new SimpleWound(owner.Gameworld, owner, damage.DamageAmount, damage.DamageType, null, lodgedItem,
					damage.ToolOrigin, damage.ActorOrigin)
			};
		}

		var existing = owner.Wounds.FirstOrDefault(x => x.DamageType == damage.DamageType);
		if (existing != null)
		{
			existing.SufferAdditionalDamage(damage);
			return new[] { existing };
		}

		return new[]
		{
			new SimpleWound(owner.Gameworld, owner, damage.DamageAmount, damage.DamageType, null, null,
				damage.ToolOrigin, damage.ActorOrigin)
		};
	}

	public override HealthTickResult PerformHealthTick(IHaveWounds thing)
	{
		var item = thing as IGameItem;

		var destroyable = item?.GetItemType<IDestroyable>();
		if (destroyable == null)
		{
			return HealthTickResult.None;
		}

		return destroyable.MaximumDamage > thing.Wounds.Sum(x => x.CurrentDamage)
			? HealthTickResult.None
			: HealthTickResult.Dead;
	}

	#region Overrides of BaseHealthStrategy

	public override HealthTickResult EvaluateStatus(IHaveWounds thing)
	{
		return PerformHealthTick(thing);
	}

	public override void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture)
	{
		// Do nothing
	}

	public override double MaxHP(IHaveWounds owner)
	{
		return ((IGameItem)owner).GetItemType<IDestroyable>()?.MaximumDamage ?? 0.0;
	}

	#endregion

	public override string ReportConditionPrompt(IHaveWounds owner, PromptType type)
	{
		// TODO - should this be possible? Should an exception be thrown?
		return "<Fine>";
	}

	public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.GameItem;

	public override bool IsCriticallyInjured(IHaveWounds owner)
	{
		return owner.Wounds.Sum(x => x.CurrentDamage * x.Bodypart?.DamageModifier) /
			(((IGameItem)owner).GetItemType<IDestroyable>()?.MaximumDamage ?? 1.0) > 0.9;
	}

	#endregion
}