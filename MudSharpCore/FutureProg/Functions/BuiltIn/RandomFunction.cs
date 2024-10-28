using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class RandomFunction : BuiltInFunction
{
	private RandomFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = new NumberVariable(RandomUtilities.Random((int)(decimal)ParameterFunctions[0].Result.GetObject,
			(int)(decimal)ParameterFunctions[1].Result.GetObject));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"random",
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Number },
			(pars, gameworld) => new RandomFunction(pars)
		));
	}
}