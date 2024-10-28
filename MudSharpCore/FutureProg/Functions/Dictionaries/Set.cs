using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Dictionaries;

internal class Set : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		foreach (var type in ProgVariableTypes.CollectionItem.GetSingleFlags())
		{
			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"set",
					[
						ProgVariableTypes.Dictionary, ProgVariableTypes.Text, type
					],
					(pars, gameworld) => new Set(pars, gameworld),
					new List<string> { "dictionary", "key", "item" },
					new List<string>
					{
						"The dictionary you want to set the item in",
						"The text key at which you want to store that item",
						"The item that you want to add to the dictionary"
					},
					"Sets the specified text key in the dictionary to be the item specified. Returns true if the set succeeded (the types were compatible), and false if not.",
					"Dictionaries",
					ProgVariableTypes.Boolean
				)
			);
		}
		
	}

	#endregion

	#region Constructors

	protected Set(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var dictionaryVariable = (IDictionaryVariable)ParameterFunctions[0].Result;
		if (!ParameterFunctions[2].Result.Type.CompatibleWith(dictionaryVariable.UnderlyingType))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var dictionary = (Dictionary<string, IProgVariable>)ParameterFunctions[0].Result?.GetObject;
		if (dictionary == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var key = ParameterFunctions[1].Result?.GetObject?.ToString();
		if (key == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var item = ParameterFunctions[2].Result;
		Result = new BooleanVariable(dictionaryVariable.Add(key, item));
		return StatementResult.Normal;
	}
}