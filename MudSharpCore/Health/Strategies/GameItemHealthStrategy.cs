using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health.Wounds;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Health.Strategies;

public class GameItemHealthStrategy : BaseHealthStrategy
{
    public override HealthStateModel HealthStateModel => HealthStateModel.GameItem;

    private const string TypeBlurb =
        "A game item damage model that relies on destroyable item maximum damage and shared severity logic.";

    private static readonly string TypeHelp = BuildTypeHelp(TypeBlurb, Enumerable.Empty<string>());

    private GameItemHealthStrategy(HealthStrategy strategy, IFuturemud gameworld)
        : base(strategy, gameworld)
    {
    }

    private GameItemHealthStrategy(IFuturemud gameworld, string name)
        : base(gameworld, name)
    {
        DoDatabaseInsert(HealthStrategyType);
    }

    private GameItemHealthStrategy(GameItemHealthStrategy rhs, string name)
        : base(rhs, name)
    {
        DoDatabaseInsert(HealthStrategyType);
    }

    public static void RegisterHealthStrategyLoader()
    {
        RegisterHealthStrategy("GameItem",
            (strategy, game) => new GameItemHealthStrategy(strategy, game),
            (game, name) => new GameItemHealthStrategy(game, name),
            TypeHelp,
            TypeBlurb);
    }

    public override string HealthStrategyType => "GameItem";
    public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.GameItem;

    public override IHealthStrategy Clone(string name)
    {
        return new GameItemHealthStrategy(this, name);
    }

    protected override void SaveSubtypeDefinition(XElement root)
    {
    }

    public override IEnumerable<IWound> SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart)
    {
        if (damage.DamageType == DamageType.Hypoxia || damage.DamageType == DamageType.Cellular)
        {
            return Enumerable.Empty<IWound>();
        }

        IGameItem lodgedItem = CheckDamageLodges(damage) ? damage.LodgableItem : null;

        if (lodgedItem != null)
        {
            return
            [
                new SimpleWound(owner.Gameworld, owner, damage.DamageAmount, damage.DamageType, null, lodgedItem,
                    damage.ToolOrigin, damage.ActorOrigin)
            ];
        }

        IWound existing = owner.Wounds.FirstOrDefault(x => x.DamageType == damage.DamageType);
        if (existing != null && Dice.Roll(1, 6) == 1)
        {
            existing.SufferAdditionalDamage(damage);
            return [existing];
        }

        return
        [
            new SimpleWound(owner.Gameworld, owner, damage.DamageAmount, damage.DamageType, null, null,
                damage.ToolOrigin, damage.ActorOrigin)
        ];
    }

    public override HealthTickResult PerformHealthTick(IHaveWounds thing)
    {
        IGameItem item = thing as IGameItem;
        IDestroyable destroyable = item?.GetItemType<IDestroyable>();
        if (destroyable == null)
        {
            return HealthTickResult.None;
        }

        return destroyable.MaximumDamage > thing.Wounds.Sum(x => x.CurrentDamage)
            ? HealthTickResult.None
            : HealthTickResult.Dead;
    }

    public override HealthTickResult EvaluateStatus(IHaveWounds thing)
    {
        return PerformHealthTick(thing);
    }

    public override void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture)
    {
    }

    public override double MaxHP(IHaveWounds owner)
    {
        return ((IGameItem)owner).GetItemType<IDestroyable>()?.MaximumDamage ?? 0.0;
    }

    public override string ReportConditionPrompt(IHaveWounds owner, PromptType type)
    {
        return "<Fine>";
    }

    public override bool IsCriticallyInjured(IHaveWounds owner)
    {
        return owner.Wounds.Sum(x => x.CurrentDamage * x.Bodypart?.DamageModifier) /
               (((IGameItem)owner).GetItemType<IDestroyable>()?.MaximumDamage ?? 1.0) > 0.9;
    }
}
