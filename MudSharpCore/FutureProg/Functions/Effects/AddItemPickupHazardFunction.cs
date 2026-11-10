#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class AddItemPickupHazardFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Effect; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new NullVariable(ReturnType);
		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item || item.Deleted || item.Destroyed) return StatementResult.Normal;
		// Saved spent markers prevent an onload proc from rearming an already expended item.
		if (item.Effects.OfType<ItemPickupHazardEffect>().FirstOrDefault() is { } existing) { Result = existing; return StatementResult.Normal; }
		var immunity = ParameterFunctions[3].Result?.GetObject as decimal? ?? -1;
		var minimum = ParameterFunctions[4].Result?.GetObject as decimal? ?? -1;
		if (immunity < 0 || immunity > long.MaxValue || immunity != decimal.Truncate(immunity) ||
			minimum < 1 || minimum > int.MaxValue || minimum != decimal.Truncate(minimum)) return StatementResult.Normal;
		try
		{
			var effect = new ItemPickupHazardEffect(item, ParameterFunctions[1].Result?.GetObject?.ToString() ?? "",
				ParameterFunctions[2].Result?.GetObject?.ToString() ?? "", (long)immunity, (int)minimum,
				ParameterFunctions[5].Result?.GetObject?.ToString() ?? "");
			item.AddEffect(effect); Result = effect;
		}
		catch (FormatException) { }
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"addpickuphazard", [ProgVariableTypes.Item, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Text],
		(parameters, _) => new AddItemPickupHazardFunction(parameters), ["item", "hpDice", "stunDice", "immunityProg", "minimumDispelGrade", "echo"],
		["The item.", "HP damage dice, or 0.", "Stun dice, or 0.", "Optional boolean(character,item) immunity prog ID; zero disables it.", "Minimum grade for intrinsic dispelling.", "Text sent to the person triggering the hazard."],
		"Adds a saved one-shot HP/stun pickup hazard without ownership exemption. Existing spent markers remain spent. Scripted loading does not trigger it.", "Effects", ProgVariableTypes.Effect));
}
