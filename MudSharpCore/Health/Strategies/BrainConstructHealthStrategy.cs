using MudSharp.Body;
using MudSharp.Body.PartProtos;
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

public class BrainConstructHealthStrategy : BaseHealthStrategy
{
    private static readonly TraitExpressionBuilderField<BrainConstructHealthStrategy>[] TraitExpressionFields =
    [
        new("MaximumHitPointsExpression", ["maxhp", "maximumhitpointsexpression"], "Maximum Hit Points Expression",
            x => x.MaximumHitPointsExpression, (x, value) => x.MaximumHitPointsExpression = value)
    ];

    private static readonly BoolBuilderField<BrainConstructHealthStrategy>[] BoolFields =
    [
        new("CheckPowerCore", ["checkpowercore"], "Check Power Core",
            x => x.CheckPowerCore, (x, value) => x.CheckPowerCore = value),
        new("CheckHeart", ["checkheart"], "Check Heart",
            x => x.CheckHeart, (x, value) => x.CheckHeart = value),
        new("UseHypoxiaDamage", ["usehypoxiadamage"], "Use Hypoxia Damage",
            x => x.UseHypoxiaDamage, (x, value) => x.UseHypoxiaDamage = value)
    ];

    private static readonly DoubleBuilderField<BrainConstructHealthStrategy>[] DoubleFields =
    [
        PercentageField<BrainConstructHealthStrategy>("CriticalInjuryThreshold", ["criticalinjurythreshold"], "Critical Injury Threshold",
            x => x.CriticalInjuryThreshold, (x, value) => x.CriticalInjuryThreshold = value)
    ];

    private const string TypeBlurb =
        "A construct model with optional heart and power-core checks, plus brain-based death handling.";

    private static readonly string TypeHelp = BuildTypeHelp(TypeBlurb,
        GetBuilderFieldHelpText(TraitExpressionFields)
            .Concat(GetBuilderFieldHelpText(BoolFields))
            .Concat(GetBuilderFieldHelpText(DoubleFields)));

    private BrainConstructHealthStrategy(HealthStrategy strategy, IFuturemud gameworld)
        : base(strategy, gameworld)
    {
        LoadDefinition(XElement.Parse(strategy.Definition), gameworld);
    }

    private BrainConstructHealthStrategy(IFuturemud gameworld, string name)
        : base(gameworld, name)
    {
        MaximumHitPointsExpression = CreateDefaultExpression(gameworld, $"{name} Max HP", "100");
        CheckPowerCore = false;
        CheckHeart = false;
        UseHypoxiaDamage = false;
        CriticalInjuryThreshold = 0.9;
        DoDatabaseInsert(HealthStrategyType);
    }

    private BrainConstructHealthStrategy(BrainConstructHealthStrategy rhs, string name)
        : base(rhs, name)
    {
        MaximumHitPointsExpression = CloneExpression(rhs.MaximumHitPointsExpression, Gameworld);
        CheckPowerCore = rhs.CheckPowerCore;
        CheckHeart = rhs.CheckHeart;
        UseHypoxiaDamage = rhs.UseHypoxiaDamage;
        CriticalInjuryThreshold = rhs.CriticalInjuryThreshold;
        DoDatabaseInsert(HealthStrategyType);
    }

    public override string HealthStrategyType => "BrainConstruct";
    public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;
    public override bool RequiresSpinalCord => false;

    public ITraitExpression MaximumHitPointsExpression { get; set; }
    public bool CheckPowerCore { get; set; }
    public bool CheckHeart { get; set; }
    public bool UseHypoxiaDamage { get; set; }
    public double CriticalInjuryThreshold { get; set; }

    protected override IEnumerable<string> SubtypeBuilderHelpText =>
        GetBuilderFieldHelpText(TraitExpressionFields)
            .Concat(GetBuilderFieldHelpText(BoolFields))
            .Concat(GetBuilderFieldHelpText(DoubleFields));

    public static void RegisterHealthStrategyLoader()
    {
        RegisterHealthStrategy("BrainConstruct",
            (strategy, game) => new BrainConstructHealthStrategy(strategy, game),
            (game, name) => new BrainConstructHealthStrategy(game, name),
            TypeHelp,
            TypeBlurb);
    }

    private void LoadDefinition(XElement root, IFuturemud gameworld)
    {
        XElement element = root.Element("MaximumHitPointsExpression");
        if (element == null)
        {
            throw new ApplicationException(
                $"BrainConstructHealthStrategy ID {Id} did not contain a MaximumHitPointsExpression element.");
        }

        if (!long.TryParse(element.Value, out long value))
        {
            throw new ApplicationException(
                $"BrainConstructHealthStrategy ID {Id} had a MaximumHitPointsExpression element that did not contain an ID.");
        }

        MaximumHitPointsExpression = gameworld.TraitExpressions.Get(value);
        CheckPowerCore = LoadBool(root, "CheckPowerCore", false);
        CheckHeart = LoadBool(root, "CheckHeart", false);
        UseHypoxiaDamage = LoadBool(root, "UseHypoxiaDamage", false);
        CriticalInjuryThreshold = LoadDouble(root, "CriticalInjuryThreshold", 0.9);
    }

    protected override void SaveSubtypeDefinition(XElement root)
    {
        SaveBuilderFields(root, this, TraitExpressionFields);
        SaveBuilderFields(root, this, BoolFields);
        SaveBuilderFields(root, this, DoubleFields);
    }

    protected override void AppendSubtypeShow(System.Text.StringBuilder sb, ICharacter actor)
    {
        sb.AppendLine();
        AppendBuilderFieldShow(sb, actor, this, TraitExpressionFields);
        AppendBuilderFieldShow(sb, actor, this, BoolFields);
        AppendBuilderFieldShow(sb, actor, this, DoubleFields);
    }

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        if (TryBuildingCommand(actor, command.GetUndo(), TraitExpressionFields))
        {
            return true;
        }

        if (TryBuildingCommand(actor, command.GetUndo(), BoolFields))
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
        return new BrainConstructHealthStrategy(this, name);
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
        if (!UseHypoxiaDamage && damage.DamageType == DamageType.Hypoxia)
        {
            return Enumerable.Empty<IWound>();
        }

        if (!CheckHeart && damage.DamageType == DamageType.Cellular)
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

        if (CheckHeart && character.Body.OrganFunction<HeartProto>() <= 0.0 && character.State.IsUnconscious())
        {
            IBodypart damagePart = character.Body.Organs.FirstOrDefault() ?? character.Body.Bodyparts.GetRandomElement();
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

        string statusString = "";
        if (CheckHeart && character.Body.OrganFunction<HeartProto>() <= 0.0)
        {
            statusString = $" - {"Cardiac Arrest".Colour(Telnet.BoldRed)}";
        }

        if (CheckPowerCore && character.Body.OrganFunction<PowerCore>() <= 0.0)
        {
            statusString = $" - {"Power Core Failure".Colour(Telnet.BoldMagenta)}";
        }

        double totalWounds = MaximumHitPointsExpression.Evaluate(character);
        return string.Format(character, "<HP: {0:N0}/{1:N0}{2}>",
            totalWounds - owner.Wounds.Sum(x => x.CurrentDamage * x.Bodypart?.DamageModifier), totalWounds,
            statusString);
    }
}
