using System;
using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Mathematical;

internal class Floor : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"Floor".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Number }, // the parameters the function takes
				(pars, gameworld) => new Floor(pars, gameworld),
				new List<string>
				{
					"number"
				}, // parameter names
				new List<string>
				{
					"The number to find the floor of"
				}, // parameter help text
				"Rounds a function down to the next lowest integral number", // help text for the function,

				"Numbers", // the category to which this function belongs,

				FutureProgVariableTypes.Number // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected Floor(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}
	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get { return FutureProgVariableTypes.Number; }
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var number = (decimal?)ParameterFunctions[0].Result?.GetObject ?? 0.0M;
		Result = new NumberVariable(Math.Floor(number));
		return StatementResult.Normal;
	}
}