using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Health.Wounds;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Health.Strategies;

public class BrainHitpointsStrategy : BaseHealthStrategy, IHitPointHealingCeiling, ICurrentHitPointsStrategy, ICriticalHealthRecoveryStrategy
{
    public override HealthStateModel HealthStateModel => HealthStateModel.Organic;

    private static readonly TraitExpressionBuilderField<BrainHitpointsStrategy>[] TraitExpressionFields =
    [
        new("MaximumHitPointsExpression", ["maxhp", "maximumhitpointsexpression"], "Maximum Hit Points Expression",
            x => x.MaximumHitPointsExpression, (x, value) => x.MaximumHitPointsExpression = value),
        new("HealingTickDamageExpression", ["healdamage", "healingtickdamageexpression"], "Healing Tick Damage Expression",
            x => x.HealingTickDamageExpression, (x, value) => x.HealingTickDamageExpression = value),
		new("MaximumStunExpression", ["maxstun"], "Maximum Stun Expression", x => x.MaximumStunExpression, (x, value) => x.MaximumStunExpression = value),
		new("HealingTickStunExpression", ["healstun"], "Stun Recovery Expression", x => x.HealingTickStunExpression, (x, value) => x.HealingTickStunExpression = value)
    ];

    private static readonly DoubleBuilderField<BrainHitpointsStrategy>[] DoubleFields =
    [
		new("DeathHitPoints", ["deathhp"], "Death Hit Points", x => x.DeathHitPoints, (x, value) => x.DeathHitPoints = value,
			Validator: (x, value) => (double.IsFinite(value) && value < x.MortallyWoundedHitPoints, "Death HP must be finite and below mortal HP.", value)),
		new("UnconsciousHitPoints", ["unconscioushp"], "Unconscious Hit Points", x => x.UnconsciousHitPoints, (x, value) => x.UnconsciousHitPoints = value,
			Validator: (x, value) => (double.IsFinite(value) && value >= x.MortallyWoundedHitPoints, "Unconscious HP must be finite and at least mortal HP.", value)),
		new("MortallyWoundedHitPoints", ["mortalhp"], "Mortally Wounded Hit Points", x => x.MortallyWoundedHitPoints, (x, value) => x.MortallyWoundedHitPoints = value,
			Validator: (x, value) => (double.IsFinite(value) && value > x.DeathHitPoints && value <= x.UnconsciousHitPoints, "Mortal HP must be above death HP and no higher than unconscious HP.", value)),
		new("CriticalRecoveryHealing", ["criticalhealing"], "Critical Recovery Healing", x => x.CriticalRecoveryHealing, (x, value) => x.CriticalRecoveryHealing = value,
			Validator: (_, value) => (double.IsFinite(value) && value >= 0, "Healing must be a finite nonnegative number.", value)),
		new("CriticalRecoveryDamage", ["criticaldamage"], "Critical Recovery Damage", x => x.CriticalRecoveryDamage, (x, value) => x.CriticalRecoveryDamage = value,
			Validator: (_, value) => (double.IsFinite(value) && value >= 0, "Damage must be a finite nonnegative number.", value)),
        PercentageField<BrainHitpointsStrategy>("PercentageHealthPerPenalty", ["percentagehealthperpenalty", "healthpenalty"], "Percentage Health Per Penalty",
            x => x.PercentageHealthPerPenalty, (x, value) => x.PercentageHealthPerPenalty = value),
        PercentageField<BrainHitpointsStrategy>("CriticalInjuryThreshold", ["criticalinjurythreshold"], "Critical Injury Threshold",
            x => x.CriticalInjuryThreshold, (x, value) => x.CriticalInjuryThreshold = value)
    ];

