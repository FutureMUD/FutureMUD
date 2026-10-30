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
			[ProgVariableTypes.Character, ProgVariableTypes.Number], (parameters, _) => new AdjustHitPointsFunction(parameters),
			["character", "amount"], ["The living character with a native HP strategy", "Positive heals; negative inflicts direct HP damage"],
			"Repairs native wound damage or inflicts direct HP damage without armour or magic. Preserves health ceilings and death rules; does not restore stun or resurrect. Returns false for unsupported health models or invalid targets.",
			"Health", ProgVariableTypes.Boolean));
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(ParameterFunctions[0].Result?.GetObject is ICharacter actor &&
			ParameterFunctions[1].Result?.GetObject is decimal amount && HitPointChanges.Apply(actor, (double)amount));
		return StatementResult.Normal;
	}
}
