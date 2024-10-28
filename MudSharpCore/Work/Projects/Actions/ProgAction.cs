using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Work.Projects.Actions;

public class ProgAction : BaseAction
{
	public ProgAction(Models.ProjectAction action, IFuturemud gameworld) : base(action, gameworld)
	{
		var root = XElement.Parse(action.Definition);
		ProgToExecute = Gameworld.FutureProgs.Get(long.Parse(root.Value));
	}

	public ProgAction(IProjectPhase phase, IFuturemud gameworld) : base(phase, gameworld, "prog")
	{
	}

	public ProgAction(ProgAction rhs, IProjectPhase newPhase) : base(rhs, newPhase, "prog")
	{
		ProgToExecute = rhs.ProgToExecute;
		Changed = true;
	}

	public IFutureProg ProgToExecute { get; protected set; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Action", ProgToExecute?.Id ?? 0L);
	}

	public override void CompleteAction(IActiveProject project)
	{
		ProgToExecute.Execute(project);
	}

	public override IProjectAction Duplicate(IProjectPhase newPhase)
	{
		return new ProgAction(this, newPhase);
	}

	public override string Show(ICharacter actor)
	{
		return
			$"[{Name}] {ProgToExecute?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)} - {Description}";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		return Description;
	}

	public override (bool Truth, string Error) CanSubmit()
	{
		if (ProgToExecute == null)
		{
			return (false, "You must set a prog to execute.");
		}

		return base.CanSubmit();
	}

	protected override string HelpText => $@"{base.HelpText}
	#3prog <prog>#0 - sets the prog executed when this action finishes";

	public override bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "prog":
				return BuildingCommandProg(actor, command);
		}

		return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"), phase);
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want this action to execute when the phase completes?");
			return false;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { ProgVariableTypes.Project }))
		{
			actor.OutputHandler.Send("You must specify a prog that accepts a single project as a parameter.");
			return false;
		}

		ProgToExecute = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This action will now execute the {ProgToExecute.MXPClickableFunctionNameWithId()} prog when the phase completes.");
		return true;
	}
}