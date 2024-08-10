using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;
public class MendEffect : IMagicSpellEffectTemplate
{
	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld => Spell.Gameworld;

	#endregion

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("mend", (root, spell) => new MendEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("mend", BuilderFactory);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new MendEffect(new XElement("Effect",
			new XAttribute("type", "mend"),
			new XElement("HealWorstWoundsFirst", true),
			new XElement("HealOverflow", true),
			new XElement("HealingAmount", new XCData("power*outcome"))
		), spell), string.Empty);
	}

	protected MendEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		HealWorstWoundsFirst = bool.Parse(root.Element("HealWorstWoundsFirst").Value);
		HealOverflow = bool.Parse(root.Element("HealOverflow").Value);
		HealingAmount = new TraitExpression(root.Element("HealingAmount").Value, Gameworld);
	}

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "mend"),
			new XElement("HealWorstWoundsFirst", HealWorstWoundsFirst),
			new XElement("HealOverflow", HealOverflow),
			new XElement("HealingAmount", new XCData(HealingAmount.ToString()))
		);
	}

	public IMagicSpell Spell { get; }

	public bool HealWorstWoundsFirst { get; set; }
	public ITraitExpression HealingAmount { get; set; }
	public bool HealOverflow { get; set; }

	#region Implementation of IEditableItem
	public string Show(ICharacter actor)
	{
		return
			$"MendEffect - {HealingAmount.OriginalFormulaText.ColourCommand()} - {(HealWorstWoundsFirst ? "[WorstFirst]".Colour(Telnet.BoldYellow) : "[RandomTarget]".Colour(Telnet.Magenta))} {(HealOverflow ? "[Overflow]".Colour(Telnet.BoldGreen) : "[Single]".Colour(Telnet.BoldYellow))}";
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

	#3worst#0 - toggles mending worst wound first (as opposed to random)
	#3overflow#0 - toggles overflow mending the next wound (as opposed to only a single wound)
	#3formula <formula>#0 - sets the formula for mending amount. See below for possible parameters.

Parameters for healing formula:

	#6power#0 - the power of the spell 0 (Insignificant) to 10 (Recklessly Powerful)
	#6outcome#0 - the outcome of the skill check 0 (Marginal) to 5 (Total)

You can also use the traits of the caster as per #3TE HELP#0.".SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandFormula(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the mending formula to?");
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
		actor.OutputHandler.Send($"The formula for mending amount is now {formula.OriginalFormulaText.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandWorst(ICharacter actor)
	{
		HealWorstWoundsFirst = !HealWorstWoundsFirst;
		Spell.Changed = true;
		actor.OutputHandler.Send(HealWorstWoundsFirst ?
			"This spell will now mend the worst wounds first." :
			"This spell will now mend wounds in a random order.");
		return true;
	}

	private bool BuildingCommandOverflow(ICharacter actor)
	{
		HealOverflow = !HealOverflow;
		Spell.Changed = true;
		actor.OutputHandler.Send(HealOverflow ?
			"This spell will now overflow excess mending onto the next wounds." :
			"This spell will now only mend a single wound.");
		return true;
	}

	#endregion

	#region Implementation of IMagicSpellEffectTemplate

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent)
	{
		if (target is not IHaveWounds hw)
		{
			return null;
		}

		var amount = HealingAmount.EvaluateWith(caster, values: new (string Name, object Value)[]
		{
			("power", (int)power),
			("outcome", (int)outcome)
		});

		var wounds = hw.Wounds
		               .Where(x => x.CanBeTreated(TreatmentType.Repair) != Difficulty.Impossible)
		               .ToList();
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

		hw.EvaluateWounds();
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new MendEffect(SaveToXml(), Spell);
	}

	#endregion
}
