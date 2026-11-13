#nullable enable

using MudSharp.Construction;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Performances;

internal sealed class OpenStageLinkFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Number; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var source = ParameterFunctions[0].Result?.GetObject as IRoom;
		var audience = ParameterFunctions[1].Result?.GetObject as IRoom;
		var label = ParameterFunctions[2].Result?.GetObject?.ToString();
		Result = new NumberVariable(source is null || audience is null || label is null
			? 0L
			: RPG.Performances.StageLinks.Open(source, audience, label));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
		"openstagelink", new[] { ProgVariableTypes.Location, ProgVariableTypes.Location, ProgVariableTypes.Text },
		(pars, _) => new OpenStageLinkFunction(pars), new List<string> { "source", "audience", "label" },
		new List<string> { "The ordinary source room", "The ordinary audience room", "The displayed stage label" },
		"Opens a stage observation link and returns its handle ID, or zero when the rooms are not supported.",
		"Performances", ProgVariableTypes.Number));
}

internal sealed class CloseStageLinkFunction(IList<IFunction> parameters, IFuturemud gameworld) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var id = ParameterFunctions[0].Result?.GetObject;
		Result = new BooleanVariable(id is decimal handle && handle is > 0 and <= long.MaxValue &&
			handle == decimal.Truncate(handle) && RPG.Performances.StageLinks.Close(gameworld, (long)handle));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
		"closestagelink", new[] { ProgVariableTypes.Number }, (pars, world) => new CloseStageLinkFunction(pars, world),
		new List<string> { "handle" }, new List<string> { "The stage-link handle returned by openstagelink" },
		"Closes the specified stage-link handle and returns whether it was live.", "Performances", ProgVariableTypes.Boolean));
}
