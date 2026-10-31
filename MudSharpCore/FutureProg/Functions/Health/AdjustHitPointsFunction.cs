using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;

#nullable enable
namespace MudSharp.FutureProg.Functions.Health;

internal sealed class AdjustHitPointsFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation("adjusthitpoints",
			[ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Number], (parameters, _) => new AdjustHitPointsFunction(parameters),
			["character", "amount", "stun"], ["Living character with an HP strategy", "Nonpositive HP adjustment", "Nonnegative direct stun damage"],
			"Applies direct HP and stun harm atomically through the native health strategy. Positive healing cannot be combined with stun. Unsupported channels fail without partial damage.",
			"Health", ProgVariableTypes.Boolean));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation("adjusthitpoints",
			[ProgVariableTypes.Character, ProgVariableTypes.Number], (parameters, _) => new AdjustHitPointsFunction(parameters),
			["character", "amount"], ["The living character with a native HP strategy", "Positive heals; negative inflicts direct HP damage"],
			"Repairs native wound damage or inflicts direct HP damage without armour or magic. Preserves health ceilings and death rules; does not restore stun or resurrect. Returns false for unsupported health models or invalid targets.",
			"Health", ProgVariableTypes.Boolean));
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(ParameterFunctions[0].Result?.GetObject is ICharacter actor &&
			ParameterFunctions[1].Result?.GetObject is decimal amount && HitPointChanges.Apply(actor, (double)amount, ParameterFunctions.Count > 2 ? Convert.ToDouble(ParameterFunctions[2].Result?.GetObject) : 0.0));
		return StatementResult.Normal;
	}
}
