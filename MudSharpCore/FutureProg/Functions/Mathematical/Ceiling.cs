using System;
using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Mathematical;

internal class Ceiling : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"Ceiling".ToLowerInvariant(),
				new[] { ProgVariableTypes.Number }, // the parameters the function takes
				(pars, gameworld) => new Ceiling(pars, gameworld),
				new List<string>
				{
					"number"
				}, // parameter names
				new List<string>
				{
					"The number to find the ceiling of"
				}, // parameter help text
				"Rounds a function up to the next highest integral number", // help text for the function,

				"Numbers", // the category to which this function belongs,

				ProgVariableTypes.Number // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected Ceiling(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}
	#endregion

	public override ProgVariableTypes ReturnType
	{
		get { return ProgVariableTypes.Number; }
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var number = (decimal?)ParameterFunctions[0].Result?.GetObject ?? 0.0M;
		Result = new NumberVariable(Math.Ceiling(number));
		return StatementResult.Normal;
	}
}