using System;
using System.Xml.Linq;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class WeightSpellEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("weight", (root, spell) => new WeightSpellEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("weight", BuilderFactory,
            "Adds extra weight to the target",
            HelpText,
            false,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new WeightSpellEffect(new XElement("Effect",
                        new XAttribute("type", "weight"),
                        new XElement("Weight", 1.0 / spell.Gameworld.UnitManager.BaseWeightToKilograms)
                    ), spell), string.Empty);
    }

    protected WeightSpellEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        AddedWeight = double.Parse(root.Element("Weight")?.Value ?? "1.0");
    }

    public IMagicSpell Spell { get; }
    public double AddedWeight { get; set; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "weight"),
            new XElement("Weight", AddedWeight)
        );
    }

    public const string HelpText = @"You can use the following options with this effect:
    #3weight <##>#0 - sets the weight added";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "weight":
                return BuildingCommandWeight(actor, command);
        }

        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandWeight(ICharacter actor, StringStack command)
    {
        if (command.IsFinished ||
            !Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, actor, out var value))
        {
            actor.OutputHandler.Send("You must enter a valid weight.");
            return false;
        }

        AddedWeight = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"The added weight is now {Gameworld.UnitManager.DescribeExact(AddedWeight, UnitType.Mass, actor).ColourValue()}.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        return $"Weight +{Gameworld.UnitManager.DescribeExact(AddedWeight, UnitType.Mass, actor)}";
    }

    public bool IsInstantaneous => false;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger types) => IsCompatibleWithTrigger(types.TargetTypes);
    public static bool IsCompatibleWithTrigger(string types)
    {
        switch (types)
        {
            case "item":
            case "items":
            case "character":
            case "characters":
                return true;
            default:
                return false;
        }
    }

    public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
    {
        if (target is not IPerceivable p)
        {
            return null;
        }
        return new SpellWeightEffect(p, parent, null, AddedWeight);
    }

    public IMagicSpellEffectTemplate Clone() => new WeightSpellEffect(SaveToXml(), Spell);
}
