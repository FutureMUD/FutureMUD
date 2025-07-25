using System.Xml.Linq;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using MudSharp.PerceptionEngine;
using MudSharp.Body.Traits;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Magic.SpellEffects;

public class StaminaDeltaSpellEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("staminadelta", (root, spell) => new StaminaDeltaSpellEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("staminadelta", BuilderFactory,
            "Instantly changes stamina",
            HelpText,
            true,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new StaminaDeltaSpellEffect(new XElement("Effect",
            new XAttribute("type", "staminadelta"),
            new XElement("Formula", new XCData("power*outcome"))
        ), spell), string.Empty);
    }

    protected StaminaDeltaSpellEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        AmountExpression = new TraitExpression(root.Element("Formula")!.Value, Gameworld);
    }

    public IMagicSpell Spell { get; }
    public ITraitExpression AmountExpression { get; set; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "staminadelta"),
            new XElement("Formula", new XCData(AmountExpression.OriginalFormulaText))
        );
    }

    public const string HelpText = @"Options:
    #3formula <expr>#0 - sets stamina change formula";

    public bool BuildingCommand(ICharacter actor, StringStack command)
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
        AmountExpression = expr;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Formula set to {expr.OriginalFormulaText.ColourCommand()}.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        return $"StaminaDelta {AmountExpression.OriginalFormulaText}";
    }

    public bool IsInstantaneous => true;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => IsCompatibleWithTrigger(trigger.TargetTypes);
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
        if (target is not ICharacter ch)
            return null;
        var amount = AmountExpression.EvaluateWith(caster, values: new (string, object)[] { ("power", (int)power), ("outcome", (int)outcome) });
        if (amount >= 0)
            ch.GainStamina(amount);
        else
            ch.SpendStamina(-amount);
        return null;
    }

    public IMagicSpellEffectTemplate Clone() => new StaminaDeltaSpellEffect(SaveToXml(), Spell);
}
