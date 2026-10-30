#nullable enable

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal sealed class GetExistingItemFunction(IList<IFunction> parameters, IFuturemud world) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Item; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var id = (decimal?)ParameterFunctions[0].Result?.GetObject ?? 0;
		Result = id > 0 && id <= long.MaxValue && decimal.Truncate(id) == id
			? world.TryGetItem((long)id, true) : null;
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"getexistingitem", [ProgVariableTypes.Number], (parameters, world) => new GetExistingItemFunction(parameters, world),
		["id"], ["The durable ID of an existing item, including one not currently loaded."],
		"Finds an existing item by ID, loading its persisted instance if necessary. Never creates from a prototype. Missing or invalid IDs return null.", "Items", ProgVariableTypes.Item));
}
