using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class TraitBoostEffect : IMagicSpellEffectTemplate
{
	public IFuturemud Gameworld => Spell.Gameworld;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("boost", (root, spell) => new TraitBoostEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("boost", BuilderFactory,
			"Boosts or penalises a skill or attribute",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new TraitBoostEffect(new XElement("Effect",
			new XAttribute("type", "glow"),
			new XAttribute("trait", 0),
			new XAttribute("bonus", 0.0),
			new XAttribute("context", (int)TraitBonusContext.None)), spell), string.Empty);
	}

	public IMagicSpell Spell { get; }
	public ITraitDefinition Trait { get; private set; }
	public double Bonus { get; private set; }
	public TraitBonusContext TraitBonusContext { get; private set; }

	protected TraitBoostEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		Trait = Gameworld.Traits.Get(long.Parse(root.Attribute("trait").Value));
		Bonus = double.Parse(root.Attribute("bonus").Value);
		TraitBonusContext = (TraitBonusContext)int.Parse(root.Attribute("context").Value);
	}

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "boost"),
			new XAttribute("trait", Trait?.Id ?? 0),
			new XAttribute("bonus", Bonus),
			new XAttribute("context", (int)TraitBonusContext)
		);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3trait <trait>#0 - sets the affected trait
	#3bonus <bonus>#0 - sets the bonus amount
	#3context <context>#0 - sets the bonus context";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "trait":
				return BuildingCommandTrait(actor, command);
			case "bonus":
				return BuildingCommandBonus(actor, command);
			case "context":
				return BuildingCommandContext(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill or attribute do you want this effect to give a bonus to?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		Trait = trait;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect now changes the {Trait.Name.ColourValue()} trait.");
		return true;
	}

	private bool BuildingCommandBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number for the bonus.");
			return false;
		}

		Bonus = value;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This effect now gives a bonus of {Bonus.ToBonusString(actor)} to its affected trait.");
		return true;
	}

	private bool BuildingCommandContext(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum<TraitBonusContext>(out var context))
		{
			actor.OutputHandler.Send(
				$"You must specify a valid trait bonus context.\nThe valid contexts are: {Enum.GetValues<TraitBonusContext>().OfType<TraitBonusContext>().Select(x => x.DescribeEnum().ColourName()).ListToString()}");
			return false;
		}

		TraitBonusContext = context;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This effect's bonus now only applies in the {context.DescribeEnum().ColourName()} context.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return
			$"TraitBoostEffect - {Trait?.Name.ColourValue() ?? "None".ColourError()} {Bonus.ToBonusString(actor)} (context: {TraitBonusContext.DescribeEnum().ColourValue()})";
	}

	#region Implementation of IMagicSpellEffectTemplate

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

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter)
		{
			return null;
		}

		if (Trait is null)
		{
			return null;
		}

		return new SpellTraitBoostEffect(target, parent, null);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new TraitBoostEffect(SaveToXml(), Spell);
	}

	#endregion
}