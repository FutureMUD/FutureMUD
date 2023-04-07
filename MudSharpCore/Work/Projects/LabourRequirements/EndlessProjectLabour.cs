using System.Text;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Work.Projects.LabourRequirements;

public class EndlessProjectLabour : ProjectLabourBase
{
	public EndlessProjectLabour(Models.ProjectLabourRequirement labour, IFuturemud gameworld) : base(labour, gameworld)
	{
	}

	public EndlessProjectLabour(IFuturemud gameworld, IProjectPhase phase, string name) : base(gameworld, phase,
		"endless", name)
	{
	}

	public EndlessProjectLabour(ProjectLabourBase rhs, IProjectPhase newPhase) : base(rhs, newPhase, "endless")
	{
	}

	#region Overrides of ProjectLabourBase

	public override double HourlyProgress(ICharacter actor, bool previewOnly = false)
	{
		return previewOnly ? double.Epsilon : 0.0;
	}

	public override IProjectLabourRequirement Duplicate(IProjectPhase newPhase)
	{
		return new EndlessProjectLabour(this, newPhase);
	}

	public override double TotalProgressRequiredForDisplay => double.PositiveInfinity;

	public override double HoursRemaining(IActiveProject project)
	{
		return double.PositiveInfinity;
	}

	public override string Show(ICharacter actor)
	{
		if (RequiredTrait == null)
		{
			return $"Endless labour{(IsMandatoryForProjectCompletion ? " [mandatory]" : "")}";
		}

		return
			$"Endless labour of {RequiredTrait.Name.ColourValue()}(>={MinimumTraitValue.ToString("N2", actor).ColourValue()})@{TraitCheckDifficulty.Describe().ColourValue()}{(IsMandatoryForProjectCompletion ? " [mandatory]" : "")}";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		if (RequiredTrait == null)
		{
			return
				$"{$"[{(IsMandatoryForProjectCompletion ? "ReqLab" : "Lab")}]".Colour(Telnet.Yellow)} {Name.Colour(Telnet.Cyan)}";
		}

		return
			$"{$"[{(IsMandatoryForProjectCompletion ? "ReqLab" : "Lab")}]".Colour(Telnet.Yellow)} {Name.Colour(Telnet.Cyan)} ({RequiredTrait.Name.ColourValue()}(>={RequiredTrait.Decorator.Decorate(MinimumTraitValue)})@{TraitCheckDifficulty.Describe().ColourValue()})";
	}

	protected override bool BuildingCommandShow(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Endless Project Labour {Id.ToString("N0", actor).ColourValue()} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Phase {phase.PhaseNumber.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Mandatory: {IsMandatoryForProjectCompletion.ToColouredString()}");
		sb.AppendLine($"Maximum Workers: {MaximumSimultaneousWorkers.ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"IsQualifiedProg: {IsQualifiedProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Tested Trait: {RequiredTrait?.Name.Colour(Telnet.Cyan) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Trait Difficulty: {TraitCheckDifficulty.Describe().ColourValue()}");
		sb.AppendLine($"Minimum Trait Value: {MinimumTraitValue.ToString("N2", actor).ColourValue()}");
		foreach (var impact in LabourImpacts)
		{
			sb.Append($"\t{impact.Name.ColourName()}: ");
			sb.AppendLine(impact.ShowFull(actor));
		}

		actor.OutputHandler.Send(sb.ToString());
		return true;
	}

	#endregion
}