    private static readonly BoolBuilderField<BrainHitpointsStrategy>[] BoolFields =
    [
		new("UseNegativeHitPoints", ["negativehp"], "Use Negative Hit Points", x => x.UseNegativeHitPoints, (x, value) => x.UseNegativeHitPoints = value),
		new("UseCriticalRecovery", ["criticalrecovery"], "Use Critical Recovery", x => x.UseCriticalRecovery, (x, value) => x.UseCriticalRecovery = value),
		new("UseStun", ["usestun"], "Use Stun", x => x.UseStun, (x, value) => x.UseStun = value),
		new("UseCellularDamage", ["usecellulardamage"], "Use Cellular Damage", x => x.UseCellularDamage, (x, value) => x.UseCellularDamage = value),
		new("SeparateCapacityPenalties", ["separatecapacitypenalties"], "Separate Capacity Penalties",
			x => x.SeparateCapacityPenalties, (x, value) => x.SeparateCapacityPenalties = value),
        new("CheckPowerCore", ["checkpowercore"], "Check Power Core",
            x => x.CheckPowerCore, (x, value) => x.CheckPowerCore = value),
        new("CheckHeart", ["checkheart"], "Check Heart",
            x => x.CheckHeart, (x, value) => x.CheckHeart = value),
        new("UseHypoxiaDamage", ["usehypoxiadamage"], "Use Hypoxia Damage",
            x => x.UseHypoxiaDamage, (x, value) => x.UseHypoxiaDamage = value),
        new("KnockoutOnCritical", ["knockoutoncritical"], "Knockout On Critical",
            x => x.KnockoutOnCritical, (x, value) => x.KnockoutOnCritical = value)
    ];

