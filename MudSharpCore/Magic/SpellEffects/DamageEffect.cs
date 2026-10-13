using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Health;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.Magic.SpellEffects;

public partial class DamageEffect : IMagicSpellEffectTemplate, IMagicSpellEffectOperation
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("damage", (root, spell) => new DamageEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("damage", BuilderFactory,
            "Deals damage to a target",
            HelpText,
            false,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new DamageEffect(new XElement("Effect",
                new XAttribute("type", "damage"),
                new XElement("DamageType", (int)DamageType.Arcane),
                new XElement("DamageExpression", new XCData("power*outcome")),
                new XElement("Bodypart", 0L),
                new XElement("Limb", -1)
            ), spell), string.Empty);
    }

    protected DamageEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        DamageType = (DamageType)int.Parse(root.Element("DamageType").Value);
        DamageExpression = new TraitExpression(root.Element("DamageExpression").Value, Gameworld);
		PainExpression = root.Element("PainExpression") is { } pain ? new TraitExpression(pain.Value, Gameworld) : null;
		StunExpression = root.Element("StunExpression") is { } stun ? new TraitExpression(stun.Value, Gameworld) : null;
		if (root.Element("IdentitySafeDamage") is { } policy)
		{
			if ((int?)policy.Attribute("schema") != 1 || (int?)policy.Attribute("version") != 1 || policy.HasElements)
				throw new FormatException("Unsupported identity-safe damage policy; schema 1/version 1 is required.");
			IdentitySafeDamage = true;
		}
        BodypartId = long.Parse(root.Element("Bodypart")?.Value ?? "0");
        Limb = (LimbType)int.Parse(root.Element("Limb")?.Value ?? "-1");
    }

    public IMagicSpell Spell { get; }
    public DamageType DamageType { get; set; }
    public ITraitExpression DamageExpression { get; set; }
	public ITraitExpression? PainExpression { get; set; }
	public ITraitExpression? StunExpression { get; set; }
	public bool IdentitySafeDamage { get; private set; }
    public long BodypartId { get; set; }
    public LimbType Limb { get; set; }
    public IFuturemud Gameworld => Spell.Gameworld;

	/// <summary>Static builder readiness checks; live callback-dependent potency is rechecked before mutation.</summary>
	public string? DefinitionError
	{
		get
		{
			if (!Enum.IsDefined(DamageType)) return "Choose a defined native damage type.";
			if (BodypartId < 0 || Limb != (LimbType)(-1) && !Enum.IsDefined(Limb)) return "Choose a valid native bodypart or limb.";
			foreach (var expression in new[] { DamageExpression, PainExpression, StunExpression }.OfType<ITraitExpression>())
			{
				if (expression.HasErrors()) return "A damage, pain or stun expression is invalid.";
				if (expression is not TraitExpression native || native.Parameters.Count != 0 ||
					native.Formula.FunctionNames.Any() || native.Formula.ParameterNames.Any()) continue;
				if (!native.Formula.TryEvaluateDoubleWith(new Dictionary<string, object>(), out var amount, out _) ||
					!double.IsFinite(amount) || amount < 0)
					return "Fixed damage, pain and stun amounts must be finite and nonnegative.";
			}
			return null;
		}
	}

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "damage"),
            new XElement("DamageType", (int)DamageType),
            new XElement("DamageExpression", new XCData(DamageExpression.OriginalFormulaText)),
			PainExpression is null ? null : new XElement("PainExpression", new XCData(PainExpression.OriginalFormulaText)),
			StunExpression is null ? null : new XElement("StunExpression", new XCData(StunExpression.OriginalFormulaText)),
			IdentitySafeDamage ? new XElement("IdentitySafeDamage", new XAttribute("schema", 1), new XAttribute("version", 1)) : null,
            new XElement("Bodypart", BodypartId),
            new XElement("Limb", (int)Limb)
        );
    }

    public const string HelpText = @"Options:
    #3type <damage type>#0 - sets the damage type
    #3formula <expr>#0 - sets damage expression
	#3pain <expr|damage>#0 - sets pain expression or restores equal damage
	#3stun <expr|damage>#0 - sets stun expression or restores equal damage
	#3identitysafe#0 - toggles frozen native damage identity/custody checks
    #3bodypart <which>#0 - target a specific bodypart
    #3limb <which>#0 - target a limb";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "type":
                return BuildingCommandType(actor, command);
            case "formula":
                return BuildingCommandFormula(actor, command);
			case "pain":
				return BuildingCommandIndependentFormula(actor, command, true);
			case "stun":
				return BuildingCommandIndependentFormula(actor, command, false);
			case "identitysafe":
				IdentitySafeDamage = !IdentitySafeDamage;
				Spell.Changed = true;
				actor.OutputHandler.Send($"Frozen damage identity and custody checks: {IdentitySafeDamage.ToColouredString()}.");
				return true;
            case "bodypart":
                return BuildingCommandBodypart(actor, command);
            case "limb":
                return BuildingCommandLimb(actor, command);
        }
        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandType(ICharacter actor, StringStack command)
    {
        if (!command.SafeRemainingArgument.TryParseEnum<DamageType>(out DamageType type))
        {
            actor.OutputHandler.Send($"Valid types are: {Enum.GetNames(typeof(DamageType)).ListToString()}");
            return false;
        }
        DamageType = type;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Damage type set to {type.DescribeEnum().ColourValue()}.");
        return true;
    }

    private bool BuildingCommandFormula(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify a formula.");
            return false;
        }
        TraitExpression expr = new(command.SafeRemainingArgument, Gameworld);
        if (expr.HasErrors())
        {
            actor.OutputHandler.Send(expr.Error);
            return false;
        }
        DamageExpression = expr;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Damage formula set to {expr.OriginalFormulaText.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandIndependentFormula(ICharacter actor, StringStack command, bool pain)
	{
		var text = command.SafeRemainingArgument;
		if (string.IsNullOrWhiteSpace(text))
		{
			actor.OutputHandler.Send("Specify a formula or damage to restore the equal-damage default.");
			return false;
		}
		ITraitExpression? expression = text.EqualTo("damage") ? null : new TraitExpression(text, Gameworld);
		if (expression?.HasErrors() == true)
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}
		if (pain) PainExpression = expression;
		else StunExpression = expression;
		Spell.Changed = true;
		actor.OutputHandler.Send($"{(pain ? "Pain" : "Stun")} formula set to {(expression?.OriginalFormulaText ?? "equal damage").ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandBodypart(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Specify a bodypart.");
            return false;
        }
        IBodypart part = Gameworld.BodypartPrototypes.GetByIdOrName(command.SafeRemainingArgument);
        if (part == null)
        {
            actor.OutputHandler.Send("No such bodypart.");
            return false;
        }
        BodypartId = part.Id;
        Limb = (LimbType)(-1);
        Spell.Changed = true;
        actor.OutputHandler.Send($"Bodypart target set to {part.Name.ColourValue()}.");
        return true;
    }

    private bool BuildingCommandLimb(ICharacter actor, StringStack command)
    {
        if (!command.SafeRemainingArgument.TryParseEnum<LimbType>(out LimbType limb))
        {
            actor.OutputHandler.Send($"Valid limbs are: {Enum.GetNames(typeof(LimbType)).ListToString()}");
            return false;
        }
        Limb = limb;
        BodypartId = 0;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Limb target set to {limb.DescribeEnum().ColourValue()}.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        string target = BodypartId != 0
            ? Gameworld.BodypartPrototypes.Get(BodypartId)?.Name ?? "none"
            : Limb != (LimbType)(-1) ? Limb.DescribeEnum() : "random";
        return SpellEffectPresentation.Describe(actor, "Damage",
            ("Type", DamageType.DescribeEnum().ColourValue()),
            ("Formula", DamageExpression.OriginalFormulaText.ColourCommand()),
			("Pain", (PainExpression?.OriginalFormulaText ?? "equal damage").ColourCommand()),
			("Stun", (StunExpression?.OriginalFormulaText ?? "equal damage").ColourCommand()),
			("Identity safe", IdentitySafeDamage.ToColouredString()),
			("Validity", DefinitionError?.ColourError() ?? "valid".ColourValue()),
            ("Target", target.ColourValue()));
    }

    public bool IsInstantaneous => true;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger types)
    {
        return IsCompatibleWithTrigger(types.TargetTypes);
    }

    public static bool IsCompatibleWithTrigger(string types)
    {
        switch (types)
        {
            case "character":
            case "characters":
                return true;
            default:
                return false;
        }
    }

    public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
        => Apply(caster, target, outcome, power, parent, additionalParameters).Effect;

    public MagicEffectOperation Apply(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
    {
		if (IdentitySafeDamage) return ApplyIdentitySafe(caster, target, outcome, power);
        if (target is not ICharacter tch)
        {
            return new(MagicEffectOperationStatus.Rejected, null);
        }
		var amount = MudSharp.Magic.Casting.SpellAreaDamageScope.ScaleDamage(caster, target, Spell,
			DamageExpression.EvaluateWith(caster, values: new (string, object)[] { ("power", (int)power), ("outcome", (int)outcome) }));
		var painAmount = PainExpression is null ? amount : MudSharp.Magic.Casting.SpellAreaDamageScope.ScaleDamage(caster, target, Spell,
			PainExpression.EvaluateWith(caster, values: new (string, object)[] { ("power", (int)power), ("outcome", (int)outcome) }));
		var stunAmount = StunExpression is null ? amount : MudSharp.Magic.Casting.SpellAreaDamageScope.ScaleDamage(caster, target, Spell,
			StunExpression.EvaluateWith(caster, values: new (string, object)[] { ("power", (int)power), ("outcome", (int)outcome) }));
		if (!double.IsFinite(amount) || amount < 0 || !double.IsFinite(painAmount) || painAmount < 0 ||
			!double.IsFinite(stunAmount) || stunAmount < 0) return new(MagicEffectOperationStatus.Rejected, null);
        IBodypart part = BodypartId != 0 ? tch.Body.Bodyparts.FirstOrDefault(x => x.Id == BodypartId) : null;
        if (part == null && Limb != (LimbType)(-1))
        {
            ILimb limb = tch.Body.Limbs.FirstOrDefault(x => x.LimbType == Limb);
            part = limb != null ? tch.Body.BodypartsForLimb(limb).GetWeightedRandom(x => x.RelativeHitChance) : null;
        }
        part ??= tch.Body.RandomBodypart;
        Damage dmg = new()
        {
            DamageType = DamageType,
            DamageAmount = amount,
			PainAmount = painAmount,
			StunAmount = stunAmount,
            Bodypart = part,
            ActorOrigin = caster
        };
		var before = tch.Wounds.ToDictionary(x => x, x => (Damage: x.CurrentDamage, Pain: x.CurrentPain, Stun: x.CurrentStun));
		var wounds = tch.SufferDamage(dmg).ToArray();
		var applied = wounds.Any(x => !before.TryGetValue(x, out var prior)
			? x.CurrentDamage > 0 || x.CurrentPain > 0 || x.CurrentStun > 0
			: x.CurrentDamage > prior.Damage || x.CurrentPain > prior.Pain || x.CurrentStun > prior.Stun);
		return new(applied ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
    }

    public IMagicSpellEffectTemplate Clone()
    {
        return new DamageEffect(SaveToXml(), Spell);
    }
}
