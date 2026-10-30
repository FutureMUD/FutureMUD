#nullable enable

using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.GameItem;

internal sealed class LoadedItemsFunction(IList<IFunction> parameters, IFuturemud gameworld) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Item | ProgVariableTypes.Collection;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new CollectionVariable(gameworld.Items.ToList(), ProgVariableTypes.Item);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"loadeditems", [], (parameters, world) => new LoadedItemsFunction(parameters, world), [], [],
		"Returns a snapshot of items currently loaded in the game, including carried items. It does not load offline inventories from the database.",
		"Items", ProgVariableTypes.Item | ProgVariableTypes.Collection));
}
