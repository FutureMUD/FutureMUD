using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health.Wounds;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Health.Strategies;

public class BrainHitpointsStrategy : BaseHealthStrategy
{
    private static readonly TraitExpressionBuilderField<BrainHitpointsStrategy>[] TraitExpressionFields =
    [
        new("MaximumHitPointsExpression", ["maxhp", "maximumhitpointsexpression"], "Maximum Hit Points Expression",
            x => x.MaximumHitPointsExpression, (x, value) => x.MaximumHitPointsExpression = value),
        new("HealingTickDamageExpression", ["healdamage", "healingtickdamageexpression"], "Healing Tick Damage Expression",
            x => x.HealingTickDamageExpression, (x, value) => x.HealingTickDamageExpression = value)
    ];

    private static readonly DoubleBuilderField<BrainHitpointsStrategy>[] DoubleFields =
    [
        PercentageField<BrainHitpointsStrategy>("PercentageHealthPerPenalty", ["percentagehealthperpenalty", "healthpenalty"], "Percentage Health Per Penalty",
            x => x.PercentageHealthPerPenalty, (x, value) => x.PercentageHealthPerPenalty = value),
        PercentageField<BrainHitpointsStrategy>("CriticalInjuryThreshold", ["criticalinjurythreshold"], "Critical Injury Threshold",
            x => x.CriticalInjuryThreshold, (x, value) => x.CriticalInjuryThreshold = value)
    ];

    private static readonly BoolBuilderField<BrainHitpointsStrategy>[] BoolFields =
    [
        new("CheckPowerCore", ["checkpowercore"], "Check Power Core",
            x => x.CheckPowerCore, (x, value) => x.CheckPowerCore = value),
        new("CheckHeart", ["checkheart"], "Check Heart",
            x => x.CheckHeart, (x, value) => x.CheckHeart = value),
        new("UseHypoxiaDamage", ["usehypoxiadamage"], "Use Hypoxia Damage",
            x => x.UseHypoxiaDamage, (x, value) => x.UseHypoxiaDamage = value),
        new("KnockoutOnCritical", ["knockoutoncritical"], "Knockout On Critical",
            x => x.KnockoutOnCritical, (x, value) => x.KnockoutOnCritical = value)
    ];

    private static readonly TimeSpanBuilderField<BrainHitpointsStrategy>[] TimeSpanFields =
    [
        new("KnockoutDuration", ["knockoutduration"], "Knockout Duration",
            x => x.KnockoutDuration, (x, value) => x.KnockoutDuration = value)
    ];

    private const string TypeBlurb =
        "A simplified organic hit-point model with optional heart, power-core, and critical knockout handling.";

    private static readonly string TypeHelp = BuildTypeHelp(TypeBlurb,
        GetBuilderFieldHelpText(TraitExpressionFields)
            .Concat(GetBuilderFieldHelpText(DoubleFields))
            .Concat(GetBuilderFieldHelpText(BoolFields))
            .Concat(GetBuilderFieldHelpText(TimeSpanFields)));

    private BrainHitpointsStrategy(HealthStrategy strategy, IFuturemud gameworld)
        : base(strategy, gameworld)
    {
        LoadDefinition(XElement.Parse(strategy.Definition), gameworld);
    }

    private BrainHitpointsStrategy(IFuturemud gameworld, string name)
        : base(gameworld, name)
    {
        MaximumHitPointsExpression = CreateDefaultExpression(gameworld, $"{name} Max HP", "100");
        HealingTickDamageExpression = CreateDefaultExpression(gameworld, $"{name} Damage Heal", "1");
        PercentageHealthPerPenalty = 1.0;
        CheckPowerCore = false;
        CheckHeart = false;
        UseHypoxiaDamage = false;
        KnockoutOnCritical = false;
        KnockoutDuration = TimeSpan.FromSeconds(240);
        CriticalInjuryThreshold = 0.9;
        DoDatabaseInsert(HealthStrategyType);
    }

    private BrainHitpointsStrategy(BrainHitpointsStrategy rhs, string name)
        : base(rhs, name)
    {
        MaximumHitPointsExpression = CloneExpression(rhs.MaximumHitPointsExpression, Gameworld);
        HealingTickDamageExpression = CloneExpression(rhs.HealingTickDamageExpression, Gameworld);
        PercentageHealthPerPenalty = rhs.PercentageHealthPerPenalty;
        CheckPowerCore = rhs.CheckPowerCore;
        CheckHeart = rhs.CheckHeart;
        UseHypoxiaDamage = rhs.UseHypoxiaDamage;
        KnockoutOnCritical = rhs.KnockoutOnCritical;
        KnockoutDuration = rhs.KnockoutDuration;
        CriticalInjuryThreshold = rhs.CriticalInjuryThreshold;
        DoDatabaseInsert(HealthStrategyType);
    }

    public override string HealthStrategyType => "BrainHitpoints";
    public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;
    public override bool RequiresSpinalCord => false;

