using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.Work.Projects.LabourRequirements;

public abstract class ProjectLabourBase : SaveableItem, IProjectLabourRequirement
{
	public sealed override string FrameworkItemType => "ProjectLabourRequirement";

	protected ProjectLabourBase(Models.ProjectLabourRequirement labour, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = labour.Id;
		_name = labour.Name;
		Description = labour.Description;
		TotalProgressRequired = labour.TotalProgressRequired;
		MaximumSimultaneousWorkers = labour.MaximumSimultaneousWorkers;
		var root = XElement.Parse(labour.Definition);
		IsMandatoryForProjectCompletion = bool.Parse(root.Element("Mandatory")?.Value ?? "true");
		IsQualifiedProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("IsQualifiedProg")?.Value ?? "0"));
		RequiredTrait = Gameworld.Traits.Get(long.Parse(root.Element("RequiredTrait")?.Value ?? "0"));
		MinimumTraitValue = double.Parse(root.Element("MinimumTraitValue")?.Value ?? "0.0");
		TraitCheckDifficulty = (Difficulty)int.Parse(root.Element("TraitCheckDifficulty")?.Value ?? "0");

		foreach (var impact in labour.ProjectLabourImpacts)
		{
			_labourImpacts.Add(ProjectFactory.LoadImpact(impact, Gameworld));
		}
	}

	protected ProjectLabourBase(IFuturemud gameworld, IProjectPhase phase, string type, string name)
	{
		Gameworld = gameworld;
		_name = phase.LabourRequirements.Select(x => x.Name).NameOrAppendNumberToName(name);
		Description = "Elbow grease and gumption.";
		MaximumSimultaneousWorkers = 1;
		TotalProgressRequired = 24.0;
		IsMandatoryForProjectCompletion = true;

		using (new FMDB())
		{
			var dbitem = new Models.ProjectLabourRequirement();
			FMDB.Context.ProjectLabourRequirements.Add(dbitem);
			dbitem.ProjectPhaseId = phase.Id;
			dbitem.Name = _name;
			dbitem.MaximumSimultaneousWorkers = MaximumSimultaneousWorkers;
			dbitem.TotalProgressRequired = TotalProgressRequired;
			dbitem.Type = type;
			dbitem.Description = Description;
			dbitem.Definition = SaveDefinition().ToString();
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	protected ProjectLabourBase(ProjectLabourBase rhs, IProjectPhase newPhase, string type)
	{
		Gameworld = rhs.Gameworld;
		if (newPhase.LabourRequirements.Contains(rhs))
		{
			_name = newPhase.LabourRequirements.Select(x => x.Name).NameOrAppendNumberToName(rhs.Name);
		}
		else
		{
			_name = rhs.Name;
		}

		Description = rhs.Description;
		MaximumSimultaneousWorkers = rhs.MaximumSimultaneousWorkers;
		TotalProgressRequired = rhs.TotalProgressRequired;
		IsMandatoryForProjectCompletion = rhs.IsMandatoryForProjectCompletion;
		IsQualifiedProg = rhs.IsQualifiedProg;
		TraitCheckDifficulty = rhs.TraitCheckDifficulty;
		RequiredTrait = rhs.RequiredTrait;
		MinimumTraitValue = rhs.MinimumTraitValue;

		using (new FMDB())
		{
			var dbitem = new Models.ProjectLabourRequirement();
			FMDB.Context.ProjectLabourRequirements.Add(dbitem);
			dbitem.ProjectPhaseId = newPhase.Id;
			dbitem.Name = _name;
			dbitem.MaximumSimultaneousWorkers = MaximumSimultaneousWorkers;
			dbitem.TotalProgressRequired = TotalProgressRequired;
			dbitem.Type = type;
			dbitem.Description = Description;
			dbitem.Definition = rhs.SaveDefinition().ToString();
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		foreach (var impact in rhs.LabourImpacts)
		{
			_labourImpacts.Add(impact.Duplicate(this));
		}
	}

	protected virtual XElement SaveDefinition()
	{
		return new XElement("Labour",
			new XElement("Mandatory", IsMandatoryForProjectCompletion),
			new XElement("IsQualifiedProg", IsQualifiedProg?.Id ?? 0),
			new XElement("RequiredTrait", RequiredTrait?.Id ?? 0),
			new XElement("MinimumTraitValue", MinimumTraitValue),
			new XElement("TraitCheckDifficulty", (int)TraitCheckDifficulty)
		);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.ProjectLabourRequirements.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.TotalProgressRequired = TotalProgressRequired;
		dbitem.MaximumSimultaneousWorkers = MaximumSimultaneousWorkers;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	protected virtual string HelpText => @"You can use the following options with this labour requirement:

	#3show#0 - shows detailed information about this labour requirement
	#3name <name>#0 - renames this requirement
	#3description <description>#0 - sets the description
	#3progress <man-hours>#0 - sets the required man-hours to complete
	#3workers <amount>#0 - sets the maximum number of workers who can simultaneously work on this requirement
	#3mandatory#0 - toggles whether this labour is mandatory to complete the stage
	#3trait <trait>#0 - sets a trait that controls qualification of who can do this labour
	#3trait none#0 - clears the trait that controls qualification
	#3minskill <amount>#0 - sets the minimum value of the trait required to count as qualified
	#3difficulty <difficulty>#0 - sets the difficulty of the trait check made on this project
	#3prog <prog>#0 - sets a prog that controls qualification INSTEAD of a trait
	#3prog none#0 - clears a prog from controlling qualification
	#3impact ...#0 - see the impact sub-command for more specific help";

	public virtual bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command, phase);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			case "progress":
			case "total":
			case "totalprogress":
			case "total progress":
			case "total_progress":
				return BuildingCommandProgress(actor, command);
			case "max":
			case "max workers":
			case "workers":
			case "max_workers":
			case "maximum":
			case "maximum workers":
			case "maximum_workers":
				return BuildingCommandMaximumWorkers(actor, command);
			case "mandatory":
				return BuildingCommandMandatory(actor);
			case "trait":
			case "skill":
				return BuildingCommandTrait(actor, command);
			case "minskill":
			case "mintrait":
			case "traitmin":
			case "skillmin":
			case "minvalue":
			case "value":
				return BuildingCommandMinimumTraitValue(actor, command);
			case "prog":
			case "qualified":
			case "qualified prog":
			case "qualified_prog":
				return BuildingCommandQualifiedProg(actor, command);
			case "difficulty":
			case "diff":
				return BuildingCommandDifficulty(actor, command);
			case "impact":
				return BuildingCommandImpact(actor, command, phase);
			case "show":
			case "view":
				return BuildingCommandShow(actor, command, phase);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What difficulty should the trait check be?");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.PopSpeech(), out var difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		TraitCheckDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"The trait check for this labour requirement will now be at {difficulty.Describe().ColourValue()} difficulty.");
		return true;
	}

	private bool BuildingCommandQualifiedProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a prog to use for qualification or 'none' to clear an existing prog.");
			return false;
		}

		var text = command.PopSpeech();
		if (text.EqualToAny("none", "clear", "remove", "delete"))
		{
			IsQualifiedProg = null;
			Changed = true;
			actor.OutputHandler.Send(
				"This labour requirement no longer requires a prog to determine qualification and will instead rely on a trait.");
			return true;
		}

		var prog = long.TryParse(text, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(text);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send("You must specify a prog that returns a boolean truth value.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send("You must specify a prog that accepts a single character as a parameter.");
			return false;
		}

		IsQualifiedProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This labour requirement will now use the {IsQualifiedProg.MXPClickableFunctionNameWithId()} prog to determine qualification of workers.");
		return true;
	}

	private bool BuildingCommandMinimumTraitValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What is the minimum value of the required trait that people need to have to work on this labour requirement?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must specify a valid number 0 or greater for the minimum value.");
			return false;
		}

		MinimumTraitValue = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This labour requirement now requires a minimum value of {MinimumTraitValue.ToString("N2", actor).ColourValue()} (or {RequiredTrait?.Decorator.Decorate(MinimumTraitValue).ColourValue() ?? "Undefined".Colour(Telnet.Red)}) to qualify.");
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify the name of a trait or use 'none' to clear this requirement.");
			return false;
		}

		var text = command.PopSpeech();
		if (text.EqualToAny("none", "clear", "remove", "delete"))
		{
			RequiredTrait = null;
			Changed = true;
			actor.OutputHandler.Send(
				"This labour requirement will now rely on a prog rather than a trait to determine who can work on it.");
			return true;
		}

		var trait = long.TryParse(text, out var value) ? Gameworld.Traits.Get(value) : Gameworld.Traits.GetByName(text);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		RequiredTrait = trait;
		Changed = true;
		actor.OutputHandler.Send(
			$"This labour requirement now uses the {RequiredTrait.Name.ColourValue()} trait for qualification and progress.");
		return true;
	}

	private bool BuildingCommandMandatory(ICharacter actor)
	{
		IsMandatoryForProjectCompletion = !IsMandatoryForProjectCompletion;
		Changed = true;
		actor.OutputHandler.Send(
			$"This labour requirement is {(IsMandatoryForProjectCompletion ? "now" : "no longer")} mandatory.");
		return true;
	}

	protected abstract bool BuildingCommandShow(ICharacter actor, StringStack command, IProjectPhase phase);

	private bool BuildingCommandImpact(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandImpactAdd(actor, command, phase);
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommandImpactDelete(actor, command);
			case "duplicate":
			case "copy":
				return BuildingCommandImpactCopy(actor, command);
			case "":
			case "help":
			case "?":
				actor.OutputHandler.Send($@"You can use the following options for labour impacts:

	#3add <type>#0 - adds a new labour impact
	#3duplicate <impact>#0 - duplicates an existing impact
	#3delete <impact>#0 - deletes an impact
	#3<impact> ...#0 - see individual impacts for detailed help".SubstituteANSIColour());
				return true;
		}

		var impact = LabourImpacts.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
		             LabourImpacts.FirstOrDefault(x =>
			             x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (impact == null)
		{
			actor.OutputHandler.Send("That labour requirement has no such impact.");
			return false;
		}

		return impact.BuildingCommand(actor, command, this);
	}

	private bool BuildingCommandImpactAdd(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of labour impact do you want to create?\nValid types are {ProjectFactory.ValidImpactTypes.Select(x => x.Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		var impactType = command.PopSpeech();
		var newName = command.IsFinished ? "Impact" : command.PopSpeech();
		var impact = ProjectFactory.CreateImpact(this, Gameworld, impactType, newName);
		if (impact == null)
		{
			actor.OutputHandler.Send(
				$"There is no such labour impact type.\nValid types are {ProjectFactory.ValidImpactTypes.Select(x => x.Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		_labourImpacts.Add(impact);
		actor.OutputHandler.Send(
			$"You create a new labour impact called {impact.Name.Colour(Telnet.Green)} within the {Name.Colour(Telnet.Green)} labour requirement of phase {phase.PhaseNumber.ToString("N0", actor).ColourValue()} of the project.");
		return true;
	}

	private bool BuildingCommandImpactDelete(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which labour impact did you want to remove?");
			return false;
		}

		var text = command.PopSpeech();
		var impact = LabourImpacts.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		             LabourImpacts.FirstOrDefault(x =>
			             x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (impact == null)
		{
			actor.OutputHandler.Send("There is no such labour impact.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to permanently delete the labour impact called {impact.Name.ColourValue()} from that labour requirement?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = txt =>
			{
				actor.OutputHandler.Send(
					$"You delete labour impact {impact.Name.ColourValue()} from the project labour requirement.");
				_labourImpacts.Remove(impact);
				impact.Delete();
			},
			RejectAction = txt => { actor.OutputHandler.Send("You decide against deleting the labour impact."); },
			ExpireAction = () => { actor.OutputHandler.Send("You decide against deleting the labour impact."); },
			DescriptionString = $"Deleting a project material requirement {impact.Name.ColourValue()}",
			Keywords = new List<string> { "impact", "labour", "labor", "delete", "project" }
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandImpactCopy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which impact did you want to duplicate?");
			return false;
		}

		var text = command.PopSpeech();
		var impact = LabourImpacts.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		             LabourImpacts.FirstOrDefault(x =>
			             x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (impact == null)
		{
			actor.OutputHandler.Send("There is no such impact.");
			return false;
		}

		var newImpact = impact.Duplicate(this);
		_labourImpacts.Add(newImpact);
		actor.OutputHandler.Send(
			$"You duplicate impact {impact.Name.ColourValue()} to a new impact named {newImpact.Name.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMaximumWorkers(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How many workers should be permitted to work on this labour requirement simultaneously?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid, positive amount of maximum workers.");
			return false;
		}

		MaximumSimultaneousWorkers = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"A maximum of {MaximumSimultaneousWorkers.ToString("N0", actor).ColourValue()} workers will now be able to work simultaneously on that labour requirement.");
		return true;
	}

	private bool BuildingCommandProgress(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many base man-hours of work is required to complete this project?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid, positive amount of base man-hours.");
			return false;
		}

		TotalProgressRequired = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This labour requirement will now require a base {TotalProgressRequired.ToString("N2", actor).ColourValue()} man-hours to complete.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to give to this labour requirement?");
			return false;
		}

		Description = command.SafeRemainingArgument.SubstituteANSIColour().ProperSentences().Fullstop();
		actor.OutputHandler.Send(
			$"You change the description of labour requirement {Name.ColourValue()} to {Description}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this labour requirement?");
			return false;
		}

		var name = command.PopSpeech().TitleCase();
		if (phase.LabourRequirements.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a labour requirement with that name. Names must be unique per phase.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the labour requirement {_name.ColourValue()} to {name.ColourValue()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public virtual bool CharacterIsQualified(ICharacter actor)
	{
		if (IsQualifiedProg?.Execute<bool?>(actor) == false)
		{
			return false;
		}

		if (RequiredTrait == null)
		{
			return true;
		}

		if (actor.TraitValue(RequiredTrait, TraitBonusContext.ProjectLabourQualification) <
		    MinimumTraitValue)
		{
			return false;
		}

		if (Gameworld.GetCheck(CheckType.ProjectLabourCheck).TargetNumber(actor, TraitCheckDifficulty, RequiredTrait) <=
		    0)
		{
			return false;
		}

		return true;
	}

	public virtual double HourlyProgress(ICharacter actor, bool previewOnly = false)
	{
		var check = Gameworld.GetCheck(CheckType.ProjectLabourCheck);
		if (!previewOnly)
		{
			check.Check(actor, TraitCheckDifficulty, RequiredTrait);
		}

		return check.TargetNumber(actor, TraitCheckDifficulty, RequiredTrait) / 100.0;
	}

	public virtual double ProgressMultiplierForOtherLabourPerPercentageComplete(IProjectLabourRequirement other,
		IActiveProject project)
	{
		return 1.0;
	}

	public IFutureProg IsQualifiedProg { get; protected set; }
	public ITraitDefinition RequiredTrait { get; protected set; }
	public double MinimumTraitValue { get; protected set; }
	public Difficulty TraitCheckDifficulty { get; protected set; }

	public bool IsMandatoryForProjectCompletion { get; protected set; }
	public double TotalProgressRequired { get; protected set; }
	public virtual double TotalProgressRequiredForDisplay => TotalProgressRequired;
	public string Description { get; protected set; }
	public int MaximumSimultaneousWorkers { get; protected set; }

	public abstract double HoursRemaining(IActiveProject project);

	public void Delete()
	{
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.ProjectLabourRequirements.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.ProjectLabourRequirements.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public abstract IProjectLabourRequirement Duplicate(IProjectPhase newPhase);

	public virtual (bool Truth, string Error) CanSubmit()
	{
		if (RequiredTrait == null && IsQualifiedProg == null)
		{
			return (false, "You must either set an IsQualifiedProg or add a RequiredTrait.");
		}

		var failedImpact = _labourImpacts.Select(x => (Impact: x, Result: x.CanSubmit()))
		                                 .FirstOrDefault(x => !x.Result.Truth);
		if (failedImpact.Impact != null)
		{
			return (false, $"[Impact {failedImpact.Impact.Name}] {failedImpact.Result.Error}");
		}

		return (true, string.Empty);
	}

	public abstract string Show(ICharacter actor);
	public abstract string ShowToPlayer(ICharacter actor);

	private readonly List<ILabourImpact> _labourImpacts = new();

	public IEnumerable<ILabourImpact> LabourImpacts => _labourImpacts;
}