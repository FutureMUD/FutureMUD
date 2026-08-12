using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Work.Projects;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class ProjectLabourContributionMerit : CharacterMeritBase, IProjectLabourContributionMerit
{
	protected ProjectLabourContributionMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		ContributionProg = gameworld.FutureProgs.Get(long.Parse(definition.Attribute("prog")?.Value ?? "0")) ??
			Gameworld.AlwaysOneProg;
	}

	protected ProjectLabourContributionMerit(IFuturemud gameworld, string name) : base(gameworld, name,
		"Project Labour Contribution", "@ contribute|contributes an altered amount of progress to projects")
	{
		ContributionProg = Gameworld.AlwaysOneProg;
		DoDatabaseInsert();
	}

	protected ProjectLabourContributionMerit()
	{
	}

	public IFutureProg ContributionProg { get; private set; } = null!;

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("prog", ContributionProg.Id));
		return root;
	}

	public double ProjectLabourContributionMultiplier(ICharacter character, IActiveProject project)
	{
		return ContributionProg.ExecuteDouble(1.0, character, project);
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PopForSwitch().EqualTo("prog"))
		{
			var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Number,
				[[ProgVariableTypes.Character, ProgVariableTypes.Project]]).LookupProg();
			if (prog is null)
			{
				return false;
			}

			ContributionProg = prog;
			Changed = true;
			actor.OutputHandler.Send($"This merit will use {prog.MXPClickableFunctionNameWithId()} to multiply project labour contribution.");
			return true;
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3prog <prog>#0 - sets the number(character, project) prog that multiplies labour progress";

	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Contribution Prog: {ContributionProg.MXPClickableFunctionNameWithId()}");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Project Labour Contribution",
			(merit, gameworld) => new ProjectLabourContributionMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Project Labour Contribution",
			(gameworld, name) => new ProjectLabourContributionMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Project Labour Contribution", "Multiplies project labour progress with a FutureProg",
			new ProjectLabourContributionMerit().HelpText);
	}
}
