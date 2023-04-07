using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class DeleteItemFunction : BuiltInFunction
{
	private DeleteItemFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result is not IGameItem itemFunction)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		itemFunction.Delete();
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"deleteitem",
			new[] { FutureProgVariableTypes.Item },
			(pars, gameworld) => new DeleteItemFunction(pars),
			new List<string> { "item" },
			new List<string> { "The item that you want to delete" },
			"Permanently deletes the specified item. Warning: this is unrecoverable. Returns true if the delete is successful.",
			"Items",
			FutureProgVariableTypes.Boolean
		));
	}
}