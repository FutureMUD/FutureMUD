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

namespace MudSharp.FutureProg.Functions.Textual
{
	internal class ToRoman : BuiltInFunction
	{
		public IFuturemud Gameworld { get; set; }
		#region Static Initialisation
		public static void RegisterFunctionCompiler()
		{
			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"ToRoman".ToLowerInvariant(),
					new[] { ProgVariableTypes.Number }, // the parameters the function takes
					(pars, gameworld) => new ToRoman(pars, gameworld),
					new List<string> { 
						"number"
					}, // parameter names
					new List<string> { 
						"The number you want to convert"
					}, // parameter help text
					"Converts a number into a text representation of that number using roman numerals. Supports numbers up to 3999999.", // help text for the function,

					"Text", // the category to which this function belongs,

					ProgVariableTypes.Text // the return type of the function
				)
			);
		}
		#endregion

		#region Constructors
		protected ToRoman(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
		{
			Gameworld = gameworld;
		}
		#endregion

		public override ProgVariableTypes ReturnType
		{
			get { return ProgVariableTypes.Boolean; }
			protected set { }
		}

		public override StatementResult Execute(IVariableSpace variables)
		{
			if (base.Execute(variables) == StatementResult.Error)
			{
				return StatementResult.Error;
			}

			var number = (int?)(decimal?)ParameterFunctions[0].Result?.GetObject;
			if (number is null)
			{
				Result = new TextVariable("null");
				return StatementResult.Normal;
			}

			try
			{
				Result = new TextVariable(number.Value.ToRomanNumeral());
			}
			catch
			{
				Result = new TextVariable(number.Value.ToString("N0"));
			}

			return StatementResult.Normal;
		}
	}
}