    public ITraitExpression MaximumHitPointsExpression { get; set; }
    public ITraitExpression HealingTickDamageExpression { get; set; }
    public double PercentageHealthPerPenalty { get; set; }
    public bool CheckPowerCore { get; set; }
    public bool CheckHeart { get; set; }
    public bool UseHypoxiaDamage { get; set; }
    public bool KnockoutOnCritical { get; set; }
    public TimeSpan KnockoutDuration { get; set; }
    public double CriticalInjuryThreshold { get; set; }

    protected override IEnumerable<string> SubtypeBuilderHelpText =>
        GetBuilderFieldHelpText(TraitExpressionFields)
            .Concat(GetBuilderFieldHelpText(DoubleFields))
            .Concat(GetBuilderFieldHelpText(BoolFields))
            .Concat(GetBuilderFieldHelpText(TimeSpanFields));

    public static void RegisterHealthStrategyLoader()
    {
        RegisterHealthStrategy("BrainHitpoints",
            (strategy, game) => new BrainHitpointsStrategy(strategy, game),
            (game, name) => new BrainHitpointsStrategy(game, name),
            TypeHelp,
            TypeBlurb);
    }

    private void LoadDefinition(XElement root, IFuturemud gameworld)
    {
        XElement element = root.Element("MaximumHitPointsExpression");
        if (element == null)
        {
            throw new ApplicationException(
                $"BrainHPHealthStrategy ID {Id} did not contain a MaximumHitPointsExpression element.");
        }

        if (!long.TryParse(element.Value, out long value))
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

        HealingTickDamageExpression = gameworld.TraitExpressions.Get(value);
        PercentageHealthPerPenalty = LoadDouble(root, "PercentageHealthPerPenalty", 1.0);
        CheckPowerCore = LoadBool(root, "CheckPowerCore", false);
        CheckHeart = LoadBool(root, "CheckHeart", false);
        UseHypoxiaDamage = LoadBool(root, "UseHypoxiaDamage", false);
        KnockoutOnCritical = LoadBool(root, "KnockoutOnCritical", false);
        KnockoutDuration = LoadTimeSpanFromSeconds(root, "KnockoutDuration", 240);
        CriticalInjuryThreshold = LoadDouble(root, "CriticalInjuryThreshold", 0.9);
    }

    protected override void SaveSubtypeDefinition(XElement root)
    {
        SaveBuilderFields(root, this, TraitExpressionFields);
        SaveBuilderFields(root, this, DoubleFields);
        SaveBuilderFields(root, this, BoolFields);
        SaveBuilderFields(root, this, TimeSpanFields);
    }

    protected override void AppendSubtypeShow(System.Text.StringBuilder sb, ICharacter actor)
    {
        sb.AppendLine();
        AppendBuilderFieldShow(sb, actor, this, TraitExpressionFields);
        AppendBuilderFieldShow(sb, actor, this, DoubleFields);
        AppendBuilderFieldShow(sb, actor, this, BoolFields);
        AppendBuilderFieldShow(sb, actor, this, TimeSpanFields);
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

        if (TryBuildingCommand(actor, command.GetUndo(), BoolFields))
        {
            return true;
        }

        if (TryBuildingCommand(actor, command.GetUndo(), TimeSpanFields))
        {
            return true;
        }

        return base.BuildingCommand(actor, command.GetUndo());
    }

    public override IHealthStrategy Clone(string name)
    {
        return new BrainHitpointsStrategy(this, name);
    }

    public override double MaxHP(IHaveWounds owner)
    {
        return MaximumHitPointsExpression.Evaluate(owner as IPerceivableHaveTraits);
    }

    public override void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture)
    {
        ICharacter cOwner = (ICharacter)owner;
        foreach (LiquidInstance liquid in mixture.Instances)
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
                        AlcoholLitres = liquid.Liquid.AlcoholLitresPerLitre /
                                        (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres),
                        SatiationPoints = liquid.Liquid.FoodSatiatedHoursPerLitre /
                                          (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres)
                    });
                    continue;
                case LiquidInjectionConsequence.BloodReplacement:
                    if (liquid is not BloodLiquidInstance blood)
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

        IGameItem lodgedItem = CheckDamageLodges(damage) ? damage.LodgableItem : null;

        return
        [
            new HealingSimpleWound(owner.Gameworld, owner, damage.DamageAmount, damage.DamageType, damage.Bodypart,
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
            if (character.Wounds.Sum(x => x.CurrentDamage) / MaximumHitPointsExpression.Evaluate(character) >
                CriticalInjuryThreshold)
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

            character.RemoveAllEffects<CriticalInjureKnockout>(fireRemovalAction: true);
        }

        return HealthTickResult.None;
    }

    public override bool IsCriticallyInjured(IHaveWounds owner)
    {
        return owner.Wounds.Sum(x => x.CurrentDamage) /
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

        double totalWounds = MaximumHitPointsExpression.Evaluate(character);
        return string.Format(character, "<HP: {0:N0}/{1:N0}{2}>",
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

        double penalty =
            charOwner.Wounds.Sum(x => x.CurrentDamage / PercentageHealthPerPenalty) /
            MaximumHitPointsExpression.Evaluate(charOwner);
        return -1 * penalty;
    }
}
