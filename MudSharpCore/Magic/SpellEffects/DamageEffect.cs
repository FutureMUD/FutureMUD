using System;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class DamageEffect : IMagicSpellEffectTemplate
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
        BodypartId = long.Parse(root.Element("Bodypart")?.Value ?? "0");
        Limb = (LimbType)int.Parse(root.Element("Limb")?.Value ?? "-1");
    }

    public IMagicSpell Spell { get; }
    public DamageType DamageType { get; set; }
    public ITraitExpression DamageExpression { get; set; }
    public long BodypartId { get; set; }
    public LimbType Limb { get; set; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "damage"),
            new XElement("DamageType", (int)DamageType),
            new XElement("DamageExpression", new XCData(DamageExpression.ToString())),
            new XElement("Bodypart", BodypartId),
            new XElement("Limb", (int)Limb)
        );
    }

    public const string HelpText = @"Options:
    #3type <damage type>#0 - sets the damage type
    #3formula <expr>#0 - sets damage expression
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
        if (!command.SafeRemainingArgument.TryParseEnum<DamageType>(out var type))
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
        var expr = new TraitExpression(command.SafeRemainingArgument, Gameworld);
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

    private bool BuildingCommandBodypart(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Specify a bodypart.");
            return false;
        }
        var part = Gameworld.BodypartPrototypes.GetByIdOrName(command.SafeRemainingArgument);
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
        if (!command.SafeRemainingArgument.TryParseEnum<LimbType>(out var limb))
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
        var target = BodypartId != 0
            ? Gameworld.BodypartPrototypes.Get(BodypartId)?.Name ?? "none"
            : Limb != (LimbType)(-1) ? Limb.DescribeEnum() : "random";
        return $"Damage {DamageType.DescribeEnum()} - {DamageExpression.OriginalFormulaText.ColourCommand()} target {target}";
    }

    public bool IsInstantaneous => true;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger types) => IsCompatibleWithTrigger(types.TargetTypes);
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
    {
        if (target is not ICharacter tch)
        {
            return null;
        }
        var amount = DamageExpression.EvaluateWith(caster, values: new (string, object)[] { ("power", (int)power), ("outcome", (int)outcome) });
        var part = BodypartId != 0 ? tch.Body.Bodyparts.FirstOrDefault(x => x.Id == BodypartId) : null;
        if (part == null && Limb != (LimbType)(-1))
        {
            var limb = tch.Body.Limbs.FirstOrDefault(x => x.LimbType == Limb);
            part = limb != null ? tch.Body.BodypartsForLimb(limb).GetWeightedRandom(x => x.RelativeHitChance) : null;
        }
        part ??= tch.Body.RandomBodypart;
        var dmg = new Damage
        {
            DamageType = DamageType,
            DamageAmount = amount,
            PainAmount = amount,
            StunAmount = amount,
            Bodypart = part,
            ActorOrigin = caster
        };
        tch.SufferDamage(dmg);
        return null;
    }

    public IMagicSpellEffectTemplate Clone() => new DamageEffect(SaveToXml(), Spell);
}
