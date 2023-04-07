using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Textual;

internal class ConcatStringCollectionFunction : BuiltInFunction
{
	public ConcatStringCollectionFunction(IList<IFunction> paramaterFunctions) : base(paramaterFunctions)
	{
	}

	#region Overrides of Function

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Text;
		protected set { }
	}

	#endregion

	#region Overrides of BuiltInFunction

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var collection =
			((IList)ParameterFunctions[0].Result.GetObject).OfType<object>().Select(x => x.ToString()).ToList();
		var joiner = ParameterFunctions[1].Result.GetObject.ToString();
		Result = new TextVariable(collection.ListToString(separator: joiner, twoItemJoiner: joiner, conjunction: ""));
		return StatementResult.Normal;
	}

	#endregion

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"concat",
			new[]
			{
				FutureProgVariableTypes.Text |
				FutureProgVariableTypes.Collection,
				FutureProgVariableTypes.Text
			},
			(pars, gameworld) =>
				new ConcatStringCollectionFunction(pars),
			new List<string> { "collection", "joiner" },
			new List<string>
			{
				"The collection of text values you want to concatenate",
				"A joiner text to be inserted between each of the values, for example a comma or a space"
			},
			"This function takes a collection of text values and joins them together with a specified joiner (which can be blank)",
			"Text",
			FutureProgVariableTypes.Text
		));
	}
}