	private static readonly IntBuilderField<BrainHitpointsStrategy>[] IntFields =
	[
		new("CriticalRecoveryDifficulty", ["criticaldifficulty"], "Critical Recovery Difficulty", x => (int)x.CriticalRecoveryDifficulty,
			(x, value) => x.CriticalRecoveryDifficulty = (Difficulty)value, (_, value) => ((Difficulty)value).DescribeEnum(),
			(_, value) => (Enum.IsDefined(typeof(Difficulty), value), "Specify a valid native difficulty number.", value)),
		new("MortalRecoveryDifficulty", ["mortaldifficulty"], "Mortal Recovery Difficulty", x => (int)x.MortalRecoveryDifficulty,
			(x, value) => x.MortalRecoveryDifficulty = (Difficulty)value, (_, value) => ((Difficulty)value).DescribeEnum(),
			(_, value) => (Enum.IsDefined(typeof(Difficulty), value), "Specify a valid native difficulty number.", value))
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
            .Concat(GetBuilderFieldHelpText(TimeSpanFields))
            .Concat(GetBuilderFieldHelpText(IntFields))
            .Append("#3criticaltrait <attribute>#0 - sets the critical recovery attribute"));

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
		MaximumStunExpression = MaximumHitPointsExpression;
		HealingTickStunExpression = HealingTickDamageExpression;
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
		MaximumStunExpression = CloneExpression(rhs.MaximumStunExpression, Gameworld);
		HealingTickStunExpression = CloneExpression(rhs.HealingTickStunExpression, Gameworld);
		UseStun = rhs.UseStun;
		UseCellularDamage = rhs.UseCellularDamage;
        PercentageHealthPerPenalty = rhs.PercentageHealthPerPenalty;
        CheckPowerCore = rhs.CheckPowerCore;
        CheckHeart = rhs.CheckHeart;
        UseHypoxiaDamage = rhs.UseHypoxiaDamage;
        KnockoutOnCritical = rhs.KnockoutOnCritical;
        KnockoutDuration = rhs.KnockoutDuration;
        CriticalInjuryThreshold = rhs.CriticalInjuryThreshold;
		SeparateCapacityPenalties = rhs.SeparateCapacityPenalties;
		UseNegativeHitPoints = rhs.UseNegativeHitPoints;
		DeathHitPoints = rhs.DeathHitPoints;
		UnconsciousHitPoints = rhs.UnconsciousHitPoints;
		MortallyWoundedHitPoints = rhs.MortallyWoundedHitPoints;
		UseCriticalRecovery = rhs.UseCriticalRecovery;
		CriticalRecoveryTrait = rhs.CriticalRecoveryTrait;
		CriticalRecoveryDifficulty = rhs.CriticalRecoveryDifficulty;
		MortalRecoveryDifficulty = rhs.MortalRecoveryDifficulty;
		CriticalRecoveryHealing = rhs.CriticalRecoveryHealing;
		CriticalRecoveryDamage = rhs.CriticalRecoveryDamage;
        DoDatabaseInsert(HealthStrategyType);
    }

    public override string HealthStrategyType => "BrainHitpoints";
    public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;
    public override bool RequiresSpinalCord => false;

	public bool UseNegativeHitPoints { get; private set; }
	public double DeathHitPoints { get; private set; } = -11;
	public double UnconsciousHitPoints { get; private set; }
	public double MortallyWoundedHitPoints { get; private set; } = -3;
	public bool UseCriticalRecovery { get; private set; }
	public ITraitDefinition CriticalRecoveryTrait { get; private set; }
	public Difficulty CriticalRecoveryDifficulty { get; private set; } = Difficulty.Normal;
	public Difficulty MortalRecoveryDifficulty { get; private set; } = Difficulty.Hard;
	public double CriticalRecoveryHealing { get; private set; } = 1;
	public double CriticalRecoveryDamage { get; private set; } = 1;

    public ITraitExpression MaximumHitPointsExpression { get; set; }
    public ITraitExpression HealingTickDamageExpression { get; set; }
	public ITraitExpression MaximumStunExpression { get; set; }
	public ITraitExpression HealingTickStunExpression { get; set; }
	public bool UseStun { get; private set; }
	public bool UseCellularDamage { get; private set; }
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
            .Concat(GetBuilderFieldHelpText(TimeSpanFields))
            .Concat(GetBuilderFieldHelpText(IntFields))
            .Append("#3criticaltrait <attribute>#0 - sets the critical recovery attribute");

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
		SeparateCapacityPenalties = LoadBool(root, "SeparateCapacityPenalties", false);
		UseStun = LoadBool(root, "UseStun", false);
		UseNegativeHitPoints = LoadBool(root, "UseNegativeHitPoints", false);
		DeathHitPoints = LoadDouble(root, "DeathHitPoints", -11);
		UnconsciousHitPoints = LoadDouble(root, "UnconsciousHitPoints", 0);
		MortallyWoundedHitPoints = LoadDouble(root, "MortallyWoundedHitPoints", -3);
		UseCriticalRecovery = LoadBool(root, "UseCriticalRecovery", false);
		CriticalRecoveryTrait = long.TryParse(root.Element("CriticalRecoveryTrait")?.Value, out var traitId) ? gameworld.Traits.Get(traitId) : null;
		CriticalRecoveryDifficulty = (Difficulty)((int?)root.Element("CriticalRecoveryDifficulty") ?? (int)Difficulty.Normal);
		MortalRecoveryDifficulty = (Difficulty)((int?)root.Element("MortalRecoveryDifficulty") ?? (int)Difficulty.Hard);
		CriticalRecoveryHealing = LoadDouble(root, "CriticalRecoveryHealing", 1);
		CriticalRecoveryDamage = LoadDouble(root, "CriticalRecoveryDamage", 1);
		if (!double.IsFinite(DeathHitPoints) || !double.IsFinite(UnconsciousHitPoints) || !double.IsFinite(MortallyWoundedHitPoints) ||
			DeathHitPoints >= MortallyWoundedHitPoints || MortallyWoundedHitPoints > UnconsciousHitPoints ||
			!double.IsFinite(CriticalRecoveryHealing) || CriticalRecoveryHealing < 0 || !double.IsFinite(CriticalRecoveryDamage) || CriticalRecoveryDamage < 0 ||
			!Enum.IsDefined(CriticalRecoveryDifficulty) || !Enum.IsDefined(MortalRecoveryDifficulty))
			throw new ApplicationException($"BrainHitpoints strategy {Id} has invalid critical HP configuration.");
		UseCellularDamage = LoadBool(root, "UseCellularDamage", CheckHeart);
		MaximumStunExpression = (long.TryParse(root.Element("MaximumStunExpression")?.Value, out var stunId) ? gameworld.TraitExpressions.Get(stunId) : null) ?? MaximumHitPointsExpression;
		HealingTickStunExpression = (long.TryParse(root.Element("HealingTickStunExpression")?.Value, out var recoveryId) ? gameworld.TraitExpressions.Get(recoveryId) : null) ?? HealingTickDamageExpression;
    }

    protected override void SaveSubtypeDefinition(XElement root)
    {
        SaveBuilderFields(root, this, TraitExpressionFields);
        SaveBuilderFields(root, this, DoubleFields);
        SaveBuilderFields(root, this, BoolFields);
        SaveBuilderFields(root, this, TimeSpanFields);
		SaveBuilderFields(root, this, IntFields);
		root.SetElementValue("CriticalRecoveryTrait", CriticalRecoveryTrait?.Id ?? 0);
    }

    protected override void AppendSubtypeShow(System.Text.StringBuilder sb, ICharacter actor)
    {
        sb.AppendLine();
        AppendBuilderFieldShow(sb, actor, this, TraitExpressionFields);
        AppendBuilderFieldShow(sb, actor, this, DoubleFields);
        AppendBuilderFieldShow(sb, actor, this, BoolFields);
        AppendBuilderFieldShow(sb, actor, this, TimeSpanFields);
		AppendBuilderFieldShow(sb, actor, this, IntFields);
		sb.AppendLine($"Critical Recovery Attribute: {(CriticalRecoveryTrait?.Name ?? "None (recovery inactive)").ColourValue()}");
    }

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
		if (command.PeekSpeech().EqualTo("criticaltrait"))
		{
			command.PopSpeech();
			var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
			if (trait is null || trait.TraitType != TraitType.Attribute)
			{
				actor.OutputHandler.Send("Specify an existing attribute for critical recovery.");
				return false;
			}
			CriticalRecoveryTrait = trait;
			Changed = true;
			actor.OutputHandler.Send($"Critical recovery now checks {trait.Name.ColourName()}.");
			return true;
		}
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

		if (TryBuildingCommand(actor, command.GetUndo(), IntFields)) return true;
        return base.BuildingCommand(actor, command.GetUndo());
    }

    public override IHealthStrategy Clone(string name)
    {
        return new BrainHitpointsStrategy(this, name);
    }

    public override double MaxHP(IHaveWounds owner)
    {
        return owner is not ICharacter charOwner ? MaximumHitPointsExpression.Evaluate(owner as IPerceivableHaveTraits) : ApplyMaximumHitPointsEffects(charOwner, MaximumHitPointsExpression.Evaluate(charOwner) + MudSharp.Magic.EquippedItemBonuses.BonusFor(charOwner, MudSharp.Magic.EquippedItemBonusKind.Health));
    }

	public bool SeparateCapacityPenalties { get; private set; }

	public override double MaxStun(IHaveWounds owner) => UseStun ? MaximumStunExpression.Evaluate(owner as IPerceivableHaveTraits) : base.MaxStun(owner);

	private double DamageHitPointBudget(IHaveWounds owner)
	{
		if (!SeparateCapacityPenalties || owner is not ICharacter actor) return MaxHP(owner);
		var effects = actor.Body.EffectsOfType<IMaximumHitPointsBonusEffect>().ToArray();
		var basis = MaximumHitPointsExpression.Evaluate(actor) + MudSharp.Magic.EquippedItemBonuses.BonusFor(actor, MudSharp.Magic.EquippedItemBonusKind.Health);
		return Math.Max(1.0, (basis + effects.Sum(effect => effect is ICapacityOnlyHitPointsPenalty { CapacityOnly: true } &&
			effect.MaximumHitPointsBonus < 0.0 ? 0.0 : effect.MaximumHitPointsBonus)) *
			effects.Aggregate(1.0, (value, effect) => value * effect.MaximumHitPointsMultiplier));
	}

	public double CurrentHitPoints(IHaveWounds owner) => Math.Min(MaxHP(owner), DamageHitPointBudget(owner) - owner.Wounds.Sum(x => x.CurrentDamage));
	public override double CurrentHealthPercentage(IHaveWounds owner) => SeparateCapacityPenalties
		? Math.Clamp(CurrentHitPoints(owner) / MaxHP(owner).IfZero(1.0), 0.0, 1.0) : base.CurrentHealthPercentage(owner);

	public double ConstrainHealedDamage(IWound wound, double proposedDamage)
	{
		if (!SeparateCapacityPenalties || !wound.Parent.Wounds.Contains(wound)) return proposedDamage;
		var floor = Math.Max(0.0, DamageHitPointBudget(wound.Parent) - MaxHP(wound.Parent));
		var otherDamage = wound.Parent.Wounds.Where(x => !ReferenceEquals(x, wound)).Sum(x => x.CurrentDamage);
		return Math.Min(wound.CurrentDamage, Math.Max(proposedDamage, floor - otherDamage));
	}

    public override void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture)
    {
        MudSharp.Magic.MagicalExposure.Liquid(owner, mixture, MudSharp.Health.DrugVector.Injected);
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

	public override DirectHealthCostChannels SupportedDirectHealthCostChannels =>
		DirectHealthCostChannels.Damage | (UseStun ? DirectHealthCostChannels.Stun : DirectHealthCostChannels.None);

	public override bool TryPlanDirectHealthCost(IHaveWounds owner, IBodypart bodypart, double damage, double pain,
		double stun, WoundSeverity maximumSeverity, out DirectHealthCostPlan plan, out string error) =>
		TryPlanHitpointCost(owner, bodypart, damage, pain, stun, maximumSeverity, false, out plan, out error);

	public override bool TryPlanDirectHealthConsequence(IHaveWounds owner, IBodypart bodypart, double damage, double pain,
		double stun, out DirectHealthCostPlan plan, out string error) =>
		TryPlanHitpointCost(owner, bodypart, damage, pain, stun, WoundSeverity.Horrifying, true, out plan, out error);

	private bool TryPlanHitpointCost(IHaveWounds owner, IBodypart bodypart, double damage, double pain, double stun,
		WoundSeverity severity, bool allowIncapacitation, out DirectHealthCostPlan plan, out string error)
	{
		if (!allowIncapacitation && damage > CurrentHitPoints(owner))
		{
			plan = null;
			error = "Your current condition cannot safely absorb that health cost.";
			return false;
		}
		// Capacity-only penalties can leave unused damage headroom above the visible HP cap.
		// Charge enough native damage to deduct the requested visible health, without healing it later.
		var nativeDamage = damage > 0 ? damage + Math.Max(0,
			DamageHitPointBudget(owner) - owner.Wounds.Sum(x => x.CurrentDamage) - CurrentHitPoints(owner)) : damage;
		if (!OrganicDirectHealthCosts.TryPlan(this, owner, bodypart, nativeDamage, pain, stun, severity,
			out plan, out error, allowIncapacitation, simpleHitpoints: true)) return false;
		plan = plan with { ExpectedDamage = damage };
		return true;
	}

	public override IReadOnlyList<IWound> ApplyDirectHealthCost(IHaveWounds owner, DirectHealthCostPlan plan) =>
		OrganicDirectHealthCosts.Apply(this, owner, plan, simpleHitpoints: true);

    public override IEnumerable<IWound> SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart)
    {
        if (!UseHypoxiaDamage && damage.DamageType == DamageType.Hypoxia)
        {
            return Enumerable.Empty<IWound>();
        }

        if (!UseCellularDamage && damage.DamageType == DamageType.Cellular)
        {
            return Enumerable.Empty<IWound>();
        }

        IGameItem lodgedItem = CheckDamageLodges(damage) ? damage.LodgableItem : null;

		return ContinuousExposureDamage.Accumulate(owner, damage, packet =>
			new HealingSimpleWound(owner.Gameworld, owner, packet.DamageAmount, packet.DamageType, packet.Bodypart,
				lodgedItem, packet.ToolOrigin, packet.ActorOrigin, ownerBody: packet.TargetBody, stun: UseStun ? packet.StunAmount : 0.0));
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

        if (UseNegativeHitPoints ? CurrentHitPoints(character) <= DeathHitPoints : DamageHitPointBudget(character) <= thing.Wounds.Sum(x => x.CurrentDamage))
        {
            return HealthTickResult.Dead;
        }
		var ignoresHitPointUnconsciousness = character.Merits.OfType<IIgnoreHitPointUnconsciousnessMerit>().Any(x => x.Applies(character));
		if (UseNegativeHitPoints && CurrentHitPoints(character) <= UnconsciousHitPoints && !ignoresHitPointUnconsciousness)
			return HealthTickResult.Unconscious;
		if (UseStun && thing.Wounds.Sum(wound => wound.CurrentStun) >= MaxStun(character) &&
			!character.EffectsOfType<IPreventStunUnconsciousnessEffect>().Any(effect => effect.Applies(character))) return HealthTickResult.Unconscious;

        if (CheckHeart && character.Body.OrganFunction<HeartProto>() <= 0.0)
        {
            return HealthTickResult.Unconscious;
        }

        if (KnockoutOnCritical && !ignoresHitPointUnconsciousness)
        {
            if ((SeparateCapacityPenalties ? 1.0 - CurrentHealthPercentage(character) : character.Wounds.Sum(x => x.CurrentDamage) / MaxHP(character)) >
                CriticalInjuryThreshold)
            {
                if (!character.AffectedBy<CriticalInjureKnockout>())
                {
                    character.AddEffect(new CriticalInjureKnockout(character, RuntimeClock.UtcNow + KnockoutDuration));
                    return HealthTickResult.PassOut;
                }

                return character.EffectsOfType<CriticalInjureKnockout>().First().WakeupTime > RuntimeClock.UtcNow
                    ? HealthTickResult.PassOut
                    : HealthTickResult.None;
            }

            character.RemoveAllEffects<CriticalInjureKnockout>(fireRemovalAction: true);
        }

        return HealthTickResult.None;
    }

    public override bool IsCriticallyInjured(IHaveWounds owner)
    {
		if (UseNegativeHitPoints && CurrentHitPoints(owner) <= MortallyWoundedHitPoints) return true;
        return (SeparateCapacityPenalties ? 1.0 - CurrentHealthPercentage(owner) : owner.Wounds.Sum(x => x.CurrentDamage) /
               MaxHP((ICharacter)owner)) > CriticalInjuryThreshold &&
               owner is ICharacter ch &&
               ch.State.HasFlag(CharacterState.Unconscious);
    }

    public override string ReportConditionPrompt(IHaveWounds owner, PromptType type)
    {
        if (owner is not ICharacter character)
        {
            return "<Fine>";
        }

        string statusString = UseNegativeHitPoints && CurrentHitPoints(owner) <= MortallyWoundedHitPoints
			? $" - {"Mortally Wounded".Colour(Telnet.BoldRed)}" : "";
        if (CheckHeart && character.Body.OrganFunction<HeartProto>() <= 0.0)
        {
            statusString = $" - {"Cardiac Arrest".Colour(Telnet.BoldRed)}";
        }

        double totalWounds = MaxHP(character);
		if (UseStun) statusString += string.Format(character, " - Stun: {0:N0}/{1:N0}", Math.Max(0.0, MaxStun(owner) - owner.Wounds.Sum(x => x.CurrentStun)), MaxStun(owner));
        return string.Format(character, "<HP: {0:N0}/{1:N0}{2}>",
            CurrentHitPoints(owner), totalWounds, statusString);
    }

    public override double GetHealingTickAmount(IWound wound, Outcome outcome, HealthDamageType type)
    {
        ITraitExpression whichExpression;
        switch (type)
        {
            case HealthDamageType.Damage:
				if (OverridesNaturalDamageHealing(wound.Parent)) return 0.0;
                whichExpression = HealingTickDamageExpression;
                break;
            case HealthDamageType.Pain:
            case HealthDamageType.Shock:
            default:
                return 0;
			case HealthDamageType.Stun:
				if (!UseStun) return 0.0;
				whichExpression = HealingTickStunExpression;
				break;
        }

        return whichExpression.EvaluateWith((ICharacter)wound.Parent,
            values: [("originaldamage", wound.OriginalDamage), ("damage", type == HealthDamageType.Stun ? wound.CurrentStun : wound.CurrentDamage),
                ("outcome", outcome.SuccessDegrees())]);
    }

	public bool OverridesNaturalDamageHealing(IHaveWounds owner) => UseNegativeHitPoints && UseCriticalRecovery && CriticalRecoveryTrait is not null &&
		CurrentHitPoints(owner) <= UnconsciousHitPoints;

	public void PerformCriticalRecovery(ICharacter actor)
	{
		if (!OverridesNaturalDamageHealing(actor) || CriticalRecoveryTrait is null || actor.State.IsDead() ||
			CurrentHitPoints(actor) <= DeathHitPoints) return;
		var body = actor.Body;
		var check = actor.Gameworld.GetCheck(CheckType.GenericAttributeCheck).Check(actor,
			CurrentHitPoints(actor) <= MortallyWoundedHitPoints ? MortalRecoveryDifficulty : CriticalRecoveryDifficulty, CriticalRecoveryTrait);
		if (!ReferenceEquals(actor.Body, body) || actor.State.IsDead()) return;
		HitPointChanges.Apply(actor, check.IsPass() ? CriticalRecoveryHealing : -CriticalRecoveryDamage);
	}

    public override double WoundPenaltyFor(IHaveWounds owner)
    {
        if (owner is not ICharacter charOwner)
        {
            return 0;
        }

        double penalty =
            SeparateCapacityPenalties ? (1.0 - CurrentHealthPercentage(charOwner)) / PercentageHealthPerPenalty :
			charOwner.Wounds.Sum(x => x.CurrentDamage / PercentageHealthPerPenalty) / MaxHP(charOwner);
        return -1 * penalty;
    }
}
