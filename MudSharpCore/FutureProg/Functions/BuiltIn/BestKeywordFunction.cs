using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class BestKeywordFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public BestKeywordFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Text;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var source = (ICharacter)ParameterFunctions[0].Result;
		if (source == null)
		{
			ErrorMessage = "Source Character was null in BestKeyword function.";
			return StatementResult.Error;
		}

		var target = (ICharacter)ParameterFunctions[1].Result;
		if (target == null)
		{
			ErrorMessage = "Target Character was null in BestKeyword function.";
			return StatementResult.Error;
		}

		var keywords = target.GetKeywordsFor(source);
		var targets = source.Location.Characters.Except(source).Where(x => x.HasKeywords(keywords, source)).ToList();
		var index = targets.IndexOf(target);
		if (index == -1)
		{
			ErrorMessage = "Unable to find suitable target in BestKeyword function.";
			// DEBUG
			Console.WriteLine("Unable to find suitable target in BestKeyword function.");
			// END DEBUG
			return StatementResult.Error;
		}

		Result = new TextVariable(
			$"{index + 1}.{keywords.ListToString(separator: ".", conjunction: "", twoItemJoiner: ".")}");
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"bestkeyword",
			new[]
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Character
			},
			(pars, gameworld) =>
				new BestKeywordFunction(pars, gameworld),
			new List<string> { "source", "target" },
			new List<string>
			{
				"The source for whom you want to identify the keyword",
				"The target whose keyword you want to identify"
			},
			"This function allows you to determine what the 'best' keyword is for a target from a source. In this case, best implies a unique keyword that will allow the source to target the target through all regular commands.",
			"Utilities",
			FutureProgVariableTypes.Text
		));
	}
}