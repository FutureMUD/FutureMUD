#nullable enable

using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class AddPublicTravelAnchorFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Effect; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new NullVariable(ReturnType);
		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item || item.Deleted || item.Destroyed ||
			!Enum.TryParse<PerceptionTypes>(ParameterFunctions[1].Result?.GetObject?.ToString(), true, out var sense) ||
			!Enum.IsDefined(sense) || (sense & ~(PerceptionTypes.AllSense | PerceptionTypes.AllVisual)) != 0) return StatementResult.Normal;

		var existing = item.Effects.OfType<ILiveTravelAnchorEffect>().ToArray();
		if (existing.Length > 0)
		{
			if (existing.Length == 1 && existing[0] is PublicTravelAnchor anchor && anchor.RequiredPerception == sense)
				Result = anchor;
			return StatementResult.Normal;
		}
		var effect = new PublicTravelAnchor(item, sense);
		item.AddEffect(effect);
		Result = effect;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"addpublictravelanchoreffect", [ProgVariableTypes.Item, ProgVariableTypes.Text],
		(parameters, _) => new AddPublicTravelAnchorFunction(parameters), ["item", "sense"],
		["The world rune item.", "Native perception type, e.g. SenseMagical, SensePsychic, or None for unrestricted perception."],
		"Adds a saved public live-room travel anchor. The required sense both reveals and admits the rune; it must be directly in a room to be used. Returns the existing matching effect without stacking, or null for invalid input or a conflicting anchor. Does not grant a travel spell or override its restrictions.",
		"Effects", ProgVariableTypes.Effect));
}
