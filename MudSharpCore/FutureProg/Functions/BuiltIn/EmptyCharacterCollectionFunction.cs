#nullable enable

using MudSharp.FutureProg.Variables;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class EmptyCharacterCollectionFunction : BuiltInFunction
{
	public EmptyCharacterCollectionFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Character | ProgVariableTypes.Collection;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = new CollectionVariable(new List<IProgVariable>(), ProgVariableTypes.Character);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"emptycharacters",
				[],
				(pars, _) => new EmptyCharacterCollectionFunction(pars),
				[],
				[],
				"Returns an empty character collection.",
				"Collection",
				ProgVariableTypes.Character | ProgVariableTypes.Collection
			)
		);
	}
}
