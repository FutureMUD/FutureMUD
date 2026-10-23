#nullable enable
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Magic;
using MudSharp.Work.Agriculture;

namespace MudSharp.Commands.Helpers;

internal sealed record BuilderDiagnostic(bool IsError, string Message, string? InspectCommand = null);

internal static class BuilderPreflight
{
	internal static IReadOnlyList<BuilderDiagnostic> Validate(IFrameworkItem item, IFuturemud world)
	{
		var diagnostics = new List<BuilderDiagnostic>();
		void Error(string message, string? command = null) => diagnostics.Add(new(true, message, command));
		void Outputs(IEnumerable<AgricultureCommodityYield> outputs, string label, bool required)
		{
			var values = outputs.ToList();
			if (required && values.Count == 0) Error($"{label}: no outputs are configured.");
			for (var i = 0; i < values.Count; i++)
			{
				var output = values[i];
				if (!double.IsFinite(output.BaseWeight) || output.BaseWeight <= 0) Error($"{label} {i + 1}: weight must be positive and finite.");
				var material = world.Materials.GetByName(output.MaterialName);
				if (material is null) Error($"{label} {i + 1}: unknown material {output.MaterialName}.", "material list");
				if (!string.IsNullOrWhiteSpace(output.TagName) && !world.Tags.Any(x => x.Name.EqualTo(output.TagName)))
					Error($"{label} {i + 1}: unknown tag {output.TagName}.", "tag list");
			}
		}
		switch (item)
		{
			case IEditableRevisableItem revision:
				if (!revision.CanSubmit()) Error(revision.WhyCannotSubmit());
				diagnostics.Add(new(false, "Checks the family's existing submission rules only; does not submit, activate or simulate runtime use."));
				break;
			case IMagicSchool school:
				var seen = new HashSet<IMagicSchool>();
				for (var ancestor = school; ancestor is not null; ancestor = ancestor.ParentSchool)
				{
					if (seen.Add(ancestor)) continue;
					Error($"Parent cycle reaches school #{ancestor.Id} ({ancestor.Name}).", $"magic school show {ancestor.Id}");
					break;
				}
				break;
			case IAgricultureCropDefinition crop:
				if (crop.BaseGrowthDays <= 0 || crop.HarvestWindowDays <= 0 || (crop.IsPerennial && crop.HarvestCycleDays <= 0)) Error("Growth, harvest window and perennial cycle must be positive.");
				if (crop.MinimumMoisture > crop.MaximumMoisture || crop.MinimumTemperature > crop.MaximumTemperature) Error("Minimum moisture/temperature must not exceed its maximum.");
				Outputs(crop.YieldOutputs, "Harvest", true);
				Outputs(crop.SeedRequirements, "Seeds", false);
				diagnostics.Add(new(false, "Seed requirements are metadata; verify sowing project material requirements separately."));
				break;
			case IAgricultureWoodlandDefinition woodland:
				if (woodland.EstablishmentDays <= 0 || woodland.HarvestCycleDays <= 0) Error("Establishment and harvest cycle must be positive.");
				Outputs(woodland.YieldOutputs, "Harvest", true);
				break;
            case IAgricultureOperation operation when operation.OperationType == AgricultureOperationType.HarvestApiary:
                if (!double.IsFinite(operation.ApiaryYieldMultiplier) || operation.ApiaryYieldMultiplier <= 0 || operation.ApiaryYieldCost < 0)
                    Error("Apiary multiplier must be positive and finite; store cost cannot be negative.");
                Outputs(operation.ApiaryYieldOutputs, "Apiary harvest", true);
                break;
			default:
				diagnostics.Add(new(false, "No preflight provider is registered for this family; use its show and setting-specific checks."));
				break;
		}
		return diagnostics;
	}

	public static string Describe(IFrameworkItem item, IFuturemud world)
	{
		var diagnostics = Validate(item, world);
		return $"Validation of {BuilderRevisionDiff.Reference(item).ColourName()}: {diagnostics.Count(x => x.IsError)} errors, {diagnostics.Count(x => !x.IsError)} warnings.\n" +
		       string.Join("\n", diagnostics.Select(x => $"{(x.IsError ? "Error".ColourError() : "Warning".ColourName())}: {x.Message}" +
			       (x.InspectCommand is null ? "" : $" Inspect: {x.InspectCommand.ColourCommand()}"))) +
		       "\nRead-only preflight; no activation or FutureProg preview execution is performed.";
	}
}
