#nullable enable

using MudSharp.Community;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Clan;

internal sealed class ClanTemplateAction : BuiltInFunction
{
	private ClanTemplateAction(IList<IFunction> parameters) : base(parameters) { }
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Text; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new TextVariable(ClanTemplateProvisioning.Execute(
			ParameterFunctions[0].Result?.GetObject as ICharacter,
			ParameterFunctions[1].Result?.GetObject as IClan,
			ParameterFunctions[2].Result?.GetObject as string ?? string.Empty,
			ParameterFunctions[3].Result?.GetObject as string ?? string.Empty));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
		"clantemplateaction", [ProgVariableTypes.Character, ProgVariableTypes.Clan, ProgVariableTypes.Text, ProgVariableTypes.Text],
		(pars, _) => new ClanTemplateAction(pars), ["administrator", "clan", "action", "arguments"],
		["An administrator; non-administrators are rejected.", "A staged copy with an authored template profile.",
		 "status, bind, variant, activate, preflight or export.", "Quoted operation arguments; empty text for status, activate and export."],
		"Runs an admin-only operation using the selected clan's declarative template profile. Does not recognise particular template names or setting policies.",
		"Clans", ProgVariableTypes.Text));
}
