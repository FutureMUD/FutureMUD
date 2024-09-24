using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.BuiltIn;
internal class ToBankFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"tobank",
				[FutureProgVariableTypes.Number], // the parameters the function takes
				(pars, gameworld) => new ToBankFunction(pars, gameworld),
			new List<string>
			{
				"id"
			}, // parameter names
			new List<string>
			{
				"The ID of the bank"
			}, // parameter help text
			"Returns the bank referenced by the ID, or null if not found", // help text for the function,
			"Lookup", // the category to which this function belongs,
			FutureProgVariableTypes.Bank // the return type of the function
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"tobank",
				[FutureProgVariableTypes.Text], // the parameters the function takes
				(pars, gameworld) => new ToBankFunction(pars, gameworld),
				new List<string>
				{
					"name"
				}, // parameter names
				new List<string>
				{
					"The name of the bank"
				}, // parameter help text
				"Returns the bank referenced by the name, or null if not found", // help text for the function,
				"Lookup", // the category to which this function belongs,
				FutureProgVariableTypes.Bank // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected ToBankFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}
	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get { return FutureProgVariableTypes.Boolean; }
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].ReturnType.CompatibleWith(FutureProgVariableTypes.Text)
		? Gameworld.Banks.GetByName((string)ParameterFunctions[0].Result.GetObject)
		: Gameworld.Banks.Get((long)(decimal)ParameterFunctions[0].Result.GetObject);

		return StatementResult.Normal;
	}
}