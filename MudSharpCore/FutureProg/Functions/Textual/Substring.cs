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

namespace MudSharp.FutureProg.Functions.Textual;
internal class Substring : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"substring",
				new[] { ProgVariableTypes.Text, ProgVariableTypes.Number }, // the parameters the function takes
				(pars, gameworld) => new Substring(pars, gameworld),
			new List<string>
			{
				"source",
				"index"
			}, // parameter names
				new List<string>
				{
					"The source text to take a substring from",
					"The 0-based index of where to start taking a substring from"
				}, // parameter help text

				"This function allows you to get a portion of a text string out", // help text for the function,

				"Text", // the category to which this function belongs,

				ProgVariableTypes.Text // the return type of the function
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"substring",
				new[] { ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Number }, // the parameters the function takes
				(pars, gameworld) => new Substring(pars, gameworld),
				new List<string>
				{
					"source",
					"index",
					"characters"
				}, // parameter names
				new List<string>
				{
					"The source text to take a substring from",
					"The 0-based index of where to start taking a substring from",
					"The number of characters to retrieve"
				}, // parameter help text

				"This function allows you to get a portion of a text string out", // help text for the function,

				"Text", // the category to which this function belongs,

				ProgVariableTypes.Text // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected Substring(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}
	#endregion

	public override ProgVariableTypes ReturnType
	{
		get { return ProgVariableTypes.Text; }
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var text = ParameterFunctions[0].Result?.GetObject?.ToString();
		if (string.IsNullOrEmpty(text))
		{
			Result = new TextVariable(string.Empty);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[1].Result is not NumberVariable indexVariable || indexVariable.GetObject is null)
		{
			Result = new TextVariable(text);
			return StatementResult.Normal;
		}

		var index = (int)(decimal)indexVariable.GetObject;
		if (index >= text.Length)
		{
			Result = new TextVariable(string.Empty);
			return StatementResult.Normal;
		}

		if (ParameterFunctions.Count == 3 && ParameterFunctions[2].Result is NumberVariable lengthVariable && lengthVariable.GetObject is not null)
		{
			var length = (int)(decimal)lengthVariable.GetObject;
			if ((index + length) >= text.Length)
			{
				Result = new TextVariable(string.Empty);
				return StatementResult.Normal;
			}

			Result = new TextVariable(text.Substring(index, length));
			return StatementResult.Normal;
		}

		Result = new TextVariable(text.Substring(index));
		return StatementResult.Normal;
	}
}
