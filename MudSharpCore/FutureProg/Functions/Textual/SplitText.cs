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
internal class SplitText : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"splittext",
				new[] { ProgVariableTypes.Text, ProgVariableTypes.Text }, // the parameters the function takes
				(pars, gameworld) => new SplitText(pars, gameworld),
			new List<string>
			{
				"text",
				"split"
			}, // parameter names
				new List<string>
				{
					"The text that you want to split up",
					"The character or text sequence that should cause the split"
				}, // parameter help text
				"Splits a text into a collection of smaller text fragments split by a particular character. Includes empty entries and doesn't include the splitting character.", // help text for the function,
				"Text", // the category to which this function belongs,
				ProgVariableTypes.Text | ProgVariableTypes.Collection // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected SplitText(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}
	#endregion

	public override ProgVariableTypes ReturnType
	{
		get { return ProgVariableTypes.Text | ProgVariableTypes.Collection; }
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var text = (string)(ParameterFunctions[0].Result?.GetObject ?? "");
		var split = (string)(ParameterFunctions[1].Result?.GetObject ?? "");
		if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(split))
		{
			Result = new CollectionVariable(new List<TextVariable>(), ProgVariableTypes.Text);
			return StatementResult.Normal;
		}

		Result = new CollectionVariable(text.Split(split).Select(x => new TextVariable(x)).ToList(), ProgVariableTypes.Text);
		return StatementResult.Normal;
	}
}
