#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.Health;

namespace MudSharp.FutureProg.Functions.Health;

internal sealed class PurgeDrugDosageFunction(IList<IFunction> parameters, bool query) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		if (ParameterFunctions[0].Result?.GetObject is not ICharacter actor || actor.Body is null ||
			ParameterFunctions[1].Result?.GetObject is not IDrug drug)
		{ Result = new BooleanVariable(false); return StatementResult.Normal; }
		var family = ParameterFunctions[2].Result?.GetObject?.ToString();
		if (string.IsNullOrEmpty(family)) family = null;
		try
		{
			var aftermath = ParameterFunctions.Count > 3 && (bool?)ParameterFunctions[3].Result?.GetObject == true;
			Result = new BooleanVariable(query ? DrugPurgeRules.HasMatching(actor, drug, family, aftermath) : DrugPurgeRules.Purge(actor, drug, family, aftermath));
			return StatementResult.Normal;
		}
		catch (InvalidOperationException error) { ErrorMessage = error.Message; return StatementResult.Error; }
	}
	public static void RegisterFunctionCompiler()
	{
		foreach (var (name, query) in new[] { ("hasdrugdosage", true), ("purgedrugdosages", false) })
		foreach (var aftermath in new[] { false, true })
			FutureProg.RegisterBuiltInFunctionCompiler(new(name,
				aftermath ? [ProgVariableTypes.Character, ProgVariableTypes.Drug, ProgVariableTypes.Text, ProgVariableTypes.Boolean] : [ProgVariableTypes.Character, ProgVariableTypes.Drug, ProgVariableTypes.Text],
				(parameters, _) => new PurgeDrugDosageFunction(parameters, query), aftermath ? ["character", "drug", "family", "includeAftereffects"] : ["character", "drug", "family"],
				aftermath ? ["The physical recipient.", "Selected native drug.", "Exact origin family or empty text.", "Also select independently timed consequences belonging to this drug/family."] : ["The physical recipient.", "One explicitly selected native drug.", "Exact spell-origin family, or empty text for all origins of this drug."],
				query ? "Pure query for matching active/latent doses or magical poison effects. Does not classify other drugs or alter anything." :
				"Removes only the selected drug and optional exact spell-origin family, including active/latent doses and linked poison effects. Other drugs/families remain untouched. Returns whether anything matched.", "Health", ProgVariableTypes.Boolean));
	}
}
