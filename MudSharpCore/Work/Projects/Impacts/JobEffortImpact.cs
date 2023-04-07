using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp.Work.Projects.Impacts;
#nullable enable
public class JobEffortImpact : BaseImpact, ILabourImpactActionAtTick
{
	/// <inheritdoc />
	public JobEffortImpact(ProjectLabourImpact impact, IFuturemud gameworld) : base(impact, gameworld)
	{
		var root = XElement.Parse(impact.Definition);
		EffortPerTickExpressionString = root.Element("Expression")!.Value;
		EffortPerTickExpression = new TraitExpression(EffortPerTickExpressionString, Gameworld);
	}

	/// <inheritdoc />
	public JobEffortImpact(JobEffortImpact rhs, IProjectLabourRequirement newLabour) : base(rhs, newLabour, "JobEffort")
	{
		EffortPerTickExpressionString = rhs.EffortPerTickExpressionString;
		EffortPerTickExpression = new TraitExpression(EffortPerTickExpressionString, Gameworld);
	}

	/// <inheritdoc />
	public JobEffortImpact(IProjectLabourRequirement requirement, string name) : base(requirement, "JobEffort", name)
	{
		EffortPerTickExpressionString = "variable / 10";
		EffortPerTickExpression = new TraitExpression(EffortPerTickExpressionString, Gameworld);
	}

	#region Overrides of BaseImpact

	/// <inheritdoc />
	protected override XElement SaveDefinition()
	{
		return new XElement("Impact",
			new XElement("Expression", new XCData(EffortPerTickExpressionString ?? "variable / 10"))
		);
	}

	/// <inheritdoc />
	public override ILabourImpact Duplicate(IProjectLabourRequirement requirement)
	{
		return new JobEffortImpact(this, requirement);
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append(base.Show(actor));
		sb.AppendLine($"Effort Formula: {EffortPerTickExpressionString.ColourCommand()}");
		return sb.ToString();
	}

	/// <inheritdoc />
	public override string ShowFull(ICharacter actor)
	{
		return $"{"[JobEffort]".Colour(Telnet.Magenta)} {EffortPerTickExpressionString.ColourCommand()}";
	}

	/// <inheritdoc />
	public override string ShowToPlayer(ICharacter actor)
	{
		return $"Contributing performance effort towards your job";
	}

	/// <inheritdoc />
	public void DoAction(ICharacter character, IActiveProject project, IProjectLabourRequirement requirement)
	{
		var job = character.ActiveJobs.FirstOrDefault(x => x.ActiveProject == project);
		if (job is null)
		{
			return;
		}

		job.CurrentPerformance += EffortPerTickExpression.Evaluate(character, requirement.RequiredTrait,
			TraitBonusContext.JobEffortCalculation);
	}

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command, IProjectLabourRequirement requirement)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "formula":
			case "expression":
			case "effort":
				return BuildingCommandExpression(actor, command, requirement);
		}

		return base.BuildingCommand(actor, command.GetUndo(), requirement);
	}

	private bool BuildingCommandExpression(ICharacter actor, StringStack command, IProjectLabourRequirement requirement)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which expression should this job effort impact use to determine contribution to job effort every 15 minutes?");
			return false;
		}

		var expr = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (expr.HasErrors())
		{
			actor.OutputHandler.Send(expr.Error);
			return false;
		}

		EffortPerTickExpressionString = command.SafeRemainingArgument;
		EffortPerTickExpression = expr;
		Changed = true;
		actor.OutputHandler.Send(
			$"This impact will now use the formula {EffortPerTickExpressionString.ColourCommand()} for job effort contribution.");
		return true;
	}

	#endregion

	public string EffortPerTickExpressionString { get; protected set; }
	public ITraitExpression EffortPerTickExpression { get; protected set; }
}