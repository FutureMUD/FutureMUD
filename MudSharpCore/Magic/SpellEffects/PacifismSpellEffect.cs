using System;
using System.Xml.Linq;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class PacifismSpellEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("pacifism", (root, spell) => new PacifismSpellEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("pacifism", BuilderFactory,
            "Induces a magical pacifism in the target",
            HelpText,
            false,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new PacifismSpellEffect(new XElement("Effect",
                        new XAttribute("type", "pacifism"),
                        new XElement("Intensity", 1.0)
                    ), spell), string.Empty);
    }

    protected PacifismSpellEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        Intensity = double.Parse(root.Element("Intensity")?.Value ?? "1.0");
    }

    public IMagicSpell Spell { get; }
    public double Intensity { get; set; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "pacifism"),
            new XElement("Intensity", Intensity)
        );
    }

    public const string HelpText = @"You can use the following options with this effect:
    #3intensity <##>#0 - sets the pacifism intensity";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "intensity":
                return BuildingCommandIntensity(actor, command);
        }

        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandIntensity(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.OutputHandler.Send("You must enter a valid intensity.");
            return false;
        }
        Intensity = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"The pacifism intensity is now {Intensity.ToString("N2", actor).ColourValue()}.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        return $"Pacifism - {Intensity.ToString("N2", actor)}";
    }

    public bool IsInstantaneous => false;
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
        if (target is not ICharacter ch)
        {
            return null;
        }

        return new SpellPacifismEffect(ch, parent, null, Intensity);
    }

    public IMagicSpellEffectTemplate Clone() => new PacifismSpellEffect(SaveToXml(), Spell);
}
