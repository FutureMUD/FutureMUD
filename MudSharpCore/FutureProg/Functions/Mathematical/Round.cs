using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.FutureProg.Functions.Mathematical;

internal class Round : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"Round".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Number }, // the parameters the function takes
				(pars, gameworld) => new Round(pars, gameworld),
				new List<string>
				{
					"number"
				}, // parameter names
				new List<string>
				{
					"The number to round"
				}, // parameter help text
				"Rounds a function to zero decimal places", // help text for the function,

				"Numbers", // the category to which this function belongs,

				FutureProgVariableTypes.Number // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"Round".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Number, FutureProgVariableTypes.Number }, // the parameters the function takes
				(pars, gameworld) => new Round(pars, gameworld),
				new List<string>
				{
					"number",
					"decimals"
				}, // parameter names
				new List<string>
				{
					"The number to round",
					"The number of decimal places"
				}, // parameter help text
				"Rounds a function to the specified amount of decimal places", // help text for the function,

				"Numbers", // the category to which this function belongs,

				FutureProgVariableTypes.Number // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected Round(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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
		var places = 0;
		if (ParameterFunctions.Count == 2)
		{
			places = (int)((decimal?)ParameterFunctions[1].Result?.GetObject ?? 0.0M);
		}

		Result = new NumberVariable(Math.Round(number, places));
		return StatementResult.Normal;
	}
}