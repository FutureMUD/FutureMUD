#nullable enable

namespace MudSharp.FutureProg.Functions.Characters;

internal sealed class TargetCharacterFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Character; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var text = ParameterFunctions[1].Result?.GetObject?.ToString();
		Result = actor is not null && !string.IsNullOrWhiteSpace(text) ? actor.TargetActor(text) : null;
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"targetcharacter", [ProgVariableTypes.Character, ProgVariableTypes.Text], (parameters, _) => new TargetCharacterFunction(parameters),
		["actor", "keywords"], ["The character doing the targeting.", "Normal local character keywords, including numeric disambiguation."],
		"Resolves a visible local character using normal native targeting. Missing or invisible targets return null.", "Perception", ProgVariableTypes.Character));
}
