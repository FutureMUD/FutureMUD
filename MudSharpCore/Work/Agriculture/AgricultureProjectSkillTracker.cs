using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Work.Projects;
using MudSharp.Work.Projects.Actions;
using MudSharp.Work.Projects.LabourRequirements;

namespace MudSharp.Work.Agriculture;

public static class AgricultureProjectSkillTracker
{
	private const string OutcomeElementName = "FarmingOutcome";

	public static void RecordLabourTick(IActiveProject project, ICharacter character, IProjectLabourRequirement labour,
		double hours, double effectiveProgress)
	{
		if (project.Id <= 0 || character == null || labour == null || hours <= 0.0 ||
		    project.CurrentPhase.CompletionActions.All(x => x is not AgricultureOperationAction))
		{
			return;
		}

		var trait = ResolveFarmingTrait(project, labour);
		if (trait == null)
		{
			return;
		}

		var skill = character.TraitValue(trait, TraitBonusContext.ProjectLabourQualification);
		using (new FMDB())
		{
			var context = FMDB.Context.AgricultureProjectContexts.Find(project.Id);
			if (context == null)
			{
				return;
			}

			var root = AgricultureXmlExtensions.RootOrDefault(context.Definition, "Context");
			var outcome = root.Element(OutcomeElementName);
			if (outcome == null)
			{
				outcome = new XElement(OutcomeElementName);
				root.Add(outcome);
			}

			outcome.SetAttributeValue("trait", trait.Id);
			AddAttribute(outcome, "hours", hours);
			AddAttribute(outcome, "weightedSkill", skill * hours);
			AddAttribute(outcome, "effectiveProgress", Math.Max(0.0, effectiveProgress));
			if (labour is SupervisionProjectLabour)
			{
				AddAttribute(outcome, "supervisorHours", hours);
				AddAttribute(outcome, "weightedSupervisorSkill", skill * hours);
			}

			context.Definition = root.ToString();
			FMDB.Context.SaveChanges();
		}
	}

	public static AgricultureWorkOutcome OutcomeFor(string definition)
	{
		var root = AgricultureXmlExtensions.RootOrDefault(definition, "Context");
		var outcome = root.Element(OutcomeElementName);
		if (outcome == null)
		{
			return AgricultureWorkOutcome.Neutral;
		}

		return AgricultureWorkOutcome.FromSkill(
			AttributeDouble(outcome, "weightedSkill"),
			AttributeDouble(outcome, "hours"),
			AttributeDouble(outcome, "weightedSupervisorSkill"),
			AttributeDouble(outcome, "supervisorHours"));
	}

	private static ITraitDefinition ResolveFarmingTrait(IActiveProject project, IProjectLabourRequirement labour)
	{
		return project.ProjectDefinition.Gameworld.Traits.GetByName("Farming") ?? labour.RequiredTrait;
	}

	private static void AddAttribute(XElement element, string attribute, double value)
	{
		element.SetAttributeValue(attribute, (AttributeDouble(element, attribute) + value).ToString(CultureInfo.InvariantCulture));
	}

	private static double AttributeDouble(XElement element, string attribute)
	{
		return double.TryParse((string)element.Attribute(attribute), NumberStyles.Float, CultureInfo.InvariantCulture,
			out var value)
			? value
			: 0.0;
	}
}
