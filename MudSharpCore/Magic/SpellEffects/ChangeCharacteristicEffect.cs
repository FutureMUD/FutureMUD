using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

#nullable enable
namespace MudSharp.Magic.SpellEffects;

public class ChangeCharacteristicEffect : IMagicSpellEffectTemplate
{
	public IFuturemud Gameworld => Spell.Gameworld;
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("changecharacteristic", (root, spell) => new ChangeCharacteristicEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("changecharacteristic", BuilderFactory);
		SpellEffectFactory.RegisterBuilderFactory("changevariable", BuilderFactory);
	}

	private static (IMagicSpellEffectTemplate? Trigger, string Error) BuilderFactory(StringStack commands,
	IMagicSpell spell)
	{
		if (commands.IsFinished)
		{
			return (null, "You must specify a characteristic definition to change.");
		}

		var definition = spell.Gameworld.Characteristics.GetByIdOrName(commands.PopSpeech());
		if (definition is null)
		{
			return (null, $"There is no such characteristic definition as {commands.Last.ColourCommand()}.");
		}

		if (commands.IsFinished)
		{
			return (null, $"You must specify a value or profile for {definition.Name.ColourName()}.");
		}

		var valueText = commands.SafeRemainingArgument;
		if (valueText[0] == '*' && valueText.Length > 1)
		{
			valueText = valueText.Substring(1);
			var profile = spell.Gameworld.CharacteristicProfiles.Where(x => x.IsProfileFor(definition)).GetByIdOrName(valueText);
			if (profile is null)
			{
				return (null, $"There is no profile called {valueText.ColourCommand()} for definition {definition.Name.ColourName()}.");
			}

			return (new GlowEffect(new XElement("Effect",
			new XAttribute("type", "changecharacteristic"),
			new XElement("WhichCharacteristic", definition.Id),
			new XElement("CharacteristicValueTarget", 0),
			new XElement("CharacteristicProfileTarget", profile.Id)
			), spell), string.Empty);
		}

		var value = spell.Gameworld.CharacteristicValues.Where(x => definition.IsValue(x)).GetByIdOrName(valueText);
		if (value is null)
		{
			return (null, $"There is no such value as {valueText.ColourCommand()} for the {definition.Name.ColourName()} characteristic.");
		}

		return (new GlowEffect(new XElement("Effect",
			new XAttribute("type", "changecharacteristic"),
			new XElement("WhichCharacteristic", definition.Id),
			new XElement("CharacteristicValueTarget", value.Id),
			new XElement("CharacteristicProfileTarget", 0)
			), spell), string.Empty);
	}

