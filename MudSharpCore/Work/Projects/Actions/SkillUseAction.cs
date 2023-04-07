using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Work.Projects.Actions;

public class SkillUseAction : BaseAction
{
	public SkillUseAction(Models.ProjectAction action, IFuturemud gameworld) : base(action, gameworld)
	{
		var root = XElement.Parse(action.Definition);
		TraitDefinition = Gameworld.Traits.Get(long.Parse(root.Element("TraitDefinition").Value));
		NumberOfFreeChecks = int.Parse(root.Element("NumberOfFreeChecks").Value);
		Difficulty = (Difficulty)int.Parse(root.Element("Difficulty").Value);
	}

	public SkillUseAction(IProjectPhase phase, IFuturemud gameworld) : base(phase, gameworld, "skilluse")
	{
		Difficulty = Difficulty.ExtremelyEasy;
		NumberOfFreeChecks = 10;
	}

	public SkillUseAction(SkillUseAction rhs, IProjectPhase newPhase) : base(rhs, newPhase, "skilluse")
	{
		TraitDefinition = rhs.TraitDefinition;
		NumberOfFreeChecks = rhs.NumberOfFreeChecks;
		Difficulty = rhs.Difficulty;
	}

	public ITraitDefinition TraitDefinition { get; protected set; }

	public int NumberOfFreeChecks { get; protected set; }

	public Difficulty Difficulty { get; protected set; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Action",
			new XElement("TraitDefinition", TraitDefinition?.Id ?? 0),
			new XElement("NumberOfFreeChecks", NumberOfFreeChecks),
			new XElement("Difficulty", (int)Difficulty)
		);
	}

	public override void CompleteAction(IActiveProject project)
	{
		var check = Gameworld.GetCheck(CheckType.ProjectSkillUseAction);
		for (var i = 0; i < NumberOfFreeChecks; i++)
		{
			check.Check(project.CharacterOwner, Difficulty, TraitDefinition);
		}
	}

	public override IProjectAction Duplicate(IProjectPhase newPhase)
	{
		return new SkillUseAction(this, newPhase);
	}

	public override string Show(ICharacter actor)
	{
		return
			$"[{Name}] {NumberOfFreeChecks.ToString("N0", actor)} x {TraitDefinition?.Name.ColourName() ?? "None".Colour(Telnet.Red)} @ {Difficulty.Describe().ColourValue()} - {Description}";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		return Description;
	}

	public override (bool Truth, string Error) CanSubmit()
	{
		if (NumberOfFreeChecks <= 0)
		{
			return (false, "There must be at least one free check.");
		}

		if (TraitDefinition == null)
		{
			return (false, "You must set a trait to be used in the free checks.");
		}

		return (true, string.Empty);
	}

	protected override string HelpText => $@"{base.HelpText}

	#3trait <which>#0 - sets the trait to get the free check
	#3checks <number>#0 - sets the number of free checks
	#3difficulty <difficulty>#0 - sets the difficulty of the free trait checks";

	public override bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "skill":
			case "trait":
			case "traitdefinition":
			case "trait definition":
			case "trait_definition":
			case "definition":
				return BuildingCommandTrait(actor, command);
			case "number":
			case "num":
			case "checks":
			case "numchecks":
			case "num checks":
			case "num_checks":
			case "times":
				return BuildingCommandChecks(actor, command);
			case "difficulty":
			case "diff":
				return BuildingCommandDifficulty(actor, command);
		}

		return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"), phase);
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a difficulty for the free checks.");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.PopSpeech(), out var difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty. See SHOW DIFFICULTY for a full list.");
			return false;
		}

		Difficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"The free trait checks for this action will now be at the {Difficulty.Describe().ColourValue()} difficulty.");
		return true;
	}

	private bool BuildingCommandChecks(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many free checks should occur when this action fires?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid number larger than zero for the number of free checks.");
			return false;
		}

		if (value > 100)
		{
			actor.OutputHandler.Send("You cannot have more than 100 free checks in one action.");
			return false;
		}

		NumberOfFreeChecks = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"There will now be {NumberOfFreeChecks.ToString("N0", actor).ColourValue()} checks made when this action is fired.");
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait do you to check when this action fires.");
			return false;
		}

		var trait = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(command.Last);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		TraitDefinition = trait;
		Changed = true;
		actor.OutputHandler.Send(
			$"This action will now give free checks against the {TraitDefinition.Name.TitleCase().ColourValue()} trait.");
		return true;
	}
}