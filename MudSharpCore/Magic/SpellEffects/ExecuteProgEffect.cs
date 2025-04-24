using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Revision;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using System.Xml.Linq;
using MudSharp.FutureProg;

namespace MudSharp.Magic.SpellEffects;
public class ExecuteProgEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("executeprog", (root, spell) => new ExecuteProgEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("executeprog", BuilderFactory,
			"Executes a prog that you supply",
			HelpText,
			true,
			false,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new ExecuteProgEffect(new XElement("Effect",
			new XAttribute("type", "executeprog"),
			new XElement("Prog", 0L)
		), spell), string.Empty);
	}

	protected ExecuteProgEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		Prog = Gameworld.FutureProgs.Get(long.Parse(root.Element("Prog").Value));
	}
	public IFuturemud Gameworld => Spell.Gameworld;

	public IMagicSpell Spell { get; }

	public IFutureProg Prog { get; set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "executeprog"),
			new XElement("Prog", Prog?.Id ?? 0L)
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => false;

	public bool IsCompatibleWithTrigger(IMagicTrigger types) => IsCompatibleWithTrigger(types.TargetTypes);
	public static bool IsCompatibleWithTrigger(string types)
	{
		return true;
	}

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var parameters = new List<object>();
		parameters.Add(caster);
		parameters.Add(target);
		foreach (var item in additionalParameters)
		{
			parameters.Add(item.Item);
		}

		Prog?.Execute(parameters.ToArray());
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new ExecuteProgEffect(SaveToXml(), Spell);
	}

	#region Implementation of IEditableItem

	public const string HelpText = @"You can use the following options with this effect:

	#3prog <which>#0 - sets the prog to be executed";
	public string Show(ICharacter actor)
	{
		return $"Execute Prog [{Prog?.MXPClickableFunctionName() ?? "None".ColourError()}]";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "prog":
				return BuildingCommandProg(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog to execute.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Void, 
			[
				[ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.Item],
				[ProgVariableTypes.Character, ProgVariableTypes.Location],
				[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Location],
				[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Location],
				[ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location]
			]
			).LookupProg();

		if (prog is null)
		{
			return false;
		}

		Prog = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The following prog will be executed: {prog.MXPClickableFunctionName()}");
		return true;
	}
	#endregion
}
