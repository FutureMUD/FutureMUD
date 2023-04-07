using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Work.Projects.LabourRequirements;

public class SimpleProjectLabour : ProjectLabourBase
{
	public SimpleProjectLabour(ProjectLabourRequirement labour, IFuturemud gameworld) : base(labour, gameworld)
	{
	}

	public SimpleProjectLabour(IFuturemud gameworld, IProjectPhase phase, string name) : base(gameworld, phase,
		"simple", name)
	{
	}

	protected SimpleProjectLabour(SimpleProjectLabour rhs, IProjectPhase newPhase) : base(rhs, newPhase, "simple")
	{
	}

	public override IProjectLabourRequirement Duplicate(IProjectPhase newPhase)
	{
		return new SimpleProjectLabour(this, newPhase);
	}

	public override double HoursRemaining(IActiveProject project)
	{
		var totalRemaining = TotalProgressRequired - project.LabourProgress[this];
		var active = project.ActiveLabour.Where(x => x.Labour == this).ToList();
		if (!active.Any())
		{
			return totalRemaining;
		}

		var hourlyProgress = active.Sum(x => HourlyProgress(x.Character, true));
		if (hourlyProgress <= 0.0)
		{
			return totalRemaining;
		}

		return totalRemaining / hourlyProgress;
	}

	public override string Show(ICharacter actor)
	{
		if (RequiredTrait == null)
		{
			return
				$"{TotalProgressRequired.ToString("N2", actor)} man-hours of labour{(IsMandatoryForProjectCompletion ? " [mandatory]" : "")}";
		}

		return
			$"{TotalProgressRequired.ToString("N2", actor)} man-hours of {RequiredTrait.Name.ColourValue()}(>={MinimumTraitValue.ToString("N2", actor).ColourValue()})@{TraitCheckDifficulty.Describe().ColourValue()}{(IsMandatoryForProjectCompletion ? " [mandatory]" : "")}";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		if (RequiredTrait == null)
		{
			return
				$"{$"[{(IsMandatoryForProjectCompletion ? "ReqLab" : "Lab")} {TotalProgressRequired.ToString("N2", actor)}hrs]".Colour(Telnet.Yellow)} {Name.Colour(Telnet.Cyan)}";
		}

		return
			$"{$"[{(IsMandatoryForProjectCompletion ? "ReqLab" : "Lab")} {TotalProgressRequired.ToString("N2", actor)}hrs]".Colour(Telnet.Yellow)} {Name.Colour(Telnet.Cyan)} ({RequiredTrait.Name.ColourValue()}(>={RequiredTrait.Decorator.Decorate(MinimumTraitValue)})@{TraitCheckDifficulty.Describe().ColourValue()})";
	}

	protected override bool BuildingCommandShow(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Simple Project Labour {Id.ToString("N0", actor).ColourValue()} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Phase {phase.PhaseNumber.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Mandatory: {IsMandatoryForProjectCompletion.ToColouredString()}");
		sb.AppendLine($"Progress Required: {TotalProgressRequired.ToString("N2", actor).ColourValue()} man-hours");
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
}