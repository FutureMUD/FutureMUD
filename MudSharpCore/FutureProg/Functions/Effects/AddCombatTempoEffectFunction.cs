#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class AddCombatTempoEffectFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Effect; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var multiplier = (double)(decimal)(ParameterFunctions[1].Result?.GetObject ?? 0m);
		if (actor is null || !double.IsFinite(multiplier) || multiplier is < 0.1 or > 10)
		{
			Result = new NullVariable(ReturnType); return StatementResult.Normal;
		}
		var effect = actor.Effects.OfType<CombatTempoStatus>().FirstOrDefault(x => x.Multiplier == multiplier);
		if (effect is null)
		{
			effect = new CombatTempoStatus(actor, multiplier); actor.AddEffect(effect);
		}
		Result = effect;
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("addcombattempoeffect",
		[ProgVariableTypes.Character, ProgVariableTypes.Number], (parameters, _) => new AddCombatTempoEffectFunction(parameters),
		["target", "multiplier"], ["The character.", "Combat recovery time multiplier within 0.1..10."],
		"Adds a saved combat recovery multiplier using native combat delay rules. Returns the effect, the existing same multiplier without stacking, or null for invalid inputs. Different multipliers combine normally. Does not alter attributes, damage or movement delays.",
		"Effects", ProgVariableTypes.Effect));
}
