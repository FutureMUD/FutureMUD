using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToCharacterFunction : BuiltInFunction
{
	protected ToCharacterFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Character;
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
			new[] { ProgVariableTypes.Perceiver },
			(pars, gameworld) => new ToCharacterFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tocharacter",
			new[] { ProgVariableTypes.Perceivable },
			(pars, gameworld) => new ToCharacterFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tocharacter",
			new[] { ProgVariableTypes.Toon },
			(pars, gameworld) => new ToCharacterFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tocharacter",
			new[] { ProgVariableTypes.CollectionItem },
			(pars, gameworld) => new ToCharacterFunction(pars)
		));
	}
}