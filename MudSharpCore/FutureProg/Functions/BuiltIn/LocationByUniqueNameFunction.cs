using MudSharp.Construction;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class LocationByUniqueNameFunction(IList<IFunction> parameters, IFuturemud gameworld)
	: BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Location; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = gameworld.Cells.FindByUniqueName(ParameterFunctions[0].Result?.GetObject?.ToString());
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"locationbyuniquename", [ProgVariableTypes.Text],
			(pars, world) => new LocationByUniqueNameFunction(pars, world),
			["uniquename"], ["The exact global cell unique name (case-insensitive)"],
			"Returns the room with that exact unique name, or null. Does not fall back to IDs, display names, here or @N. Renames retain no aliases.",
			"Lookup", ProgVariableTypes.Location));
	}
}
