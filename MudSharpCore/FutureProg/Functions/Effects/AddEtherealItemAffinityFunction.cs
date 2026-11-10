#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class AddEtherealItemAffinityFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Effect; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new NullVariable(ReturnType);
		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item || item.Deleted || item.Destroyed ||
			ParameterFunctions[1].Result?.GetObject is not decimal plane || plane <= 0 || plane > long.MaxValue ||
			plane != decimal.Truncate(plane) || item.Gameworld.Planes.Get((long)plane) is null ||
			ParameterFunctions[2].Result?.GetObject is not bool ready || ParameterFunctions[3].Result?.GetObject is not bool visible)
			return StatementResult.Normal;
		// Preserve a saved, dispelled affinity marker on subsequent loads.
		var effect = item.Effects.OfType<EtherealItemAffinityEffect>().FirstOrDefault(x => x.PlaneId == (long)plane);
		if (effect is null)
		{
			effect = new EtherealItemAffinityEffect(item, (long)plane, ready, visible);
			item.AddEffect(effect);
		}
		Result = effect;
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"addetherealitemaffinity", [ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Boolean, ProgVariableTypes.Boolean],
		(parameters, _) => new AddEtherealItemAffinityFunction(parameters), ["item", "plane", "ready", "visible"],
		["The item.", "The plane identity.", "Whether it is ready for configured inventory retention.", "Whether it requires ethereal detection or presence on that plane to see."],
		"Adds saved ethereal item affinity. Existing markers, including dispelled ones, are preserved rather than rearmed on load.", "Effects", ProgVariableTypes.Effect));
}
