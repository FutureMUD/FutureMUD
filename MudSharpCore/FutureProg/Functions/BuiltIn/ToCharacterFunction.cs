using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToCharacterFunction : BuiltInFunction
{
	protected ToCharacterFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Character;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].Result as ICharacter;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tocharacter",
			new[] { FutureProgVariableTypes.Perceiver },
			(pars, gameworld) => new ToCharacterFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tocharacter",
			new[] { FutureProgVariableTypes.Perceivable },
			(pars, gameworld) => new ToCharacterFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tocharacter",
			new[] { FutureProgVariableTypes.Toon },
			(pars, gameworld) => new ToCharacterFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tocharacter",
			new[] { FutureProgVariableTypes.CollectionItem },
			(pars, gameworld) => new ToCharacterFunction(pars)
		));
	}
}