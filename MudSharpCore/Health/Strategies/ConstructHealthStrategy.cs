using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health.Wounds;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Health.Strategies;

public class ConstructHealthStrategy : BaseHealthStrategy
{
    public override HealthStateModel HealthStateModel => HealthStateModel.Construct;

    private static readonly TraitExpressionBuilderField<ConstructHealthStrategy>[] TraitExpressionFields =
    [
        new("MaximumHitPointsExpression", ["maxhp", "maximumhitpointsexpression"], "Maximum Hit Points Expression",
            x => x.MaximumHitPointsExpression, (x, value) => x.MaximumHitPointsExpression = value)
    ];

    private static readonly DoubleBuilderField<ConstructHealthStrategy>[] DoubleFields =
    [
        PercentageField<ConstructHealthStrategy>("CriticalInjuryThreshold", ["criticalinjurythreshold"], "Critical Injury Threshold",
            x => x.CriticalInjuryThreshold, (x, value) => x.CriticalInjuryThreshold = value)
    ];

    private const string TypeBlurb =
        "A non-organic construct model that ignores hypoxia and cellular damage and dies when its hit points are exhausted.";

    private static readonly string TypeHelp = BuildTypeHelp(TypeBlurb,
        GetBuilderFieldHelpText(TraitExpressionFields)
            .Concat(GetBuilderFieldHelpText(DoubleFields)));

    private ConstructHealthStrategy(HealthStrategy strategy, IFuturemud gameworld)
        : base(strategy, gameworld)
    {
        LoadDefinition(XElement.Parse(strategy.Definition), gameworld);
    }

    private ConstructHealthStrategy(IFuturemud gameworld, string name)
        : base(gameworld, name)
    {
        MaximumHitPointsExpression = CreateDefaultExpression(gameworld, $"{name} Max HP", "100");
        CriticalInjuryThreshold = 0.9;
        DoDatabaseInsert(HealthStrategyType);
    }

    private ConstructHealthStrategy(ConstructHealthStrategy rhs, string name)
        : base(rhs, name)
    {
        MaximumHitPointsExpression = CloneExpression(rhs.MaximumHitPointsExpression, Gameworld);
        CriticalInjuryThreshold = rhs.CriticalInjuryThreshold;
        DoDatabaseInsert(HealthStrategyType);
    }

    public override string HealthStrategyType => "Construct";
    public override bool RequiresSpinalCord => false;
    public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;

    public ITraitExpression MaximumHitPointsExpression { get; set; }
    public double CriticalInjuryThreshold { get; set; }

    protected override IEnumerable<string> SubtypeBuilderHelpText =>
        GetBuilderFieldHelpText(TraitExpressionFields)
            .Concat(GetBuilderFieldHelpText(DoubleFields));

    public static void RegisterHealthStrategyLoader()
    {
        RegisterHealthStrategy("Construct",
            (strategy, game) => new ConstructHealthStrategy(strategy, game),
            (game, name) => new ConstructHealthStrategy(game, name),
            TypeHelp,
            TypeBlurb);
    }

    private void LoadDefinition(XElement root, IFuturemud gameworld)
    {
        XElement element = root.Element("MaximumHitPointsExpression");
        if (element == null)
        {
            throw new ApplicationException(
                $"ConstructHealthStrategy ID {Id} did not contain a MaximumHitPointsExpression element.");
        }

        if (!long.TryParse(element.Value, out long value))
        {
            throw new ApplicationException(
                $"ConstructHealthStrategy ID {Id} had a MaximumHitPointsExpression element that did not contain an ID.");
        }

        MaximumHitPointsExpression = gameworld.TraitExpressions.Get(value);
        CriticalInjuryThreshold = LoadDouble(root, "CriticalInjuryThreshold", 0.9);
    }

    protected override void SaveSubtypeDefinition(XElement root)
    {
        SaveBuilderFields(root, this, TraitExpressionFields);
        SaveBuilderFields(root, this, DoubleFields);
    }

    protected override void AppendSubtypeShow(System.Text.StringBuilder sb, ICharacter actor)
    {
        sb.AppendLine();
        AppendBuilderFieldShow(sb, actor, this, TraitExpressionFields);
        AppendBuilderFieldShow(sb, actor, this, DoubleFields);
    }

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        if (TryBuildingCommand(actor, command.GetUndo(), TraitExpressionFields))
        {
            return true;
        }

        if (TryBuildingCommand(actor, command.GetUndo(), DoubleFields))
        {
            return true;
        }

        return base.BuildingCommand(actor, command.GetUndo());
    }

    public override IHealthStrategy Clone(string name)
    {
        return new ConstructHealthStrategy(this, name);
    }

    public override double MaxHP(IHaveWounds owner)
    {
        return MaximumHitPointsExpression.Evaluate(owner as IPerceivableHaveTraits);
    }

    public override void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture)
    {
    }

    public override IEnumerable<IWound> SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart)
    {
        if (damage.DamageType == DamageType.Hypoxia || damage.DamageType == DamageType.Cellular)
        {
            return Enumerable.Empty<IWound>();
        }

        IGameItem lodgedItem = CheckDamageLodges(damage) ? damage.LodgableItem : null;

        return
        [
            new SimpleWound(owner.Gameworld, owner, damage.DamageAmount, damage.DamageType, damage.Bodypart,
                lodgedItem, damage.ToolOrigin, damage.ActorOrigin)
        ];
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

    public override HealthTickResult EvaluateStatus(IHaveWounds thing)
    {
        return PerformHealthTick(thing);
    }

    public override bool IsCriticallyInjured(IHaveWounds owner)
    {
        return owner.Wounds.Sum(x => x.CurrentDamage * x.Bodypart?.DamageModifier) /
               MaximumHitPointsExpression.Evaluate((ICharacter)owner) > CriticalInjuryThreshold &&
               owner is ICharacter ch &&
               ch.State.HasFlag(CharacterState.Unconscious);
    }

    public override string ReportConditionPrompt(IHaveWounds owner, PromptType type)
    {
        if (owner is not ICharacter character)
        {
            return "<Fine>";
        }

        double totalWounds = MaximumHitPointsExpression.Evaluate(character);
        return string.Format(character, "<HP: {0:N0}/{1:N0}>",
            totalWounds - owner.Wounds.Sum(x => x.CurrentDamage * x.Bodypart?.DamageModifier), totalWounds);
    }
}
