using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.Work.Projects.ConcreteTypes;

public abstract class Project : EditableItem, IProject
{
	protected Project(MudSharp.Models.Project project, IFuturemud gameworld) : base(project.EditableItem)
	{
		Gameworld = gameworld;
		_id = project.Id;
		RevisionNumber = project.RevisionNumber;
		_name = project.Name;
		AppearInJobsList = project.AppearInJobsList;
		foreach (var phase in project.ProjectPhases)
		{
			_phases.Add(new ProjectPhase(phase, gameworld));
		}

		LoadFromXml(XElement.Parse(project.Definition));
	}

	protected virtual void LoadFromXml(XElement root)
	{
		Tagline = root.Element("Tagline")?.Value ??
		          throw new ApplicationException($"Project definition {Id} did not have a tagline element.");
		AppearInProjectListProg =
			long.TryParse(
				root.Element("AppearInProjectListProg")?.Value ??
				throw new ApplicationException(
					$"Project definition {Id} did not have an AppearInProjectListProg element."), out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(root.Element("AppearInProjectListProg").Value);
		if (AppearInProjectListProg != null)
		{
			if (!AppearInProjectListProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
			{
				throw new ApplicationException(
					$"Project definition {Id} specified an AppearInProjectListProg that does not return a boolean.");
			}

			if (!AppearInProjectListProg.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
			{
				throw new ApplicationException(
					$"Project definition {Id} specified an AppearInProjectListProg that does not accept only a single character as a parameter.");
			}
		}

		CanInitiateProg =
			long.TryParse(
				root.Element("CanInitiateProg")?.Value ??
				throw new ApplicationException($"Project definition {Id} did not have a CanInitiateProg element."),
				out value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(root.Element("CanInitiateProg").Value);
		if (CanInitiateProg != null)
		{
			if (!CanInitiateProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
			{
				throw new ApplicationException(
					$"Project definition {Id} specified a CanInitiateProg that does not return a boolean.");
			}

			if (!CanInitiateProg.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
			{
				throw new ApplicationException(
					$"Project definition {Id} specified a CanInitiateProg that does not accept only a single character as a parameter.");
			}
		}

		WhyCannotInitiateProg =
			long.TryParse(
				root.Element("WhyCannotInitiateProg")?.Value ??
				throw new ApplicationException(
					$"Project definition {Id} did not have a WhyCannotInitiateProg element."), out value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(root.Element("WhyCannotInitiateProg").Value);
		if (WhyCannotInitiateProg != null)
		{
			if (!WhyCannotInitiateProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Text))
			{
				throw new ApplicationException(
					$"Project definition {Id} specified a WhyCannotInitiateProg that does not return text.");
			}

			if (!WhyCannotInitiateProg.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
			{
				throw new ApplicationException(
					$"Project definition {Id} specified a WhyCannotInitiateProg that does not accept only a single character as a parameter.");
			}
		}

		OnStartProg = long.TryParse(root.Element("OnStartProg")?.Value ?? "0", out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(root.Element("OnStartProg").Value);
		if (OnStartProg != null)
		{
			if (!OnStartProg.MatchesParameters(new[] { FutureProgVariableTypes.Project }))
			{
				throw new ApplicationException(
					$"Project definition {Id} specified a OnStartProg that does not accept only a single project as a parameter.");
			}
		}

		OnFinishProg = long.TryParse(root.Element("OnFinishProg")?.Value ?? "0", out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(root.Element("OnFinishProg").Value);
		if (OnFinishProg != null)
		{
			if (!OnFinishProg.MatchesParameters(new[] { FutureProgVariableTypes.Project }))
			{
				throw new ApplicationException(
					$"Project definition {Id} specified a OnFinishProg that does not accept only a single project as a parameter.");
			}
		}

		OnCancelProg = long.TryParse(root.Element("OnCancelProg")?.Value ?? "0", out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(root.Element("OnCancelProg").Value);
		if (OnCancelProg != null)
		{
			if (!OnCancelProg.MatchesParameters(new[] { FutureProgVariableTypes.Project }))
			{
				throw new ApplicationException(
					$"Project definition {Id} specified a OnCancelProg that does not accept only a single project as a parameter.");
			}
		}
	}

	protected Project(IAccount originator, string type) : base(originator)
	{
		Gameworld = originator.Gameworld;
		_name = "New Project";
		Tagline = "A short description of the project";
		_id = Gameworld.Projects.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		using (new FMDB())
		{
			var dbitem = new Models.Project
			{
				EditableItem = new Models.EditableItem()
			};
			FMDB.Context.Projects.Add(dbitem);
			FMDB.Context.EditableItems.Add(dbitem.EditableItem);
			dbitem.EditableItem.RevisionStatus = (int)Status;
			dbitem.EditableItem.RevisionNumber = 0;
			dbitem.EditableItem.BuilderAccountId = BuilderAccountID;
			dbitem.EditableItem.BuilderComment = BuilderComment;
			dbitem.EditableItem.BuilderDate = BuilderDate;
			dbitem.Id = Id;
			dbitem.RevisionNumber = RevisionNumber;
			dbitem.Name = Name;
			dbitem.Type = type;
			dbitem.Definition = SaveDefinition().ToString();
			FMDB.Context.SaveChanges();
			_phases.Add(new ProjectPhase(Gameworld, this));
		}
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.Projects.GetAll(Id);
	}

	protected XElement SaveDefinition()
	{
		return SaveDefinition(new XElement("Project",
			new XElement("Tagline", new XCData(Tagline)),
			new XElement("AppearInProjectListProg", AppearInProjectListProg?.Id ?? 0),
			new XElement("CanInitiateProg", CanInitiateProg?.Id ?? 0),
			new XElement("WhyCannotInitiateProg", WhyCannotInitiateProg?.Id ?? 0),
			new XElement("OnStartProg", OnStartProg?.Id ?? 0),
			new XElement("OnFinishProg", OnFinishProg?.Id ?? 0),
			new XElement("OnCancelProg", OnCancelProg?.Id ?? 0)
		));
	}

	public string Tagline { get; protected set; }

	public IFutureProg AppearInProjectListProg { get; protected set; }

	public IFutureProg CanInitiateProg { get; protected set; }

	public IFutureProg WhyCannotInitiateProg { get; protected set; }

	public IFutureProg OnStartProg { get; protected set; }
	public IFutureProg OnCancelProg { get; protected set; }
	public IFutureProg OnFinishProg { get; protected set; }
	public bool AppearInJobsList { get; protected set; }

	public abstract IEnumerable<string> ProjectCatalogueColumns(ICharacter actor);

	public bool AppearInProjectList(ICharacter actor)
	{
		return
			Status == RevisionStatus.Current &&
			AppearInProjectListProg.Execute<bool?>(actor) == true;
	}

	public bool CanInitiateProject(ICharacter actor)
	{
		return Status == RevisionStatus.Current &&
		       CanInitiateProg.Execute<bool?>(actor) == true &&
		       !actor.PersonalProjects.Any(x => x.ProjectDefinition.Id == Id);
	}

	public string WhyCannotInitiateProject(ICharacter actor)
	{
		if (Status != RevisionStatus.Current)
		{
			return "That project is not approved for use.";
		}

		if (CanInitiateProg.Execute<bool?>(actor) != true)
		{
			return WhyCannotInitiateProg.Execute<string>(actor);
		}

		return "You cannot start multiple instances of the same personal project.";
	}

	public abstract string ShowToPlayer(ICharacter actor);

	public abstract void InitiateProject(ICharacter actor);

	public abstract bool CanCancelProject(ICharacter actor, IActiveProject local);

	private readonly List<IProjectPhase> _phases = new();
	public IEnumerable<IProjectPhase> Phases => _phases;

	protected virtual string HelpText => @"The valid options for this command are:

	#3name <name>#0 - sets the name of this project
	#3tagline <tagline>#0 - sets the tagline for project catalogue
	#3appear <prog>#0 - sets the prog that determines appearance in catalogue
	#3can <prog>#0 - sets the caninitiateprog
	#3why <prog>#0 - sets the whycannotinitiateprog
	#3start <prog>#0 - sets the OnStartProg
	#3start clear#0 - clears the OnStartProg
	#3finish <prog>#0 - sets the OnFinishProg
	#3finish clear#0 - clears the OnFinishProg
	#3cancel <prog>#0 - sets the OnCancelProg
	#3cancel clear#0 - clears the OnCancelProg

Editing Phases:

	#3phase add#0 - creates a new phase at the end of the existing ones
	#3phase remove <phase>#0 - removes an existing phase
	#3phase duplicate <phase>#0 - creates a new phase that is a copy of the specified phase
	#3phase swap <phase1> <phase2>#0 - swap the position of two phases
	#3phase <phase> description <description>#0 - sets the description of a phase

Editing Labour:

	#3phase <phase> labour add <type>#0 - adds a new requirment with specified type
	#3phase <phase> labour remove <labour>#0 - removes a labour requirement
	#3phase <phase> labour duplicate <labour>#0 - duplicates a labour requirement
	#3phase <phase> labour <labour> show#0 - shows detailed information about this labour requirement
	#3phase <phase> labour <labour> name <name>#0 - renames this requirement
	#3phase <phase> labour <labour> description <description>#0 - sets the description
	#3phase <phase> labour <labour> progress <man-hours>#0 - sets the required man-hours to complete
	#3phase <phase> labour <labour> workers <amount>#0 - sets the maximum number of workers who can simultaneously work on this requirement
	#3phase <phase> labour <labour> mandatory#0 - toggles whether this labour is mandatory to complete the stage
	#3phase <phase> labour <labour> trait <trait>#0 - sets a trait that controls qualification of who can do this labour
	#3phase <phase> labour <labour> trait none#0 - clears the trait that controls qualification
	#3phase <phase> labour <labour> minskill <amount>#0 - sets the minimum value of the trait required to count as qualified
	#3phase <phase> labour <labour> difficulty <difficulty>#0 - sets the difficulty of the trait check made on this project
	#3phase <phase> labour <labour> prog <prog>#0 - sets a prog that controls qualification INSTEAD of a trait
	#3phase <phase> labour <labour> prog none#0 - clears a prog from controlling qualification
	#3phase <phase> labour <labour> ...#0 - further type-specific commands

Editing Labour Impacts:

	#3phase <phase> labour <labour> impact ...#0 - see the impact sub-command for more specific help
	#3phase <phase> labour <labour> impact add <type>#0 - adds a new labour impact
	#3phase <phase> labour <labour> impact duplicate <impact>#0 - duplicates an existing impact
	#3phase <phase> labour <labour> impact delete <impact>#0 - deletes an impact
	#3phase <phase> labour <labour> impact <impact> show#0 - view detailed information about this impact
	#3phase <phase> labour <labour> impact <impact> name <name>#0 - rename this impact
	#3phase <phase> labour <labour> impact <impact> description <description>#0 - sets the description
	#3phase <phase> labour <labour> impact <impact> hours <hours>#0 - hours before this impact kicks in
	#3phase <phase> labour <labour> impact <impact> ...#0 - further type-specific commands

Editing Materials:
	#3phase <phase> material add <type>#0 - adds a new requirement with specified type
	#3phase <phase> material remove <material>#0 - removes a material requirement
	#3phase <phase> material duplicate <material>#0 - duplicates a material requirement
	#3phase <phase> material ...#0 - see the material sub-command for full help

Editing Actions:

	#3phase <phase> action add <type>#0 - adds a new action with specified type
	#3phase <phase> action remove <action>#0 - removes an action
	#3phase <phase> action duplicate <action>#0 - duplicates an action
	#3phase <phase> action <action> name <name>#0 - rename this action
	#3phase <phase> action <action> description <description>#0 - sets the description
	#3phase <phase> action <action> sort <number>#0 - set the sort order of action execution
	#3phase <phase> action <action> ...#0 - see sub-command for further help";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "tagline":
				return BuildingCommandTagline(actor, command);
			case "phase":
				return BuildingCommandPhase(actor, command);
			case "appear":
			case "appearprog":
			case "appear_prog":
			case "appear prog":
			case "appears":
			case "appearsprog":
			case "appears_prog":
			case "appears prog":
				return BuildingCommandAppear(actor, command);
			case "start":
			case "begin":
			case "onstart":
			case "onbegin":
				return BuildingCommandOnStartProg(actor, command);
			case "end":
			case "finish":
			case "onfinish":
			case "onend":
				return BuildingCommandOnFinishProg(actor, command);
			case "cancel":
			case "oncancel":
				return BuildingCommandOnCancelProg(actor, command);
			case "can":
			case "caninitiate":
			case "initiate":
			case "can initiate":
			case "can_initiate":
			case "caninitiateprog":
			case "initiateprog":
			case "can initiateprog":
			case "can_initiateprog":
				return BuildingCommandCanInitiate(actor, command);
			case "cant":
			case "can't":
			case "why":
			case "whycant":
			case "why cant":
			case "why_cant":
			case "why can't":
			case "why_can't":
			case "whycan't":
			case "cantprog":
			case "can'tprog":
			case "whyprog":
			case "whycantprog":
			case "why cantprog":
			case "why_cantprog":
			case "why can'tprog":
			case "why_can'tprog":
			case "whycan'tprog":
				return BuildingCommandWhyCannotInitiate(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandOnStartProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use 'clear' to remove the existing one.");
			return false;
		}

		if (command.Peek().EqualToAny("clear", "none", "delete", "remove"))
		{
			OnStartProg = null;
			Changed = true;
			actor.OutputHandler.Send("This project will no longer have an OnStartProg.");
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Project }))
		{
			actor.OutputHandler.Send("The prog you specify must have only a single project as a parameter.");
			return false;
		}

		OnStartProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This project will now launch the {prog.MXPClickableFunctionNameWithId()} prog when it first begins.");
		return true;
	}

	private bool BuildingCommandOnFinishProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use 'clear' to remove the existing one.");
			return false;
		}

		if (command.Peek().EqualToAny("clear", "none", "delete", "remove"))
		{
			OnFinishProg = null;
			Changed = true;
			actor.OutputHandler.Send("This project will no longer have an OnFinishProg.");
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Project }))
		{
			actor.OutputHandler.Send("The prog you specify must have only a single project as a parameter.");
			return false;
		}

		OnFinishProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This project will now launch the {prog.MXPClickableFunctionNameWithId()} prog when it completes.");
		return true;
	}

	private bool BuildingCommandOnCancelProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use 'clear' to remove the existing one.");
			return false;
		}

		if (command.Peek().EqualToAny("clear", "none", "delete", "remove"))
		{
			OnCancelProg = null;
			Changed = true;
			actor.OutputHandler.Send("This project will no longer have an OnCancelProg.");
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Project }))
		{
			actor.OutputHandler.Send("The prog you specify must have only a single project as a parameter.");
			return false;
		}

		OnCancelProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This project will now launch the {prog.MXPClickableFunctionNameWithId()} prog when it is cancelled before it has complete.");
		return true;
	}

	private bool BuildingCommandWhyCannotInitiate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use to send error messages about why it can't be initated?");
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

		if (prog.ReturnType != FutureProgVariableTypes.Text)
		{
			actor.OutputHandler.Send("The WhyCannotInitiateProg must return a text value.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send("The WhyCannotInitiateProg must be compatible with a single character parameter.");
			return false;
		}

		WhyCannotInitiateProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This project will now use the {prog.FunctionName.Colour(Telnet.Cyan)} prog to send error messages about why it can't be initiated.");
		return true;
	}

	private bool BuildingCommandCanInitiate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use to determine whether a character can initiate this project?");
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

		if (prog.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.OutputHandler.Send("The InitiateProg must return a boolean value.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send("The InitiateProg must be compatible with a single character parameter.");
			return false;
		}

		CanInitiateProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This project will now use the {prog.FunctionName.Colour(Telnet.Cyan)} prog to determine whether a character can initiate a project.");
		return true;
	}

	private bool BuildingCommandAppear(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use to control whether this project appears in people's catalogue?");
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

		if (prog.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.OutputHandler.Send("The AppearProg must return a boolean value.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send("The AppearProg must be compatible with a single character parameter.");
			return false;
		}

		AppearInProjectListProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This project will now use the {prog.FunctionName.Colour(Telnet.Cyan)} prog to determine whether it appears in project catalogues.");
		return true;
	}

	private bool BuildingCommandPhase(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "new":
			case "add":
			case "create":
				return BuildingCommandPhaseAdd(actor, command);
			case "delete":
			case "remove":
			case "del":
			case "rem":
				return BuildingCommandPhaseRemove(actor, command);
			case "swap":
				return BuildingCommandPhaseSwap(actor, command);
			case "duplicate":
			case "copy":
				return BuildingCommandPhaseDuplicate(actor, command);
			case "help":
			case "?":
			case "":
				actor.OutputHandler.Send(@"You can use the following options with the phase subcommand:

	#3add#0 - creates a new phase at the end of the existing ones
	#3remove <phase>#0 - removes an existing phase
	#3duplicate <phase>#0 - creates a new phase that is a copy of the specified phase
	#3swap <phase1> <phase2>#0 - swap the position of two phases
	#3<phase> description <description>#0 - sets the description of a phase
	#3<phase> labour ...#0 - see the labour sub-command for full help
	#3<phase> material ...#0 - see the material sub-command for full help".ColourCommand());
				return true;
		}

		var phase = TargetPhase(command.Last);
		if (phase == null)
		{
			actor.OutputHandler.Send("There is no such phase.");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "description":
			case "desc":
				return BuildingCommandPhaseDescription(actor, phase, command);
			case "labour":
			case "labor":
				return BuildingCommandPhaseLabour(actor, phase, command);
			case "material":
			case "mat":
				return BuildingCommandPhaseMaterial(actor, phase, command);
			case "action":
				return BuildingCommandPhaseAction(actor, phase, command);
			default:
				actor.OutputHandler.Send(@"You can use the following options with the phase subcommand:

	#3add#0 - creates a new phase at the end of the existing ones
	#3remove <phase>#0 - removes an existing phase
	#3duplicate <phase>#0 - creates a new phase that is a copy of the specified phase
	#3swap <phase1> <phase2>#0 - swap the position of two phases
	#3<phase> description <description>#0 - sets the description of a phase
	#3<phase> labour ...#0 - see the labour sub-command for full help
	#3<phase> material ...#0 - see the material sub-command for full help".ColourCommand());
				return false;
		}
	}

	private bool BuildingCommandPhaseAction(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandPhaseActionCreate(actor, phase, command);
			case "delete":
			case "del":
			case "remove":
			case "rem":
				return BuildingCommandPhaseActionRemove(actor, phase, command);
			case "duplicate":
			case "copy":
				return BuildingCommandPhaseActionDuplicate(actor, phase, command);
			case "help":
			case "?":
			case "":
				actor.OutputHandler.Send(@"The syntax options for this subcommand are as follows:

	#3add <type>#0 - adds a new action with specified type
	#3remove <action>#0 - removes an action
	#3duplicate <action>#0 - duplicates an action
	#3<action> ...#0 - see sub-command for further help".SubstituteANSIColour());
				return false;
		}

		var action = phase.CompletionActions.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
		             phase.CompletionActions.FirstOrDefault(x =>
			             x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (action == null)
		{
			actor.OutputHandler.Send("There is no such completion action.");
			return false;
		}

		return action.BuildingCommand(actor, command, phase);
	}

	private bool BuildingCommandPhaseActionDuplicate(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which completion action did you want to duplicate?");
			return false;
		}

		var text = command.PopSpeech();
		var action = phase.CompletionActions.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		             phase.CompletionActions.FirstOrDefault(x =>
			             x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (action == null)
		{
			actor.OutputHandler.Send("There is no such completion action.");
			return false;
		}

		var newAction = action.Duplicate(phase);
		phase.AddAction(newAction);
		actor.OutputHandler.Send(
			$"You duplicate completion action {action.Name.ColourValue()} as {newAction.Name.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPhaseActionRemove(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which completion action did you want to remove?");
			return false;
		}

		var text = command.PopSpeech();
		var action = phase.CompletionActions.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		             phase.CompletionActions.FirstOrDefault(x =>
			             x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (action == null)
		{
			actor.OutputHandler.Send("There is no such completion action.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to permanently delete the completion action called {action.Name.ColourValue()} from that project phase?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = txt =>
			{
				actor.OutputHandler.Send(
					$"You delete completion action {action.Name.ColourValue()} from the project phase.");
				action.Delete();
			},
			RejectAction = txt =>
			{
				actor.OutputHandler.Send("You decide against deleting the project completion action.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide against deleting the project completion action.");
			},
			DescriptionString = $"Deleting a project completion action {action.Name.ColourValue()}",
			Keywords = new List<string> { "action", "delete", "project" }
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandPhaseActionCreate(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of completion action do you want to create?\nValid types are {ProjectFactory.ValidActionTypes.Select(x => x.Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		var action = ProjectFactory.CreateAction(phase, Gameworld, command.PopSpeech());
		if (action == null)
		{
			actor.OutputHandler.Send(
				$"There is no such completion action type.\nValid types are {ProjectFactory.ValidActionTypes.Select(x => x.Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		phase.AddAction(action);
		actor.OutputHandler.Send(
			$"You create a new completion action called {action.Name.Colour(Telnet.Green)} within phase {phase.PhaseNumber.ToString("N0", actor).ColourValue()} of the project.");
		return true;
	}

	protected IProjectPhase TargetPhase(string keyword)
	{
		if (int.TryParse(keyword, out var value))
		{
			return _phases.ElementAtOrDefault(value - 1);
		}

		if (keyword.EqualTo("last"))
		{
			return _phases.LastOrDefault();
		}

		if (keyword.EqualTo("first"))
		{
			return _phases.FirstOrDefault();
		}

		return null;
	}

	private bool BuildingCommandPhaseMaterial(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandPhaseMaterialCreate(actor, phase, command);
			case "delete":
			case "del":
			case "remove":
			case "rem":
				return BuildingCommandPhaseMaterialRemove(actor, phase, command);
			case "duplicate":
			case "copy":
				return BuildingCommandPhaseMaterialDuplicate(actor, phase, command);
			case "help":
			case "?":
			case "":
				actor.OutputHandler.Send(@"The syntax options for this subcommand are as follows:

	#3add <type>#0 - adds a new requirement with specified type
	#3remove <material>#0 - removes a material requirement
	#3duplicate <material>#0 - duplicates a material requirement
	#3<material> ...#0 - see sub-command for further help".SubstituteANSIColour());
				return false;
		}

		var material = phase.MaterialRequirements.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
		               phase.MaterialRequirements.FirstOrDefault(x =>
			               x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material requirement.");
			return false;
		}

		return material.BuildingCommand(actor, command, phase);
	}

	private bool BuildingCommandPhaseMaterialDuplicate(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which material requirement did you want to duplicate?");
			return false;
		}

		var text = command.PopSpeech();
		var material = phase.MaterialRequirements.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		               phase.MaterialRequirements.FirstOrDefault(x =>
			               x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material requirement.");
			return false;
		}

		var newMaterial = material.Duplicate(phase);
		phase.AddMaterial(newMaterial);
		actor.OutputHandler.Send(
			$"You duplicate material requirement {material.Name.ColourValue()} as {newMaterial.Name.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPhaseMaterialRemove(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which material requirement did you want to remove?");
			return false;
		}

		var text = command.PopSpeech();
		var material = phase.MaterialRequirements.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		               phase.MaterialRequirements.FirstOrDefault(x =>
			               x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material requirement.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to permanently delete the material requirement called {material.Name.ColourValue()} from that project phase?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = txt =>
			{
				actor.OutputHandler.Send(
					$"You delete project material requirement {material.Name.ColourValue()} from the project phase.");
				material.Delete();
			},
			RejectAction = txt =>
			{
				actor.OutputHandler.Send("You decide against deleting the project material requirement.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide against deleting the project material requirement.");
			},
			DescriptionString = $"Deleting a project material requirement {material.Name.ColourValue()}",
			Keywords = new List<string> { "material", "delete", "project" }
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandPhaseMaterialCreate(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of material requirement do you want to create?\nValid types are {ProjectFactory.ValidMaterialTypes.Select(x => x.Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		var material = ProjectFactory.CreateMaterial(phase, Gameworld, command.PopSpeech());
		if (material == null)
		{
			actor.OutputHandler.Send(
				$"There is no such material requirement type.\nValid types are {ProjectFactory.ValidMaterialTypes.Select(x => x.Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		phase.AddMaterial(material);
		actor.OutputHandler.Send(
			$"You create a new material requirement called {material.Name.Colour(Telnet.Green)} within phase {phase.PhaseNumber.ToString("N0", actor).ColourValue()} of the project.");
		return true;
	}

	private bool BuildingCommandPhaseLabour(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandPhaseLabourCreate(actor, phase, command);
			case "delete":
			case "del":
			case "remove":
			case "rem":
				return BuildingCommandPhaseLabourRemove(actor, phase, command);
			case "duplicate":
			case "copy":
				return BuildingCommandPhaseLabourDuplicate(actor, phase, command);
			case "help":
			case "?":
			case "":
				actor.OutputHandler.Send(@"The syntax options for this subcommand are as follows:
	#3add <type>#0 - adds a new requirment with specified type
	#3remove <labour>#0 - removes a labour requirement
	#3duplicate <labour>#0 - duplicates a labour requirement
	#3<labour> show#0 - shows detailed information about this labour requirement
	#3<labour> name <name>#0 - renames this requirement
	#3<labour> description <description>#0 - sets the description
	#3<labour> progress <man-hours>#0 - sets the required man-hours to complete
	#3<labour> workers <amount>#0 - sets the maximum number of workers who can simultaneously work on this requirement
	#3<labour> mandatory#0 - toggles whether this labour is mandatory to complete the stage
	#3<labour> trait <trait>#0 - sets a trait that controls qualification of who can do this labour
	#3<labour> trait none#0 - clears the trait that controls qualification
	#3<labour> minskill <amount>#0 - sets the minimum value of the trait required to count as qualified
	#3<labour> difficulty <difficulty>#0 - sets the difficulty of the trait check made on this project
	#3<labour> prog <prog>#0 - sets a prog that controls qualification INSTEAD of a trait
	#3<labour> prog none#0 - clears a prog from controlling qualification
	#3<labour> impact ...#0 - see the impact sub-command for more specific help".SubstituteANSIColour());
				return false;
		}

		var labour = phase.LabourRequirements.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
		             phase.LabourRequirements.FirstOrDefault(x =>
			             x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (labour == null)
		{
			actor.OutputHandler.Send("There is no such labour requirement.");
			return false;
		}

		return labour.BuildingCommand(actor, command, phase);
	}

	private bool BuildingCommandPhaseLabourDuplicate(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which labour requirement did you want to duplicate?");
			return false;
		}

		var text = command.PopSpeech();
		var labour = phase.LabourRequirements.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		             phase.LabourRequirements.FirstOrDefault(x =>
			             x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (labour == null)
		{
			actor.OutputHandler.Send("There is no such labour requirement.");
			return false;
		}

		var newLabour = labour.Duplicate(phase);
		phase.AddLabour(newLabour);
		actor.OutputHandler.Send(
			$"You duplicate labour requirement {labour.Name.ColourValue()} as {newLabour.Name.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPhaseLabourRemove(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which labour requirement did you want to remove?");
			return false;
		}

		var text = command.PopSpeech();
		var labour = phase.LabourRequirements.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		             phase.LabourRequirements.FirstOrDefault(x =>
			             x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (labour == null)
		{
			actor.OutputHandler.Send("There is no such labour requirement.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to permanently delete the labour requirement called {labour.Name.ColourValue()} from that project phase?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = txt =>
			{
				actor.OutputHandler.Send(
					$"You delete project labour requirement {labour.Name.ColourValue()} from the project phase.");
				labour.Delete();
			},
			RejectAction = txt =>
			{
				actor.OutputHandler.Send("You decide against deleting the project labour requirement.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide against deleting the project labour requirement.");
			},
			DescriptionString = $"Deleting a project labour requirement {labour.Name.ColourValue()}",
			Keywords = new List<string> { "labour", "delete", "project" }
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandPhaseLabourCreate(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of labour requirement do you want to create?\nValid types are {ProjectFactory.ValidLabourTypes.Select(x => x.Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		var labourType = command.PopSpeech();
		var newName = "Work";
		if (!command.IsFinished)
		{
			newName = command.PopSpeech().TitleCase();
		}

		var labour = ProjectFactory.CreateLabour(phase, Gameworld, labourType, newName);
		if (labour == null)
		{
			actor.OutputHandler.Send(
				$"There is no such labour requirement type.\nValid types are {ProjectFactory.ValidLabourTypes.Select(x => x.Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		phase.AddLabour(labour);
		actor.OutputHandler.Send(
			$"You create a new labour requirement called {labour.Name.Colour(Telnet.Green)} within phase {phase.PhaseNumber.ToString("N0", actor).ColourValue()} of the project.");
		return true;
	}

	private bool BuildingCommandPhaseDescription(ICharacter actor, IProjectPhase phase, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description would you like to give to that phase?");
			return false;
		}

		phase.Description = command.SafeRemainingArgument.SubstituteANSIColour().ProperSentences().Fullstop();
		actor.OutputHandler.Send(
			$"You change the description of phase {phase.PhaseNumber.ToString("N0", actor).ColourValue()} to: {phase.Description}");
		return true;
	}

	private bool BuildingCommandPhaseDuplicate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which phase do you want to duplicate?");
			return false;
		}

		var phase = TargetPhase(command.PopSpeech());
		if (phase == null)
		{
			actor.OutputHandler.Send("There is no such phase.");
			return false;
		}

		var newPhase = new ProjectPhase(this, phase);
		_phases.Add(newPhase);
		actor.OutputHandler.Send(
			$"You create a new project phase as a duplicate of phase {phase.PhaseNumber.ToString("N0", actor).ColourValue()}, with phase number {newPhase.PhaseNumber.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPhaseSwap(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which phase do you want to swap the order of?");
			return false;
		}

		var phase = TargetPhase(command.PopSpeech());
		if (phase == null)
		{
			actor.OutputHandler.Send("There is no such phase.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which phase do you want to swap it with?");
			return false;
		}

		var otherPhase = TargetPhase(command.PopSpeech());
		if (otherPhase == null)
		{
			actor.OutputHandler.Send("There is no such phase.");
			return false;
		}

		if (phase == otherPhase)
		{
			actor.OutputHandler.Send("You cannot swap a phase with itself.");
			return false;
		}

		_phases.Swap(phase, otherPhase);
		RenumberPhases();
		actor.OutputHandler.Send(
			$"You swap the order of phases {phase.PhaseNumber.ToString("N0", actor).ColourValue()} and {otherPhase.PhaseNumber.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private void RenumberPhases()
	{
		var i = 0;
		foreach (var phase in Phases)
		{
			phase.PhaseNumber = ++i;
		}
	}

	private bool BuildingCommandPhaseRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which phase do you want to remove?");
			return false;
		}

		var phase = TargetPhase(command.PopSpeech());
		if (phase == null)
		{
			actor.OutputHandler.Send("There is no such phase.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Do you really want to delete project phase {phase.PhaseNumber}?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				if (!_phases.Contains(phase))
				{
					actor.OutputHandler.Send("That phase has already been deleted.");
					return;
				}

				actor.OutputHandler.Send(
					$"You delete project phase {phase.PhaseNumber.ToString("N0", actor).ColourValue()}.");
				_phases.Remove(phase);
				phase.Delete();
				RenumberPhases();
			},
			RejectAction = text => { actor.OutputHandler.Send("You decide not to delete that project phase."); },
			ExpireAction = () => { actor.OutputHandler.Send("You decide not to delete that project phase."); },
			Keywords = new List<string> { "project", "phase", "delete" },
			DescriptionString = $"Deleting project phase {phase.PhaseNumber.ToString("N0", actor)}"
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandPhaseAdd(ICharacter actor, StringStack command)
	{
		var newPhase = new ProjectPhase(Gameworld, this);
		_phases.Add(newPhase);
		actor.OutputHandler.Send(
			$"You create a new project phase, with phase number {newPhase.PhaseNumber.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTagline(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What tagline do you want to give for this project in the PROJECT LIST print out?");
			return false;
		}

		Tagline = command.SafeRemainingArgument.ProperSentences();
		Changed = true;
		actor.OutputHandler.Send($"This project will now have the tagline \"{Tagline.ColourCommand()}\"");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this project?");
			return false;
		}

		_name = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"This project is now called {Name.Colour(Telnet.Cyan)}.");
		return true;
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var dbold = FMDB.Context.Projects.Find(Id, RevisionNumber);
			var dbnew = new Models.Project();
			FMDB.Context.Projects.Add(dbnew);
			dbnew.Id = Id;
			dbnew.RevisionNumber = FMDB.Context.Projects.Where(x => x.Id == Id)
			                           .Select(x => x.RevisionNumber)
			                           .AsEnumerable()
			                           .DefaultIfEmpty(0)
			                           .Max() + 1;
			dbnew.Type = dbold.Type;
			dbnew.Name = Name;
			dbnew.Definition = SaveDefinition().ToString();
			dbnew.EditableItem = new Models.EditableItem();
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = dbnew.RevisionNumber;
			dbnew.EditableItem.BuilderAccountId = initiator.Account.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;
			FMDB.Context.SaveChanges();

			var newProject = (Project)ProjectFactory.LoadProject(dbnew, Gameworld);
			foreach (var phase in Phases)
			{
				newProject._phases.Add(phase.Duplicate(newProject));
			}

			return newProject;
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Projects.Find(Id, RevisionNumber);
		base.Save(dbitem.EditableItem);
		dbitem.Name = Name;
		dbitem.Definition = SaveDefinition().ToString();
		dbitem.AppearInJobsList = AppearInJobsList;
		Changed = false;
	}

	protected abstract XElement SaveDefinition(XElement baseDefinition);

	public override string FrameworkItemType => "Project";

	public override string EditHeader()
	{
		return $"Project {Id}r{RevisionNumber} [{Name}]";
	}

	public override bool CanSubmit()
	{
		if (!_phases.Any())
		{
			return false;
		}

		if (_phases.Any(x => !x.CanSubmit().Truth))
		{
			return false;
		}

		if (AppearInProjectListProg == null)
		{
			return false;
		}

		if (CanInitiateProg == null)
		{
			return false;
		}

		if (WhyCannotInitiateProg == null)
		{
			return false;
		}

		return true;
	}

	public override string WhyCannotSubmit()
	{
		if (!_phases.Any())
		{
			return "You must have at least one phase in every project.";
		}

		var phase = _phases.Select(x => (Phase: x, Result: x.CanSubmit())).FirstOrDefault(x => !x.Result.Truth);
		if (phase.Phase != null)
		{
			return $"[Phase {phase.Phase.PhaseNumber}] {phase.Result.Error}";
		}

		if (AppearInProjectListProg == null)
		{
			return "You must set an AppearInProjectListProg.";
		}

		if (CanInitiateProg == null)
		{
			return "You must set a CanInitiateProg.";
		}

		if (WhyCannotInitiateProg == null)
		{
			return "You must set a WhyCannotInitiateProg.";
		}

		return base.WhyCannotSubmit();
	}

	public override IEnumerable<string> Keywords => new ExplodedString(Name).Words;

	public abstract IActiveProject LoadActiveProject(MudSharp.Models.ActiveProject project);
}