#nullable enable

using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

/// <summary>Pure native identity query; posture eligibility belongs to the calling prog.</summary>
internal sealed class PositionIdFunction : BuiltInFunction
{
	public PositionIdFunction(IList<IFunction> parameters) : base(parameters) { }

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var character = ParameterFunctions[0].Result?.GetObject as ICharacter;
		Result = new NumberVariable(character?.PositionState?.Id ?? 0);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(
		new FunctionCompilerInformation("positionid", [ProgVariableTypes.Character],
			(parameters, _) => new PositionIdFunction(parameters), ["character"],
			["The character whose current native posture identity is queried"],
			"Returns the character's current native position-state ID, or 0 for null/unavailable characters or positions. IDs are identities, not an ordered height or casting-position scale. This read does not change position, invoke eligibility progs, inspect combat, or decide whether an action is allowed.",
			"Character", ProgVariableTypes.Number));
}