	public IMagicSpell Spell { get; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public ICharacteristicDefinition WhichCharacteristic { get; private set; }
	public ICharacteristicValue? CharacteristicValueTarget { get; private set; }
	public ICharacteristicProfile? CharacteristicProfileTarget { get; private set; }

	#region Constructors and Saving

	public ChangeCharacteristicEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		WhichCharacteristic = spell.Gameworld.Characteristics.Get(long.Parse(root.Element("WhichCharacteristic")!.Value))!;
		CharacteristicValueTarget = spell.Gameworld.CharacteristicValues.Get(long.Parse(root.Element("CharacteristicValueTarget")?.Value ?? "0L"));
		CharacteristicProfileTarget = spell.Gameworld.CharacteristicProfiles.Get(long.Parse(root.Element("CharacteristicProfileTarget")?.Value ?? "0L"));
	}

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "changecharacteristic"),
			new XElement("WhichCharacteristic", WhichCharacteristic.Id),
			new XElement("CharacteristicValueTarget", CharacteristicValueTarget?.Id ?? 0L),
			new XElement("CharacteristicProfileTarget", CharacteristicProfileTarget?.Id ?? 0L)
		// Further xattributes
		);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new ChangeCharacteristicEffect(SaveToXml(), Spell);
	}
	#endregion

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
			SpellPower power, IMagicSpellEffectParent parent)
	{		
		// Remove or change if target is not IHaveCharacteristics
		if (target is not IHaveCharacteristics)
		{
			return null;
		}

		var ch = target as ICharacter;
		return new SpellChangeCharacteristicEffect(target, parent, (ch is not null ? (CharacteristicProfileTarget?.GetRandomCharacteristic(ch) ?? CharacteristicValueTarget) : (CharacteristicProfileTarget?.GetRandomCharacteristic() ?? CharacteristicValueTarget))!, null);
	}

	#region Building Commands
	public string Show(ICharacter actor)
	{
		// A one-line description of the spell effect
		return $"Change {WhichCharacteristic.Name.ColourValue()} to {CharacteristicValueTarget?.Name.ColourValue() ?? $"profile {CharacteristicProfileTarget?.Name.ColourValue() ?? "unknown".ColourError()}"}";
	}

	public const string HelpText = @"You can use the following options with this spell effect:

	#3value <which>#0 - sets the variable value for the effect
	#3profile <which>#0 - sets a profile to select randomly from instead of a specific value
	#3definition <which> <value>|*<profile>#0 - changes the characteristic definition that this profile applies to";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "definition":
				return BuildingCommandDefinition(actor, command);
			case "value":
				return BuildingCommandValue(actor, command);
			case "profile":
				return BuildingCommandProfile(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandProfile(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which profile do you want this effect to select values from?");
			return false;
		}

		var profile = Gameworld.CharacteristicProfiles.Where(x => x.IsProfileFor(WhichCharacteristic)).GetByIdOrName(command.SafeRemainingArgument);
		if (profile is null)
		{
			actor.OutputHandler.Send($"There is no such characteristic profile for the {WhichCharacteristic.Name.ColourName()} definition.");
			return false;
		}

		CharacteristicProfileTarget = profile;
		CharacteristicValueTarget = null;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This spell effect will now select values from the {profile.Name.ColourValue()} profile.");
		return true;
	}
	
	private bool BuildingCommandValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which value do you want this effect to give to the target's {WhichCharacteristic.Name.ColourName()}?");
			return false;
		}

		var value = Gameworld.CharacteristicValues.Where(x => WhichCharacteristic.IsValue(x)).GetByIdOrName(command.SafeRemainingArgument);
		if (value is null)
		{
			actor.OutputHandler.Send($"There is no such characteristic value for the {WhichCharacteristic.Name.ColourName()} definition.");
			return false;
		}

		CharacteristicProfileTarget = null;
		CharacteristicValueTarget = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This spell effect will now use the value {value.Name.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDefinition(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a characteristic definition to change.");
			return false;
		}

		var definition = Gameworld.Characteristics.GetByIdOrName(command.PopSpeech());
		if (definition is null)
		{
			actor.OutputHandler.Send($"There is no such characteristic definition as {command.Last.ColourCommand()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a value or profile for {definition.Name.ColourName()}.");
			return false;
		}

		var valueText = command.SafeRemainingArgument;
		if (valueText[0] == '*' && valueText.Length > 1)
		{
			valueText = valueText.Substring(1);
			var profile = Gameworld.CharacteristicProfiles.Where(x => x.IsProfileFor(definition)).GetByIdOrName(valueText);
			if (profile is null)
			{
				actor.OutputHandler.Send($"There is no profile called {valueText.ColourCommand()} for definition {definition.Name.ColourName()}.");
				return false;
			}

			WhichCharacteristic = definition;
			CharacteristicProfileTarget = profile;
			CharacteristicValueTarget = null;
			Spell.Changed = true;
			actor.OutputHandler.Send($"This effect will now change the {definition.Name.ColourName()} characteristic with values from the {profile.Name.ColourValue()} profile.");
			return true;
		}

		var value = Gameworld.CharacteristicValues.Where(x => definition.IsValue(x)).GetByIdOrName(valueText);
		if (value is null)
		{
			actor.OutputHandler.Send( $"There is no such value as {valueText.ColourCommand()} for the {definition.Name.ColourName()} characteristic.");
			return false;
		}

		WhichCharacteristic = definition;
		CharacteristicProfileTarget = null;
		CharacteristicValueTarget = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now change the {definition.Name.ColourName()} characteristic to the value {value.Name.ColourValue()}.");
		return true;
	}
	#endregion
}