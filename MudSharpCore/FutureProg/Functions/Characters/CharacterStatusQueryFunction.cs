#nullable enable

using MudSharp.Effects.Interfaces;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

/// <summary>Queries native status contracts, including starting statuses without a parent spell.</summary>
internal sealed class CharacterStatusQueryFunction(IList<IFunction> parameters, string status) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public static void RegisterFunctionCompiler()
	{
		foreach (var name in new[] { "hasblindnesseffect", "hassilenceeffect", "hasparalysiseffect", "haspausedaieffect" })
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(name, [ProgVariableTypes.Character],
				(pars, _) => new CharacterStatusQueryFunction(pars, name), ["character"], ["The embodied character to inspect"],
				"Pure query for the corresponding applicable native status contract, including non-spell and body effects. Does not change condition.", "Character", ProgVariableTypes.Boolean));
	}
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var present = actor?.Body is not null && (status switch
		{
			"hasblindnesseffect" => actor.Body.CombinedEffectsOfType<IBlindnessEffect>().Any(x => x.Applies()),
			"hassilenceeffect" => actor.Body.CombinedEffectsOfType<ISilencedEffect>().Any(x => x.Applies()),
			"hasparalysiseffect" => actor.State.HasFlag(CharacterState.Paralysed) || actor.Body.CombinedEffectsOfType<IForceParalysisEffect>().Any(x => x.Applies()),
			_ => actor.EffectsOfType<IPauseAIEffect>().Any()
		});
		Result = new BooleanVariable(present);
		return StatementResult.Normal;
	}
}
