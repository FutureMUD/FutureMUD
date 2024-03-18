using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Health;
using NCalc;

namespace MudSharp.Magic.SpellEffects;

public class HealEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("heal", (root, spell) => new HealEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("heal", BuilderFactory);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new HealEffect(new XElement("Effect",
			new XAttribute("type", "heal"),
			new XElement("HealWorstWoundsFirst", true),
			new XElement("HealOverflow", true),
			new XElement("HealingAmount", new XCData("power*outcome"))
			), spell), string.Empty);
	}

	protected HealEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		HealWorstWoundsFirst = bool.Parse(root.Element("HealWorstWoundsFirst").Value);
		HealOverflow = bool.Parse(root.Element("HealOverflow").Value);
		HealingAmount = new TraitExpression(root.Element("HealingAmount").Value, Gameworld);
	}

	public IMagicSpell Spell { get; }

	public bool HealWorstWoundsFirst { get; set; }
	public ITraitExpression HealingAmount { get; set; }
	public bool HealOverflow { get; set; }

	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld => Spell.Gameworld;

	#endregion

	#region Implementation of IXmlSavable

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "resurrect"),
			new XElement("HealWorstWoundsFirst", HealWorstWoundsFirst),
			new XElement("HealOverflow", HealOverflow),
			new XElement("HealingAmount", new XCData(HealingAmount.ToString()))
		);
	}

	#endregion

	#region Implementation of IEditableItem
	public string Show(ICharacter actor)
	{
		return
			$"HealEffect - {HealingAmount.OriginalFormulaText.ColourCommand()} - {(HealWorstWoundsFirst ? "[WorstFirst]".Colour(Telnet.BoldYellow) : "[RandomTarget]".Colour(Telnet.Magenta))} {(HealOverflow ? "[Overflow]".Colour(Telnet.BoldGreen) : "[Single]".Colour(Telnet.BoldYellow))}";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "overflow":
				return BuildingCommandOverflow(actor);
			case "worst":
				return BuildingCommandWorst(actor);
			case "formula":
			case "heal":
			case "healing":
				return BuildingCommandFormula(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following options with this effect:

    #3worst#0 - toggles healing worst wound first (as opposed to random)
	#3overflow#0 - toggles overflow healing the next wound (as opposed to only a single wound)
	#3formula <formula>#0 - sets the formula for healing amount. See below for possible parameters.

Parameters for healing formula:

	#6power#0 - the power of the spell 0 (Insignificant) to 10 (Recklessly Powerful)
	#6outcome#0 - the outcome of the skill check 0 (Marginal) to 5 (Total)

You can also use the traits of the caster as per #3TE HELP#0.");
		return false;
	}

	private bool BuildingCommandFormula(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the healing formula to?");
			return false;
		}

		var formula = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (formula.HasErrors())
		{
			actor.OutputHandler.Send(formula.Error);
			return false;
		}

		HealingAmount = formula;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The formula for healing amount is now {formula.OriginalFormulaText.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandWorst(ICharacter actor)
	{
		HealWorstWoundsFirst = !HealWorstWoundsFirst;
		Spell.Changed = true;
		actor.OutputHandler.Send(HealWorstWoundsFirst ?
			"This spell will now heal the worst wounds first." :
			"This spell will now heal wounds in a random order.");
		return true;
	}

	private bool BuildingCommandOverflow(ICharacter actor)
	{
		HealOverflow = !HealOverflow;
		Spell.Changed = true;
		actor.OutputHandler.Send(HealOverflow ?
			"This spell will now overflow excess healing onto the next wounds." :
			"This spell will now only heal a single wound.");
		return true;
	}

	#endregion

	#region Implementation of IMagicSpellEffectTemplate

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent)
	{
		if (target is not ICharacter tch)
		{
			return null;
		}

		var amount = HealingAmount.EvaluateWith(caster, values: new (string Name, object Value)[]
		{
			("power", (int)power),
			("outcome", (int)outcome)
		});

		var wounds = tch.Body.Wounds.ToList();
		if (HealWorstWoundsFirst)
		{
			wounds = wounds.OrderByDescending(x => x.CurrentDamage).ToList();
		}
		else
		{
			wounds = wounds.Shuffle().ToList();
		}

		while (amount > 0.0)
		{
			var wound = wounds.FirstOrDefault();
			if (wound is null)
			{
				break;
			}

			if (amount > wound.CurrentDamage)
			{
				amount -= wound.CurrentDamage;
				wound.CurrentDamage = 0.0;
			}
			else
			{
				wound.CurrentDamage -= amount;
				amount = 0.0;
			}

			if (!HealOverflow)
			{
				break;
			}
		}

		tch.Body.EvaluateWounds();
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new HealEffect(SaveToXml(), Spell);
	}

	#endregion
}