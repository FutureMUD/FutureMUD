#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;

namespace MudSharp.FutureProg.Functions.Magic;

internal sealed class HasPurgeablePoisonFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(ParameterFunctions[0].Result?.GetObject is ICharacter actor &&
			ParameterFunctions[1].Result?.GetObject is IMagicSpell spell &&
			spell.SpellEffects.OfType<RemovePoisonEffect>().Any(effect => effect.HasMatchingPoison(actor)));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"haspurgeablepoison", [ProgVariableTypes.Character, ProgVariableTypes.MagicSpell], (parameters, _) => new HasPurgeablePoisonFunction(parameters),
		["character", "spell"], ["The physical patient.", "The native cure spell whose explicit poison selection is inspected."],
		"Pure query for poison effects or active/latent doses selected by this spell's remove-poison effects. Does not cast, charge or cure.", "Magic", ProgVariableTypes.Boolean));
}
