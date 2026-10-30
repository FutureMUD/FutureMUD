#nullable enable

using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal sealed class CommandNameFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Text; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var input = ParameterFunctions[1].Result?.GetObject?.ToString() ?? "";
		Result = new TextVariable(actor?.CommandTree?.Commands.LocateCommand(actor, ref input)?.Name ?? "");
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"commandname", [ProgVariableTypes.Character, ProgVariableTypes.Text], (parameters, _) => new CommandNameFunction(parameters),
		["actor", "input"], ["The character whose command tree is used.", "Original command input."],
		"Resolves a native command, including its normal abbreviations, without executing it. Returns its registered name, or empty text if unresolved.", "Characters", ProgVariableTypes.Text));
}
