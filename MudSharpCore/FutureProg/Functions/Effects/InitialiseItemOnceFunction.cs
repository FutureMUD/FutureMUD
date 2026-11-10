#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class InitialiseItemOnceFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(false);
		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item || item.Deleted || item.Destroyed ||
			ParameterFunctions[1].Result?.GetObject is not string key || string.IsNullOrWhiteSpace(key)) return StatementResult.Normal;
		var marker = item.Effects.OfType<ItemInitialisation>().FirstOrDefault();
		if (marker is null)
		{
			marker = new ItemInitialisation(item);
			item.AddEffect(marker);
		}
		Result = new BooleanVariable(marker.TryInitialise(key));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"initialiseitemonce", [ProgVariableTypes.Item, ProgVariableTypes.Text], (parameters, _) => new InitialiseItemOnceFunction(parameters),
		["item", "key"], ["The item.", "A stable content initialisation key."],
		"Returns true only the first time this key is initialised on the item. The saved marker survives native copying and splitting; use to guard one-time onload setup.", "Effects", ProgVariableTypes.Boolean));
}
