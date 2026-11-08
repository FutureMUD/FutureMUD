#nullable enable

using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class AddItemPresentationFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Effect; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new NullVariable(ReturnType);
		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item || item.Deleted) return StatementResult.Normal;
		var mode = ParameterFunctions[1].Result?.GetObject?.ToString()?.ToLowerInvariant();
		Effect? effect = mode switch
		{
			"glow" => item.Effects.OfType<ItemPresentation>().FirstOrDefault(x => !x.Humming) ?? new ItemPresentation(item, false),
			"hum" => item.Effects.OfType<ItemPresentation>().FirstOrDefault(x => x.Humming) ?? new ItemPresentation(item, true),
			"magic" => item.Effects.OfType<ItemMagicAura>().FirstOrDefault() ?? new ItemMagicAura(item),
			_ => null
		};
		if (effect is null) return StatementResult.Normal;
		if (!item.Effects.Contains(effect)) item.AddEffect(effect);
		Result = effect; return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"applyitempresentationeffect", [ProgVariableTypes.Item, ProgVariableTypes.Text],
		(parameters, _) => new AddItemPresentationFunction(parameters), ["item", "mode"],
		["The item.", "Glow (descriptive only), hum (local hearing), or magic (detectable classification)."],
		"Adds a saved, non-stacking intrinsic item presentation effect. Returns null for invalid inputs.", "Effects", ProgVariableTypes.Effect));
}

internal sealed class AddDetectionVisibilityFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Effect; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new NullVariable(ReturnType);
		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item || item.Deleted ||
			!Enum.TryParse<PerceptionTypes>(ParameterFunctions[1].Result?.GetObject?.ToString(), true, out var senses) ||
			senses == PerceptionTypes.None || (senses & ~(PerceptionTypes.AllSense | PerceptionTypes.AllVisual)) != 0) return StatementResult.Normal;
		var rawPlane = ParameterFunctions[2].Result?.GetObject as decimal? ?? -1;
		if (rawPlane < 0 || rawPlane > long.MaxValue || decimal.Truncate(rawPlane) != rawPlane) return StatementResult.Normal;
		var plane = (long)rawPlane;
		if (plane != 0 && item.Gameworld.Planes.Get(plane) is null) return StatementResult.Normal;
		var effect = item.Effects.OfType<DetectionVisibility>().FirstOrDefault(x => x.Senses == senses && x.VisiblePlane == plane)
			?? new DetectionVisibility(item, senses, plane);
		if (!item.Effects.Contains(effect)) item.AddEffect(effect);
		Result = effect; return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"adddetectionvisibilityeffect", [ProgVariableTypes.Item, ProgVariableTypes.Text, ProgVariableTypes.Number],
		(parameters, _) => new AddDetectionVisibilityFunction(parameters), ["item", "senses", "plane"],
		["The item.", "Any one of these native detection senses reveals the item.", "An existing plane ID whose occupants can also see it, or 0."],
		"Adds a saved visual detection requirement without changing physical interaction. Independent requirements stack.", "Effects", ProgVariableTypes.Effect));
}

internal sealed class ClearItemMagicAuraFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var item = ParameterFunctions[0].Result?.GetObject as IGameItem;
		if (item is not null) foreach (var effect in item.Effects.OfType<ItemMagicAura>().ToArray()) item.RemoveEffect(effect);
		Result = new BooleanVariable(item is not null); return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"clearitemmagicaura", [ProgVariableTypes.Item], (parameters, _) => new ClearItemMagicAuraFunction(parameters),
		["item"], ["The item."], "Removes intrinsic magic classification without dispelling active spells.", "Effects", ProgVariableTypes.Boolean));
}

internal sealed class RetireCarriedItemsFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var valid = ParameterFunctions[0].Result?.GetObject is MudSharp.Character.ICharacter;
		if (!Enum.TryParse<MudSharp.GameItems.Interfaces.ItemRetirementTrigger>(ParameterFunctions[1].Result?.GetObject?.ToString(), true, out var trigger) ||
		    trigger is not (MudSharp.GameItems.Interfaces.ItemRetirementTrigger.TransientBodyDeath or MudSharp.GameItems.Interfaces.ItemRetirementTrigger.HostBodyChange)) valid = false;
		if (valid) ItemRetirement.RetireCarriedItems((MudSharp.Character.ICharacter)ParameterFunctions[0].Result.GetObject, trigger);
		Result = new BooleanVariable(valid); return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"retirecarrieditems", [ProgVariableTypes.Character, ProgVariableTypes.Text], (parameters, _) => new RetireCarriedItemsFunction(parameters),
		["character", "trigger"], ["The character whose carried roots are being disposed of.", "TransientBodyDeath or HostBodyChange."],
		"Retires only items explicitly configured for this disposal event, excluding worn and wielded equipment. Does not run on ordinary death or body changes automatically.", "Items", ProgVariableTypes.Boolean));
}
