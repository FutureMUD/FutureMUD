#nullable enable

using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal sealed class CanSeeFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(ParameterFunctions[0].Result?.GetObject is IPerceiver observer &&
			ParameterFunctions[1].Result?.GetObject is IPerceivable target && observer.CanSee(target));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"cansee", [ProgVariableTypes.Perceiver, ProgVariableTypes.Perceivable], (parameters, _) => new CanSeeFunction(parameters),
		["observer", "target"], ["The character or perceiving item.", "The visible target to test."],
		"Tests ordinary native sight, including visibility, blindness, light and consciousness. Null inputs return false.",
		"Perception", ProgVariableTypes.Boolean));
}
