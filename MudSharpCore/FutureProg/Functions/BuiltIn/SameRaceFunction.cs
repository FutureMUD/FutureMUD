using System.Collections.Generic;
using MudSharp.Character.Heritage;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class SameRaceFunction : BuiltInFunction
{
	public SameRaceFunction(IList<IFunction> parameters)
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

		var race2 = ParameterFunctions[1].Result.GetObject as IRace;

		if (ParameterFunctions[0].Result.GetObject is not IRace race1)
		{
			ErrorMessage = "The first race in the SameRace function cannot be null";
			return StatementResult.Error;
		}

		Result = new BooleanVariable(race1.SameRace(race2));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"samerace",
				new[] { FutureProgVariableTypes.Race, FutureProgVariableTypes.Race },
				(pars, gameworld) => new SameRaceFunction(pars)
			)
		);
	}
}