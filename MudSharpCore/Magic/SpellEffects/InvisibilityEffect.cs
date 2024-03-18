using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class InvisibilityEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("invisibility", (root, spell) => new InvisibilityEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("invisibility", BuilderFactory);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new InvisibilityEffect(new XElement("Effect",
			new XAttribute("type", "glow"),
			new XElement("FilterProg", 0)
		), spell), string.Empty);
	}

	public IMagicSpell Spell { get; }
	public IFutureProg FilterProg { get; set; }

	protected InvisibilityEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		FilterProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("FilterProg")?.Value ?? "0"));
	}

	#region Implementation of IXmlSavable

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "glow"),
			new XElement("FilterProg", FilterProg?.Id ?? 0)
		);
	}

	#endregion

	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld => Spell.Gameworld;

	#endregion

	#region Implementation of IEditableItem

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "filter":
			case "prog":
				return BuildingCommandFilter(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following options with this effect:

    #3filter <prog>#0 - sets a prog that controls who can see the invisible thing");
		return false;
	}

	private bool BuildingCommandFilter(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What prog do you want to set as the filter prog for this effect?");
			return false;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes> { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single character (the viewer), whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"The invisibility effect will now use the {prog.MXPClickableFunctionNameWithId()} prog to filter whether it applies (true means that the invisibility applies, e.g. the target cannot see)");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return $"Invisibility - Filter: {FilterProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}";
	}

	#endregion

	#region Implementation of IMagicSpellEffectTemplate

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent)
	{
		if (target is not IGameItem && target is not ICharacter && target is not PerceivableGroup)
		{
			return null;
		}

		return new SpellInvisibilityEffect(target, parent, FilterProg);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new InvisibilityEffect(SaveToXml(), Spell);
	}

	#endregion
}