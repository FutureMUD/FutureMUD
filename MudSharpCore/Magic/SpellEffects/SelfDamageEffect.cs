using System;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class SelfDamageEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("selfdamage", (root, spell) => new SelfDamageEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("selfdamage", BuilderFactory,
            "Deals damage to the caster",
            DamageEffect.HelpText,
            false,
            false,
            SpellTriggerFactory.MagicTriggerTypes.ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new SelfDamageEffect(new XElement("Effect",
                new XAttribute("type", "selfdamage"),
                new XElement("DamageType", (int)DamageType.Arcane),
                new XElement("DamageExpression", new XCData("power*outcome"))
            ), spell), string.Empty);
    }

    protected SelfDamageEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        DamageType = (DamageType)int.Parse(root.Element("DamageType").Value);
        DamageExpression = new TraitExpression(root.Element("DamageExpression").Value, Gameworld);
    }

    public IMagicSpell Spell { get; }
    public DamageType DamageType { get; set; }
    public ITraitExpression DamageExpression { get; set; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "selfdamage"),
            new XElement("DamageType", (int)DamageType),
            new XElement("DamageExpression", new XCData(DamageExpression.ToString()))
        );
    }

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "type":
                return BuildingCommandType(actor, command);
            case "formula":
                return BuildingCommandFormula(actor, command);
        }
        actor.OutputHandler.Send(DamageEffect.HelpText.SubstituteANSIColour());
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

    public string Show(ICharacter actor)
    {
        return $"SelfDamage {DamageType.DescribeEnum()} - {DamageExpression.OriginalFormulaText.ColourCommand()}";
    }

    public bool IsInstantaneous => true;
    public bool RequiresTarget => false;

    public bool IsCompatibleWithTrigger(IMagicTrigger types) => true;
    public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
    {
        var amount = DamageExpression.EvaluateWith(caster, values: new (string, object)[] { ("power", (int)power), ("outcome", (int)outcome) });
        var part = caster.Body.RandomBodypart;
        var dmg = new Damage
        {
            DamageType = DamageType,
            DamageAmount = amount,
            PainAmount = amount,
            StunAmount = amount,
            Bodypart = part,
            ActorOrigin = caster
        };
        caster.SufferDamage(dmg);
        return null;
    }

    public IMagicSpellEffectTemplate Clone() => new SelfDamageEffect(SaveToXml(), Spell);
}
