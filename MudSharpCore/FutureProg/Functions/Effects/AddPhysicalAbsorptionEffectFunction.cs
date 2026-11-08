#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class AddPhysicalAbsorptionEffectFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Effect; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var amount = (double)(decimal)(ParameterFunctions[1].Result?.GetObject ?? -1m);
		if (actor is null || !double.IsFinite(amount) || amount < 0)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		var existing = actor.Effects.OfType<PhysicalDamageAbsorptionEffect>().ToArray();
		if (amount == 0)
		{
			foreach (var effect in existing) actor.RemoveEffect(effect);
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		var configured = existing.FirstOrDefault(x => x.PhysicalAbsorption == amount);
		foreach (var effect in existing.Where(x => !ReferenceEquals(x, configured))) actor.RemoveEffect(effect);
		if (configured is null)
		{
			configured = new PhysicalDamageAbsorptionEffect(actor, amount);
			actor.AddEffect(configured);
		}
		Result = configured;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"addphysicalabsorptioneffect", [ProgVariableTypes.Character, ProgVariableTypes.Number],
		(parameters, _) => new AddPhysicalAbsorptionEffectFunction(parameters), ["target", "amount"],
		["The character.", "Flat nonnegative physical damage and stun absorption."],
		"Sets the character's permanent saved post-armour physical absorption. Repeated calls update one effect; zero removes it. Returns null for invalid inputs.",
		"Effects", ProgVariableTypes.Effect));
}
