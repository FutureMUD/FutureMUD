using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;
public class RelocateEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("relocate", (root, spell) => new RelocateEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("relocate", BuilderFactory,
			"Relocates a dislocated bone",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new RelocateEffect(new XElement("Effect",
			new XAttribute("type", "relocate"),
			new XElement("HealWorstWoundsFirst", true),
			new XElement("HealOverflow", true),
			new XElement("HealingAmount", new XCData("power*outcome"))
			), spell), string.Empty);
	}

	protected RelocateEffect(XElement root, IMagicSpell spell)
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
			new XAttribute("type", "relocate"),
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
			$"RelocateEffect - {HealingAmount.OriginalFormulaText.ColourCommand()} - {(HealWorstWoundsFirst ? "[WorstFirst]".Colour(Telnet.BoldYellow) : "[RandomTarget]".Colour(Telnet.Magenta))} {(HealOverflow ? "[Overflow]".Colour(Telnet.BoldGreen) : "[Single]".Colour(Telnet.BoldYellow))}";
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3worst#0 - toggles relocating worst wound first (as opposed to easiest)
	#3overflow#0 - toggles overflow relocating the next wound (as opposed to only a single wound)
	#3formula <formula>#0 - sets the formula for relocation total difficulty amount. See below for possible parameters.

Parameters for healing formula:

	#6power#0 - the power of the spell 0 (Insignificant) to 10 (Recklessly Powerful)
	#6outcome#0 - the outcome of the skill check 0 (Marginal) to 5 (Total)

The result of the formula is the number of ""levels"" of difficulty that can be relocated, e.g. a ""normal"" difficulty relocation requires >5.0 healing.

You can also use the traits of the caster as per #3TE HELP#0.";

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

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
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
		actor.OutputHandler.Send($"The formula for relocation healing amount is now {formula.OriginalFormulaText.ColourCommand()} difficulty levels.");
		return true;
	}

	private bool BuildingCommandWorst(ICharacter actor)
	{
		HealWorstWoundsFirst = !HealWorstWoundsFirst;
		Spell.Changed = true;
		actor.OutputHandler.Send(HealWorstWoundsFirst ?
			"This spell will now relocate the worst wounds first." :
			"This spell will now relocate the easiest wounds first.");
		return true;
	}

	private bool BuildingCommandOverflow(ICharacter actor)
	{
		HealOverflow = !HealOverflow;
		Spell.Changed = true;
		actor.OutputHandler.Send(HealOverflow ?
			"This spell will now overflow excess relocation onto the next wounds." :
			"This spell will now only relocate a single wound.");
		return true;
	}

	#endregion

	#region Implementation of IMagicSpellEffectTemplate

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

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
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

		var wounds = tch.Body.Wounds
						.Where(x => x.CanBeTreated(TreatmentType.Relocation) != Difficulty.Impossible)
						.ToList();
		if (HealWorstWoundsFirst)
		{
			wounds = wounds.OrderByDescending(x => x.CanBeTreated(TreatmentType.Relocation)).ToList();
		}
		else
		{
			wounds = wounds.OrderBy(x => x.CanBeTreated(TreatmentType.Relocation)).ToList();
		}

		while (amount > 0.0)
		{
			var wound = wounds.FirstOrDefault();
			if (wound is null)
			{
				break;
			}

			var difficulty = (int)wound.CanBeTreated(TreatmentType.Relocation);

			if (amount >= difficulty)
			{
				amount -= difficulty;
				wound.Treat(null, TreatmentType.Relocation, null, Outcome.MajorPass, false);
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
		return new RelocateEffect(SaveToXml(), Spell);
	}

	#endregion
}
