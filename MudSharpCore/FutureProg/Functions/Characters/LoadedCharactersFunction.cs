#nullable enable

using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal sealed class LoadedCharactersFunction(IList<IFunction> parameters, IFuturemud gameworld) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Character | ProgVariableTypes.Collection;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new CollectionVariable(gameworld.Actors.ToList(), ProgVariableTypes.Character);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"loadedcharacters", [], (parameters, world) => new LoadedCharactersFunction(parameters, world), [], [],
		"Returns a snapshot of characters currently loaded in the game, including NPCs. It does not load offline characters from the database.",
		"Characters", ProgVariableTypes.Character | ProgVariableTypes.Collection));
}
