#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.Planes;

namespace MudSharp.FutureProg.Functions.Characters;

internal sealed class IsIncorporealFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(ParameterFunctions[0].Result?.GetObject is IPerceivable target && target.SuspendsPhysicalContact());
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"isincorporeal", [ProgVariableTypes.Perceivable], (parameters, _) => new IsIncorporealFunction(parameters),
		["target"], ["The entity whose native planar presence suspends physical contact."],
		"Returns whether native planar presence suspends physical contact. Null returns false.", "Planes", ProgVariableTypes.Boolean));
}
