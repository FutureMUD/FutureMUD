#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class AddMovementTempoEffectFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
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
		var effect = actor.Effects.OfType<MovementTempoStatus>().FirstOrDefault(x => x.Multiplier == multiplier);
		if (effect is null)
		{
			effect = new MovementTempoStatus(actor, multiplier); actor.AddEffect(effect);
		}
		Result = effect;
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("addmovementtempoeffect",
		[ProgVariableTypes.Character, ProgVariableTypes.Number], (parameters, _) => new AddMovementTempoEffectFunction(parameters),
		["target", "multiplier"], ["The character.", "Movement time multiplier within 0.1..10."],
		"Adds a saved movement-delay multiplier using native movement rules. Returns the effect, the existing same multiplier without stacking, or null for invalid inputs. Different multipliers combine normally. Does not alter attributes, combat waits or resource costs.",
		"Effects", ProgVariableTypes.Effect));
}
