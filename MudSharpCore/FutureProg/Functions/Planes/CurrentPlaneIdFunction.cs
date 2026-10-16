#nullable enable
using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.Planes;

namespace MudSharp.FutureProg.Functions.Planes;

/// <summary>The native selected plane identity, including default-plane and display-order precedence.</summary>
internal sealed class CurrentPlaneIdFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(
		new FunctionCompilerInformation("currentplaneid", new[] { ProgVariableTypes.Perceivable },
			(parameters, _) => new CurrentPlaneIdFunction(parameters), new[] { "perceivable" },
			new[] { "The perceivable whose current native plane identity is required" },
			"Returns the native CurrentPlane identity, or zero when no plane is available. Uses default-plane precedence and native display ordering.",
			"Planes", ProgVariableTypes.Number));
	private CurrentPlaneIdFunction(IList<IFunction> parameters) : base(parameters) { }
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Number; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var perceivable = ParameterFunctions[0].Result?.GetObject as IPerceivable;
		Result = new NumberVariable(perceivable?.CurrentPlane()?.Id ?? 0L);
		return StatementResult.Normal;
	}
}
