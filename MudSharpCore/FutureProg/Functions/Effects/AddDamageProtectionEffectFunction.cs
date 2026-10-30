#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class AddDamageProtectionEffectFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Effect; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var multiplier = (double)(decimal)(ParameterFunctions[1].Result?.GetObject ?? -1m);
		var hits = (decimal)(ParameterFunctions[2].Result?.GetObject ?? -1m);
		if (actor is null || !double.IsFinite(multiplier) || multiplier is < 0 or > 1 || hits < 0 || hits > int.MaxValue || hits != decimal.Truncate(hits))
		{
			Result = new NullVariable(ReturnType); return StatementResult.Normal;
		}
		var effect = actor.Effects.OfType<DamageProtectionStatus>().FirstOrDefault(x => x.Multiplier == multiplier && x.ConfiguredHits == (int)hits);
		if (effect is null)
		{
			effect = new DamageProtectionStatus(actor, multiplier, (int)hits); actor.AddEffect(effect);
		}
		Result = effect; return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("adddamageprotectioneffect",
		[ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Number],
		(parameters, _) => new AddDamageProtectionEffectFunction(parameters), ["target", "multiplier", "hits"],
		["The character.", "Damage multiplier in [0,1]. Zero fully blocks.", "Whole qualifying hits; zero is unlimited."],
		"Adds saved post-armour protection. A qualifying hit is one incoming positive body-damage packet, including internal descendants. Previews and zero-damage packets do not spend hits. Returns the existing identical configuration without stacking/refreshing, or null for invalid inputs.",
		"Effects", ProgVariableTypes.Effect));
}
