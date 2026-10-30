#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Effects;

/// <summary>Applies native administrator-only visibility to a perceivable.</summary>
internal sealed class AddAdminInvisEffectFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Effect; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		if (ParameterFunctions[0].Result?.GetObject is not IPerceivable owner)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		var effect = owner.Effects.FirstOrDefault(x => x is IAdminInvisEffect);
		if (effect is null)
		{
			effect = new AdminInvis(owner);
			owner.AddEffect(effect);
		}
		Result = effect;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new("addadmininviseffect", [ProgVariableTypes.Perceivable],
			(parameters, _) => new AddAdminInvisEffectFunction(parameters), ["target"],
			["The item or other perceivable to hide from ordinary characters."],
			"Adds the saved native admin-invisibility effect. Returns the existing effect without stacking, or null for a missing target. Ordinary invisibility detection does not reveal it.",
			"Effects", ProgVariableTypes.Effect));
	}
}
