#nullable enable

using MudSharp.Community;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Clan;

/// <summary>Evaluates data-authored standing rules for generated template progs.</summary>
internal sealed class ClanTemplateEligibilityError : BuiltInFunction
{
	private readonly IFuturemud _world;
	private ClanTemplateEligibilityError(IList<IFunction> parameters, IFuturemud world) : base(parameters) { _world = world; }
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Text; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new TextVariable(ClanTemplateEligibility.Error(_world,
			ParameterFunctions[0].Result?.GetObject as ICharacter,
			Convert.ToInt64(ParameterFunctions[1].Result?.GetObject),
			Convert.ToInt64(ParameterFunctions[2].Result?.GetObject),
			ParameterFunctions[3].Result?.GetObject as string ?? string.Empty));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
		"clantemplateeligibilityerror", [ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Text],
		(pars, world) => new ClanTemplateEligibilityError(pars, world), ["character", "clan", "office", "purpose"],
		["Living primary character.", "Configured template profile clan ID.", "Mapped office ID.", "nominate, votes, recall, preflight, or a generated address:<rule-id> condition."],
		"Returns an empty string when the configured template profile permits the action, otherwise the first failure. Does not install holders or enforce raw appointment commands.",
		"Clans", ProgVariableTypes.Text));
}
