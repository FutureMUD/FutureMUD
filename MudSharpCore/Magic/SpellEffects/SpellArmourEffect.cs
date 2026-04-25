#nullable enable
using ExpressionEngine;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class SpellArmourEffect : CharacterSpellEffectTemplateBase
{
	public const string HelpText = @"You can use the following options with this effect:
	#3part <which>#0 - toggles a bodypart shape being covered
	#3part all#0 - sets the armour as applying to all bodyparts
	#3obscured#0 - toggles armour being hidden by items worn on the bodypart
	#3desc <text>#0 - sets a full desc addendum when the armour affect applies
	#3desc none#0 - clears a full desc addendum
	#3absorb <formula>#0 - sets the formula for maximum damage absorbed
	#3applies <prog>#0 - sets a prog for whether the armour applies
	#3armourtype <type>#0 - sets the armour type
	#3armourmaterial <material>#0 - sets the armour material
	#3armourquality <quality>#0 - sets the armour quality";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("spellarmour", (root, spell) => new SpellArmourEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("spellarmour", BuilderFactory,
			"Surrounds the target with spell-forged magical armour",
			HelpText,
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		var configuration = new MagicArmourConfiguration(spell.Gameworld)
		{
			ArmourAppliesProg = spell.Gameworld.AlwaysTrueProg,
			Quality = ItemQuality.Standard,
			ArmourType = spell.Gameworld.ArmourTypes.First(),
			ArmourMaterial = spell.Gameworld.Materials.First(),
			ArmourCanBeObscuredByInventory = false,
			MaximumDamageAbsorbed = new TraitExpression("power*5", spell.Gameworld),
			FullDescriptionAddendum = string.Empty
		};

		var effect = new XElement("Effect", new XAttribute("type", "spellarmour"));
		configuration.SaveToXml(effect);
		return (new SpellArmourEffect(effect, spell), string.Empty);
	}

	protected SpellArmourEffect(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	public MagicArmourConfiguration ArmourConfiguration { get; private set; } = null!;

	protected override string BuilderEffectType => "spellarmour";
	protected override string ShowText => "Spell Armour";

	protected override void LoadFromXml(XElement root)
	{
		ArmourConfiguration = new MagicArmourConfiguration(root, Gameworld);
	}

	protected override void SaveToXml(XElement root)
	{
		ArmourConfiguration.SaveToXml(root);
	}

	public override string Show(ICharacter actor)
	{
		return $"Spell Armour - {ArmourConfiguration.Show(actor)}";
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "part":
			case "bodypart":
			case "shape":
				return BuildingCommandShape(actor, command);
			case "obscured":
				return BuildingCommandObscured(actor);
			case "applies":
				return BuildingCommandApplies(actor, command);
			case "armourtype":
				return BuildingCommandArmourType(actor, command);
			case "armourmaterial":
				return BuildingCommandArmourMaterial(actor, command);
			case "armourquality":
				return BuildingCommandArmourQuality(actor, command);
			case "absorb":
				return BuildingCommandAbsorb(actor, command);
			case "desc":
			case "description":
			case "addendum":
			case "descaddendum":
			case "descriptionaddendum":
				return BuildingCommandDescriptionAddendum(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandShape(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualTo("all"))
		{
			actor.OutputHandler.Send("This armour now applies to all bodyparts of the owner.");
			ArmourConfiguration.CoveredShapes.Clear();
			Spell.Changed = true;
			return true;
		}

		IBodypartShape? shape = Gameworld.BodypartShapes.GetByIdOrName(command.SafeRemainingArgument);
		if (shape is null)
		{
			actor.OutputHandler.Send($"There is no shape identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		Spell.Changed = true;
		if (ArmourConfiguration.CoveredShapes.Remove(shape))
		{
			actor.OutputHandler.Send($"This armour no longer covers {shape.Name.ColourValue()} bodypart shapes.");
		}
		else
		{
			ArmourConfiguration.CoveredShapes.Add(shape);
			actor.OutputHandler.Send($"This armour now covers {shape.Name.ColourValue()} bodypart shapes.");
		}

		return true;
	}

	private bool BuildingCommandObscured(ICharacter actor)
	{
		ArmourConfiguration.ArmourCanBeObscuredByInventory = !ArmourConfiguration.ArmourCanBeObscuredByInventory;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This magical armour can {ArmourConfiguration.ArmourCanBeObscuredByInventory.NowNoLonger()} be obscured by worn items.");
		return true;
	}

	private bool BuildingCommandApplies(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog do you want to use for whether the armour applies?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[
				[ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.Character]
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ArmourConfiguration.ArmourAppliesProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now control whether the armour effect applies.");
		return true;
	}

	private bool BuildingCommandArmourType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which armour type should this magical armour be?");
			return false;
		}

		IArmourType? type = Gameworld.ArmourTypes.GetByIdOrName(command.SafeRemainingArgument);
		if (type is null)
		{
			actor.OutputHandler.Send($"There is no such armour type identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		ArmourConfiguration.ArmourType = type;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This magic armour effect now uses the armour type {type.Name.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandArmourMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which armour material should this magical armour be?");
			return false;
		}

		ISolid? material = Gameworld.Materials.GetByIdOrName(command.SafeRemainingArgument);
		if (material is null)
		{
			actor.OutputHandler.Send($"There is no such material identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		ArmourConfiguration.ArmourMaterial = material;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This magic armour effect now uses the armour material {material.Name.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandArmourQuality(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What effective quality should this magical armour effect be?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<ItemQuality>(out ItemQuality value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid item quality. See {"show qualities".MXPSend()} for a list of valid qualities.");
			return false;
		}

		ArmourConfiguration.Quality = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This magical armour now has an effective armour quality of {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandAbsorb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the formula for how much damage this magical armour can absorb?\nSee {"te".MXPSend()} for a list of possible functions that can be used.");
			return false;
		}

		TraitExpression te = new(command.SafeRemainingArgument, Gameworld);
		if (te.HasErrors())
		{
			actor.OutputHandler.Send(te.Error);
			return false;
		}

		ArmourConfiguration.MaximumDamageAbsorbed = te;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The formula for how much damage this armour can absorb is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandDescriptionAddendum(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a description addendum or use #3none#0 to clear one.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			ArmourConfiguration.FullDescriptionAddendum = string.Empty;
			Spell.Changed = true;
			actor.OutputHandler.Send("This magical armour will no longer apply a description addendum.");
			return true;
		}

		ArmourConfiguration.FullDescriptionAddendum = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This magical armour now applies the following description addendum:\n\n{ArmourConfiguration.FullDescriptionAddendum.SubstituteANSIColour()}");
		return true;
	}

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return new SpellArmourProtectionEffect(target, parent, ArmourConfiguration);
	}

	public override IMagicSpellEffectTemplate Clone()
	{
		return new SpellArmourEffect(SaveToXml(), Spell);
	}
}
