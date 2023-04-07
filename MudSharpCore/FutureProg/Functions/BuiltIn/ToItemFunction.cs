using System.Collections.Generic;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToItemFunction : BuiltInFunction
{
	protected ToItemFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Item;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].Result as IGameItem;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toitem",
			new[] { FutureProgVariableTypes.Perceiver },
			(pars, gameworld) => new ToItemFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toitem",
			new[] { FutureProgVariableTypes.Perceivable },
			(pars, gameworld) => new ToItemFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toitem",
			new[] { FutureProgVariableTypes.CollectionItem },
			(pars, gameworld) => new ToItemFunction(pars)
		));
	}
}