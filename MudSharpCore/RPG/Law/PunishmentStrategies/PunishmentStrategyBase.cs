using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Law.PunishmentStrategies;

public abstract class PunishmentStrategyBase : IPunishmentStrategy
{
	public IFuturemud Gameworld { get; private set; }

	protected PunishmentStrategyBase(IFuturemud gameworld)
	{
		Gameworld = gameworld;
	}

	protected PunishmentStrategyBase(IFuturemud gameworld, XElement root)
	{
		Gameworld = gameworld;
		OnPunishProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnPunish")?.Value ?? "0"));
	}

	public abstract string TypeSpecificHelpText { get; }

	public string HelpText => $@"You can use the following options with this punishment strategy:

	onpunish <prog>|none - sets a prog that fires when this punishment is chosen{TypeSpecificHelpText}";

	public virtual bool BuildingCommand(ICharacter actor, ILegalAuthority authority, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "onpunish":
				return BuildingCommandOnPunish(actor, command);
			default:
				actor.OutputHandler.Send(HelpText);
				return false;
		}
	}

	protected IFutureProg OnPunishProg { get; private set; }

	private bool BuildingCommandOnPunish(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a prog or use the keyword 'none' to clear an existing prog.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			OnPunishProg = null;
			actor.OutputHandler.Send(
				$"This punishment strategy will no longer call a prog when assigning a punishment.");
			return true;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void,
			new[]
			{
				new[] { FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Text }
			}).LookupProg();

		if (prog is null)
		{
			return false;
		}

		actor.OutputHandler.Send(
			$"This punishment strategy will now call the {prog.MXPClickableFunctionName()} prog when assigning a punishment.");
		OnPunishProg = prog;
		return true;
	}

	public abstract string Describe(IPerceiver voyeur);
	public abstract PunishmentResult GetResult(ICharacter actor, ICrime crime, double severity = 0);
	public abstract PunishmentOptions GetOptions(ICharacter actor, ICrime crime);
	public string SaveResult()
	{
		return SaveResultXElement().ToString();
	}

	public XElement SaveResultXElement()
	{
		var element = new XElement("Punishment",
			new XElement("onpunish", OnPunishProg?.Id ?? 0L)
		);

		SaveSpecificType(element);
		return element;
	}

	protected abstract void SaveSpecificType(XElement root);

	protected void BaseShowText(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"On Punish Prog: {OnPunishProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
	}

	public abstract string Show(ICharacter actor);
}